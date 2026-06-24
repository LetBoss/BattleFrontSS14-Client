using System;
using System.Diagnostics;
using Robust.Shared.Analyzers;
using Robust.Shared.Map;

namespace Robust.Shared.Input;

[Virtual]
[DebuggerDisplay("{Function}, {State}, CF: {CanFocus}, H: {Handled}")]
public class BoundKeyEventArgs : EventArgs
{
	public readonly bool IsRepeat;

	public BoundKeyFunction Function { get; }

	public BoundKeyState State { get; }

	public ScreenCoordinates PointerLocation { get; }

	public bool CanFocus { get; internal set; }

	public bool Handled { get; private set; }

	public BoundKeyEventArgs(BoundKeyFunction function, BoundKeyState state, ScreenCoordinates pointerLocation, bool canFocus)
	{
		Function = function;
		State = state;
		PointerLocation = pointerLocation;
		CanFocus = canFocus;
	}

	public BoundKeyEventArgs(BoundKeyFunction function, BoundKeyState state, ScreenCoordinates pointerLocation, bool canFocus, bool isRepeat = false)
		: this(function, state, pointerLocation, canFocus)
	{
		IsRepeat = isRepeat;
	}

	public void Handle()
	{
		Handled = true;
	}
}
