using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
namespace negleft.AGS{
    public class FaderBehaviour : MonoBehaviour
    {
        [FormerlySerializedAs("start")] [HideInInspector]
        public bool startFlag = false;
        [FormerlySerializedAs("fadeDamp")] [HideInInspector]
        public float fadeDampValue = 0.0f;
        [FormerlySerializedAs("fadeScene")] [HideInInspector]
        public string fadeSceneName;
        [FormerlySerializedAs("alpha")] [HideInInspector]
        public float alphaValue = 0.0f;
        [FormerlySerializedAs("fadeColor")] [HideInInspector]
        public Color fadeColorValue;
        [FormerlySerializedAs("isFadeIn")] [HideInInspector]
        public bool isFadeInFlag = false;
        private CanvasGroup currCanvas;
        private Image bgImage;
        private float lastTimeValue = 0;
        private bool startedLoadingFlag = false;
        //Set callback
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnLevelFinishedLoadingListener;
        }
        //Remove callback
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnLevelFinishedLoadingListener;
        }

        public void InitiateFaderBehaviour()
        {

            DontDestroyOnLoad(gameObject);

            //Getting the visual elements
            if (transform.GetComponent<CanvasGroup>())
                currCanvas = transform.GetComponent<CanvasGroup>();

            if (transform.GetComponentInChildren<Image>())
            {
                bgImage = transform.GetComponent<Image>();
                bgImage.color = fadeColorValue;
            }
            //Checking and starting the coroutine
            if (currCanvas && bgImage)
            {
                currCanvas.alpha = 0.0f;
                StartCoroutine(FadeItCoroutine());
            }
            else
                Debug.LogWarning("Something is missing please reimport the package.");
        }

        private IEnumerator FadeItCoroutine()
        {

            while (!startFlag)
            {
                //waiting to start
                yield return null;
            }
            lastTimeValue = Time.time;
            float coDelta = lastTimeValue;
            bool hasFadedIn = false;

            while (!hasFadedIn)
            {
                coDelta = Time.time - lastTimeValue;
                if (!isFadeInFlag)
                {
                    //Fade in
                    alphaValue = newAlphaValue(coDelta, 1, alphaValue);
                    if (alphaValue == 1 && !startedLoadingFlag)
                    {
                        startedLoadingFlag = true;
                        SceneManager.LoadScene(fadeSceneName);
                    }

                }
                else
                {
                    //Fade out
                    alphaValue = newAlphaValue(coDelta, 0, alphaValue);
                    if (alphaValue == 0)
                    {
                        hasFadedIn = true;
                    }


                }
                lastTimeValue = Time.time;
                currCanvas.alpha = alphaValue;
                yield return null;
            }

            InitiateFader.DoneFadingFlag();

            Debug.Log("Your scene has been loaded , and fading in has just ended");

            Destroy(gameObject);

            yield return null;
        }


        private float newAlphaValue(float delta, int to, float currAlpha)
        {

            switch (to)
            {
                case 0:
                    currAlpha -= fadeDampValue * delta;
                    if (currAlpha <= 0)
                        currAlpha = 0;

                    break;
                case 1:
                    currAlpha += fadeDampValue * delta;
                    if (currAlpha >= 1)
                        currAlpha = 1;

                    break;
            }

            return currAlpha;
        }

        private void OnLevelFinishedLoadingListener(Scene scene, LoadSceneMode mode)
        {
            StartCoroutine(FadeItCoroutine());
            //We can now fade in
            isFadeInFlag = true;
        }
    }
}