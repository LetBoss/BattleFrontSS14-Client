using System.Numerics;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Client.Shuttles.Systems;

public sealed class EmergencyShuttleOverlay : Overlay
{
	private readonly EntityQuery<TransformComponent> _transformQuery;

	private readonly SharedTransformSystem _transformSystem;

	public EntityUid? StationUid;

	public Box2? Position;

	public override OverlaySpace Space => (OverlaySpace)4;

	public EmergencyShuttleOverlay(EntityQuery<TransformComponent> transformQuery, SharedTransformSystem transformSystem)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		_transformQuery = transformQuery;
		_transformSystem = transformSystem;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent val = default(TransformComponent);
		if (Position.HasValue && _transformQuery.TryGetComponent(StationUid, ref val))
		{
			DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
			Matrix3x2 worldMatrix = _transformSystem.GetWorldMatrix(val);
			((DrawingHandleBase)worldHandle).SetTransform(ref worldMatrix);
			DrawingHandleWorld worldHandle2 = ((OverlayDrawArgs)(ref args)).WorldHandle;
			Box2 value = Position.Value;
			Color red = Color.Red;
			worldHandle2.DrawRect(value, ((Color)(ref red)).WithAlpha((byte)100), true);
			DrawingHandleWorld worldHandle3 = ((OverlayDrawArgs)(ref args)).WorldHandle;
			worldMatrix = Matrix3x2.Identity;
			((DrawingHandleBase)worldHandle3).SetTransform(ref worldMatrix);
		}
	}
}
