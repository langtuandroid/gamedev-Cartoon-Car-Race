using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace negleft.AGS{
    public class SendUIInput : MonoBehaviour {

        AICarDriver to;
        /// <summary>
        /// Steering wheel for UI based input
        /// </summary>
        public SteerWheelUI steerWheel;
        /// <summary>
        /// Assign the holder of this type of controls
        /// </summary>
        public GameObject UI_Tap_Acc;
        /// <summary>
        /// Assign the holder of this type of controls
        /// </summary>
        public GameObject UI_Steer_BTNS;
        // next Y value
        float toY = 0.0f;
        //var for lerp method
        float yCounter = 0.0f;
        // Use this for initialization
        void Start () {
            SetUIControls();
        }
        /// <summary>
        /// Setting the UI controls
        /// </summary>
        void SetUIControls() {
            int currType = PlayerPrefs.GetInt("InputType", 0);

            switch (currType) {
                case 1:
                    if (UI_Tap_Acc)
                        UI_Tap_Acc.SetActive(true);
                    break;
                case 2:
                    if (UI_Steer_BTNS)
                        UI_Steer_BTNS.SetActive(true);
                    break;
            }
        }

        // Update is called once per frame
        void Update () {
            RefreshControls();
        }
        
        public void GiveDriver(AICarDriver driver) {
            to = driver;
        }
        /// <summary>
        /// Get the new Input values
        /// </summary>
        public void RefreshControls() {
            if (!to)
                return;

            switch (to.controlType) {
                case AICarDriver.ControlTypes.UI_Tap_and_Acc_Control:
                    to.SetInputY(LerpY());

                    to.SetInputX(Mathf.Clamp(Input.acceleration.x * 1.5f, -1.0f, 1.0f));

                    break;

                case AICarDriver.ControlTypes.UI_SteerWheel_BTNS:

                    if (steerWheel)
                        to.SetInputX(steerWheel.GetClampedValue());

                    to.SetInputY(LerpY());

                    break;
            }
        }
        //Lerping
        float LerpY() {
            if (yCounter < 1.0f) {
                yCounter += Time.deltaTime*5.0f;
            }

            if (yCounter > 1.0f)
            {
                yCounter = 1.0f;
            }

            float newY = Mathf.Clamp(Mathf.Lerp(0, toY, yCounter), -1.0f, 1.0f);
            return newY;
        }
        /// <summary>
        /// Set new Y value
        /// </summary>
        /// <param name="newY">pass this as new Y</param>
        public void SetInputY(float newY) {
            toY = newY;
        }
        
    }
}