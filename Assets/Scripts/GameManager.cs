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
    public AudioManager audioManager;
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
        //Some these depend on the Game Manager singleton being set.
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
        
        playerManager.RestoreHealth();
        
        userInterfaceManager.EnableStartCanvas();
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
    

    [ContextMenu("Player Damaged")]
    public void PlayerDamaged(int damage)
    {
        GameArea.CreateDestructionParticle(ReturnPlayer().position);
        audioManager.PlayDestructionSound();
        
        userInterfaceManager.UpdateLivesDisplay(false);
        
        EnablePlayerConstraints();

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
                    EnablePlayerConstraints(false);
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
        EnablePlayerConstraints();
        playerManager.playerRigidBody.simulated = false;
        playerManager.playerRenderer.enabled = false;
        
        TimeTracker.StopTimer();
        HighSores.SetHighScore(ScoreTracker.score);
        userInterfaceManager.EnableGameOverCanvas();

        DisplayDebugMessage("Game play over.");
    }
    
    
    [ContextMenu("Enable Player Constraints")]
    public static void EnablePlayerConstraints(bool state = true)
    {
        PlayerMovement.EnablePlayerMovementConstraints(state);
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
        [HideInInspector]
        public Rigidbody2D playerRigidBody;
        [HideInInspector]
        public SpriteRenderer playerRenderer;
        public const float SecondsBeforePlayerCanMoveAfterReSpawn = 0.3f;
        private const float InvincibilityFlashes = 25;
        private static Coroutine _playerDamageSequence;

        public void PlayerInitialise()
        {
            playerRigidBody = playerTransform.GetComponent<Rigidbody2D>();
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
            playerRigidBody.simulated = false;
            _playerDamageSequence = GameManager.instance.StartCoroutine(FlashSprite.Flash(playerRenderer, InvincibilityFlashes, () =>
            {
                playerRigidBody.simulated = true;
                StopInvincibilitySequence();
            }));
        }
        
        private void StopInvincibilitySequence()
        {
            if (_playerDamageSequence != null)
            {
                GameManager.instance.StopCoroutine(_playerDamageSequence);
            }
            playerRigidBody.simulated = true;
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
    }
}

