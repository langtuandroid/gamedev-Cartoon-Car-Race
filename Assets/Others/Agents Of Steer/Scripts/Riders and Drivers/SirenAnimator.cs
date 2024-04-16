using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    namespace negleft.AGS{
    public class SirenAnimator : MonoBehaviour {
        /// <summary>
        /// My police agent
        /// </summary>
        public AgentPolice myPoliceAgent;
        /// <summary>
        /// Siren Flares
        /// </summary>
        public GameObject[] myFlares;

        /// <summary>
        /// Colors
        /// </summary>

        [ColorUsageAttribute(true,true)]
        public Color colourRed;
        [ColorUsageAttribute(true,true)]
        public Color colourBlue;

        /// <summary>
        /// Siren Frequency
        /// </summary>
        public float sirenFrequency = 10.0f;
        /// <summary>
        /// Max Siren brightness
        /// </summary>
        public float maxBrightness = 20.0f;

        public bool forceSirenOn = false;

        // Use this for initialization
        void Start () {

            StartCoroutine(AnimateSiren());

            
        }


        IEnumerator AnimateSiren(){

                    //Give Colors
            for (int i = 0; i < myFlares.Length; i++)
                    {
                        if (myFlares[i] && myFlares[i].GetComponent<Renderer>())
                        {
                            myFlares[i].SetActive(true);
                            if (i%2 == 0)
                                myFlares[i].GetComponent<Renderer>().material.SetColor("_Color" , colourRed);
                            else
                                myFlares[i].GetComponent<Renderer>().material.SetColor("_Color" , colourBlue);

                        }
            }
            //-----
            while (myPoliceAgent || forceSirenOn) {
                if (forceSirenOn || myPoliceAgent.AreWePolicing())
                {
                    for (int i = 0; i < myFlares.Length; i++)
                    {
                        if (myFlares[i])
                        {
                            myFlares[i].transform.rotation =Quaternion.identity;
                            myFlares[i].SetActive( Mathf.Sin((Time.time + i) * sirenFrequency) > 0.5f);
                        }
                    }
                }
                else {
                    for (int i = 0; i < myFlares.Length; i++)
                    {
                        if (myFlares[i])
                        {
                            myFlares[i].SetActive(false);
                        }
                    }
                }

                yield return null;
            }
            yield return null;
        }
    }
}