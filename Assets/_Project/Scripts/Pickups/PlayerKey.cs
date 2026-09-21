using UnityEngine;

public sealed class PlayerKey : MonoBehaviour
{
    public bool HasKey { get; private set; }

    public void TakeKey()
    {
        HasKey = true;

        Debug.Log("Key picked up");
    }
}