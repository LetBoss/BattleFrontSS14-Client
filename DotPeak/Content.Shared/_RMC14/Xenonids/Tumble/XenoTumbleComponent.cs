// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Tumble.XenoTumbleComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Tumble;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (XenoTumbleSystem)})]
public sealed class XenoTumbleComponent : 
  Component,
  ISerializationGenerated<XenoTumbleComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Range = 2f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier Sound = (SoundSpecifier) new SoundCollectionSpecifier("XenoTailSwipe");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2? Target;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan StunTime = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ImpactRange = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier Damage = new DamageSpecifier();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int ArmorPiercing = 100;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoTumbleComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoTumbleComponent) target1;
    if (serialization.TryCustomCopy<XenoTumbleComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Range, ref target2, hookCtx, false, context))
      target2 = this.Range;
    target.Range = target2;
    SoundSpecifier target3 = (SoundSpecifier) null;
    if (this.Sound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target3;
    Vector2? target4 = new Vector2?();
    if (!serialization.TryCustomCopy<Vector2?>(this.Target, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<Vector2?>(this.Target, hookCtx, context);
    target.Target = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.StunTime, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.StunTime, hookCtx, context);
    target.StunTime = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ImpactRange, ref target6, hookCtx, false, context))
      target6 = this.ImpactRange;
    target.ImpactRange = target6;
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
    int target8 = 0;
    if (!serialization.TryCustomCopy<int>(this.ArmorPiercing, ref target8, hookCtx, false, context))
      target8 = this.ArmorPiercing;
    target.ArmorPiercing = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoTumbleComponent target,
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
    XenoTumbleComponent target1 = (XenoTumbleComponent) target;
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
    XenoTumbleComponent target1 = (XenoTumbleComponent) target;
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
    XenoTumbleComponent target1 = (XenoTumbleComponent) target;
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
  virtual XenoTumbleComponent Component.Instantiate() => new XenoTumbleComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoTumbleComponent_AutoState : IComponentState
  {
    public float Range;
    public SoundSpecifier Sound;
    public Vector2? Target;
    public TimeSpan StunTime;
    public float ImpactRange;
    public DamageSpecifier Damage;
    public int ArmorPiercing;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoTumbleComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoTumbleComponent, ComponentGetState>(new ComponentEventRefHandler<XenoTumbleComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoTumbleComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoTumbleComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoTumbleComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoTumbleComponent.XenoTumbleComponent_AutoState()
      {
        Range = component.Range,
        Sound = component.Sound,
        Target = component.Target,
        StunTime = component.StunTime,
        ImpactRange = component.ImpactRange,
        Damage = component.Damage,
        ArmorPiercing = component.ArmorPiercing
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoTumbleComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoTumbleComponent.XenoTumbleComponent_AutoState current))
        return;
      component.Range = current.Range;
      component.Sound = current.Sound;
      component.Target = current.Target;
      component.StunTime = current.StunTime;
      component.ImpactRange = current.ImpactRange;
      component.Damage = current.Damage;
      component.ArmorPiercing = current.ArmorPiercing;
    }
  }
}
