// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Projectiles.SpawnOnTerminateComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Projectiles;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (RMCProjectileSystem)})]
public sealed class SpawnOnTerminateComponent : 
  Component,
  ISerializationGenerated<SpawnOnTerminateComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityCoordinates? Origin;
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public EntProtoId Spawn;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId? Popup;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Content.Shared.Popups.PopupType? PopupType;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ProjectileAdjust = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SpawnOnTerminateComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SpawnOnTerminateComponent) target1;
    if (serialization.TryCustomCopy<SpawnOnTerminateComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityCoordinates? target2 = new EntityCoordinates?();
    if (!serialization.TryCustomCopy<EntityCoordinates?>(this.Origin, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityCoordinates?>(this.Origin, hookCtx, context);
    target.Origin = target2;
    EntProtoId target3 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Spawn, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntProtoId>(this.Spawn, hookCtx, context);
    target.Spawn = target3;
    LocId? target4 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.Popup, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<LocId?>(this.Popup, hookCtx, context);
    target.Popup = target4;
    Content.Shared.Popups.PopupType? target5 = new Content.Shared.Popups.PopupType?();
    if (!serialization.TryCustomCopy<Content.Shared.Popups.PopupType?>(this.PopupType, ref target5, hookCtx, false, context))
      target5 = this.PopupType;
    target.PopupType = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.ProjectileAdjust, ref target6, hookCtx, false, context))
      target6 = this.ProjectileAdjust;
    target.ProjectileAdjust = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SpawnOnTerminateComponent target,
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
    SpawnOnTerminateComponent target1 = (SpawnOnTerminateComponent) target;
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
    SpawnOnTerminateComponent target1 = (SpawnOnTerminateComponent) target;
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
    SpawnOnTerminateComponent target1 = (SpawnOnTerminateComponent) target;
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
  virtual SpawnOnTerminateComponent Component.Instantiate() => new SpawnOnTerminateComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class SpawnOnTerminateComponent_AutoState : IComponentState
  {
    public NetCoordinates? Origin;
    public EntProtoId Spawn;
    public LocId? Popup;
    public Content.Shared.Popups.PopupType? PopupType;
    public bool ProjectileAdjust;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class SpawnOnTerminateComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<SpawnOnTerminateComponent, ComponentGetState>(new ComponentEventRefHandler<SpawnOnTerminateComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<SpawnOnTerminateComponent, ComponentHandleState>(new ComponentEventRefHandler<SpawnOnTerminateComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      SpawnOnTerminateComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new SpawnOnTerminateComponent.SpawnOnTerminateComponent_AutoState()
      {
        Origin = this.GetNetCoordinates(component.Origin),
        Spawn = component.Spawn,
        Popup = component.Popup,
        PopupType = component.PopupType,
        ProjectileAdjust = component.ProjectileAdjust
      };
    }

    private void OnHandleState(
      EntityUid uid,
      SpawnOnTerminateComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is SpawnOnTerminateComponent.SpawnOnTerminateComponent_AutoState current))
        return;
      component.Origin = this.EnsureCoordinates<SpawnOnTerminateComponent>(current.Origin, uid);
      component.Spawn = current.Spawn;
      component.Popup = current.Popup;
      component.PopupType = current.PopupType;
      component.ProjectileAdjust = current.ProjectileAdjust;
    }
  }
}
