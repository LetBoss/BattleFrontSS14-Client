namespace Content.Shared._CIV14merka.Loadout;

public sealed class CivLoadoutItemInfo
{
	public readonly string Slot;

	public readonly string Proto;

	public readonly int Count;

	public string Key => CivLoadoutKeys.Item(Slot, Proto);

	public CivLoadoutItemInfo(string slot, string proto, int count)
	{
		Slot = slot;
		Proto = proto;
		Count = count;
	}
}
