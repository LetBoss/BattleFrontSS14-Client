using System;
using System.Collections.Generic;
using System.Text;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.Utility;
using Robust.Shared.Graphics.RSI;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.ViewVariables;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Content.Client.Clickable;

internal sealed class ClickMapManager : IClickMapManager, IPostInjectInit
{
	private sealed class RsiClickMapData
	{
		public readonly ClickMap ClickMap;

		public readonly Dictionary<StateId, Vector2i[][]> Offsets;

		public RsiClickMapData(ClickMap clickMap, Dictionary<StateId, Vector2i[][]> offsets)
		{
			ClickMap = clickMap;
			Offsets = offsets;
		}
	}

	internal sealed class ClickMap
	{
		[ViewVariables]
		private readonly byte[] _data;

		public int Width { get; }

		public int Height { get; }

		[ViewVariables]
		public Vector2i Size => Vector2i.op_Implicit((Width, Height));

		public bool IsOccluded(int x, int y)
		{
			int num = y * Width + x;
			return (_data[num / 8] & (1 << num % 8)) != 0;
		}

		public bool IsOccluded(Vector2i vector)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			Vector2i val = vector;
			int num = default(int);
			int num2 = default(int);
			((Vector2i)(ref val)).Deconstruct(ref num, ref num2);
			int x = num;
			int y = num2;
			return IsOccluded(x, y);
		}

		private ClickMap(byte[] data, int width, int height)
		{
			Width = width;
			Height = height;
			_data = data;
		}

		public static ClickMap FromImage<T>(Image<T> image, float threshold) where T : unmanaged, IPixel<T>
		{
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			byte b = (byte)(threshold * 255f);
			int width = ((Image)image).Width;
			int height = ((Image)image).Height;
			byte[] array = new byte[(int)Math.Ceiling((float)(width * height) / 8f)];
			Span<T> pixelSpan = ImageSharpExt.GetPixelSpan<T>(image);
			for (int i = 0; i < pixelSpan.Length; i++)
			{
				Rgba32 val = default(Rgba32);
				((IPixel)pixelSpan[i]/*cast due to constrained. prefix*/).ToRgba32(ref val);
				if (val.A >= b)
				{
					array[i / 8] |= (byte)(1 << i % 8);
				}
			}
			return new ClickMap(array, width, height);
		}

		public string DumpText()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < Height; i++)
			{
				for (int j = 0; j < Width; j++)
				{
					stringBuilder.Append(IsOccluded(j, i) ? "1" : "0");
				}
				stringBuilder.AppendLine();
			}
			return stringBuilder.ToString();
		}
	}

	private static readonly string[] IgnoreTexturePaths = new string[4] { "/Textures/Interface", "/Textures/LobbyScreens", "/Textures/Parallaxes", "/Textures/Logo" };

	private const float Threshold = 0.1f;

	private const int ClickRadius = 2;

	[Dependency]
	private IResourceCache _resourceCache;

	[ViewVariables]
	private readonly Dictionary<Texture, ClickMap> _textureMaps = new Dictionary<Texture, ClickMap>();

	[ViewVariables]
	private readonly Dictionary<RSI, RsiClickMapData> _rsiMaps = new Dictionary<RSI, RsiClickMapData>();

	public void PostInject()
	{
		_resourceCache.OnRawTextureLoaded += OnRawTextureLoaded;
		_resourceCache.OnRsiLoaded += OnOnRsiLoaded;
	}

	private void OnOnRsiLoaded(RsiLoadedEventArgs obj)
	{
		if (((RsiLoadedEventArgs)(ref obj)).Atlas is Image<Rgba32> image)
		{
			RsiClickMapData value = new RsiClickMapData(ClickMap.FromImage<Rgba32>(image, 0.1f), ((RsiLoadedEventArgs)(ref obj)).AtlasOffsets);
			_rsiMaps[((RsiLoadedEventArgs)(ref obj)).Resource.RSI] = value;
		}
	}

	private void OnRawTextureLoaded(TextureLoadedEventArgs obj)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!(((TextureLoadedEventArgs)(ref obj)).Image is Image<Rgba32> image))
		{
			return;
		}
		string text = ((object)((TextureLoadedEventArgs)(ref obj)).Path/*cast due to constrained. prefix*/).ToString();
		string[] ignoreTexturePaths = IgnoreTexturePaths;
		foreach (string value in ignoreTexturePaths)
		{
			if (text.StartsWith(value, StringComparison.Ordinal))
			{
				return;
			}
		}
		_textureMaps[TextureResource.op_Implicit(((TextureLoadedEventArgs)(ref obj)).Resource)] = ClickMap.FromImage<Rgba32>(image, 0.1f);
	}

	public bool IsOccluding(Texture texture, Vector2i pos)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (!_textureMaps.TryGetValue(texture, out ClickMap value))
		{
			return false;
		}
		return SampleClickMap(value, pos, value.Size, Vector2i.Zero);
	}

	public bool IsOccluding(RSI rsi, StateId state, RsiDirection dir, int frame, Vector2i pos)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Invalid comparison between I4 and Unknown
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (!_rsiMaps.TryGetValue(rsi, out RsiClickMapData value))
		{
			return false;
		}
		if (!value.Offsets.TryGetValue(state, out Vector2i[][] value2) || value2.Length <= (int)dir)
		{
			return false;
		}
		Vector2i[] array = value2[dir];
		if (array.Length <= frame)
		{
			return false;
		}
		Vector2i offset = array[frame];
		return SampleClickMap(value.ClickMap, pos, rsi.Size, offset);
	}

	private static bool SampleClickMap(ClickMap map, Vector2i pos, Vector2i bounds, Vector2i offset)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		Vector2i val = bounds;
		int num = default(int);
		int num2 = default(int);
		((Vector2i)(ref val)).Deconstruct(ref num, ref num2);
		int num3 = num;
		int num4 = num2;
		val = pos;
		((Vector2i)(ref val)).Deconstruct(ref num2, ref num);
		int num5 = num2;
		int num6 = num;
		for (int i = -2; i <= 2; i++)
		{
			int num7 = num5 + i;
			if (num7 < 0 || num7 >= num3)
			{
				continue;
			}
			for (int j = -2; j <= 2; j++)
			{
				int num8 = num6 + j;
				if (num8 >= 0 && num8 < num4 && map.IsOccluded(Vector2i.op_Implicit((num7, num8)) + offset))
				{
					return true;
				}
			}
		}
		return false;
	}
}
