namespace Robust.Shared.GameObjects;

internal readonly record struct PvsIndex(int Index)
{
	public static readonly PvsIndex Invalid = new PvsIndex(-1);
}
