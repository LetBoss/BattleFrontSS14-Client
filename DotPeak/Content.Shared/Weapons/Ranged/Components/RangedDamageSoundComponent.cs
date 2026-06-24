// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Ranged.Components.RangedDamageSoundComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage.Prototypes;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Weapons.Ranged.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class RangedDamageSoundComponent : 
  Component,
  ISerializationGenerated<RangedDamageSoundComponent>,
  ISerializationGenerated
{
  [DataField("soundGroups", false, 1, false, false, typeof (PrototypeIdDictionarySerializer<SoundSpecifier, DamageGroupPrototype>))]
  public System.Collections.Generic.Dictionary<string, SoundSpecifier>? SoundGroups;
  [DataField("soundTypes", false, 1, false, false, typeof (PrototypeIdDictionarySerializer<SoundSpecifier, DamageTypePrototype>))]
  public System.Collections.Generic.Dictionary<string, SoundSpecifier>? SoundTypes;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RangedDamageSoundComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RangedDamageSoundComponent) target1;
    if (serialization.TryCustomCopy<RangedDamageSoundComponent>(this, ref target, hookCtx, false, context))
      return;
    System.Collections.Generic.Dictionary<string, SoundSpecifier> target2 = (System.Collections.Generic.Dictionary<string, SoundSpecifier>) null;
    if (!serialization.TryCustomCopy<System.Collections.Generic.Dictionary<string, SoundSpecifier>>(this.SoundGroups, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<System.Collections.Generic.Dictionary<string, SoundSpecifier>>(this.SoundGroups, hookCtx, context);
    target.SoundGroups = target2;
    System.Collections.Generic.Dictionary<string, SoundSpecifier> target3 = (System.Collections.Generic.Dictionary<string, SoundSpecifier>) null;
    if (!serialization.TryCustomCopy<System.Collections.Generic.Dictionary<string, SoundSpecifier>>(this.SoundTypes, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<System.Collections.Generic.Dictionary<string, SoundSpecifier>>(this.SoundTypes, hookCtx, context);
    target.SoundTypes = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RangedDamageSoundComponent target,
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
    RangedDamageSoundComponent target1 = (RangedDamageSoundComponent) target;
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
    RangedDamageSoundComponent target1 = (RangedDamageSoundComponent) target;
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
    RangedDamageSoundComponent target1 = (RangedDamageSoundComponent) target;
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
  virtual RangedDamageSoundComponent Component.Instantiate() => new RangedDamageSoundComponent();
}
