using System;
using System.Numerics;
using Content.Shared._RMC14.Projectiles.Aimed;
using Content.Shared._RMC14.Stamina;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Weapons.Ranged.AimedShot;
using Content.Shared._RMC14.Weapons.Ranged.AimedShot.FocusedShooting;
using Content.Shared._RMC14.Xenonids.Fortify;
using Content.Shared.Camera;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Content.Shared.Stunnable;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Shared._RMC14.Projectiles.StoppingPower;

public sealed class RMCStoppingPowerSystem : EntitySystem
{
	[Dependency]
	private RMCSizeStunSystem _sizeStun;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedStunSystem _stun;

	[Dependency]
	private RMCStaminaSystem _stamina;

	[Dependency]
	private SharedCameraRecoilSystem _cameraRecoil;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private MobStateSystem _mobState;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RMCStoppingPowerComponent, MapInitEvent>((EntityEventRefHandler<RMCStoppingPowerComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCStoppingPowerComponent, ProjectileHitEvent>((EntityEventRefHandler<RMCStoppingPowerComponent, ProjectileHitEvent>)OnStoppingPowerHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCStoppingPowerComponent, ShotByAimedShotEvent>((EntityEventRefHandler<RMCStoppingPowerComponent, ShotByAimedShotEvent>)OnShotByAimedShot, (Type[])null, (Type[])null);
	}

	private void OnMapInit(Entity<RMCStoppingPowerComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.ShotFrom = _transform.GetMapCoordinates(Entity<RMCStoppingPowerComponent>.op_Implicit(ent), (TransformComponent)null);
		((EntitySystem)this).Dirty<RMCStoppingPowerComponent>(ent, (MetaDataComponent)null);
	}

	private void OnShotByAimedShot(Entity<RMCStoppingPowerComponent> ent, ref ShotByAimedShotEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		RMCFocusedShootingComponent focused = default(RMCFocusedShootingComponent);
		if (((EntitySystem)this).TryComp<RMCFocusedShootingComponent>(args.Gun, ref focused))
		{
			ent.Comp.FocusedCounter = focused.FocusCounter;
		}
	}

	private void OnStoppingPowerHit(Entity<RMCStoppingPowerComponent> ent, ref ProjectileHitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.CurrentStoppingPower = 0f;
		((EntitySystem)this).Dirty<RMCStoppingPowerComponent>(ent, (MetaDataComponent)null);
		AimedProjectileComponent aimedShot = default(AimedProjectileComponent);
		if ((ent.Comp.RequiresAimedShot && !((EntitySystem)this).TryComp<AimedProjectileComponent>(Entity<RMCStoppingPowerComponent>.op_Implicit(ent), ref aimedShot)) || (ent.Comp.FocusedCounterThreshold.HasValue && ent.Comp.FocusedCounter < ent.Comp.FocusedCounterThreshold))
		{
			return;
		}
		float stoppingPower = (float)Math.Min(Math.Ceiling((double)args.Damage.GetTotal().Float() / ent.Comp.StoppingPowerDivider), ent.Comp.MaxStoppingPower);
		if (!(stoppingPower > (float)ent.Comp.StoppingThreshold))
		{
			return;
		}
		EntityUid target = args.Target;
		_sizeStun.TryGetSize(target, out var size);
		ent.Comp.CurrentStoppingPower = stoppingPower;
		((EntitySystem)this).Dirty<RMCStoppingPowerComponent>(ent, (MetaDataComponent)null);
		if ((int)size >= 5)
		{
			if (stoppingPower >= (float)ent.Comp.BigXenoScreenShakeThreshold)
			{
				_cameraRecoil.KickCamera(target, new Vector2(stoppingPower - 3f, stoppingPower - 2f));
			}
			if (stoppingPower >= (float)ent.Comp.BigXenoInterruptThreshold)
			{
				AimedProjectileComponent aimedProjectile = default(AimedProjectileComponent);
				if ((int)size < 6 || (((EntitySystem)this).TryComp<AimedProjectileComponent>(Entity<RMCStoppingPowerComponent>.op_Implicit(ent), ref aimedProjectile) && !(aimedProjectile.Target != target)))
				{
					if (((EntitySystem)this).HasComp<XenoFortifyComponent>(target))
					{
						_transform.SetWorldRotation(target, _transform.GetWorldRotation(target) + Angle.FromDegrees(45.0));
					}
					_stun.TryParalyze(target, ent.Comp.BigXenoStunTime, refresh: true);
					SendMessage(target, base.Loc.GetString("rmc-xeno-stun-interrupt-shaken"), PopupType.SmallCaution);
				}
			}
			else
			{
				SendMessage(target, base.Loc.GetString("rmc-xeno-shaken"));
			}
		}
		else
		{
			_cameraRecoil.KickCamera(target, new Vector2(stoppingPower - 2f, stoppingPower - 1f));
			if (!((EntitySystem)this).HasComp<KnockedDownComponent>(target) && !_mobState.IsDead(target) && ent.Comp.ShotFrom.HasValue)
			{
				_sizeStun.KnockBack(target, ent.Comp.ShotFrom.Value);
				SendMessage(target, base.Loc.GetString("rmc-xeno-knocked-back"), PopupType.SmallCaution);
			}
			else
			{
				SendMessage(target, base.Loc.GetString("rmc-xeno-shaken"));
			}
			TimeSpan stunTime = TimeSpan.FromSeconds((stoppingPower - (float)ent.Comp.StoppingThreshold) * ent.Comp.XenoStunMultiplier);
			if ((int)size >= 2)
			{
				_stun.TryParalyze(target, stunTime, refresh: true);
			}
			else if (((EntitySystem)this).HasComp<RMCStaminaComponent>(target))
			{
				_stamina.DoStaminaDamage(Entity<RMCStaminaComponent>.op_Implicit(target), args.Damage.GetTotal().Float());
			}
			else
			{
				_stun.TryParalyze(target, stunTime, refresh: true);
			}
		}
	}

	private void SendMessage(EntityUid target, string message, PopupType popupType = PopupType.Small)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		_popup.PopupEntity(message, target, target, popupType);
	}
}
