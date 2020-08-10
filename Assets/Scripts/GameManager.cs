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
    private const int DestructionParticlePrefabPoolIndex = 3;
    [SerializeField]
    private PlayerManager playerManager;
    [SerializeField]
    private ObstacleManager obstacleManager;
    public AudioManager audioManager;
    public Camera mainCamera;
    public ObjectPooling objectPools;
    public UserInterfaceManager userInterfaceManager;
    private const float SecondsBeforePlayerCanMoveAfterReSpawn = 0;
    private const float invincibilityFlashes = 25;
    private static Coroutine _playerDamageSequence;
    
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
        playerManager.PlayerUpdate();
        BulletManager.MoveBullets();
        ObstacleManager.MoveObstacles();
    }

    public void ReloadGame()
    {
        ScoreTracker.Initialise();
        TimeTracker.Initialise();
        
        BulletManager.DestroyAllBullets();
        ObstacleManager.DestroyAllObstacles();
        
        RestoreHealth();
        
        userInterfaceManager.EnableStartCanvas();
    }

    private void RestoreHealth()
    {
        playerManager.playerHealth.RestoreHealth();
        
        for (var i = 0; i <= Health.MaxHealth; i++)
        {
            userInterfaceManager.UpdateLivesDisplay(true);
        }
    }

    public void StartGamePlay()
    {
        EnablePlayerConstraints(false);
        
        TimeTracker.StartTimer();
        
        obstacleManager.StartCreatObstacleSequence();
        
        playerManager.playerRenderer.enabled = true;
        playerManager.playerRigidBody.simulated = true;

        DisplayDebugMessage("Game play started.");
    }

    private void StartInvincibilitySequence()
    {
        playerManager.playerRigidBody.simulated = false;
        _playerDamageSequence = StartCoroutine(FlashSprite.Flash(playerManager.playerRenderer, invincibilityFlashes, () =>
        {
            playerManager.playerRigidBody.simulated = true;
            StopInvincibilitySequence();
        }));
    }

    [ContextMenu("Player Damaged")]
    public void PlayerDamaged(int damage)
    {
        CreateDestructionParticle();
        audioManager.PlayDestructionSound();
        
        userInterfaceManager.UpdateLivesDisplay(false);

        GameAreaTransporter.PlaceObjectInCentre(ReturnPlayer());
        
        Health.TakeDamage(damage, delegate(bool gameOver)
        {
            if (gameOver)
            {
                EndGame();
            }
            else
            {
                StartInvincibilitySequence();
            }
        });
        DisplayDebugMessage("Player received " + damage + "damage.");
    }

    private static void CreateDestructionParticle()
    {
        ObjectPooling.ReturnObjectFromPool(DestructionParticlePrefabPoolIndex, ReturnPlayer().position, Quaternion.identity);
    }

    public void OnObstacleDestruction(int asteroidSize, Transform position)
    {
        audioManager.PlayDestructionSound();
        if (asteroidSize < 0)
        {
            CreateDestructionParticle();
            return;
        }
        
        ScoreTracker.IncrementScore(asteroidSize);

        obstacleManager.CreateObstacle(asteroidSize, position, true);
        obstacleManager.CreateObstacle(asteroidSize, position, true);
    }
    
    [ContextMenu("End Game Play")]
    private void EndGame()
    {
        EnablePlayerConstraints(true);
        playerManager.playerRigidBody.simulated = false;
        playerManager.playerRenderer.enabled = false;
        
        TimeTracker.StopTimer();
        //obstacleManager.StopCreatObstacleSequence();
        HighSores.SetHighScore(ScoreTracker.score);
        userInterfaceManager.EnableGameOverCanvas();

        DisplayDebugMessage("Game play over.");
    }
    
    public static void EnablePlayerConstraints(bool state)
    {
        PlayerMovement.EnablePlayerMovementConstraints(state);
        PlayerShooting.canShoot = !state;
    }

    private static void StopInvincibilitySequence()
    {
        if (_playerDamageSequence != null)
        {
            instance.StopCoroutine(_playerDamageSequence);
        }
        instance.playerManager.playerRigidBody.simulated = true;
        instance.playerManager.playerRenderer.enabled = true;
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
        [HideInInspector]
        public Rigidbody2D playerRigidBody;
        [HideInInspector]
        public SpriteRenderer playerRenderer;
        public Health playerHealth;
        public PlayerMovement playerMovement;
        public PlayerShooting playerShooting;

        public void PlayerInitialise()
        {
            playerRigidBody = playerTransform.GetComponent<Rigidbody2D>();
            playerRenderer = playerTransform.GetComponent<SpriteRenderer>();
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

