// Decompiled with JetBrains decompiler
// Type: Content.Client.Guidebook.Components.GuideHelpComponent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Guidebook;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.Guidebook.Components;

[RegisterComponent]
[Access(new Type[] {typeof (GuidebookSystem)})]
public sealed class GuideHelpComponent : 
  Component,
  ISerializationGenerated<GuideHelpComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public List<ProtoId<GuideEntryPrototype>> Guides = new List<ProtoId<GuideEntryPrototype>>();
  [DataField("includeChildren", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public bool IncludeChildren = true;
  [DataField("openOnActivation", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public bool OpenOnActivation;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GuideHelpComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (GuideHelpComponent) component;
    if (serialization.TryCustomCopy<GuideHelpComponent>(this, ref target, hookCtx, false, context))
      return;
    List<ProtoId<GuideEntryPrototype>> protoIdList = (List<ProtoId<GuideEntryPrototype>>) null;
    if (this.Guides == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<ProtoId<GuideEntryPrototype>>>(this.Guides, ref protoIdList, hookCtx, true, context))
      protoIdList = serialization.CreateCopy<List<ProtoId<GuideEntryPrototype>>>(this.Guides, hookCtx, context, false);
    target.Guides = protoIdList;
    bool flag1 = false;
    if (!serialization.TryCustomCopy<bool>(this.IncludeChildren, ref flag1, hookCtx, false, context))
      flag1 = this.IncludeChildren;
    target.IncludeChildren = flag1;
    bool flag2 = false;
    if (!serialization.TryCustomCopy<bool>(this.OpenOnActivation, ref flag2, hookCtx, false, context))
      flag2 = this.OpenOnActivation;
    target.OpenOnActivation = flag2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GuideHelpComponent target,
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
    GuideHelpComponent target1 = (GuideHelpComponent) target;
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
    GuideHelpComponent target1 = (GuideHelpComponent) target;
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
    GuideHelpComponent target1 = (GuideHelpComponent) target;
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
  virtual GuideHelpComponent Component.Instantiate() => new GuideHelpComponent();
}
