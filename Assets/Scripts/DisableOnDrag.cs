using UnityEngine;
using System.Collections;

public class DisableOnDrag : MonoBehaviour {

  //  private bool isBounce; 

	// Use this for initialization
	void Start () {
     //   isBounce = true;
	}
	
	// Update is called once per frame
	void Update () {
        if (!Control.flying)
        {
            GetComponent<Collider2D>().enabled = false;
        }
        else {
            GetComponent<Collider2D>().enabled = true;  
        }
    }
    void OnCollisionEnter2D(Collision2D col) {
      //  if (isBounce) {
            Debug.Log("bounce sound");
            FMODUnity.RuntimeManager.PlayOneShot("event:/bounce", Camera.main.transform.position);
      //      isBounce = false;
      //  }
        
    }
}
