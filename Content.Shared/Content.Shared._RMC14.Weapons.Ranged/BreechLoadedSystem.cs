using System;
using Content.Shared._RMC14.Weapons.Common;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Tag;
using Content.Shared.Timing;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared._RMC14.Weapons.Ranged;

public sealed class BreechLoadedSystem : EntitySystem
{
	[Dependency]
	private SharedAppearanceSystem _appearanceSystem;

	[Dependency]
	private SharedAudioSystem _audioSystem;

	[Dependency]
	private SharedPopupSystem _popupSystem;

	[Dependency]
	private TagSystem _tagSystem;

	[Dependency]
	private UseDelaySystem _useDelay;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<BreechLoadedComponent, AttemptShootEvent>((EntityEventRefHandler<BreechLoadedComponent, AttemptShootEvent>)OnAttemptShoot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BreechLoadedComponent, GunShotEvent>((EntityEventRefHandler<BreechLoadedComponent, GunShotEvent>)OnGunShot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BreechLoadedComponent, RMCTryAmmoEjectEvent>((EntityEventRefHandler<BreechLoadedComponent, RMCTryAmmoEjectEvent>)OnTryAmmoEject, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BreechLoadedComponent, UniqueActionEvent>((EntityEventRefHandler<BreechLoadedComponent, UniqueActionEvent>)OnUniqueAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BreechLoadedComponent, InteractUsingEvent>((EntityEventRefHandler<BreechLoadedComponent, InteractUsingEvent>)OnInteractUsing, new Type[1] { typeof(SharedGunSystem) }, (Type[])null);
	}

	private void OnAttemptShoot(Entity<BreechLoadedComponent> gun, ref AttemptShootEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && (gun.Comp.Open || (gun.Comp.NeedOpenClose && !gun.Comp.Ready)))
		{
			args.Cancelled = true;
			if (gun.Comp.Open)
			{
				_popupSystem.PopupClient(base.Loc.GetString("rmc-breech-loaded-open-shoot-attempt"), args.User, args.User);
			}
			else
			{
				_popupSystem.PopupClient(base.Loc.GetString("rmc-breech-loaded-not-ready-to-shoot"), args.User, args.User);
			}
		}
	}

	private void OnGunShot(Entity<BreechLoadedComponent> gun, ref GunShotEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (gun.Comp.NeedOpenClose)
		{
			gun.Comp.Ready = false;
			((EntitySystem)this).Dirty<BreechLoadedComponent>(gun, (MetaDataComponent)null);
		}
	}

	private void OnTryAmmoEject(Entity<BreechLoadedComponent> gun, ref RMCTryAmmoEjectEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (!gun.Comp.Open)
		{
			_popupSystem.PopupClient(base.Loc.GetString("rmc-breech-loaded-closed-extract-attempt"), args.User, args.User);
			args.Cancelled = true;
		}
	}

	private void OnUniqueAction(Entity<BreechLoadedComponent> gun, ref UniqueActionEvent args)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		UseDelayComponent useDelay = default(UseDelayComponent);
		if (((EntitySystem)this).TryComp<UseDelayComponent>(Entity<BreechLoadedComponent>.op_Implicit(gun), ref useDelay) && _useDelay.IsDelayed(Entity<UseDelayComponent>.op_Implicit((Entity<BreechLoadedComponent>.op_Implicit(gun), useDelay)), gun.Comp.DelayId))
		{
			string actionLocale = (gun.Comp.Open ? base.Loc.GetString("rmc-breech-loaded-close") : base.Loc.GetString("rmc-breech-loaded-open"));
			string popup = base.Loc.GetString("rmc-breech-loaded-toggle-attempt-cooldown", (ValueTuple<string, object>)("action", actionLocale));
			_popupSystem.PopupClient(popup, args.UserUid, args.UserUid);
			return;
		}
		gun.Comp.Open = !gun.Comp.Open;
		if (!gun.Comp.Open)
		{
			gun.Comp.Ready = true;
		}
		AppearanceComponent appearanceComponent = default(AppearanceComponent);
		if (gun.Comp.ShowBreechOpen && ((EntitySystem)this).TryComp<AppearanceComponent>(gun.Owner, ref appearanceComponent))
		{
			_appearanceSystem.SetData(Entity<BreechLoadedComponent>.op_Implicit(gun), (Enum)BreechVisuals.Open, (object)gun.Comp.Open, appearanceComponent);
		}
		if (useDelay != null)
		{
			_useDelay.SetLength(Entity<UseDelayComponent>.op_Implicit((Entity<BreechLoadedComponent>.op_Implicit(gun), useDelay)), gun.Comp.ToggleDelay, gun.Comp.DelayId);
			_useDelay.TryResetDelay(Entity<UseDelayComponent>.op_Implicit((Entity<BreechLoadedComponent>.op_Implicit(gun), useDelay)), checkDelayed: false, gun.Comp.DelayId);
		}
		((EntitySystem)this).Dirty<BreechLoadedComponent>(gun, (MetaDataComponent)null);
		SoundSpecifier sound = (gun.Comp.Open ? gun.Comp.OpenSound : gun.Comp.CloseSound);
		_audioSystem.PlayPredicted(sound, Entity<BreechLoadedComponent>.op_Implicit(gun), (EntityUid?)args.UserUid, (AudioParams?)null);
	}

	private void OnInteractUsing(Entity<BreechLoadedComponent> gun, ref InteractUsingEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		BallisticAmmoProviderComponent ammoProviderComponent = default(BallisticAmmoProviderComponent);
		if (!gun.Comp.Open && ((EntitySystem)this).TryComp<BallisticAmmoProviderComponent>(gun.Owner, ref ammoProviderComponent) && ammoProviderComponent.Whitelist != null && ammoProviderComponent.Whitelist.Tags != null && _tagSystem.HasAnyTag(args.Used, ammoProviderComponent.Whitelist.Tags))
		{
			_popupSystem.PopupClient(base.Loc.GetString("rmc-breech-loaded-closed-load-attempt"), args.User, args.User);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}
}
