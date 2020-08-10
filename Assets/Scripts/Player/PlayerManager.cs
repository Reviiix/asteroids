using System;
using UnityEngine;

namespace Player
{
    [Serializable]
    public class PlayerManager 
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
