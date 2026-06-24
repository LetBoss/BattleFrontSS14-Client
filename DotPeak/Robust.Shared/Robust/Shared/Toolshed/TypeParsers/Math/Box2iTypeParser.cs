// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.TypeParsers.Math.Box2iTypeParser
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using System;

#nullable disable
namespace Robust.Shared.Toolshed.TypeParsers.Math;

public sealed class Box2iTypeParser : SpanLikeTypeParser<Box2i, int>
{
  public override int Elements => 4;

  public override Box2i Create(Span<int> elements)
  {
    return new Box2i(elements[0], elements[1], elements[2], elements[3]);
  }
}
