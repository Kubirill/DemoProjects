using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bike : MonoBehaviour
{
    public class WheelData //класс хранения информации о колесе
    {
        public Transform wheelTransform; // трансформ колеса
        public WheelCollider col; //колайдер колеса
        public Vector3 wheelStartPos; // начальная позиция колеса
        public float rotation = 0.0f;  // угол колеа
    }
    protected WheelData[] wheels = new WheelData[2]; // массив колёс
    public Transform COM; // центр масс
    public WheelCollider WColForward; //колайдеры передних колёс
    public WheelCollider WColBack;  //колайдеры задних колёс
    public Transform wheelsF; //трансформы передних колёс
    public Transform wheelsB; //трансформы задних колёс
    public float wheelOffset = 0.1f; //дистанция от колеса до корпуса
    public float wheelRadius = 0.13f; //радиус колес
    public float maxAccel = 25; //Ускорение
    public float maxBrake = 50; //Торможение
    public float maxRPM = 500;
    public float distanceMass =1;
    public Text speedText;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Rigidbody>().centerOfMass = COM.localPosition; // изменить центр масс обайка
        wheels[0] = SetupWheels(wheelsF, WColForward);
        wheels[1] = SetupWheels(wheelsB, WColBack);
    }

    private WheelData SetupWheels(Transform wheel, WheelCollider col)
    {
        WheelData result = new WheelData();// результат для заполнения массива
        result.wheelTransform = wheel; //заполнить трансформ
        result.col = col; //заполнить колайдер
        result.wheelStartPos = wheel.transform.localPosition; //заполнить стартовую позицию
        return result; //вернуть результат

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float accel = 0; //усорение от нажатия
        float angle = 0;
        accel = Input.GetAxis("Vertical");  //получитья нажатие для разгона
        angle = Input.GetAxis("Horizontal");  //получитья нажатие для наклона
        if (Mathf.Abs(angle)>0.5) UpdateCOM(angle*distanceMass);
        CarMove(accel); //Двигать байк
        UpdateWheels();//обновить состояние колёс
        speedText.text = "speed " + WColBack.rpm + " WColBack.motorTorqu "+ WColBack.motorTorque;
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

    private void CarMove(float accel)
    {
        if (accel < 0) //если не газует то тормозить
        {
            WColBack.brakeTorque = maxBrake;
        }
        else
        {
            if (Mathf.Abs(COM.localPosition.z) < 0.5)
                {
                    WColBack.brakeTorque = 0f;
                    if (((WColBack.rpm < maxRPM) && (accel > 0)) || ((WColBack.rpm > -maxRPM/4) && (accel < 0))) WColBack.motorTorque = (accel * maxAccel)- WColBack.motorTorque;//если не достигнута максимальная скорость то разгоняться
                    else WColBack.motorTorque = 0;
                }
        }
            
    }
    private void UpdateCOM(float centerOfMass)//перемещение центра масс. Пока не используется
    {
        gameObject.GetComponent<Rigidbody>().angularVelocity = new Vector3(-centerOfMass, 0, 0);
        //            gameObject.GetComponent<Rigidbody>().centerOfMass = COM.localPosition;

    }
}

