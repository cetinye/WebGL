using W54;

public class W54_LevelConfig
{
    public int levelIndex;
    public W54_equationType firstEquationType;
    public W54_equationType secondEquationType;
    public int answerMaxValue;
    public int levelUpCriteria;
    public int levelDownCriteria;
    public int levelScore;

    public W54_LevelConfig(int levelIndex, W54_equationType firstEquationType, W54_equationType secondEquationType, int answerMaxValue, int levelUpCriteria, int levelDownCriteria, int levelScore)
    {
        this.levelIndex = levelIndex;
        this.firstEquationType = firstEquationType;
        this.secondEquationType = secondEquationType;
        this.answerMaxValue = answerMaxValue;
        this.levelUpCriteria = levelUpCriteria;
        this.levelDownCriteria = levelDownCriteria;
        this.levelScore = levelScore;
    }
}