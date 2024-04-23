using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MainMenuSceneController : MonoBehaviour
{
    [SerializeField] private GameObject chaptersPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject garagePanel;

    [SerializeField] private ImageSwitcher soundImageSwitcher;
    [SerializeField] private ImageSwitcher musicImageSwitcher;

    [SerializeField] private Text playerGoldText;

    [SerializeField] private VehicleItem vehicleItem;
    [SerializeField] private Transform vehiclesContent;

    private readonly List<VehicleItem> vehicleItems = new List<VehicleItem>();

    private bool soundFlag;
    private bool musicFlag;

    [Inject] private VehiclesConfig vehiclesConfig;
    [Inject] private PlayerDataManager playerDataManager;
    [Inject] private MusicManager musicManager;
    [Inject] private MapLoadManager mapLoadManager;

    private void Start()
    {
        PlayerPrefs.SetInt("InputType", 2);

        soundFlag = PlayerPrefs.GetInt("Sound", 1) == 1;
        soundImageSwitcher.Switch(soundFlag);
        musicFlag = PlayerPrefs.GetInt("Music", 1) == 1;
        musicImageSwitcher.Switch(musicFlag);

        InitializeVehicles();
        playerGoldText.text = playerDataManager.Gold.ToString();
    }

    public void ShowOptionsPanel()
    {
        optionsPanel.gameObject.SetActive(!optionsPanel.gameObject.activeSelf);
    }

    public void ShowChaptersPanel()
    {
        chaptersPanel.gameObject.SetActive(!chaptersPanel.gameObject.activeSelf);
    }

    public void ShowGaragePanel()
    {
        garagePanel.gameObject.SetActive(!garagePanel.gameObject.activeSelf);
    }

    public void SwitchSound()
    {
        soundFlag = !soundFlag;
        PlayerPrefs.SetInt("Sound", soundFlag ? 1 : 0);
        soundImageSwitcher.Switch(soundFlag);
    }

    public void SwitchMusic()
    {
        musicFlag = !musicFlag;
        PlayerPrefs.SetInt("Music", musicFlag ? 1 : 0);
        musicImageSwitcher.Switch(musicFlag);
        musicManager.TurnMusic(musicFlag);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void StartGame(int mapId)
    {
        mapLoadManager.LoadNewMap(mapId);
    }

    private void InitializeVehicles()
    {
        vehiclesConfig.Vehicles.ForEach(vehicle =>
        {
            var item = Instantiate(vehicleItem, vehiclesContent);
            item.Initialize(vehicle, playerDataManager.CheckPurchasedVehicle(vehicle.Id),
                playerDataManager.SelectedVehicle == vehicle.Id);
            item.BuyAction += BuyVehicle;
            item.SelectAction += SelectVehicle;
            vehicleItems.Add(item);
        });
    }

    private void BuyVehicle(int id)
    {
        if (!playerDataManager.TryBuyVehicle(id)) return;
        vehicleItems.ForEach(vehicle => vehicle.Buy(id));
        playerGoldText.text = playerDataManager.Gold.ToString();
    }

    private void SelectVehicle(int id)
    {
        playerDataManager.SelectVehicle(id);
        vehicleItems.ForEach(vehicle => vehicle.Select(id));
    }

    private void OnDestroy()
    {
        vehicleItems.ForEach(vehicle =>
        {
            vehicle.BuyAction -= BuyVehicle;
            vehicle.SelectAction -= SelectVehicle;
        });
    }
}
