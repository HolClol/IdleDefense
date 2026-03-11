using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeCollider : MonoBehaviour
{
    private EdgeCollider2D edgeCollder;
    private LayerMask projectileLayer;
    private void Awake()
    {
        edgeCollder = GetComponent<EdgeCollider2D>();
        projectileLayer = LayerMask.GetMask("PlayerProjectile");
        CreateEdgeCollider();
    }
    //call this at start and whenever the resolution changes
    public void CreateEdgeCollider()
    {
        List<Vector2> edges = new List<Vector2>
        {
            Camera.main.ScreenToWorldPoint(Vector2.zero),
            Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)),
            Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height)),
            Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height)),
            Camera.main.ScreenToWorldPoint(Vector2.zero)
        };
        edgeCollder.SetPoints(edges);
    }
}
