// Decompiled with JetBrains decompiler
// Type: Content.Shared.StatusEffectNew.Components.StatusEffectComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Alert;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.StatusEffectNew.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedStatusEffectsSystem)})]
[EntityCategory(new string[] {"StatusEffects"})]
public sealed class StatusEffectComponent : 
  Component,
  ISerializationGenerated<StatusEffectComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? AppliedTo;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<AlertPrototype>? Alert;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoPausedField]
  [AutoNetworkedField]
  public TimeSpan? EndEffectTime;
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? Whitelist;
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? Blacklist;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref StatusEffectComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (StatusEffectComponent) target1;
    if (serialization.TryCustomCopy<StatusEffectComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid? target2 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.AppliedTo, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityUid?>(this.AppliedTo, hookCtx, context);
    target.AppliedTo = target2;
    ProtoId<AlertPrototype>? target3 = new ProtoId<AlertPrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<AlertPrototype>?>(this.Alert, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<ProtoId<AlertPrototype>?>(this.Alert, hookCtx, context);
    target.Alert = target3;
    TimeSpan? target4 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.EndEffectTime, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan?>(this.EndEffectTime, hookCtx, context);
    target.EndEffectTime = target4;
    EntityWhitelist target5 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref target5, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        target5 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref target5, hookCtx, context);
    }
    target.Whitelist = target5;
    EntityWhitelist target6 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Blacklist, ref target6, hookCtx, false, context))
    {
      if (this.Blacklist == null)
        target6 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Blacklist, ref target6, hookCtx, context);
    }
    target.Blacklist = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref StatusEffectComponent target,
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
    StatusEffectComponent target1 = (StatusEffectComponent) target;
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
    StatusEffectComponent target1 = (StatusEffectComponent) target;
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
    StatusEffectComponent target1 = (StatusEffectComponent) target;
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
  virtual StatusEffectComponent Component.Instantiate() => new StatusEffectComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class StatusEffectComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<StatusEffectComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<StatusEffectComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      StatusEffectComponent component,
      ref EntityUnpausedEvent args)
    {
      if (component.EndEffectTime.HasValue)
        component.EndEffectTime = new TimeSpan?(component.EndEffectTime.Value + args.PausedTime);
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class StatusEffectComponent_AutoState : IComponentState
  {
    public NetEntity? AppliedTo;
    public TimeSpan? EndEffectTime;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class StatusEffectComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<StatusEffectComponent, ComponentGetState>(new ComponentEventRefHandler<StatusEffectComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<StatusEffectComponent, ComponentHandleState>(new ComponentEventRefHandler<StatusEffectComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      #nullable enable
      StatusEffectComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new StatusEffectComponent.StatusEffectComponent_AutoState()
      {
        AppliedTo = this.GetNetEntity(component.AppliedTo),
        EndEffectTime = component.EndEffectTime
      };
    }

    private void OnHandleState(
      EntityUid uid,
      StatusEffectComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is StatusEffectComponent.StatusEffectComponent_AutoState current))
        return;
      component.AppliedTo = this.EnsureEntity<StatusEffectComponent>(current.AppliedTo, uid);
      component.EndEffectTime = current.EndEffectTime;
    }
  }
}
