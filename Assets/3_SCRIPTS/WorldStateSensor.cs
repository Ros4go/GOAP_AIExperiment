using UnityEngine;
using System.Collections.Generic;

public class WorldStateSensor : MonoBehaviour
{
    [Header("Sensor Configuration")]
    [Tooltip("La fréquence à laquelle le capteur met à jour l'état du monde (en secondes).")]
    public float UpdateInterval = 0.2f;

    public HealthComponent MyHealth;
    public Transform EnemyTransform;
    public InteractiveObject WeaponObject;

    [Tooltip("Distance maximale pour être considéré 'près' d'un objet")]
    public float NearDistanceThreshold = 3.0f; 

    private WorldStateBit _currentWorldState;
    private float _nextUpdateSensorTime;
    
    private float smokedRecentlyUntil = 0f;
    [Tooltip("Durée pendant laquelle l'agent considère avoir fumé récemment.")]
    public float smokeCooldown = 10f;
    public float probabilityToWantToSmoke = 0.05f;
    private float relaxUntil = 0f;
    public float relaxCooldown = 10f;
    public float relaxProbability = 0.05f;
    private float nextRelaxableTime = 0f;

    protected virtual void Awake()
    {
        if (MyHealth == null) MyHealth = GetComponent<HealthComponent>();

        GameObject weaponGO = GameObject.FindGameObjectWithTag("Weapon");
        if (weaponGO != null) WeaponObject = weaponGO.GetComponent<InteractiveObject>();
    }

    protected virtual void Start()
    {
        _currentWorldState = new WorldStateBit();
        _nextUpdateSensorTime = Time.time + UpdateInterval;
        GetCurrentWorldState();
    }

    protected virtual void Update()
    {
        if (Time.time >= _nextUpdateSensorTime)
        {
            GetCurrentWorldState();
            _nextUpdateSensorTime = Time.time + UpdateInterval;
        }
    }

    public WorldStateBit GetCurrentWorldState()
    {
        WorldStateBit newState = new WorldStateBit(); 

        if (MyHealth != null)
        {
            newState.Set(WorldStateKey.IsHurt, MyHealth.CurrentHealth <= MyHealth.MaxHealth * 0.5f);
            newState.Set(WorldStateKey.IsDead, MyHealth.CurrentHealth <= 0);
        }
        else
        {
            newState.Set(WorldStateKey.IsHurt, false);
            newState.Set(WorldStateKey.IsDead, false);
        }

        bool weaponTakenByEnemy = WeaponObject != null && WeaponObject.IsPickedUp && WeaponObject.IsPickedUpBy != gameObject;
        newState.Set(WorldStateKey.ShouldFlee, weaponTakenByEnemy);

        bool isNearWeapon = false;
        bool hasWeapon = false; 
        if (WeaponObject != null)
        {
            isNearWeapon = Vector3.Distance(transform.position, WeaponObject.transform.position) <= NearDistanceThreshold;
            hasWeapon = WeaponObject.IsPickedUpBy == gameObject;
            newState.Set(WorldStateKey.IsNearWeapon, isNearWeapon);
            newState.Set(WorldStateKey.HasWeapon, hasWeapon);
        }
        else
        {
            newState.Set(WorldStateKey.IsNearWeapon, false);
            newState.Set(WorldStateKey.HasWeapon, false);
        }

        bool isEnemyDead = false;
        if (EnemyTransform != null)
        {
            HealthComponent enemyHealth = EnemyTransform.GetComponent<HealthComponent>();
            if (enemyHealth != null)
            {
                isEnemyDead = enemyHealth.CurrentHealth <= 0;
            }
            newState.Set(WorldStateKey.IsEnemyDead, isEnemyDead);
            newState.Set(WorldStateKey.IsNearEnemy, Vector3.Distance(transform.position, EnemyTransform.position) <= NearDistanceThreshold);
        }
        else
        {
            newState.Set(WorldStateKey.IsEnemyDead, true); 
            newState.Set(WorldStateKey.IsNearEnemy, false);
        }
        
        newState.Set(WorldStateKey.SmokedRecently, Time.time < smokedRecentlyUntil);

        bool needsToRelax = Time.time < relaxUntil;

        if (!needsToRelax && Time.time >= nextRelaxableTime && Random.value < relaxProbability * Time.deltaTime * 5f)
        {
            relaxUntil = Time.time + relaxCooldown;
            nextRelaxableTime = Time.time + relaxCooldown + 2f; 
            needsToRelax = true;
            Debug.Log($"{gameObject.name} ressent le besoin de fumer.");
        }

        newState.Set(WorldStateKey.NeedsToRelax, needsToRelax);

        _currentWorldState = newState; 
        return _currentWorldState;
    }

    public void SetSmokedRecently(float duration)
    {
        smokedRecentlyUntil = Time.time + duration;
        relaxUntil = 0f;
        nextRelaxableTime = Time.time + duration + 2f;
        _currentWorldState = GetCurrentWorldState(); 
    }

    private void OnEnable()
    {
        GameEvents.OnEnemyDied += HandleEnemyDied;
        GameEvents.OnWeaponPickedUp += HandleWeaponPickedUp;
    }

    private void OnDisable()
    {
        GameEvents.OnEnemyDied -= HandleEnemyDied;
        GameEvents.OnWeaponPickedUp -= HandleWeaponPickedUp;
    }

    private void HandleEnemyDied(GameObject deadEnemy)
    {
        if (EnemyTransform != null && deadEnemy == EnemyTransform.gameObject)
        {
            _currentWorldState.Set(WorldStateKey.IsEnemyDead, true);
            Debug.Log($"GOAP Sensor: Enemy died! Updating world state for {gameObject.name}.");
        }
    }

    private void HandleWeaponPickedUp(GameObject pickedUpWeapon, GameObject picker)
    {
        if (WeaponObject != null && pickedUpWeapon == WeaponObject.gameObject)
        {
            _currentWorldState.Set(WorldStateKey.HasWeapon, picker == gameObject);
            _currentWorldState.Set(WorldStateKey.IsNearWeapon, false); 
            Debug.Log($"GOAP Sensor: Weapon picked up! Updating world state for {gameObject.name}.");
        }
    }
    
}