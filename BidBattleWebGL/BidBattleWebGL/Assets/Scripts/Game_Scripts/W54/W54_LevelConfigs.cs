using W54;

public class W54_LevelConfigs
{
    public static W54_LevelConfig[] levelConfigs = new[]
    {
        new W54_LevelConfig(1,W54_equationType.SINGLE_DIGIT, W54_equationType.SINGLE_DIGIT, 10,3, 40,155),
        new W54_LevelConfig(2,W54_equationType.TWO_DIGIT, W54_equationType.TWO_DIGIT, 30,3, 2,185),
        new W54_LevelConfig(3,W54_equationType.TWO_DIGIT, W54_equationType.TWO_DIGIT, 50,3, 2,200),
        new W54_LevelConfig(4,W54_equationType.TWO_OPS, W54_equationType.TWO_DIGIT, 30,3, 2,230),
        new W54_LevelConfig(5,W54_equationType.FOUR_OPS, W54_equationType.TWO_DIGIT, 30,4, 2,307),
        new W54_LevelConfig(6,W54_equationType.TWO_OPS, W54_equationType.TWO_OPS, 40,4, 2,370),
        new W54_LevelConfig(7,W54_equationType.FOUR_OPS, W54_equationType.FOUR_OPS, 40,4, 2,400),
        new W54_LevelConfig(8,W54_equationType.ONE_PARENTHESIS, W54_equationType.TWO_DIGIT, 30,4, 2,460),
        new W54_LevelConfig(9,W54_equationType.ONE_PARENTHESIS, W54_equationType.FOUR_OPS, 30,5, 2,510),
        new W54_LevelConfig(10,W54_equationType.ONE_PARENTHESIS, W54_equationType.ONE_PARENTHESIS, 30,5,2, 615),
        new W54_LevelConfig(11,W54_equationType.ONE_PARENTHESIS, W54_equationType.ONE_PARENTHESIS, 50,5,1, 666),
        new W54_LevelConfig(12,W54_equationType.TWO_PARENTHESIS, W54_equationType.ONE_PARENTHESIS, 30,5,1, 770),
        new W54_LevelConfig(13,W54_equationType.TWO_PARENTHESIS, W54_equationType.TWO_PARENTHESIS, 30,6,1, 850),
        new W54_LevelConfig(14,W54_equationType.TWO_PARENTHESIS, W54_equationType.TWO_PARENTHESIS, 40,6,1, 920),
        new W54_LevelConfig(15,W54_equationType.TWO_PARENTHESIS, W54_equationType.TWO_PARENTHESIS, 50,6,1, 1000),
    };
}