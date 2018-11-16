using System;
using UnityEngine;
using System.Collections;

public class ParallaxEffect : MonoBehaviour {
    public GameObject bgPrefab;
    public GameObject bgStartPrefab;
    public GameObject bgStart2Prefab;
    public GameObject bgStart3Prefab;
    public GameObject bgStart4Prefab;

    public GameObject bgTilePrefab;
    public GameObject bgTile2Prefab;
    public GameObject bgTile3Prefab;
    public GameObject bgTile4Prefab;

    private GameObject[] backgrounds;
    private GameObject[] backgrounds2;
    private GameObject[] backgrounds3;

    public float bgheight;

    private GameObject bgBase;

    private float[] parallaxScales;
    public float smoothing = 1.0f;

    private Transform cam;
    private Vector3 previousCamPos;

    void Awake() {
        cam = Camera.main.transform;
    }

    // Use this for initialization
    void Start() {
        previousCamPos = cam.position;

        parallaxScales = new float[4];
        for (int i = 0; i < 4; i++) {
            parallaxScales[i] = -i * 0.25f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        for (int i = 0; i < backgrounds2.Length; i++) {

            float bgUpperEdge = 0;
            float bg2UpperEdge = 0;
            GameObject temp;

            float camBottomEdge = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, 0.0f)).y; //cam.position.y - cam.GetComponent<Camera>().orthographicSize / 2;

            float parallax = (previousCamPos.y - cam.position.y) * parallaxScales[i];

            float background2TargetPosY = backgrounds2[i].transform.position.y + parallax;
            Vector3 backgroundStartTargetPos = new Vector3(backgrounds2[i].transform.position.x, background2TargetPosY, backgrounds2[i].transform.position.z);
            backgrounds2[i].transform.position = backgroundStartTargetPos;
            bg2UpperEdge = backgrounds2[i].transform.position.y + bgheight / 2;

            if (backgrounds[i] != null) {
                float backgroundTargetPosY = bgheight + backgrounds2[i].transform.position.y;
                Vector3 backgroundTargetPos = new Vector3(backgrounds[i].transform.position.x, backgroundTargetPosY, backgrounds[i].transform.position.z);
                backgrounds[i].transform.position = backgroundTargetPos;
                bgUpperEdge = backgrounds[i].transform.position.y + bgheight / 2;

                if (bgUpperEdge < camBottomEdge) {
                    Debug.Log("cam bottom edge: " + camBottomEdge + " bg-" + i + " upper edge: " + bgUpperEdge + " bg2-" + i + " upper edge" + bg2UpperEdge + " bg upper was lower than cam bottom!!");
                    temp = backgrounds[i];
                    backgrounds[i] = backgrounds2[i];
                    backgrounds2[i] = temp;
                }
                if (bg2UpperEdge < camBottomEdge) {
                    Debug.Log("cam bottom edge: " + camBottomEdge + " bg-" + i + " upper edge: " + bgUpperEdge + " bg2-" + i + " upper edge" + bg2UpperEdge + " bg2 upper was lower than cam bottom!!");
                    if (backgrounds2[i].name.ToLower().Contains("start"))
                    {
                        backgrounds2[i] = backgrounds3[i];
                    }
                    temp = backgrounds2[i];
                    backgrounds2[i] = backgrounds[i];
                    backgrounds[i] = temp;

                }
            }
        }

        previousCamPos = cam.position;
        bgBase.transform.position = new Vector3(bgBase.transform.position.x,cam.position.y);
    }

    public void InitBackgrounds(bool withStart, float x) {
        GameObject[] bg = new GameObject[4];
        GameObject[] bg2 = new GameObject[4];
        GameObject[] bg3 = new GameObject[4];
        Quaternion noRot = new Quaternion(0, 0, 0, 0);

        Debug.Log(bgheight);

        float y = bgheight/2;


        bgBase = (GameObject) Instantiate(bgPrefab, new Vector3(x, cam.transform.position.y), noRot);

        if (withStart) {
            bg2[0] = (GameObject)Instantiate(bgStartPrefab, new Vector3(x, y), noRot);
            bg2[1] = (GameObject)Instantiate(bgStart2Prefab, new Vector3(x, y), noRot);
            bg2[2] = (GameObject)Instantiate(bgStart3Prefab, new Vector3(x, y), noRot);
            bg2[3] = (GameObject)Instantiate(bgStart4Prefab, new Vector3(x, y), noRot);
        }

        y = 1000.0f;

        bg[0] = (GameObject)Instantiate(bgTilePrefab, new Vector3(x, y), noRot);
        bg[1] = (GameObject)Instantiate(bgTile2Prefab, new Vector3(x, y), noRot);
        bg[2] = (GameObject)Instantiate(bgTile3Prefab, new Vector3(x, y), noRot);
        bg[3] = (GameObject)Instantiate(bgTile4Prefab, new Vector3(x, y), noRot);
        bg3[0] = (GameObject)Instantiate(bgTilePrefab, new Vector3(x, y), noRot);
        bg3[1] = (GameObject)Instantiate(bgTile2Prefab, new Vector3(x, y), noRot);
        bg3[2] = (GameObject)Instantiate(bgTile3Prefab, new Vector3(x, y), noRot);
        bg3[3] = (GameObject)Instantiate(bgTile4Prefab, new Vector3(x, y), noRot);


        if (bg2[0] != null) {
            backgrounds2 = bg2;
        }
        backgrounds = bg;
        backgrounds3 = bg3;


    }
}
