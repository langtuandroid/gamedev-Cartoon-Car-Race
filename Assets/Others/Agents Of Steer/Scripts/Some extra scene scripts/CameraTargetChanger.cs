using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    namespace negleft.AGS{
    public class CameraTargetChanger : MonoBehaviour {
        /// <summary>
        /// Targets to be followed by the camera
        /// </summary>
        public Transform[] targets;
        CameraScript myCamera;
        int currentTarget = 0;
        // Use this for initialization
        void Start () {
            //get the camera
            if (gameObject.GetComponent<CameraScript>()) {
                myCamera = gameObject.GetComponent<CameraScript>();
            }
        }

        /// <summary>
        /// Switch to previous target
        /// </summary>
        public void PrevTarget() {
            if (!myCamera)
                return;

            currentTarget--;
            if (currentTarget < 0)
                currentTarget = targets.Length-1;

            if (targets[currentTarget])
            myCamera.target = targets[currentTarget];
        }

        /// <summary>
        /// Switch to next target
        /// </summary>
        public void NextTarget()
        {
            if (!myCamera)
                return;

            currentTarget++;
            if (currentTarget >=  targets.Length)
                currentTarget = 0;

            if (targets[currentTarget])
                myCamera.target = targets[currentTarget];
        }

    }
}