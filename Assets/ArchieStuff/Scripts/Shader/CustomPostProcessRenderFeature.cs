using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CustomPostProcessRenderFeature : ScriptableRendererFeature
{
    [SerializeField] private Shader m_shader;

    [SerializeField,Range(0.0f,5.0f)] private float radius;
    [SerializeField,Range(0.0f,5.0f)] private float feather;
    [SerializeField] private Color Colour;
  
    private Material m_Material;
  
    private CustomPostProcessPass m_customPass;
    
    public override void Create()
    { 
    
      
       if (m_shader == null) { return;}
     
       m_Material = CoreUtils.CreateEngineMaterial(m_shader);
       
       m_customPass = new CustomPostProcessPass(m_Material);
    }
    
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (m_Material == null || m_customPass == null) {return;}
        
        if(renderingData.cameraData.cameraType != CameraType.Game){return;}
        
        m_customPass.SetRadius(radius);
        m_customPass.SetFeather(feather);
        m_customPass.SetColour(Colour);
       
        renderer.EnqueuePass(m_customPass);
    }
    
    

    protected override void Dispose(bool disposing)
    {
        CoreUtils.Destroy(m_Material);
        m_Material = null;
        m_customPass = null;
    }
}
