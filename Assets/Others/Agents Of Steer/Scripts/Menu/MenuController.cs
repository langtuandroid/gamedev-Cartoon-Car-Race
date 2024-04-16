using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

    namespace negleft.AGS{
    public class MenuController : MonoBehaviour {
        /// <summary>
        /// AI Count text display UI
        /// </summary>
        public Text AICount;
        /// <summary>
        /// AI Count Slider AI
        /// </summary>
        public Slider AICountSlider;
        /// <summary>
        /// Lap Count display UI
        /// </summary>
        public Text LapCount;
        /// <summary>
        /// Lap Count Slider
        /// </summary>
        public Slider LapCountSlider;
        /// <summary>
        /// AI Count
        /// </summary>
        int AI = 1;
        /// <summary>
        /// Lap Count
        /// </summary>
        int Lap = 2;
        /// <summary>
        /// Is it a circuit UI
        /// </summary>
        public Toggle circuitToggle;
        /// <summary>
        /// Car type for AI
        /// </summary>
        public Toggle[] AIType;
        /// <summary>
        /// Car type for Player
        /// </summary>
        public Toggle[] PlayerType;
        /// <summary>
        /// Car type for Player
        /// </summary>
        public Toggle includePolice;

        /// <summary>
        /// Music prefab for background music
        /// </summary>
        public GameObject musicPrefab;

        /// <summary>
        /// scene to load when circuit is selected
        /// </summary>
        public string circuitSceneName = "CircuitRace";
        /// <summary>
        /// scene to load when sprint is selected
        /// </summary>
        public string sprintSceneName = "SprintRace";
        /// <summary>
        /// Is this a mobile app
        /// </summary>
        public bool IsAMobileApp = false;

        /// <summary>
        /// Toggle controls for input type selection
        /// </summary>
        [System.Serializable]
        public struct ControlHandle {
            public Toggle tapAndAcc;
            public Toggle steerAndButtons;
        };
        /// <summary>
        /// Object for ControlHandle 
        /// </summary>
        public ControlHandle controlHandles;
        /// <summary>
        /// Prefab to display fps 
        /// </summary>
        public GameObject fpsDisplayPrefab;

        /// <summary>
        /// Initialize
        /// </summary>
        private void Start()
        {
            Application.targetFrameRate = 60;
            SpawnMusic();
            SpawnFPS();
            UpdateAICount();
            UpdateLapCount();
            UpdateInputOptions();
        }

        /// <summary>
        /// Updating the input
        /// </summary>
        public void UpdateInputOptions(){
            int type = PlayerPrefs.GetInt("InputType", 0);

            if (type == 0 && IsAMobileApp) {
                PlayerPrefs.SetInt("InputType", 1);
                type = 1;
            }

            switch (type) {
                case 1:
                    if (controlHandles.tapAndAcc) {
                        controlHandles.tapAndAcc.isOn = true;
                    }
                    break;
                case 2:
                    if (controlHandles.steerAndButtons)
                    {
                        controlHandles.steerAndButtons.isOn = true;
                    }
                    break;
            }

        }

        public void SetInputType(int newType) {
            PlayerPrefs.SetInt("InputType", newType);
        }


        /// <summary>
        /// Spawn the music prefab
        /// </summary>
        void SpawnMusic()
        {
            bool alreadyHasMusic = false;

            if (GameObject.FindObjectOfType<MusicController>()) {
                alreadyHasMusic = true;
            }

            if (!alreadyHasMusic && musicPrefab) {
                Instantiate(musicPrefab);
            }
        }

        /// <summary>
        /// Spawn the FPS prefab
        /// </summary>
        void SpawnFPS()
        {
            bool alreadyHasFPS = false;

            if (GameObject.FindObjectOfType<FPSDisplay>()) {
                alreadyHasFPS = true;
            }

            if (!alreadyHasFPS && fpsDisplayPrefab) {
                Instantiate(fpsDisplayPrefab);
            }
        }

        /// <summary>
        /// Update the AI count
        /// </summary>
        public void UpdateAICount() {
            if (AICount && AICountSlider) {
                AI = (int)AICountSlider.value;
                AICount.text = AI.ToString();
            }
        }

        /// <summary>
        /// Update the Lap count
        /// </summary>
        public void UpdateLapCount()
        {
            if (LapCount && LapCountSlider)
            {
                Lap = (int)LapCountSlider.value;
                LapCount.text = Lap.ToString();
            }
        }
        /// <summary>
        /// Initialize new AI race
        /// </summary>
        public void InitiateNewRaceConfig() {
            GameObject raceInitiater = new GameObject("RaceInitiater");
            raceInitiater.AddComponent<AgentRaceStarterInitiater>();
            AgentRaceStarterInitiater init = raceInitiater.GetComponent<AgentRaceStarterInitiater>();

            int aitype = 0;
            int playerType = 0;
            int lap = 1;
            int aicount = 1;
            bool policeAgents = true;

            if (includePolice) {
                if (!includePolice.isOn)
                    policeAgents = false;
            }

            //get the AI type
            for (int i = 0; i < AIType.Length; i++) {
                if (AIType[i]) {
                    if (AIType[i].isOn) {
                        aitype = i;
                        break;
                    }
                }
            }
            //get the player type
            for (int i = 0; i < PlayerType.Length; i++)
            {
                if (PlayerType[i])
                {
                    if (PlayerType[i].isOn)
                    {
                        playerType = i;
                        break;
                    }
                }
            }

            if (LapCountSlider)
                lap = (int)LapCountSlider.value;

            if (AICountSlider)
                aicount = (int)AICountSlider.value;

            if (init)
                init.AssignVars(aitype, playerType, lap, aicount,policeAgents,IsAMobileApp);
            //Load the appropriate map
            if (circuitToggle.isOn)
                Initiate.Fade(circuitSceneName, Color.black, 2.0f);
            else
                Initiate.Fade(sprintSceneName, Color.black, 2.0f);


        }

    }
}