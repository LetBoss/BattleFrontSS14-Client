using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Nett;
using Robust.Client.Utility;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Noise;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Content.Client.Parallax;

public sealed class ParallaxGenerator
{
	private abstract class Layer
	{
		public abstract void Apply(Image<Rgba32> bitmap);
	}

	private abstract class LayerConversion : Layer
	{
		public abstract Color ConvertColor(Color input);

		public override void Apply(Image<Rgba32> bitmap)
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			Span<Rgba32> pixelSpan = ImageSharpExt.GetPixelSpan<Rgba32>(bitmap);
			for (int i = 0; i < ((Image)bitmap).Height; i++)
			{
				for (int j = 0; j < ((Image)bitmap).Width; j++)
				{
					int index = i * ((Image)bitmap).Width + j;
					pixelSpan[index] = ImageSharpExt.ConvertImgSharp(ConvertColor(ImageSharpExt.ConvertImgSharp(pixelSpan[index])));
				}
			}
		}
	}

	private sealed class LayerClear : LayerConversion
	{
		private readonly Color Color = Color.Black;

		public LayerClear(TomlTable table)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			TomlObject val = default(TomlObject);
			if (table.TryGetValue("color", ref val))
			{
				Color = Color.FromHex((ReadOnlySpan<char>)((TomlValue<string>)(object)val).Value, (Color?)null);
			}
		}

		public override Color ConvertColor(Color input)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return Color;
		}
	}

	private sealed class LayerToAlpha : LayerConversion
	{
		public LayerToAlpha(TomlTable table)
		{
		}

		public override Color ConvertColor(Color input)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			return new Color(input.R, input.G, input.B, MathF.Min(input.R + input.G + input.B, 1f));
		}
	}

	private sealed class LayerNoise : Layer
	{
		private readonly Color InnerColor = Color.White;

		private readonly Color OuterColor = Color.Black;

		private readonly FractalType NoiseType = (FractalType)1;

		private readonly uint Seed = 1234u;

		private readonly float Persistence = 0.5f;

		private readonly float Lacunarity = MathF.PI / 3f;

		private readonly float Frequency = 1f;

		private readonly uint Octaves = 3u;

		private readonly float Threshold;

		private readonly float Power = 1f;

		private readonly BlendFactor SrcFactor = (BlendFactor)1;

		private readonly BlendFactor DstFactor = (BlendFactor)1;

		public LayerNoise(TomlTable table)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			TomlObject val = default(TomlObject);
			if (table.TryGetValue("innercolor", ref val))
			{
				InnerColor = Color.FromHex((ReadOnlySpan<char>)((TomlValue<string>)(object)val).Value, (Color?)null);
			}
			if (table.TryGetValue("outercolor", ref val))
			{
				OuterColor = Color.FromHex((ReadOnlySpan<char>)((TomlValue<string>)(object)val).Value, (Color?)null);
			}
			if (table.TryGetValue("seed", ref val))
			{
				Seed = (uint)((TomlValue<long>)(object)val).Value;
			}
			if (table.TryGetValue("persistence", ref val))
			{
				Persistence = float.Parse(((TomlValue<string>)(object)val).Value, CultureInfo.InvariantCulture);
			}
			if (table.TryGetValue("lacunarity", ref val))
			{
				Lacunarity = float.Parse(((TomlValue<string>)(object)val).Value, CultureInfo.InvariantCulture);
			}
			if (table.TryGetValue("frequency", ref val))
			{
				Frequency = float.Parse(((TomlValue<string>)(object)val).Value, CultureInfo.InvariantCulture);
			}
			if (table.TryGetValue("octaves", ref val))
			{
				Octaves = (uint)((TomlValue<long>)(object)val).Value;
			}
			if (table.TryGetValue("threshold", ref val))
			{
				Threshold = float.Parse(((TomlValue<string>)(object)val).Value, CultureInfo.InvariantCulture);
			}
			if (table.TryGetValue("sourcefactor", ref val))
			{
				SrcFactor = (BlendFactor)Enum.Parse(typeof(BlendFactor), ((TomlValue<string>)(object)val).Value);
			}
			if (table.TryGetValue("destfactor", ref val))
			{
				DstFactor = (BlendFactor)Enum.Parse(typeof(BlendFactor), ((TomlValue<string>)(object)val).Value);
			}
			if (table.TryGetValue("power", ref val))
			{
				Power = float.Parse(((TomlValue<string>)(object)val).Value, CultureInfo.InvariantCulture);
			}
			if (!table.TryGetValue("noise_type", ref val))
			{
				return;
			}
			string value = ((TomlValue<string>)(object)val).Value;
			if (!(value == "fbm"))
			{
				if (!(value == "ridged"))
				{
					throw new InvalidOperationException();
				}
				NoiseType = (FractalType)2;
			}
			else
			{
				NoiseType = (FractalType)1;
			}
		}

		public override void Apply(Image<Rgba32> bitmap)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			FastNoiseLite val = new FastNoiseLite((int)Seed);
			val.SetFractalType(NoiseType);
			val.SetFrequency(Frequency);
			val.SetFractalLacunarity(Lacunarity);
			val.SetFractalOctaves((int)Octaves);
			float num = 1f / (1f - Threshold);
			float y = 1f / Power;
			Span<Rgba32> pixelSpan = ImageSharpExt.GetPixelSpan<Rgba32>(bitmap);
			for (int i = 0; i < ((Image)bitmap).Height; i++)
			{
				for (int j = 0; j < ((Image)bitmap).Width; j++)
				{
					float num2 = MathF.Min(1f, MathF.Max(0f, (val.GetNoise((float)j, (float)i) + 1f) / 2f));
					num2 = MathF.Max(0f, num2 - Threshold);
					num2 *= num;
					num2 = MathF.Pow(num2, y);
					Color val2 = Color.InterpolateBetween(OuterColor, InnerColor, num2);
					Color val3 = ((Color)(ref val2)).WithAlpha(num2);
					int index = i * ((Image)bitmap).Width + j;
					Color val4 = ImageSharpExt.ConvertImgSharp(pixelSpan[index]);
					pixelSpan[index] = ImageSharpExt.ConvertImgSharp(Color.Blend(val4, val3, DstFactor, SrcFactor));
				}
			}
		}
	}

	private sealed class LayerPoints : Layer
	{
		private readonly int Seed = 1234;

		private readonly int PointCount = 100;

		private readonly Color CloseColor = Color.White;

		private readonly Color FarColor = Color.Black;

		private readonly BlendFactor SrcFactor = (BlendFactor)1;

		private readonly BlendFactor DstFactor = (BlendFactor)1;

		private readonly bool Masked;

		private readonly FractalType MaskNoiseType = (FractalType)1;

		private readonly uint MaskSeed = 1234u;

		private readonly float MaskPersistence = 0.5f;

		private readonly float MaskLacunarity = MathF.PI * 2f / 3f;

		private readonly float MaskFrequency = 1f;

		private readonly uint MaskOctaves = 3u;

		private readonly float MaskThreshold;

		private readonly int PointSize = 1;

		private readonly float MaskPower = 1f;

		public LayerPoints(TomlTable table)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0306: Unknown result type (might be due to invalid IL or missing references)
			TomlObject val = default(TomlObject);
			if (table.TryGetValue("seed", ref val))
			{
				Seed = (int)((TomlValue<long>)(object)val).Value;
			}
			if (table.TryGetValue("count", ref val))
			{
				PointCount = (int)((TomlValue<long>)(object)val).Value;
			}
			if (table.TryGetValue("sourcefactor", ref val))
			{
				SrcFactor = (BlendFactor)Enum.Parse(typeof(BlendFactor), ((TomlValue<string>)(object)val).Value);
			}
			if (table.TryGetValue("destfactor", ref val))
			{
				DstFactor = (BlendFactor)Enum.Parse(typeof(BlendFactor), ((TomlValue<string>)(object)val).Value);
			}
			if (table.TryGetValue("farcolor", ref val))
			{
				FarColor = Color.FromHex((ReadOnlySpan<char>)((TomlValue<string>)(object)val).Value, (Color?)null);
			}
			if (table.TryGetValue("closecolor", ref val))
			{
				CloseColor = Color.FromHex((ReadOnlySpan<char>)((TomlValue<string>)(object)val).Value, (Color?)null);
			}
			if (table.TryGetValue("pointsize", ref val))
			{
				PointSize = (int)((TomlValue<long>)(object)val).Value;
			}
			if (table.TryGetValue("mask", ref val))
			{
				Masked = ((TomlValue<bool>)(object)val).Value;
			}
			if (table.TryGetValue("maskseed", ref val))
			{
				MaskSeed = (uint)((TomlValue<long>)(object)val).Value;
			}
			if (table.TryGetValue("maskpersistence", ref val))
			{
				MaskPersistence = float.Parse(((TomlValue<string>)(object)val).Value, CultureInfo.InvariantCulture);
			}
			if (table.TryGetValue("masklacunarity", ref val))
			{
				MaskLacunarity = float.Parse(((TomlValue<string>)(object)val).Value, CultureInfo.InvariantCulture);
			}
			if (table.TryGetValue("maskfrequency", ref val))
			{
				MaskFrequency = float.Parse(((TomlValue<string>)(object)val).Value, CultureInfo.InvariantCulture);
			}
			if (table.TryGetValue("maskoctaves", ref val))
			{
				MaskOctaves = (uint)((TomlValue<long>)(object)val).Value;
			}
			if (table.TryGetValue("maskthreshold", ref val))
			{
				MaskThreshold = float.Parse(((TomlValue<string>)(object)val).Value, CultureInfo.InvariantCulture);
			}
			if (table.TryGetValue("masknoise_type", ref val))
			{
				string value = ((TomlValue<string>)(object)val).Value;
				if (!(value == "fbm"))
				{
					if (!(value == "ridged"))
					{
						throw new InvalidOperationException();
					}
					MaskNoiseType = (FractalType)2;
				}
				else
				{
					MaskNoiseType = (FractalType)1;
				}
			}
			if (table.TryGetValue("maskpower", ref val))
			{
				MaskPower = float.Parse(((TomlValue<string>)(object)val).Value, CultureInfo.InvariantCulture);
			}
		}

		public override void Apply(Image<Rgba32> bitmap)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			Image<Rgba32> val = new Image<Rgba32>(Configuration.Default, ((Image)bitmap).Width, ((Image)bitmap).Height, new Rgba32((byte)0, (byte)0, (byte)0, (byte)0));
			try
			{
				if (Masked)
				{
					GenPointsMasked(val);
				}
				else
				{
					GenPoints(val);
				}
				Span<Rgba32> pixelSpan = ImageSharpExt.GetPixelSpan<Rgba32>(val);
				Span<Rgba32> pixelSpan2 = ImageSharpExt.GetPixelSpan<Rgba32>(bitmap);
				int width = ((Image)bitmap).Width;
				int height = ((Image)bitmap).Height;
				for (int i = 0; i < height; i++)
				{
					for (int j = 0; j < width; j++)
					{
						int index = i * width + j;
						Color val2 = ImageSharpExt.ConvertImgSharp(pixelSpan2[index]);
						Color val3 = ImageSharpExt.ConvertImgSharp(pixelSpan[index]);
						pixelSpan2[index] = ImageSharpExt.ConvertImgSharp(Color.Blend(val2, val3, DstFactor, SrcFactor));
					}
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}

		private void GenPoints(Image<Rgba32> buffer)
		{
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			int num = PointSize - 1;
			Random random = new Random(Seed);
			Span<Rgba32> pixelSpan = ImageSharpExt.GetPixelSpan<Rgba32>(buffer);
			for (int i = 0; i < PointCount; i++)
			{
				int num2 = random.Next(0, ((Image)buffer).Width);
				int num3 = random.Next(0, ((Image)buffer).Height);
				float num4 = random.NextSingle();
				for (int j = num3 - num; j <= num3 + num; j++)
				{
					for (int k = num2 - num; k <= num2 + num; k++)
					{
						int num5 = MathHelper.Mod(k, ((Image)buffer).Width);
						int num6 = MathHelper.Mod(j, ((Image)buffer).Height);
						Rgba32 val = ImageSharpExt.ConvertImgSharp(Color.InterpolateBetween(FarColor, CloseColor, num4));
						pixelSpan[num6 * ((Image)buffer).Width + num5] = val;
					}
				}
			}
		}

		private void GenPointsMasked(Image<Rgba32> buffer)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Expected O, but got Unknown
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			int num = PointSize - 1;
			Random random = new Random(Seed);
			FastNoiseLite val = new FastNoiseLite((int)MaskSeed);
			val.SetFractalType(MaskNoiseType);
			val.SetFractalLacunarity(MaskLacunarity);
			val.SetFractalOctaves((int)MaskOctaves);
			float num2 = 1f / (1f - MaskThreshold);
			float y = 1f / MaskPower;
			int num3 = 0;
			Span<Rgba32> pixelSpan = ImageSharpExt.GetPixelSpan<Rgba32>(buffer);
			for (int i = 0; i < PointCount; i++)
			{
				int num4 = random.Next(0, ((Image)buffer).Width);
				int num5 = random.Next(0, ((Image)buffer).Height);
				float num6 = MathF.Min(1f, MathF.Max(0f, (val.GetNoise((float)num4, (float)num5) + 1f) / 2f));
				num6 = MathF.Max(0f, num6 - MaskThreshold);
				num6 *= num2;
				num6 = MathF.Pow(num6, y);
				if (random.NextSingle() > num6)
				{
					if (++num3 <= 9999)
					{
						i--;
					}
					continue;
				}
				float num7 = random.NextSingle();
				for (int j = num5 - num; j <= num5 + num; j++)
				{
					for (int k = num4 - num; k <= num4 + num; k++)
					{
						int num8 = MathHelper.Mod(k, ((Image)buffer).Width);
						int num9 = MathHelper.Mod(j, ((Image)buffer).Height);
						Rgba32 val2 = ImageSharpExt.ConvertImgSharp(Color.InterpolateBetween(FarColor, CloseColor, num7));
						pixelSpan[num9 * ((Image)buffer).Width + num8] = val2;
					}
				}
			}
		}
	}

	private readonly List<Layer> Layers = new List<Layer>();

	public static Image<Rgba32> GenerateParallax(TomlTable config, Size size, ISawmill sawmill, List<Image<Rgba32>>? debugLayerDump, CancellationToken cancel = default(CancellationToken))
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		sawmill.Debug("Generating parallax!");
		ParallaxGenerator parallaxGenerator = new ParallaxGenerator();
		parallaxGenerator._loadConfig(config);
		sawmill.Debug("Timing start!");
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		Image<Rgba32> val = new Image<Rgba32>(Configuration.Default, ((Size)(ref size)).Width, ((Size)(ref size)).Height, new Rgba32((byte)0, (byte)0, (byte)0, (byte)0));
		int num = 0;
		foreach (Layer layer in parallaxGenerator.Layers)
		{
			cancel.ThrowIfCancellationRequested();
			layer.Apply(val);
			debugLayerDump?.Add(val.Clone());
			sawmill.Debug("Layer {0} done!", new object[1] { num++ });
		}
		stopwatch.Stop();
		sawmill.Debug("Total time: {0}", new object[1] { stopwatch.Elapsed.TotalSeconds });
		return val;
	}

	private void _loadConfig(TomlTable config)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		foreach (TomlTable item5 in ((TomlTableArray)config.Get("layers")).Items)
		{
			switch (((TomlValue<string>)(object)item5.Get("type")).Value)
			{
			case "clear":
			{
				LayerClear item4 = new LayerClear(item5);
				Layers.Add(item4);
				break;
			}
			case "toalpha":
			{
				LayerToAlpha item3 = new LayerToAlpha(item5);
				Layers.Add(item3);
				break;
			}
			case "noise":
			{
				LayerNoise item2 = new LayerNoise(item5);
				Layers.Add(item2);
				break;
			}
			case "points":
			{
				LayerPoints item = new LayerPoints(item5);
				Layers.Add(item);
				break;
			}
			default:
				throw new NotSupportedException();
			}
		}
	}
}
