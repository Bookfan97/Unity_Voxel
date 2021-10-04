using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VertexData = System.Tuple<UnityEngine.Vector3, UnityEngine.Vector3, UnityEngine.Vector2>;

public static class MeshUtils
{
    public static Mesh mergedMeshes(Mesh[] meshes)
    {
        Mesh mesh = new Mesh();
        
        Dictionary<VertexData, int> pointsOrder = new Dictionary<VertexData, int>();
        HashSet<VertexData> pointsHash = new HashSet<VertexData>();
        List<int> triangles = new List<int>();

        int pIndex = 0;

        //Loop through each mesh
        for (int i = 0; i < meshes.Length; i++)
        {
            if (meshes[i] == null)
            {
                continue;
            }

            //Loop through each vertex
                for (int j = 0; j < meshes[i].vertices.Length; j++)
                {
                    Vector3 vertex = meshes[i].vertices[j];
                    Vector3 normal = meshes[i].normals[j];
                    Vector2 uv = meshes[i].uv[j];
                    VertexData point = new VertexData(vertex, normal, uv);

                    if (!pointsHash.Contains(point))
                    {
                        pointsOrder.Add(point, pIndex);
                        pointsHash.Add(point);
                        pIndex++;
                    }
                }

                for (int t = 0; t < meshes[i].triangles.Length; t++)
                {
                    int trainglePoint = meshes[i].triangles[t];
                    Vector3 vertex = meshes[i].vertices[trainglePoint];
                    Vector3 normal = meshes[i].normals[trainglePoint];
                    Vector2 uv = meshes[i].uv[trainglePoint];
                    VertexData point = new VertexData(vertex, normal, uv);

                    int index;
                    pointsOrder.TryGetValue(point, out index);
                    triangles.Add(index);
                }

                meshes[i] = null;
            }
        ExtractArrays(pointsOrder, mesh);
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateBounds();
        return mesh;
    }

    public static void ExtractArrays(Dictionary<VertexData, int> list, Mesh mesh)
    {
        List<Vector3> vertexes = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();

        foreach (VertexData vKey in list.Keys)
        {
            vertexes.Add(vKey.Item1);
            normals.Add(vKey.Item2);
            uvs.Add(vKey.Item3);
        }

        mesh.vertices = vertexes.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uvs.ToArray();
    }
}
