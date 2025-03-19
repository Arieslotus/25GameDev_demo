using UnityEngine;

public class LineRendererCollider : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private EdgeCollider2D edgeCollider;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        edgeCollider = GetComponent<EdgeCollider2D>();

        UpdateCollider();
    }

    private void Update()
    {
        UpdateCollider();
    }
    void UpdateCollider()
    {
        Vector2[] points = new Vector2[lineRenderer.positionCount];
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            points[i] = lineRenderer.GetPosition(i);
        }
        edgeCollider.points = points;
    }
}