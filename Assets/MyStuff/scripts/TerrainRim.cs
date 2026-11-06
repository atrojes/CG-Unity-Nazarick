using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TerrainRim : MonoBehaviour
{
    public Terrain terrain;

    [Header("Forma")]
    public float innerRadius = 60f;     // radio donde empieza el desnivel (base del acantilado)
    public float rimHeight = 16f;       // cuánto sube la tierra respecto al suelo interior
    public float cliffWidth = 2f;       // ancho horizontal del desnivel (más pequeño = más vertical)
    public float outerFalloff = 40f;    // distancia para fundirse con el terreno original

    [Header("Interior")]
    public bool flattenInterior = true; // aplana el suelo dentro del círculo

    [ContextMenu("Build Rim")]
    public void BuildRim()
    {
        if (terrain == null) terrain = Terrain.activeTerrain;
        TerrainData td = terrain.terrainData;

#if UNITY_EDITOR
        Undo.RegisterCompleteObjectUndo(td, "Build Rim");
#endif

        Vector3 tp = terrain.transform.position;
        Vector3 size = td.size;
        Vector3 c = transform.position;
        int res = td.heightmapResolution;

        float floorY = terrain.SampleHeight(c) + tp.y;  // altura del suelo interior
        float rimY = floorY + rimHeight;
        float outer = innerRadius + cliffWidth + outerFalloff;

        int xMin = Mathf.Clamp(Mathf.FloorToInt((c.x - outer - tp.x) / size.x * (res - 1)), 0, res - 1);
        int xMax = Mathf.Clamp(Mathf.CeilToInt((c.x + outer - tp.x) / size.x * (res - 1)), 0, res - 1);
        int zMin = Mathf.Clamp(Mathf.FloorToInt((c.z - outer - tp.z) / size.z * (res - 1)), 0, res - 1);
        int zMax = Mathf.Clamp(Mathf.CeilToInt((c.z + outer - tp.z) / size.z * (res - 1)), 0, res - 1);

        int w = xMax - xMin + 1, h = zMax - zMin + 1;
        float[,] heights = td.GetHeights(xMin, zMin, w, h); // índice [z, x]

        float floorN = (floorY - tp.y) / size.y;
        float rimN = (rimY - tp.y) / size.y;

        for (int j = 0; j < h; j++)
        for (int i = 0; i < w; i++)
        {
            float wx = tp.x + (xMin + i) / (float)(res - 1) * size.x;
            float wz = tp.z + (zMin + j) / (float)(res - 1) * size.z;
            float d = Vector2.Distance(new Vector2(wx, wz), new Vector2(c.x, c.z));

            if (d <= innerRadius)
            {
                if (flattenInterior) heights[j, i] = floorN;
            }
            else if (d <= innerRadius + cliffWidth)
            {
                // El desnivel: sube del suelo al borde de tierra
                float k = (d - innerRadius) / cliffWidth;
                heights[j, i] = Mathf.Lerp(floorN, rimN, Mathf.SmoothStep(0f, 1f, k));
            }
            else if (d <= outer)
            {
                // Desde el borde elevado, se funde con el terreno original
                float k = (d - innerRadius - cliffWidth) / outerFalloff;
                heights[j, i] = Mathf.Lerp(rimN, heights[j, i], Mathf.SmoothStep(0f, 1f, k));
            }
        }
        td.SetHeights(xMin, zMin, heights);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        DrawCircle(innerRadius);
        Gizmos.color = Color.yellow;
        DrawCircle(innerRadius + cliffWidth + outerFalloff);
    }

    void DrawCircle(float r)
    {
        Vector3 prev = transform.position + new Vector3(r, 0, 0);
        for (int i = 1; i <= 64; i++)
        {
            float a = i / 64f * Mathf.PI * 2f;
            Vector3 next = transform.position + new Vector3(Mathf.Cos(a), 0, Mathf.Sin(a)) * r;
            Gizmos.DrawLine(prev, next);
            prev = next;
        }
    }
}