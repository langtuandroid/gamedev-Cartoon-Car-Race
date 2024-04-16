using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace negleft.AGS{
    /// <summary>
    /// Simple smooth follow of the camera
    /// </summary>
    public class CameraScript : MonoBehaviour {
        /// <summary>
        /// Current target of the camera
        /// </summary>
        public Transform target;
        /// <summary>
        /// Distance between target ad camera
        /// </summary>
        public float distance = 10.0f;
        /// <summary>
        /// Height difference between camera and target
        /// </summary>
        public float height = 3.0f;
        /// <summary>
        /// Height damping
        /// </summary>
        public float heightDamping = 2.0f;
        /// <summary>
        /// Rotation Damping
        /// </summary>
        public float rotationDamping = 3.0f;
        

        void LateUpdate()
        {//no target ,return
            if (!target)
                return;

            //Calculate camera's position
            float wantedRotationAngle = target.eulerAngles.y;
            float wantedHeight = target.position.y + height;

            float currentRotationAngle = transform.eulerAngles.y;
            float currentHeight = transform.position.y;

            currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);

            currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

            Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

            transform.position = target.position;
            transform.position -= currentRotation * Vector3.forward * distance;
            Vector3 temp = transform.position;
            temp.y = currentHeight;
            transform.position = temp;

            transform.LookAt(target);
        }
    }
}