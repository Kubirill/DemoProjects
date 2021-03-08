using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotate : MonoBehaviour
{

    public float speed = 1500f;
    public float rotationSpeed = 15f;


    public Rigidbody rb;
    private float movement = 0f;
    private float rotation = 0f;

    void Update()
    {
        movement = -Input.GetAxisRaw("Vertical") * speed;
        rotation = Input.GetAxisRaw("Horizontal");
        rb.AddTorque(speed * movement*transform.up);
    }
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    
}
