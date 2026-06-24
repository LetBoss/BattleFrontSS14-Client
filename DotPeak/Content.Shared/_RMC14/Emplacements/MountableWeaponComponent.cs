// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Emplacements.MountableWeaponComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Emplacements;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (MountableWeaponSystem), typeof (SharedWeaponMountSystem)})]
public sealed class MountableWeaponComponent : 
  Component,
  ISerializationGenerated<MountableWeaponComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool RequiresMount = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int RequiredFreeHands = 2;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public NetEntity? MountedTo;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int ShootArc = 100;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MountableWeaponComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MountableWeaponComponent) target1;
    if (serialization.TryCustomCopy<MountableWeaponComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.RequiresMount, ref target2, hookCtx, false, context))
      target2 = this.RequiresMount;
    target.RequiresMount = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.RequiredFreeHands, ref target3, hookCtx, false, context))
      target3 = this.RequiredFreeHands;
    target.RequiredFreeHands = target3;
    NetEntity? target4 = new NetEntity?();
    if (!serialization.TryCustomCopy<NetEntity?>(this.MountedTo, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<NetEntity?>(this.MountedTo, hookCtx, context);
    target.MountedTo = target4;
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.ShootArc, ref target5, hookCtx, false, context))
      target5 = this.ShootArc;
    target.ShootArc = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MountableWeaponComponent target,
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
    MountableWeaponComponent target1 = (MountableWeaponComponent) target;
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
    MountableWeaponComponent target1 = (MountableWeaponComponent) target;
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
    MountableWeaponComponent target1 = (MountableWeaponComponent) target;
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
  virtual MountableWeaponComponent Component.Instantiate() => new MountableWeaponComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class MountableWeaponComponent_AutoState : IComponentState
  {
    public bool RequiresMount;
    public int RequiredFreeHands;
    public NetEntity? MountedTo;
    public int ShootArc;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class MountableWeaponComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<MountableWeaponComponent, ComponentGetState>(new ComponentEventRefHandler<MountableWeaponComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<MountableWeaponComponent, ComponentHandleState>(new ComponentEventRefHandler<MountableWeaponComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      MountableWeaponComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new MountableWeaponComponent.MountableWeaponComponent_AutoState()
      {
        RequiresMount = component.RequiresMount,
        RequiredFreeHands = component.RequiredFreeHands,
        MountedTo = component.MountedTo,
        ShootArc = component.ShootArc
      };
    }

    private void OnHandleState(
      EntityUid uid,
      MountableWeaponComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is MountableWeaponComponent.MountableWeaponComponent_AutoState current))
        return;
      component.RequiresMount = current.RequiresMount;
      component.RequiredFreeHands = current.RequiredFreeHands;
      component.MountedTo = current.MountedTo;
      component.ShootArc = current.ShootArc;
    }
  }
}
