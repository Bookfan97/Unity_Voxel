using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    public Mesh mesh;

    public Block(Vector3 offset, MeshUtils.BlockType type)
    {
        Quad[] quads = new Quad[6];
        quads[0] = new Quad(MeshUtils.BlockSide.BOTTOM, offset, type);
        quads[1] = new Quad(MeshUtils.BlockSide.TOP, offset, type);
        quads[2] = new Quad(MeshUtils.BlockSide.LEFT, offset, type);
        quads[3] = new Quad(MeshUtils.BlockSide.RIGHT, offset, type);
        quads[4] = new Quad(MeshUtils.BlockSide.FRONT, offset, type);
        quads[5] = new Quad(MeshUtils.BlockSide.BACK, offset, type);
        Mesh[] sidesMeshes = new Mesh[6];
        for (int i = 0; i < quads.Length; i++)
        {
            sidesMeshes[i] = quads[i].mesh;
        }

        mesh = MeshUtils.mergedMeshes(sidesMeshes);
        mesh.name = $"Cube_{offset.x}_{offset.y}_{offset.z}";
    }
}
