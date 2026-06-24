using System.Collections.Generic;

namespace Robust.Shared.Input;

public sealed class PlayerCommandStates : IPlayerCommandStates
{
	private readonly Dictionary<BoundKeyFunction, BoundKeyState> _functionStates = new Dictionary<BoundKeyFunction, BoundKeyState>();

	public BoundKeyState this[BoundKeyFunction function]
	{
		get
		{
			return GetState(function);
		}
		set
		{
			SetState(function, value);
		}
	}

	public BoundKeyState GetState(BoundKeyFunction function)
	{
		if (!_functionStates.TryGetValue(function, out var value))
		{
			return BoundKeyState.Up;
		}
		return value;
	}

	public void SetState(BoundKeyFunction function, BoundKeyState state)
	{
		_functionStates[function] = state;
	}
}
