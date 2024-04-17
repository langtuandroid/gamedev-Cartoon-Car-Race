using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace negleft.AGS{
    /// <summary>
    /// Specfic area where you want to set the maximum speed of an agent
    /// </summary>
    public class LimitedSpeedZoneBehaviour : MonoBehaviour {
        /// <summary>
        /// Maximum speed of the agent in this zone
        /// </summary>
        [FormerlySerializedAs("maxSpeed")] public float maxSpeedValue = 100;

        // Use this for initialization
        private void Start () {
            if (transform.GetComponent<Renderer>())
                transform.GetComponent<Renderer>().enabled = false;
        }
    }
}