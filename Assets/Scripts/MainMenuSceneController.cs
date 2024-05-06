using System.Collections.Generic;
using Integration;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MainMenuSceneController : MonoBehaviour
{
    [SerializeField] private GameObject chaptersPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject garagePanel;
    [SerializeField] private GameObject diamondsPanel;

    [SerializeField] private ImageSwitcher soundImageSwitcher;
    [SerializeField] private ImageSwitcher musicImageSwitcher;

    [SerializeField] private List<Text> playerGoldTextsList;
    [SerializeField] private List<Text> playerDiamondsTextsList;

    [SerializeField] private VehicleItem vehicleItem;
    [SerializeField] private Transform vehiclesContent;

    private readonly List<VehicleItem> vehicleItems = new List<VehicleItem>();

    private bool soundFlag;
    private bool musicFlag;

    private int currentVehicle;

    [Inject] private VehiclesConfig vehiclesConfig;
    [Inject] private PlayerDataManager playerDataManager;
    [Inject] private MusicManager musicManager;
    [Inject] private MapLoadManager mapLoadManager;
    [Inject] private RewardedAdController rewardedAdController;
    [Inject] private IAPService iapService;

    private void Start()
    {
        PlayerPrefs.SetInt("InputType", 2);

        soundFlag = PlayerPrefs.GetInt("Sound", 1) == 1;
        soundImageSwitcher.Switch(soundFlag);
        musicFlag = PlayerPrefs.GetInt("Music", 1) == 1;
        musicImageSwitcher.Switch(musicFlag);

        InitializeVehicles();
        UpdateCurrencies();

        rewardedAdController.GetRewarded += GetReward;
        iapService.DiamondsPurchased += UpdateCurrencies;
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

    public void ShowDiamondsPanel()
    {
        diamondsPanel.gameObject.SetActive(!diamondsPanel.gameObject.activeSelf);
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

    public void BuyPack1()
    {
        iapService.BuyPack1();
    }

    public void BuyPack2()
    {
        iapService.BuyPack2();
    }

    public void BuyPack3()
    {
        iapService.BuyPack3();
    }

    public void BuyPack4()
    {
        iapService.BuyPack4();
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

    private void BuyVehicle(int id, PurchaseType purchaseType)
    {
        switch (purchaseType)
        {
            case PurchaseType.Gold:
                if (!playerDataManager.TryBuyVehicle(id)) return;
                break;
            case PurchaseType.Diamonds:
                if (!playerDataManager.TryBuyVehicleByDiamonds(id)) return;
                break;
            case PurchaseType.Ads:
                ShowRewardedAd(id);
                return;
        }

        vehicleItems.ForEach(vehicle => vehicle.Buy(id));
        UpdateCurrencies();
    }

    private void ShowRewardedAd(int id)
    {
        currentVehicle = id;
        rewardedAdController.ShowAd();
    }

    private void GetReward()
    {
        if (currentVehicle <= 0 || !playerDataManager.TryBuyVehicleByAd(currentVehicle)) return;

        vehicleItems.ForEach(vehicle => vehicle.Buy(currentVehicle));
    }

    private void UpdateCurrencies()
    {
        var gold = playerDataManager.Gold.ToString();
        var diamonds = PlayerPrefs.GetInt("Diamond", 0).ToString();
        playerGoldTextsList.ForEach(t => t.text = gold);
        playerDiamondsTextsList.ForEach(t => t.text = diamonds);
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
        rewardedAdController.GetRewarded -= GetReward;
        iapService.DiamondsPurchased -= UpdateCurrencies;
    }
}
