using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Robust.Shared.Input.Binding;

public sealed class CommandBinds
{
	public sealed class BindingsBuilder
	{
		private readonly List<CommandBind> _bindings = new List<CommandBind>();

		public static BindingsBuilder Create()
		{
			return new BindingsBuilder();
		}

		public BindingsBuilder Bind(BoundKeyFunction function, InputCmdHandler command)
		{
			return Bind(new CommandBind(function, command));
		}

		public BindingsBuilder Bind(BoundKeyFunction function, IEnumerable<InputCmdHandler> commands)
		{
			foreach (InputCmdHandler command in commands)
			{
				Bind(new CommandBind(function, command));
			}
			return this;
		}

		public BindingsBuilder BindAfter(BoundKeyFunction function, InputCmdHandler command, params Type[] after)
		{
			return Bind(new CommandBind(function, command, null, after));
		}

		public BindingsBuilder BindBefore(BoundKeyFunction function, InputCmdHandler command, params Type[] before)
		{
			return Bind(new CommandBind(function, command, before));
		}

		public BindingsBuilder Bind(CommandBind commandBind)
		{
			_bindings.Add(commandBind);
			return this;
		}

		public CommandBinds Build()
		{
			return new CommandBinds(_bindings);
		}

		public CommandBinds Register<TOwner>(ICommandBindRegistry registry)
		{
			CommandBinds commandBinds = Build();
			registry.Register<TOwner>(commandBinds);
			return commandBinds;
		}

		public CommandBinds Register<TOwner>()
		{
			return Register<TOwner>(IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<SharedInputSystem>().BindRegistry);
		}
	}

	private readonly List<CommandBind> _bindings;

	public IEnumerable<CommandBind> Bindings => _bindings;

	public static BindingsBuilder Builder => new BindingsBuilder();

	private CommandBinds(List<CommandBind> bindings)
	{
		_bindings = bindings;
	}

	public static void Unregister<TOwner>()
	{
		if (IoCManager.Resolve<IEntitySystemManager>().TryGetEntitySystem<SharedInputSystem>(out SharedInputSystem entitySystem))
		{
			Unregister<TOwner>(entitySystem.BindRegistry);
		}
	}

	public static void Unregister<TOwner>(ICommandBindRegistry bindRegistry)
	{
		bindRegistry.Unregister<TOwner>();
	}
}
