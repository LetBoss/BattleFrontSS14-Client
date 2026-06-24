// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.RMCUserFireGroupComponent
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedFireGroupSystem)})]
public sealed class RMCUserFireGroupComponent : 
  Component,
  ISerializationGenerated<RMCUserFireGroupComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<string, TimeSpan> LastFired = new Dictionary<string, TimeSpan>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<string, EntityUid> LastGun = new Dictionary<string, EntityUid>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCUserFireGroupComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCUserFireGroupComponent) target1;
    if (serialization.TryCustomCopy<RMCUserFireGroupComponent>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<string, TimeSpan> target2 = (Dictionary<string, TimeSpan>) null;
    if (this.LastFired == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, TimeSpan>>(this.LastFired, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<Dictionary<string, TimeSpan>>(this.LastFired, hookCtx, context);
    target.LastFired = target2;
    Dictionary<string, EntityUid> target3 = (Dictionary<string, EntityUid>) null;
    if (this.LastGun == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, EntityUid>>(this.LastGun, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<Dictionary<string, EntityUid>>(this.LastGun, hookCtx, context);
    target.LastGun = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCUserFireGroupComponent target,
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
    RMCUserFireGroupComponent target1 = (RMCUserFireGroupComponent) target;
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
    RMCUserFireGroupComponent target1 = (RMCUserFireGroupComponent) target;
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
    RMCUserFireGroupComponent target1 = (RMCUserFireGroupComponent) target;
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
  virtual RMCUserFireGroupComponent Component.Instantiate() => new RMCUserFireGroupComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCUserFireGroupComponent_AutoState : IComponentState
  {
    public Dictionary<string, TimeSpan> LastFired;
    public Dictionary<string, NetEntity> LastGun;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCUserFireGroupComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCUserFireGroupComponent, ComponentGetState>(new ComponentEventRefHandler<RMCUserFireGroupComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCUserFireGroupComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCUserFireGroupComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCUserFireGroupComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCUserFireGroupComponent.RMCUserFireGroupComponent_AutoState()
      {
        LastFired = component.LastFired,
        LastGun = this.GetNetEntityDictionary<string>(component.LastGun)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCUserFireGroupComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCUserFireGroupComponent.RMCUserFireGroupComponent_AutoState current))
        return;
      component.LastFired = current.LastFired == null ? (Dictionary<string, TimeSpan>) null : new Dictionary<string, TimeSpan>((IDictionary<string, TimeSpan>) current.LastFired);
      this.EnsureEntityDictionary<RMCUserFireGroupComponent, string>(current.LastGun, uid, component.LastGun);
    }
  }
}
