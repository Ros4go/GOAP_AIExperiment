using UnityEngine;

public class InteractiveObject : MonoBehaviour
{
    public string ObjectName = "InteractiveItem";
    public bool IsPickedUp { get; private set; } = false;
    public GameObject IsPickedUpBy { get; private set; } = null; 

    public virtual bool PickUp(GameObject picker)
    {
        if (!IsPickedUp)
        {
            IsPickedUp = true;
            IsPickedUpBy = picker;
            Debug.Log($"{ObjectName} a été ramassé par {picker.name}.");
            GameEvents.TriggerOnWeaponPickedUp(gameObject, picker);
            gameObject.SetActive(false); 
            return true;
        }
        return false;
    }

    public virtual void Use()
    {
        Debug.Log($"{ObjectName} a été utilisé.");
        IsPickedUp = false;
        IsPickedUpBy = null;
    }
}