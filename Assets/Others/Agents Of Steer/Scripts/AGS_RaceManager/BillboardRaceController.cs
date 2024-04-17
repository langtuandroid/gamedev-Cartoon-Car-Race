using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
namespace negleft.AGS{
public class BillboardRaceController : MonoBehaviour {
        //Cam to look at
        private Transform cam;
        /// <summary>
        /// Minimum fade distance
        /// </summary>
        [FormerlySerializedAs("minFadeDist")] [SerializeField] private float minFadeDistValue = 5.0f;
        /// <summary>
        /// Maximum fade distance
        /// </summary>
        [FormerlySerializedAs("maxfadeDist")] [SerializeField] private float maxfadeDistValue = 10f;
        /// <summary>
        /// Canvas group to make the head display visible of invisible
        /// </summary>
        private CanvasGroup myGroup;

        /// <summary>
        /// Assign the camera
        /// </summary>
        /// <param name="suppCam">supplied the camera</param>
        public void GiveTheCam(Transform suppCam) {
            cam = suppCam;

            if (transform.GetComponentInChildren<CanvasGroup>())
                myGroup = transform.GetComponentInChildren<CanvasGroup>();

            StartCoroutine(TopDisplayCoroutine());
        }

        /// <summary>
        /// Update the look of the head display
        /// </summary>
        /// <returns></returns>
        private IEnumerator TopDisplayCoroutine() {
            float newAlpha = 0.0f;
            while (cam) {
                transform.LookAt(cam.transform);
                if (myGroup) {
                    newAlpha = (transform.position - cam.position).magnitude;
                    if (newAlpha > maxfadeDistValue) {
                        newAlpha = 0;
                    }
                    else {
                        newAlpha = minFadeDistValue / newAlpha;
                    }
                    myGroup.alpha = newAlpha;
                }
                yield return null;
            }
            yield return null;
        }

    }
}