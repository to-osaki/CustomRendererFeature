using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace to.Lib
{
	public class DrawOnRenderTextureFeature : ScriptableRendererFeature
	{
		[System.Serializable]
		public class Settings
		{
			public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
			public Material blitMaterial = null;
			public int shaderPassIndex = 0;
			public RenderTexture targetTexture = null;
		}

		[SerializeField]
		Settings settings = new Settings();

		class CustomRenderPass : ScriptableRenderPass
		{
			public Settings settings;

			RenderTargetIdentifier source;

			string m_ProfilerTag = nameof(DrawOnRenderTextureFeature);

			// This method is called before executing the render pass.
			// It can be used to configure render targets and their clear state. Also to create temporary render target textures.
			// When empty this render pass will render to the active camera render target.
			// You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
			// The render pipeline will ensure target setup and clearing happens in a performant manner.
			public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
			{
				RenderTextureDescriptor blitTargetDescriptor = renderingData.cameraData.cameraTargetDescriptor;
				blitTargetDescriptor.depthBufferBits = 0;

				var renderer = renderingData.cameraData.renderer;
				source = renderer.cameraColorTarget;
			}

			// Here you can implement the rendering logic.
			// Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
			// https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
			// You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
			public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
			{
				if (renderingData.cameraData.isSceneViewCamera) { return; }

				CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);
				Blit(cmd, source, settings.targetTexture, settings.blitMaterial, settings.shaderPassIndex);
				context.ExecuteCommandBuffer(cmd);
				CommandBufferPool.Release(cmd);
			}

			// Cleanup any allocated resources that were created during the execution of this render pass.
			public override void OnCameraCleanup(CommandBuffer cmd)
			{
			}
		}

		CustomRenderPass m_ScriptablePass;

		/// <inheritdoc/>
		public override void Create()
		{
			m_ScriptablePass = new CustomRenderPass();

			// Configures where the render pass should be injected.
			m_ScriptablePass.renderPassEvent = settings.renderPassEvent;
		}

		// Here you can inject one or multiple render passes in the renderer.
		// This method is called whe	n setting up the renderer once per-camera.
		public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
		{
			if (settings.blitMaterial == null || settings.targetTexture == null)
			{
				return;
			}

			m_ScriptablePass.renderPassEvent = settings.renderPassEvent;
			m_ScriptablePass.settings = settings;
			renderer.EnqueuePass(m_ScriptablePass);
		}
	}
}