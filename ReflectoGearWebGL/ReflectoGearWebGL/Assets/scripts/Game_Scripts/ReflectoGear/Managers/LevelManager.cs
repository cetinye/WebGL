using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace W91_ReflectoGear
{
    public class LevelManager : MonoBehaviour
    {
        public int levelId;
        public W91_LevelSO level;
        public List<W91_LevelSO> levelList;

        public int errorCounter = 0;
        public int correctCounter = 0;
        public int bonusScore;
        public int score = 0;

        public List<Gear> AnswerList = new List<Gear>();

        [SerializeField] private int mirrorPosY;
        [SerializeField] private int mirrorPosX;

        [SerializeField] private List<GameObject> gears = new List<GameObject>();
        [SerializeField] private List<GameObject> mirrors = new List<GameObject>();

        [SerializeField] private float gearSpawnTime;
        [SerializeField] private int numOfUnchangeable;
        [SerializeField] private float amountToMovePosX;
        [SerializeField] private float amountToMovePosY;
        [SerializeField] private GameObject changeableGear;
        [SerializeField] private GameObject unchangeableGear;
        [SerializeField] private GameObject emptyCell;
        [SerializeField] private GameObject mirror;
        [SerializeField] private GameObject levelParent;
        [SerializeField] private LevelDifficultyState levelDifState = LevelDifficultyState.Harder;

        private Vector3 startingPos;
        private float cellSize;
        private int totalPlayCount = 0;
        private Gear tappedGear;

        [SerializeField] private GameManager gameManager;
        [SerializeField] private UIManager uiManager;

        private void ReadLevelData()
        {
            Debug.Log("ReadLevelData");
            level = levelList[levelId];
            Debug.Log("levelId: " + level);
            mirrorPosY = level.mirrorYPos;
            Debug.Log("mirrorPosY: " + mirrorPosY);
            mirrorPosX = level.mirrorXPos;
            Debug.Log("mirrorPosX: " + mirrorPosX);
            bonusScore = level.levelScore;
            Debug.Log("bonusScore: " + bonusScore);


        }

        private void ChangeGearSprite()
        {
            Debug.Log("ChangeGearSprite");
            //change sprite regarding levels choice
            changeableGear.GetComponent<Gear>().unselectedImage = level.unselected;
            changeableGear.GetComponent<Gear>().selectedImage = level.selected;
        }

        private void GenerateLevel()
        {
            Debug.Log("GenerateLevel");
            //make grid arrangements if autofill enabled
            if (level.autoFill)
                ArrangeGrid();

            else
                ManualArrangeGrid();

            //scale gears colliders before spawning
            ScaleGearCollider();

            if (level.randomizeMirrorOnX)
                mirrorPosX = Random.Range(level.minRandomMirrorX, level.maxRandomMirrorX);

            if (level.randomizeMirrorOnY)
                mirrorPosY = Random.Range(level.minRandomMirrorY, level.maxRandomMirrorY);

            if (level.randomizeLshapePosition)
                level.LshapePosition = (LshapePosition)Random.Range(0, Enum.GetValues(typeof(LshapePosition)).Length);

            //row count
            for (int y = 0; y < level.rowCount; y++)
            {
                //column count
                for (int x = 0; x < level.columnCount; x++)
                {
                    //vertical mirror
                    if (x != 0 && x == mirrorPosX)
                    {
                        GameObject mirrorObj = Instantiate(mirror, new Vector3(startingPos.x, startingPos.y, startingPos.z), Quaternion.identity, levelParent.transform);
                        mirrorObj.GetComponent<Mirror>().X = x;
                        mirrorObj.GetComponent<Mirror>().Y = y;
                        mirrorObj.transform.eulerAngles = new Vector3(mirrorObj.transform.rotation.x, mirrorObj.transform.rotation.y, 90.0f);
                        mirrors.Add(mirrorObj);
                        mirrorObj.gameObject.name = mirrorObj.gameObject.name + " " + x + "," + y;
                        mirrorObj.SetActive(false);

                        if (level.Lshape && x == mirrorPosX && y == mirrorPosY)
                        {
                            mirrorObj.transform.localScale = new Vector3(0.46f, 0.52f, 1f);
                            mirrorObj.GetComponent<Image>().enabled = true;
                        }
                    }
                    //horizontal mirror
                    else if (y != 0 && y == mirrorPosY)
                    {
                        GameObject mirrorObj = Instantiate(mirror, new Vector3(startingPos.x, startingPos.y, startingPos.z), Quaternion.identity, levelParent.transform);
                        mirrorObj.GetComponent<Mirror>().X = x;
                        mirrorObj.GetComponent<Mirror>().Y = y;
                        mirrors.Add(mirrorObj);
                        mirrorObj.gameObject.name = mirrorObj.gameObject.name + " " + x + "," + y;
                        mirrorObj.SetActive(false);

                    }
                    //gear
                    else
                    {
                        GameObject tempGear = Instantiate(changeableGear, new Vector3(startingPos.x, startingPos.y, startingPos.z), Quaternion.identity, levelParent.transform);
                        tempGear.GetComponent<Gear>().X = x;
                        tempGear.GetComponent<Gear>().Y = y;
                        tempGear.GetComponent<Gear>().levelManager = this;
                        tempGear.gameObject.name = tempGear.gameObject.name + x + " -" + y;
                        tempGear.GetComponent<Image>().enabled = false;
                        gears.Add(tempGear);
                    }
                }
            }

            if (level.Lshape)
                ArrangeMirrors();

            RandomizeGears();
        }

        //randomly select unchangeable gears
        private void RandomizeGears()
        {
            Debug.Log("RandomizeGears");
            int counter = 0;
            while (counter < level.unchangeableGearCount)
            {
                int num = Random.Range(0, level.rowCount * level.columnCount);

                if (levelParent.transform.GetChild(num).TryGetComponent(out IGear iGear) && levelParent.transform.GetChild(num).GetComponent<Gear>().changable == true)
                {
                    levelParent.transform.GetChild(num).GetComponent<Gear>().changable = false;
                    levelParent.transform.GetChild(num).GetComponent<Image>().sprite = level.selected;
                    levelParent.transform.GetChild(num).GetComponent<Gear>().highlighted = true;

                    counter++;
                }
            }
        }

        //close mirrors regarding selected shape
        private void ArrangeMirrors()
        {
            for (int i = 0; i < mirrors.Count; i++)
            {
                if (level.LshapePosition == LshapePosition.TopRight)
                    if (mirrors[i].GetComponent<Mirror>().X < mirrorPosX ||
                        mirrors[i].GetComponent<Mirror>().Y > mirrorPosY)
                        mirrors[i].GetComponent<Image>().enabled = false;

                if (level.LshapePosition == LshapePosition.TopLeft)
                    if (mirrors[i].GetComponent<Mirror>().X > mirrorPosX ||
                        mirrors[i].GetComponent<Mirror>().Y > mirrorPosY)
                        mirrors[i].GetComponent<Image>().enabled = false;

                if (level.LshapePosition == LshapePosition.BottomRight)
                    if (mirrors[i].GetComponent<Mirror>().X < mirrorPosX ||
                        mirrors[i].GetComponent<Mirror>().Y < mirrorPosY)
                        mirrors[i].GetComponent<Image>().enabled = false;

                if (level.LshapePosition == LshapePosition.BottomLeft)
                    if (mirrors[i].GetComponent<Mirror>().X > mirrorPosX ||
                        mirrors[i].GetComponent<Mirror>().Y < mirrorPosY)
                        mirrors[i].GetComponent<Image>().enabled = false;
            }
        }

        public void Check(Gear gearToCheck)
        {
            //if tapped gear exists in answerlist then correct move
            if (AnswerList.Contains(gearToCheck))
            {
                uiManager.UpdateProgressBar();
                gearToCheck.changable = false;
                gearToCheck.TurnGreen();
                AudioManager.instance.PlayOneShot("Correct");
                AnswerList.Remove(gearToCheck);
                CheckLevelComplete();
                uiManager.LightGreen();
                correctCounter++;
            }
            //if not wrong move
            else
            {
                errorCounter++;

                //unselect gear
                tappedGear = gearToCheck;
                AudioManager.instance.PlayOneShot("Wrong");
                gearToCheck.TurnRed();

                uiManager.LightRed();
            }
            levelDifState = LevelDifficultyState.Same;
        }

        public void CheckLevelComplete()
        {
            //if answerlist is empty then all moves have played 
            if (AnswerList.Count == 0)
            {
                gameManager.state = GameManager.GameState.Success;
                StartCoroutine(AnimateUnloadLevel());
            }
        }

        public void CalculateCorrectGears()
        {
            List<GameObject> gearList = gears;

            for (int i = 0; i < gearList.Count; i++)
            {
                //calculate only the spawned unchangable gears
                if (gearList[i].GetComponent<Gear>().changable == false)
                {
                    //if mirror is only horizontal then find gears mirroring their Y value
                    if (level.randomizeMirrorOnY && !level.Lshape)
                        FindGear(gearList[i].GetComponent<Gear>().X, MirrorOnY(gearList[i].GetComponent<Gear>()));

                    //if mirror is only vertical then find gears mirroring their X value
                    else if (level.randomizeMirrorOnX && !level.Lshape)
                        FindGear(MirrorOnX(gearList[i].GetComponent<Gear>()), gearList[i].GetComponent<Gear>().Y);

                    //if mirror is L shaped then find gears mirroring both positions but exclude unreachable gears
                    else if (level.Lshape)
                    {
                        switch (level.LshapePosition)
                        {
                            case LshapePosition.TopRight:
                                if (gearList[i].GetComponent<Gear>().X > mirrorPosX ||
                                    gearList[i].GetComponent<Gear>().Y < mirrorPosY)
                                {
                                    FindGear(MirrorOnX(gearList[i].GetComponent<Gear>()), gearList[i].GetComponent<Gear>().Y);
                                    FindGear(gearList[i].GetComponent<Gear>().X, MirrorOnY(gearList[i].GetComponent<Gear>()));
                                    FindGear(MirrorOnX(gearList[i].GetComponent<Gear>()), MirrorOnY(gearList[i].GetComponent<Gear>()));

                                }
                                break;
                            case LshapePosition.TopLeft:
                                if (gearList[i].GetComponent<Gear>().X < mirrorPosX ||
                                    gearList[i].GetComponent<Gear>().Y < mirrorPosY)
                                {
                                    FindGear(MirrorOnX(gearList[i].GetComponent<Gear>()), gearList[i].GetComponent<Gear>().Y);
                                    FindGear(gearList[i].GetComponent<Gear>().X, MirrorOnY(gearList[i].GetComponent<Gear>()));
                                    FindGear(MirrorOnX(gearList[i].GetComponent<Gear>()), MirrorOnY(gearList[i].GetComponent<Gear>()));

                                }
                                break;
                            case LshapePosition.BottomRight:
                                if (gearList[i].GetComponent<Gear>().X > mirrorPosX ||
                                    gearList[i].GetComponent<Gear>().Y > mirrorPosY)
                                {
                                    FindGear(MirrorOnX(gearList[i].GetComponent<Gear>()), gearList[i].GetComponent<Gear>().Y);
                                    FindGear(gearList[i].GetComponent<Gear>().X, MirrorOnY(gearList[i].GetComponent<Gear>()));
                                    FindGear(MirrorOnX(gearList[i].GetComponent<Gear>()), MirrorOnY(gearList[i].GetComponent<Gear>()));

                                }
                                break;
                            case LshapePosition.BottomLeft:
                                if (gearList[i].GetComponent<Gear>().X < mirrorPosX ||
                                    gearList[i].GetComponent<Gear>().Y > mirrorPosY)
                                {
                                    FindGear(MirrorOnX(gearList[i].GetComponent<Gear>()), gearList[i].GetComponent<Gear>().Y);
                                    FindGear(gearList[i].GetComponent<Gear>().X, MirrorOnY(gearList[i].GetComponent<Gear>()));
                                    FindGear(MirrorOnX(gearList[i].GetComponent<Gear>()), MirrorOnY(gearList[i].GetComponent<Gear>()));

                                }
                                break;
                        }
                    }
                }
            }

            if (AnswerList.Count == 0)
            {
                Debug.LogError("Level unsolvable");
                StopAllCoroutines();
                AnswerList.Clear();
                StartCoroutine(LoadNextLevel());
            }
        }

        public void FindGear(int x, int y)
        {
            List<GameObject> gearsList = gears;

            for (int i = 0; i < gearsList.Count; i++)
            {
                //find the gear that is not highlighted at the start and add if it doesnt exist in the answer key
                if (gearsList[i].GetComponent<Gear>().X == x && gearsList[i].GetComponent<Gear>().Y == y
                    && gearsList[i].GetComponent<Gear>().highlighted == false)
                    if (!AnswerList.Contains(gearsList[i].GetComponent<Gear>()))
                        AnswerList.Add(gearsList[i].GetComponent<Gear>());

                //L-shape remove out of reach gears from answer key
                if (level.Lshape)
                {
                    switch (level.LshapePosition)
                    {
                        case LshapePosition.TopRight:
                            if (gearsList[i].GetComponent<Gear>().X < mirrorPosX &&
                                gearsList[i].GetComponent<Gear>().Y > mirrorPosY)
                            {
                                AnswerList.Remove(gearsList[i].GetComponent<Gear>());
                            }
                            break;

                        case LshapePosition.TopLeft:
                            if (gearsList[i].GetComponent<Gear>().X > mirrorPosX &&
                                gearsList[i].GetComponent<Gear>().Y > mirrorPosY)
                            {
                                AnswerList.Remove(gearsList[i].GetComponent<Gear>());
                            }
                            break;

                        case LshapePosition.BottomRight:
                            if (gearsList[i].GetComponent<Gear>().X < mirrorPosX &&
                                gearsList[i].GetComponent<Gear>().Y < mirrorPosY)
                            {
                                AnswerList.Remove(gearsList[i].GetComponent<Gear>());
                            }
                            break;

                        case LshapePosition.BottomLeft:
                            if (gearsList[i].GetComponent<Gear>().X > mirrorPosX &&
                                gearsList[i].GetComponent<Gear>().Y < mirrorPosY)
                            {
                                AnswerList.Remove(gearsList[i].GetComponent<Gear>());
                            }
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        //take the symmetry of the given gear | vertical mirror | x value changes
        public int MirrorOnX(Gear gear)
        {
            int mirrorX = GetXofMirrorOnY(gear.Y);
            int newX = mirrorX - gear.X + mirrorX;
            return newX;
        }

        //take the symmetry of the given gear | horizontal mirror | y value changes
        public int MirrorOnY(Gear gear)
        {
            int mirrorY = GetYofMirrorOnX(gear.X);
            int newY = mirrorY - gear.Y + mirrorY;
            return newY;
        }

        //returns the Y position of the mirror located in given X pos
        private int GetYofMirrorOnX(int x)
        {
            List<GameObject> listMirror = mirrors;

            for (int i = 0; i < listMirror.Count; i++)
            {
                if (listMirror[i].GetComponent<Mirror>().X == x)
                    return listMirror[i].GetComponent<Mirror>().Y;
            }

            return -1;
        }

        //returns the X position of the mirror located in given Y pos
        private int GetXofMirrorOnY(int y)
        {
            List<GameObject> listMirror = mirrors;

            for (int i = 0; i < listMirror.Count; i++)
            {
                if (listMirror[i].GetComponent<Mirror>().Y == y)
                    return listMirror[i].GetComponent<Mirror>().X;
            }

            return -1;
        }


        private void ScaleGearCollider()
        {
            Debug.Log("ScaleGearCollider");
            changeableGear.GetComponent<CircleCollider2D>().radius = cellSize / 2f;
            unchangeableGear.GetComponent<CircleCollider2D>().radius = cellSize / 2f;
        }

        private void ArrangeGrid()
        {
            Debug.Log("ArrangeGrid");
            //12 is a constant i decided for the alignment
            cellSize = (10f - level.rowCount) / 10f;

            //constraint for not going above 10 row size
            if (cellSize < 0.28f)
                cellSize = 0.28f;

            levelParent.GetComponent<GridLayoutGroup>().cellSize = new Vector2(cellSize, cellSize);
            levelParent.GetComponent<GridLayoutGroup>().constraint = GridLayoutGroup.Constraint.FixedRowCount;
            levelParent.GetComponent<GridLayoutGroup>().constraintCount = level.rowCount;
        }

        private void ManualArrangeGrid()
        {
            Debug.Log("ManualArrangeGrid");
            cellSize = level.cellsize;

            levelParent.GetComponent<GridLayoutGroup>().cellSize = new Vector2(cellSize, cellSize);
            levelParent.GetComponent<GridLayoutGroup>().constraint = level.constraint;
            levelParent.GetComponent<GridLayoutGroup>().constraintCount = level.constraintCount;
        }

        public IEnumerator LoadNextLevel()
        {
            //Empty all previous level objects
            for (int i = 0; i < levelParent.transform.childCount; i++)
            {
                Destroy(levelParent.transform.GetChild(i).gameObject);
            }
            mirrors.Clear();
            gears.Clear();
            uiManager.updateProgressbarFlag = true;

            yield return new WaitForSeconds(1f);

            if (levelParent.transform.childCount != 0)
                yield return LoadNextLevel();
            else
            {
                //create level
                ReadLevelData();
                yield return new WaitForEndOfFrame();
                ChangeGearSprite();
                yield return new WaitForEndOfFrame();
                GenerateLevel();
                yield return new WaitForEndOfFrame();
                uiManager.UpdateLevelNo();
                yield return new WaitForEndOfFrame();
                uiManager.UpdateBottomGearImage();
                yield return new WaitForEndOfFrame();
                uiManager.counterIndicator = 0;
                errorCounter = 0;
                StartCoroutine(AnimateLoadLevel());
            }
        }

        private void DecideLevelIndex()
        {
            if (errorCounter == 0)
            {
                int upCounter = PlayerPrefs.GetInt("ReflectoGear_UpCounter", 0);
                if (++upCounter >= 2)
                {
                    upCounter = 0;
                    levelId++;
                }
                PlayerPrefs.SetInt("ReflectoGear_UpCounter", upCounter);
            }
            else
            {
                levelId--;
            }

            levelId = Mathf.Clamp(levelId, 0, levelList.Count - 1);
            PlayerPrefs.SetInt("level", levelId);
        }

        public void StartGameRoutine()
        {
            Debug.Log("StartGameRoutine");
            StartCoroutine(StartGame());
        }

        IEnumerator StartGame()
        {
            Debug.Log("StartGame");
            ReadLevelData();
            yield return new WaitForEndOfFrame();
            ChangeGearSprite();
            yield return new WaitForEndOfFrame();
            GenerateLevel();
            yield return new WaitForEndOfFrame();

            uiManager.UpdateBottomGearImage();
            uiManager.UpdateLevelNo();

            StartCoroutine(AnimateLoadLevel());
        }

        IEnumerator AnimateLoadLevel()
        {
            CalculateCorrectGears();
            yield return new WaitForSeconds(1f);

            for (int i = 0; i < levelParent.transform.childCount; i++)
            {
                if (!levelParent.transform.GetChild(i).TryGetComponent(out Mirror _mirror))
                {
                    levelParent.transform.GetChild(i).GetComponent<Image>().enabled = true;
                    AudioManager.instance.PlayOneShot("Spawn");
                    yield return new WaitForSeconds(gearSpawnTime);
                }
            }
            uiManager.nextText.GetComponent<TextMeshProUGUI>().enabled = false;
            StartCoroutine(AnimateMirrors(true));
            StartCoroutine(uiManager.OpenLid());
            gameManager.state = GameManager.GameState.Playing;
        }

        public IEnumerator AnimateUnloadLevel()
        {
            gameManager.state = GameManager.GameState.Idle;
            uiManager.nextText.GetComponent<TextMeshProUGUI>().enabled = true;

            //delay mirror despawning
            yield return new WaitForSeconds(1f);

            StartCoroutine(AnimateMirrors(false));

            for (int i = levelParent.transform.childCount - 1; i >= 0; i--)
            {
                if (!levelParent.transform.GetChild(i).TryGetComponent(out Mirror _mirror))
                {
                    levelParent.transform.GetChild(i).GetComponent<Image>().enabled = false;
                    AudioManager.instance.PlayOneShot("Spawn");
                    yield return new WaitForSeconds(gearSpawnTime);
                }
            }

            yield return new WaitForEndOfFrame();

            CalculateScore();

            //decide on which level to load
            DecideLevelIndex();

            if (CheckPlayLimit())
                yield break;

            StartCoroutine(LoadNextLevel());
        }

        private bool CheckPlayLimit()
        {
            totalPlayCount++;

            if (totalPlayCount == 2)
            {
                gameManager.Finish();
                return true;
            }

            return false;
        }

        private void CalculateScore()
        {
            if (errorCounter == 0)
                score += (correctCounter * 2) + bonusScore;
            else
                score += (correctCounter - errorCounter) + (bonusScore / errorCounter);
        }

        public int GetTotalScore()
        {
            return Mathf.Clamp(score / totalPlayCount, 0, 1000);
        }

        IEnumerator AnimateMirrors(bool boolean)
        {
            for (int i = 0; i < mirrors.Count; i++)
            {
                mirrors[i].SetActive(boolean);
            }
            AudioManager.instance.PlayOneShot("MirrorSpawn");
            yield return null;
        }

        public enum LshapePosition
        {
            TopRight,
            TopLeft,
            BottomRight,
            BottomLeft
        }

        public enum LevelDifficultyState
        {
            Easier,
            Same,
            Harder
        }
    }
}