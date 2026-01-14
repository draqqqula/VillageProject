using System.Collections.Generic;
using UnityEngine;

public class DetachObjects : MonoBehaviour
{
    private const string BodiesLayer = "Bodies";

    [SerializeField] private List<Transform> _objects;

    public void Detach()
    {
        foreach (var obj in _objects)
        {
            Debug.Log(obj.name);

            Collider collider = obj.gameObject.GetComponent<BoxCollider>();
            if (collider == null)
            {
                collider = obj.gameObject.GetComponent<CapsuleCollider>();
            }
            if (collider != null)
            {
                Destroy(collider);
            }

            Mesh mesh;
            var meshRenderer = obj.gameObject.GetComponent<SkinnedMeshRenderer>();
            if (meshRenderer != null)
            {
                mesh = new Mesh();
                meshRenderer.BakeMesh(mesh, true);
                meshRenderer.rootBone = null;
                meshRenderer.bounds = mesh.bounds;
            }
            else
            {
                var renderer = obj.gameObject.GetComponent<MeshFilter>();
                mesh = renderer.mesh;
            }

            var newCollider = obj.gameObject.AddComponent<MeshCollider>();
            newCollider.sharedMesh = null;
            newCollider.sharedMesh = mesh;
            newCollider.convex = true;
            obj.gameObject.layer = LayerMask.NameToLayer(BodiesLayer);

            var position = obj.position;
            var rotation = obj.rotation;
            var scale = obj.lossyScale;
            obj.transform.SetParent(null, true);
            obj.position = position;
            obj.rotation = rotation;
            obj.localScale = scale;

            if (meshRenderer != null)
            {
                meshRenderer.bounds = newCollider.bounds;
            }

            obj.gameObject.AddComponent<Rigidbody>();
        }
    }
}