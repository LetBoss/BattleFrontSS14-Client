// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Punch.XenoPunchComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Punch;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (XenoPunchSystem)})]
public sealed class XenoPunchComponent : 
  Component,
  ISerializationGenerated<XenoPunchComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public DamageSpecifier Damage = new DamageSpecifier();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Range = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ThrowSpeed = 10f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId Effect = (EntProtoId) "CMEffectPunch";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier Sound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Xeno/alien_claw_block.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan SlowDuration = TimeSpan.FromSeconds(5L);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoPunchComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoPunchComponent) target1;
    if (serialization.TryCustomCopy<XenoPunchComponent>(this, ref target, hookCtx, false, context))
      return;
    DamageSpecifier target2 = (DamageSpecifier) null;
    if (this.Damage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.Damage, ref target2, hookCtx, false, context))
    {
      if (this.Damage == null)
        target2 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.Damage, ref target2, hookCtx, context, true);
    }
    target.Damage = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Range, ref target3, hookCtx, false, context))
      target3 = this.Range;
    target.Range = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ThrowSpeed, ref target4, hookCtx, false, context))
      target4 = this.ThrowSpeed;
    target.ThrowSpeed = target4;
    EntProtoId target5 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Effect, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntProtoId>(this.Effect, hookCtx, context);
    target.Effect = target5;
    SoundSpecifier target6 = (SoundSpecifier) null;
    if (this.Sound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.SlowDuration, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.SlowDuration, hookCtx, context);
    target.SlowDuration = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoPunchComponent target,
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
    XenoPunchComponent target1 = (XenoPunchComponent) target;
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
    XenoPunchComponent target1 = (XenoPunchComponent) target;
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
    XenoPunchComponent target1 = (XenoPunchComponent) target;
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
  virtual XenoPunchComponent Component.Instantiate() => new XenoPunchComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoPunchComponent_AutoState : IComponentState
  {
    public float Range;
    public float ThrowSpeed;
    public EntProtoId Effect;
    public SoundSpecifier Sound;
    public TimeSpan SlowDuration;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoPunchComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoPunchComponent, ComponentGetState>(new ComponentEventRefHandler<XenoPunchComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoPunchComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoPunchComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoPunchComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoPunchComponent.XenoPunchComponent_AutoState()
      {
        Range = component.Range,
        ThrowSpeed = component.ThrowSpeed,
        Effect = component.Effect,
        Sound = component.Sound,
        SlowDuration = component.SlowDuration
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoPunchComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoPunchComponent.XenoPunchComponent_AutoState current))
        return;
      component.Range = current.Range;
      component.ThrowSpeed = current.ThrowSpeed;
      component.Effect = current.Effect;
      component.Sound = current.Sound;
      component.SlowDuration = current.SlowDuration;
    }
  }
}
