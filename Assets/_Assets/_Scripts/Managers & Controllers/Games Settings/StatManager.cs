using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
public class StatManager : MonoBehaviour {
    [SerializeField] private GunSO gunSo;
    [SerializeField] private GameObject statViw;
    [SerializeField] private CanvasRenderer stateMeshRenderer;
    [SerializeField] private Material stateMeshMaterials;
    [SerializeField] private Texture2D stateTextureGrandient;
    [SerializeField] private float maxStatMoveRange = 145f;
    private Mesh stateMesh;
    private Vector3[] vertices;
    private Vector2[] uv;
    private int[] triangles;
    private void Start(){
        stateMesh = new Mesh();
        vertices = new Vector3[7];
        uv = new Vector2[7];
        triangles = new int[3 * 6];
    }
    private void Update(){
        if(gunSo == null){
            statViw.SetActive(false);
            return;
        }
        statViw.SetActive(true);
        UpdateVisuals();
    }
    private void UpdateVisuals(){
        float angleIncrement = 360f/ 6;
        int damageVertIndex = 1;
        int firerateStatVertIndex = 2;
        int recoilStatIndex = 3;
        int rangeStatIndex = 4;
        int ammoStatIndex = 5;
        int accuracyStatIndex = 6;


        Vector3 damageStatVert = Quaternion.Euler(0,0,-angleIncrement * 0) * Vector3.up * maxStatMoveRange * gunSo.shootConfig.damageConfig.GetStateNormalizedValue();
        Vector3 firerateStatVert = Quaternion.Euler(0,0,-angleIncrement * 1) * Vector3.up * maxStatMoveRange * gunSo.shootConfig.fireRate.GetStateNormalizedValue();
        Vector3 recoilStatVert = Quaternion.Euler(0,0,-angleIncrement * 2) * Vector3.up * maxStatMoveRange * gunSo.shootConfig.recoilConfig.GetStateNormalizedValue();
        Vector3 rangeStatVert = Quaternion.Euler(0,0,-angleIncrement * 3) * Vector3.up * maxStatMoveRange * gunSo.shootConfig.shootRange.GetStateNormalizedValue();
        Vector3 ammoStatVert = Quaternion.Euler(0,0,-angleIncrement * 4) * Vector3.up * maxStatMoveRange * gunSo.ammoConfig.GetStateNormalizedValue();
        Vector3 accuracyStatVert = Quaternion.Euler(0,0,-angleIncrement * 5) * Vector3.up * maxStatMoveRange * gunSo.accuracyStat.GetStateNormalizedValue();

        vertices[0] = Vector3.zero;
        vertices[1] = damageStatVert;
        vertices[2] = firerateStatVert;
        vertices[3] = recoilStatVert;
        vertices[4] = rangeStatVert;
        vertices[5] = ammoStatVert;
        vertices[6] = accuracyStatVert;

        uv[0] = Vector2.zero;
        uv[damageVertIndex] = Vector2.one;
        uv[firerateStatVertIndex] = Vector2.one;
        uv[recoilStatIndex] = Vector2.one;
        uv[rangeStatIndex] = Vector2.one;
        uv[ammoStatIndex] = Vector2.one;
        uv[accuracyStatIndex] = Vector2.one;


        triangles[0] = 0;
        triangles[1] = damageVertIndex;
        triangles[2] = firerateStatVertIndex;

        triangles[3] = 0;
        triangles[4] = firerateStatVertIndex;
        triangles[5] = recoilStatIndex;

        triangles[6] = 0;
        triangles[7] = recoilStatIndex;
        triangles[8] = rangeStatIndex;

        triangles[9] = 0;
        triangles[10] = rangeStatIndex;
        triangles[11] = ammoStatIndex;

        triangles[12] = 0;
        triangles[13] = ammoStatIndex;
        triangles[14] = accuracyStatIndex;

        triangles[15] = 0;
        triangles[16] = accuracyStatIndex;
        triangles[17] = damageVertIndex;


        stateMesh.vertices = vertices;
        stateMesh.uv = uv;
        stateMesh.triangles = triangles;
        stateMeshRenderer.SetMesh(stateMesh);
        stateMeshRenderer.SetMaterial(stateMeshMaterials,stateTextureGrandient);
    }
    public void SetGun(GunSO gun){
        this.gunSo = gun;
        
    }
}