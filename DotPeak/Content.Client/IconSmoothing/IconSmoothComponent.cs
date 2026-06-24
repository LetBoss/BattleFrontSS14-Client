// Decompiled with JetBrains decompiler
// Type: Content.Client.IconSmoothing.IconSmoothComponent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.IconSmoothing;

[RegisterComponent]
public sealed class IconSmoothComponent : 
  Component,
  ISerializationGenerated<IconSmoothComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField("enabled", false, 1, false, false, null)]
  public bool Enabled = true;
  public (EntityUid?, Vector2i)? LastPosition;
  [DataField(null, false, 1, false, false, null)]
  public List<string> AdditionalKeys = new List<string>();
  [DataField("shader", false, 1, false, false, typeof (PrototypeIdSerializer<ShaderPrototype>))]
  public string? Shader;
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField("mode", false, 1, false, false, null)]
  public IconSmoothingMode Mode;

  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField("key", false, 1, false, false, null)]
  public string? SmoothKey { get; private set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField("base", false, 1, false, false, null)]
  public string StateBase { get; set; } = string.Empty;

  internal int UpdateGeneration { get; set; }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref IconSmoothComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (IconSmoothComponent) component;
    if (serialization.TryCustomCopy<IconSmoothComponent>(this, ref target, hookCtx, false, context))
      return;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.Enabled, ref flag, hookCtx, false, context))
      flag = this.Enabled;
    target.Enabled = flag;
    string str1 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.SmoothKey, ref str1, hookCtx, false, context))
      str1 = this.SmoothKey;
    target.SmoothKey = str1;
    List<string> stringList = (List<string>) null;
    if (this.AdditionalKeys == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<string>>(this.AdditionalKeys, ref stringList, hookCtx, true, context))
      stringList = serialization.CreateCopy<List<string>>(this.AdditionalKeys, hookCtx, context, false);
    target.AdditionalKeys = stringList;
    string str2 = (string) null;
    if (this.StateBase == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.StateBase, ref str2, hookCtx, false, context))
      str2 = this.StateBase;
    target.StateBase = str2;
    string str3 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.Shader, ref str3, hookCtx, false, context))
      str3 = this.Shader;
    target.Shader = str3;
    IconSmoothingMode iconSmoothingMode = IconSmoothingMode.Corners;
    if (!serialization.TryCustomCopy<IconSmoothingMode>(this.Mode, ref iconSmoothingMode, hookCtx, false, context))
      iconSmoothingMode = this.Mode;
    target.Mode = iconSmoothingMode;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref IconSmoothComponent target,
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
    IconSmoothComponent target1 = (IconSmoothComponent) target;
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
    IconSmoothComponent target1 = (IconSmoothComponent) target;
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
    IconSmoothComponent target1 = (IconSmoothComponent) target;
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
  virtual IconSmoothComponent Component.Instantiate() => new IconSmoothComponent();
}
