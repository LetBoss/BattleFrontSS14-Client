using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;

namespace Robust.Shared.Input;

[Serializable]
[NetSerializable]
public abstract class InputCmdMessage : EntityEventArgs, IComparable<InputCmdMessage>
{
	public GameTick Tick { get; }

	public ushort SubTick { get; }

	public KeyFunctionId InputFunctionId { get; }

	public uint InputSequence { get; set; }

	public InputCmdMessage(GameTick tick, ushort subTick, KeyFunctionId inputFunctionId)
	{
		Tick = tick;
		SubTick = subTick;
		InputFunctionId = inputFunctionId;
	}

	public int CompareTo(InputCmdMessage? other)
	{
		if (other == null)
		{
			return 1;
		}
		if (this == other)
		{
			return 0;
		}
		if (other == null)
		{
			return 1;
		}
		return InputSequence.CompareTo(other.InputSequence);
	}

	public override string ToString()
	{
		return $"tick={Tick}, subTick={SubTick}, seq={InputSequence} func={InputFunctionId}";
	}
}
