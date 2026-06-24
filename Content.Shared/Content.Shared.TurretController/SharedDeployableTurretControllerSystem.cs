using System;
using System.Collections.Generic;
using Content.Shared.Access;
using Content.Shared.Access.Systems;
using Content.Shared.Popups;
using Content.Shared.Turrets;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.TurretController;

public abstract class SharedDeployableTurretControllerSystem : EntitySystem
{
	[Dependency]
	private AccessReaderSystem _accessreader;

	[Dependency]
	private TurretTargetSettingsSystem _turretTargetingSettings;

	[Dependency]
	private SharedUserInterfaceSystem _userInterfaceSystem;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<DeployableTurretControllerComponent, DeployableTurretArmamentSettingChangedMessage>((EntityEventRefHandler<DeployableTurretControllerComponent, DeployableTurretArmamentSettingChangedMessage>)OnArmamentSettingChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DeployableTurretControllerComponent, DeployableTurretExemptAccessLevelChangedMessage>((EntityEventRefHandler<DeployableTurretControllerComponent, DeployableTurretExemptAccessLevelChangedMessage>)OnExemptAccessLevelsChanged, (Type[])null, (Type[])null);
	}

	private void OnArmamentSettingChanged(Entity<DeployableTurretControllerComponent> ent, ref DeployableTurretArmamentSettingChangedMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (IsUserAllowedAccess(ent, ((BaseBoundUserInterfaceEvent)args).Actor))
		{
			ChangeArmamentSetting(ent, args.ArmamentState, ((BaseBoundUserInterfaceEvent)args).Actor);
		}
		BoundUserInterface bui = default(BoundUserInterface);
		if (_userInterfaceSystem.TryGetOpenUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)DeployableTurretControllerUiKey.Key, ref bui))
		{
			bui.Update<DeployableTurretControllerBoundInterfaceState>();
		}
	}

	private void OnExemptAccessLevelsChanged(Entity<DeployableTurretControllerComponent> ent, ref DeployableTurretExemptAccessLevelChangedMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (IsUserAllowedAccess(ent, ((BaseBoundUserInterfaceEvent)args).Actor))
		{
			ChangeExemptAccessLevels(ent, args.AccessLevels, args.Enabled, ((BaseBoundUserInterfaceEvent)args).Actor);
		}
		BoundUserInterface bui = default(BoundUserInterface);
		if (_userInterfaceSystem.TryGetOpenUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)DeployableTurretControllerUiKey.Key, ref bui))
		{
			bui.Update<DeployableTurretControllerBoundInterfaceState>();
		}
	}

	protected virtual void ChangeArmamentSetting(Entity<DeployableTurretControllerComponent> ent, int armamentState, EntityUid? user = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.ArmamentState = armamentState;
		((EntitySystem)this).Dirty<DeployableTurretControllerComponent>(ent, (MetaDataComponent)null);
		_appearance.SetData(Entity<DeployableTurretControllerComponent>.op_Implicit(ent), (Enum)TurretControllerVisuals.ControlPanel, (object)armamentState, (AppearanceComponent)null);
	}

	protected virtual void ChangeExemptAccessLevels(Entity<DeployableTurretControllerComponent> ent, HashSet<ProtoId<AccessLevelPrototype>> exemptions, bool enabled, EntityUid? user = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		TurretTargetSettingsComponent targetSettings = default(TurretTargetSettingsComponent);
		if (!((EntitySystem)this).TryComp<TurretTargetSettingsComponent>(Entity<DeployableTurretControllerComponent>.op_Implicit(ent), ref targetSettings))
		{
			return;
		}
		Entity<TurretTargetSettingsComponent> controller = default(Entity<TurretTargetSettingsComponent>);
		controller._002Ector(Entity<DeployableTurretControllerComponent>.op_Implicit(ent), targetSettings);
		foreach (ProtoId<AccessLevelPrototype> accessLevel in exemptions)
		{
			if (ent.Comp.AccessLevels.Contains(accessLevel))
			{
				_turretTargetingSettings.SetAccessLevelExemption(controller, accessLevel, enabled);
			}
		}
		((EntitySystem)this).Dirty<TurretTargetSettingsComponent>(controller, (MetaDataComponent)null);
	}

	public bool IsUserAllowedAccess(Entity<DeployableTurretControllerComponent> ent, EntityUid user)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		if (_accessreader.IsAllowed(user, Entity<DeployableTurretControllerComponent>.op_Implicit(ent)))
		{
			return true;
		}
		_popup.PopupClient(base.Loc.GetString("turret-controls-access-denied"), Entity<DeployableTurretControllerComponent>.op_Implicit(ent), user);
		_audio.PlayPredicted(ent.Comp.AccessDeniedSound, Entity<DeployableTurretControllerComponent>.op_Implicit(ent), (EntityUid?)user, (AudioParams?)null);
		return false;
	}
}
