using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace negleft.AGS{
    public class AntiRollControl : MonoBehaviour {
        /// <summary>
        /// Left wheel on the axle
        /// </summary>
        [FormerlySerializedAs("WheelL")] [SerializeField] private WheelCollider WheelLCollider;
        /// <summary>
        /// Right wheel on axle
        /// </summary>
        [FormerlySerializedAs("WheelR")] [SerializeField] private WheelCollider WheelRCollider;
        /// <summary>
        /// Anti roll force multiplier
        /// </summary>
        [FormerlySerializedAs("antiRoll")] [SerializeField] private float antiRollValue = 5000.0f;
        private Rigidbody myRigidbodyObject;

        private void Start()
        {
            //Assign the rigidbody
            if (transform.GetComponent<Rigidbody>())
                myRigidbodyObject = transform.GetComponent<Rigidbody>();
        }
        /// <summary>
        /// Apply the antiroll
        /// </summary>
        public void FixedUpdate()
        {

            if (!myRigidbodyObject || !WheelLCollider || !WheelRCollider)
                return;

            WheelHit hit;
            float travelL = 1.0f;
            float travelR = 1.0f;

            bool groundedL = WheelLCollider.GetGroundHit(out hit);
            if (groundedL)
                travelL = (-WheelLCollider.transform.InverseTransformPoint(hit.point).y - WheelLCollider.radius) / WheelLCollider.suspensionDistance;

            bool groundedR = WheelRCollider.GetGroundHit(out hit);
            if (groundedR)
                travelR = (-WheelRCollider.transform.InverseTransformPoint(hit.point).y - WheelRCollider.radius) / WheelRCollider.suspensionDistance;

            float antiRollForce = (travelL - travelR) * antiRollValue;

            if (groundedL)
                myRigidbodyObject.AddForceAtPosition(WheelLCollider.transform.up * -antiRollForce,
                    WheelLCollider.transform.position);
            if (groundedR)
                myRigidbodyObject.AddForceAtPosition(WheelRCollider.transform.up * antiRollForce,
                    WheelRCollider.transform.position);
        }
    }
}