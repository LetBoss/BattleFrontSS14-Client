using System;
using System.Collections.Generic;
using System.Text;
using Content.Client._RMC14.TacticalMap;
using Content.Client._RMC14.UserInterface;
using Content.Client.Eye;
using Content.Client.UserInterface.ControlExtensions;
using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.Dropship;
using Content.Shared._RMC14.Dropship.AttachmentPoint;
using Content.Shared._RMC14.Dropship.ElectronicSystem;
using Content.Shared._RMC14.Dropship.Utility.Components;
using Content.Shared._RMC14.Dropship.Utility.Systems;
using Content.Shared._RMC14.Dropship.Weapon;
using Content.Shared._RMC14.TacticalMap;
using Content.Shared.FixedPoint;
using Content.Shared.ParaDrop;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.Client._RMC14.Dropship.Weapon;

public sealed class DropshipWeaponsBui : RMCPopOutBui<DropshipWeaponsWindow>
{
	private readonly ContainerSystem _container;

	private readonly SharedRMCEquipmentDeployerSystem _equipmentDeployer;

	private readonly EyeLerpingSystem _eyeLerping;

	private readonly DropshipSystem _system;

	private readonly DropshipWeaponSystem _weaponSystem;

	private readonly TacticalMapSystem _tacticalMapSystem;

	private EntityUid? _oldEye;

	private TacticalMapWrapper? _embeddedTacMapWrapperScreen1;

	private TacticalMapWrapper? _embeddedTacMapWrapperScreen2;

	protected override DropshipWeaponsWindow? Window { get; set; }

	public DropshipWeaponsBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		_container = ((BoundUserInterface)this).EntMan.System<ContainerSystem>();
		_eyeLerping = ((BoundUserInterface)this).EntMan.System<EyeLerpingSystem>();
		_system = ((BoundUserInterface)this).EntMan.System<DropshipSystem>();
		_weaponSystem = ((BoundUserInterface)this).EntMan.System<DropshipWeaponSystem>();
		_tacticalMapSystem = ((BoundUserInterface)this).EntMan.System<TacticalMapSystem>();
		_equipmentDeployer = ((BoundUserInterface)this).EntMan.System<SharedRMCEquipmentDeployerSystem>();
	}

	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		Window = ((BoundUserInterface)(object)this).CreatePopOutableWindow<DropshipWeaponsWindow>();
		Window.OffsetUpButton.Text = "^";
		((BaseButton)Window.OffsetUpButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new DropshipTerminalWeaponsAdjustOffsetMsg((Direction)4));
		};
		Window.OffsetLeftButton.Text = "<";
		((BaseButton)Window.OffsetLeftButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new DropshipTerminalWeaponsAdjustOffsetMsg((Direction)6));
		};
		Window.OffsetRightButton.Text = ">";
		((BaseButton)Window.OffsetRightButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new DropshipTerminalWeaponsAdjustOffsetMsg((Direction)2));
		};
		Window.OffsetDownButton.Text = "v";
		((BaseButton)Window.OffsetDownButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new DropshipTerminalWeaponsAdjustOffsetMsg((Direction)0));
		};
		((BaseButton)Window.ResetOffsetButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new DropshipTerminalWeaponsResetOffsetMsg());
		};
		Window.ScreenOne.TopRow.Refresh();
		Window.ScreenOne.LeftRow.Refresh();
		Window.ScreenOne.RightRow.Refresh();
		Window.ScreenOne.BottomRow.Refresh();
		Window.ScreenTwo.TopRow.Refresh();
		Window.ScreenTwo.LeftRow.Refresh();
		Window.ScreenTwo.RightRow.Refresh();
		Window.ScreenTwo.BottomRow.Refresh();
		Refresh();
	}

	public void Refresh()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (Window != null)
		{
			DropshipTerminalWeaponsComponent dropshipTerminalWeaponsComponent = default(DropshipTerminalWeaponsComponent);
			if (((BoundUserInterface)this).EntMan.TryGetComponent<DropshipTerminalWeaponsComponent>(((BoundUserInterface)this).Owner, ref dropshipTerminalWeaponsComponent))
			{
				SetScreen(first: true, dropshipTerminalWeaponsComponent.ScreenOne);
				SetScreen(first: false, dropshipTerminalWeaponsComponent.ScreenTwo);
			}
			RefreshButtons();
			if (_embeddedTacMapWrapperScreen1 != null && dropshipTerminalWeaponsComponent != null && dropshipTerminalWeaponsComponent.ScreenOne.State == DropshipTerminalWeaponsScreen.TacMap)
			{
				RefreshEmbeddedTacMap(_embeddedTacMapWrapperScreen1);
			}
			if (_embeddedTacMapWrapperScreen2 != null && dropshipTerminalWeaponsComponent != null && dropshipTerminalWeaponsComponent.ScreenTwo.State == DropshipTerminalWeaponsScreen.TacMap)
			{
				RefreshEmbeddedTacMap(_embeddedTacMapWrapperScreen2);
			}
		}
	}

	private void RefreshButtons()
	{
		if (Window == null)
		{
			return;
		}
		foreach (DropshipWeaponsButton item in ((Control)(object)Window.Panel).GetControlOfType<DropshipWeaponsButton>())
		{
			item.Refresh();
		}
	}

	private void SetScreen(bool first, DropshipTerminalWeaponsComponent.Screen compScreen)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_044d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0708: Unknown result type (might be due to invalid IL or missing references)
		//IL_0737: Unknown result type (might be due to invalid IL or missing references)
		//IL_073c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dc5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1139: Unknown result type (might be due to invalid IL or missing references)
		//IL_09cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_09db: Unknown result type (might be due to invalid IL or missing references)
		//IL_09eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0807: Unknown result type (might be due to invalid IL or missing references)
		//IL_076c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0787: Unknown result type (might be due to invalid IL or missing references)
		//IL_079f: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ddc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dde: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a04: Unknown result type (might be due to invalid IL or missing references)
		//IL_0975: Unknown result type (might be due to invalid IL or missing references)
		//IL_097c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a22: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a13: Unknown result type (might be due to invalid IL or missing references)
		//IL_0994: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e25: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e2a: Unknown result type (might be due to invalid IL or missing references)
		//IL_11e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e3c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e40: Unknown result type (might be due to invalid IL or missing references)
		//IL_1244: Unknown result type (might be due to invalid IL or missing references)
		DropshipTerminalWeaponsComponent terminal = default(DropshipTerminalWeaponsComponent);
		if (Window == null || !((BoundUserInterface)this).EntMan.TryGetComponent<DropshipTerminalWeaponsComponent>(((BoundUserInterface)this).Owner, ref terminal))
		{
			return;
		}
		DropshipWeaponsScreen screen = (first ? Window.ScreenOne : Window.ScreenTwo);
		((Control)screen.Viewport).RemoveAllChildren();
		((Control)screen.Viewport).Visible = false;
		DropshipWeaponsButtonData value = Button("equip", DropshipTerminalWeaponsScreen.Equip);
		DropshipWeaponsButtonData value2 = ButtonAction("exit", delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new DropshipTerminalWeaponsExitMsg(first));
		});
		DropshipWeaponsButtonData value3 = Button("target", DropshipTerminalWeaponsScreen.Target);
		DropshipWeaponsButtonData value4 = Button("cams", DropshipTerminalWeaponsScreen.Cams);
		DropshipWeaponsButtonData value5 = Button("maps", DropshipTerminalWeaponsScreen.TacMap);
		DropshipWeaponsButtonData value6 = ButtonAction("fire", delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new DropshipTerminalWeaponsFireMsg(first));
		});
		DropshipWeaponsButtonData value7 = Button("strike", DropshipTerminalWeaponsScreen.Strike);
		DropshipWeaponsButtonData value8 = ButtonAction("night-vision-on", delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new DropshipTerminalWeaponsNightVisionMsg(on: true));
		});
		DropshipWeaponsButtonData value9 = ButtonAction("night-vision-off", delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new DropshipTerminalWeaponsNightVisionMsg(on: false));
		});
		DropshipWeaponsButtonData value10 = ButtonAction("cancel", delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new DropshipTerminalWeaponsCancelMsg(first));
		});
		DropshipWeaponsButtonData value11 = Button("weapon", DropshipTerminalWeaponsScreen.StrikeWeapon);
		DropshipWeaponsButtonData dropshipWeaponsButtonData = ButtonAction("lock", delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new DropShipTerminalWeaponsParaDropTargetSelectMsg(on: true));
		});
		DropshipWeaponsButtonData dropshipWeaponsButtonData2 = ButtonAction("clear", delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new DropShipTerminalWeaponsParaDropTargetSelectMsg(on: false));
		});
		DropshipWeaponsButtonData dropshipWeaponsButtonData3 = ButtonAction("enable", delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new DropShipTerminalWeaponsSpotlightToggleMsg(on: true));
		});
		DropshipWeaponsButtonData dropshipWeaponsButtonData4 = ButtonAction("disable", delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new DropShipTerminalWeaponsSpotlightToggleMsg(on: false));
		});
		DropshipWeaponsButtonData dropshipWeaponsButtonData5 = ButtonAction("deploy", delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new DropShipTerminalWeaponsEquipmentDeployToggleMsg(deploy: true));
		});
		DropshipWeaponsButtonData dropshipWeaponsButtonData6 = ButtonAction("retract", delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new DropShipTerminalWeaponsEquipmentDeployToggleMsg(deploy: false));
		});
		DropshipWeaponsButtonData dropshipWeaponsButtonData7 = ButtonAction("auto-deploy", delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new DropShipTerminalWeaponsEquipmentAutoDeployToggleMsg(autoDeploy: true));
		});
		DropshipWeaponsButtonData dropshipWeaponsButtonData8 = ButtonAction("auto-deploy", delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new DropShipTerminalWeaponsEquipmentAutoDeployToggleMsg(autoDeploy: false));
		});
		screen.ScreenLabel.Text = Loc.GetString("rmc-dropship-weapons-main-screen-text");
		((Control)screen.ScreenLabel).VerticalAlignment = (VAlignment)0;
		((Control)screen.ScreenLabel).Margin = default(Thickness);
		((Control)screen.ScreenLabel).Visible = true;
		ClearNames(screen);
		StringBuilder text4;
		switch (compScreen.State)
		{
		case DropshipTerminalWeaponsScreen.Main:
		{
			DropshipWeaponsButtonRow bottomRow3 = screen.BottomRow;
			DropshipWeaponsButtonData? utilityTwo = value5;
			DropshipWeaponsButtonData? five = value4;
			bottomRow3.SetData(null, utilityTwo, five);
			DropshipWeaponsButtonRow topRow3 = screen.TopRow;
			DropshipWeaponsButtonData? one8 = value;
			five = value3;
			topRow3.SetData(one8, null, null, five);
			break;
		}
		case DropshipTerminalWeaponsScreen.Equip:
		{
			screen.BottomRow.SetData(value2);
			TryGetWeapons(first, out var one6, out var two2, out var three2, out var four2, out var utilityOne, out var utilityTwo2, out var utilityThree2, out var electronicSystemOne2, out var electronicSystemTwo2);
			screen.LeftRow.SetData(one6, two2, utilityOne, utilityTwo2, utilityThree2);
			screen.RightRow.SetData(three2, four2, electronicSystemOne2, electronicSystemTwo2);
			text4 = new StringBuilder();
			AddWeaponEntry(one6);
			AddWeaponEntry(two2);
			AddWeaponEntry(three2);
			AddWeaponEntry(four2);
			screen.ScreenLabel.Text = text4.ToString();
			((Control)screen.ScreenLabel).VerticalAlignment = (VAlignment)1;
			((Control)screen.ScreenLabel).Margin = new Thickness(10f);
			break;
		}
		case DropshipTerminalWeaponsScreen.Target:
		{
			AddTargets(out var previous4, out var next4);
			DropshipWeaponsButtonRow bottomRow4 = screen.BottomRow;
			DropshipWeaponsButtonData? one9 = value2;
			DropshipWeaponsButtonData? five = next4;
			bottomRow4.SetData(one9, null, null, null, five);
			DropshipWeaponsButtonRow topRow4 = screen.TopRow;
			DropshipWeaponsButtonData? one10 = value6;
			five = previous4;
			topRow4.SetData(one10, null, null, null, five);
			screen.LeftRow.SetData(value7);
			screen.ScreenLabel.Text = TargetAcquisition();
			break;
		}
		case DropshipTerminalWeaponsScreen.Strike:
		{
			AddTargets(out var previous, out var next);
			DropshipWeaponsButtonRow bottomRow = screen.BottomRow;
			DropshipWeaponsButtonData? one = value2;
			DropshipWeaponsButtonData? five = next;
			bottomRow.SetData(one, null, null, null, five);
			DropshipWeaponsButtonRow topRow = screen.TopRow;
			DropshipWeaponsButtonData? one2 = value6;
			five = previous;
			topRow.SetData(one2, null, null, null, five);
			screen.LeftRow.SetData(value10, value11);
			screen.ScreenLabel.Text = TargetAcquisition();
			break;
		}
		case DropshipTerminalWeaponsScreen.StrikeWeapon:
		{
			AddTargets(out var previous2, out var next2);
			DropshipWeaponsButtonRow bottomRow2 = screen.BottomRow;
			DropshipWeaponsButtonData? one3 = value2;
			DropshipWeaponsButtonData? five = next2;
			bottomRow2.SetData(one3, null, null, null, five);
			DropshipWeaponsButtonRow topRow2 = screen.TopRow;
			DropshipWeaponsButtonData? one4 = value6;
			five = previous2;
			DropshipWeaponsButtonData? utilityTwo = null;
			topRow2.SetData(one4, null, null, utilityTwo, five);
			TryGetWeapons(first, out var one5, out var two, out var three, out var four, out five, out utilityTwo, out var _, out var _, out var _);
			screen.LeftRow.SetData(value10, one5, two, three, four);
			screen.ScreenLabel.Text = TargetAcquisition();
			break;
		}
		case DropshipTerminalWeaponsScreen.SelectingWeapon:
		{
			((Control)screen.ScreenLabel).VerticalAlignment = (VAlignment)1;
			((Control)screen.ScreenLabel).Margin = new Thickness(0f, 10f);
			EntityUid? val5 = default(EntityUid?);
			if (((BoundUserInterface)this).EntMan.TryGetEntity(compScreen.Weapon, ref val5))
			{
				if (_weaponSystem.TryGetWeaponAmmo(Entity<DropshipWeaponComponent>.op_Implicit(val5.Value), out Entity<DropshipAmmoComponent> ammo))
				{
					screen.ScreenLabel.Text = Loc.GetString("rmc-dropship-weapons-weapon-selected-ammo", new(string, object)[4]
					{
						("weapon", val5.Value),
						("ammo", ammo),
						("rounds", ammo.Comp.Rounds),
						("maxRounds", ammo.Comp.MaxRounds)
					});
				}
				else
				{
					screen.ScreenLabel.Text = Loc.GetString("rmc-dropship-weapons-weapon-selected", new(string, object)[1] { ("weapon", val5.Value) });
				}
			}
			screen.TopRow.SetData(value);
			screen.LeftRow.SetData(value6);
			screen.BottomRow.SetData(value2);
			break;
		}
		case DropshipTerminalWeaponsScreen.Cams:
		{
			screen.LeftRow.SetData(value8, value9);
			((Control)screen.ScreenLabel).Visible = false;
			if (_oldEye.HasValue)
			{
				EntityUid? oldEye = _oldEye;
				EntityUid? target = terminal.Target;
				if (oldEye.HasValue != target.HasValue || (oldEye.HasValue && oldEye.GetValueOrDefault() != target.GetValueOrDefault()))
				{
					_eyeLerping.RemoveEye(_oldEye.Value);
				}
			}
			_oldEye = terminal.Target;
			if (terminal.Target.HasValue && _weaponSystem.TryGetTargetEye(Entity<DropshipTerminalWeaponsComponent>.op_Implicit((((BoundUserInterface)this).Owner, terminal)), Entity<DropshipTargetComponent>.op_Implicit(terminal.Target.Value), out var eye))
			{
				if (!((BoundUserInterface)this).EntMan.HasComponent<LerpingEyeComponent>(eye))
				{
					_eyeLerping.AddEye(eye);
				}
				EyeComponent val6 = default(EyeComponent);
				if (((BoundUserInterface)this).EntMan.TryGetComponent<EyeComponent>(eye, ref val6))
				{
					screen.Viewport.Eye = (IEye?)(object)val6.Eye;
				}
			}
			((Control)screen.Viewport).Visible = true;
			screen.BottomRow.SetData(value2);
			break;
		}
		case DropshipTerminalWeaponsScreen.TacMap:
		{
			((Control)screen.ScreenLabel).Visible = false;
			TacticalMapWrapper tacticalMapWrapper = (first ? _embeddedTacMapWrapperScreen1 : _embeddedTacMapWrapperScreen2);
			if (tacticalMapWrapper == null)
			{
				tacticalMapWrapper = new TacticalMapWrapper();
				SetupEmbeddedTacMap(tacticalMapWrapper);
				if (first)
				{
					_embeddedTacMapWrapperScreen1 = tacticalMapWrapper;
				}
				else
				{
					_embeddedTacMapWrapperScreen2 = tacticalMapWrapper;
				}
			}
			((Control)screen.Viewport).AddChild((Control)(object)tacticalMapWrapper);
			((Control)screen.Viewport).Visible = true;
			screen.BottomRow.SetData(value2);
			RefreshEmbeddedTacMap(tacticalMapWrapper);
			break;
		}
		case DropshipTerminalWeaponsScreen.Medevac:
		{
			AddButtons((NetEntity target2) => (BoundUserInterfaceMessage)(object)new DropshipTerminalWeaponsMedevacSelectMsg(target2), (BoundUserInterfaceMessage)(object)new DropshipTerminalWeaponsMedevacPreviousMsg(), (BoundUserInterfaceMessage)(object)new DropshipTerminalWeaponsMedevacNextMsg(), screen.LeftRow, terminal.Medevacs, terminal.MedevacsPage, out var previous5, out var next5);
			screen.TopRow.SetData(value);
			screen.BottomRow.SetData(value2);
			DropshipWeaponsButtonRow rightRow2 = screen.RightRow;
			DropshipWeaponsButtonData? one11 = previous5;
			DropshipWeaponsButtonData? electronicSystemTwo = next5;
			rightRow2.SetData(one11, null, null, null, electronicSystemTwo);
			screen.ScreenLabel.Text = Loc.GetString("rmc-dropship-medevac-system-screen-text");
			break;
		}
		case DropshipTerminalWeaponsScreen.Fulton:
		{
			AddButtons((NetEntity target2) => (BoundUserInterfaceMessage)(object)new DropshipTerminalWeaponsFultonSelectMsg(target2), (BoundUserInterfaceMessage)(object)new DropshipTerminalWeaponsFultonPreviousMsg(), (BoundUserInterfaceMessage)(object)new DropshipTerminalWeaponsFultonNextMsg(), screen.LeftRow, terminal.Fultons, terminal.FultonsPage, out var previous3, out var next3);
			screen.TopRow.SetData(value);
			screen.BottomRow.SetData(value2);
			DropshipWeaponsButtonRow rightRow = screen.RightRow;
			DropshipWeaponsButtonData? one7 = previous3;
			DropshipWeaponsButtonData? electronicSystemTwo = next3;
			rightRow.SetData(one7, null, null, null, electronicSystemTwo);
			screen.ScreenLabel.Text = Loc.GetString("rmc-dropship-fulton-system-screen-text");
			break;
		}
		case DropshipTerminalWeaponsScreen.Paradrop:
		{
			string text3 = null;
			ActiveParaDropComponent activeParaDropComponent = default(ActiveParaDropComponent);
			NetEntity? val3 = default(NetEntity?);
			if (terminal.Target.HasValue && _system.TryGetGridDropship(((BoundUserInterface)this).Owner, out Entity<DropshipComponent> dropship) && ((BoundUserInterface)this).EntMan.TryGetComponent<ActiveParaDropComponent>(Entity<DropshipComponent>.op_Implicit(dropship), ref activeParaDropComponent) && ((EntitySystem)_weaponSystem).TryGetNetEntity(activeParaDropComponent.DropTarget, ref val3, (MetaDataComponent)null))
			{
				foreach (DropshipTerminalWeaponsComponent.TargetEnt target2 in terminal.Targets)
				{
					NetEntity id = target2.Id;
					NetEntity? val4 = val3;
					if (val4.HasValue && !(id != val4.GetValueOrDefault()))
					{
						text3 = target2.Name;
						break;
					}
				}
			}
			screen.ScreenLabel.Text = Loc.GetString("rmc-dropship-paradrop-target-screen-text", new(string, object)[1] { ("hasTarget", (text3 == null) ? Loc.GetString("rmc-dropship-paradrop-target-screen-target-none") : Loc.GetString("rmc-dropship-paradrop-target-screen-target-targeting", new(string, object)[1] { ("dropTarget", text3) })) });
			screen.BottomRow.SetData(value2);
			screen.LeftRow.SetData((text3 == null) ? dropshipWeaponsButtonData : dropshipWeaponsButtonData2);
			screen.TopRow.SetData(value);
			break;
		}
		case DropshipTerminalWeaponsScreen.Spotlight:
		{
			screen.TopRow.SetData(value);
			screen.BottomRow.SetData(value2);
			DropshipSpotlightComponent dropshipSpotlightComponent = default(DropshipSpotlightComponent);
			if (((BoundUserInterface)this).EntMan.TryGetComponent<DropshipSpotlightComponent>(((BoundUserInterface)this).EntMan.GetEntity(terminal.SelectedSystem), ref dropshipSpotlightComponent))
			{
				screen.LeftRow.SetData((!dropshipSpotlightComponent.Enabled) ? dropshipWeaponsButtonData3 : dropshipWeaponsButtonData4);
			}
			break;
		}
		case DropshipTerminalWeaponsScreen.EquipmentDeployer:
		{
			EntityUid? entity = ((BoundUserInterface)this).EntMan.GetEntity(terminal.SelectedSystem);
			screen.TopRow.SetData(value);
			screen.BottomRow.SetData(value2);
			if (!entity.HasValue || !_equipmentDeployer.TryGetContainer(entity.Value, out BaseContainer container))
			{
				break;
			}
			EntityUid? val = null;
			if (container.Count > 0)
			{
				val = container.ContainedEntities[0];
			}
			RMCEquipmentDeployerComponent rMCEquipmentDeployerComponent = default(RMCEquipmentDeployerComponent);
			MetaDataComponent val2 = default(MetaDataComponent);
			if (!((BoundUserInterface)this).EntMan.TryGetComponent<RMCEquipmentDeployerComponent>(val, ref rMCEquipmentDeployerComponent) || !((BoundUserInterface)this).EntMan.TryGetComponent<MetaDataComponent>(val, ref val2))
			{
				break;
			}
			string text = "";
			string text2 = "";
			EntityUid? entity2 = ((BoundUserInterface)this).EntMan.GetEntity(rMCEquipmentDeployerComponent.DeployEntity);
			if (entity2.HasValue && !((BoundUserInterface)this).EntMan.Deleted(entity2))
			{
				if (_equipmentDeployer.TryGetDeployedAmmo(entity2.Value, out var ammoCount, out var ammoCapacity))
				{
					text = Loc.GetString("rmc-dropship-equipment-deployer-ammo", new(string, object)[2]
					{
						("ammoCount", ammoCount),
						("totalAmmoCount", ammoCapacity)
					}) + "\n";
				}
				FixedPoint2 damage;
				bool flag = _equipmentDeployer.TryGetDeployedDamage(entity2.Value, out damage);
				text2 = Loc.GetString("rmc-dropship-equipment-deployer-health", new(string, object)[1] { ("status", flag ? Loc.GetString("rmc-dropship-equipment-damaged") : Loc.GetString("rmc-dropship-equipment-operational")) }) + "\n";
			}
			else
			{
				text2 = Loc.GetString("rmc-dropship-equipment-deployer-health", new(string, object)[1] { ("status", Loc.GetString("rmc-dropship-equipment-destroyed")) }) + "\n";
			}
			screen.ScreenLabel.Text = Loc.GetString("rmc-dropship-equipment-deployer-text", new(string, object)[1] { ("deployName", val2.EntityName) }) + "\n" + text2 + text + Loc.GetString("rmc-dropship-equipment-deployer-status", new(string, object)[1] { ("deployed", rMCEquipmentDeployerComponent.IsDeployed ? Loc.GetString("rmc-dropship-equipment-deployed") : Loc.GetString("rmc-dropship-equipment-undeployed")) }) + "\n" + Loc.GetString("rmc-dropship-equipment-deployer-auto-deploy", new(string, object)[1] { ("autoDeploy", rMCEquipmentDeployerComponent.AutoDeploy ? Loc.GetString("rmc-dropship-equipment-enabled") : Loc.GetString("rmc-dropship-equipment-disabled")) });
			screen.LeftRow.SetData((!rMCEquipmentDeployerComponent.IsDeployed) ? dropshipWeaponsButtonData5 : dropshipWeaponsButtonData6, (!rMCEquipmentDeployerComponent.AutoDeploy) ? dropshipWeaponsButtonData7 : dropshipWeaponsButtonData8);
			break;
		}
		default:
			screen.BottomRow.SetData(value2);
			break;
		}
		RefreshButtons();
		void AddButtons(Func<NetEntity, BoundUserInterfaceMessage> selectMsg, BoundUserInterfaceMessage previousMsg, BoundUserInterfaceMessage nextMsg, DropshipWeaponsButtonRow row, List<DropshipTerminalWeaponsComponent.TargetEnt> targets, int page, out DropshipWeaponsButtonData? reference, out DropshipWeaponsButtonData? reference2)
		{
			reference = null;
			reference2 = null;
			int num = page * 5;
			if (targets.Count <= 5)
			{
				num = 0;
			}
			else if (num > targets.Count - 5)
			{
				num = targets.Count - 5;
			}
			if (num > 0)
			{
				reference = ButtonAction("previous", delegate
				{
					((BoundUserInterface)this).SendPredictedMessage(previousMsg);
				});
			}
			if (num + 4 < targets.Count - 1)
			{
				reference2 = ButtonAction("next", delegate
				{
					((BoundUserInterface)this).SendPredictedMessage(nextMsg);
				});
			}
			DropshipWeaponsButtonData? one12 = GetTargetData(num);
			DropshipWeaponsButtonData? two3 = GetTargetData(num + 1);
			DropshipWeaponsButtonData? three3 = GetTargetData(num + 2);
			DropshipWeaponsButtonData? four3 = GetTargetData(num + 3);
			DropshipWeaponsButtonData? five2 = GetTargetData(num + 4);
			row.SetData(one12, two3, three3, four3, five2);
			DropshipWeaponsButtonData? GetTargetData(int index)
			{
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_0046: Unknown result type (might be due to invalid IL or missing references)
				DropshipTerminalWeaponsComponent.TargetEnt targetEnt = default(DropshipTerminalWeaponsComponent.TargetEnt);
				if (!Extensions.TryGetValue<DropshipTerminalWeaponsComponent.TargetEnt>((IList<DropshipTerminalWeaponsComponent.TargetEnt>)targets, index, ref targetEnt))
				{
					return null;
				}
				BoundUserInterfaceMessage msg = selectMsg(targetEnt.Id);
				return new DropshipWeaponsButtonData(LocId.op_Implicit(targetEnt.Name), delegate
				{
					((BoundUserInterface)this).SendPredictedMessage(msg);
				});
			}
		}
		void AddTargets(out DropshipWeaponsButtonData? previous6, out DropshipWeaponsButtonData? next6)
		{
			AddButtons((NetEntity target2) => (BoundUserInterfaceMessage)(object)new DropshipTerminalWeaponsTargetsSelectMsg(target2), (BoundUserInterfaceMessage)(object)new DropshipTerminalWeaponsTargetsPreviousMsg(), (BoundUserInterfaceMessage)(object)new DropshipTerminalWeaponsTargetsNextMsg(), screen.RightRow, terminal.Targets, terminal.TargetsPage, out previous6, out next6);
		}
		void AddWeaponEntry(DropshipWeaponsButtonData? data)
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			NetEntity? val7 = data?.Weapon;
			if (val7.HasValue)
			{
				NetEntity valueOrDefault = val7.GetValueOrDefault();
				EntityUid? val8 = default(EntityUid?);
				if (((BoundUserInterface)this).EntMan.TryGetEntity(valueOrDefault, ref val8))
				{
					int num = _weaponSystem.GetWeaponRounds(Entity<DropshipWeaponComponent>.op_Implicit(val8.Value));
					RMCEquipmentDeployerComponent rMCEquipmentDeployerComponent2 = default(RMCEquipmentDeployerComponent);
					if (((BoundUserInterface)this).EntMan.TryGetComponent<RMCEquipmentDeployerComponent>(val8.Value, ref rMCEquipmentDeployerComponent2) && rMCEquipmentDeployerComponent2.DeployEntity.HasValue && _equipmentDeployer.TryGetDeployedAmmo(((BoundUserInterface)this).EntMan.GetEntity(rMCEquipmentDeployerComponent2.DeployEntity.Value), out var ammoCount2, out var _))
					{
						num = ammoCount2.Value;
					}
					text4.AppendLine(Loc.GetString("rmc-dropship-weapons-equip-weapon-ammo", new(string, object)[2]
					{
						("weapon", val8),
						("rounds", num)
					}));
					text4.AppendLine();
				}
			}
		}
		DropshipWeaponsButtonData Button(string suffix, DropshipTerminalWeaponsScreen change)
		{
			DropshipTerminalWeaponsChangeScreenMsg msg = new DropshipTerminalWeaponsChangeScreenMsg(first, change);
			return ButtonAction(suffix, OnPressed);
			void OnPressed(ButtonEventArgs _)
			{
				((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)msg);
			}
		}
		static DropshipWeaponsButtonData ButtonAction(string suffix, Action<ButtonEventArgs> onPressed)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return new DropshipWeaponsButtonData(LocId.op_Implicit("rmc-dropship-weapons-" + suffix), onPressed);
		}
		string TargetAcquisition()
		{
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			EntityUid? entity3 = ((BoundUserInterface)this).EntMan.GetEntity(compScreen.Weapon);
			return Loc.GetString("rmc-dropship-weapons-target-strike", new(string, object)[5]
			{
				("mode", (!compScreen.Weapon.HasValue) ? "NONE" : "WEAPON"),
				("weapon", (!entity3.HasValue) ? "" : ((object)entity3)),
				("target", (!terminal.Target.HasValue) ? "NONE" : ((object)terminal.Target.Value)),
				("xOffset", terminal.Offset.X),
				("yOffset", terminal.Offset.Y)
			});
		}
	}

	private void SetupEmbeddedTacMap(TacticalMapWrapper wrapper)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		TacticalMapComputerComponent tacticalMapComputerComponent = default(TacticalMapComputerComponent);
		if (wrapper != null && ((BoundUserInterface)this).EntMan.TryGetComponent<TacticalMapComputerComponent>(((BoundUserInterface)this).Owner, ref tacticalMapComputerComponent))
		{
			TabContainer.SetTabTitle((Control)(object)wrapper.MapTab, Loc.GetString("rmc-dropship-weapons-maps"));
			TabContainer.SetTabVisible((Control)(object)wrapper.MapTab, true);
			TabContainer.SetTabVisible((Control)(object)wrapper.CanvasTab, false);
			AreaGridComponent item = default(AreaGridComponent);
			if (tacticalMapComputerComponent.Map.HasValue && ((BoundUserInterface)this).EntMan.TryGetComponent<AreaGridComponent>(tacticalMapComputerComponent.Map.Value, ref item))
			{
				wrapper.UpdateTexture(Entity<AreaGridComponent>.op_Implicit((tacticalMapComputerComponent.Map.Value, item)));
			}
			int lineLimit = _tacticalMapSystem.LineLimit;
			wrapper.SetLineLimit(lineLimit);
			wrapper.LastUpdateAt = tacticalMapComputerComponent.LastAnnounceAt;
			wrapper.NextUpdateAt = tacticalMapComputerComponent.NextAnnounceAt;
		}
	}

	private void RefreshEmbeddedTacMap(TacticalMapWrapper wrapper)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		TacticalMapComputerComponent tacticalMapComputerComponent = default(TacticalMapComputerComponent);
		if (wrapper == null || !((BoundUserInterface)this).EntMan.TryGetComponent<TacticalMapComputerComponent>(((BoundUserInterface)this).Owner, ref tacticalMapComputerComponent))
		{
			return;
		}
		TacticalMapBlip[] array = new TacticalMapBlip[tacticalMapComputerComponent.Blips.Count];
		int num = 0;
		foreach (TacticalMapBlip value in tacticalMapComputerComponent.Blips.Values)
		{
			array[num++] = value;
		}
		wrapper.UpdateBlips(array);
		wrapper.Map.Lines.Clear();
		TacticalMapLinesComponent componentOrNull = EntityManagerExt.GetComponentOrNull<TacticalMapLinesComponent>(((BoundUserInterface)this).EntMan, ((BoundUserInterface)this).Owner);
		if (componentOrNull != null)
		{
			wrapper.Map.Lines.AddRange(componentOrNull.MarineLines);
		}
		TacticalMapLabelsComponent componentOrNull2 = EntityManagerExt.GetComponentOrNull<TacticalMapLabelsComponent>(((BoundUserInterface)this).EntMan, ((BoundUserInterface)this).Owner);
		if (componentOrNull2 != null)
		{
			wrapper.UpdateTacticalLabels(componentOrNull2.MarineLabels);
		}
		wrapper.LastUpdateAt = tacticalMapComputerComponent.LastAnnounceAt;
		wrapper.NextUpdateAt = tacticalMapComputerComponent.NextAnnounceAt;
	}

	private void ClearNames(DropshipWeaponsScreen screen)
	{
		screen.TopRow.SetData();
		screen.LeftRow.SetData();
		screen.RightRow.SetData();
		screen.BottomRow.SetData();
	}

	private void TryGetWeapons(bool first, out DropshipWeaponsButtonData? one, out DropshipWeaponsButtonData? two, out DropshipWeaponsButtonData? three, out DropshipWeaponsButtonData? four, out DropshipWeaponsButtonData? utilityOne, out DropshipWeaponsButtonData? utilityTwo, out DropshipWeaponsButtonData? utilityThree, out DropshipWeaponsButtonData? electronicSystemOne, out DropshipWeaponsButtonData? electronicSystemTwo)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		one = null;
		two = null;
		three = null;
		four = null;
		utilityOne = null;
		utilityTwo = null;
		utilityThree = null;
		electronicSystemOne = null;
		electronicSystemTwo = null;
		if (!_system.TryGetGridDropship(((BoundUserInterface)this).Owner, out Entity<DropshipComponent> dropship))
		{
			return;
		}
		List<DropshipWeaponsButtonData?> list = new List<DropshipWeaponsButtonData?>();
		List<DropshipWeaponsButtonData?> list2 = new List<DropshipWeaponsButtonData?>();
		List<DropshipWeaponsButtonData?> list3 = new List<DropshipWeaponsButtonData?>();
		DropshipUtilityPointComponent dropshipUtilityPointComponent = default(DropshipUtilityPointComponent);
		BaseContainer val = default(BaseContainer);
		RMCEquipmentDeployerComponent rMCEquipmentDeployerComponent = default(RMCEquipmentDeployerComponent);
		DropshipElectronicSystemPointComponent dropshipElectronicSystemPointComponent = default(DropshipElectronicSystemPointComponent);
		BaseContainer val3 = default(BaseContainer);
		DropshipWeaponPointComponent dropshipWeaponPointComponent = default(DropshipWeaponPointComponent);
		BaseContainer val5 = default(BaseContainer);
		DropshipWeaponComponent dropshipWeaponComponent = default(DropshipWeaponComponent);
		RMCEquipmentDeployerComponent rMCEquipmentDeployerComponent2 = default(RMCEquipmentDeployerComponent);
		foreach (EntityUid attachmentPoint in dropship.Comp.AttachmentPoints)
		{
			if (((BoundUserInterface)this).EntMan.TryGetComponent<DropshipUtilityPointComponent>(attachmentPoint, ref dropshipUtilityPointComponent) && ((SharedContainerSystem)_container).TryGetContainer(attachmentPoint, dropshipUtilityPointComponent.UtilitySlotId, ref val, (ContainerManagerComponent)null) && val.ContainedEntities.Count > 0)
			{
				EntityUid val2 = val.ContainedEntities[0];
				string text;
				BoundUserInterfaceMessage msg;
				if (((BoundUserInterface)this).EntMan.HasComponent<MedevacComponent>(val2))
				{
					text = "Medevac";
					msg = (BoundUserInterfaceMessage)(object)new DropshipTerminalWeaponsChooseMedevacMsg(first);
				}
				else if (((BoundUserInterface)this).EntMan.HasComponent<RMCFultonComponent>(val2))
				{
					text = "Fulton";
					msg = (BoundUserInterfaceMessage)(object)new DropshipTerminalWeaponsChooseFultonMsg(first);
				}
				else if (((BoundUserInterface)this).EntMan.HasComponent<RMCParaDropComponent>(val2))
				{
					text = "PDS";
					msg = (BoundUserInterfaceMessage)(object)new DropshipTerminalWeaponsChooseParaDropMsg(first);
				}
				else
				{
					if (!((BoundUserInterface)this).EntMan.TryGetComponent<RMCEquipmentDeployerComponent>(val2, ref rMCEquipmentDeployerComponent))
					{
						continue;
					}
					text = rMCEquipmentDeployerComponent.DropShipWindowButtonText;
					msg = (BoundUserInterfaceMessage)(object)new DropshipTerminalWeaponsChooseEquipmentDeployerMsg(first, ((BoundUserInterface)this).EntMan.GetNetEntity(attachmentPoint, (MetaDataComponent)null));
				}
				NetEntity netEntity = ((BoundUserInterface)this).EntMan.GetNetEntity(val2, (MetaDataComponent)null);
				DropshipWeaponsButtonData value = new DropshipWeaponsButtonData(LocId.op_Implicit(text), delegate
				{
					((BoundUserInterface)this).SendPredictedMessage(msg);
				}, netEntity);
				list2.Add(value);
			}
			if (((BoundUserInterface)this).EntMan.TryGetComponent<DropshipElectronicSystemPointComponent>(attachmentPoint, ref dropshipElectronicSystemPointComponent) && ((SharedContainerSystem)_container).TryGetContainer(attachmentPoint, dropshipElectronicSystemPointComponent.ContainerId, ref val3, (ContainerManagerComponent)null) && val3.ContainedEntities.Count > 0)
			{
				EntityUid val4 = val3.ContainedEntities[0];
				if (!((BoundUserInterface)this).EntMan.HasComponent<DropshipSpotlightComponent>(val4))
				{
					continue;
				}
				string text2 = "Spotlight";
				BoundUserInterfaceMessage msg2 = (BoundUserInterfaceMessage)(object)new DropshipTerminalWeaponsChooseSpotlightMsg(first, ((BoundUserInterface)this).EntMan.GetNetEntity(val4, (MetaDataComponent)null));
				NetEntity netEntity2 = ((BoundUserInterface)this).EntMan.GetNetEntity(val4, (MetaDataComponent)null);
				DropshipWeaponsButtonData value2 = new DropshipWeaponsButtonData(LocId.op_Implicit(text2), delegate
				{
					((BoundUserInterface)this).SendPredictedMessage(msg2);
				}, netEntity2);
				list3.Add(value2);
			}
			if (!((BoundUserInterface)this).EntMan.TryGetComponent<DropshipWeaponPointComponent>(attachmentPoint, ref dropshipWeaponPointComponent) || !((SharedContainerSystem)_container).TryGetContainer(attachmentPoint, dropshipWeaponPointComponent.WeaponContainerSlotId, ref val5, (ContainerManagerComponent)null))
			{
				continue;
			}
			foreach (EntityUid containedEntity in val5.ContainedEntities)
			{
				((BoundUserInterface)this).EntMan.TryGetComponent<DropshipWeaponComponent>(containedEntity, ref dropshipWeaponComponent);
				((BoundUserInterface)this).EntMan.TryGetComponent<RMCEquipmentDeployerComponent>(containedEntity, ref rMCEquipmentDeployerComponent2);
				if (dropshipWeaponComponent != null || rMCEquipmentDeployerComponent2 != null)
				{
					string text3 = string.Empty;
					if (dropshipWeaponComponent != null)
					{
						text3 = dropshipWeaponComponent.Abbreviation;
					}
					else if (rMCEquipmentDeployerComponent2 != null)
					{
						text3 = rMCEquipmentDeployerComponent2.DropShipWindowButtonText;
					}
					NetEntity netEntity3 = ((BoundUserInterface)this).EntMan.GetNetEntity(containedEntity, (MetaDataComponent)null);
					DropshipTerminalWeaponsChooseWeaponMsg msg3 = new DropshipTerminalWeaponsChooseWeaponMsg(first, netEntity3);
					DropshipWeaponsButtonData value3 = new DropshipWeaponsButtonData(LocId.op_Implicit(text3), delegate
					{
						((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)msg3);
					}, netEntity3);
					list.Add(value3);
					if (list.Count >= 4)
					{
						break;
					}
				}
			}
		}
		Extensions.TryGetValue<DropshipWeaponsButtonData?>((IList<DropshipWeaponsButtonData?>)list, 0, ref one);
		Extensions.TryGetValue<DropshipWeaponsButtonData?>((IList<DropshipWeaponsButtonData?>)list, 1, ref two);
		Extensions.TryGetValue<DropshipWeaponsButtonData?>((IList<DropshipWeaponsButtonData?>)list, 2, ref three);
		Extensions.TryGetValue<DropshipWeaponsButtonData?>((IList<DropshipWeaponsButtonData?>)list, 3, ref four);
		Extensions.TryGetValue<DropshipWeaponsButtonData?>((IList<DropshipWeaponsButtonData?>)list2, 0, ref utilityOne);
		Extensions.TryGetValue<DropshipWeaponsButtonData?>((IList<DropshipWeaponsButtonData?>)list2, 1, ref utilityTwo);
		Extensions.TryGetValue<DropshipWeaponsButtonData?>((IList<DropshipWeaponsButtonData?>)list2, 2, ref utilityThree);
		Extensions.TryGetValue<DropshipWeaponsButtonData?>((IList<DropshipWeaponsButtonData?>)list3, 0, ref electronicSystemOne);
		Extensions.TryGetValue<DropshipWeaponsButtonData?>((IList<DropshipWeaponsButtonData?>)list3, 1, ref electronicSystemTwo);
	}

	protected override void Dispose(bool disposing)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if (disposing)
		{
			TacticalMapWrapper? embeddedTacMapWrapperScreen = _embeddedTacMapWrapperScreen1;
			if (embeddedTacMapWrapperScreen != null)
			{
				((Control)embeddedTacMapWrapperScreen).Orphan();
			}
			_embeddedTacMapWrapperScreen1 = null;
			TacticalMapWrapper? embeddedTacMapWrapperScreen2 = _embeddedTacMapWrapperScreen2;
			if (embeddedTacMapWrapperScreen2 != null)
			{
				((Control)embeddedTacMapWrapperScreen2).Orphan();
			}
			_embeddedTacMapWrapperScreen2 = null;
			if (_oldEye.HasValue)
			{
				_eyeLerping.RemoveEye(_oldEye.Value);
			}
		}
		base.Dispose(disposing);
	}
}
