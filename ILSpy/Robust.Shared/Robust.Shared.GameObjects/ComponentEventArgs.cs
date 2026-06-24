namespace Robust.Shared.GameObjects;

public readonly struct ComponentEventArgs
{
	public IComponent Component { get; }

	public EntityUid Owner { get; }

	public ComponentEventArgs(IComponent component, EntityUid owner)
	{
		Component = component;
		Owner = owner;
	}
}
