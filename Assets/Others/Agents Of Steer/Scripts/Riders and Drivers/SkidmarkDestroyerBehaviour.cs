using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace negleft.AGS{
    /// <summary>
    /// Destroys skidmarks based on some values
    /// </summary>
    public class SkidmarkDestroyerBehaviour : MonoBehaviour {
        /// <summary>
        /// Start fading the skidmark in
        /// </summary>
        [FormerlySerializedAs("destroyAfter")] [SerializeField] private float destroyAfterValue = 5.0f;
        /// <summary>
        /// Fade out in this many seconds
        /// </summary>
        [FormerlySerializedAs("fadeIn")] [SerializeField] private float fadeInValue = 5.0f;
        private float lastTimeValue = 0.0f;
        private float timeCounterValue = 0.0f;
        private LineRenderer currLine;
        // Use this for initialization
        private void Start() {
            if (transform.GetComponent<LineRenderer>()) {
                currLine = transform.GetComponent<LineRenderer>();
            }
            StartCoroutine(DestroyerCoroutine());
        }
        //Destroys skid marks by fading them
        private IEnumerator DestroyerCoroutine() {
            yield return new WaitForSeconds(destroyAfterValue);
            lastTimeValue = Time.time;
            Color tempColor = currLine.material.color;
            while (true)
            {
                timeCounterValue += Time.time - lastTimeValue;
                tempColor = Color.Lerp(tempColor, Color.clear, timeCounterValue / fadeInValue);
                currLine.material.color = tempColor;
                if ((timeCounterValue / fadeInValue) >= 1.0f) {
                    break;
                }
                lastTimeValue = Time.time;
                yield return null;
            }

            Destroy(gameObject);
        }
    }
}