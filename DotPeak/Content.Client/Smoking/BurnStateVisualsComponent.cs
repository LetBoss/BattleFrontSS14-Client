// Decompiled with JetBrains decompiler
// Type: Content.Client.Smoking.BurnStateVisualsComponent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.Smoking;

[RegisterComponent]
public sealed class BurnStateVisualsComponent : 
  Component,
  ISerializationGenerated<BurnStateVisualsComponent>,
  ISerializationGenerated
{
  [DataField("burntIcon", false, 1, false, false, null)]
  public string BurntIcon = "burnt-icon";
  [DataField("litIcon", false, 1, false, false, null)]
  public string LitIcon = "lit-icon";
  [DataField("unlitIcon", false, 1, false, false, null)]
  public string UnlitIcon = "icon";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref BurnStateVisualsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (BurnStateVisualsComponent) component;
    if (serialization.TryCustomCopy<BurnStateVisualsComponent>(this, ref target, hookCtx, false, context))
      return;
    string str1 = (string) null;
    if (this.BurntIcon == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.BurntIcon, ref str1, hookCtx, false, context))
      str1 = this.BurntIcon;
    target.BurntIcon = str1;
    string str2 = (string) null;
    if (this.LitIcon == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.LitIcon, ref str2, hookCtx, false, context))
      str2 = this.LitIcon;
    target.LitIcon = str2;
    string str3 = (string) null;
    if (this.UnlitIcon == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.UnlitIcon, ref str3, hookCtx, false, context))
      str3 = this.UnlitIcon;
    target.UnlitIcon = str3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref BurnStateVisualsComponent target,
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
    BurnStateVisualsComponent target1 = (BurnStateVisualsComponent) target;
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
    BurnStateVisualsComponent target1 = (BurnStateVisualsComponent) target;
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
    BurnStateVisualsComponent target1 = (BurnStateVisualsComponent) target;
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
  virtual BurnStateVisualsComponent Component.Instantiate() => new BurnStateVisualsComponent();
}
