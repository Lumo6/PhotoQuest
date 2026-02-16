using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "NewPhotoQuest", menuName = "PhotoGame/Photo Quest")]
public class PhotoQuestObject : ScriptableObject
{
    public enum TimeRequirement
    {
        None,
        Day,
        Night,
        Sunset,
        Concert
    }

    [Header("Quest Info")]
    public string questTitle;
    [TextArea]
    public string description;

    [Header("Validation Criteria")]
    public List<string> requiredTags;
    public TimeRequirement timeRequirement;

    [Header("Rewards")]
    public int goodVibesPoints;
    public PhotoQuestObject nextQuest;
}
