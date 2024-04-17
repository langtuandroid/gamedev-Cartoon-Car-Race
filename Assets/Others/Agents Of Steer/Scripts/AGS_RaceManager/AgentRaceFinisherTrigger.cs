using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace negleft.AGS{
    public class AgentRaceFinisherTrigger : MonoBehaviour {
        /// <summary>
        /// The race manager to finish the race
        /// </summary>
        [FormerlySerializedAs("myRaceManager")] [SerializeField] private AgentRacePathManager myRaceManagerAgent;
        /// <summary>
        /// trigger to finish the race
        /// </summary>
        /// <param name="other"></param>
        public void OnTriggerEnter(Collider other)
        {
            if (!myRaceManagerAgent)
            {
                Debug.LogWarning("Agents cant finish, because myRaceManager is not assigned");
                return;
            }

            if (other.GetComponentInParent<AgentRaceController>()) {
                myRaceManagerAgent.AgentFinishedLap(other.GetComponentInParent<AgentRaceController>());
            }
        }

    }
}