using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block {

    public Mesh mesh;
    Chunk parentChunk;

    public Block(Vector3 offset, MeshUtils.BlockType type, Chunk chunk)
    {
        parentChunk = chunk;
        Vector3 blockLocalPos = offset - chunk.location;
        if (type != MeshUtils.BlockType.AIR)
        {
            List<Quad> quads = new List<Quad>();
            if (!HasSolidNeighbor((int)offset.x, (int)offset.y - 1, (int)offset.z))
            {
                if (type == MeshUtils.BlockType.GRASSSIDE)
                {
                    quads.Add(new Quad(MeshUtils.BlockSide.BOTTOM, offset, MeshUtils.BlockType.DIRT));
                }
                else
                {
                    quads.Add(new Quad(MeshUtils.BlockSide.BOTTOM, offset, type));
                }
            }

            if (!HasSolidNeighbor((int)offset.x, (int)offset.y + 1, (int)offset.z))
            {
                if (type == MeshUtils.BlockType.GRASSSIDE)
                {
                    quads.Add(new Quad(MeshUtils.BlockSide.TOP, offset, MeshUtils.BlockType.GRASSTOP));
                }
                else
                {
                    quads.Add(new Quad(MeshUtils.BlockSide.TOP, offset, type));
                }
            }

            if (!HasSolidNeighbor((int)offset.x - 1, (int)offset.y, (int)offset.z))
            {
                quads.Add(new Quad(MeshUtils.BlockSide.LEFT, offset, type));
            }

            if (!HasSolidNeighbor((int)offset.x + 1, (int)offset.y, (int)offset.z))
            {
                quads.Add(new Quad(MeshUtils.BlockSide.RIGHT, offset, type));
            }

            if (!HasSolidNeighbor((int)offset.x, (int)offset.y, (int)offset.z + 1))
            {
                quads.Add(new Quad(MeshUtils.BlockSide.FRONT, offset, type));
            }

            if (!HasSolidNeighbor((int)offset.x, (int)offset.y, (int)offset.z - 1))
            {
                quads.Add(new Quad(MeshUtils.BlockSide.BACK, offset, type));
            }

            if (quads.Count == 0) return;

            Mesh[] sideMeshes = new Mesh[quads.Count];
            int m = 0;
            foreach (Quad q in quads)
            {
                sideMeshes[m] = q.mesh;
                m++;
            }

            mesh = MeshUtils.mergedMeshes(sideMeshes);
            mesh.name = "Cube_0_0_0";
        }
    }

    public bool HasSolidNeighbor(int x, int y, int z)
    {
        if (x < 0 || x >= parentChunk.width ||
            y < 0 || y >= parentChunk.height ||
            z < 0 || z >= parentChunk.depth)
        {
            return false;
        }
        if(parentChunk.chunkData[x + parentChunk.width * (y + parentChunk.depth * z)] == MeshUtils.BlockType.AIR
            || parentChunk.chunkData[x + parentChunk.width * (y + parentChunk.depth * z)] == MeshUtils.BlockType.WATER)
        return false;
        return true;
    }
}
