using System.Linq;
using negleft.AGS;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class MainMenuSceneController : MonoBehaviour
{
    [SerializeField] private Text playerGoldText;
    [SerializeField] private Image playerVehicleImage;
    [SerializeField] private Text playerVehiclePriceText;
    [SerializeField] private Button buyVehicleButton;
    [SerializeField] private Button selectVehicleButton;
    [SerializeField] private GameObject vehicleCheckImage;

    private int currentVehicle = 0;

    [Inject] private VehiclesConfig vehiclesConfig;
    [Inject] private PlayerDataManager playerDataManager;

    private void Start()
    {
        PlayerPrefs.SetInt("InputType", 2);

        currentVehicle = playerDataManager.SelectedVehicle;
        UpdateVehicleUi();
        buyVehicleButton.onClick.AddListener(BuyVehicle);
        selectVehicleButton.onClick.AddListener(SelectVehicle);
    }

    public void NextVehicle()
    {
        if (currentVehicle >= vehiclesConfig.Vehicles.Count - 1) return;
        currentVehicle++;
        UpdateVehicleUi();
    }

    public void PrevVehicle()
    {
        if (currentVehicle <= 0) return;
        currentVehicle--;
        UpdateVehicleUi();
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
        int playerType = playerDataManager.SelectedVehicle;
        int lap = 2;
        int aicount = 5;
        bool policeAgents = false;

        if (init)
            init.AssignVariables(aitype, playerType, lap, aicount, policeAgents, true);

        InitiateFader.CreateFader("CircuitRace_Map_" + mapId, Color.black, 2.0f);
    }

    private void BuyVehicle()
    {
        if (playerDataManager.TryBuyVehicle(currentVehicle)) SelectVehicle();
    }

    private void SelectVehicle()
    {
        playerDataManager.SelectVehicle(currentVehicle);
        UpdateVehicleUi();
    }

    private void UpdateVehicleUi()
    {
        playerGoldText.text = playerDataManager.Gold.ToString();
        var vehicleStatus = playerDataManager.CheckPurchasedVehicle(currentVehicle);
        buyVehicleButton.gameObject.SetActive(!vehicleStatus);
        selectVehicleButton.gameObject.SetActive(vehicleStatus);
        vehicleCheckImage.SetActive(playerDataManager.SelectedVehicle == currentVehicle);
        var vehicle = vehiclesConfig.Vehicles.FirstOrDefault(v => v.Id == currentVehicle);
        playerVehicleImage.sprite = vehicle.Sprite;
        if (vehicleStatus) playerVehiclePriceText.gameObject.SetActive(false);
        else
        {
            playerVehiclePriceText.gameObject.SetActive(true);
            playerVehiclePriceText.text = vehicle.Price + "$";
        }
    }
}
