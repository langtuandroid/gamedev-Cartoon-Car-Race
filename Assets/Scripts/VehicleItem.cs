using System;
using UnityEngine;
using UnityEngine.UI;

public class VehicleItem : MonoBehaviour
{
    public event Action<int, PurchaseType> BuyAction;
    public event Action<int> SelectAction;

    [SerializeField] private Button selectButton;
    [SerializeField] private Button buyButton;
    [SerializeField] private Button buyDiamondsButton;
    [SerializeField] private Button watchButton;
    [SerializeField] private Image image;
    [SerializeField] private Text nameText;
    [SerializeField] private Text priceText;
    [SerializeField] private Text priceDiamondsText;

    private VehicleData currentVehicle;
    private bool accessStatus;
    private bool activityStatus;

    public void Initialize(VehicleData vehicle, bool access, bool activity)
    {
        currentVehicle = vehicle;
        accessStatus = access;
        activityStatus = activity;
        nameText.text = currentVehicle.Name;
        switch (vehicle.purchaseType)
        {
            case PurchaseType.Gold:
                priceText.text = currentVehicle.Price.ToString();
                break;
            case PurchaseType.Diamonds:
                priceDiamondsText.text = currentVehicle.Price.ToString();
                break;
        }

        image.sprite = vehicle.Sprite;
        UpdateButtons();
    }

    public void Select()
    {
        SelectAction?.Invoke(currentVehicle.Id);
    }

    public void Buy()
    {
        BuyAction?.Invoke(currentVehicle.Id, currentVehicle.purchaseType);
    }

    public void Select(int id)
    {
        activityStatus = currentVehicle.Id == id;
        UpdateButtons();
    }

    public void Buy(int id)
    {
        if (currentVehicle.Id != id) return;
        accessStatus = true;
        UpdateButtons();
    }

    private void UpdateButtons()
    {
        selectButton.gameObject.SetActive(accessStatus);
        selectButton.interactable = !activityStatus;
        buyButton.gameObject.SetActive(false);
        buyDiamondsButton.gameObject.SetActive(false);
        watchButton.gameObject.SetActive(false);
        if (accessStatus) return;
        switch (currentVehicle.purchaseType)
        {
            case PurchaseType.Gold:
                buyButton.gameObject.SetActive(true);
                break;
            case PurchaseType.Diamonds:
                buyDiamondsButton.gameObject.SetActive(true);
                break;
            case PurchaseType.Ads:
                watchButton.gameObject.SetActive(true);
                break;
        }
    }
}
