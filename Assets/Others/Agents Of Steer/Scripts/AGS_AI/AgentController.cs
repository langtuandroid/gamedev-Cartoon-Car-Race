using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace negleft.AGS{
    /// <summary>
    /// Calculates the best path for the agent to move on but dosen't apply
    /// any type of movement
    /// </summary>
    public class AgentController : MonoBehaviour
    {
        /// <summary>
        /// Current Target
        /// </summary>
        public Transform target;
        /// <summary>
        ///This describes which behaviour will have more or less priority than other  
        /// </summary>
        [System.Serializable]
        public class WeightsClass {
            /// <summary>
            /// Defines priority and power of persuit behaviour
            /// </summary>
            public float pursuit;
            /// <summary>
            /// Defines priority and power of obstacle avoidence behaviour
            /// </summary>
            public float avoidObstacle;
            /// <summary>
            /// Defines priority and power of wall containment behaviour
            /// </summary>
            public float containment;
            /// <summary>
            /// Defines priority and power of path following behaviour
            /// </summary>
            public float pathFollowing;
            /// <summary>
            /// Defines priority and power of Sepration behaviour
            /// </summary>
            public float seperation;
            /// <summary>
            /// Defines priority and power of Unalligned Collision Avoidence behaviour
            /// </summary>
            public float unallignedCollisionAvoidence;
            /// <summary>
            /// Defines priority and power of Queuing behaviour
            /// </summary>
            public float queuing; 
            WeightsClass() {
                pursuit = 1.0f;
                avoidObstacle = 1.0f;
                containment = 1.0f;
                pathFollowing = 3.0f;
                seperation = 10.0f;
                unallignedCollisionAvoidence = 3.0f;
                queuing = 2.0f;
            }
        }
        /// <summary>
        /// Object of weights class
        /// </summary>
        public WeightsClass weights;

        /// <summary>
        /// This class holds strength variables for every Behaviour
        /// </summary>
        [System.Serializable]
        public class StrengthClass
        {
            /// <summary>
            ///Pursuit look ahead
            /// </summary>
            public float pursuitAhead;
            /// <summary>
            ///Queuing if ahead 
            /// </summary>
            public float queuingAhead;
            /// <summary>
            /// Speration Distance
            /// </summary>
            public float seperationDistance;
            /// <summary>
            /// UnalignedAvoidence how much furter in future
            /// </summary>
            public float unalignedAvoidenceAhead;
            /// <summary>
            /// Avoidence for containment behaviours
            /// </summary>
            public float containmentAhead;
            /// <summary>
            /// Pridict path ahead
            /// </summary>
            public float pridictPath = 1.0f;
            StrengthClass()
            {
                pursuitAhead = 10.0f;
                seperationDistance = 8.0f;
                queuingAhead = 0.1f;
                unalignedAvoidenceAhead = 1.0f;
                containmentAhead = 10.0f;
            }
        }
        /// <summary>
        /// An object of strength class
        /// </summary>
        public StrengthClass strengths;


        /// <summary>
        /// This class holds the variables that either limit or influence the vehicle behaviour for every Behaviour
        /// </summary>
        [System.Serializable]
        public class LimitAndInfluence
        {
            /// <summary>
            /// Minimum distance to start overtaking another agent
            /// </summary>
            public float maximumDistanceForPursuit = 30.0f;
            /// <summary>
            /// Maximum Distance to enable the containment behaviour 
            /// </summary>
            public float maximumContainmentDistance = 25.0f;
            /// <summary>
            /// Maximum Distance to enable the avoidence behaviour
            /// </summary>
            public float maximumObstacleAvoidenceDistance = 20.0f;
            /// <summary>
            /// Minimum Detection length for every sensor (each of sensor can be scaled to minimize or maximize the minimum detection length)
            /// </summary>
            public float detectionLength = 5.0f;
            /// <summary>
            /// how much influence behaviours have on the agent
            /// </summary>
            public float avoidForce = 1.0f;
            /// <summary>
            /// How long ahead can the agent see based on velocity
            /// </summary>
            public float velocitySensorMultiplier = 0.25f;

            LimitAndInfluence()
            {
            
            }
        }
        /// <summary>
        /// An object of LimitAndInfluence class
        /// </summary>
        public LimitAndInfluence limitersAndInfluencers;


        /// <summary>
        /// This class holds Reversing variables
        /// </summary>
        [System.Serializable]
        public class ReversingVariables
        {
            /// <summary>
            /// can use reversing logic
            /// </summary>
            public bool canReverse = false;
            /// <summary>
            /// Minimum velocity ti check if we are really stuck
            /// </summary>
            public float minVelocityForReverseTimeOut = 0.25f;
            [HideInInspector]
            public float reverseTimeOutCounter = 0.0f;
            /// <summary>
            /// Reverse in "amount of time"
            /// </summary>
            public float reverseTimeOutIn = .25f;
            /// <summary>
            /// Sensor detection while reversing
            /// </summary>
            public float reversingSensorMultiplier = .35f;
            [HideInInspector]
            public bool reversing = false;

            ReversingVariables()
            {

            }
        }
        /// <summary>
        /// An object of ReversingVariables class
        /// </summary>
        public ReversingVariables reverseControl;


        Vector3 lastMag = Vector3.zero;
        /// <summary>
        /// Avoidence and Pursuit Sensor made up of empty Transforms that can be scaled
        /// on forward axis to adjust the senstivity
        /// </summary>
        public Transform[] sensors;
        Vector3 steerVector;



        bool didHit = false;
        Vector3 lastPos;

        Vector3 velocity;
        
        
        /// <summary>
        /// Define wall which the agent will use containment logic on
        /// </summary>
        public LayerMask whatIsaWall;
        /// <summary>
        /// Uncheck The Floor
        /// </summary>
        public LayerMask useSensorsOn;
        /// <summary>
        /// Show debug rays
        /// </summary>
        public bool showDebugRays = true;
        Vector3 pursuitVelocity = Vector3.zero;
        AgentController[] allAgents;
        float desiredCurrSepration = 0.0f;
        //Is this guy is police
        bool police = false;
        bool isPolicing = false;
        bool initialized = false;
        // Use this for initialization
        void Start()
        {
            StartCoroutine(GetInitialized());
        }
        /// <summary>
        /// initializing the Agent
        /// </summary>
        /// <returns></returns>
        IEnumerator GetInitialized() {
            while (!initialized) {

                if (target) {
                    if (transform.GetComponent<AgentPolice>())
                        police = true;
                    if (!police)
                        allAgents = GameObject.FindObjectsOfType<AgentController>();
                    else
                        allAgents = OnlyPolice();
                    lastPos = transform.position;
                    initialized = true;
                }

                yield return null;
            }
            yield return null;

        }


        /// <summary>
        /// Are we on a police job?
        /// </summary>
        /// <param name="status"></param>
        public void SetPolicing(bool status) {
            isPolicing = status;
        }

        /// <summary>
        /// return true is we are policing
        /// </summary>
        /// <returns></returns>
        public bool GetPolicing() {
            return isPolicing;
        }

        /// <summary>
        /// is This Agent a police
        /// </summary>
        /// <returns></returns>
        public bool IsAPolice() {
            return police;
        }

        //Only include police Agents
        AgentController[] OnlyPolice() {
            List<AgentController> currentAgents = new List<AgentController>(GameObject.FindObjectsOfType<AgentController>());

            for (int i = 0; i < currentAgents.Count; i++) {
                if (!currentAgents[i].IsAPolice())
                    currentAgents.RemoveAt(i);
            }

            return currentAgents.ToArray();

        }

        // Update is called once per frame
        void Update()
        {

            if (!target || !initialized)
            {
                return;
            }

            desiredCurrSepration = strengths.seperationDistance;

            if (isReversing())
                desiredCurrSepration *= reverseControl.reversingSensorMultiplier;
            else
                desiredCurrSepration *=limitersAndInfluencers.velocitySensorMultiplier;


            CalculateVelocity();
            GetSteer();
            
        }
        //Calculating velocity here
        void CalculateVelocity()
        {
            velocity = (transform.position - lastPos) / Time.deltaTime;
            lastPos = transform.position;
    
        }
        /// <summary>
        /// Returns velocity based on transforms position
        /// </summary>
        /// <returns></returns>
        public Vector3 GetVelocity() {

            return velocity;
        }

        //Calculating the steer vector
        void GetSteer()
        {
            if (target)
            {
                steerVector = ((target.position - transform.position).normalized) * weights.pathFollowing;
            }
            else {
                steerVector = transform.forward;
            }
            //Calling steeringBehaviours
            steerVector += SteeringBehaviours();

            steerVector *= limitersAndInfluencers.avoidForce;
            if (showDebugRays)
                Debug.DrawRay(transform.position, steerVector.normalized * 5.0f);
            
        }
        /// <summary>
        /// Returns current steer vector
        /// </summary>
        /// <returns></returns>
        public Vector3 SteerVector() {
            return steerVector;
        }
        //All the gizmos on the car
        private void OnDrawGizmos()
        {
            if (strengths == null || limitersAndInfluencers == null)
                return;
            
            float desiredCurrSeprationGiz = strengths.seperationDistance;

            if (isReversing())
                desiredCurrSeprationGiz *= reverseControl.reversingSensorMultiplier;
            else
                desiredCurrSeprationGiz *= limitersAndInfluencers.velocitySensorMultiplier;

            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, desiredCurrSeprationGiz);

            Gizmos.color = Color.red;
            if (sensors == null) {
                sensors = new Transform[0];
            }
            if (sensors.Length <= 0)
                return;

            for (int i = 0; i < sensors.Length; i++)
                {
                    if (sensors[i])
                    {
                        Gizmos.DrawRay(sensors[i].position, sensors[i].forward * (limitersAndInfluencers.detectionLength * sensors[i].localScale.z));
                    }
                }
            if (target) {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(target.position, 1.0f);
                Gizmos.color = Color.red;
            }


        }
        //Combination of steering behaviours
        Vector3 SteeringBehaviours()
        {
            Vector3 avoidDir = Vector3.zero;
            Vector3 finalAvoidDir = Vector3.zero;
            Vector3 dirVector;
            finalAvoidDir = Vector3.zero;
            lastMag = (transform.forward + steerVector).normalized;
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
            for (int i = 0; i < sensors.Length; i++)
            {
                if (sensors[i])
                {
                    dirVector = sensors[i].forward;

                    rayMag = (limitersAndInfluencers.detectionLength +(velocity.magnitude * limitersAndInfluencers.velocitySensorMultiplier)) * sensors[i].localScale.z; ;
                    
                    if (reverseControl.reversing)
                        rayMag = (limitersAndInfluencers.detectionLength + (velocity.magnitude * reverseControl.reversingSensorMultiplier)) * sensors[i].localScale.z;

                        bool gotHit = Physics.Raycast(new Ray(sensors[i].position, dirVector), out hit3D, rayMag,useSensorsOn);

                    if (gotHit)
                    {
                        overrideDidHit = true;

                        avoidDir = Vector3.zero;

                        obsVelocity = 0.0f;

                        pursuitVelocity = Vector3.zero;
                            
                        hit3DDistance = hit3D.distance;

                        if (hit3D.collider.GetComponentInParent<Rigidbody>())
                        {
                            obsVelocity = Vector3.Dot(transform.position - hit3D.transform.position, GetVelocity() - hit3D.collider.GetComponentInParent<Rigidbody>().velocity);
                            pursuitVelocity = hit3D.collider.GetComponentInParent<Rigidbody>().velocity;
                        
                        }
                        

                        //Containment
                        if ((whatIsaWall & 1 << hit3D.collider.gameObject.layer) == 1 << hit3D.collider.gameObject.layer)
                        {
                            if (hit3DDistance <= limitersAndInfluencers.maximumContainmentDistance)
                            {
                                avoidDir = hit3D.normal * strengths.containmentAhead;
                                if (showDebugRays)
                                    Debug.DrawRay(hit3D.point, avoidDir, Color.cyan);
                                tempBehavValue = 3;//Containment Executed
                            }

                        }
                        else if (obsVelocity <= 0.0f && pursuitVelocity == Vector3.zero && !hit3D.transform.GetComponentInParent<AgentController>() && hit3DDistance <= limitersAndInfluencers.maximumObstacleAvoidenceDistance)
                        {
                            //Avoid Obstacle
                            avoidDir = (new Vector3(hit3D.point.x, transform.position.y, hit3D.point.z) - hit3D.collider.bounds.center);
                            if (showDebugRays)
                                Debug.DrawRay(hit3D.point, avoidDir, Color.blue);
                            tempBehavValue = 2;//Obstacle Avoidence executed
                        }
                            
                        
                        //Pursuit
                        if (obsVelocity > 0.0f)
                        {

                            if (hit3DDistance <= limitersAndInfluencers.maximumDistanceForPursuit)
                            {
                                persuitVector = pursuitVelocity.normalized * (strengths.pursuitAhead+pursuitVelocity.magnitude);
                                if (showDebugRays)
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

                        float currAngle = Mathf.Abs(Vector3.SignedAngle(transform.forward, sensors[i].forward , transform.up));
                        //Select most threatning and mostpowerful
                        
                        if (currAngle < lastAngle && tempBehavValue > lastExecutedBehav)
                        {
                            lastAngle = currAngle;
                            lastMag = transform.forward + avoidDir;
                            lastExecutedBehav = tempBehavValue;
                        }
                        

                        if (showDebugRays)
                            Debug.DrawLine(sensors[i].position, hit3D.point, Color.yellow);
                        
                    }
                }
            }

            switch (lastExecutedBehav) {
                case 1: lastMag = lastMag.normalized * weights.pursuit; break;
                case 2: lastMag = lastMag.normalized * weights.avoidObstacle; break;
                case 3: lastMag = lastMag.normalized * weights.containment; break;
            }

            //Seperation
            Vector3 seperation = SeperationLogic(allAgents).normalized * weights.seperation;
            lastMag += seperation;

            //UnAllignedCollisionAcoidence
            Vector3 unallignedAvoidenceLocal = Vector3.zero;
            if (seperation == Vector3.zero)
            {
                unallignedAvoidenceLocal = UnAllignedCollisionAvoidence(allAgents).normalized * weights.unallignedCollisionAvoidence;
                lastMag += unallignedAvoidenceLocal;
            }

            if (seperation == Vector3.zero)
            {
                //Queuing
                Vector3 currQueue = CheckForQueuing().normalized * weights.queuing;
                lastMag += currQueue;
            }

            finalAvoidDir = Vector3.zero;
            finalAvoidDir = lastMag;

            if (lastMag == Vector3.zero && !overrideDidHit)
                didHit = false;
            else
                didHit = true;
            //Reverse logic
            if (didHit && velocity.magnitude <= reverseControl.minVelocityForReverseTimeOut && reverseControl.canReverse)
            {
                reverseControl.reverseTimeOutCounter += Time.deltaTime;
                if (reverseControl.reverseTimeOutCounter >= reverseControl.reverseTimeOutIn)
                {
                    reverseControl.reverseTimeOutCounter = 0.0f;
                    reverseControl.reversing = !reverseControl.reversing;
                }
            }
            else {
                reverseControl.reverseTimeOutCounter = 0.0f;
            }

            if (!didHit && reverseControl.reversing)
                reverseControl.reversing = false;
            
            return finalAvoidDir;
        }
        //Seperation logic based on desiredSeperation distance
        Vector3 SeperationLogic(AgentController[] allActiveAgents) {
            Vector3 calculatedSeparation = Vector3.zero;
            Vector3 dist = Vector3.zero;
            float count = 0.0f;
            float desiredCurrSepration = strengths.seperationDistance;

            if (isReversing())
                desiredCurrSepration *= reverseControl.reversingSensorMultiplier;
            else
                desiredCurrSepration *= limitersAndInfluencers.velocitySensorMultiplier;

            foreach (AgentController activeAgent in allActiveAgents) {
                if (activeAgent != this) {
                    dist = transform.position - activeAgent.transform.position;
                    if (dist.magnitude <= (desiredCurrSepration+(activeAgent.strengths.seperationDistance*activeAgent.limitersAndInfluencers.velocitySensorMultiplier))) {
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
        public void SetNewTarget(Transform newTar) {
            target = newTar;
        }
        /// <summary>
        /// Are we reversing?
        /// </summary>
        /// <returns></returns>
        public bool isReversing() {
            if (reverseControl != null)
                return reverseControl.reversing;
            else
                return false;
        }
        /// <summary>
        /// Is there something that is blocking the path? includes all the behaviours
        /// </summary>
        /// <returns></returns>
        public bool SomethingDetected() {
            return didHit;
        }

        /// <summary>
        /// Unalligned Collision avoidence from other agents
        /// </summary>
        /// <param name="allAgents">all the agents</param>
        /// <returns></returns>
        Vector3 UnAllignedCollisionAvoidence(AgentController[] allAgents) {

            float minTime = strengths.unalignedAvoidenceAhead;
            
            float steer = 0.0f;
            Vector3 xThreatPosNearestApproach = Vector3.zero;
            Vector3 xMyPosAtnearestApproach = Vector3.zero;
            AgentController currThreat = null;
            //Calculate approaches
            foreach (AgentController currAgent in allAgents) {
                if (currAgent != this) {
                
                    float collisionThreshold = desiredCurrSepration + (currAgent.strengths.seperationDistance * currAgent.limitersAndInfluencers.velocitySensorMultiplier);
                    float nearestApproachTime = GetNearestApproach(currAgent);
                    if ((nearestApproachTime >= 0) && (nearestApproachTime < minTime))
                    {
                        Vector3 ourPos = Vector3.zero;
                        Vector3 threatPos = Vector3.zero;
                        float currDist = GetNearestPositions(currAgent, nearestApproachTime , ref ourPos , ref threatPos);

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
                    if (GetVelocity().magnitude <= currThreat.GetVelocity().magnitude || currThreat.GetVelocity().magnitude == 0 || gameObject.GetInstanceID() < currThreat.gameObject.GetInstanceID())
                    {
                        float sideDot = Vector3.Dot(transform.right, currThreat.GetVelocity());
                        steer = (sideDot > 0.0f) ? -1.0f : 1.0f;
                    }
                }
                steer *= strengths.seperationDistance + currThreat.strengths.seperationDistance * currThreat.limitersAndInfluencers.velocitySensorMultiplier;
            }


            if (showDebugRays)
                Debug.DrawLine(xMyPosAtnearestApproach, xThreatPosNearestApproach, Color.magenta);

            return (transform.right * steer);
        }
        //Calculating nearest approach
        float GetNearestApproach(AgentController currAgent) {

            Vector3 myVelocity = GetVelocity();
            Vector3 currAgentVelocity = currAgent.GetVelocity();
            Vector3 relVelocity = currAgentVelocity - myVelocity;
            float relSpeed = relVelocity.magnitude;

            if (relSpeed == 0) return 0.0f;

            Vector3 relTangent = relVelocity / relSpeed;

            Vector3 relPosition = transform.position - currAgent.transform.position;

            float projection = Vector3.Dot(relTangent, relPosition);

            return (projection/relSpeed);
        }
        //Calculate nearest positions
        float GetNearestPositions(AgentController currAgent, float approachTime , ref Vector3 ourPos , ref Vector3 threatPos) {
            Vector3 myTravel = GetVelocity() * approachTime;
    
            Vector3 currAgentTravel = currAgent.GetVelocity() * approachTime;

            Vector3 myFinal = transform.position + myTravel;
            Vector3 currAgentFinal = currAgent.transform.position + currAgentTravel;

            ourPos = myFinal;
            threatPos = currAgentFinal;

            return ((myFinal-currAgentFinal).magnitude);
        }
        //Queuing logic for trailing behaviour
        Vector3 CheckForQueuing() {
            bool threatning = GetNeighbourAhead(allAgents);
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
        bool GetNeighbourAhead(AgentController[] allActiveAgents) {
            Vector3 myFuturePos = transform.position + (GetVelocity() * strengths.queuingAhead);

            AgentController curreThreat = null;
            
            foreach (AgentController currAgent in allActiveAgents) {
                if (currAgent != this) {
                    if ((currAgent.transform.position - myFuturePos).magnitude <= (currAgent.strengths.seperationDistance * currAgent.limitersAndInfluencers.velocitySensorMultiplier + desiredCurrSepration)) {
                        curreThreat = currAgent;
                        break;
                    }
                }
            }

            if (curreThreat)
            {
                if (showDebugRays)
                    Debug.DrawLine(transform.position, curreThreat.transform.position, Color.red);
                return true;
            }
            else
                return false;
        }
    }
}