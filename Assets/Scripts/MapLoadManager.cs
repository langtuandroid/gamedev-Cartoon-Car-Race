using negleft.AGS;
using UnityEngine;

public class MapLoadManager
{
    private int lastLoadedMapId = 1;

    private PlayerDataManager playerDataManager;

    public MapLoadManager(PlayerDataManager playerDataManager)
    {
        this.playerDataManager = playerDataManager;
    }

    public void LoadNewMap(int mapId)
    {
        LoadMap(mapId);
        lastLoadedMapId = mapId;
    }

    public void ReloadLastMap()
    {
        LoadMap(lastLoadedMapId);
    }

    private void LoadMap(int mapId)
    {
        GameObject raceInitiater = new GameObject("RaceInitiater");
        raceInitiater.AddComponent<AgentRaceStarterInitializer>();
        AgentRaceStarterInitializer init = raceInitiater.GetComponent<AgentRaceStarterInitializer>();

        int aitype = 9;
        int playerType = playerDataManager.SelectedVehicle;
        int lap = 2;
        int aicount = 5;
        bool policeAgents = false;

        if (init)
            init.AssignVariables(aitype, playerType, lap, aicount, policeAgents, true);

        InitiateFader.CreateFader("CircuitRace_Map_" + mapId, Color.black, 2.0f);
    }
}
