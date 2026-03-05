using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Utilise le pattern Singleton.
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private Transform scrollContent;
    [SerializeField] private Sprite completedIconSprite;

    [SerializeField] private GameObject endGameUI;
    [SerializeField] private GameObject GameUI;
    [SerializeField] private GameObject PhotoUI;
    [SerializeField] private TMPro.TMP_Text endGameText;
    [SerializeField] private TMPro.TMP_Text dayNumberText;
    [SerializeField] private TMPro.TMP_Text vibeScoreText;



    private Dictionary<PhotoQuestObject, Image> questIcons = new Dictionary<PhotoQuestObject, Image>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void ActualiazeView(List<PhotoQuestObject> quests)
    {
        foreach (Transform child in scrollContent)
        {
            Destroy(child.gameObject);
        }

        questIcons.Clear();

        foreach (PhotoQuestObject quest in quests)
        {
            CreateQuestUI(quest);
        }
    }

    void CreateQuestUI(PhotoQuestObject quest)
    {
        GameObject questItem = new GameObject(quest.questTitle);
        questItem.transform.SetParent(scrollContent);

        VerticalLayoutGroup layout = questItem.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(10, 10, 10, 10);

        // TITLE
        GameObject titleGO = new GameObject("Title");
        titleGO.transform.SetParent(questItem.transform);
        TextMeshProUGUI titleText = titleGO.AddComponent<TextMeshProUGUI>();
        titleText.text = quest.questTitle;
        titleText.fontSize = 28;

        // DESCRIPTION
        GameObject descGO = new GameObject("Description");
        descGO.transform.SetParent(questItem.transform);
        TextMeshProUGUI descText = descGO.AddComponent<TextMeshProUGUI>();
        descText.text = quest.description;
        descText.fontSize = 20;

        // REWARD
        GameObject rewardGO = new GameObject("Reward");
        rewardGO.transform.SetParent(questItem.transform);
        TextMeshProUGUI rewardText = rewardGO.AddComponent<TextMeshProUGUI>();
        rewardText.text = "Good vibes : +" + quest.goodVibesPoints;
        rewardText.fontSize = 18;

        // ICON
        GameObject iconGO = new GameObject("CompletedIcon");
        iconGO.transform.SetParent(questItem.transform);

        Image icon = iconGO.AddComponent<Image>();
        icon.sprite = completedIconSprite;
        icon.enabled = false; // désactivé par défaut

        questIcons.Add(quest, icon);
    }

    public void ShowQuestCompletedIcon(PhotoQuestObject quest)
    {
        if (questIcons.ContainsKey(quest))
        {
            questIcons[quest].enabled = true;
        }
    }

    public void ShowEndGameUI(string msg)
    {
        PhotoUI.SetActive(false);
        GameUI.SetActive(false);
        endGameUI.SetActive(true);
        endGameText.text = msg;
    }

    public void UpdateDayNumber(int day)
    {
        dayNumberText.text = "Day n°" + day;
    }

    public void UpdateVibeScore(int score)
    {
        vibeScoreText.text = "Good Vibes Score : " + score;
    }

    public void DeactivateGameUI()
    {
        GameUI.SetActive(false);
    }

    public void ActivateGameUI()
    {
        GameUI.SetActive(true);
    }
}