// Decompiled with JetBrains decompiler
// Type: Content.Client.Holiday.HolidayRsiSwapComponent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.Holiday;

[RegisterComponent]
public sealed class HolidayRsiSwapComponent : 
  Component,
  ISerializationGenerated<HolidayRsiSwapComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<string, string> Sprite = new Dictionary<string, string>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref HolidayRsiSwapComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (HolidayRsiSwapComponent) component;
    if (serialization.TryCustomCopy<HolidayRsiSwapComponent>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<string, string> dictionary = (Dictionary<string, string>) null;
    if (this.Sprite == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, string>>(this.Sprite, ref dictionary, hookCtx, true, context))
      dictionary = serialization.CreateCopy<Dictionary<string, string>>(this.Sprite, hookCtx, context, false);
    target.Sprite = dictionary;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref HolidayRsiSwapComponent target,
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
    HolidayRsiSwapComponent target1 = (HolidayRsiSwapComponent) target;
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
    HolidayRsiSwapComponent target1 = (HolidayRsiSwapComponent) target;
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
    HolidayRsiSwapComponent target1 = (HolidayRsiSwapComponent) target;
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
  virtual HolidayRsiSwapComponent Component.Instantiate() => new HolidayRsiSwapComponent();
}
