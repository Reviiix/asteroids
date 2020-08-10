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
    private const int DestructionPrefabPoolIndex = 3;
    private const int SecondsBeforeContinuingGamePlay = 1;
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

    private void InitialisePlaneOldCSharpClasses()
    {
        userInterfaceManager.Initialise();
        playerManager.PlayerInitialise();
        AudioManager.CreateHashSetOfAudioSourcesFromPools();
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

        DisplayDebugMessage("Game play started.");
    }

    [ContextMenu("Player Damaged")]
    public void PlayerDamaged(int damage)
    {
        EnablePlayerConstraints(true);
        userInterfaceManager.UpdateLivesDisplay(false);
        Health.TakeDamage(damage, delegate(bool gameOver)
        {
            playerManager.playerRenderer.enabled = false;
            PlayerDeathSequence(() =>
            {
                GameAreaTransporter.PlaceObjectInCentre(ReturnPlayer());
                playerManager.playerRenderer.enabled = true;
                if (gameOver)
                {
                    EndGame();
                }
                else
                {
                    StartCoroutine(Wait(SecondsBeforeContinuingGamePlay, ()=>
                    {
                        EnablePlayerConstraints(false);
                    }));
                }
            });
            DisplayDebugMessage("PlayerManager received " + damage + "damage.");
        });
    }

    private static void PlayerDeathSequence(Action callBack)
    {
        ObjectPooling.ReturnObjectFromPool(DestructionPrefabPoolIndex, ReturnPlayer().position, Quaternion.identity);
        instance.StartCoroutine(Wait(SecondsBeforeContinuingGamePlay, callBack));
    }

    public void OnObstacleDestruction(int asteroidSize, Transform position)
    {
        if (asteroidSize < 0)
        {
            ObjectPooling.ReturnObjectFromPool(DestructionPrefabPoolIndex, position.position, Quaternion.identity);
            return;
        }
        
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
        DisplayDebugMessage("Game play over.");
    }
    
    public static void EnablePlayerConstraints(bool state)
    {
        PlayerMovement.EnablePlayerMovementConstraints(state);
        PlayerShooting.canShoot = !state;
        instance.StartCoroutine(Wait(1, () =>
        {
            instance.playerManager.playerRigidBody.simulated = !state;
        }));
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

