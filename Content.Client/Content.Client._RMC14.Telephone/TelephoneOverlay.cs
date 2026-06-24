using Content.Shared._RMC14.Telephone;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Containers;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Client._RMC14.Telephone;

public sealed class TelephoneOverlay : Overlay
{
	[Dependency]
	private IEntityManager _entity;

	public override OverlaySpace Space => (OverlaySpace)64;

	public TelephoneOverlay()
	{
		IoCManager.InjectDependencies<TelephoneOverlay>(this);
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		ContainerSystem val = _entity.System<ContainerSystem>();
		TransformSystem val2 = _entity.System<TransformSystem>();
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		EntityQueryEnumerator<RotaryPhoneComponent> val3 = _entity.EntityQueryEnumerator<RotaryPhoneComponent>();
		EntityUid val4 = default(EntityUid);
		RotaryPhoneComponent rotaryPhoneComponent = default(RotaryPhoneComponent);
		BaseContainer val5 = default(BaseContainer);
		TransformComponent val6 = default(TransformComponent);
		TransformComponent val7 = default(TransformComponent);
		while (val3.MoveNext(ref val4, ref rotaryPhoneComponent))
		{
			EntityUid? phone = rotaryPhoneComponent.Phone;
			if (phone.HasValue)
			{
				EntityUid valueOrDefault = phone.GetValueOrDefault();
				if (((EntityUid)(ref valueOrDefault)).Valid && ((SharedContainerSystem)val).TryGetContainer(val4, rotaryPhoneComponent.ContainerId, ref val5, (ContainerManagerComponent)null) && val5.ContainedEntities.Count <= 0 && _entity.TryGetComponent<TransformComponent>(val4, ref val6) && !(val6.MapID == MapId.Nullspace) && _entity.TryGetComponent<TransformComponent>(valueOrDefault, ref val7) && !(val7.MapID == MapId.Nullspace))
				{
					MapCoordinates mapCoordinates = ((SharedTransformSystem)val2).GetMapCoordinates(val4, (TransformComponent)null);
					MapCoordinates mapCoordinates2 = ((SharedTransformSystem)val2).GetMapCoordinates(valueOrDefault, (TransformComponent)null);
					((DrawingHandleBase)worldHandle).DrawLine(mapCoordinates.Position, mapCoordinates2.Position, Color.Black);
				}
			}
		}
	}
}
