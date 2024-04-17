using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace negleft.AGS{
    /// <summary>
    /// Simple smooth follow of the camera
    /// </summary>
    public class CameraFollowScript : MonoBehaviour {
        /// <summary>
        /// Current target of the camera
        /// </summary>
        [FormerlySerializedAs("target")] public Transform targetTransform;
        /// <summary>
        /// Distance between target ad camera
        /// </summary>
        [FormerlySerializedAs("distance")] [SerializeField] private float distanceValue = 10.0f;
        /// <summary>
        /// Height difference between camera and target
        /// </summary>
        [FormerlySerializedAs("height")] [SerializeField] private float heightValue = 3.0f;
        /// <summary>
        /// Height damping
        /// </summary>
        [FormerlySerializedAs("heightDamping")] [SerializeField] private float heightDampingValue = 2.0f;
        /// <summary>
        /// Rotation Damping
        /// </summary>
        [FormerlySerializedAs("rotationDamping")] [SerializeField] private float rotationDampingValue = 3.0f;
        

        private void LateUpdate()
        {//no target ,return
            if (!targetTransform)
                return;

            //Calculate camera's position
            float wantedRotationAngle = targetTransform.eulerAngles.y;
            float wantedHeight = targetTransform.position.y + heightValue;

            float currentRotationAngle = transform.eulerAngles.y;
            float currentHeight = transform.position.y;

            currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDampingValue * Time.deltaTime);

            currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDampingValue * Time.deltaTime);

            Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

            transform.position = targetTransform.position;
            transform.position -= currentRotation * Vector3.forward * distanceValue;
            Vector3 temp = transform.position;
            temp.y = currentHeight;
            transform.position = temp;

            transform.LookAt(targetTransform);
        }
    }
}