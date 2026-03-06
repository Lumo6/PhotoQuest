using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PhotoCapture : MonoBehaviour
{
    [Header("Photo Taker")]
    [SerializeField] private Image photoDisplayer;
    [SerializeField] private GameObject photoFrame;

    [Header("Flash Effect")]
    [SerializeField] private GameObject cameraFlash;
    [SerializeField] private float flashTime;

    [Header("Photo Fader Effect")]
    [SerializeField] private Animator fadingAnimation;

    [Header("Detection")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float detectionDistance = 80f;
    [SerializeField] private float sphereRadius = 5f;
    [SerializeField] private float maxCenterAngle = 150f;
    [SerializeField] private LayerMask detectionLayer;
    [SerializeField] private LayerMask obstructionLayer;

    [Tooltip("Clip du son de photo")]
    [SerializeField] private AudioClip photoMusicClip;

    private Texture2D screenCapture;
    public bool viewingPhoto = false;
    private GameManager gm;

    private class DetectedPhotoObject
    {
        public Transform transform;
        public List<string> tags;
        public float distance;
        public float angle;
        public float score;
    }


    private void Start()
    {
        screenCapture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        gm = GameManager.Instance;
    }

    public IEnumerator CapturePhoto()
    {
        yield return new WaitForEndOfFrame();

        UIManager.Instance.DeactivateGameUI();

        Rect regionToRead = new Rect(0, 0, Screen.width, Screen.height);

        screenCapture.ReadPixels(regionToRead, 0, 0, false);
        screenCapture.Apply();

        SavePhoto();

        float photoQuality = 0f;
        List<PhotoQuestObject> success = ValidatePhoto(out photoQuality);

        foreach (PhotoQuestObject quest in success)
        {
            quest.completed = true;
            UIManager.Instance.ShowQuestCompletedIcon(quest);
            GameManager.Instance.AddGoodVibes(quest.goodVibesPoints);
        }

        UIManager.Instance.ActualiazeView(gm.quests);

        ShowPhoto();
        UIManager.Instance.ActivateGameUI();
        StartCoroutine(FlashEffect());
        SoundFXManager.Instance.PlaySound(photoMusicClip, playerCamera.transform);
    }

    void SavePhoto()
    {
        string folderPath = Path.Combine(Application.persistentDataPath, "Photos");

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string fileName = "photo_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
        string filePath = Path.Combine(folderPath, fileName);

        byte[] pngData = screenCapture.EncodeToPNG();
        File.WriteAllBytes(filePath, pngData);

        Debug.Log("Photo saved to: " + filePath);
    }

    void ShowPhoto()
    {
        Sprite photoSprite = Sprite.Create(screenCapture, new Rect(0.0f, 0.0f, screenCapture.width, screenCapture.height), new Vector2(0.5f, 0.5f), 100.0f);
        photoDisplayer.sprite = photoSprite;

        viewingPhoto = true;
        photoFrame.SetActive(true);

        fadingAnimation.Play("PhotoFade");

    }

    IEnumerator FlashEffect()
    {
        cameraFlash.SetActive(true);
        yield return new WaitForSeconds(flashTime);
        cameraFlash.SetActive(false);
    }

    public void RemovePhoto()
    {
        viewingPhoto = false;
        photoFrame.SetActive(false);
    }

    List<PhotoQuestObject> ValidatePhoto(out float averageScore)
    {
        averageScore = 0f;

        List<DetectedPhotoObject> objects = DetectTags();
        List<PhotoQuestObject> completedQuests = new List<PhotoQuestObject>();

        float totalScore = 0f;
        int validTagCount = 0;

        foreach (PhotoQuestObject quest in gm.quests)
        {
            bool questValid = true;

            // Vérification du moment de la journée pour cette quête
            if (!IsTimeMatching(quest.timeRequirement))
                questValid = false;

            if (!questValid || quest.completed)
                continue;

            foreach (string requiredTag in quest.requiredTags)
            {
                float bestScoreForTag = -1f;

                foreach (DetectedPhotoObject obj in objects)
                {
                    if (obj.tags.Contains(requiredTag))
                    {
                        if (obj.score > bestScoreForTag)
                            bestScoreForTag = obj.score;
                    }
                }

                if (bestScoreForTag < 0f)
                {
                    questValid = false;
                    break;
                }

                totalScore += bestScoreForTag;
                validTagCount++;
            }

            if (questValid)
            {
                completedQuests.Add(quest);
                UIManager.Instance.ShowQuestCompletedIcon(quest);
            }
        }

        if (validTagCount > 0)
            averageScore = totalScore / validTagCount;

        return completedQuests;
    }

    List<DetectedPhotoObject> DetectTags()
    {
        List<DetectedPhotoObject> detectedObjects = new List<DetectedPhotoObject>();

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        RaycastHit[] hits = Physics.SphereCastAll(
            ray,
            sphereRadius,
            detectionDistance,
            detectionLayer
        );

        foreach (RaycastHit hit in hits)
        {
            Transform target = hit.collider.transform;
            TagScript tagScript = target.GetComponent<TagScript>();
            if (tagScript == null)
                continue;

            Vector3 dir = hit.point - playerCamera.transform.position;
            float distance = dir.magnitude;

            if (distance > detectionDistance)
                continue;

            float angle = Vector3.Angle(playerCamera.transform.forward, dir);
            if (angle > maxCenterAngle)
                continue;

            float distanceScore = Mathf.InverseLerp(detectionDistance, 0, distance);
            float angleScore = Mathf.InverseLerp(maxCenterAngle, 0, angle);
            float score = (distanceScore * 0.5f + angleScore * 0.5f) * 100f;

            DetectedPhotoObject obj = new DetectedPhotoObject
            {
                transform = target,
                tags = tagScript.tags,
                distance = distance,
                angle = angle,
                score = score
            };
            detectedObjects.Add(obj);
        }

        return detectedObjects;
    }

    bool IsTimeMatching(PhotoQuestObject.TimeRequirement requirement)
    {
        GameManager.TimeOfDay current = gm.CurrentTimeOfDay;

        switch (requirement)
        {
            case PhotoQuestObject.TimeRequirement.None:
                return true;

            case PhotoQuestObject.TimeRequirement.Day:
                return current == GameManager.TimeOfDay.Day;

            case PhotoQuestObject.TimeRequirement.Sunset:
                return current == GameManager.TimeOfDay.Sunset;

            case PhotoQuestObject.TimeRequirement.Night:
                return current == GameManager.TimeOfDay.Night;

            case PhotoQuestObject.TimeRequirement.Concert:
                return current == GameManager.TimeOfDay.Concert;

            default:
                return false;
        }
    }
}
