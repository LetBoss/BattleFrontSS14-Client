using System;
using System.Collections.Generic;
using System.Linq;
using Content.Client._RMC14.UserInterface;
using Content.Client.Chemistry.Containers.EntitySystems;
using Content.Shared._RMC14.Chemistry.ChemMaster;
using Content.Shared._RMC14.Chemistry.Reagent;
using Content.Shared._RMC14.Extensions;
using Content.Shared._RMC14.IconLabel;
using Content.Shared._RMC14.UserInterface;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.FixedPoint;
using Content.Shared.Storage;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client._RMC14.Chemistry.Master;

public sealed class RMCChemMasterBui : BoundUserInterface, IRefreshableBui
{
	[Dependency]
	private ILocalizationManager _localization;

	[Dependency]
	private RMCReagentSystem _reagent;

	private readonly ContainerSystem _container;

	private readonly ItemSlotsSystem _itemSlots;

	private readonly SolutionContainerSystem _solution;

	private readonly SpriteSystem _sprite;

	private RMCChemMasterWindow? _window;

	private RMCChemMasterPopupWindow? _colorPopup;

	private FixedPoint2? _lastBottleAmount;

	private readonly List<EntityUid> _lastPillBottleRows = new List<EntityUid>();

	private readonly Dictionary<EntityUid, RMCChemMasterPillBottleRow> _bottleRows = new Dictionary<EntityUid, RMCChemMasterPillBottleRow>();

	private readonly Dictionary<int, RMCChemMasterReagentRow> _beakerRows = new Dictionary<int, RMCChemMasterReagentRow>();

	private readonly Dictionary<int, RMCChemMasterReagentRow> _bufferRows = new Dictionary<int, RMCChemMasterReagentRow>();

	private readonly List<int> _toRemove = new List<int>();

	public RMCChemMasterBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		_container = base.EntMan.System<ContainerSystem>();
		_itemSlots = base.EntMan.System<ItemSlotsSystem>();
		_solution = base.EntMan.System<SolutionContainerSystem>();
		_sprite = base.EntMan.System<SpriteSystem>();
	}

	protected override void Open()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Expected O, but got Unknown
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Expected O, but got Unknown
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<RMCChemMasterWindow>((BoundUserInterface)(object)this);
		MetaDataComponent val = default(MetaDataComponent);
		if (base.EntMan.TryGetComponent<MetaDataComponent>(((BoundUserInterface)this).Owner, ref val))
		{
			((DefaultWindow)_window).Title = val.EntityName;
		}
		RMCChemMasterComponent chemMaster = default(RMCChemMasterComponent);
		if (!base.EntMan.TryGetComponent<RMCChemMasterComponent>(((BoundUserInterface)this).Owner, ref chemMaster))
		{
			return;
		}
		((BaseButton)_window.BeakerEjectButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new RMCChemMasterBeakerEjectMsg());
		};
		((BaseButton)_window.BeakerAllButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new RMCChemMasterBeakerTransferAllMsg());
		};
		((BaseButton)_window.BufferModeButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new RMCChemMasterBufferModeMsg(chemMaster.BufferTransferMode.NextWrap()));
		};
		((BaseButton)_window.BufferAllButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new RMCChemMasterBufferTransferAllMsg());
		};
		FloatSpinBox val2 = UIExtensions.CreateDialSpinBox(0f, null, buttons: false, 10);
		val2.OnValueChanged += delegate(FloatSpinBoxEventArgs args)
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new RMCChemMasterSetPillAmountMsg((int)args.Value));
		};
		_window.PillAmountContainer.AddChild((Control)(object)val2);
		ButtonGroup val3 = new ButtonGroup(true);
		for (int num = 0; num < chemMaster.PillTypes; num++)
		{
			RMCChemMasterPillButton rMCChemMasterPillButton = new RMCChemMasterPillButton();
			((BaseButton)rMCChemMasterPillButton.Button).Group = val3;
			int type = num + 1;
			Rsi val4 = new Rsi(chemMaster.PillRsi.RsiPath, $"pill{type}");
			rMCChemMasterPillButton.Texture.Texture = _sprite.Frame0((SpriteSpecifier)(object)val4);
			((BaseButton)rMCChemMasterPillButton.Button).OnPressed += delegate
			{
				((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new RMCChemMasterSetPillTypeMsg((uint)type));
			};
			((Control)_window.PillTypeContainer).AddChild((Control)(object)rMCChemMasterPillButton);
		}
		((BaseButton)_window.CreatePillsButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new RMCChemMasterCreatePillsMsg());
		};
		Refresh();
	}

	public unsafe void Refresh()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		RMCChemMasterComponent rMCChemMasterComponent = default(RMCChemMasterComponent);
		if (_window == null || !base.EntMan.TryGetComponent<RMCChemMasterComponent>(((BoundUserInterface)this).Owner, ref rMCChemMasterComponent))
		{
			return;
		}
		if (_lastBottleAmount != rMCChemMasterComponent.BottleSize)
		{
			_lastBottleAmount = rMCChemMasterComponent.BottleSize;
			_window.CreateBottleButton.Text = Loc.GetString("rmc-chem-master-create-bottle", new(string, object)[1] { ("amount", rMCChemMasterComponent.BottleSize) });
		}
		UpdateBeaker(Entity<RMCChemMasterComponent>.op_Implicit((((BoundUserInterface)this).Owner, rMCChemMasterComponent)));
		UpdatePillBottles(Entity<RMCChemMasterComponent>.op_Implicit((((BoundUserInterface)this).Owner, rMCChemMasterComponent)));
		UpdateBuffer(Entity<RMCChemMasterComponent>.op_Implicit((((BoundUserInterface)this).Owner, rMCChemMasterComponent)));
		int num = (int)(rMCChemMasterComponent.SelectedType - 1);
		if (rMCChemMasterComponent.SelectedType < ((Control)_window.PillTypeContainer).ChildCount && ((Control)_window.PillTypeContainer).GetChild(num) is RMCChemMasterPillButton rMCChemMasterPillButton)
		{
			((BaseButton)rMCChemMasterPillButton.Button).Pressed = true;
		}
		Enumerator enumerator = _window.PillAmountContainer.Children.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				Control current = ((Enumerator)(ref enumerator)).Current;
				FloatSpinBox val = (FloatSpinBox)(object)((current is FloatSpinBox) ? current : null);
				if (val != null)
				{
					val.Value = rMCChemMasterComponent.PillAmount;
				}
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		bool flag = rMCChemMasterComponent.SelectedBottles.Count > 0;
		((BaseButton)_window.CreatePillsButton).Disabled = !flag;
	}

	private void UpdateBeaker(Entity<RMCChemMasterComponent> chemMaster)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		if (_window == null)
		{
			return;
		}
		if (_itemSlots.TryGetSlot(((BoundUserInterface)this).Owner, chemMaster.Comp.BeakerSlot, out ItemSlot itemSlot))
		{
			ContainerSlot? containerSlot = itemSlot.ContainerSlot;
			EntityUid? val = ((containerSlot != null) ? containerSlot.ContainedEntity : ((EntityUid?)null));
			if (val.HasValue)
			{
				EntityUid valueOrDefault = val.GetValueOrDefault();
				FitsInDispenserComponent fitsInDispenserComponent = default(FitsInDispenserComponent);
				if (base.EntMan.TryGetComponent<FitsInDispenserComponent>(valueOrDefault, ref fitsInDispenserComponent) && _solution.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(valueOrDefault), fitsInDispenserComponent.Solution, out Entity<SolutionComponent>? entity))
				{
					_window.BeakerLabel.Text = Loc.GetString("rmc-chem-master-beaker-amount", new(string, object)[1] { ("amount", entity.Value.Comp.Solution.Volume) });
					bool flag = entity.Value.Comp.Solution.Volume > FixedPoint2.Zero;
					((Control)_window.BeakerEmptyLabel).Visible = !flag;
					((Control)_window.BeakerAllButton).Visible = flag;
					for (int i = 0; i < entity.Value.Comp.Solution.Contents.Count; i++)
					{
						ReagentQuantity content = entity.Value.Comp.Solution.Contents[i];
						if (!_beakerRows.TryGetValue(i, out RMCChemMasterReagentRow value))
						{
							value = CreateReagentRow(Entity<RMCChemMasterComponent>.op_Implicit(chemMaster), content, delegate(FixedPoint2 setting)
							{
								//IL_0019: Unknown result type (might be due to invalid IL or missing references)
								((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new RMCChemMasterBeakerTransferMsg(ProtoId<ReagentPrototype>.op_Implicit(content.Reagent.Prototype), setting));
							});
							_beakerRows[i] = value;
							((Control)_window.BeakerContentsContainer).AddChild((Control)(object)value);
						}
						UpdateReagentRow(value, content, delegate(FixedPoint2 setting)
						{
							//IL_0019: Unknown result type (might be due to invalid IL or missing references)
							((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new RMCChemMasterBeakerTransferMsg(ProtoId<ReagentPrototype>.op_Implicit(content.Reagent.Prototype), setting));
						});
						value.AllButton.ClearOnPressed();
						value.AllButton.OnPressed += delegate
						{
							//IL_0019: Unknown result type (might be due to invalid IL or missing references)
							((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new RMCChemMasterBeakerTransferMsg(ProtoId<ReagentPrototype>.op_Implicit(content.Reagent.Prototype), content.Quantity));
						};
						value.OnSubmit = null;
						RMCChemMasterReagentRow rMCChemMasterReagentRow = value;
						rMCChemMasterReagentRow.OnSubmit = (Action<LineEditEventArgs>)Delegate.Combine(rMCChemMasterReagentRow.OnSubmit, (Action<LineEditEventArgs>)delegate(LineEditEventArgs args)
						{
							//IL_0029: Unknown result type (might be due to invalid IL or missing references)
							if (double.TryParse(args.Text, out var result))
							{
								((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new RMCChemMasterBeakerTransferMsg(ProtoId<ReagentPrototype>.op_Implicit(content.Reagent.Prototype), FixedPoint2.New(result)));
							}
						});
					}
					_toRemove.Clear();
					foreach (var (num2, rMCChemMasterReagentRow3) in _beakerRows)
					{
						if (num2 >= entity.Value.Comp.Solution.Contents.Count)
						{
							((Control)rMCChemMasterReagentRow3).Orphan();
							_toRemove.Add(num2);
						}
					}
					{
						foreach (int item in _toRemove)
						{
							_beakerRows.Remove(item);
						}
						return;
					}
				}
			}
		}
		_window.BeakerLabel.Text = Loc.GetString("rmc-chem-master-beaker-none");
		((Control)_window.BeakerEmptyLabel).Visible = true;
		((Control)_window.BeakerAllButton).Visible = false;
		((Control)_window.BeakerContentsContainer).RemoveAllChildren();
		_beakerRows.Clear();
	}

	private unsafe void UpdatePillBottles(Entity<RMCChemMasterComponent> chemMaster)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		if (_window == null)
		{
			return;
		}
		bool flag = chemMaster.Comp.SelectedBottles.Count > 0;
		BaseContainer val = default(BaseContainer);
		if (((SharedContainerSystem)_container).TryGetContainer(((BoundUserInterface)this).Owner, chemMaster.Comp.PillBottleContainer, ref val, (ContainerManagerComponent)null) && val.ContainedEntities.Count > 0)
		{
			((Control)_window.PillBottleColumnLabel).Margin = new Thickness(0f, 3f, 5f, 0f);
			((Control)_window.PillBottlesNoneLabel).Visible = false;
			Enumerator enumerator = ((Control)_window.PillBottlesContainer).Children.GetEnumerator();
			try
			{
				while (((Enumerator)(ref enumerator)).MoveNext())
				{
					if (((Enumerator)(ref enumerator)).Current is RMCChemMasterPillBottleRow rMCChemMasterPillBottleRow)
					{
						((Control)rMCChemMasterPillBottleRow.LabelInput).Visible = flag;
						((BaseButton)rMCChemMasterPillBottleRow.ColorButton).Disabled = !flag;
						break;
					}
				}
			}
			finally
			{
				((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
			}
			if (_lastPillBottleRows.SequenceEqual(val.ContainedEntities))
			{
				foreach (EntityUid containedEntity in val.ContainedEntities)
				{
					if (_bottleRows.TryGetValue(containedEntity, out RMCChemMasterPillBottleRow value))
					{
						UpdatePillBottleFill(value, containedEntity);
						UpdatePillBottleName(value, containedEntity);
					}
				}
				return;
			}
			_lastPillBottleRows.Clear();
			_lastPillBottleRows.AddRange(val.ContainedEntities);
			((Control)(object)_window.PillBottlesContainer).RemoveChildExcept((Control)(object)_window.PillBottlesNoneLabel);
			_bottleRows.Clear();
			NetEntity? netContained = default(NetEntity?);
			for (int i = 0; i < val.ContainedEntities.Count; i++)
			{
				EntityUid val2 = val.ContainedEntities[i];
				if (!base.EntMan.TryGetNetEntity(val2, ref netContained, (MetaDataComponent)null))
				{
					continue;
				}
				RMCChemMasterPillBottleRow rMCChemMasterPillBottleRow2 = new RMCChemMasterPillBottleRow();
				HashSet<EntityUid> selectedBottles = chemMaster.Comp.SelectedBottles;
				((BaseButton)rMCChemMasterPillBottleRow2.FillBottleButton).Pressed = selectedBottles.Contains(val2);
				((BaseButton)rMCChemMasterPillBottleRow2.FillBottleButton).OnPressed += delegate(ButtonEventArgs args)
				{
					//IL_0011: Unknown result type (might be due to invalid IL or missing references)
					((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new RMCChemMasterPillBottleFillMsg(netContained.Value, args.Button.Pressed));
				};
				UpdatePillBottleFill(rMCChemMasterPillBottleRow2, val2);
				UpdatePillBottleName(rMCChemMasterPillBottleRow2, val2);
				if (i == 0)
				{
					rMCChemMasterPillBottleRow2.LabelInput.OnTextEntered += OnLabelInputChanged;
					rMCChemMasterPillBottleRow2.LabelInput.OnFocusExit += OnLabelInputChanged;
					((BaseButton)rMCChemMasterPillBottleRow2.ColorButton).OnPressed += delegate
					{
						//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
						//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
						//IL_00db: Expected O, but got Unknown
						//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
						//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
						//IL_00ef: Expected O, but got Unknown
						if (_colorPopup != null)
						{
							((BaseWindow)_colorPopup).OpenCentered();
						}
						else
						{
							RMCChemMasterBui rMCChemMasterBui = this;
							RMCChemMasterPopupWindow rMCChemMasterPopupWindow = new RMCChemMasterPopupWindow();
							((DefaultWindow)rMCChemMasterPopupWindow).Title = Loc.GetString("rmc-chem-master-pill-bottle-window-title");
							rMCChemMasterBui._colorPopup = rMCChemMasterPopupWindow;
							((BaseWindow)_colorPopup).OnClose += delegate
							{
								_colorPopup = null;
							};
							((BaseWindow)_colorPopup).OpenCentered();
							for (int num = 0; num < chemMaster.Comp.PillCanisterTypes; num++)
							{
								State state = _sprite.GetState(new Rsi(chemMaster.Comp.PillCanisterRsi, $"pill_canister{num}"));
								TextureButton val3 = new TextureButton
								{
									TextureNormal = state.Frame0
								};
								int type = num;
								((BaseButton)val3).OnPressed += delegate
								{
									((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new RMCChemMasterPillBottleColorMsg((RMCPillBottleColors)type));
									((BaseWindow)_colorPopup).Close();
								};
								((Control)_colorPopup.Grid).AddChild((Control)(object)val3);
							}
						}
					};
					if (!flag)
					{
						((Control)rMCChemMasterPillBottleRow2.LabelInput).Visible = false;
						((BaseButton)rMCChemMasterPillBottleRow2.ColorButton).Disabled = true;
					}
					Control parent = ((Control)rMCChemMasterPillBottleRow2.ColorView).Parent;
					if (parent != null)
					{
						parent.Margin = default(Thickness);
					}
					((Control)rMCChemMasterPillBottleRow2.ColorView).Orphan();
					((Control)rMCChemMasterPillBottleRow2.ColorButton).AddChild((Control)(object)rMCChemMasterPillBottleRow2.ColorView);
				}
				else
				{
					((Control)rMCChemMasterPillBottleRow2.LabelInput).Visible = false;
					((Control)rMCChemMasterPillBottleRow2.ColorButton).Visible = false;
				}
				rMCChemMasterPillBottleRow2.ColorView.SetEntity((EntityUid?)val2);
				((BaseButton)rMCChemMasterPillBottleRow2.TransferButton).OnPressed += delegate
				{
					//IL_0011: Unknown result type (might be due to invalid IL or missing references)
					((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new RMCChemMasterPillBottleTransferMsg(netContained.Value));
				};
				((BaseButton)rMCChemMasterPillBottleRow2.EjectButton).OnPressed += delegate
				{
					//IL_0011: Unknown result type (might be due to invalid IL or missing references)
					((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new RMCChemMasterPillBottleEjectMsg(netContained.Value));
				};
				((Control)_window.PillBottlesContainer).AddChild((Control)(object)rMCChemMasterPillBottleRow2);
				_bottleRows[val2] = rMCChemMasterPillBottleRow2;
			}
		}
		else
		{
			_lastPillBottleRows.Clear();
			((Control)_window.PillBottleColumnLabel).Margin = new Thickness(0f, 0f, 5f, 0f);
			((Control)(object)_window.PillBottlesContainer).RemoveChildExcept((Control)(object)_window.PillBottlesNoneLabel);
			((Control)_window.PillBottlesNoneLabel).Visible = true;
		}
	}

	private void UpdateBuffer(Entity<RMCChemMasterComponent> chemMaster)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		if (_window == null)
		{
			return;
		}
		Button bufferModeButton = _window.BufferModeButton;
		bufferModeButton.Text = chemMaster.Comp.BufferTransferMode switch
		{
			RMCChemMasterBufferMode.ToBeaker => Loc.GetString("rmc-chem-master-buffer-to-beaker"), 
			RMCChemMasterBufferMode.ToDisposal => Loc.GetString("rmc-chem-master-buffer-to-disposal"), 
			_ => _window.BufferModeButton.Text, 
		};
		if (!_solution.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(((BoundUserInterface)this).Owner), chemMaster.Comp.BufferSolutionId, out Entity<SolutionComponent>? entity) || entity.Value.Comp.Solution.Volume <= FixedPoint2.Zero)
		{
			((Control)_window.BufferEmptyLabel).Visible = true;
			((Control)_window.BufferAllButton).Visible = false;
			((Control)_window.BufferContainer).RemoveAllChildren();
			_bufferRows.Clear();
			return;
		}
		((Control)_window.BufferEmptyLabel).Visible = false;
		((Control)_window.BufferAllButton).Visible = true;
		for (int i = 0; i < entity.Value.Comp.Solution.Contents.Count; i++)
		{
			ReagentQuantity content = entity.Value.Comp.Solution.Contents[i];
			if (!_bufferRows.TryGetValue(i, out RMCChemMasterReagentRow value))
			{
				value = CreateReagentRow(Entity<RMCChemMasterComponent>.op_Implicit(chemMaster), content, delegate(FixedPoint2 setting)
				{
					//IL_0019: Unknown result type (might be due to invalid IL or missing references)
					((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new RMCChemMasterBufferTransferMsg(ProtoId<ReagentPrototype>.op_Implicit(content.Reagent.Prototype), setting));
				});
				_bufferRows[i] = value;
				((Control)_window.BufferContainer).AddChild((Control)(object)value);
			}
			UpdateReagentRow(value, content, delegate(FixedPoint2 setting)
			{
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new RMCChemMasterBufferTransferMsg(ProtoId<ReagentPrototype>.op_Implicit(content.Reagent.Prototype), setting));
			});
			value.AllButton.ClearOnPressed();
			value.AllButton.OnPressed += delegate
			{
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new RMCChemMasterBufferTransferMsg(ProtoId<ReagentPrototype>.op_Implicit(content.Reagent.Prototype), content.Quantity));
			};
			value.OnSubmit = null;
			RMCChemMasterReagentRow rMCChemMasterReagentRow = value;
			rMCChemMasterReagentRow.OnSubmit = (Action<LineEditEventArgs>)Delegate.Combine(rMCChemMasterReagentRow.OnSubmit, (Action<LineEditEventArgs>)delegate(LineEditEventArgs args)
			{
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				if (double.TryParse(args.Text, out var result))
				{
					((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new RMCChemMasterBufferTransferMsg(ProtoId<ReagentPrototype>.op_Implicit(content.Reagent.Prototype), FixedPoint2.New(result)));
				}
			});
		}
		_toRemove.Clear();
		foreach (var (num2, rMCChemMasterReagentRow3) in _bufferRows)
		{
			if (num2 >= entity.Value.Comp.Solution.Contents.Count)
			{
				((Control)rMCChemMasterReagentRow3).Orphan();
				_toRemove.Add(num2);
			}
		}
		foreach (int item in _toRemove)
		{
			_bufferRows.Remove(item);
		}
	}

	private RMCChemMasterReagentRow CreateReagentRow(RMCChemMasterComponent chemMaster, ReagentQuantity reagent, Action<FixedPoint2> onTransfer)
	{
		RMCChemMasterReagentRow rMCChemMasterReagentRow = new RMCChemMasterReagentRow();
		FixedPoint2[] transferSettings = chemMaster.TransferSettings;
		foreach (FixedPoint2 amount in transferSettings)
		{
			RMCChemMasterTransferButton rMCChemMasterTransferButton = new RMCChemMasterTransferButton
			{
				Amount = amount
			};
			((Control)rMCChemMasterReagentRow.TransferSettingsContainer).AddChild((Control)(object)rMCChemMasterTransferButton);
		}
		UpdateReagentRow(rMCChemMasterReagentRow, reagent, onTransfer);
		return rMCChemMasterReagentRow;
	}

	private unsafe void UpdateReagentRow(RMCChemMasterReagentRow row, ReagentQuantity reagent, Action<FixedPoint2> onTransfer)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		string text = reagent.Reagent.Prototype;
		if (_reagent.TryIndex(ProtoId<ReagentPrototype>.op_Implicit(text), out Reagent reagent2))
		{
			text = reagent2.LocalizedName;
		}
		row.ReagentLabel.Text = Loc.GetString("rmc-chem-master-reagent-amount", new(string, object)[2]
		{
			("name", text),
			("amount", reagent.Quantity)
		});
		Enumerator enumerator = ((Control)row.TransferSettingsContainer).Children.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				if (((Enumerator)(ref enumerator)).Current is RMCChemMasterTransferButton rMCChemMasterTransferButton)
				{
					rMCChemMasterTransferButton.Button.Text = rMCChemMasterTransferButton.Amount.ToString();
					rMCChemMasterTransferButton.OnPressed = null;
					rMCChemMasterTransferButton.OnPressed = (Action<FixedPoint2>)Delegate.Combine(rMCChemMasterTransferButton.OnPressed, onTransfer);
				}
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	private void OnLabelInputChanged(LineEditEventArgs args)
	{
		if (((Control)args.Control).Root != null)
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new RMCChemMasterPillBottleLabelMsg(args.Text));
		}
	}

	private void UpdatePillBottleFill(RMCChemMasterPillBottleRow row, EntityUid contained)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		StorageComponent storageComponent = default(StorageComponent);
		if (base.EntMan.TryGetComponent<StorageComponent>(contained, ref storageComponent))
		{
			int num = 0;
			Box2i? val = default(Box2i?);
			if (Extensions.TryFirstOrNull<Box2i>((IEnumerable<Box2i>)storageComponent.Grid, ref val))
			{
				Box2i value = val.Value;
				num = ((Box2i)(ref value)).Width + 1;
			}
			row.PillAmountLabel.Text = Loc.GetString("rmc-chem-master-pill-bottle-pills", new(string, object)[2]
			{
				("amount", storageComponent.StoredItems.Count),
				("total", num)
			});
			row.ColorView.SetEntity((EntityUid?)contained);
		}
	}

	private void UpdatePillBottleName(RMCChemMasterPillBottleRow row, EntityUid contained)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		IconLabelComponent iconLabelComponent = default(IconLabelComponent);
		if (base.EntMan.TryGetComponent<IconLabelComponent>(contained, ref iconLabelComponent) && iconLabelComponent.LabelTextLocId.HasValue)
		{
			ILocalizationManager localization = _localization;
			LocId? labelTextLocId = iconLabelComponent.LabelTextLocId;
			string text = default(string);
			if (localization.TryGetString(labelTextLocId.HasValue ? LocId.op_Implicit(labelTextLocId.GetValueOrDefault()) : null, ref text, iconLabelComponent.LabelTextParams.ToArray()) && text.Length > 0)
			{
				if (text.Length > 3)
				{
					text = text.Substring(0, 3);
				}
				row.NameLabel.Text = "(" + text + ")";
				return;
			}
		}
		row.NameLabel.Text = string.Empty;
	}
}
