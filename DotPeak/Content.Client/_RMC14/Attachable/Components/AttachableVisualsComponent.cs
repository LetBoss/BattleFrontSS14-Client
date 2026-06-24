// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Attachable.Components.AttachableVisualsComponent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._RMC14.Attachable.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client._RMC14.Attachable.Components;

[RegisterComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (AttachableHolderVisualsSystem)})]
public sealed class AttachableVisualsComponent : 
  Component,
  ISerializationGenerated<AttachableVisualsComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ResPath? Rsi;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string? Prefix;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string? Suffix = "_a";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool IncludeSlotName;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ShowActive;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool RedrawOnAppearanceChange;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Layer;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2 Offset;
  [DataField(null, false, 1, false, false, null)]
  public string? LastSlotId;
  [DataField(null, false, 1, false, false, null)]
  public string? LastSuffix;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AttachableVisualsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (AttachableVisualsComponent) component;
    if (serialization.TryCustomCopy<AttachableVisualsComponent>(this, ref target, hookCtx, false, context))
      return;
    ResPath? nullable = new ResPath?();
    if (!serialization.TryCustomCopy<ResPath?>(this.Rsi, ref nullable, hookCtx, false, context))
      nullable = serialization.CreateCopy<ResPath?>(this.Rsi, hookCtx, context, false);
    target.Rsi = nullable;
    string str1 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.Prefix, ref str1, hookCtx, false, context))
      str1 = this.Prefix;
    target.Prefix = str1;
    string str2 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.Suffix, ref str2, hookCtx, false, context))
      str2 = this.Suffix;
    target.Suffix = str2;
    bool flag1 = false;
    if (!serialization.TryCustomCopy<bool>(this.IncludeSlotName, ref flag1, hookCtx, false, context))
      flag1 = this.IncludeSlotName;
    target.IncludeSlotName = flag1;
    bool flag2 = false;
    if (!serialization.TryCustomCopy<bool>(this.ShowActive, ref flag2, hookCtx, false, context))
      flag2 = this.ShowActive;
    target.ShowActive = flag2;
    bool flag3 = false;
    if (!serialization.TryCustomCopy<bool>(this.RedrawOnAppearanceChange, ref flag3, hookCtx, false, context))
      flag3 = this.RedrawOnAppearanceChange;
    target.RedrawOnAppearanceChange = flag3;
    int num = 0;
    if (!serialization.TryCustomCopy<int>(this.Layer, ref num, hookCtx, false, context))
      num = this.Layer;
    target.Layer = num;
    Vector2 vector2 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.Offset, ref vector2, hookCtx, false, context))
      vector2 = serialization.CreateCopy<Vector2>(this.Offset, hookCtx, context, false);
    target.Offset = vector2;
    string str3 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.LastSlotId, ref str3, hookCtx, false, context))
      str3 = this.LastSlotId;
    target.LastSlotId = str3;
    string str4 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.LastSuffix, ref str4, hookCtx, false, context))
      str4 = this.LastSuffix;
    target.LastSuffix = str4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AttachableVisualsComponent target,
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
    AttachableVisualsComponent target1 = (AttachableVisualsComponent) target;
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
    AttachableVisualsComponent target1 = (AttachableVisualsComponent) target;
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
    AttachableVisualsComponent target1 = (AttachableVisualsComponent) target;
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
  virtual AttachableVisualsComponent Component.Instantiate() => new AttachableVisualsComponent();
}
