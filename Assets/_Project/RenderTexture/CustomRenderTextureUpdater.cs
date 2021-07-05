using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace to.Lib
{
	[DisallowMultipleComponent]
	public class CustomRenderTextureUpdater : MonoBehaviour
	{
		[SerializeField]
		string TexturePropertyName;
		[SerializeField]
		CustomRenderTexture TargetTexture;
		[SerializeField]
		int DefaultPassIndex = 0;
		[SerializeField]
		CustomRenderTextureUpdateMode UpdateMode = CustomRenderTextureUpdateMode.OnDemand;

		public CustomRenderTextureUpdateZoneSpace ZoneSpace => _targetTexture.updateZoneSpace;
		public Vector2Int Size => new Vector2Int(_targetTexture.width, _targetTexture.height);
		public Texture GetTexture() => TargetTexture;

		bool HasDefaultPass => DefaultPassIndex >= 0;
		CustomRenderTexture _targetTexture;
		CustomRenderTextureUpdateZone DefaultZone;
		Queue<CustomRenderTextureUpdateZone> _requests = new Queue<CustomRenderTextureUpdateZone>();

		public void UpdateZone(int passIndex, Vector2 center, Vector2 size, float rotation)
		{
			if (HasDefaultPass && _requests.Count == 0)
			{
				_requests.Enqueue(DefaultZone);
			}
			var clickZone = new CustomRenderTextureUpdateZone()
			{
				needSwap = true,
				passIndex = passIndex,
				rotation = rotation,
				updateZoneCenter = center,
				updateZoneSize = size,
			};
			_requests.Enqueue(clickZone);
		}

		void Start()
		{
			if (TargetTexture != null)
			{
				_targetTexture = TargetTexture;
			}
			else
			{
				_targetTexture = new CustomRenderTexture(Screen.width, Screen.height);
				_targetTexture.updateZoneSpace = CustomRenderTextureUpdateZoneSpace.Pixel;
				_targetTexture.doubleBuffered = true;
			}
			_targetTexture.initializationMode = CustomRenderTextureUpdateMode.OnDemand;
			_targetTexture.updateMode = CustomRenderTextureUpdateMode.OnDemand;
			_targetTexture.Initialize();

			if (HasDefaultPass)
			{
				DefaultZone = new CustomRenderTextureUpdateZone()
				{
					needSwap = true,
					passIndex = DefaultPassIndex,
					rotation = 0f,
					updateZoneCenter = new Vector2(0.5f, 0.5f),
					updateZoneSize = new Vector2(1f, 1f),
				};
			}

			if (UpdateMode == CustomRenderTextureUpdateMode.OnLoad && HasDefaultPass && _requests.Count == 0)
			{
				_requests.Enqueue(DefaultZone);
			}

			if (!string.IsNullOrEmpty(TexturePropertyName))
			{
				Shader.SetGlobalTexture(TexturePropertyName, _targetTexture);
			}
		}

		private void OnDestroy()
		{
			if (_targetTexture != null)
			{
				_targetTexture.ClearUpdateZones();
				if (_targetTexture != TargetTexture)
				{
					_targetTexture.Release();
					Destroy(_targetTexture);
				}
			}
			_targetTexture = null;
		}

		void Update()
		{
			if (UpdateMode == CustomRenderTextureUpdateMode.Realtime && HasDefaultPass && _requests.Count == 0)
			{
				_requests.Enqueue(DefaultZone);
			}
			UpdateZones();
		}

		void UpdateZones()
		{
			_targetTexture.ClearUpdateZones();

			if (_requests.Count == 0) { return; }

			var zones = new CustomRenderTextureUpdateZone[_requests.Count];
			_requests.CopyTo(zones, 0);
			_requests.Clear();

			_targetTexture.SetUpdateZones(zones);

			_targetTexture.Update(1);
		}
	}
}