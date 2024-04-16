using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace negleft.AGS{
    public class AgentRaceStarterInitiater : MonoBehaviour {
        //Config variables
        int aiType;
        int playerType;
        int lap = 1;
        int aiCount = 1;
        bool includedPolice = false;
        bool isMobile = false;
        //Set callback
        void OnEnable()
        {
            SceneManager.sceneLoaded += OnLevelFinishedLoading;
        }
        //Disable callback
        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        }
        /// <summary>
        /// Assing the variables for new race config
        /// </summary>
        /// <param name="AIType">AI type</param>
        /// <param name="PlayerType">Player type</param>
        /// <param name="Lap">laps</param>
        /// <param name="AICount">AI count</param>
        /// <param name="includePolice">map will contain police agents</param>
        public void AssignVars(int AIType, int PlayerType, int Lap, int AICount , bool includePolice,bool areWeOnAMobile)
        {
            DontDestroyOnLoad(gameObject);
            includedPolice = includePolice;
            aiType = AIType;
            playerType = PlayerType;
            lap = Lap;
            aiCount = AICount;
            isMobile = areWeOnAMobile;
        }
        /// <summary>
        /// Starting the race
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="mode"></param>
        void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
        {

            StartCoroutine(LookForManager());
            
        }

        IEnumerator LookForManager() {

            AgentRaceStarter myStarter = null;
            while (!myStarter)
            {
                if (FindObjectOfType<AgentRaceStarter>())
                {
                    myStarter = FindObjectOfType<AgentRaceStarter>();
                }
                yield return null;
            }

            if (myStarter)
            {
                myStarter.InitiateTheRaceStarter(aiType, aiCount, playerType, lap,includedPolice,isMobile);
            }

            Destroy(gameObject);

            yield return null;
        }
    }
}
