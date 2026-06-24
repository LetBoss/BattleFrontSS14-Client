// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.WeedKiller.WeedKillerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
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
namespace Content.Shared._RMC14.WeedKiller;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (WeedKillerSystem)})]
public sealed class WeedKillerComponent : 
  Component,
  ISerializationGenerated<WeedKillerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Deployed;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan DeployAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Disabled;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan DisableAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? Sound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Effects/rocketpod_fire.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Dropship;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<EntProtoId> AreaPrototypes = new HashSet<EntProtoId>();
  [DataField(null, false, 1, false, false, null)]
  public List<EntityUid> LinkedAreas = new List<EntityUid>();
  [DataField(null, false, 1, false, false, null)]
  public List<(Entity<MapGridComponent> Grid, Vector2i Indices)> Positions = new List<(Entity<MapGridComponent>, Vector2i)>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref WeedKillerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (WeedKillerComponent) target1;
    if (serialization.TryCustomCopy<WeedKillerComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Deployed, ref target2, hookCtx, false, context))
      target2 = this.Deployed;
    target.Deployed = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DeployAt, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.DeployAt, hookCtx, context);
    target.DeployAt = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.Disabled, ref target4, hookCtx, false, context))
      target4 = this.Disabled;
    target.Disabled = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DisableAt, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.DisableAt, hookCtx, context);
    target.DisableAt = target5;
    SoundSpecifier target6 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target6;
    EntityUid? target7 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Dropship, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<EntityUid?>(this.Dropship, hookCtx, context);
    target.Dropship = target7;
    HashSet<EntProtoId> target8 = (HashSet<EntProtoId>) null;
    if (this.AreaPrototypes == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<EntProtoId>>(this.AreaPrototypes, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<HashSet<EntProtoId>>(this.AreaPrototypes, hookCtx, context);
    target.AreaPrototypes = target8;
    List<EntityUid> target9 = (List<EntityUid>) null;
    if (this.LinkedAreas == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntityUid>>(this.LinkedAreas, ref target9, hookCtx, true, context))
      target9 = serialization.CreateCopy<List<EntityUid>>(this.LinkedAreas, hookCtx, context);
    target.LinkedAreas = target9;
    List<(Entity<MapGridComponent>, Vector2i)> target10 = (List<(Entity<MapGridComponent>, Vector2i)>) null;
    if (this.Positions == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<(Entity<MapGridComponent>, Vector2i)>>(this.Positions, ref target10, hookCtx, true, context))
      target10 = serialization.CreateCopy<List<(Entity<MapGridComponent>, Vector2i)>>(this.Positions, hookCtx, context);
    target.Positions = target10;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref WeedKillerComponent target,
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
    WeedKillerComponent target1 = (WeedKillerComponent) target;
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
    WeedKillerComponent target1 = (WeedKillerComponent) target;
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
    WeedKillerComponent target1 = (WeedKillerComponent) target;
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
  virtual WeedKillerComponent Component.Instantiate() => new WeedKillerComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class WeedKillerComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<WeedKillerComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<WeedKillerComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      WeedKillerComponent component,
      ref EntityUnpausedEvent args)
    {
      component.DeployAt += args.PausedTime;
      component.DisableAt += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class WeedKillerComponent_AutoState : IComponentState
  {
    public bool Deployed;
    public TimeSpan DeployAt;
    public bool Disabled;
    public TimeSpan DisableAt;
    public 
    #nullable enable
    SoundSpecifier? Sound;
    public NetEntity? Dropship;
    public HashSet<EntProtoId> AreaPrototypes;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class WeedKillerComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<WeedKillerComponent, ComponentGetState>(new ComponentEventRefHandler<WeedKillerComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<WeedKillerComponent, ComponentHandleState>(new ComponentEventRefHandler<WeedKillerComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      WeedKillerComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new WeedKillerComponent.WeedKillerComponent_AutoState()
      {
        Deployed = component.Deployed,
        DeployAt = component.DeployAt,
        Disabled = component.Disabled,
        DisableAt = component.DisableAt,
        Sound = component.Sound,
        Dropship = this.GetNetEntity(component.Dropship),
        AreaPrototypes = component.AreaPrototypes
      };
    }

    private void OnHandleState(
      EntityUid uid,
      WeedKillerComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is WeedKillerComponent.WeedKillerComponent_AutoState current))
        return;
      component.Deployed = current.Deployed;
      component.DeployAt = current.DeployAt;
      component.Disabled = current.Disabled;
      component.DisableAt = current.DisableAt;
      component.Sound = current.Sound;
      component.Dropship = this.EnsureEntity<WeedKillerComponent>(current.Dropship, uid);
      component.AreaPrototypes = current.AreaPrototypes == null ? (HashSet<EntProtoId>) null : new HashSet<EntProtoId>((IEnumerable<EntProtoId>) current.AreaPrototypes);
    }
  }
}
