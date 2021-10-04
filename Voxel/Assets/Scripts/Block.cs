using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    [System.Serializable]
    public enum BlockSide
    {
        BOTTOM, TOP, LEFT, RIGHT, FRONT, BACK
    }
    
    public BlockSide Side = BlockSide.FRONT;
    public Vector3 offset = new Vector3(0, 0, 0);
    // Start is called before the first frame update
    void Start()
    {
        MeshFilter meshFilter = this.gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = this.gameObject.AddComponent<MeshRenderer>();

        Quad quad = new Quad();
        meshFilter.mesh = quad.Build(Side, offset);
    }

    // Update is called once per frame
    void Update()
    {
         
    }
}
