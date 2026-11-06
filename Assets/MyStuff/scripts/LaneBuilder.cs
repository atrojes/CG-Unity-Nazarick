using UnityEngine;

[ExecuteInEditMode]
public class LaneBuilder : MonoBehaviour
{
    public GameObject pillarPrefab;
    public GameObject wallSegmentPrefab;
    public int pillarCount = 12;
    public float lineLength = 60f;      // largo total, centrado en el GameObject

    [Header("Tamaño del pilar")]
    public float pillarHeight = 15f;
    public float pillarThickness = 2f;

    [Header("Viga superior")]
    public float wallHeight = 1f;
    public float wallThickness = 1f;

    [ContextMenu("Build")]
    public void Build()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);

        Vector3[] tops = new Vector3[pillarCount];

        for (int i = 0; i < pillarCount; i++)
        {
            float t = pillarCount > 1 ? i / (float)(pillarCount - 1) : 0f;

            // De -largo/2 a +largo/2 a lo largo del eje X local
            float offset = Mathf.Lerp(-lineLength / 2f, lineLength / 2f, t);
            Vector3 world = transform.position + transform.right * offset;

            if (Terrain.activeTerrain != null)
                world.y = Terrain.activeTerrain.SampleHeight(world)
                          + Terrain.activeTerrain.transform.position.y;

            var p = Instantiate(pillarPrefab, world, Quaternion.identity, transform);
            p.name = $"Pillar_{i}";

            ResizePillar(p, pillarHeight, pillarThickness);
            tops[i] = new Vector3(world.x, GetTopY(p), world.z);
        }

        if (wallSegmentPrefab == null) return;

        for (int i = 0; i < pillarCount - 1; i++)
        {
            Vector3 mid = (tops[i] + tops[i + 1]) / 2f;
            Vector3 dir = tops[i + 1] - tops[i];
            mid.y += wallHeight / 2f;

            var w = Instantiate(wallSegmentPrefab, mid,
                Quaternion.LookRotation(dir, Vector3.up), transform);
            w.name = $"Wall_{i}";
            w.transform.localScale = new Vector3(wallThickness, wallHeight, dir.magnitude);
        }
    }

    void ResizePillar(GameObject go, float targetHeight, float targetThickness)
    {
        var renderers = go.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return;

        Bounds b = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
            b.Encapsulate(renderers[i].bounds);

        float scaleY = targetHeight / b.size.y;
        float scaleXZ = targetThickness / Mathf.Max(b.size.x, b.size.z);

        Vector3 s = go.transform.localScale;
        go.transform.localScale = new Vector3(s.x * scaleXZ, s.y * scaleY, s.z * scaleXZ);
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
}