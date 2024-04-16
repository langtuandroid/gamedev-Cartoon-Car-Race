using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace negleft.AGS{
    public class AgentProgressTab : MonoBehaviour {
        /// <summary>
        /// Whats this agents position in race UI
        /// </summary>
        public Text myPos;
        /// <summary>
        /// Name of the agent UI
        /// </summary>
        public Text myName;
        // has this agent finished
        bool hasFinished = false;
        /// <summary>
        /// Give this agent a name UI
        /// </summary>
        /// <param name="newName"></param>
        public void GiveName(string newName) {
            if (myName)
                myName.text = newName;
        }

        /// <summary>
        /// Update the progress
        /// </summary>
        /// <param name="posInHierarchy">whats the pos</param>
        /// <param name="isFinished">has this agent finished</param>
        public void UpdateProgress(int posInHierarchy , bool isFinished) {
            if (myPos)
                myPos.text = (posInHierarchy +1).ToString();

            if (isFinished && myName && !hasFinished)
            {
                myName.text = myName.text + " - Finished";
                hasFinished = true;
            }

            transform.SetSiblingIndex(posInHierarchy);
        }
    }
}
