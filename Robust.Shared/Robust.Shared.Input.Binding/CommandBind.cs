using System;
using System.Collections.Generic;
using System.Linq;

namespace Robust.Shared.Input.Binding;

public sealed class CommandBind
{
	private readonly BoundKeyFunction _boundKeyFunction;

	private readonly IEnumerable<Type> _after;

	private readonly IEnumerable<Type> _before;

	private readonly InputCmdHandler _handler;

	public BoundKeyFunction BoundKeyFunction => _boundKeyFunction;

	public IEnumerable<Type> After => _after;

	public IEnumerable<Type> Before => _before;

	public InputCmdHandler Handler => _handler;

	public CommandBind(BoundKeyFunction boundKeyFunction, InputCmdHandler handler, IEnumerable<Type>? before = null, IEnumerable<Type>? after = null)
	{
		_boundKeyFunction = boundKeyFunction;
		_after = after ?? Enumerable.Empty<Type>();
		_before = before ?? Enumerable.Empty<Type>();
		_handler = handler;
	}
}
