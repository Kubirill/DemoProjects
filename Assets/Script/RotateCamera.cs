using UnityEngine;
using System.Collections;


public class RotateCamera : MonoBehaviour
{
    
    public Transform head;

    public float sensitivity = 5f; // чувствительность мыши
    public float headMinY = -40f; // ограничение угла для головы
    public float headMaxY = 40f;

   
    private float rotationY;

    void Start()
    {
       
    }

    void FixedUpdate()
    {
    }



    void Update()
    {
        
        // управление головой (камерой)
        float rotationX = head.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivity;
        rotationY += Input.GetAxis("Mouse Y") * sensitivity;
        rotationY = Mathf.Clamp(rotationY, headMinY, headMaxY);
        head.localEulerAngles = new Vector3(-rotationY, rotationX, 0);

        
    }

}