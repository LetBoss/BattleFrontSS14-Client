// Decompiled with JetBrains decompiler
// Type: Content.Shared.Access.Systems.AccessToggleSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Access.Components;
using Content.Shared.Item.ItemToggle.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared.Access.Systems;

public sealed class AccessToggleSystem : EntitySystem
{
  [Dependency]
  private SharedAccessSystem _access;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AccessToggleComponent, ItemToggledEvent>(new EntityEventRefHandler<AccessToggleComponent, ItemToggledEvent>((object) this, __methodptr(OnToggled)), (Type[]) null, (Type[]) null);
  }

  private void OnToggled(Entity<AccessToggleComponent> ent, ref ItemToggledEvent args)
  {
    this._access.SetAccessEnabled(Entity<AccessToggleComponent>.op_Implicit(ent), args.Activated);
  }
}
