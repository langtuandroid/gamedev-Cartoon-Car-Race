using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class PlayerDataManager
{
    private PlayerData playerData;
    private string saveDataPath;
    private string playerName = "Player";
    private VehiclesConfig vehiclesConfig;

    public int GameNumber = 0;

    public int Gold => playerData.Gold;

    public int SelectedVehicle => playerData.SelectedVehicle;

    public PlayerDataManager(VehiclesConfig vehiclesConfig)
    {
        saveDataPath = Application.persistentDataPath + "/";
        LoadData();
        this.vehiclesConfig = vehiclesConfig;
    }

    public bool CheckPurchasedVehicle(int vehicleId) => playerData.PurchasedVehicles.Contains(vehicleId);

    public bool TryBuyVehicle(int vehicleId)
    {
        var vehicleData = vehiclesConfig.Vehicles.FirstOrDefault(v => v.Id == vehicleId);

        if (CheckPurchasedVehicle(vehicleId) || vehicleData == null || playerData.Gold < vehicleData.Price)
            return false;

        playerData.Gold -= vehicleData.Price;
        playerData.PurchasedVehicles.Add(vehicleId);
        SaveData();

        return true;
    }

    public bool TryBuyVehicleByDiamonds(int vehicleId)
    {
        var vehicleData = vehiclesConfig.Vehicles.FirstOrDefault(v => v.Id == vehicleId);
        var diamonds = PlayerPrefs.GetInt("Diamond", 0);

        if (CheckPurchasedVehicle(vehicleId) || vehicleData == null || diamonds < vehicleData.Price)
            return false;

        diamonds -= vehicleData.Price;
        playerData.PurchasedVehicles.Add(vehicleId);
        SaveData();
        PlayerPrefs.SetInt("Diamond", diamonds);

        return true;
    }

    public bool TryBuyVehicleByAd(int vehicleId)
    {
        if (CheckPurchasedVehicle(vehicleId)) return false;

        playerData.PurchasedVehicles.Add(vehicleId);
        SaveData();

        return true;
    }

    public void AddGold(int gold)
    {
        if (gold == 0) return;
        playerData.Gold += gold;
        SaveData();
    }

    public void SelectVehicle(int vehicleId)
    {
        if (playerData.SelectedVehicle != vehicleId && CheckPurchasedVehicle(vehicleId))
        {
            playerData.SelectedVehicle = vehicleId;
            SaveData();
        }
    }

    private void LoadData()
    {
        var path = saveDataPath + playerName + ".save";
        if (File.Exists(path))
        {
            var bf = new BinaryFormatter();
            var file = File.Open(path, FileMode.Open);
            try
            {
                playerData = (PlayerData) bf.Deserialize(file);
            }
            catch (Exception)
            {
                SetDefaultPlayerData();
            }
            finally
            {
                file.Close();
            }

        }
        else SetDefaultPlayerData();
    }

    private void SaveData()
    {
        var path = saveDataPath + playerName + ".save";
        var bf = new BinaryFormatter();
        var file = File.Create(path);
        try
        {
            bf.Serialize(file, playerData);
        }
        finally
        {
            file.Close();
        }
    }

    private void SetDefaultPlayerData()
    {
        playerData = new PlayerData()
        {
            Gold = 0,
            SelectedVehicle = 0,
            PurchasedVehicles = new List<int>() {0}
        };
    }
}
