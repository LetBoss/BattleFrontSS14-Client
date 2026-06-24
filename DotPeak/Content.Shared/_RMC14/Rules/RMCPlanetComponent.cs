// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Rules.RMCPlanetComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Rules;

[RegisterComponent]
[NetworkedComponent]
public sealed class RMCPlanetComponent : 
  Component,
  ISerializationGenerated<RMCPlanetComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public Vector2i Offset;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCPlanetComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCPlanetComponent) target1;
    if (serialization.TryCustomCopy<RMCPlanetComponent>(this, ref target, hookCtx, false, context))
      return;
    Vector2i target2 = new Vector2i();
    if (!serialization.TryCustomCopy<Vector2i>(this.Offset, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<Vector2i>(this.Offset, hookCtx, context);
    target.Offset = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCPlanetComponent target,
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
    RMCPlanetComponent target1 = (RMCPlanetComponent) target;
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
    RMCPlanetComponent target1 = (RMCPlanetComponent) target;
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
    RMCPlanetComponent target1 = (RMCPlanetComponent) target;
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
  virtual RMCPlanetComponent Component.Instantiate() => new RMCPlanetComponent();
}
