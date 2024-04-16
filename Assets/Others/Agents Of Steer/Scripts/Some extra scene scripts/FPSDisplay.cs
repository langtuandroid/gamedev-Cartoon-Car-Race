using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace negleft.AGS{
    public class FPSDisplay : MonoBehaviour
    {
        //text to display fps to
        public Text fpsText;
        float deltaTime;

    private void Start() {
        DontDestroyOnLoad(gameObject);
    }

        // Update is called once per frame
        void Update()
        {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            float fps = 1.0f / deltaTime;
            if (fpsText)
                fpsText.text = "FPS : "  + Mathf.Ceil (fps).ToString ();
        }
    }
}