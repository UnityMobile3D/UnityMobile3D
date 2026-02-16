#if UNITY_EDITOR
using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public static class NavMeshHardReset
{
    [MenuItem("Tools/NavMesh/Hard Reset (Remove All)")]
    public static void RemoveAll()
    {
        NavMesh.RemoveAllNavMeshData();
        Debug.Log("All NavMeshData removed.");
    }
}

public class NavMeshGridBakerWindow : EditorWindow
{
    [Header("Bounds (world)")]
    public Vector3 origin = new Vector3(-50f, 0f, -50f);
    public Vector3 size = new Vector3(100f, 0f, 100f); // y ignored

    [Header("Grid")]
    public float cellSize = 0.5f;

    [Header("NavMesh Area")]
    public int areaMask = NavMesh.AllAreas;

    [Header("Character")]
    public float characterRadius = 0.5f;
    public bool applyDilation = true;

    [Header("Output")]
    public string outputFileName = "map.txt";

    [MenuItem("Tools/NavMesh Grid Baker")]
    public static void Open()
    {
        GetWindow<NavMeshGridBakerWindow>("NavMesh Grid Baker");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("NavMesh -> Walkable Grid Export (Raycast)", EditorStyles.boldLabel);

        origin = EditorGUILayout.Vector3Field("Origin", origin);
        size = EditorGUILayout.Vector3Field("Size (X,Z used)", size);

        cellSize = EditorGUILayout.FloatField("Cell Size (m)", cellSize);


        GUILayout.Space(5);
        EditorGUILayout.LabelField("NavMesh Area", EditorStyles.boldLabel);
        areaMask = EditorGUILayout.IntField("Area Mask", areaMask);

        GUILayout.Space(5);
        EditorGUILayout.LabelField("Character", EditorStyles.boldLabel);
        characterRadius = EditorGUILayout.FloatField("Character Radius (m)", characterRadius);
        applyDilation = EditorGUILayout.Toggle("Apply Dilation", applyDilation);

        GUILayout.Space(5);
        outputFileName = EditorGUILayout.TextField("Output File Name", outputFileName);

        GUILayout.Space(10);

        if (GUILayout.Button("Bake & Export map.txt"))
        {
            BakeAndExport();
        }

        GUILayout.Space(10);
        EditorGUILayout.HelpBox(
            "Raycast로 바닥(y)을 먼저 찾고, 그 점 근처에서 NavMesh.SamplePosition으로 walkable(1/0)을 결정합니다.\n" +
            "NavMesh가 Bake 되어 있어야 하며, groundLayerMask에 바닥/지형 레이어가 포함되어야 합니다.",
            MessageType.Info);
    }

    private void BakeAndExport()
    {
        if (cellSize <= 0.0f)
        {
            Debug.LogError("cellSize must be > 0");
            return;
        }

        int width = Mathf.CeilToInt(size.x / cellSize);
        int height = Mathf.CeilToInt(size.z / cellSize);

        byte[,] walkable = new byte[height, width]; // [z, x]

        int successCount = 0;
        int rayHitCount = 0;

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                float worldX = origin.x + (x + 0.5f) * cellSize;
                float worldZ = origin.z + (z + 0.5f) * cellSize;

                NavMeshHit navHit;
                Vector3 queryPos = new Vector3(worldX, origin.y, worldZ);

                // 1) 후보 NavMesh를 넉넉한 반경으로 찾는다 (이 값은 커도 됨)
                bool found = NavMesh.SamplePosition(queryPos, out navHit, 50.0f, areaMask);

                bool ok = false;
                if (found)
                {
                    // 2) 판정은 "셀 중심이 navmesh에 얼마나 가까운지"로 한다 (XZ 기준)
                    float dx = navHit.position.x - worldX;
                    float dz = navHit.position.z - worldZ;
                    float distXZ = Mathf.Sqrt(dx * dx + dz * dz);

                    // 임계값: 셀 크기 기반으로 고정
                    // cellSize=0.5면 threshold는 보통 0.35~0.45 사이가 안정적
                    float threshold = cellSize * 0.49f;

                    ok = (distXZ <= threshold);
                }

                walkable[z, x] = (byte)(ok ? 1 : 0);
                if (ok) successCount++;
            }
        }

        byte[,] finalMap = walkable;

        //if (applyDilation)
        //{
        //    int k = Mathf.CeilToInt(characterRadius / cellSize);
        //    finalMap = DilateBlocked(walkable, width, height, k);
        //    Debug.Log("Dilation cells k = " + k);
        //}

        string path = Path.Combine(Application.dataPath, outputFileName);

        StringBuilder sb = new StringBuilder(64 + width * height);

        sb.Append("origin ").Append(origin.x.ToString("F3")).Append(" ")
          .Append(origin.z.ToString("F3")).Append("\n");
        sb.Append("cell ").Append(cellSize.ToString("F3")).Append("\n");
        sb.Append("size ").Append(width).Append(" ").Append(height).Append("\n");

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                sb.Append(finalMap[z, x] == 1 ? '1' : '0');
            }
            sb.Append('\n');
        }

        File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        AssetDatabase.Refresh();

        Debug.Log("Exported: " + path);
        Debug.Log("Ray hit count: " + rayHitCount + " / " + (width * height));
        Debug.Log("NavMesh success count: " + successCount + " / " + (width * height));
    }

    // 막힌 셀(0)을 주변으로 k만큼 퍼뜨림(팽창) -> 캐릭터 반지름 고려
    private byte[,] DilateBlocked(byte[,] src, int width, int height, int k)
    {
        byte[,] dst = new byte[height, width];

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                dst[z, x] = src[z, x];
            }
        }

        if (k <= 0)
            return dst;

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                if (src[z, x] == 0)
                {
                    int z0 = Math.Max(0, z - k);
                    int z1 = Math.Min(height - 1, z + k);
                    int x0 = Math.Max(0, x - k);
                    int x1 = Math.Min(width - 1, x + k);

                    for (int zz = z0; zz <= z1; zz++)
                    {
                        for (int xx = x0; xx <= x1; xx++)
                        {
                            dst[zz, xx] = 0;
                        }
                    }
                }
            }
        }

        return dst;
    }

    // LayerMask를 EditorWindow에서 편하게 편집하기 위한 헬퍼
    private LayerMask LayerMaskField(string label, LayerMask selected)
    {
        string[] layers = GetAllLayerNames();
        int mask = selected.value;

        mask = EditorGUILayout.MaskField(label, mask, layers);
        selected.value = mask;
        return selected;
    }

    private string[] GetAllLayerNames()
    {
        string[] layers = new string[32];
        for (int i = 0; i < 32; i++)
        {
            string layerName = LayerMask.LayerToName(i);
            if (string.IsNullOrEmpty(layerName))
                layerName = "Layer " + i;
            layers[i] = layerName;
        }
        return layers;
    }
}

 
#endif