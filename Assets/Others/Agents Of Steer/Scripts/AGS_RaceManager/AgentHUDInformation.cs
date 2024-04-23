using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace negleft.AGS{
    [RequireComponent(typeof(AgentRacePathManager))]
    public class AgentHUDInformation : MonoBehaviour {
        /// <summary>
        /// Id of the agent whose HUD it is
        /// </summary>
        [FormerlySerializedAs("agentID")] public int agentIDHUD = 0;
        private AgentRacePathManager myRaceManagerAgent;
        /// <summary>
        /// Display the lap
        /// </summary>
        [FormerlySerializedAs("lapDisplay")] [SerializeField] private Text lapDisplayText;
        /// <summary>
        /// Display the position
        /// </summary>
        [FormerlySerializedAs("positionDisplay")] [SerializeField] private  Text positionDisplayText;
        /// <summary>
        /// Hold speedOmeter Information
        /// </summary>
        [System.Serializable]
        public struct speedDisplayInfo{
            [FormerlySerializedAs("needle")] public Image needleImage;
            public Image speedImage;
            [FormerlySerializedAs("digitalDisplay")] public Text digitalDisplayText;
            [FormerlySerializedAs("minAngle")] public float minAngleValue;
            [FormerlySerializedAs("maxAngle")] public float maxAngleValue;
            [FormerlySerializedAs("myDriver")] [HideInInspector]
            public AICarDriverControl myDriverAI;
        }
        /// <summary>
        ///Object  of speed display 
        /// </summary>
        [FormerlySerializedAs("speedOMeter")] public speedDisplayInfo speedOMeterInfo;

        // Use this for initialization
        private void Start () {
            myRaceManagerAgent = transform.GetComponent<AgentRacePathManager>();
            
        }
        /// <summary>
        /// Update the hud
        /// </summary>
        /// <returns></returns>
        private IEnumerator UpdateHUDCoroutine() {
            float currClampedVelocity = 0.0f;
            float targetAngle = 0.0f;
            float lastTime = Time.time;
            float deltaTime = 0.0f;
            while (myRaceManagerAgent) {
                if (lapDisplayText) {
                    lapDisplayText.text = /*"LAP - " + */myRaceManagerAgent.GetTheLapNumberValue(agentIDHUD).ToString() +"/"+myRaceManagerAgent.HowManyRaceLaps().ToString();
                }

                if (positionDisplayText)
                {
                    positionDisplayText.text = /*"POSITION - " +*/ myRaceManagerAgent.GetPositionInTheRaceHierarchy(agentIDHUD).ToString() /*+ "/" + myRaceManagerAgent.HowManyRacingAgents().ToString()*/;
                }

                if (speedOMeterInfo.myDriverAI) {
                    currClampedVelocity = speedOMeterInfo.myDriverAI.GetClampedVelocityValue();
                    targetAngle = Mathf.Lerp(speedOMeterInfo.minAngleValue, speedOMeterInfo.maxAngleValue, currClampedVelocity);
                    if (false && speedOMeterInfo.needleImage) {
                        speedOMeterInfo.needleImage.rectTransform.localEulerAngles = new Vector3(0.0f, 0.0f, Mathf.LerpAngle(speedOMeterInfo.needleImage.rectTransform.localEulerAngles.z , targetAngle,deltaTime*5.0f));
                    }
                    if (speedOMeterInfo.digitalDisplayText) {
                        speedOMeterInfo.digitalDisplayText.text = ((int)(currClampedVelocity * speedOMeterInfo.myDriverAI.maxSpeedValue)).ToString() /*+ " KM/h"*/;
                    }
                    speedOMeterInfo.speedImage.fillAmount = Mathf.LerpAngle(speedOMeterInfo.speedImage.fillAmount,
                        currClampedVelocity, deltaTime * 5.0f);
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
        public void DisableInfoHUD() {
            if (positionDisplayText)
                positionDisplayText.transform.parent.gameObject.SetActive(false);
            if (lapDisplayText)
                lapDisplayText.transform.parent.gameObject.SetActive(false);
            if (speedOMeterInfo.digitalDisplayText)
                speedOMeterInfo.digitalDisplayText.transform.parent.gameObject.SetActive(false);

            if (speedOMeterInfo.myDriverAI) {
                if (speedOMeterInfo.digitalDisplayText)
                    speedOMeterInfo.digitalDisplayText.enabled = false;
            }
            StopCoroutine(UpdateHUDCoroutine());
        }
        /// <summary>
        /// Enable the hud
        /// </summary>
        public void EnableInfoHUD() {
            
            if (positionDisplayText)
                positionDisplayText.transform.parent.gameObject.SetActive(true);
            if (lapDisplayText)
                lapDisplayText.transform.parent.gameObject.SetActive(true);
            if (speedOMeterInfo.digitalDisplayText)
                speedOMeterInfo.digitalDisplayText.transform.parent.gameObject.SetActive(true);

            GetCurrentDriver();

            StartCoroutine(UpdateHUDCoroutine());
        }

        public void GetCurrentDriver() {
            if (!myRaceManagerAgent )
                return;

            speedOMeterInfo.myDriverAI = myRaceManagerAgent.GetDriver(agentIDHUD);
        }
    }
}
