// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.OccluderComponent
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.ComponentTrees;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.GameObjects;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (OccluderSystem)})]
public sealed class OccluderComponent : 
  Component,
  IComponentTreeEntry<OccluderComponent>,
  ISerializationGenerated<OccluderComponent>,
  ISerializationGenerated
{
  [DataField("enabled", false, 1, false, false, null)]
  public bool Enabled = true;
  [DataField("boundingBox", false, 1, false, false, null)]
  public Box2 BoundingBox = new Box2(-0.5f, -0.5f, 0.5f, 0.5f);
  [Robust.Shared.ViewVariables.ViewVariables]
  public (EntityUid Grid, Vector2i Tile)? LastPosition;
  [Robust.Shared.ViewVariables.ViewVariables]
  public OccluderComponent.OccluderDir Occluding;

  public EntityUid? TreeUid { get; set; }

  public DynamicTree<ComponentTreeEntry<OccluderComponent>>? Tree { get; set; }

  public bool AddToTree => this.Enabled;

  public bool TreeUpdateQueued { get; set; }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref OccluderComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (OccluderComponent) target1;
    if (serialization.TryCustomCopy<OccluderComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Enabled, ref target2, hookCtx, false, context))
      target2 = this.Enabled;
    target.Enabled = target2;
    Box2 target3 = new Box2();
    if (!serialization.TryCustomCopy<Box2>(this.BoundingBox, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<Box2>(this.BoundingBox, hookCtx, context);
    target.BoundingBox = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref OccluderComponent target,
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
    OccluderComponent target1 = (OccluderComponent) target;
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
    OccluderComponent target1 = (OccluderComponent) target;
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
    OccluderComponent target1 = (OccluderComponent) target;
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
  virtual OccluderComponent Component.Instantiate() => new OccluderComponent();

  [Flags]
  public enum OccluderDir : byte
  {
    None = 0,
    North = 1,
    East = 2,
    South = 4,
    West = 8,
  }

  [NetSerializable]
  [Serializable]
  public sealed class OccluderComponentState : ComponentState
  {
    public bool Enabled { get; }

    public Box2 BoundingBox { get; }

    public OccluderComponentState(bool enabled, Box2 boundingBox)
    {
      this.Enabled = enabled;
      this.BoundingBox = boundingBox;
    }
  }
}
