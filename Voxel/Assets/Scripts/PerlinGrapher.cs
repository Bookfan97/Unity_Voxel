using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PerlinGrapher : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float heightScale =2f, scale = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = this.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 100;
        Graph();
    }

    private void Graph()
    {
        int z = 11;
        Vector3[] positions = new Vector3[lineRenderer.positionCount];
        for (int x = 0; x < lineRenderer.positionCount; x++)
        {
            float y = Mathf.PerlinNoise(x * scale, z* scale) * heightScale;
            positions[x] = new Vector3(x, y, z);
        }
        lineRenderer.SetPositions(positions);
    }

    private void OnValidate()
    {
        Graph();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
