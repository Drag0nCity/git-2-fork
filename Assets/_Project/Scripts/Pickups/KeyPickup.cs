using UnityEngine;

public sealed class KeyPickup : MonoBehaviour, IPickup
{
    public void Pickup(GameObject picker)
    {
        Debug.Log("KEY: Pickup вызван");

        PlayerKey playerKey =
            picker.GetComponentInParent<PlayerKey>();

        if (playerKey == null)
        {
            Debug.LogError("KEY: PlayerKey НЕ найден");
            return;
        }

        playerKey.TakeKey();

        Debug.Log("KEY: Ключ получен");

        Destroy(gameObject);
    }
}