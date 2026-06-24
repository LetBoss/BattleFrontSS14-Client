// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.IComponent
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Robust.Shared.GameObjects;

[ImplicitDataDefinitionForInheritors]
[NotContentImplementable]
public interface IComponent : ISerializationGenerated<IComponent>, ISerializationGenerated
{
  ComponentLifeStage LifeStage { get; internal set; }

  internal bool Networked { get; set; }

  bool NetSyncEnabled { get; set; }

  bool SendOnlyToOwner { get; }

  bool SessionSpecific { get; }

  [Obsolete("Update your API to allow accessing Owner through other means")]
  EntityUid Owner { get; set; }

  bool Initialized { get; }

  bool Running { get; }

  bool Deleted { get; }

  [Obsolete]
  void Dirty(IEntityManager? entManager = null);

  GameTick CreationTick { get; internal set; }

  GameTick LastModifiedTick { get; internal set; }

  internal void ClearTicks();

  internal void ClearCreationTick();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  new void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    serialization.TryCustomCopy<IComponent>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  new void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  new void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    IComponent target1 = (IComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  new IComponent Instantiate() => throw new NotImplementedException();
}
