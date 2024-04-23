using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace negleft.AGS{
    public class SendMobileUIInput : MonoBehaviour {

        private AICarDriverControl toDriver;
        /// <summary>
        /// Steering wheel for UI based input
        /// </summary>
        [FormerlySerializedAs("steerWheel")] [SerializeField] private SteerWheelMobileUI steerWheelUI;
        /// <summary>
        /// Assign the holder of this type of controls
        /// </summary>
        [FormerlySerializedAs("UI_Tap_Acc")] [SerializeField] private GameObject UI_Tap_Acc_Object;
        /// <summary>
        /// Assign the holder of this type of controls
        /// </summary>
        [FormerlySerializedAs("UI_Steer_BTNS")] [SerializeField] private GameObject UI_Steer_BTNS_Object;
        // next Y value
        private float toYValue = 0.0f;
        private float toXValue = 0.0f;
        //var for lerp method
        private float yCounterValue = 0.0f;
        private float xCounterValue = 0.0f;
        // Use this for initialization
        private void Start () {
            SetMobileUIControls();
        }
        /// <summary>
        /// Setting the UI controls
        /// </summary>
        private void SetMobileUIControls() {
            int currType = PlayerPrefs.GetInt("InputType", 0);

            switch (currType) {
                case 1:
                    if (UI_Tap_Acc_Object)
                        UI_Tap_Acc_Object.SetActive(true);
                    break;
                case 2:
                    if (UI_Steer_BTNS_Object)
                        UI_Steer_BTNS_Object.SetActive(true);
                    break;
            }
        }

        // Update is called once per frame
        private void Update () {
            RefreshControlsData();
        }
        
        public void GiveCarDriver(AICarDriverControl driver) {
            toDriver = driver;
        }
        /// <summary>
        /// Get the new Input values
        /// </summary>
        public void RefreshControlsData() {
            if (!toDriver)
                return;

            switch (toDriver.currControlType) {
                case AICarDriverControl.ControlsTypes.UI_Tap_and_Acc_Control:
                    toDriver.SetInputYValue(LerpYValue());

                    toDriver.SetInputXValue(Mathf.Clamp(Input.acceleration.x * 1.5f, -1.0f, 1.0f));

                    break;

                case AICarDriverControl.ControlsTypes.UI_SteerWheel_BTNS:

                    /*if (steerWheelUI)
                        toDriver.SetInputXValue(steerWheelUI.GetClampedWheelValue());*/
                    toDriver.SetInputXValue(LerpXValue());

                    toDriver.SetInputYValue(LerpYValue());

                    break;
            }
        }
        //Lerping
        private float LerpYValue() {
            if (yCounterValue < 1.0f) {
                yCounterValue += Time.deltaTime*5.0f;
            }

            if (yCounterValue > 1.0f)
            {
                yCounterValue = 1.0f;
            }

            float newY = Mathf.Clamp(Mathf.Lerp(0, toYValue, yCounterValue), -1.0f, 1.0f);
            return newY;
        }
        private float LerpXValue() {
            if (xCounterValue < 1.0f) {
                xCounterValue += Time.deltaTime*5.0f;
            }

            if (xCounterValue > 1.0f)
            {
                xCounterValue = 1.0f;
            }

            float newX = Mathf.Clamp(Mathf.Lerp(0, toXValue, xCounterValue), -1.0f, 1.0f);
            return newX;
        }
        /// <summary>
        /// Set new Y value
        /// </summary>
        /// <param name="newY">pass this as new Y</param>
        public void SetInputYValue(float newY) {
            toYValue = newY;
        }
        public void SetInputXValue(float newX) {
            toXValue = newX;
        }
        
    }
}