using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightController : MonoBehaviour {

    public float height;

    private void Update () {
        if (BallController.ball == null) {
            transform.position = Vector3.down;
        } else {
            transform.position = new Vector3 (BallController.ball.transform.position.x, height, BallController.ball.transform.position.z);
        }
    }
}