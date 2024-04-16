using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace negleft.AGS{
    /// <summary>
    /// Specfic area where you want to set the maximum speed of an agent
    /// </summary>
    public class LimitedSpeedZone : MonoBehaviour {
        /// <summary>
        /// Maximum speed of the agent in this zone
        /// </summary>
        public float maxSpeed = 100;

        // Use this for initialization
        void Start () {
            if (transform.GetComponent<Renderer>())
                transform.GetComponent<Renderer>().enabled = false;

        }
        
    }
}