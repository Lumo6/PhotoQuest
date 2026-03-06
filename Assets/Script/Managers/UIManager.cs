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

    private void Start()
    {
        endGameUI.SetActive(false);
        GameUI.SetActive(true);
        PhotoUI.SetActive(true);

        UpdateVibeScore(0);
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
        GameObject questItem = new GameObject(quest.questTitle, typeof(RectTransform));
        questItem.transform.SetParent(scrollContent, false);

        // Background
        Image bg = questItem.AddComponent<Image>();
        bg.color = new Color(0.15f, 0.15f, 0.15f, 0.7f);

        // Layout horizontal principal
        HorizontalLayoutGroup rootLayout = questItem.AddComponent<HorizontalLayoutGroup>();
        rootLayout.padding = new RectOffset(20, 20, 20, 20);
        rootLayout.spacing = 15;
        rootLayout.childAlignment = TextAnchor.MiddleLeft;
        rootLayout.childControlWidth = true;
        rootLayout.childControlHeight = true;
        rootLayout.childForceExpandWidth = false;
        rootLayout.childForceExpandHeight = true;

        ContentSizeFitter fitter = questItem.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // =========================
        // TEXT CONTAINER
        // =========================

        GameObject textContainer = new GameObject("TextContainer", typeof(RectTransform));
        textContainer.transform.SetParent(questItem.transform, false);

        VerticalLayoutGroup textLayout = textContainer.AddComponent<VerticalLayoutGroup>();
        textLayout.spacing = 8;
        textLayout.childForceExpandHeight = false;
        textLayout.childForceExpandWidth = true;

        LayoutElement textElement = textContainer.AddComponent<LayoutElement>();
        textElement.flexibleWidth = 1;

        // HEADER
        GameObject header = new GameObject("Header", typeof(RectTransform));
        header.transform.SetParent(textContainer.transform, false);

        HorizontalLayoutGroup headerLayout = header.AddComponent<HorizontalLayoutGroup>();
        headerLayout.spacing = 10;
        headerLayout.childForceExpandWidth = false;

        // TITLE
        GameObject titleGO = new GameObject("Title", typeof(RectTransform));
        titleGO.transform.SetParent(header.transform, false);

        TextMeshProUGUI titleText = titleGO.AddComponent<TextMeshProUGUI>();
        titleText.text = quest.questTitle;
        titleText.fontSize = 28;
        titleText.color = Color.white;

        // DESCRIPTION
        GameObject descGO = new GameObject("Description", typeof(RectTransform));
        descGO.transform.SetParent(textContainer.transform, false);

        TextMeshProUGUI descText = descGO.AddComponent<TextMeshProUGUI>();
        descText.text = quest.description;
        descText.fontSize = 20;
        descText.color = new Color(0.8f, 0.8f, 0.8f);

        // REWARD
        GameObject rewardGO = new GameObject("Reward", typeof(RectTransform));
        rewardGO.transform.SetParent(textContainer.transform, false);

        TextMeshProUGUI rewardText = rewardGO.AddComponent<TextMeshProUGUI>();
        rewardText.text = "Good vibes : +" + quest.goodVibesPoints;
        rewardText.fontSize = 18;
        rewardText.color = Color.yellow;

        // ICON

        GameObject iconGO = new GameObject("CompletedIcon", typeof(RectTransform));
        iconGO.transform.SetParent(questItem.transform, false);

        Image icon = iconGO.AddComponent<Image>();
        icon.sprite = completedIconSprite;
        icon.enabled = false;

        LayoutElement iconLayout = iconGO.AddComponent<LayoutElement>();
        iconLayout.preferredWidth = 60;
        iconLayout.minWidth = 60;
        iconLayout.flexibleHeight = 1;

        // force carré
        AspectRatioFitter aspect = iconGO.AddComponent<AspectRatioFitter>();
        aspect.aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;
        aspect.aspectRatio = 1;

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