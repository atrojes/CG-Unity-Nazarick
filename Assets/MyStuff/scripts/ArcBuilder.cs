using UnityEngine;

[ExecuteInEditMode]
public class ArcBuilder : MonoBehaviour
{
    public GameObject pillarPrefab;
    public GameObject wallSegmentPrefab;   // un cubo sirve
    public float radius = 20f;
    public float startAngle = 0f;
    public float endAngle = 270f;
    public int pillarCount = 12;

    [Header("Viga superior")]
    public float wallHeight = 1f;          // alto de la viga
    public float wallThickness = 1f;       // grosor de la viga

    [Header("Tamaño del pilar")]
    public float pillarHeight = 15f;
    public float pillarThickness= 2f;

    [ContextMenu("Build")]
    public void Build()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);

        float step = (endAngle - startAngle) / (pillarCount - 1);
        Vector3[] tops = new Vector3[pillarCount];

        for (int i = 0; i < pillarCount; i++)
        {
            float a = (startAngle + step * i) * Mathf.Deg2Rad;
            Vector3 world = transform.position +
                            new Vector3(Mathf.Cos(a), 0, Mathf.Sin(a)) * radius;

            if (Terrain.activeTerrain != null)
                world.y = Terrain.activeTerrain.SampleHeight(world)
                          + Terrain.activeTerrain.transform.position.y;

            var p = Instantiate(pillarPrefab, world, Quaternion.identity, transform);
            p.name = $"Pillar_{i}";

            ResizePillar(p, pillarHeight, pillarThickness);

            // Punto superior real del pilar, medido con sus renderers
            tops[i] = new Vector3(world.x, GetTopY(p), world.z);
        }

        if (wallSegmentPrefab == null) return;

        for (int i = 0; i < pillarCount - 1; i++)
        {
            Vector3 mid = (tops[i] + tops[i + 1]) / 2f;
            Vector3 dir = tops[i + 1] - tops[i];

            // La viga apoya sobre la parte superior: subimos medio alto
            mid.y += wallHeight / 2f;

            var w = Instantiate(wallSegmentPrefab, mid,
                Quaternion.LookRotation(dir, Vector3.up), transform);
            w.name = $"Wall_{i}";
            w.transform.localScale = new Vector3(wallThickness, wallHeight, dir.magnitude);
        }
    }

    float GetTopY(GameObject go)
    {
        var renderers = go.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return go.transform.position.y;

        Bounds b = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
            b.Encapsulate(renderers[i].bounds);

        return b.max.y;
    }

    void ResizePillar(GameObject go, float targetHeight, float targetThickness)
    {
        var renderers = go.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return;

        // Tamaño actual del pilar (con la escala que trae el prefab)
        Bounds b = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
            b.Encapsulate(renderers[i].bounds);

        // Factores de escala necesarios para llegar al tamaño deseado
        float scaleY = targetHeight / b.size.y;
        float thickness = Mathf.Max(b.size.x, b.size.z);
        float scaleXZ = targetThickness / thickness;

        Vector3 s = go.transform.localScale;
        go.transform.localScale = new Vector3(s.x * scaleXZ, s.y * scaleY, s.z * scaleXZ);
    }
}