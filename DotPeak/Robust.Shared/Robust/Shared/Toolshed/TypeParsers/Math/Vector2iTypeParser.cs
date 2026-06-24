// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.TypeParsers.Math.Vector2iTypeParser
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using System;

#nullable disable
namespace Robust.Shared.Toolshed.TypeParsers.Math;

internal sealed class Vector2iTypeParser : SpanLikeTypeParser<Vector2i, int>
{
  public override int Elements => 2;

  public override Vector2i Create(Span<int> elements) => new Vector2i(elements[0], elements[1]);
}
