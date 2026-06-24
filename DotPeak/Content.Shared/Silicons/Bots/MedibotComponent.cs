// Decompiled with JetBrains decompiler
// Type: Content.Shared.Silicons.Bots.MedibotComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Mobs;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Silicons.Bots;

[RegisterComponent]
[Access(new Type[] {typeof (MedibotSystem)})]
public sealed class MedibotComponent : 
  Component,
  ISerializationGenerated<MedibotComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public Dictionary<MobState, MedibotTreatment> Treatments = new Dictionary<MobState, MedibotTreatment>();
  [DataField("injectSound", false, 1, false, false, null)]
  public SoundSpecifier InjectSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Items/hypospray.ogg");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MedibotComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MedibotComponent) target1;
    if (serialization.TryCustomCopy<MedibotComponent>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<MobState, MedibotTreatment> target2 = (Dictionary<MobState, MedibotTreatment>) null;
    if (this.Treatments == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<MobState, MedibotTreatment>>(this.Treatments, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<Dictionary<MobState, MedibotTreatment>>(this.Treatments, hookCtx, context);
    target.Treatments = target2;
    SoundSpecifier target3 = (SoundSpecifier) null;
    if (this.InjectSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.InjectSound, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<SoundSpecifier>(this.InjectSound, hookCtx, context);
    target.InjectSound = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MedibotComponent target,
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
    MedibotComponent target1 = (MedibotComponent) target;
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
    MedibotComponent target1 = (MedibotComponent) target;
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
    MedibotComponent target1 = (MedibotComponent) target;
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
  virtual MedibotComponent Component.Instantiate() => new MedibotComponent();
}
