// Decompiled with JetBrains decompiler
// Type: Content.Shared.Access.Systems.SharedAccessOverriderSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Access.Components;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.DoAfter;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Access.Systems;

public abstract class SharedAccessOverriderSystem : EntitySystem
{
  [Dependency]
  private ItemSlotsSystem _itemSlotsSystem;
  [Dependency]
  private ILogManager _log;
  public const string Sawmill = "accessoverrider";
  protected ISawmill _sawmill;

  public virtual void Initialize()
  {
    base.Initialize();
    this._sawmill = this._log.GetSawmill("accessoverrider");
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AccessOverriderComponent, ComponentInit>(new ComponentEventHandler<AccessOverriderComponent, ComponentInit>((object) this, __methodptr(OnComponentInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AccessOverriderComponent, ComponentRemove>(new ComponentEventHandler<AccessOverriderComponent, ComponentRemove>((object) this, __methodptr(OnComponentRemove)), (Type[]) null, (Type[]) null);
  }

  private void OnComponentInit(
    EntityUid uid,
    AccessOverriderComponent component,
    ComponentInit args)
  {
    this._itemSlotsSystem.AddItemSlot(uid, AccessOverriderComponent.PrivilegedIdCardSlotId, component.PrivilegedIdSlot);
  }

  private void OnComponentRemove(
    EntityUid uid,
    AccessOverriderComponent component,
    ComponentRemove args)
  {
    this._itemSlotsSystem.RemoveItemSlot(uid, component.PrivilegedIdSlot);
  }

  [NetSerializable]
  [Serializable]
  public sealed class AccessOverriderDoAfterEvent : 
    DoAfterEvent,
    ISerializationGenerated<SharedAccessOverriderSystem.AccessOverriderDoAfterEvent>,
    ISerializationGenerated
  {
    public override DoAfterEvent Clone() => (DoAfterEvent) this;

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void InternalCopy(
      ref SharedAccessOverriderSystem.AccessOverriderDoAfterEvent target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      DoAfterEvent target1 = (DoAfterEvent) target;
      this.InternalCopy(ref target1, serialization, hookCtx, context);
      target = (SharedAccessOverriderSystem.AccessOverriderDoAfterEvent) target1;
      serialization.TryCustomCopy<SharedAccessOverriderSystem.AccessOverriderDoAfterEvent>(this, ref target, hookCtx, false, context);
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void Copy(
      ref SharedAccessOverriderSystem.AccessOverriderDoAfterEvent target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      this.InternalCopy(ref target, serialization, hookCtx, context);
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public override void Copy(
      ref DoAfterEvent target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      SharedAccessOverriderSystem.AccessOverriderDoAfterEvent target1 = (SharedAccessOverriderSystem.AccessOverriderDoAfterEvent) target;
      this.Copy(ref target1, serialization, hookCtx, context);
      target = (DoAfterEvent) target1;
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public override void Copy(
      ref object target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      SharedAccessOverriderSystem.AccessOverriderDoAfterEvent target1 = (SharedAccessOverriderSystem.AccessOverriderDoAfterEvent) target;
      this.Copy(ref target1, serialization, hookCtx, context);
      target = (object) target1;
    }

    [PreserveBaseOverrides]
    [Obsolete("Use ISerializationManager.CreateCopy instead")]
    virtual SharedAccessOverriderSystem.AccessOverriderDoAfterEvent DoAfterEvent.Instantiate()
    {
      return new SharedAccessOverriderSystem.AccessOverriderDoAfterEvent();
    }
  }
}
