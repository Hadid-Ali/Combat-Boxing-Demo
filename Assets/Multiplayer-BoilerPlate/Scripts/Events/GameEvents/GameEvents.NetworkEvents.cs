using Photon.Realtime;

public static partial class GameEvents
{
    public static class NetworkEvents
    {
        public static GameEvent<string, int> PlayerReceiveCardsData = new();
        public static GameEvent NetworkDisconnectedEvent = new();

        public static GameEvent<string, float> NetworkTimerStartRequest = new();
        
        public static GameEvent<Region> PlayerRegionSelect = new ();
        public static GameEvent<RoomOptions> PlayerRoomCreation = new ();
        
        public static GameEvent RoomJoinFailed = new();
        public static GameEvent<RegionConfig> ConnectionTransition = new();
        public static GameEvent<string> NetworkStatus = new();
        public static GameEvent LobbyJoined = new();
        public static GameEvent PlayerCharacterSelected = new();
        public static GameEvent PlayerJoinedRoom = new();
        
        public static GameEvent<string> MatchStartTimer = new();
        public static GameEvent<string> RoundRestartTimer = new();
        public static GameEvent<string> SubmissionTimerTick = new();
        
        public static GameEvent GameRoomCreated = new();

        public static GameEvent OnMasterGameplayLoaded = new();
    }
}
