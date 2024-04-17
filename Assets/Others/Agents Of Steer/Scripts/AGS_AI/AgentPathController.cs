using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace negleft.AGS
{

    /// <summary>
    /// Creates path for the agents and controls their progress
    /// </summary>
    public class AgentPathController : MonoBehaviour {
        /// <summary>
        /// Starts all the agents at the start of the scene
        /// </summary>
        [FormerlySerializedAs("LaunchAtStart")] [SerializeField] private bool LaunchAtStartFlag = true;
        /// <summary>
        /// Radius of the path
        /// </summary>
        [FormerlySerializedAs("radius")] [SerializeField] private float radiusValue = 5.0f;
        /// <summary>
        /// is this path a circuit
        /// </summary>
        [FormerlySerializedAs("circuit")] [SerializeField] private bool circuitFlag = false;
        /// <summary>
        /// make all the agents go one way
        /// </summary>
        [FormerlySerializedAs("oneWay")] [SerializeField] private bool oneWayFlag = false;
        /// <summary>
        /// a constant value to fix oneWayLogic
        /// </summary>
        [FormerlySerializedAs("oneWaySkipPoint")] [SerializeField] private int oneWaySkipPointValue = 3;
        /// <summary>
        /// Reset fix - play with this value if you vehicle loses control at the end of the path
        /// </summary>
        [FormerlySerializedAs("resetFixThreshold")] [SerializeField] private int resetFixThresholdValue = 5;
        /// <summary>
        /// velocity based pridiction for every agent to make tem stay on te path
        /// </summary>
        [FormerlySerializedAs("pathPridictionMultiplier")] [SerializeField] private float pathPridictionMultiplierValue = 0.5f;
        /// <summary>
        /// Agents that you want to be controlled by this path
        /// </summary>
        [FormerlySerializedAs("agents")] [SerializeField] private AgentRaceController[] agentsControllers;
        private bool firstRuntimeAgentRecievedFlag = false; 
        /// <summary>
        /// These are the police agents who will be patroling
        /// </summary>
        [FormerlySerializedAs("policeAgents")] [SerializeField] private AgentRaceController[] policeAgentsControllers;
        private int[] agentsCurrentPoints;
        private Vector3[] paths;
        private GameObject handleHolderObject;
        private List<Vector3> smoothPathList = new List<Vector3>();
        /// <summary>
        /// Resolution for catmull-rom based smoothness of the path,make sure it adds up to 1
        /// </summary>
        [FormerlySerializedAs("pathResolution")] [SerializeField] private float pathResolutionValue = 0.1f;
        private bool isStartedFlag = false;
        /// <summary>
        /// can race?
        /// </summary>
        private AgentRacePathManager myRacePathManager;

        /// <summary>
        /// Human Racint?
        /// </summary>
        private bool humanIsRacingFlag = false;
        private int humanRacingAtValue = 0;


        private void Start()
        {
            if (LaunchAtStartFlag)
                StartRace(1);

        }
    /// <summary>
    /// Sets everything and starts the race
    /// </summary>
        public void StartRace(int laps)
        {
            paths = GetPathPoints();
            if (agentsControllers != null)
                AssignAgentsHandels(agentsControllers);
            if (policeAgentsControllers != null)
                AssignAgentsHandels(policeAgentsControllers);


            agentsCurrentPoints = new int[agentsControllers.Length];
            SetPointsPath();
            isStartedFlag = true;

            if (!circuitFlag)
                laps = 1;

            if (transform.GetComponent<AgentRacePathManager>()) {
                myRacePathManager = transform.GetComponent<AgentRacePathManager>();
                myRacePathManager.InitiateRacePathManager(agentsControllers, paths,humanIsRacingFlag,humanRacingAtValue,laps,circuitFlag);
            }
        }

        /// <summary>
        /// Assign handles for the agents to follow
        /// </summary>
        private void AssignAgentsHandels(AgentRaceController[] passedAgents) {
            if (!handleHolderObject) {
                handleHolderObject = new GameObject();
                handleHolderObject.name = "HandlesHolder";
            }

            GameObject tempObj;

            for (int i = 0; i < passedAgents.Length; i++) {
                if (passedAgents[i]) {
                    tempObj = new GameObject();
                    tempObj.name = "HandleFor_" + passedAgents[i].name;
                    tempObj.transform.parent = handleHolderObject.transform;
                    tempObj.transform.position = passedAgents[i].transform.position + passedAgents[i].transform.forward ;
                    passedAgents[i].currTarget = tempObj.transform;
                }
            }

        }


        /// <summary>
        /// Set path points
        /// </summary>
        private void SetPointsPath()
        {
            for (int i = 0; i < agentsControllers.Length; i++)
            {
                if (agentsControllers[i])
                agentsControllers[i].currTarget.position = (paths[agentsCurrentPoints[i]]);
            }
        }

        private void Update() {
            if (!isStartedFlag)
                return;
            if (agentsControllers != null)
                PathBasedAgents(agentsControllers, false);
            if (policeAgentsControllers != null)
                PathBasedAgents(policeAgentsControllers, true);
        }

    
        /// <summary>
        /// keeps the agents on the path
        /// </summary>
        private void PathBasedAgents(AgentRaceController[] passedAgents , bool areThesePolice) {
            Vector3 a = Vector3.zero;//startPoint
            Vector3 b = Vector3.zero;//endPoint
            Vector3 p = Vector3.zero;//pridectedLocation
            Vector3 pHalf = Vector3.zero;//pridectedLocationHalved

            Vector3 v1 = Vector3.zero;
            Vector3 v2 = Vector3.zero;
            Vector3 v3 = Vector3.zero;
            Vector3 v2h = Vector3.zero;
            Vector3 v3h = Vector3.zero;
            Vector3 normalPoint = Vector3.zero;
            Vector3 normalPointHalved = Vector3.zero;
            Vector3 newHandlePos = Vector3.zero;
            
            float dist = Mathf.Infinity;
            float distHalf = Mathf.Infinity;

            float tempvar = 0.0f;
            float tempvarHalf = 0.0f;
            int nearestHalfPoint = 0;
            int nearestMainPoint = 0;
            Vector3 currentRadius = Vector3.zero;
            Vector3 finalNormal = Vector3.zero;
            Vector3 finalHalved = Vector3.zero;
            AgentRaceController[] givenAgents = passedAgents;

            for (int i = 0; i < givenAgents.Length; i++)
            {
                if (givenAgents[i] && !givenAgents[i].GetPolicingStatus()) {
                    
                    newHandlePos = Vector3.zero;

                    dist = Mathf.Infinity;
                    distHalf = Mathf.Infinity;
                    finalNormal = Vector3.zero;
                    //Future pos
                    p = (givenAgents[i].transform.forward + givenAgents[i].transform.position) + ((givenAgents[i].GetVelocityVector().normalized * givenAgents[i].GetVelocityVector().magnitude) * pathPridictionMultiplierValue * givenAgents[i].strengthsInfo.pridictPathValue);
                    //future half
                    pHalf = (givenAgents[i].transform.forward + givenAgents[i].transform.position);
                    newHandlePos = p;

                    for (int j = 0; j < paths.Length; j++) {

                        a = paths[j];
                        if (j >= paths.Length - 1)
                        {
                            b = paths[0];
                            if (!circuitFlag)
                                b = paths[paths.Length - 2];
                        }
                        else
                        {
                            b = paths[j + 1];
                        }
                        //normal points to get parallel cast on the path
                        normalPoint = a + Vector3.Dot((p - a), (b - a)) / Vector3.Dot((b - a), (b - a)) * (b - a);
                        normalPointHalved = a + Vector3.Dot((pHalf - a), (b - a)) / Vector3.Dot((b - a), (b - a)) * (b - a);
                        v1 = b - a;
                        v2 = normalPoint - a;
                        v3 = normalPoint - b;

                        if (Vector3.Dot(v2, v1) < 0.0f || Vector3.Dot(v3, v1) > 0.0f)
                        {
                            normalPoint = b;

                        }
                        
                        v2h = normalPointHalved - a;
                        v3h = normalPointHalved - b;



                        if (Vector3.Dot(v2h, v1) < 0.0f || Vector3.Dot(v3h, v1) > 0.0f)
                        {
                            normalPointHalved = b;
                        }


                        tempvar = (normalPoint - p).magnitude;
                        tempvarHalf = (normalPointHalved - pHalf).magnitude;




                        if (tempvarHalf <= distHalf)
                        {
                            distHalf = tempvarHalf;
                            nearestHalfPoint = j;
                            finalHalved = normalPointHalved;
                        }


                        if (tempvar <= dist)
                        {
                            dist = tempvar;
                            nearestMainPoint = j;
                            finalNormal = normalPoint;
                        }

                        if ((j == paths.Length - 1)) {
                            
                            if (nearestHalfPoint > nearestMainPoint && oneWayFlag && (Mathf.Abs(nearestHalfPoint - nearestMainPoint) < resetFixThresholdValue)) {
                                
                                if (oneWaySkipPointValue >= paths.Length) {
                                    oneWaySkipPointValue = 0;
                                }
                                int selectedPoint = nearestHalfPoint + oneWaySkipPointValue;
                                
                                if (selectedPoint >= paths.Length) {
                                    
                                    selectedPoint = (nearestHalfPoint + oneWaySkipPointValue) - (paths.Length-1);
                                }
                                finalNormal = paths[selectedPoint];
                            }
                        }
                    }

                    if (myRacePathManager && !areThesePolice)
                    {
                        myRacePathManager.UpdateAgentsCurrentRaceLapPosition(i, nearestHalfPoint, finalHalved);
                    }


                    if (((p - finalNormal).magnitude > radiusValue)) {
                        newHandlePos = (finalNormal + (p - finalNormal).normalized * radiusValue);
                    }
                    Debug.DrawLine(p, newHandlePos);
                    if (givenAgents[i].currTarget){
                        if (!float.IsNaN(newHandlePos.x) && !float.IsNaN(newHandlePos.y) && !float.IsNaN(newHandlePos.z))
                            givenAgents[i].currTarget.position = newHandlePos;
                    }
                }
                

            }

            if (myRacePathManager && !areThesePolice)
                myRacePathManager.SortTheRaceAgents();
        }
        /// <summary>
        ///Returns a vector 3 array with path points
        /// </summary>
        /// <returns></returns>
        public Vector3[] GetPathPoints() {


            if (transform.childCount < 4) {
                Vector3[] straightPath = new Vector3[transform.childCount];

                for (int i = 0; i < transform.childCount; i++) {
                    straightPath[i] = transform.GetChild(i).position;
                }

                if (transform.childCount < 2) {
                    straightPath = new Vector3[2];
                    straightPath[0] = transform.position;
                    straightPath[1] = straightPath[0];
                    Debug.LogWarning("A path requires 4 or more control points to work correctly , please add more , it would still work though");
                }


                return straightPath;
            }

            smoothPathList.Clear();
            for (int i = 0; i < transform.childCount; i++) {

                if (circuitFlag)
                    CatmullRomDisplayAndRecordPath(i, true);
                else {
                    if (i != transform.childCount - 1)
                        CatmullRomDisplayAndRecordPath(i, true);
                }
            }
                return smoothPathList.ToArray();
        }
        
        /// <summary>
        /// Draw the path
        /// </summary>
        private void OnDrawGizmos()
        {
            int count = transform.childCount;
            for (int i = 0; i < count; i++) {
            
                    Gizmos.color = Color.blue;
                    
                        Gizmos.color = Color.white;
                        Gizmos.DrawWireSphere(transform.GetChild(i).position, 0.75f);
                    
                    Gizmos.color = Color.yellow;
                    if (transform.childCount < 4)
                    {
                    if (i < count - 1)
                        Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(i + 1).position);
                    else if (circuitFlag){
                        Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(0).position);
                    }
                }
                    else {
                    
                        CatmullRomDisplayAndRecordPath(i, false);
                    
                    }
                
                
            }
            if (circuitFlag && count < 4 && count >= 2)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.GetChild(0).position, transform.GetChild(count-1).position);
            }
        }

        //Catmull-Rom

        private void CatmullRomDisplayAndRecordPath (int pos , bool recordPath)
        {

            if (transform.childCount < 4) {
                Debug.LogWarning("Smooth path reuires 4 or more points");
                return;
            }

            bool endThis = false;

            //The 4 points we need to form a spline between p1 and p2
            Vector3 p0;
            Vector3 p1;
            Vector3 p2;
            Vector3 p3;

            //Circuit
            p0 = transform.GetChild(ClampListPositions(pos - 1 ,ref endThis)).position;
            p1 = transform.GetChild(pos).position;
            p2 = transform.GetChild(ClampListPositions(pos + 1, ref endThis)).position;
            p3 = transform.GetChild(ClampListPositions(pos + 2, ref endThis)).position;

            if (endThis)
                return;

            //The start position of the line
            Vector3 lastPos = p1;
            if (recordPath) {
                smoothPathList.Add(lastPos);
            }

            //The spline's resolution
            //Make sure it's is adding up to 1, so 0.3 will give a gap, but 0.2 will work
            float resolution = pathResolutionValue;

            //How many times should we loop?
            int loops = Mathf.FloorToInt(1f / resolution);

            for (int i = 1; i <= loops; i++)
            {
                //Which t position are we at?
                float t = i * resolution;

                //Find the coordinate between the end points with a Catmull-Rom spline
                Vector3 newPos = GetCatmullRomPos(t, p0, p1, p2, p3);

                //Draw this line segment
                if (!recordPath)
                {
                    Gizmos.DrawLine(lastPos, newPos);
                }
                else {
                    smoothPathList.Add(newPos);
                }

                //Save this pos so we can draw the next line segment
                lastPos = newPos;
            }
        }

        //Clamp the list positions to allow looping
        private int ClampListPositions(int pos , ref bool endit)
        {
            if (circuitFlag)
            {
                if (pos < 0)
                {
                    pos = transform.childCount - 1;
                }
                if (pos > transform.childCount)
                {
                    pos = 1;
                }
                else if (pos > transform.childCount - 1)
                {
                    pos = 0;
                }
                
            }
            else {
                if (pos < 0)
                {
                    pos = 0;
                }
                if (pos > transform.childCount)
                {
                    endit = true;
                    pos = 1;
                }
                else if (pos > transform.childCount - 1)
                {
                    pos = transform.childCount - 1;
                }
            }
            return pos;
        }

        //Returns a position between 4 Vector3 with Catmull-Rom spline algorithm
        private Vector3 GetCatmullRomPos(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            //The coefficients of the cubic polynomial (except the 0.5f * which I added later for performance)
            Vector3 a = 2f * p1;
            Vector3 b = p2 - p0;
            Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
            Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;

            //The cubic polynomial: a + b * t + c * t^2 + d * t^3
            Vector3 pos = 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));

            return pos;
        }
        /// <summary>
        /// Add new agent for the race
        /// </summary>
        /// <param name="newAgent"></param>
        public void AddNewRaceAgent( AgentRaceController newAgent) {
            if (isStartedFlag) {
                Debug.LogWarning("You can't add more agents to the race once it's started");
            }
            if (!firstRuntimeAgentRecievedFlag) {
                agentsControllers = new AgentRaceController[0];
                firstRuntimeAgentRecievedFlag = true;
            }

            List<AgentRaceController> currAgents = new List<AgentRaceController>();
            currAgents.Clear();
            currAgents.AddRange(agentsControllers);
            if (newAgent != null)
            {
                currAgents.Add(newAgent);
                agentsControllers = currAgents.ToArray();
            }

        }
        /// <summary>
        /// Did we spawn a human Controlled agent
        /// </summary>
        /// <param name="at"></param>
        public void PlayerSpawnedAt (int at) {
            humanIsRacingFlag = true;
            humanRacingAtValue = at;
        }
    }
}
