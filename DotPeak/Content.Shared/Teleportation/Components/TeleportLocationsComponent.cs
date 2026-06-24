// Decompiled with JetBrains decompiler
// Type: Content.Shared.Teleportation.Components.TeleportLocationsComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Teleportation.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Teleportation.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedTeleportLocationsSystem)})]
[AutoGenerateComponentState(false, false)]
public sealed class TeleportLocationsComponent : 
  Component,
  ISerializationGenerated<TeleportLocationsComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<TeleportPoint> AvailableWarps = new HashSet<TeleportPoint>();
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId? TeleportEffect;
  [DataField(null, false, 1, false, false, null)]
  public bool CloseAfterTeleport;
  [DataField(null, false, 1, false, false, null)]
  public LocId Name;
  [DataField(null, false, 1, false, false, null)]
  public LocId? Speech;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref TeleportLocationsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (TeleportLocationsComponent) target1;
    if (serialization.TryCustomCopy<TeleportLocationsComponent>(this, ref target, hookCtx, false, context))
      return;
    HashSet<TeleportPoint> target2 = (HashSet<TeleportPoint>) null;
    if (this.AvailableWarps == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<TeleportPoint>>(this.AvailableWarps, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<HashSet<TeleportPoint>>(this.AvailableWarps, hookCtx, context);
    target.AvailableWarps = target2;
    EntProtoId? target3 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.TeleportEffect, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntProtoId?>(this.TeleportEffect, hookCtx, context);
    target.TeleportEffect = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.CloseAfterTeleport, ref target4, hookCtx, false, context))
      target4 = this.CloseAfterTeleport;
    target.CloseAfterTeleport = target4;
    LocId target5 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.Name, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<LocId>(this.Name, hookCtx, context);
    target.Name = target5;
    LocId? target6 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.Speech, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<LocId?>(this.Speech, hookCtx, context);
    target.Speech = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref TeleportLocationsComponent target,
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
    TeleportLocationsComponent target1 = (TeleportLocationsComponent) target;
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
    TeleportLocationsComponent target1 = (TeleportLocationsComponent) target;
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
    TeleportLocationsComponent target1 = (TeleportLocationsComponent) target;
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
  virtual TeleportLocationsComponent Component.Instantiate() => new TeleportLocationsComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class TeleportLocationsComponent_AutoState : IComponentState
  {
    public HashSet<TeleportPoint> AvailableWarps;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class TeleportLocationsComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<TeleportLocationsComponent, ComponentGetState>(new ComponentEventRefHandler<TeleportLocationsComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<TeleportLocationsComponent, ComponentHandleState>(new ComponentEventRefHandler<TeleportLocationsComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      TeleportLocationsComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new TeleportLocationsComponent.TeleportLocationsComponent_AutoState()
      {
        AvailableWarps = component.AvailableWarps
      };
    }

    private void OnHandleState(
      EntityUid uid,
      TeleportLocationsComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is TeleportLocationsComponent.TeleportLocationsComponent_AutoState current))
        return;
      component.AvailableWarps = current.AvailableWarps == null ? (HashSet<TeleportPoint>) null : new HashSet<TeleportPoint>((IEnumerable<TeleportPoint>) current.AvailableWarps);
    }
  }
}
