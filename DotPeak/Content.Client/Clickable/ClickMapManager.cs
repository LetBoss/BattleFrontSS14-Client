// Decompiled with JetBrains decompiler
// Type: Content.Client.Clickable.ClickMapManager
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.Utility;
using Robust.Shared.Graphics.RSI;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Text;

#nullable enable
namespace Content.Client.Clickable;

internal sealed class ClickMapManager : IClickMapManager, IPostInjectInit
{
  private static readonly string[] IgnoreTexturePaths = new string[4]
  {
    "/Textures/Interface",
    "/Textures/LobbyScreens",
    "/Textures/Parallaxes",
    "/Textures/Logo"
  };
  private const float Threshold = 0.1f;
  private const int ClickRadius = 2;
  [Dependency]
  private IResourceCache _resourceCache;
  [Robust.Shared.ViewVariables.ViewVariables]
  private readonly Dictionary<Texture, ClickMapManager.ClickMap> _textureMaps = new Dictionary<Texture, ClickMapManager.ClickMap>();
  [Robust.Shared.ViewVariables.ViewVariables]
  private readonly Dictionary<Robust.Client.Graphics.RSI, ClickMapManager.RsiClickMapData> _rsiMaps = new Dictionary<Robust.Client.Graphics.RSI, ClickMapManager.RsiClickMapData>();

  public void PostInject()
  {
    this._resourceCache.OnRawTextureLoaded += new Action<TextureLoadedEventArgs>(this.OnRawTextureLoaded);
    this._resourceCache.OnRsiLoaded += new Action<RsiLoadedEventArgs>(this.OnOnRsiLoaded);
  }

  private void OnOnRsiLoaded(RsiLoadedEventArgs obj)
  {
    if (!(((RsiLoadedEventArgs) ref obj).Atlas is Image<Rgba32> atlas))
      return;
    ClickMapManager.RsiClickMapData rsiClickMapData = new ClickMapManager.RsiClickMapData(ClickMapManager.ClickMap.FromImage<Rgba32>(atlas, 0.1f), ((RsiLoadedEventArgs) ref obj).AtlasOffsets);
    this._rsiMaps[((RsiLoadedEventArgs) ref obj).Resource.RSI] = rsiClickMapData;
  }

  private void OnRawTextureLoaded(TextureLoadedEventArgs obj)
  {
    if (!(((TextureLoadedEventArgs) ref obj).Image is Image<Rgba32> image))
      return;
    string str = ((TextureLoadedEventArgs) ref obj).Path.ToString();
    foreach (string ignoreTexturePath in ClickMapManager.IgnoreTexturePaths)
    {
      if (str.StartsWith(ignoreTexturePath, StringComparison.Ordinal))
        return;
    }
    this._textureMaps[TextureResource.op_Implicit(((TextureLoadedEventArgs) ref obj).Resource)] = ClickMapManager.ClickMap.FromImage<Rgba32>(image, 0.1f);
  }

  public bool IsOccluding(Texture texture, Vector2i pos)
  {
    ClickMapManager.ClickMap map;
    return this._textureMaps.TryGetValue(texture, out map) && ClickMapManager.SampleClickMap(map, pos, map.Size, Vector2i.Zero);
  }

  public bool IsOccluding(Robust.Client.Graphics.RSI rsi, Robust.Client.Graphics.RSI.StateId state, RsiDirection dir, int frame, Vector2i pos)
  {
    ClickMapManager.RsiClickMapData rsiClickMapData;
    Vector2i[][] vector2iArray1;
    if (!this._rsiMaps.TryGetValue(rsi, out rsiClickMapData) || !rsiClickMapData.Offsets.TryGetValue(state, out vector2iArray1) || vector2iArray1.Length <= dir)
      return false;
    Vector2i[] vector2iArray2 = vector2iArray1[dir];
    if (vector2iArray2.Length <= frame)
      return false;
    Vector2i offset = vector2iArray2[frame];
    return ClickMapManager.SampleClickMap(rsiClickMapData.ClickMap, pos, rsi.Size, offset);
  }

  private static bool SampleClickMap(
    ClickMapManager.ClickMap map,
    Vector2i pos,
    Vector2i bounds,
    Vector2i offset)
  {
    Vector2i vector2i1 = bounds;
    int num1;
    int num2;
    ((Vector2i) ref vector2i1).Deconstruct(ref num1, ref num2);
    int num3 = num1;
    int num4 = num2;
    Vector2i vector2i2 = pos;
    ((Vector2i) ref vector2i2).Deconstruct(ref num2, ref num1);
    int num5 = num2;
    int num6 = num1;
    for (int index1 = -2; index1 <= 2; ++index1)
    {
      int num7 = num5 + index1;
      if (num7 >= 0 && num7 < num3)
      {
        for (int index2 = -2; index2 <= 2; ++index2)
        {
          int num8 = num6 + index2;
          if (num8 >= 0 && num8 < num4 && map.IsOccluded(Vector2i.op_Addition(Vector2i.op_Implicit((num7, num8)), offset)))
            return true;
        }
      }
    }
    return false;
  }

  private sealed class RsiClickMapData
  {
    public readonly ClickMapManager.ClickMap ClickMap;
    public readonly Dictionary<Robust.Client.Graphics.RSI.StateId, Vector2i[][]> Offsets;

    public RsiClickMapData(
      ClickMapManager.ClickMap clickMap,
      Dictionary<Robust.Client.Graphics.RSI.StateId, Vector2i[][]> offsets)
    {
      this.ClickMap = clickMap;
      this.Offsets = offsets;
    }
  }

  internal sealed class ClickMap
  {
    [Robust.Shared.ViewVariables.ViewVariables]
    private readonly byte[] _data;

    public int Width { get; }

    public int Height { get; }

    [Robust.Shared.ViewVariables.ViewVariables]
    public Vector2i Size => Vector2i.op_Implicit((this.Width, this.Height));

    public bool IsOccluded(int x, int y)
    {
      int num = y * this.Width + x;
      return ((uint) this._data[num / 8] & (uint) (1 << num % 8)) > 0U;
    }

    public bool IsOccluded(Vector2i vector)
    {
      Vector2i vector2i = vector;
      int x;
      int y;
      ((Vector2i) ref vector2i).Deconstruct(ref x, ref y);
      return this.IsOccluded(x, y);
    }

    private ClickMap(byte[] data, int width, int height)
    {
      this.Width = width;
      this.Height = height;
      this._data = data;
    }

    public static ClickMapManager.ClickMap FromImage<T>(Image<T> image, float threshold) where T : unmanaged, IPixel<T>
    {
      byte num = (byte) ((double) threshold * (double) byte.MaxValue);
      int width = ((Image) image).Width;
      int height = ((Image) image).Height;
      byte[] data = new byte[(int) Math.Ceiling((double) (width * height) / 8.0)];
      Span<T> pixelSpan = ImageSharpExt.GetPixelSpan<T>(image);
      for (int index = 0; index < pixelSpan.Length; ++index)
      {
        Rgba32 rgba32 = new Rgba32();
        pixelSpan[index].ToRgba32(ref rgba32);
        if ((int) rgba32.A >= (int) num)
          data[index / 8] |= (byte) (1 << index % 8);
      }
      return new ClickMapManager.ClickMap(data, width, height);
    }

    public string DumpText()
    {
      StringBuilder stringBuilder = new StringBuilder();
      for (int y = 0; y < this.Height; ++y)
      {
        for (int x = 0; x < this.Width; ++x)
          stringBuilder.Append(this.IsOccluded(x, y) ? "1" : "0");
        stringBuilder.AppendLine();
      }
      return stringBuilder.ToString();
    }
  }
}
