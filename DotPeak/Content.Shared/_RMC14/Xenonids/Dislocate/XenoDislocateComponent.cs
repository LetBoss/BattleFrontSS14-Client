// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Dislocate.XenoDislocateComponent
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
namespace Content.Shared._RMC14.Xenonids.Dislocate;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class XenoDislocateComponent : 
  Component,
  ISerializationGenerated<XenoDislocateComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier Sound = (SoundSpecifier) new SoundCollectionSpecifier("Punch");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId Effect = (EntProtoId) "CMEffectPunch";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float FlingRange = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan RootTime = TimeSpan.FromSeconds(1.2);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan CooldownReductionTime = TimeSpan.FromSeconds(5L);
  [DataField(null, false, 1, false, false, null)]
  public DamageSpecifier Damage = new DamageSpecifier();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoDislocateComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoDislocateComponent) target1;
    if (serialization.TryCustomCopy<XenoDislocateComponent>(this, ref target, hookCtx, false, context))
      return;
    SoundSpecifier target2 = (SoundSpecifier) null;
    if (this.Sound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target2;
    EntProtoId target3 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Effect, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntProtoId>(this.Effect, hookCtx, context);
    target.Effect = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.FlingRange, ref target4, hookCtx, false, context))
      target4 = this.FlingRange;
    target.FlingRange = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.RootTime, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.RootTime, hookCtx, context);
    target.RootTime = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.CooldownReductionTime, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.CooldownReductionTime, hookCtx, context);
    target.CooldownReductionTime = target6;
    DamageSpecifier target7 = (DamageSpecifier) null;
    if (this.Damage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.Damage, ref target7, hookCtx, false, context))
    {
      if (this.Damage == null)
        target7 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.Damage, ref target7, hookCtx, context, true);
    }
    target.Damage = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoDislocateComponent target,
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
    XenoDislocateComponent target1 = (XenoDislocateComponent) target;
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
    XenoDislocateComponent target1 = (XenoDislocateComponent) target;
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
    XenoDislocateComponent target1 = (XenoDislocateComponent) target;
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
  virtual XenoDislocateComponent Component.Instantiate() => new XenoDislocateComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoDislocateComponent_AutoState : IComponentState
  {
    public SoundSpecifier Sound;
    public EntProtoId Effect;
    public float FlingRange;
    public TimeSpan RootTime;
    public TimeSpan CooldownReductionTime;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoDislocateComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoDislocateComponent, ComponentGetState>(new ComponentEventRefHandler<XenoDislocateComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoDislocateComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoDislocateComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoDislocateComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoDislocateComponent.XenoDislocateComponent_AutoState()
      {
        Sound = component.Sound,
        Effect = component.Effect,
        FlingRange = component.FlingRange,
        RootTime = component.RootTime,
        CooldownReductionTime = component.CooldownReductionTime
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoDislocateComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoDislocateComponent.XenoDislocateComponent_AutoState current))
        return;
      component.Sound = current.Sound;
      component.Effect = current.Effect;
      component.FlingRange = current.FlingRange;
      component.RootTime = current.RootTime;
      component.CooldownReductionTime = current.CooldownReductionTime;
    }
  }
}
