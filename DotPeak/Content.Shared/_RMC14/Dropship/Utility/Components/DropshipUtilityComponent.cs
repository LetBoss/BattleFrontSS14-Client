// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Dropship.Utility.Components.DropshipUtilityComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Skills;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Dropship.Utility.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
public sealed class DropshipUtilityComponent : 
  Component,
  ISerializationGenerated<DropshipUtilityComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ActivateDelay = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan? NextActivateAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ActivateInTransport;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SkillWhitelist? Skills;
  public EntityUid? AttachmentPoint;
  [AutoNetworkedField]
  public EntityUid? Target;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DropshipUtilityComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (DropshipUtilityComponent) target1;
    if (serialization.TryCustomCopy<DropshipUtilityComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ActivateDelay, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.ActivateDelay, hookCtx, context);
    target.ActivateDelay = target2;
    TimeSpan? target3 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.NextActivateAt, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan?>(this.NextActivateAt, hookCtx, context);
    target.NextActivateAt = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.ActivateInTransport, ref target4, hookCtx, false, context))
      target4 = this.ActivateInTransport;
    target.ActivateInTransport = target4;
    SkillWhitelist target5 = (SkillWhitelist) null;
    if (!serialization.TryCustomCopy<SkillWhitelist>(this.Skills, ref target5, hookCtx, false, context))
    {
      if (this.Skills == null)
        target5 = (SkillWhitelist) null;
      else
        serialization.CopyTo<SkillWhitelist>(this.Skills, ref target5, hookCtx, context);
    }
    target.Skills = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DropshipUtilityComponent target,
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
    DropshipUtilityComponent target1 = (DropshipUtilityComponent) target;
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
    DropshipUtilityComponent target1 = (DropshipUtilityComponent) target;
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
    DropshipUtilityComponent target1 = (DropshipUtilityComponent) target;
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
  virtual DropshipUtilityComponent Component.Instantiate() => new DropshipUtilityComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DropshipUtilityComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<DropshipUtilityComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<DropshipUtilityComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      DropshipUtilityComponent component,
      ref EntityUnpausedEvent args)
    {
      if (component.NextActivateAt.HasValue)
        component.NextActivateAt = new TimeSpan?(component.NextActivateAt.Value + args.PausedTime);
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class DropshipUtilityComponent_AutoState : IComponentState
  {
    public TimeSpan ActivateDelay;
    public TimeSpan? NextActivateAt;
    public bool ActivateInTransport;
    public 
    #nullable enable
    SkillWhitelist? Skills;
    public NetEntity? Target;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DropshipUtilityComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<DropshipUtilityComponent, ComponentGetState>(new ComponentEventRefHandler<DropshipUtilityComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<DropshipUtilityComponent, ComponentHandleState>(new ComponentEventRefHandler<DropshipUtilityComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      DropshipUtilityComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new DropshipUtilityComponent.DropshipUtilityComponent_AutoState()
      {
        ActivateDelay = component.ActivateDelay,
        NextActivateAt = component.NextActivateAt,
        ActivateInTransport = component.ActivateInTransport,
        Skills = component.Skills,
        Target = this.GetNetEntity(component.Target)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      DropshipUtilityComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is DropshipUtilityComponent.DropshipUtilityComponent_AutoState current))
        return;
      component.ActivateDelay = current.ActivateDelay;
      component.NextActivateAt = current.NextActivateAt;
      component.ActivateInTransport = current.ActivateInTransport;
      component.Skills = current.Skills;
      component.Target = this.EnsureEntity<DropshipUtilityComponent>(current.Target, uid);
    }
  }
}
