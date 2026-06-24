// Decompiled with JetBrains decompiler
// Type: Content.Client.Clickable.ClickableComponent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.Clickable;

[RegisterComponent]
public sealed class ClickableComponent : 
  Component,
  ISerializationGenerated<ClickableComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public ClickableComponent.DirBoundData? Bounds;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ClickableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (ClickableComponent) component;
    if (serialization.TryCustomCopy<ClickableComponent>(this, ref target, hookCtx, false, context))
      return;
    ClickableComponent.DirBoundData dirBoundData = (ClickableComponent.DirBoundData) null;
    if (!serialization.TryCustomCopy<ClickableComponent.DirBoundData>(this.Bounds, ref dirBoundData, hookCtx, false, context))
    {
      if (this.Bounds == null)
        dirBoundData = (ClickableComponent.DirBoundData) null;
      else
        serialization.CopyTo<ClickableComponent.DirBoundData>(this.Bounds, ref dirBoundData, hookCtx, context, false);
    }
    target.Bounds = dirBoundData;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ClickableComponent target,
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
    ClickableComponent target1 = (ClickableComponent) target;
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
    ClickableComponent target1 = (ClickableComponent) target;
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
    ClickableComponent target1 = (ClickableComponent) target;
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
  virtual ClickableComponent Component.Instantiate() => new ClickableComponent();

  [DataDefinition]
  public sealed class DirBoundData : 
    ISerializationGenerated<ClickableComponent.DirBoundData>,
    ISerializationGenerated
  {
    [DataField(null, false, 1, false, false, null)]
    public Box2 All;
    [DataField(null, false, 1, false, false, null)]
    public Box2 North;
    [DataField(null, false, 1, false, false, null)]
    public Box2 South;
    [DataField(null, false, 1, false, false, null)]
    public Box2 East;
    [DataField(null, false, 1, false, false, null)]
    public Box2 West;

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void InternalCopy(
      ref ClickableComponent.DirBoundData target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      if (serialization.TryCustomCopy<ClickableComponent.DirBoundData>(this, ref target, hookCtx, false, context))
        return;
      Box2 box2_1 = new Box2();
      if (!serialization.TryCustomCopy<Box2>(this.All, ref box2_1, hookCtx, false, context))
        box2_1 = serialization.CreateCopy<Box2>(this.All, hookCtx, context, false);
      target.All = box2_1;
      Box2 box2_2 = new Box2();
      if (!serialization.TryCustomCopy<Box2>(this.North, ref box2_2, hookCtx, false, context))
        box2_2 = serialization.CreateCopy<Box2>(this.North, hookCtx, context, false);
      target.North = box2_2;
      Box2 box2_3 = new Box2();
      if (!serialization.TryCustomCopy<Box2>(this.South, ref box2_3, hookCtx, false, context))
        box2_3 = serialization.CreateCopy<Box2>(this.South, hookCtx, context, false);
      target.South = box2_3;
      Box2 box2_4 = new Box2();
      if (!serialization.TryCustomCopy<Box2>(this.East, ref box2_4, hookCtx, false, context))
        box2_4 = serialization.CreateCopy<Box2>(this.East, hookCtx, context, false);
      target.East = box2_4;
      Box2 box2_5 = new Box2();
      if (!serialization.TryCustomCopy<Box2>(this.West, ref box2_5, hookCtx, false, context))
        box2_5 = serialization.CreateCopy<Box2>(this.West, hookCtx, context, false);
      target.West = box2_5;
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void Copy(
      ref ClickableComponent.DirBoundData target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      this.InternalCopy(ref target, serialization, hookCtx, context);
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void Copy(
      ref object target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      ClickableComponent.DirBoundData target1 = (ClickableComponent.DirBoundData) target;
      this.Copy(ref target1, serialization, hookCtx, context);
      target = (object) target1;
    }

    [Obsolete("Use ISerializationManager.CreateCopy instead")]
    public ClickableComponent.DirBoundData Instantiate() => new ClickableComponent.DirBoundData();
  }
}
