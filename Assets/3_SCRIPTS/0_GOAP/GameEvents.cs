using System;
using UnityEngine;

public static class GameEvents
{
    public static event Action<GameObject> OnEnemyDied;
    public static void TriggerOnEnemyDied(GameObject deadEnemy)
    {
        OnEnemyDied?.Invoke(deadEnemy);
    }

    public static event Action<GameObject, GameObject> OnWeaponPickedUp;
    public static void TriggerOnWeaponPickedUp(GameObject pickedUpWeapon, GameObject picker)
    {
        OnWeaponPickedUp?.Invoke(pickedUpWeapon, picker);
    }

    public static event Action<GameObject, GameObject> OnHealingKitUsed;
    public static void TriggerOnHealingKitUsed(GameObject healingKit, GameObject user)
    {
        OnHealingKitUsed?.Invoke(healingKit, user);
    }
}