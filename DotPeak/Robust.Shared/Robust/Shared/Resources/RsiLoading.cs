// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Resources.RsiLoading
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Graphics;
using Robust.Shared.Maths;
using Robust.Shared.Utility;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

#nullable enable
namespace Robust.Shared.Resources;

internal static class RsiLoading
{
  private static readonly float[] OneArray = new float[1]
  {
    1f
  };
  private static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
  public const uint MINIMUM_RSI_VERSION = 1;
  public const uint MAXIMUM_RSI_VERSION = 1;
  internal const string RsicPngField = "robusttoolbox_rsic_meta";

  internal static RsiLoading.RsiMetadata LoadRsiMetadata(string metadata)
  {
    RsiLoading.RsiJsonMetadata manifestJson = JsonSerializer.Deserialize<RsiLoading.RsiJsonMetadata>(metadata, RsiLoading.SerializerOptions);
    return !(manifestJson == (RsiLoading.RsiJsonMetadata) null) ? RsiLoading.LoadRsiMetadataCore(manifestJson) : throw new RSILoadException("Manifest JSON failed to deserialize!");
  }

  internal static RsiLoading.RsiMetadata LoadRsiMetadata(Stream manifestFile)
  {
    RsiLoading.RsiJsonMetadata manifestJson = JsonSerializer.Deserialize<RsiLoading.RsiJsonMetadata>(manifestFile, RsiLoading.SerializerOptions);
    return !(manifestJson == (RsiLoading.RsiJsonMetadata) null) ? RsiLoading.LoadRsiMetadataCore(manifestJson) : throw new RSILoadException("Manifest JSON failed to deserialize!");
  }

  private static RsiLoading.RsiMetadata LoadRsiMetadataCore(RsiLoading.RsiJsonMetadata manifestJson)
  {
    Vector2i size = manifestJson.Size;
    RsiLoading.StateMetadata[] states = new RsiLoading.StateMetadata[manifestJson.States.Length];
    for (int index1 = 0; index1 < manifestJson.States.Length; ++index1)
    {
      RsiLoading.StateJsonMetadata state = manifestJson.States[index1];
      string name = state.Name;
      int? directions = state.Directions;
      int dirCount;
      if (directions.HasValue)
      {
        int valueOrDefault = directions.GetValueOrDefault();
        dirCount = valueOrDefault;
        if (valueOrDefault != 1 && valueOrDefault != 4 && valueOrDefault != 8)
          throw new RSILoadException($"Invalid direction for state '{name}': {dirCount}. Expected 1, 4 or 8");
      }
      else
        dirCount = 1;
      float[][] delays;
      if (state.Delays != null)
      {
        delays = state.Delays;
        if (delays.Length != dirCount)
          throw new RSILoadException($"Direction frames list count ({dirCount}) does not match amount of delays specified ({delays.Length}) for state '{name}'.");
        for (int index2 = 0; index2 < delays.Length; ++index2)
        {
          if (delays[index2].Length == 0)
            delays[index2] = RsiLoading.OneArray;
        }
      }
      else
      {
        delays = new float[dirCount][];
        for (int index3 = 0; index3 < dirCount; ++index3)
          delays[index3] = RsiLoading.OneArray;
      }
      states[index1] = new RsiLoading.StateMetadata(name, dirCount, delays);
    }
    TextureLoadParameters loadParameters = TextureLoadParameters.Default;
    RsiLoading.RsiJsonLoad load = manifestJson.Load;
    if ((object) load != null)
      loadParameters = new TextureLoadParameters()
      {
        SampleParameters = TextureSampleParameters.Default,
        Srgb = load.Srgb
      };
    for (int index4 = 0; index4 < states.Length; ++index4)
    {
      string stateId = states[index4].StateId;
      for (int index5 = index4 + 1; index5 < states.Length; ++index5)
      {
        if (stateId == states[index5].StateId)
          throw new RSILoadException($"RSI has a duplicate stateId '{stateId}'.");
      }
    }
    return new RsiLoading.RsiMetadata(size, states, loadParameters, manifestJson.MetaAtlas, manifestJson.Rsic);
  }

  internal static int[] CalculateFrameCounts(RsiLoading.RsiMetadata metadata)
  {
    int[] frameCounts = new int[metadata.States.Length];
    for (int index = 0; index < metadata.States.Length; ++index)
    {
      RsiLoading.StateMetadata state = metadata.States[index];
      frameCounts[index] = ((IEnumerable<float[]>) state.Delays).Sum<float[]>((Func<float[], int>) (delayList => delayList.Length));
    }
    return frameCounts;
  }

  internal static Image<Rgba32>[] LoadImages(
    RsiLoading.RsiMetadata metadata,
    Configuration configuration,
    Func<string, Stream> openStream)
  {
    Image<Rgba32>[] imageArray = new Image<Rgba32>[metadata.States.Length];
    DecoderOptions decoderOptions = new DecoderOptions()
    {
      Configuration = configuration
    };
    Vector2i size = metadata.Size;
    try
    {
      for (int index = 0; index < metadata.States.Length; ++index)
      {
        RsiLoading.StateMetadata state = metadata.States[index];
        using (Stream stream = openStream(state.StateId))
        {
          Image<Rgba32> image = Image.Load<Rgba32>(decoderOptions, stream);
          imageArray[index] = image;
          if (((Image) image).Width % size.X == 0)
          {
            if (((Image) image).Height % size.Y == 0)
              continue;
          }
          string str1 = $"{((Image) image).Width}x{((Image) image).Height}";
          string str2 = $"{size.X}x{size.Y}";
          throw new RSILoadException($"State '{state.StateId}' image size ({str1}) is not a multiple of the icon size ({str2}).");
        }
      }
      return imageArray;
    }
    catch
    {
      foreach (Image<Rgba32> image in imageArray)
        ((Image) image)?.Dispose();
      throw;
    }
  }

  internal static Image<Rgba32> GenerateAtlas(
    RsiLoading.RsiMetadata metadata,
    int[] frameCounts,
    Image<Rgba32>[] images,
    Configuration configuration,
    out int dimX)
  {
    Vector2i size = metadata.Size;
    int num1;
    int num2 = (int) MathF.Ceiling(MathF.Sqrt((float) (num1 = ((IEnumerable<int>) frameCounts).Sum())));
    int num3 = (int) MathF.Ceiling((float) num1 / (float) num2);
    dimX = num2;
    Image<Rgba32> destination = new Image<Rgba32>(configuration, num2 * size.X, num3 * size.Y);
    try
    {
      int num4 = 0;
      for (int index1 = 0; index1 < frameCounts.Length; ++index1)
      {
        int frameCount = frameCounts[index1];
        Image<Rgba32> image = images[index1];
        for (int index2 = 0; index2 < frameCount; ++index2)
        {
          int num5 = ((Image) image).Width / size.X;
          int num6 = index2 % num5;
          int num7 = index2 / num5;
          (int, int) valueTuple1 = (num6 * size.X, num7 * size.Y);
          int num8 = (num4 + index2) % num2;
          int num9 = (num4 + index2) / num2;
          (int, int) valueTuple2 = (num8 * size.X, num9 * size.Y);
          UIBox2i sourceRect = UIBox2i.FromDimensions(Vector2i.op_Implicit(valueTuple1), size);
          ImageOps.Blit<Rgba32>(image, sourceRect, destination, Vector2i.op_Implicit(valueTuple2));
        }
        num4 += frameCount;
      }
    }
    catch
    {
      ((Image) destination).Dispose();
      throw;
    }
    return destination;
  }

  public static void Warmup()
  {
    JsonSerializer.Deserialize<RsiLoading.RsiJsonMetadata>("{\"version\":1,\"license\":\"CC-BY-SA-3.0\",\"copyright\":\"Space Wizards Federation\",\"size\":{\"x\":32,\"y\":32},\"states\":[{\"name\":\"mono\"}]}", RsiLoading.SerializerOptions);
  }

  internal sealed class RsiMetadata(
    Vector2i size,
    RsiLoading.StateMetadata[] states,
    TextureLoadParameters loadParameters,
    bool metaAtlas,
    bool rsic)
  {
    public readonly Vector2i Size = size;
    public readonly RsiLoading.StateMetadata[] States = states;
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
      this.StateId = stateId;
      this.DirCount = dirCount;
      this.Delays = delays;
    }
  }

  private sealed record RsiJsonMetadata(
    Vector2i Size,
    RsiLoading.StateJsonMetadata[] States,
    RsiLoading.RsiJsonLoad? Load,
    bool MetaAtlas = true,
    bool Rsic = true)
  ;

  private sealed record StateJsonMetadata(string Name, int? Directions, float[][]? Delays);

  private sealed record RsiJsonLoad(bool Srgb = true);
}
