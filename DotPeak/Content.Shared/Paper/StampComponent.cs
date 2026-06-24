// Decompiled with JetBrains decompiler
// Type: Content.Shared.Paper.StampComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Paper;

[RegisterComponent]
public sealed class StampComponent : 
  Component,
  ISerializationGenerated<StampComponent>,
  ISerializationGenerated
{
  [DataField("stampedColor", false, 1, false, false, null)]
  public Color StampedColor = Color.FromHex((ReadOnlySpan<char>) "#BB3232", new Color?());
  [DataField("sound", false, 1, false, false, null)]
  public SoundSpecifier? Sound;

  [DataField("stampedName", false, 1, false, false, null)]
  public string StampedName { get; set; } = "stamp-component-stamped-name-default";

  [DataField("stampState", false, 1, false, false, null)]
  public string StampState { get; set; } = "paper_stamp-generic";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref StampComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (StampComponent) target1;
    if (serialization.TryCustomCopy<StampComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.StampedName == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.StampedName, ref target2, hookCtx, false, context))
      target2 = this.StampedName;
    target.StampedName = target2;
    string target3 = (string) null;
    if (this.StampState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.StampState, ref target3, hookCtx, false, context))
      target3 = this.StampState;
    target.StampState = target3;
    Color target4 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.StampedColor, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<Color>(this.StampedColor, hookCtx, context);
    target.StampedColor = target4;
    SoundSpecifier target5 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref StampComponent target,
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
    StampComponent target1 = (StampComponent) target;
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
    StampComponent target1 = (StampComponent) target;
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
    StampComponent target1 = (StampComponent) target;
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
  virtual StampComponent Component.Instantiate() => new StampComponent();
}
