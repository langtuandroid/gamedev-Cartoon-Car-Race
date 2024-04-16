using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour {

    public Animator optionsPanelAnim;
    public Animator tournamentsPanelAnim;
    public Animator storePanelAnim;

    public void ShowOptionsPanelButton()
    {
        optionsPanelAnim.gameObject.SetActive(!optionsPanelAnim.gameObject.activeSelf);
    }

    public void ShowTournamentsPanelButton()
    {
        tournamentsPanelAnim.gameObject.SetActive(!tournamentsPanelAnim.gameObject.activeSelf);
    }

    public void ShowStorePanelButton()
    {
        storePanelAnim.gameObject.SetActive(!storePanelAnim.gameObject.activeSelf);
    }

    public void CloseOptionsPanelButton()
    {
        optionsPanelAnim.SetTrigger("IsOff");
        Invoke("ShowOptionsPanelButton", 0.5f);
    }

    public void CloseTournamentsPanelPanelButton()
    {
        tournamentsPanelAnim.SetTrigger("IsOff");
        Invoke("ShowTournamentsPanelButton", 0.5f);
    }

    public void CloseStorePanelPanelButton()
    {
        storePanelAnim.SetTrigger("IsOff");
        Invoke("ShowStorePanelButton", 0.5f);
    }

    public void LoadSceneButton(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
