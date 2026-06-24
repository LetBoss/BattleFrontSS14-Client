namespace Robust.Shared.GameObjects;

public readonly struct AddedComponentEventArgs
{
	public readonly ComponentEventArgs BaseArgs;

	public readonly ComponentRegistration ComponentType;

	internal AddedComponentEventArgs(ComponentEventArgs baseArgs, ComponentRegistration componentType)
	{
		BaseArgs = baseArgs;
		ComponentType = componentType;
	}
}
