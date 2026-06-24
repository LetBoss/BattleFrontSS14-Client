// Decompiled with JetBrains decompiler
// Type: Content.Client.PDA.PdaBorderColorComponent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.PDA;

[RegisterComponent]
public sealed class PdaBorderColorComponent : 
  Component,
  ISerializationGenerated<PdaBorderColorComponent>,
  ISerializationGenerated
{
  [DataField("borderColor", false, 1, true, false, null)]
  public string? BorderColor;
  [DataField("accentHColor", false, 1, false, false, null)]
  public string? AccentHColor;
  [DataField("accentVColor", false, 1, false, false, null)]
  public string? AccentVColor;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PdaBorderColorComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (PdaBorderColorComponent) component;
    if (serialization.TryCustomCopy<PdaBorderColorComponent>(this, ref target, hookCtx, false, context))
      return;
    string str1 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.BorderColor, ref str1, hookCtx, false, context))
      str1 = this.BorderColor;
    target.BorderColor = str1;
    string str2 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.AccentHColor, ref str2, hookCtx, false, context))
      str2 = this.AccentHColor;
    target.AccentHColor = str2;
    string str3 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.AccentVColor, ref str3, hookCtx, false, context))
      str3 = this.AccentVColor;
    target.AccentVColor = str3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PdaBorderColorComponent target,
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
    PdaBorderColorComponent target1 = (PdaBorderColorComponent) target;
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
    PdaBorderColorComponent target1 = (PdaBorderColorComponent) target;
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
    PdaBorderColorComponent target1 = (PdaBorderColorComponent) target;
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
  virtual PdaBorderColorComponent Component.Instantiate() => new PdaBorderColorComponent();
}
