using UnityEngine;
using System.Collections;

public class ParticleLeaves : MonoBehaviour {

    public GameObject particlePrefab;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }


    public void OnCollisionEnter2D(Collision2D col)
    {
        Vector2 pos = col.contacts[0].point;
        Debug.Log("leaves particle");
        Quaternion rot = new Quaternion(0, 0, 0, 0);
        Instantiate(particlePrefab, pos, rot);
    }
}
