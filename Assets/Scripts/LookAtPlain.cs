using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlain : MonoBehaviour {

	void Update () {
        if (BallController.ball != null) {
            transform.LookAt (BallController.ball.transform);
        }
	}
}
