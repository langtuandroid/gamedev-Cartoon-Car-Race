using System;
using UnityEngine;

[Serializable]
public class VehicleData
{
    public int Id;
    public int Price;
    public Sprite Sprite;
    public string Name;
    public PurchaseType purchaseType;
}

public enum PurchaseType
{
    Gold,
    Diamonds,
    Ads
}
