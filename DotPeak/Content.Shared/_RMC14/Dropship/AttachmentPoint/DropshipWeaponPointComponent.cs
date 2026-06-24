// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Dropship.AttachmentPoint.DropshipWeaponPointComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Dropship.AttachmentPoint;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedDropshipSystem)})]
public sealed class DropshipWeaponPointComponent : 
  Component,
  ISerializationGenerated<DropshipWeaponPointComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string WeaponContainerSlotId = "rmc_dropship_weapon_point_weapon_container_slot";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string AmmoContainerSlotId = "rmc_dropship_weapon_point_ammo_container_slot";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string DirOffset;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DropshipWeaponPointLocation? Location;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DropshipWeaponPointComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (DropshipWeaponPointComponent) target1;
    if (serialization.TryCustomCopy<DropshipWeaponPointComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.WeaponContainerSlotId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.WeaponContainerSlotId, ref target2, hookCtx, false, context))
      target2 = this.WeaponContainerSlotId;
    target.WeaponContainerSlotId = target2;
    string target3 = (string) null;
    if (this.AmmoContainerSlotId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.AmmoContainerSlotId, ref target3, hookCtx, false, context))
      target3 = this.AmmoContainerSlotId;
    target.AmmoContainerSlotId = target3;
    string target4 = (string) null;
    if (this.DirOffset == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.DirOffset, ref target4, hookCtx, false, context))
      target4 = this.DirOffset;
    target.DirOffset = target4;
    DropshipWeaponPointLocation? target5 = new DropshipWeaponPointLocation?();
    if (!serialization.TryCustomCopy<DropshipWeaponPointLocation?>(this.Location, ref target5, hookCtx, false, context))
      target5 = this.Location;
    target.Location = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DropshipWeaponPointComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DropshipWeaponPointComponent target1 = (DropshipWeaponPointComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DropshipWeaponPointComponent target1 = (DropshipWeaponPointComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DropshipWeaponPointComponent target1 = (DropshipWeaponPointComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual DropshipWeaponPointComponent Component.Instantiate()
  {
    return new DropshipWeaponPointComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class DropshipWeaponPointComponent_AutoState : IComponentState
  {
    public string WeaponContainerSlotId;
    public string AmmoContainerSlotId;
    public string DirOffset;
    public DropshipWeaponPointLocation? Location;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DropshipWeaponPointComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<DropshipWeaponPointComponent, ComponentGetState>(new ComponentEventRefHandler<DropshipWeaponPointComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<DropshipWeaponPointComponent, ComponentHandleState>(new ComponentEventRefHandler<DropshipWeaponPointComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      DropshipWeaponPointComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new DropshipWeaponPointComponent.DropshipWeaponPointComponent_AutoState()
      {
        WeaponContainerSlotId = component.WeaponContainerSlotId,
        AmmoContainerSlotId = component.AmmoContainerSlotId,
        DirOffset = component.DirOffset,
        Location = component.Location
      };
    }

    private void OnHandleState(
      EntityUid uid,
      DropshipWeaponPointComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is DropshipWeaponPointComponent.DropshipWeaponPointComponent_AutoState current))
        return;
      component.WeaponContainerSlotId = current.WeaponContainerSlotId;
      component.AmmoContainerSlotId = current.AmmoContainerSlotId;
      component.DirOffset = current.DirOffset;
      component.Location = current.Location;
    }
  }
}
