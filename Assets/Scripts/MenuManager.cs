using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {

    public static MenuManager info;

    public GameObject [] prefabs;

    public MenuStatRanking player1;
    public MenuStatRanking player2;

    public int player1Character;
    public int player2Character;

    private void Awake () {
        if (info != null) {
            Destroy (info);
            info = this;
            DontDestroyOnLoad (gameObject);
        } else {
            info = this;
            DontDestroyOnLoad (gameObject);
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	

	// Update is called once per frame
	void Update () {
        if (player1 != null && player2 != null) {
            if (player1.ready && player2.ready) {
                player1Character = player1.menuHeight;
                player2Character = player2.menuHeight;
                SceneManager.LoadScene ("testScene");
            }
        }
        if (Input.GetAxis ("Back") > 0) {
            SceneManager.LoadScene ("characterSelector");
        }
	}
}
