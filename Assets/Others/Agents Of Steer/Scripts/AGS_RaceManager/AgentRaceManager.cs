using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
namespace negleft.AGS{
    [RequireComponent(typeof(AgentPathCreator))]
    public class AgentRaceManager : MonoBehaviour {

        /// <summary>
        /// Agent in race info
        /// </summary>
        [System.Serializable]
        public class AgentsInRaceInfos {
            /// <summary>
            /// Agent's name
            /// </summary>
            public string agentsName = "";
            /// <summary>
            /// Agent's ID
            /// </summary>
            public int agentID = 0;
            /// <summary>
            /// Current laps progress
            /// </summary>
            public float agentsCurrentLapProgress = 0.0f;
            /// <summary>
            /// Current Lap no.
            /// </summary>
            public float agentsLapNumber = 1f;
            /// <summary>
            /// Max lap progress covered legally
            /// </summary>
            public float longestLegalTravel = 0.0f;
            /// <summary>
            /// Has this agent finished the race?
            /// </summary>
            public bool finished = false;
            /// <summary>
            /// Has this agent been disqualified
            /// </summary>
            public bool disqualified = false;
            /// <summary>
            /// Can this agent finish the current lap?
            /// </summary>
            public bool canFinishTheLap = false;
            /// <summary>
            /// get the progress at start
            /// </summary>
            bool recievedFirstProgress = false;
            /// <summary>
            /// Agent controller of this agent
            /// </summary>
            public AgentController myAgentController;
            /// <summary>
            /// progress tab for leaderbodard
            /// </summary>
            public AgentProgressTab myProgressTab;
            /// <summary>
            /// head pos info to be displayed at the head if the agent
            /// </summary>
            public Text headPosInfo;
            /// <summary>
            /// have we skipped the first lap // depends on where the finish trigger is placed
            /// </summary>
            public bool skipedFirstLap = false;

            /// <summary>
            /// Finish the lap and the race
            /// </summary>
            /// <param name="maxLaps">no of laps</param>
            /// <param name="stillRacing">how many are still racing</param>
            /// <param name="itsACircuit">is this map a circuit</param>
            public void FinishTheLap(int maxLaps, ref float stillRacing , bool itsACircuit) {
                if (!canFinishTheLap)
                    return;

                if (skipedFirstLap)
                {
                    if (agentsLapNumber < maxLaps && agentsLapNumber < maxLaps)
                        agentsLapNumber++;
                    else if (!finished)
                    {
                        finished = true;
                        agentsCurrentLapProgress = (maxLaps + 1) + stillRacing;
                        stillRacing--;
                        if (myAgentController) {
                            if (itsACircuit)
                            {
                                //turn the player into an AI
                                if (myAgentController.transform.GetComponent<AICarDriver>())
                                {
                                    if (myAgentController.transform.GetComponent<AICarDriver>().controlledByPlayer)
                                    {
                                        myAgentController.transform.GetComponent<AICarDriver>().controlledByPlayer = false;
                                    }
                                }
                            }
                            else {
                                myAgentController.target = null;
                            }


                        }
                    }

                }
                else
                    skipedFirstLap = true;

                canFinishTheLap = false;
                longestLegalTravel = 0.0f;
            }

            /// <summary>
            /// Calculates longest legal travel
            /// </summary>
            /// <param name="threshold">reverse cheeting threshold</param>
            void CalculateLongestLegalTravel(float threshold) {

                if (finished) {
                    return;
                }

                float currentTravel = agentsCurrentLapProgress - agentsLapNumber;
                //compares and gets the longest legal travel
                if (Mathf.Abs(currentTravel - longestLegalTravel) < threshold && currentTravel > longestLegalTravel)
                {
                    longestLegalTravel = currentTravel;
                    if (longestLegalTravel > (1 - threshold))
                    {
                        canFinishTheLap = true;
                    }
                }
            }
            /// <summary>
            /// Current lap progress
            /// </summary>
            /// <param name="newProgress"> new calculated progress </param>
            /// <param name="threshold">reverse cheeting threshold</param>
            public void NewLapProgress(float newProgress, float threshold) {

                if (finished)
                {
                    return;
                }

                if (Mathf.Abs(longestLegalTravel - newProgress) < threshold || !recievedFirstProgress) {
                    agentsCurrentLapProgress = newProgress + agentsLapNumber;
                    if (!recievedFirstProgress)
                    {
                        longestLegalTravel = newProgress;
                        recievedFirstProgress = true;
                    }
                    CalculateLongestLegalTravel(threshold);

                }

            }
            /// <summary>
            /// The agent this block belongs to
            /// </summary>
            /// <param name="thisAgent">AI</param>
            public void AssignAgentController(AgentController thisAgent) {
                myAgentController = thisAgent;
            }

        }
        //Variables
        /// <summary>
        /// where is finish trigger is placed
        /// </summary>
        public enum TriggerPlacementOption { Ahead, Behind };
        /// <summary>
        /// Object
        /// </summary>
        public TriggerPlacementOption whereIsTriggerIsPlaced;
        /// <summary>
        /// Finishing trigger
        /// </summary>
        public Collider finishTrigger;
        /// <summary>
        /// Reverse cheeting threshold fixes the winning of race by just revesing back to finish line
        /// </summary>
        public float reverseFinishLineCheetingThreshold = 0.2f;
        /// <summary>
        /// total length of path
        /// </summary>
        float totalDist = 0.0f;
        /// <summary>
        /// current path used to race on
        /// </summary>
        Vector3[] currentPath;
        /// <summary>
        /// Agents racing in the race
        /// </summary>
        AgentsInRaceInfos[] agentsRacing;
        /// <summary>
        /// Agents in order of their respective positions in race
        /// </summary>
        AgentsInRaceInfos[] agentsInOrder;
        /// <summary>
        /// Whats the main camera // will be assigned to billboard heads
        /// </summary>
        public Transform currentMainCamera;
        /// <summary>
        /// has the race been initiated
        /// </summary>
        bool raceManagerInitiated = false;
        /// <summary>
        /// Indexes of path based on finish trigger
        /// </summary>
        int[] indexesBasedOnPivot;
        /// <summary>
        /// Content of leaderboard scroll
        /// </summary>
        public Transform progressContent;
        /// <summary>
        /// Prefab of progress tab for leaderboard
        /// </summary>
        public GameObject progressTab;
        /// <summary>
        /// Prefab of progress tab for leaderboard and this one is for human
        /// </summary>
        public GameObject progressTabForHuman;
        /// <summary>
        /// Enable disable this progress board
        /// </summary>
        public GameObject wholeProgressBoard;
        public bool autoHideProgressBoard = true;
        
        /// <summary>
        /// End panel to show when you finish the race
        /// </summary>
        public GameObject endPanel;
        /// <summary>
        /// Prefab for head info display
        /// </summary>
        public GameObject headPosInfo;
        /// <summary>
        /// Names to be randomly assigned
        /// </summary>
        public String[] agentNames;
        /// <summary>
        /// How many agents are still racing
        /// </summary>
        float remainingToFinish = 0;
        /// <summary>
        /// How many laps
        /// </summary>
        int laps = 1;
        /// <summary>
        /// Has the race concluded
        /// </summary>
        bool isRaceConcluded = false;
        bool alwaysShowLeaderboard = false;
        /// <summary>
        /// is a human racing
        /// </summary>
        bool humanInRace = false;
        /// <summary>
        /// Id of human
        /// </summary>
        int humanID = 0;
        /// <summary>
        /// is this a circuit
        /// </summary>
        bool onACircuit;
        /// <summary>
        /// Add the object that hold the UI based controls
        /// </summary>
        public GameObject mobileControlsHolder;


        /// <summary>
        /// Initiates the race manager
        /// </summary>
        /// <param name="racingAgents">who are all the agents that are racing</param>
        /// <param name="path">path array</param>
        /// <param name="humanRacing">is a human racing</param>
        /// <param name="humanAt">index of human agent</param>
        /// <param name="noOfLaps">how many laps , if any?</param>
        /// <param name="isACircuit">is this a circuit</param>
        public void InitiateRaceManager(AgentController[] racingAgents, Vector3[] path, bool humanRacing, int humanAt , int noOfLaps,bool isACircuit)
        {

            onACircuit = isACircuit;
            if (!isACircuit)
            {
                whereIsTriggerIsPlaced = TriggerPlacementOption.Behind;
                laps = 1;
            }
            else {
                laps = noOfLaps;
            }


            totalDist = 0.0f;
            agentsRacing = new AgentsInRaceInfos[racingAgents.Length];
            currentPath = path;
            //will recaculate the path from new pivot
            if (isACircuit)
                ReCalculateThePathFromPivot();

            GameObject temp = null;//ProgressTab
            GameObject temp2 = null;//HeadPosInfo

            List<String> tempNames = new List<String>(agentNames);//Random Names
            int randChosen = 0;//Randomly Chosen Name
            
            //Catagorize of the agents
            for (int i = 0; i < agentsRacing.Length; i++) {
                if (racingAgents[i])
                {
                    remainingToFinish++;
                    agentsRacing[i] = new AgentsInRaceInfos();
                    if (tempNames.Count > 0) {
                        randChosen = UnityEngine.Random.Range(0, tempNames.Count);
                        agentsRacing[i].agentsName = tempNames[randChosen];
                        tempNames.RemoveAt(randChosen);
                    }
                    else
                        agentsRacing[i].agentsName = racingAgents[i].gameObject.name;

                    if (i == humanAt) {
                        agentsRacing[i].agentsName = "Player";
                        humanID = humanAt;
                        humanInRace = humanRacing;
                    }

                    agentsRacing[i].agentID = i;

                    agentsRacing[i].AssignAgentController(racingAgents[i]);

                    if (whereIsTriggerIsPlaced == TriggerPlacementOption.Behind) {
                        agentsRacing[i].skipedFirstLap = true;
                    }
                    //ProgressTabs
                    if (progressContent && progressTab) {
                        if (progressTabForHuman && i == humanAt && humanRacing)
                            temp = Instantiate(progressTabForHuman) as GameObject;
                        else
                            temp = Instantiate(progressTab) as GameObject;
                        temp.transform.SetParent(progressContent);
                        temp.transform.localScale = Vector3.one;
                        if (temp.GetComponent<AgentProgressTab>())
                        {
                            agentsRacing[i].myProgressTab = temp.GetComponent<AgentProgressTab>();
                            agentsRacing[i].myProgressTab.GiveName(agentsRacing[i].agentsName);
                            agentsRacing[i].myProgressTab.UpdateProgress(i,false);
                        }
                    }
                    //Headinfo
                    if (headPosInfo && i != humanAt) {
                        temp2 = Instantiate(headPosInfo) as GameObject;
                        temp2.transform.SetParent(racingAgents[i].transform);
                        temp2.transform.localPosition = Vector3.zero;
                        if (temp2.GetComponent<BillboardController>() && currentMainCamera) {
                            temp2.GetComponent<BillboardController>().GiveCam(currentMainCamera);
                        }
                        if (agentsRacing[i] != null && temp2.transform.GetComponentInChildren<Text>()) {
                            agentsRacing[i].headPosInfo = temp2.transform.GetComponentInChildren<Text>();
                            agentsRacing[i].headPosInfo.text = "";
                        }

                    }


                }
                else {
                    if (agentsRacing[i] == null)
                        agentsRacing[i] = new AgentsInRaceInfos();
                    agentsRacing[i].disqualified = true;
                }
            }

            

            //Get the pathLength in meters
            if (currentPath.Length > 2)
            {
                for (int i = 1; i < currentPath.Length; i++)
                {
                    totalDist += (currentPath[i] - currentPath[i - 1]).magnitude;
                }

                agentsInOrder = agentsRacing;

                raceManagerInitiated = true;
                Debug.Log("Race Initiated - Total Path Length : " + totalDist);
            }
            else {
                Debug.Log("Race Can't be initiated because the path length is too small");
            }

            //Assign hud info
            if (transform.GetComponent<AgentHUDInfo>())
            {
                transform.GetComponent<AgentHUDInfo>().agentID = humanAt;
                transform.GetComponent<AgentHUDInfo>().EnableHUD();
            }

            //Update all the displays of positions and names
            StartCoroutine(UpdatePositionDisplays());
        }

        /// <summary>
        /// Update the progress the an agent in the race
        /// </summary>
        /// <param name="agentIndex">at what index this agent is at?</param>
        /// <param name="pathPoint"> nearest path point</param>
        /// <param name="currentPos">agent's position</param>
        public void UpdateAgentsCurrentLapPosition(int agentIndex, int pathPoint, Vector3 currentPos) {
            if (!raceManagerInitiated || agentsRacing[agentIndex].disqualified)
                return;
            pathPoint = GetNewIndexBasedOnPivot(pathPoint);
            float lapProgress = CalculateProgress(pathPoint, currentPos);

            agentsRacing[agentIndex].NewLapProgress( Mathf.Clamp01(lapProgress / totalDist),reverseFinishLineCheetingThreshold);
        }

        //Calculate the new progress
        float CalculateProgress(int pathPoint, Vector3 currentPos) {
            if (!raceManagerInitiated)
                return 0;
        
            float calculatedProgress = 0.0f;

            for (int i = 0; i < pathPoint - 1; i++)
            {
                calculatedProgress += (currentPath[i] - currentPath[i + 1]).magnitude;
            }
            calculatedProgress += (currentPath[pathPoint] - currentPos).magnitude;
            return calculatedProgress;
        }

        /// <summary>
        /// Reorder the racing agents based on their progress
        /// </summary>
        public void SortTheRacingAgents() {
            if (!raceManagerInitiated)
                return;
            List<AgentsInRaceInfos> tempInOrder = new List<AgentsInRaceInfos>(agentsRacing);
            tempInOrder.Sort(PositionCompare);
            agentsInOrder = tempInOrder.ToArray();

        }
        /// <summary>
        /// position comparer
        /// </summary>
        /// <param name="agent1"></param>
        /// <param name="agent2"></param>
        /// <returns></returns>
        int PositionCompare(AgentsInRaceInfos agent1, AgentsInRaceInfos agent2) {
            return agent2.agentsCurrentLapProgress.CompareTo(agent1.agentsCurrentLapProgress);
        }
        /// <summary>
        /// Recalculate the path taking finish trigger as the pivot
        /// </summary>
        void ReCalculateThePathFromPivot() {
            if (!finishTrigger)
                return;

            float minDist = Mathf.Infinity;
            int closestIndex = 0;

            indexesBasedOnPivot = new int[currentPath.Length];

            for (int i = 0; i < currentPath.Length; i++) {
                if ((finishTrigger.transform.position - currentPath[i]).magnitude < minDist) {
                    closestIndex = i;
                    minDist = (finishTrigger.transform.position - currentPath[i]).magnitude;
                }
            }

            //Debug.Log("Closest Index : " + closestIndex + " - " + currentPath.Length);
            
            int length = currentPath.Length;

            //Recalculating The CurrentPath
            Vector3[] pathClone = new Vector3[length];

            for (int i = 0; i < length; i++) {
                pathClone[i] = currentPath[closestIndex];
                indexesBasedOnPivot[closestIndex] = i;
                closestIndex++;
                if (closestIndex >= length) {
                    closestIndex = 0;
                }
            }

            currentPath = pathClone;
            
        }
        /// <summary>
        /// return the new index based on pivot
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        int GetNewIndexBasedOnPivot(int index) {
            if (indexesBasedOnPivot != null)
            {
                return indexesBasedOnPivot[index];

            }
            else
                return index;
        }
        /// <summary>
        /// This agent finished the lap
        /// </summary>
        /// <param name="thisAgent">pass the agent</param>
        public void AgentFinishedTheLap (AgentController thisAgent){
            if (!raceManagerInitiated)
                return;

            foreach (AgentsInRaceInfos currInfo in agentsRacing) {
                if (currInfo.myAgentController == thisAgent)
                {
                    currInfo.FinishTheLap(laps,ref remainingToFinish , onACircuit);

                    if (humanInRace && humanID < agentsRacing.Length) {
                        if (agentsRacing[humanID].finished) {
                            ShowEndPanel();
                        }
                    }

                    if (remainingToFinish == 0 && !isRaceConcluded) {
                        RaceConcluded();
                    }

                    break;
                }
            }
        }
        /// <summary>
        /// Draw the pivot
        /// </summary>
        void OnDrawGizmos() {
            if (raceManagerInitiated) {
                Gizmos.color = Color.red;
                if (currentPath.Length > 0)
                Gizmos.DrawSphere(currentPath[0],1.0f);
            }

        }
        /// <summary>
        /// Update all the displays of positions and names
        /// </summary>
        /// <returns></returns>
        IEnumerator UpdatePositionDisplays() {
            while (progressContent && progressTab)
            {
                for (int i = 0; i < agentsInOrder.Length; i++) {
                    if (agentsInOrder[i].myProgressTab) {
                        agentsInOrder[i].myProgressTab.UpdateProgress(i, agentsInOrder[i].finished);
                    }
                    if (agentsInOrder[i].headPosInfo) {
                        agentsInOrder[i].headPosInfo.text = agentsInOrder[i].agentsName + " - " + (i+1).ToString();
                    }
                }
                //Tab to show the leaderboard
                if (wholeProgressBoard && !alwaysShowLeaderboard && autoHideProgressBoard)
                {
                    wholeProgressBoard.SetActive(Input.GetKey(KeyCode.Tab));
                }
                else if (wholeProgressBoard && !autoHideProgressBoard) {
                    wholeProgressBoard.SetActive(true);
                }

                

                yield return null;
            }

            yield return null;
        }
        /// <summary>
        /// Get the lap no.
        /// </summary>
        /// <param name="id">of this agent</param>
        /// <returns></returns>
        public float GetTheLapNumber( int id) {
            if (raceManagerInitiated)
            {
                if (id >= agentsRacing.Length)
                {
                    return 1;
                }
                else
                {
                    return agentsRacing[id].agentsLapNumber;
                }
            }
            else {
                return 1;
            }
        }
        /// <summary>
        /// At what position
        /// </summary>
        /// <param name="id">this agent is at?</param>
        /// <returns></returns>
        public int GetPositionInRaceHierarchy( int id) {

            if (raceManagerInitiated)
            {
                if (id < agentsRacing.Length)
                {
                    for (int i = 0; i < agentsInOrder.Length; i++)
                    {
                        if (agentsInOrder[i].agentID == id)
                        {
                            return i+1;
                        }
                    }
                }
            }

            return 1;
        }


        /// <summary>
        /// Give me the driver 
        /// </summary>
        /// <param name="id">of this agent</param>
        /// <returns></returns>
        public AICarDriver GetTheDriver(int id)
        {
            
            AICarDriver thisDriver = null;
            if (raceManagerInitiated)
            {
                if (id < agentsRacing.Length)
                {
                    
                    for (int i = 0; i < agentsRacing.Length; i++)
                    {
                        if (agentsRacing[i].agentID == id)
                        {
                            
                            if (agentsRacing[i].myAgentController.GetComponent<AICarDriver>()) {
                                thisDriver = agentsRacing[i].myAgentController.GetComponent<AICarDriver>();
                                return thisDriver;
                            }
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// How many laps in this race
        /// </summary>
        /// <returns></returns>
        public int HowManyLaps() {
            return laps;
        }
        /// <summary>
        /// How many agents are racing
        /// </summary>
        /// <returns></returns>
        public int HowManyRacing() {
            if (agentsRacing != null)
                return agentsRacing.Length;
            else
                return 1;
        }
        /// <summary>
        /// Show the end panel
        /// </summary>
        void ShowEndPanel() {

            alwaysShowLeaderboard = true;

            if (wholeProgressBoard && alwaysShowLeaderboard)
            {
                wholeProgressBoard.SetActive(true);
            }

            if (endPanel) {
                endPanel.SetActive(true);
            }

            if (mobileControlsHolder) {
                mobileControlsHolder.SetActive(false);
            }

            if (transform.GetComponent<AgentHUDInfo>()) {
                transform.GetComponent<AgentHUDInfo>().DisableHUD();
            }

        }
        /// <summary>
        /// Runs when the race is concluded
        /// </summary>
        void RaceConcluded() {
            if (agentsInOrder.Length > 0)
                Debug.Log("This race has been concluded and the winner is : " + agentsInOrder[0].agentsName);

            isRaceConcluded = true;

            ShowEndPanel();
        }
        /// <summary>
        /// is race manager initiated
        /// </summary>
        /// <returns></returns>
        public bool IsRaceInitiated() {
            return raceManagerInitiated;
        }
    }
}