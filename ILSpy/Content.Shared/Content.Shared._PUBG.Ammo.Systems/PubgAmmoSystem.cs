using System;
using System.Collections.Generic;
using Content.Shared._PUBG.Ammo.Components;
using Content.Shared._PUBG.Input;
using Content.Shared._PUBG.Loadout;
using Content.Shared.DoAfter;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Inventory;
using Content.Shared.Stacks;
using Content.Shared.Tag;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Shared._PUBG.Ammo.Systems;

public sealed class PubgAmmoSystem : EntitySystem
{
	[Dependency]
	private IPrototypeManager _proto;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private InventorySystem _inventory;

	[Dependency]
	private SharedGunSystem _gun;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private INetManager _netMan;

	[Dependency]
	private SharedStackSystem _stack;

	[Dependency]
	private TagSystem _tag;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private PubgWeaponModulesSystem _modules;

	public override void Initialize()
	{
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Expected O, but got Unknown
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Expected O, but got Unknown
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<PubgAmmoProviderComponent, TakeAmmoEvent>((ComponentEventHandler<PubgAmmoProviderComponent, TakeAmmoEvent>)OnTakeAmmo, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PubgAmmoProviderComponent, GetAmmoCountEvent>((ComponentEventRefHandler<PubgAmmoProviderComponent, GetAmmoCountEvent>)OnGetAmmoCount, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PubgAmmoProviderComponent, PubgReloadDoAfterEvent>((ComponentEventHandler<PubgAmmoProviderComponent, PubgReloadDoAfterEvent>)OnReloadComplete, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PubgAmmoProviderComponent, PubgUnloadDoAfterEvent>((ComponentEventHandler<PubgAmmoProviderComponent, PubgUnloadDoAfterEvent>)OnUnloadComplete, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PubgAmmoProviderComponent, GunShotEvent>((ComponentEventRefHandler<PubgAmmoProviderComponent, GunShotEvent>)OnGunShot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PubgAmmoProviderComponent, GotEquippedHandEvent>((ComponentEventHandler<PubgAmmoProviderComponent, GotEquippedHandEvent>)OnEquippedHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PubgAmmoProviderComponent, GotUnequippedHandEvent>((ComponentEventHandler<PubgAmmoProviderComponent, GotUnequippedHandEvent>)OnUnequippedHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PubgAmmoProviderComponent, HandSelectedEvent>((ComponentEventHandler<PubgAmmoProviderComponent, HandSelectedEvent>)OnHandSelected, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PubgAmmoProviderComponent, HandDeselectedEvent>((ComponentEventHandler<PubgAmmoProviderComponent, HandDeselectedEvent>)OnHandDeselected, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PubgAmmoProviderComponent, EntInsertedIntoContainerMessage>((ComponentEventHandler<PubgAmmoProviderComponent, EntInsertedIntoContainerMessage>)OnWeaponContainerInserted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PubgAmmoProviderComponent, EntRemovedFromContainerMessage>((ComponentEventHandler<PubgAmmoProviderComponent, EntRemovedFromContainerMessage>)OnWeaponContainerRemoved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EntInsertedIntoContainerMessage>((EntityEventHandler<EntInsertedIntoContainerMessage>)OnItemInserted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EntRemovedFromContainerMessage>((EntityEventHandler<EntRemovedFromContainerMessage>)OnItemRemoved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StackComponent, StackCountChangedEvent>((ComponentEventHandler<StackComponent, StackCountChangedEvent>)OnStackCountChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PubgAmmoProviderComponent, GetVerbsEvent<AlternativeVerb>>((ComponentEventHandler<PubgAmmoProviderComponent, GetVerbsEvent<AlternativeVerb>>)OnGetVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PubgMagazineModuleAmmoComponent, GetVerbsEvent<AlternativeVerb>>((ComponentEventHandler<PubgMagazineModuleAmmoComponent, GetVerbsEvent<AlternativeVerb>>)OnMagazineVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<PubgReloadRequestEvent>((EntitySessionEventHandler<PubgReloadRequestEvent>)OnReloadRequest, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<PubgUnloadRequestEvent>((EntitySessionEventHandler<PubgUnloadRequestEvent>)OnUnloadRequest, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<PubgAmmoRefreshRequestEvent>((EntitySessionEventHandler<PubgAmmoRefreshRequestEvent>)OnRefreshRequest, (Type[])null, (Type[])null);
		CommandBinds.Builder.Bind(PubgKeyFunctions.PubgReload, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate
		{
			if (_netMan.IsClient)
			{
				((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new PubgReloadRequestEvent());
			}
		}, (StateInputCmdDelegate)null, false, true)).Bind(PubgKeyFunctions.PubgUnload, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate
		{
			if (_netMan.IsClient)
			{
				((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new PubgUnloadRequestEvent());
			}
		}, (StateInputCmdDelegate)null, false, true)).Register<PubgAmmoSystem>();
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		CommandBinds.Unregister<PubgAmmoSystem>();
	}

	private void OnReloadRequest(PubgReloadRequestEvent msg, EntitySessionEventArgs args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? attachedEntity = ((EntitySessionEventArgs)(ref args)).SenderSession.AttachedEntity;
		if (attachedEntity.HasValue)
		{
			EntityUid userUid = attachedEntity.GetValueOrDefault();
			PubgAmmoProviderComponent ammoProvider = default(PubgAmmoProviderComponent);
			if (_hands.TryGetActiveItem(Entity<HandsComponent>.op_Implicit(userUid), out var held) && ((EntitySystem)this).TryComp<PubgAmmoProviderComponent>(held, ref ammoProvider))
			{
				TryStartReload(held.Value, userUid, ammoProvider);
			}
		}
	}

	private void OnUnloadRequest(PubgUnloadRequestEvent msg, EntitySessionEventArgs args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? attachedEntity = ((EntitySessionEventArgs)(ref args)).SenderSession.AttachedEntity;
		if (attachedEntity.HasValue)
		{
			EntityUid userUid = attachedEntity.GetValueOrDefault();
			PubgAmmoProviderComponent ammoProvider = default(PubgAmmoProviderComponent);
			if (_hands.TryGetActiveItem(Entity<HandsComponent>.op_Implicit(userUid), out var held) && ((EntitySystem)this).TryComp<PubgAmmoProviderComponent>(held, ref ammoProvider))
			{
				TryStartUnload(held.Value, userUid, ammoProvider);
			}
		}
	}

	private void OnRefreshRequest(PubgAmmoRefreshRequestEvent msg, EntitySessionEventArgs args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? attachedEntity = ((EntitySessionEventArgs)(ref args)).SenderSession.AttachedEntity;
		if (attachedEntity.HasValue)
		{
			EntityUid userUid = attachedEntity.GetValueOrDefault();
			SendAmmoUiState(userUid);
		}
	}

	public void SendAmmoUiState(EntityUid userUid)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (_netMan.IsServer)
		{
			PubgAmmoProviderComponent provider = default(PubgAmmoProviderComponent);
			if (_hands.TryGetActiveItem(Entity<HandsComponent>.op_Implicit(userUid), out var held) && ((EntitySystem)this).TryComp<PubgAmmoProviderComponent>(held.Value, ref provider))
			{
				UpdateAmmoUI(userUid, held.Value, provider);
			}
			else
			{
				((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new PubgAmmoUpdateEvent(0, 0, 0), userUid);
			}
		}
	}

	private void OnGetVerbs(EntityUid uid, PubgAmmoProviderComponent component, GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		if (!args.CanAccess || !args.CanInteract)
		{
			return;
		}
		EntityUid? activeItem = _hands.GetActiveItem(Entity<HandsComponent>.op_Implicit(args.User));
		EntityUid val = uid;
		if (!activeItem.HasValue || activeItem.GetValueOrDefault() != val)
		{
			return;
		}
		if (TryGetWeaponUnloadInfo(uid, component, out int ammoCount, out string _) && ammoCount > 0)
		{
			args.Verbs.Add(new AlternativeVerb
			{
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					TryStartUnload(uid, args.User, component);
				},
				Text = base.Loc.GetString("pubg-loadout-verb-unload-weapon"),
				Priority = 1
			});
		}
		if (!_modules.HasRequiredModulesForReload(uid))
		{
			return;
		}
		int maxAmmo = GetEffectiveMaxAmmo(uid, component);
		if (GetCurrentAmmoCount(uid, component) < maxAmmo && CountAmmoInInventory(args.User, component.AmmoTag) != 0)
		{
			AlternativeVerb verb = new AlternativeVerb
			{
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					TryStartReload(uid, args.User, component);
				},
				Text = "Перезарядить",
				Priority = 2
			};
			args.Verbs.Add(verb);
		}
	}

	private void OnMagazineVerbs(EntityUid uid, PubgMagazineModuleAmmoComponent component, GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanAccess && args.CanInteract && component.CurrentAmmo > 0 && !string.IsNullOrWhiteSpace(GetAmmoStackPrototype(component)))
		{
			args.Verbs.Add(new AlternativeVerb
			{
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					UnloadMagazine(uid, args.User, component);
				},
				Text = base.Loc.GetString("pubg-loadout-verb-unload-magazine"),
				Priority = 1
			});
		}
	}

	private void OnTakeAmmo(EntityUid uid, PubgAmmoProviderComponent component, TakeAmmoEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		int currentAmmo = GetCurrentAmmoCount(uid, component);
		int maxAmmo = GetEffectiveMaxAmmo(uid, component);
		int shotsToTake = Math.Min(args.Shots, currentAmmo);
		if (shotsToTake == 0)
		{
			args.Reason = $"No ammo in gun ({currentAmmo}/{maxAmmo})";
			return;
		}
		EntityPrototype proto = default(EntityPrototype);
		for (int i = 0; i < shotsToTake; i++)
		{
			if (!_proto.TryIndex<EntityPrototype>(component.AmmoPrototype, ref proto))
			{
				args.Reason = "Unknown ammo prototype: " + component.AmmoPrototype;
				return;
			}
			EntityUid ammoEnt = ((EntitySystem)this).Spawn(component.AmmoPrototype, args.Coordinates);
			args.Ammo.Add((ammoEnt, _gun.EnsureShootable(ammoEnt)));
		}
		SetCurrentAmmoCount(uid, currentAmmo - shotsToTake, component);
		if (args.User.HasValue)
		{
			UpdateAmmoUI(args.User.Value, uid, component);
		}
	}

	private void OnGetAmmoCount(EntityUid uid, PubgAmmoProviderComponent component, ref GetAmmoCountEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		int maxAmmo = GetEffectiveMaxAmmo(uid, component);
		args.Count = Math.Min(GetCurrentAmmoCount(uid, component), maxAmmo);
		args.Capacity = maxAmmo;
	}

	private void OnGunShot(EntityUid uid, PubgAmmoProviderComponent component, ref GunShotEvent args)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		if (component.IsReloading)
		{
			component.IsReloading = false;
			DoAfterId? activeReloadDoAfter = component.ActiveReloadDoAfter;
			if (activeReloadDoAfter.HasValue)
			{
				DoAfterId reloadDoAfterId = activeReloadDoAfter.GetValueOrDefault();
				_doAfter.Cancel(reloadDoAfterId);
			}
			component.ActiveReloadDoAfter = null;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		}
		if (component.IsUnloading)
		{
			component.IsUnloading = false;
			DoAfterId? activeReloadDoAfter = component.ActiveUnloadDoAfter;
			if (activeReloadDoAfter.HasValue)
			{
				DoAfterId unloadDoAfterId = activeReloadDoAfter.GetValueOrDefault();
				_doAfter.Cancel(unloadDoAfterId);
			}
			component.ActiveUnloadDoAfter = null;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		}
	}

	public int GetCurrentAmmoCount(EntityUid gunUid, PubgAmmoProviderComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<PubgAmmoProviderComponent>(gunUid, ref component, false))
		{
			return 0;
		}
		int currentAmmo = ResolveCurrentAmmo(gunUid, component);
		SyncWeaponCurrentAmmo(gunUid, component, currentAmmo);
		return currentAmmo;
	}

	public int GetMaxAmmoCount(EntityUid gunUid, PubgAmmoProviderComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<PubgAmmoProviderComponent>(gunUid, ref component, false))
		{
			return 0;
		}
		return GetEffectiveMaxAmmo(gunUid, component);
	}

	public void SetCurrentAmmoCount(EntityUid gunUid, int currentAmmo, PubgAmmoProviderComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<PubgAmmoProviderComponent>(gunUid, ref component, false))
		{
			return;
		}
		int maxAmmo = GetEffectiveMaxAmmo(gunUid, component);
		int clampedAmmo = Math.Clamp(currentAmmo, 0, maxAmmo);
		if (TryGetMagazineAmmoModule(gunUid, component, out EntityUid moduleUid, out PubgMagazineModuleAmmoComponent magazineAmmo))
		{
			bool dirtyMagazine = false;
			if (magazineAmmo.AmmoTag != component.AmmoTag)
			{
				magazineAmmo.AmmoTag = component.AmmoTag;
				dirtyMagazine = true;
			}
			if (magazineAmmo.AmmoStackPrototype != component.AmmoTag)
			{
				magazineAmmo.AmmoStackPrototype = component.AmmoTag;
				dirtyMagazine = true;
			}
			if (magazineAmmo.Capacity != maxAmmo)
			{
				magazineAmmo.Capacity = maxAmmo;
				dirtyMagazine = true;
			}
			if (magazineAmmo.CurrentAmmo != clampedAmmo)
			{
				magazineAmmo.CurrentAmmo = clampedAmmo;
				dirtyMagazine = true;
			}
			if (dirtyMagazine && _netMan.IsServer)
			{
				((EntitySystem)this).Dirty(moduleUid, (IComponent)(object)magazineAmmo, (MetaDataComponent)null);
			}
			SyncWeaponCurrentAmmo(gunUid, component, clampedAmmo);
			RefreshAmmoVisuals(gunUid, component, clampedAmmo);
		}
		else if (UsesMagazineModuleAmmo(gunUid))
		{
			SyncWeaponCurrentAmmo(gunUid, component, 0);
			RefreshAmmoVisuals(gunUid, component, 0);
		}
		else
		{
			SyncWeaponCurrentAmmo(gunUid, component, clampedAmmo);
		}
	}

	public void RefreshHeldWeaponAmmoUi(EntityUid gunUid, PubgAmmoProviderComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<PubgAmmoProviderComponent>(gunUid, ref component, false))
		{
			UpdateHeldWeaponAmmoUi(gunUid, component);
		}
	}

	public bool TryStartReload(EntityUid gunUid, EntityUid userUid, PubgAmmoProviderComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<PubgAmmoProviderComponent>(gunUid, ref component, true))
		{
			return false;
		}
		if (!_modules.HasRequiredModulesForReload(gunUid))
		{
			return false;
		}
		if (component.IsReloading)
		{
			return false;
		}
		int maxAmmo = GetEffectiveMaxAmmo(gunUid, component);
		if (GetCurrentAmmoCount(gunUid, component) >= maxAmmo)
		{
			return false;
		}
		if (CountAmmoInInventory(userUid, component.AmmoTag) == 0)
		{
			return false;
		}
		EntityUid? activeItem = _hands.GetActiveItem(Entity<HandsComponent>.op_Implicit(userUid));
		if (!activeItem.HasValue || activeItem.GetValueOrDefault() != gunUid)
		{
			return false;
		}
		float reloadMultiplier = _modules.GetReloadTimeMultiplier(gunUid);
		float reloadTime = MathF.Max(0.1f, component.ReloadTime * reloadMultiplier);
		DoAfterArgs doAfterArgs = new DoAfterArgs((IEntityManager)(object)base.EntityManager, userUid, TimeSpan.FromSeconds(reloadTime), new PubgReloadDoAfterEvent(), gunUid, gunUid, gunUid)
		{
			BreakOnMove = false,
			BreakOnDamage = false,
			NeedHand = true
		};
		if (!_doAfter.TryStartDoAfter(doAfterArgs, out var doAfterId))
		{
			return false;
		}
		component.IsReloading = true;
		component.ActiveReloadDoAfter = doAfterId;
		((EntitySystem)this).Dirty(gunUid, (IComponent)(object)component, (MetaDataComponent)null);
		if (component.ReloadSound != null && _netMan.IsServer)
		{
			_audio.PlayPvs(component.ReloadSound, gunUid, (AudioParams?)null);
		}
		return true;
	}

	private void OnReloadComplete(EntityUid gunUid, PubgAmmoProviderComponent component, PubgReloadDoAfterEvent args)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		component.ActiveReloadDoAfter = null;
		if (!component.IsReloading)
		{
			((EntitySystem)this).Dirty(gunUid, (IComponent)(object)component, (MetaDataComponent)null);
			return;
		}
		component.IsReloading = false;
		if (args.Cancelled || ((HandledEntityEventArgs)args).Handled)
		{
			((EntitySystem)this).Dirty(gunUid, (IComponent)(object)component, (MetaDataComponent)null);
			return;
		}
		if (!_modules.HasRequiredModulesForReload(gunUid))
		{
			((EntitySystem)this).Dirty(gunUid, (IComponent)(object)component, (MetaDataComponent)null);
			UpdateAmmoUI(args.User, gunUid, component);
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		int maxAmmo = GetEffectiveMaxAmmo(gunUid, component);
		int currentAmmo = GetCurrentAmmoCount(gunUid, component);
		int ammoNeeded = Math.Max(0, maxAmmo - currentAmmo);
		int ammoToLoad = ((component.ReloadAmount > 0) ? Math.Min(component.ReloadAmount, ammoNeeded) : ammoNeeded);
		int ammoTaken = TakeAmmoFromInventory(args.User, component.AmmoTag, ammoToLoad);
		SetCurrentAmmoCount(gunUid, currentAmmo + ammoTaken, component);
		UpdateAmmoUI(args.User, gunUid, component);
		if (component.ReloadAmount > 0 && ammoTaken > 0 && currentAmmo + ammoTaken < maxAmmo && CountAmmoInInventory(args.User, component.AmmoTag) > 0)
		{
			EntityUid? activeItem = _hands.GetActiveItem(Entity<HandsComponent>.op_Implicit(args.User));
			if (activeItem.HasValue && activeItem.GetValueOrDefault() == gunUid)
			{
				TryStartReload(gunUid, args.User, component);
			}
		}
	}

	private void UpdateAmmoUI(EntityUid userUid, EntityUid gunUid, PubgAmmoProviderComponent component)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if (_netMan.IsServer)
		{
			int reserveAmmo = CountAmmoInInventory(userUid, component.AmmoTag);
			string ammoType = component.GetAmmoTypeDisplay();
			int maxAmmo = GetEffectiveMaxAmmo(gunUid, component);
			PubgAmmoUpdateEvent ev = new PubgAmmoUpdateEvent(Math.Min(GetCurrentAmmoCount(gunUid, component), maxAmmo), maxAmmo, reserveAmmo, ammoType);
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)ev, userUid);
		}
	}

	private void OnEquippedHand(EntityUid uid, PubgAmmoProviderComponent component, GotEquippedHandEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		UpdateAmmoUI(args.User, uid, component);
	}

	private void OnUnequippedHand(EntityUid uid, PubgAmmoProviderComponent component, GotUnequippedHandEvent args)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (_netMan.IsServer)
		{
			PubgAmmoUpdateEvent ev = new PubgAmmoUpdateEvent(0, 0, 0);
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)ev, args.User);
		}
	}

	private void OnHandSelected(EntityUid uid, PubgAmmoProviderComponent component, HandSelectedEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		UpdateAmmoUI(args.User, uid, component);
	}

	private void OnHandDeselected(EntityUid uid, PubgAmmoProviderComponent component, HandDeselectedEvent args)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (_netMan.IsServer)
		{
			PubgAmmoUpdateEvent ev = new PubgAmmoUpdateEvent(0, 0, 0);
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)ev, args.User);
		}
	}

	private void OnWeaponContainerInserted(EntityUid uid, PubgAmmoProviderComponent component, EntInsertedIntoContainerMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetAmmoStorageSlot(uid, ((ContainerModifiedMessage)args).Container.ID, out PubgWeaponModuleSlotDefinition _))
		{
			PrepareInsertedMagazine(uid, component, ((ContainerModifiedMessage)args).Entity);
			SyncWeaponCurrentAmmo(uid, component, ResolveCurrentAmmo(uid, component));
			RefreshAmmoVisuals(uid, component);
			UpdateHeldWeaponAmmoUi(uid, component);
		}
	}

	private void OnWeaponContainerRemoved(EntityUid uid, PubgAmmoProviderComponent component, EntRemovedFromContainerMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetAmmoStorageSlot(uid, ((ContainerModifiedMessage)args).Container.ID, out PubgWeaponModuleSlotDefinition _))
		{
			SyncWeaponCurrentAmmo(uid, component, ResolveCurrentAmmo(uid, component));
			RefreshAmmoVisuals(uid, component);
			UpdateHeldWeaponAmmoUi(uid, component);
		}
	}

	private void OnItemInserted(EntInsertedIntoContainerMessage args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<StackComponent>(((ContainerModifiedMessage)args).Entity))
		{
			return;
		}
		EntityUid currentOwner = ((ContainerModifiedMessage)args).Container.Owner;
		EntityUid? playerUid = null;
		int depth = 0;
		BaseContainer parentContainer = default(BaseContainer);
		while (((EntityUid)(ref currentOwner)).IsValid() && depth < 10)
		{
			if (((EntitySystem)this).HasComp<InventoryComponent>(currentOwner))
			{
				playerUid = currentOwner;
				break;
			}
			if (!_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit(currentOwner), ref parentContainer))
			{
				break;
			}
			currentOwner = parentContainer.Owner;
			depth++;
		}
		HandsComponent handsComp = default(HandsComponent);
		if (!playerUid.HasValue || !((EntitySystem)this).TryComp<HandsComponent>(playerUid.Value, ref handsComp))
		{
			return;
		}
		EntityUid? weaponUid = null;
		PubgAmmoProviderComponent ammoProvider = null;
		PubgAmmoProviderComponent provider = default(PubgAmmoProviderComponent);
		foreach (string handId in handsComp.Hands.Keys)
		{
			if (_hands.TryGetHeldItem(Entity<HandsComponent>.op_Implicit(playerUid.Value), handId, out var heldEntity) && ((EntitySystem)this).TryComp<PubgAmmoProviderComponent>(heldEntity.Value, ref provider))
			{
				weaponUid = heldEntity.Value;
				ammoProvider = provider;
				break;
			}
		}
		if (weaponUid.HasValue && ammoProvider != null && _tag.HasTag(((ContainerModifiedMessage)args).Entity, ProtoId<TagPrototype>.op_Implicit(ammoProvider.AmmoTag)))
		{
			UpdateAmmoUI(playerUid.Value, weaponUid.Value, ammoProvider);
		}
	}

	private void OnItemRemoved(EntRemovedFromContainerMessage args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<StackComponent>(((ContainerModifiedMessage)args).Entity))
		{
			return;
		}
		EntityUid currentOwner = ((ContainerModifiedMessage)args).Container.Owner;
		EntityUid? playerUid = null;
		BaseContainer parentContainer = default(BaseContainer);
		while (((EntityUid)(ref currentOwner)).IsValid())
		{
			if (((EntitySystem)this).HasComp<InventoryComponent>(currentOwner))
			{
				playerUid = currentOwner;
				break;
			}
			if (!_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit(currentOwner), ref parentContainer))
			{
				break;
			}
			currentOwner = parentContainer.Owner;
		}
		HandsComponent handsComp = default(HandsComponent);
		if (!playerUid.HasValue || !((EntitySystem)this).TryComp<HandsComponent>(playerUid.Value, ref handsComp))
		{
			return;
		}
		EntityUid? weaponUid = null;
		PubgAmmoProviderComponent ammoProvider = null;
		PubgAmmoProviderComponent provider = default(PubgAmmoProviderComponent);
		foreach (string handId in handsComp.Hands.Keys)
		{
			if (_hands.TryGetHeldItem(Entity<HandsComponent>.op_Implicit(playerUid.Value), handId, out var heldEntity) && ((EntitySystem)this).TryComp<PubgAmmoProviderComponent>(heldEntity.Value, ref provider))
			{
				weaponUid = heldEntity.Value;
				ammoProvider = provider;
				break;
			}
		}
		if (weaponUid.HasValue && ammoProvider != null && _tag.HasTag(((ContainerModifiedMessage)args).Entity, ProtoId<TagPrototype>.op_Implicit(ammoProvider.AmmoTag)))
		{
			UpdateAmmoUI(playerUid.Value, weaponUid.Value, ammoProvider);
		}
	}

	private void OnStackCountChanged(EntityUid uid, StackComponent component, StackCountChangedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		if (!_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit(uid), ref container))
		{
			return;
		}
		EntityUid currentOwner = container.Owner;
		HandsComponent handsComp = default(HandsComponent);
		PubgAmmoProviderComponent ammoProvider = default(PubgAmmoProviderComponent);
		while (((EntityUid)(ref currentOwner)).IsValid())
		{
			if (((EntitySystem)this).HasComp<InventoryComponent>(currentOwner))
			{
				if (!((EntitySystem)this).TryComp<HandsComponent>(currentOwner, ref handsComp))
				{
					break;
				}
				{
					foreach (string handId in handsComp.Hands.Keys)
					{
						if (_hands.TryGetHeldItem(Entity<HandsComponent>.op_Implicit(currentOwner), handId, out var heldEntity) && ((EntitySystem)this).TryComp<PubgAmmoProviderComponent>(heldEntity.Value, ref ammoProvider))
						{
							if (_tag.HasTag(uid, ProtoId<TagPrototype>.op_Implicit(ammoProvider.AmmoTag)))
							{
								UpdateAmmoUI(currentOwner, heldEntity.Value, ammoProvider);
							}
							break;
						}
					}
					break;
				}
			}
			if (_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit(currentOwner), ref container))
			{
				currentOwner = container.Owner;
				continue;
			}
			break;
		}
	}

	private bool TryGetAmmoStorageSlot(EntityUid gunUid, string containerId, out PubgWeaponModuleSlotDefinition slotDefinition)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		slotDefinition = null;
		PubgWeaponModulesComponent modulesComp = default(PubgWeaponModulesComponent);
		if (!((EntitySystem)this).TryComp<PubgWeaponModulesComponent>(gunUid, ref modulesComp) || !_modules.TryGetSlotDefinition(gunUid, containerId, out slotDefinition, modulesComp) || !slotDefinition.StoresAmmo)
		{
			return false;
		}
		return true;
	}

	private bool UsesMagazineModuleAmmo(EntityUid gunUid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		PubgWeaponModulesComponent modulesComp = default(PubgWeaponModulesComponent);
		if (((EntitySystem)this).TryComp<PubgWeaponModulesComponent>(gunUid, ref modulesComp) && _modules.TryGetSlotDefinition(gunUid, PubgModuleSlotType.Magazine, out PubgWeaponModuleSlotDefinition slotDefinition, modulesComp))
		{
			return slotDefinition.StoresAmmo;
		}
		return false;
	}

	private bool TryGetWeaponUnloadInfo(EntityUid weaponUid, PubgAmmoProviderComponent component, out int ammoCount, out string ammoPrototype)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		ammoCount = 0;
		ammoPrototype = string.Empty;
		if (TryGetMagazineAmmoModule(weaponUid, component, out EntityUid _, out PubgMagazineModuleAmmoComponent magazineAmmo))
		{
			ammoCount = magazineAmmo.CurrentAmmo;
			ammoPrototype = GetAmmoStackPrototype(magazineAmmo);
			if (ammoCount > 0)
			{
				return !string.IsNullOrWhiteSpace(ammoPrototype);
			}
			return false;
		}
		ammoCount = GetCurrentAmmoCount(weaponUid, component);
		ammoPrototype = component.AmmoTag;
		if (ammoCount > 0)
		{
			return !string.IsNullOrWhiteSpace(ammoPrototype);
		}
		return false;
	}

	private bool TryGetMagazineAmmoModule(EntityUid gunUid, PubgAmmoProviderComponent component, out EntityUid moduleUid, out PubgMagazineModuleAmmoComponent magazineAmmo)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		moduleUid = EntityUid.Invalid;
		magazineAmmo = null;
		PubgWeaponModulesComponent modulesComp = default(PubgWeaponModulesComponent);
		if (!((EntitySystem)this).TryComp<PubgWeaponModulesComponent>(gunUid, ref modulesComp) || !_modules.TryGetInstalledModule(gunUid, PubgModuleSlotType.Magazine, out moduleUid, out PubgWeaponModuleSlotDefinition slotDefinition, modulesComp) || !slotDefinition.StoresAmmo)
		{
			return false;
		}
		PubgMagazineModuleAmmoComponent magazineAmmoComp = default(PubgMagazineModuleAmmoComponent);
		if (!((EntitySystem)this).TryComp<PubgMagazineModuleAmmoComponent>(moduleUid, ref magazineAmmoComp))
		{
			return false;
		}
		magazineAmmo = magazineAmmoComp;
		return true;
	}

	private bool TryGetOwningWeapon(EntityUid moduleUid, out EntityUid weaponUid, out PubgAmmoProviderComponent ammoProvider)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		weaponUid = EntityUid.Invalid;
		ammoProvider = null;
		BaseContainer containing = default(BaseContainer);
		if (!_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit(moduleUid), ref containing))
		{
			return false;
		}
		weaponUid = containing.Owner;
		PubgAmmoProviderComponent weaponAmmoProvider = default(PubgAmmoProviderComponent);
		if (!((EntitySystem)this).TryComp<PubgAmmoProviderComponent>(weaponUid, ref weaponAmmoProvider) || weaponAmmoProvider == null)
		{
			return false;
		}
		ammoProvider = weaponAmmoProvider;
		return true;
	}

	private string GetAmmoStackPrototype(PubgMagazineModuleAmmoComponent component)
	{
		if (!string.IsNullOrWhiteSpace(component.AmmoStackPrototype))
		{
			return component.AmmoStackPrototype;
		}
		return component.AmmoTag;
	}

	private void PrepareInsertedMagazine(EntityUid gunUid, PubgAmmoProviderComponent component, EntityUid moduleUid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		PubgMagazineModuleAmmoComponent magazineAmmo = default(PubgMagazineModuleAmmoComponent);
		if (!((EntitySystem)this).TryComp<PubgMagazineModuleAmmoComponent>(moduleUid, ref magazineAmmo))
		{
			return;
		}
		int maxAmmo = GetEffectiveMaxAmmo(gunUid, component);
		bool dirtyMagazine = false;
		int clampedAmmo = Math.Clamp(magazineAmmo.CurrentAmmo, 0, maxAmmo);
		if (magazineAmmo.CurrentAmmo != clampedAmmo)
		{
			magazineAmmo.CurrentAmmo = clampedAmmo;
			dirtyMagazine = true;
		}
		if (string.IsNullOrEmpty(magazineAmmo.AmmoTag) && magazineAmmo.CurrentAmmo == 0 && component.CurrentAmmo > 0)
		{
			magazineAmmo.CurrentAmmo = Math.Clamp(component.CurrentAmmo, 0, maxAmmo);
			dirtyMagazine = true;
		}
		if (!magazineAmmo.AmmoTag.Equals(component.AmmoTag, StringComparison.Ordinal))
		{
			if (!string.IsNullOrEmpty(magazineAmmo.AmmoTag) && magazineAmmo.CurrentAmmo != 0)
			{
				magazineAmmo.CurrentAmmo = 0;
				dirtyMagazine = true;
			}
			magazineAmmo.AmmoTag = component.AmmoTag;
			dirtyMagazine = true;
		}
		if (magazineAmmo.AmmoStackPrototype != component.AmmoTag)
		{
			magazineAmmo.AmmoStackPrototype = component.AmmoTag;
			dirtyMagazine = true;
		}
		if (magazineAmmo.Capacity != maxAmmo)
		{
			magazineAmmo.Capacity = maxAmmo;
			dirtyMagazine = true;
		}
		if (dirtyMagazine && _netMan.IsServer)
		{
			((EntitySystem)this).Dirty(moduleUid, (IComponent)(object)magazineAmmo, (MetaDataComponent)null);
		}
	}

	public bool TryStartUnload(EntityUid gunUid, EntityUid userUid, PubgAmmoProviderComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<PubgAmmoProviderComponent>(gunUid, ref component, true))
		{
			return false;
		}
		if (component.IsUnloading)
		{
			return false;
		}
		if (!TryGetWeaponUnloadInfo(gunUid, component, out int ammoCount, out string _))
		{
			return false;
		}
		if (ammoCount <= 0)
		{
			return false;
		}
		EntityUid? activeItem = _hands.GetActiveItem(Entity<HandsComponent>.op_Implicit(userUid));
		if (!activeItem.HasValue || activeItem.GetValueOrDefault() != gunUid)
		{
			return false;
		}
		float unloadTime = component.UnloadTime;
		DoAfterArgs doAfterArgs = new DoAfterArgs((IEntityManager)(object)base.EntityManager, userUid, TimeSpan.FromSeconds(unloadTime), new PubgUnloadDoAfterEvent(), gunUid, gunUid, gunUid)
		{
			BreakOnMove = false,
			BreakOnDamage = false,
			NeedHand = true
		};
		if (!_doAfter.TryStartDoAfter(doAfterArgs, out var doAfterId))
		{
			return false;
		}
		component.IsUnloading = true;
		component.ActiveUnloadDoAfter = doAfterId;
		((EntitySystem)this).Dirty(gunUid, (IComponent)(object)component, (MetaDataComponent)null);
		SoundSpecifier soundToPlay = component.UnloadSound ?? component.ReloadSound;
		if (soundToPlay != null && _netMan.IsServer)
		{
			_audio.PlayPvs(soundToPlay, gunUid, (AudioParams?)null);
		}
		return true;
	}

	private void OnUnloadComplete(EntityUid gunUid, PubgAmmoProviderComponent component, PubgUnloadDoAfterEvent args)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		component.ActiveUnloadDoAfter = null;
		if (!component.IsUnloading)
		{
			((EntitySystem)this).Dirty(gunUid, (IComponent)(object)component, (MetaDataComponent)null);
			return;
		}
		component.IsUnloading = false;
		if (args.Cancelled || ((HandledEntityEventArgs)args).Handled)
		{
			((EntitySystem)this).Dirty(gunUid, (IComponent)(object)component, (MetaDataComponent)null);
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		UnloadWeapon(gunUid, args.User, component);
	}

	private void UnloadWeapon(EntityUid weaponUid, EntityUid userUid, PubgAmmoProviderComponent component)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetWeaponUnloadInfo(weaponUid, component, out int ammoCount, out string ammoPrototype) && ammoCount > 0 && TryReturnAmmoToUser(userUid, ammoPrototype, ammoCount))
		{
			SetCurrentAmmoCount(weaponUid, 0, component);
			RefreshHeldWeaponAmmoUi(weaponUid, component);
		}
	}

	private void UnloadMagazine(EntityUid magazineUid, EntityUid userUid, PubgMagazineModuleAmmoComponent component)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		if (component.CurrentAmmo <= 0)
		{
			return;
		}
		string ammoPrototype = GetAmmoStackPrototype(component);
		if (string.IsNullOrWhiteSpace(ammoPrototype) || !TryReturnAmmoToUser(userUid, ammoPrototype, component.CurrentAmmo))
		{
			return;
		}
		if (TryGetOwningWeapon(magazineUid, out EntityUid weaponUid, out PubgAmmoProviderComponent ammoProvider))
		{
			SetCurrentAmmoCount(weaponUid, 0, ammoProvider);
			RefreshHeldWeaponAmmoUi(weaponUid, ammoProvider);
			return;
		}
		component.CurrentAmmo = 0;
		if (_netMan.IsServer)
		{
			((EntitySystem)this).Dirty(magazineUid, (IComponent)(object)component, (MetaDataComponent)null);
		}
	}

	private bool TryReturnAmmoToUser(EntityUid userUid, string ammoTag, int amount)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		if (amount <= 0 || string.IsNullOrWhiteSpace(ammoTag))
		{
			return false;
		}
		int remaining = amount;
		remaining -= TryMergeAmmoIntoUser(userUid, ammoTag, remaining);
		StackComponent spawnedStack = default(StackComponent);
		while (remaining > 0)
		{
			string ammoProto = FindAmmoEntityByTag(ammoTag);
			if (string.IsNullOrWhiteSpace(ammoProto))
			{
				return false;
			}
			EntityUid spawned = ((EntitySystem)this).Spawn(ammoProto, ((EntitySystem)this).Transform(userUid).Coordinates);
			int spawnCount = 1;
			if (((EntitySystem)this).TryComp<StackComponent>(spawned, ref spawnedStack))
			{
				spawnCount = Math.Min(_stack.GetMaxCount(spawnedStack), remaining);
				_stack.SetCount(spawned, spawnCount, spawnedStack);
			}
			remaining -= spawnCount;
			_stack.TryMergeToHands(spawned, userUid);
		}
		return true;
	}

	private string? FindAmmoEntityByTag(string ammoTag)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		TagComponent tagComp = default(TagComponent);
		StackComponent stackComp = default(StackComponent);
		foreach (EntityPrototype proto in _proto.EnumeratePrototypes<EntityPrototype>())
		{
			if (!proto.TryGetComponent<TagComponent>(ref tagComp, base.EntityManager.ComponentFactory) || !proto.TryGetComponent<StackComponent>(ref stackComp, base.EntityManager.ComponentFactory))
			{
				continue;
			}
			foreach (ProtoId<TagPrototype> tag2 in tagComp.Tags)
			{
				if (((object)tag2/*cast due to constrained. prefix*/).ToString().Equals(ammoTag, StringComparison.OrdinalIgnoreCase))
				{
					return proto.ID;
				}
			}
		}
		return null;
	}

	private int TryMergeAmmoIntoUser(EntityUid userUid, string ammoTag, int amount)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		if (amount <= 0)
		{
			return 0;
		}
		int transferred = 0;
		HashSet<EntityUid> visited = new HashSet<EntityUid>();
		if (_inventory.TryGetContainerSlotEnumerator(Entity<InventoryComponent>.op_Implicit(userUid), out var enumerator))
		{
			ContainerSlot slot;
			while (enumerator.MoveNext(out slot) && transferred < amount)
			{
				EntityUid? containedEntity = slot.ContainedEntity;
				if (containedEntity.HasValue)
				{
					EntityUid item = containedEntity.GetValueOrDefault();
					transferred += TryMergeAmmoIntoEntity(item, ammoTag, amount - transferred, visited);
				}
			}
		}
		HandsComponent hands = default(HandsComponent);
		if (((EntitySystem)this).TryComp<HandsComponent>(userUid, ref hands))
		{
			foreach (EntityUid held in _hands.EnumerateHeld(Entity<HandsComponent>.op_Implicit((userUid, hands))))
			{
				if (transferred >= amount)
				{
					break;
				}
				transferred += TryMergeAmmoIntoEntity(held, ammoTag, amount - transferred, visited);
			}
		}
		return transferred;
	}

	private unsafe int TryMergeAmmoIntoEntity(EntityUid entity, string ammoTag, int amount, HashSet<EntityUid> visited)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		if (amount <= 0 || !visited.Add(entity))
		{
			return 0;
		}
		int transferred = 0;
		StackComponent stack = default(StackComponent);
		if (((EntitySystem)this).TryComp<StackComponent>(entity, ref stack) && _tag.HasTag(entity, ProtoId<TagPrototype>.op_Implicit(ammoTag)))
		{
			int add = Math.Min(_stack.GetAvailableSpace(stack), amount);
			if (add > 0)
			{
				_stack.SetCount(entity, stack.Count + add, stack);
				transferred += add;
			}
		}
		ContainerManagerComponent containerManager = default(ContainerManagerComponent);
		if (transferred >= amount || !((EntitySystem)this).TryComp<ContainerManagerComponent>(entity, ref containerManager))
		{
			return transferred;
		}
		AllContainersEnumerable allContainers = _container.GetAllContainers(entity, containerManager);
		AllContainersEnumerator enumerator = ((AllContainersEnumerable)(ref allContainers)).GetEnumerator();
		try
		{
			while (((AllContainersEnumerator)(ref enumerator)).MoveNext())
			{
				foreach (EntityUid contained in ((AllContainersEnumerator)(ref enumerator)).Current.ContainedEntities)
				{
					if (transferred >= amount)
					{
						break;
					}
					transferred += TryMergeAmmoIntoEntity(contained, ammoTag, amount - transferred, visited);
				}
				if (transferred >= amount)
				{
					break;
				}
			}
		}
		finally
		{
			((IDisposable)(*(AllContainersEnumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		return transferred;
	}

	private int ResolveCurrentAmmo(EntityUid gunUid, PubgAmmoProviderComponent component)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		int maxAmmo = GetEffectiveMaxAmmo(gunUid, component);
		if (TryGetMagazineAmmoModule(gunUid, component, out EntityUid _, out PubgMagazineModuleAmmoComponent magazineAmmo))
		{
			return Math.Clamp(magazineAmmo.CurrentAmmo, 0, maxAmmo);
		}
		if (UsesMagazineModuleAmmo(gunUid))
		{
			return 0;
		}
		return Math.Clamp(component.CurrentAmmo, 0, maxAmmo);
	}

	private void SyncWeaponCurrentAmmo(EntityUid gunUid, PubgAmmoProviderComponent component, int currentAmmo)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (component.CurrentAmmo != currentAmmo)
		{
			component.CurrentAmmo = currentAmmo;
			if (_netMan.IsServer)
			{
				((EntitySystem)this).Dirty(gunUid, (IComponent)(object)component, (MetaDataComponent)null);
			}
		}
	}

	private void RefreshAmmoVisuals(EntityUid gunUid, PubgAmmoProviderComponent component, int? currentAmmo = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		AppearanceComponent appearance = default(AppearanceComponent);
		if (UsesMagazineModuleAmmo(gunUid) && ((EntitySystem)this).TryComp<AppearanceComponent>(gunUid, ref appearance))
		{
			int maxAmmo = GetEffectiveMaxAmmo(gunUid, component);
			int ammoCount = Math.Clamp(currentAmmo ?? ResolveCurrentAmmo(gunUid, component), 0, maxAmmo);
			EntityUid moduleUid;
			PubgMagazineModuleAmmoComponent magazineAmmo;
			bool magLoaded = TryGetMagazineAmmoModule(gunUid, component, out moduleUid, out magazineAmmo);
			_appearance.SetData(gunUid, (Enum)AmmoVisuals.MagLoaded, (object)magLoaded, appearance);
			_appearance.SetData(gunUid, (Enum)AmmoVisuals.HasAmmo, (object)(ammoCount > 0), appearance);
			_appearance.SetData(gunUid, (Enum)AmmoVisuals.AmmoCount, (object)ammoCount, appearance);
			_appearance.SetData(gunUid, (Enum)AmmoVisuals.AmmoMax, (object)maxAmmo, appearance);
		}
	}

	private bool TryGetHoldingUser(EntityUid gunUid, out EntityUid userUid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		userUid = EntityUid.Invalid;
		BaseContainer containing = default(BaseContainer);
		if (!_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit(gunUid), ref containing))
		{
			return false;
		}
		EntityUid holder = containing.Owner;
		HandsComponent handsComp = default(HandsComponent);
		if (!((EntitySystem)this).TryComp<HandsComponent>(holder, ref handsComp) || !_hands.IsHolding(Entity<HandsComponent>.op_Implicit((holder, handsComp)), gunUid))
		{
			return false;
		}
		userUid = holder;
		return true;
	}

	private void UpdateHeldWeaponAmmoUi(EntityUid gunUid, PubgAmmoProviderComponent component)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if (_netMan.IsServer && TryGetHoldingUser(gunUid, out var userUid))
		{
			EntityUid? activeItem = _hands.GetActiveItem(Entity<HandsComponent>.op_Implicit(userUid));
			if (activeItem.HasValue && !(activeItem.GetValueOrDefault() != gunUid))
			{
				UpdateAmmoUI(userUid, gunUid, component);
			}
		}
	}

	private int GetEffectiveMaxAmmo(EntityUid gunUid, PubgAmmoProviderComponent component)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		int maxAmmo = component.MaxAmmo;
		PubgWeaponModulesComponent modulesComp = default(PubgWeaponModulesComponent);
		if (((EntitySystem)this).TryComp<PubgWeaponModulesComponent>(gunUid, ref modulesComp))
		{
			maxAmmo += _modules.GetMagazineCapacityBonus(gunUid, modulesComp);
		}
		return Math.Max(1, maxAmmo);
	}

	private int CountAmmoInInventory(EntityUid userUid, string ammoTag)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		int count = 0;
		if (_inventory.TryGetContainerSlotEnumerator(Entity<InventoryComponent>.op_Implicit(userUid), out var enumerator))
		{
			ContainerSlot slot;
			while (enumerator.MoveNext(out slot))
			{
				if (slot.ContainedEntity.HasValue)
				{
					count += CountAmmoInEntity(slot.ContainedEntity.Value, ammoTag);
				}
			}
		}
		HandsComponent handsComp = default(HandsComponent);
		if (((EntitySystem)this).TryComp<HandsComponent>(userUid, ref handsComp))
		{
			foreach (string handId in handsComp.Hands.Keys)
			{
				if (_hands.TryGetHeldItem(Entity<HandsComponent>.op_Implicit(userUid), handId, out var heldEntity) && heldEntity.HasValue)
				{
					count += CountAmmoInEntity(heldEntity.Value, ammoTag);
				}
			}
		}
		return count;
	}

	private unsafe int CountAmmoInEntity(EntityUid entity, string ammoTag)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		int count = 0;
		StackComponent stack = default(StackComponent);
		if (_tag.HasTag(entity, ProtoId<TagPrototype>.op_Implicit(ammoTag)) && ((EntitySystem)this).TryComp<StackComponent>(entity, ref stack))
		{
			count += stack.Count;
		}
		ContainerManagerComponent containerManager = default(ContainerManagerComponent);
		if (((EntitySystem)this).TryComp<ContainerManagerComponent>(entity, ref containerManager))
		{
			AllContainersEnumerable allContainers = _container.GetAllContainers(entity, containerManager);
			AllContainersEnumerator enumerator = ((AllContainersEnumerable)(ref allContainers)).GetEnumerator();
			try
			{
				while (((AllContainersEnumerator)(ref enumerator)).MoveNext())
				{
					foreach (EntityUid containedEntity in ((AllContainersEnumerator)(ref enumerator)).Current.ContainedEntities)
					{
						count += CountAmmoInEntity(containedEntity, ammoTag);
					}
				}
			}
			finally
			{
				((IDisposable)(*(AllContainersEnumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
			}
		}
		return count;
	}

	private int TakeAmmoFromInventory(EntityUid userUid, string ammoTag, int amount)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		int taken = 0;
		HandsComponent handsComp = default(HandsComponent);
		if (((EntitySystem)this).TryComp<HandsComponent>(userUid, ref handsComp))
		{
			foreach (string handId in handsComp.Hands.Keys)
			{
				if (_hands.TryGetHeldItem(Entity<HandsComponent>.op_Implicit(userUid), handId, out var heldEntity) && heldEntity.HasValue)
				{
					taken += TakeAmmoFromEntity(heldEntity.Value, ammoTag, amount - taken);
					if (taken >= amount)
					{
						return taken;
					}
				}
			}
		}
		if (_inventory.TryGetContainerSlotEnumerator(Entity<InventoryComponent>.op_Implicit(userUid), out var enumerator2))
		{
			ContainerSlot slot;
			while (enumerator2.MoveNext(out slot) && taken < amount)
			{
				if (slot.ContainedEntity.HasValue)
				{
					taken += TakeAmmoFromEntity(slot.ContainedEntity.Value, ammoTag, amount - taken);
				}
			}
		}
		return taken;
	}

	private unsafe int TakeAmmoFromEntity(EntityUid entity, string ammoTag, int amount)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		int taken = 0;
		StackComponent stack = default(StackComponent);
		if (_tag.HasTag(entity, ProtoId<TagPrototype>.op_Implicit(ammoTag)) && ((EntitySystem)this).TryComp<StackComponent>(entity, ref stack))
		{
			int toTake = Math.Min(amount, stack.Count);
			_stack.SetCount(entity, stack.Count - toTake);
			taken += toTake;
			if (stack.Count <= 0 && _netMan.IsServer)
			{
				((EntitySystem)this).QueueDel((EntityUid?)entity);
			}
			if (taken >= amount)
			{
				return taken;
			}
		}
		ContainerManagerComponent containerManager = default(ContainerManagerComponent);
		if (((EntitySystem)this).TryComp<ContainerManagerComponent>(entity, ref containerManager))
		{
			AllContainersEnumerable allContainers = _container.GetAllContainers(entity, containerManager);
			AllContainersEnumerator enumerator = ((AllContainersEnumerable)(ref allContainers)).GetEnumerator();
			try
			{
				while (((AllContainersEnumerator)(ref enumerator)).MoveNext())
				{
					foreach (EntityUid containedEntity in new List<EntityUid>(((AllContainersEnumerator)(ref enumerator)).Current.ContainedEntities))
					{
						taken += TakeAmmoFromEntity(containedEntity, ammoTag, amount - taken);
						if (taken >= amount)
						{
							return taken;
						}
					}
				}
			}
			finally
			{
				((IDisposable)(*(AllContainersEnumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
			}
		}
		return taken;
	}
}
