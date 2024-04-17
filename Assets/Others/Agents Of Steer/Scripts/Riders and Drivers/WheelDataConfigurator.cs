using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace negleft.AGS{
    public class WheelDataConfigurator : MonoBehaviour
    {

        //Current Wheel Ref
        private WheelFrictionCurve currWheelForwardFriction = new WheelFrictionCurve();
        private WheelFrictionCurve currWheelSidewaysFriction = new WheelFrictionCurve();

        private JointSpring currSpring = new JointSpring();
        //----------------

        [FormerlySerializedAs("suspensionDistance")]
        [Range(0.01f,5.0f)]
        [SerializeField] private float suspensionDistanceValue = 0.5f;

        [FormerlySerializedAs("suspensionSpring")]
        [Range(500.0f,9999.0f)]
        [SerializeField] private float suspensionSpringValue = 6000f;

        [FormerlySerializedAs("suspensionDamper")]
        [Range(500.0f,9999.0f)]
        [SerializeField] private float suspensionDamperValue = 1000.0f;

        [FormerlySerializedAs("suspensionTargetPos")]
        [Range(0f,1.0f)]
        [SerializeField] private float suspensionTargetPosValue = 0.5f;

        [FormerlySerializedAs("forwardStiffness")]
        [Range(0f,8f)]
        [SerializeField] private float forwardStiffnessValue = 2.0f;

        [FormerlySerializedAs("sidewayStiffness")]
        [Range(0f,8f)]
        [SerializeField] private float sidewayStiffnessValue = 4.0f;

        // Start is called before the first frame update
        private void Start()
        {
            //Check if we have a driver
            AICarDriverControl carDriver;

            if (gameObject.TryGetComponent(out AICarDriverControl driver)){
                carDriver = driver;
            }
            else {
                Debug.LogWarning("Wheel Config aborted | no driver found");
                return;
            }
            //Get the wheels
            AICarDriverControl.WheelInformation[] wheels = carDriver.GetTheWheelsInfo();
            //-----Some Variables----

            //-------End of variables ----

            //Go Through Wheels
            for (int i = 0; i < wheels.Length; i++){
                if (wheels[i].wheelCollider){
                    SetupWheelColliderData(wheels[i].wheelCollider);
                }
            }

        }
        private void SetupWheelColliderData(WheelCollider col)
        {
            currWheelForwardFriction.extremumSlip = 0.6f;
            currWheelForwardFriction.extremumValue = 1;
            currWheelForwardFriction.asymptoteSlip = 0.8f;
            currWheelForwardFriction.asymptoteValue = 0.5f;
            currWheelForwardFriction.stiffness = forwardStiffnessValue;
            col.forwardFriction = currWheelForwardFriction;
    
            currWheelSidewaysFriction.extremumSlip = 0.5f;
            currWheelSidewaysFriction.extremumValue = 1;
            currWheelSidewaysFriction.asymptoteSlip = 0.8f;
            currWheelSidewaysFriction.asymptoteValue = 0.75f;
            currWheelSidewaysFriction.stiffness = sidewayStiffnessValue;
            col.sidewaysFriction = currWheelSidewaysFriction;
    
            currSpring.spring = suspensionSpringValue;
            currSpring.damper = suspensionDamperValue;
            currSpring.targetPosition = suspensionTargetPosValue;
    
            col.suspensionSpring = currSpring;
            col.suspensionDistance = suspensionDistanceValue;
        }

    }
}