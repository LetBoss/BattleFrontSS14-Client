// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Impale.XenoSecondImpaleComponent
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
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Impale;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (XenoImpaleSystem)})]
public sealed class XenoSecondImpaleComponent : 
  Component,
  ISerializationGenerated<XenoSecondImpaleComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan ImpaleAt;
  [DataField(null, false, 1, false, false, null)]
  public DamageSpecifier Damage;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId Animation = (EntProtoId) "RMCEffectTailHit";
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier Sound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Xeno/alien_tail_attack.ogg");
  [DataField(null, false, 1, false, false, null)]
  public int AP = 10;
  [DataField(null, false, 1, false, false, null)]
  public EntityUid Origin;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoSecondImpaleComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoSecondImpaleComponent) target1;
    if (serialization.TryCustomCopy<XenoSecondImpaleComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ImpaleAt, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.ImpaleAt, hookCtx, context);
    target.ImpaleAt = target2;
    DamageSpecifier target3 = (DamageSpecifier) null;
    if (this.Damage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.Damage, ref target3, hookCtx, false, context))
    {
      if (this.Damage == null)
        target3 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.Damage, ref target3, hookCtx, context, true);
    }
    target.Damage = target3;
    EntProtoId target4 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Animation, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntProtoId>(this.Animation, hookCtx, context);
    target.Animation = target4;
    SoundSpecifier target5 = (SoundSpecifier) null;
    if (this.Sound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target5;
    int target6 = 0;
    if (!serialization.TryCustomCopy<int>(this.AP, ref target6, hookCtx, false, context))
      target6 = this.AP;
    target.AP = target6;
    EntityUid target7 = new EntityUid();
    if (!serialization.TryCustomCopy<EntityUid>(this.Origin, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<EntityUid>(this.Origin, hookCtx, context);
    target.Origin = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoSecondImpaleComponent target,
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
    XenoSecondImpaleComponent target1 = (XenoSecondImpaleComponent) target;
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
    XenoSecondImpaleComponent target1 = (XenoSecondImpaleComponent) target;
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
    XenoSecondImpaleComponent target1 = (XenoSecondImpaleComponent) target;
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
  virtual XenoSecondImpaleComponent Component.Instantiate() => new XenoSecondImpaleComponent();
}
