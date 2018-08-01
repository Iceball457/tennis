using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour {


    public const float FAULT_LINE = 6.4f;
    public const float COURT_WIDTH = 8.23f;
    public bool serveHit;
    //Singleton reference
    public static BallController ball;
    //otehr references
    public Rigidbody rb;
    AudioSource aud;
    public AudioClip bounce;
    //stats
    public Vector2 curve;
    public float curveDecay;
    public int bounces;
    public int recent;
    public float spinMagnitude;

    //singleton setup
    private void Awake () {
        if (ball != null) {
            ball.GetComponent<SphereCollider> ().enabled = false;
            ball = this;
        } else {
            ball = this;
        }
    }

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody> ();
        aud = GetComponent<AudioSource> ();
        serveHit = true;
    }

    // Update is called once per frame
    void FixedUpdate () {
        rb.AddForce(new Vector3 (curve.x * rb.velocity.z, curve.y * rb.velocity.magnitude, 0f) * spinMagnitude);
        curve /= curveDecay;
        if (transform.position.y < -10f) {
            Destroy (gameObject);
        }
	}

    private void OnCollisionEnter (Collision collision) {
        aud.pitch = Random.Range (0.8f, 1.2f);
        aud.PlayOneShot (bounce);
        curve = Vector2.zero;
        if (serveHit && collision.gameObject.GetComponent<PlayerController> () == null) {
            if (GameManager.info.currentServer == 1) {
                if (GameManager.info.leftServe) {
                    if (transform.position.z < 0 || transform.position.z > FAULT_LINE || transform.position.x < 0 || transform.position.x > COURT_WIDTH / 2f) {
                        Fault ();
                    } else {
                        GameManager.info.leftServe = !GameManager.info.leftServe;
                    }
                } else {
                    if (transform.position.z < 0 || transform.position.z > FAULT_LINE || transform.position.x > 0 || transform.position.x < -COURT_WIDTH / 2f) {
                        Fault ();
                    } else {
                        GameManager.info.leftServe = !GameManager.info.leftServe;
                    }
                }
            } else {
                if (GameManager.info.leftServe) {
                    if (transform.position.z > 0 || transform.position.z < -FAULT_LINE || transform.position.x > 0 || transform.position.x < -COURT_WIDTH / 2f) {
                        Fault ();
                    } else {
                        GameManager.info.leftServe = !GameManager.info.leftServe;
                    }
                } else {
                    if (transform.position.z > 0 || transform.position.z < -FAULT_LINE || transform.position.x < 0 || transform.position.x > COURT_WIDTH / 2f) {
                        Fault ();
                    } else {
                        GameManager.info.leftServe = !GameManager.info.leftServe;
                    }
                }
            }
            bounces += 1;
            serveHit = false;
        } else if (collision.gameObject.name == "Stadium") {
            
            if (bounces == 0) {
                if (recent == 1) {
                    GameManager.info.Score (2);
                } else {
                    GameManager.info.Score (1);
                }
                GameManager.info.Alert ("Out");
                ball.GetComponent<SphereCollider> ().enabled = false;
                ball = null;

            }
            bounces += 1;

        } else if (collision.gameObject.GetComponent<PlayerController> () != null) {
            if (collision.gameObject.tag == "Player1") {
                GameManager.info.Score (2);
            } else {
                GameManager.info.Score (1);
            }
            GameManager.info.Alert ("Body Shot");
            ball.GetComponent<SphereCollider> ().enabled = false;
            ball = null;

        } else if (collision.gameObject.name == "Court") {
            
            bounces += 1;
            if (recent == 1 && transform.position.z < 0) {
                GameManager.info.Score (2);
                GameManager.info.Alert ("Short");
                ball.GetComponent<SphereCollider> ().enabled = false;
                ball = null;

            }
            if (recent == 2 && transform.position.z > 0) {
                GameManager.info.Score (1);
                GameManager.info.Alert ("Short");
                ball.GetComponent<SphereCollider> ().enabled = false;
                ball = null;

            }
        }
        if (bounces >= 2) {
            GameManager.info.Score (recent);
            GameManager.info.Alert ("Double Bounce");
            ball.GetComponent<SphereCollider> ().enabled = false;
            ball = null;

        }
    }
    void Fault () {
        ball.GetComponent<SphereCollider> ().enabled = false;
         GameManager.info.StartCoroutine (GameManager.info.WaitPeriod ());
        if (GameManager.info.firstserve) {
            GameManager.info.firstserve = false;
            GameManager.info.Alert ("Fault");
        } else {
            if (GameManager.info.currentServer == 1) {
                GameManager.info.Score (2);
            } else if (GameManager.info.currentServer == 2) {
                GameManager.info.Score (1);
            }
            GameManager.info.Alert ("Double Fault");
            GameManager.info.leftServe = !GameManager.info.leftServe;
        }
        ball.GetComponent<SphereCollider> ().enabled = false;
        ball = null;
    }
}
