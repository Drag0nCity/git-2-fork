using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class GridTeleport : MonoBehaviour
{
    [SerializeField] private GridTeleport targetTeleport;

    private static bool isTeleporting;

    private void Awake()
    {
        GetComponent<BoxCollider>().isTrigger = true;

        if (targetTeleport == null)
        {
            Debug.LogError(
                "GridTeleport: Target Teleport is not assigned!",
                this
            );
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isTeleporting)
            return;

        GridPlayerController player =
            other.GetComponentInParent<GridPlayerController>();

        if (player == null || targetTeleport == null)
            return;

        StartCoroutine(TeleportPlayer(player));
    }

    private IEnumerator TeleportPlayer(GridPlayerController player)
    {
        isTeleporting = true;

        player.TeleportTo(targetTeleport.transform.position);
             
        yield return new WaitForFixedUpdate();

        yield return new WaitForSeconds(0.3f);

        isTeleporting = false;
    }
}
