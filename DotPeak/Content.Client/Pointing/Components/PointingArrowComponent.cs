// Decompiled with JetBrains decompiler
// Type: Content.Client.Pointing.Components.PointingArrowComponent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Pointing.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.Pointing.Components;

[RegisterComponent]
public sealed class PointingArrowComponent : 
  SharedPointingArrowComponent,
  ISerializationGenerated<PointingArrowComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField("offset", false, 1, false, false, null)]
  public Vector2 Offset = new Vector2(0.0f, 0.25f);
  public readonly string AnimationKey = "pointingarrow";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PointingArrowComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SharedPointingArrowComponent target1 = (SharedPointingArrowComponent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PointingArrowComponent) target1;
    if (serialization.TryCustomCopy<PointingArrowComponent>(this, ref target, hookCtx, false, context))
      return;
    Vector2 vector2 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.Offset, ref vector2, hookCtx, false, context))
      vector2 = serialization.CreateCopy<Vector2>(this.Offset, hookCtx, context, false);
    target.Offset = vector2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PointingArrowComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref SharedPointingArrowComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    PointingArrowComponent target1 = (PointingArrowComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (SharedPointingArrowComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    PointingArrowComponent target1 = (PointingArrowComponent) target;
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
    PointingArrowComponent target1 = (PointingArrowComponent) target;
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
    base.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual PointingArrowComponent SharedPointingArrowComponent.Instantiate()
  {
    return new PointingArrowComponent();
  }
}
