using UnityEngine;
using Zenject;

public sealed class GameEndTrigger : MonoBehaviour
{
    private GameEndController gameEndController;

    [Inject]
    private void Construct(GameEndController gameEndController)
    {
        this.gameEndController = gameEndController;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(
            $"GAME END: вошёл объект {other.name}, Tag = {other.tag}");

        if (!other.CompareTag("Player"))
        {
            Debug.Log("GAME END: это не Player");
            return;
        }

        if (gameEndController == null)
        {
            Debug.LogError("GAME END: GameEndController не найден");
            return;
        }

        Debug.Log("GAME END: PLAYER FOUND!");

        gameEndController.FinishGame();
    }
}