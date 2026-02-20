using UnityEngine;

/// <summary>
/// Gère l'état global du jeu, la progression, la gestion des scores et la logique principale.
/// Utilise le pattern Singleton.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum TimeOfDay
    {
        Day,
        Sunset,
        Night,
        Concert
    }

    [Header("Cycle Settings")]
    [SerializeField] private float totalDayDuration = 1200f;
    [SerializeField] private float sunsetStart = 800f;
    [SerializeField] private float nightStart = 1000f;
    [SerializeField] private int day = 0;

    [Header("Quest")]
    [SerializeField] private PhotoQuestObject[] quests;
    public PhotoQuestObject currentQuest;

    [Header("Good Vibes System")]
    [SerializeField] private float goodVibesPoints = 0f;
    [SerializeField] private float goodVibesMultiplier = 1f;

    private float currentTime;
    public TimeOfDay CurrentTimeOfDay { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (quests.Length > 0)
            currentQuest = quests[0];
        else
            Debug.Log("Remember to assign quests in the GameManager!");
    }

    private void Update()
    {
        currentTime += Time.deltaTime;

        if (currentTime >= totalDayDuration)
        {
            currentTime = 0f;
            day += 1;
        }


        UpdateTimeOfDay();
    }

    private void UpdateTimeOfDay()
    {
        if (day % 7 == 6) // Concert every sunday
            CurrentTimeOfDay = TimeOfDay.Concert;
        else if (currentTime < sunsetStart)
            CurrentTimeOfDay = TimeOfDay.Day;
        else if (currentTime < nightStart)
            CurrentTimeOfDay = TimeOfDay.Sunset;
        else
            CurrentTimeOfDay = TimeOfDay.Night;
    }

    public void AddGoodVibes(float points)
    {
        goodVibesPoints += points * goodVibesMultiplier;
    }

    public void nextQuest()
    {
        int currentIndex = System.Array.IndexOf(quests, currentQuest);
        if (currentIndex < quests.Length - 1)
            currentQuest = quests[currentIndex + 1];
        else
            Debug.Log("All quests completed!");
    }
}
