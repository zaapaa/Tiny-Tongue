using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

    public float smoothTimeX;
    public float smoothTimeY;
    public GameObject player1;

    private Vector2 velocity;
    private float posX;
    private float posY;

	// Use this for initialization
	void Start() {
        //player1 = GameObject.FindGameObjectWithTag("Player");
	    posX = transform.position.x;
	    posY = transform.position.y;
	}

    void FixedUpdate() {
        if (Control.flying)
        {
            if (player1.transform.position.y > transform.position.y)
            {
                posY = Mathf.SmoothDamp(transform.position.y, player1.transform.position.y, ref velocity.y, smoothTimeY);
            }
            else if (player1.GetComponent<SpringJoint2D>().connectedBody.gameObject.transform.position.y >
                     transform.position.y)
            {

            }

            //posX = Mathf.SmoothDamp(transform.position.x, player1.transform.position.x, ref velocity.x, smoothTimeX);

            transform.position = new Vector3(posX, posY, transform.position.z);
        }
        else if(!Control.dragging)
        {
            posY = Mathf.SmoothDamp(transform.position.y, player1.transform.position.y, ref velocity.y, smoothTimeY);
            transform.position = new Vector3(posX, posY, transform.position.z);
        }

        /*if (Control.dragging)
        {
            posY = player1.transform.position.y;

            posX = player1.transform.position.x;

            transform.position = new Vector3(posX, posY, transform.position.z);
        }*/
        /*if (player1.GetComponent<SpringJoint2D>().connectedBody.gameObject.transform.position.y >
            transform.position.y)
        {
            posY = player1.transform.position.y;
            transform.position = new Vector3(posX, posY, transform.position.z);
        }*/
        posX = transform.position.x;
        posY = transform.position.y;
    }

}
