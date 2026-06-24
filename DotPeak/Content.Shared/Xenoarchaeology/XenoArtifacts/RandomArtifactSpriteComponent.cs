// Decompiled with JetBrains decompiler
// Type: Content.Shared.Xenoarchaeology.XenoArtifacts.RandomArtifactSpriteComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Xenoarchaeology.XenoArtifacts;

[RegisterComponent]
public sealed class RandomArtifactSpriteComponent : 
  Component,
  ISerializationGenerated<RandomArtifactSpriteComponent>,
  ISerializationGenerated
{
  [DataField("minSprite", false, 1, false, false, null)]
  public int MinSprite = 1;
  [DataField("maxSprite", false, 1, false, false, null)]
  public int MaxSprite = 14;
  [DataField("activationTime", false, 1, false, false, null)]
  public double ActivationTime = 0.4;
  public TimeSpan? ActivationStart;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RandomArtifactSpriteComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RandomArtifactSpriteComponent) target1;
    if (serialization.TryCustomCopy<RandomArtifactSpriteComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.MinSprite, ref target2, hookCtx, false, context))
      target2 = this.MinSprite;
    target.MinSprite = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxSprite, ref target3, hookCtx, false, context))
      target3 = this.MaxSprite;
    target.MaxSprite = target3;
    double target4 = 0.0;
    if (!serialization.TryCustomCopy<double>(this.ActivationTime, ref target4, hookCtx, false, context))
      target4 = this.ActivationTime;
    target.ActivationTime = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RandomArtifactSpriteComponent target,
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
    RandomArtifactSpriteComponent target1 = (RandomArtifactSpriteComponent) target;
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
    RandomArtifactSpriteComponent target1 = (RandomArtifactSpriteComponent) target;
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
    RandomArtifactSpriteComponent target1 = (RandomArtifactSpriteComponent) target;
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
  virtual RandomArtifactSpriteComponent Component.Instantiate()
  {
    return new RandomArtifactSpriteComponent();
  }
}
