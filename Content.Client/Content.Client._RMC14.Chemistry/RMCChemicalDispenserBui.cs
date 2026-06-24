using System;
using System.Collections.Generic;
using System.Globalization;
using Content.Client.Chemistry.Containers.EntitySystems;
using Content.Client.UserInterface.ControlExtensions;
using Content.Shared._RMC14.Chemistry;
using Content.Shared._RMC14.Chemistry.Reagent;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client._RMC14.Chemistry;

public sealed class RMCChemicalDispenserBui : BoundUserInterface
{
	[Dependency]
	private RMCReagentSystem _reagent;

	private RMCChemicalDispenserWindow? _window;

	private readonly ContainerSystem _container;

	private readonly SolutionContainerSystem _solution;

	private readonly List<(Button Button, FixedPoint2 Amount)> _dispenseButtons = new List<(Button, FixedPoint2)>();

	public RMCChemicalDispenserBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_container = base.EntMan.System<ContainerSystem>();
		_solution = base.EntMan.System<SolutionContainerSystem>();
	}

	protected override void Open()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Expected O, but got Unknown
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Expected O, but got Unknown
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<RMCChemicalDispenserWindow>((BoundUserInterface)(object)this);
		((BaseButton)_window.EjectBeakerButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new RMCChemicalDispenserEjectBeakerBuiMsg());
		};
		RMCChemicalDispenserComponent rMCChemicalDispenserComponent = default(RMCChemicalDispenserComponent);
		if (base.EntMan.TryGetComponent<RMCChemicalDispenserComponent>(((BoundUserInterface)this).Owner, ref rMCChemicalDispenserComponent))
		{
			ProtoId<ReagentPrototype> reagentId = default(ProtoId<ReagentPrototype>);
			for (int num = 0; num < rMCChemicalDispenserComponent.Reagents.Length; num += 3)
			{
				BoxContainer row = new BoxContainer();
				for (int num2 = num; num2 < num + 3; num2++)
				{
					if (Extensions.TryGetValue<ProtoId<ReagentPrototype>>((IList<ProtoId<ReagentPrototype>>)rMCChemicalDispenserComponent.Reagents, num2, ref reagentId))
					{
						AddButton(reagentId);
					}
				}
				((Control)_window.ChemicalsContainer).AddChild((Control)(object)row);
				void AddButton(ProtoId<ReagentPrototype> val3)
				{
					//IL_000e: Unknown result type (might be due to invalid IL or missing references)
					//IL_000f: Unknown result type (might be due to invalid IL or missing references)
					//IL_001b: Unknown result type (might be due to invalid IL or missing references)
					//IL_0029: Unknown result type (might be due to invalid IL or missing references)
					//IL_002e: Unknown result type (might be due to invalid IL or missing references)
					//IL_0053: Unknown result type (might be due to invalid IL or missing references)
					//IL_005a: Unknown result type (might be due to invalid IL or missing references)
					//IL_006b: Expected O, but got Unknown
					if (_reagent.TryIndex(val3, out Reagent reagent))
					{
						Button val4 = new Button
						{
							Text = "▼ " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(reagent.LocalizedName),
							HorizontalExpand = true,
							StyleClasses = { "OpenBoth" }
						};
						((Control)val4.Label).AddStyleClass("CMAlignLeft");
						((BaseButton)val4).OnPressed += delegate
						{
							//IL_0007: Unknown result type (might be due to invalid IL or missing references)
							((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new RMCChemicalDispenserDispenseBuiMsg(val3));
						};
						((Control)row).AddChild((Control)(object)val4);
					}
				}
			}
			FixedPoint2[] settings = rMCChemicalDispenserComponent.Settings;
			for (int num3 = 0; num3 < settings.Length; num3++)
			{
				FixedPoint2 setting = settings[num3];
				Button val = new Button
				{
					Text = $"+ {setting.Int()}",
					StyleClasses = { "OpenBoth" },
					SetWidth = 45f,
					Margin = new Thickness(0f, 0f, 0f, 3f),
					Pressed = (rMCChemicalDispenserComponent.DispenseSetting == setting)
				};
				((BaseButton)val).OnPressed += delegate
				{
					((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new RMCChemicalDispenserDispenseSettingBuiMsg(setting));
				};
				((Control)_window.DispenseContainer).AddChild((Control)(object)val);
				_dispenseButtons.Add((val, setting));
				Button val2 = new Button
				{
					Text = $"- {setting.Int()}",
					StyleClasses = { "OpenBoth" },
					SetWidth = 45f,
					Margin = new Thickness(0f, 0f, 0f, 3f)
				};
				((BaseButton)val2).OnPressed += delegate
				{
					((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new RMCChemicalDispenserBeakerBuiMsg(setting));
				};
				((Control)_window.BeakerContainer).AddChild((Control)(object)val2);
			}
		}
		Refresh();
	}

	public void Refresh()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Expected O, but got Unknown
		RMCChemicalDispenserWindow window = _window;
		RMCChemicalDispenserComponent rMCChemicalDispenserComponent = default(RMCChemicalDispenserComponent);
		if (window == null || !((BaseWindow)window).IsOpen || !base.EntMan.TryGetComponent<RMCChemicalDispenserComponent>(((BoundUserInterface)this).Owner, ref rMCChemicalDispenserComponent))
		{
			return;
		}
		FixedPoint2 maxEnergy = rMCChemicalDispenserComponent.MaxEnergy;
		((Range)_window.EnergyBar).MaxValue = maxEnergy.Float();
		FixedPoint2 energy = rMCChemicalDispenserComponent.Energy;
		((Range)_window.EnergyBar).Value = energy.Float();
		_window.EnergyLabel.Text = $"{energy.Int()} energy";
		BaseContainer val = default(BaseContainer);
		EntityUid? val2 = default(EntityUid?);
		if (!((SharedContainerSystem)_container).TryGetContainer(((BoundUserInterface)this).Owner, rMCChemicalDispenserComponent.ContainerSlotId, ref val, (ContainerManagerComponent)null) || !Extensions.TryFirstOrNull<EntityUid>((IEnumerable<EntityUid>)val.ContainedEntities, ref val2))
		{
			_window.BeakerStatus.Text = "No beaker loaded!";
			((Control)_window.EjectBeakerButton).Visible = false;
			((Control)_window.ContentsNone).Visible = true;
			((Control)_window.BeakerContents).Visible = false;
			((Control)_window.BeakerContents).DisposeAllChildren();
			foreach (Button item in ((Control)(object)_window.ChemicalsContainer).GetControlOfType<Button>())
			{
				((BaseButton)item).Disabled = true;
			}
		}
		else
		{
			((Control)_window.EjectBeakerButton).Visible = true;
			((Control)_window.ContentsNone).Visible = false;
			((Control)_window.BeakerContents).Visible = true;
			((Control)_window.BeakerContents).DisposeAllChildren();
			foreach (Button item2 in ((Control)(object)_window.ChemicalsContainer).GetControlOfType<Button>())
			{
				((BaseButton)item2).Disabled = false;
			}
			FixedPoint2 value = FixedPoint2.Zero;
			FixedPoint2 value2 = FixedPoint2.Zero;
			if (_solution.TryGetMixableSolution(Entity<MixableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(val2.Value), out Entity<SolutionComponent>? _, out Solution solution))
			{
				value = solution.Volume;
				value2 = solution.MaxVolume;
				foreach (ReagentQuantity content in solution.Contents)
				{
					string text = content.Reagent.Prototype;
					if (_reagent.TryIndex(ProtoId<ReagentPrototype>.op_Implicit(text), out Reagent reagent))
					{
						text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(reagent.LocalizedName);
					}
					((Control)_window.BeakerContents).AddChild((Control)new Label
					{
						Text = $"{content.Quantity} units of {text}"
					});
				}
			}
			_window.BeakerStatus.Text = $"{value}/{value2} units";
		}
		foreach (var (val3, fixedPoint) in _dispenseButtons)
		{
			((BaseButton)val3).Pressed = rMCChemicalDispenserComponent.DispenseSetting == fixedPoint;
		}
	}
}
