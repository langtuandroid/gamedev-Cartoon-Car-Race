using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace negleft.AGS{
    [RequireComponent(typeof(AgentRaceManager))]
    public class AgentHUDInfo : MonoBehaviour {
        /// <summary>
        /// Id of the agent whose HUD it is
        /// </summary>
        public int agentID = 0;
        AgentRaceManager myRaceManager;
        /// <summary>
        /// Display the lap
        /// </summary>
        public Text lapDisplay;
        /// <summary>
        /// Display the position
        /// </summary>
        public Text positionDisplay;
        /// <summary>
        /// Hold speedOmeter Information
        /// </summary>
        [System.Serializable]
        public struct speedDisplay{
            public Image needle;
            public Text digitalDisplay;
            public float minAngle;
            public float maxAngle;
            [HideInInspector]
            public AICarDriver myDriver;
        }
        /// <summary>
        ///Object  of speed display 
        /// </summary>
        public speedDisplay speedOMeter;

        // Use this for initialization
        void Start () {
            myRaceManager = transform.GetComponent<AgentRaceManager>();
            
        }
        /// <summary>
        /// Update the hud
        /// </summary>
        /// <returns></returns>
        IEnumerator UpdateHUD() {
            float currClampedVelocity = 0.0f;
            float targetAngle = 0.0f;
            float lastTime = Time.time;
            float deltaTime = 0.0f;
            while (myRaceManager) {
                if (lapDisplay) {
                    lapDisplay.text = "LAP - " + myRaceManager.GetTheLapNumber(agentID).ToString() +"/"+myRaceManager.HowManyLaps().ToString();
                }

                if (positionDisplay)
                {
                    positionDisplay.text = "POSITION - " + myRaceManager.GetPositionInRaceHierarchy(agentID).ToString() + "/" + myRaceManager.HowManyRacing().ToString();
                }

                if (speedOMeter.myDriver) {
                    currClampedVelocity = speedOMeter.myDriver.GetClampedVelocity();
                    targetAngle = Mathf.Lerp(speedOMeter.minAngle, speedOMeter.maxAngle, currClampedVelocity);
                    if (speedOMeter.needle) {
                        speedOMeter.needle.rectTransform.localEulerAngles = new Vector3(0.0f, 0.0f, Mathf.LerpAngle(speedOMeter.needle.rectTransform.localEulerAngles.z , targetAngle,deltaTime*5.0f));
                    }
                    if (speedOMeter.digitalDisplay) {
                        speedOMeter.digitalDisplay.text = (currClampedVelocity * speedOMeter.myDriver.maxSpeed).ToString() + " KM/h";
                    }
                }
                deltaTime = Time.time - lastTime;
                lastTime = Time.time;
                yield return null;
            }
            yield return null;
        }
        /// <summary>
        /// Disable the hud
        /// </summary>
        public void DisableHUD() {
            if (positionDisplay)
                positionDisplay.enabled = false;
            if (lapDisplay)
                lapDisplay.enabled = false;

            if (speedOMeter.myDriver) {
                if (speedOMeter.digitalDisplay)
                    speedOMeter.digitalDisplay.enabled = false;
            }
            StopCoroutine(UpdateHUD());
        }
        /// <summary>
        /// Enable the hud
        /// </summary>
        public void EnableHUD() {
            
            if (positionDisplay)
                positionDisplay.enabled = true;
            if (lapDisplay)
                lapDisplay.enabled = true;

            GetDriver();

            StartCoroutine(UpdateHUD());
        }

        public void GetDriver() {
            if (!myRaceManager )
                return;

            speedOMeter.myDriver = myRaceManager.GetTheDriver(agentID);
        }
    }
}
