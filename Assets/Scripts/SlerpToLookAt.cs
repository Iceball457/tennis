using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlerpToLookAt : MonoBehaviour {


    public static void LookAt (Transform self, Vector3 target, float speed) {
        //values for internal use
        Quaternion _lookRotation;
        Vector3 _direction;

        //find the vector pointing from our position to the target
        _direction = (target - self.transform.position).normalized;

        //create the rotation we need to be in to look at the target
        _lookRotation = Quaternion.LookRotation (_direction);

        //rotate us over time according to speed until we are in the required rotation
        self.transform.rotation = Quaternion.Slerp (self.transform.rotation, _lookRotation, speed);
    }
}
