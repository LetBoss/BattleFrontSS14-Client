// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Lunge.XenoLungeStunnedComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.StatusEffect;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Lunge;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (XenoLungeSystem)})]
public sealed class XenoLungeStunnedComponent : 
  Component,
  ISerializationGenerated<XenoLungeStunnedComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<StatusEffectPrototype>[] Effects = new ProtoId<StatusEffectPrototype>[2]
  {
    (ProtoId<StatusEffectPrototype>) "Stun",
    (ProtoId<StatusEffectPrototype>) "KnockedDown"
  };
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan ExpireAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public NetEntity? Stunner;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoLungeStunnedComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoLungeStunnedComponent) target1;
    if (serialization.TryCustomCopy<XenoLungeStunnedComponent>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<StatusEffectPrototype>[] target2 = (ProtoId<StatusEffectPrototype>[]) null;
    if (this.Effects == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<ProtoId<StatusEffectPrototype>[]>(this.Effects, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<ProtoId<StatusEffectPrototype>[]>(this.Effects, hookCtx, context);
    target.Effects = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ExpireAt, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.ExpireAt, hookCtx, context);
    target.ExpireAt = target3;
    NetEntity? target4 = new NetEntity?();
    if (!serialization.TryCustomCopy<NetEntity?>(this.Stunner, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<NetEntity?>(this.Stunner, hookCtx, context);
    target.Stunner = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoLungeStunnedComponent target,
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
    XenoLungeStunnedComponent target1 = (XenoLungeStunnedComponent) target;
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
    XenoLungeStunnedComponent target1 = (XenoLungeStunnedComponent) target;
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
    XenoLungeStunnedComponent target1 = (XenoLungeStunnedComponent) target;
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
  virtual XenoLungeStunnedComponent Component.Instantiate() => new XenoLungeStunnedComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoLungeStunnedComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoLungeStunnedComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<XenoLungeStunnedComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      XenoLungeStunnedComponent component,
      ref EntityUnpausedEvent args)
    {
      component.ExpireAt += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoLungeStunnedComponent_AutoState : IComponentState
  {
    public 
    #nullable enable
    ProtoId<StatusEffectPrototype>[] Effects;
    public TimeSpan ExpireAt;
    public NetEntity? Stunner;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoLungeStunnedComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoLungeStunnedComponent, ComponentGetState>(new ComponentEventRefHandler<XenoLungeStunnedComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoLungeStunnedComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoLungeStunnedComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoLungeStunnedComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoLungeStunnedComponent.XenoLungeStunnedComponent_AutoState()
      {
        Effects = component.Effects,
        ExpireAt = component.ExpireAt,
        Stunner = component.Stunner
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoLungeStunnedComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoLungeStunnedComponent.XenoLungeStunnedComponent_AutoState current))
        return;
      component.Effects = current.Effects;
      component.ExpireAt = current.ExpireAt;
      component.Stunner = current.Stunner;
    }
  }
}
