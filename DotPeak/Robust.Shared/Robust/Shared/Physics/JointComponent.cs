// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.JointComponent
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Physics.Dynamics.Joints;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.Physics;

[RegisterComponent]
[NetworkedComponent]
public sealed class JointComponent : 
  Component,
  ISerializationGenerated<JointComponent>,
  ISerializationGenerated
{
  [DataField("relay", false, 1, false, false, null)]
  public EntityUid? Relay;
  [DataField("joints", false, 1, false, false, null)]
  internal Dictionary<string, Joint> Joints = new Dictionary<string, Joint>();

  [Robust.Shared.ViewVariables.ViewVariables]
  public int JointCount => this.Joints.Count;

  public IReadOnlyDictionary<string, Joint> GetJoints
  {
    get => (IReadOnlyDictionary<string, Joint>) this.Joints;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref JointComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (JointComponent) target1;
    if (serialization.TryCustomCopy<JointComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid? target2 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Relay, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityUid?>(this.Relay, hookCtx, context);
    target.Relay = target2;
    Dictionary<string, Joint> target3 = (Dictionary<string, Joint>) null;
    if (this.Joints == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, Joint>>(this.Joints, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<Dictionary<string, Joint>>(this.Joints, hookCtx, context);
    target.Joints = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref JointComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    JointComponent target1 = (JointComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    JointComponent target1 = (JointComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    JointComponent target1 = (JointComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual JointComponent Component.Instantiate() => new JointComponent();
}
