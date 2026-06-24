using System;
using Content.Shared._CIV14merka.Atgm;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Client._CIV14merka.Atgm;

public sealed class CivAtgmWireOverlay : Overlay
{
	[Dependency]
	private readonly IEntityManager _entity;

	public override OverlaySpace Space => (OverlaySpace)64;

	public CivAtgmWireOverlay()
	{
		IoCManager.InjectDependencies<CivAtgmWireOverlay>(this);
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		TransformSystem val = _entity.System<TransformSystem>();
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		EntityQueryEnumerator<CivAtgmWireComponent, TransformComponent> val2 = _entity.EntityQueryEnumerator<CivAtgmWireComponent, TransformComponent>();
		EntityUid val3 = default(EntityUid);
		CivAtgmWireComponent civAtgmWireComponent = default(CivAtgmWireComponent);
		TransformComponent val4 = default(TransformComponent);
		while (val2.MoveNext(ref val3, ref civAtgmWireComponent, ref val4))
		{
			NetCoordinates? origin = civAtgmWireComponent.Origin;
			if (!origin.HasValue)
			{
				continue;
			}
			NetCoordinates valueOrDefault = origin.GetValueOrDefault();
			if (!(val4.MapID == MapId.Nullspace) && !(val4.MapID != args.MapId))
			{
				MapCoordinates val5 = ((SharedTransformSystem)val).ToMapCoordinates(_entity.GetCoordinates(valueOrDefault), true);
				if (!(val5.MapId != args.MapId))
				{
					MapCoordinates mapCoordinates = ((SharedTransformSystem)val).GetMapCoordinates(val3, (TransformComponent)null);
					((DrawingHandleBase)worldHandle).DrawLine(val5.Position, mapCoordinates.Position, Color.FromHex((ReadOnlySpan<char>)"#3A3A3A", (Color?)null));
				}
			}
		}
	}
}
