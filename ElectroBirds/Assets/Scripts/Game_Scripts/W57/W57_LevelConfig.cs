public class W57_LevelConfig
{
    public int levelIndex;
    public int numberOfBirds;
    public float electricFlowDuration;
    public float flowInterval;
    public int levelScore;

    public W57_LevelConfig(int levelIndex, int numberOfBirds, float electricFlowDuration, float flowInterval,  int levelScore)
    {
        this.levelIndex = levelIndex;
        this.numberOfBirds = numberOfBirds;
        this.electricFlowDuration = electricFlowDuration;
        this.flowInterval = flowInterval;
        this.levelScore = levelScore;
    }
}