// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.SafePrimitives
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using NetSerializer;
using System;
using System.IO;

#nullable enable
namespace Robust.Shared.Serialization;

internal static class SafePrimitives
{
  public static void ReadPrimitive(Stream stream, out float value)
  {
    float f;
    Primitives.ReadPrimitive(stream, ref f);
    value = float.IsNaN(f) ? 0.0f : f;
  }

  public static void ReadPrimitive(Stream stream, out double value)
  {
    double d;
    Primitives.ReadPrimitive(stream, ref d);
    value = double.IsNaN(d) ? 0.0 : d;
  }

  public static void ReadPrimitive(Stream stream, out Half value)
  {
    Half half;
    Primitives.ReadPrimitive(stream, ref half);
    value = Half.IsNaN(half) ? Half.Zero : half;
  }
}
