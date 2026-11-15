using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

public class CustomPostProcessPass : ScriptableRenderPass
{
    private const string passName = "Custom post process";
    
    private static readonly int radiusID = Shader.PropertyToID("_VignetteRadius");
    private static readonly int featherID = Shader.PropertyToID("_VignetteFeather");
    private static readonly int colourID = Shader.PropertyToID("_VignetteColour");
    
    private float radius;
    private float feather;
    private Color colour;
    
    private Material m_material;

    public CustomPostProcessPass(Material material)
    {
        m_material = material;

        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public void SetRadius(float radius)
    {
        this.radius = radius;
    }

    public void SetFeather(float feather)
    {
        this.feather = feather;
    }

    public void SetColour(Color colour)
    {
        this.colour = colour;
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        if (m_material == null) {return;}

        UniversalResourceData resources = frameData.Get<UniversalResourceData>();

        if (resources.isActiveTargetBackBuffer) { return; }

        TextureHandle source = resources.activeColorTexture;

        TextureDesc descriptor = renderGraph.GetTextureDesc(source);
        
        descriptor.name = passName;
        descriptor.clearBuffer = false;
        descriptor.depthBufferBits = 0;
        
        TextureHandle destination = renderGraph.CreateTexture(descriptor);
        
        RenderGraphUtils.BlitMaterialParameters blitParams = 
        new RenderGraphUtils.BlitMaterialParameters(
        source,
        destination,
        m_material,
        0);
        
        blitParams.material.SetFloat(radiusID, radius);
        blitParams.material.SetFloat(featherID, feather);
        blitParams.material.SetVector(colourID, colour);
        
       
        
       
        
        renderGraph.AddBlitPass(blitParams,passName: passName);
        resources.cameraColor = destination;

    }
}
