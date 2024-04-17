using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace negleft.AGS{
    public class AgentRacePolice : MonoBehaviour {

        /// <summary>
        /// How close should the speeder be for policing
        /// </summary>
        [FormerlySerializedAs("startPatroling")] [SerializeField] private bool startPatrolingFlag;
        /// <summary>
        /// How close should the speeder be for policing
        /// </summary>
        [FormerlySerializedAs("getTriggeredDistace")] [SerializeField] private float getTriggeredDistaceValue = 30.0f;
        /// <summary>
        /// How far the speeder should be to be considered evaded
        /// </summary>
        [FormerlySerializedAs("evadedDistance")] [SerializeField] private float evadedDistanceValue = 100.0f;
        /// <summary>
        /// Hindrence Layer
        /// </summary>
        [FormerlySerializedAs("hindrences")] [SerializeField] private LayerMask hindrencesLayer;
        private AgentRaceController[] allTheAgentsControllers;
        private bool gotTheAgentsFlag = false;
        private bool gotFirstSpeederFlag = false;
        private bool canStartFlag = false;
        private Transform currentSpeederTransform;
        private AgentRaceController currAgentController;
        private AICarDriverControl currAIDriver;
        private float maxSpeedValue = 0.0f;
        /// <summary>
        /// Speed when Patroling
        /// </summary>
        [FormerlySerializedAs("patrolSpeed")] [SerializeField] private float patrolSpeedValue = 70.0f;
        private Transform handelForPathTransform;
        /// <summary>
        /// SFX for sirens
        /// </summary>
        [FormerlySerializedAs("audioPlayer")] [SerializeField] private AudioSource audioPlayerSource;

        // Use this for initialization
        private void Start() {

            if (transform.GetComponent<AgentRaceController>())
                currAgentController = transform.GetComponent<AgentRaceController>();

            if (transform.GetComponent<AICarDriverControl>())
            {
                currAIDriver = transform.GetComponent<AICarDriverControl>();
                maxSpeedValue = currAIDriver.maxSpeedValue;
                currAIDriver.maxSpeedValue = patrolSpeedValue;
            }

            StartCoroutine(GetTheAgentsCoroutine());

        }


        private IEnumerator GetTheAgentsCoroutine(){
            AgentRacePathManager myRaceManager = null;

            if (GameObject.FindObjectOfType<AgentRacePathManager>()) {
                myRaceManager = GameObject.FindObjectOfType<AgentRacePathManager>();
            }

            while (myRaceManager) {
                if (myRaceManager.IsTheRaceInitiated())
                    break;
                yield return null;
            }
            canStartFlag = true;
            yield return null;
        }



        // Update is called once per frame
        private void Update () {
            if (!gotTheAgentsFlag) {
                if (canStartFlag) {
                    allTheAgentsControllers = GameObject.FindObjectsOfType<AgentRaceController>();
                    gotTheAgentsFlag = true;
                }
                return;
            }
                

            if (!gotFirstSpeederFlag && currAgentController && !handelForPathTransform)
            {
                if (currAgentController.currTarget) {
                    handelForPathTransform = currAgentController.currTarget;
                    if (!startPatrolingFlag)
                        currAgentController.currTarget = null;
                }
                
            }
            if (!currentSpeederTransform)
                LookForNearestSpeeder();
            else
                HasTheSpeederEvadedR();
        }
        //Find the nearest speeder
        private void LookForNearestSpeeder() {

            if (currentSpeederTransform)
                return;

            float mag = Mathf.Infinity;
            AgentRaceController newSpeeder = null;
            Vector3 dirTemp;

            for (int i = 0; i < allTheAgentsControllers.Length; i++) {
                if (!allTheAgentsControllers[i].IsAPoliceStatus())
                {
                    dirTemp = (transform.position - allTheAgentsControllers[i].transform.position);
                    if (dirTemp.magnitude <= getTriggeredDistaceValue)
                    {
                        if (mag > dirTemp.magnitude)
                        {
                            mag = dirTemp.magnitude;
                            newSpeeder = allTheAgentsControllers[i];
                        }
                    }
                }
            }
            if (newSpeeder && !currentSpeederTransform) {
                currentSpeederTransform = newSpeeder.transform;
                if (audioPlayerSource)
                {
                    audioPlayerSource.Play();
                }
                if (currAgentController)
                {
                    currAgentController.currTarget = currentSpeederTransform;

                    currAgentController.SetPolicingStatus(true);
                    gotFirstSpeederFlag = true;
                    if (currAIDriver)
                    {
                        currAIDriver.maxSpeedValue = maxSpeedValue;
                    }
                }
            }
        }

        //Has the speeder evaded
        private void HasTheSpeederEvadedR() {

            if (!currentSpeederTransform)
                return;

            if ((transform.position - currentSpeederTransform.position).magnitude > evadedDistanceValue)
            {
                if (currAgentController)
                {
                    currAgentController.SetPolicingStatus(false);
                    if (handelForPathTransform)
                    {
                        currAgentController.currTarget = handelForPathTransform;
                    }
                    else {
                        currAgentController.currTarget = null;
                    }
                    
                }
                
                currentSpeederTransform = null;

            }
            if (currentSpeederTransform)
            {
                Vector3 dir = (currentSpeederTransform.position - transform.position);
                bool hit = Physics.Raycast(transform.position, dir.normalized, dir.magnitude, hindrencesLayer);
                if (hit)
                {
                    if (currAgentController)
                    {
                        currAgentController.SetPolicingStatus(false);
                        if (handelForPathTransform)
                        {
                            currAgentController.currTarget = handelForPathTransform;
                        }
                        else
                        {
                            currAgentController.currTarget = null;
                        }

                    }

                    currentSpeederTransform = null;
                }
            }

            if (!currentSpeederTransform) {

                if (currAIDriver)
                {
                    currAIDriver.maxSpeedValue = patrolSpeedValue;
                }

                if (audioPlayerSource)
                {
                    audioPlayerSource.Stop();
                }
            }
            

        }

        /// <summary>
        /// are we policing
        /// </summary>
        public bool AreWePolicingFlag() {
            if (currentSpeederTransform)
                return true;
            else
                return false;
        }
    }

}