using Content.Shared._RMC14.Medical.IV;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Client._RMC14.Medical.IV;

public sealed class IVDripOverlay : Overlay
{
	[Dependency]
	private IEntityManager _entity;

	public override OverlaySpace Space => (OverlaySpace)64;

	public IVDripOverlay()
	{
		IoCManager.InjectDependencies<IVDripOverlay>(this);
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		TransformSystem val = _entity.System<TransformSystem>();
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		EntityQueryEnumerator<IVDripComponent> val2 = _entity.EntityQueryEnumerator<IVDripComponent>();
		EntityUid val3 = default(EntityUid);
		IVDripComponent iVDripComponent = default(IVDripComponent);
		while (val2.MoveNext(ref val3, ref iVDripComponent))
		{
			EntityUid? attachedTo = iVDripComponent.AttachedTo;
			if (!attachedTo.HasValue)
			{
				continue;
			}
			EntityUid valueOrDefault = attachedTo.GetValueOrDefault();
			if (((EntityUid)(ref valueOrDefault)).Valid)
			{
				MapCoordinates mapCoordinates = ((SharedTransformSystem)val).GetMapCoordinates(val3, (TransformComponent)null);
				MapCoordinates mapCoordinates2 = ((SharedTransformSystem)val).GetMapCoordinates(valueOrDefault, (TransformComponent)null);
				if (!(mapCoordinates.MapId == MapId.Nullspace) && !(mapCoordinates2.MapId == MapId.Nullspace))
				{
					((DrawingHandleBase)worldHandle).DrawLine(mapCoordinates.Position, mapCoordinates2.Position, Color.White);
				}
			}
		}
		EntityQueryEnumerator<BloodPackComponent> val4 = _entity.EntityQueryEnumerator<BloodPackComponent>();
		EntityUid val5 = default(EntityUid);
		BloodPackComponent bloodPackComponent = default(BloodPackComponent);
		while (val4.MoveNext(ref val5, ref bloodPackComponent))
		{
			EntityUid? attachedTo = bloodPackComponent.AttachedTo;
			if (!attachedTo.HasValue)
			{
				continue;
			}
			EntityUid valueOrDefault2 = attachedTo.GetValueOrDefault();
			if (((EntityUid)(ref valueOrDefault2)).Valid)
			{
				MapCoordinates mapCoordinates3 = ((SharedTransformSystem)val).GetMapCoordinates(val5, (TransformComponent)null);
				MapCoordinates mapCoordinates4 = ((SharedTransformSystem)val).GetMapCoordinates(valueOrDefault2, (TransformComponent)null);
				if (!(mapCoordinates3.MapId == MapId.Nullspace) && !(mapCoordinates4.MapId == MapId.Nullspace))
				{
					((DrawingHandleBase)worldHandle).DrawLine(mapCoordinates3.Position, mapCoordinates4.Position, Color.White);
				}
			}
		}
	}
}
