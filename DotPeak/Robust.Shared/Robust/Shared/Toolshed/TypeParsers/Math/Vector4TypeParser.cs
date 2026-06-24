// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.TypeParsers.Math.Vector4TypeParser
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Numerics;

#nullable disable
namespace Robust.Shared.Toolshed.TypeParsers.Math;

internal sealed class Vector4TypeParser : SpanLikeTypeParser<Vector4, float>
{
  public override int Elements => 4;

  public override Vector4 Create(Span<float> elements)
  {
    return new Vector4(elements[0], elements[1], elements[2], elements[4]);
  }
}
