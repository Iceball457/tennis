using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public Vector3 offset;
    public float speed;
    Vector3 targetPosition;


    // Update is called once per frame
    void Update () {
        int averageDivisor = 1;
        targetPosition = Vector3.zero;
        if (gameObject.tag == "Camera1") {
            if (GameManager.info.player1 != null) {
                targetPosition += GameManager.info.player1.gameObject.transform.position;
                averageDivisor += 1;
            }
        } else if (gameObject.tag == "Camera2") {
            if (GameManager.info.player2 != null) {
                targetPosition += GameManager.info.player2.gameObject.transform.position;
                averageDivisor += 1;
            }
        } else
        if (BallController.ball != null) {
            targetPosition += BallController.ball.transform.position;
            averageDivisor += 1;
        }
        targetPosition /= averageDivisor;
        targetPosition += offset;
        Vector3 startPosition = transform.position;
        transform.position = Vector3.Lerp (startPosition, targetPosition, speed);
    }
}
