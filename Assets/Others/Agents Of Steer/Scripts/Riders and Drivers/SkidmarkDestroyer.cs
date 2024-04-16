using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    namespace negleft.AGS{
    /// <summary>
    /// Destroys skidmarks based on some values
    /// </summary>
    public class SkidmarkDestroyer : MonoBehaviour {
        /// <summary>
        /// Start fading the skidmark in
        /// </summary>
        public float destroyAfter = 5.0f;
        /// <summary>
        /// Fade out in this many seconds
        /// </summary>
        public float fadeIn = 5.0f;
        float lastTime = 0.0f;
        float timeCounter = 0.0f;
        LineRenderer myLine;
        // Use this for initialization
        void Start() {
            if (transform.GetComponent<LineRenderer>()) {
                myLine = transform.GetComponent<LineRenderer>();
            }
            StartCoroutine(Destroyer());
        }
        //Destroys skid marks by fading them
        IEnumerator Destroyer() {
            yield return new WaitForSeconds(destroyAfter);
            lastTime = Time.time;
            Color tempColor = myLine.material.color;
            while (true)
            {
                timeCounter += Time.time - lastTime;
                tempColor = Color.Lerp(tempColor, Color.clear, timeCounter / fadeIn);
                myLine.material.color = tempColor;
                if ((timeCounter / fadeIn) >= 1.0f) {
                    break;
                }
                lastTime = Time.time;
                yield return null;
            }

            Destroy(gameObject);
        }
    }
}