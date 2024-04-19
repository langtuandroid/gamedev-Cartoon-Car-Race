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

    public int Gold => playerData.Gold;

    public int SelectedVehicle => playerData.SelectedVehicle;

    public PlayerDataManager(VehiclesConfig vehiclesConfig)
    {
        saveDataPath = Application.persistentDataPath + "/";
        LoadData();
        this.vehiclesConfig = vehiclesConfig;
    }

    public bool CheckPurchasedVehicle(int vehicleId) => playerData.PurchasedVehicles.Contains(vehicleId);

    public bool TryBuyTransportVehicle(int vehicleId)
    {
        var vehicleData = vehiclesConfig.Vehicles.FirstOrDefault(v => v.Id == vehicleId);

        if (CheckPurchasedVehicle(vehicleId) || vehicleData == null || playerData.Gold < vehicleData.Price)
            return false;

        playerData.Gold -= vehicleData.Price;
        playerData.PurchasedVehicles.Add(vehicleId);

        return true;
    }

    public void AddGold(int gold)
    {
        playerData.Gold += gold;
        SaveData();
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
            Gold = 10,
            SelectedVehicle = 1,
            PurchasedVehicles = new List<int>() {1}
        };
    }
}
