using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EdgeLine_Post : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public Material normalMat;
        public Material postMat;
        public RenderPassEvent delRenderLayer;
        public LayerMask drawLayer;
    }
    public Settings settings = new Settings();

    class NormalGet : ScriptableRenderPass
    {
        private Settings settings;
        EdgeLine_Post feature;
        FilteringSettings filteringSettings;
        ShaderTagId[] shaderTagId =
        {
           new ShaderTagId("SRPDefaultUnlit"),
           new ShaderTagId("UniversalForward")
        };
        public NormalGet(Settings settings, EdgeLine_Post feature)
        {
            this.settings = settings;
            this.feature = feature;

            RenderQueueRange renderQueue = new RenderQueueRange();
            renderQueue.lowerBound = 1000;
            renderQueue.upperBound = 3500;          
            filteringSettings = new FilteringSettings(renderQueue,settings.drawLayer);
        }
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            base.Configure(cmd, cameraTextureDescriptor);
            int temp = Shader.PropertyToID("_NormalTex");
            cmd.GetTemporaryRT(temp,cameraTextureDescriptor);
            ConfigureTarget(temp);
            ConfigureClear(ClearFlag.All, Color.black);
        }
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("法线图像绘制");
            DrawingSettings draw = CreateDrawingSettings(shaderTagId[0],ref renderingData,renderingData.cameraData.defaultOpaqueSortFlags);
            draw.SetShaderPassName(1, shaderTagId[1]);
            draw.overrideMaterial = settings.normalMat;
            draw.overrideMaterialPassIndex = 0;
            context.DrawRenderers(renderingData.cullResults, ref draw,ref filteringSettings);         
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
            
        }
    }
    class EdgeLinePost : ScriptableRenderPass
    {
        private Settings setting;
        EdgeLine_Post feature;
        public EdgeLinePost(Settings setting, EdgeLine_Post feature)
        {
            this.setting = setting;
            this.feature = feature;
        }
        public override void FrameCleanup(CommandBuffer cmd)
        {
            base.FrameCleanup(cmd);
            cmd.ReleaseTemporaryRT(Shader.PropertyToID("_NormalTex"));
        }
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("后处理效果");
            int sceneColor = Shader.PropertyToID("_CameraColorTexture");
            int camera = Shader.PropertyToID("_CameraColorAttachmentA_1920x1080_B10G11R11_UFloatPack32_Tex2D");//
            cmd.GetTemporaryRT(camera, renderingData.cameraData.cameraTargetDescriptor);
            float[] tempArrayX = new float[25]
            {1, 2, 3, 2, 1,
            0, 3, 5, 3, 0,
            0, 0, 0, 0, 0,
            0,-3,-5,-3, 0,
           -1,-2,-3,-2,-1
            };
            float[] tempArrayY = new float[25]
            {1, 0, 0, 0, -1,
            2, 3, 0,-3, -2,
            3, 5, 0,-5, -3,
            2, 3, 0,-3, -2,
            1, 0, 0, 0, -1
            };
            setting.postMat.SetFloatArray("_SobelX", tempArrayX);
            setting.postMat.SetFloatArray("_SobelY", tempArrayY);
            cmd.Blit(sceneColor, camera, setting.postMat, 0);
            cmd.ReleaseTemporaryRT(sceneColor);
            cmd.ReleaseTemporaryRT(camera);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    EdgeLinePost _EdgeLinePost;
    NormalGet _NormalGet;

    public override void Create()
    {
        _EdgeLinePost = new EdgeLinePost(settings,this);
        _EdgeLinePost.renderPassEvent = settings.delRenderLayer;
        _NormalGet = new NormalGet(settings, this);
        _NormalGet.renderPassEvent = settings.delRenderLayer-1;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(_EdgeLinePost);
        renderer.EnqueuePass(_NormalGet);
    }
}

