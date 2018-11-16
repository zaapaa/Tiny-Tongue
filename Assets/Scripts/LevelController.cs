using UnityEngine;
using System.Collections.Generic;
using System.Deployment.Internal;

public class LevelController : MonoBehaviour {
    public GameObject cam;

    public GameObject wallPrefab;

    public GameObject bgStartPrefab;

    public GameObject flowerPrefab;
    public GameObject flowerFirstPrefab;
    public GameObject flowerBadPrefab;

    public GameObject playerPrefab;
    public GameObject tonguePrefab;

    public GameObject obstacleMiddlePrefab;
    public GameObject obstacleSide1Prefab;
    public GameObject obstacleSide2Prefab;
    public GameObject windPrefab;

    public GameObject winPrefab;

    public int objectSpacing = 128;
    public float springDistance;

    public bool generateRandom;

    private float lastWallY;
    private float lastObjectY;

    private GameObject firstFlower;
    private GameObject flower;
    public GameObject player;
    private GameObject win;
    private Texture2D level;
    private Quaternion noRot;
    private float wallwidth;
    private float wallheight;
    private float bgwidth;
    public float bgheight;
    private float levelheight;
    private float levelwidth;

    private List<GameObject> flowers;

    // Use this for initialization
    void Start() {
        cam = Camera.main.gameObject;
        flowers = new List<GameObject>();

        noRot = new Quaternion(0, 0, 0, 0);

        InitializeLevel();
        InitializePlayer();
    }

    // Update is called once per frame
    void Update() {
        RandomLevel();
    }

    private void InitializeLevel() {
        if (!generateRandom) {
            level = Resources.Load<Texture2D>("Levels/demo");
        } else {
            level = Resources.Load<Texture2D>("Levels/randomStart");
        }

        wallwidth = wallPrefab.GetComponent<SpriteRenderer>().sprite.texture.width / 100.0f;
        wallheight = wallPrefab.GetComponent<SpriteRenderer>().sprite.texture.height / 100.0f;
        bgwidth = bgStartPrefab.GetComponent<SpriteRenderer>().sprite.texture.width / 100.0f;
        bgheight = bgStartPrefab.GetComponent<SpriteRenderer>().sprite.texture.height / 100.0f;
        levelheight = level.height * objectSpacing / 100.0f;
        levelwidth = bgwidth;

        lastWallY = wallheight / 2;

        GetComponent<ParallaxEffect>().bgheight = bgheight;


        int wallAmt = (int)Mathf.Ceil(levelheight / wallheight);

        //Debug.Log("level height: " + levelheight + " wallheight: " + wallheight + " wallAmt: " + wallAmt);
        //Debug.Log("bgStartHeight " + bgStartHeight + " bgEndHeight: " + bgEndHeight + " bgTileHeight: " + bgTileHeight);
        //Debug.Log("bgheight: " + bgheight + " bgAmt: " + bgAmt);

        Color32 pixelColor;



        float x;
        float y = 0;

        //1. generate walls
        lastWallY = GenerateWalls(wallAmt);
        //2. generate backgrounds
        GenerateBackgrounds(true);
        

        //3. generate predefined level from bitmap pixel colours. width always 3 --> 3 "lanes".
        Color realGray = new Color32(127, 127, 127, 255);
        Color badFlower = new Color32(50, 50, 50, 255);
        Color obstacleMiddleColor = new Color32(255, 0, 0, 255);
        Color obstacleSide1Color = new Color32(255, 0, 50, 255);
        Color obstacleSide2Color = new Color32(255, 0, 100, 255);
        Color winColor = new Color32(255, 255, 0, 255);
        Color32[] pixels = level.GetPixels32();
        for (int i = 0; i < level.width; i++) {
            for (int j = 0; j < level.height; j++) {
                x = levelwidth / (level.width + 1) * (i + 1) + wallwidth; // -(wallwidth/5f);
                y = levelheight / level.height * (j + 1); //+ wallheight - (wallheight/5f);
                if (generateRandom) {
                    y += 3f;
                }
                int index = (j * level.width) + i;
                pixelColor = pixels[index];
                //Debug.Log("colour: " + pixelColor.ToString() + ", loc x:" + i + " y:" + j + " index:" + index);
                if (pixelColor == Color.white) {
                    flower = (GameObject)Instantiate(flowerPrefab, new Vector3(x, y), noRot);
                    flowers.Add(flower);
                } else if (pixelColor == realGray) {
                    //Debug.Log("Found pixel colour: " + pixelColor.ToString() + " with location x:" + i + " y: " + j + " Spawning first flower!");
                    firstFlower = (GameObject)Instantiate(flowerFirstPrefab, new Vector3(x, y), noRot);
                } else if (pixelColor == obstacleMiddleColor) {
                    Instantiate(obstacleMiddlePrefab, new Vector3(x, y), noRot);
                } else if (pixelColor == obstacleSide1Color) {
                    GenerateObstacleSide(x, y, i, obstacleSide1Prefab);
                } else if (pixelColor == obstacleSide2Color) {
                    GenerateObstacleSide(x, y, i, obstacleSide2Prefab);
                } else if (pixelColor == winColor) {
                    win = (GameObject)Instantiate(winPrefab, new Vector3(x, y), noRot);
                } else if (pixelColor == badFlower) {
                    Instantiate(flowerBadPrefab, new Vector3(x, y), noRot);
                }
                //ADD more types here if needed
            }
        }
        lastObjectY = y;

        Camera.main.orthographicSize = 10;
        Camera.main.transform.position = new Vector3(wallwidth + bgwidth / 2, Camera.main.orthographicSize, Camera.main.transform.position.z);

    }
    void InitializePlayer() {
        //4. generate player and attach to the starting flower with a spring joint
        player = (GameObject)Instantiate(playerPrefab, new Vector3(firstFlower.transform.position.x,
            firstFlower.transform.position.y - springDistance), new Quaternion(0, 0, 0, 0));
        GameObject tongue = (GameObject)Instantiate(tonguePrefab, (firstFlower.transform.position + player.transform.position) / 2, new Quaternion(0, 0, 0, 0));
        player.GetComponent<Control>().flower = firstFlower;
        player.GetComponent<Control>().tongue = tongue;
        player.GetComponent<Control>().flowers = flowers;
        player.GetComponent<SpringJoint2D>().connectedBody = firstFlower.GetComponent<Rigidbody2D>();
        player.GetComponent<SpringJoint2D>().distance = springDistance;
        player.GetComponent<WinLose>().winText = GameObject.FindGameObjectWithTag("GameController").GetComponent<GUIText>();
        tongue.GetComponent<TongueMovement>().flower = firstFlower;
        tongue.GetComponent<TongueMovement>().player = player;
        cam.GetComponent<CameraFollow>().player1 = player;

        foreach (GameObject f in flowers) {
            f.GetComponent<FlowerWithering>().player = player;
        }
    }

    float GenerateWalls(int wallAmount)//returns last wall's Y-coordinate
    {
        float y = 0;
        wallAmount++;
        for (int i = 0; i < wallAmount; i++) {
            y = lastWallY + i * wallheight;
            GameObject o = (GameObject)Instantiate(wallPrefab, new Vector3(wallwidth / 2, y), noRot);
            o.GetComponent<SpriteRenderer>().flipX = true; //flip one wall
            Instantiate(wallPrefab, new Vector3(wallwidth / 2 + wallwidth + bgwidth, y), noRot);
        }
        return y;
    }

    void GenerateBackgrounds(bool withStart) {
        GetComponent<ParallaxEffect>().InitBackgrounds(withStart, wallwidth + bgwidth / 2);
    }

    void RandomLevel() {
        if (generateRandom) {
            float playerY = player.transform.position.y;

            //Debug.Log("playerY: " + playerY + ", pixels: " + lastObjectY);
            if (playerY > lastWallY - 20.0f) {
                int pixelsToGenerate = 6;
                lastWallY = GenerateWalls(pixelsToGenerate);
            }
            if (playerY > lastObjectY - 20.0f) {
                int pixelsToGenerate = 10;
                GenerateLevelObj(pixelsToGenerate);
            }
        }
    }

    void GenerateLevelObj(int pixels) {

        //flowerPrefab;         0
        //flowerBadPrefab;      1
        //obstacleMiddlePrefab; 2
        //obstacleSide1Prefab;  3
        //obstacleSide2Prefab;  4
        //WindPrefab;           5

        int flowerP = 25;
        int flowerBadP = 20 + flowerP;
        int obstacleMiddleP = 25 + flowerBadP;
        int obstacleSide1P = 10 + flowerBadP;
        int obstacleSide2P = 15 + obstacleSide1P;
        int windP = 10 + obstacleSide2P;

        float y = 0;
        float x;
        int i, j;
        int[,] row = new int[pixels, level.width];
        for (i = 0; i < pixels; i++) {
            bool isValidRow = false;
            while (!isValidRow) {
                for (j = 0; j < level.width; j++) {
                    int r = Random.Range(0, 100);
                    if (r < flowerP) {
                        row[i, j] = 0;
                    } else if (r < flowerBadP && r >= flowerP) {
                        row[i, j] = 1;
                    } else if (r < obstacleMiddleP && r >= flowerBadP && j == 1) {
                        row[i, j] = 2;
                    } else if (r < obstacleSide1P && r >= flowerBadP && (j == 0 || j == 2)) {
                        row[i, j] = 3;
                        row[i, 1] = -1;
                        if (j == 0) {
                            row[i, 2] = 0;
                        } else {
                            row[i, 0] = 0;
                        }
                        //Debug.Log("i: " + i + "j: " + j + "row: " + row[i, 1]);
                    } else if (r < obstacleSide2P && r >= obstacleSide1P && (j == 0 || j == 2)) {
                        row[i, j] = 4;
                    } else if (r < windP && r >= obstacleSide2P && j == 1) {
                        row[i, j] = 5;
                    } else {
                        row[i, j] = -1;
                    }
                }
                if (CheckRow(row, i, 0) > 0) {
                    isValidRow = true;
                }
                if (CheckRow(row, i, 3) == 1 && row[i, 1] != -1) {
                    isValidRow = false;
                }
            }
        }


        for (i = 0; i < pixels; i++) {
            for (j = 0; j < level.width; j++) {
                y = lastObjectY + objectSpacing * 2 * (i + 1) / 100f;
                x = levelwidth / (level.width + 1) * (j + 1) + wallwidth;


                switch (row[i, j]) {
                    case 0:
                        GenerateFlower(x, y);
                        break;
                    case 1:
                        GenerateFlowerBad(x, y);
                        break;
                    case 2:
                        GenerateObstacleMiddle(x, y, j);
                        break;
                    case 3:
                        GenerateObstacleSide(x, y, j, obstacleSide1Prefab);
                        break;
                    case 4:
                        GenerateObstacleSide(x, y, j, obstacleSide2Prefab);
                        break;
                    case 5:
                        GenerateWind(x, y);
                        Debug.Log("WIND JEEEEEEEEEEEEEEEEEE!!!!!!!!!!");
                        break;
                }

            }
        }
        lastObjectY = y;
    }

    GameObject GenerateFlower(float x, float y) {
        flower = (GameObject)Instantiate(flowerPrefab, new Vector3(x, y), noRot);
        flowers.Add(flower);
        flower.GetComponent<FlowerWithering>().player = player;
        return flower;
    }

    GameObject GenerateFlowerBad(float x, float y) {
        GameObject obj = (GameObject)Instantiate(flowerBadPrefab, new Vector3(x, y), noRot);
        return obj;
    }

    GameObject GenerateObstacleMiddle(float x, float y, int i) {
        GameObject obj = (GameObject)Instantiate(obstacleMiddlePrefab, new Vector3(x, y), noRot);
        return obj;
    }

    GameObject GenerateObstacleSide(float x, float y, int i, GameObject prefab) {
        if (i == 0) {
            x = wallwidth + prefab.GetComponent<SpriteRenderer>().sprite.texture.width / 100.0f * prefab.transform.localScale.x / 2;
        } else if (i == level.width - 1) {
            x = wallwidth + levelwidth - prefab.GetComponent<SpriteRenderer>().sprite.texture.width / 100.0f * prefab.transform.localScale.x / 2;
        }
        GameObject obstacle = (GameObject)Instantiate(prefab, new Vector3(x, y), noRot);
        if (i == level.width - 1) {
            obstacle.transform.localScale = new Vector3(obstacle.transform.localScale.x * -1, obstacle.transform.localScale.y, obstacle.transform.localScale.z);
        }
        return obstacle;
    }

    GameObject GenerateWind(float x, float y) {
        GameObject obj = (GameObject)Instantiate(windPrefab, new Vector3(x, y), noRot);

        if (Random.Range(0,2) == 0) {
            obj.GetComponent<SpriteRenderer>().flipX = true;
            obj.GetComponent<AreaEffector2D>().forceMagnitude *= -1;
        }

        return obj;
    }

    int CheckRow(int[,] row, int rowToCheck, int objectType) {
        int sum = 0;
        for (int k = 0; k < level.width; k++) {
            if (row[rowToCheck, k] == objectType) {
                sum++;
            }
        }
        return sum;
    }

    int CheckRowForAll(int[,] row, int rowToCheck, int exclude = -1) {
        int sum = 0;
        for (int k = 0; k < level.width; k++) {
            if (row[rowToCheck, k] != -1 && row[rowToCheck, k] != exclude) {
                sum++;
            }
        }
        return sum;
    }
}
