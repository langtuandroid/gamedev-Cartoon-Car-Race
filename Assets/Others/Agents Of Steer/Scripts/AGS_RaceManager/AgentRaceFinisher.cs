using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace negleft.AGS{
    public class AgentRaceFinisher : MonoBehaviour {
        /// <summary>
        /// The race manager to finish the race
        /// </summary>
        public AgentRaceManager myRaceManager;
        /// <summary>
        /// trigger to finish the race
        /// </summary>
        /// <param name="other"></param>
        public void OnTriggerEnter(Collider other)
        {
            if (!myRaceManager)
            {
                Debug.LogWarning("Agents cant finish, because myRaceManager is not assigned");
                return;
            }

            if (other.GetComponentInParent<AgentController>()) {
                myRaceManager.AgentFinishedTheLap(other.GetComponentInParent<AgentController>());
            }
        }

    }
}