public class W56_LevelConfig
{
    public int levelIndex;
    public int numberOfTypes;
    public int numberOfSideSwitches;
    public int numberOfQuestions;
    public int levelScore;

    public W56_LevelConfig(int levelIndex, int numberOfTypes, int numberOfSideSwitches, int numberOfQuestions, int levelScore)
    {
        this.levelIndex = levelIndex;
        this.numberOfTypes = numberOfTypes;
        this.numberOfSideSwitches = numberOfSideSwitches;
        this.numberOfQuestions = numberOfQuestions;
        this.levelScore = levelScore;
    }
}