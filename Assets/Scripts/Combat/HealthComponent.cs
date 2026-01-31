using System;
using System.Linq;
using UnityEngine;
using JetBrains.Annotations;


namespace Combat
{
    /**
     * Tracks health and damage modifiers (e.g., Armor, Debuffs...).
     *
     * Health is stored as an unsigned integer - only positive values are allowed!
     * When health reaches 0, the OnDeath event is emitted.
     */
    public sealed class HealthComponent : MonoBehaviour
    {
        public event Action<int, DamageType> OnDamage;
        public event Action<int> OnHeal;
        public event Action OnDeath;
        public event Action OnRevive;

        [SerializeField] private int maxHealth = 100;
        public int GetMaxHealth => maxHealth;

        [SerializeField] private int currentHealth = 100;
        public int GetHealth => currentHealth;

        [SerializeField] public DamageModifier[] attachedDamageModifiers = { };

        
        public bool Alive => currentHealth > 0;


        /** Calculates damage modifiers, then applies the damage. */
        public void Damage(int amount, DamageType type = DamageType.Any)
        {
            if (amount <= 0 || !Alive) return;

            float factor = 1.0f;
            foreach (DamageModifier modifier in attachedDamageModifiers)
                if (modifier.damageType == type || modifier.damageType == DamageType.Any)
                    factor *= modifier.factor;

            int modifiedDamage = Mathf.RoundToInt(amount * factor);
            int appliedDamage = Math.Min(currentHealth, modifiedDamage);
            if (appliedDamage == 0) return;

            currentHealth -= appliedDamage;

            OnDamage?.Invoke(appliedDamage, type);
            if (currentHealth == 0) OnDeath?.Invoke();
        }

        /** Heals damage modifiers, then applies the damage. */
        public void Heal(int amount)
        {
            if (amount <= 0 || currentHealth == maxHealth) return;
            bool wasDead = !Alive;

            int appliedHealing = Math.Min(maxHealth - currentHealth, amount);
            if (appliedHealing == 0) return;

            currentHealth += appliedHealing;

            OnHeal?.Invoke(appliedHealing);
            if (wasDead && currentHealth > 0) OnRevive?.Invoke();
        }

        /**
         * Returns the health component of all game objects that
         * have any of the given target tags within a given radius.
         *
         * Matches all game objects if targetTags is null or empty.
         */
        public static HealthComponent[] GetNearbyHealthComponents(
            GameObject originGameObject,
            float radius,
            string[] targetTags
        )
        {
            Collider[] targets = Physics.OverlapSphere(originGameObject.transform.position, radius);
            return targets
                .Select(target => target.gameObject)
                .Where(targetObject => targetObject.TryGetComponent(out HealthComponent _))
                .Where(targetObject => !targetObject.Equals(originGameObject))
                .Where(targetObject => targetTags.Length == 0 || targetTags.Any(targetObject.CompareTag))
                .Select(targetObject => targetObject.GetComponent<HealthComponent>())
                .ToArray();
        }

        /**
         * Returns the health component of the closest game object that
         * has any of the given target tags within a given radius.
         *
         * Matches all game objects if targetTags is null or empty.
         */
        [CanBeNull]
        public static HealthComponent GetClosestHealthComponent(
            GameObject originGameObject,
            float radius,
            string[] targetTags
        ) => GetNearbyHealthComponents(originGameObject, radius, targetTags)
            .OrderBy(
                targetHealth => Vector3.Distance(
                    originGameObject.transform.position,
                    targetHealth.gameObject.transform.position
                )
            )
            .FirstOrDefault();
    }
}