using Obstacles;
using Statistics;
using Player;
using PlayArea;
using UnityEngine;
using System;
using System.Collections;
using Shooting;
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

    private void Awake()
    {
        InitialiseVariables();
    }
    
    private void Start()
    {
        playerManager.PlayerStart();
        BulletManager.InitialiseBulletList();
        ObstacleManager.InitialiseObstacleList();
    }
    
    private void InitialiseVariables()
    {
        Instance = this;
    }
    
    private void Update()
    {
        //One central update is better than multiple. Slight performance increase and easier to trace bugs.
        playerManager.PlayerUpdate();
        BulletManager.MoveBullets();
        ObstacleManager.MoveObstacles();
    }

    public void StartGamePlay()
    {
        EnablePlayerConstraints(false);
        timeManager.StartTimer();
        obstacleManager.StartCreatObstacleSequence();
        
        DisplayDebugMessage("Game play started.");
    }

    public void PlayerDamaged(int damage)
    {
        userInterfaceManager.UpdateLivesDisplay(false);
        Health.TakeDamage(damage, delegate(bool dead)
        {
            if (dead)
            {
                EndGame();
            }
        });
        
        DisplayDebugMessage("PlayerManager received " + damage + "damage.");
    }

    public static void CreateNewAsteroidFromOld(int asteroidSize, Transform position)
    {
        ObstacleManager.CreateObstacle(asteroidSize, position);
    }
    
    private void EndGame()
    {
        obstacleManager.StopCreatObstacleSequence();
        timeManager.StopTimer();
        
        DisplayDebugMessage("Game over");
    }
    
    public static void EnablePlayerConstraints(bool state)
    {
        PlayerMovement.EnablePlayerConstraints(state);
        PlayerShooting.canShoot = !state;
    }

    public static Transform ReturnPlayer()
    {
        return Instance.playerManager.playerTransform;
    }

    public static IEnumerator Wait(float seconds, Action callBack)
    {
        yield return new WaitForSeconds(seconds);
        callBack();
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
        [FormerlySerializedAs("playerHealth")] public Health health;
        public PlayerMovement playerMovement;
        public PlayerShooting playerShooting;

        public void PlayerStart()
        {
            playerMovement.Initialise();
            playerShooting.Initialise();
        }

        public void PlayerUpdate()
        {
            playerMovement.PlayerMovementManagerUpdate();
            playerShooting.PlayerShootUpdate();
        }
    }
}

