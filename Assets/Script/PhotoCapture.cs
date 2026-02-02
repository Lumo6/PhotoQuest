using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PhotoCapture : MonoBehaviour
{
    private Texture2D screenCapture;

    private void Start()
    {
        screenCapture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
    }

    private void Update()
    {
        
    }

    IEnumerator CapturePhoto()
    {
        yield return new WaitForEndOfFrame();


    }
}
