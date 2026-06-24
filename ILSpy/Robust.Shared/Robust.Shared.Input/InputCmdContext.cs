using System.Collections;
using System.Collections.Generic;

namespace Robust.Shared.Input;

internal sealed class InputCmdContext : IInputCmdContext, IEnumerable<BoundKeyFunction>, IEnumerable
{
	private readonly List<BoundKeyFunction> _commands = new List<BoundKeyFunction>();

	private readonly IInputCmdContext? _parent;

	public string Name { get; }

	internal InputCmdContext(IInputCmdContext? parent, string name)
	{
		_parent = parent;
		Name = name;
	}

	internal InputCmdContext(string name)
	{
		Name = name;
	}

	public void AddFunction(BoundKeyFunction function)
	{
		_commands.Add(function);
	}

	public bool FunctionExists(BoundKeyFunction function)
	{
		return _commands.Contains(function);
	}

	public bool FunctionExistsHierarchy(BoundKeyFunction function)
	{
		if (_commands.Contains(function))
		{
			return true;
		}
		if (_parent != null)
		{
			return _parent.FunctionExistsHierarchy(function);
		}
		return false;
	}

	public void RemoveFunction(BoundKeyFunction function)
	{
		_commands.Remove(function);
	}

	public IEnumerator<BoundKeyFunction> GetEnumerator()
	{
		return _commands.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
