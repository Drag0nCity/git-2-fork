using UnityEngine;

public class BoardRotationTest : MonoBehaviour
{
    [SerializeField] private BoardRotator boardRotator;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            boardRotator.RotateLeft();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            boardRotator.RotateRight();
        }
    }
}