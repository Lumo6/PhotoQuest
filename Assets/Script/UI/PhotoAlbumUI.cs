using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class PhotoAlbumUI : MonoBehaviour
{
    [Header("UI")]
    public List<RawImage> photoSlots;
    public Button nextButton;
    public Button prevButton;
    public Text pageText;

    private List<Texture2D> photos = new List<Texture2D>();
    private int currentPage = 0;
    private int totalPages = 0;

    void Start()
    {
        LoadPhotos();
        ShowPage(0);

        nextButton.onClick.AddListener(NextPage);
        prevButton.onClick.AddListener(PrevPage);
    }

    void LoadPhotos()
    {
        string folder = Path.Combine(Application.persistentDataPath, "Photos");

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        string[] files = Directory.GetFiles(folder, "*.png");

        foreach (string file in files)
        {
            byte[] bytes = File.ReadAllBytes(file);

            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(bytes);

            photos.Add(tex);
        }

        totalPages = Mathf.CeilToInt((float)photos.Count / photoSlots.Count);
    }

    void ShowPage(int page)
    {
        currentPage = Mathf.Clamp(page, 0, totalPages - 1);

        for (int i = 0; i < photoSlots.Count; i++)
        {
            int photoIndex = currentPage * photoSlots.Count + i;

            if (photoIndex < photos.Count)
            {
                photoSlots[i].texture = photos[photoIndex];
                photoSlots[i].gameObject.SetActive(true);
            }
            else
            {
                photoSlots[i].gameObject.SetActive(false);
            }
        }

        if (pageText != null)
            pageText.text = $"Page {currentPage + 1}/{Mathf.Max(totalPages, 1)}";

        prevButton.interactable = currentPage > 0;
        nextButton.interactable = currentPage < totalPages - 1;
    }

    public void NextPage()
    {
        ShowPage(currentPage + 1);
    }

    public void PrevPage()
    {
        ShowPage(currentPage - 1);
    }
}
