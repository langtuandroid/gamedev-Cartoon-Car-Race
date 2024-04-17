using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
namespace negleft.AGS{
    [RequireComponent(typeof(AgentPathController))]
    public class AgentRaceStarterR : MonoBehaviour {
        /// <summary>
        /// Initiate the race with default values
        /// </summary>
        [FormerlySerializedAs("initiateAtStart")] [SerializeField] private bool initiateAtStartFlag = false;
        /// <summary>
        /// Start 321 count elements
        /// </summary>
        [FormerlySerializedAs("startCountElements")] [SerializeField] private Image[] startCountElementsImages;
        /// <summary>
        /// Audio player
        /// </summary>
        [FormerlySerializedAs("audioPlayer")] [SerializeField] private AudioSource audioPlayerSource;
        /// <summary>
        /// Count SFX
        /// </summary>
        [FormerlySerializedAs("countSound")] [SerializeField] private AudioClip countSoundClip;
        /// <summary>
        /// Delay in count
        /// </summary>
        [FormerlySerializedAs("delay")] [SerializeField] private float delayValue = 1.0f;
        /// <summary>
        /// Current path
        /// </summary>
        private AgentPathController currentPathAgent;
        /// <summary>
        /// Types of cars
        /// </summary>
        [FormerlySerializedAs("carTypes")] [SerializeField] private GameObject[] carTypesObjects;
        /// <summary>
        /// Spawn points for the cars
        /// </summary>
        [FormerlySerializedAs("spawnPoints")] [SerializeField] private GameObject[] spawnPointsObjects;
        /// <summary>
        /// Spawn point for player
        /// </summary>
        [FormerlySerializedAs("playerSpawnPoint")] [SerializeField] private GameObject playerSpawnPointObjects;
        /// <summary>
        /// Assign this camera to player
        /// </summary>
        [FormerlySerializedAs("cameraToBeAssigned")] [SerializeField] private CameraFollowScript cameraToBeAssignedPlayer;
        /// <summary>
        /// Spawn height above spawn point
        /// </summary>
        [FormerlySerializedAs("spawnAbove")] [SerializeField] private float spawnAboveValue = 5.0f;
        /// <summary>
        /// holds police cars
        /// </summary>
        [FormerlySerializedAs("policeHolder")] [SerializeField] private GameObject policeHolderObject;
        /// <summary>
        /// UI input Objecy
        /// </summary>
        [FormerlySerializedAs("uIInput")] [SerializeField] private SendMobileUIInput uIInputObject;

        // Use this for initialization
        private void Start () {
            //Start the race at play
            if (initiateAtStartFlag)
                InitiateTheRaceStarterObject(3, 6, 4,2,true,false);

        }
        /// <summary>
        /// Initiate the race , spawn players and starts the countdown
        /// </summary>
        /// <param name="reqAIType">type of AI</param>
        /// <param name="noOfAIs">No. of AI's</param>
        /// <param name="reqPlayerType">player car type</param>
        /// <param name="noOfLaps">no. of laps</param>
        public void InitiateTheRaceStarterObject(int reqAIType , int noOfAIs , int reqPlayerType , int noOfLaps , bool hasPolice,bool isMobile) {
            currentPathAgent = transform.GetComponent<AgentPathController>();

            if (policeHolderObject) {
                policeHolderObject.SetActive(hasPolice);
            }


            for (int i = 0; i < startCountElementsImages.Length; i++)
            {
                if (startCountElementsImages[i])
                {
                    startCountElementsImages[i].enabled = false;
                }
            }
            SpawnTheCars(reqAIType, noOfAIs,reqPlayerType,isMobile);
            StartCoroutine(StartCountdownCoroutine(noOfLaps));
        }
        /// <summary>
        /// Count down
        /// </summary>
        /// <param name="lapCount"></param>
        /// <returns></returns>
        private IEnumerator StartCountdownCoroutine(int lapCount) {
            Image lastActive = null;
            

            for (int i = 0; i < startCountElementsImages.Length; i++)
            {

                yield return new WaitForSeconds(delayValue);

                if (lastActive)
                    lastActive.enabled = false;

                if (startCountElementsImages[i])
                {
                    startCountElementsImages[i].enabled = true;
                    lastActive = startCountElementsImages[i];
                    if (audioPlayerSource && countSoundClip) {
                        if (i == startCountElementsImages.Length - 1)
                            audioPlayerSource.pitch = 2.0f;
                        audioPlayerSource.PlayOneShot(countSoundClip);
                    }
                }

                
            }
            //starting the race here
            if (currentPathAgent) {
                currentPathAgent.StartRace(lapCount);
            }

            yield return new WaitForSeconds(delayValue);

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
        private void SpawnTheCars(int AICarType, int MaxAI, int playerCarType,bool isMobile) {
            if (MaxAI > spawnPointsObjects.Length)
                MaxAI = spawnPointsObjects.Length;
            GameObject currSpawnedCar = null;
            GameObject agentHolder = new GameObject("Agents_Racing");
            Debug.Log("Spawning");
            bool spawnRandom = false;
            List<GameObject> spawnedCars = new List<GameObject>() ;

            if (AICarType >= carTypesObjects.Length)
                spawnRandom = true;

            int rand;
            int totalSpawned = 0;

            //spawn the AI
            for (int i = 0; i < MaxAI; i++) {

                rand = Random.Range(0, carTypesObjects.Length);

                currSpawnedCar = null;

                if (spawnPointsObjects[i]) {
                    if (!spawnRandom)
                    {
                        if (carTypesObjects[AICarType])
                        currSpawnedCar = Instantiate(carTypesObjects[AICarType], spawnPointsObjects[i].transform.position+Vector3.up*spawnAboveValue, spawnPointsObjects[i].transform.rotation) as GameObject;
                    }
                    else
                    {
                        if (carTypesObjects[rand])
                            currSpawnedCar = Instantiate(carTypesObjects[rand], spawnPointsObjects[i].transform.position + Vector3.up * spawnAboveValue, spawnPointsObjects[i].transform.rotation) as GameObject;

                    }
                    if (currSpawnedCar != null)
                    {
                        currSpawnedCar.transform.SetParent(agentHolder.transform);

                        if (currSpawnedCar.GetComponent<AgentRaceController>() && currentPathAgent)
                        {
                            currentPathAgent.AddNewRaceAgent(currSpawnedCar.GetComponent<AgentRaceController>());
                            totalSpawned++;
                        }
                    }
                }

                if (currSpawnedCar)
                    spawnedCars.Add(currSpawnedCar);
            }

            if (playerCarType >= carTypesObjects.Length)
                playerCarType = Random.Range(0, carTypesObjects.Length);
            //Spawning Player
            if (carTypesObjects.Length > 0)
            if (playerSpawnPointObjects && carTypesObjects[playerCarType])
            {
                currSpawnedCar = Instantiate(carTypesObjects[playerCarType], playerSpawnPointObjects.transform.position + Vector3.up * spawnAboveValue, playerSpawnPointObjects.transform.rotation) as GameObject;

                if (currSpawnedCar != null)
                    currSpawnedCar.transform.SetParent(agentHolder.transform);

                    if (currSpawnedCar.GetComponent<AICarDriverControl>())
                    {
                        if (uIInputObject)
                        uIInputObject.GiveCarDriver(currSpawnedCar.GetComponent<AICarDriverControl>());

                        currSpawnedCar.GetComponent<AICarDriverControl>().controlledByPlayerFlag = true;
                        if (isMobile)
                            currSpawnedCar.GetComponent<AICarDriverControl>().SetNewInputControls();
                    }
                if (currSpawnedCar.GetComponent<AgentRaceController>() && currentPathAgent)
                {
                    currentPathAgent.AddNewRaceAgent(currSpawnedCar.GetComponent<AgentRaceController>());
                    currentPathAgent.PlayerSpawnedAt(totalSpawned);
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
            if (cameraToBeAssignedPlayer) {
                    cameraToBeAssignedPlayer.targetTransform = currSpawnedCar.transform;
            }

        }
        
    }
}