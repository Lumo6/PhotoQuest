using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PnjAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private GameObject player;
    [SerializeField] private Animator animator;

    [Header("Patrol")]
    public List<GameObject> patrolPoints;
    private int currentIndex = 0;

    private float stopTimer = 300f;
    private bool waiting = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Start()
    {
        if (patrolPoints.Count > 0)
        {
            agent.SetDestination(patrolPoints[currentIndex].transform.position);
        }
        else
        {
            Debug.LogWarning("No patrol points assigned to " + gameObject.name);
        }
    }

    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        if (patrolPoints.Count == 0)
            return;

        // Check if the agent reached the destination
        if (!waiting && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            waiting = true;
            stopTimer = GameManager.Instance.getTime();
            agent.isStopped = true;
            animator.SetBool("iswalking",false);
            ActionAnimation(patrolPoints[currentIndex]);
        }

        // Wait for 30 seconds
        if (waiting)
        {
            if (GameManager.Instance.getTime() - stopTimer >= 5f)
            {
                animator.SetBool("iswalking", true);
                CancelActionAnimation(patrolPoints[currentIndex]);
                GoToNextPoint();
            }
        }
    }

    void ActionAnimation(GameObject POI)
    {
        string animationName = POI.GetComponent<TagScript>().tags[0];

        switch (animationName)
        {
            case "sitting":
                animator.SetBool("issitting", true);
                break;
            case "praying":
                animator.SetBool("ispraying", true);
                break;
            case "idle":
                animator.SetBool("isidle", true);
                break;
            default:
                Debug.LogWarning("Unknown animation tag: " + animationName);
                break;
        }
    }

    void CancelActionAnimation(GameObject POI)
    {
        string animationName = POI.GetComponent<TagScript>()?.tags[0];

        switch (animationName)
        {
            case "sitting":
                animator.SetBool("issitting", false);
                break;
            case "praying":
                animator.SetBool("ispraying", false);
                break;
            case "idle":
                animator.SetBool("isidle", false);
                break;
            default:
                Debug.LogWarning("Unknown animation tag: " + animationName);
                break;
        }
    }

    void GoToNextPoint()
    {
        waiting = false;
        agent.isStopped = false;

        currentIndex++;
        if (currentIndex >= patrolPoints.Count)
        {
            GameManager.Instance.removeNpc(gameObject);
            return;
        }

        agent.SetDestination(patrolPoints[currentIndex].transform.position);
    }

    void RotateTowardsPlayer()
    {
        if (player == null)
            return;

        Vector3 direction = player.transform.position - transform.position;
        direction.y = 0f;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            1.0f * Time.deltaTime
        );
    }

    public void SetActivity(List<GameObject> activityPoints)
    {
        patrolPoints = activityPoints;
        currentIndex = 0;
        if (patrolPoints.Count > 0)
        {
            agent.SetDestination(patrolPoints[currentIndex].transform.position);
        }
    }
}