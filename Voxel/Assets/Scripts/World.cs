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
    public static Vector3Int worldDimensions = new Vector3Int(4, 4, 4);
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

    void BuildChunkColumn(int x, int z)
    {
        for (int y = 0; y < worldDimensions.y; y++)
        {
            GameObject chunk = Instantiate(chunkPrefab);
            Vector3Int position = new Vector3Int(x * chunkDimensions.x, y * chunkDimensions.y, z * chunkDimensions.z);
            chunk.GetComponent<Chunk>().CreateChunk(chunkDimensions, position);
        }
    }
    
    IEnumerator BuildWorld()
    {
        for (int z = 0; z < worldDimensions.z; z++)
        {
            for (int x = 0; x < worldDimensions.x; x++) 
            { 
                BuildChunkColumn(x,z);
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
