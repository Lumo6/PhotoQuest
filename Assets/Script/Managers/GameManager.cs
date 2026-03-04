using NUnit.Framework;
using System.Collections.Generic;
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
    [SerializeField] private float totalDayDuration;
    [SerializeField] private float sunsetStart;
    [SerializeField] private float nightStart;
    [SerializeField] private int day = 0;

    [Header("Quest")]
    public List<PhotoQuestObject> quests;
    public PhotoQuestObject currentQuest;

    [Header("Good Vibes System")]
    [SerializeField] private float goodVibesPoints = 0f;
    [SerializeField] private float goodVibesMultiplier = 1f;

    [Header("PNJ")]
    [SerializeField] private List<GameObject> npcs;
    [SerializeField] private List<GameObject> npcprefabs;
    [SerializeField] private List<GameObject> poiactivity;
    [SerializeField] private List<GameObject> poispawn;

    [Header("Sky")]
    [SerializeField] private Light sun;
    [SerializeField] private Material skyboxMaterial;

    [SerializeField] private float daySunIntensity = 1.2f;
    [SerializeField] private float nightSunIntensity = 0f;
    


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

        //Randomize the order of quests at the start of the game
        if (quests.Count > 0)
        {
            int n = quests.Count;
            while (n >= 1)
            {
                int k = Random.Range(0, n);
                PhotoQuestObject value = quests[k];
                quests[k] = quests[n-1];
                quests[n-1] = value;
                n--;
            }
            //assign the first quest
            currentQuest = quests[0];
        }
            
        else
            Debug.Log("Remember to assign quests in the GameManager!");

        //UIManager.Instance.ActualiazeView();

        for (int i = 0; i < poispawn.Count; i++)
        {
            spawnNpc();
        }
    }

    private void Update()
    {
        currentTime += Time.deltaTime;
        Debug.Log("Current Time: " + currentTime);

        if (currentTime >= totalDayDuration)
        {
            currentTime = 0f;
            day += 1;
        }


        UpdateTimeOfDay();

        UpdateSky();
    }

    void UpdateSky()
    {
        float normalizedTime = currentTime / totalDayDuration;

        float sunAngle = normalizedTime * 360f;

        // Rotate sun across the sky
        sun.transform.rotation = Quaternion.Euler(sunAngle, 0f, 0f);

        float sunHeight = Vector3.Dot(sun.transform.forward, Vector3.down);
        float t = Mathf.Clamp01((sunHeight + 1f) * 0.5f);

        sun.intensity = Mathf.Lerp(nightSunIntensity, daySunIntensity, t);

        if (skyboxMaterial.HasProperty("_Exposure"))
        {
            float exposure = Mathf.Lerp(0.3f, 1.3f, t);
            skyboxMaterial.SetFloat("_Exposure", exposure);
        }

        DynamicGI.UpdateEnvironment();
    }
    private void spawnNpc()
    {
        GameObject npc = Instantiate(npcprefabs[Random.Range(0, npcprefabs.Count)], poispawn[Random.Range(0, poispawn.Count)].transform.position, Quaternion.identity);
        npcs.Add(npc);
        List<GameObject> activities = new List<GameObject>();
        for (int i = 0; i < 3; i++)
        {
            activities.Add(poiactivity[Random.Range(0, poiactivity.Count)]);
        }
        npc.GetComponent<PnjAI>().SetActivity(activities);
    }

    public void removeNpc(GameObject npc)
    {
        npcs.Remove(npc);
        Destroy(npc);
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
        int currentIndex = quests.IndexOf(currentQuest); // retourne l'index de l'élément dans le tableau, sinon retourne -1
        if (currentIndex != -1)
            currentQuest = quests[currentIndex + 1];
        else
            Debug.Log("All quests completed!");
    }

    public float getTime()
    {
        return currentTime;
    }
}
