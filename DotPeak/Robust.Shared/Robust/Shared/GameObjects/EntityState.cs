// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.EntityState
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using NetSerializer;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.GameObjects;

[NetSerializable]
[Serializable]
public sealed class EntityState
{
  public NetEntity NetEntity;
  public readonly GameTick EntityLastModified;
  public HashSet<ushort>? NetComponents;

  public NetListAsArray<ComponentChange> ComponentChanges { get; }

  public bool Empty
  {
    get
    {
      IReadOnlyCollection<ComponentChange> componentChanges = this.ComponentChanges.Value;
      return (componentChanges == null || componentChanges.Count == 0) && this.NetComponents == null;
    }
  }

  public EntityState(
    NetEntity netEntity,
    NetListAsArray<ComponentChange> changedComponents,
    GameTick lastModified,
    HashSet<ushort>? netComps = null)
  {
    this.NetEntity = netEntity;
    this.ComponentChanges = changedComponents;
    this.EntityLastModified = lastModified;
    this.NetComponents = netComps;
  }
}
