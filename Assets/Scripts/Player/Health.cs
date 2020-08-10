using System;
using UnityEngine;

namespace Player
{
    [Serializable]
    public class Health
    {
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

        public void RestoreHealth()
        {
            _health = MaxHealth;
        }

    }
}
