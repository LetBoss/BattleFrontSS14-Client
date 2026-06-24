// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.ManageHive.Boons.HiveBoonDefinitionComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.ManageHive.Boons;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (ManageHiveSystem)})]
public sealed class HiveBoonDefinitionComponent : 
  Component,
  ISerializationGenerated<HiveBoonDefinitionComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Cost = 1;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Pylons = 1;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Reusable = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Duration;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Cooldown;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan UnlockAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan UnlockAtRandomAdd;
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public EntProtoId<HiveBoonDefinitionComponent>? DuplicateId;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool NoLivingKing;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool RequiresCore;
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public HiveBoonEvent? Event;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref HiveBoonDefinitionComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (HiveBoonDefinitionComponent) target1;
    if (serialization.TryCustomCopy<HiveBoonDefinitionComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.Cost, ref target2, hookCtx, false, context))
      target2 = this.Cost;
    target.Cost = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.Pylons, ref target3, hookCtx, false, context))
      target3 = this.Pylons;
    target.Pylons = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.Reusable, ref target4, hookCtx, false, context))
      target4 = this.Reusable;
    target.Reusable = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Duration, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.Duration, hookCtx, context);
    target.Duration = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Cooldown, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.Cooldown, hookCtx, context);
    target.Cooldown = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.UnlockAt, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.UnlockAt, hookCtx, context);
    target.UnlockAt = target7;
    TimeSpan target8 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.UnlockAtRandomAdd, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<TimeSpan>(this.UnlockAtRandomAdd, hookCtx, context);
    target.UnlockAtRandomAdd = target8;
    EntProtoId<HiveBoonDefinitionComponent>? target9 = new EntProtoId<HiveBoonDefinitionComponent>?();
    if (!serialization.TryCustomCopy<EntProtoId<HiveBoonDefinitionComponent>?>(this.DuplicateId, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<EntProtoId<HiveBoonDefinitionComponent>?>(this.DuplicateId, hookCtx, context);
    target.DuplicateId = target9;
    bool target10 = false;
    if (!serialization.TryCustomCopy<bool>(this.NoLivingKing, ref target10, hookCtx, false, context))
      target10 = this.NoLivingKing;
    target.NoLivingKing = target10;
    bool target11 = false;
    if (!serialization.TryCustomCopy<bool>(this.RequiresCore, ref target11, hookCtx, false, context))
      target11 = this.RequiresCore;
    target.RequiresCore = target11;
    HiveBoonEvent target12 = (HiveBoonEvent) null;
    if (!serialization.TryCustomCopy<HiveBoonEvent>(this.Event, ref target12, hookCtx, true, context))
    {
      if (this.Event == null)
        target12 = (HiveBoonEvent) null;
      else
        serialization.CopyTo<HiveBoonEvent>(this.Event, ref target12, hookCtx, context);
    }
    target.Event = target12;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref HiveBoonDefinitionComponent target,
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
    HiveBoonDefinitionComponent target1 = (HiveBoonDefinitionComponent) target;
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
    HiveBoonDefinitionComponent target1 = (HiveBoonDefinitionComponent) target;
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
    HiveBoonDefinitionComponent target1 = (HiveBoonDefinitionComponent) target;
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
  virtual HiveBoonDefinitionComponent Component.Instantiate() => new HiveBoonDefinitionComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class HiveBoonDefinitionComponent_AutoState : IComponentState
  {
    public int Cost;
    public int Pylons;
    public bool Reusable;
    public TimeSpan Duration;
    public TimeSpan Cooldown;
    public TimeSpan UnlockAt;
    public TimeSpan UnlockAtRandomAdd;
    public EntProtoId<HiveBoonDefinitionComponent>? DuplicateId;
    public bool NoLivingKing;
    public bool RequiresCore;
    public HiveBoonEvent? Event;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class HiveBoonDefinitionComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<HiveBoonDefinitionComponent, ComponentGetState>(new ComponentEventRefHandler<HiveBoonDefinitionComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<HiveBoonDefinitionComponent, ComponentHandleState>(new ComponentEventRefHandler<HiveBoonDefinitionComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      HiveBoonDefinitionComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new HiveBoonDefinitionComponent.HiveBoonDefinitionComponent_AutoState()
      {
        Cost = component.Cost,
        Pylons = component.Pylons,
        Reusable = component.Reusable,
        Duration = component.Duration,
        Cooldown = component.Cooldown,
        UnlockAt = component.UnlockAt,
        UnlockAtRandomAdd = component.UnlockAtRandomAdd,
        DuplicateId = component.DuplicateId,
        NoLivingKing = component.NoLivingKing,
        RequiresCore = component.RequiresCore,
        Event = component.Event
      };
    }

    private void OnHandleState(
      EntityUid uid,
      HiveBoonDefinitionComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is HiveBoonDefinitionComponent.HiveBoonDefinitionComponent_AutoState current))
        return;
      component.Cost = current.Cost;
      component.Pylons = current.Pylons;
      component.Reusable = current.Reusable;
      component.Duration = current.Duration;
      component.Cooldown = current.Cooldown;
      component.UnlockAt = current.UnlockAt;
      component.UnlockAtRandomAdd = current.UnlockAtRandomAdd;
      component.DuplicateId = current.DuplicateId;
      component.NoLivingKing = current.NoLivingKing;
      component.RequiresCore = current.RequiresCore;
      component.Event = current.Event;
    }
  }
}
