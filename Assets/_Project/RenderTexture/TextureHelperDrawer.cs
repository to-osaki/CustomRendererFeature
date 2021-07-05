using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace to.Lib
{
	[DisallowMultipleComponent, RequireComponent(typeof(RawImage))]
	public class TextureHelperDrawer : MonoBehaviour
	{
		void _PerlinNoise()
		{
			TextureWriter = new PerlinNoiseTextureWriter();
			this.OnValidate();
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				UnityEditor.EditorUtility.SetDirty(this);
			}
#endif
		}
		void _VoronoiUV()
		{
			var random = new System.Random(System.DateTime.Now.Second);
			var points = Enumerable.Range(0, 10).Select(_ => new Vector2((float)random.NextDouble(), (float)random.NextDouble())).ToArray();
			TextureWriter = new VoronoiUVTextureWriter(points);
			this.OnValidate();
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				UnityEditor.EditorUtility.SetDirty(this);
			}
#endif
		}

		[SerializeReference]
		[ContextMenuItem(nameof(_PerlinNoise), nameof(_PerlinNoise))]
		[ContextMenuItem(nameof(_VoronoiUV), nameof(_VoronoiUV))]
		ITextureHelperWriter TextureWriter;

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