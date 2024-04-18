using negleft.AGS;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class MainMenuController : MonoBehaviour
{
    public Sprite[] playerCarsSprites;
    public Image playerCarImage;
    public string[] playerCarsNames;
    public int carCounter;
    public Text carNameText;

    [Inject] private VehiclesConfig vehiclesConfig;
    [Inject] private PlayerDataManager playerDataManager;

    void Start()
    {
        PlayerPrefs.SetInt("InputType", 2);

    }

    public void Quit()
    {
        Application.Quit();
    }

    public void SetNextCar()
    {
        if (carCounter >= playerCarsSprites.Length - 1)
            return;

        SetCar(+1);
    }

    public void SetPrevCar()
    {
        if (carCounter == 0)
            return;

        SetCar(-1);
    }

    private void SetCar(int value)
    {
        carCounter = carCounter + value;
        playerCarImage.sprite = playerCarsSprites[carCounter];
        carNameText.text = playerCarsNames[carCounter];
        PlayerPrefs.SetInt("PlayerCar", carCounter);
    }

    public void LoadSceneButton(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void InitiateNewRaceConfig_Map1()
    {
        GameObject raceInitiater = new GameObject("RaceInitiater");
        raceInitiater.AddComponent<AgentRaceStarterInitializer>();
        AgentRaceStarterInitializer init = raceInitiater.GetComponent<AgentRaceStarterInitializer>();

        int aitype = 9;
        int playerType = 0;
        int lap = 5;
        int aicount = 5;
        bool policeAgents = false;

        if (init)
            init.AssignVariables(aitype, playerType, lap, aicount, policeAgents, true);

        InitiateFader.CreateFader("CircuitRace_Map_1", Color.black, 2.0f);

    }

    public void InitiateNewRaceConfig_Map2()
    {
        GameObject raceInitiater = new GameObject("RaceInitiater");
        raceInitiater.AddComponent<AgentRaceStarterInitializer>();
        AgentRaceStarterInitializer init = raceInitiater.GetComponent<AgentRaceStarterInitializer>();

        int aitype = 9;
        int playerType = 1;
        int lap = 5;
        int aicount = 5;
        bool policeAgents = false;

        if (init)
            init.AssignVariables(aitype, playerType, lap, aicount, policeAgents, true);

        InitiateFader.CreateFader("CircuitRace_Map_2", Color.black, 2.0f);

    }

    public void InitiateNewRaceConfig_Map3()
    {
        GameObject raceInitiater = new GameObject("RaceInitiater");
        raceInitiater.AddComponent<AgentRaceStarterInitializer>();
        AgentRaceStarterInitializer init = raceInitiater.GetComponent<AgentRaceStarterInitializer>();

        int aitype = 9;
        int playerType = 2;
        int lap = 5;
        int aicount = 5;
        bool policeAgents = false;

        if (init)
            init.AssignVariables(aitype, playerType, lap, aicount, policeAgents, true);

        InitiateFader.CreateFader("CircuitRace_Map_3", Color.black, 2.0f);

    }
    
    public void InitiateNewRaceConfig_Map4()
    {
        GameObject raceInitiater = new GameObject("RaceInitiater");
        raceInitiater.AddComponent<AgentRaceStarterInitializer>();
        AgentRaceStarterInitializer init = raceInitiater.GetComponent<AgentRaceStarterInitializer>();

        int aitype = 9;
        int playerType = 3;
        int lap = 5;
        int aicount = 5;
        bool policeAgents = false;

        if (init)
            init.AssignVariables(aitype, playerType, lap, aicount, policeAgents, true);

        InitiateFader.CreateFader("CircuitRace_Map_4", Color.black, 2.0f);

    }
    
    public void InitiateNewRaceConfig_Map5()
    {
        GameObject raceInitiater = new GameObject("RaceInitiater");
        raceInitiater.AddComponent<AgentRaceStarterInitializer>();
        AgentRaceStarterInitializer init = raceInitiater.GetComponent<AgentRaceStarterInitializer>();

        int aitype = 9;
        int playerType = 4;
        int lap = 5;
        int aicount = 5;
        bool policeAgents = false;

        if (init)
            init.AssignVariables(aitype, playerType, lap, aicount, policeAgents, true);

        InitiateFader.CreateFader("CircuitRace_Map_5", Color.black, 2.0f);

    }
}
