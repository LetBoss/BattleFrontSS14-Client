// Decompiled with JetBrains decompiler
// Type: Content.Shared.Machines.Components.MachinePart
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;

#nullable enable
namespace Content.Shared.Machines.Components;

[DataDefinition]
[NetSerializable]
[Serializable]
public sealed class MachinePart : ISerializationGenerated<MachinePart>, ISerializationGenerated
{
  [DataField(null, false, 1, true, false, typeof (ComponentNameSerializer))]
  public string Component = "";
  [DataField(null, false, 1, true, false, null)]
  public Vector2i Offset;
  [DataField(null, false, 1, false, false, null)]
  public bool Optional;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId? GhostProto;
  [DataField(null, false, 1, false, false, null)]
  public Angle Rotation = Angle.Zero;
  public Robust.Shared.GameObjects.NetEntity? NetEntity;
  [DataField(null, false, 1, false, false, null)]
  [NonSerialized]
  public EntityUid? Entity;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId Graph;
  [DataField(null, false, 1, false, false, null)]
  public string ExpectedNode;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MachinePart target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<MachinePart>(this, ref target, hookCtx, false, context))
      return;
    string target1 = (string) null;
    if (this.Component == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Component, ref target1, hookCtx, false, context))
      target1 = this.Component;
    target.Component = target1;
    Vector2i target2 = new Vector2i();
    if (!serialization.TryCustomCopy<Vector2i>(this.Offset, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<Vector2i>(this.Offset, hookCtx, context);
    target.Offset = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.Optional, ref target3, hookCtx, false, context))
      target3 = this.Optional;
    target.Optional = target3;
    EntProtoId? target4 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.GhostProto, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntProtoId?>(this.GhostProto, hookCtx, context);
    target.GhostProto = target4;
    Angle target5 = new Angle();
    if (!serialization.TryCustomCopy<Angle>(this.Rotation, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<Angle>(this.Rotation, hookCtx, context);
    target.Rotation = target5;
    EntityUid? target6 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Entity, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<EntityUid?>(this.Entity, hookCtx, context);
    target.Entity = target6;
    EntProtoId target7 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Graph, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<EntProtoId>(this.Graph, hookCtx, context);
    target.Graph = target7;
    string target8 = (string) null;
    if (this.ExpectedNode == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ExpectedNode, ref target8, hookCtx, false, context))
      target8 = this.ExpectedNode;
    target.ExpectedNode = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MachinePart target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    MachinePart target1 = (MachinePart) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public MachinePart Instantiate() => new MachinePart();
}
