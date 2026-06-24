// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.Reaction.ReactiveComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Chemistry.Reaction;

[RegisterComponent]
public sealed class ReactiveComponent : 
  Component,
  ISerializationGenerated<ReactiveComponent>,
  ISerializationGenerated
{
  [DataField("groups", true, 1, false, true, typeof (PrototypeIdDictionarySerializer<HashSet<ReactionMethod>, ReactiveGroupPrototype>))]
  public System.Collections.Generic.Dictionary<string, HashSet<ReactionMethod>>? ReactiveGroups;
  [DataField("reactions", true, 1, false, true, null)]
  public List<ReactiveReagentEffectEntry>? Reactions;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ReactiveComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (ReactiveComponent) component;
    if (serialization.TryCustomCopy<ReactiveComponent>(this, ref target, hookCtx, false, context))
      return;
    System.Collections.Generic.Dictionary<string, HashSet<ReactionMethod>> dictionary = (System.Collections.Generic.Dictionary<string, HashSet<ReactionMethod>>) null;
    if (!serialization.TryCustomCopy<System.Collections.Generic.Dictionary<string, HashSet<ReactionMethod>>>(this.ReactiveGroups, ref dictionary, hookCtx, true, context))
      dictionary = serialization.CreateCopy<System.Collections.Generic.Dictionary<string, HashSet<ReactionMethod>>>(this.ReactiveGroups, hookCtx, context, false);
    target.ReactiveGroups = dictionary;
    List<ReactiveReagentEffectEntry> reagentEffectEntryList = (List<ReactiveReagentEffectEntry>) null;
    if (!serialization.TryCustomCopy<List<ReactiveReagentEffectEntry>>(this.Reactions, ref reagentEffectEntryList, hookCtx, true, context))
      reagentEffectEntryList = serialization.CreateCopy<List<ReactiveReagentEffectEntry>>(this.Reactions, hookCtx, context, false);
    target.Reactions = reagentEffectEntryList;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ReactiveComponent target,
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
    ReactiveComponent target1 = (ReactiveComponent) target;
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
    ReactiveComponent target1 = (ReactiveComponent) target;
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
    ReactiveComponent target1 = (ReactiveComponent) target;
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
  virtual ReactiveComponent Component.Instantiate() => new ReactiveComponent();
}
