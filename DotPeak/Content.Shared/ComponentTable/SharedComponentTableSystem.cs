// Decompiled with JetBrains decompiler
// Type: Content.Shared.ComponentTable.SharedComponentTableSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.EntityTable;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Shared.ComponentTable;

public sealed class SharedComponentTableSystem : EntitySystem
{
  [Dependency]
  private EntityTableSystem _entTable;
  [Dependency]
  private IPrototypeManager _proto;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ComponentTableComponent, MapInitEvent>(new EntityEventRefHandler<ComponentTableComponent, MapInitEvent>((object) this, __methodptr(OnTableInit)), (Type[]) null, (Type[]) null);
  }

  private void OnTableInit(Entity<ComponentTableComponent> ent, ref MapInitEvent args)
  {
    foreach (EntProtoId spawn in this._entTable.GetSpawns(ent.Comp.Table))
    {
      EntityPrototype entityPrototype;
      if (this._proto.TryIndex(spawn, ref entityPrototype))
        this.EntityManager.AddComponents(Entity<ComponentTableComponent>.op_Implicit(ent), entityPrototype.Components, true);
    }
  }
}
