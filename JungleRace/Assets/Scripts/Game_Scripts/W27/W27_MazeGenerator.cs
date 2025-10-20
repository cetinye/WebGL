using System;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using W27;
using W27_JungleRace;
using Button = UnityEngine.UI.Button;
using Random = UnityEngine.Random;

public class W27_MazeGenerator : MonoBehaviour
{
    Stack<Vector2> LongLine = new Stack<Vector2>();
    List<Vector2> FinalLongLine = new List<Vector2>();
    List<Vector2> shortestPath = new List<Vector2>();
    List<Vector2> shortestPathList = new List<Vector2>();
    List<Vector2> visited = new List<Vector2>();
    List<int> tried = new List<int>();

    W27_Block[,] mazeArray;
    public W27_Block blockInstance;
    private bool backtracking = false;

    public W27_GameController gm;
    public int cameraRotation = 0;
    public Camera cam;
    private float cameraRotationSpeed = 5f;
    private float cameraSize = 0;
    private int width = 5;
    private int height = 5;
    private int targetDistanceMin = 5;
    private int targetDistanceMax = 10;
    private float ghostSpeed = 1f;

    private float blockSize = 2f;
    private float blockCenterOffset;
    private float playerSpeed;
    Vector2 currentPoint = new Vector2(0, 0);

    public GameObject AngleIndicator;
    public float angleHeightOffset;
    public W27_Player player;
    public TrailRenderer playerTrailRenderer;

    public W27_Ghost ghost;
    public GameObject target;
    public SpriteRenderer levelCompleteIndicator;
    public Sprite[] levelIndicators;
    private int ghostStep = 1;

    private int distance = 0;
    private int numberOfRotations = 1;
    private List<Vector2> rotationSteps = new List<Vector2>();

    public Button leftButton, rightButton, upButton, downButton;
    public bool left, right, up, down;

    public bool isSuccessful;

    public bool moving, clickable = true;
    public bool playerHitWall;
    public bool playerReturnFromWall;
    public bool levelDone;
    private Vector3 playerCurrentPos, playerNextPos;

    private Quaternion targetRotation;

    public List<Vector3> visitedBlockPos = new List<Vector3>();
    public LineRenderer lineRenderer;

    public AudioSource playerWalkingSound;
    public AudioSource ghostWalkingSound;


    private float numberOfButtonClicksInTutorialStep1;
    private bool tutorialStep3Set;

    public void setLevelParameters(int width, int height, int numberOfRotations, int targetDistanceMin,
        int targetDistanceMax, float ghostSpeed, float playerSpeed, float cameraSize)
    {
        levelCompleteIndicator.transform.localScale = new Vector3(1f, 1f, 1f);

        this.width = width;
        this.height = height;
        this.numberOfRotations = numberOfRotations;
        this.targetDistanceMin = targetDistanceMin;
        this.targetDistanceMax = targetDistanceMax;
        this.ghostSpeed = ghostSpeed;
        this.playerSpeed = playerSpeed;
        this.cameraSize = cameraSize;

        makeLevel();
    }


    void makeLevel()
    {
        playerTrailRenderer.enabled = false;
        cam.transform.position = new Vector3(0, 0, -15);
        cam.enabled = true;
        cam.orthographicSize = cameraSize;
        AngleIndicator.transform.position = new Vector3(0, height + angleHeightOffset, 0);

        blockCenterOffset = 2f / 2; // bloğun pivot noktası sol altı olduğu için ortasına gelmek için / 2 offset.
        transform.position = new Vector3(width, height, 0); //maze'i ekranın ortasına al

        visited.Clear();
        LongLine.Clear();
        FinalLongLine.Clear();
        shortestPathList.Clear();
        shortestPath.Clear();
        tried.Clear();

        visited.Add(currentPoint);
        LongLine.Push(currentPoint); //başlangıç noktasını listelere ekle
        FinalLongLine.Add(currentPoint);

        while (visited.Count < width * height)
        {
            currentPoint = makeMove(currentPoint);
            visited.Add(currentPoint);
            LongLine.Push(currentPoint);
            FinalLongLine.Add(currentPoint);
        }

        mazeArray = new W27_Block[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++) //maze'i blocklar ile oluştur.
            {
                W27_Block new_block = Instantiate(blockInstance, new Vector3(i * blockSize, j * blockSize, 0),
                    Quaternion.identity);
                mazeArray[i, j] = new_block;
                new_block.position = new Vector2(i * blockSize, j * blockSize);
                new_block.transform.parent = transform;
            }
        }

        for (int i = 1; i < FinalLongLine.Count; i++)
        {
            if (FinalLongLine[i - 1].magnitude - FinalLongLine[i].magnitude <= 1.1
            ) // iki kutucuk komşu ise. Olmayabilir backtracking varsa.
            {
                int xIndexFirst = (int)FinalLongLine[i - 1].x;
                int yIndexFirst = (int)FinalLongLine[i - 1].y; //birinci bloğun koordinatları

                int xIndexSecond = (int)FinalLongLine[i].x;
                int yIndexSecond = (int)FinalLongLine[i].y;
                ; //ikinci bloğun koordinatları

                if (Mathf.Abs(FinalLongLine[i - 1].x - FinalLongLine[i].x) < 0.1) // x'ler aynı yani y'ler farklı
                {
                    if (FinalLongLine[i - 1].y > FinalLongLine[i].y) //i -1 üstte
                    {
                        mazeArray[xIndexFirst, yIndexFirst].DownWall.SetActive(false);
                        mazeArray[xIndexSecond, yIndexSecond].UpWall.SetActive(false);
                    }
                    else if (FinalLongLine[i - 1].y < FinalLongLine[i].y) //i -1 altta
                    {
                        mazeArray[xIndexFirst, yIndexFirst].UpWall.SetActive(false);
                        mazeArray[xIndexSecond, yIndexSecond].DownWall.SetActive(false);
                    }
                }
                else // y'ler aynı yani x'ler farklı
                {
                    if (FinalLongLine[i - 1].x > FinalLongLine[i].x) // i-1 sağda
                    {
                        mazeArray[xIndexFirst, yIndexFirst].LeftWall.SetActive(false);
                        mazeArray[xIndexSecond, yIndexSecond].RightWall.SetActive(false);
                    }
                    else if (FinalLongLine[i - 1].x < FinalLongLine[i].x) // i-1 solda
                    {
                        mazeArray[xIndexFirst, yIndexFirst].RightWall.SetActive(false);
                        mazeArray[xIndexSecond, yIndexSecond].LeftWall.SetActive(false);
                    }
                }
            }
        }

        ghost.transform.position =
            new Vector3(blockCenterOffset, blockCenterOffset, 0); //ekrana hayaleti ve player'ı koy.
        player.transform.position = new Vector3(blockCenterOffset, blockCenterOffset, 0);
        playerCurrentPos = player.transform.position;

        lineRenderer.positionCount++;
        visitedBlockPos.Add(player.transform.localPosition);

        int tempTargetX = 0;
        int tempTargetY = 0;
        Vector2 selectedTarget;
        do
        {
            shortestPathList.Clear();
            shortestPath.Clear();
            distance = 0;

            tempTargetX = Random.Range(0, width);
            tempTargetY = Random.Range(0, height);
            selectedTarget = new Vector2(tempTargetX, tempTargetY);

            foreach (var var in FinalLongLine)
            {
                shortestPathList.Add(var);
                if (selectedTarget == var)
                    break;
            }

            distance = calculateShortestPath(selectedTarget);
        } while (distance < targetDistanceMin || distance > targetDistanceMax);

        target.transform.position = new Vector3(mazeArray[tempTargetX, tempTargetY].position.x + blockCenterOffset,
            mazeArray[tempTargetX, tempTargetY].position.y + blockCenterOffset, 0.2f);

        var previousTurningPoint = -1;
        for (int i = 0; i < numberOfRotations; i++)
        {
            int[] angles = { 0 };

            if (W27_GameController.LevelSO.rotation90 && !W27_GameController.LevelSO.rotation180)
                angles = new int[] { 90, -90 };

            else if (W27_GameController.LevelSO.rotation180 && !W27_GameController.LevelSO.rotation90)
                angles = new int[] { 180 };

            else if (W27_GameController.LevelSO.rotation180 && W27_GameController.LevelSO.rotation90)
                angles = new int[] { 90, -90, 180 };

            int temp = Random.Range(0, 4);
            int randomPoint = Random.Range(3, distance - 2);

            while (Math.Abs(randomPoint - previousTurningPoint) <= 1)
            {
                randomPoint = Random.Range(3, distance - 2);
            }
            previousTurningPoint = randomPoint;

            rotationSteps.Add(new Vector2(randomPoint, angles[temp]));
        }

        // if (gm.gameTutorialController != null)
        // {
        //     if (gm.tutorialStepID == 3 && !tutorialStep3Set)
        //     {
        //         rotationSteps.Clear();
        //         tutorialStep3Set = true;
        //     }
        // }
    }

    public void levelCompleted(bool isSuccessful)
    {
        if (!isSuccessful)
        {
            AudioManager.instance.PlayOneShot(SoundType.Fail);
            levelCompleteIndicator.sprite = levelIndicators[0];
            // if (gm.gameTutorialController == null)
            if (true)
            {
                gm.currentNumberOfLosses++;
                gm.numberOfLossesTotal++;
                gm.roundsFailed++;
                // if (gm.numberOfLosesLimitList[gm.level] == gm.currentNumberOfLosses)
                // {
                //     gm.level--;
                //     gm.currentNumberOfLosses = 0;
                // }
            }
        }
        else
        {
            addLine();

            AudioManager.instance.PlayOneShot(SoundType.Win);
            AudioManager.instance.PlayOneShot(SoundType.Carrot);
            levelCompleteIndicator.sprite = levelIndicators[1];
            gm.currentNumberOfWins++;
            gm.numberOfWinsTotal++;
            gm.roundsCompleted++;
            // gm.level++;
            gm.currentNumberOfLosses = 0;
        }

        gm.CalculateLevelScore(isSuccessful);

        if (gm.roundsCompleted >= W27_GameController.LevelSO.levelUpCriteria * 2)
        {
            ChangeLevel(1);
        }
        else if (gm.roundsFailed >= W27_GameController.LevelSO.levelDownCriteria * 2)
        {
            ChangeLevel(-1);
        }

        if (++gm.roundsPlayed >= W27_GameController.LevelSO.totalRounds)
        {
            gm.handleGameOver();
            return;
        }

        gm.timerActive = false;
        levelCompleteIndicator.transform.DOScale(1.4f, 2f).SetEase(Ease.OutElastic).OnComplete(clearLevel);
    }

    private void ChangeLevel(int changeAmount)
    {
        gm.level += changeAmount;
        PlayerPrefs.SetInt("JungleRace_Level", gm.level);

        gm.roundsFailed = 0;
        gm.roundsCompleted = 0;

        gm.AssignLevel();
    }


    public void clearLevel()
    {
        levelCompleteIndicator.sprite = null;
        player.resetRotation();
        player.resetMotion();
        gm.getReadyTween();
        Destroy(gameObject);
    }

    private int calculateShortestPath(Vector2 selectedTarget)
    {
        int selectedIndex = shortestPathList.Count - 1;
        int repetitionStartIndex = 0;
        int repetitionEndIndex = 0;

        List<int> repeatedPart = new List<int>();
        repeatedPart.Clear();

        for (int j = shortestPathList.Count - 1; j > 0; j--)
        {
            selectedIndex = j;
            for (int i = selectedIndex; i > 0; i--)
            {
                if (shortestPathList[selectedIndex] == shortestPathList[i] && i != selectedIndex)
                {
                    repetitionStartIndex = i;
                    repetitionEndIndex = selectedIndex;

                    for (int k = repetitionStartIndex; k < repetitionEndIndex; k++)
                    {
                        repeatedPart.Add(k);
                    }

                    selectedIndex = repetitionEndIndex;
                }
            }
        }

        for (int i = 0; i < shortestPathList.Count; i++)
        {
            if (repeatedPart.Contains(i) == false)
            {
                shortestPath.Add(shortestPathList[i]);
            }
        }

        return shortestPath.Count;
    }

    private void Update()
    {
        checkButtonClicks();
        controlPlayerMovement();
        moveGhost();
        handleRotation();
    }

    void controlPlayerMovement()
    {
        transform.position = new Vector3(0, 0, 0);

        if (playerHitWall)
        {
            playerReturnFromWall = true;
        }

        if (playerReturnFromWall && player != null)
        {
            player.transform.DOMove(playerCurrentPos, 0.1f).OnComplete(() =>
            {
                player.playHitWallAnimation();
                visitedBlockPos.Add(player.transform.position);
                lineRenderer.positionCount++;
                playerNextPos = playerCurrentPos;
                player.transform.DOKill();
                playerReturnFromWall = false;
            });
            // player.transform.position = playerCurrentPos;
        }
        else if (moving)
        {
            player.transform.position =
                Vector3.MoveTowards(player.transform.position, playerNextPos, playerSpeed * Time.deltaTime);

            if (player.transform.position != playerNextPos)
            {
                controlPlayerSound(true);
                clickable = false;
                player.setAnimSpeed(playerSpeed);
            }
            else
            {
                controlPlayerSound(false);
                player.setAnimSpeed(0f);
                moving = false;
                clickable = true;

                var contains = false;
                foreach (var visitedPos in visitedBlockPos)
                {
                    if (visitedPos == player.transform.position)
                    {
                        contains = true;
                    }
                }

                if (!contains)
                {
                    addLine();
                }
                else
                {
                    removeLine();
                }
            }
        }
    }

    void addLine()
    {
        // visitedBlockPos.Add(player.transform.position);
        // lineRenderer.positionCount++;
        // lineRenderer.SetPositions(visitedBlockPos.ToArray());
    }

    void removeLine()
    {
        // visitedBlockPos.RemoveAt(visitedBlockPos.Count - 1);
        // lineRenderer.positionCount--;
        // lineRenderer.SetPositions(visitedBlockPos.ToArray());
    }

    void checkButtonClicks()
    {
        if (player.isStunned) return;

        var someButtonClicked = false;
        if (left)
        {
            leftClicked();
            someButtonClicked = true;
        }

        if (right)
        {
            rightClicked();
            someButtonClicked = true;
        }

        if (up)
        {
            upClicked();
            someButtonClicked = true;
        }

        if (down)
        {
            downClicked();
            someButtonClicked = true;
        }

        if (gm.tutorialStepID == 0 && someButtonClicked)
        {
            numberOfButtonClicksInTutorialStep1 += Time.deltaTime;
            if (numberOfButtonClicksInTutorialStep1 > 2)
            {
                if (gm.tutorialStepID == 0)
                {
                    gm.tutorialStepID++;
                    // gm.MarkExpectedAction(true);
                    StartCoroutine(gm.tutorialNext(gm.tutorialStepID));
                }
            }
        }
    }

    void handleRotation()
    {
        foreach (var step in rotationSteps)
        {
            if ((int)step.x == ghostStep)
            {
                cameraRotation = (int)step.y;
            }
        }

        // if (gm.gameTutorialController != null)
        if (false)
        {
            if (gm.tutorialStepID == 3)
            {
                if (visitedBlockPos.Count >= 4 && !isSuccessful)
                {
                    cameraRotation = 180;
                }
            }
        }

        var direction = new Vector3();

        direction = new Vector3(cam.transform.rotation.eulerAngles.x, cam.transform.rotation.eulerAngles.y, cameraRotation);
        targetRotation = Quaternion.Euler(direction);

        cam.transform.rotation =
            Quaternion.Lerp(cam.transform.rotation, targetRotation, Time.deltaTime * cameraRotationSpeed);
        ghost.transform.rotation =
            Quaternion.Lerp(ghost.transform.rotation, targetRotation, Time.deltaTime * cameraRotationSpeed);
        target.transform.rotation =
            Quaternion.Lerp(target.transform.rotation, targetRotation, Time.deltaTime * cameraRotationSpeed);
        player.transform.rotation =
            Quaternion.Lerp(target.transform.rotation, targetRotation, Time.deltaTime * cameraRotationSpeed);
    }

    private void controlPlayerSound(bool state)
    {
        if (playerWalkingSound.enabled != state)
        {
            playerWalkingSound.enabled = state;
        }
    }

    void moveGhost()
    {
        if (levelDone) return;

        ghostWalkingSound.enabled = true;

        // var animationThreshold = 0.3f;
        var animationThreshold = 0f;
        if (ghostStep == shortestPath.Count)
        {
            if (!levelDone)
            {
                levelDone = true;
                isSuccessful = false;

                // if (gm.gameTutorialController != null)
                // {
                //     if (gm.tutorialStepID == 2)
                //     {
                //         gm.MarkExpectedAction(false);
                //         gm.gameTutorialController.DestroyLastComponent();
                //     }
                //     else if (gm.tutorialStepID == 3)
                //     {
                //         gm.MarkExpectedAction(false);
                //         gm.gameTutorialController.DestroyLastComponent();
                //     }
                // }
                levelCompleted(isSuccessful);
            }

            return;
        }

        playerTrailRenderer.enabled = true;

        int ghostBlockX = (int)shortestPath[ghostStep].x;
        int ghostBlockY = (int)shortestPath[ghostStep].y;

        Vector2 ghostNextDestination =
            new Vector2(mazeArray[ghostBlockX, ghostBlockY].transform.position.x + blockCenterOffset,
                mazeArray[ghostBlockX, ghostBlockY].transform.position.y + blockCenterOffset);

        ghost.transform.position =
            Vector3.MoveTowards(ghost.transform.position, ghostNextDestination, ghostSpeed * Time.deltaTime);
        ghost.setAnimSpeed(ghostSpeed);


        if (ghost.transform.position.x > ghostNextDestination.x + animationThreshold)
        {
            if (cameraRotation == 0)
            {
                ghost.playSideAnimation(true);
            }
            else if (cameraRotation == 90)
            {
                ghost.playBackAnimation();
            }
            else if (cameraRotation == -90)
            {
                ghost.playFrontAnimation();
            }
            else if (cameraRotation == 180)
            {
                ghost.playSideAnimation(false);
            }
        }
        else if (ghost.transform.position.x + animationThreshold < ghostNextDestination.x)
        {
            if (cameraRotation == 0)
            {
                ghost.playSideAnimation(false);
            }
            else if (cameraRotation == 90)
            {
                ghost.playFrontAnimation();
            }
            else if (cameraRotation == -90)
            {
                ghost.playBackAnimation();
            }
            else if (cameraRotation == 180)
            {
                ghost.playSideAnimation(true);
            }
        }

        else if (ghost.transform.position.y > ghostNextDestination.y + animationThreshold)
        {
            if (cameraRotation == 0)
            {
                ghost.playFrontAnimation();
            }
            else if (cameraRotation == 90)
            {
                ghost.playSideAnimation(true);
            }
            else if (cameraRotation == -90)
            {
                ghost.playSideAnimation(false);
            }
            else if (cameraRotation == 180)
            {
                ghost.playBackAnimation();
            }
        }
        else if (ghost.transform.position.y + animationThreshold < ghostNextDestination.y)
        {
            if (cameraRotation == 0)
            {
                ghost.playBackAnimation();
            }
            else if (cameraRotation == 90)
            {
                ghost.playSideAnimation(false);
            }
            else if (cameraRotation == -90)
            {
                ghost.playSideAnimation(true);
            }
            else if (cameraRotation == 180)
            {
                ghost.playFrontAnimation();
            }
        }

        if ((Vector2)ghost.transform.position == ghostNextDestination)
        {
            ghostStep++;
        }
    }

    void leftClicked()
    {
        if (clickable && !moving)
        {
            player.resetRotation();
            playerCurrentPos = player.transform.position;
            playerNextPos = new Vector3(playerCurrentPos.x - blockSize, playerCurrentPos.y, 0);
            player.left = true;
            moving = true;
        }
    }

    void rightClicked()
    {
        if (clickable && !moving)
        {
            player.resetRotation();
            playerCurrentPos = player.transform.position;
            playerNextPos = new Vector3(playerCurrentPos.x + blockSize, playerCurrentPos.y, 0);
            player.right = true;
            moving = true;
        }
    }

    void upClicked()
    {
        if (clickable && !moving)
        {
            player.resetRotation();
            playerCurrentPos = player.transform.position;
            playerNextPos = new Vector3(playerCurrentPos.x, playerCurrentPos.y + blockSize, 0);
            player.up = true;
            moving = true;
        }
    }

    void downClicked()
    {
        if (clickable && !moving)
        {
            player.resetRotation();
            playerCurrentPos = player.transform.position;
            playerNextPos = new Vector3(playerCurrentPos.x, playerCurrentPos.y - blockSize, 0);
            player.down = true;
            moving = true;
        }
    }

    Vector2 makeMove(Vector2 currentPoint)
    {
        Vector2 nextPos = new Vector2();
        Vector2[] nextPossibleMove;
        tried.Clear();
        bool itStuck = false;
        nextPossibleMove = new Vector2[]
        {
            new Vector2(currentPoint.x + 1, currentPoint.y), new Vector2(currentPoint.x - 1, currentPoint.y),
            new Vector2(currentPoint.x, currentPoint.y + 1), new Vector2(currentPoint.x, currentPoint.y - 1)
        };
        do
        {
            if (itStuck)
            {
                backtracking = true;
                return makeMove(LongLine.Pop());
            }

            var randomMove = Random.Range(0, 4);
            if (!tried.Contains(randomMove))
            {
                tried.Add(randomMove);
            }

            if (tried.Count == 4)
            {
                itStuck = true;
            }

            nextPos = nextPossibleMove[randomMove];
        } while (nextPos == currentPoint || visited.Contains(nextPos) || nextPos.x < 0 || nextPos.y < 0 ||
                 nextPos.x >= width || nextPos.y >= height);

        if (backtracking)
        {
            FinalLongLine.Add(currentPoint);
            backtracking = false;
        }

        return nextPos;
    }
}