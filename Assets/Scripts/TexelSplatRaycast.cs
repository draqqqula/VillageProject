using System.Collections.Generic;
using UnityEngine;

public class TexelSplatRaycast : MonoBehaviour
{
    [Header("Ray sampling")]
    [SerializeField] int verticalRays = 64;
    [SerializeField] int horizontalRays = 128;
    [SerializeField] float maxDistance = 100f;
    [SerializeField] LayerMask hitMask = ~0;

    [Header("Texel quads")]
    [SerializeField] float quadScale = 0.1f;
    [SerializeField] int texelLayer = 8;

    [Header("Material")]
    [SerializeField] Material quadMaterial;

    List<GameObject> spawnedQuads = new List<GameObject>();

    private void Start()
    {
        quadMaterial = new Material(quadMaterial);
    }

    // ---------- PUBLIC API ----------

    [ContextMenu("Generate Texels")]
    public void GenerateTexels()
    {
        ClearTexels();

        Vector3 origin = transform.position;

        for (int y = 0; y < verticalRays; y++)
        {
            float v = (y + 0.5f) / verticalRays;
            float theta = v * Mathf.PI;

            for (int x = 0; x < horizontalRays; x++)
            {
                float u = (x + 0.5f) / horizontalRays;
                float phi = u * Mathf.PI * 2f;

                Vector3 dir = new Vector3(
                    Mathf.Sin(theta) * Mathf.Cos(phi),
                    Mathf.Cos(theta),
                    Mathf.Sin(theta) * Mathf.Sin(phi)
                );

                if (Physics.Raycast(origin, dir, out RaycastHit hit, maxDistance, hitMask))
                {
                    CreateTexel(hit, dir);
                }
            }
        }
    }

    [ContextMenu("Clear Texels")]
    public void ClearTexels()
    {
        foreach (var go in spawnedQuads)
        {
            if (go) Destroy(go);
        }
        spawnedQuads.Clear();
    }

    // ---------- INTERNAL ----------

    void CreateTexel(RaycastHit hit, Vector3 rayDir)
    {
        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.name = "TexelQuad";
        quad.layer = texelLayer;

        DestroyImmediate(quad.GetComponent<Collider>());

        quad.transform.position = hit.point + hit.normal * 0.001f;

        // ориентация по нормали
        Vector3 n = hit.normal;
        Vector3 up = Vector3.up;
        if (Mathf.Abs(Vector3.Dot(n, up)) > 0.99f)
            up = Vector3.right;

        quad.transform.rotation = Quaternion.LookRotation(n, up);

        quad.AddComponent<FaceCamera>();

        // масштабирование квадрата: базовый размер * (1 / плотность) * расстояние до точки
        float baseScale = quadScale * (1f / Mathf.Sqrt(verticalRays * horizontalRays));
        float distanceScale = hit.distance; // можно добавить коэффициент, если нужно
        quad.transform.localScale = Vector3.one * baseScale * distanceScale;

        Vector3 scale = quad.transform.localScale;
        scale.z *= -1; // разворачиваем лицевую сторону
        quad.transform.localScale = scale;

        MeshRenderer mr = quad.GetComponent<MeshRenderer>();

        // создаём уникальный материал для каждого квадрата
        mr.material = quadMaterial;

        //TrySampleColor(hit, mr);

        spawnedQuads.Add(quad);
    }

    public void SetAlpha(float newAlpha)
    {
        var alpha = Mathf.Clamp01(newAlpha);

        // ВАЖНО: material, а не sharedMaterial
        Color color = quadMaterial.color;
        color.a = alpha;
        quadMaterial.color = color;
    }

    void TrySampleColor(RaycastHit hit, MeshRenderer quadRenderer)
    {
        Renderer srcRenderer = hit.collider.GetComponent<Renderer>();
        if (!srcRenderer) return;

        Material srcMat = srcRenderer.sharedMaterial;
        Debug.Log(srcMat.mainTexture);
        if (!srcMat || !srcMat.mainTexture) return;

        Texture2D tex = srcMat.mainTexture as Texture2D;
        Debug.Log(tex.isReadable);
        if (!tex || !tex.isReadable) return;

        Vector2 uv = hit.textureCoord;
        Color c = tex.GetPixelBilinear(uv.x, uv.y);

        Debug.Log("color successful");

        quadRenderer.material.color = c;
    }
}