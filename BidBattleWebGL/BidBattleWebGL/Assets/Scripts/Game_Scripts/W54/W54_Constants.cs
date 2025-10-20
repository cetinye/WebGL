namespace W54
{
    public  class W54_Constants
    {
        /// <summary>
        /// This is the readonly list of sound fx are used in the W37 Game,
        /// and should match enum of <see cref="eW54FxSoundStates"/>
        /// </summary>
        public readonly string[] FxSoundList = new[]
        {
            "W54_CurtainMove",
            "W54_HitToPad", 
            "W54_QuestionAnswered", 
        };
        
        /// <summary>
        /// This is a readonly list of environment sound list in W37 game,
        /// should match enum of <see cref="eW54EnvironmentSoundStates"/>
        /// </summary>
        public readonly string[] EnvironmentSoundList = new[]
        {
            "W54_CrowdNoise",
        };
    }
}