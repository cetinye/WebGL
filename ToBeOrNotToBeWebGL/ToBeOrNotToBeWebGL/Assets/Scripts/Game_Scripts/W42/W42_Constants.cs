namespace W42
{
    public class W42_Constants
    {
        /// <summary>
        /// This is the readonly list of sound fx are used in the W42 Game,
        /// and should match enum of <see cref="eW42FxSoundStates"/>
        /// </summary>
        public readonly string[] FxSoundList = new[]
        {
            "W42_Move",
            "W42_Correct",
            "W42_Wrong",
            "W42_BadScore",
            "W42_GoodScore",
            "W42_PerfectScore",
            "W42_ButtonClick"
        };

        /// <summary>
        /// This is a readonly list of environment sound list in W42 game,
        /// should match enum of <see cref="eW42EnvironmentSoundStates"/>
        /// </summary>
        public readonly string[] EnvironmentSoundList = new[]
        {
            "W42_Background",
        };

        public readonly string[] LocalizationKeys = new[]
        {
            "red",
            "blue",
            "green",
            "orange",
            "black",
            "thisIs",
            "not",
            "a",
        };

        public readonly string[] LocalizationModelKeys = new[]
        {
           "ambulance",
            "shoe",
            "fish",
            "toothbrush",
            "apple",
            "mouse",
            "elephant",
            "cup",
            "camera",
            "brush",
            "guitar",
            "eyeglass",
            "giftBox",
            "watch",
            "necktie",
            "joystick",
            "scissors",
            "bus",
            "palette",
            "gramophone",
            "radio",
            "tshirt",
            "comb",
            "chicken",
            "telephone",
            "ball",
            "bag",
            "sock",
            "hat",
            "umbrella"
        };
    }
}