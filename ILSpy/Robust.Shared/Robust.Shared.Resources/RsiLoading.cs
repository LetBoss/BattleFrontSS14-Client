using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Robust.Shared.Graphics;
using Robust.Shared.Maths;
using Robust.Shared.Utility;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;

namespace Robust.Shared.Resources;

internal static class RsiLoading
{
	internal sealed class RsiMetadata(Vector2i size, StateMetadata[] states, TextureLoadParameters loadParameters, bool metaAtlas, bool rsic)
	{
		public readonly Vector2i Size = size;

		public readonly StateMetadata[] States = states;

		public readonly TextureLoadParameters LoadParameters = loadParameters;

		public readonly bool MetaAtlas = metaAtlas;

		public readonly bool Rsic = rsic;
	}

	internal sealed class StateMetadata
	{
		public readonly string StateId;

		public readonly int DirCount;

		public readonly float[][] Delays;

		public StateMetadata(string stateId, int dirCount, float[][] delays)
		{
			StateId = stateId;
			DirCount = dirCount;
			Delays = delays;
		}
	}

	private sealed record RsiJsonMetadata(Vector2i Size, StateJsonMetadata[] States, RsiJsonLoad? Load, bool MetaAtlas = true, bool Rsic = true);

	private sealed record StateJsonMetadata(string Name, int? Directions, float[][]? Delays);

	private sealed record RsiJsonLoad(bool Srgb = true);

	private static readonly float[] OneArray = new float[1] { 1f };

	private static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

	public const uint MINIMUM_RSI_VERSION = 1u;

	public const uint MAXIMUM_RSI_VERSION = 1u;

	internal const string RsicPngField = "robusttoolbox_rsic_meta";

	internal static RsiMetadata LoadRsiMetadata(string metadata)
	{
		RsiJsonMetadata? rsiJsonMetadata = JsonSerializer.Deserialize<RsiJsonMetadata>(metadata, SerializerOptions);
		if (rsiJsonMetadata == null)
		{
			throw new RSILoadException("Manifest JSON failed to deserialize!");
		}
		return LoadRsiMetadataCore(rsiJsonMetadata);
	}

	internal static RsiMetadata LoadRsiMetadata(Stream manifestFile)
	{
		RsiJsonMetadata? rsiJsonMetadata = JsonSerializer.Deserialize<RsiJsonMetadata>(manifestFile, SerializerOptions);
		if (rsiJsonMetadata == null)
		{
			throw new RSILoadException("Manifest JSON failed to deserialize!");
		}
		return LoadRsiMetadataCore(rsiJsonMetadata);
	}

	private static RsiMetadata LoadRsiMetadataCore(RsiJsonMetadata manifestJson)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		Vector2i size = manifestJson.Size;
		StateMetadata[] array = new StateMetadata[manifestJson.States.Length];
		for (int i = 0; i < manifestJson.States.Length; i++)
		{
			StateJsonMetadata stateJsonMetadata = manifestJson.States[i];
			string name = stateJsonMetadata.Name;
			int? directions = stateJsonMetadata.Directions;
			int num;
			if (directions.HasValue)
			{
				int valueOrDefault = directions.GetValueOrDefault();
				num = valueOrDefault;
				if ((valueOrDefault != 1 && valueOrDefault != 4 && valueOrDefault != 8) || 1 == 0)
				{
					throw new RSILoadException($"Invalid direction for state '{name}': {num}. Expected 1, 4 or 8");
				}
			}
			else
			{
				num = 1;
			}
			float[][] array2;
			if (stateJsonMetadata.Delays != null)
			{
				array2 = stateJsonMetadata.Delays;
				if (array2.Length != num)
				{
					throw new RSILoadException($"Direction frames list count ({num}) does not match amount of delays specified ({array2.Length}) for state '{name}'.");
				}
				for (int j = 0; j < array2.Length; j++)
				{
					if (array2[j].Length == 0)
					{
						array2[j] = OneArray;
					}
				}
			}
			else
			{
				array2 = new float[num][];
				for (int k = 0; k < num; k++)
				{
					array2[k] = OneArray;
				}
			}
			array[i] = new StateMetadata(name, num, array2);
		}
		TextureLoadParameters loadParameters = TextureLoadParameters.Default;
		RsiJsonLoad load = manifestJson.Load;
		if ((object)load != null)
		{
			loadParameters = new TextureLoadParameters
			{
				SampleParameters = TextureSampleParameters.Default,
				Srgb = load.Srgb
			};
		}
		for (int l = 0; l < array.Length; l++)
		{
			string stateId = array[l].StateId;
			for (int m = l + 1; m < array.Length; m++)
			{
				if (stateId == array[m].StateId)
				{
					throw new RSILoadException("RSI has a duplicate stateId '" + stateId + "'.");
				}
			}
		}
		return new RsiMetadata(size, array, loadParameters, manifestJson.MetaAtlas, manifestJson.Rsic);
	}

	internal static int[] CalculateFrameCounts(RsiMetadata metadata)
	{
		int[] array = new int[metadata.States.Length];
		for (int i = 0; i < metadata.States.Length; i++)
		{
			StateMetadata stateMetadata = metadata.States[i];
			array[i] = stateMetadata.Delays.Sum((float[] delayList) => delayList.Length);
		}
		return array;
	}

	internal static Image<Rgba32>[] LoadImages(RsiMetadata metadata, Configuration configuration, Func<string, Stream> openStream)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		Image<Rgba32>[] array = new Image<Rgba32>[metadata.States.Length];
		DecoderOptions val = new DecoderOptions();
		val.set_Configuration(configuration);
		DecoderOptions val2 = val;
		Vector2i size = metadata.Size;
		try
		{
			for (int i = 0; i < metadata.States.Length; i++)
			{
				StateMetadata stateMetadata = metadata.States[i];
				using Stream stream = openStream(stateMetadata.StateId);
				Image<Rgba32> val3 = (array[i] = Image.Load<Rgba32>(val2, stream));
				if (((Image)val3).Width % size.X != 0 || ((Image)val3).Height % size.Y != 0)
				{
					string value = $"{((Image)val3).Width}x{((Image)val3).Height}";
					string value2 = $"{size.X}x{size.Y}";
					throw new RSILoadException($"State '{stateMetadata.StateId}' image size ({value}) is not a multiple of the icon size ({value2}).");
				}
			}
			return array;
		}
		catch
		{
			Image<Rgba32>[] array2 = array;
			foreach (Image<Rgba32> obj2 in array2)
			{
				if (obj2 != null)
				{
					((Image)obj2).Dispose();
				}
			}
			throw;
		}
	}

	internal static Image<Rgba32> GenerateAtlas(RsiMetadata metadata, int[] frameCounts, Image<Rgba32>[] images, Configuration configuration, out int dimX)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		Vector2i size = metadata.Size;
		int num = frameCounts.Sum();
		int num2 = (int)MathF.Ceiling(MathF.Sqrt(num));
		int num3 = (int)MathF.Ceiling((float)num / (float)num2);
		dimX = num2;
		Image<Rgba32> val = new Image<Rgba32>(configuration, num2 * size.X, num3 * size.Y);
		try
		{
			int num4 = 0;
			for (int i = 0; i < frameCounts.Length; i++)
			{
				int num5 = frameCounts[i];
				Image<Rgba32> val2 = images[i];
				for (int j = 0; j < num5; j++)
				{
					int num6 = ((Image)val2).Width / size.X;
					int num7 = j % num6;
					int num8 = j / num6;
					(int, int) valueTuple = (num7 * size.X, num8 * size.Y);
					int num9 = (num4 + j) % num2;
					(int, int) tuple = new ValueTuple<int, int>(item2: (num4 + j) / num2 * size.Y, item1: num9 * size.X);
					UIBox2i sourceRect = UIBox2i.FromDimensions(Vector2i.op_Implicit(valueTuple), size);
					ImageOps.Blit<Rgba32>(val2, sourceRect, val, Vector2i.op_Implicit(tuple));
				}
				num4 += num5;
			}
			return val;
		}
		catch
		{
			((Image)val).Dispose();
			throw;
		}
	}

	public static void Warmup()
	{
		JsonSerializer.Deserialize<RsiJsonMetadata>("{\"version\":1,\"license\":\"CC-BY-SA-3.0\",\"copyright\":\"Space Wizards Federation\",\"size\":{\"x\":32,\"y\":32},\"states\":[{\"name\":\"mono\"}]}", SerializerOptions);
	}
}
