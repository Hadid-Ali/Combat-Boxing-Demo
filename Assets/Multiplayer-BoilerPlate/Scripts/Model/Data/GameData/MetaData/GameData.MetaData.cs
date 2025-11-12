using UnityEngine;

public static partial class GameData
{
    public static class MetaData
    {
        public const int HandWinReward = 10;
        public const int DecksCount = 3;
        public const int DeckSize = 5;

        public const int TotalScoreToWin = 100;
        public const int TotalScoreToWinOffline = 100;
        
        public const int OffsetCards = 2;

        public const int MinimumNameLength = 3;
        public const int MaximumNameLength = 8;

        public const int MaxPlayersLimit = 6;
        public const int MinimumRequiredPlayers = 2;
        public const int MaximumPlayers = 6;

        public const float RevealTimeForThreePlayersGame = 8f;
        public const float RevealTimeForSixPlayersGame = 26f;
        
        public const int WaitBeforeAutomaticMatchStart = 30;
        public const int WaitBeforeAutomaticRoundStart = 5;
        public const int WaitBeforeAutomaticCardsSubmission = 180;
        public const int WaitBeforeSubmissionTimerStart = 5;
        
        public const int NullID = -1000;

        public const string PrivacyPolicyLink = "https://triplehandpoker.com/privacy-policy/";
        public const string TermsOfUsageLink = "https://triplehandpoker.com/terms-and-conditions/";
        public const string FeedbackFormLink = "https://triplehandpoker.com/feedback-form/";
        public const string AppWebsiteLink = "https://triplehandpoker.com/";
        public const string MuteString = "MuteString";
        
        public const string BotDifficulty = "BwaotsDifficulty";
        public const BotsDifficulty DefaultBotDifficulty = BotsDifficulty.Hard;
        public const bool IsTestMode = false;

        public static Vector2 deafaultCardImageSize = new Vector2(250, 350);
        public static Vector2 deafaultCharacterImageSize = new Vector2(450, 500);
    }
}