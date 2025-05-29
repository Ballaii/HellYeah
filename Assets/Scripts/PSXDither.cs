using UnityEngine;

[ExecuteInEditMode]
public class RetroDitherEffect : MonoBehaviour
{
    public Material ditherMaterial;

    [Range(1, 8)]
    public float colorDepth = 4f;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (ditherMaterial != null)
        {
            ditherMaterial.SetFloat("_ColorDepth", colorDepth);
            Graphics.Blit(src, dest, ditherMaterial);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
