using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class WinLose : MonoBehaviour {

    public GameObject restartButtonPrefab;

    public GUIText winText;
   // public GUIText loseText;
    private float time = 5.0f;
    private float timeRounded;
    private bool timerStarted = false;

    private Vector3 position;
    private string winString = "LEVEL CLEARED!\nNext level in ";
    private string loseString = "GAME OVER!\nLevel restarting in ";
    private string resetString;

    private LineRenderer lineRenderer;

    public bool isLevelFail = true;

	// Use this for initialization
	void Start() {
        winText.text = "";
        lineRenderer = GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
      
	}

    void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.layer == 11) { // end of level, win
            Debug.Log("collider: " + col.name);
            Win();
        }
    }

    void Die() {
        winText.gameObject.GetComponent<Score>().isEnabled = false;
       // timerStarted = true;
        resetString = loseString;
        position = winText.GetComponent<RectTransform>().position;
        winText.GetComponent<RectTransform>().position = new Vector3(0.5f, 0.35f, 0.5f);
        winText.enabled = true;
        GetComponent<Rigidbody2D>().isKinematic = true;

        //GetComponent<LineRenderer>().enabled = false;
        lineRenderer.enabled = false;

        if (GameObject.Find("resetButtonPrefab(Clone)") == null) {
            Debug.Log("restartButton created");
            Instantiate(restartButtonPrefab, new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0.0f), new Quaternion(0, 0, 0, 0));
        }
        /*else if (GameObject.Find("resetButtonPrefab(Clone)") != null && GameObject.FindGameObjectWithTag("RestartButton").activeSelf == false) {
            Debug.Log("restartButton activated");
            GameObject.FindGameObjectWithTag("RestartButton").SetActive(true);
        }*/
        
        if (isLevelFail) {
            FMODUnity.RuntimeManager.PlayOneShot("event:/levelFail", Camera.main.transform.position);
            isLevelFail = false;
        }

    }

    void Reset() {
        isLevelFail = true;
        winText.GetComponent<RectTransform>().position = position;
        winText.enabled = false;
        GetComponent<Rigidbody2D>().isKinematic = false;
        winText.gameObject.GetComponent<Score>().isEnabled = true;
        SendMessage("ResetLevel");
    }

    void Win() {
        winText.gameObject.GetComponent<Score>().isEnabled = false;
        timerStarted = true;
        resetString = winString;
        position = winText.GetComponent<RectTransform>().position;
        winText.GetComponent<RectTransform>().position = new Vector3(0.5f, 0.5f, 0.5f);
        winText.enabled = true;
        GetComponent<Rigidbody2D>().isKinematic = true;

        //GetComponent<LineRenderer>().enabled = false;
        lineRenderer.enabled = false;
        GetComponent<Score>().isEnabled = false;
    }
}
