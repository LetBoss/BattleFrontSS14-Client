// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.IVerticesSimplifier
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Physics;

[NotContentImplementable]
public interface IVerticesSimplifier
{
  List<Vector2> Simplify(List<Vector2> vertices, float tolerance);
}
