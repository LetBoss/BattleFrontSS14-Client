using Content.Shared._PUBG.Repair;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Client._PUBG.Repair;

public sealed class CivRepairStationOverlay : Overlay
{
	[Dependency]
	private readonly IEntityManager _entity;

	public override OverlaySpace Space => (OverlaySpace)64;

	public CivRepairStationOverlay()
	{
		IoCManager.InjectDependencies<CivRepairStationOverlay>(this);
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		TransformSystem val = _entity.System<TransformSystem>();
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		EntityQueryEnumerator<CivRepairStationComponent> val2 = _entity.EntityQueryEnumerator<CivRepairStationComponent>();
		EntityUid val3 = default(EntityUid);
		CivRepairStationComponent civRepairStationComponent = default(CivRepairStationComponent);
		TransformComponent val4 = default(TransformComponent);
		TransformComponent val5 = default(TransformComponent);
		while (val2.MoveNext(ref val3, ref civRepairStationComponent))
		{
			EntityUid? welder = civRepairStationComponent.Welder;
			if (!welder.HasValue)
			{
				continue;
			}
			EntityUid valueOrDefault = welder.GetValueOrDefault();
			if (((EntityUid)(ref valueOrDefault)).Valid && _entity.TryGetComponent<TransformComponent>(val3, ref val4) && !(val4.MapID == MapId.Nullspace) && _entity.TryGetComponent<TransformComponent>(valueOrDefault, ref val5) && !(val5.MapID == MapId.Nullspace))
			{
				MapCoordinates mapCoordinates = ((SharedTransformSystem)val).GetMapCoordinates(val3, (TransformComponent)null);
				MapCoordinates mapCoordinates2 = ((SharedTransformSystem)val).GetMapCoordinates(valueOrDefault, (TransformComponent)null);
				if (!(mapCoordinates.MapId != mapCoordinates2.MapId))
				{
					((DrawingHandleBase)worldHandle).DrawLine(mapCoordinates.Position, mapCoordinates2.Position, Color.Black);
				}
			}
		}
	}
}
