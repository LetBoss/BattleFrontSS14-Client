// Decompiled with JetBrains decompiler
// Type: Content.Shared.Medical.Cryogenics.InsideCryoPodComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Medical.Cryogenics;

[RegisterComponent]
[NetworkedComponent]
public sealed class InsideCryoPodComponent : 
  Component,
  ISerializationGenerated<InsideCryoPodComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField("previousOffset", false, 1, false, false, null)]
  public Vector2 PreviousOffset { get; set; } = new Vector2(0.0f, 0.0f);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref InsideCryoPodComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (InsideCryoPodComponent) target1;
    if (serialization.TryCustomCopy<InsideCryoPodComponent>(this, ref target, hookCtx, false, context))
      return;
    Vector2 target2 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.PreviousOffset, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<Vector2>(this.PreviousOffset, hookCtx, context);
    target.PreviousOffset = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref InsideCryoPodComponent target,
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
    InsideCryoPodComponent target1 = (InsideCryoPodComponent) target;
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
    InsideCryoPodComponent target1 = (InsideCryoPodComponent) target;
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
    InsideCryoPodComponent target1 = (InsideCryoPodComponent) target;
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
  virtual InsideCryoPodComponent Component.Instantiate() => new InsideCryoPodComponent();
}
