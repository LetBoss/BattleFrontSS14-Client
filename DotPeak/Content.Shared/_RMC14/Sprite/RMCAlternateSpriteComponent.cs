// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Sprite.RMCAlternateSpriteComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Sprite;

[RegisterComponent]
[NetworkedComponent]
public sealed class RMCAlternateSpriteComponent : 
  Component,
  ISerializationGenerated<RMCAlternateSpriteComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public string NormalSprite;
  [DataField(null, false, 1, false, false, null)]
  public string AlternateSprite;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCAlternateSpriteComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCAlternateSpriteComponent) target1;
    if (serialization.TryCustomCopy<RMCAlternateSpriteComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.NormalSprite == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.NormalSprite, ref target2, hookCtx, false, context))
      target2 = this.NormalSprite;
    target.NormalSprite = target2;
    string target3 = (string) null;
    if (this.AlternateSprite == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.AlternateSprite, ref target3, hookCtx, false, context))
      target3 = this.AlternateSprite;
    target.AlternateSprite = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCAlternateSpriteComponent target,
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
    RMCAlternateSpriteComponent target1 = (RMCAlternateSpriteComponent) target;
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
    RMCAlternateSpriteComponent target1 = (RMCAlternateSpriteComponent) target;
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
    RMCAlternateSpriteComponent target1 = (RMCAlternateSpriteComponent) target;
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
  virtual RMCAlternateSpriteComponent Component.Instantiate() => new RMCAlternateSpriteComponent();
}
