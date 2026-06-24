// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Map.ITileDefinition
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Map;

public interface ITileDefinition : IPrototype
{
  ushort TileId { get; }

  string Name { get; }

  ResPath? Sprite { get; }

  Dictionary<Direction, ResPath> EdgeSprites { get; }

  int EdgeSpritePriority { get; }

  float Friction { get; }

  byte Variants { get; }

  bool AllowRotationMirror => false;

  void AssignTileId(ushort id);

  bool EditorHidden => false;
}
