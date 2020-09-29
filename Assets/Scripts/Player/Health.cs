using System;
using UnityEngine;

namespace Assets.Scripts.Player
{
    //Override this class and remove its static token if there are ever multiple things in the game with health.
    public static class Health
    {
        public static bool canBeDamaged = true;
        public const int MaxHealth = 2;
        [Range(0,MaxHealth)]
        private static int _health = MaxHealth;

        public static void TakeDamage(int damage, Action<bool> dead)
        {
            _health -= damage;
            if (_health < 0)
            {
                dead(true);
                return;
            }
            dead(false);
        }

        public static void RestoreHealth()
        {
            _health = MaxHealth;
        }
    }
}
