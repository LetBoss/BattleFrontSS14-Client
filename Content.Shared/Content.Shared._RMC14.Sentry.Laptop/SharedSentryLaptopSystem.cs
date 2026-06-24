using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.Weapons.Ranged.IFF;
using Content.Shared.Damage;
using Content.Shared.DeviceLinking;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Item.ItemToggle;
using Content.Shared.Placeable;
using Content.Shared.Popups;
using Content.Shared.UserInterface;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Sentry.Laptop;

public abstract class SharedSentryLaptopSystem : EntitySystem
{
	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private ItemToggleSystem _toggle;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedUserInterfaceSystem _ui;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedSentryTargetingSystem _sentryTargeting;

	[Dependency]
	private GunIFFSystem _iff;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private AreaSystem _area;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private SharedContainerSystem _containers;

	[Dependency]
	private SharedDeviceLinkSystem _deviceLink;

	private const float UpdateInterval = 1f;

	private float _updateTimer;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<SentryLaptopComponent, AfterInteractEvent>((EntityEventRefHandler<SentryLaptopComponent, AfterInteractEvent>)OnLaptopAfterInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SentryLaptopComponent, ComponentShutdown>((EntityEventRefHandler<SentryLaptopComponent, ComponentShutdown>)OnLaptopShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SentryLaptopComponent, ActivatableUIOpenAttemptEvent>((EntityEventRefHandler<SentryLaptopComponent, ActivatableUIOpenAttemptEvent>)OnLaptopUIOpenAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SentryLaptopComponent, EntParentChangedMessage>((EntityEventRefHandler<SentryLaptopComponent, EntParentChangedMessage>)OnLaptopParentChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SentryLaptopLinkedComponent, ComponentShutdown>((EntityEventRefHandler<SentryLaptopLinkedComponent, ComponentShutdown>)OnSentryLinkedShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SentryLaptopComponent, SentryLaptopUnlinkBuiMsg>((EntityEventRefHandler<SentryLaptopComponent, SentryLaptopUnlinkBuiMsg>)OnUnlinkMessage, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SentryLaptopComponent, SentryLaptopUnlinkAllBuiMsg>((EntityEventRefHandler<SentryLaptopComponent, SentryLaptopUnlinkAllBuiMsg>)OnUnlinkAllMessage, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SentryLaptopComponent, SentryLaptopSetFactionsBuiMsg>((EntityEventRefHandler<SentryLaptopComponent, SentryLaptopSetFactionsBuiMsg>)OnSetFactionsMessage, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SentryLaptopComponent, SentryLaptopToggleFactionBuiMsg>((EntityEventRefHandler<SentryLaptopComponent, SentryLaptopToggleFactionBuiMsg>)OnToggleFactionMessage, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SentryLaptopComponent, SentryLaptopResetTargetingBuiMsg>((EntityEventRefHandler<SentryLaptopComponent, SentryLaptopResetTargetingBuiMsg>)OnResetTargetingMessage, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SentryLaptopComponent, SentryLaptopTogglePowerBuiMsg>((EntityEventRefHandler<SentryLaptopComponent, SentryLaptopTogglePowerBuiMsg>)OnTogglePowerMessage, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SentryComponent, DamageChangedEvent>((EntityEventRefHandler<SentryComponent, DamageChangedEvent>)OnSentryDamageChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SentryComponent, GunShotEvent>((EntityEventRefHandler<SentryComponent, GunShotEvent>)OnSentryShot, (Type[])null, (Type[])null);
	}

	public override void Update(float frameTime)
	{
		((EntitySystem)this).Update(frameTime);
		if (_net.IsServer)
		{
			_updateTimer += frameTime;
			if (!(_updateTimer < 1f))
			{
				_updateTimer = 0f;
				UpdateAllOpenUIs();
				CheckSentryAlerts();
			}
		}
	}

	private void UpdateAllOpenUIs()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<SentryLaptopComponent> query = ((EntitySystem)this).EntityQueryEnumerator<SentryLaptopComponent>();
		EntityUid uid = default(EntityUid);
		SentryLaptopComponent laptop = default(SentryLaptopComponent);
		while (query.MoveNext(ref uid, ref laptop))
		{
			if (_ui.IsUiOpen(Entity<UserInterfaceComponent>.op_Implicit(uid), (Enum)SentryLaptopUiKey.Key))
			{
				UpdateUI(Entity<SentryLaptopComponent>.op_Implicit((uid, laptop)));
			}
		}
	}

	private void CheckSentryAlerts()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<SentryLaptopComponent> laptopQuery = ((EntitySystem)this).EntityQueryEnumerator<SentryLaptopComponent>();
		EntityUid laptopUid = default(EntityUid);
		SentryLaptopComponent laptop = default(SentryLaptopComponent);
		SentryComponent sentry = default(SentryComponent);
		while (laptopQuery.MoveNext(ref laptopUid, ref laptop))
		{
			if (!laptop.IsPowered || !_ui.IsUiOpen(Entity<UserInterfaceComponent>.op_Implicit(laptopUid), (Enum)SentryLaptopUiKey.Key))
			{
				continue;
			}
			foreach (EntityUid sentryUid in GetLinkedSentries(Entity<SentryLaptopComponent>.op_Implicit((laptopUid, laptop))))
			{
				if (((EntitySystem)this).TryComp<SentryComponent>(sentryUid, ref sentry))
				{
					CheckLowAmmoAlert(laptopUid, Entity<SentryComponent>.op_Implicit((sentryUid, sentry)), laptop, time);
					CheckHealthAlert(laptopUid, Entity<SentryComponent>.op_Implicit((sentryUid, sentry)), laptop, time);
				}
			}
		}
	}

	private void CheckLowAmmoAlert(EntityUid laptopUid, Entity<SentryComponent> sentry, SentryLaptopComponent laptop, TimeSpan time)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		int maxAmmo;
		int ammo = GetSentryAmmo(Entity<SentryComponent>.op_Implicit(sentry), out maxAmmo);
		if (maxAmmo > 0 && (float)ammo / (float)maxAmmo <= sentry.Comp.LowAmmoThreshold && time - sentry.Comp.LastLowAmmoAlert > sentry.Comp.AlertCooldown)
		{
			sentry.Comp.LastLowAmmoAlert = time;
			((EntitySystem)this).Dirty<SentryComponent>(sentry, (MetaDataComponent)null);
			SendAlert(laptopUid, Entity<SentryComponent>.op_Implicit(sentry), SentryAlertType.LowAmmo, $"{GetSentryDisplayName(Entity<SentryLaptopComponent>.op_Implicit((laptopUid, laptop)), Entity<SentryComponent>.op_Implicit(sentry))}: LOW AMMO ({ammo}/{maxAmmo})");
		}
	}

	private void CheckHealthAlert(EntityUid laptopUid, Entity<SentryComponent> sentry, SentryLaptopComponent laptop, TimeSpan time)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		float maxHealth;
		float health = GetSentryHealth(sentry, out maxHealth);
		if (maxHealth > 0f && health / maxHealth <= sentry.Comp.CriticalHealthThreshold && time - sentry.Comp.LastHealthAlert > sentry.Comp.AlertCooldown)
		{
			sentry.Comp.LastHealthAlert = time;
			((EntitySystem)this).Dirty<SentryComponent>(sentry, (MetaDataComponent)null);
			SendAlert(laptopUid, Entity<SentryComponent>.op_Implicit(sentry), SentryAlertType.CriticalHealth, GetSentryDisplayName(Entity<SentryLaptopComponent>.op_Implicit((laptopUid, laptop)), Entity<SentryComponent>.op_Implicit(sentry)) + ": CRITICAL DAMAGE");
		}
	}

	private void OnSentryDamageChanged(Entity<SentryComponent> sentry, ref DamageChangedEvent args)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsServer || !args.DamageIncreased || args.DamageDelta == null)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		if (!(time - sentry.Comp.LastHealthAlert < sentry.Comp.AlertCooldown))
		{
			sentry.Comp.LastHealthAlert = time;
			((EntitySystem)this).Dirty<SentryComponent>(sentry, (MetaDataComponent)null);
			if (TryGetLinkedLaptop(sentry.Owner, out Entity<SentryLaptopComponent>? laptop))
			{
				float maxHealth;
				float health = GetSentryHealth(sentry, out maxHealth);
				int healthPercent = ((maxHealth > 0f) ? ((int)(health / maxHealth * 100f)) : 0);
				Entity<SentryLaptopComponent> laptopEntity = laptop.Value;
				SendAlert(laptopEntity.Owner, Entity<SentryComponent>.op_Implicit(sentry), SentryAlertType.Damaged, $"{GetSentryDisplayName(laptopEntity, Entity<SentryComponent>.op_Implicit(sentry))}: Taking damage! ({healthPercent}% health)");
			}
		}
	}

	private void OnSentryShot(Entity<SentryComponent> sentry, ref GunShotEvent args)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsServer)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		GunComponent gun = default(GunComponent);
		if (!(time - sentry.Comp.LastTargetAlert < sentry.Comp.AlertCooldown) && ((EntitySystem)this).TryComp<GunComponent>(Entity<SentryComponent>.op_Implicit(sentry), ref gun) && gun.Target.HasValue)
		{
			sentry.Comp.LastTargetAlert = time;
			((EntitySystem)this).Dirty<SentryComponent>(sentry, (MetaDataComponent)null);
			if (TryGetLinkedLaptop(sentry.Owner, out Entity<SentryLaptopComponent>? laptop))
			{
				string targetName = ((EntitySystem)this).Name(gun.Target.Value, (MetaDataComponent)null);
				Entity<SentryLaptopComponent> laptopEntity = laptop.Value;
				SendAlert(laptopEntity.Owner, Entity<SentryComponent>.op_Implicit(sentry), SentryAlertType.TargetAcquired, GetSentryDisplayName(laptopEntity, Entity<SentryComponent>.op_Implicit(sentry)) + ": Engaging " + targetName);
			}
		}
	}

	private void SendAlert(EntityUid laptop, EntityUid sentry, SentryAlertType alertType, string message)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsServer)
		{
			return;
		}
		(string color, int size) alertStyle = GetAlertStyle(alertType);
		string color = alertStyle.color;
		int size = alertStyle.size;
		SentryAlertEvent alert = new SentryAlertEvent(((EntitySystem)this).GetNetEntity(sentry, (MetaDataComponent)null), alertType, message, color, size);
		if (_ui.IsUiOpen(Entity<UserInterfaceComponent>.op_Implicit(laptop), (Enum)SentryLaptopUiKey.Key))
		{
			_ui.ServerSendUiMessage(Entity<UserInterfaceComponent>.op_Implicit(laptop), (Enum)SentryLaptopUiKey.Key, (BoundUserInterfaceMessage)(object)alert);
			return;
		}
		EntityUid parent = ((EntitySystem)this).Transform(laptop).ParentUid;
		if (((EntitySystem)this).HasComp<PlaceableSurfaceComponent>(parent))
		{
			PopupType popupType = GetPopupType(alertType);
			_popup.PopupEntity(message, laptop, popupType);
		}
	}

	private static (string color, int size) GetAlertStyle(SentryAlertType alertType)
	{
		return alertType switch
		{
			SentryAlertType.LowAmmo => (color: "#CED22B", size: 14), 
			SentryAlertType.CriticalHealth => (color: "#A42625", size: 16), 
			SentryAlertType.TargetAcquired => (color: "#A42625", size: 14), 
			SentryAlertType.Damaged => (color: "#A42625", size: 14), 
			_ => (color: "#88C7FA", size: 14), 
		};
	}

	private static PopupType GetPopupType(SentryAlertType alertType)
	{
		return alertType switch
		{
			SentryAlertType.CriticalHealth => PopupType.LargeCaution, 
			SentryAlertType.Damaged => PopupType.MediumCaution, 
			SentryAlertType.TargetAcquired => PopupType.MediumCaution, 
			SentryAlertType.LowAmmo => PopupType.Medium, 
			_ => PopupType.Medium, 
		};
	}

	private void OnLaptopAfterInteract(Entity<SentryLaptopComponent> laptop, ref AfterInteractEvent args)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && args.CanReach && args.Target.HasValue && ((EntitySystem)this).HasComp<PlaceableSurfaceComponent>(args.Target.Value))
		{
			((HandledEntityEventArgs)args).Handled = true;
			if (_hands.TryDrop(Entity<HandsComponent>.op_Implicit(args.User), laptop.Owner, null, checkActionBlocker: false) && _net.IsServer)
			{
				PlaceLaptopOnSurface(laptop, args.Target.Value, args.User);
			}
		}
	}

	private void OnLaptopUIOpenAttempt(Entity<SentryLaptopComponent> laptop, ref ActivatableUIOpenAttemptEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		EntityUid parent = ((EntitySystem)this).Transform(Entity<SentryLaptopComponent>.op_Implicit(laptop)).ParentUid;
		if (!((EntitySystem)this).HasComp<PlaceableSurfaceComponent>(parent))
		{
			_popup.PopupClient("Place the laptop on a table first!", Entity<SentryLaptopComponent>.op_Implicit(laptop), args.User);
			((CancellableEntityEventArgs)args).Cancel();
		}
		else if (!laptop.Comp.IsOpen)
		{
			laptop.Comp.IsOpen = true;
			SetPowered(laptop, powered: true);
			UpdateLaptopVisuals(laptop);
			((EntitySystem)this).Dirty<SentryLaptopComponent>(laptop, (MetaDataComponent)null);
		}
	}

	private void OnLaptopShutdown(Entity<SentryLaptopComponent> laptop, ref ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UnlinkAllSentries(laptop);
	}

	private void OnSentryLinkedShutdown(Entity<SentryLaptopLinkedComponent> sentry, ref ComponentShutdown args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		SentryLaptopComponent laptop = default(SentryLaptopComponent);
		if (sentry.Comp.LinkedLaptop.HasValue && ((EntitySystem)this).TryComp<SentryLaptopComponent>(sentry.Comp.LinkedLaptop.Value, ref laptop))
		{
			UnlinkSentry(Entity<SentryLaptopComponent>.op_Implicit((sentry.Comp.LinkedLaptop.Value, laptop)), sentry.Owner);
		}
	}

	private void OnUnlinkMessage(Entity<SentryLaptopComponent> laptop, ref SentryLaptopUnlinkBuiMsg args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? sentryEnt = default(EntityUid?);
		if (_net.IsServer && ((EntitySystem)this).TryGetEntity(args.Sentry, ref sentryEnt))
		{
			UnlinkSentry(laptop, sentryEnt.Value);
			UpdateUI(laptop);
		}
	}

	private void OnUnlinkAllMessage(Entity<SentryLaptopComponent> laptop, ref SentryLaptopUnlinkAllBuiMsg args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsServer)
		{
			UnlinkAllSentries(laptop);
			UpdateUI(laptop);
		}
	}

	private void OnSetFactionsMessage(Entity<SentryLaptopComponent> laptop, ref SentryLaptopSetFactionsBuiMsg args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? sentryEnt = default(EntityUid?);
		SentryTargetingComponent targeting = default(SentryTargetingComponent);
		if (_net.IsServer && ((EntitySystem)this).TryGetEntity(args.Sentry, ref sentryEnt) && GetLinkedSentries(laptop).Contains(sentryEnt.Value) && ((EntitySystem)this).TryComp<SentryTargetingComponent>(sentryEnt.Value, ref targeting))
		{
			HashSet<string> factionSet = new HashSet<string>(args.Factions);
			_sentryTargeting.SetFriendlyFactions(Entity<SentryTargetingComponent>.op_Implicit((sentryEnt.Value, targeting)), factionSet);
			UpdateUI(laptop);
		}
	}

	private void OnToggleFactionMessage(Entity<SentryLaptopComponent> laptop, ref SentryLaptopToggleFactionBuiMsg args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? sentryEnt = default(EntityUid?);
		SentryTargetingComponent targeting = default(SentryTargetingComponent);
		if (_net.IsServer && ((EntitySystem)this).TryGetEntity(args.Sentry, ref sentryEnt) && GetLinkedSentries(laptop).Contains(sentryEnt.Value) && ((EntitySystem)this).TryComp<SentryTargetingComponent>(sentryEnt.Value, ref targeting))
		{
			_sentryTargeting.ToggleFaction(Entity<SentryTargetingComponent>.op_Implicit((sentryEnt.Value, targeting)), args.Faction, args.Targeted);
			UpdateUI(laptop);
		}
	}

	private void OnTogglePowerMessage(Entity<SentryLaptopComponent> laptop, ref SentryLaptopTogglePowerBuiMsg args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? sentryEnt = default(EntityUid?);
		SentryComponent sentry = default(SentryComponent);
		if (_net.IsServer && ((EntitySystem)this).TryGetEntity(args.Sentry, ref sentryEnt) && GetLinkedSentries(laptop).Contains(sentryEnt.Value) && ((EntitySystem)this).TryComp<SentryComponent>(sentryEnt.Value, ref sentry) && sentry.Mode != SentryMode.Item)
		{
			SentryMode newMode = ((sentry.Mode == SentryMode.On) ? SentryMode.Off : SentryMode.On);
			base.EntityManager.System<SentrySystem>().TrySetMode(Entity<SentryComponent>.op_Implicit((sentryEnt.Value, sentry)), newMode);
			UpdateUI(laptop);
		}
	}

	private void OnResetTargetingMessage(Entity<SentryLaptopComponent> laptop, ref SentryLaptopResetTargetingBuiMsg args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? sentryEnt = default(EntityUid?);
		SentryTargetingComponent targeting = default(SentryTargetingComponent);
		if (_net.IsServer && ((EntitySystem)this).TryGetEntity(args.Sentry, ref sentryEnt) && GetLinkedSentries(laptop).Contains(sentryEnt.Value) && ((EntitySystem)this).TryComp<SentryTargetingComponent>(sentryEnt.Value, ref targeting))
		{
			_sentryTargeting.ResetToDefault(Entity<SentryTargetingComponent>.op_Implicit((sentryEnt.Value, targeting)));
			UpdateUI(laptop);
		}
	}

	private void PlaceLaptopOnSurface(Entity<SentryLaptopComponent> laptop, EntityUid surface, EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent surfaceXform = ((EntitySystem)this).Transform(surface);
		_transform.SetCoordinates(laptop.Owner, surfaceXform.Coordinates);
		_transform.SetParent(laptop.Owner, surface);
		laptop.Comp.IsOpen = true;
		SetPowered(laptop, powered: true);
		UpdateLaptopVisuals(laptop);
		((EntitySystem)this).Dirty<SentryLaptopComponent>(laptop, (MetaDataComponent)null);
	}

	private void OnLaptopParentChanged(Entity<SentryLaptopComponent> laptop, ref EntParentChangedMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		EntityUid parent = ((EntitySystem)this).Transform(Entity<SentryLaptopComponent>.op_Implicit(laptop)).ParentUid;
		bool onSurface = ((EntitySystem)this).HasComp<PlaceableSurfaceComponent>(parent);
		laptop.Comp.IsOpen = onSurface;
		SetPowered(laptop, onSurface);
		UpdateLaptopVisuals(laptop);
		((EntitySystem)this).Dirty<SentryLaptopComponent>(laptop, (MetaDataComponent)null);
		if (_net.IsServer && !onSurface)
		{
			_ui.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(laptop.Owner), (Enum)SentryLaptopUiKey.Key);
		}
	}

	private bool IsSentryAlreadyLinked(Entity<SentryComponent> sentry)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		SentryLaptopLinkedComponent linked = default(SentryLaptopLinkedComponent);
		if (((EntitySystem)this).TryComp<SentryLaptopLinkedComponent>(Entity<SentryComponent>.op_Implicit(sentry), ref linked))
		{
			return linked.LinkedLaptop.HasValue;
		}
		return false;
	}

	private bool ValidateLaptopForLinking(Entity<SentryLaptopComponent> laptop, Entity<SentryComponent> sentry, EntityUid user)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		if (!laptop.Comp.IsOpen)
		{
			_popup.PopupClient("The laptop must be opened first!", Entity<SentryLaptopComponent>.op_Implicit(laptop), user);
			return false;
		}
		if (GetLinkedSentries(laptop).Count >= laptop.Comp.MaxLinkedSentries)
		{
			_popup.PopupClient($"The laptop can only control {laptop.Comp.MaxLinkedSentries} sentries at once!", Entity<SentryLaptopComponent>.op_Implicit(laptop), user);
			return false;
		}
		return true;
	}

	private void LinkSentryToLaptop(Entity<SentryLaptopComponent> laptop, Entity<SentryComponent> sentry, EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		DeviceLinkSourceComponent source = ((EntitySystem)this).EnsureComp<DeviceLinkSourceComponent>(laptop.Owner);
		DeviceLinkSinkComponent sink = ((EntitySystem)this).EnsureComp<DeviceLinkSinkComponent>(sentry.Owner);
		_deviceLink.LinkDefaults(user, laptop.Owner, sentry.Owner, source, sink);
		laptop.Comp.LinkedSentries.Add(Entity<SentryComponent>.op_Implicit(sentry));
		SentryLaptopLinkedComponent linkedComp = ((EntitySystem)this).EnsureComp<SentryLaptopLinkedComponent>(Entity<SentryComponent>.op_Implicit(sentry));
		linkedComp.LinkedLaptop = Entity<SentryLaptopComponent>.op_Implicit(laptop);
		((EntitySystem)this).Dirty(Entity<SentryComponent>.op_Implicit(sentry), (IComponent)(object)linkedComp, (MetaDataComponent)null);
		InitializeSentryTargeting(sentry.Owner);
		_popup.PopupEntity("Successfully linked " + ((EntitySystem)this).Name(Entity<SentryComponent>.op_Implicit(sentry), (MetaDataComponent)null) + " to the laptop.", Entity<SentryComponent>.op_Implicit(sentry), user);
		if (laptop.Comp.LinkedSentries.Count == 1)
		{
			SetPowered(laptop, powered: true);
		}
		((EntitySystem)this).Dirty<SentryLaptopComponent>(laptop, (MetaDataComponent)null);
		UpdateUI(laptop);
	}

	private void InitializeSentryTargeting(EntityUid sentry)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		SentryTargetingComponent targeting = default(SentryTargetingComponent);
		if (!((EntitySystem)this).TryComp<SentryTargetingComponent>(sentry, ref targeting))
		{
			targeting = ((EntitySystem)this).EnsureComp<SentryTargetingComponent>(sentry);
		}
		if (!((EntitySystem)this).HasComp<GunIFFComponent>(sentry) && ((EntitySystem)this).HasComp<GunComponent>(sentry))
		{
			_iff.EnableIntrinsicIFF(sentry);
		}
		HashSet<string> defaultFactions = ((targeting.FriendlyFactions.Count > 0) ? new HashSet<string>(targeting.FriendlyFactions) : new HashSet<string>());
		if (defaultFactions.Count == 0)
		{
			if (!string.IsNullOrEmpty(targeting.OriginalFaction))
			{
				defaultFactions.Add(targeting.OriginalFaction);
			}
			else
			{
				defaultFactions.Add("UNMC");
			}
		}
		foreach (string faction in _sentryTargeting.GetHumanoidFactions())
		{
			defaultFactions.Add(faction);
		}
		_sentryTargeting.SetFriendlyFactions(Entity<SentryTargetingComponent>.op_Implicit((sentry, targeting)), defaultFactions);
	}

	public void UnlinkSentry(Entity<SentryLaptopComponent> laptop, EntityUid sentry)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		if (laptop.Comp.LinkedSentries.Remove(sentry))
		{
			DeviceLinkSinkComponent sink = default(DeviceLinkSinkComponent);
			if (((EntitySystem)this).TryComp<DeviceLinkSinkComponent>(sentry, ref sink))
			{
				_deviceLink.RemoveAllFromSink(sentry, sink);
			}
			laptop.Comp.SentryCustomNames.Remove(sentry);
			((EntitySystem)this).RemComp<SentryLaptopLinkedComponent>(sentry);
			SentryTargetingComponent targeting = default(SentryTargetingComponent);
			if (((EntitySystem)this).TryComp<SentryTargetingComponent>(sentry, ref targeting))
			{
				_sentryTargeting.ResetToDefault(Entity<SentryTargetingComponent>.op_Implicit((sentry, targeting)));
			}
			if (laptop.Comp.LinkedSentries.Count == 0)
			{
				SetPowered(laptop, powered: false);
			}
			((EntitySystem)this).Dirty<SentryLaptopComponent>(laptop, (MetaDataComponent)null);
		}
	}

	private void UnlinkAllSentries(Entity<SentryLaptopComponent> laptop)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		foreach (EntityUid sentry in GetLinkedSentries(laptop).ToList())
		{
			UnlinkSentry(laptop, sentry);
		}
	}

	public void SetPowered(Entity<SentryLaptopComponent> laptop, bool powered)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		laptop.Comp.IsPowered = powered;
		UpdateLaptopVisuals(laptop);
		((EntitySystem)this).Dirty<SentryLaptopComponent>(laptop, (MetaDataComponent)null);
	}

	private void UpdateLaptopVisuals(Entity<SentryLaptopComponent> laptop)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		SentryLaptopState state = SentryLaptopState.Closed;
		if (laptop.Comp.IsOpen)
		{
			state = ((!laptop.Comp.IsPowered) ? SentryLaptopState.Open : SentryLaptopState.Active);
		}
		_appearance.SetData(Entity<SentryLaptopComponent>.op_Implicit(laptop), (Enum)SentryLaptopVisuals.State, (object)state, (AppearanceComponent)null);
	}

	protected virtual void UpdateUI(Entity<SentryLaptopComponent> laptop)
	{
	}

	protected List<SentryInfo> BuildSentryInfoList(Entity<SentryLaptopComponent> laptop)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		List<SentryInfo> sentries = new List<SentryInfo>();
		SentryComponent sentry = default(SentryComponent);
		foreach (EntityUid sentryUid in GetLinkedSentries(laptop))
		{
			if (((EntitySystem)this).TryComp<SentryComponent>(sentryUid, ref sentry))
			{
				SentryInfo info = BuildSentryInfo(laptop, sentryUid, sentry);
				sentries.Add(info);
			}
		}
		return sentries;
	}

	protected SentryInfo BuildSentryInfo(Entity<SentryLaptopComponent> laptop, EntityUid sentryUid, SentryComponent sentry)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		string displayName = GetSentryDisplayName(laptop, sentryUid);
		string cName;
		string customName = (laptop.Comp.SentryCustomNames.TryGetValue(sentryUid, out cName) ? cName : null);
		float visionRadius = GetSentryVisionRadius(sentryUid);
		float maxDeviation = (float)((Angle)(ref sentry.MaxDeviation)).Degrees;
		float maxHealth;
		int maxAmmo;
		return new SentryInfo(((EntitySystem)this).GetNetEntity(sentryUid, (MetaDataComponent)null), displayName, sentry.Mode, GetSentryHealth(sentryUid, out maxHealth), maxHealth, GetSentryAmmo(sentryUid, out maxAmmo), maxAmmo, GetSentryLocation(sentryUid), GetSentryTarget(sentryUid), GetSentryFriendlyFactions(sentryUid), customName, visionRadius, maxDeviation, GetSentryHumanoidAdded(sentryUid));
	}

	protected virtual float GetSentryVisionRadius(EntityUid sentry)
	{
		return 5f;
	}

	private string GetSentryDisplayName(Entity<SentryLaptopComponent> laptop, EntityUid sentry)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (laptop.Comp.SentryCustomNames.TryGetValue(sentry, out string customName))
		{
			return customName;
		}
		return ((EntitySystem)this).Name(sentry, (MetaDataComponent)null);
	}

	private float GetSentryHealth(EntityUid sentry, out float maxHealth)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		maxHealth = GetSentryMaxHealth(sentry);
		float health = maxHealth;
		DamageableComponent damageable = default(DamageableComponent);
		if (((EntitySystem)this).TryComp<DamageableComponent>(sentry, ref damageable))
		{
			float damage = damageable.TotalDamage.Float();
			health = Math.Max(0f, maxHealth - damage);
		}
		return health;
	}

	private float GetSentryHealth(Entity<SentryComponent> sentry, out float maxHealth)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return GetSentryHealth(sentry.Owner, out maxHealth);
	}

	private int GetSentryAmmo(EntityUid sentry, out int maxAmmo)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		maxAmmo = 0;
		if (!base.EntityManager.System<SentrySystem>().TryGetSentryAmmo(sentry, out var ammoCount, out var ammoCapacity))
		{
			return 0;
		}
		maxAmmo = ammoCapacity.Value;
		return ammoCount.Value;
	}

	private string GetSentryLocation(EntityUid sentry)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (_area.TryGetArea(sentry, out Entity<AreaComponent>? area, out EntityPrototype _))
		{
			return ((EntitySystem)this).Name(Entity<AreaComponent>.op_Implicit(area.Value), (MetaDataComponent)null);
		}
		return "Unknown";
	}

	protected List<EntityUid> GetLinkedSentries(Entity<SentryLaptopComponent> laptop)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		List<EntityUid> linked = new List<EntityUid>();
		DeviceLinkSourceComponent source = default(DeviceLinkSourceComponent);
		if (((EntitySystem)this).TryComp<DeviceLinkSourceComponent>(Entity<SentryLaptopComponent>.op_Implicit(laptop), ref source))
		{
			foreach (EntityUid sink in source.LinkedPorts.Keys)
			{
				if (((EntitySystem)this).HasComp<SentryComponent>(sink))
				{
					linked.Add(sink);
				}
			}
			laptop.Comp.LinkedSentries = linked.ToHashSet();
			return linked;
		}
		linked.AddRange(laptop.Comp.LinkedSentries);
		return linked;
	}

	private NetEntity? GetSentryTarget(EntityUid sentry)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		GunComponent gun = default(GunComponent);
		if (((EntitySystem)this).TryComp<GunComponent>(sentry, ref gun) && gun.Target.HasValue)
		{
			return ((EntitySystem)this).GetNetEntity(gun.Target.Value, (MetaDataComponent)null);
		}
		return null;
	}

	private HashSet<string> GetSentryFriendlyFactions(EntityUid sentry)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		SentryTargetingComponent targeting = default(SentryTargetingComponent);
		if (((EntitySystem)this).TryComp<SentryTargetingComponent>(sentry, ref targeting))
		{
			return targeting.FriendlyFactions;
		}
		return new HashSet<string>();
	}

	private HashSet<string> GetSentryHumanoidAdded(EntityUid sentry)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		SentryTargetingComponent targeting = default(SentryTargetingComponent);
		if (((EntitySystem)this).TryComp<SentryTargetingComponent>(sentry, ref targeting))
		{
			return targeting.HumanoidAdded;
		}
		return new HashSet<string>();
	}

	protected virtual float GetSentryMaxHealth(EntityUid sentry)
	{
		return 100f;
	}

	private bool TryGetLinkedLaptop(EntityUid sentry, out Entity<SentryLaptopComponent>? laptop)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		laptop = null;
		DeviceLinkSinkComponent sink = default(DeviceLinkSinkComponent);
		if (!((EntitySystem)this).TryComp<DeviceLinkSinkComponent>(sentry, ref sink))
		{
			return false;
		}
		SentryLaptopComponent comp = default(SentryLaptopComponent);
		foreach (EntityUid source in sink.LinkedSources)
		{
			if (((EntitySystem)this).TryComp<SentryLaptopComponent>(source, ref comp))
			{
				laptop = new Entity<SentryLaptopComponent>(source, comp);
				return true;
			}
		}
		return false;
	}
}
