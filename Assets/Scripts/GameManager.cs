using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public static GameManager info;

    public int team1score;
    public int team1games;
    public int team1sets;
    public int team2score;
    public int team2games;
    public int team2sets;

    public int currentServer;
    public bool leftServe;
    public bool firstserve = true;


    public int gamesPerSet;
    public int sets;

    public PlayerController player1;
    public PlayerController player2;

    public Text _team1score;
    public Text _team1games;
    public Text _team1sets;
    public Text _team2score;
    public Text _team2games;
    public Text _team2sets;

    //Notification Banner
    public Animator anim;
    public Text text;

    private void Awake () {
        //singleton
        if (info != null) {
            Destroy (gameObject);
        } else {
            info = this;
        }
        Application.targetFrameRate = 120;
    }
    private void Start () {
        SetupGame ();
    }
    public void SetupGame () {
        player1 = Instantiate (MenuManager.info.prefabs [MenuManager.info.player1Character], new Vector3 (0f, 1.03f, -11f), Quaternion.Euler (0f, 0f, 0f)).GetComponent<PlayerController> ();
        player1.name.Replace ("(Clone)", "");
        player1.tag = "Player1";
        player1.transform.GetChild (1).gameObject.layer = 9;
        player1.transform.GetChild (2).gameObject.layer = 9;
        player1.transform.GetChild (2).GetChild (0).gameObject.layer = 9;
        player1.transform.GetChild (3).gameObject.layer = 9;
        player1.horizontal = "Hori1";
        player1.vertical = "Vert1";
        player1.hit = "Hit1";
        player1.curve = "Curve1";
        player1.lob = "Lob1";
        player1.cancel = "Cancel1";
        player2 = Instantiate (MenuManager.info.prefabs [MenuManager.info.player2Character], new Vector3 (0f, 1.03f, 11f), Quaternion.Euler (0f, 180f, 0f)).GetComponent<PlayerController> ();
        player2.name.Replace ("(Clone)", "");
        player2.tag = "Player2";
        player2.transform.GetChild (1).gameObject.layer = 10;
        player2.transform.GetChild (2).gameObject.layer = 10;
        player2.transform.GetChild (2).GetChild (0).gameObject.layer = 10;
        player2.transform.GetChild (3).gameObject.layer = 10;
        player2.horizontal = "Hori2";
        player2.vertical = "Vert2";
        player2.hit = "Hit2";
        player2.curve = "Curve2";
        player2.lob = "Lob2";
        player2.cancel = "Cancel2";
        //player1.state = PlayerStates.Serving;
        StartCoroutine (WaitPeriod ());
        Alert (player1.gameObject.name + " vs " + player2.gameObject.name);
    }
    void LateUpdate () {
        if (BallController.ball == null && !(player1.state == PlayerStates.Waiting || player2.state == PlayerStates.Waiting || player1.state == PlayerStates.Serving || player2.state == PlayerStates.Serving)) {
            SetServer ();
        }
    }
    public void Score (int team) {
        if (team == 1) {
            if (team1score == 0) {
                team1score = 15;
            } else if (team1score == 15) {
                team1score = 30;
            } else if (team1score == 30) {
                team1score = 40;
            } else if (team1score == 40) {
                if (team2score < 40) {
                    team1games += 1;
                    team1score = 0;
                    team2score = 0;
                } else {
                    if (team2score == 50) {
                        team2score = 40;
                    } else {
                        team1score = 50;
                    }
                }
            } else if (team1score == 50) {
                team1games += 1;
                team1score = 0;
                team2score = 0;
            }
            if (team1games >= gamesPerSet) {
                if (team1games - team2games >= 2) {
                    team1games = 0;
                    team2games = 0;
                    team1sets += 1;
                    if (currentServer == 1) {
                        currentServer = 2;
                    } else {
                        currentServer = 1;
                    }
                }
            }
            if (team1sets == sets) {
                //team 1 wins

            }
        }
        if (team == 2) {
            if (team2score == 0) {
                team2score = 15;
            } else if (team2score == 15) {
                team2score = 30;
            } else if (team2score == 30) {
                team2score = 40;
            } else if (team2score == 40) {
                if (team1score < 40) {
                    team2games += 1;
                    team2score = 0;
                    team1score = 0;
                } else {
                    if (team1score == 50) {
                        team1score = 40;
                    } else {
                        team2score = 50;
                    }
                }
            } else if (team2score == 50) {
                team2games += 1;
                team2score = 0;
                team1score = 0;
            }
            if (team2games >= gamesPerSet) {
                
                if (team2games - team1games >= 2) {
                    team2games = 0;
                    team1games = 0;
                    team2sets += 1;
                    if (currentServer == 1) {
                        currentServer = 2;
                    } else {
                        currentServer = 1;
                    }
                }
            }
            if (team2sets == sets) {
                //team 2 wins

            }
        }
        
        firstserve = true;
        Inject ();
        StartCoroutine (WaitPeriod ());
    }
    public IEnumerator WaitPeriod () {
        player1.state = PlayerStates.Waiting;
        player2.state = PlayerStates.Waiting;
        yield return new WaitForSeconds (2f);
        if (currentServer == 1) {
            player1.state = PlayerStates.Serving;
            player2.state = PlayerStates.Moving;
        } else {
            player1.state = PlayerStates.Moving;
            player2.state = PlayerStates.Serving;
        }
    }
    void SetServer () {
        Debug.Log ("Setting Server");
        if (currentServer == 1) {
            player1.state = PlayerStates.Serving;
            player1.target.GetComponent<SpriteRenderer> ().enabled = false;
            player1.Arrow.GetComponentInChildren<SpriteRenderer> ().enabled = false;
        } else {
            player2.state = PlayerStates.Serving;
            player1.target.GetComponent<SpriteRenderer> ().enabled = false;
            player1.Arrow.GetComponentInChildren<SpriteRenderer> ().enabled = false;
        }
    }
    void Inject () {
        _team1score.text = team1score.ToString();
        _team1games.text = team1games.ToString ();
        _team1sets.text = team1sets.ToString ();
        _team2score.text = team2score.ToString ();
        _team2games.text = team2games.ToString ();
        _team2sets.text = team2sets.ToString ();
    }
    public void Alert (string notification) {
        text.text = notification;
        anim.SetTrigger ("popup");
    }
}
