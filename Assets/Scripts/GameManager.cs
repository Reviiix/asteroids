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
        EnablePlayerConstraints(true);
        userInterfaceManager.UpdateLivesDisplay(false);
        Health.TakeDamage(damage, delegate(bool gameOver)
        {
            PlayerDeathSequence(() =>
            {
                GameAreaTransporter.PlaceObjectInCentre(ReturnPlayer());
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
        public Health playerHealth;
        public PlayerMovement playerMovement;
        public PlayerShooting playerShooting;

        public void PlayerInitialise()
        {
            playerRigidBody = playerTransform.GetComponent<Rigidbody2D>();
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

