namespace Robust.Shared.GameObjects;

[ByRefEvent]
public struct MetaFlagRemoveAttemptEvent(MetaDataFlags toRemove)
{
	public MetaDataFlags ToRemove = toRemove;
}
