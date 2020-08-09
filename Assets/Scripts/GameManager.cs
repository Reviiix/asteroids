using Obstacles;
using Statistics;
using Player;
using PlayArea;
using UnityEngine;
using System;
using System.Collections;
using Shooting;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private const bool ShowDebugMessages = true;
    [SerializeField]
    private PlayerManager playerManager;
    [SerializeField]
    private ObstacleManager obstacleManager;
    [SerializeField]
    private AudioManager audioManager;
    public Camera mainCamera;
    public ObjectPooling objectPools;
    public UserInterfaceManager userInterfaceManager;

    private void Awake()
    {
        InitialiseVariables();
    }
    
    private void Start()
    {
        InitialisePlaneOldCSharpClasses();
    }

    private void InitialisePlaneOldCSharpClasses()
    {
        userInterfaceManager.Initialise();
        playerManager.PlayerInitialise();
        BulletManager.Initialise();
        ObstacleManager.Initialise();
        TimeTracker.Initialise();
        ScoreTracker.Initialise();
        ShakeObject.Initialise();
    }
    
    private void InitialiseVariables()
    {
        instance = this;
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
        TimeTracker.StartTimer();
        obstacleManager.StartCreatObstacleSequence();
        
        DisplayDebugMessage("Game play started.");
    }

    [ContextMenu("Player Damaged")]
    public void PlayerDamaged(int damage)
    {
        ShakeObject.Shake(ReturnPlayer());
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

    public void OnLargerObstacleDestruction(int asteroidSize, Transform position)
    {
        if (asteroidSize < 0) return;
        
        ScoreTracker.IncrementScore(asteroidSize);
        
        obstacleManager.CreateObstacle(asteroidSize, position, true);
        
        obstacleManager.CreateObstacle(asteroidSize, position, true);
    }
    
    [ContextMenu("End Game Play")]
    private void EndGame()
    {
        TimeTracker.StopTimer();
        EnablePlayerConstraints(true);
        obstacleManager.StopCreatObstacleSequence();
        HighSores.SetHighScore(ScoreTracker.score);
        userInterfaceManager.EnableGameOverCanvas();
        DisplayDebugMessage("Game over");
    }
    
    public static void EnablePlayerConstraints(bool state)
    {
        PlayerMovement.EnablePlayerConstraints(state);
        PlayerShooting.canShoot = !state;
    }

    public static Transform ReturnPlayer()
    {
        return instance.playerManager.playerTransform;
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
        public Transform playerTransform;
        public Health playerHealth;
        public PlayerMovement playerMovement;
        public PlayerShooting playerShooting;

        public void PlayerInitialise()
        {
            playerMovement.Initialise();
            playerShooting.Initialise();
        }

        public void PlayerUpdate()
        {
            playerMovement.PlayerMovementUpdate();
            playerShooting.PlayerShootUpdate();
        }
    }
}

