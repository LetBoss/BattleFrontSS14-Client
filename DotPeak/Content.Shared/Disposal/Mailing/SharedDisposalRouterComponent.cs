// Decompiled with JetBrains decompiler
// Type: Content.Shared.Disposal.Components.SharedDisposalRouterComponent
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

public sealed class SharedDisposalRouterComponent : 
  Component,
  ISerializationGenerated<SharedDisposalRouterComponent>,
  ISerializationGenerated
{
  public static readonly Regex TagRegex = new Regex("^[a-zA-Z0-9, ]*$", RegexOptions.Compiled);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SharedDisposalRouterComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (SharedDisposalRouterComponent) component;
    serialization.TryCustomCopy<SharedDisposalRouterComponent>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SharedDisposalRouterComponent target,
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
    SharedDisposalRouterComponent target1 = (SharedDisposalRouterComponent) target;
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
    SharedDisposalRouterComponent target1 = (SharedDisposalRouterComponent) target;
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
    SharedDisposalRouterComponent target1 = (SharedDisposalRouterComponent) target;
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
  virtual SharedDisposalRouterComponent Component.Instantiate()
  {
    return new SharedDisposalRouterComponent();
  }

  [NetSerializable]
  [Serializable]
  public sealed class DisposalRouterUserInterfaceState : BoundUserInterfaceState
  {
    public readonly string Tags;

    public DisposalRouterUserInterfaceState(string tags) => this.Tags = tags;
  }

  [NetSerializable]
  [Serializable]
  public sealed class UiActionMessage : BoundUserInterfaceMessage
  {
    public readonly SharedDisposalRouterComponent.UiAction Action;
    public readonly string Tags = "";

    public UiActionMessage(SharedDisposalRouterComponent.UiAction action, string tags)
    {
      this.Action = action;
      if (this.Action != SharedDisposalRouterComponent.UiAction.Ok)
        return;
      this.Tags = tags.Substring(0, Math.Min(tags.Length, 150));
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
  public enum DisposalRouterUiKey
  {
    Key,
  }
}
