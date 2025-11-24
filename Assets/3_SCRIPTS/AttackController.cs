using UnityEngine;

public class AttackController : MonoBehaviour
{
    [Tooltip("Dégâts infligés par attaque.")]
    public int AttackDamage = 10;
    [Tooltip("Intervalle entre les attaques (en secondes).")]
    public float AttackInterval = 1.0f;
    [Tooltip("Portée maximale de l'attaque.")]
    public float AttackRange = 5.0f;

    private float _nextAttackTime;
    private HealthComponent _targetHealth;

    public bool TryAttack(GameObject target)
    {
        if (Time.time < _nextAttackTime)
        {
            return false;
        }

        if (target == null)
        {
            Debug.LogWarning("AttackController: Cible d'attaque nulle.");
            return false;
        }

        if (Vector3.Distance(transform.position, target.transform.position) > AttackRange)
        {
            Debug.LogWarning($"AttackController: Cible {target.name} hors de portée d'attaque.");
            return false; 
        }

        _targetHealth = target.GetComponent<HealthComponent>();
        if (_targetHealth != null)
        {
            _targetHealth.TakeDamage(AttackDamage);
            _nextAttackTime = Time.time + AttackInterval;
            Debug.Log($"{gameObject.name} attaque {target.name} pour {AttackDamage} dégâts.");
            return true;
        }
        else
        {
            Debug.LogWarning($"AttackController: Cible {target.name} n'a pas de HealthComponent.");
            return false;
        }
    }

    public bool IsReadyToAttack()
    {
        return Time.time >= _nextAttackTime;
    }

    public void SetAttackTarget(GameObject target)
    {
        _targetHealth = target != null ? target.GetComponent<HealthComponent>() : null;
    }

    public GameObject GetAttackTarget()
    {
        return _targetHealth != null ? _targetHealth.gameObject : null;
    }
}