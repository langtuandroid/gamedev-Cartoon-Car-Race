using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace negleft.AGS
{

    /// <summary>
    /// Creates path for the agents and controls their progress
    /// </summary>
    public class AgentPathCreator : MonoBehaviour {
        /// <summary>
        /// Starts all the agents at the start of the scene
        /// </summary>
        public bool LaunchAtStart = true;
        /// <summary>
        /// Radius of the path
        /// </summary>
        public float radius = 5.0f;
        /// <summary>
        /// is this path a circuit
        /// </summary>
        public bool circuit = false;
        /// <summary>
        /// make all the agents go one way
        /// </summary>
        public bool oneWay = false;
        /// <summary>
        /// a constant value to fix oneWayLogic
        /// </summary>
        public int oneWaySkipPoint = 3;
        /// <summary>
        /// Reset fix - play with this value if you vehicle loses control at the end of the path
        /// </summary>
        public int resetFixThreshold = 5;
        /// <summary>
        /// velocity based pridiction for every agent to make tem stay on te path
        /// </summary>
        public float pathPridictionMultiplier = 0.5f;
        /// <summary>
        /// Agents that you want to be controlled by this path
        /// </summary>
        public AgentController[] agents;
        bool firstRuntimeAgentRecieved = false; 
        /// <summary>
        /// These are the police agents who will be patroling
        /// </summary>
        public AgentController[] policeAgents;
        int[] agentsCurrentPoint;
        Vector3[] path;
        GameObject handleHolder;
        List<Vector3> smoothPath = new List<Vector3>();
        /// <summary>
        /// Resolution for catmull-rom based smoothness of the path,make sure it adds up to 1
        /// </summary>
        public float pathResolution = 0.1f;
        bool isStarted = false;
        /// <summary>
        /// can race?
        /// </summary>
        AgentRaceManager myRaceManager;

        /// <summary>
        /// Human Racint?
        /// </summary>
        bool humanIsRacing = false;
        int humanRacingAt = 0;


        private void Start()
        {
            if (LaunchAtStart)
                StartTheRace(1);

        }
    /// <summary>
    /// Sets everything and starts the race
    /// </summary>
        public void StartTheRace(int laps)
        {
            path = GetPath();
            if (agents != null)
                AssignHandels(agents);
            if (policeAgents != null)
                AssignHandels(policeAgents);


            agentsCurrentPoint = new int[agents.Length];
            SetPath();
            isStarted = true;

            if (!circuit)
                laps = 1;

            if (transform.GetComponent<AgentRaceManager>()) {
                myRaceManager = transform.GetComponent<AgentRaceManager>();
                myRaceManager.InitiateRaceManager(agents, path,humanIsRacing,humanRacingAt,laps,circuit);
            }
        }

        /// <summary>
        /// Assign handles for the agents to follow
        /// </summary>
        void AssignHandels(AgentController[] passedAgents) {
            if (!handleHolder) {
                handleHolder = new GameObject();
                handleHolder.name = "HandlesHolder";
            }

            GameObject tempObj;

            for (int i = 0; i < passedAgents.Length; i++) {
                if (passedAgents[i]) {
                    tempObj = new GameObject();
                    tempObj.name = "HandleFor_" + passedAgents[i].name;
                    tempObj.transform.parent = handleHolder.transform;
                    tempObj.transform.position = passedAgents[i].transform.position + passedAgents[i].transform.forward ;
                    passedAgents[i].target = tempObj.transform;
                }
            }

        }


        /// <summary>
        /// Set path points
        /// </summary>
        void SetPath()
        {
            for (int i = 0; i < agents.Length; i++)
            {
                if (agents[i])
                agents[i].target.position = (path[agentsCurrentPoint[i]]);
            }
        }

        void Update() {
            if (!isStarted)
                return;
            if (agents != null)
                PathBased(agents, false);
            if (policeAgents != null)
                PathBased(policeAgents, true);
        }

    
        /// <summary>
        /// keeps the agents on the path
        /// </summary>
        void PathBased(AgentController[] passedAgents , bool areThesePolice) {
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
            AgentController[] givenAgents = passedAgents;

            for (int i = 0; i < givenAgents.Length; i++)
            {
                if (givenAgents[i] && !givenAgents[i].GetPolicing()) {
                    
                    newHandlePos = Vector3.zero;

                    dist = Mathf.Infinity;
                    distHalf = Mathf.Infinity;
                    finalNormal = Vector3.zero;
                    //Future pos
                    p = (givenAgents[i].transform.forward + givenAgents[i].transform.position) + ((givenAgents[i].GetVelocity().normalized * givenAgents[i].GetVelocity().magnitude) * pathPridictionMultiplier * givenAgents[i].strengths.pridictPath);
                    //future half
                    pHalf = (givenAgents[i].transform.forward + givenAgents[i].transform.position);
                    newHandlePos = p;

                    for (int j = 0; j < path.Length; j++) {

                        a = path[j];
                        if (j >= path.Length - 1)
                        {
                            b = path[0];
                            if (!circuit)
                                b = path[path.Length - 2];
                        }
                        else
                        {
                            b = path[j + 1];
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

                        if ((j == path.Length - 1)) {
                            
                            if (nearestHalfPoint > nearestMainPoint && oneWay && (Mathf.Abs(nearestHalfPoint - nearestMainPoint) < resetFixThreshold)) {
                                
                                if (oneWaySkipPoint >= path.Length) {
                                    oneWaySkipPoint = 0;
                                }
                                int selectedPoint = nearestHalfPoint + oneWaySkipPoint;
                                
                                if (selectedPoint >= path.Length) {
                                    
                                    selectedPoint = (nearestHalfPoint + oneWaySkipPoint) - (path.Length-1);
                                }
                                finalNormal = path[selectedPoint];
                            }
                        }
                    }

                    if (myRaceManager && !areThesePolice)
                    {
                        myRaceManager.UpdateAgentsCurrentLapPosition(i, nearestHalfPoint, finalHalved);
                    }


                    if (((p - finalNormal).magnitude > radius)) {
                        newHandlePos = (finalNormal + (p - finalNormal).normalized * radius);
                    }
                    Debug.DrawLine(p, newHandlePos);
                    if (givenAgents[i].target){
                        if (!float.IsNaN(newHandlePos.x) && !float.IsNaN(newHandlePos.y) && !float.IsNaN(newHandlePos.z))
                            givenAgents[i].target.position = newHandlePos;
                    }
                }
                

            }

            if (myRaceManager && !areThesePolice)
                myRaceManager.SortTheRacingAgents();
        }
        /// <summary>
        ///Returns a vector 3 array with path points
        /// </summary>
        /// <returns></returns>
        public Vector3[] GetPath() {


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

            smoothPath.Clear();
            for (int i = 0; i < transform.childCount; i++) {

                if (circuit)
                    CatmullRomDisplayAndRecord(i, true);
                else {
                    if (i != transform.childCount - 1)
                        CatmullRomDisplayAndRecord(i, true);
                }
            }
                return smoothPath.ToArray();
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
                    else if (circuit){
                        Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(0).position);
                    }
                }
                    else {
                    
                        CatmullRomDisplayAndRecord(i, false);
                    
                    }
                
                
            }
            if (circuit && count < 4 && count >= 2)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.GetChild(0).position, transform.GetChild(count-1).position);
            }
        }

        //Catmull-Rom

        void CatmullRomDisplayAndRecord (int pos , bool recordPath)
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
            p0 = transform.GetChild(ClampListPos(pos - 1 ,ref endThis)).position;
            p1 = transform.GetChild(pos).position;
            p2 = transform.GetChild(ClampListPos(pos + 1, ref endThis)).position;
            p3 = transform.GetChild(ClampListPos(pos + 2, ref endThis)).position;

            if (endThis)
                return;

            //The start position of the line
            Vector3 lastPos = p1;
            if (recordPath) {
                smoothPath.Add(lastPos);
            }

            //The spline's resolution
            //Make sure it's is adding up to 1, so 0.3 will give a gap, but 0.2 will work
            float resolution = pathResolution;

            //How many times should we loop?
            int loops = Mathf.FloorToInt(1f / resolution);

            for (int i = 1; i <= loops; i++)
            {
                //Which t position are we at?
                float t = i * resolution;

                //Find the coordinate between the end points with a Catmull-Rom spline
                Vector3 newPos = GetCatmullRomPosition(t, p0, p1, p2, p3);

                //Draw this line segment
                if (!recordPath)
                {
                    Gizmos.DrawLine(lastPos, newPos);
                }
                else {
                    smoothPath.Add(newPos);
                }

                //Save this pos so we can draw the next line segment
                lastPos = newPos;
            }
        }

        //Clamp the list positions to allow looping
        int ClampListPos(int pos , ref bool endit)
        {
            if (circuit)
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
        Vector3 GetCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
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
        public void AddNewAgent( AgentController newAgent) {
            if (isStarted) {
                Debug.LogWarning("You can't add more agents to the race once it's started");
            }
            if (!firstRuntimeAgentRecieved) {
                agents = new AgentController[0];
                firstRuntimeAgentRecieved = true;
            }

            List<AgentController> currAgents = new List<AgentController>();
            currAgents.Clear();
            currAgents.AddRange(agents);
            if (newAgent != null)
            {
                currAgents.Add(newAgent);
                agents = currAgents.ToArray();
            }

        }
        /// <summary>
        /// Did we spawn a human Controlled agent
        /// </summary>
        /// <param name="at"></param>
        public void PlayerSpawned (int at) {
            humanIsRacing = true;
            humanRacingAt = at;
        }

        

    }
}
