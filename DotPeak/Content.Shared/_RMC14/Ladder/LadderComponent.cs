// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Ladder.LadderComponent
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
namespace Content.Shared._RMC14.Ladder;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedLadderSystem)})]
public sealed class LadderComponent : 
  Component,
  ISerializationGenerated<LadderComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string? Id;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Other;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Delay = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Range = 1.6f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? LastDoAfterEnt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ushort? LastDoAfterId;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan LastDoAfterTime;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<EntityUid> Watching = new HashSet<EntityUid>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref LadderComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (LadderComponent) target1;
    if (serialization.TryCustomCopy<LadderComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.Id, ref target2, hookCtx, false, context))
      target2 = this.Id;
    target.Id = target2;
    EntityUid? target3 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Other, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntityUid?>(this.Other, hookCtx, context);
    target.Other = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Delay, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.Delay, hookCtx, context);
    target.Delay = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Range, ref target5, hookCtx, false, context))
      target5 = this.Range;
    target.Range = target5;
    EntityUid? target6 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.LastDoAfterEnt, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<EntityUid?>(this.LastDoAfterEnt, hookCtx, context);
    target.LastDoAfterEnt = target6;
    ushort? target7 = new ushort?();
    if (!serialization.TryCustomCopy<ushort?>(this.LastDoAfterId, ref target7, hookCtx, false, context))
      target7 = this.LastDoAfterId;
    target.LastDoAfterId = target7;
    TimeSpan target8 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LastDoAfterTime, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<TimeSpan>(this.LastDoAfterTime, hookCtx, context);
    target.LastDoAfterTime = target8;
    HashSet<EntityUid> target9 = (HashSet<EntityUid>) null;
    if (this.Watching == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<EntityUid>>(this.Watching, ref target9, hookCtx, true, context))
      target9 = serialization.CreateCopy<HashSet<EntityUid>>(this.Watching, hookCtx, context);
    target.Watching = target9;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref LadderComponent target,
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
    LadderComponent target1 = (LadderComponent) target;
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
    LadderComponent target1 = (LadderComponent) target;
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
    LadderComponent target1 = (LadderComponent) target;
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
  virtual LadderComponent Component.Instantiate() => new LadderComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class LadderComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<LadderComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<LadderComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      LadderComponent component,
      ref EntityUnpausedEvent args)
    {
      component.LastDoAfterTime += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class LadderComponent_AutoState : IComponentState
  {
    public 
    #nullable enable
    string? Id;
    public NetEntity? Other;
    public TimeSpan Delay;
    public float Range;
    public NetEntity? LastDoAfterEnt;
    public ushort? LastDoAfterId;
    public TimeSpan LastDoAfterTime;
    public HashSet<NetEntity> Watching;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class LadderComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<LadderComponent, ComponentGetState>(new ComponentEventRefHandler<LadderComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<LadderComponent, ComponentHandleState>(new ComponentEventRefHandler<LadderComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, LadderComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new LadderComponent.LadderComponent_AutoState()
      {
        Id = component.Id,
        Other = this.GetNetEntity(component.Other),
        Delay = component.Delay,
        Range = component.Range,
        LastDoAfterEnt = this.GetNetEntity(component.LastDoAfterEnt),
        LastDoAfterId = component.LastDoAfterId,
        LastDoAfterTime = component.LastDoAfterTime,
        Watching = this.GetNetEntitySet(component.Watching)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      LadderComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is LadderComponent.LadderComponent_AutoState current))
        return;
      component.Id = current.Id;
      component.Other = this.EnsureEntity<LadderComponent>(current.Other, uid);
      component.Delay = current.Delay;
      component.Range = current.Range;
      component.LastDoAfterEnt = this.EnsureEntity<LadderComponent>(current.LastDoAfterEnt, uid);
      component.LastDoAfterId = current.LastDoAfterId;
      component.LastDoAfterTime = current.LastDoAfterTime;
      this.EnsureEntitySet<LadderComponent>(current.Watching, uid, component.Watching);
    }
  }
}
