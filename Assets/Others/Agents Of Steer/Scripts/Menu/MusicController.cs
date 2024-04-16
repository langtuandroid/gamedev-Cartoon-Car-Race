using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

    namespace negleft.AGS{
    public class MusicController : MonoBehaviour {
        //current listner
        Transform currListner = null;

        /// <summary>
        /// dont destoy
        /// </summary>
        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
        //Call back
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnLevelFinishedLoading;
        }
        //Start the routine
        void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode) {
            StartCoroutine(LookForListner());
        }
        //Find the listner
        IEnumerator LookForListner() {
            while (!currListner) {

                if (GameObject.FindObjectOfType<AudioListener>()) {
                    currListner = GameObject.FindObjectOfType<AudioListener>().transform;
                    StartCoroutine(GetAttached());
                }

                yield return null;
            }

            yield return null;
        }
        //stay with the listner
        IEnumerator GetAttached()
        {
            while (currListner){
                transform.position = currListner.position;
                yield return null;
            }
            StartCoroutine(LookForListner());
            yield return null;
        }

    }
}