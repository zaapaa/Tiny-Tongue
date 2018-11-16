using UnityEngine;
using System.Collections;
using System;

public class TouchControl : MonoBehaviour {
    private GameObject lastHitObject;

    public static bool isRestartButton = false;

    // Use this for initialization
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        RaycastHit2D hit;
        GameObject hitObject = null;
        // for (int i = 0; i < Input.touchCount; ++i)
        if (Input.touchCount > 0 && !Control.flying) {
            //   Debug.Log("Input.touchCount for");
            // Construct a ray from the current touch coordinates
            hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position), Vector2.zero);
            /* try
             {
                 if (hit.collider == null) {
                     Debug.Log("HIT.COLLIDER IS NULL");
                 }
                 hitObject = hit.transform.gameObject;
             }
             catch (Exception e) {
                 Debug.Log("HITOBJECT ERROR CATCH: " + e);
             }*/
            //Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
            if (hit.transform != null || lastHitObject != null) {
                if (hit.transform != null) {
                    hitObject = hit.transform.gameObject;
                }

                if (Input.GetTouch(0).phase == TouchPhase.Began) {
                    Debug.Log("Began ");

                    if (hitObject != null && lastHitObject == null) {
                        lastHitObject = hitObject;
                        Debug.Log("HITOBJECT IS NOT NULL AND LASTHITOBJECT WAS NULL");
                    } else if (hitObject == null) {
                        hitObject = lastHitObject;
                        Debug.Log("HITOBJECT WAS NULL");
                    }

                    //added
                    if (hitObject.tag == "RestartButton") {
                        Debug.Log("Began RESTARTBUTTON");
                        lastHitObject = hitObject;
                        isRestartButton = true;
                    }


                    hitObject.SendMessage("OnPointerDown");
                }
            } else {
                Debug.Log("BOTH ARE NULL");
            }
            if (Input.GetTouch(0).phase == TouchPhase.Ended) {
                Debug.Log("Ended");
                /*if (lastHitObject == hitObject)
                {
                  hitObject.SendMessage("OnPointerUpAsButton");
                }*/

                // added
                if (hitObject.tag == "RestartButton") {
                    Debug.Log("Ended RESTARTBUTTON");
                    isRestartButton = true;
                }

                lastHitObject.SendMessage("OnPointerUp");
                lastHitObject = null;
            }
        }
    }
}
