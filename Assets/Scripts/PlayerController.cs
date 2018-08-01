using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

    //Constants
    public const float COURT_DEPTH = 23.77f;
    public const float COURT_WIDTH = 8.23f;

    //player input
    public string horizontal;
    public string vertical;
    public string hit;
    public string lob;
    public string curve;
    public string cancel;

    //input management
    Vector2 movement;
    Vector2 aim;
    bool hitBuffer;
    bool _hit;
    bool lobBuffer;
    bool _lob;
    bool curveBuffer;
    bool _curve;
    bool cancelBuffer;
    bool _cancel;

    //Player State
    public PlayerStates state;

    //Movement Multipliers
    public float dexterity; //Acceleration when speed is below the dash threshold
    public float dashThreshhold;
    public float agility; //acceleration when speed is above the dash threshold
    public float speed; //max dash speed
    public float stopForce; //slowdown effect to be applied when not moving or turning around
    public float animScale;

    //Ball Control
    public float power; //the speed the ball is returned with
    public float upForce;
    public float control; //the force to be applied left and right multiplied by the input
    public float spin; //the amount of force to be applied during a "spin"
    public float aimSpeed;

    //References
    Rigidbody rb;
    Animator anim;
    public GameObject ball;
    public AudioSource aud;
    public GameObject target;
    public GameObject Arrow;
    //SFX references
    public AudioClip footstep;
    public AudioClip squeak;
    public AudioClip ballHit;


    // Use this for initialization
    void Start () {
        //Setup references
        rb = GetComponent<Rigidbody> ();
        anim = GetComponentInChildren<Animator> ();
        aud = GetComponent<AudioSource> ();

        //Prepare Stats
        state = PlayerStates.Moving;
        if (gameObject.tag == "Player1") {
            state = PlayerStates.Serving;
            GameManager.info.firstserve = true;
        }
        
    }

    // Update is called once per frame
    void FixedUpdate () {
        if (state != PlayerStates.Waiting) {
            GetInputs ();
        }
        if (state == PlayerStates.Serving) {
            Serve ();
        } else {
            Movement ();
        }
        AimAssistance ();
        Animator ();
        ShotCharge ();
    }

    //Input Retrieval
    void GetInputs () {
        movement = new Vector2 (Input.GetAxis (horizontal), Input.GetAxis (vertical));
        if (state == PlayerStates.Hitting || state == PlayerStates.Curving || state == PlayerStates.Lobbing) {
            aim += movement * aimSpeed * Time.deltaTime;
            if (aim.x > 1) {
                aim.x = 1;
            }
            if (aim.x < -1) {
                aim.x = -1;
            }
            if (aim.y > 1) {
                aim.y = 1;
            }
            if (aim.y < -1) {
                aim.y = -1;
            }
        } else {
            aim = Vector2.zero;
        }
        if (Input.GetAxis (hit) > 0 && hitBuffer == false) {
            _hit = true;
        } else {
            _hit = false;
        }
        if (Input.GetAxis (lob) > 0 && lobBuffer == false) {
            _lob = true;
        } else {
            _lob = false;
        }
        if (Input.GetAxis (curve) > 0 && curveBuffer == false) {
            _curve = true;
        } else {
            _curve = false;
        }
        if (Input.GetAxis (cancel) > 0 && cancelBuffer == false) {
            _cancel = true;
        } else {
            _cancel = false;
        }
        if (Input.GetAxis (hit) > 0) {
            hitBuffer = true;
        } else {
            hitBuffer = false;
        }
        if (Input.GetAxis (lob) > 0) {
            lobBuffer = true;
        } else {
            lobBuffer = false;
        }
        if (Input.GetAxis (curve) > 0) {
            curveBuffer = true;
        } else {
            curveBuffer = false;
        }
        if (Input.GetAxis (cancel) > 0) {
            cancelBuffer = true;
        } else {
            cancelBuffer = false;
        }
        if (Input.GetAxis ("Reset") > 0) {
            SceneManager.LoadScene ("characterSelector");
        }
    }

    void Movement ()  {
        if (state == PlayerStates.Moving) {
            //speeding up
            if (rb.velocity.magnitude < dashThreshhold) {
                rb.AddRelativeForce (new Vector3 (movement.x, 0f, movement.y) * dexterity);
            } else if (rb.velocity.magnitude < speed) {
                rb.AddRelativeForce (new Vector3 (movement.x, 0f, movement.y) * agility);
            }
            //slowing down
            anim.SetBool ("Sliding", false);
            if (rb.velocity.magnitude < dashThreshhold && movement.magnitude == 0f) {
                rb.velocity = Vector3.zero;
            } else if (movement.magnitude == 0) {
                rb.AddForce (new Vector3 (rb.velocity.x, 0f, rb.velocity.z).normalized * -stopForce);
                aud.pitch = Random.Range (0.8f, 1.2f);
                aud.PlayOneShot (squeak);
                anim.SetBool ("Sliding", true);
            }
        } else {
            //slowing down
            anim.SetBool ("Sliding", false);
            if (rb.velocity.magnitude < dashThreshhold) {
                rb.velocity = Vector3.zero;
                
            } else {
                rb.AddForce (new Vector3 (rb.velocity.x, 0f, rb.velocity.z).normalized * -stopForce);
                aud.pitch = Random.Range (0.8f, 1.2f);
                aud.PlayOneShot (squeak);
                anim.SetBool ("Sliding", true);
            }
        }
        

    }

    void AimAssistance () {
        if (state == PlayerStates.Hitting) {
            target.transform.localPosition = CalculateArc (new Vector3 (aim.x * control, upForce, power + aim.y * control));
            Arrow.transform.LookAt (new Vector3 (target.transform.position.x, upForce, target.transform.position.z));
        } else if (state == PlayerStates.Lobbing) {
            target.transform.localPosition = CalculateArc (new Vector3 (aim.x * control, upForce * 2f + aim.y * control / 2f, power / 1.5f));
            Arrow.transform.LookAt (new Vector3 (target.transform.position.x, 0f, target.transform.position.z));
            Arrow.transform.localEulerAngles = new Vector3 (45f + (upForce * 2 + aim.y) * -10, Arrow.transform.localEulerAngles.y, Arrow.transform.localEulerAngles.z);
        } else if (state == PlayerStates.Curving){
            Arrow.transform.localEulerAngles = new Vector3 (aim.y * -10f - 10f, aim.x * spin * 10f, 0f);
        }
    }
    private void Animator () {
        if (rb.velocity.magnitude > 0) {
            anim.SetBool ("Moving", true);
            anim.speed = animScale * rb.velocity.magnitude;
        } else {
            anim.SetBool ("Moving", false);
            anim.speed = 1;
        }

        anim.SetFloat ("Vertical", transform.InverseTransformDirection (rb.velocity).z / speed);
        anim.SetFloat ("Horizontal", transform.InverseTransformDirection (rb.velocity).x / speed);
    }

    void ShotCharge () {
        if (_hit) {
            state = PlayerStates.Hitting;
        }
        if (_lob) {
            state = PlayerStates.Lobbing;
        }
        if (_curve) {
            state = PlayerStates.Curving;
            Arrow.GetComponentInChildren<SpriteRenderer> ().enabled = true;
            target.GetComponent<SpriteRenderer> ().enabled = false;

        }
        if (_hit || _lob) {
            target.GetComponent<SpriteRenderer> ().enabled = true;
            Arrow.GetComponentInChildren<SpriteRenderer> ().enabled = true;
        }
        if (_cancel) {
            state = PlayerStates.Moving;
            target.GetComponent<SpriteRenderer> ().enabled = false;
            Arrow.GetComponentInChildren<SpriteRenderer> ().enabled = false;
        }
    }

    Vector3 CalculateArc (Vector3 velocity) {
        //t = (−velocity + √(velocity ^ 2 + 2 * gravity * height)) / gravity
        float flightTime;
        if (BallController.ball == null) {
            flightTime = (velocity.y + Mathf.Sqrt (Mathf.Pow (velocity.y, 2f) + 2f * 9.81f * 2f)) / 9.81f;
        } else {
            flightTime = (velocity.y + Mathf.Sqrt (Mathf.Pow (velocity.y, 2f) + 2f * 9.81f * BallController.ball.transform.position.y)) / 9.81f;
        }
        Vector3 output = new Vector3 (velocity.x * flightTime, -0.9f, velocity.z * flightTime);
        return output;
    }

    private void Serve () {
        if (gameObject.tag == "Player1") {
            transform.position = new Vector3 (transform.position.x + movement.x * speed * Time.deltaTime, 1f, -COURT_DEPTH / 2f);
            if (GameManager.info.leftServe) {
                if (transform.position.x < -COURT_WIDTH / 2f) {
                    transform.position = new Vector3 (-COURT_WIDTH / 2f, 1f, -COURT_DEPTH / 2f);
                }
                if (transform.position.x > 0f) {
                    transform.position = new Vector3 (0f, 1f, -COURT_DEPTH / 2f);
                }
            } else {
                if (transform.position.x > COURT_WIDTH / 2f) {
                    transform.position = new Vector3 (COURT_WIDTH / 2f, 1f, -COURT_DEPTH / 2f);
                }
                if (transform.position.x < 0f) {
                    transform.position = new Vector3 (0f / 2f, 1f, -COURT_DEPTH / 2f);
                }
            }
        } else {
            transform.position = new Vector3 (transform.position.x + movement.x * -speed * Time.deltaTime, 1f, COURT_DEPTH / 2f);
            if (GameManager.info.leftServe) {
                if (transform.position.x > COURT_WIDTH / 2f) {
                    transform.position = new Vector3 (COURT_WIDTH / 2f, 1f, COURT_DEPTH / 2f);
                }
                if (transform.position.x < 0f) {
                    transform.position = new Vector3 (0f, 1f, COURT_DEPTH / 2f);
                }
            } else {
                if (transform.position.x < -COURT_WIDTH / 2f) {
                    transform.position = new Vector3 (-COURT_WIDTH / 2f, 1f, COURT_DEPTH / 2f);
                }
                if (transform.position.x > 0f) {
                    transform.position = new Vector3 (0f / 2f, 1f, COURT_DEPTH / 2f);
                }
            }
        }
        if (_hit || _lob || _curve) {
            state = PlayerStates.Moving;
            GameObject current = Instantiate (ball, transform.TransformPoint (new Vector3 (0f, 0f, 1f)), Quaternion.identity);
            current.GetComponent<Rigidbody> ().velocity = new Vector3 (0f, 10f, 0f);
            GetInputs ();
            ShotCharge ();
            
        }
    }

    private void OnTriggerEnter (Collider other) {
        if (other.gameObject == BallController.ball.gameObject) {
            //play the right animation
            if (state != PlayerStates.Moving) {
                if (anim.GetBool ("Fore") == false || anim.GetBool ("Back") == false) {
                    if (transform.InverseTransformPoint (other.gameObject.transform.position).x < 0f) {
                        anim.SetTrigger ("Back");
                    } else {
                        anim.SetTrigger ("Fore");
                    }
                }
            }
            
            
        }
    }
    public void HitBall () {
        //change the direction of the tennis ball
        //potentially sync this to the animation
        if (state == PlayerStates.Hitting) {
            BallController.ball.rb.velocity = transform.InverseTransformDirection (new Vector3 (aim.x * control, upForce, power + aim.y * control));
        }
        if (state == PlayerStates.Lobbing) {
            BallController.ball.rb.velocity = transform.InverseTransformDirection (new Vector3 (aim.x * control, upForce * 2f + aim.y * control / 2f, power / 1.5f));
        }
        if (state == PlayerStates.Curving) {
            BallController.ball.rb.velocity = transform.InverseTransformDirection (new Vector3 (0f, upForce + movement.y, power));
            BallController.ball.curve = aim * spin;
            if (BallController.ball.curve.y > 0) {
                BallController.ball.curve.y /= 4f;
            }
        }
        state = PlayerStates.Moving;
        target.GetComponent<SpriteRenderer> ().enabled = false;
        Arrow.GetComponentInChildren<SpriteRenderer> ().enabled = false;
        if (BallController.ball.serveHit) {
            if (gameObject.tag == "Player1") {
                if (BallController.ball.recent == 2) {
                    GameManager.info.Score (1);
                    GameManager.info.Alert ("Recieving Fault");
                    BallController.ball.GetComponent<SphereCollider> ().enabled = false;
                    BallController.ball = null;
                }
            } else {
                if (BallController.ball.recent == 1) {
                    GameManager.info.Score (2);
                    GameManager.info.Alert ("Recieving Fault");
                    BallController.ball.GetComponent<SphereCollider> ().enabled = false;
                    BallController.ball = null;
                }
            }
        }
        //ensure the ball knows who hit it
        if (gameObject.tag == "Player1") {
            BallController.ball.recent = 1;
            BallController.ball.bounces = 0;
        } else {
            BallController.ball.recent = 2;
            BallController.ball.bounces = 0;
        }
        
    }
}

public enum PlayerStates {
    Moving,
    Hitting,
    Lobbing,
    Curving,
    Serving,
    Waiting
}