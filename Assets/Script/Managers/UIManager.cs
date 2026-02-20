using UnityEngine;

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
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

}
