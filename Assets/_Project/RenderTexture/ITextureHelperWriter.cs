using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace to.Lib
{
	public interface ITextureHelperWriter
	{
		void Write(Texture2D texture);
	}

	[System.Serializable]
	public class PerlinNoiseTextureWriter : ITextureHelperWriter
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
	public class VoronoiUVTextureWriter : ITextureHelperWriter
	{
		[SerializeField]
		Vector2[] points;

		public VoronoiUVTextureWriter() { }

		public VoronoiUVTextureWriter(Vector2[] points) => this.points = points;

		public void Write(Texture2D texture)
		{
			TextureHelper.WriteVoronoiUV(texture, points);
		}
	}
}
