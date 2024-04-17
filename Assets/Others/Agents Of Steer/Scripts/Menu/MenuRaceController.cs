using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

    namespace negleft.AGS{
    public class MenuRaceController : MonoBehaviour {
        /// <summary>
        /// AI Count text display UI
        /// </summary>
        [FormerlySerializedAs("AICount")] [SerializeField] private Text AICountText;
        /// <summary>
        /// AI Count Slider AI
        /// </summary>
        [FormerlySerializedAs("AICountSlider")] [SerializeField] private Slider AICountUISlider;
        /// <summary>
        /// Lap Count display UI
        /// </summary>
        [FormerlySerializedAs("LapCount")] [SerializeField] private Text LapCountText;
        /// <summary>
        /// Lap Count Slider
        /// </summary>
        [FormerlySerializedAs("LapCountSlider")] [SerializeField] private Slider LapCountUISlider;
        /// <summary>
        /// AI Count
        /// </summary>
        private int AICount = 1;
        /// <summary>
        /// Lap Count
        /// </summary>
        private int LapCount = 2;
        /// <summary>
        /// Is it a circuit UI
        /// </summary>
        [FormerlySerializedAs("circuitToggle")] [SerializeField] private Toggle circuitUIToggle;
        /// <summary>
        /// Car type for AI
        /// </summary>
        [FormerlySerializedAs("AIType")] [SerializeField] private Toggle[] AITypes;
        /// <summary>
        /// Car type for Player
        /// </summary>
        [FormerlySerializedAs("PlayerType")] [SerializeField] private Toggle[] PlayerTypes;
        /// <summary>
        /// Car type for Player
        /// </summary>
        [FormerlySerializedAs("includePolice")] [SerializeField] private Toggle includePoliceToggle;

        /// <summary>
        /// Music prefab for background music
        /// </summary>
        [FormerlySerializedAs("musicPrefab")] [SerializeField] private GameObject musicBackgroundPrefab;

        /// <summary>
        /// scene to load when circuit is selected
        /// </summary>
        [FormerlySerializedAs("circuitSceneName")] [SerializeField] private string circuitSceneNameValue = "CircuitRace";
        /// <summary>
        /// scene to load when sprint is selected
        /// </summary>
        [FormerlySerializedAs("sprintSceneName")] [SerializeField] private string sprintSceneNameValue = "SprintRace";
        /// <summary>
        /// Is this a mobile app
        /// </summary>
        [FormerlySerializedAs("IsAMobileApp")] [SerializeField] private bool IsAMobileAppFlag = false;

        /// <summary>
        /// Toggle controls for input type selection
        /// </summary>
        [System.Serializable]
        public struct ControlInputHandle {
            public Toggle tapAndAcc;
            public Toggle steerAndButtons;
        };
        /// <summary>
        /// Object for ControlHandle 
        /// </summary>
        [FormerlySerializedAs("controlHandles")] [SerializeField] private ControlInputHandle controlInputHandles;
        /// <summary>
        /// Prefab to display fps 
        /// </summary>
        [FormerlySerializedAs("fpsDisplayPrefab")] [SerializeField] private GameObject fpsDisplayObjectPrefab;

        /// <summary>
        /// Initialize
        /// </summary>
        private void Start()
        {
            Application.targetFrameRate = 60;
            SpawnBackgroundMusic();
            SpawnFPSPrefab();
            UpdateAICountValue();
            UpdateLapCountValue();
            UpdateInputOptionsData();
        }

        /// <summary>
        /// Updating the input
        /// </summary>
        public void UpdateInputOptionsData(){
            int type = PlayerPrefs.GetInt("InputType", 0);

            if (type == 0 && IsAMobileAppFlag) {
                PlayerPrefs.SetInt("InputType", 1);
                type = 1;
            }

            switch (type) {
                case 1:
                    if (controlInputHandles.tapAndAcc) {
                        controlInputHandles.tapAndAcc.isOn = true;
                    }
                    break;
                case 2:
                    if (controlInputHandles.steerAndButtons)
                    {
                        controlInputHandles.steerAndButtons.isOn = true;
                    }
                    break;
            }

        }

        public void SetNewInputType(int newType) {
            PlayerPrefs.SetInt("InputType", newType);
        }


        /// <summary>
        /// Spawn the music prefab
        /// </summary>
        private void SpawnBackgroundMusic()
        {
            bool alreadyHasMusic = false;

            if (GameObject.FindObjectOfType<MusicRaceController>()) {
                alreadyHasMusic = true;
            }

            if (!alreadyHasMusic && musicBackgroundPrefab) {
                Instantiate(musicBackgroundPrefab);
            }
        }

        /// <summary>
        /// Spawn the FPS prefab
        /// </summary>
        private void SpawnFPSPrefab()
        {
            bool alreadyHasFPS = false;

            if (GameObject.FindObjectOfType<FPSUIDisplay>()) {
                alreadyHasFPS = true;
            }

            if (!alreadyHasFPS && fpsDisplayObjectPrefab) {
                Instantiate(fpsDisplayObjectPrefab);
            }
        }

        /// <summary>
        /// Update the AI count
        /// </summary>
        public void UpdateAICountValue() {
            if (AICountText && AICountUISlider) {
                AICount = (int)AICountUISlider.value;
                AICountText.text = AICount.ToString();
            }
        }

        /// <summary>
        /// Update the Lap count
        /// </summary>
        public void UpdateLapCountValue()
        {
            if (LapCountText && LapCountUISlider)
            {
                LapCount = (int)LapCountUISlider.value;
                LapCountText.text = LapCount.ToString();
            }
        }
        /// <summary>
        /// Initialize new AI race
        /// </summary>
        public void InitiateNewAIRaceConfig() {
            GameObject raceInitiater = new GameObject("RaceInitiater");
            raceInitiater.AddComponent<AgentRaceStarterInitializer>();
            AgentRaceStarterInitializer init = raceInitiater.GetComponent<AgentRaceStarterInitializer>();

            int aitype = 0;
            int playerType = 0;
            int lap = 1;
            int aicount = 1;
            bool policeAgents = true;

            if (includePoliceToggle) {
                if (!includePoliceToggle.isOn)
                    policeAgents = false;
            }

            //get the AI type
            for (int i = 0; i < AITypes.Length; i++) {
                if (AITypes[i]) {
                    if (AITypes[i].isOn) {
                        aitype = i;
                        break;
                    }
                }
            }
            //get the player type
            for (int i = 0; i < PlayerTypes.Length; i++)
            {
                if (PlayerTypes[i])
                {
                    if (PlayerTypes[i].isOn)
                    {
                        playerType = i;
                        break;
                    }
                }
            }

            if (LapCountUISlider)
                lap = (int)LapCountUISlider.value;

            if (AICountUISlider)
                aicount = (int)AICountUISlider.value;

            if (init)
                init.AssignVariables(aitype, playerType, lap, aicount,policeAgents,IsAMobileAppFlag);
            //Load the appropriate map
            if (circuitUIToggle.isOn)
                InitiateFader.CreateFader(circuitSceneNameValue, Color.black, 2.0f);
            else
                InitiateFader.CreateFader(sprintSceneNameValue, Color.black, 2.0f);


        }

    }
}