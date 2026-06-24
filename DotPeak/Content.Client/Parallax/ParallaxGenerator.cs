// Decompiled with JetBrains decompiler
// Type: Content.Client.Parallax.ParallaxGenerator
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Nett;
using Robust.Client.Utility;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Noise;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

#nullable enable
namespace Content.Client.Parallax;

public sealed class ParallaxGenerator
{
  private readonly List<ParallaxGenerator.Layer> Layers = new List<ParallaxGenerator.Layer>();

  public static Image<Rgba32> GenerateParallax(
    TomlTable config,
    Size size,
    ISawmill sawmill,
    List<Image<Rgba32>>? debugLayerDump,
    CancellationToken cancel = default (CancellationToken))
  {
    sawmill.Debug("Generating parallax!");
    ParallaxGenerator parallaxGenerator = new ParallaxGenerator();
    parallaxGenerator._loadConfig(config);
    sawmill.Debug("Timing start!");
    Stopwatch stopwatch = new Stopwatch();
    stopwatch.Start();
    Image<Rgba32> parallax = new Image<Rgba32>(Configuration.Default, ((Size) ref size).Width, ((Size) ref size).Height, new Rgba32((byte) 0, (byte) 0, (byte) 0, (byte) 0));
    int num = 0;
    foreach (ParallaxGenerator.Layer layer in parallaxGenerator.Layers)
    {
      cancel.ThrowIfCancellationRequested();
      Image<Rgba32> bitmap = parallax;
      layer.Apply(bitmap);
      debugLayerDump?.Add(parallax.Clone());
      sawmill.Debug("Layer {0} done!", new object[1]
      {
        (object) num++
      });
    }
    stopwatch.Stop();
    sawmill.Debug("Total time: {0}", new object[1]
    {
      (object) stopwatch.Elapsed.TotalSeconds
    });
    return parallax;
  }

  private void _loadConfig(TomlTable config)
  {
    foreach (TomlTable table in ((TomlTableArray) config.Get("layers")).Items)
    {
      switch (((TomlValue<string>) table.Get("type")).Value)
      {
        case "clear":
          this.Layers.Add((ParallaxGenerator.Layer) new ParallaxGenerator.LayerClear(table));
          continue;
        case "toalpha":
          this.Layers.Add((ParallaxGenerator.Layer) new ParallaxGenerator.LayerToAlpha(table));
          continue;
        case "noise":
          this.Layers.Add((ParallaxGenerator.Layer) new ParallaxGenerator.LayerNoise(table));
          continue;
        case "points":
          this.Layers.Add((ParallaxGenerator.Layer) new ParallaxGenerator.LayerPoints(table));
          continue;
        default:
          throw new NotSupportedException();
      }
    }
  }

  private abstract class Layer
  {
    public abstract void Apply(Image<Rgba32> bitmap);
  }

  private abstract class LayerConversion : ParallaxGenerator.Layer
  {
    public abstract Color ConvertColor(Color input);

    public override void Apply(Image<Rgba32> bitmap)
    {
      Span<Rgba32> pixelSpan = ImageSharpExt.GetPixelSpan<Rgba32>(bitmap);
      for (int index1 = 0; index1 < ((Image) bitmap).Height; ++index1)
      {
        for (int index2 = 0; index2 < ((Image) bitmap).Width; ++index2)
        {
          int index3 = index1 * ((Image) bitmap).Width + index2;
          pixelSpan[index3] = ImageSharpExt.ConvertImgSharp(this.ConvertColor(ImageSharpExt.ConvertImgSharp(pixelSpan[index3])));
        }
      }
    }
  }

  private sealed class LayerClear : ParallaxGenerator.LayerConversion
  {
    private readonly Color Color = Color.Black;

    public LayerClear(TomlTable table)
    {
      TomlObject tomlObject;
      if (!table.TryGetValue("color", ref tomlObject))
        return;
      this.Color = Color.FromHex((ReadOnlySpan<char>) ((TomlValue<string>) tomlObject).Value, new Color?());
    }

    public override Color ConvertColor(Color input) => this.Color;
  }

  private sealed class LayerToAlpha : ParallaxGenerator.LayerConversion
  {
    public LayerToAlpha(TomlTable table)
    {
    }

    public override Color ConvertColor(Color input)
    {
      return new Color(input.R, input.G, input.B, MathF.Min(input.R + input.G + input.B, 1f));
    }
  }

  private sealed class LayerNoise : ParallaxGenerator.Layer
  {
    private readonly Color InnerColor = Color.White;
    private readonly Color OuterColor = Color.Black;
    private readonly FastNoiseLite.FractalType NoiseType = (FastNoiseLite.FractalType) 1;
    private readonly uint Seed = 1234;
    private readonly float Persistence = 0.5f;
    private readonly float Lacunarity = 1.04719758f;
    private readonly float Frequency = 1f;
    private readonly uint Octaves = 3;
    private readonly float Threshold;
    private readonly float Power = 1f;
    private readonly Color.BlendFactor SrcFactor = (Color.BlendFactor) 1;
    private readonly Color.BlendFactor DstFactor = (Color.BlendFactor) 1;

    public LayerNoise(TomlTable table)
    {
      TomlObject tomlObject;
      if (table.TryGetValue("innercolor", ref tomlObject))
        this.InnerColor = Color.FromHex((ReadOnlySpan<char>) ((TomlValue<string>) tomlObject).Value, new Color?());
      if (table.TryGetValue("outercolor", ref tomlObject))
        this.OuterColor = Color.FromHex((ReadOnlySpan<char>) ((TomlValue<string>) tomlObject).Value, new Color?());
      if (table.TryGetValue("seed", ref tomlObject))
        this.Seed = (uint) ((TomlValue<long>) tomlObject).Value;
      if (table.TryGetValue("persistence", ref tomlObject))
        this.Persistence = float.Parse(((TomlValue<string>) tomlObject).Value, (IFormatProvider) CultureInfo.InvariantCulture);
      if (table.TryGetValue("lacunarity", ref tomlObject))
        this.Lacunarity = float.Parse(((TomlValue<string>) tomlObject).Value, (IFormatProvider) CultureInfo.InvariantCulture);
      if (table.TryGetValue("frequency", ref tomlObject))
        this.Frequency = float.Parse(((TomlValue<string>) tomlObject).Value, (IFormatProvider) CultureInfo.InvariantCulture);
      if (table.TryGetValue("octaves", ref tomlObject))
        this.Octaves = (uint) ((TomlValue<long>) tomlObject).Value;
      if (table.TryGetValue("threshold", ref tomlObject))
        this.Threshold = float.Parse(((TomlValue<string>) tomlObject).Value, (IFormatProvider) CultureInfo.InvariantCulture);
      if (table.TryGetValue("sourcefactor", ref tomlObject))
        this.SrcFactor = (Color.BlendFactor) Enum.Parse(typeof (Color.BlendFactor), ((TomlValue<string>) tomlObject).Value);
      if (table.TryGetValue("destfactor", ref tomlObject))
        this.DstFactor = (Color.BlendFactor) Enum.Parse(typeof (Color.BlendFactor), ((TomlValue<string>) tomlObject).Value);
      if (table.TryGetValue("power", ref tomlObject))
        this.Power = float.Parse(((TomlValue<string>) tomlObject).Value, (IFormatProvider) CultureInfo.InvariantCulture);
      if (!table.TryGetValue("noise_type", ref tomlObject))
        return;
      switch (((TomlValue<string>) tomlObject).Value)
      {
        case "fbm":
          this.NoiseType = (FastNoiseLite.FractalType) 1;
          break;
        case "ridged":
          this.NoiseType = (FastNoiseLite.FractalType) 2;
          break;
        default:
          throw new InvalidOperationException();
      }
    }

    public override void Apply(Image<Rgba32> bitmap)
    {
      FastNoiseLite fastNoiseLite = new FastNoiseLite((int) this.Seed);
      fastNoiseLite.SetFractalType(this.NoiseType);
      fastNoiseLite.SetFrequency(this.Frequency);
      fastNoiseLite.SetFractalLacunarity(this.Lacunarity);
      fastNoiseLite.SetFractalOctaves((int) this.Octaves);
      float num1 = (float) (1.0 / (1.0 - (double) this.Threshold));
      float y = 1f / this.Power;
      Span<Rgba32> pixelSpan = ImageSharpExt.GetPixelSpan<Rgba32>(bitmap);
      for (int index1 = 0; index1 < ((Image) bitmap).Height; ++index1)
      {
        for (int index2 = 0; index2 < ((Image) bitmap).Width; ++index2)
        {
          float num2 = MathF.Pow(MathF.Max(0.0f, MathF.Min(1f, MathF.Max(0.0f, (float) (((double) fastNoiseLite.GetNoise((float) index2, (float) index1) + 1.0) / 2.0))) - this.Threshold) * num1, y);
          Color color1 = Color.InterpolateBetween(this.OuterColor, this.InnerColor, num2);
          Color color2 = ((Color) ref color1).WithAlpha(num2);
          int index3 = index1 * ((Image) bitmap).Width + index2;
          Color color3 = ImageSharpExt.ConvertImgSharp(pixelSpan[index3]);
          pixelSpan[index3] = ImageSharpExt.ConvertImgSharp(Color.Blend(color3, color2, this.DstFactor, this.SrcFactor));
        }
      }
    }
  }

  private sealed class LayerPoints : ParallaxGenerator.Layer
  {
    private readonly int Seed = 1234;
    private readonly int PointCount = 100;
    private readonly Color CloseColor = Color.White;
    private readonly Color FarColor = Color.Black;
    private readonly Color.BlendFactor SrcFactor = (Color.BlendFactor) 1;
    private readonly Color.BlendFactor DstFactor = (Color.BlendFactor) 1;
    private readonly bool Masked;
    private readonly FastNoiseLite.FractalType MaskNoiseType = (FastNoiseLite.FractalType) 1;
    private readonly uint MaskSeed = 1234;
    private readonly float MaskPersistence = 0.5f;
    private readonly float MaskLacunarity = 2.09439516f;
    private readonly float MaskFrequency = 1f;
    private readonly uint MaskOctaves = 3;
    private readonly float MaskThreshold;
    private readonly int PointSize = 1;
    private readonly float MaskPower = 1f;

    public LayerPoints(TomlTable table)
    {
      TomlObject tomlObject;
      if (table.TryGetValue("seed", ref tomlObject))
        this.Seed = (int) ((TomlValue<long>) tomlObject).Value;
      if (table.TryGetValue("count", ref tomlObject))
        this.PointCount = (int) ((TomlValue<long>) tomlObject).Value;
      if (table.TryGetValue("sourcefactor", ref tomlObject))
        this.SrcFactor = (Color.BlendFactor) Enum.Parse(typeof (Color.BlendFactor), ((TomlValue<string>) tomlObject).Value);
      if (table.TryGetValue("destfactor", ref tomlObject))
        this.DstFactor = (Color.BlendFactor) Enum.Parse(typeof (Color.BlendFactor), ((TomlValue<string>) tomlObject).Value);
      if (table.TryGetValue("farcolor", ref tomlObject))
        this.FarColor = Color.FromHex((ReadOnlySpan<char>) ((TomlValue<string>) tomlObject).Value, new Color?());
      if (table.TryGetValue("closecolor", ref tomlObject))
        this.CloseColor = Color.FromHex((ReadOnlySpan<char>) ((TomlValue<string>) tomlObject).Value, new Color?());
      if (table.TryGetValue("pointsize", ref tomlObject))
        this.PointSize = (int) ((TomlValue<long>) tomlObject).Value;
      if (table.TryGetValue("mask", ref tomlObject))
        this.Masked = ((TomlValue<bool>) tomlObject).Value;
      if (table.TryGetValue("maskseed", ref tomlObject))
        this.MaskSeed = (uint) ((TomlValue<long>) tomlObject).Value;
      if (table.TryGetValue("maskpersistence", ref tomlObject))
        this.MaskPersistence = float.Parse(((TomlValue<string>) tomlObject).Value, (IFormatProvider) CultureInfo.InvariantCulture);
      if (table.TryGetValue("masklacunarity", ref tomlObject))
        this.MaskLacunarity = float.Parse(((TomlValue<string>) tomlObject).Value, (IFormatProvider) CultureInfo.InvariantCulture);
      if (table.TryGetValue("maskfrequency", ref tomlObject))
        this.MaskFrequency = float.Parse(((TomlValue<string>) tomlObject).Value, (IFormatProvider) CultureInfo.InvariantCulture);
      if (table.TryGetValue("maskoctaves", ref tomlObject))
        this.MaskOctaves = (uint) ((TomlValue<long>) tomlObject).Value;
      if (table.TryGetValue("maskthreshold", ref tomlObject))
        this.MaskThreshold = float.Parse(((TomlValue<string>) tomlObject).Value, (IFormatProvider) CultureInfo.InvariantCulture);
      if (table.TryGetValue("masknoise_type", ref tomlObject))
      {
        switch (((TomlValue<string>) tomlObject).Value)
        {
          case "fbm":
            this.MaskNoiseType = (FastNoiseLite.FractalType) 1;
            break;
          case "ridged":
            this.MaskNoiseType = (FastNoiseLite.FractalType) 2;
            break;
          default:
            throw new InvalidOperationException();
        }
      }
      if (!table.TryGetValue("maskpower", ref tomlObject))
        return;
      this.MaskPower = float.Parse(((TomlValue<string>) tomlObject).Value, (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public override void Apply(Image<Rgba32> bitmap)
    {
      using (Image<Rgba32> buffer = new Image<Rgba32>(Configuration.Default, ((Image) bitmap).Width, ((Image) bitmap).Height, new Rgba32((byte) 0, (byte) 0, (byte) 0, (byte) 0)))
      {
        if (this.Masked)
          this.GenPointsMasked(buffer);
        else
          this.GenPoints(buffer);
        Span<Rgba32> pixelSpan1 = ImageSharpExt.GetPixelSpan<Rgba32>(buffer);
        Span<Rgba32> pixelSpan2 = ImageSharpExt.GetPixelSpan<Rgba32>(bitmap);
        int width = ((Image) bitmap).Width;
        int height = ((Image) bitmap).Height;
        for (int index1 = 0; index1 < height; ++index1)
        {
          for (int index2 = 0; index2 < width; ++index2)
          {
            int index3 = index1 * width + index2;
            Color color1 = ImageSharpExt.ConvertImgSharp(pixelSpan2[index3]);
            Color color2 = ImageSharpExt.ConvertImgSharp(pixelSpan1[index3]);
            pixelSpan2[index3] = ImageSharpExt.ConvertImgSharp(Color.Blend(color1, color2, this.DstFactor, this.SrcFactor));
          }
        }
      }
    }

    private void GenPoints(Image<Rgba32> buffer)
    {
      int num1 = this.PointSize - 1;
      Random random = new Random(this.Seed);
      Span<Rgba32> pixelSpan = ImageSharpExt.GetPixelSpan<Rgba32>(buffer);
      for (int index1 = 0; index1 < this.PointCount; ++index1)
      {
        int num2 = random.Next(0, ((Image) buffer).Width);
        int num3 = random.Next(0, ((Image) buffer).Height);
        float num4 = random.NextSingle();
        for (int index2 = num3 - num1; index2 <= num3 + num1; ++index2)
        {
          for (int index3 = num2 - num1; index3 <= num2 + num1; ++index3)
          {
            int num5 = MathHelper.Mod(index3, ((Image) buffer).Width);
            int num6 = MathHelper.Mod(index2, ((Image) buffer).Height);
            Rgba32 rgba32 = ImageSharpExt.ConvertImgSharp(Color.InterpolateBetween(this.FarColor, this.CloseColor, num4));
            pixelSpan[num6 * ((Image) buffer).Width + num5] = rgba32;
          }
        }
      }
    }

    private void GenPointsMasked(Image<Rgba32> buffer)
    {
      int num1 = this.PointSize - 1;
      Random random = new Random(this.Seed);
      FastNoiseLite fastNoiseLite = new FastNoiseLite((int) this.MaskSeed);
      fastNoiseLite.SetFractalType(this.MaskNoiseType);
      fastNoiseLite.SetFractalLacunarity(this.MaskLacunarity);
      fastNoiseLite.SetFractalOctaves((int) this.MaskOctaves);
      float num2 = (float) (1.0 / (1.0 - (double) this.MaskThreshold));
      float y = 1f / this.MaskPower;
      int num3 = 0;
      Span<Rgba32> pixelSpan = ImageSharpExt.GetPixelSpan<Rgba32>(buffer);
      for (int index1 = 0; index1 < this.PointCount; ++index1)
      {
        int num4 = random.Next(0, ((Image) buffer).Width);
        int num5 = random.Next(0, ((Image) buffer).Height);
        float num6 = MathF.Pow(MathF.Max(0.0f, MathF.Min(1f, MathF.Max(0.0f, (float) (((double) fastNoiseLite.GetNoise((float) num4, (float) num5) + 1.0) / 2.0))) - this.MaskThreshold) * num2, y);
        if ((double) random.NextSingle() > (double) num6)
        {
          if (++num3 <= 9999)
            --index1;
        }
        else
        {
          float num7 = random.NextSingle();
          for (int index2 = num5 - num1; index2 <= num5 + num1; ++index2)
          {
            for (int index3 = num4 - num1; index3 <= num4 + num1; ++index3)
            {
              int num8 = MathHelper.Mod(index3, ((Image) buffer).Width);
              int num9 = MathHelper.Mod(index2, ((Image) buffer).Height);
              Rgba32 rgba32 = ImageSharpExt.ConvertImgSharp(Color.InterpolateBetween(this.FarColor, this.CloseColor, num7));
              pixelSpan[num9 * ((Image) buffer).Width + num8] = rgba32;
            }
          }
        }
      }
    }
  }
}
