using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Robust.Shared.Console;

public sealed class CommandBuffer
{
	private const string DelayMarker = "-DELAY-";

	private int _tickrate;

	private int _delay;

	private readonly LinkedList<string> _commandBuffer = new LinkedList<string>();

	public void Append(string command)
	{
		_commandBuffer.AddLast(command);
	}

	public void Insert(string command)
	{
		_commandBuffer.AddFirst(command);
	}

	public void Tick(ushort tickRate)
	{
		_tickrate = tickRate;
		if (_delay > 0)
		{
			_delay--;
		}
	}

	public bool TryGetCommand([MaybeNullWhen(false)] out string command)
	{
		LinkedListNode<string> first = _commandBuffer.First;
		if (first == null)
		{
			command = null;
			return false;
		}
		if (first.Value.Equals("-DELAY-"))
		{
			if (_delay == 0)
			{
				_commandBuffer.RemoveFirst();
				return TryGetCommand(out command);
			}
			command = null;
			return false;
		}
		if (first.Value.StartsWith("wait "))
		{
			string text = first.Value.Substring(5);
			_commandBuffer.RemoveFirst();
			if (string.IsNullOrWhiteSpace(text) || !int.TryParse(text, out var result))
			{
				return TryGetCommand(out command);
			}
			_commandBuffer.AddFirst("-DELAY-");
			_delay = result;
			command = null;
			return false;
		}
		_commandBuffer.RemoveFirst();
		command = first.Value;
		return true;
	}
}
