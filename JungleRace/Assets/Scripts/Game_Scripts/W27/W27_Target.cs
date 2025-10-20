using UnityEngine;

public class W27_Target : MonoBehaviour
{
    public W27_MazeGenerator mg;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!mg.levelDone)
        {
            mg.levelDone = true;
            mg.isSuccessful = true;
            mg.levelCompleted(mg.isSuccessful);

            // if (mg.gm.gameTutorialController != null)
            // {
            //     if (mg.gm.tutorialStepID == 1)
            //     {
            //         mg.gm.tutorialStepID++;
            //         mg.gm.MarkExpectedAction(true);
            //         StartCoroutine(mg.gm.tutorialNext(mg.gm.tutorialStepID));
            //     }
            //     else if (mg.gm.tutorialStepID == 2)
            //     {
            //         mg.gm.tutorialStepID++;
            //         mg.gm.MarkExpectedAction(true);
            //         StartCoroutine(mg.gm.tutorialNext(mg.gm.tutorialStepID));
            //     }
            //     else if (mg.gm.tutorialStepID == 3)
            //     {
            //         mg.gm.tutorialStepID++;
            //         mg.gm.MarkExpectedAction(true);
            //         mg.gm.gameTutorialController.setTutorialIsVisited();
            //         mg.gm.gameTutorialController = null;
            //     }
            // }
            gameObject.SetActive(false);
        }
    }
}