// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Dropship.Weapon.DropshipTargetComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Medical.MedevacStretcher;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Dropship.Weapon;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedDropshipWeaponSystem)})]
public sealed class DropshipTargetComponent : 
  Component,
  ISerializationGenerated<DropshipTargetComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  [Access(new Type[] {typeof (SharedDropshipWeaponSystem), typeof (MedevacStretcherSystem)})]
  public string Abbreviation = string.Empty;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool IsTargetableByWeapons = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<EntityUid, EntityUid> Eyes = new Dictionary<EntityUid, EntityUid>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DropshipTargetComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (DropshipTargetComponent) target1;
    if (serialization.TryCustomCopy<DropshipTargetComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.Abbreviation == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Abbreviation, ref target2, hookCtx, false, context))
      target2 = this.Abbreviation;
    target.Abbreviation = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsTargetableByWeapons, ref target3, hookCtx, false, context))
      target3 = this.IsTargetableByWeapons;
    target.IsTargetableByWeapons = target3;
    Dictionary<EntityUid, EntityUid> target4 = (Dictionary<EntityUid, EntityUid>) null;
    if (this.Eyes == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<EntityUid, EntityUid>>(this.Eyes, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<Dictionary<EntityUid, EntityUid>>(this.Eyes, hookCtx, context);
    target.Eyes = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DropshipTargetComponent target,
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
    DropshipTargetComponent target1 = (DropshipTargetComponent) target;
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
    DropshipTargetComponent target1 = (DropshipTargetComponent) target;
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
    DropshipTargetComponent target1 = (DropshipTargetComponent) target;
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
  virtual DropshipTargetComponent Component.Instantiate() => new DropshipTargetComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class DropshipTargetComponent_AutoState : IComponentState
  {
    public string Abbreviation;
    public bool IsTargetableByWeapons;
    public Dictionary<NetEntity, NetEntity> Eyes;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DropshipTargetComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<DropshipTargetComponent, ComponentGetState>(new ComponentEventRefHandler<DropshipTargetComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<DropshipTargetComponent, ComponentHandleState>(new ComponentEventRefHandler<DropshipTargetComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      DropshipTargetComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new DropshipTargetComponent.DropshipTargetComponent_AutoState()
      {
        Abbreviation = component.Abbreviation,
        IsTargetableByWeapons = component.IsTargetableByWeapons,
        Eyes = this.GetNetEntityDictionary(component.Eyes)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      DropshipTargetComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is DropshipTargetComponent.DropshipTargetComponent_AutoState current))
        return;
      component.Abbreviation = current.Abbreviation;
      component.IsTargetableByWeapons = current.IsTargetableByWeapons;
      this.EnsureEntityDictionary<DropshipTargetComponent>(current.Eyes, uid, component.Eyes);
    }
  }
}
