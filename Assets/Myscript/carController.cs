using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class carController : MonoBehaviour
{

    public WheelCollider[] WColForward; //колайдеры передних колёс
    public WheelCollider[] WColBack;  //колайдеры задних колёс
    public GameObject wheeljump; //для прыжка колеса
    public Transform[] wheelsF; //трансформы передних колёс
    public Transform[] wheelsB; //трансформы задних колёс

    public float wheelOffset = 0.1f; //дистанция от колеса до корпуса
    public float wheelRadius = 0.13f; //радиус колес
    public int jumpStreang = 10; //сила прыжка

    public float maxSteer = 30; //Угол поворота колеса
    public float maxAccel = 25; //Ускорение
    public float maxBrake = 50; //Торможение
    public Transform COM; // центр масс
    public float angleFall = 30; // угол наклона мотоцикла
   
    public float longClick = 0.0f; //время удержания кнопки
    private bool turningMotorcycle=false;
    public float rotationSpeed = 5f;
    public Transform playerCamera;

    public class WheelData //класс хранения информации о колесе
    { 
        public Transform wheelTransform; // трансформ колеса
        public WheelCollider col; //колайдер колеса
        public Vector3 wheelStartPos; // начальная позиция колеса
        public float rotation = 0.0f;  // угол колеа
    }

    protected WheelData[] wheels; // массив колёс
    
    void Start()
    {
        gameObject.GetComponent<Rigidbody>().centerOfMass = COM.localPosition; // изменить центр масс обайка

        wheels = new WheelData[WColForward.Length + WColBack.Length]; // создать ячейки массива колёс

        for (int i = 0; i < WColForward.Length; i++)
        { 
            wheels[i] = SetupWheels(wheelsF[i], WColForward[i]); // запоолнить массив передними колёсами
        }

        for (int i = 0; i < WColBack.Length; i++)
        { //9
            wheels[i + WColForward.Length] = SetupWheels(wheelsB[i], WColBack[i]); // запоолнить массив задними колёсами
        }
    }

    private WheelData SetupWheels(Transform wheel, WheelCollider col)
    { 
        WheelData result = new WheelData();// результат для заполнения массива
        result.wheelTransform = wheel; //заполнить трансформ
        result.col = col; //заполнить колайдер
        result.wheelStartPos = wheel.transform.localPosition; //заполнить стартовую позицию
        return result; //вернуть результат

    }
    
    void FixedUpdate()
    {
        float accel = 0; //усорение от нажатия
        float steer = 0; // угол от нажатия
        //float centerOfMass = 0; // центр масс от нажатия

        accel = Input.GetAxis("Vertical");  //получитья нажатие для разгона
        //accel = accel + Mathf.Abs(accel) / 2;
        //centerOfMass = Input.GetAxis("Vertical");
        steer = Input.GetAxis("Horizontal");     //получить нажатие для разворота	
                                                 // if (centerOfMass!=0) UpdateCOM(centerOfMass);
        if (!turningMotorcycle)
        {
            CarMove(accel, steer); //Двигать байк

            if (Input.GetKey(KeyCode.J)) JumpCar();// прыгать байку
        }                                     //gameObject.transform.localRotation = Quaternion.Euler(gameObject.transform.localEulerAngles.x, gameObject.transform.localEulerAngles.y, steer * angleFall);
        UpdateWheels();//обновить состояние колёс
        if ((Input.GetKey(KeyCode.Z))&& (!turningMotorcycle))
        {
            turningMotorcycle = true;
            StartCoroutine( turnMotorcicle((gameObject.transform.rotation.y+360)%360-180));
        }
    }
    private void JumpCar()
    {
        if (WColBack[0].isGrounded && WColForward[0].isGrounded)// если оба колеса на земле
        {
            gameObject.GetComponent<Rigidbody>().AddForce(0f, jumpStreang*150, 0f);// то совершить прыжок к телу
            
        }
    }

    private void UpdateCOM(float centerOfMass)//перемещение центра масс. Пока не используется
    {
        if (!WColBack[0].isGrounded && !WColForward[0].isGrounded)
        {
            COM.localPosition = new Vector3(0, -0.584f, centerOfMass * 2.8f);
            gameObject.GetComponent<Rigidbody>().centerOfMass = COM.localPosition;
        }
        else
        {
            COM.localPosition = new Vector3(0, -0.584f, 0);
            gameObject.GetComponent<Rigidbody>().centerOfMass = COM.localPosition;
        }
    }

    private void UpdateWheels()
    {
        float delta = Time.fixedDeltaTime; //установить одну условную еденицу времени 


        foreach (WheelData w in wheels)// для каждого колеса
        { 
            WheelHit hit; // точка соприкосновения с землёй

            Vector3 lp = w.wheelTransform.localPosition; // запомнить позицию колеса
            if (w.col.GetGroundHit(out hit)) // если есть косание
            { 
                lp.y -= Vector3.Dot(w.wheelTransform.position - hit.point, transform.up) - wheelRadius; //берётся проекция вектора (от позиции колеса до соприкосновения) относительно оси Y и прибавляется радиус
            }
            else
            { 

                lp.y = w.wheelStartPos.y - wheelOffset; // или устанавливаем на максимальное расстояние от байка
            }
            w.wheelTransform.localPosition = lp; //установить полученную позицию
            w.rotation = Mathf.Repeat(w.rotation + delta * w.col.rpm * 360.0f / 60.0f, 360.0f);// находим следующий угол поворота
            w.wheelTransform.localRotation = Quaternion.Euler(w.rotation, w.col.steerAngle, 90.0f); //устанавливает углы
        }

    }

    private void CarMove(float accel, float steer)
    {

        if ((steer == 0)&& (longClick !=0))//если мы не поварачиваем
        {
            longClick =longClick -longClick/Mathf.Abs(longClick)*4.1f*Time.fixedDeltaTime;//то приближаем число к 0
            if (Mathf.Abs(longClick) - 0.1f < 0) longClick = 0;// если число достаточно близко к 0 то присваиваем 0
        }
        else
        {
            if ((steer != 0) && (longClick*steer<2)) longClick = longClick + 4.1f*steer * Time.fixedDeltaTime;//постепеноо изменять длину клика
        }
        foreach (WheelCollider col in WColForward)
        {
            gameObject.transform.localRotation = Quaternion.Euler(gameObject.transform.localEulerAngles.x, gameObject.transform.localEulerAngles.y, longClick * angleFall);// наклонять байк в зависимости от длины нажатия
            //5555555555555555555кепппррррррррррпии    playerCamera.localRotation = Quaternion.Euler(playerCamera.localEulerAngles.x, playerCamera.localEulerAngles.y, gameObject.transform.localEulerAngles.z);// наклонять байк в зависимости от длины нажатия
            col.steerAngle = steer * maxSteer;// изменить угол поворота
            
            if (longClick>1)
            {
                foreach (WheelCollider cold in WColBack)
                {
                   // cold.sidewaysFriction.stiffness = 1;
                   
                }
            }
        }

        if (accel == 0) //если не газует то тормозить
        {
            
            foreach (WheelCollider col in WColBack)
            {

                if (steer != 0)//если поворот на месте
                {
                    col.steerAngle = -steer * maxSteer*3; //то поворот заднего колеса для более резкого разворота
                    col.brakeTorque = 0f; //не тормозить
                    col.motorTorque = 200;// и небольшой разгон
                }
                else col.brakeTorque = maxBrake;// иначе тормоз
            }
        }
        else//иначе
        {
            
            foreach (WheelCollider col in WColBack)
            {
                if (Mathf.Abs(COM.localPosition.z) < 0.5)
                {
                    if ((col.rpm) < 20) col.steerAngle = -steer * maxSteer * 3;//если скорость низкая то больший разворот
                    else col.steerAngle = 0;
                    col.brakeTorque = 0f;//не тормозить
                    if (((col.rpm <500 - Mathf.Abs(steer) * 100) && (accel>0))|| ((col.rpm > -150 + Mathf.Abs(steer) * 50) && (accel < 0))) col.motorTorque = accel * maxAccel;//если не достигнута максимальная скорость то разгоняться
                    else col.motorTorque = 0;
                }
            }
            foreach (WheelCollider col in WColForward)
            {
              /*  if ((col.rpm > 200) && (accel < 0))
                {
                    col.brakeTorque = maxBrake * 20f;
                    COM.localPosition = new Vector3(0, -0.584f, 9.8f);
                    gameObject.GetComponent<Rigidbody>().centerOfMass = COM.localPosition;
                }
                else
                {
                    COM.localPosition = new Vector3(0, -0.584f, 0.0f);
                    gameObject.GetComponent<Rigidbody>().centerOfMass = COM.localPosition;
                    maxBrake = 0f;
                }*/
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {

    }

    private IEnumerator turnMotorcicle(float targetAngle)
    {
        Debug.Log("vjvjvjvj");
        
        gameObject.GetComponent<Rigidbody>().isKinematic = true;

        //  COM.localPosition = new Vector3(0, -0.584f, 9.8f);
        // gameObject.GetComponent<Rigidbody>().centerOfMass = COM.localPosition;


        
        
        
        yield return new WaitForSeconds(Time.deltaTime);
          for (float i=0;i<180;i+=rotationSpeed)
          {
              gameObject.transform.Rotate(0, rotationSpeed, 0,Space.World );
              if (i<50) gameObject.transform.Rotate(5, 0, 0, Space.Self);
              if (i>130) gameObject.transform.Rotate(-5, 0, 0, Space.Self);
              yield return new WaitForSeconds(0.03f);
          }
        
        //yield return new WaitForSeconds(Time.deltaTime);
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
        foreach (WheelCollider wcol in WColBack)wcol.steerAngle = 10000;
        foreach (WheelCollider wcol in WColForward) wcol.steerAngle = 10000;
        //gameObject.GetComponent<Rigidbody>().AddForce(2000f * Mathf.Sin(gameObject.transform.localRotation.y), 0f, 2000f*Mathf.Cos(gameObject.transform.localRotation.y),ForceMode.Acceleration);
        turningMotorcycle = false;
    }
}
