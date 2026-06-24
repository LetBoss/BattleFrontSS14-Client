// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Misc.PhysicsCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Physics.Components;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Misc;

[ToolshedCommand]
internal sealed class PhysicsCommand : ToolshedCommand
{
  private SharedTransformSystem? _xform;

  [CommandImplementation("velocity")]
  public IEnumerable<float> Velocity([PipedArgument] IEnumerable<EntityUid> input)
  {
    EntityQuery<PhysicsComponent> physQuery = this.GetEntityQuery<PhysicsComponent>();
    foreach (EntityUid uid in input)
    {
      PhysicsComponent component;
      if (physQuery.TryGetComponent(uid, out component))
        yield return component.LinearVelocity.Length();
    }
  }

  [CommandImplementation("parent")]
  public IEnumerable<EntityUid> Parent([PipedArgument] IEnumerable<EntityUid> input)
  {
    if (this._xform == null)
      this._xform = this.GetSys<SharedTransformSystem>();
    return input.Select<EntityUid, EntityUid>((Func<EntityUid, EntityUid>) (x => this.Comp<TransformComponent>(x).ParentUid));
  }

  [CommandImplementation("angular_velocity")]
  public IEnumerable<float> AngularVelocity([PipedArgument] IEnumerable<EntityUid> input)
  {
    EntityQuery<PhysicsComponent> physQuery = this.GetEntityQuery<PhysicsComponent>();
    foreach (EntityUid uid in input)
    {
      PhysicsComponent component;
      if (physQuery.TryGetComponent(uid, out component))
        yield return component.AngularVelocity;
    }
  }
}
