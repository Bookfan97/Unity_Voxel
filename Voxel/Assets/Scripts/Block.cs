using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public Material atlas;
    
    [System.Serializable]
    public enum BlockSide
    {
        BOTTOM, TOP, LEFT, RIGHT, FRONT, BACK
    }

    public Vector3 offset = new Vector3(0, 0, 0);
    // Start is called before the first frame update
    void Start()
    {
        MeshFilter meshFilter = this.gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = atlas;
        Quad[] quads = new Quad[6];
        quads[0] = new Quad(BlockSide.BOTTOM, offset);
        quads[1] = new Quad(BlockSide.TOP, offset);
        quads[2] = new Quad(BlockSide.LEFT, offset);
        quads[3] = new Quad(BlockSide.RIGHT, offset);
        quads[4] = new Quad(BlockSide.FRONT, offset);
        quads[5] = new Quad(BlockSide.BACK, offset);
        Mesh[] sidesMeshes = new Mesh[6];
        for (int i = 0; i < quads.Length; i++)
        {
            sidesMeshes[i] = quads[i].mesh;
        }

        meshFilter.mesh = MeshUtils.mergedMeshes(sidesMeshes);
        meshFilter.mesh.name = $"Cube_{offset.x}_{offset.y}_{offset.z}";
    }

    // Update is called once per frame
    void Update()
    {
         
    }
}
