// Decompiled with JetBrains decompiler
// Type: Content.Shared.Light.Components.SunShadowCastComponent
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
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Light.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class SunShadowCastComponent : 
  Component,
  ISerializationGenerated<SunShadowCastComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public Vector2[] Points = new Vector2[4]
  {
    new Vector2(-0.5f, -0.5f),
    new Vector2(0.5f, -0.5f),
    new Vector2(0.5f, 0.5f),
    new Vector2(-0.5f, 0.5f)
  };

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SunShadowCastComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SunShadowCastComponent) target1;
    if (serialization.TryCustomCopy<SunShadowCastComponent>(this, ref target, hookCtx, false, context))
      return;
    Vector2[] target2 = (Vector2[]) null;
    if (this.Points == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Vector2[]>(this.Points, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<Vector2[]>(this.Points, hookCtx, context);
    target.Points = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SunShadowCastComponent target,
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
    SunShadowCastComponent target1 = (SunShadowCastComponent) target;
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
    SunShadowCastComponent target1 = (SunShadowCastComponent) target;
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
    SunShadowCastComponent target1 = (SunShadowCastComponent) target;
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
  virtual SunShadowCastComponent Component.Instantiate() => new SunShadowCastComponent();
}
