using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class GridGoal : MonoBehaviour
{
    private bool isCompleted;

    private void Awake()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    public void Complete()
    {
        if (isCompleted)
            return;

        isCompleted = true;

        Debug.Log("Goal reached! Level completed!");

        GameEvents.RaiseLevelCompleted();
    }
}
