using UnityEngine;
using System.Collections;
using UnityEngine.UI;
namespace negleft.AGS{
    public static class InitiateFader
    {
        static bool areWeFadingFlag = false;

        //Create Fader object and assing the fade scripts and assign all the variables
        public static void CreateFader(string scene, Color col, float multiplier)
        {
            if (areWeFadingFlag)
            {
                Debug.Log("Already Fading");
                return;
            }

            GameObject init = new GameObject();
            init.name = "Fader";
            Canvas myCanvas = init.AddComponent<Canvas>();
            myCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            myCanvas.sortingOrder = 10;
            init.AddComponent<FaderBehaviour>();
            init.AddComponent<CanvasGroup>();
            init.AddComponent<Image>();

            FaderBehaviour scr = init.GetComponent<FaderBehaviour>();
            scr.fadeDampValue = multiplier;
            scr.fadeSceneName = scene;
            scr.fadeColorValue = col;
            scr.startFlag = true;
            areWeFadingFlag = true;
            scr.InitiateFaderBehaviour();
            
        }

        public static void DoneFadingFlag() {
            areWeFadingFlag = false;
        }
    }
}