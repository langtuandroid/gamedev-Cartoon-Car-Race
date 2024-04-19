using negleft.AGS;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class MainMenuSceneController : MonoBehaviour
{
    [SerializeField] private Image playerCarImage;

    private int carCounter;
    private Text carNameText;

    [Inject] private VehiclesConfig vehiclesConfig;
    [Inject] private PlayerDataManager playerDataManager;

    private void Start()
    {
        PlayerPrefs.SetInt("InputType", 2);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void InitiateRaceConfig_Map1()
    {
        StartGame(1);
    }

    public void InitiateRaceConfig_Map2()
    {
        StartGame(2);
    }

    public void InitiateRaceConfig_Map3()
    {
        StartGame(3);
    }

    public void InitiateRaceConfig_Map4()
    {
        StartGame(4);
    }

    public void InitiateRaceConfig_Map5()
    {
        StartGame(5);
    }

    private void StartGame(int mapId)
    {
        GameObject raceInitiater = new GameObject("RaceInitiater");
        raceInitiater.AddComponent<AgentRaceStarterInitializer>();
        AgentRaceStarterInitializer init = raceInitiater.GetComponent<AgentRaceStarterInitializer>();

        int aitype = 9;
        int playerType = playerDataManager.SelectedVehicle - 1;
        int lap = 5;
        int aicount = 5;
        bool policeAgents = false;

        if (init)
            init.AssignVariables(aitype, playerType, lap, aicount, policeAgents, true);

        InitiateFader.CreateFader("CircuitRace_Map_" + mapId, Color.black, 2.0f);
    }
}
