using UnityEngine;
using UnityEngine.UI;

public class LevelSelectManager : MonoBehaviour
{
    public Button level2Button;
    public Button level3Button;

    public GameObject lockIcon2;
    public GameObject lockIcon3;

    public Image[] level1Stars;
    public Image[] level2Stars;
    public Image[] level3Stars;

    public Sprite starOn;
    public Sprite starOff;

    void Start()
    {
        UpdateLevelLocks();
        UpdateStars();
    }

    public void UpdateLevelLocks()
    {
        bool level1Completed = PlayerPrefs.GetInt("Level1_Completed", 0) == 1;
        bool level2Completed = PlayerPrefs.GetInt("Level2_Completed", 0) == 1;

        level2Button.interactable = level1Completed;
        lockIcon2.SetActive(!level1Completed);

        level3Button.interactable = level2Completed;
        lockIcon3.SetActive(!level2Completed);
    }

    public void UpdateStars()
    {
        // Level 1
        SetStars(level1Stars, PlayerPrefs.GetInt("Level1_Completed", 0), true);

        // Level 2
        bool level2Unlocked = PlayerPrefs.GetInt("Level1_Completed", 0) == 1;
        SetStars(level2Stars, PlayerPrefs.GetInt("Level2_Completed", 0), level2Unlocked);

        // Level 3
        bool level3Unlocked = PlayerPrefs.GetInt("Level2_Completed", 0) == 1;
        SetStars(level3Stars, PlayerPrefs.GetInt("Level3_Completed", 0), level3Unlocked);
    }

    void SetStars(Image[] stars, int isCompleted, bool isUnlocked)
    {
        for (int i = 0; i < stars.Length; i++)
        {
            if (isCompleted == 1)
                stars[i].sprite = starOn;
            else if (isUnlocked)
                stars[i].sprite = starOff;
            else
                stars[i].enabled = false; // Ẩn nếu chưa mở
        }
    }
}