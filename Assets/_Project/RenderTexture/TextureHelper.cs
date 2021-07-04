using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace to.Lib
{
	static public class TextureHelper
	{
		static public void WriteRandom(Texture2D texture, FilterMode filterMode)
		{
			Write(texture, (x, y) =>
			{
				return new Color(
					UnityEngine.Random.Range(0f, 1f),
					UnityEngine.Random.Range(0f, 1f),
					UnityEngine.Random.Range(0f, 1f),
					1);
			});
		}

		static public void WritePerlinNoise(Texture2D texture, Vector2 npos, float nscale)
		{
			Write(texture, (x, y) =>
			{
				return new Color(
					Mathf.PerlinNoise(npos.x + x * nscale, npos.y + y * nscale),
					0,
					0,
					1);
			});
		}

		static public void WriteVoronoiUV(Texture2D texture, Vector2[] uvPoints)
		{
			float width = texture.width;
			float height = texture.height;
			Write(texture, (x, y) =>
			{
				Vector2 uv = new Vector2(x / width, y / height);
				float min = (uv - uvPoints[0]).SqrMagnitude();
				int idx = 0;
				for (int i = 1; i < uvPoints.Length; i++)
				{
					float d = (uv - uvPoints[i]).SqrMagnitude();
					if (d < min)
					{
						min = d;
						idx = i;
					}
				}
				return new Color(uvPoints[idx].x, uvPoints[idx].y, 0, 1);
			});
		}

		static public void Write(Texture2D texture, System.Func<int, int, Color> generator)
		{
			int width = texture.width;
			int height = texture.height;
			Color[] colors = new Color[width * height];
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					var color = generator(x, y);
					colors[x + y * width] = color;
				}
			}
			texture.SetPixels(colors);
			texture.Apply();
		}
	}
}