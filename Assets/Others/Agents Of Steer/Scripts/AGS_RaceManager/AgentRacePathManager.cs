using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace negleft.AGS{
    [RequireComponent(typeof(AgentPathController))]
    public class AgentRacePathManager : MonoBehaviour {

        /// <summary>
        /// Agent in race info
        /// </summary>
        [System.Serializable]
        public class AgentsInRaceInfosR {
            /// <summary>
            /// Agent's name
            /// </summary>
            [FormerlySerializedAs("agentsName")] public string agentsNameValue = "";
            /// <summary>
            /// Agent's ID
            /// </summary>
            [FormerlySerializedAs("agentID")] public int agentIDValue = 0;
            /// <summary>
            /// Current laps progress
            /// </summary>
            [FormerlySerializedAs("agentsCurrentLapProgress")] public float agentsCurrentLapProgressValue = 0.0f;
            /// <summary>
            /// Current Lap no.
            /// </summary>
            [FormerlySerializedAs("agentsLapNumber")] public float agentsLapNumberValue = 1f;
            /// <summary>
            /// Max lap progress covered legally
            /// </summary>
            [FormerlySerializedAs("longestLegalTravel")] public float longestLegalTravelValue = 0.0f;
            /// <summary>
            /// Has this agent finished the race?
            /// </summary>
            [FormerlySerializedAs("finished")] public bool finishedFlag = false;
            /// <summary>
            /// Has this agent been disqualified
            /// </summary>
            [FormerlySerializedAs("disqualified")] public bool disqualifiedFlag = false;
            /// <summary>
            /// Can this agent finish the current lap?
            /// </summary>
            [FormerlySerializedAs("canFinishTheLap")] public bool canFinishTheLapFlag = false;
            /// <summary>
            /// get the progress at start
            /// </summary>
            private bool recievedFirstProgressFlag = false;
            /// <summary>
            /// Agent controller of this agent
            /// </summary>
            [FormerlySerializedAs("myAgentController")] public AgentRaceController myAgentControllerValue;
            /// <summary>
            /// progress tab for leaderbodard
            /// </summary>
            [FormerlySerializedAs("myProgressTab")] public AgentProgressTabRace myProgressTabRace;
            /// <summary>
            /// head pos info to be displayed at the head if the agent
            /// </summary>
            [FormerlySerializedAs("headPosInfo")] public Text headPosInfoText;
            /// <summary>
            /// have we skipped the first lap // depends on where the finish trigger is placed
            /// </summary>
            [FormerlySerializedAs("skipedFirstLap")] public bool skipedFirstLapFlag = false;

            /// <summary>
            /// Finish the lap and the race
            /// </summary>
            /// <param name="maxLaps">no of laps</param>
            /// <param name="stillRacing">how many are still racing</param>
            /// <param name="itsACircuit">is this map a circuit</param>
            public void FinishTheRaceLap(int maxLaps, ref float stillRacing , bool itsACircuit) {
                if (!canFinishTheLapFlag)
                    return;

                if (skipedFirstLapFlag)
                {
                    if (agentsLapNumberValue < maxLaps && agentsLapNumberValue < maxLaps)
                        agentsLapNumberValue++;
                    else if (!finishedFlag)
                    {
                        finishedFlag = true;
                        agentsCurrentLapProgressValue = (maxLaps + 1) + stillRacing;
                        stillRacing--;
                        if (myAgentControllerValue) {
                            if (itsACircuit)
                            {
                                //turn the player into an AI
                                if (myAgentControllerValue.transform.GetComponent<AICarDriverControl>())
                                {
                                    if (myAgentControllerValue.transform.GetComponent<AICarDriverControl>().controlledByPlayerFlag)
                                    {
                                        myAgentControllerValue.transform.GetComponent<AICarDriverControl>().controlledByPlayerFlag = false;
                                    }
                                }
                            }
                            else {
                                myAgentControllerValue.currTarget = null;
                            }


                        }
                    }

                }
                else
                    skipedFirstLapFlag = true;

                canFinishTheLapFlag = false;
                longestLegalTravelValue = 0.0f;
            }

            /// <summary>
            /// Calculates longest legal travel
            /// </summary>
            /// <param name="threshold">reverse cheeting threshold</param>
            private void CalculateLongestLegalTravelValue(float threshold) {

                if (finishedFlag) {
                    return;
                }

                float currentTravel = agentsCurrentLapProgressValue - agentsLapNumberValue;
                //compares and gets the longest legal travel
                if (Mathf.Abs(currentTravel - longestLegalTravelValue) < threshold && currentTravel > longestLegalTravelValue)
                {
                    longestLegalTravelValue = currentTravel;
                    if (longestLegalTravelValue > (1 - threshold))
                    {
                        canFinishTheLapFlag = true;
                    }
                }
            }
            /// <summary>
            /// Current lap progress
            /// </summary>
            /// <param name="newProgress"> new calculated progress </param>
            /// <param name="threshold">reverse cheeting threshold</param>
            public void NewLapProgressValue(float newProgress, float threshold) {

                if (finishedFlag)
                {
                    return;
                }

                if (Mathf.Abs(longestLegalTravelValue - newProgress) < threshold || !recievedFirstProgressFlag) {
                    agentsCurrentLapProgressValue = newProgress + agentsLapNumberValue;
                    if (!recievedFirstProgressFlag)
                    {
                        longestLegalTravelValue = newProgress;
                        recievedFirstProgressFlag = true;
                    }
                    CalculateLongestLegalTravelValue(threshold);

                }

            }
            /// <summary>
            /// The agent this block belongs to
            /// </summary>
            /// <param name="thisAgent">AI</param>
            public void AssignCurrentAgentController(AgentRaceController thisAgent) {
                myAgentControllerValue = thisAgent;
            }

        }
        //Variables
        /// <summary>
        /// where is finish trigger is placed
        /// </summary>
        public enum TriggerPlacementOptionValue { Ahead, Behind };
        /// <summary>
        /// Object
        /// </summary>
        [FormerlySerializedAs("whereIsTriggerIsPlaced")] public TriggerPlacementOptionValue whereIsCurrentTriggerIsPlaced;
        /// <summary>
        /// Finishing trigger
        /// </summary>
        [FormerlySerializedAs("finishTrigger")] [SerializeField] private Collider finishTriggerCollider;
        /// <summary>
        /// Reverse cheeting threshold fixes the winning of race by just revesing back to finish line
        /// </summary>
        [FormerlySerializedAs("reverseFinishLineCheetingThreshold")] [SerializeField] private float reverseFinishLineCheetingThresholdValue = 0.2f;
        /// <summary>
        /// total length of path
        /// </summary>
        private float totalDistValue = 0.0f;
        /// <summary>
        /// current path used to race on
        /// </summary>
        private Vector3[] currentPathValue;
        /// <summary>
        /// Agents racing in the race
        /// </summary>
        private AgentsInRaceInfosR[] agentsRacingInfos;
        /// <summary>
        /// Agents in order of their respective positions in race
        /// </summary>
        private AgentsInRaceInfosR[] agentsInOrderInfos;
        /// <summary>
        /// Whats the main camera // will be assigned to billboard heads
        /// </summary>
        [FormerlySerializedAs("currentMainCamera")] [SerializeField] private Transform currentMainCameraTransform;
        /// <summary>
        /// has the race been initiated
        /// </summary>
        private bool raceManagerInitiatedFlag = false;
        /// <summary>
        /// Indexes of path based on finish trigger
        /// </summary>
        private int[] indexesBasedOnPivotValue;
        /// <summary>
        /// Content of leaderboard scroll
        /// </summary>
        [FormerlySerializedAs("progressContent")] [SerializeField] private Transform progressContentTransform;
        /// <summary>
        /// Prefab of progress tab for leaderboard
        /// </summary>
        [FormerlySerializedAs("progressTab")] [SerializeField] private GameObject progressTabObject;
        /// <summary>
        /// Prefab of progress tab for leaderboard and this one is for human
        /// </summary>
        [FormerlySerializedAs("progressTabForHuman")] [SerializeField] private GameObject progressTabForHumanObject;
        /// <summary>
        /// Enable disable this progress board
        /// </summary>
        [FormerlySerializedAs("wholeProgressBoard")] [SerializeField] private GameObject wholeProgressBoardObject;
        [FormerlySerializedAs("autoHideProgressBoard")] [SerializeField] private bool autoHideProgressBoardFlag = true;
        
        /// <summary>
        /// End panel to show when you finish the race
        /// </summary>
        [FormerlySerializedAs("endPanel")] [SerializeField] private GameObject endPanelObject;
        /// <summary>
        /// Prefab for head info display
        /// </summary>
        [FormerlySerializedAs("headPosInfo")] [SerializeField] private GameObject headPosInfoObject;
        /// <summary>
        /// Names to be randomly assigned
        /// </summary>
        [FormerlySerializedAs("agentNames")] [SerializeField] private String[] agentNamesValue;
        /// <summary>
        /// How many agents are still racing
        /// </summary>
        private float remainingToFinishValue = 0;
        /// <summary>
        /// How many laps
        /// </summary>
        private int lapsNum = 1;
        /// <summary>
        /// Has the race concluded
        /// </summary>
        private bool isRaceConcludedFlag = false;
        private bool alwaysShowLeaderboardFlag = false;
        /// <summary>
        /// is a human racing
        /// </summary>
        private bool humanInRaceFlag = false;
        /// <summary>
        /// Id of human
        /// </summary>
        private int humanIDValue = 0;
        /// <summary>
        /// is this a circuit
        /// </summary>
        private bool onACircuitFlag;
        /// <summary>
        /// Add the object that hold the UI based controls
        /// </summary>
        [FormerlySerializedAs("mobileControlsHolder")] [SerializeField] private GameObject mobileControlsHolderObject;
        private bool playerFinishFlag = false;
        [SerializeField] private Text raceRewardText;
        [Inject] private PlayerDataManager playerDataManager;
        [Inject] private RaceRewardConfig raceRewardConfig;


        /// <summary>
        /// Initiates the race manager
        /// </summary>
        /// <param name="racingAgents">who are all the agents that are racing</param>
        /// <param name="path">path array</param>
        /// <param name="humanRacing">is a human racing</param>
        /// <param name="humanAt">index of human agent</param>
        /// <param name="noOfLaps">how many laps , if any?</param>
        /// <param name="isACircuit">is this a circuit</param>
        public void InitiateRacePathManager(AgentRaceController[] racingAgents, Vector3[] path, bool humanRacing, int humanAt , int noOfLaps,bool isACircuit)
        {

            onACircuitFlag = isACircuit;
            if (!isACircuit)
            {
                whereIsCurrentTriggerIsPlaced = TriggerPlacementOptionValue.Behind;
                lapsNum = 1;
            }
            else {
                lapsNum = noOfLaps;
            }


            totalDistValue = 0.0f;
            agentsRacingInfos = new AgentsInRaceInfosR[racingAgents.Length];
            currentPathValue = path;
            //will recaculate the path from new pivot
            if (isACircuit)
                RecalculateThePathFromPivot();

            GameObject temp = null;//ProgressTab
            GameObject temp2 = null;//HeadPosInfo

            List<String> tempNames = new List<String>(agentNamesValue);//Random Names
            int randChosen = 0;//Randomly Chosen Name
            
            //Catagorize of the agents
            for (int i = 0; i < agentsRacingInfos.Length; i++) {
                if (racingAgents[i])
                {
                    remainingToFinishValue++;
                    agentsRacingInfos[i] = new AgentsInRaceInfosR();
                    if (tempNames.Count > 0) {
                        randChosen = UnityEngine.Random.Range(0, tempNames.Count);
                        agentsRacingInfos[i].agentsNameValue = tempNames[randChosen];
                        tempNames.RemoveAt(randChosen);
                    }
                    else
                        agentsRacingInfos[i].agentsNameValue = racingAgents[i].gameObject.name;

                    if (i == humanAt) {
                        agentsRacingInfos[i].agentsNameValue = "Player";
                        humanIDValue = humanAt;
                        humanInRaceFlag = humanRacing;
                    }

                    agentsRacingInfos[i].agentIDValue = i;

                    agentsRacingInfos[i].AssignCurrentAgentController(racingAgents[i]);

                    if (whereIsCurrentTriggerIsPlaced == TriggerPlacementOptionValue.Behind) {
                        agentsRacingInfos[i].skipedFirstLapFlag = true;
                    }
                    //ProgressTabs
                    if (progressContentTransform && progressTabObject) {
                        if (progressTabForHumanObject && i == humanAt && humanRacing)
                            temp = Instantiate(progressTabForHumanObject) as GameObject;
                        else
                            temp = Instantiate(progressTabObject) as GameObject;
                        temp.transform.SetParent(progressContentTransform);
                        temp.transform.localScale = Vector3.one;
                        if (temp.GetComponent<AgentProgressTabRace>())
                        {
                            agentsRacingInfos[i].myProgressTabRace = temp.GetComponent<AgentProgressTabRace>();
                            agentsRacingInfos[i].myProgressTabRace.GiveNewName(agentsRacingInfos[i].agentsNameValue);
                            agentsRacingInfos[i].myProgressTabRace.UpdateProgressHud(i,false);
                        }
                    }
                    //Headinfo
                    if (headPosInfoObject && i != humanAt) {
                        temp2 = Instantiate(headPosInfoObject) as GameObject;
                        temp2.transform.SetParent(racingAgents[i].transform);
                        temp2.transform.localPosition = Vector3.zero;
                        if (temp2.GetComponent<BillboardRaceController>() && currentMainCameraTransform) {
                            temp2.GetComponent<BillboardRaceController>().GiveTheCam(currentMainCameraTransform);
                        }
                        if (agentsRacingInfos[i] != null && temp2.transform.GetComponentInChildren<Text>()) {
                            agentsRacingInfos[i].headPosInfoText = temp2.transform.GetComponentInChildren<Text>();
                            agentsRacingInfos[i].headPosInfoText.text = "";
                        }

                    }


                }
                else {
                    if (agentsRacingInfos[i] == null)
                        agentsRacingInfos[i] = new AgentsInRaceInfosR();
                    agentsRacingInfos[i].disqualifiedFlag = true;
                }
            }

            

            //Get the pathLength in meters
            if (currentPathValue.Length > 2)
            {
                for (int i = 1; i < currentPathValue.Length; i++)
                {
                    totalDistValue += (currentPathValue[i] - currentPathValue[i - 1]).magnitude;
                }

                agentsInOrderInfos = agentsRacingInfos;

                raceManagerInitiatedFlag = true;
                Debug.Log("Race Initiated - Total Path Length : " + totalDistValue);
            }
            else {
                Debug.Log("Race Can't be initiated because the path length is too small");
            }

            //Assign hud info
            if (transform.GetComponent<AgentHUDInformation>())
            {
                transform.GetComponent<AgentHUDInformation>().agentIDHUD = humanAt;
                transform.GetComponent<AgentHUDInformation>().EnableInfoHUD();
            }

            //Update all the displays of positions and names
            StartCoroutine(UpdatePositionDisplaysCoroutine());
        }

        /// <summary>
        /// Update the progress the an agent in the race
        /// </summary>
        /// <param name="agentIndex">at what index this agent is at?</param>
        /// <param name="pathPoint"> nearest path point</param>
        /// <param name="currentPos">agent's position</param>
        public void UpdateAgentsCurrentRaceLapPosition(int agentIndex, int pathPoint, Vector3 currentPos) {
            if (!raceManagerInitiatedFlag || agentsRacingInfos[agentIndex].disqualifiedFlag)
                return;
            pathPoint = GetNewIndexBasedOnPivotValue(pathPoint);
            float lapProgress = CalculatePathProgress(pathPoint, currentPos);

            agentsRacingInfos[agentIndex].NewLapProgressValue( Mathf.Clamp01(lapProgress / totalDistValue),reverseFinishLineCheetingThresholdValue);
        }

        //Calculate the new progress
        private float CalculatePathProgress(int pathPoint, Vector3 currentPos) {
            if (!raceManagerInitiatedFlag)
                return 0;
        
            float calculatedProgress = 0.0f;

            for (int i = 0; i < pathPoint - 1; i++)
            {
                calculatedProgress += (currentPathValue[i] - currentPathValue[i + 1]).magnitude;
            }
            calculatedProgress += (currentPathValue[pathPoint] - currentPos).magnitude;
            return calculatedProgress;
        }

        /// <summary>
        /// Reorder the racing agents based on their progress
        /// </summary>
        public void SortTheRaceAgents() {
            if (!raceManagerInitiatedFlag)
                return;
            List<AgentsInRaceInfosR> tempInOrder = new List<AgentsInRaceInfosR>(agentsRacingInfos);
            tempInOrder.Sort(AgentsPositionCompare);
            agentsInOrderInfos = tempInOrder.ToArray();

        }
        /// <summary>
        /// position comparer
        /// </summary>
        /// <param name="agent1"></param>
        /// <param name="agent2"></param>
        /// <returns></returns>
        private int AgentsPositionCompare(AgentsInRaceInfosR agent1, AgentsInRaceInfosR agent2) {
            return agent2.agentsCurrentLapProgressValue.CompareTo(agent1.agentsCurrentLapProgressValue);
        }
        /// <summary>
        /// Recalculate the path taking finish trigger as the pivot
        /// </summary>
        private void RecalculateThePathFromPivot() {
            if (!finishTriggerCollider)
                return;

            float minDist = Mathf.Infinity;
            int closestIndex = 0;

            indexesBasedOnPivotValue = new int[currentPathValue.Length];

            for (int i = 0; i < currentPathValue.Length; i++) {
                if ((finishTriggerCollider.transform.position - currentPathValue[i]).magnitude < minDist) {
                    closestIndex = i;
                    minDist = (finishTriggerCollider.transform.position - currentPathValue[i]).magnitude;
                }
            }

            //Debug.Log("Closest Index : " + closestIndex + " - " + currentPath.Length);
            
            int length = currentPathValue.Length;

            //Recalculating The CurrentPath
            Vector3[] pathClone = new Vector3[length];

            for (int i = 0; i < length; i++) {
                pathClone[i] = currentPathValue[closestIndex];
                indexesBasedOnPivotValue[closestIndex] = i;
                closestIndex++;
                if (closestIndex >= length) {
                    closestIndex = 0;
                }
            }

            currentPathValue = pathClone;
            
        }
        /// <summary>
        /// return the new index based on pivot
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private int GetNewIndexBasedOnPivotValue(int index) {
            if (indexesBasedOnPivotValue != null)
            {
                return indexesBasedOnPivotValue[index];

            }
            else
                return index;
        }
        /// <summary>
        /// This agent finished the lap
        /// </summary>
        /// <param name="thisAgent">pass the agent</param>
        public void AgentFinishedLap (AgentRaceController thisAgent){
            if (!raceManagerInitiatedFlag)
                return;

            foreach (AgentsInRaceInfosR currInfo in agentsRacingInfos) {
                if (currInfo.myAgentControllerValue == thisAgent)
                {
                    currInfo.FinishTheRaceLap(lapsNum,ref remainingToFinishValue , onACircuitFlag);

                    if (humanInRaceFlag && humanIDValue < agentsRacingInfos.Length) {
                        if (agentsRacingInfos[humanIDValue].finishedFlag) {
                            ShowEndPanelHud();
                            if (!playerFinishFlag) ShowPlayerPlace();
                        }
                    }

                    if (remainingToFinishValue == 0 && !isRaceConcludedFlag) {
                        RaceIsConcluded();
                    }

                    break;
                }
            }
        }
        /// <summary>
        /// Draw the pivot
        /// </summary>
        void OnDrawGizmos() {
            if (raceManagerInitiatedFlag) {
                Gizmos.color = Color.red;
                if (currentPathValue.Length > 0)
                Gizmos.DrawSphere(currentPathValue[0],1.0f);
            }

        }
        /// <summary>
        /// Update all the displays of positions and names
        /// </summary>
        /// <returns></returns>
        private IEnumerator UpdatePositionDisplaysCoroutine() {
            while (progressContentTransform && progressTabObject)
            {
                for (int i = 0; i < agentsInOrderInfos.Length; i++) {
                    if (agentsInOrderInfos[i].myProgressTabRace) {
                        agentsInOrderInfos[i].myProgressTabRace.UpdateProgressHud(i, agentsInOrderInfos[i].finishedFlag);
                    }
                    if (agentsInOrderInfos[i].headPosInfoText) {
                        agentsInOrderInfos[i].headPosInfoText.text = agentsInOrderInfos[i].agentsNameValue + " - " + (i+1).ToString();
                    }
                }
                //Tab to show the leaderboard
                if (wholeProgressBoardObject && !alwaysShowLeaderboardFlag && autoHideProgressBoardFlag)
                {
                    wholeProgressBoardObject.SetActive(Input.GetKey(KeyCode.Tab));
                }
                else if (wholeProgressBoardObject && !autoHideProgressBoardFlag) {
                    wholeProgressBoardObject.SetActive(true);
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
        public float GetTheLapNumberValue( int id) {
            if (raceManagerInitiatedFlag)
            {
                if (id >= agentsRacingInfos.Length)
                {
                    return 1;
                }
                else
                {
                    return agentsRacingInfos[id].agentsLapNumberValue;
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
        public int GetPositionInTheRaceHierarchy( int id) {

            if (raceManagerInitiatedFlag)
            {
                if (id < agentsRacingInfos.Length)
                {
                    for (int i = 0; i < agentsInOrderInfos.Length; i++)
                    {
                        if (agentsInOrderInfos[i].agentIDValue == id)
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
        public AICarDriverControl GetDriver(int id)
        {
            
            AICarDriverControl thisDriver = null;
            if (raceManagerInitiatedFlag)
            {
                if (id < agentsRacingInfos.Length)
                {
                    
                    for (int i = 0; i < agentsRacingInfos.Length; i++)
                    {
                        if (agentsRacingInfos[i].agentIDValue == id)
                        {
                            
                            if (agentsRacingInfos[i].myAgentControllerValue.GetComponent<AICarDriverControl>()) {
                                thisDriver = agentsRacingInfos[i].myAgentControllerValue.GetComponent<AICarDriverControl>();
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
        public int HowManyRaceLaps() {
            return lapsNum;
        }
        /// <summary>
        /// How many agents are racing
        /// </summary>
        /// <returns></returns>
        public int HowManyRacingAgents() {
            if (agentsRacingInfos != null)
                return agentsRacingInfos.Length;
            else
                return 1;
        }
        /// <summary>
        /// Show the end panel
        /// </summary>
        private void ShowEndPanelHud() {

            alwaysShowLeaderboardFlag = true;

            if (wholeProgressBoardObject && alwaysShowLeaderboardFlag)
            {
                wholeProgressBoardObject.SetActive(true);
            }

            if (endPanelObject) {
                endPanelObject.SetActive(true);
            }

            if (mobileControlsHolderObject) {
                mobileControlsHolderObject.SetActive(false);
            }

            if (transform.GetComponent<AgentHUDInformation>()) {
                transform.GetComponent<AgentHUDInformation>().DisableInfoHUD();
            }

        }
        /// <summary>
        /// Runs when the race is concluded
        /// </summary>
        private void RaceIsConcluded() {
            if (agentsInOrderInfos.Length > 0)
                Debug.Log("This race has been concluded and the winner is : " + agentsInOrderInfos[0].agentsNameValue);

            isRaceConcludedFlag = true;

            ShowEndPanelHud();
        }
        /// <summary>
        /// is race manager initiated
        /// </summary>
        /// <returns></returns>
        public bool IsTheRaceInitiated() {
            return raceManagerInitiatedFlag;
        }

        private void ShowPlayerPlace()
        {
            if (humanInRaceFlag && agentsInOrderInfos.Length > 0)
            {
                var playerName = agentsRacingInfos[humanIDValue].agentsNameValue;
                var playerPlace = -1;
                for (var i = 0; i < agentsInOrderInfos.Length; i++)
                    if (agentsInOrderInfos[i].agentsNameValue.Equals(playerName))
                    {
                        playerPlace = i;
                        break;
                    }

                if (raceRewardConfig != null && playerDataManager != null && playerPlace != -1)
                {
                    var reward = raceRewardConfig.GetReward(playerPlace);
                    playerDataManager.AddGold(reward);
                    if (reward > 0 && raceRewardText != null) raceRewardText.text = "+" + reward + "$";
                }
            }

            playerFinishFlag = true;
        }
    }
}