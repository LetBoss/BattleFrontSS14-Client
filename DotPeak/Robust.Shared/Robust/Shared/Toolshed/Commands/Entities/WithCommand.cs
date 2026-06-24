// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Entities.WithCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Toolshed.TypeParsers;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Entities;

[ToolshedCommand]
internal sealed class WithCommand : ToolshedCommand
{
  [Dependency]
  private readonly IComponentFactory _componentFactory;

  [CommandImplementation(null)]
  public IEnumerable<EntityUid> With([PipedArgument] IEnumerable<EntityUid> input, [CommandArgument(typeof (ComponentTypeParser), false)] Type component, [CommandInverted] bool inverted)
  {
    if (inverted)
      return input.Where<EntityUid>((Func<EntityUid, bool>) (x => !this.EntityManager.HasComponent(x, component)));
    return input is EntitiesCommand.AllEntityEnumerator ? (IEnumerable<EntityUid>) this.EntityManager.AllEntityUids(component) : input.Where<EntityUid>((Func<EntityUid, bool>) (x => this.EntityManager.HasComponent(x, component)));
  }

  [CommandImplementation(null)]
  public IEnumerable<EntityPrototype> With(
    [PipedArgument] IEnumerable<EntityPrototype> input,
    [CommandArgument(typeof (ComponentTypeParser), false)] Type component,
    [CommandInverted] bool inverted)
  {
    string name = this._componentFactory.GetComponentName(component);
    return input.Where<EntityPrototype>((Func<EntityPrototype, bool>) (x => x.Components.ContainsKey(name) ^ inverted));
  }

  [CommandImplementation(null)]
  [TakesPipedTypeAsGeneric]
  public IEnumerable<ProtoId<T>> With<T>(
    [PipedArgument] IEnumerable<ProtoId<T>> input,
    ProtoId<T> protoId,
    [CommandInverted] bool inverted)
    where T : class, IPrototype
  {
    return input.Where<ProtoId<T>>((Func<ProtoId<T>, bool>) (x => x == protoId ^ inverted));
  }
}
