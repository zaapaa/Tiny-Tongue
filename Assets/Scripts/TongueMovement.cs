using UnityEngine;
using System.Collections;

public class TongueMovement : MonoBehaviour {

    public GameObject player;
    public GameObject flower;

    public GameObject tipPrefab;

    public static bool stretchAllowed = true;
    private GameObject tip;
    // Use this for initialization
    void Start() {
        //transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 90.0f);
        tip = (GameObject)Instantiate(tipPrefab,flower.transform.position,new Quaternion(0,0,0,0));
    }

    // Update is called once per frame
    void Update() {
        if (Control.flying) {
            GetComponent<SpriteRenderer>().enabled = false;
            tip.GetComponent<SpriteRenderer>().enabled = false;
            //   Debug.Log("Active Self: " + gameObject.activeSelf);
        }
        if (!Control.flying) {
            GetComponent<SpriteRenderer>().enabled = true;
            tip.GetComponent<SpriteRenderer>().enabled = true;
        }
        if (stretchAllowed) {
            Stretch(transform.gameObject, flower.transform.position, player.transform.position, false);
        }

        // rotate tip towards player
        Vector3 vectorToTarget = tip.transform.position - player.transform.position;
        float angle = (Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg) + 270;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        tip.transform.rotation = Quaternion.Slerp(transform.rotation, q, 90);

        tip.transform.position = flower.transform.position;

    }

    public void Stretch(GameObject sprite, Vector3 initialPosition, Vector3 finalPosition, bool mirrorZ) {
        Vector3 centerPos = (initialPosition + finalPosition) / 2f;
        sprite.transform.position = centerPos;
        Vector3 direction = finalPosition - initialPosition;
        direction = Vector3.Normalize(direction);
        sprite.transform.right = direction;
        if (mirrorZ)
            sprite.transform.right *= -1f;
        Vector3 scale = new Vector3(0.7f, 0.7f, 0.7f);
        scale.x = Vector3.Distance(initialPosition, finalPosition) / 2f;
        sprite.transform.localScale = scale;
    }
}

