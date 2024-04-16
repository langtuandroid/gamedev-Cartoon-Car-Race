using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace negleft.AGS{
    /// <summary>
    /// Controls the AI car
    /// </summary>
    [RequireComponent(typeof(AgentController))]
    public class AICarDriver : MonoBehaviour {
        /// <summary>
        /// Holds wheel Info
        /// </summary>
        [System.Serializable]
        public class WheelInfo
        {
            /// <summary>
            /// Wheel Collider of this wheel
            /// </summary>
            public WheelCollider wheelCol;
            /// <summary>
            /// the wheel with geomatry
            /// </summary>
            public Transform wheelTrans;
            /// <summary>
            /// Is this a motor wheel
            /// </summary>
            public bool isAMotorWheel;
            /// <summary>
            /// is this a steering wheel
            /// </summary>
            public bool isASteeringWheel;
            /// <summary>
            /// Not visible in inspector
            /// holds current skid mark
            /// </summary>
            [HideInInspector]
            public LineRenderer skidRenderer;
            /// <summary>
            /// Width of the skid mark
            /// </summary>
            public float skidMarkWidth;
            /// <summary>
            /// Not visible in inspector
            /// holds current skid mark positions
            /// </summary>
            [HideInInspector]
            public List<Vector3> skidVertexHolder;
        }
        /// <summary>
        /// Holds gearinfo
        /// </summary>
        [System.Serializable]
        public class GearInfo
        {
            /// <summary>
            /// till what rpm of the wheel we will use this gear
            /// </summary>
            public float tillWhatRPM;
            /// <summary>
            /// what the motor torque gonna be on this gear
            /// </summary>
            public float maxTorqueMultiplier;
        }
        /// <summary>
        /// Agent controlller of the vehical
        /// </summary>
        AgentController myAgentController;
        /// <summary>
        /// All the wheels of this vehical
        /// </summary>
        public List<WheelInfo> wheels;
        /// <summary>
        /// Gives controls of this car to player
        /// </summary>
        public bool controlledByPlayer = false;
        public enum ControlTypes {Keyboard,UI_Tap_and_Acc_Control,UI_SteerWheel_BTNS };
        /// <summary>
        /// Control types to be chosen
        /// </summary>
        public ControlTypes controlType;

        /// <summary>
        /// Wheels to calculate the speed of car
        /// </summary>
        public WheelCollider[] wheelsForSpeedCalculation;
        float wheelRadius = 1.0f;
        float currentRPM = 0.0f;
        /// <summary>
        /// Center of mass at start
        /// </summary>
        public Vector3 centerOfMass = Vector3.zero;
        /// <summary>
        /// lowered center of mass when at maimum speed
        /// </summary>
        public float centerOfMassAdditiveY = -0.99f;
        Vector3 currentCenterOfMass = Vector3.zero;
        /// <summary>
        /// Maximum torque
        /// </summary>
        public float maxTorque = 200;
        /// <summary>
        /// Maximum brake torque
        /// </summary>
        public float handBrakeTorque = 1000.0f;
        /// <summary>
        /// Brake senstivity
        /// </summary>
        public float brakeSenstivity = 0.75f;
        /// <summary>
        /// Maximum steer angle
        /// </summary>
        public float maxSteerAngle = 30.0f;
        /// <summary>
        /// Restrict steer based on speed
        /// </summary>
        public bool speedRelativeSteer = false;
        /// <summary>
        /// Set a value between 0 and 1 it will try to stop vehicle wobbling
        /// </summary>
        public float steerStabilityThreshold = 0.15f;
        /// <summary>
        /// current calculated steer angle
        /// </summary>
        float currentSteer = 0.0f;
        /// <summary>
        /// Flip steer on reverse
        /// </summary>
        public bool flipSteerOnReverse = false;
        /// <summary>
        /// How responsive the steering will be
        /// </summary>
        public float steerResponsiveness = 20.0f;
        /// <summary>
        /// Pressure applied to car downwards when climbing 
        /// </summary>
        public float pressureOnClimbs = 0.15f;
        Rigidbody myRigidBody;
        /// <summary>
        /// All the gears
        /// </summary>
        public GearInfo[] gears; //MaxSpeed for Gears
        float currentSpeed = 0.0f;//KMPH if the "wheelForSpeedCalculation" 
        /// <summary>
        /// Maximum speed of the car in KMPH
        /// </summary>
        public float maxSpeed = 120.0f;
        /// <summary>
        /// Max reverse speed
        /// </summary>
        public float maxReverseSpeed = 50.0f;
        int currentGear;
        /// <summary>
        /// maximum distace between two skidmark points
        /// </summary>
        public float skidVertexDistance = 0.15f;
        /// <summary>
        /// maximum skid verticies for one geomatry
        /// </summary>
        public int maxSkidVerticies = 10;
        /// <summary>
        /// emnable skid effects after this much slip
        /// </summary>
        public float slideAfterSlip = 0.002f;
        /// <summary>
        /// Material applied to the skid mark
        /// </summary>
        public Material skidMaterial;
        /// <summary>
        /// Skidsmoke for the wheels
        /// </summary>
        public GameObject skidSmoke;
        /// <summary>
        /// Spark effect on collisions
        /// </summary>
        public GameObject spark;
        /// <summary>
        /// Skidding sound
        /// </summary>
        public AudioClip skidSound;

        AudioSource skidSource;
        /// <summary>
        /// Are we playing the skidding audio
        /// </summary>
        bool isScreeching = false;
        /// <summary>
        /// All skid smokes
        /// </summary>
        ParticleSystem[] allSmokes;
        /// <summary>
        /// Can the car reset itself when stuck
        /// </summary>
        public bool canFlip = true;
        /// <summary>
        /// Reset after this much time
        /// </summary>
        public float flipTimeOut = 1.0f;
        float flipCounter = 0.0f;
        AudioSource mySource;
        GameObject skidmarkKeeper;
        /// <summary>
        /// Rear brake lights
        /// </summary>
        public Renderer rearLights;
        bool isBraking = false;
        /// <summary>
        /// Brake color
        /// </summary>
        [ColorUsageAttribute(true,true)]
        public Color brakeColor = Color.red;
        /// <summary>
        /// Idle color
        /// </summary>
        [ColorUsageAttribute(true,true)]

        public Color idleColor = Color.black;
        /// <summary>
        /// Reverse color
        /// </summary>
        [ColorUsageAttribute(true,true)]
        public Color reverseColor = Color.white;
        Color currentColor = Color.black;
        bool limitSpeed = false;
        float limitedSpeed = 0.0f;
        bool isPlayerReversing = false;
        /// <summary>
        /// are we grounded
        /// </summary>
        bool grounded = false;
        //BikeGyro bike; // will be used in the next update
        /// <summary>
        /// can vehicle be driven
        /// </summary>
        bool engineOn = true;
        //This is the input axis to be controlled by player
        Vector2 inputAxis = Vector2.zero;


        // Use this for initialization
        void Start () {
            //Rigidbody check
            if (transform.GetComponent<Rigidbody>())
                myRigidBody = transform.GetComponent<Rigidbody>();
            else
                Debug.Log("No Rigidbody Found");
            if (myRigidBody)
                myRigidBody.centerOfMass = centerOfMass;
            //Setting the starting variables
            currentCenterOfMass = centerOfMass;
            myAgentController = transform.GetComponent<AgentController>();
            /*--Reserved for futureUpdate
            if (transform.GetComponent<BikeGyro>()) {
                bike = transform.GetComponent<BikeGyro>();
            }
            */
            if (transform.GetComponent<AudioSource>())
                mySource = transform.GetComponent<AudioSource>();
            SetSkidSmokes();
            GameObject initSkidSource = new GameObject();
            initSkidSource.name = "SkidSource";
            initSkidSource.AddComponent<AudioSource>();
            skidSource = initSkidSource.GetComponent<AudioSource>();
            initSkidSource.transform.parent = transform;
            initSkidSource.transform.localPosition = Vector3.zero;
            skidSource.spatialBlend = 1.0f;
            skidSource.playOnAwake = false;
            if (mySource) {
                skidSource.maxDistance = mySource.maxDistance;
                skidSource.minDistance = mySource.minDistance;
            }
            
        }

        /// <summary>
        /// Call When On mobile
        /// </summary>
        public void SetNewControls() {
            int currType = PlayerPrefs.GetInt("InputType", 0);
            switch (currType) {
                case 0: controlType = ControlTypes.Keyboard; break;
                case 1: controlType = ControlTypes.UI_Tap_and_Acc_Control; break;
                case 2: controlType = ControlTypes.UI_SteerWheel_BTNS; break;
            }
        }


        // Update is called once per frame
        void Update () {
            //Calculate speed and rpm
            CalculateSpeed();
            //Play engine sound
            EngineSound();
            
        }
        //Create and position skid smokes
        void SetSkidSmokes() {
            if (skidSmoke) {
                allSmokes = new ParticleSystem[wheels.Count];
                for (int i = 0; i < allSmokes.Length; i++) {
                    if (wheels[i].wheelCol) {
                        GameObject currSmokeObj = Instantiate(skidSmoke, wheels[i].wheelCol.transform.position - (wheels[i].wheelCol.transform.up * wheels[i].wheelCol.radius), wheels[i].wheelCol.transform.rotation);
                        if (currSmokeObj.GetComponent<ParticleSystem>()) {
                            allSmokes[i] = currSmokeObj.GetComponent<ParticleSystem>();
                            currSmokeObj.transform.parent = transform;
                            allSmokes[i].Stop();

                        }
                    }
                }
            }
        }
        //Flip the car if  stuck
        void FlipCheck() {
            if (!myRigidBody && !canFlip)
                return;
            
            if (myRigidBody.velocity.magnitude <= 0.05f)
            {
                flipCounter += Time.fixedDeltaTime;
                if (flipCounter >= flipTimeOut) {
                    flipCounter = 0.0f;
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
        public float CurrentCalculatedSteer() {
            return currentSteer;
        }

        private void FixedUpdate()
        {
            if (!myRigidBody || !myAgentController)
                return;

            if (controlledByPlayer)
                PlayerControls();

            currentCenterOfMass = centerOfMass + new Vector3(0.0f, Mathf.Clamp01(myRigidBody.velocity.magnitude / (maxSpeed * 0.27f)) * centerOfMassAdditiveY, 0.0f);
            myRigidBody.centerOfMass = currentCenterOfMass;
            if (myRigidBody.velocity.y > 0)
            myRigidBody.AddForce(-Vector3.up * Mathf.Abs((myRigidBody.velocity.y * myRigidBody.mass) * pressureOnClimbs));
            int tempGroundedFlag = 0;
            float tempBrake = 0.0f;

            float angle = Vector3.SignedAngle(transform.forward, myAgentController.SteerVector().normalized, transform.up);
            float signedAngle = angle;
            float currMaxSpeed = maxSpeed;
            currentSteer = angle;

            float tempMaxSteer = maxSteerAngle;

            if (speedRelativeSteer)
                tempMaxSteer *= 1.0f - (myRigidBody.velocity.magnitude /( maxSpeed * 0.277778f));

            if (angle > tempMaxSteer)
                angle = tempMaxSteer;
            else if (angle < -tempMaxSteer)
                angle = -tempMaxSteer;

            if (Mathf.Abs(angle) < (maxSteerAngle * steerStabilityThreshold) && !controlledByPlayer)
                angle = 0.0f;

            if ((myAgentController.isReversing() && !controlledByPlayer) || (isPlayerReversing&&controlledByPlayer))
            {
                currMaxSpeed = maxReverseSpeed;
                if (flipSteerOnReverse)
                    angle *= -1.0f;
            }

            bool willScreech = false;
            //Controlling Powered Wheels
            for (int i = 0; i < wheels.Count; i++)
            {
                //Controlling the car
                if (((currentSpeed < currMaxSpeed && !limitSpeed && myAgentController.target) || (currentSpeed <= limitedSpeed && limitSpeed && currentSpeed < currMaxSpeed && myAgentController.target)) && engineOn)
                {
                    if (wheels[i].wheelCol && wheels[i].isAMotorWheel)
                    {
                        if (!controlledByPlayer)
                        {//Ai Controls
                            if (!myAgentController.isReversing())
                            {
                                if (wheels[i].wheelCol.motorTorque < 0.0f)
                                {
                                    wheels[i].wheelCol.motorTorque = 0.0f;
                                    wheels[i].wheelCol.brakeTorque = handBrakeTorque;
                                }
                                else
                                {
                                    wheels[i].wheelCol.brakeTorque = 0.0f;
                                }


                                if (Mathf.Abs(signedAngle) > (90.0f + ((1.0f - brakeSenstivity) * 90.0f)) && myAgentController.SomethingDetected() && !myAgentController.isReversing())
                                    tempBrake = 1.0f;
                                else
                                    tempBrake = 0.0f;


                                //Braking set and reset
                                if (!isBraking && tempBrake != 0)
                                {
                                    StopCoroutine(FadeBrakeLights());
                                    currentColor = brakeColor;
                                    StartCoroutine(FadeBrakeLights());
                                    isBraking = true;
                                }
                                else if (isBraking && tempBrake == 0)
                                {
                                    StopCoroutine(FadeBrakeLights());
                                    currentColor = idleColor;
                                    StartCoroutine(FadeBrakeLights());
                                    isBraking = false;
                                }

                                if (!isBraking)
                                {
                                    wheels[i].wheelCol.motorTorque = GetCurrentTorque(wheels[i].wheelCol.rpm);
                                    wheels[i].wheelCol.brakeTorque = 0.0f;
                                }
                                else
                                {
                                    wheels[i].wheelCol.motorTorque = 0.0f;
                                    wheels[i].wheelCol.brakeTorque = handBrakeTorque;
                                }
                            }
                            else
                            {
                                isBraking = true;


                                if (wheels[i].wheelCol.motorTorque > 0.0f)
                                {
                                    StopCoroutine(FadeBrakeLights());
                                    currentColor = brakeColor;
                                    StartCoroutine(FadeBrakeLights());
                                    wheels[i].wheelCol.brakeTorque = handBrakeTorque;
                                    wheels[i].wheelCol.motorTorque = 0.0f;
                                }
                                else
                                {

                                    if (gears.Length > 0)
                                    {
                                        if (Mathf.Abs(signedAngle) < (brakeSenstivity * 90.0f))
                                            tempBrake = 1.0f;
                                        else
                                            tempBrake = 0.0f;

                                        if (tempBrake != 0)
                                        {
                                            StopCoroutine(FadeBrakeLights());
                                            currentColor = brakeColor;
                                            StartCoroutine(FadeBrakeLights());
                                            wheels[i].wheelCol.brakeTorque = handBrakeTorque;
                                        }
                                        else if (tempBrake == 0)
                                        {
                                            StopCoroutine(FadeBrakeLights());
                                            currentColor = reverseColor;
                                            StartCoroutine(FadeBrakeLights());
                                            wheels[i].wheelCol.brakeTorque = 0.0f;
                                        }


                                        wheels[i].wheelCol.motorTorque = -(maxTorque * gears[0].maxTorqueMultiplier);
                                    }
                                }
                                
                            }
                        }
                        else {//Player controls
                            float yMove = inputAxis.y;
                            float currentTorque = 0.0f;
                            bool tempRev = false;
                            if (yMove > 0)
                            {
                                if (Mathf.Round(currentRPM) < 0)
                                {
                                    currentTorque = 0;
                                    tempBrake = 1;
                                }
                                else
                                {
                                    currentTorque = GetCurrentTorque(wheels[i].wheelCol.rpm) * yMove;
                                    tempBrake = 0;
                                    tempRev = false;

                                }
                            }
                            else if (yMove < 0)
                            {
                                if (Mathf.Round(currentRPM) > 0)
                                {
                                    currentTorque = 0;
                                    tempBrake = 1;
                                }
                                else
                                {
                                    if (gears.Length > 0)
                                    currentTorque = (gears[0].maxTorqueMultiplier * maxTorque) * yMove;
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
                                wheels[i].wheelCol.brakeTorque = handBrakeTorque;
                                tempBrake = 1;

                            }
                            else {
                                wheels[i].wheelCol.brakeTorque = 0;
                                tempBrake = 0;
                            }

                            //Applying torque here
                            wheels[i].wheelCol.motorTorque = currentTorque;


                            if (!isBraking && tempBrake != 0 && !isPlayerReversing)
                            {
                                StopCoroutine(FadeBrakeLights());
                                currentColor = brakeColor;
                                StartCoroutine(FadeBrakeLights());
                                isBraking = true;
                            }
                            else if (isBraking && tempBrake == 0 && !isPlayerReversing)
                            {
                                StopCoroutine(FadeBrakeLights());
                                currentColor = idleColor;
                                StartCoroutine(FadeBrakeLights());
                                isBraking = false;
                            }

                            if (!isPlayerReversing && tempRev) {
                                isPlayerReversing = true;
                                StopCoroutine(FadeBrakeLights());
                                currentColor = reverseColor;
                                StartCoroutine(FadeBrakeLights());
                            } else if (isPlayerReversing && !tempRev) {
                                isPlayerReversing = false;
                                StopCoroutine(FadeBrakeLights());
                                currentColor = idleColor;
                                StartCoroutine(FadeBrakeLights());
                            }

                        }

                    }

                }
                else {
                    if (wheels[i].wheelCol && wheels[i].isAMotorWheel) {
                        wheels[i].wheelCol.motorTorque = 0.0f;
                        wheels[i].wheelCol.brakeTorque = handBrakeTorque;
                    }
                }
                //Controlling steering wheels
                
                if (wheels[i].wheelCol && wheels[i].isASteeringWheel) {
                    if (!controlledByPlayer)
                        wheels[i].wheelCol.steerAngle = Mathf.LerpAngle(wheels[i].wheelCol.steerAngle, angle, Time.fixedDeltaTime * steerResponsiveness);
                    else
                        wheels[i].wheelCol.steerAngle = inputAxis.x * tempMaxSteer;

                }
                //Apply visuals of the wheels
                if (wheels[i].wheelCol && wheels[i].wheelTrans) {
                    ApplyLocalPositionToVisuals(wheels[i].wheelCol, wheels[i].wheelTrans);
                }

                SkidMark(wheels[i] , i,ref willScreech,ref tempGroundedFlag);
                
            }
            //Screeching sound
            if (willScreech && skidSource && !isScreeching && skidSound)
            {
                isScreeching = true;
                skidSource.PlayOneShot(skidSound);
                Invoke("StopScreech", skidSound.length);
                
            }
            //Ground Check
            if (tempGroundedFlag > 0)
                grounded = true;
            else
                grounded = false;
        }

        //Controls based on players Prefrence
        void PlayerControls() {
            switch (controlType) {
                case ControlTypes.Keyboard:
                inputAxis = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                break;
                case ControlTypes.UI_SteerWheel_BTNS: break;
                case ControlTypes.UI_Tap_and_Acc_Control: inputAxis = new Vector2(Mathf.Clamp(Input.acceleration.x, -1.0f, 1.0f), inputAxis.y); break;
            }
        }
        /// <summary>
        /// Set Y input
        /// </summary>
        /// <param name="to"></param>
        public void SetInputY (float to) {
            inputAxis = new Vector2(inputAxis.x, to);
        }

        /// <summary>
        /// Set X input
        /// </summary>
        /// <param name="to"></param>
        public void SetInputX(float to)
        {
            inputAxis = new Vector2(to,inputAxis.y);
        }


        /// <summary>
        /// Are we grounded?
        /// </summary>
        /// <returns></returns>
        public bool GetGround() {
            return grounded;
        }

        /// <summary>
        /// Set engine On or Off
        /// </summary>

        public void SetEngine( bool recVal) {
            engineOn = recVal;
        }

        //Stop screeching
        void StopScreech() {
            isScreeching = false;
        }
        //Create Skidmark for the passed wheel
        void SkidMark(WheelInfo currWheel , int index , ref bool willScreech , ref int tempGroundFlag) {
            WheelHit currHit;
            float realSlip = 0;
            GameObject newSkid;
            if (currWheel.wheelTrans && currWheel.wheelCol)
            {
                currWheel.wheelCol.GetGroundHit(out currHit);
                realSlip = Mathf.Abs(currHit.sidewaysSlip);
                if (currHit.collider && realSlip > slideAfterSlip)
                {
                    if (skidSmoke)
                    {
                        if (currWheel.wheelCol)
                            allSmokes[index].transform.position = currWheel.wheelCol.transform.position - currWheel.wheelCol.transform.up * currWheel.wheelCol.radius;
                        allSmokes[index].Play();
                    }
                    
                    willScreech = true;
                    

                    if (!currWheel.skidRenderer)
                    {
                        
                        //Create line renderer for skid mark
                        newSkid = new GameObject();
                        newSkid.name = "Skidmark";
                        if (!skidmarkKeeper)
                        {
                            skidmarkKeeper = new GameObject();
                            skidmarkKeeper.name = "SkidMarks-" + gameObject.name;
                        }
                        newSkid.transform.parent = skidmarkKeeper.transform;
                        wheels[index].skidRenderer = newSkid.AddComponent<LineRenderer>();
                        wheels[index].skidRenderer.startWidth = wheels[index].skidMarkWidth;
                        wheels[index].skidRenderer.endWidth = wheels[index].skidMarkWidth;
                        wheels[index].skidRenderer.useWorldSpace = true;
                        wheels[index].skidRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                        if (skidMaterial) {
                            wheels[index].skidRenderer.material = skidMaterial;
                        }
                        wheels[index].skidVertexHolder.Capacity = 10;
                        wheels[index].skidVertexHolder.Add(currHit.point - (currHit.point - currWheel.wheelCol.transform.position).normalized*0.001f);
                        wheels[index].skidRenderer.alignment = LineAlignment.TransformZ;
                        currWheel.skidRenderer.positionCount = wheels[index].skidVertexHolder.Count;
                        currWheel.skidRenderer.transform.localEulerAngles = new Vector3(90.0f, 0.0f, 0.0f);

                    }
                    else {
                    //Postion skid mark
                        if ((currWheel.skidVertexHolder[currWheel.skidVertexHolder.Count - 1] - currHit.point).magnitude >= skidVertexDistance) {

                            if (wheels[index].skidVertexHolder.Count >= maxSkidVerticies) {

                                if (wheels[index].skidRenderer)
                                {
                                    wheels[index].skidRenderer.gameObject.AddComponent<SkidmarkDestroyer>();
                                    wheels[index].skidRenderer = null;
                                }
                                if (wheels[index].skidVertexHolder.Count != 0)
                                    wheels[index].skidVertexHolder.Clear();
                                return;
                            }
                            currWheel.skidRenderer.positionCount = wheels[index].skidVertexHolder.Count;
                            wheels[index].skidVertexHolder.Add(currHit.point - (currHit.point - currWheel.wheelCol.transform.position).normalized * 0.001f);
                                currWheel.skidRenderer.SetPositions(wheels[index].skidVertexHolder.ToArray());
                        }
                    }
                }
                else {

                    if (skidSmoke)
                    {
                        allSmokes[index].Stop();
                    }
                    if (wheels[index].skidRenderer)
                    {
                        wheels[index].skidRenderer.gameObject.AddComponent<SkidmarkDestroyer>();
                        wheels[index].skidRenderer = null;
                    }
                    if (wheels[index].skidVertexHolder.Count != 0)
                        wheels[index].skidVertexHolder.Clear();
                }
                if (!currHit.collider)
                {
                    //if (!bike)
                        FlipCheck();
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
        float GetCurrentTorque(float RPM) {
            float newTorque = maxTorque;
            
            for (int i = 0; i < gears.Length; i++)
            {
                if (RPM < gears[i].tillWhatRPM) {
                    newTorque = maxTorque *  gears[i].maxTorqueMultiplier;
                    currentGear = i;
                    
                    break;
                }
            }

            return newTorque;
        }
        /// <summary>
        /// Returns car's current speed
        /// </summary>
        /// <returns></returns>
        public float GetCurrentSpeed() {
            return currentSpeed;
        }

        /// <summary>
        /// position the transform based on wheel colliders calculated positon
        /// </summary>
        /// <param name="col">passed wheel collider</param>
        /// <param name="visualTransform">passed wheel geomatry</param>
        public void ApplyLocalPositionToVisuals(WheelCollider col, Transform visualTransform)
        {
            Vector3 position;
            Quaternion rotation;
            col.GetWorldPose(out position, out rotation);

            visualTransform.transform.position = position;
            visualTransform.transform.rotation = rotation;
        }
        //Calculate speed and rpm
        void CalculateSpeed() {
            if (wheelsForSpeedCalculation.Length != 0)
            {
                float counted = 0;
                float tempRPM = 0.0f;
                for (int i = 0; i < wheelsForSpeedCalculation.Length; i++) {
                    if (wheelsForSpeedCalculation[i]) {
                        if (counted == 0)
                            wheelRadius = wheelsForSpeedCalculation[i].radius;

                        counted++;
                        tempRPM += wheelsForSpeedCalculation[i].rpm;
                    }
                }
                currentRPM = tempRPM / counted;
                currentSpeed = (((2.0f * Mathf.PI * wheelRadius) * Mathf.Abs(currentRPM) ) * 60.0f)/1000.0f;
                currentSpeed = Mathf.Abs(currentSpeed);
                currentSpeed = Mathf.Round(currentSpeed);
            }
            else {
                currentSpeed = myAgentController.GetVelocity().magnitude;
            }
        }
        //Control engine sound
        void EngineSound()
        {
                if (!mySource)
                {
                    return;
                }

                float tempVar = 0.0f;

                float minValue = 0;
                float maxValue = 0;
                if (currentGear < gears.Length)
                {
                    tempVar = (((2.0f * 3.14f * wheelRadius) * gears[currentGear].tillWhatRPM) * 60.0f) / 1000.0f;
                    tempVar = Mathf.Abs(tempVar);
                    tempVar = Mathf.Round(tempVar);
                    maxValue = tempVar;
                }
                tempVar = 0.0f;
                if ((currentGear - 1) >= 0 && (currentGear - 1) < gears.Length)
                {
                    tempVar = (((2.0f * 3.14f * wheelRadius) * gears[currentGear - 1].tillWhatRPM) * 60.0f) / 1000.0f;
                    tempVar = Mathf.Abs(tempVar);
                    tempVar = Mathf.Round(tempVar);

                    minValue = tempVar;
                }
                if (minValue == maxValue)
                    return;
                float enginePitch = Mathf.Lerp(mySource.pitch , ((currentSpeed - minValue) / (maxValue - minValue)) + 1.0f , Time.deltaTime * (( maxSpeed / currentSpeed) * 2.0f) );

            if (enginePitch < 0.85f)
                enginePitch = 0.85f;
            else
                enginePitch += 0.05f;
                mySource.pitch = Mathf.Clamp(Mathf.Abs(enginePitch) , -3.0f,3.0f);
                return;
            
        }
        //Fading if brake lights
        IEnumerator FadeBrakeLights() {

            if (!rearLights)
                yield return null;

            float lastTime = Time.time;
            float lightLerper = 0.0f;

            while (lightLerper <= 1.0f) {
                lightLerper += (Time.time - lastTime)*2.5f;
                if (rearLights)
                rearLights.material.SetColor("_EmissionColor", Color.Lerp(Color.black, currentColor, lightLerper));
                lastTime = Time.time;
                yield return null;
            }

            yield return null;
        }

        //Speed limiter enter
        private void OnTriggerStay(Collider other)
        {
            if (other.GetComponent<LimitedSpeedZone>()) {
                limitSpeed = true;
                limitedSpeed = other.GetComponent<LimitedSpeedZone>().maxSpeed;
            }
        }
        //Speed limiter exit
        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<LimitedSpeedZone>())
            {
                limitSpeed = false;
            }
        }
        //Sparks on collision
        void OnCollisionEnter(Collision collision)
        {
            if (!spark)
                return;

            ContactPoint[] contacts = collision.contacts;

            foreach (ContactPoint cp in contacts) {
                Instantiate(spark, cp.point, Quaternion.identity);
            }
            
        }
        /// <summary>
        /// Are we reversing?
        /// </summary>
        /// <returns></returns>
        public bool IsReversing() {
            if (controlledByPlayer)
                return isPlayerReversing;
            else
                return myAgentController.isReversing();
        }
        /// <summary>
        /// Get the speed value clamped between 0 and 1
        /// </summary>
        /// <returns></returns>
        public float GetClampedVelocity() {
            float newValue = Mathf.Clamp01(currentSpeed / maxSpeed);
            return newValue;
        }

        public WheelInfo[] GetTheWheels(){
            return wheels.ToArray();
        }
    }
}