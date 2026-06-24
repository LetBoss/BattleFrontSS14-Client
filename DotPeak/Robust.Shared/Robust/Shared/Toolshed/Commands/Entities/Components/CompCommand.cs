// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Entities.Components.CompCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Toolshed.TypeParsers;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Entities.Components;

[ToolshedCommand]
internal sealed class CompCommand : ToolshedCommand
{
  private static Type[] _parsers = new Type[1]
  {
    typeof (ComponentTypeParser)
  };

  public override Type[] TypeParameterParsers => CompCommand._parsers;

  [CommandImplementation("get")]
  public IEnumerable<T> CompEnumerable<T>([PipedArgument] IEnumerable<EntityUid> input) where T : IComponent
  {
    return input.Where<EntityUid>(new Func<EntityUid, bool>(((ToolshedCommand) this).HasComp<T>)).Select<EntityUid, T>(new Func<EntityUid, T>(((ToolshedCommand) this).Comp<T>));
  }

  [CommandImplementation("get")]
  public T? CompDirect<T>([PipedArgument] EntityUid input) where T : IComponent
  {
    T component;
    this.TryComp<T>(input, out component);
    return component;
  }

  [CommandImplementation("add")]
  public EntityUid Add<T>([PipedArgument] EntityUid input) where T : IComponent, new()
  {
    this.AddComp<T>(input);
    return input;
  }

  [CommandImplementation("add")]
  public IEnumerable<EntityUid> Add<T>([PipedArgument] IEnumerable<EntityUid> input) where T : IComponent, new()
  {
    return input.Select<EntityUid, EntityUid>(new Func<EntityUid, EntityUid>(this.Add<T>));
  }

  [CommandImplementation("rm")]
  public EntityUid Rm<T>([PipedArgument] EntityUid input) where T : IComponent, new()
  {
    this.RemComp<T>(input);
    return input;
  }

  [CommandImplementation("rm")]
  public IEnumerable<EntityUid> Rm<T>([PipedArgument] IEnumerable<EntityUid> input) where T : IComponent, new()
  {
    return input.Select<EntityUid, EntityUid>(new Func<EntityUid, EntityUid>(this.Rm<T>));
  }

  [CommandImplementation("ensure")]
  public EntityUid Ensure<T>([PipedArgument] EntityUid input) where T : IComponent, new()
  {
    this.EnsureComp<T>(input);
    return input;
  }

  [CommandImplementation("ensure")]
  public IEnumerable<EntityUid> Ensure<T>([PipedArgument] IEnumerable<EntityUid> input) where T : IComponent, new()
  {
    return input.Select<EntityUid, EntityUid>(new Func<EntityUid, EntityUid>(this.Ensure<T>));
  }

  [CommandImplementation("has")]
  public bool Has<T>([PipedArgument] EntityUid input) where T : IComponent
  {
    return this.HasComp<T>(input);
  }

  [CommandImplementation("has")]
  public IEnumerable<bool> Has<T>([PipedArgument] IEnumerable<EntityUid> input) where T : IComponent
  {
    return input.Select<EntityUid, bool>(new Func<EntityUid, bool>(this.Has<T>));
  }
}
