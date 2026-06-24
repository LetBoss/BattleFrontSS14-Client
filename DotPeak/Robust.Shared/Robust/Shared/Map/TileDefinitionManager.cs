// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Map.TileDefinitionManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Random;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Robust.Shared.Map;

[Virtual]
internal class TileDefinitionManager : 
  ITileDefinitionManager,
  IEnumerable<ITileDefinition>,
  IEnumerable
{
  protected readonly List<ITileDefinition> TileDefs;
  private readonly Dictionary<string, ITileDefinition> _tileNames;
  private readonly Dictionary<string, List<string>> _awaitingAliases;

  public TileDefinitionManager()
  {
    this.TileDefs = new List<ITileDefinition>();
    this._tileNames = new Dictionary<string, ITileDefinition>();
    this._awaitingAliases = new Dictionary<string, List<string>>();
  }

  public virtual void Initialize()
  {
  }

  public virtual void Register(ITileDefinition tileDef)
  {
    string id = tileDef.ID;
    if (this._tileNames.ContainsKey(id))
      throw new ArgumentException("Another tile definition or alias with the same name has already been registered.", nameof (tileDef));
    ushort count = checked ((ushort) this.TileDefs.Count);
    tileDef.AssignTileId(count);
    this.TileDefs.Add(tileDef);
    this._tileNames[id] = tileDef;
  }

  public Tile GetVariantTile(string name, IRobustRandom random)
  {
    return this.GetVariantTile(this[name], random);
  }

  public Tile GetVariantTile(string name, System.Random random)
  {
    return this.GetVariantTile(this[name], random);
  }

  public Tile GetVariantTile(ITileDefinition tileDef, IRobustRandom random)
  {
    return new Tile((int) tileDef.TileId, variant: random.NextByte(tileDef.Variants));
  }

  public Tile GetVariantTile(ITileDefinition tileDef, System.Random random)
  {
    return new Tile((int) tileDef.TileId, variant: random.NextByte(tileDef.Variants));
  }

  public ITileDefinition this[string name] => this._tileNames[name];

  public ITileDefinition this[int id] => this.TileDefs[id];

  public bool TryGetDefinition(string name, [NotNullWhen(true)] out ITileDefinition? definition)
  {
    return this._tileNames.TryGetValue(name, out definition);
  }

  public bool TryGetDefinition(int id, [NotNullWhen(true)] out ITileDefinition? definition)
  {
    if (id >= this.TileDefs.Count)
    {
      definition = (ITileDefinition) null;
      return false;
    }
    definition = this.TileDefs[id];
    return true;
  }

  public int Count => this.TileDefs.Count;

  public IEnumerator<ITileDefinition> GetEnumerator()
  {
    return (IEnumerator<ITileDefinition>) this.TileDefs.GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
}
