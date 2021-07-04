using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace to.Lib
{
	interface ITextureWriter
	{
		void Write(Texture2D texture);
	}

	[System.Serializable]
	class PerlinNoiseTextureWriter : ITextureWriter
	{
		[SerializeField]
		Vector2 npos;
		[SerializeField]
		float nscale = 1 / 32f;

		public void Write(Texture2D texture)
		{
			TextureHelper.WritePerlinNoise(texture, npos, nscale);
		}
	}
	[System.Serializable]
	class VoronoiUVTextureWriter : ITextureWriter
	{
		[SerializeField]
		Vector2[] points;

		public VoronoiUVTextureWriter()
		{
			var random = new System.Random(System.DateTime.Now.Second);
			points = Enumerable.Range(0, 10).Select(_ => new Vector2((float)random.NextDouble(), (float)random.NextDouble())).ToArray();
		}

		public void Write(Texture2D texture)
		{
			TextureHelper.WriteVoronoiUV(texture, points);
		}
	}

	[DisallowMultipleComponent, RequireComponent(typeof(RawImage))]
	public class TextureHelperDrawer : MonoBehaviour
	{
		void _PerlinNoise()
		{
			TextureWriter = new PerlinNoiseTextureWriter();
			this.OnValidate();
			if (!Application.isPlaying)
			{
				UnityEditor.EditorUtility.SetDirty(this);
			}
		}
		void _VoronoiUV()
		{
			TextureWriter = new VoronoiUVTextureWriter();
			this.OnValidate();
			if (!Application.isPlaying)
			{
				UnityEditor.EditorUtility.SetDirty(this);
			}
		}

		[SerializeReference]
		[ContextMenuItem(nameof(_PerlinNoise), nameof(_PerlinNoise))]
		[ContextMenuItem(nameof(_VoronoiUV), nameof(_VoronoiUV))]
		ITextureWriter TextureWriter;

		RawImage image;
		Texture2D texture;

		private void OnValidate()
		{
			if (texture == null)
			{
				texture = new Texture2D(64, 64);
				texture.filterMode = FilterMode.Point;
			}
			TextureWriter?.Write(texture);
			image = GetComponent<RawImage>();
			image.texture = texture;
		}

		private void Start()
		{
#if !UNITY_EDITOR
		this.OnValidate();
#endif
		}

		private void OnDestroy()
		{
			Dispose();
		}

		void Dispose()
		{
			if (image != null)
			{
				image.texture = null;
			}
			if (texture != null)
			{
				Destroy(texture);
			}
			texture = null;
		}
	}
}