using UnityEngine;
using System.Collections;

public class FlowerWithering : MonoBehaviour {

    public GameObject player;

    private float baseTime = 3.0f;
    private float time;
    
    private bool timerStarted = false;

    private float alphaLevel;

    // Use this for initialization
    void Start () {
        time = baseTime;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (timerStarted) {
            time -= Time.deltaTime;

            alphaLevel = time / baseTime;
            GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, alphaLevel);

            if (time <= 0) {
                timerStarted = false;
                time = baseTime;
                GetComponent<CircleCollider2D>().enabled = false;
                gameObject.SetActive(false);
                if (!Control.flying && player.GetComponent<Control>().flower==transform.gameObject) {
                    player.SendMessage("FlowerDisappeared");
                }
            }
        }
    }

    void Wither() {
        timerStarted = true;
    }

    void Reset() {
        time = baseTime;
    }
}
