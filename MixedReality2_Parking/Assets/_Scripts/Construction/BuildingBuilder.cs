using System.Collections.Generic;
using UnityEngine;

// https://bitbucket.org/sloankelly/youtube-source-repository/src/02676ed7f8dc8b2ae124bc922a94062d28de3aaa/unity/Real%20World%20Map%20Data/Assets/Scripts/BuildingMaker.cs?at=master&fileviewer=file-view-default
public class BuildingBuilder {

    public static void CreateBuilding(OSMWay way, OSMapInfo mapInfo, Material material)
    {
        // Create an instance of the object and place it in the centre of its points
        GameObject go = new GameObject();
        Vector3 localOrigin = GetCenter(way, mapInfo);
        go.transform.position = localOrigin - mapInfo.Bounds.Center;

        // Add the mesh filter and renderer components to the object
        MeshFilter mf = go.AddComponent<MeshFilter>();
        MeshRenderer mr = go.AddComponent<MeshRenderer>();

        // Apply the material
        mr.material = material;

        // Create the collections for the object's vertices, indices, UVs etc.
        List<Vector3> vectors = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> indices = new List<int>();

        // Call the child class' object creation code
        OnObjectCreated(mapInfo, way, localOrigin, vectors, normals, uvs, indices);

        // Apply the data to the mesh
        mf.mesh.vertices = vectors.ToArray();
        mf.mesh.normals = normals.ToArray();
        mf.mesh.triangles = indices.ToArray();
        mf.mesh.uv = uvs.ToArray();
    }

    /// <summary>
    /// Build the object using the data from the OsmWay instance.
    /// </summary>
    /// <param name="way">OsmWay instance</param>
    /// <param name="origin">The origin of the structure</param>
    /// <param name="vectors">The vectors (vertices) list</param>
    /// <param name="normals">The normals list</param>
    /// <param name="uvs">The UVs list</param>
    /// <param name="indices">The indices list</param>
    private static void OnObjectCreated(OSMapInfo mapInfo, OSMWay way, Vector3 origin, List<Vector3> vectors, List<Vector3> normals, List<Vector2> uvs, List<int> indices)
    {
        // Get the centre of the roof
        Vector3 oTop = new Vector3(0, way.Height, 0);

        // First vector is the middle point in the roof
        vectors.Add(oTop);
        normals.Add(Vector3.up);
        uvs.Add(new Vector2(0.5f, 0.5f));

        for (int i = 1; i < way.NodeIDs.Count; i++)
        {
            OSMNode p1 = mapInfo.Nodes[way.NodeIDs[i - 1]];
            OSMNode p2 = mapInfo.Nodes[way.NodeIDs[i]];

            Vector3 v1 = p1 - origin;
            Vector3 v2 = p2 - origin;
            Vector3 v3 = v1 + new Vector3(0, way.Height, 0);
            Vector3 v4 = v2 + new Vector3(0, way.Height, 0);

            vectors.Add(v1);
            vectors.Add(v2);
            vectors.Add(v3);
            vectors.Add(v4);

            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(1, 1));

            normals.Add(-Vector3.forward);
            normals.Add(-Vector3.forward);
            normals.Add(-Vector3.forward);
            normals.Add(-Vector3.forward);

            int idx1, idx2, idx3, idx4;
            idx4 = vectors.Count - 1;
            idx3 = vectors.Count - 2;
            idx2 = vectors.Count - 3;
            idx1 = vectors.Count - 4;

            // first triangle v1, v3, v2
            indices.Add(idx1);
            indices.Add(idx3);
            indices.Add(idx2);

            // second         v3, v4, v2
            indices.Add(idx3);
            indices.Add(idx4);
            indices.Add(idx2);

            // third          v2, v3, v1
            indices.Add(idx2);
            indices.Add(idx3);
            indices.Add(idx1);

            // fourth         v2, v4, v3
            indices.Add(idx2);
            indices.Add(idx4);
            indices.Add(idx3);

            // And now the roof triangles
            indices.Add(0);
            indices.Add(idx3);
            indices.Add(idx4);

            // Don't forget the upside down one!
            indices.Add(idx4);
            indices.Add(idx3);
            indices.Add(0);
        }
    }

    private static Vector3 GetCenter(OSMWay way, OSMapInfo mapInfo)
    {
        Vector3 total = Vector3.zero;

        foreach (ulong nodeID in way.NodeIDs)
        {
            total += mapInfo.Nodes[nodeID];
        }

        return total / way.NodeIDs.Count;
    }
}
