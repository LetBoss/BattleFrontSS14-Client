using System;
using System.Linq;
using Content.Shared._RMC14.Targeting;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Client._RMC14.Weapons.Ranged.Targeting;

public sealed class RMCTargetingSystem : SharedRMCTargetingSystem
{
	private const string TargetedKey = "targetedDirection";

	private const string TargetedDirectionKey = "targetedDirectionIntense";

	[Dependency]
	private IOverlayManager _overlay;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		base.Initialize();
		_overlay.AddOverlay((Overlay)(object)new TargetingOverlay((IEntityManager)(object)((EntitySystem)this).EntityManager));
		((EntitySystem)this).SubscribeLocalEvent<RMCTargetedComponent, GotTargetedEvent>((EntityEventRefHandler<RMCTargetedComponent, GotTargetedEvent>)OnGotTargeted, (Type[])null, (Type[])null);
	}

	private void OnGotTargeted(Entity<RMCTargetedComponent> ent, ref GotTargetedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(Entity<RMCTargetedComponent>.op_Implicit(ent), ref item) && _sprite.LayerExists(Entity<SpriteComponent>.op_Implicit((ent.Owner, item)), "targetedDirection") && _sprite.LayerExists(Entity<SpriteComponent>.op_Implicit((ent.Owner, item)), "targetedDirectionIntense"))
		{
			(EntityCoordinates, Angle) moverCoordinateRotation = _transform.GetMoverCoordinateRotation(Entity<RMCTargetedComponent>.op_Implicit(ent), ((EntitySystem)this).Transform(Entity<RMCTargetedComponent>.op_Implicit(ent)));
			EntityCoordinates moverCoordinates = _transform.GetMoverCoordinates(ent.Comp.TargetedBy.Last());
			Angle val = DirectionExtensions.ToAngle(moverCoordinateRotation.Item1.Position - moverCoordinates.Position);
			Angle val2 = DirectionExtensions.ToAngle(DirectionExtensions.GetClockwise90Degrees(((Angle)(ref val)).GetCardinalDir())) - moverCoordinateRotation.Item2;
			_sprite.LayerSetRotation(Entity<SpriteComponent>.op_Implicit((ent.Owner, item)), "targetedDirection", val2);
			_sprite.LayerSetRotation(Entity<SpriteComponent>.op_Implicit((ent.Owner, item)), "targetedDirectionIntense", val2);
		}
	}
}
