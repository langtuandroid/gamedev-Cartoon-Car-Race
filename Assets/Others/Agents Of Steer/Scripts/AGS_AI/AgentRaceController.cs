using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace negleft.AGS{
    /// <summary>
    /// Calculates the best path for the agent to move on but dosen't apply
    /// any type of movement
    /// </summary>
    public class AgentRaceController : MonoBehaviour
    {
        /// <summary>
        /// Current Target
        /// </summary>
        [FormerlySerializedAs("target")] public Transform currTarget;
        /// <summary>
        ///This describes which behaviour will have more or less priority than other  
        /// </summary>
        [System.Serializable]
        public class WeightsInfoClass {
            /// <summary>
            /// Defines priority and power of persuit behaviour
            /// </summary>
            [FormerlySerializedAs("pursuit")] public float pursuitValue;
            /// <summary>
            /// Defines priority and power of obstacle avoidence behaviour
            /// </summary>
            [FormerlySerializedAs("avoidObstacle")] public float avoidObstacleValue;
            /// <summary>
            /// Defines priority and power of wall containment behaviour
            /// </summary>
            [FormerlySerializedAs("containment")] public float containmentValue;
            /// <summary>
            /// Defines priority and power of path following behaviour
            /// </summary>
            [FormerlySerializedAs("pathFollowing")] public float pathFollowingValue;
            /// <summary>
            /// Defines priority and power of Sepration behaviour
            /// </summary>
            [FormerlySerializedAs("seperation")] public float seperationValue;
            /// <summary>
            /// Defines priority and power of Unalligned Collision Avoidence behaviour
            /// </summary>
            [FormerlySerializedAs("unallignedCollisionAvoidence")] public float unallignedCollisionAvoidenceValue;
            /// <summary>
            /// Defines priority and power of Queuing behaviour
            /// </summary>
            [FormerlySerializedAs("queuing")] public float queuingValue; 
            WeightsInfoClass() {
                pursuitValue = 1.0f;
                avoidObstacleValue = 1.0f;
                containmentValue = 1.0f;
                pathFollowingValue = 3.0f;
                seperationValue = 10.0f;
                unallignedCollisionAvoidenceValue = 3.0f;
                queuingValue = 2.0f;
            }
        }
        /// <summary>
        /// Object of weights class
        /// </summary>
        [FormerlySerializedAs("weights")] public WeightsInfoClass weightsInfo;

        /// <summary>
        /// This class holds strength variables for every Behaviour
        /// </summary>
        [System.Serializable]
        public class StrengthInfoClass
        {
            /// <summary>
            ///Pursuit look ahead
            /// </summary>
            [FormerlySerializedAs("pursuitAhead")] public float pursuitAheadValue;
            /// <summary>
            ///Queuing if ahead 
            /// </summary>
            [FormerlySerializedAs("queuingAhead")] public float queuingAheadValue;
            /// <summary>
            /// Speration Distance
            /// </summary>
            [FormerlySerializedAs("seperationDistance")] public float seperationDistanceValue;
            /// <summary>
            /// UnalignedAvoidence how much furter in future
            /// </summary>
            [FormerlySerializedAs("unalignedAvoidenceAhead")] public float unalignedAvoidenceAheadValue;
            /// <summary>
            /// Avoidence for containment behaviours
            /// </summary>
            [FormerlySerializedAs("containmentAhead")] public float containmentAheadValue;
            /// <summary>
            /// Pridict path ahead
            /// </summary>
            [FormerlySerializedAs("pridictPath")] public float pridictPathValue = 1.0f;
            StrengthInfoClass()
            {
                pursuitAheadValue = 10.0f;
                seperationDistanceValue = 8.0f;
                queuingAheadValue = 0.1f;
                unalignedAvoidenceAheadValue = 1.0f;
                containmentAheadValue = 10.0f;
            }
        }
        /// <summary>
        /// An object of strength class
        /// </summary>
        [FormerlySerializedAs("strengths")] public StrengthInfoClass strengthsInfo;


        /// <summary>
        /// This class holds the variables that either limit or influence the vehicle behaviour for every Behaviour
        /// </summary>
        [System.Serializable]
        public class LimitAndInfluenceInfo
        {
            /// <summary>
            /// Minimum distance to start overtaking another agent
            /// </summary>
            [FormerlySerializedAs("maximumDistanceForPursuit")] public float maximumDistanceForPursuitValue = 30.0f;
            /// <summary>
            /// Maximum Distance to enable the containment behaviour 
            /// </summary>
            [FormerlySerializedAs("maximumContainmentDistance")] public float maximumContainmentDistanceValue = 25.0f;
            /// <summary>
            /// Maximum Distance to enable the avoidence behaviour
            /// </summary>
            [FormerlySerializedAs("maximumObstacleAvoidenceDistance")] public float maximumObstacleAvoidenceDistanceValue = 20.0f;
            /// <summary>
            /// Minimum Detection length for every sensor (each of sensor can be scaled to minimize or maximize the minimum detection length)
            /// </summary>
            [FormerlySerializedAs("detectionLength")] public float detectionLengthValue = 5.0f;
            /// <summary>
            /// how much influence behaviours have on the agent
            /// </summary>
            [FormerlySerializedAs("avoidForce")] public float avoidForceValue = 1.0f;
            /// <summary>
            /// How long ahead can the agent see based on velocity
            /// </summary>
            [FormerlySerializedAs("velocitySensorMultiplier")] public float velocitySensorMultiplierValue = 0.25f;

            LimitAndInfluenceInfo()
            {
            
            }
        }
        /// <summary>
        /// An object of LimitAndInfluence class
        /// </summary>
        [FormerlySerializedAs("limitersAndInfluencers")] public LimitAndInfluenceInfo limitersAndInfluencersValue;


        /// <summary>
        /// This class holds Reversing variables
        /// </summary>
        [System.Serializable]
        public class ReversingVariablesInfo
        {
            /// <summary>
            /// can use reversing logic
            /// </summary>
            [FormerlySerializedAs("canReverse")] public bool canReverseFlag = false;
            /// <summary>
            /// Minimum velocity ti check if we are really stuck
            /// </summary>
            [FormerlySerializedAs("minVelocityForReverseTimeOut")] public float minVelocityForReverseTimeOutValue = 0.25f;
            [FormerlySerializedAs("reverseTimeOutCounter")] [HideInInspector]
            public float reverseTimeOutCounterValue = 0.0f;
            /// <summary>
            /// Reverse in "amount of time"
            /// </summary>
            [FormerlySerializedAs("reverseTimeOutIn")] public float reverseTimeOutInValue = .25f;
            /// <summary>
            /// Sensor detection while reversing
            /// </summary>
            [FormerlySerializedAs("reversingSensorMultiplier")] public float reversingSensorMultiplierValue = .35f;
            [FormerlySerializedAs("reversing")] [HideInInspector]
            public bool reversingFlag = false;

            ReversingVariablesInfo()
            {

            }
        }
        /// <summary>
        /// An object of ReversingVariables class
        /// </summary>
        [FormerlySerializedAs("reverseControl")] public ReversingVariablesInfo reverseControlInfo;


        private Vector3 lastMagnitude = Vector3.zero;
        /// <summary>
        /// Avoidence and Pursuit Sensor made up of empty Transforms that can be scaled
        /// on forward axis to adjust the senstivity
        /// </summary>
        [FormerlySerializedAs("sensors")] public Transform[] sensorsTransforms;
        private Vector3 steerVectorValue;



        private bool didHitFlag = false;
        private Vector3 lastPosVector;

        private Vector3 velocityVector;
        
        
        /// <summary>
        /// Define wall which the agent will use containment logic on
        /// </summary>
        [FormerlySerializedAs("whatIsaWall")] public LayerMask whatIsaWallMask;
        /// <summary>
        /// Uncheck The Floor
        /// </summary>
        [FormerlySerializedAs("useSensorsOn")] public LayerMask useSensorsOnMask;
        /// <summary>
        /// Show debug rays
        /// </summary>
        [FormerlySerializedAs("showDebugRays")] public bool showDebugRaysFlag = true;
        private Vector3 pursuitVelocityVector = Vector3.zero;
        private AgentRaceController[] allAgentsControllers;
        private float desiredCurrSeprationValue = 0.0f;
        //Is this guy is police
        private bool policeFlag = false;
        private bool isPolicingFlag = false;
        private bool initializedFlag = false;
        // Use this for initialization
        private void Start()
        {
            StartCoroutine(GetInitializedCoroutine());
        }
        /// <summary>
        /// initializing the Agent
        /// </summary>
        /// <returns></returns>
        private IEnumerator GetInitializedCoroutine() {
            while (!initializedFlag) {

                if (currTarget) {
                    if (transform.GetComponent<AgentRacePolice>())
                        policeFlag = true;
                    if (!policeFlag)
                        allAgentsControllers = GameObject.FindObjectsOfType<AgentRaceController>();
                    else
                        allAgentsControllers = OnlyPoliceAgents();
                    lastPosVector = transform.position;
                    initializedFlag = true;
                }

                yield return null;
            }
            yield return null;

        }


        /// <summary>
        /// Are we on a police job?
        /// </summary>
        /// <param name="status"></param>
        public void SetPolicingStatus(bool status) {
            isPolicingFlag = status;
        }

        /// <summary>
        /// return true is we are policing
        /// </summary>
        /// <returns></returns>
        public bool GetPolicingStatus() {
            return isPolicingFlag;
        }

        /// <summary>
        /// is This Agent a police
        /// </summary>
        /// <returns></returns>
        public bool IsAPoliceStatus() {
            return policeFlag;
        }

        //Only include police Agents
        private AgentRaceController[] OnlyPoliceAgents() {
            List<AgentRaceController> currentAgents = new List<AgentRaceController>(GameObject.FindObjectsOfType<AgentRaceController>());

            for (int i = 0; i < currentAgents.Count; i++) {
                if (!currentAgents[i].IsAPoliceStatus())
                    currentAgents.RemoveAt(i);
            }

            return currentAgents.ToArray();

        }

        // Update is called once per frame
        private void Update()
        {

            if (!currTarget || !initializedFlag)
            {
                return;
            }

            desiredCurrSeprationValue = strengthsInfo.seperationDistanceValue;

            if (isReversingFlag())
                desiredCurrSeprationValue *= reverseControlInfo.reversingSensorMultiplierValue;
            else
                desiredCurrSeprationValue *=limitersAndInfluencersValue.velocitySensorMultiplierValue;


            CalculateVelocityVector();
            GetSteerVector();
            
        }
        //Calculating velocity here
        private void CalculateVelocityVector()
        {
            velocityVector = (transform.position - lastPosVector) / Time.deltaTime;
            lastPosVector = transform.position;
    
        }
        /// <summary>
        /// Returns velocity based on transforms position
        /// </summary>
        /// <returns></returns>
        public Vector3 GetVelocityVector() {

            return velocityVector;
        }

        //Calculating the steer vector
        private void GetSteerVector()
        {
            if (currTarget)
            {
                steerVectorValue = ((currTarget.position - transform.position).normalized) * weightsInfo.pathFollowingValue;
            }
            else {
                steerVectorValue = transform.forward;
            }
            //Calling steeringBehaviours
            steerVectorValue += SteeringBehavioursVector();

            steerVectorValue *= limitersAndInfluencersValue.avoidForceValue;
            if (showDebugRaysFlag)
                Debug.DrawRay(transform.position, steerVectorValue.normalized * 5.0f);
            
        }
        /// <summary>
        /// Returns current steer vector
        /// </summary>
        /// <returns></returns>
        public Vector3 SteerVectorValue() {
            return steerVectorValue;
        }
        //All the gizmos on the car
        private void OnDrawGizmos()
        {
            if (strengthsInfo == null || limitersAndInfluencersValue == null)
                return;
            
            float desiredCurrSeprationGiz = strengthsInfo.seperationDistanceValue;

            if (isReversingFlag())
                desiredCurrSeprationGiz *= reverseControlInfo.reversingSensorMultiplierValue;
            else
                desiredCurrSeprationGiz *= limitersAndInfluencersValue.velocitySensorMultiplierValue;

            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, desiredCurrSeprationGiz);

            Gizmos.color = Color.red;
            if (sensorsTransforms == null) {
                sensorsTransforms = new Transform[0];
            }
            if (sensorsTransforms.Length <= 0)
                return;

            for (int i = 0; i < sensorsTransforms.Length; i++)
                {
                    if (sensorsTransforms[i])
                    {
                        Gizmos.DrawRay(sensorsTransforms[i].position, sensorsTransforms[i].forward * (limitersAndInfluencersValue.detectionLengthValue * sensorsTransforms[i].localScale.z));
                    }
                }
            if (currTarget) {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(currTarget.position, 1.0f);
                Gizmos.color = Color.red;
            }


        }
        //Combination of steering behaviours
        private Vector3 SteeringBehavioursVector()
        {
            Vector3 avoidDir = Vector3.zero;
            Vector3 finalAvoidDir = Vector3.zero;
            Vector3 dirVector;
            finalAvoidDir = Vector3.zero;
            lastMagnitude = (transform.forward + steerVectorValue).normalized;
            bool overrideDidHit = false;
            float rayMag = 0.0f;
            float obsVelocity = 0.0f;
            Vector3 persuitVector = Vector3.zero;
            RaycastHit hit3D = new RaycastHit();
            float hit3DDistance = 0.0f;
            float lastAngle = 360.0f;
            int tempBehavValue = 0;
            int lastExecutedBehav = 0;
            //Calculate avoidence and persuit
            for (int i = 0; i < sensorsTransforms.Length; i++)
            {
                if (sensorsTransforms[i])
                {
                    dirVector = sensorsTransforms[i].forward;

                    rayMag = (limitersAndInfluencersValue.detectionLengthValue +(velocityVector.magnitude * limitersAndInfluencersValue.velocitySensorMultiplierValue)) * sensorsTransforms[i].localScale.z; ;
                    
                    if (reverseControlInfo.reversingFlag)
                        rayMag = (limitersAndInfluencersValue.detectionLengthValue + (velocityVector.magnitude * reverseControlInfo.reversingSensorMultiplierValue)) * sensorsTransforms[i].localScale.z;

                        bool gotHit = Physics.Raycast(new Ray(sensorsTransforms[i].position, dirVector), out hit3D, rayMag,useSensorsOnMask);

                    if (gotHit)
                    {
                        overrideDidHit = true;

                        avoidDir = Vector3.zero;

                        obsVelocity = 0.0f;

                        pursuitVelocityVector = Vector3.zero;
                            
                        hit3DDistance = hit3D.distance;

                        if (hit3D.collider.GetComponentInParent<Rigidbody>())
                        {
                            obsVelocity = Vector3.Dot(transform.position - hit3D.transform.position, GetVelocityVector() - hit3D.collider.GetComponentInParent<Rigidbody>().velocity);
                            pursuitVelocityVector = hit3D.collider.GetComponentInParent<Rigidbody>().velocity;
                        
                        }
                        

                        //Containment
                        if ((whatIsaWallMask & 1 << hit3D.collider.gameObject.layer) == 1 << hit3D.collider.gameObject.layer)
                        {
                            if (hit3DDistance <= limitersAndInfluencersValue.maximumContainmentDistanceValue)
                            {
                                avoidDir = hit3D.normal * strengthsInfo.containmentAheadValue;
                                if (showDebugRaysFlag)
                                    Debug.DrawRay(hit3D.point, avoidDir, Color.cyan);
                                tempBehavValue = 3;//Containment Executed
                            }

                        }
                        else if (obsVelocity <= 0.0f && pursuitVelocityVector == Vector3.zero && !hit3D.transform.GetComponentInParent<AgentRaceController>() && hit3DDistance <= limitersAndInfluencersValue.maximumObstacleAvoidenceDistanceValue)
                        {
                            //Avoid Obstacle
                            avoidDir = (new Vector3(hit3D.point.x, transform.position.y, hit3D.point.z) - hit3D.collider.bounds.center);
                            if (showDebugRaysFlag)
                                Debug.DrawRay(hit3D.point, avoidDir, Color.blue);
                            tempBehavValue = 2;//Obstacle Avoidence executed
                        }
                            
                        
                        //Pursuit
                        if (obsVelocity > 0.0f)
                        {

                            if (hit3DDistance <= limitersAndInfluencersValue.maximumDistanceForPursuitValue)
                            {
                                persuitVector = pursuitVelocityVector.normalized * (strengthsInfo.pursuitAheadValue+pursuitVelocityVector.magnitude);
                                if (showDebugRaysFlag)
                                    Debug.DrawRay(hit3D.point, persuitVector, Color.green);
                                avoidDir = persuitVector;
                                tempBehavValue = 1;//Pursuit Executed
                            }

                        }
                        /* 

                        if (lastMag.magnitude < avoidDir.magnitude)
                        {
                            lastMag = avoidDir;
                        }
                        */

                        float currAngle = Mathf.Abs(Vector3.SignedAngle(transform.forward, sensorsTransforms[i].forward , transform.up));
                        //Select most threatning and mostpowerful
                        
                        if (currAngle < lastAngle && tempBehavValue > lastExecutedBehav)
                        {
                            lastAngle = currAngle;
                            lastMagnitude = transform.forward + avoidDir;
                            lastExecutedBehav = tempBehavValue;
                        }
                        

                        if (showDebugRaysFlag)
                            Debug.DrawLine(sensorsTransforms[i].position, hit3D.point, Color.yellow);
                        
                    }
                }
            }

            switch (lastExecutedBehav) {
                case 1: lastMagnitude = lastMagnitude.normalized * weightsInfo.pursuitValue; break;
                case 2: lastMagnitude = lastMagnitude.normalized * weightsInfo.avoidObstacleValue; break;
                case 3: lastMagnitude = lastMagnitude.normalized * weightsInfo.containmentValue; break;
            }

            //Seperation
            Vector3 seperation = SeperationLogicVector(allAgentsControllers).normalized * weightsInfo.seperationValue;
            lastMagnitude += seperation;

            //UnAllignedCollisionAcoidence
            Vector3 unallignedAvoidenceLocal = Vector3.zero;
            if (seperation == Vector3.zero)
            {
                unallignedAvoidenceLocal = UnAllignedCollisionAvoidenceVector(allAgentsControllers).normalized * weightsInfo.unallignedCollisionAvoidenceValue;
                lastMagnitude += unallignedAvoidenceLocal;
            }

            if (seperation == Vector3.zero)
            {
                //Queuing
                Vector3 currQueue = CheckForQueuingVector().normalized * weightsInfo.queuingValue;
                lastMagnitude += currQueue;
            }

            finalAvoidDir = Vector3.zero;
            finalAvoidDir = lastMagnitude;

            if (lastMagnitude == Vector3.zero && !overrideDidHit)
                didHitFlag = false;
            else
                didHitFlag = true;
            //Reverse logic
            if (didHitFlag && velocityVector.magnitude <= reverseControlInfo.minVelocityForReverseTimeOutValue && reverseControlInfo.canReverseFlag)
            {
                reverseControlInfo.reverseTimeOutCounterValue += Time.deltaTime;
                if (reverseControlInfo.reverseTimeOutCounterValue >= reverseControlInfo.reverseTimeOutInValue)
                {
                    reverseControlInfo.reverseTimeOutCounterValue = 0.0f;
                    reverseControlInfo.reversingFlag = !reverseControlInfo.reversingFlag;
                }
            }
            else {
                reverseControlInfo.reverseTimeOutCounterValue = 0.0f;
            }

            if (!didHitFlag && reverseControlInfo.reversingFlag)
                reverseControlInfo.reversingFlag = false;
            
            return finalAvoidDir;
        }
        //Seperation logic based on desiredSeperation distance
        private Vector3 SeperationLogicVector(AgentRaceController[] allActiveAgents) {
            Vector3 calculatedSeparation = Vector3.zero;
            Vector3 dist = Vector3.zero;
            float count = 0.0f;
            float desiredCurrSepration = strengthsInfo.seperationDistanceValue;

            if (isReversingFlag())
                desiredCurrSepration *= reverseControlInfo.reversingSensorMultiplierValue;
            else
                desiredCurrSepration *= limitersAndInfluencersValue.velocitySensorMultiplierValue;

            foreach (AgentRaceController activeAgent in allActiveAgents) {
                if (activeAgent != this) {
                    dist = transform.position - activeAgent.transform.position;
                    if (dist.magnitude <= (desiredCurrSepration+(activeAgent.strengthsInfo.seperationDistanceValue*activeAgent.limitersAndInfluencersValue.velocitySensorMultiplierValue))) {
                        dist.Normalize();
                        calculatedSeparation += dist;
                        count++;
                    }
                }
            }

            if (count > 0)
                calculatedSeparation /= count;
            return calculatedSeparation;
        }


        /// <summary>
        /// Set the new target for the agent passed as newTar
        /// </summary>
        /// <param name="newTar"></param>
        public void SetNewTargetTransform(Transform newTar) {
            currTarget = newTar;
        }
        /// <summary>
        /// Are we reversing?
        /// </summary>
        /// <returns></returns>
        public bool isReversingFlag() {
            if (reverseControlInfo != null)
                return reverseControlInfo.reversingFlag;
            else
                return false;
        }
        /// <summary>
        /// Is there something that is blocking the path? includes all the behaviours
        /// </summary>
        /// <returns></returns>
        public bool SomethingBlockingDetected() {
            return didHitFlag;
        }

        /// <summary>
        /// Unalligned Collision avoidence from other agents
        /// </summary>
        /// <param name="allAgents">all the agents</param>
        /// <returns></returns>
        private Vector3 UnAllignedCollisionAvoidenceVector(AgentRaceController[] allAgents) {

            float minTime = strengthsInfo.unalignedAvoidenceAheadValue;
            
            float steer = 0.0f;
            Vector3 xThreatPosNearestApproach = Vector3.zero;
            Vector3 xMyPosAtnearestApproach = Vector3.zero;
            AgentRaceController currThreat = null;
            //Calculate approaches
            foreach (AgentRaceController currAgent in allAgents) {
                if (currAgent != this) {
                
                    float collisionThreshold = desiredCurrSeprationValue + (currAgent.strengthsInfo.seperationDistanceValue * currAgent.limitersAndInfluencersValue.velocitySensorMultiplierValue);
                    float nearestApproachTime = GetNearestApproachValue(currAgent);
                    if ((nearestApproachTime >= 0) && (nearestApproachTime < minTime))
                    {
                        Vector3 ourPos = Vector3.zero;
                        Vector3 threatPos = Vector3.zero;
                        float currDist = GetNearestPositionsDistance(currAgent, nearestApproachTime , ref ourPos , ref threatPos);

                        if (currDist < collisionThreshold)
                        {
                            minTime = nearestApproachTime;
                            currThreat = currAgent;
                            xThreatPosNearestApproach = threatPos;
                            xMyPosAtnearestApproach = ourPos;
                        }
                    }
                    
                }
            }
            //Is there any threat
            if (currThreat != null)
            {
                float parallelness = Vector3.Dot(transform.forward, currThreat.transform.forward);
                float angle = 0.707f;

                if (parallelness < -angle)
                {
                    Vector3 offset = xThreatPosNearestApproach - transform.position;
                    float sideDot = Vector3.Dot(offset, transform.right);
                    steer = (sideDot > 0.0f) ? -1.0f : 1.0f;
                }
                else if(parallelness > angle)
                    {
                    Vector3 offset = currThreat.transform.position - transform.position;
                    float sideDot = Vector3.Dot(offset, transform.right);
                    steer = (sideDot > 0) ? -1.0f : 1.0f;
                }
                else
                {
                    if (GetVelocityVector().magnitude <= currThreat.GetVelocityVector().magnitude || currThreat.GetVelocityVector().magnitude == 0 || gameObject.GetInstanceID() < currThreat.gameObject.GetInstanceID())
                    {
                        float sideDot = Vector3.Dot(transform.right, currThreat.GetVelocityVector());
                        steer = (sideDot > 0.0f) ? -1.0f : 1.0f;
                    }
                }
                steer *= strengthsInfo.seperationDistanceValue + currThreat.strengthsInfo.seperationDistanceValue * currThreat.limitersAndInfluencersValue.velocitySensorMultiplierValue;
            }


            if (showDebugRaysFlag)
                Debug.DrawLine(xMyPosAtnearestApproach, xThreatPosNearestApproach, Color.magenta);

            return (transform.right * steer);
        }
        //Calculating nearest approach
        private float GetNearestApproachValue(AgentRaceController currAgent) {

            Vector3 myVelocity = GetVelocityVector();
            Vector3 currAgentVelocity = currAgent.GetVelocityVector();
            Vector3 relVelocity = currAgentVelocity - myVelocity;
            float relSpeed = relVelocity.magnitude;

            if (relSpeed == 0) return 0.0f;

            Vector3 relTangent = relVelocity / relSpeed;

            Vector3 relPosition = transform.position - currAgent.transform.position;

            float projection = Vector3.Dot(relTangent, relPosition);

            return (projection/relSpeed);
        }
        //Calculate nearest positions
        private float GetNearestPositionsDistance(AgentRaceController currAgent, float approachTime , ref Vector3 ourPos , ref Vector3 threatPos) {
            Vector3 myTravel = GetVelocityVector() * approachTime;
    
            Vector3 currAgentTravel = currAgent.GetVelocityVector() * approachTime;

            Vector3 myFinal = transform.position + myTravel;
            Vector3 currAgentFinal = currAgent.transform.position + currAgentTravel;

            ourPos = myFinal;
            threatPos = currAgentFinal;

            return ((myFinal-currAgentFinal).magnitude);
        }
        //Queuing logic for trailing behaviour
        private Vector3 CheckForQueuingVector() {
            bool threatning = GetIfNeighbourAhead(allAgentsControllers);
            if (threatning)
            {
                return -transform.forward;
            }
            return Vector3.zero;
        }

        /// <summary>
        /// Get the neighbour ahead
        /// </summary>
        /// <param name="allActiveAgents">all the agents</param>
        /// <returns></returns>
        private bool GetIfNeighbourAhead(AgentRaceController[] allActiveAgents) {
            Vector3 myFuturePos = transform.position + (GetVelocityVector() * strengthsInfo.queuingAheadValue);

            AgentRaceController curreThreat = null;
            
            foreach (AgentRaceController currAgent in allActiveAgents) {
                if (currAgent != this) {
                    if ((currAgent.transform.position - myFuturePos).magnitude <= (currAgent.strengthsInfo.seperationDistanceValue * currAgent.limitersAndInfluencersValue.velocitySensorMultiplierValue + desiredCurrSeprationValue)) {
                        curreThreat = currAgent;
                        break;
                    }
                }
            }

            if (curreThreat)
            {
                if (showDebugRaysFlag)
                    Debug.DrawLine(transform.position, curreThreat.transform.position, Color.red);
                return true;
            }
            else
                return false;
        }
    }
}