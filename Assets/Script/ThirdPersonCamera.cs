using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    Vector3 offset;
    public Transform player;
    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - player.position;
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        transform.position = player.position + offset;
    }
}
