using negleft.AGS;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public Sprite[] playerCarsSprites;
    public Image playerCarImage;
    public string[] playerCarsNames;
    public int carCounter;
    public Text carNameText;

    void Start()
    {
        PlayerPrefs.SetInt("InputType", 2);

    }

    void Update()
    {

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
        raceInitiater.AddComponent<AgentRaceStarterInitiater>();
        AgentRaceStarterInitiater init = raceInitiater.GetComponent<AgentRaceStarterInitiater>();

        int aitype = 10;
        int playerType = UnityEngine.Random.Range(0, aitype);
        int lap = 5;
        int aicount = 5;
        bool policeAgents = false;

        if (init)
            init.AssignVars(aitype, playerType, lap, aicount, policeAgents, true);

        Initiate.Fade("CircuitRace_Map_1", Color.black, 2.0f);

    }

    public void InitiateNewRaceConfig_Map2()
    {
        GameObject raceInitiater = new GameObject("RaceInitiater");
        raceInitiater.AddComponent<AgentRaceStarterInitiater>();
        AgentRaceStarterInitiater init = raceInitiater.GetComponent<AgentRaceStarterInitiater>();

        int aitype = 6;
        int playerType = UnityEngine.Random.Range(0, aitype);
        int lap = 5;
        int aicount = 5;
        bool policeAgents = false;

        if (init)
            init.AssignVars(aitype, playerType, lap, aicount, policeAgents, true);

        Initiate.Fade("CircuitRace_Map_2", Color.black, 2.0f);

    }

    public void InitiateNewRaceConfig_Map3()
    {
        GameObject raceInitiater = new GameObject("RaceInitiater");
        raceInitiater.AddComponent<AgentRaceStarterInitiater>();
        AgentRaceStarterInitiater init = raceInitiater.GetComponent<AgentRaceStarterInitiater>();

        int aitype = 16;
        int playerType = UnityEngine.Random.Range(0, aitype);
        int lap = 5;
        int aicount = 5;
        bool policeAgents = false;

        if (init)
            init.AssignVars(aitype, playerType, lap, aicount, policeAgents, true);

        Initiate.Fade("CircuitRace_Map_3", Color.black, 2.0f);

    }
}
