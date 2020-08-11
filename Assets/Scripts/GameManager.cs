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
    public const Difficulty GameDifficulty = Difficulty.Hard;
    public static bool gameOver = false;
    private const bool ShowDebugMessages = true;
    public static GameManager instance;
    public UserInterfaceManager userInterfaceManager;
    public PlayerManager playerManager;
    public ObstacleManager obstacleManager;
    public AudioManager audioManager;
    public ObjectPooling objectPools;
    public Camera mainCamera;

    private void Awake()
    {
        InitialiseVariables();
    }
    
    private void Start()
    {
        InitialisePlaneOldCSharpClasses();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
    
    private void InitialiseVariables()
    {
        instance = this;
    }

    private void InitialisePlaneOldCSharpClasses()
    {
        userInterfaceManager.Initialise();
        playerManager.PlayerInitialise();
        AudioManager.CreateHashSetOfAudioSourcesFromPools();
        BulletManager.Initialise();
        ObstacleManager.Initialise();
        TimeTracker.Initialise();
        ScoreTracker.Initialise();
    }

    private void Update()
    {
        if (gameOver) return;
            
        playerManager.PlayerUpdate();
        BulletManager.MoveBullets();
        ObstacleManager.MoveObstacles();
    }

    [ContextMenu("Reload Game")]
    public void ReloadGame()
    {
        gameOver = false;
        ScoreTracker.Initialise();
        TimeTracker.Initialise();
        
        BulletManager.DestroyAllBullets();
        ObstacleManager.DestroyAllObstacles();
        
        playerManager.RestoreHealth();
        
        userInterfaceManager.EnableStartCanvas();
        
        DisplayDebugMessage("Game reloaded. Obstacles and bullets \"destroyed\", time/score reset and start canvas enabled.");
    }

    [ContextMenu("Start Game Play")]
    public void StartGamePlay()
    {
        PlayerManager.EnablePlayerConstraints(false);
        
        TimeTracker.StartTimer();
        
        obstacleManager.StartCreatObstacleSequence();
        
        playerManager.playerRenderer.enabled = true;
        playerManager.StopInvincibilitySequence();
        PlayerShooting.canShoot = true;

        DisplayDebugMessage("Game play started.");
    }
    
    
    public void OnPlayerCollision(int damage)
    {
        if (!PlayerHealth.canBeDamaged)
        {
            return;
        }
        GameArea.CreateDestructionParticle(ReturnPlayer().position);
        
        audioManager.PlayDamageSound();
        
        userInterfaceManager.UpdateLivesDisplay(false);
        
        PlayerManager.EnablePlayerConstraints();

        GameAreaTransporter.PlaceObjectInCentre(ReturnPlayer());
        
        PlayerHealth.TakeDamage(damage, delegate(bool gameOver)
        {
            if (gameOver)
            {
                EndGame();
            }
            else
            {
                StartCoroutine(Wait(PlayerManager.SecondsBeforePlayerCanMoveAfterReSpawn, () =>
                {
                    PlayerManager.EnablePlayerConstraints(false);
                    playerManager.StartInvincibilitySequence();
                }));
            }
        });
        DisplayDebugMessage("Player received " + damage + "damage.");
    }

    public void OnObstacleDestruction(int asteroidSize, Transform position)
    {
        audioManager.PlayDestructionSound();
        if (asteroidSize < 0)
        {
            GameArea.CreateDestructionParticle(position.position);
            return;
        }
        
        ScoreTracker.IncrementScore(asteroidSize);

        obstacleManager.CreateObstacle(asteroidSize, position, true);
        obstacleManager.CreateObstacle(asteroidSize, position, true);
    }
    
    [ContextMenu("End Game Play")]
    private void EndGame()
    {
        gameOver = true;
        PlayerManager.EnablePlayerConstraints();
        PlayerHealth.canBeDamaged = false;
        PlayerShooting.canShoot = false;
        playerManager.playerRenderer.enabled = false;
        
        obstacleManager.StopCreatObstacleSequence();

        TimeTracker.StopTimer();
        HighSores.SetHighScore(ScoreTracker.score);
        
        userInterfaceManager.EnableGameOverCanvas();

        DisplayDebugMessage("Game play over.");
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

public enum Difficulty
{
    //obstacles have a 1 in x chance of not spawning.
    Easy  = 2,
    Medium  = 10,
    Hard = 100
}


