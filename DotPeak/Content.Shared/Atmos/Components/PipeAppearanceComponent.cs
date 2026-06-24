// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.Components.PipeAppearanceComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Atmos.Components;

[RegisterComponent]
public sealed class PipeAppearanceComponent : 
  Component,
  ISerializationGenerated<PipeAppearanceComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public SpriteSpecifier.Rsi[] Sprite = new SpriteSpecifier.Rsi[3]
  {
    new SpriteSpecifier.Rsi(new ResPath("Structures/Piping/Atmospherics/pipe.rsi"), "pipeConnector"),
    new SpriteSpecifier.Rsi(new ResPath("Structures/Piping/Atmospherics/pipe_alt1.rsi"), "pipeConnector"),
    new SpriteSpecifier.Rsi(new ResPath("Structures/Piping/Atmospherics/pipe_alt2.rsi"), "pipeConnector")
  };

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PipeAppearanceComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (PipeAppearanceComponent) component;
    if (serialization.TryCustomCopy<PipeAppearanceComponent>(this, ref target, hookCtx, false, context))
      return;
    SpriteSpecifier.Rsi[] rsiArray = (SpriteSpecifier.Rsi[]) null;
    if (this.Sprite == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SpriteSpecifier.Rsi[]>(this.Sprite, ref rsiArray, hookCtx, true, context))
      rsiArray = serialization.CreateCopy<SpriteSpecifier.Rsi[]>(this.Sprite, hookCtx, context, false);
    target.Sprite = rsiArray;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PipeAppearanceComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    PipeAppearanceComponent target1 = (PipeAppearanceComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    PipeAppearanceComponent target1 = (PipeAppearanceComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    PipeAppearanceComponent target1 = (PipeAppearanceComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    base.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual PipeAppearanceComponent Component.Instantiate() => new PipeAppearanceComponent();
}
