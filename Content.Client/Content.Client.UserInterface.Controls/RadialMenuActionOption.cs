using System;

namespace Content.Client.UserInterface.Controls;

public abstract class RadialMenuActionOption(Action onPressed) : RadialMenuOption
{
	public Action OnPressed { get; } = onPressed;
}
public sealed class RadialMenuActionOption<T> : RadialMenuActionOption
{
	public RadialMenuActionOption(Action<T> onPressed, T data)
		: base(delegate
		{
			onPressed(data);
		})
	{
	}
}
