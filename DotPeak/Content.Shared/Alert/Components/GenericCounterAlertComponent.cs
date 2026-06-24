// Decompiled with JetBrains decompiler
// Type: Content.Shared.Alert.Components.GenericCounterAlertComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Alert.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class GenericCounterAlertComponent : 
  Component,
  ISerializationGenerated<GenericCounterAlertComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public int GlyphWidth = 6;
  [DataField(null, false, 1, false, false, null)]
  public bool CenterGlyph = true;
  [DataField(null, false, 1, false, false, null)]
  public bool HideLeadingZeroes = true;
  [DataField(null, false, 1, false, false, null)]
  public Vector2i AlertSize = new Vector2i(32 /*0x20*/, 32 /*0x20*/);
  [DataField(null, false, 1, false, false, null)]
  public List<string> DigitKeys = new List<string>()
  {
    "1",
    "10",
    "100",
    "1000",
    "10000"
  };

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GenericCounterAlertComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (GenericCounterAlertComponent) component;
    if (serialization.TryCustomCopy<GenericCounterAlertComponent>(this, ref target, hookCtx, false, context))
      return;
    int num = 0;
    if (!serialization.TryCustomCopy<int>(this.GlyphWidth, ref num, hookCtx, false, context))
      num = this.GlyphWidth;
    target.GlyphWidth = num;
    bool flag1 = false;
    if (!serialization.TryCustomCopy<bool>(this.CenterGlyph, ref flag1, hookCtx, false, context))
      flag1 = this.CenterGlyph;
    target.CenterGlyph = flag1;
    bool flag2 = false;
    if (!serialization.TryCustomCopy<bool>(this.HideLeadingZeroes, ref flag2, hookCtx, false, context))
      flag2 = this.HideLeadingZeroes;
    target.HideLeadingZeroes = flag2;
    Vector2i vector2i = new Vector2i();
    if (!serialization.TryCustomCopy<Vector2i>(this.AlertSize, ref vector2i, hookCtx, false, context))
      vector2i = serialization.CreateCopy<Vector2i>(this.AlertSize, hookCtx, context, false);
    target.AlertSize = vector2i;
    List<string> stringList = (List<string>) null;
    if (this.DigitKeys == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<string>>(this.DigitKeys, ref stringList, hookCtx, true, context))
      stringList = serialization.CreateCopy<List<string>>(this.DigitKeys, hookCtx, context, false);
    target.DigitKeys = stringList;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GenericCounterAlertComponent target,
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
    GenericCounterAlertComponent target1 = (GenericCounterAlertComponent) target;
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
    GenericCounterAlertComponent target1 = (GenericCounterAlertComponent) target;
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
    GenericCounterAlertComponent target1 = (GenericCounterAlertComponent) target;
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
  virtual GenericCounterAlertComponent Component.Instantiate()
  {
    return new GenericCounterAlertComponent();
  }
}
