// Decompiled with JetBrains decompiler
// Type: Content.Shared.Nutrition.Components.UtensilComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Nutrition.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Nutrition.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (UtensilSystem)})]
public sealed class UtensilComponent : 
  Component,
  ISerializationGenerated<UtensilComponent>,
  ISerializationGenerated
{
  [DataField("types", false, 1, false, false, null)]
  private UtensilType _types;
  [DataField("breakChance", false, 1, false, false, null)]
  public float BreakChance;
  [DataField("breakSound", false, 1, false, false, null)]
  public SoundSpecifier BreakSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Items/snap.ogg");

  [Robust.Shared.ViewVariables.ViewVariables]
  public UtensilType Types
  {
    get => this._types;
    set
    {
      if (this._types.Equals((object) value))
        return;
      this._types = value;
    }
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref UtensilComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (UtensilComponent) target1;
    if (serialization.TryCustomCopy<UtensilComponent>(this, ref target, hookCtx, false, context))
      return;
    UtensilType target2 = UtensilType.None;
    if (!serialization.TryCustomCopy<UtensilType>(this._types, ref target2, hookCtx, false, context))
      target2 = this._types;
    target._types = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BreakChance, ref target3, hookCtx, false, context))
      target3 = this.BreakChance;
    target.BreakChance = target3;
    SoundSpecifier target4 = (SoundSpecifier) null;
    if (this.BreakSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.BreakSound, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<SoundSpecifier>(this.BreakSound, hookCtx, context);
    target.BreakSound = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref UtensilComponent target,
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
    UtensilComponent target1 = (UtensilComponent) target;
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
    UtensilComponent target1 = (UtensilComponent) target;
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
    UtensilComponent target1 = (UtensilComponent) target;
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
  virtual UtensilComponent Component.Instantiate() => new UtensilComponent();
}
