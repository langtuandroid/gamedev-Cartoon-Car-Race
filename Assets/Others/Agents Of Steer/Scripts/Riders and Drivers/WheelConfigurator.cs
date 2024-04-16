using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace negleft.AGS{
    public class WheelConfigurator : MonoBehaviour
    {

        //Current Wheel Ref
        WheelFrictionCurve wheelForwardFriction = new WheelFrictionCurve();
        WheelFrictionCurve wheelSidewaysFriction = new WheelFrictionCurve();

        JointSpring spring = new JointSpring();
        //----------------

        [Range(0.01f,5.0f)]
        public float suspensionDistance = 0.5f;

        [Range(500.0f,9999.0f)]
        public float suspensionSpring = 6000f;

        [Range(500.0f,9999.0f)]
        public float suspensionDamper = 1000.0f;

        [Range(0f,1.0f)]
        public float suspensionTargetPos = 0.5f;

        [Range(0f,8f)]
        public float forwardStiffness = 2.0f;

        [Range(0f,8f)]
        public float sidewayStiffness = 4.0f;

        // Start is called before the first frame update
        void Start()
        {
            //Check if we have a driver
            AICarDriver carDriver;

            if (gameObject.TryGetComponent(out AICarDriver driver)){
                carDriver = driver;
            }
            else {
                Debug.LogWarning("Wheel Config aborted | no driver found");
                return;
            }
            //Get the wheels
            AICarDriver.WheelInfo[] wheels = carDriver.GetTheWheels();
            //-----Some Variables----

            //-------End of variables ----

            //Go Through Wheels
            for (int i = 0; i < wheels.Length; i++){
                if (wheels[i].wheelCol){
                    SetupWheelCollider(wheels[i].wheelCol);
                }
            }

        }
        void SetupWheelCollider(WheelCollider col)
        {
            wheelForwardFriction.extremumSlip = 0.6f;
            wheelForwardFriction.extremumValue = 1;
            wheelForwardFriction.asymptoteSlip = 0.8f;
            wheelForwardFriction.asymptoteValue = 0.5f;
            wheelForwardFriction.stiffness = forwardStiffness;
            col.forwardFriction = wheelForwardFriction;
    
            wheelSidewaysFriction.extremumSlip = 0.5f;
            wheelSidewaysFriction.extremumValue = 1;
            wheelSidewaysFriction.asymptoteSlip = 0.8f;
            wheelSidewaysFriction.asymptoteValue = 0.75f;
            wheelSidewaysFriction.stiffness = sidewayStiffness;
            col.sidewaysFriction = wheelSidewaysFriction;
    
            spring.spring = suspensionSpring;
            spring.damper = suspensionDamper;
            spring.targetPosition = suspensionTargetPos;
    
            col.suspensionSpring = spring;
            col.suspensionDistance = suspensionDistance;
        }

    }
}