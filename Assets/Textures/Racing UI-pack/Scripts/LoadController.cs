using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadController : MonoBehaviour
{

    IEnumerator Start()
    {
        yield return new WaitForSeconds(3f);

        AsyncOperation ao = SceneManager.LoadSceneAsync("MainMenu");
        ao.allowSceneActivation = false;

        while (!ao.isDone)
        {
            float progress = Mathf.Clamp01(ao.progress / 0.9f);
            if (ao.progress == 0.9f)
            {
                ao.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
