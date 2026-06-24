namespace Robust.Shared.Physics.Collision;

internal ref struct DistanceInput
{
	public DistanceProxy ProxyA;

	public DistanceProxy ProxyB;

	public Transform TransformA;

	public Transform TransformB;

	public bool UseRadii;
}
