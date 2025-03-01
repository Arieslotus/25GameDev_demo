using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BlurRendererFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class BlurSettings
    {
        public int iterations = 10;  // 模糊迭代次数
        public float blurSpread = 0.6f;  // 每次迭代的扩散度
        public Shader blurShader;  // 模糊Shader
    }

    public BlurSettings settings = new BlurSettings();

    class BlurRenderPass : ScriptableRenderPass
    {
        ProfilingSampler m_ProfilingSampler = new("BlurRenderPass");


        private Material blurMaterial;
        private RTHandle source;
        private RTHandle temporaryTexture1, temporaryTexture2;
        private int width, height;
        private BlurSettings settings;

        public BlurRenderPass(BlurSettings blurSettings)
        {
            settings = blurSettings;
            blurMaterial = new Material(settings.blurShader) { hideFlags = HideFlags.DontSave };
        }

        // 执行高斯模糊操作
        public void Setup(RTHandle source)
        {
            this.source = source;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            // 计算纹理的宽度和高度
            width = renderingData.cameraData.cameraTargetDescriptor.width / 4;  // 下采样
            height = renderingData.cameraData.cameraTargetDescriptor.height / 4;

            //初始化设置这张图
            RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.width = width;
            desc.height = height;
            desc.depthBufferBits = 0;
            RenderingUtils.ReAllocateIfNeeded(ref temporaryTexture1, desc , FilterMode.Bilinear);//分配
            RenderingUtils.ReAllocateIfNeeded(ref temporaryTexture2, desc , FilterMode.Bilinear);
            cmd.SetGlobalTexture("_TemporaryTexture1", temporaryTexture1.nameID);//这样通过名字_MyColorTex就能在shader中找到_GrabTex
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("BlurRenderPass");

            using (new UnityEngine.Rendering.ProfilingScope(cmd, m_ProfilingSampler))
            {
                //kawase blur

                // 下采样到临时纹理
                blurMaterial.SetFloat("_BlurRange", 0.0f);
                cmd.Blit(source, temporaryTexture1, blurMaterial , 0);
                //Blitter.BlitCameraTexture(cmd, source, temporaryTexture1,blurMaterial,0);

                // 执行多次模糊迭代
                for (int i = 0; i < settings.iterations; i++)
                {
                    //float offset = 0.5f + i * settings.blurSpread;
                    float blurRange = (i + 1) * settings.blurSpread;

                    // 设置模糊偏移量
                    //blurMaterial.SetFloat("_BlurOffsets", offset);
                    blurMaterial.SetFloat("_BlurRange", blurRange);

                    // 使用 Graphics.Blit 执行模糊操作
                    cmd.Blit(temporaryTexture1, source, blurMaterial,0);
                    //Blitter.BlitCameraTexture(cmd, temporaryTexture1, temporaryTexture2, blurMaterial, 0);                   
                    //Blitter.BlitCameraTexture(cmd, temporaryTexture2, temporaryTexture1);

                    //var temRT = temporaryTexture1;
                    //temporaryTexture1 = temporaryTexture2;
                    //temporaryTexture2 = temRT;

                    // 将临时纹理作为输入进行下一次迭代
                    cmd.Blit(source, temporaryTexture1);
                    //Blitter.BlitCameraTexture(cmd, source, temporaryTexture);

                }
                //CoreUtils.SetRenderTarget(cmd, renderingData.cameraData.targetTexture);
                
                //Blitter.BlitCameraTexture(cmd, temporaryTexture1, source, blurMaterial, 0);

            }
            // 执行命令
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            cmd.Dispose();
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }

        public void OnDispose()
        {
            temporaryTexture1?.Release();
            temporaryTexture2?.Release();
        }
    }

    private BlurRenderPass blurRenderPass;

    public override void Create()
    {
        blurRenderPass = new BlurRenderPass(settings);
        blurRenderPass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;  // 在渲染透明物体之后执行
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        //if
        Camera camera = renderingData.cameraData.camera;
        if (!camera.CompareTag("BlurCamera")) return;

        // 把 `ScriptableRenderPass` 添加到渲染队列中
        renderer.EnqueuePass(blurRenderPass);
    }

    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        //if
        Camera camera = renderingData.cameraData.camera;
        if (!camera.CompareTag("BlurCamera")) return;

        // 使用 RTHandle 获取当前渲染目标
        RTHandle cameraColorHandle = renderingData.cameraData.renderer.cameraColorTargetHandle;
        blurRenderPass.Setup(cameraColorHandle);
    }
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        blurRenderPass.OnDispose();
    }
}
