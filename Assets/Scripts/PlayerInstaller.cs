using UnityEngine;
using Zenject;

public class PlayerInstaller : MonoInstaller
{
    [SerializeField] private VehiclesConfig vehiclesConfig;
    [SerializeField] private RaceRewardConfig raceRewardConfig;

    public override void InstallBindings()
    {
        Container.Bind<VehiclesConfig>().FromInstance(vehiclesConfig);
        Container.Bind<RaceRewardConfig>().FromInstance(raceRewardConfig);
        Container.Bind<PlayerDataManager>().FromNew().AsSingle().NonLazy();
    }
}
