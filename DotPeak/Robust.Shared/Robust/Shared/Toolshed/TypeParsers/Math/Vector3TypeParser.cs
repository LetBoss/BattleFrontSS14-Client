// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.TypeParsers.Math.Vector3TypeParser
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Numerics;

#nullable disable
namespace Robust.Shared.Toolshed.TypeParsers.Math;

internal sealed class Vector3TypeParser : SpanLikeTypeParser<Vector3, float>
{
  public override int Elements => 3;

  public override Vector3 Create(Span<float> elements)
  {
    return new Vector3(elements[0], elements[1], elements[2]);
  }
}
