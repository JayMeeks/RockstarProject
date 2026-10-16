using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CustomPostProcessRenderFeature : ScriptableRendererFeature
{
    [SerializeField] private Shader m_vignetteShader;
  
    private Material m_vignetteMaterial;
  
    private CustomPostProcessPass m_customPass;
    
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_customPass);
    }

/*
    public override void SetupRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        
    }*/
    
    public override void Create()
    {
       m_vignetteMaterial = CoreUtils.CreateEngineMaterial(m_vignetteShader);
       
       m_customPass = new CustomPostProcessPass(m_vignetteMaterial);
    }

    protected override void Dispose(bool disposing)
    {
        CoreUtils.Destroy(m_vignetteMaterial);
    }
}
