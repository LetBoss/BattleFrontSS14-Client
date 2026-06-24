// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.ChunkSplitDebugMessage
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Robust.Shared.GameObjects;

[NetSerializable]
[Serializable]
public sealed class ChunkSplitDebugMessage : EntityEventArgs
{
  public NetEntity Grid;
  public Dictionary<Vector2i, List<List<Vector2i>>> Nodes = new Dictionary<Vector2i, List<List<Vector2i>>>();
  public List<(Vector2 Start, Vector2 End)> Connections = new List<(Vector2, Vector2)>();
}
