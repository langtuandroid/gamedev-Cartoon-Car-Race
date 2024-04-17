using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace negleft.AGS{
    public class SirenAnimatorController : MonoBehaviour {
        /// <summary>
        /// My police agent
        /// </summary>
        [FormerlySerializedAs("myPoliceAgent")] [SerializeField] private AgentRacePolice currPoliceAgent;
        /// <summary>
        /// Siren Flares
        /// </summary>
        [FormerlySerializedAs("myFlares")] [SerializeField] private GameObject[] currFlares;

        /// <summary>
        /// Colors
        /// </summary>

        [FormerlySerializedAs("colourRed")]
        [ColorUsageAttribute(true,true)]
        [SerializeField] private Color colourRedValue;
        [FormerlySerializedAs("colourBlue")]
        [ColorUsageAttribute(true,true)]
        [SerializeField] private Color colourBlueValue;

        /// <summary>
        /// Siren Frequency
        /// </summary>
        [FormerlySerializedAs("sirenFrequency")] [SerializeField] private float sirenFrequencyValue = 10.0f;
        /// <summary>
        /// Max Siren brightness
        /// </summary>
        [FormerlySerializedAs("maxBrightness")] [SerializeField] private float maxBrightnessValue = 20.0f;

        [FormerlySerializedAs("forceSirenOn")] [SerializeField] private bool forceSirenOnFlag = false;

        // Use this for initialization
        private void Start () {

            StartCoroutine(AnimateSirenCoroutine());

            
        }


        private IEnumerator AnimateSirenCoroutine(){

                    //Give Colors
            for (int i = 0; i < currFlares.Length; i++)
                    {
                        if (currFlares[i] && currFlares[i].GetComponent<Renderer>())
                        {
                            currFlares[i].SetActive(true);
                            if (i%2 == 0)
                                currFlares[i].GetComponent<Renderer>().material.SetColor("_Color" , colourRedValue);
                            else
                                currFlares[i].GetComponent<Renderer>().material.SetColor("_Color" , colourBlueValue);

                        }
            }
            //-----
            while (currPoliceAgent || forceSirenOnFlag) {
                if (forceSirenOnFlag || currPoliceAgent.AreWePolicingFlag())
                {
                    for (int i = 0; i < currFlares.Length; i++)
                    {
                        if (currFlares[i])
                        {
                            currFlares[i].transform.rotation =Quaternion.identity;
                            currFlares[i].SetActive( Mathf.Sin((Time.time + i) * sirenFrequencyValue) > 0.5f);
                        }
                    }
                }
                else {
                    for (int i = 0; i < currFlares.Length; i++)
                    {
                        if (currFlares[i])
                        {
                            currFlares[i].SetActive(false);
                        }
                    }
                }

                yield return null;
            }
            yield return null;
        }
    }
}