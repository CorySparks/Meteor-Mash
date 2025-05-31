using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class screenCollider : MonoBehaviour
{
    EdgeCollider2D edgeCollder;
    void Awake()
    {
        edgeCollder = this.GetComponent<EdgeCollider2D>();
        CreateEdgeCollider();
    }
    //call this at start and whenever the resolution changes
    void CreateEdgeCollider()
    {
        List<Vector2> edges = new List<Vector2>();
        edges.Add(Camera.main.ScreenToWorldPoint(Vector2.zero));
        edges.Add(Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)));
        edgeCollder.SetPoints(edges);
    }
}
