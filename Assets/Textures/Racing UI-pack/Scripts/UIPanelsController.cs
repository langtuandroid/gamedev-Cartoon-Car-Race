using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class UIPanelsController : MonoBehaviour {

    [FormerlySerializedAs("optionsPanelAnim")] [SerializeField] private Animator optionsPanelAnimator;
    [FormerlySerializedAs("tournamentsPanelAnim")] [SerializeField] private  Animator tournamentsPanelAnimator;
    [FormerlySerializedAs("storePanelAnim")] [SerializeField] private  Animator storePanelAnimator;

    public void ShowOptionsPanel()
    {
        optionsPanelAnimator.gameObject.SetActive(!optionsPanelAnimator.gameObject.activeSelf);
    }

    public void ShowTournamentsPanel()
    {
        tournamentsPanelAnimator.gameObject.SetActive(!tournamentsPanelAnimator.gameObject.activeSelf);
    }

    public void ShowStorePanel()
    {
        storePanelAnimator.gameObject.SetActive(!storePanelAnimator.gameObject.activeSelf);
    }

    public void CloseOptionsPanel()
    {
        optionsPanelAnimator.SetTrigger("IsOff");
        Invoke("ShowOptionsPanel", 0.5f);
    }

    public void CloseTournamentsPanelPanel()
    {
        tournamentsPanelAnimator.SetTrigger("IsOff");
        Invoke("ShowTournamentsPanel", 0.5f);
    }

    public void CloseStorePanelPanel()
    {
        storePanelAnimator.SetTrigger("IsOff");
        Invoke("ShowStorePanel", 0.5f);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
