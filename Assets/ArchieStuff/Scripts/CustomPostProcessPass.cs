using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CustomPostProcessPass : ScriptableRenderPass
{
    private Material m_vignetteMaterial;

    public CustomPostProcessPass(Material vignetteMaterial)
    {
        m_vignetteMaterial = vignetteMaterial;
    }
  
  
  /*  public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        
    }   */
}
