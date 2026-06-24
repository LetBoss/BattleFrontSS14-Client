using System;
using System.Collections.Generic;
using Content.Shared._RMC14.AlertLevel;
using Content.Shared._RMC14.Dialog;
using Content.Shared.Access;
using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.Coordinates;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Keycard;

public sealed class KeycardDeviceSystem : EntitySystem
{
	[Dependency]
	private AccessReaderSystem _accessReader;

	[Dependency]
	private RMCAlertLevelSystem _alertLevel;

	[Dependency]
	private DialogSystem _dialog;

	[Dependency]
	private EntityLookupSystem _entityLookup;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private IGameTiming _timing;

	private readonly HashSet<Entity<KeycardDeviceComponent>> _devices = new HashSet<Entity<KeycardDeviceComponent>>();

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<KeycardDeviceComponent, InteractHandEvent>((EntityEventRefHandler<KeycardDeviceComponent, InteractHandEvent>)OnInteractHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<KeycardDeviceComponent, KeycardDeviceSetModeEvent>((EntityEventRefHandler<KeycardDeviceComponent, KeycardDeviceSetModeEvent>)OnSetMode, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<KeycardDeviceComponent, InteractUsingEvent>((EntityEventRefHandler<KeycardDeviceComponent, InteractUsingEvent>)OnInteractUsing, (Type[])null, (Type[])null);
	}

	private void OnInteractHand(Entity<KeycardDeviceComponent> ent, ref InteractHandEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (!_accessReader.IsAllowed(args.User, Entity<KeycardDeviceComponent>.op_Implicit(ent)))
		{
			_popup.PopupClient(base.Loc.GetString("rmc-access-denied"), Entity<KeycardDeviceComponent>.op_Implicit(ent), args.User, PopupType.SmallCaution);
			return;
		}
		List<DialogOption> options = new List<DialogOption>
		{
			new DialogOption(base.Loc.GetString("rmc-alert-red-alert"), new KeycardDeviceSetModeEvent(KeycardDeviceMode.RedAlert))
		};
		_dialog.OpenOptions(Entity<KeycardDeviceComponent>.op_Implicit(ent), args.User, base.Loc.GetString("rmc-keycard-device"), options, base.Loc.GetString("rmc-keycard-device-description"));
	}

	private void OnSetMode(Entity<KeycardDeviceComponent> ent, ref KeycardDeviceSetModeEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.Mode = args.Mode;
		((EntitySystem)this).Dirty<KeycardDeviceComponent>(ent, (MetaDataComponent)null);
	}

	private void OnInteractUsing(Entity<KeycardDeviceComponent> ent, ref InteractUsingEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		AccessReaderComponent accessReader = default(AccessReaderComponent);
		if (((EntitySystem)this).TryComp<AccessReaderComponent>(Entity<KeycardDeviceComponent>.op_Implicit(ent), ref accessReader))
		{
			ICollection<ProtoId<AccessLevelPrototype>> access = _accessReader.FindAccessTags(args.Used);
			if (!_accessReader.AreAccessTagsAllowed(access, accessReader))
			{
				_popup.PopupClient(base.Loc.GetString("rmc-access-denied"), Entity<KeycardDeviceComponent>.op_Implicit(ent), args.User, PopupType.SmallCaution);
				return;
			}
		}
		TimeSpan time = _timing.CurTime;
		ent.Comp.LastActivated = time;
		((EntitySystem)this).Dirty<KeycardDeviceComponent>(ent, (MetaDataComponent)null);
		if (AllEnabled(ent))
		{
			switch (ent.Comp.Mode)
			{
			case KeycardDeviceMode.None:
				return;
			case KeycardDeviceMode.RedAlert:
				_alertLevel.Set(RMCAlertLevels.Red, args.User);
				return;
			}
			((EntitySystem)this).Log.Warning($"Unknown {"KeycardDeviceMode"}: {ent.Comp.Mode}");
		}
	}

	private bool AllEnabled(Entity<KeycardDeviceComponent> ent)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		_devices.Clear();
		_entityLookup.GetEntitiesInRange<KeycardDeviceComponent>(ent.Owner.ToCoordinates(), ent.Comp.Range, _devices, (LookupFlags)110);
		TimeSpan time = _timing.CurTime;
		foreach (Entity<KeycardDeviceComponent> device in _devices)
		{
			if (ent.Comp.Mode != device.Comp.Mode)
			{
				return false;
			}
			if (device.Comp.LastActivated < time - device.Comp.Time)
			{
				return false;
			}
		}
		return true;
	}
}
