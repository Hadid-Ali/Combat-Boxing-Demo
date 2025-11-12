using System;

public interface INetworkPlayerSpawner
{
    public void Initialize(Action<PlayerController> onPlayerSpawned);
    public void SpawnPlayer();
    public void RegisterPlayer(PlayerController playerController);
    public void ReIteratePlayerSpawns();
    public PlayerController GetPlayerAgainstViewID(int ID);
    public PlayerController GetPlayerAgainstActorID(int ID);
    public string GetPlayerName(int ID);
    public int GetLocalPlayerNetworkID();
    public int GetPlayerLocalID(int ID);

    public int GetPlayerSelectedCard(int ID);
}
