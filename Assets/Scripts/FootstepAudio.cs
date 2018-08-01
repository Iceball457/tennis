using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepAudio : MonoBehaviour {
    PlayerController pc;
    private void Start () {
        pc = transform.parent.GetComponent<PlayerController> ();
    }
    public void Footstep () {
        pc.aud.pitch = Random.Range (0.8f, 1.2f);
        pc.aud.PlayOneShot (pc.footstep, 0.1f);
    }
    public void BallHit () {
        pc.aud.pitch = Random.Range (0.8f, 1.2f);
        pc.aud.PlayOneShot (pc.ballHit);
        pc.HitBall ();
    }
}
