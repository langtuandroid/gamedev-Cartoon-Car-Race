using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace negleft.AGS{
    /// <summary>
    /// Destroys the particle system after it stops playing
    /// </summary>
    public class ParticleKillerBehaviour : MonoBehaviour {
        private ParticleSystem pSystem;
        // Use this for initialization
        private void Start () {
            if (transform.GetComponent<ParticleSystem>())
                pSystem = transform.GetComponent<ParticleSystem>();
            if (pSystem)
                StartCoroutine(KillCoroutine());
        }
        //Killing routine
        private IEnumerator KillCoroutine() {
            while (pSystem) {
                if (!pSystem.IsAlive())
                    Destroy(gameObject);
                yield return null;
            }
            yield return null;
        }

        
    }
}
