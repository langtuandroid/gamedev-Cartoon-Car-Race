using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace negleft.AGS{
public class ObjectTransformRotator : MonoBehaviour {
        //Speed of rotation
        [FormerlySerializedAs("rotateSpeed")] [SerializeField] private float rotateSpeedValue = 0.2f;

        // Use this for initialization
        private void Start() {
            StartCoroutine(RotateMeCoroutine());
        }
        /// <summary>
        /// Roatate the transform on Y axis
        /// </summary>
        /// <returns></returns>
        private IEnumerator RotateMeCoroutine() {
            float coDeltaTime = Time.time;
            while (true) {
                coDeltaTime = Time.time - coDeltaTime;
                transform.Rotate(Vector3.up * (rotateSpeedValue * coDeltaTime));

                coDeltaTime = Time.time;
                yield return null;

            }
        }
    }
}