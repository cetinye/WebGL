namespace W44
{
    public class W44_Constants
    {
        /// <summary>
        /// This is the readonly list of sound fx are used in the W44 Game,
        /// and should match enum of <see cref="eW44FxSoundStates"/>
        /// </summary>
        public readonly string[] FxSoundList = new[]
        {
            "W44_DoorOpenSound",
            "W44_DoorCloseSound",
            "W44_CrowdSound",
            "W44_QuestionSound",
            "W44_CorrectAnswer",
            "W44_WrongAnswer",
            "W44_LevelCompleted",

        };

        /// <summary>
        /// This is a readonly list of environment sound list in W44 game,
        /// should match enum of <see cref="eW44EnvironmentSoundStates"/>
        /// </summary>
        public readonly string[] EnvironmentSoundList;

        public readonly string[] LocalizationKeys = new[]
        {
            "numberOfPassengersToGetOff",
            "numberOfTotalPassengers",
            "level",
            "completed",
            "areYouReady"
        };
    }
}