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
    private const bool ShowDebugMessages = true;
    public static GameManager instance;
    public PlayerManagerffff playerManagerffff;
    public ObstacleManager obstacleManager;
    public AudioManager audioManager;
    public ObjectPooling objectPools;
    public Camera mainCamera;
    public UserInterfaceManager userInterfaceManager;

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
        playerManagerffff.PlayerInitialise();
        AudioManager.CreateHashSetOfAudioSourcesFromPools();
        BulletManager.Initialise();
        ObstacleManager.Initialise();
        TimeTracker.Initialise();
        ScoreTracker.Initialise();
    }

    private void Update()
    {
        playerManagerffff.PlayerUpdate();
        BulletManager.MoveBullets();
        ObstacleManager.MoveObstacles();
    }

    [ContextMenu("Reload Game")]
    public void ReloadGame()
    {
        ScoreTracker.Initialise();
        TimeTracker.Initialise();
        
        BulletManager.DestroyAllBullets();
        ObstacleManager.DestroyAllObstacles();
        
        playerManagerffff.RestoreHealth();
        
        userInterfaceManager.EnableStartCanvas();
        
        DisplayDebugMessage("Game reloaded. Obstacles and bullets \"destroyed\", Time and score reset. Start canvas enabled.");
    }

    [ContextMenu("Start Game Play")]
    public void StartGamePlay()
    {
        PlayerManagerffff.EnablePlayerConstraints(false);
        
        TimeTracker.StartTimer();
        
        obstacleManager.StartCreatObstacleSequence();
        
        playerManagerffff.playerRenderer.enabled = true;
        playerManagerffff.StopInvincibilitySequence();

        DisplayDebugMessage("Game play started.");
    }
    
    
    public void OnPlayerCollision(int damage)
    {
        if (!PlayerHealth.canBeDamaged)
        {
            return;
        }
        GameArea.CreateDestructionParticle(ReturnPlayer().position);
        audioManager.PlayDestructionSound();
        
        userInterfaceManager.UpdateLivesDisplay(false);
        
        PlayerManagerffff.EnablePlayerConstraints();

        GameAreaTransporter.PlaceObjectInCentre(ReturnPlayer());
        
        PlayerHealth.TakeDamage(damage, delegate(bool gameOver)
        {
            if (gameOver)
            {
                EndGame();
            }
            else
            {
                StartCoroutine(Wait(PlayerManagerffff.SecondsBeforePlayerCanMoveAfterReSpawn, () =>
                {
                    PlayerManagerffff.EnablePlayerConstraints(false);
                    playerManagerffff.StartInvincibilitySequence();
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
        PlayerManagerffff.EnablePlayerConstraints();
        PlayerHealth.canBeDamaged = false;
        obstacleManager.StopCreatObstacleSequence();
        playerManagerffff.playerRenderer.enabled = false;

        TimeTracker.StopTimer();
        HighSores.SetHighScore(ScoreTracker.score);
        userInterfaceManager.EnableGameOverCanvas();

        DisplayDebugMessage("Game play over.");
    }

    public static Transform ReturnPlayer()
    {
        return instance.playerManagerffff.playerTransform;
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
    public class PlayerManagerffff
    {
        public Transform playerTransform;
        [HideInInspector]
        public SpriteRenderer playerRenderer;
        public const float SecondsBeforePlayerCanMoveAfterReSpawn = 0.3f;
        private const float InvincibilityFlashes = 25;
        private static Coroutine _playerDamageSequence;

        public void PlayerInitialise()
        {
            playerRenderer = playerTransform.GetComponent<SpriteRenderer>();
            
            PlayerMovement.Initialise();
            PlayerShooting.Initialise();
        }

        public void PlayerUpdate()
        {
            PlayerMovement.PlayerMovementUpdate();
            PlayerShooting.PlayerShootUpdate();
        }
        
        public void StartInvincibilitySequence()
        {
            PlayerHealth.canBeDamaged = false;
            _playerDamageSequence = GameManager.instance.StartCoroutine(FlashSprite.Flash(playerRenderer, InvincibilityFlashes, StopInvincibilitySequence));
        }
        
        public void StopInvincibilitySequence()
        {
            if (_playerDamageSequence != null)
            {
                GameManager.instance.StopCoroutine(_playerDamageSequence);
            }
            PlayerHealth.canBeDamaged = true;
            playerRenderer.enabled = true;
        }
        
        public  void RestoreHealth()
        {
            var userInterface = GameManager.instance.userInterfaceManager;
            const int maxHealth = PlayerHealth.MaxHealth;
            
            PlayerHealth.RestoreHealth();

            for (var i = 0; i <= maxHealth; i++)
            {
                userInterface.UpdateLivesDisplay(true);
            }
        }
        
        public static void EnablePlayerConstraints(bool state = true)
        {
            PlayerMovement.EnablePlayerMovementConstraints(state);
            PlayerShooting.canShoot = !state;
        }
    }
}

