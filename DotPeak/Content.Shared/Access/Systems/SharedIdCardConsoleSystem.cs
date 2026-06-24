// Decompiled with JetBrains decompiler
// Type: Content.Shared.Access.Systems.SharedIdCardConsoleSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Access.Components;
using Content.Shared.Containers.ItemSlots;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Access.Systems;

public abstract class SharedIdCardConsoleSystem : EntitySystem
{
  [Dependency]
  private ItemSlotsSystem _itemSlotsSystem;
  [Dependency]
  private ILogManager _log;
  public const string Sawmill = "idconsole";
  protected ISawmill _sawmill;

  public virtual void Initialize()
  {
    base.Initialize();
    this._sawmill = this._log.GetSawmill("idconsole");
    // ISSUE: method pointer
    this.SubscribeLocalEvent<IdCardConsoleComponent, ComponentInit>(new ComponentEventHandler<IdCardConsoleComponent, ComponentInit>((object) this, __methodptr(OnComponentInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<IdCardConsoleComponent, ComponentRemove>(new ComponentEventHandler<IdCardConsoleComponent, ComponentRemove>((object) this, __methodptr(OnComponentRemove)), (Type[]) null, (Type[]) null);
  }

  private void OnComponentInit(EntityUid uid, IdCardConsoleComponent component, ComponentInit args)
  {
    this._itemSlotsSystem.AddItemSlot(uid, IdCardConsoleComponent.PrivilegedIdCardSlotId, component.PrivilegedIdSlot);
    this._itemSlotsSystem.AddItemSlot(uid, IdCardConsoleComponent.TargetIdCardSlotId, component.TargetIdSlot);
  }

  private void OnComponentRemove(
    EntityUid uid,
    IdCardConsoleComponent component,
    ComponentRemove args)
  {
    this._itemSlotsSystem.RemoveItemSlot(uid, component.PrivilegedIdSlot);
    this._itemSlotsSystem.RemoveItemSlot(uid, component.TargetIdSlot);
  }

  [NetSerializable]
  [Serializable]
  private sealed class IdCardConsoleComponentState : ComponentState
  {
    public List<string> AccessLevels;

    public IdCardConsoleComponentState(List<string> accessLevels)
    {
      this.AccessLevels = accessLevels;
    }
  }
}
