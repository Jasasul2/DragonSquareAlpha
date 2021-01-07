using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelClass : MonoBehaviour
{
    public LevelClass previousLevel;

    public LevelAttributes thisLevel;
    public LevelData data = new LevelData(false, 80, 0);

    public int levelID; //1, 2, 3, atd... same as index in scenemanager
    public bool unlocked = false, completed = false; 
    public int obtainedStars = 0;
    public float highscore = 69f;
    private Button play;

    public TextMeshProUGUI IDnumber; 

    private int stepIndex;

    public RectTransform boardPosition; 
    public GameObject boardPrefab; 
    private LevelBoardReferences b;

    public Image outline;
    public Sprite basicOutline, openedOutline; 
    [HideInInspector]
    public bool showingBoard = false;
    [HideInInspector]
    public Image board;


    public void Initialize()
    {
        IDnumber.text = levelID.ToString();

        stepIndex = transform.GetSiblingIndex();
        play = GetComponent<Button>();
        Load();

        if (LevelSelectionScreen.xToLeftByID.Contains(levelID))
        {
            boardPosition.localPosition = new Vector3(boardPosition.localPosition.x * -1, boardPosition.localPosition.y, 1);
        }

        float yOffset = 0;

        foreach (int idBorder in LevelSelectionScreen.yMoveUpBorders)
        {
            if (levelID >= idBorder)
                yOffset += LevelSelectionScreen.yOffset;
        }

        boardPosition.localPosition = new Vector3(boardPosition.localPosition.x, boardPosition.localPosition.y + yOffset, 1);

        board = Instantiate(boardPrefab, boardPosition).GetComponent<Image>();

        b = board.GetComponent<LevelBoardReferences>();
        b.levelImageBoard.sprite = thisLevel.lImage;
        b.levelNameBoard.text = thisLevel.lName;
        b.flavourBoard.text = thisLevel.description;

        b.goldCoins.text = thisLevel.dragoinRewards[2].ToString();
        b.silverCoins.text = thisLevel.dragoinRewards[1].ToString();
        b.bronzeCoins.text = thisLevel.dragoinRewards[0].ToString();

        b.levelID = levelID;
        b.gold.text = thisLevel.wayPointLimits[0].ToString();
        b.silver.text = thisLevel.wayPointLimits[1].ToString();
        b.bronze.text = thisLevel.wayPointLimits[2].ToString();

        if (completed)
        {
            if (data.obtainedStars >= 1)
            {
                b.bronze.gameObject.SetActive(false);
                b.bronzeCoins.gameObject.SetActive(false);
                b.checkMarks[0].SetActive(true);

                RecolorStars(1, 0);
                if (data.obtainedStars >= 2)
                {
                    b.silver.gameObject.SetActive(false);
                    b.silverCoins.gameObject.SetActive(false);
                    b.checkMarks[1].SetActive(true);

                    RecolorStars(2, 1);
                    if (data.obtainedStars == 3)
                    {
                        b.gold.gameObject.SetActive(false);
                        b.goldCoins.gameObject.SetActive(false);
                        b.checkMarks[2].SetActive(true);

                        RecolorStars(3, 2);
                    }
                }
            }
            else
            {
                RecolorScore(LevelSelectionScreen.instance.noStarsColor);
            }
        }

        if (!unlocked)
            UnlockLevel();

        if (completed)
        {
            b.stateBoard.text = "Completed!";
            b.stateBoard.enableVertexGradient = false; 
        }

        board.gameObject.SetActive(false);

    }

    public void ClickSound()
    {
        AudioManager.PlaySound(AudioManager.Sound.Click);
    }

    public void RecolorStars(int count = 3, int index = 0)
    {
        for (int i = 0; i < count; i++)
        {
            b.stars[i].material = LevelSelectionScreen.instance.starObtainedMaterial;
            b.stars[i].color = LevelSelectionScreen.instance.starColors[index];
            b.checkMarks[i].GetComponent<Image>().color = LevelSelectionScreen.instance.starColors[index];
        }
        outline.color = LevelSelectionScreen.instance.starColors[index];
        RecolorScore(LevelSelectionScreen.instance.starColors[index]);
    }

    public void RecolorScore(Color color)
    {
        IDnumber.color = color;
        b.decoration.color = color;
        b.stateBoard.color = color;
        b.levelNameBoard.color = color;
        b.flavourBoard.color = color; 
        b.highScoreHead.color = color;
    }

    public void UnlockLevel()
    {
        if (previousLevel == null)  //to unlock the first level 
        { 
            unlocked = true;
            return; 
        }
        if (previousLevel.completed && SceneManager.sceneCountInBuildSettings - 1 >= levelID)
        { 
            unlocked = true;
        }

        if (!unlocked)
        {
            outline.gameObject.SetActive(false);
            play.interactable = false;
        }
    }

    public void Complete()
    {
        completed = true; 
    }

    public void ShowBoard(bool hideOthers = true)
    {
        if (hideOthers)
            LevelSelectionScreen.HideLevelBoards(levelID);
        showingBoard = !showingBoard;
        board.gameObject.SetActive(showingBoard);
        if (showingBoard)
        {
            outline.sprite = openedOutline;
            transform.SetAsLastSibling();
            LevelSelectionScreen.instance.coverPanel.SetActive(true);
            LevelSelectionScreen.instance.coverPanel.transform.SetSiblingIndex(transform.GetSiblingIndex() - 1);
        }
        else
        {
            outline.sprite = basicOutline;
            LevelSelectionScreen.instance.coverPanel.SetActive(false);
            transform.SetSiblingIndex(stepIndex);
        }
    }

    public void Load()
    {
        string json = SaveSystem.LoadLevelData(levelID);
        if (json != null)
        {
            data = JsonUtility.FromJson<LevelData>(json);
            highscore = data.waypointHighscore;
            completed = data.completed; 
        }
    }
}
