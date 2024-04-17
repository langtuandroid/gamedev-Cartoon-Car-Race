﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace negleft.AGS{
public class SimpleSceneBehaviourController : MonoBehaviour {
        /// <summary>
        /// Load tis scene
        /// </summary>
        /// <param name="scene"></param>
        public void LoadThisScene(string scene) {
            InitiateFader.CreateFader(scene, Color.black, 2.0f);
        }
        /// <summary>
        /// open this link
        /// </summary>
        public void LogoLink() {
            #if !UNITY_EDITOR
                Debug.Log("Why did you click the logo?");
            #endif
        }
        /// <summary>
        /// Quit the application
        /// </summary>
        public void ExitGame() {
            Debug.Log("Quittin'");
            Application.Quit();
        }

    }
}