using UnityEngine;
using UnityEngine.AI;

public class PnjAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NavMeshAgent agent;// Reference to the NavMeshAgent component
    [SerializeField] private GameObject player;// Reference to the player GameObject

    [Header("Patrol")]
    public Vector3[] patrolPoints;// Points to move between
    private int currentIndex = 0;// Current index in the patrol points array


    private float stopTimer = 0f;// Time at which the pnj stoped


    void Awake()
    {
        // Initialize references and settings
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        HandleMovement();
    }


    /// <summary>
    /// Handle the movement of the professor based on player visibility.
    /// </summary>
    /// <param name="seesPlayer"></param>// True if the player is seen, false otherwise.
    void HandleMovement()
    {

    }

    /// <summary>
    /// Rotate the professor to face the player.
    /// </summary>
    void RotateTowardsPlayer()
    {
        if (player == null)
            return;
        // Calculate direction to player
        Vector3 direction = player.transform.position - transform.position;
        direction.y = 0f;
        // Rotate smoothly towards the player
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            1.0f * Time.deltaTime
        );
    }
}
