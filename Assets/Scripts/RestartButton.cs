using UnityEngine;
using System.Collections;

public class RestartButton : MonoBehaviour {

    private FMOD.Studio.EventInstance clickOut;
    private FMOD.Studio.EventInstance clickIn;

	// Use this for initialization
	void Start () {
        clickOut = FMODUnity.RuntimeManager.CreateInstance("event:/clickOut");
        clickIn = FMODUnity.RuntimeManager.CreateInstance("event:/clickIn");
	}
	
	// Update is called once per frame
	void Update () {
        RaycastHit2D hit;
        GameObject hitObject = null;
        if (Input.touchCount > 0) {
     
            // Construct a ray from the current touch coordinates
            hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position), Vector2.zero);
            
            if (hit.transform != null) {
                hitObject = hit.transform.gameObject;

                if (Input.GetTouch(0).phase == TouchPhase.Began && hitObject.tag == "RestartButton") {
                    Debug.Log("Began RESTARTBUTTON");

                    OnPointerDown();
                }
            }
            if (Input.GetTouch(0).phase == TouchPhase.Ended && hitObject.tag == "RestartButton") {
                Debug.Log("Ended RESTARTBUTTON");

                

                //hitObject.SendMessage("OnPointerUp");
                OnPointerUp();
            }
        }
	}

    void OnPointerDown() {
        Debug.Log("OnPointerDown RESTARTBUTTON");
        FMODUnity.RuntimeManager.PlayOneShot("event:/clickIn", Camera.main.transform.position);
       // GameObject.FindGameObjectWithTag("Player").SendMessage("ResetLevel");
    }

    void OnPointerUp() {
        Debug.Log("OnPointerUp RESTARTBUTTON");
        FMODUnity.RuntimeManager.PlayOneShot("event:/clickOut", Camera.main.transform.position);
        Destroy(GameObject.FindGameObjectWithTag("RestartButton"));

        //GameObject.FindGameObjectWithTag("RestartButton").SetActive(false);

        //GameObject.FindGameObjectWithTag("Player").SendMessage("ResetLevel");
        GameObject.FindGameObjectWithTag("Player").SendMessage("Reset");
    }

    void OnMouseDown() {
        OnPointerDown();
    }

    void OnMouseUp() {
        OnPointerUp();
    }
}
