// Decompiled with JetBrains decompiler
// Type: Content.Shared.MapText.SharedMapTextComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.MapText;

[NetworkedComponent]
[Access(new Type[] {typeof (SharedMapTextSystem)})]
public abstract class SharedMapTextComponent : 
  Component,
  ISerializationGenerated<SharedMapTextComponent>,
  ISerializationGenerated
{
  public const string DefaultFont = "Default";
  [DataField(null, false, 1, false, false, null)]
  public string? Text;
  [DataField(null, false, 1, false, false, null)]
  public LocId LocText = (LocId) "map-text-default";
  [DataField(null, false, 1, false, false, null)]
  public Color Color = Color.White;
  [DataField(null, false, 1, false, false, null)]
  public string FontId = "Default";
  [DataField(null, false, 1, false, false, null)]
  public int FontSize = 12;
  [DataField(null, false, 1, false, false, null)]
  public Vector2 Offset = Vector2.Zero;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref SharedMapTextComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SharedMapTextComponent) target1;
    if (serialization.TryCustomCopy<SharedMapTextComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.Text, ref target2, hookCtx, false, context))
      target2 = this.Text;
    target.Text = target2;
    LocId target3 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.LocText, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<LocId>(this.LocText, hookCtx, context);
    target.LocText = target3;
    Color target4 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.Color, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<Color>(this.Color, hookCtx, context);
    target.Color = target4;
    string target5 = (string) null;
    if (this.FontId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.FontId, ref target5, hookCtx, false, context))
      target5 = this.FontId;
    target.FontId = target5;
    int target6 = 0;
    if (!serialization.TryCustomCopy<int>(this.FontSize, ref target6, hookCtx, false, context))
      target6 = this.FontSize;
    target.FontSize = target6;
    Vector2 target7 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.Offset, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<Vector2>(this.Offset, hookCtx, context);
    target.Offset = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref SharedMapTextComponent target,
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
    SharedMapTextComponent target1 = (SharedMapTextComponent) target;
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
    SharedMapTextComponent target1 = (SharedMapTextComponent) target;
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
    SharedMapTextComponent target1 = (SharedMapTextComponent) target;
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
  virtual SharedMapTextComponent Component.Instantiate() => throw new NotImplementedException();
}
