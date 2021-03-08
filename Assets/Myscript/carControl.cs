using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carControl : MonoBehaviour
{
   // public GameObject TopWheelRight;
    public GameObject TopWheelLeft;
    //public GameObject BackWheelRight;
    public GameObject BackWheelLeft;
    public float WheelSpeed;
    public float WheelRpmMax;
    public bool BackWheels;
    public bool TopWheels;
    public int WheelRotateAngle = 10;
    public int angleFall;

    public Rigidbody car;
    // Start is called before the first frame update
    void Start()
    {
      //  car.GetComponent(Rigidbody).centerOfMass = new Vector3(0, -car.transform.localScale.y, 0);
         gameObject.GetComponent<Rigidbody>().centerOfMass = new Vector3(0, -gameObject.transform.localScale.y*2, 0);
    }

    // Update is called once per frame
    void Update()
    {
        //BackWheelRight.GetComponent<WheelCollider>().motorTorque = WheelSpeed * (Input.GetAxis("Vertical"));
       
        BackWheelLeft.GetComponent<WheelCollider>().motorTorque = WheelSpeed * (Input.GetAxis("Vertical"));
        
        //TopWheelRight.transform.localRotation= Quaternion.Euler(0, WheelRotateAngle * Input.GetAxis("Horizontal"), 0);
        TopWheelLeft.transform.localRotation = Quaternion.Euler(0, WheelRotateAngle * Input.GetAxis("Horizontal"), 0);

        TopWheelLeft.GetComponent<WheelCollider>().steerAngle= TopWheelLeft.transform.localEulerAngles.y;
        //TopWheelRight.GetComponent<WheelCollider>().steerAngle = TopWheelLeft.transform.localEulerAngles.y;
        gameObject.transform.localRotation = Quaternion.Euler(gameObject.transform.localEulerAngles.x,  gameObject.transform.localEulerAngles.y,Input.GetAxis("Horizontal")*angleFall);
    }

}
