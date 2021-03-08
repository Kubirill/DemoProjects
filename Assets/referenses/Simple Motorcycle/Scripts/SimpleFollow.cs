using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFollow : MonoBehaviour {

    public Transform Target;

    public float XOffset;
    public float YOffset;
    public float ZOffset;

    void Update() {
        transform.position = new Vector3(Target.position.x + XOffset, Target.position.y + YOffset, Target.position.z + ZOffset);
    }
}
