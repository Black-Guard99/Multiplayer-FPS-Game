using TMPro;
using Photon.Pun;
using Cinemachine;
using UnityEngine;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using Unity.Mathematics;
using UnityEngine.UI;

[RequireComponent(typeof(PhotonView),typeof(HealthSystem))]
public class NewPlayerMovement : MonoBehaviourPun,IPunObservable {
    [SerializeField] private GameObject visualBody;
    [SerializeField] private LocalPlayerUiManager localPlayerUiManager;
    [SerializeField] private CinemachineVirtualCamera respawnTimeViewCamera;
    [Header("External Referances")]
    [SerializeField] private bool isAi;
    public ProfileData playerProfile;
    [SerializeField] private TextMeshProUGUI playerNickName;
    [SerializeField] private bool awayTeam;
    [SerializeField] private Image teamIndicatorImage;
    [SerializeField] private Renderer[] upperBodyParts;
    [SerializeField] private BodyAnimationManager animationManager;

    [Header("Health")]
    [SerializeField] private UIHealthBar healthBar;
    [SerializeField] private ParticleSystem bloodEffect;

    [Space(20)]
    [SerializeField] private NewPlayerInputController inputController;
    [SerializeField] private GameObject controllerCanvas;
    [SerializeField] private Loadout loadout;
    [SerializeField] private List<Transform> lookPoints;
    [SerializeField] private Transform lookPointRef;
    [SerializeField] private CapsuleCollider bodyCollider;
    [SerializeField] private HealthSystem healthSystem;



    [Header("Camera Settings")]
    [SerializeField] private float lookPointSpeed = 5f;
    [Tooltip("Lock the cursor to the game screen on play")]
    [SerializeField] private bool lockCursor = true;
    [Tooltip("Clamp the camera angle (Stop the camera form \"snapping its neck\")")]
    [SerializeField] private Vector2 cameraRotLimit = new Vector2(360f, 180f);
    // [Tooltip("The mouse sensitivity, both x and y")]
    [SerializeField] private float sensitivity = 1.8f;
    [Tooltip("Smoothing of the mouse movement (Try with and without)")]
    [SerializeField] private Vector2 smoothing = new Vector2(1.5f, 1.5f);
    [SerializeField] private Transform cameraHolder;
    
    [Tooltip("Needs to be the same name as your main cam")]
    [SerializeField] private GameObject rotationCamera,mainCamera,miniMapIcon;


    //----------------------------------------------------
    [Space]
    [Header("Movement Settings")]
    [SerializeField] private float slideForce = 20f;

    [Tooltip("Max walk speed")]
    [SerializeField] private float walkMoveSpeed = 7.5f;
    [Tooltip("Max sprint speed")]
    [SerializeField] private float sprintMoveSpeed = 11f;
    [Tooltip("Max jump speed")]
    [SerializeField] private float jumpMoveSpeed = 6f;
    [Tooltip("Max crouch speed")]
    [SerializeField] private float crouchMoveSpeed = 4f;
    [SerializeField] private float maxSlideTimer = .5f;

    //----------------------------------------------------
    [Header("Crouch Settings")]

    [Tooltip("How long it takes to crouch")]
    [SerializeField] private float crouchDownSpeed = 0.2f;
    
    [SerializeField] private float crouchColliderOffset;
    [SerializeField] private float crouchColliderHeight;
    [SerializeField] private float crouchCameraHeight;
    [SerializeField] private float crouchHeadColliderCenterY;
    [Tooltip("Lerp between crouching and standing")]
    [SerializeField] private bool smoothCrouch = true;
    [Tooltip("Can you crouch while in the air")]
    [SerializeField] private bool jumpCrouching = true;
    

    [Header("Testing Grapics")]
    // [SerializeField] private Transform bodyMesh;
    [Tooltip("How tall the character is when they crouch")]
    [SerializeField] private float crouchMeshScale = 0.68f; //change for how large you want when crouching
    [Tooltip("How tall the character is when they stand")]
    [SerializeField] private float standingMeshScale = 1f;
    //----------------------------------------------------
    [Header("Jump Settings")]

    [Tooltip("Initial jump force")]
    [SerializeField] private float jumpForce = 110f;
    [Tooltip("Continuous jump force")]
    [SerializeField] private float jumpAccel = 10f;
    [Tooltip("Max jump up time")]
    [SerializeField] private float jumpTime = 0.4f;
    [Tooltip("How long you have to jump after leaving a ledge (seconds)")]
    [SerializeField] private float coyoteTime = 0.2f;
    [Tooltip("How long I should buffer your jump input for (seconds)")]
    [SerializeField] private float jumpBuffer = 0.1f;
    [Tooltip("How long do I have to wait before I can jump again")]
    [SerializeField] private float jumpCooldown = 0.6f;
    [Tooltip("Fall quicker")]
    [SerializeField] private float extraGravity = 0.1f;
    [Tooltip("The tag that will be considered the ground")]
    [SerializeField] private string groundTag = "Ground";

    //----------------------------------------------------
    [Space]
    [Header("Keyboard Settings")]

    [Tooltip("The key used to jump")]
    [SerializeField] private KeyCode jump = KeyCode.Space;
    [Tooltip("The key used to sprint")]
    [SerializeField] private KeyCode sprint = KeyCode.LeftShift;
    [Tooltip("The key used to crouch")]
    [SerializeField] private KeyCode crouchToggle = KeyCode.LeftControl;
    [Tooltip("The key used to toggle the cursor")]
    [SerializeField] private KeyCode lockToggle = KeyCode.Q;

    //----------------------------------------------------
    [Space]
    // [Header("Debug Info")]
    [Tooltip("Are we on the ground?")]
    private bool areWeGrounded = true;
    private bool isJumping = false;
    [Tooltip("Are we crouching?")]
    private bool areWeCrouching = false;
    private bool areWeSliding = false;
    private float currentSlideTime;
    [Tooltip("The current speed I should be moving at")]
    private float currentSpeed;

    
    //----------------------------------------------------
    // Reference vars (These are the vars used in calculations, they don't need to be set by the user)
    private Rigidbody rb;
    private Vector3 input = new Vector3();
    private Vector2 _mouseAbsolute, _smoothMouse, targetDirection, targetCharacterDirection;
    private float coyoteTimeCounter, jumpBufferCounter, startJumpTime, endJumpTime;
    private bool wantingToJump = false, wantingToCrouch = false, wantingToSprint = false, jumpCooldownOver = true,wantToSlide;
    private float defultColliderHeight,camerHolderDefultHeight,defultBodyColliderCenterY,defultHeadColliderCenterY;

    // private P_SERIALIZED PSerialized;
    #region Sendign Cloud Data.......
    private float currentCamearHeight,currentBodyColliderHeight,currentBodyColliderCenterY/* ,currentHeadColliderCenterY */;
    private Vector3 CurrentLookPoint;
    private float animatonCurrentSpeedX,animatonCurrentSpeedY,weponMovementAnimationSpeed;
    private float speedChangeAnimationRef,weponSpeedChangeAnimRef;
    private float crossHairExpandOnFootMove = 0f,crossHairExpandOnCameraMove = 0f;
    private PlayerData playerData;
    private bool isReady;
    private float animationMoveSpeedX = 0f;
    private float animationMoveSpeedY = 0f;
    private bool startResapwnTimer;
    private float respawnTimer = 5f;
    private float pitch;
    #endregion

    /* public void SetAI(bool _isAi){
        isAi = _isAi;
    } */
    private void Awake(){
        PhotonNetwork.SendRate = 30;
        PhotonNetwork.SerializationRate = 10;
        respawnTimeViewCamera.gameObject.SetActive(false);
        playerData = GetComponent<PlayerData>();
        rb = gameObject.GetComponent<Rigidbody>();
    }
    private void Start() {
        isReady = true;
        if(isAi){
            mainCamera.SetActive(false);
            return;
        }
        Debug.Log("Player Actor Number " + PhotonNetwork.LocalPlayer.ActorNumber);
        if(!photonView.IsMine){
            HideUnHideUpperBody(true);
            playerNickName.gameObject.SetActive(true);
            controllerCanvas.SetActive(false);
            miniMapIcon.SetActive(false);
            if(mainCamera.TryGetComponent(out Camera camComp)){
                camComp.enabled = false;
            }
            if(mainCamera.TryGetComponent(out AudioListener camAudioListiner)){
                camAudioListiner.enabled = false;
            }
            // if(mainCamera.TryGetComponent(out CinemachineBrain))
            if(mainCamera.TryGetComponent(out CinemachineBrain camBrain)){
                camBrain.enabled = false;
            }
            // networkPositon = transform;
        }else{
            HideUnHideUpperBody(false);
            playerNickName.gameObject.SetActive(false);
            miniMapIcon.SetActive(true);
            rotationCamera.transform.localRotation = Quaternion.Euler(Vector3.zero);
            controllerCanvas.SetActive(true);
        }
        loadout.SetUpLoadout();
        isReady = true;
        //* Setting Defult Values for Crouching;
        currentCamearHeight = cameraHolder.localPosition.y;
        defultColliderHeight = bodyCollider.height;
        defultBodyColliderCenterY = bodyCollider.center.y;
        camerHolderDefultHeight = cameraHolder.localPosition.y;
        // defultHeadColliderCenterY = headCollider.center.y;
        // currentHeadColliderCenterY = defultHeadColliderCenterY;

        // Just set rb to the rigidbody of the gameobject containing this script
        
        // Set the currentSpeed to walking as no keys should be pressed yet
        currentSpeed = walkMoveSpeed;

        // Set target direction to the camera's initial orientation.
        targetDirection = transform.localRotation.eulerAngles;
        // Set target direction for the character body to its inital state.
        targetCharacterDirection = transform.localRotation.eulerAngles;
        if(photonView.IsMine){
            healthSystem.OnTakeDamgage += OnTakeDamgage_HealthSystem;
            healthSystem.OnHealthRegenerating += OnHealthRegenerating;
            healthSystem.OnDeath += OnDeath_HealthSystem;
            healthBar.SetPreviouseAndTotalHealth(healthSystem.previousHealthAmount,healthSystem.totalHealth);
            healthBar.SetRegenerationHealth(healthSystem.GetHealthNormalized(),healthSystem.previousHealthAmount / healthSystem.totalHealth);
            healthBar.SetCurrentHealth(healthSystem.GetHealthNormalized());
            healthBar.SetHealthPPVolume(healthSystem.GetInverseHealthNormalized());
        }else{
            photonView.RPC(nameof(ShowNickName),RpcTarget.All);
            healthSystem.OnTakeDamgage += (float damageValueNormalized,Vector3 damagePoint,Vector3 shooterPos,string username,string gunName)=>{
                photonView.RPC(nameof(PlayBloodEffect),RpcTarget.All,damagePoint);
            };
        }
        CurrentLookPoint = lookPointRef.position;
        if(loadout.GetCurrentGun != null){
            playerProfile.gunName = loadout.GetCurrentGun.GetGunName;
        }
    }
    private void OnHealthRegenerating(){
        healthBar.SetRegenerationHealth(healthSystem.GetHealthNormalized(),healthSystem.previousHealthAmount / healthSystem.totalHealth);
        healthBar.SetHealthPPVolume(healthSystem.GetInverseHealthNormalized());
    }
    private void OnTakeDamgage_HealthSystem(float damageValueNormalized,Vector3 damagePoint,Vector3 shooterPos,string username,string gunName){
        healthBar.SetPreviouseAndTotalHealth(healthSystem.previousHealthAmount,healthSystem.totalHealth);
        healthBar.SetCurrentHealth(damageValueNormalized);
        healthBar.SetDamageIndicatorDirection(shooterPos,transform.position,transform.eulerAngles);
        healthBar.SetHealthPPVolume(healthSystem.GetInverseHealthNormalized());
    }
    private void OnDeath_HealthSystem(Vector3 hitPoint,int p_actor,string username,string gunName){
        isReady = false;
        respawnTimeViewCamera.gameObject.SetActive(true);
        loadout.Die();
        rb.isKinematic = true;
        bodyCollider.enabled = false;
        photonView.RPC(nameof(OnDeathHideBody),RpcTarget.All);
        startResapwnTimer = true;
        /* rb.isKinematic = true;
        bodyCollider.enabled = false;
        visualBody.gameObject.SetActive(false);
        loadout.Die();
        isReady = false;
        respawnTimeViewCamera.gameObject.SetActive(true);
        startResapwnTimer = true; */
        MatchHandler.Current?.ChangeStat_S(PhotonNetwork.LocalPlayer.ActorNumber, 1, 1,gunName);
        if (p_actor >= 0){
            MatchHandler.Current?.ChangeStat_S(p_actor, 0, 1,playerProfile.gunName);
        }
        BotSpawnManager.current.RemoveEnemyFromList(this.transform);
    }

    [PunRPC]
    private void OnDeathHideBody(){
        visualBody.gameObject.SetActive(false);
    }
    
    private void Respawn(){
        respawnTimeViewCamera.gameObject.SetActive(false);
        MatchHandler.Current?.Spawn(awayTeam ? SpawningArea.SpawnAreaType.Away : SpawningArea.SpawnAreaType.Home);
        PhotonNetwork.Destroy(this.gameObject);
    }
    private void HideUnHideUpperBody(bool show){
        foreach(Renderer upperBody in upperBodyParts){
            upperBody.gameObject.SetActive(show);
        }
        teamIndicatorImage.gameObject.SetActive(show);
        
    }

    [PunRPC]
    private void PlayBloodEffect(Vector3 hitPoint){
        bloodEffect.gameObject.SetActive(true);
        bloodEffect.transform.position = hitPoint;
        bloodEffect.Play();
    }
    [PunRPC]
    private void ShowNickName(){
        playerNickName.SetText(photonView.Owner.NickName);
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if(stream.IsWriting){
            CurrentLookPoint = lookPointRef.position;
            stream.SendNext(_mouseAbsolute.y);
            stream.SendNext(pitch);
            stream.SendNext(currentCamearHeight);
            stream.SendNext(currentBodyColliderHeight);
            stream.SendNext(currentBodyColliderCenterY);
            stream.SendNext(CurrentLookPoint);
            stream.SendNext(playerProfile.username);            
        }else{
            // Receving.. Data............
            _mouseAbsolute.y = (float)stream.ReceiveNext();
            pitch = (float)stream.ReceiveNext();
            currentCamearHeight = (float)stream.ReceiveNext();
            currentBodyColliderHeight = (float)stream.ReceiveNext();
            currentBodyColliderCenterY =(float)stream.ReceiveNext();
            CurrentLookPoint = (Vector3)stream.ReceiveNext();
            playerProfile.username = (string)stream.ReceiveNext();
            rotationCamera.transform.localRotation = Quaternion.Euler(pitch,0f,0f);
            bodyCollider.height = currentBodyColliderHeight;
            bodyCollider.center = new Vector3(bodyCollider.center.x,currentBodyColliderCenterY,bodyCollider.center.z);
            cameraHolder.localPosition = new Vector3(cameraHolder.localPosition.x,currentCamearHeight,cameraHolder.localPosition.z);
            SetLookTransform();
        }
    }
    private void SetLookTransform(){
        CurrentLookPoint = Vector3.Lerp(CurrentLookPoint,lookPointRef.position,lookPointSpeed * Time.deltaTime);
        foreach(Transform lookPoint in lookPoints){
            lookPoint.position = CurrentLookPoint;
        }

    }
    private void Update() {
        if(isAi) return;
        if(photonView.IsMine){
            if(startResapwnTimer){
                respawnTimer -= Time.deltaTime;
                localPlayerUiManager.ShowRespawnTimer(Mathf.CeilToInt(respawnTimer));
                if(respawnTimer <= 0f){
                    Respawn();
                    startResapwnTimer = false;
                }
                return;
            }
            // Debug.Log("Ping : " + PhotonNetwork.GetPing());
            // UIControllers.Current.SetPingAmount(PhotonNetwork.GetPing());
            if(!isReady) return;
            if(Input.GetKeyDown(KeyCode.T)){
                healthSystem.TakeDamageRPC(10f,transform.position,-1,Vector3.forward,playerProfile.username,playerProfile.gunName);
            }
            // Mouse lock toggle (KeyDown only fires once)
            if (Input.GetKeyDown(lockToggle)){
                lockCursor = !lockCursor;
            }
            // Update the camera pos
            CameraUpdate();
            SetLookTransform();
            switch(MatchHandler.Current.GetGameState()){
                case GameState.Starting:
                case GameState.Ending:
                    return;
            }
            // Move all input to Update(), then use given input on FixedUpdate()

            // movement
            
            
            input = new Vector3(inputController.GetMoveVector.x,0,inputController.GetMoveVector.y);

            if(inputController.GetPcInputPressing(crouchToggle)){
                if(!areWeSliding && !areWeCrouching && !isJumping){
                    if(input.z > 0f && wantingToSprint){
                        areWeSliding = true;
                        animationManager.SetSlide(true);
                        return;
                    }
                    
                }
            }
            // Jump key
            if(inputController.OnPc){
                wantingToJump = inputController.GetPcInputPress(jump);
            }
            if(inputController.GetPcInputPress(crouchToggle)){
                // Crouch key
                wantingToCrouch = !wantingToCrouch;
                animationManager.SetCrouch(wantingToCrouch);
            }
            if(wantingToJump && !areWeCrouching  && !loadout.IsAiming){
                isJumping = true;
                animationManager.Jump(true);
            }
            // Sprint key
            wantingToSprint = inputController.Sprinting && input.z > 0f && !loadout.IsAiming;
            // Crosshair Movement Amount
            crossHairExpandOnFootMove = (wantingToSprint ? 1f: 0.5f) * input.magnitude;
            if(!isJumping){
                if(input.x > 0){
                    animationMoveSpeedX = .5f;
                }
                if(input.x < 0){
                    animationMoveSpeedX = -.5f;
                }
                if(input.z > 0){
                    animationMoveSpeedY = (wantingToSprint ? 1: 0.5f);
                }
                if(input.z < 0){
                    animationMoveSpeedY = -.5f;
                }
                if(input.x == 0){
                    animationMoveSpeedX = 0f;
                }
                if(input.z == 0){
                    animationMoveSpeedY = 0f;
                }
                if(animatonCurrentSpeedX != animationMoveSpeedX){
                    animatonCurrentSpeedX = Mathf.SmoothDamp(animatonCurrentSpeedX,animationMoveSpeedX,ref speedChangeAnimationRef,5f * Time.deltaTime);
                }
                if(animatonCurrentSpeedY != animationMoveSpeedY){
                    animatonCurrentSpeedY = Mathf.SmoothDamp(animatonCurrentSpeedY,animationMoveSpeedY,ref speedChangeAnimationRef,5f * Time.deltaTime);
                }
                animationManager.SetSpeed(animatonCurrentSpeedX,animatonCurrentSpeedY);
            }
            if(weponMovementAnimationSpeed != crossHairExpandOnFootMove){
                weponMovementAnimationSpeed = Mathf.SmoothDamp(weponMovementAnimationSpeed,crossHairExpandOnFootMove,ref weponSpeedChangeAnimRef,5f * Time.deltaTime);
            }
            loadout.SetPlayerSpeed(weponMovementAnimationSpeed);
            
            
        }else{
            photonView.RPC(nameof(SyncTeam),RpcTarget.All,awayTeam);
        }
    }
    private void Slide(){
        if(currentSlideTime >= maxSlideTimer){
            currentSlideTime = 0f;
            animationManager.SetSlide(false);
            areWeSliding = false;
        }else{
            input = Vector3.zero;
            if(wantingToJump){
                isJumping = true;
                animationManager.Jump(true);
                CrouchCollider(false);
                currentSlideTime = 0f;
                animationManager.SetSlide(false);
                areWeSliding = false;
                return;
            }
            CrouchCollider(true);
            rb.AddForce(transform.forward * slideForce,ForceMode.Impulse);
            currentSlideTime += Time.deltaTime;
        }
    }

    public void JumpPressUI(){
        wantingToJump = true;
        CancelInvoke(nameof(ResetWantToJump));
        Invoke(nameof(ResetWantToJump),.1f);
    }
    private void ResetWantToJump(){
        wantingToJump = false;
    }
    public void ToggleCrouchUI(){
        if(!areWeSliding && !areWeCrouching && !isJumping){
            if(input.z > 0f && wantingToSprint){
                areWeSliding = true;
                animationManager.SetCrouch(true);
                return;
            }
            
        }
        wantingToCrouch = !wantingToCrouch;
        animationManager.SetCrouch(wantingToCrouch);
    }
    private Vector2 currentRecoilAmount;
    private void CameraUpdate() {
        /* // Allow the script to clamp based on a desired target value.
        var targetOrientation = Quaternion.Euler(targetDirection);
        var targetCharacterOrientation = Quaternion.Euler(targetCharacterDirection);

        // Get raw mouse input for a cleaner reading on more sensitive mice.
        Vector2 mouseDelta = new Vector2(inputController.GetLookVector.x,inputController.GetLookVector.y);
        crossHairExpandOnCameraMove = mouseDelta.magnitude > 1.5f ? 1f : 0f;
        // Scale input against the sensitivity setting and multiply that against the smoothing value.
        mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity.x * smoothing.x, sensitivity.y * smoothing.y));
        // Interpolate mouse movement over time to apply smoothing delta.
        _smoothMouse.x = Mathf.Lerp(_smoothMouse.x, mouseDelta.x, 1f / smoothing.x);
        _smoothMouse.y = Mathf.Lerp(_smoothMouse.y, mouseDelta.y, 1f / smoothing.y);

        // Find the absolute mouse movement value from point zero.
        _mouseAbsolute += _smoothMouse;
        _mouseAbsolute = new Vector2(_mouseAbsolute.x + loadout.RecoilAmount.y,_mouseAbsolute.y + loadout.RecoilAmount.x);
        // Clamp and apply the local x value first, so as not to be affected by world transforms.
        if (clampInDegrees.x < 360){
            _mouseAbsolute.x = Mathf.Clamp(_mouseAbsolute.x, -clampInDegrees.x * 0.5f, clampInDegrees.x * 0.5f);
        }

        // Then clamp and apply the global y value.
        if (clampInDegrees.y < 360){
            _mouseAbsolute.y = Mathf.Clamp(_mouseAbsolute.y, -clampInDegrees.y * 0.5f, clampInDegrees.y * 0.5f);
        }
        // PSerialized.UpdateData(_mouseAbsolute.y,currentBodyColliderHeight,currentCamearHeight,bodyCollider.center);
        rotationCamera.transform.localRotation = Quaternion.AngleAxis(-_mouseAbsolute.y, targetOrientation * Vector3.right) * targetOrientation;

        var yRotation = Quaternion.AngleAxis(_mouseAbsolute.x, Vector3.up);
        // var yRotation = Quaternion.AngleAxis(_mouseAbsolute.x, Vector3.up);
        transform.localRotation = yRotation * targetCharacterOrientation; */
        _mouseAbsolute = new Vector3(inputController.GetLookVector.x + loadout.RecoilAmount.y,
            inputController.GetLookVector.y + loadout.RecoilAmount.x) * sensitivity * Time.deltaTime;
        
        pitch = Mathf.Clamp(pitch -_mouseAbsolute.y,cameraRotLimit.x,cameraRotLimit.y);
        rotationCamera.transform.localRotation = Quaternion.Euler(pitch,0f,0f);

        transform.Rotate(transform.up,_mouseAbsolute.x);
        
        
    }

    private void FixedUpdate() {
        if(isAi) return;
        if(photonView.IsMine){
            // Lock cursor handling
            if (lockCursor)
                Cursor.lockState = CursorLockMode.Locked;
            else
                Cursor.lockState = CursorLockMode.None;

            // Double check if we are on the ground or not (Changes current speed if true)
            // --- QUICK EXPLINATION --- 
            // transform.position.y - transform.localScale.y + 0.1f
            // This puts the start of the ray 0.1f above the bottom of the player
            // We then shoot a ray 0.15f down, this exists the player with 0.5f to hit objects
            // Removing this +- of 0.1f and having it shoot directly under the player can skip the ground as sometimes the capsules bottom clips through the ground
            if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y - transform.localScale.y + 0.1f, transform.position.z), Vector3.down, 0.15f)){
                handleHitGround();
            }

            // Sprinting
            if (wantingToSprint && areWeGrounded && !areWeCrouching)
                currentSpeed = sprintMoveSpeed;
            else if (!areWeCrouching && areWeGrounded)
                currentSpeed = walkMoveSpeed;
            // Crouching 
            if(areWeCrouching){
                if(wantingToJump){
                    CrouchCollider(false);
                }
            }
            // Can be simplified to Crouch((wantingToCrouch && jumpCrouching)); though the bellow is more readable
            if(!areWeSliding){
                if (wantingToCrouch){
                    CrouchCollider(true);
                } else{
                    CrouchCollider(false);
                }
            }else{
                Slide();
            }


            // Coyote timer (When the player leaves the ground, start counting down from the set value coyoteTime)
            // This allows players to jump late. After they have left 
            if (areWeGrounded)
                coyoteTimeCounter = coyoteTime;
            else
                coyoteTimeCounter -= Time.deltaTime;

            // Jump buffer timer (When the player leaves the ground, start counting down from the set value jumpBuffer)
            // This will "buffer" the input and allow for early space presses to be valid and no longer ignored
            if (wantingToJump && !wantingToCrouch)
                jumpBufferCounter = jumpBuffer;
            else
                jumpBufferCounter -= Time.deltaTime;

            if(!areWeCrouching){
                currentSpeed = loadout.IsAiming ? walkMoveSpeed / 2f : walkMoveSpeed;
            }
            // If the coyote timer has not run out and our jump buffer has not run out and we our cool down (canJump) is now over
            if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f && jumpCooldownOver) {
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

                jumpCooldownOver = false;
                areWeGrounded = false;
                jumpBufferCounter = 0f;
                currentSpeed = jumpMoveSpeed;
                endJumpTime = Time.time + jumpTime;

                // Wait jumpCooldown (1f = 1 second) then run the jumpCoolDownCountdown() void
                Invoke(nameof(jumpCoolDownCountdown), jumpCooldown);
            } else if (wantingToJump && !wantingToCrouch && !areWeGrounded && endJumpTime > Time.time) {
                // Hold down space for a further jump (until the timer runs out)
                rb.AddForce(Vector3.up * jumpAccel, ForceMode.Acceleration);
            }

            // WSAD movement
            input = input.normalized;
            Vector3 forwardVel = transform.forward * currentSpeed * input.z;
            Vector3 horizontalVel = transform.right * currentSpeed * input.x;
            rb.velocity = horizontalVel + forwardVel + new Vector3(0, rb.velocity.y, 0);

            //Extra gravity for more nicer jumping
            rb.AddForce(new Vector3(0, -extraGravity, 0), ForceMode.Impulse);
        }/* else{
            rb.position = Vector3.MoveTowards(rb.position, networkPositon, Time.fixedDeltaTime);
            rb.rotation = Quaternion.RotateTowards(rb.rotation, networkRotation, Time.fixedDeltaTime * 100.0f);
        } */
    }
    

    private void jumpCoolDownCountdown() {
        jumpCooldownOver = true;
    }
    
    // Crouch handling
    private void CrouchCollider(bool crouch) {
        areWeCrouching = crouch;
        

        if (crouch) {
            // If the player is crouching
            currentSpeed = crouchMoveSpeed;

            if (smoothCrouch) {

                currentBodyColliderHeight = Mathf.Lerp(currentBodyColliderHeight,crouchColliderHeight,crouchDownSpeed);
                currentBodyColliderCenterY = Mathf.Lerp(currentBodyColliderCenterY,crouchColliderOffset,crouchDownSpeed);
                currentCamearHeight = Mathf.Lerp(currentCamearHeight,crouchCameraHeight,crouchDownSpeed);
                // currentMeshScaleY = Mathf.Lerp(currentMeshScaleY,crouchMeshScale,crouchDownSpeed);
                // currentHeadColliderCenterY = Mathf.Lerp(currentHeadColliderCenterY,crouchHeadColliderCenterY,crouchDownSpeed);
                bodyCollider.height = currentBodyColliderHeight;

                bodyCollider.center = new Vector3(bodyCollider.center.x,/* Setting Final Value */ currentBodyColliderCenterY,bodyCollider.center.z);
                cameraHolder.localPosition = new Vector3(cameraHolder.localPosition.x,currentCamearHeight,cameraHolder.localPosition.z);
                // headCollider.center = new Vector3(headCollider.center.x,currentHeadColliderCenterY,headCollider.center.z);
            }
        } else {
            // If the player is standing
            if (smoothCrouch) {
                currentBodyColliderHeight = Mathf.Lerp(currentBodyColliderHeight,defultColliderHeight,crouchDownSpeed);
                currentBodyColliderCenterY = Mathf.Lerp(currentBodyColliderCenterY,defultBodyColliderCenterY,crouchDownSpeed);
                currentCamearHeight = Mathf.Lerp(currentCamearHeight,camerHolderDefultHeight,crouchDownSpeed);
                // currentMeshScaleY = Mathf.Lerp(currentMeshScaleY,standingMeshScale,crouchDownSpeed);
                // currentHeadColliderCenterY =Mathf.Lerp(currentHeadColliderCenterY,defultHeadColliderCenterY,crouchDownSpeed);
                bodyCollider.height = currentBodyColliderHeight;
                bodyCollider.center = new Vector3(bodyCollider.center.x,/* Setting Final Value */ currentBodyColliderCenterY,bodyCollider.center.z);
                cameraHolder.localPosition = new Vector3(cameraHolder.localPosition.x,currentCamearHeight,cameraHolder.localPosition.z);
                // headCollider.center = new Vector3(headCollider.center.x,currentHeadColliderCenterY,headCollider.center.z);
            }
        }
    }
    

    // Ground check
    //****** make sure whatever you want to be the ground in your game matches the tag set in the script
    private void OnCollisionEnter(Collision other) {
        if(photonView.IsMine){
            if (other.gameObject.CompareTag(groundTag)){
                float angle = Vector3.Angle(other.GetContact(0).normal,Vector3.up);
                // Debug.Log("Angle to hit " + angle);
                if(angle <= 0){
                    handleHitGround();
                    isJumping = false;
                    animationManager.Jump(false);
                }
            }
        }
    }

    // This is separated in its own void as this code needs to be run on two separate occasions, saves copy pasting code
    // Just double checking if we are crouching and setting the speed accordingly 
    private void handleHitGround() {
        if (areWeCrouching){
            currentSpeed = crouchMoveSpeed;
        } else{
            currentSpeed = walkMoveSpeed;
        }

        areWeGrounded = true;
    }

    public void TrySync () {
        if (photonView.IsMine){
            photonView.RPC(nameof(SyncProfile), RpcTarget.All, Launcher.myProfile.username, Launcher.myProfile.level, Launcher.myProfile.xp);
            switch(MatchHandler.Current.GetGameSettingsSO().gameMode){
                case GameMode.FFA:
                case GameMode.NIGHTMARE:
                    playerData.SetPlayerType(PlayerData.PlayerType.Frindely);
                    ColorTeamIndicators(Random.ColorHSV());
                break;
                case GameMode.TDM:
                    photonView.RPC(nameof(SyncTeam), RpcTarget.All, MatchHandler.Current.GetGameSettingsSO().IsAwayTeam);
                break;
            }
        }else{
            playerData.SetPlayerType(PlayerData.PlayerType.Enemy);
        } 
    }
    private void ColorTeamIndicators (Color p_color) {
        teamIndicatorImage.color = p_color;
    }
    [PunRPC]
    private void SyncProfile(string p_username, int p_level, int p_xp) {
        playerProfile = new ProfileData(p_username, p_level, p_xp);
        loadout.SetPlayerData(playerProfile);
    }

    [PunRPC]
    private void SyncTeam(bool p_awayTeam) {
        awayTeam = p_awayTeam;
        if (p_awayTeam){
            ColorTeamIndicators(Color.red);
        }else{
            ColorTeamIndicators(Color.blue);
        }
    }
    public bool GetIsRead(){
        return isReady;
    }

}

