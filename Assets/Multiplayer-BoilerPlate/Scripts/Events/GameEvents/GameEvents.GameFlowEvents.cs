
public static partial class GameEvents
{
    public static class GameFlowEvents
    {
        public static GameEvent RoundStart = new();
        public static GameEvent RestartRound = new();
        public static GameEvent MatchOver = new();
        public static GameEvent LeaveMatch = new();
        
        public static GameEvent SubmissionTimerOver = new();
    }

    public static class BoxingDemoGameFlowEvents
    {
        public static GameEvent<int> RoundStart = new();
        public static GameEvent<int, int> RoundComplete = new();
        public static GameEvent<int> MatchFinished = new();
        public static GameEvent PlayerLeftRoom = new();
    }
}
