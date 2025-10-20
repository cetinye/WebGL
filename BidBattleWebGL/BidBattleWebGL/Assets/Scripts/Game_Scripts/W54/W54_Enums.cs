namespace W54
{
    public enum W54_equationType
    {
        SINGLE_DIGIT,
        TWO_DIGIT,
        TWO_OPS,
        FOUR_OPS,
        ONE_PARENTHESIS,
        TWO_PARENTHESIS
    }

    public enum W54_OP
    {
        ADD,
        SUBTRACT,
        MULTIPLY,
        DIVIDE
    }

    public enum W54_ANSWER_BUTTON
    {
        LEFT,
        RIGHT,
    }

    public enum eW54FxSoundStates
    {
        CURTAIN_MOVE,
        HIT_TO_PAD,
        QUESTION_ANSWERED,
    }
    
    public enum eW54EnvironmentSoundStates
    {
        CROWD_NOISE,
    }
}