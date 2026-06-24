// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Map.ITileDefinitionManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Random;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Robust.Shared.Map;

[NotContentImplementable]
public interface ITileDefinitionManager : IEnumerable<ITileDefinition>, IEnumerable
{
  Tile GetVariantTile(string name, IRobustRandom random);

  Tile GetVariantTile(string name, System.Random random);

  Tile GetVariantTile(ITileDefinition tileDef, IRobustRandom random);

  Tile GetVariantTile(ITileDefinition tileDef, System.Random random);

  ITileDefinition this[string name] { get; }

  ITileDefinition this[int id] { get; }

  bool TryGetDefinition(string name, [NotNullWhen(true)] out ITileDefinition? definition);

  bool TryGetDefinition(int id, [NotNullWhen(true)] out ITileDefinition? definition);

  int Count { get; }

  void Initialize();

  void Register(ITileDefinition tileDef);
}
