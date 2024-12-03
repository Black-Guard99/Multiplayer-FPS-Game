using System;
using System.Collections.Generic;
using System.IO;

using Best.HTTP.Shared;
using Best.HTTP.Shared.Databases;
using Best.HTTP.Shared.Databases.Indexing;
using Best.HTTP.Shared.Databases.Indexing.Comparers;
using Best.HTTP.Shared.Databases.MetadataIndexFinders;
using Best.HTTP.Shared.Databases.Utils;
using Best.HTTP.Shared.Extensions;
using Best.HTTP.Shared.Logger;
using Best.HTTP.Shared.PlatformSupport.Threading;

using UnityEngine;

namespace Best.HTTP.Caching
{
    struct v128View
    {
        public ulong low;
        public ulong high;
    }

    /// <summary>
    /// Possible lock-states a cache-content can be in.
    /// </summary>
    public enum LockTypes : byte
    {
        /// <summary>
        /// No reads or writes are happening on the cached content.
        /// </summary>
        Unlocked,

        /// <summary>
        /// There's one writer operating on the cached content. No other writes or reads allowed while this lock is held on the content.
        /// </summary>
        Write,

        /// <summary>
        /// There's at least one read operation happening on the cached content. No writes allowed while this lock is held on the content.
        /// </summary>
        Read
    }

    /// <summary>
    /// Metadata stored for every cached content. It contains only limited data about the content to help early cache decision making and cache management.
    /// </summary>
    internal class CacheMetadata : Metadata
    {
        /// <summary>
        /// Unique hash of the cached content, generated by <see cref="HTTPCache.CalculateHash(HTTPMethods, Uri)"/>.
        /// </summary>
        public UnityEngine.Hash128 Hash { get; set; }

        /// <summary>
        /// Size of the stored content in bytes.
        /// </summary>
        public ulong ContentLength { get; set; }

        /// <summary>
        /// When the last time the content is accessed. Also initialized when the initial download completes.
        /// </summary>
        public DateTime LastAccessTime { get; set; }

        /// <summary>
        /// What kind of lock the content is currently in.
        /// </summary>
        public LockTypes Lock { get; set; }

        /// <summary>
        /// Number of readers.
        /// </summary>
        public int ReadLockCount { get; set; }

        public unsafe override void SaveTo(Stream stream)
        {
            base.SaveTo(stream);

            var hash = this.Hash;
            v128View view = *(v128View*)&hash;

            stream.EncodeUnsignedVariableByteInteger(view.low);
            stream.EncodeUnsignedVariableByteInteger(view.high);
            stream.EncodeUnsignedVariableByteInteger(ContentLength);
            stream.EncodeSignedVariableByteInteger(LastAccessTime.ToBinary() >> CacheMetadataContentParser.PrecisionShift);

            // Only Write locks should persist as Reads doesn't alter the cached content
            if (this.Lock == LockTypes.Write)
                stream.EncodeUnsignedVariableByteInteger((byte)this.Lock);
            else
                stream.EncodeUnsignedVariableByteInteger((byte)LockTypes.Unlocked);
        }

        public unsafe override void LoadFrom(Stream stream)
        {
            base.LoadFrom(stream);

            var hash = default(v128View);
            hash.low = stream.DecodeUnsignedVariableByteInteger();
            hash.high = stream.DecodeUnsignedVariableByteInteger();

            this.Hash = *(UnityEngine.Hash128*)&hash;

            this.ContentLength = stream.DecodeUnsignedVariableByteInteger();
            this.LastAccessTime = DateTime.FromBinary(stream.DecodeSignedVariableByteInteger() << CacheMetadataContentParser.PrecisionShift);

            this.Lock = (LockTypes)stream.DecodeUnsignedVariableByteInteger();
        }

        public override string ToString() => $"[Metadata {Hash}, {ContentLength:N0}, {Lock}, {ReadLockCount}]";
    }

    /// <summary>
    /// Possible caching flags that a `Cache-Control` header can send.
    /// </summary>
    [Flags]
    public enum CacheFlags : byte
    {
        /// <summary>
        /// No special treatment required.
        /// </summary>
        None = 0x00,

        /// <summary>
        /// Indicates whether the entity must be revalidated with the server or can be serverd directly from the cache without touching the server when the content is considered stale.
        /// </summary>
        /// <remarks>
        /// More details can be found here:
        /// <list type="bullet">
        ///     <item><description><see href="https://www.rfc-editor.org/rfc/rfc9111.html#name-must-revalidate"/></description></item>
        /// </list>
        /// </remarks>
        MustRevalidate = 0x01,

        /// <summary>
        /// If it's true, the client always have to revalidate the cached content when it's stale.
        /// </summary>
        /// <remarks>
        /// More details can be found here:
        /// <list type="bullet">
        ///     <item><description><see href="https://www.rfc-editor.org/rfc/rfc9111.html#name-no-cache-2"/></description></item>
        /// </list>
        /// </remarks>
        NoCache = 0x02
    }

    /// <summary>
    /// Cached content associated with a <see cref="CacheMetadata"/>.
    /// </summary>
    /// <remarks>This is NOT the cached content received from the server! It's for storing caching values to decide on how the content can be used.</remarks>
    internal sealed class CacheMetadataContent
    {
        /// <summary>
        /// ETag of the entity.
        /// </summary>
        /// <remarks>
        /// More details can be found here:
        /// <list type="bullet">
        ///     <item><description><see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/ETag"/></description></item>
        /// </list>
        /// </remarks>
        public string ETag;

        /// <summary>
        /// LastModified date of the entity. Use ToString("r") to convert it to the format defined in RFC 1123.
        /// </summary>
        public DateTime LastModified = DateTime.MinValue;

        /// <summary>
        /// When the cache will expire.
        /// </summary>
        /// <remarks>
        /// More details can be found here:
        /// <list type="bullet">
        ///     <item><description><see href="https://www.rfc-editor.org/rfc/rfc9111.html#name-expires"/></description></item>
        /// </list>
        /// </remarks>
        public DateTime Expires = DateTime.MinValue;

        /// <summary>
        /// The age that came with the response
        /// </summary>
        /// <remarks>
        /// More details can be found here:
        /// <list type="bullet">
        ///     <item><description><see href="https://www.rfc-editor.org/rfc/rfc9111.html#name-age"/></description></item>
        /// </list>
        /// </remarks>
        public uint Age;

        /// <summary>
        /// Maximum how long the entry should served from the cache without revalidation.
        /// </summary>
        /// <remarks>
        /// More details can be found here:
        /// <list type="bullet">
        ///     <item><description><see href="https://www.rfc-editor.org/rfc/rfc9111.html#name-max-age-2"/></description></item>
        /// </list>
        /// </remarks>
        public uint MaxAge;

        /// <summary>
        /// The Date that came with the response.
        /// </summary>
        /// <remarks>
        /// More details can be found here:
        /// <list type="bullet">
        ///     <item><description><see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Date"/></description></item>
        /// </list>
        /// </remarks>
        public DateTime Date = DateTime.MinValue;

        /// <summary>
        /// It's a grace period to serve staled content without revalidation.
        /// </summary>
        /// <remarks>
        /// More details can be found here:
        /// <list type="bullet">
        ///     <item><description><see href="https://www.rfc-editor.org/rfc/rfc5861.html#section-3"/></description></item>
        /// </list>
        /// </remarks>
        public uint StaleWhileRevalidate;

        /// <summary>
        /// Allows the client to serve stale content if the server responds with an 5xx error.
        /// </summary>
        /// <remarks>
        /// More details can be found here:
        /// <list type="bullet">
        ///     <item><description><see href="https://www.rfc-editor.org/rfc/rfc5861.html#section-4"/></description></item>
        /// </list>
        /// </remarks>
        public uint StaleIfError;

        /// <summary>
        /// bool values packed into one single flag.
        /// </summary>
        public CacheFlags Flags = CacheFlags.None;

        /// <summary>
        /// The value of the clock at the time of the request that resulted in the stored response.
        /// </summary>
        /// <remarks>
        /// More details can be found here:
        /// <list type="bullet">
        ///     <item><description><see href="https://www.rfc-editor.org/rfc/rfc9111.html#section-4.2.3-3.8"/></description></item>
        /// </list>
        /// </remarks>
        public DateTime RequestTime = DateTime.MinValue;

        /// <summary>
        /// The value of the clock at the time the response was received.
        /// </summary>
        public DateTime ResponseTime = DateTime.MinValue;

        public CacheMetadataContent()
        {
        }

        internal void From(Dictionary<string, List<string>> headers)
        {
            this.ETag = headers.GetFirstHeaderValue("ETag").ToStr(this.ETag ?? string.Empty);
            this.Expires = headers.GetFirstHeaderValue("Expires").ToDateTime(this.Expires);
            if (this.Expires < DateTime.UtcNow)
                this.Expires = DateTime.MinValue;

            this.LastModified = headers.GetFirstHeaderValue("Last-Modified").ToDateTime(DateTime.MinValue);
            this.Age = headers.GetFirstHeaderValue("Age").ToUInt32(this.Age);
            this.Date = headers.GetFirstHeaderValue("Date").ToDateTime(this.Date);

            // https://www.rfc-editor.org/rfc/rfc9111.html#section-4.2.1-4
            // When there is more than one value present for a given directive
            // (e.g., two Expires header field lines or multiple Cache-Control: max-age directives),
            // either the first occurrence should be used or the response should be considered stale.
            var cacheControl = headers.GetFirstHeaderValue("cache-control");
            if (!string.IsNullOrEmpty(cacheControl))
            {
                HeaderParser parser = new HeaderParser(cacheControl);

                if (parser.Values != null)
                {
                    this.Flags = CacheFlags.None;

                    for (int i = 0; i < parser.Values.Count; ++i)
                    {
                        var kvp = parser.Values[i];

                        switch (kvp.Key.ToLowerInvariant())
                        {
                            // https://www.rfc-editor.org/rfc/rfc9111.html#name-max-age-2
                            case "max-age":
                                if (kvp.HasValue)
                                {
                                    // Some cache proxies will return float values
                                    double maxAge;
                                    if (double.TryParse(kvp.Value, out maxAge) && maxAge >= 0)
                                        this.MaxAge = (uint)maxAge;
                                    else
                                        this.MaxAge = 0;
                                }
                                else
                                    this.MaxAge = 0;

                                // https://www.rfc-editor.org/rfc/rfc9111.html#section-5.3-8
                                // https://www.rfc-editor.org/rfc/rfc9111.html#cache-response-directive.s-maxage
                                // If a response includes a Cache-Control header field with the max-age directive (Section 5.2.2.1), a recipient MUST ignore the Expires header field.
                                this.Expires = DateTime.MinValue;
                                break;

                            // https://www.rfc-editor.org/rfc/rfc9111.html#name-must-revalidate
                            case "must-revalidate": this.Flags |= CacheFlags.MustRevalidate; break;
                            // https://www.rfc-editor.org/rfc/rfc9111.html#name-no-cache-2
                            case "no-cache": this.Flags |= CacheFlags.NoCache; break;
                            // https://www.rfc-editor.org/rfc/rfc5861.html#section-3
                            case "stale-while-revalidate": this.StaleWhileRevalidate = kvp.HasValue ? kvp.Value.ToUInt32(0) : 0; break;
                            // https://www.rfc-editor.org/rfc/rfc5861.html#section-4
                            case "stale-if-error": this.StaleIfError = kvp.HasValue ? kvp.Value.ToUInt32(0) : 0; break;
                        }
                    }
                }
                
            }
        }
    }

    internal sealed class CacheMetadataContentParser : IDiskContentParser<CacheMetadataContent>
    {
        public const int PrecisionShift = 24;

        public void Encode(Stream stream, CacheMetadataContent content)
        {
            stream.EncodeUnsignedVariableByteInteger(content.MaxAge);
            stream.WriteLengthPrefixedString(content.ETag);
            stream.EncodeSignedVariableByteInteger(content.LastModified.ToBinary() >> PrecisionShift);
            stream.EncodeSignedVariableByteInteger(content.Expires.ToBinary() >> PrecisionShift);
            stream.EncodeUnsignedVariableByteInteger(content.Age);
            stream.EncodeSignedVariableByteInteger(content.Date.ToBinary() >> PrecisionShift);
            stream.EncodeUnsignedVariableByteInteger((byte)content.Flags);
            stream.EncodeUnsignedVariableByteInteger(content.StaleWhileRevalidate);
            stream.EncodeUnsignedVariableByteInteger(content.StaleIfError);
            stream.EncodeSignedVariableByteInteger(content.RequestTime.ToBinary() >> PrecisionShift);
            stream.EncodeSignedVariableByteInteger(content.ResponseTime.ToBinary() >> PrecisionShift);
        }

        public CacheMetadataContent Parse(Stream stream, int length)
        {
            CacheMetadataContent content = new CacheMetadataContent();

            content.MaxAge = (uint)stream.DecodeUnsignedVariableByteInteger();
            content.ETag = stream.ReadLengthPrefixedString();
            content.LastModified = DateTime.FromBinary(stream.DecodeSignedVariableByteInteger() << PrecisionShift);
            content.Expires = DateTime.FromBinary(stream.DecodeSignedVariableByteInteger() << PrecisionShift);
            content.Age = (uint)stream.DecodeUnsignedVariableByteInteger();
            content.Date = DateTime.FromBinary(stream.DecodeSignedVariableByteInteger() << PrecisionShift);
            content.Flags = (CacheFlags)stream.DecodeUnsignedVariableByteInteger();
            content.StaleWhileRevalidate = (uint)stream.DecodeUnsignedVariableByteInteger();
            content.StaleIfError = (uint)stream.DecodeUnsignedVariableByteInteger();
            content.RequestTime = DateTime.FromBinary(stream.DecodeSignedVariableByteInteger() << PrecisionShift);
            content.ResponseTime = DateTime.FromBinary(stream.DecodeSignedVariableByteInteger() << PrecisionShift);

            return content;
        }
    }

    internal sealed class CacheMetadataIndexingService : IndexingService<CacheMetadataContent, CacheMetadata>
    {
        private AVLTree<UnityEngine.Hash128, int> index_Hash = new AVLTree<UnityEngine.Hash128, int>(new Hash128Comparer());
        
        public override void Index(CacheMetadata metadata)
        {
            base.Index(metadata);
            this.index_Hash.Add(metadata.Hash, metadata.Index);
        }

        public override void Remove(CacheMetadata metadata)
        {
            base.Remove(metadata);
            this.index_Hash.Remove(metadata.Hash);
        }

        public override void Clear()
        {
            base.Clear();
            this.index_Hash.Clear();
        }

        public override IEnumerable<int> GetOptimizedIndexes() => this.index_Hash.WalkHorizontal();

        public bool ContainsHash(UnityEngine.Hash128 hash) => this.index_Hash.ContainsKey(hash);

        public List<int> FindByHash(UnityEngine.Hash128 hash) => this.index_Hash.Find(hash);
    }

    internal sealed class CacheMetadataService : MetadataService<CacheMetadata, CacheMetadataContent>
    {
        public CacheMetadataService(IndexingService<CacheMetadataContent, CacheMetadata> indexingService, IEmptyMetadataIndexFinder<CacheMetadata> emptyMetadataIndexFinder)
            : base(indexingService, emptyMetadataIndexFinder)
        {
        }

        public override CacheMetadata CreateFrom(Stream stream)
        {
            return base.CreateFrom(stream);
        }

        public CacheMetadata Create(UnityEngine.Hash128 hash, CacheMetadataContent value, int filePos, int length)
        {
            var result = base.CreateDefault(value, filePos, length, (content, metadata) => {
                metadata.Hash = hash;
            });

            return result;
        }
    }

    internal sealed class CacheDatabaseOptions : DatabaseOptions
    {
        public CacheDatabaseOptions() : base("CacheDatabase")
        {
            base.UseHashFile = false;
        }
    }

    internal sealed class HTTPCacheDatabase : Database<CacheMetadataContent, CacheMetadata, CacheMetadataIndexingService, CacheMetadataService>
    {
        public HTTPCacheDatabase(string directory)
            : this(directory, new CacheDatabaseOptions(), new CacheMetadataIndexingService())
        {
        }

        private HTTPCacheDatabase(string directory,
            DatabaseOptions options,
            CacheMetadataIndexingService indexingService)
            : base(directory, options, indexingService, new CacheMetadataContentParser(), new CacheMetadataService(indexingService, new FindDeletedMetadataIndexFinder<CacheMetadata>()))
        {

        }

        public CacheMetadataContent FindByHashAndUpdateRequestTime(UnityEngine.Hash128 hash, LoggingContext context)
        {
            if (HTTPManager.Logger.IsDiagnostic)
                HTTPManager.Logger.Verbose(nameof(HTTPCacheDatabase), $"{nameof(FindByHashAndUpdateRequestTime)}({hash})", context);

            if (!hash.isValid)
                return default;

            using var _ = new WriteLock(this.rwlock);

            var (content, metadata) = FindContentAndMetadata(hash);

            if (content != null)
            {
                content.RequestTime = DateTime.UtcNow;

                UpdateMetadataAndContent(metadata, content);
            }

            return content;
        }

        public bool TryAcquireWriteLock(Hash128 hash, Dictionary<string, List<string>> headers, LoggingContext context)
        {
            if (HTTPManager.Logger.IsDiagnostic)
                HTTPManager.Logger.Verbose(nameof(HTTPCacheDatabase), $"{nameof(TryAcquireWriteLock)}({hash}, {headers?.Count})", context);

            if (!hash.isValid)
                return false;

            using var _ = new WriteLock(this.rwlock);

            // FindMetadata filters out logically deleted entries, what we need here because we want to load it too.
            var metadata = FindMetadata(hash);
            CacheMetadataContent content = null;
            if (metadata != null)
            {
                if (HTTPManager.Logger.IsDiagnostic)
                    HTTPManager.Logger.Verbose(nameof(HTTPCacheDatabase), $"{nameof(TryAcquireWriteLock)} - Metadata found: {metadata}", context);

                if (metadata.Lock != LockTypes.Unlocked)
                    return false;
                metadata.Lock = LockTypes.Write;

                content = this.FromMetadata(metadata);
                content.From(headers);

                UpdateMetadataAndContent(metadata, content);
            }
            else
            {
                if (HTTPManager.Logger.IsDiagnostic)
                    HTTPManager.Logger.Verbose(nameof(HTTPCacheDatabase), $"{nameof(TryAcquireWriteLock)} - Creating new DB entry", context);

                content = new CacheMetadataContent();
                content.RequestTime = DateTime.UtcNow;
                content.From(headers);

                (int filePos, int length) = this.DiskManager.Append(content);
                metadata = this.MetadataService.Create(hash, content, filePos, length);

                metadata.Lock = LockTypes.Write;
            }

            FlagDirty(1);

            return true;
        }

        public bool Update(Hash128 hash, Dictionary<string, List<string>> headers, LoggingContext context)
        {
            if (HTTPManager.Logger.IsDiagnostic)
                HTTPManager.Logger.Verbose(nameof(HTTPCacheDatabase), $"{nameof(Update)}({hash}, {headers?.Count})", context);

            if (!hash.isValid)
                return false;

            using var _ = new WriteLock(this.rwlock);

            var (content, metadata) = FindContentAndMetadata(hash);

            if (content != null)
            {
                content.From(headers);
                content.ResponseTime = DateTime.UtcNow;

                UpdateMetadataAndContent(metadata, content);
            }

            return content != null;
        }

        public void ReleaseWriteLock(Hash128 hash, ulong length, LoggingContext context)
        {
            if (HTTPManager.Logger.IsDiagnostic)
                HTTPManager.Logger.Verbose(nameof(HTTPCacheDatabase), $"{nameof(ReleaseWriteLock)}({hash}, {length:N0})", context);

            if (!hash.isValid)
                return;

            using var _ = new WriteLock(this.rwlock);

            var (content, metadata) = FindContentAndMetadata(hash);
            if (content == null)
            {
                HTTPManager.Logger.Warning(nameof(HTTPCacheDatabase), $"{nameof(ReleaseWriteLock)} - Couldn't find content!", context);
                return;
            }

            if (metadata.Lock != LockTypes.Write)
                HTTPManager.Logger.Error(nameof(HTTPCacheDatabase), $"{nameof(ReleaseWriteLock)} - Is NOT Write Locked! {metadata}", context);

            metadata.Lock = LockTypes.Unlocked;
            
            if (content != null)
            {
                metadata.ContentLength = length;

                var now = DateTime.UtcNow;
                metadata.LastAccessTime = now;
                content.ResponseTime = now;

                UpdateMetadataAndContent(metadata, content);
            }

            FlagDirty(1);
        }

        public bool TryAcquireReadLock(Hash128 hash, LoggingContext context)
        {
            if (HTTPManager.Logger.IsDiagnostic)
                HTTPManager.Logger.Verbose(nameof(HTTPCacheDatabase), $"{nameof(TryAcquireReadLock)}({hash})", context);

            if (!hash.isValid)
                return false;

            using var _ = new WriteLock(this.rwlock);

            var metadata = FindMetadata(hash);
            if (metadata == null)
                return false;

            if (metadata.Lock == LockTypes.Write)
                return false;

            metadata.Lock = LockTypes.Read;
            // we are behind a write lock, it's safe to increment it like this
            metadata.ReadLockCount++;

            if (HTTPManager.Logger.IsDiagnostic)
                HTTPManager.Logger.Verbose(nameof(HTTPCacheDatabase), $"{nameof(TryAcquireReadLock)} - {metadata}", context);

            return true;
        }

        public void ReleaseReadLock(Hash128 hash, LoggingContext context)
        {
            if (HTTPManager.Logger.IsDiagnostic)
                HTTPManager.Logger.Verbose(nameof(HTTPCacheDatabase), $"{nameof(ReleaseReadLock)}({hash})", context);

            if (!hash.isValid)
                return;

            using var _ = new WriteLock(this.rwlock);

            var metadata = FindMetadata(hash);

            if (metadata == null)
            {
                HTTPManager.Logger.Warning(nameof(HTTPCacheDatabase), $"{nameof(ReleaseReadLock)} - Couldn't find metadata!", context);
                return;
            }

            if (metadata.Lock != LockTypes.Read)
                HTTPManager.Logger.Warning(nameof(HTTPCacheDatabase), $"{nameof(ReleaseReadLock)} - Is NOT Locked!", context);

            if (metadata.ReadLockCount == 0)
                HTTPManager.Logger.Error(nameof(HTTPCacheDatabase), $"{nameof(ReleaseReadLock)} - ReadLockCount already zero!", context);

            if (--metadata.ReadLockCount == 0)
                metadata.Lock = LockTypes.Unlocked;

            if (HTTPManager.Logger.IsDiagnostic)
                HTTPManager.Logger.Verbose(nameof(HTTPCacheDatabase), $"{nameof(ReleaseReadLock)} - {metadata}", context);
        }

        internal ulong Delete(Hash128 hash, LoggingContext context)
        {
            if (HTTPManager.Logger.IsDiagnostic)
                HTTPManager.Logger.Verbose(nameof(HTTPCacheDatabase), $"{nameof(Delete)}({hash})", context);

            if (!hash.isValid)
                return 0;

            using var _ = new WriteLock(this.rwlock);

            // Don't use FindMetadata, because it would return null for a logically deleted metadata
            //  so DeleteMetadata wouldn't be called!
            var byHash = this.IndexingService.FindByHash(hash);
            if (byHash == null || byHash.Count == 0)
                return 0;

            var metadata = this.MetadataService.Metadatas[byHash[0]];

            if (metadata == null)
                return 0;

            var contentLength = metadata.ContentLength;

            base.DeleteMetadata(metadata);

            return contentLength;
        }

        public void EnterWriteLock(LoggingContext context)
        {
            if (HTTPManager.Logger.IsDiagnostic)
                HTTPManager.Logger.Verbose(nameof(HTTPCacheDatabase), $"{nameof(EnterWriteLock)}()", context);
            this.rwlock.EnterWriteLock();
        }

        public void ExitWriteLock(LoggingContext context)
        {
            if (HTTPManager.Logger.IsDiagnostic)
                HTTPManager.Logger.Verbose(nameof(HTTPCacheDatabase), $"{nameof(ExitWriteLock)}()", context);
            this.rwlock.ExitWriteLock();
        }

        public void UpdateLastAccessTime(Hash128 hash, LoggingContext context)
        {
            if (HTTPManager.Logger.IsDiagnostic)
                HTTPManager.Logger.Verbose(nameof(HTTPCacheDatabase), $"{nameof(UpdateLastAccessTime)}({hash})", context);

            if (!hash.isValid)
                return;

            using var _ = new WriteLock(this.rwlock);

            var metadata = FindMetadata(hash);
            if (metadata != null)
            {
                metadata.LastAccessTime = DateTime.UtcNow;
                FlagDirty(1);
            }
        }

        private CacheMetadata FindMetadata(Hash128 hash)
        {
            var byHash = this.IndexingService.FindByHash(hash);
            if (byHash == null || byHash.Count == 0)
                return null;

            var metadata = this.MetadataService.Metadatas[byHash[0]];

            if (metadata != null && metadata.IsDeleted)
                return null;

            return metadata;
        }

        public (CacheMetadataContent, CacheMetadata) FindContentAndMetadataLocked(Hash128 hash)
        {
            using var _ = new WriteLock(this.rwlock);
            return FindContentAndMetadata(hash);
        }

        private (CacheMetadataContent, CacheMetadata) FindContentAndMetadata(Hash128 hash)
        {
            var byHash = this.IndexingService.FindByHash(hash);
            if (byHash == null || byHash.Count == 0)
                return (null, null);

            var metadata = this.MetadataService.Metadatas[byHash[0]];
            if (metadata != null && metadata.IsDeleted)
                return (null, null);

            var content = this.FromMetadataIndex(metadata.Index);
            return (content, metadata);
        }

        private void UpdateMetadataAndContent(Metadata metadata, CacheMetadataContent content)
        {
            this.DiskManager.SaveChanged(metadata, content);

            FlagDirty(1);
        }
    }
}
