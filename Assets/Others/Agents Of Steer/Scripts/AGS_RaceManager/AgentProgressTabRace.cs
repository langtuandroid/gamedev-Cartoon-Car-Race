using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
namespace negleft.AGS{
    public class AgentProgressTabRace : MonoBehaviour {
        /// <summary>
        /// Whats this agents position in race UI
        /// </summary>
        [FormerlySerializedAs("myPos")] [SerializeField] private Text myPosText;
        /// <summary>
        /// Name of the agent UI
        /// </summary>
        [FormerlySerializedAs("myName")] [SerializeField] private Text myNameText;
        // has this agent finished
        private bool hasFinished = false;
        /// <summary>
        /// Give this agent a name UI
        /// </summary>
        /// <param name="newName"></param>
        public void GiveNewName(string newName) {
            if (myNameText)
                myNameText.text = newName;
        }

        /// <summary>
        /// Update the progress
        /// </summary>
        /// <param name="posInHierarchy">whats the pos</param>
        /// <param name="isFinished">has this agent finished</param>
        public void UpdateProgressHud(int posInHierarchy , bool isFinished) {
            if (myPosText)
                myPosText.text = (posInHierarchy +1).ToString();

            if (isFinished && myNameText && !hasFinished)
            {
                myNameText.text = myNameText.text + " - Finished";
                hasFinished = true;
            }

            transform.SetSiblingIndex(posInHierarchy);
        }
    }
}
