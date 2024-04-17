using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace negleft.AGS{
    public class AgentRaceStarterInitializer : MonoBehaviour {
        //Config variables
        private int aiTypeValue;
        private int playerTypeValue;
        private int lapNum = 1;
        private int aiCountNum = 1;
        private bool includedPoliceFlag = false;
        private bool isMobileFlag = false;
        //Set callback
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnLevelFinishedLoadingListener;
        }
        //Disable callback
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnLevelFinishedLoadingListener;
        }
        /// <summary>
        /// Assing the variables for new race config
        /// </summary>
        /// <param name="AIType">AI type</param>
        /// <param name="PlayerType">Player type</param>
        /// <param name="Lap">laps</param>
        /// <param name="AICount">AI count</param>
        /// <param name="includePolice">map will contain police agents</param>
        public void AssignVariables(int AIType, int PlayerType, int Lap, int AICount , bool includePolice,bool areWeOnAMobile)
        {
            DontDestroyOnLoad(gameObject);
            includedPoliceFlag = includePolice;
            aiTypeValue = AIType;
            playerTypeValue = PlayerType;
            lapNum = Lap;
            aiCountNum = AICount;
            isMobileFlag = areWeOnAMobile;
        }
        /// <summary>
        /// Starting the race
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="mode"></param>
        private void OnLevelFinishedLoadingListener(Scene scene, LoadSceneMode mode)
        {
            StartCoroutine(LookForManagerCoroutine());
        }

        private IEnumerator LookForManagerCoroutine() {

            AgentRaceStarterR myStarter = null;
            while (!myStarter)
            {
                if (FindObjectOfType<AgentRaceStarterR>())
                {
                    myStarter = FindObjectOfType<AgentRaceStarterR>();
                }
                yield return null;
            }

            if (myStarter)
            {
                myStarter.InitiateTheRaceStarterObject(aiTypeValue, aiCountNum, playerTypeValue, lapNum,includedPoliceFlag,isMobileFlag);
            }

            Destroy(gameObject);

            yield return null;
        }
    }
}
