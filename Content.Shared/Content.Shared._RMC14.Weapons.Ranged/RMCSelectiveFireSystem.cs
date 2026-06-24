using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Attachable.Systems;
using Content.Shared._RMC14.Input;
using Content.Shared.Examine;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Content.Shared.Wieldable;
using Content.Shared.Wieldable.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;

namespace Content.Shared._RMC14.Weapons.Ranged;

public sealed class RMCSelectiveFireSystem : EntitySystem
{
	[Dependency]
	private SharedGunSystem _gunSystem;

	private const string scatterExamineColour = "yellow";

	private const SelectiveFire allFireModes = SelectiveFire.SemiAuto | SelectiveFire.Burst | SelectiveFire.FullAuto;

	public override void Initialize()
	{
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Expected O, but got Unknown
		((EntitySystem)this).SubscribeAllEvent<RequestStopShootEvent>((EntitySessionEventHandler<RequestStopShootEvent>)OnStopShootRequest, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCSelectiveFireComponent, ExaminedEvent>((EntityEventRefHandler<RMCSelectiveFireComponent, ExaminedEvent>)OnExamine, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCSelectiveFireComponent, ItemWieldedEvent>((EntityEventRefHandler<RMCSelectiveFireComponent, ItemWieldedEvent>)SelectiveFireRefreshWield, (Type[])null, new Type[1] { typeof(AttachableHolderSystem) });
		((EntitySystem)this).SubscribeLocalEvent<RMCSelectiveFireComponent, ItemUnwieldedEvent>((EntityEventRefHandler<RMCSelectiveFireComponent, ItemUnwieldedEvent>)SelectiveFireRefreshWield, (Type[])null, new Type[1] { typeof(AttachableHolderSystem) });
		((EntitySystem)this).SubscribeLocalEvent<RMCSelectiveFireComponent, MapInitEvent>((EntityEventRefHandler<RMCSelectiveFireComponent, MapInitEvent>)OnSelectiveFireMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCSelectiveFireComponent, RMCFireModeChangedEvent>((EntityEventRefHandler<RMCSelectiveFireComponent, RMCFireModeChangedEvent>)OnSelectiveFireModeChanged, (Type[])null, (Type[])null);
		CommandBinds.Builder.Bind(CMKeyFunctions.RMCCycleFireMode, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate(ICommonSession? session)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			EntityUid? val = ((session != null) ? session.AttachedEntity : ((EntityUid?)null));
			if (val.HasValue)
			{
				EntityUid valueOrDefault = val.GetValueOrDefault();
				if (_gunSystem.TryGetGun(valueOrDefault, out EntityUid gunEntity, out GunComponent gunComp))
				{
					_gunSystem.CycleFire(gunEntity, gunComp, valueOrDefault);
				}
			}
		}, (StateInputCmdDelegate)null, false, true)).Register<RMCSelectiveFireSystem>();
	}

	private void OnStopShootRequest(RequestStopShootEvent ev, EntitySessionEventArgs args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		EntityUid gunUid = ((EntitySystem)this).GetEntity(ev.Gun);
		GunComponent gunComponent = default(GunComponent);
		if (((EntitySessionEventArgs)(ref args)).SenderSession.AttachedEntity.HasValue && ((EntitySystem)this).TryComp<GunComponent>(gunUid, ref gunComponent) && _gunSystem.TryGetGun(((EntitySessionEventArgs)(ref args)).SenderSession.AttachedEntity.Value, out EntityUid _, out GunComponent userGun) && userGun == gunComponent)
		{
			gunComponent.CurrentAngle = gunComponent.MinAngleModified;
			((EntitySystem)this).Dirty(gunUid, (IComponent)(object)gunComponent, (MetaDataComponent)null);
		}
	}

	private void OnExamine(Entity<RMCSelectiveFireComponent> gun, ref ExaminedEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		GunComponent gunComponent = default(GunComponent);
		if (!args.IsInDetailsRange || !((EntitySystem)this).TryComp<GunComponent>(gun.Owner, ref gunComponent))
		{
			return;
		}
		using (args.PushGroup("RMCSelectiveFireComponent"))
		{
			args.PushMarkup(base.Loc.GetString("rmc-examine-text-scatter-max", (ValueTuple<string, object>)("colour", "yellow"), (ValueTuple<string, object>)("scatter", ((Angle)(ref gunComponent.MaxAngleModified)).Degrees)));
			args.PushMarkup(base.Loc.GetString("rmc-examine-text-scatter-min", (ValueTuple<string, object>)("colour", "yellow"), (ValueTuple<string, object>)("scatter", ((Angle)(ref gunComponent.MinAngleModified)).Degrees)));
			if (ContainsMods(gun, gunComponent.SelectedMode))
			{
				SelectiveFireModifierSet mods = gun.Comp.Modifiers[gunComponent.SelectedMode];
				if (mods.ShotsToMaxScatter.HasValue)
				{
					args.PushMarkup(base.Loc.GetString("rmc-examine-text-shots-to-max-scatter", (ValueTuple<string, object>)("colour", "yellow"), (ValueTuple<string, object>)("shots", mods.ShotsToMaxScatter)));
				}
			}
		}
	}

	private void OnSelectiveFireMapInit(Entity<RMCSelectiveFireComponent> gun, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		gun.Comp.BurstScatterMultModified = gun.Comp.BurstScatterMult;
		RefreshFireModes(Entity<RMCSelectiveFireComponent>.op_Implicit((gun.Owner, gun.Comp)), forceValueRefresh: true);
	}

	private void OnSelectiveFireModeChanged(Entity<RMCSelectiveFireComponent> gun, ref RMCFireModeChangedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		RefreshFireModeGunValues(gun);
	}

	private void SelectiveFireRefreshWield<T>(Entity<RMCSelectiveFireComponent> gun, ref T args) where T : notnull
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		RefreshWieldableFireModeValues(gun);
	}

	public void RefreshFireModeGunValues(Entity<RMCSelectiveFireComponent> gun)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		GunComponent gunComponent = default(GunComponent);
		if (!((EntitySystem)this).TryComp<GunComponent>(gun.Owner, ref gunComponent))
		{
			return;
		}
		gunComponent.AngleIncrease = gun.Comp.ScatterIncrease;
		gunComponent.AngleDecay = gun.Comp.ScatterDecay;
		GunGetFireRateEvent ev = new GunGetFireRateEvent((gunComponent.SelectedMode == SelectiveFire.Burst) ? (gun.Comp.BaseFireRate * gun.Comp.BurstFireRateMultiplier) : gun.Comp.BaseFireRate);
		((EntitySystem)this).RaiseLocalEvent<GunGetFireRateEvent>(Entity<RMCSelectiveFireComponent>.op_Implicit(gun), ref ev, false);
		gunComponent.FireRate = ev.FireRate;
		if (ContainsMods(gun, gunComponent.SelectedMode))
		{
			SelectiveFireModifierSet mods = gun.Comp.Modifiers[gunComponent.SelectedMode];
			ev = new GunGetFireRateEvent(1f / (1f / gunComponent.FireRate + mods.FireDelay));
			((EntitySystem)this).RaiseLocalEvent<GunGetFireRateEvent>(Entity<RMCSelectiveFireComponent>.op_Implicit(gun), ref ev, false);
			if (gunComponent.SelectedMode == SelectiveFire.Burst)
			{
				gunComponent.BurstFireRate = ev.FireRate;
			}
			else
			{
				gunComponent.FireRate = ev.FireRate;
			}
		}
		RefreshWieldableFireModeValues(gun);
	}

	public bool ContainsMods(Entity<RMCSelectiveFireComponent> gun, SelectiveFire mode)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return gun.Comp.Modifiers.ContainsKey(mode);
	}

	public void RefreshWieldableFireModeValues(Entity<RMCSelectiveFireComponent> gun)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		GunComponent gunComponent = default(GunComponent);
		if (((EntitySystem)this).TryComp<GunComponent>(gun.Owner, ref gunComponent))
		{
			WieldableComponent wieldableComponent = default(WieldableComponent);
			bool wielded = ((EntitySystem)this).TryComp<WieldableComponent>(gun.Owner, ref wieldableComponent) && wieldableComponent.Wielded;
			gunComponent.CameraRecoilScalar = (wielded ? gun.Comp.RecoilWielded : gun.Comp.RecoilUnwielded);
			gunComponent.MinAngle = (wielded ? gun.Comp.ScatterWielded : gun.Comp.ScatterUnwielded);
			gunComponent.MaxAngle = gunComponent.MinAngle;
			RefreshBurstScatter(Entity<RMCSelectiveFireComponent>.op_Implicit((gun.Owner, gun.Comp)));
			_gunSystem.RefreshModifiers(Entity<GunComponent>.op_Implicit(gun.Owner));
			gunComponent.CurrentAngle = gunComponent.MinAngleModified;
		}
	}

	public void RefreshFireModes(Entity<RMCSelectiveFireComponent?> gun, bool forceValueRefresh = false)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		GunComponent gunComponent = default(GunComponent);
		if ((gun.Comp != null || ((EntitySystem)this).TryComp<RMCSelectiveFireComponent>(gun.Owner, ref gun.Comp)) && ((EntitySystem)this).TryComp<GunComponent>(gun.Owner, ref gunComponent))
		{
			SelectiveFire initialMode = gunComponent.SelectedMode;
			GetFireModesEvent ev = new GetFireModesEvent(gun.Comp.BaseFireModes);
			((EntitySystem)this).RaiseLocalEvent<GetFireModesEvent>(gun.Owner, ref ev, false);
			SetFireModes(Entity<GunComponent>.op_Implicit((gun.Owner, gunComponent)), ev.Modes, !forceValueRefresh && initialMode == gunComponent.SelectedMode);
			GunComponent gunComp = default(GunComponent);
			if (((EntitySystem)this).TryComp<GunComponent>(Entity<RMCSelectiveFireComponent>.op_Implicit(gun), ref gunComp) && (gunComp.AvailableModes & ev.Set) != SelectiveFire.Invalid)
			{
				_gunSystem.SelectFire(Entity<RMCSelectiveFireComponent>.op_Implicit(gun), gunComponent, ev.Set);
			}
			if (forceValueRefresh || initialMode != gunComponent.SelectedMode)
			{
				RefreshFireModeGunValues(Entity<RMCSelectiveFireComponent>.op_Implicit((gun.Owner, gun.Comp)));
			}
		}
	}

	public void RefreshModifiableFireModeValues(Entity<RMCSelectiveFireComponent?> gun)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (gun.Comp != null || ((EntitySystem)this).TryComp<RMCSelectiveFireComponent>(gun.Owner, ref gun.Comp))
		{
			GetFireModeValuesEvent ev = new GetFireModeValuesEvent(gun.Comp.BurstScatterMult);
			((EntitySystem)this).RaiseLocalEvent<GetFireModeValuesEvent>(gun.Owner, ref ev, false);
			gun.Comp.BurstScatterMultModified = ev.BurstScatterMult;
			RefreshWieldableFireModeValues(Entity<RMCSelectiveFireComponent>.op_Implicit((gun.Owner, gun.Comp)));
		}
	}

	private void RefreshBurstScatter(Entity<RMCSelectiveFireComponent> gun)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		GunComponent gunComponent = default(GunComponent);
		if (!((EntitySystem)this).TryComp<GunComponent>(gun.Owner, ref gunComponent))
		{
			return;
		}
		WieldableComponent wieldableComponent = default(WieldableComponent);
		bool wielded = ((EntitySystem)this).TryComp<WieldableComponent>(gun.Owner, ref wieldableComponent) && wieldableComponent.Wielded;
		if (ContainsMods(gun, gunComponent.SelectedMode))
		{
			SelectiveFireModifierSet mods = gun.Comp.Modifiers[gunComponent.SelectedMode];
			double mult = (mods.UseBurstScatterMult ? gun.Comp.BurstScatterMultModified : 1.0);
			gunComponent.MaxAngle = (wielded ? Angle.FromDegrees(Math.Max(((Angle)(ref gunComponent.MinAngle)).Degrees + mods.MaxScatterModifier * mult, ((Angle)(ref gunComponent.MinAngle)).Degrees)) : Angle.FromDegrees(Math.Max(((Angle)(ref gunComponent.MinAngle)).Degrees + mods.MaxScatterModifier * mult * mods.UnwieldedScatterMultiplier, ((Angle)(ref gunComponent.MinAngle)).Degrees)));
			if (mods.ShotsToMaxScatter.HasValue)
			{
				gunComponent.AngleIncrease = new Angle(Angle.op_Implicit(gunComponent.MaxAngle - gunComponent.MinAngle) / (double)mods.ShotsToMaxScatter.Value);
			}
		}
	}

	public void AddFireMode(Entity<GunComponent?> gun, SelectiveFire newMode)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (gun.Comp != null || ((EntitySystem)this).TryComp<GunComponent>(gun.Owner, ref gun.Comp))
		{
			gun.Comp.AvailableModes |= newMode;
			((EntitySystem)this).Dirty<GunComponent>(gun, (MetaDataComponent)null);
		}
	}

	public void SetFireModes(Entity<GunComponent?> gun, SelectiveFire modes, bool dirty = true)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		if ((gun.Comp != null || ((EntitySystem)this).TryComp<GunComponent>(gun.Owner, ref gun.Comp)) && (modes & (SelectiveFire.SemiAuto | SelectiveFire.Burst | SelectiveFire.FullAuto)) != SelectiveFire.Invalid)
		{
			gun.Comp.AvailableModes = SelectiveFire.SemiAuto | SelectiveFire.Burst | SelectiveFire.FullAuto;
			while ((gun.Comp.SelectedMode & modes) != gun.Comp.SelectedMode)
			{
				_gunSystem.CycleFire(gun.Owner, gun.Comp);
			}
			gun.Comp.AvailableModes = modes;
			if (dirty)
			{
				((EntitySystem)this).Dirty<GunComponent>(gun, (MetaDataComponent)null);
			}
		}
	}

	public void SetModifiers(Entity<RMCSelectiveFireComponent?> ent, Dictionary<SelectiveFire, SelectiveFireModifierSet> dict)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp != null || ((EntitySystem)this).TryComp<RMCSelectiveFireComponent>(ent.Owner, ref ent.Comp))
		{
			ent.Comp.Modifiers = new Dictionary<SelectiveFire, SelectiveFireModifierSet>(dict);
			RefreshFireModes(ent, forceValueRefresh: true);
			((EntitySystem)this).Dirty<RMCSelectiveFireComponent>(ent, (MetaDataComponent)null);
		}
	}
}
