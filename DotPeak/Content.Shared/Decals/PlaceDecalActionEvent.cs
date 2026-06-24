// Decompiled with JetBrains decompiler
// Type: Content.Shared.Decals.PlaceDecalActionEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Decals;

public sealed class PlaceDecalActionEvent : 
  WorldTargetActionEvent,
  ISerializationGenerated<PlaceDecalActionEvent>,
  ISerializationGenerated
{
  [DataField("decalId", false, 1, true, false, typeof (PrototypeIdSerializer<DecalPrototype>))]
  public string DecalId = string.Empty;
  [DataField("color", false, 1, false, false, null)]
  public Color Color;
  [DataField("rotation", false, 1, false, false, null)]
  public double Rotation;
  [DataField("snap", false, 1, false, false, null)]
  public bool Snap;
  [DataField("zIndex", false, 1, false, false, null)]
  public int ZIndex;
  [DataField("cleanable", false, 1, false, false, null)]
  public bool Cleanable;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PlaceDecalActionEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    WorldTargetActionEvent target1 = (WorldTargetActionEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PlaceDecalActionEvent) target1;
    if (serialization.TryCustomCopy<PlaceDecalActionEvent>(this, ref target, hookCtx, false, context))
      return;
    string str = (string) null;
    if (this.DecalId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.DecalId, ref str, hookCtx, false, context))
      str = this.DecalId;
    target.DecalId = str;
    Color color = new Color();
    if (!serialization.TryCustomCopy<Color>(this.Color, ref color, hookCtx, false, context))
      color = serialization.CreateCopy<Color>(this.Color, hookCtx, context, false);
    target.Color = color;
    double num1 = 0.0;
    if (!serialization.TryCustomCopy<double>(this.Rotation, ref num1, hookCtx, false, context))
      num1 = this.Rotation;
    target.Rotation = num1;
    bool flag1 = false;
    if (!serialization.TryCustomCopy<bool>(this.Snap, ref flag1, hookCtx, false, context))
      flag1 = this.Snap;
    target.Snap = flag1;
    int num2 = 0;
    if (!serialization.TryCustomCopy<int>(this.ZIndex, ref num2, hookCtx, false, context))
      num2 = this.ZIndex;
    target.ZIndex = num2;
    bool flag2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Cleanable, ref flag2, hookCtx, false, context))
      flag2 = this.Cleanable;
    target.Cleanable = flag2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PlaceDecalActionEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref WorldTargetActionEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    PlaceDecalActionEvent target1 = (PlaceDecalActionEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (WorldTargetActionEvent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    PlaceDecalActionEvent target1 = (PlaceDecalActionEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual PlaceDecalActionEvent WorldTargetActionEvent.Instantiate() => new PlaceDecalActionEvent();
}
