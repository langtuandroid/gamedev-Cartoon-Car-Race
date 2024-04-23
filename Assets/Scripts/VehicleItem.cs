using System;
using UnityEngine;
using UnityEngine.UI;

public class VehicleItem : MonoBehaviour
{
    public event Action<int> BuyAction;
    public event Action<int> SelectAction;

    [SerializeField] private Button selectButton;
    [SerializeField] private Button buyButton;
    [SerializeField] private Image image;
    [SerializeField] private Text nameText;
    [SerializeField] private Text priceText;

    private VehicleData currentVehicle;
    private bool accessStatus;
    private bool activityStatus;

    public void Initialize(VehicleData vehicle, bool access, bool activity)
    {
        currentVehicle = vehicle;
        accessStatus = access;
        activityStatus = activity;
        nameText.text = currentVehicle.Name;
        priceText.text = currentVehicle.Price.ToString();
        image.sprite = vehicle.Sprite;
        UpdateButtons();
    }

    public void Select()
    {
        SelectAction?.Invoke(currentVehicle.Id);
    }

    public void Buy()
    {
        BuyAction?.Invoke(currentVehicle.Id);
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
        buyButton.gameObject.SetActive(!accessStatus);
    }
}
