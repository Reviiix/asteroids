using Obstacles;
using Statistics;
using Player;
using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private const bool ShowDebugMessages = true;
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

    private void Awake()
    {
        InitialiseVariables();
    }
    
    private void Start()
    {
        StartGame();
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

    private void StartGame()
    {
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

    public Transform ReturnPLayer()
    {
        return playerManager.playerObject;
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
        public Transform playerObject;
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

