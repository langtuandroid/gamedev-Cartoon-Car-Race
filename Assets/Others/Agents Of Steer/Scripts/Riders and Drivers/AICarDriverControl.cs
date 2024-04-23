using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace negleft.AGS{
    /// <summary>
    /// Controls the AI car
    /// </summary>
    [RequireComponent(typeof(AgentRaceController))]
    public class AICarDriverControl : MonoBehaviour {
        /// <summary>
        /// Holds wheel Info
        /// </summary>
        [System.Serializable]
        public class WheelInformation
        {
            /// <summary>
            /// Wheel Collider of this wheel
            /// </summary>
            [FormerlySerializedAs("wheelCol")] public WheelCollider wheelCollider;
            /// <summary>
            /// the wheel with geomatry
            /// </summary>
            [FormerlySerializedAs("wheelTrans")] public Transform wheelTransform;
            /// <summary>
            /// Is this a motor wheel
            /// </summary>
            [FormerlySerializedAs("isAMotorWheel")] public bool isAMotorWheelFlag;
            /// <summary>
            /// is this a steering wheel
            /// </summary>
            [FormerlySerializedAs("isASteeringWheel")] public bool isASteeringWheelFlag;
            /// <summary>
            /// Not visible in inspector
            /// holds current skid mark
            /// </summary>
            [FormerlySerializedAs("skidRenderer")] [HideInInspector]
            public LineRenderer skidLineRenderer;
            /// <summary>
            /// Width of the skid mark
            /// </summary>
            [FormerlySerializedAs("skidMarkWidth")] public float skidMarkWidthValue;
            /// <summary>
            /// Not visible in inspector
            /// holds current skid mark positions
            /// </summary>
            [FormerlySerializedAs("skidVertexHolder")] [HideInInspector]
            public List<Vector3> skidVertexHolderList;
        }
        /// <summary>
        /// Holds gearinfo
        /// </summary>
        [System.Serializable]
        public class GearInformation
        {
            /// <summary>
            /// till what rpm of the wheel we will use this gear
            /// </summary>
            [FormerlySerializedAs("tillWhatRPM")] public float tillWhatRPMValue;
            /// <summary>
            /// what the motor torque gonna be on this gear
            /// </summary>
            [FormerlySerializedAs("maxTorqueMultiplier")] public float maxTorqueMultiplierValue;
        }
        /// <summary>
        /// Agent controlller of the vehical
        /// </summary>
        private AgentRaceController myAgentVehicleController;
        /// <summary>
        /// All the wheels of this vehical
        /// </summary>
        [FormerlySerializedAs("wheels")] public List<WheelInformation> wheelsList;
        /// <summary>
        /// Gives controls of this car to player
        /// </summary>
        [FormerlySerializedAs("controlledByPlayer")] public bool controlledByPlayerFlag = false;
        public enum ControlsTypes {Keyboard,UI_Tap_and_Acc_Control,UI_SteerWheel_BTNS };
        /// <summary>
        /// Control types to be chosen
        /// </summary>
        [FormerlySerializedAs("controlType")] public ControlsTypes currControlType;

        /// <summary>
        /// Wheels to calculate the speed of car
        /// </summary>
        [FormerlySerializedAs("wheelsForSpeedCalculation")] public WheelCollider[] wheelsForSpeedCalculationColliders;
        private float wheelRadiusValue = 1.0f;
        private float currentRPMValue = 0.0f;
        /// <summary>
        /// Center of mass at start
        /// </summary>
        [FormerlySerializedAs("centerOfMass")] public Vector3 centerOfMassVector = Vector3.zero;
        /// <summary>
        /// lowered center of mass when at maimum speed
        /// </summary>
        [FormerlySerializedAs("centerOfMassAdditiveY")] public float centerOfMassAdditiveYValue = -0.99f;
        private Vector3 currentCenterOfMassValue = Vector3.zero;
        /// <summary>
        /// Maximum torque
        /// </summary>
        [FormerlySerializedAs("maxTorque")] public float maxTorqueValue = 200;
        /// <summary>
        /// Maximum brake torque
        /// </summary>
        [FormerlySerializedAs("handBrakeTorque")] public float handBrakeTorqueValue = 1000.0f;
        /// <summary>
        /// Brake senstivity
        /// </summary>
        [FormerlySerializedAs("brakeSenstivity")] public float brakeSenstivityValue = 0.75f;
        /// <summary>
        /// Maximum steer angle
        /// </summary>
        [FormerlySerializedAs("maxSteerAngle")] public float maxSteerAngleValue = 30.0f;
        /// <summary>
        /// Restrict steer based on speed
        /// </summary>
        [FormerlySerializedAs("speedRelativeSteer")] public bool speedRelativeSteerFlag = false;
        /// <summary>
        /// Set a value between 0 and 1 it will try to stop vehicle wobbling
        /// </summary>
        [FormerlySerializedAs("steerStabilityThreshold")] public float steerStabilityThresholdValue = 0.15f;
        /// <summary>
        /// current calculated steer angle
        /// </summary>
        private float currentSteerValue = 0.0f;
        /// <summary>
        /// Flip steer on reverse
        /// </summary>
        [FormerlySerializedAs("flipSteerOnReverse")] public bool flipSteerOnReverseFlag = false;
        /// <summary>
        /// How responsive the steering will be
        /// </summary>
        [FormerlySerializedAs("steerResponsiveness")] public float steerResponsivenessValue = 20.0f;
        /// <summary>
        /// Pressure applied to car downwards when climbing 
        /// </summary>
        [FormerlySerializedAs("pressureOnClimbs")] public float pressureOnClimbsValue = 0.15f;
        private Rigidbody myRigidbody;
        /// <summary>
        /// All the gears
        /// </summary>
        [FormerlySerializedAs("gears")] public GearInformation[] gearsInfo; //MaxSpeed for Gears
        private float currentSpeedValue = 0.0f;//KMPH if the "wheelForSpeedCalculation" 
        /// <summary>
        /// Maximum speed of the car in KMPH
        /// </summary>
        [FormerlySerializedAs("maxSpeed")] public float maxSpeedValue = 120.0f;
        /// <summary>
        /// Max reverse speed
        /// </summary>
        [FormerlySerializedAs("maxReverseSpeed")] public float maxReverseSpeedValue = 50.0f;
        private int currentGearValue;
        /// <summary>
        /// maximum distace between two skidmark points
        /// </summary>
        [FormerlySerializedAs("skidVertexDistance")] public float skidVertexDistanceValue = 0.15f;
        /// <summary>
        /// maximum skid verticies for one geomatry
        /// </summary>
        [FormerlySerializedAs("maxSkidVerticies")] public int maxSkidVerticiesValue = 10;
        /// <summary>
        /// emnable skid effects after this much slip
        /// </summary>
        [FormerlySerializedAs("slideAfterSlip")] public float slideAfterSlipValue = 0.002f;
        /// <summary>
        /// Material applied to the skid mark
        /// </summary>
        [FormerlySerializedAs("skidMaterial")] public Material skidMarkMaterial;
        /// <summary>
        /// Skidsmoke for the wheels
        /// </summary>
        [FormerlySerializedAs("skidSmoke")] public GameObject skidSmokeObject;
        /// <summary>
        /// Spark effect on collisions
        /// </summary>
        [FormerlySerializedAs("spark")] public GameObject sparkObject;
        /// <summary>
        /// Skidding sound
        /// </summary>
        [FormerlySerializedAs("skidSound")] public AudioClip skidSoundoClip;

        private AudioSource skidAudioSource;
        /// <summary>
        /// Are we playing the skidding audio
        /// </summary>
        private bool isScreechingFlag = false;
        /// <summary>
        /// All skid smokes
        /// </summary>
        private ParticleSystem[] allSmokesParticles;
        /// <summary>
        /// Can the car reset itself when stuck
        /// </summary>
        [FormerlySerializedAs("canFlip")] public bool canFlipFlag = true;
        /// <summary>
        /// Reset after this much time
        /// </summary>
        [FormerlySerializedAs("flipTimeOut")] public float flipTimeOutValue = 1.0f;
        private float flipCounterValue = 0.0f;
        private AudioSource myAudioSource;
        private bool muteFlag;
        private GameObject skidmarkKeeperObject;
        /// <summary>
        /// Rear brake lights
        /// </summary>
        [FormerlySerializedAs("rearLights")] public Renderer rearLightsRenderer;
        private bool isBrakingFlag = false;
        /// <summary>
        /// Brake color
        /// </summary>
        [FormerlySerializedAs("brakeColor")] [ColorUsageAttribute(true,true)]
        public Color brakeColorValue = Color.red;
        /// <summary>
        /// Idle color
        /// </summary>
        [FormerlySerializedAs("idleColor")] [ColorUsageAttribute(true,true)]

        public Color idleColorValue = Color.black;
        /// <summary>
        /// Reverse color
        /// </summary>
        [FormerlySerializedAs("reverseColor")] [ColorUsageAttribute(true,true)]
        public Color reverseColorValue = Color.white;
        private Color currentColorValue = Color.black;
        private bool limitSpeedFlag = false;
        private float limitedSpeedValue = 0.0f;
        private bool isPlayerReversingFlag = false;
        /// <summary>
        /// are we grounded
        /// </summary>
        private bool groundedFlag = false;
        //BikeGyro bike; // will be used in the next update
        /// <summary>
        /// can vehicle be driven
        /// </summary>
        private bool engineOnFlag = true;
        //This is the input axis to be controlled by player
        private Vector2 inputAxisVector = Vector2.zero;


        // Use this for initialization
        private void Start () {
            //Rigidbody check
            if (transform.GetComponent<Rigidbody>())
                myRigidbody = transform.GetComponent<Rigidbody>();
            else
                Debug.Log("No Rigidbody Found");
            if (myRigidbody)
                myRigidbody.centerOfMass = centerOfMassVector;
            //Setting the starting variables
            currentCenterOfMassValue = centerOfMassVector;
            myAgentVehicleController = transform.GetComponent<AgentRaceController>();
            /*--Reserved for futureUpdate
            if (transform.GetComponent<BikeGyro>()) {
                bike = transform.GetComponent<BikeGyro>();
            }
            */
            if (transform.GetComponent<AudioSource>())
                myAudioSource = transform.GetComponent<AudioSource>();
            SetSkidSmokesParticles();
            GameObject initSkidSource = new GameObject();
            initSkidSource.name = "SkidSource";
            initSkidSource.AddComponent<AudioSource>();
            skidAudioSource = initSkidSource.GetComponent<AudioSource>();
            initSkidSource.transform.parent = transform;
            initSkidSource.transform.localPosition = Vector3.zero;
            skidAudioSource.spatialBlend = 1.0f;
            skidAudioSource.playOnAwake = false;
            if (myAudioSource) {
                skidAudioSource.maxDistance = myAudioSource.maxDistance;
                skidAudioSource.minDistance = myAudioSource.minDistance;
            }
            
            muteFlag = PlayerPrefs.GetInt("Sound", 1) == 0;

        }

        /// <summary>
        /// Call When On mobile
        /// </summary>
        public void SetNewInputControls() {
            int currType = PlayerPrefs.GetInt("InputType", 0);
            switch (currType) {
                case 0: currControlType = ControlsTypes.Keyboard; break;
                case 1: currControlType = ControlsTypes.UI_Tap_and_Acc_Control; break;
                case 2: currControlType = ControlsTypes.UI_SteerWheel_BTNS; break;
            }
        }


        // Update is called once per frame
        private void Update () {
            //Calculate speed and rpm
            CalculateSpeedAndPRM();
            //Play engine sound
            EngineSoundControl();
            
        }
        //Create and position skid smokes
        private void SetSkidSmokesParticles() {
            if (skidSmokeObject) {
                allSmokesParticles = new ParticleSystem[wheelsList.Count];
                for (int i = 0; i < allSmokesParticles.Length; i++) {
                    if (wheelsList[i].wheelCollider) {
                        GameObject currSmokeObj = Instantiate(skidSmokeObject, wheelsList[i].wheelCollider.transform.position - (wheelsList[i].wheelCollider.transform.up * wheelsList[i].wheelCollider.radius), wheelsList[i].wheelCollider.transform.rotation);
                        if (currSmokeObj.GetComponent<ParticleSystem>()) {
                            allSmokesParticles[i] = currSmokeObj.GetComponent<ParticleSystem>();
                            currSmokeObj.transform.parent = transform;
                            allSmokesParticles[i].Stop();

                        }
                    }
                }
            }
        }
        //Flip the car if  stuck
        private void FlipCarCheck() {
            if (!myRigidbody && !canFlipFlag)
                return;
            
            if (myRigidbody.velocity.magnitude <= 0.05f)
            {
                flipCounterValue += Time.fixedDeltaTime;
                if (flipCounterValue >= flipTimeOutValue) {
                    flipCounterValue = 0.0f;
                    transform.position += Vector3.up * 1.0f;
                    transform.localEulerAngles = new Vector3(0.0f, transform.localEulerAngles.y,0.0f);
                    /*--Reserved for futureUpdate
                    if (transform.GetComponent<BikeGyro>()) {
                        transform.GetComponent<BikeGyro>().BikeReset();
                    }*/
                }
            }


        
        }
        /// <summary>
        /// Gives current calculated steer angle
        /// </summary>
        /// <returns></returns>
        public float CurrentCalculatedSteerAngle() {
            return currentSteerValue;
        }

        private void FixedUpdate()
        {
            if (!myRigidbody || !myAgentVehicleController)
                return;

            if (controlledByPlayerFlag)
                PlayerPreferenceControls();

            currentCenterOfMassValue = centerOfMassVector + new Vector3(0.0f, Mathf.Clamp01(myRigidbody.velocity.magnitude / (maxSpeedValue * 0.27f)) * centerOfMassAdditiveYValue, 0.0f);
            myRigidbody.centerOfMass = currentCenterOfMassValue;
            if (myRigidbody.velocity.y > 0)
            myRigidbody.AddForce(-Vector3.up * Mathf.Abs((myRigidbody.velocity.y * myRigidbody.mass) * pressureOnClimbsValue));
            int tempGroundedFlag = 0;
            float tempBrake = 0.0f;

            float angle = Vector3.SignedAngle(transform.forward, myAgentVehicleController.SteerVectorValue().normalized, transform.up);
            float signedAngle = angle;
            float currMaxSpeed = maxSpeedValue;
            currentSteerValue = angle;

            float tempMaxSteer = maxSteerAngleValue;

            if (speedRelativeSteerFlag)
                tempMaxSteer *= 1.0f - (myRigidbody.velocity.magnitude /( maxSpeedValue * 0.277778f));

            if (angle > tempMaxSteer)
                angle = tempMaxSteer;
            else if (angle < -tempMaxSteer)
                angle = -tempMaxSteer;

            if (Mathf.Abs(angle) < (maxSteerAngleValue * steerStabilityThresholdValue) && !controlledByPlayerFlag)
                angle = 0.0f;

            if ((myAgentVehicleController.isReversingFlag() && !controlledByPlayerFlag) || (isPlayerReversingFlag&&controlledByPlayerFlag))
            {
                currMaxSpeed = maxReverseSpeedValue;
                if (flipSteerOnReverseFlag)
                    angle *= -1.0f;
            }

            bool willScreech = false;
            //Controlling Powered Wheels
            for (int i = 0; i < wheelsList.Count; i++)
            {
                //Controlling the car
                if (((currentSpeedValue < currMaxSpeed && !limitSpeedFlag && myAgentVehicleController.currTarget) || (currentSpeedValue <= limitedSpeedValue && limitSpeedFlag && currentSpeedValue < currMaxSpeed && myAgentVehicleController.currTarget)) && engineOnFlag)
                {
                    if (wheelsList[i].wheelCollider && wheelsList[i].isAMotorWheelFlag)
                    {
                        if (!controlledByPlayerFlag)
                        {//Ai Controls
                            if (!myAgentVehicleController.isReversingFlag())
                            {
                                if (wheelsList[i].wheelCollider.motorTorque < 0.0f)
                                {
                                    wheelsList[i].wheelCollider.motorTorque = 0.0f;
                                    wheelsList[i].wheelCollider.brakeTorque = handBrakeTorqueValue;
                                }
                                else
                                {
                                    wheelsList[i].wheelCollider.brakeTorque = 0.0f;
                                }


                                if (Mathf.Abs(signedAngle) > (90.0f + ((1.0f - brakeSenstivityValue) * 90.0f)) && myAgentVehicleController.SomethingBlockingDetected() && !myAgentVehicleController.isReversingFlag())
                                    tempBrake = 1.0f;
                                else
                                    tempBrake = 0.0f;


                                //Braking set and reset
                                if (!isBrakingFlag && tempBrake != 0)
                                {
                                    StopCoroutine(FadeBrakeLightsCoroutine());
                                    currentColorValue = brakeColorValue;
                                    StartCoroutine(FadeBrakeLightsCoroutine());
                                    isBrakingFlag = true;
                                }
                                else if (isBrakingFlag && tempBrake == 0)
                                {
                                    StopCoroutine(FadeBrakeLightsCoroutine());
                                    currentColorValue = idleColorValue;
                                    StartCoroutine(FadeBrakeLightsCoroutine());
                                    isBrakingFlag = false;
                                }

                                if (!isBrakingFlag)
                                {
                                    wheelsList[i].wheelCollider.motorTorque = GetCurrentTorqueByRPM(wheelsList[i].wheelCollider.rpm);
                                    wheelsList[i].wheelCollider.brakeTorque = 0.0f;
                                }
                                else
                                {
                                    wheelsList[i].wheelCollider.motorTorque = 0.0f;
                                    wheelsList[i].wheelCollider.brakeTorque = handBrakeTorqueValue;
                                }
                            }
                            else
                            {
                                isBrakingFlag = true;


                                if (wheelsList[i].wheelCollider.motorTorque > 0.0f)
                                {
                                    StopCoroutine(FadeBrakeLightsCoroutine());
                                    currentColorValue = brakeColorValue;
                                    StartCoroutine(FadeBrakeLightsCoroutine());
                                    wheelsList[i].wheelCollider.brakeTorque = handBrakeTorqueValue;
                                    wheelsList[i].wheelCollider.motorTorque = 0.0f;
                                }
                                else
                                {

                                    if (gearsInfo.Length > 0)
                                    {
                                        if (Mathf.Abs(signedAngle) < (brakeSenstivityValue * 90.0f))
                                            tempBrake = 1.0f;
                                        else
                                            tempBrake = 0.0f;

                                        if (tempBrake != 0)
                                        {
                                            StopCoroutine(FadeBrakeLightsCoroutine());
                                            currentColorValue = brakeColorValue;
                                            StartCoroutine(FadeBrakeLightsCoroutine());
                                            wheelsList[i].wheelCollider.brakeTorque = handBrakeTorqueValue;
                                        }
                                        else if (tempBrake == 0)
                                        {
                                            StopCoroutine(FadeBrakeLightsCoroutine());
                                            currentColorValue = reverseColorValue;
                                            StartCoroutine(FadeBrakeLightsCoroutine());
                                            wheelsList[i].wheelCollider.brakeTorque = 0.0f;
                                        }


                                        wheelsList[i].wheelCollider.motorTorque = -(maxTorqueValue * gearsInfo[0].maxTorqueMultiplierValue);
                                    }
                                }
                                
                            }
                        }
                        else {//Player controls
                            float yMove = inputAxisVector.y;
                            float currentTorque = 0.0f;
                            bool tempRev = false;
                            if (yMove > 0)
                            {
                                if (Mathf.Round(currentRPMValue) < 0)
                                {
                                    currentTorque = 0;
                                    tempBrake = 1;
                                }
                                else
                                {
                                    currentTorque = GetCurrentTorqueByRPM(wheelsList[i].wheelCollider.rpm) * yMove;
                                    tempBrake = 0;
                                    tempRev = false;

                                }
                            }
                            else if (yMove < 0)
                            {
                                if (Mathf.Round(currentRPMValue) > 0)
                                {
                                    currentTorque = 0;
                                    tempBrake = 1;
                                }
                                else
                                {
                                    if (gearsInfo.Length > 0)
                                    currentTorque = (gearsInfo[0].maxTorqueMultiplierValue * maxTorqueValue) * yMove;
                                    tempBrake = 0;
                                    tempRev = true;
                                }
                            }
                            else {
                                currentTorque = 0.0f;
                                tempBrake = 1.0f;
                            }

                            if (Input.GetButton("Jump") || tempBrake == 1.0)
                            {
                                currentTorque = 0.0f;
                                wheelsList[i].wheelCollider.brakeTorque = handBrakeTorqueValue;
                                tempBrake = 1;

                            }
                            else {
                                wheelsList[i].wheelCollider.brakeTorque = 0;
                                tempBrake = 0;
                            }

                            //Applying torque here
                            wheelsList[i].wheelCollider.motorTorque = currentTorque;


                            if (!isBrakingFlag && tempBrake != 0 && !isPlayerReversingFlag)
                            {
                                StopCoroutine(FadeBrakeLightsCoroutine());
                                currentColorValue = brakeColorValue;
                                StartCoroutine(FadeBrakeLightsCoroutine());
                                isBrakingFlag = true;
                            }
                            else if (isBrakingFlag && tempBrake == 0 && !isPlayerReversingFlag)
                            {
                                StopCoroutine(FadeBrakeLightsCoroutine());
                                currentColorValue = idleColorValue;
                                StartCoroutine(FadeBrakeLightsCoroutine());
                                isBrakingFlag = false;
                            }

                            if (!isPlayerReversingFlag && tempRev) {
                                isPlayerReversingFlag = true;
                                StopCoroutine(FadeBrakeLightsCoroutine());
                                currentColorValue = reverseColorValue;
                                StartCoroutine(FadeBrakeLightsCoroutine());
                            } else if (isPlayerReversingFlag && !tempRev) {
                                isPlayerReversingFlag = false;
                                StopCoroutine(FadeBrakeLightsCoroutine());
                                currentColorValue = idleColorValue;
                                StartCoroutine(FadeBrakeLightsCoroutine());
                            }

                        }

                    }

                }
                else {
                    if (wheelsList[i].wheelCollider && wheelsList[i].isAMotorWheelFlag) {
                        wheelsList[i].wheelCollider.motorTorque = 0.0f;
                        wheelsList[i].wheelCollider.brakeTorque = handBrakeTorqueValue;
                    }
                }
                //Controlling steering wheels
                
                if (wheelsList[i].wheelCollider && wheelsList[i].isASteeringWheelFlag) {
                    if (!controlledByPlayerFlag)
                        wheelsList[i].wheelCollider.steerAngle = Mathf.LerpAngle(wheelsList[i].wheelCollider.steerAngle, angle, Time.fixedDeltaTime * steerResponsivenessValue);
                    else
                        wheelsList[i].wheelCollider.steerAngle = inputAxisVector.x * tempMaxSteer;

                }
                //Apply visuals of the wheels
                if (wheelsList[i].wheelCollider && wheelsList[i].wheelTransform) {
                    ApplyLocalPositionToVisualsByColliders(wheelsList[i].wheelCollider, wheelsList[i].wheelTransform);
                }

                SkidWheelMark(wheelsList[i] , i,ref willScreech,ref tempGroundedFlag);
                
            }
            //Screeching sound
            if (willScreech && skidAudioSource && !isScreechingFlag && skidSoundoClip && !muteFlag)
            {
                isScreechingFlag = true;
                skidAudioSource.PlayOneShot(skidSoundoClip);
                Invoke("StopScreeching", skidSoundoClip.length);
                
            }
            //Ground Check
            if (tempGroundedFlag > 0)
                groundedFlag = true;
            else
                groundedFlag = false;
        }

        //Controls based on players Prefrence
        private void PlayerPreferenceControls() {
            switch (currControlType) {
                case ControlsTypes.Keyboard:
                inputAxisVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                break;
                case ControlsTypes.UI_SteerWheel_BTNS: break;
                case ControlsTypes.UI_Tap_and_Acc_Control: inputAxisVector = new Vector2(Mathf.Clamp(Input.acceleration.x, -1.0f, 1.0f), inputAxisVector.y); break;
            }
        }
        /// <summary>
        /// Set Y input
        /// </summary>
        /// <param name="to"></param>
        public void SetInputYValue (float to) {
            inputAxisVector = new Vector2(inputAxisVector.x, to);
        }

        /// <summary>
        /// Set X input
        /// </summary>
        /// <param name="to"></param>
        public void SetInputXValue(float to)
        {
            inputAxisVector = new Vector2(to,inputAxisVector.y);
        }


        /// <summary>
        /// Are we grounded?
        /// </summary>
        /// <returns></returns>
        public bool GetGroundFlag() {
            return groundedFlag;
        }

        /// <summary>
        /// Set engine On or Off
        /// </summary>

        public void SetEngineFlag( bool recVal) {
            engineOnFlag = recVal;
        }

        //Stop screeching
        private void StopScreeching() {
            isScreechingFlag = false;
        }
        //Create Skidmark for the passed wheel
        private void SkidWheelMark(WheelInformation currWheel , int index , ref bool willScreech , ref int tempGroundFlag) {
            WheelHit currHit;
            float realSlip = 0;
            GameObject newSkid;
            if (currWheel.wheelTransform && currWheel.wheelCollider)
            {
                currWheel.wheelCollider.GetGroundHit(out currHit);
                realSlip = Mathf.Abs(currHit.sidewaysSlip);
                if (currHit.collider && realSlip > slideAfterSlipValue)
                {
                    if (skidSmokeObject)
                    {
                        if (currWheel.wheelCollider)
                            allSmokesParticles[index].transform.position = currWheel.wheelCollider.transform.position - currWheel.wheelCollider.transform.up * currWheel.wheelCollider.radius;
                        allSmokesParticles[index].Play();
                    }
                    
                    willScreech = true;
                    

                    if (!currWheel.skidLineRenderer)
                    {
                        
                        //Create line renderer for skid mark
                        newSkid = new GameObject();
                        newSkid.name = "Skidmark";
                        if (!skidmarkKeeperObject)
                        {
                            skidmarkKeeperObject = new GameObject();
                            skidmarkKeeperObject.name = "SkidMarks-" + gameObject.name;
                        }
                        newSkid.transform.parent = skidmarkKeeperObject.transform;
                        wheelsList[index].skidLineRenderer = newSkid.AddComponent<LineRenderer>();
                        wheelsList[index].skidLineRenderer.startWidth = wheelsList[index].skidMarkWidthValue;
                        wheelsList[index].skidLineRenderer.endWidth = wheelsList[index].skidMarkWidthValue;
                        wheelsList[index].skidLineRenderer.useWorldSpace = true;
                        wheelsList[index].skidLineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                        if (skidMarkMaterial) {
                            wheelsList[index].skidLineRenderer.material = skidMarkMaterial;
                        }
                        wheelsList[index].skidVertexHolderList.Capacity = 10;
                        wheelsList[index].skidVertexHolderList.Add(currHit.point - (currHit.point - currWheel.wheelCollider.transform.position).normalized*0.001f);
                        wheelsList[index].skidLineRenderer.alignment = LineAlignment.TransformZ;
                        currWheel.skidLineRenderer.positionCount = wheelsList[index].skidVertexHolderList.Count;
                        currWheel.skidLineRenderer.transform.localEulerAngles = new Vector3(90.0f, 0.0f, 0.0f);

                    }
                    else {
                    //Postion skid mark
                        if ((currWheel.skidVertexHolderList[currWheel.skidVertexHolderList.Count - 1] - currHit.point).magnitude >= skidVertexDistanceValue) {

                            if (wheelsList[index].skidVertexHolderList.Count >= maxSkidVerticiesValue) {

                                if (wheelsList[index].skidLineRenderer)
                                {
                                    wheelsList[index].skidLineRenderer.gameObject.AddComponent<SkidmarkDestroyerBehaviour>();
                                    wheelsList[index].skidLineRenderer = null;
                                }
                                if (wheelsList[index].skidVertexHolderList.Count != 0)
                                    wheelsList[index].skidVertexHolderList.Clear();
                                return;
                            }
                            currWheel.skidLineRenderer.positionCount = wheelsList[index].skidVertexHolderList.Count;
                            wheelsList[index].skidVertexHolderList.Add(currHit.point - (currHit.point - currWheel.wheelCollider.transform.position).normalized * 0.001f);
                                currWheel.skidLineRenderer.SetPositions(wheelsList[index].skidVertexHolderList.ToArray());
                        }
                    }
                }
                else {

                    if (skidSmokeObject)
                    {
                        allSmokesParticles[index].Stop();
                    }
                    if (wheelsList[index].skidLineRenderer)
                    {
                        wheelsList[index].skidLineRenderer.gameObject.AddComponent<SkidmarkDestroyerBehaviour>();
                        wheelsList[index].skidLineRenderer = null;
                    }
                    if (wheelsList[index].skidVertexHolderList.Count != 0)
                        wheelsList[index].skidVertexHolderList.Clear();
                }
                if (!currHit.collider)
                {
                    //if (!bike)
                        FlipCarCheck();
                }
                else {
                    tempGroundFlag++;
                }
                /*
                if (bike) {
                    if (bike.isCrashed())
                        FlipCheck();
                }*/
            }
            

        }
        //Returns current torque given by gears based on rpm
        private float GetCurrentTorqueByRPM(float RPM) {
            float newTorque = maxTorqueValue;
            
            for (int i = 0; i < gearsInfo.Length; i++)
            {
                if (RPM < gearsInfo[i].tillWhatRPMValue) {
                    newTorque = maxTorqueValue *  gearsInfo[i].maxTorqueMultiplierValue;
                    currentGearValue = i;
                    
                    break;
                }
            }

            return newTorque;
        }
        /// <summary>
        /// Returns car's current speed
        /// </summary>
        /// <returns></returns>
        public float GetCurrentSpeedValue() {
            return currentSpeedValue;
        }

        /// <summary>
        /// position the transform based on wheel colliders calculated positon
        /// </summary>
        /// <param name="col">passed wheel collider</param>
        /// <param name="visualTransform">passed wheel geomatry</param>
        private void ApplyLocalPositionToVisualsByColliders(WheelCollider col, Transform visualTransform)
        {
            Vector3 position;
            Quaternion rotation;
            col.GetWorldPose(out position, out rotation);

            visualTransform.transform.position = position;
            visualTransform.transform.rotation = rotation;
        }
        //Calculate speed and rpm
        private void CalculateSpeedAndPRM() {
            if (wheelsForSpeedCalculationColliders.Length != 0)
            {
                float counted = 0;
                float tempRPM = 0.0f;
                for (int i = 0; i < wheelsForSpeedCalculationColliders.Length; i++) {
                    if (wheelsForSpeedCalculationColliders[i]) {
                        if (counted == 0)
                            wheelRadiusValue = wheelsForSpeedCalculationColliders[i].radius;

                        counted++;
                        tempRPM += wheelsForSpeedCalculationColliders[i].rpm;
                    }
                }
                currentRPMValue = tempRPM / counted;
                currentSpeedValue = (((2.0f * Mathf.PI * wheelRadiusValue) * Mathf.Abs(currentRPMValue) ) * 60.0f)/1000.0f;
                currentSpeedValue = Mathf.Abs(currentSpeedValue);
                currentSpeedValue = Mathf.Round(currentSpeedValue);
            }
            else {
                currentSpeedValue = myAgentVehicleController.GetVelocityVector().magnitude;
            }
        }
        //Control engine sound
        private void EngineSoundControl()
        {
                if (!myAudioSource)
                {
                    return;
                }

                float tempVar = 0.0f;

                float minValue = 0;
                float maxValue = 0;
                if (currentGearValue < gearsInfo.Length)
                {
                    tempVar = (((2.0f * 3.14f * wheelRadiusValue) * gearsInfo[currentGearValue].tillWhatRPMValue) * 60.0f) / 1000.0f;
                    tempVar = Mathf.Abs(tempVar);
                    tempVar = Mathf.Round(tempVar);
                    maxValue = tempVar;
                }
                tempVar = 0.0f;
                if ((currentGearValue - 1) >= 0 && (currentGearValue - 1) < gearsInfo.Length)
                {
                    tempVar = (((2.0f * 3.14f * wheelRadiusValue) * gearsInfo[currentGearValue - 1].tillWhatRPMValue) * 60.0f) / 1000.0f;
                    tempVar = Mathf.Abs(tempVar);
                    tempVar = Mathf.Round(tempVar);

                    minValue = tempVar;
                }
                if (minValue == maxValue)
                    return;
                float enginePitch = Mathf.Lerp(myAudioSource.pitch , ((currentSpeedValue - minValue) / (maxValue - minValue)) + 1.0f , Time.deltaTime * (( maxSpeedValue / currentSpeedValue) * 2.0f) );

            if (enginePitch < 0.85f)
                enginePitch = 0.85f;
            else
                enginePitch += 0.05f;
                myAudioSource.pitch = Mathf.Clamp(Mathf.Abs(enginePitch) , -3.0f,3.0f);
                return;
            
        }
        //Fading if brake lights
        private IEnumerator FadeBrakeLightsCoroutine() {

            if (!rearLightsRenderer)
                yield return null;

            float lastTime = Time.time;
            float lightLerper = 0.0f;

            while (lightLerper <= 1.0f) {
                lightLerper += (Time.time - lastTime)*2.5f;
                if (rearLightsRenderer)
                rearLightsRenderer.material.SetColor("_EmissionColor", Color.Lerp(Color.black, currentColorValue, lightLerper));
                lastTime = Time.time;
                yield return null;
            }

            yield return null;
        }

        //Speed limiter enter
        private void OnTriggerStay(Collider other)
        {
            if (other.GetComponent<LimitedSpeedZoneBehaviour>()) {
                limitSpeedFlag = true;
                limitedSpeedValue = other.GetComponent<LimitedSpeedZoneBehaviour>().maxSpeedValue;
            }
        }
        //Speed limiter exit
        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<LimitedSpeedZoneBehaviour>())
            {
                limitSpeedFlag = false;
            }
        }
        //Sparks on collision
        private void OnCollisionEnter(Collision collision)
        {
            if (!sparkObject)
                return;

            ContactPoint[] contacts = collision.contacts;

            foreach (ContactPoint cp in contacts) {
                Instantiate(sparkObject, cp.point, Quaternion.identity);
            }
            
        }
        /// <summary>
        /// Are we reversing?
        /// </summary>
        /// <returns></returns>
        public bool IsReversingFlag() {
            if (controlledByPlayerFlag)
                return isPlayerReversingFlag;
            else
                return myAgentVehicleController.isReversingFlag();
        }
        /// <summary>
        /// Get the speed value clamped between 0 and 1
        /// </summary>
        /// <returns></returns>
        public float GetClampedVelocityValue() {
            float newValue = Mathf.Clamp01(currentSpeedValue / maxSpeedValue);
            return newValue;
        }

        public WheelInformation[] GetTheWheelsInfo(){
            return wheelsList.ToArray();
        }
    }
}