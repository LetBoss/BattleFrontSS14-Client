// Decompiled with JetBrains decompiler
// Type: Content.Shared.Salvage.Fulton.FultonedComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Salvage.Fulton;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[AutoGenerateComponentPause]
public sealed class FultonedComponent : 
  Component,
  ISerializationGenerated<FultonedComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("beacon", false, 1, false, false, null)]
  public EntityUid? Beacon;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("fultonDuration", false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan FultonDuration = TimeSpan.FromSeconds(45L);
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("nextFulton", false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan NextFulton;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("sound", false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? Sound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Items/Mining/fultext_launch.ogg");
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("removeable", false, 1, false, false, null)]
  public bool Removeable = true;

  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField("effect", false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid Effect { get; set; }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref FultonedComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (FultonedComponent) target1;
    if (serialization.TryCustomCopy<FultonedComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid target2 = new EntityUid();
    if (!serialization.TryCustomCopy<EntityUid>(this.Effect, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityUid>(this.Effect, hookCtx, context);
    target.Effect = target2;
    EntityUid? target3 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Beacon, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntityUid?>(this.Beacon, hookCtx, context);
    target.Beacon = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.FultonDuration, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.FultonDuration, hookCtx, context);
    target.FultonDuration = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextFulton, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.NextFulton, hookCtx, context);
    target.NextFulton = target5;
    SoundSpecifier target6 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.Removeable, ref target7, hookCtx, false, context))
      target7 = this.Removeable;
    target.Removeable = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref FultonedComponent target,
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
    FultonedComponent target1 = (FultonedComponent) target;
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
    FultonedComponent target1 = (FultonedComponent) target;
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
    FultonedComponent target1 = (FultonedComponent) target;
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
  virtual FultonedComponent Component.Instantiate() => new FultonedComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class FultonedComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<FultonedComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<FultonedComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      FultonedComponent component,
      ref EntityUnpausedEvent args)
    {
      component.NextFulton += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class FultonedComponent_AutoState : IComponentState
  {
    public NetEntity Effect;
    public TimeSpan FultonDuration;
    public TimeSpan NextFulton;
    public 
    #nullable enable
    SoundSpecifier? Sound;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class FultonedComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<FultonedComponent, ComponentGetState>(new ComponentEventRefHandler<FultonedComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<FultonedComponent, ComponentHandleState>(new ComponentEventRefHandler<FultonedComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, FultonedComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new FultonedComponent.FultonedComponent_AutoState()
      {
        Effect = this.GetNetEntity(component.Effect),
        FultonDuration = component.FultonDuration,
        NextFulton = component.NextFulton,
        Sound = component.Sound
      };
    }

    private void OnHandleState(
      EntityUid uid,
      FultonedComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is FultonedComponent.FultonedComponent_AutoState current))
        return;
      component.Effect = this.EnsureEntity<FultonedComponent>(current.Effect, uid);
      component.FultonDuration = current.FultonDuration;
      component.NextFulton = current.NextFulton;
      component.Sound = current.Sound;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, FultonedComponent>(uid, component, ref args1);
    }
  }
}
