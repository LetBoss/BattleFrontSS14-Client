namespace Robust.Shared.GameObjects;

public readonly struct RemovedComponentEventArgs
{
	public readonly ComponentEventArgs BaseArgs;

	public readonly bool Terminating;

	public readonly MetaDataComponent Meta;

	public readonly CompIdx Idx;

	internal RemovedComponentEventArgs(ComponentEventArgs baseArgs, bool terminating, MetaDataComponent meta, CompIdx idx)
	{
		BaseArgs = baseArgs;
		Terminating = terminating;
		Meta = meta;
		Idx = idx;
	}
}
