using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PhotoCapture : MonoBehaviour
{
    [Header("Photo Taker")]
    [SerializeField] private Image photoDisplayer;
    [SerializeField] private GameObject photoFrame;
    [SerializeField] private AudioClip photoSoundEffect;

    [Header("Flash Effect")]
    [SerializeField] private GameObject cameraFlash;
    [SerializeField] private float flashTime;

    [Header("Photo Fader Effect")]
    [SerializeField] private Animator fadingAnimation;

    private Texture2D screenCapture;
    public bool viewingPhoto = false;

    private void Start()
    {
        screenCapture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
    }

    public IEnumerator CapturePhoto()
    {
        yield return new WaitForEndOfFrame();
        
        Rect regionToRead = new Rect(0, 0, Screen.width, Screen.height);

        screenCapture.ReadPixels(regionToRead, 0, 0, false);
        screenCapture.Apply();

        SavePhoto();

        //SoundFXManager.Instance.PlaySound(photoSoundEffect, transform);
        ShowPhoto();
        StartCoroutine(FlashEffect());
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
}
