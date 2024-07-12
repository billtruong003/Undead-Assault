using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    public MeshRenderer[] meshRenderers;
    public SkinnedMeshRenderer[] skinnedMeshRenderers;
    public Movement movement;
    public GameObject cam;
    public Camera renderObjectCam;

    public void SetUpRenderer()
    {
        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        }
        foreach (SkinnedMeshRenderer skinnedMeshRenderer in skinnedMeshRenderers)
        {
            skinnedMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        }
    }
    public void IsLocalPlayer()
    {
        movement.enabled = true;
        cam.SetActive(true);
        SetUpRenderer();
    }

}
