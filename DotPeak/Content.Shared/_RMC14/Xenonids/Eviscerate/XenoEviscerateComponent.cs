// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Eviscerate.XenoEviscerateComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chat.Prototypes;
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Eviscerate;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class XenoEviscerateComponent : 
  Component,
  ISerializationGenerated<XenoEviscerateComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public List<DamageSpecifier> DamageAtRageLevels;
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public List<float> RangeAtRageLevels;
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public List<TimeSpan> WindupReductionAtRageLevels;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int LifeStealPerMarine = 50;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MaxLifeSteal = 250;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan WindupTime = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan StunTime = TimeSpan.FromSeconds(1.25);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan HealDelay = TimeSpan.FromSeconds(0.05);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier Sound = (SoundSpecifier) new SoundCollectionSpecifier("XenoTailSwipe");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier HitSound = (SoundSpecifier) new SoundCollectionSpecifier("RCMXenoClaw");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier RageHitSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Effects/gibbed.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<EmotePrototype> RoarEmote = (ProtoId<EmotePrototype>) "XenoRoar";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoEviscerateComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoEviscerateComponent) target1;
    if (serialization.TryCustomCopy<XenoEviscerateComponent>(this, ref target, hookCtx, false, context))
      return;
    List<DamageSpecifier> target2 = (List<DamageSpecifier>) null;
    if (this.DamageAtRageLevels == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<DamageSpecifier>>(this.DamageAtRageLevels, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<DamageSpecifier>>(this.DamageAtRageLevels, hookCtx, context);
    target.DamageAtRageLevels = target2;
    List<float> target3 = (List<float>) null;
    if (this.RangeAtRageLevels == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<float>>(this.RangeAtRageLevels, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<List<float>>(this.RangeAtRageLevels, hookCtx, context);
    target.RangeAtRageLevels = target3;
    List<TimeSpan> target4 = (List<TimeSpan>) null;
    if (this.WindupReductionAtRageLevels == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<TimeSpan>>(this.WindupReductionAtRageLevels, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<List<TimeSpan>>(this.WindupReductionAtRageLevels, hookCtx, context);
    target.WindupReductionAtRageLevels = target4;
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.LifeStealPerMarine, ref target5, hookCtx, false, context))
      target5 = this.LifeStealPerMarine;
    target.LifeStealPerMarine = target5;
    int target6 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxLifeSteal, ref target6, hookCtx, false, context))
      target6 = this.MaxLifeSteal;
    target.MaxLifeSteal = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.WindupTime, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.WindupTime, hookCtx, context);
    target.WindupTime = target7;
    TimeSpan target8 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.StunTime, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<TimeSpan>(this.StunTime, hookCtx, context);
    target.StunTime = target8;
    TimeSpan target9 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.HealDelay, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<TimeSpan>(this.HealDelay, hookCtx, context);
    target.HealDelay = target9;
    SoundSpecifier target10 = (SoundSpecifier) null;
    if (this.Sound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target10, hookCtx, true, context))
      target10 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target10;
    SoundSpecifier target11 = (SoundSpecifier) null;
    if (this.HitSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.HitSound, ref target11, hookCtx, true, context))
      target11 = serialization.CreateCopy<SoundSpecifier>(this.HitSound, hookCtx, context);
    target.HitSound = target11;
    SoundSpecifier target12 = (SoundSpecifier) null;
    if (this.RageHitSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.RageHitSound, ref target12, hookCtx, true, context))
      target12 = serialization.CreateCopy<SoundSpecifier>(this.RageHitSound, hookCtx, context);
    target.RageHitSound = target12;
    ProtoId<EmotePrototype> target13 = new ProtoId<EmotePrototype>();
    if (!serialization.TryCustomCopy<ProtoId<EmotePrototype>>(this.RoarEmote, ref target13, hookCtx, false, context))
      target13 = serialization.CreateCopy<ProtoId<EmotePrototype>>(this.RoarEmote, hookCtx, context);
    target.RoarEmote = target13;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoEviscerateComponent target,
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
    XenoEviscerateComponent target1 = (XenoEviscerateComponent) target;
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
    XenoEviscerateComponent target1 = (XenoEviscerateComponent) target;
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
    XenoEviscerateComponent target1 = (XenoEviscerateComponent) target;
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
  virtual XenoEviscerateComponent Component.Instantiate() => new XenoEviscerateComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoEviscerateComponent_AutoState : IComponentState
  {
    public List<DamageSpecifier> DamageAtRageLevels;
    public List<float> RangeAtRageLevels;
    public List<TimeSpan> WindupReductionAtRageLevels;
    public int LifeStealPerMarine;
    public int MaxLifeSteal;
    public TimeSpan WindupTime;
    public TimeSpan StunTime;
    public TimeSpan HealDelay;
    public SoundSpecifier Sound;
    public SoundSpecifier HitSound;
    public SoundSpecifier RageHitSound;
    public ProtoId<EmotePrototype> RoarEmote;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoEviscerateComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoEviscerateComponent, ComponentGetState>(new ComponentEventRefHandler<XenoEviscerateComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoEviscerateComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoEviscerateComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoEviscerateComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoEviscerateComponent.XenoEviscerateComponent_AutoState()
      {
        DamageAtRageLevels = component.DamageAtRageLevels,
        RangeAtRageLevels = component.RangeAtRageLevels,
        WindupReductionAtRageLevels = component.WindupReductionAtRageLevels,
        LifeStealPerMarine = component.LifeStealPerMarine,
        MaxLifeSteal = component.MaxLifeSteal,
        WindupTime = component.WindupTime,
        StunTime = component.StunTime,
        HealDelay = component.HealDelay,
        Sound = component.Sound,
        HitSound = component.HitSound,
        RageHitSound = component.RageHitSound,
        RoarEmote = component.RoarEmote
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoEviscerateComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoEviscerateComponent.XenoEviscerateComponent_AutoState current))
        return;
      component.DamageAtRageLevels = current.DamageAtRageLevels == null ? (List<DamageSpecifier>) null : new List<DamageSpecifier>((IEnumerable<DamageSpecifier>) current.DamageAtRageLevels);
      component.RangeAtRageLevels = current.RangeAtRageLevels == null ? (List<float>) null : new List<float>((IEnumerable<float>) current.RangeAtRageLevels);
      component.WindupReductionAtRageLevels = current.WindupReductionAtRageLevels == null ? (List<TimeSpan>) null : new List<TimeSpan>((IEnumerable<TimeSpan>) current.WindupReductionAtRageLevels);
      component.LifeStealPerMarine = current.LifeStealPerMarine;
      component.MaxLifeSteal = current.MaxLifeSteal;
      component.WindupTime = current.WindupTime;
      component.StunTime = current.StunTime;
      component.HealDelay = current.HealDelay;
      component.Sound = current.Sound;
      component.HitSound = current.HitSound;
      component.RageHitSound = current.RageHitSound;
      component.RoarEmote = current.RoarEmote;
    }
  }
}
