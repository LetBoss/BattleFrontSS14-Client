// Decompiled with JetBrains decompiler
// Type: Content.Shared.Crayon.SharedCrayonComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Crayon;

[NetworkedComponent]
[ComponentProtoName("Crayon")]
[Access(new Type[] {typeof (SharedCrayonSystem)})]
public abstract class SharedCrayonComponent : 
  Component,
  ISerializationGenerated<SharedCrayonComponent>,
  ISerializationGenerated
{
  [DataField("color", false, 1, false, false, null)]
  public Color Color;

  public string SelectedState { get; set; } = string.Empty;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref SharedCrayonComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (SharedCrayonComponent) component;
    if (serialization.TryCustomCopy<SharedCrayonComponent>(this, ref target, hookCtx, false, context))
      return;
    Color color = new Color();
    if (!serialization.TryCustomCopy<Color>(this.Color, ref color, hookCtx, false, context))
      color = serialization.CreateCopy<Color>(this.Color, hookCtx, context, false);
    target.Color = color;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref SharedCrayonComponent target,
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
    SharedCrayonComponent target1 = (SharedCrayonComponent) target;
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
    SharedCrayonComponent target1 = (SharedCrayonComponent) target;
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
    SharedCrayonComponent target1 = (SharedCrayonComponent) target;
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
  virtual SharedCrayonComponent Component.Instantiate() => throw new NotImplementedException();

  [NetSerializable]
  [Serializable]
  public enum CrayonUiKey : byte
  {
    Key,
  }
}
