// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Overwatch.OverwatchConsoleComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Overwatch;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedOverwatchConsoleSystem)})]
public sealed class OverwatchConsoleComponent : 
  Component,
  ISerializationGenerated<OverwatchConsoleComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public NetEntity? Squad;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string? Operator;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public OverwatchLocation? Location;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ShowDead = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ShowHidden;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<NetEntity> Hidden = new HashSet<NetEntity>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public OverwatchSavedLocation?[] SavedLocations = new OverwatchSavedLocation?[3];
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int LastLocation;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan LastMessage;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan MessageCooldown = TimeSpan.FromSeconds(0.5);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool CanMessageSquad = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool HasOrbital;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2i OrbitalCoordinates;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan NextOrbitalLaunch;
  [DataField(null, false, 1, false, false, null)]
  public string Group = "UNMC";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool CanOrbitalBombardment = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref OverwatchConsoleComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (OverwatchConsoleComponent) target1;
    if (serialization.TryCustomCopy<OverwatchConsoleComponent>(this, ref target, hookCtx, false, context))
      return;
    NetEntity? target2 = new NetEntity?();
    if (!serialization.TryCustomCopy<NetEntity?>(this.Squad, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<NetEntity?>(this.Squad, hookCtx, context);
    target.Squad = target2;
    string target3 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.Operator, ref target3, hookCtx, false, context))
      target3 = this.Operator;
    target.Operator = target3;
    OverwatchLocation? target4 = new OverwatchLocation?();
    if (!serialization.TryCustomCopy<OverwatchLocation?>(this.Location, ref target4, hookCtx, false, context))
      target4 = this.Location;
    target.Location = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.ShowDead, ref target5, hookCtx, false, context))
      target5 = this.ShowDead;
    target.ShowDead = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.ShowHidden, ref target6, hookCtx, false, context))
      target6 = this.ShowHidden;
    target.ShowHidden = target6;
    HashSet<NetEntity> target7 = (HashSet<NetEntity>) null;
    if (this.Hidden == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<NetEntity>>(this.Hidden, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<HashSet<NetEntity>>(this.Hidden, hookCtx, context);
    target.Hidden = target7;
    OverwatchSavedLocation?[] target8 = (OverwatchSavedLocation?[]) null;
    if (this.SavedLocations == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<OverwatchSavedLocation?[]>(this.SavedLocations, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<OverwatchSavedLocation?[]>(this.SavedLocations, hookCtx, context);
    target.SavedLocations = target8;
    int target9 = 0;
    if (!serialization.TryCustomCopy<int>(this.LastLocation, ref target9, hookCtx, false, context))
      target9 = this.LastLocation;
    target.LastLocation = target9;
    TimeSpan target10 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LastMessage, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<TimeSpan>(this.LastMessage, hookCtx, context);
    target.LastMessage = target10;
    TimeSpan target11 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.MessageCooldown, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<TimeSpan>(this.MessageCooldown, hookCtx, context);
    target.MessageCooldown = target11;
    bool target12 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanMessageSquad, ref target12, hookCtx, false, context))
      target12 = this.CanMessageSquad;
    target.CanMessageSquad = target12;
    bool target13 = false;
    if (!serialization.TryCustomCopy<bool>(this.HasOrbital, ref target13, hookCtx, false, context))
      target13 = this.HasOrbital;
    target.HasOrbital = target13;
    Vector2i target14 = new Vector2i();
    if (!serialization.TryCustomCopy<Vector2i>(this.OrbitalCoordinates, ref target14, hookCtx, false, context))
      target14 = serialization.CreateCopy<Vector2i>(this.OrbitalCoordinates, hookCtx, context);
    target.OrbitalCoordinates = target14;
    TimeSpan target15 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextOrbitalLaunch, ref target15, hookCtx, false, context))
      target15 = serialization.CreateCopy<TimeSpan>(this.NextOrbitalLaunch, hookCtx, context);
    target.NextOrbitalLaunch = target15;
    string target16 = (string) null;
    if (this.Group == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Group, ref target16, hookCtx, false, context))
      target16 = this.Group;
    target.Group = target16;
    bool target17 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanOrbitalBombardment, ref target17, hookCtx, false, context))
      target17 = this.CanOrbitalBombardment;
    target.CanOrbitalBombardment = target17;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref OverwatchConsoleComponent target,
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
    OverwatchConsoleComponent target1 = (OverwatchConsoleComponent) target;
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
    OverwatchConsoleComponent target1 = (OverwatchConsoleComponent) target;
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
    OverwatchConsoleComponent target1 = (OverwatchConsoleComponent) target;
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
  virtual OverwatchConsoleComponent Component.Instantiate() => new OverwatchConsoleComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class OverwatchConsoleComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<OverwatchConsoleComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<OverwatchConsoleComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      OverwatchConsoleComponent component,
      ref EntityUnpausedEvent args)
    {
      component.LastMessage += args.PausedTime;
      component.NextOrbitalLaunch += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class OverwatchConsoleComponent_AutoState : IComponentState
  {
    public NetEntity? Squad;
    public 
    #nullable enable
    string? Operator;
    public OverwatchLocation? Location;
    public bool ShowDead;
    public bool ShowHidden;
    public HashSet<NetEntity> Hidden;
    public OverwatchSavedLocation?[] SavedLocations;
    public int LastLocation;
    public TimeSpan LastMessage;
    public TimeSpan MessageCooldown;
    public bool CanMessageSquad;
    public bool HasOrbital;
    public Vector2i OrbitalCoordinates;
    public TimeSpan NextOrbitalLaunch;
    public bool CanOrbitalBombardment;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class OverwatchConsoleComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<OverwatchConsoleComponent, ComponentGetState>(new ComponentEventRefHandler<OverwatchConsoleComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<OverwatchConsoleComponent, ComponentHandleState>(new ComponentEventRefHandler<OverwatchConsoleComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      OverwatchConsoleComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new OverwatchConsoleComponent.OverwatchConsoleComponent_AutoState()
      {
        Squad = component.Squad,
        Operator = component.Operator,
        Location = component.Location,
        ShowDead = component.ShowDead,
        ShowHidden = component.ShowHidden,
        Hidden = component.Hidden,
        SavedLocations = component.SavedLocations,
        LastLocation = component.LastLocation,
        LastMessage = component.LastMessage,
        MessageCooldown = component.MessageCooldown,
        CanMessageSquad = component.CanMessageSquad,
        HasOrbital = component.HasOrbital,
        OrbitalCoordinates = component.OrbitalCoordinates,
        NextOrbitalLaunch = component.NextOrbitalLaunch,
        CanOrbitalBombardment = component.CanOrbitalBombardment
      };
    }

    private void OnHandleState(
      EntityUid uid,
      OverwatchConsoleComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is OverwatchConsoleComponent.OverwatchConsoleComponent_AutoState current))
        return;
      component.Squad = current.Squad;
      component.Operator = current.Operator;
      component.Location = current.Location;
      component.ShowDead = current.ShowDead;
      component.ShowHidden = current.ShowHidden;
      component.Hidden = current.Hidden == null ? (HashSet<NetEntity>) null : new HashSet<NetEntity>((IEnumerable<NetEntity>) current.Hidden);
      component.SavedLocations = current.SavedLocations;
      component.LastLocation = current.LastLocation;
      component.LastMessage = current.LastMessage;
      component.MessageCooldown = current.MessageCooldown;
      component.CanMessageSquad = current.CanMessageSquad;
      component.HasOrbital = current.HasOrbital;
      component.OrbitalCoordinates = current.OrbitalCoordinates;
      component.NextOrbitalLaunch = current.NextOrbitalLaunch;
      component.CanOrbitalBombardment = current.CanOrbitalBombardment;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, OverwatchConsoleComponent>(uid, component, ref args1);
    }
  }
}
