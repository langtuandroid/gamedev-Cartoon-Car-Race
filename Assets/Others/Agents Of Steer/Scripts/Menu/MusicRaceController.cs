using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

    namespace negleft.AGS{
    public class MusicRaceController : MonoBehaviour {
        //current listner
        private Transform currListnerTransform = null;

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
            SceneManager.sceneLoaded += OnLevelFinishedLoadingListener;
        }
        //Start the routine
        private void OnLevelFinishedLoadingListener(Scene scene, LoadSceneMode mode) {
            StartCoroutine(LookForListnerCoroutine());
        }
        //Find the listner
        private IEnumerator LookForListnerCoroutine() {
            while (!currListnerTransform) {

                if (GameObject.FindObjectOfType<AudioListener>()) {
                    currListnerTransform = GameObject.FindObjectOfType<AudioListener>().transform;
                    StartCoroutine(GetAttachedCoroutine());
                }

                yield return null;
            }

            yield return null;
        }
        //stay with the listner
        private IEnumerator GetAttachedCoroutine()
        {
            while (currListnerTransform){
                transform.position = currListnerTransform.position;
                yield return null;
            }
            StartCoroutine(LookForListnerCoroutine());
            yield return null;
        }

    }
}