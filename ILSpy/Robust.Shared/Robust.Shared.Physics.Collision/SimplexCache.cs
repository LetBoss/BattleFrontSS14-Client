namespace Robust.Shared.Physics.Collision;

internal struct SimplexCache
{
	public ushort Count;

	public unsafe fixed byte IndexA[3];

	public unsafe fixed byte IndexB[3];

	public float Metric;
}
