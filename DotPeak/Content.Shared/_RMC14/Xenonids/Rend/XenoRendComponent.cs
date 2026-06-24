// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Rend.XenoRendComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chat.Prototypes;
using Content.Shared.Damage;
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
namespace Content.Shared._RMC14.Xenonids.Rend;

[RegisterComponent]
[NetworkedComponent]
public sealed class XenoRendComponent : 
  Component,
  ISerializationGenerated<XenoRendComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public DamageSpecifier Damage = new DamageSpecifier();
  [DataField(null, false, 1, false, false, null)]
  public float Range = 1.5f;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId Effect = (EntProtoId) "RMCEffectExtraSlash";
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier Sound = (SoundSpecifier) new SoundCollectionSpecifier("AlienClaw");
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<EmotePrototype> HissEmote = (ProtoId<EmotePrototype>) "Hiss";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoRendComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoRendComponent) target1;
    if (serialization.TryCustomCopy<XenoRendComponent>(this, ref target, hookCtx, false, context))
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
    EntProtoId target4 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Effect, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntProtoId>(this.Effect, hookCtx, context);
    target.Effect = target4;
    SoundSpecifier target5 = (SoundSpecifier) null;
    if (this.Sound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target5;
    ProtoId<EmotePrototype> target6 = new ProtoId<EmotePrototype>();
    if (!serialization.TryCustomCopy<ProtoId<EmotePrototype>>(this.HissEmote, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<ProtoId<EmotePrototype>>(this.HissEmote, hookCtx, context);
    target.HissEmote = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoRendComponent target,
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
    XenoRendComponent target1 = (XenoRendComponent) target;
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
    XenoRendComponent target1 = (XenoRendComponent) target;
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
    XenoRendComponent target1 = (XenoRendComponent) target;
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
  virtual XenoRendComponent Component.Instantiate() => new XenoRendComponent();
}
