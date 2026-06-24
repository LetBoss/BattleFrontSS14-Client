namespace Robust.Shared.GameObjects;

internal record struct BroadphaseData(EntityUid Uid, bool CanCollide, bool Static)
{
	public bool Valid => IsValid();

	public static readonly BroadphaseData Invalid;

	public bool IsValid()
	{
		return Uid.IsValid();
	}
}
