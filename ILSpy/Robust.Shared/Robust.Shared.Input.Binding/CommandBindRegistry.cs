using System;
using System.Collections.Generic;
using System.Linq;
using Robust.Shared.Log;
using Robust.Shared.Utility;

namespace Robust.Shared.Input.Binding;

public sealed class CommandBindRegistry : ICommandBindRegistry
{
	private sealed class TypedCommandBind
	{
		public readonly Type ForType;

		public readonly CommandBind CommandBind;

		public TypedCommandBind(Type forType, CommandBind commandBind)
		{
			ForType = forType;
			CommandBind = commandBind;
		}
	}

	private readonly ISawmill _sawmill;

	private List<TypedCommandBind> _bindings = new List<TypedCommandBind>();

	private Dictionary<BoundKeyFunction, List<InputCmdHandler>> _bindingsForKey = new Dictionary<BoundKeyFunction, List<InputCmdHandler>>();

	private bool _graphDirty;

	public CommandBindRegistry(ISawmill sawmill)
	{
		_sawmill = sawmill;
	}

	public void Register<TOwner>(CommandBinds commandBinds)
	{
		Register(commandBinds, typeof(TOwner));
	}

	public void Register(CommandBinds commandBinds, Type owner)
	{
		if (_bindings.Any((TypedCommandBind existing) => existing.ForType == owner))
		{
			_sawmill.Warning("Command binds already registered for type {0}, but you are trying to register more. This may be a programming error. Did you register these under the wrong type, or did you forget to unregister these bindings when your system / manager is shutdown?", owner.Name);
		}
		foreach (CommandBind binding in commandBinds.Bindings)
		{
			_bindings.Add(new TypedCommandBind(owner, binding));
		}
		_graphDirty = true;
	}

	public IEnumerable<InputCmdHandler> GetHandlers(BoundKeyFunction function)
	{
		if (_graphDirty)
		{
			RebuildGraph();
		}
		if (_bindingsForKey.TryGetValue(function, out List<InputCmdHandler> value))
		{
			return value;
		}
		return Enumerable.Empty<InputCmdHandler>();
	}

	public void Unregister(Type owner)
	{
		_bindings.RemoveAll((TypedCommandBind binding) => binding.ForType == owner);
		_graphDirty = true;
	}

	public void Unregister<TOwner>()
	{
		Unregister(typeof(TOwner));
	}

	internal void RebuildGraph()
	{
		_bindingsForKey.Clear();
		foreach (KeyValuePair<BoundKeyFunction, List<TypedCommandBind>> item in FunctionToBindings())
		{
			_bindingsForKey[item.Key] = ResolveDependencies(item.Key, item.Value);
		}
		_graphDirty = false;
	}

	private Dictionary<BoundKeyFunction, List<TypedCommandBind>> FunctionToBindings()
	{
		Dictionary<BoundKeyFunction, List<TypedCommandBind>> dictionary = new Dictionary<BoundKeyFunction, List<TypedCommandBind>>();
		foreach (TypedCommandBind binding in _bindings)
		{
			if (!dictionary.ContainsKey(binding.CommandBind.BoundKeyFunction))
			{
				dictionary[binding.CommandBind.BoundKeyFunction] = new List<TypedCommandBind>();
			}
			dictionary[binding.CommandBind.BoundKeyFunction].Add(binding);
		}
		return dictionary;
	}

	private List<InputCmdHandler> ResolveDependencies(BoundKeyFunction function, List<TypedCommandBind> bindingsForFunction)
	{
		List<TopologicalSort.GraphNode<TypedCommandBind>> list = new List<TopologicalSort.GraphNode<TypedCommandBind>>();
		Dictionary<Type, List<TopologicalSort.GraphNode<TypedCommandBind>>> dictionary = new Dictionary<Type, List<TopologicalSort.GraphNode<TypedCommandBind>>>();
		foreach (TypedCommandBind item2 in bindingsForFunction)
		{
			if (!dictionary.ContainsKey(item2.ForType))
			{
				dictionary[item2.ForType] = new List<TopologicalSort.GraphNode<TypedCommandBind>>();
			}
			TopologicalSort.GraphNode<TypedCommandBind> item = new TopologicalSort.GraphNode<TypedCommandBind>(item2);
			dictionary[item2.ForType].Add(item);
			list.Add(item);
		}
		foreach (TopologicalSort.GraphNode<TypedCommandBind> item3 in list)
		{
			foreach (Type item4 in item3.Value.CommandBind.After)
			{
				if (!dictionary.TryGetValue(item4, out var value))
				{
					continue;
				}
				foreach (TopologicalSort.GraphNode<TypedCommandBind> item5 in value)
				{
					item5.Dependant.Add(item3);
				}
			}
			foreach (Type item6 in item3.Value.CommandBind.Before)
			{
				if (!dictionary.TryGetValue(item6, out var value2))
				{
					continue;
				}
				foreach (TopologicalSort.GraphNode<TypedCommandBind> item7 in value2)
				{
					item3.Dependant.Add(item7);
				}
			}
		}
		return (from c in TopologicalSort.Sort(list)
			select c.CommandBind.Handler).ToList();
	}
}
