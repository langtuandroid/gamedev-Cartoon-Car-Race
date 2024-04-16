using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    namespace negleft.AGS{
    public class AntiRoll : MonoBehaviour {
        /// <summary>
        /// Left wheel on the axle
        /// </summary>
        public WheelCollider WheelL;
        /// <summary>
        /// Right wheel on axle
        /// </summary>
        public WheelCollider WheelR;
        /// <summary>
        /// Anti roll force multiplier
        /// </summary>
        public float antiRoll = 5000.0f;
        Rigidbody myRigidbody;

        private void Start()
        {
            //Assign the rigidbody
            if (transform.GetComponent<Rigidbody>())
                myRigidbody = transform.GetComponent<Rigidbody>();
        }
        /// <summary>
        /// Apply the antiroll
        /// </summary>
        public void FixedUpdate()
        {

            if (!myRigidbody || !WheelL || !WheelR)
                return;

            WheelHit hit;
            float travelL = 1.0f;
            float travelR = 1.0f;

            bool groundedL = WheelL.GetGroundHit(out hit);
            if (groundedL)
                travelL = (-WheelL.transform.InverseTransformPoint(hit.point).y - WheelL.radius) / WheelL.suspensionDistance;

            bool groundedR = WheelR.GetGroundHit(out hit);
            if (groundedR)
                travelR = (-WheelR.transform.InverseTransformPoint(hit.point).y - WheelR.radius) / WheelR.suspensionDistance;

            float antiRollForce = (travelL - travelR) * antiRoll;

            if (groundedL)
                myRigidbody.AddForceAtPosition(WheelL.transform.up * -antiRollForce,
                    WheelL.transform.position);
            if (groundedR)
                myRigidbody.AddForceAtPosition(WheelR.transform.up * antiRollForce,
                    WheelR.transform.position);
        }
    }
}