using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Motorcycle
{
    [RequireComponent(typeof (MotorcycleController))]
    public class MotorcycleUserControl : MonoBehaviour
    {
        private MotorcycleController m_Motorcycle; // the car controller we want to use


        private void Awake()
        {
            // get the car controller
            m_Motorcycle = GetComponent<MotorcycleController>();
        }


        private void FixedUpdate()
        {
            // pass the input to the car!
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInputManager.GetAxis("Vertical");
#if !MOBILE_INPUT
            float handbrake = CrossPlatformInputManager.GetAxis("Jump");
            m_Motorcycle.Move(h, v, v, handbrake);
#else
            m_Motorcycle.Move(h, v, v, 0f);
#endif
        }
    }
}
