using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace negleft.AGS{
public class SimpleSceneController : MonoBehaviour {
        /// <summary>
        /// Load tis scene
        /// </summary>
        /// <param name="scene"></param>
        public void LoadThis(string scene) {
            Initiate.Fade(scene, Color.black, 2.0f);
        }
        /// <summary>
        /// open this link
        /// </summary>
        public void Link() {
            #if !UNITY_EDITOR
                Debug.Log("Why did you click the logo?");
            #endif
        }
        /// <summary>
        /// Quit the application
        /// </summary>
        public void Exit() {
            Debug.Log("Quittin'");
            Application.Quit();
        }

    }
}