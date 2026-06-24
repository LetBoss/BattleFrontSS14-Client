// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.TypeParsers.Math.Vector2TypeParser
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Numerics;

#nullable disable
namespace Robust.Shared.Toolshed.TypeParsers.Math;

internal sealed class Vector2TypeParser : SpanLikeTypeParser<Vector2, float>
{
  public override int Elements => 2;

  public override Vector2 Create(Span<float> elements)
  {
    return new Vector2((ReadOnlySpan<float>) elements);
  }
}
