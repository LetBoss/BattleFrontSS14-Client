// Decompiled with JetBrains decompiler
// Type: Content.Shared.Disposal.Components.SharedDisposalTaggerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

#nullable enable
namespace Content.Shared.Disposal.Components;

public sealed class SharedDisposalTaggerComponent : 
  Component,
  ISerializationGenerated<SharedDisposalTaggerComponent>,
  ISerializationGenerated
{
  public static readonly Regex TagRegex = new Regex("^[a-zA-Z0-9 ]*$", RegexOptions.Compiled);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SharedDisposalTaggerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (SharedDisposalTaggerComponent) component;
    serialization.TryCustomCopy<SharedDisposalTaggerComponent>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SharedDisposalTaggerComponent target,
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
    SharedDisposalTaggerComponent target1 = (SharedDisposalTaggerComponent) target;
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
    SharedDisposalTaggerComponent target1 = (SharedDisposalTaggerComponent) target;
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
    SharedDisposalTaggerComponent target1 = (SharedDisposalTaggerComponent) target;
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
  virtual SharedDisposalTaggerComponent Component.Instantiate()
  {
    return new SharedDisposalTaggerComponent();
  }

  [NetSerializable]
  [Serializable]
  public sealed class DisposalTaggerUserInterfaceState : BoundUserInterfaceState
  {
    public readonly string Tag;

    public DisposalTaggerUserInterfaceState(string tag) => this.Tag = tag;
  }

  [NetSerializable]
  [Serializable]
  public sealed class UiActionMessage : BoundUserInterfaceMessage
  {
    public readonly SharedDisposalTaggerComponent.UiAction Action;
    public readonly string Tag = "";

    public UiActionMessage(SharedDisposalTaggerComponent.UiAction action, string tag)
    {
      this.Action = action;
      if (this.Action != SharedDisposalTaggerComponent.UiAction.Ok)
        return;
      this.Tag = tag.Substring(0, Math.Min(tag.Length, 30));
    }
  }

  [NetSerializable]
  [Serializable]
  public enum UiAction
  {
    Ok,
  }

  [NetSerializable]
  [Serializable]
  public enum DisposalTaggerUiKey
  {
    Key,
  }
}
