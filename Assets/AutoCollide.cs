using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class AutoCollide : MonoBehaviour
{
    [SerializeField] private List<Env> envObjects = new List<Env>();

    [Button("Add Colliders")]
    public void AddCollidersToChildren()
    {
        envObjects.Clear();
        AddCollidersToChildren(transform);
    }

    private void AddCollidersToChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            MeshFilter meshFilter = child.GetComponent<MeshFilter>();
            if (meshFilter != null && meshFilter.sharedMesh != null)
            {
                // Remove existing colliders
                Collider[] existingColliders = child.GetComponents<Collider>();
                foreach (Collider existingCollider in existingColliders)
                {
                    DestroyImmediate(existingCollider);
                }

                // Try to add compound colliders first
                if (!AddCompoundColliders(child))
                {
                    // If compound colliders are not suitable, add a MeshCollider
                    MeshCollider meshCollider = child.gameObject.AddComponent<MeshCollider>();
                    meshCollider.convex = false;

                    Env envAdd = new Env
                    {
                        objectEnv = child.gameObject,
                        meshCollider = meshCollider
                    };

                    envObjects.Add(envAdd);
                }
            }

            // Recursive call to handle children of children
            if (child.childCount > 0)
            {
                AddCollidersToChildren(child);
            }
        }
    }

    private bool AddCompoundColliders(Transform child)
    {
        Mesh mesh = child.GetComponent<MeshFilter>().sharedMesh;
        Bounds bounds = mesh.bounds;

        // Example check for suitability: simple heuristic based on bounds dimensions
        bool useBoxCollider = bounds.size.x < 2 && bounds.size.y < 2 && bounds.size.z < 2;

        if (useBoxCollider)
        {
            BoxCollider boxCollider = child.gameObject.AddComponent<BoxCollider>();
            boxCollider.center = bounds.center;
            boxCollider.size = bounds.size;

            Env envAdd = new Env
            {
                objectEnv = child.gameObject,
                meshCollider = null // No MeshCollider since we used BoxCollider
            };

            envObjects.Add(envAdd);
            return true;
        }

        return false; // Indicating that compound colliders were not suitable
    }

    [Button("Remove Colliders")]
    public void Clear()
    {
        foreach (var env in envObjects)
        {
            if (env.objectEnv != null)
            {
                Collider[] colliders = env.objectEnv.GetComponents<Collider>();
                foreach (Collider collider in colliders)
                {
                    DestroyImmediate(collider);
                }
            }
        }
        envObjects.Clear();
    }

    [System.Serializable]
    private class Env
    {
        [PreviewField] public GameObject objectEnv;
        public MeshCollider meshCollider;
    }
}
