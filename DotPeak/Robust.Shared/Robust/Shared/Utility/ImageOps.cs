// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.ImageOps
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;

#nullable enable
namespace Robust.Shared.Utility;

internal static class ImageOps
{
  public static void Blit<T>(
    Image<T> source,
    UIBox2i sourceRect,
    Image<T> destination,
    Vector2i destinationOffset)
    where T : unmanaged, IPixel<T>
  {
    ImageOps.Blit<T>((ReadOnlySpan<T>) ImageOps.GetPixelSpan<T>(source), ((Image) source).Width, sourceRect, destination, destinationOffset);
  }

  public static void Blit<T>(
    ReadOnlySpan<T> source,
    int sourceWidth,
    UIBox2i sourceRect,
    Image<T> destination,
    Vector2i destinationOffset)
    where T : unmanaged, IPixel<T>
  {
    Span<T> pixelSpan = ImageOps.GetPixelSpan<T>(destination);
    int width1 = ((Image) destination).Width;
    int height = ((UIBox2i) ref sourceRect).Height;
    int width2 = ((UIBox2i) ref sourceRect).Width;
    Vector2i vector2i = destinationOffset;
    int num1;
    int num2;
    ((Vector2i) ref vector2i).Deconstruct(ref num1, ref num2);
    int num3 = num1;
    int num4 = num2;
    for (int index = 0; index < height; ++index)
    {
      int num5 = sourceWidth * (index + sourceRect.Top) + sourceRect.Left;
      int num6 = width1 * (index + num4) + num3;
      ref ReadOnlySpan<T> local1 = ref source;
      int num7 = num5;
      int start1 = num7;
      int length1 = num5 + width2 - num7;
      ReadOnlySpan<T> readOnlySpan = local1.Slice(start1, length1);
      ref Span<T> local2 = ref pixelSpan;
      int num8 = num6;
      int start2 = num8;
      int length2 = num6 + width2 - num8;
      Span<T> destination1 = local2.Slice(start2, length2);
      readOnlySpan.CopyTo(destination1);
    }
  }

  public static Span<T> GetPixelSpan<T>(Image<T> image) where T : unmanaged, IPixel<T>
  {
    Memory<T> memory;
    return image.DangerousTryGetSinglePixelMemory(ref memory) ? memory.Span : throw new ArgumentException("Image is not backed by a single buffer, cannot fetch span.");
  }
}
