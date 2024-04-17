using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace negleft.AGS{
    public class CameraTargetChangerBehaviour : MonoBehaviour {
        /// <summary>
        /// Targets to be followed by the camera
        /// </summary>
        [FormerlySerializedAs("targets")] [SerializeField] private Transform[] targetsTransforms;
        private CameraFollowScript currCamera;
        private int currTarget = 0;
        // Use this for initialization
        private void Start () {
            //get the camera
            if (gameObject.GetComponent<CameraFollowScript>()) {
                currCamera = gameObject.GetComponent<CameraFollowScript>();
            }
        }

        /// <summary>
        /// Switch to previous target
        /// </summary>
        public void PrevTargetSwitch() {
            if (!currCamera)
                return;

            currTarget--;
            if (currTarget < 0)
                currTarget = targetsTransforms.Length-1;

            if (targetsTransforms[currTarget])
            currCamera.targetTransform = targetsTransforms[currTarget];
        }

        /// <summary>
        /// Switch to next target
        /// </summary>
        public void NextTargetSwitch()
        {
            if (!currCamera)
                return;

            currTarget++;
            if (currTarget >=  targetsTransforms.Length)
                currTarget = 0;

            if (targetsTransforms[currTarget])
                currCamera.targetTransform = targetsTransforms[currTarget];
        }

    }
}