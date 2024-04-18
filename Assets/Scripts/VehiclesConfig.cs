using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VehiclesConfig", menuName = "Configs/VehiclesConfig")]
public class VehiclesConfig : ScriptableObject
{
    public List<VehicleData> Vehicles;
}
