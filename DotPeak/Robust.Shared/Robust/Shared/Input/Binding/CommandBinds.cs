// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Input.Binding.CommandBinds
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Input.Binding;

public sealed class CommandBinds
{
  private readonly List<CommandBind> _bindings;

  public IEnumerable<CommandBind> Bindings => (IEnumerable<CommandBind>) this._bindings;

  private CommandBinds(List<CommandBind> bindings) => this._bindings = bindings;

  public static CommandBinds.BindingsBuilder Builder => new CommandBinds.BindingsBuilder();

  public static void Unregister<TOwner>()
  {
    SharedInputSystem entitySystem;
    if (!IoCManager.Resolve<IEntitySystemManager>().TryGetEntitySystem<SharedInputSystem>(out entitySystem))
      return;
    CommandBinds.Unregister<TOwner>(entitySystem.BindRegistry);
  }

  public static void Unregister<TOwner>(ICommandBindRegistry bindRegistry)
  {
    bindRegistry.Unregister<TOwner>();
  }

  public sealed class BindingsBuilder
  {
    private readonly List<CommandBind> _bindings = new List<CommandBind>();

    public static CommandBinds.BindingsBuilder Create() => new CommandBinds.BindingsBuilder();

    public CommandBinds.BindingsBuilder Bind(BoundKeyFunction function, InputCmdHandler command)
    {
      return this.Bind(new CommandBind(function, command));
    }

    public CommandBinds.BindingsBuilder Bind(
      BoundKeyFunction function,
      IEnumerable<InputCmdHandler> commands)
    {
      foreach (InputCmdHandler command in commands)
        this.Bind(new CommandBind(function, command));
      return this;
    }

    public CommandBinds.BindingsBuilder BindAfter(
      BoundKeyFunction function,
      InputCmdHandler command,
      params Type[] after)
    {
      return this.Bind(new CommandBind(function, command, after: (IEnumerable<Type>) after));
    }

    public CommandBinds.BindingsBuilder BindBefore(
      BoundKeyFunction function,
      InputCmdHandler command,
      params Type[] before)
    {
      return this.Bind(new CommandBind(function, command, (IEnumerable<Type>) before));
    }

    public CommandBinds.BindingsBuilder Bind(CommandBind commandBind)
    {
      this._bindings.Add(commandBind);
      return this;
    }

    public CommandBinds Build() => new CommandBinds(this._bindings);

    public CommandBinds Register<TOwner>(ICommandBindRegistry registry)
    {
      CommandBinds commandBinds = this.Build();
      registry.Register<TOwner>(commandBinds);
      return commandBinds;
    }

    public CommandBinds Register<TOwner>()
    {
      return this.Register<TOwner>(IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<SharedInputSystem>().BindRegistry);
    }
  }
}
