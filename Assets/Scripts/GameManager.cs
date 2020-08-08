using Obstacles;
using Statistics;
using Player;
using PlayArea;
using UnityEngine;
using System;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private const bool ShowDebugMessages = true;
    [SerializeField]
    private AudioManager audioManager;
    public Camera mainCamera;
    public ObjectPooling objectPools;
    [SerializeField]
    private PlayerManager playerManager;
    [SerializeField]
    private ScoreManager scoreManager;
    [SerializeField]
    private TimeTracker timeManager;
    [SerializeField]
    private ObstacleManager obstacleManager;
    public UserInterfaceManager userInterfaceManager;
    private static GameAreaTransporter _transporter = new GameAreaTransporter();

    private void Awake()
    {
        InitialiseVariables();
    }
    
    private void Start()
    {
        playerManager.PlayerStart();
    }
    
    private void InitialiseVariables()
    {
        Instance = this;
    }
    
    private void Update()
    {
        //One central update is better than multiple. Slight performance increase and easier to trace bugs.
        playerManager.PlayerUpdate();
    }

    public void StartGamePlay()
    {
        ReleasePlayerConstraints();
        timeManager.StartTimer();
        obstacleManager.StartCreatObstacleSequence();
        
        DisplayDebugMessage("Game play started.");
    }

    public void PlayerDamaged(int damage)
    {
        userInterfaceManager.UpdateLivesDisplay(false);
        PlayerHealth.TakeDamage(damage, delegate(bool dead)
        {
            if (dead)
            {
                EndGame();
            }
        });
        
        DisplayDebugMessage("PlayerManager received " + damage + "damage.");
    }

    public void CreateNewAsteroidFromOld(int asteroidSize, Transform position)
    {
        obstacleManager.CreateObstacle(asteroidSize, position);
    }
    
    private void EndGame()
    {
        obstacleManager.StopCreatObstacleSequence();
        timeManager.StopTimer();
        
        DisplayDebugMessage("Game over");
    }
    
    private static void ReleasePlayerConstraints()
    {
        PlayerMovement.CanMove = true;
        PlayerMovement.CanRotate = true;
    }

    public static Transform ReturnPlayer()
    {
        return Instance.playerManager.playerTransform;
    }
    
    public static void DisplayDebugMessage(string message)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        if (ShowDebugMessages)
        {
            Debug.Log(message);
        }
    }
}

namespace Player
{
    [Serializable]
    public class PlayerManager
    {
        [FormerlySerializedAs("playerObject")] public Transform playerTransform;
        public PlayerHealth playerHealth;
        public PlayerMovement playerMovement;
        public PlayerShooting playerShooting;

        public void PlayerStart()
        {
            playerMovement.Initialise();
        }

        public void PlayerUpdate()
        {
            playerMovement.PlayerMovementManagerUpdate();
            playerShooting.PlayerShootUpdate();
        }
    }
}

