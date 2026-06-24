// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Prototypes.EntityCategoryPrototype
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Generic;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Prototypes;

[Prototype(null, 1)]
public sealed class EntityCategoryPrototype : IPrototype
{
  [DataField(null, false, 1, false, false, null)]
  public string? Name;
  [DataField(null, false, 1, false, false, null)]
  public string? Description;
  [DataField(null, false, 1, false, false, null)]
  public string? Suffix;
  [DataField(null, false, 1, false, false, null)]
  public bool HideSpawnMenu;
  [DataField(null, false, 1, false, false, typeof (CustomHashSetSerializer<string, ComponentNameSerializer>))]
  public HashSet<string>? Components;
  [DataField(null, false, 1, false, false, null)]
  public bool Inheritable = true;

  [IdDataField(1, null)]
  public string ID { get; private set; }
}
