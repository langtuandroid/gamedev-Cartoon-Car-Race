using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
namespace negleft.AGS{
    public class FPSUIDisplay : MonoBehaviour
    {
        //text to display fps to
        [FormerlySerializedAs("fpsText")] [SerializeField] private Text fpsDisplayText;
        private float deltaTimeValue;

        private void Start() {
            DontDestroyOnLoad(gameObject);
        }

        // Update is called once per frame
        private void Update()
        {
            deltaTimeValue += (Time.deltaTime - deltaTimeValue) * 0.1f;
            float fps = 1.0f / deltaTimeValue;
            if (fpsDisplayText)
                fpsDisplayText.text = "FPS : " + Mathf.Ceil(fps).ToString();
        }
    }
}