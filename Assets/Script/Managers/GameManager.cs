using UnityEngine;

/// <summary>
/// Gère l'état global du jeu, la progression, la gestion des scores et la logique principale.
/// Utilise le pattern Singleton.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Instance unique du GameManager

    // Enumération des différents états possibles du jeu
    

    /// <summary>
    /// Initialisation du Singleton et des paramètres de difficulté.
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
