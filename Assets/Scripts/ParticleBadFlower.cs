using UnityEngine;
using System.Collections;

public class ParticleBadFlower : MonoBehaviour
{

    public GameObject particlePrefab ;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    public void Init(Vector2 pos)
    {
        Debug.Log("bad flower");
        Quaternion rot = new Quaternion(0,0,0,0);
        Instantiate(particlePrefab,pos,rot);
    }
}
