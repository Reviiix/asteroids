﻿using System;
using Assets.Scripts;
using Assets.Scripts.Player;
using PureFunctions.UnitySpecific;
using UnityEngine;

namespace Player
{
    [Serializable]
    public class PlayerManager 
    {
        public Transform playerTransform;
        public SpriteRenderer PlayerRenderer { get; private set; }
        public const float SecondsBeforePlayerCanMoveAfterReSpawn = 0.3f;
        private const float InvincibilityFlashes = 25;
        private static Coroutine _playerDamageSequence;

        public void PlayerInitialise()
        {
            PlayerRenderer = playerTransform.GetComponent<SpriteRenderer>();
            
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
            Health.canBeDamaged = false;
            _playerDamageSequence = Coroutiner.StartCoroutine(FlashSprite.Flash(PlayerRenderer, InvincibilityFlashes, new WaitForSeconds(0.1f), StopInvincibilitySequence)).Coroutine;
        }
        
        public void StopInvincibilitySequence()
        {
            if (_playerDamageSequence != null)
            {
                Coroutiner.StopCoroutine(_playerDamageSequence);
            }
            Health.canBeDamaged = true;
            PlayerRenderer.enabled = true;
        }
        
        public  void RestoreHealth()
        {
            var userInterface = GameManager.Instance.userInterfaceManager;
            const int maxHealth = Health.MaxHealth;
            
            Health.RestoreHealth();

            for (var i = 0; i <= maxHealth; i++)
            {
                userInterface.UpdateLivesDisplay(true);
            }
        }
        
        public static void EnablePlayerConstraints(bool state = true)
        {
            PlayerMovement.RestrictPlayerMovement(state);
            PlayerShooting.canShoot = !state;
        }
    }
}
