using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace negleft.AGS{
    public class AgentPolice : MonoBehaviour {

        /// <summary>
        /// How close should the speeder be for policing
        /// </summary>
        public bool startPatroling;
        /// <summary>
        /// How close should the speeder be for policing
        /// </summary>
        public float getTriggeredDistace = 30.0f;
        /// <summary>
        /// How far the speeder should be to be considered evaded
        /// </summary>
        public float evadedDistance = 100.0f;
        /// <summary>
        /// Hindrence Layer
        /// </summary>
        public LayerMask hindrences;
        AgentController[] allTheAgents;
        bool gotTheAgents = false;
        bool gotFirstSpeeder = false;
        bool canStart = false;
        Transform currentSpeeder;
        AgentController myAgentController;
        AICarDriver myAIDriver;
        float maxSpeed = 0.0f;
        /// <summary>
        /// Speed when Patroling
        /// </summary>
        public float patrolSpeed = 70.0f;
        Transform handelForPath;
        /// <summary>
        /// SFX for sirens
        /// </summary>
        public AudioSource audioPlayer;

        // Use this for initialization
        void Start() {

            if (transform.GetComponent<AgentController>())
                myAgentController = transform.GetComponent<AgentController>();

            if (transform.GetComponent<AICarDriver>())
            {
                myAIDriver = transform.GetComponent<AICarDriver>();
                maxSpeed = myAIDriver.maxSpeed;
                myAIDriver.maxSpeed = patrolSpeed;
            }

            StartCoroutine(GetTheAgents());

        }


        IEnumerator GetTheAgents(){
            AgentRaceManager myRaceManager = null;

            if (GameObject.FindObjectOfType<AgentRaceManager>()) {
                myRaceManager = GameObject.FindObjectOfType<AgentRaceManager>();
            }

            while (myRaceManager) {
                if (myRaceManager.IsRaceInitiated())
                    break;
                yield return null;
            }
            canStart = true;
            yield return null;
        }



        // Update is called once per frame
        void Update () {
            if (!gotTheAgents) {
                if (canStart) {
                    allTheAgents = GameObject.FindObjectsOfType<AgentController>();
                    gotTheAgents = true;
                }
                return;
            }
                

            if (!gotFirstSpeeder && myAgentController && !handelForPath)
            {
                if (myAgentController.target) {
                    handelForPath = myAgentController.target;
                    if (!startPatroling)
                        myAgentController.target = null;
                }
                
            }
            if (!currentSpeeder)
                LookForSpeeder();
            else
                HasTheSpeederEvaded();
        }
        //Find the nearest speeder
        void LookForSpeeder() {

            if (currentSpeeder)
                return;

            float mag = Mathf.Infinity;
            AgentController newSpeeder = null;
            Vector3 dirTemp;

            for (int i = 0; i < allTheAgents.Length; i++) {
                if (!allTheAgents[i].IsAPolice())
                {
                    dirTemp = (transform.position - allTheAgents[i].transform.position);
                    if (dirTemp.magnitude <= getTriggeredDistace)
                    {
                        if (mag > dirTemp.magnitude)
                        {
                            mag = dirTemp.magnitude;
                            newSpeeder = allTheAgents[i];
                        }
                    }
                }
            }
            if (newSpeeder && !currentSpeeder) {
                currentSpeeder = newSpeeder.transform;
                if (audioPlayer)
                {
                    audioPlayer.Play();
                }
                if (myAgentController)
                {
                    myAgentController.target = currentSpeeder;

                    myAgentController.SetPolicing(true);
                    gotFirstSpeeder = true;
                    if (myAIDriver)
                    {
                        myAIDriver.maxSpeed = maxSpeed;
                    }
                }
            }


        }


    
        //Has the speeder evaded
        void HasTheSpeederEvaded() {

            if (!currentSpeeder)
                return;

            if ((transform.position - currentSpeeder.position).magnitude > evadedDistance)
            {
                if (myAgentController)
                {
                    myAgentController.SetPolicing(false);
                    if (handelForPath)
                    {
                        myAgentController.target = handelForPath;
                    }
                    else {
                        myAgentController.target = null;
                    }
                    
                }
                
                currentSpeeder = null;

            }
            if (currentSpeeder)
            {
                Vector3 dir = (currentSpeeder.position - transform.position);
                bool hit = Physics.Raycast(transform.position, dir.normalized, dir.magnitude, hindrences);
                if (hit)
                {
                    if (myAgentController)
                    {
                        myAgentController.SetPolicing(false);
                        if (handelForPath)
                        {
                            myAgentController.target = handelForPath;
                        }
                        else
                        {
                            myAgentController.target = null;
                        }

                    }

                    currentSpeeder = null;
                }
            }

            if (!currentSpeeder) {

                if (myAIDriver)
                {
                    myAIDriver.maxSpeed = patrolSpeed;
                }

                if (audioPlayer)
                {
                    audioPlayer.Stop();
                }
            }
            

        }

        /// <summary>
        /// are we policing
        /// </summary>
        public bool AreWePolicing() {
            if (currentSpeeder)
                return true;
            else
                return false;
        }
    }

}