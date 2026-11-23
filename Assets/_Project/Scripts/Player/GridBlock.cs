using UnityEngine;

public enum GridBlockType
{
    Normal,
    Light,
    Heavy
}

[RequireComponent(typeof(BoxCollider))]
public class GridBlock : MonoBehaviour
{
    [Header("Block Settings")]
    [SerializeField] private GridBlockType blockType = GridBlockType.Normal;
    [SerializeField] private bool isPushable = true;

    public GridBlockType BlockType => blockType;
    public bool IsPushable => isPushable;

    private void Awake()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        boxCollider.isTrigger = false;
    }
}
