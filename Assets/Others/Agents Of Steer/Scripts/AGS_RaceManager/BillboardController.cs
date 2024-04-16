using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace negleft.AGS{
public class BillboardController : MonoBehaviour {
        //Cam to look at
        Transform cam;
        /// <summary>
        /// Minimum fade distance
        /// </summary>
        public float minFadeDist = 5.0f;
        /// <summary>
        /// Maximum fade distance
        /// </summary>
        public float maxfadeDist = 10f;
        /// <summary>
        /// Canvas group to make the head display visible of invisible
        /// </summary>
        CanvasGroup myGroup;

        /// <summary>
        /// Assign the camera
        /// </summary>
        /// <param name="suppCam">supplied the camera</param>
        public void GiveCam(Transform suppCam) {
            cam = suppCam;

            if (transform.GetComponentInChildren<CanvasGroup>())
                myGroup = transform.GetComponentInChildren<CanvasGroup>();

            StartCoroutine(TopDisplay());
        }

        /// <summary>
        /// Update the look of the head display
        /// </summary>
        /// <returns></returns>
        IEnumerator TopDisplay() {
            float newAlpha = 0.0f;
            while (cam) {
                transform.LookAt(cam.transform);
                if (myGroup) {
                    newAlpha = (transform.position - cam.position).magnitude;
                    if (newAlpha > maxfadeDist) {
                        newAlpha = 0;
                    }
                    else {
                        newAlpha = minFadeDist / newAlpha;
                    }
                    myGroup.alpha = newAlpha;
                }
                yield return null;
            }
            yield return null;
        }

    }
}