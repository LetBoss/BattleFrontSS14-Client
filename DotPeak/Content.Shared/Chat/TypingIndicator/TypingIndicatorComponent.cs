// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chat.TypingIndicator.TypingIndicatorComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Chat.TypingIndicator;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedTypingIndicatorSystem)})]
public sealed class TypingIndicatorComponent : 
  Component,
  ISerializationGenerated<TypingIndicatorComponent>,
  ISerializationGenerated
{
  [DataField("proto", false, 1, false, false, null)]
  public ProtoId<Content.Shared.Chat.TypingIndicator.TypingIndicatorPrototype> TypingIndicatorPrototype = ProtoId<Content.Shared.Chat.TypingIndicator.TypingIndicatorPrototype>.op_Implicit("default");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref TypingIndicatorComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (TypingIndicatorComponent) component;
    if (serialization.TryCustomCopy<TypingIndicatorComponent>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<Content.Shared.Chat.TypingIndicator.TypingIndicatorPrototype> protoId = new ProtoId<Content.Shared.Chat.TypingIndicator.TypingIndicatorPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<Content.Shared.Chat.TypingIndicator.TypingIndicatorPrototype>>(this.TypingIndicatorPrototype, ref protoId, hookCtx, false, context))
      protoId = serialization.CreateCopy<ProtoId<Content.Shared.Chat.TypingIndicator.TypingIndicatorPrototype>>(this.TypingIndicatorPrototype, hookCtx, context, false);
    target.TypingIndicatorPrototype = protoId;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref TypingIndicatorComponent target,
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
    TypingIndicatorComponent target1 = (TypingIndicatorComponent) target;
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
    TypingIndicatorComponent target1 = (TypingIndicatorComponent) target;
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
    TypingIndicatorComponent target1 = (TypingIndicatorComponent) target;
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
  virtual TypingIndicatorComponent Component.Instantiate() => new TypingIndicatorComponent();
}
