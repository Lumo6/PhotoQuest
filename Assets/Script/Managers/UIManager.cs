using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 
/// Utilise le pattern Singleton.
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    
    /// <summary>
    /// Initialise le Singleton.
    /// </summary>
    
    [SerializeField] private GameObject scrollView;


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void ActualiazeView()
    {

    }
}
