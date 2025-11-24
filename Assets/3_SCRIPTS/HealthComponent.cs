using UnityEngine;
using System;

public class HealthComponent : MonoBehaviour
{
    [Tooltip("Points de vie maximum.")]
    public int MaxHealth = 100;
    
    [Tooltip("Points de vie actuels.")]
    [SerializeField] 
    private int _currentHealth = 100;

    public int CurrentHealth => _currentHealth;
    public bool IsDead => _currentHealth <= 0;

    public static event Action<GameObject, int, int> OnHealthChanged;
    public static event Action<GameObject> OnDied; 

    public void TakeDamage(int amount)
    {
        if (IsDead) return;

        _currentHealth -= amount;
        _currentHealth = Mathf.Max(0, _currentHealth);

        OnHealthChanged?.Invoke(gameObject, _currentHealth, MaxHealth);
        Debug.Log($"{gameObject.name} a subi {amount} dégâts. HP restants: {_currentHealth}");

        if (IsDead)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (IsDead) return;

        _currentHealth += amount;
        _currentHealth = Mathf.Min(MaxHealth, _currentHealth);

        OnHealthChanged?.Invoke(gameObject, _currentHealth, MaxHealth);
        Debug.Log($"{gameObject.name} a été soigné de {amount} HP. HP actuels: {_currentHealth}");
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} est mort !");
        OnDied?.Invoke(gameObject);
        GameEvents.TriggerOnEnemyDied(gameObject); 
        Destroy(gameObject);
    }
}