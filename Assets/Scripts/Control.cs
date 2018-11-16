using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Control : MonoBehaviour {

    private FMOD.Studio.EventInstance release;
    private FMOD.Studio.EventInstance falling;
    private FMOD.Studio.EventInstance stretch;
    private FMOD.Studio.EventInstance levelFail;
    private FMOD.Studio.EventInstance levelDone; // probably not needed
    private FMOD.Studio.EventInstance attach;

    public GameObject flower;
    public GameObject tongue;
    public float maxStretch = 15.0f;
    public float bottomOfScreen;
    public static bool dragReleased = false;

    private SpringJoint2D spring;
    private new Rigidbody2D rigidbody;
    private SpriteRenderer spriteRenderer;
    private new CircleCollider2D collider;
    private LineRenderer lineRenderer;
    private Animator animator;

    public GameObject originalFlower;

    public static bool dragging = false;
    public static bool flying = false;
    private Ray rayToMouse;
    private float maxStretchSqr;
    private Vector2 prevVelocity;
    private Vector2 maxVelocity;
    private float calcVelocity;
    private Vector3 springDistance;
    private Vector3 respawnPosition;
    private Quaternion originalRotation;
    private float rotationResetSpeed = 1.0f;
    private Vector3 initialVelocity;
    private bool flyStarted = false;
    public List<GameObject> flowers;
    private bool flowerDisappearedDragging = false;

    private float idleTime;

    void Awake() {
        spring = GetComponent<SpringJoint2D>();
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<CircleCollider2D>();
        // colliderCircle = GetComponent<CircleCollider2D>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.sortingOrder = 7;
        animator = GetComponent<Animator>();
        // disable multitouch:
        Input.multiTouchEnabled = false;
    }

    // Use this for initialization
    void Start() {
        release = FMODUnity.RuntimeManager.CreateInstance("event:/release");
        stretch = FMODUnity.RuntimeManager.CreateInstance("event:/stretch");
        falling = FMODUnity.RuntimeManager.CreateInstance("event:/falling");
        attach = FMODUnity.RuntimeManager.CreateInstance("event:/attach");
        levelFail = FMODUnity.RuntimeManager.CreateInstance("event:/levelFail");

        Debug.Log("sprite order: " + spriteRenderer.sortingOrder + " line order: " + lineRenderer.sortingOrder);
        rayToMouse = new Ray(flower.transform.position, Vector3.zero);
        maxStretchSqr = maxStretch * maxStretch;
        bottomOfScreen = -5.0f;
        respawnPosition = transform.position;
        originalRotation = transform.rotation;
        initialVelocity = Vector3.zero;

        flower.layer = 9;
        //Debug.Log("flower: "+flower.gameObject.name);
        originalFlower = flower;

        idleTime = Random.Range(2.00f, 4.00f);

    }

    // Update is called once per frame
    void FixedUpdate() {

        //Debug.Log("flying = " + flying + ", dragging = " + dragging + "\nflyStarted = " + flyStarted + ", flowerDisappearedDragging = " + flowerDisappearedDragging);
        if (dragging) {
            Dragging();
        }
        if (flying) {
            Flying();
            //lineRenderer.enabled = true;
            dragReleased = false;
        }
        if (!flying) {
            if (!rigidbody.isKinematic && prevVelocity.sqrMagnitude > rigidbody.velocity.sqrMagnitude) {
                spring.enabled = false;
                flying = true;
                rigidbody.velocity = prevVelocity;
                maxVelocity = prevVelocity;
                Debug.Log("FLY!! " + springDistance + ", " + maxVelocity + ", " + initialVelocity);

                // jumping sound yeeey!
                FMODUnity.RuntimeManager.PlayOneShot("event:/release", Camera.main.transform.position);
            }
            if (!dragging) {
                prevVelocity = rigidbody.velocity;
            }
        }

        if (idleTime <= 0)
        {
            animator.SetTrigger("idle");
            idleTime = Random.Range(2.00f, 4.00f);

        }
        else
        {
            idleTime -= Time.deltaTime;
            //Debug.Log(idleTime);
        }


    }

    void OnPointerDown() {
        spring.enabled = false;
        dragging = true;
        //animator.SetBool("reset", false);
        //animator.SetBool("grab", false);
    }

    void OnPointerUp() {
        animator.SetBool("grab", false);

        if (!flowerDisappearedDragging) {
            //animator.SetBool("launch", true);
            animator.SetTrigger("launch");
            spring.enabled = true;
            rigidbody.isKinematic = false;
            dragging = false;
            dragReleased = true;
            lineRenderer.enabled = false;
        } else {
            flowerDisappearedDragging = false;
        }

        collider.radius = 0.7f;
    }

#if UNITY_EDITOR
    void OnMouseDown() {
        OnPointerDown();
    }

    void OnMouseUp() {
        OnPointerUp();
    }
#endif

    void Dragging() {
        Vector3 mouseWorldPoint = Vector3.zero;

#if UNITY_EDITOR
        mouseWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
#endif
#if UNITY_ANDROID
        for (int i = 0; i < Input.touchCount; ++i) {
            mouseWorldPoint = Camera.main.ScreenToWorldPoint(Input.GetTouch(i).position);
        }
#endif
        Vector2 flowerToMouse = mouseWorldPoint - flower.transform.position;

        rayToMouse.direction = flowerToMouse;

        if (flowerToMouse.sqrMagnitude > maxStretchSqr) {
            mouseWorldPoint = rayToMouse.GetPoint(maxStretch);
        }

        mouseWorldPoint.z = 0f;
        transform.position = mouseWorldPoint;

        // rotate player towards flower
        Vector3 vectorToTarget = transform.position - flower.transform.position;
        float angle = (Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg) + 90;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, 90);

        springDistance = rayToMouse.GetPoint(spring.distance) - transform.position;

        initialVelocity = (springDistance) * 11.0f;
        if (springDistance.sqrMagnitude > 0.75f && springDistance.y > -0.3) {
            lineRenderer.enabled = true;
            UpdateTrajectory(transform.position, initialVelocity, Physics2D.gravity * rigidbody.gravityScale);
        } else {
            lineRenderer.enabled = false;
        }

        // stretch sound
        //FMODUnity.RuntimeManager.PlayOneShot("event:/stretch", Camera.main.transform.position);
    }

    void Flying() {
        if (!flyStarted) {
            collider.radius = 0.7f;
            rigidbody.angularVelocity = 500.0f;
            flyStarted = true;
        }


        if (transform.position.y < (Camera.main.ScreenToWorldPoint(new Vector3(0.0f, bottomOfScreen)).y - GetComponent<CircleCollider2D>().radius)) {
            SendMessage("Die");
        }

    }

    void Reset() {
        transform.position = respawnPosition;
        spring.enabled = true;
        rigidbody.isKinematic = true;
        flying = false;
        //animator.SetBool("launch", false);
        animator.SetTrigger("grab");
        //animator.SetBool("grab", true);

        collider.radius = 2.5f;

        transform.rotation = Quaternion.Slerp(transform.rotation, originalRotation, Time.time * rotationResetSpeed);
        lineRenderer.enabled = false;

        flyStarted = false;

        flower.layer = 9;
        flower.GetComponent<CircleCollider2D>().enabled = false;
        flower.GetComponent<CircleCollider2D>().isTrigger = false;
        rayToMouse.origin = flower.transform.position;
        tongue.GetComponent<TongueMovement>().flower = flower;

        TongueMovement.stretchAllowed = true;
        flower.SendMessage("Reset");
    }

    void ResetLevel() {
        flower = originalFlower;
        flower.layer = 8;
        Grabbing();
        foreach (GameObject f in flowers) {
            f.SetActive(true);
            f.GetComponent<CircleCollider2D>().enabled = true;
            f.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            f.layer = 8;
            f.GetComponent<CircleCollider2D>().isTrigger = true;
        }
        //animator.SetBool("reset", true);
        animator.SetTrigger("reset");
        GameObject.FindGameObjectWithTag("GameController").GetComponent<Score>().ResetScore();
    }

    void Grabbing() {
        //spring.connectedAnchor = flower.transform.position;
        spring.connectedBody = flower.GetComponent<Rigidbody2D>();
        spring.enabled = true;
        respawnPosition = flower.transform.position;
        respawnPosition.y -= spring.distance;
        Reset();
        flower.SendMessage("Wither");
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.layer == 8 && flying) {
            flower.layer = 8;
            flower.GetComponent<CircleCollider2D>().enabled = true;
            flower.GetComponent<CircleCollider2D>().isTrigger = true;
            flower = col.gameObject;
            Grabbing();
            Debug.Log("attempting to grab!" + col.gameObject.name);
            FMODUnity.RuntimeManager.PlayOneShot("event:/attach", Camera.main.transform.position);
        }

        if (col.gameObject.layer == 12 && flying) {
            SendMessage("Die");
            Vector2 pos = transform.position;
            col.GetComponent<ParticleBadFlower>().Init(pos);
        }
    }

    void UpdateTrajectory(Vector3 initialPosition, Vector3 initialVelocity, Vector3 gravity) {

        int numSteps = 30; // for example
        float timeDelta = 0.5f / initialVelocity.magnitude; // for example

        lineRenderer.SetVertexCount(numSteps);
        Vector3 position = initialPosition;
        Vector3 velocity = initialVelocity;
        for (int i = 0; i < numSteps; ++i) {
            lineRenderer.SetPosition(i, position);
            /*if (Physics2D.OverlapPoint((Vector2)position, 1 << 10))
            {
                velocity.x = -velocity.x;
            }*/
            position += velocity * timeDelta + 0.5f * gravity * timeDelta * timeDelta;
            position.z = -0.5f;
            velocity += gravity * timeDelta;

        }

    }

    void FlowerDisappeared() {
        collider.radius = 0.7f;
        Debug.Log("FlowerGONE!");
        flying = true;
        rigidbody.isKinematic = false;
        spring.enabled = false;
        //animator.SetBool("grab", false);
        //animator.SetBool("launch", true);
        animator.SetTrigger("launch");
        TongueMovement.stretchAllowed = false;
        if (dragging) {
            dragging = false;
            flowerDisappearedDragging = true;
        }

        FMODUnity.RuntimeManager.PlayOneShot("event:/falling", Camera.main.transform.position);
    }
}