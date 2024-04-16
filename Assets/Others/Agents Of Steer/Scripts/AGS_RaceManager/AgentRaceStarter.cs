using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace negleft.AGS{
    [RequireComponent(typeof(AgentPathCreator))]
    public class AgentRaceStarter : MonoBehaviour {
        /// <summary>
        /// Initiate the race with default values
        /// </summary>
        public bool initiateAtStart = false;
        /// <summary>
        /// Start 321 count elements
        /// </summary>
        public Image[] startCountElements;
        /// <summary>
        /// Audio player
        /// </summary>
        public AudioSource audioPlayer;
        /// <summary>
        /// Count SFX
        /// </summary>
        public AudioClip countSound;
        /// <summary>
        /// Delay in count
        /// </summary>
        public float delay = 1.0f;
        /// <summary>
        /// Current path
        /// </summary>
        AgentPathCreator currentPath;
        /// <summary>
        /// Types of cars
        /// </summary>
        public GameObject[] carTypes;
        /// <summary>
        /// Spawn points for the cars
        /// </summary>
        public GameObject[] spawnPoints;
        /// <summary>
        /// Spawn point for player
        /// </summary>
        public GameObject playerSpawnPoint;
        /// <summary>
        /// Assign this camera to player
        /// </summary>
        public CameraScript cameraToBeAssigned;
        /// <summary>
        /// Spawn height above spawn point
        /// </summary>
        public float spawnAbove = 5.0f;
        /// <summary>
        /// holds police cars
        /// </summary>
        public GameObject policeHolder;
        /// <summary>
        /// UI input Objecy
        /// </summary>
        public SendUIInput uIInput;

        // Use this for initialization
        void Start () {
            //Start the race at play
            if (initiateAtStart)
                InitiateTheRaceStarter(3, 6, 4,2,true,false);

        }
        /// <summary>
        /// Initiate the race , spawn players and starts the countdown
        /// </summary>
        /// <param name="reqAIType">type of AI</param>
        /// <param name="noOfAIs">No. of AI's</param>
        /// <param name="reqPlayerType">player car type</param>
        /// <param name="noOfLaps">no. of laps</param>
        public void InitiateTheRaceStarter(int reqAIType , int noOfAIs , int reqPlayerType , int noOfLaps , bool hasPolice,bool isMobile) {
            currentPath = transform.GetComponent<AgentPathCreator>();

            if (policeHolder) {
                policeHolder.SetActive(hasPolice);
            }


            for (int i = 0; i < startCountElements.Length; i++)
            {
                if (startCountElements[i])
                {
                    startCountElements[i].enabled = false;
                }
            }
            SpawnCars(reqAIType, noOfAIs,reqPlayerType,isMobile);
            StartCoroutine(StartCountdown(noOfLaps));
        }
        /// <summary>
        /// Count down
        /// </summary>
        /// <param name="lapCount"></param>
        /// <returns></returns>
        IEnumerator StartCountdown(int lapCount) {
            Image lastActive = null;
            

            for (int i = 0; i < startCountElements.Length; i++)
            {

                yield return new WaitForSeconds(delay);

                if (lastActive)
                    lastActive.enabled = false;

                if (startCountElements[i])
                {
                    startCountElements[i].enabled = true;
                    lastActive = startCountElements[i];
                    if (audioPlayer && countSound) {
                        if (i == startCountElements.Length - 1)
                            audioPlayer.pitch = 2.0f;
                        audioPlayer.PlayOneShot(countSound);
                    }
                }

                
            }
            //starting the race here
            if (currentPath) {
                currentPath.StartTheRace(lapCount);
            }

            yield return new WaitForSeconds(delay);

            if (lastActive)
                lastActive.enabled = false;

            

            yield return null;
        }
        /// <summary>
        /// Spawn the cars
        /// </summary>
        /// <param name="AICarType"></param>
        /// <param name="MaxAI"></param>
        /// <param name="playerCarType"></param>
        void SpawnCars(int AICarType, int MaxAI, int playerCarType,bool isMobile) {
            if (MaxAI > spawnPoints.Length)
                MaxAI = spawnPoints.Length;
            GameObject currSpawnedCar = null;
            GameObject agentHolder = new GameObject("Agents_Racing");
            Debug.Log("Spawning");
            bool spawnRandom = false;
            List<GameObject> spawnedCars = new List<GameObject>() ;

            if (AICarType >= carTypes.Length)
                spawnRandom = true;

            int rand;
            int totalSpawned = 0;

            //spawn the AI
            for (int i = 0; i < MaxAI; i++) {

                rand = Random.Range(0, carTypes.Length);

                currSpawnedCar = null;

                if (spawnPoints[i]) {
                    if (!spawnRandom)
                    {
                        if (carTypes[AICarType])
                        currSpawnedCar = Instantiate(carTypes[AICarType], spawnPoints[i].transform.position+Vector3.up*spawnAbove, spawnPoints[i].transform.rotation) as GameObject;
                    }
                    else
                    {
                        if (carTypes[rand])
                            currSpawnedCar = Instantiate(carTypes[rand], spawnPoints[i].transform.position + Vector3.up * spawnAbove, spawnPoints[i].transform.rotation) as GameObject;

                    }
                    if (currSpawnedCar != null)
                    {
                        currSpawnedCar.transform.SetParent(agentHolder.transform);

                        if (currSpawnedCar.GetComponent<AgentController>() && currentPath)
                        {
                            currentPath.AddNewAgent(currSpawnedCar.GetComponent<AgentController>());
                            totalSpawned++;
                        }
                    }
                }

                if (currSpawnedCar)
                    spawnedCars.Add(currSpawnedCar);
            }

            if (playerCarType >= carTypes.Length)
                playerCarType = Random.Range(0, carTypes.Length);
            //Spawning Player
            if (carTypes.Length > 0)
            if (playerSpawnPoint && carTypes[playerCarType])
            {
                currSpawnedCar = Instantiate(carTypes[playerCarType], playerSpawnPoint.transform.position + Vector3.up * spawnAbove, playerSpawnPoint.transform.rotation) as GameObject;

                if (currSpawnedCar != null)
                    currSpawnedCar.transform.SetParent(agentHolder.transform);

                    if (currSpawnedCar.GetComponent<AICarDriver>())
                    {
                        if (uIInput)
                        uIInput.GiveDriver(currSpawnedCar.GetComponent<AICarDriver>());

                        currSpawnedCar.GetComponent<AICarDriver>().controlledByPlayer = true;
                        if (isMobile)
                            currSpawnedCar.GetComponent<AICarDriver>().SetNewControls();
                    }
                if (currSpawnedCar.GetComponent<AgentController>() && currentPath)
                {
                    currentPath.AddNewAgent(currSpawnedCar.GetComponent<AgentController>());
                    currentPath.PlayerSpawned(totalSpawned);
                }
            }
            else {
                rand = Random.Range(0, spawnedCars.Count);
                if (spawnedCars.Count > 0)
                    currSpawnedCar = spawnedCars[rand];
                else
                    currSpawnedCar = gameObject;
            }
            //assigning the camera
            if (cameraToBeAssigned) {
                    cameraToBeAssigned.target = currSpawnedCar.transform;
            }

        }
        
    }
}