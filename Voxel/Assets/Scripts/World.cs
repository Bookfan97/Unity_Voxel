using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct PerlinSettings
{
    public float heightScale;
    public float scale;
    public int octaves;
    public float heightOffset;
    public float probability;

    public PerlinSettings(float hs, float s, int o, float ho, float p)
    {
        heightScale = hs;
        scale = s;
        octaves = o;
        heightOffset = ho;
        probability = p;
    }
}


public class World : MonoBehaviour
{
    public static Vector3Int worldDimensions = new Vector3Int(20, 5, 20);
    public static Vector3Int extraWorldDimensions = new Vector3Int(10, 5, 10);
    public static Vector3Int chunkDimensions = new Vector3Int(10, 10, 10);
    public GameObject chunkPrefab;
    public GameObject mCamera;
    public GameObject fpc;
    public Slider loadingBar;

    public static PerlinSettings surfaceSettings;
    public PerlinGrapher surface;
    public static PerlinSettings stoneSettings;
    public PerlinGrapher stone;
    public static PerlinSettings DiamondTopSettings;
    public PerlinGrapher DiamondTop; 
    public static PerlinSettings DiamondBottomSettings;
    public PerlinGrapher DiamondBottom;
    public static PerlinSettings CaveSettings;
    public Perlin3DGrapher cave;

    private HashSet<Vector3Int> chunkChecker = new HashSet<Vector3Int>();
    private HashSet<Vector2Int> chunkColumn = new HashSet<Vector2Int>();
    private Dictionary<Vector3Int, Chunk> chunks = new Dictionary<Vector3Int, Chunk>();
    private Vector3Int lastBuildPosition;
    private Queue<IEnumerator> buildQueue = new Queue<IEnumerator>();
    private int drawRadius = 3;

    IEnumerator BuildCoordinator()
    {
        while (true)
        {
            while (buildQueue.Count > 0)
            {
                yield return StartCoroutine(buildQueue.Dequeue());
            }

            yield return null;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        loadingBar.maxValue = worldDimensions.x * worldDimensions.z;
        surfaceSettings = new PerlinSettings(surface.heightScale, surface.scale, surface.octaves, 
            surface.heightOffset, surface.probability);
        stoneSettings = new PerlinSettings(stone.heightScale, stone.scale, stone.octaves, 
            stone.heightOffset, stone.probability);
        DiamondTopSettings = new PerlinSettings(DiamondTop.heightScale, DiamondTop.scale, DiamondTop.octaves, 
            DiamondTop.heightOffset, DiamondTop.probability);
        DiamondBottomSettings = new PerlinSettings(DiamondBottom.heightScale, DiamondBottom.scale, DiamondBottom.octaves, 
            DiamondBottom.heightOffset, DiamondBottom.probability);
        CaveSettings = new PerlinSettings(cave.heightScale, cave.scale, cave.octaves, cave.heightOffset, 
            cave.DrawCutOff);
        StartCoroutine(BuildWorld());
    }

    void BuildChunkColumn(int x, int z, bool meshEnabled = true)
    {
        for (int y = 0; y < worldDimensions.y; y++)
        {
            Vector3Int position = new Vector3Int(x, y * chunkDimensions.y, z);
            if (!chunkChecker.Contains(position))
            {
                GameObject chunk = Instantiate(chunkPrefab);
                chunk.name = $"Chunk_{position.x}_{position.y}_{position.z}";
                Chunk c = chunk.GetComponent<Chunk>();
                c.CreateChunk(chunkDimensions, position);
                chunkChecker.Add(position);
                chunks.Add(position, c);
            }
            chunks[position].meshRenderer.enabled = meshEnabled;
        }

        chunkColumn.Add(new Vector2Int(x, z));
    }
    
    IEnumerator BuildExtraWorld()
    {
        int zEnd = worldDimensions.z + extraWorldDimensions.z;
        int zStart = worldDimensions.z - 1;
        int xEnd = worldDimensions.x + extraWorldDimensions.x;
        int xStart = worldDimensions.x - 1;
        
        for (int z = zStart; z < zEnd; z++)
        {
            for (int x = 0; x < xEnd; x++)
            {
                BuildChunkColumn(x * chunkDimensions.x, z * chunkDimensions.z, false);
                yield return null;
            }
        }
        
        for (int z = 0; z < zEnd; z++)
        {
            for (int x = xStart; x < xEnd; x++)
            {
                BuildChunkColumn(x * chunkDimensions.x, z * chunkDimensions.z, false);
                yield return null;
            }
        }
    }
    
    IEnumerator BuildWorld()
    {
        for (int z = 0; z < worldDimensions.z; z++)
        {
            for (int x = 0; x < worldDimensions.x; x++) 
            { 
                BuildChunkColumn(x * chunkDimensions.x,z*chunkDimensions.z);
                loadingBar.value++;
                yield return null;
            }
        }

        mCamera.SetActive(false);
        

        int xpos = (int)((worldDimensions.x * chunkDimensions.x) / 2.0f);
        int zpos = (int)((worldDimensions.z * chunkDimensions.z) / 2.0f);
        int ypos = (int)(MeshUtils.fBM(xpos, zpos, surfaceSettings.octaves, surfaceSettings.scale, surfaceSettings.heightScale,
            stoneSettings.heightOffset) + 20);
        fpc.transform.position = new Vector3Int(xpos, ypos, zpos);
        loadingBar.gameObject.SetActive(false);
        fpc.SetActive(true);
        lastBuildPosition = Vector3Int.CeilToInt(fpc.transform.position);
        StartCoroutine(BuildCoordinator());
        StartCoroutine(UpdateWorld());
        StartCoroutine(BuildExtraWorld());
    }

    private WaitForSeconds wfs = new WaitForSeconds(0.5f);
    IEnumerator UpdateWorld()
    {
        while (true)
        {
            if ((lastBuildPosition - fpc.transform.position).magnitude > chunkDimensions.x)
            {
                lastBuildPosition = Vector3Int.CeilToInt(fpc.transform.position);
                int posX = (int)(fpc.transform.position.x / chunkDimensions.x) * chunkDimensions.x;
                int posZ = (int)(fpc.transform.position.z / chunkDimensions.z) * chunkDimensions.z;
                buildQueue.Enqueue(BuildRecursiveWorld(posX, posZ, drawRadius));
                buildQueue.Enqueue(HideColumns(posX, posZ));
            }

            yield return wfs;
        }
    }

    public void HideChunkColumn(int x, int z)
    {
        for (int y = 0; y < worldDimensions.y; y++)
        {
            Vector3Int pos = new Vector3Int(x, y * chunkDimensions.y, z);
            if (chunkChecker.Contains(pos))
            {
                chunks[pos].meshRenderer.enabled = false;
            }
        } 
    }

    IEnumerator HideColumns(int x, int z)
    {
        Vector2Int fpcPos = new Vector2Int(x, z);
        foreach (Vector2Int cc in chunkColumn)
        {
            if ((cc-fpcPos).magnitude >= drawRadius * chunkDimensions.x)
            {
                HideColumns(cc.x, cc.y);
            }
        }
        yield return null;
    }
    IEnumerator BuildRecursiveWorld(int x, int z, int rad)
    {
        int nextRad = rad - 1;
        if (rad <= 0)
        {
            yield break;
        }
        
        BuildChunkColumn(x,z + (int) chunkDimensions.z);
        buildQueue.Enqueue(BuildRecursiveWorld(x,z + (int) chunkDimensions.z, nextRad));
        yield return null;
        
        BuildChunkColumn(x,z - (int) chunkDimensions.z);
        buildQueue.Enqueue(BuildRecursiveWorld(x,z - (int) chunkDimensions.z, nextRad));
        yield return null;
        
        BuildChunkColumn(x + (int) chunkDimensions.x, z);
        buildQueue.Enqueue(BuildRecursiveWorld(x + (int) chunkDimensions.x, z, nextRad));
        yield return null;
        
        BuildChunkColumn(x - (int) chunkDimensions.x, z);
        buildQueue.Enqueue(BuildRecursiveWorld(x - (int) chunkDimensions.x, z, nextRad));
        yield return null;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
