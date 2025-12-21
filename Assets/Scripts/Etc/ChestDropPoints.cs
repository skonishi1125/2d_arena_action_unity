using UnityEngine;

public class ChestDropPoints : MonoBehaviour
{
    [SerializeField] private Transform[] points;

    public Transform GetRandomPoint()
    {
        if (points == null || points.Length == 0)
            return null;

        return points[Random.Range(0, points.Length)];
    }
}
