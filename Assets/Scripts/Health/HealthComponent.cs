using System;
using UnityEngine;


public enum DamageType { Any, Slash, Blunt, Tear, Pierce }
    
[Serializable]
public class DamageModifier
{
    [SerializeField]
    public DamageType damageType = DamageType.Any;

    [Range(0.1f, 5f)]
    public float factor = 1.0f;
}


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
    
    public bool Alive => currentHealth > 0;
    
    [SerializeField]
    private int maxHealth = 100;
    public int MaxHealth => maxHealth;
    
    [SerializeField]
    private int currentHealth = 100;
    public int Health => currentHealth;
    
    [SerializeField]
    public DamageModifier[] attachedDamageModifiers = {};
    
    
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
}
