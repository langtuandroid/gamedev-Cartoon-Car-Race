using UnityEngine;
using Zenject;

public class PlayerInstaller : MonoInstaller
{
    [SerializeField] private VehiclesConfig vehiclesConfig;

    public override void InstallBindings()
    {
        Container.Bind<VehiclesConfig>().FromInstance(vehiclesConfig);
        Container.Bind<PlayerDataManager>().FromNew().AsSingle().NonLazy();
    }
}
