using UnityEngine;
using System.Collections;

public class Score : MonoBehaviour {

    public GUIText scoreText;
    private int score = 0;
    private float playerPositionY;

    public GameObject player;

    public bool isEnabled = true;

    private float scorePositionX = 0.90f;

    void Awake() {
        scoreText = GetComponent<GUIText>();
    }

	// Use this for initialization
	void Start () {
        scoreText.enabled = true;
        scoreText.text = "Score: " + score;
        
        player = GetComponent<LevelController>().player;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (isEnabled) {
            scoreText.GetComponent<RectTransform>().position = new Vector3(scorePositionX, 0.98f, 0.5f);
            playerPositionY = player.transform.position.y;
            scoreText.enabled = true;
            scoreText.fontSize = 25;
            scoreText.color = Color.white;
            UpdateScore();
        }
	}

    void UpdateScore() {
        if (score < (int)Mathf.Ceil(player.transform.position.y) - 11 && Control.flying) {
            score = (int)Mathf.Ceil(player.transform.position.y) - 11;
        }
        if (score >= 100) { // JOKU JÄRKEVÄ KEINO SIIRTÄÄ AINA 0.25f KUN 100, 1000 yms...
            scorePositionX -= 0.10f; // 0.25f
        }
        scoreText.text = "Score: " + score;
    }

    public void ResetScore() {
        score = 0;
    }
}
