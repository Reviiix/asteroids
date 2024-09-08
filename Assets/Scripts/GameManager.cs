using System;
using System.Collections;
using Assets.Scripts.Obstacles;
using Assets.Scripts.PlayArea;
using Assets.Scripts.Player;
using Player;
using PureFunctions.UnitySpecific;
using Statistics;
using UnityEngine;

namespace Assets.Scripts
{
    public class GameManager : Singleton<GameManager>
    {
        public const Difficulty GameDifficulty = Difficulty.Hard;
        private static bool _gameOver;
        public AudioManager audioManager;
        public ObstacleManager obstacleManager;
        public ObjectPooling objectPools;
        public UserInterfaceManager userInterfaceManager;
        public Camera mainCamera;
        [SerializeField] private PlayerManager playerManager;

        protected override void OnEnable()
        {
            base.OnEnable();
            Initialise();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            StopAllCoroutines();
        }

        private void Initialise()
        {
            userInterfaceManager.Initialise();
            playerManager.PlayerInitialise();
            objectPools.Initialise();
            
            AudioManager.Initialise();
            BulletManager.Initialise();
            ObstacleManager.Initialise();
            TimeTracker.Initialise();
            ScoreTracker.Initialise();
        }

        private void Update()
        {
            BulletManager.MoveBullets();
            ObstacleManager.MoveObstacles();
        
            if (_gameOver) return;
            
            playerManager.PlayerUpdate();
        }

        [ContextMenu("Reload Game")]
        public void ReloadGame()
        {
            _gameOver = false;
            ScoreTracker.Initialise();
            TimeTracker.Initialise();
        
            BulletManager.DestroyAllBullets();
            ObstacleManager.DestroyAllObstacles();
        
            playerManager.RestoreHealth();
        
            userInterfaceManager.EnableStartCanvas();
        }

        [ContextMenu("Start Game Play")]
        public void StartGamePlay()
        {
            PlayerManager.EnablePlayerConstraints(false);
        
            TimeTracker.StartTimer();
        
            obstacleManager.StartCreatObstacleSequence();
        
            playerManager.PlayerRenderer.enabled = true;
            playerManager.StopInvincibilitySequence();
            PlayerShooting.canShoot = true;
        }
    
    
        public void OnPlayerCollision(int damage)
        {
            if (!Health.canBeDamaged)
            {
                return;
            }
            GameArea.CreateDestructionParticle(ReturnPlayer().position);
        
            audioManager.PlayDamageSound();
        
            userInterfaceManager.UpdateLivesDisplay(false);
        
            PlayerManager.EnablePlayerConstraints();

            GameAreaTransporter.PlaceObjectInCentre(ReturnPlayer());
        
            Health.TakeDamage(damage, delegate(bool gameOver)
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
            _gameOver = true;
            PlayerManager.EnablePlayerConstraints();
            Health.canBeDamaged = false;
            PlayerShooting.canShoot = false;
            playerManager.PlayerRenderer.enabled = false;
        
            obstacleManager.StopCreatObstacleSequence();

            TimeTracker.StopTimer();
            HighScores.SetHighScore(ScoreTracker.score);
        
            userInterfaceManager.EnableGameOverCanvas();
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
    }

    public enum Difficulty
    {
        //Obstacles have a 1 in X chance of not spawning every cycle.
        Easy  = 2,
        Medium  = 10,
        Hard = 100
    }
}