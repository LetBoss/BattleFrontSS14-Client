using System;
using Content.Client._RMC14.Weapons.Ranged.Prediction;
using Content.Shared._RMC14.Xenonids.Projectile;
using Robust.Client.GameObjects;
using Robust.Client.Physics;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;

namespace Content.Client._RMC14.Xenonids.Projectile;

public sealed class ClientXenoProjectileSystem : EntitySystem
{
	[Dependency]
	private GunPredictionSystem _gunPrediction;

	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoProjectileShotComponent, ComponentStartup>((EntityEventRefHandler<XenoProjectileShotComponent, ComponentStartup>)OnShotStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoClientProjectileShotComponent, UpdateIsPredictedEvent>((EntityEventRefHandler<XenoClientProjectileShotComponent, UpdateIsPredictedEvent>)OnUpdateIsPredicted, (Type[])null, (Type[])null);
	}

	private void OnShotStartup(Entity<XenoProjectileShotComponent> ent, ref ComponentStartup args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if (_gunPrediction.GunPrediction)
		{
			EntityUid? shooterEnt = ent.Comp.ShooterEnt;
			EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
			if (shooterEnt.HasValue == localEntity.HasValue && (!shooterEnt.HasValue || !(shooterEnt.GetValueOrDefault() != localEntity.GetValueOrDefault())))
			{
				_sprite.SetVisible(Entity<SpriteComponent>.op_Implicit(ent.Owner), false);
			}
		}
	}

	private void OnUpdateIsPredicted(Entity<XenoClientProjectileShotComponent> ent, ref UpdateIsPredictedEvent args)
	{
		args.IsPredicted = true;
	}
}
