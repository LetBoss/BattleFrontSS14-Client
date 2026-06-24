// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Prototypes.TileAliasPrototype
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Serialization.Manager.Attributes;

#nullable enable
namespace Robust.Shared.Prototypes;

[Prototype(null, 1)]
public sealed class TileAliasPrototype : IPrototype
{
  [DataField(null, false, 1, false, false, null)]
  public string Target { get; private set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  [IdDataField(1, null)]
  public string ID { get; private set; }
}
