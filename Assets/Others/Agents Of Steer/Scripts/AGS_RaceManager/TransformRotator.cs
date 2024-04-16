using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace negleft.AGS{
public class TransformRotator : MonoBehaviour {
        //Speed of rotation
        public float rotateSpeed = 0.2f;

        // Use this for initialization
        void Start() {
            StartCoroutine(RotateMe());
        }
        /// <summary>
        /// Roatate the transform on Y axis
        /// </summary>
        /// <returns></returns>
        IEnumerator RotateMe() {
            float coDeltaTime = Time.time;
            while (true) {
                coDeltaTime = Time.time - coDeltaTime;
                transform.Rotate(Vector3.up * (rotateSpeed * coDeltaTime));

                coDeltaTime = Time.time;
                yield return null;

            }
        }
    }
}