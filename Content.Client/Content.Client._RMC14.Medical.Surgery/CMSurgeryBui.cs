using System;
using System.Collections.Generic;
using Content.Client._RMC14.Xenonids.UI;
using Content.Client.Administration.UI.CustomControls;
using Content.Shared._RMC14.Medical.Surgery;
using Content.Shared.Body.Part;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Client._RMC14.Medical.Surgery;

public sealed class CMSurgeryBui : BoundUserInterface
{
	private enum ViewType
	{
		Parts,
		Surgeries,
		Steps
	}

	private enum StepStatus
	{
		Next,
		Complete,
		Incomplete
	}

	[Dependency]
	private IEntityManager _entities;

	[Dependency]
	private IPlayerManager _player;

	private readonly CMSurgerySystem _system;

	[ViewVariables]
	private CMSurgeryWindow? _window;

	private EntityUid? _part;

	private (EntityUid Ent, EntProtoId Proto)? _surgery;

	private readonly List<EntProtoId> _previousSurgeries = new List<EntProtoId>();

	public CMSurgeryBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_system = _entities.System<CMSurgerySystem>();
	}

	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_system.OnRefresh += delegate
		{
			UpdateDisabledPanel();
			RefreshUI();
		};
		if (((BoundUserInterface)this).State is CMSurgeryBuiState state)
		{
			Update(state);
		}
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		if (state is CMSurgeryBuiState state2)
		{
			Update(state2);
		}
	}

	private void Update(CMSurgeryBuiState state)
	{
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		if (_window == null)
		{
			_window = BoundUserInterfaceExt.CreateWindow<CMSurgeryWindow>((BoundUserInterface)(object)this);
			((BaseWindow)_window).OnClose += delegate
			{
				_system.OnRefresh -= RefreshUI;
			};
			((DefaultWindow)_window).Title = "Surgery";
			((BaseButton)_window.PartsButton).OnPressed += delegate
			{
				_part = null;
				_surgery = null;
				_previousSurgeries.Clear();
				View(ViewType.Parts);
			};
			((BaseButton)_window.SurgeriesButton).OnPressed += delegate
			{
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				//IL_0056: Unknown result type (might be due to invalid IL or missing references)
				_surgery = null;
				_previousSurgeries.Clear();
				NetEntity? val3 = default(NetEntity?);
				if (_entities.TryGetNetEntity(_part, ref val3, (MetaDataComponent)null) && ((BoundUserInterface)this).State is CMSurgeryBuiState cMSurgeryBuiState && cMSurgeryBuiState.Choices.TryGetValue(val3.Value, out List<EntProtoId> value))
				{
					OnPartPressed(val3.Value, value);
				}
			};
			((BaseButton)_window.StepsButton).OnPressed += delegate
			{
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_0037: Unknown result type (might be due to invalid IL or missing references)
				//IL_0056: Unknown result type (might be due to invalid IL or missing references)
				//IL_0069: Unknown result type (might be due to invalid IL or missing references)
				//IL_006e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0075: Unknown result type (might be due to invalid IL or missing references)
				//IL_0081: Unknown result type (might be due to invalid IL or missing references)
				//IL_0088: Unknown result type (might be due to invalid IL or missing references)
				//IL_008f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0094: Unknown result type (might be due to invalid IL or missing references)
				NetEntity? val3 = default(NetEntity?);
				if (_entities.TryGetNetEntity(_part, ref val3, (MetaDataComponent)null) && _previousSurgeries.Count != 0)
				{
					List<EntProtoId> previousSurgeries = _previousSurgeries;
					EntProtoId val4 = previousSurgeries[previousSurgeries.Count - 1];
					_previousSurgeries.RemoveAt(_previousSurgeries.Count - 1);
					EntityUid? singleton2 = _system.GetSingleton(val4);
					if (singleton2.HasValue)
					{
						EntityUid valueOrDefault2 = singleton2.GetValueOrDefault();
						CMSurgeryComponent item3 = default(CMSurgeryComponent);
						if (_entities.TryGetComponent<CMSurgeryComponent>(valueOrDefault2, ref item3))
						{
							OnSurgeryPressed(Entity<CMSurgeryComponent>.op_Implicit((valueOrDefault2, item3)), val3.Value, val4);
						}
					}
				}
			};
		}
		((Control)_window.Surgeries).DisposeAllChildren();
		((Control)_window.Steps).DisposeAllChildren();
		((Control)_window.Parts).DisposeAllChildren();
		View(ViewType.Parts);
		(EntityUid, EntProtoId)? surgery = _surgery;
		EntityUid? part = _part;
		_part = null;
		_surgery = null;
		List<Entity<BodyPartComponent>> list = new List<Entity<BodyPartComponent>>(state.Choices.Keys.Count);
		EntityUid? val = default(EntityUid?);
		BodyPartComponent item = default(BodyPartComponent);
		foreach (NetEntity key in state.Choices.Keys)
		{
			if (_entities.TryGetEntity(key, ref val) && _entities.TryGetComponent<BodyPartComponent>(val, ref item))
			{
				list.Add(Entity<BodyPartComponent>.op_Implicit((val.Value, item)));
			}
		}
		list.Sort((Entity<BodyPartComponent> a, Entity<BodyPartComponent> b) => GetScore(a) - GetScore(b));
		CMSurgeryComponent item2 = default(CMSurgeryComponent);
		foreach (Entity<BodyPartComponent> item4 in list)
		{
			NetEntity netPart = _entities.GetNetEntity(item4.Owner, (MetaDataComponent)null);
			List<EntProtoId> surgeries = state.Choices[netPart];
			string entityName = _entities.GetComponent<MetaDataComponent>(Entity<BodyPartComponent>.op_Implicit(item4)).EntityName;
			XenoChoiceControl xenoChoiceControl = new XenoChoiceControl();
			xenoChoiceControl.Set(entityName, null);
			((BaseButton)xenoChoiceControl.Button).OnPressed += delegate
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				OnPartPressed(netPart, surgeries);
			};
			((Control)_window.Parts).AddChild((Control)(object)xenoChoiceControl);
			EntityUid? singleton;
			EntityUid val2;
			foreach (EntProtoId item5 in surgeries)
			{
				singleton = _system.GetSingleton(item5);
				if (!singleton.HasValue)
				{
					continue;
				}
				EntityUid valueOrDefault = singleton.GetValueOrDefault();
				if (_entities.TryGetComponent<CMSurgeryComponent>(valueOrDefault, ref item2))
				{
					singleton = part;
					val2 = Entity<BodyPartComponent>.op_Implicit(item4);
					if (singleton.HasValue && singleton.GetValueOrDefault() == val2 && surgery.HasValue && surgery.GetValueOrDefault().Item2 == item5)
					{
						OnSurgeryPressed(Entity<CMSurgeryComponent>.op_Implicit((valueOrDefault, item2)), netPart, item5);
					}
				}
			}
			singleton = part;
			val2 = Entity<BodyPartComponent>.op_Implicit(item4);
			if (singleton.HasValue && singleton.GetValueOrDefault() == val2 && !surgery.HasValue)
			{
				OnPartPressed(netPart, surgeries);
			}
		}
		RefreshUI();
		UpdateDisabledPanel();
		if (!((BaseWindow)_window).IsOpen)
		{
			((BaseWindow)_window).OpenCentered();
		}
		static int GetScore(Entity<BodyPartComponent> val3)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return val3.Comp.PartType switch
			{
				BodyPartType.Head => 1, 
				BodyPartType.Torso => 2, 
				BodyPartType.Arm => 3, 
				BodyPartType.Hand => 4, 
				BodyPartType.Leg => 5, 
				BodyPartType.Foot => 6, 
				BodyPartType.Tail => 7, 
				BodyPartType.Other => 8, 
				_ => 0, 
			};
		}
	}

	private void AddStep(EntProtoId stepId, NetEntity netPart, EntProtoId surgeryId)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		if (_window == null)
		{
			return;
		}
		EntityUid? singleton = _system.GetSingleton(stepId);
		if (singleton.HasValue)
		{
			EntityUid valueOrDefault = singleton.GetValueOrDefault();
			new FormattedMessage().AddText(_entities.GetComponent<MetaDataComponent>(valueOrDefault).EntityName);
			CMSurgeryStepButton cMSurgeryStepButton = new CMSurgeryStepButton
			{
				Step = valueOrDefault
			};
			((BaseButton)cMSurgeryStepButton.Button).OnPressed += delegate
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new CMSurgeryStepChosenBuiMsg(netPart, surgeryId, stepId));
			};
			((Control)_window.Steps).AddChild((Control)(object)cMSurgeryStepButton);
		}
	}

	private void OnSurgeryPressed(Entity<CMSurgeryComponent> surgery, NetEntity netPart, EntProtoId surgeryId)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Expected O, but got Unknown
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		if (_window == null)
		{
			return;
		}
		_part = _entities.GetEntity(netPart);
		_surgery = (Entity<CMSurgeryComponent>.op_Implicit(surgery), surgeryId);
		((Control)_window.Steps).DisposeAllChildren();
		EntProtoId? requirement = surgery.Comp.Requirement;
		if (requirement.HasValue)
		{
			EntProtoId requirementId = requirement.GetValueOrDefault();
			EntityUid? singleton = _system.GetSingleton(requirementId);
			if (singleton.HasValue)
			{
				EntityUid requirement2 = singleton.GetValueOrDefault();
				XenoChoiceControl xenoChoiceControl = new XenoChoiceControl();
				((BaseButton)xenoChoiceControl.Button).OnPressed += delegate
				{
					//IL_000c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0022: Unknown result type (might be due to invalid IL or missing references)
					//IL_0037: Unknown result type (might be due to invalid IL or missing references)
					//IL_0042: Unknown result type (might be due to invalid IL or missing references)
					//IL_0048: Unknown result type (might be due to invalid IL or missing references)
					//IL_004e: Unknown result type (might be due to invalid IL or missing references)
					_previousSurgeries.Add(surgeryId);
					CMSurgeryComponent item = default(CMSurgeryComponent);
					if (_entities.TryGetComponent<CMSurgeryComponent>(requirement2, ref item))
					{
						OnSurgeryPressed(Entity<CMSurgeryComponent>.op_Implicit((requirement2, item)), netPart, requirementId);
					}
				};
				FormattedMessage val = new FormattedMessage();
				string entityName = _entities.GetComponent<MetaDataComponent>(requirement2).EntityName;
				val.AddMarkupOrThrow("[bold]Requires: " + entityName + "[/bold]");
				xenoChoiceControl.Set(val, null);
				((Control)_window.Steps).AddChild((Control)(object)xenoChoiceControl);
				BoxContainer steps = _window.Steps;
				HSeparator obj = new HSeparator
				{
					Color = Color.FromHex((ReadOnlySpan<char>)"#4972A1", (Color?)null)
				};
				((Control)obj).Margin = new Thickness(0f, 0f, 0f, 1f);
				((Control)steps).AddChild((Control)(object)obj);
			}
		}
		foreach (EntProtoId step in surgery.Comp.Steps)
		{
			AddStep(step, netPart, surgeryId);
		}
		View(ViewType.Steps);
		RefreshUI();
	}

	private void OnPartPressed(NetEntity netPart, List<EntProtoId> surgeryIds)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		if (_window == null)
		{
			return;
		}
		_part = _entities.GetEntity(netPart);
		((Control)_window.Surgeries).DisposeAllChildren();
		List<(Entity<CMSurgeryComponent>, EntProtoId, string)> list = new List<(Entity<CMSurgeryComponent>, EntProtoId, string)>();
		CMSurgeryComponent item = default(CMSurgeryComponent);
		foreach (EntProtoId surgeryId in surgeryIds)
		{
			EntityUid? singleton = _system.GetSingleton(surgeryId);
			if (singleton.HasValue)
			{
				EntityUid valueOrDefault = singleton.GetValueOrDefault();
				if (_entities.TryGetComponent<CMSurgeryComponent>(valueOrDefault, ref item))
				{
					string entityName = _entities.GetComponent<MetaDataComponent>(valueOrDefault).EntityName;
					list.Add((Entity<CMSurgeryComponent>.op_Implicit((valueOrDefault, item)), surgeryId, entityName));
				}
			}
		}
		list.Sort(delegate((Entity<CMSurgeryComponent> Ent, EntProtoId Id, string Name) a, (Entity<CMSurgeryComponent> Ent, EntProtoId Id, string Name) b)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			int num = a.Ent.Comp.Priority.CompareTo(b.Ent.Comp.Priority);
			return (num != 0) ? num : string.Compare(a.Name, b.Name, StringComparison.Ordinal);
		});
		foreach (var surgery in list)
		{
			XenoChoiceControl xenoChoiceControl = new XenoChoiceControl();
			xenoChoiceControl.Set(surgery.Item3, null);
			((BaseButton)xenoChoiceControl.Button).OnPressed += delegate
			{
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				OnSurgeryPressed(surgery.Item1, netPart, surgery.Item2);
			};
			((Control)_window.Surgeries).AddChild((Control)(object)xenoChoiceControl);
		}
		RefreshUI();
		View(ViewType.Surgeries);
	}

	private unsafe void RefreshUI()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Expected O, but got Unknown
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		BodyPartComponent bodyPartComponent = default(BodyPartComponent);
		if (_window == null || !_entities.HasComponent<CMSurgeryComponent>(_surgery?.Ent) || !_entities.TryGetComponent<BodyPartComponent>(_part, ref bodyPartComponent))
		{
			return;
		}
		(Entity<CMSurgeryComponent>, int)? nextStep = _system.GetNextStep(((BoundUserInterface)this).Owner, _part.Value, _surgery.Value.Ent);
		int num = 0;
		Enumerator enumerator = ((Control)_window.Steps).Children.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				if (!(((Enumerator)(ref enumerator)).Current is CMSurgeryStepButton cMSurgeryStepButton))
				{
					continue;
				}
				StepStatus stepStatus = StepStatus.Incomplete;
				if (!nextStep.HasValue)
				{
					stepStatus = StepStatus.Complete;
				}
				else if (nextStep.Value.Item1.Owner != _surgery.Value.Ent)
				{
					stepStatus = StepStatus.Incomplete;
				}
				else if (nextStep.Value.Item2 == num)
				{
					stepStatus = StepStatus.Next;
				}
				else if (num < nextStep.Value.Item2)
				{
					stepStatus = StepStatus.Complete;
				}
				((BaseButton)cMSurgeryStepButton.Button).Disabled = stepStatus != StepStatus.Next;
				FormattedMessage val = new FormattedMessage();
				val.AddText(_entities.GetComponent<MetaDataComponent>(cMSurgeryStepButton.Step).EntityName);
				if (stepStatus == StepStatus.Complete)
				{
					((Control)cMSurgeryStepButton.Button).Modulate = Color.Green;
				}
				else
				{
					((Control)cMSurgeryStepButton.Button).Modulate = Color.White;
					EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
					if (localEntity.HasValue)
					{
						EntityUid valueOrDefault = localEntity.GetValueOrDefault();
						if (!_system.CanPerformStep(valueOrDefault, ((BoundUserInterface)this).Owner, bodyPartComponent.PartType, cMSurgeryStepButton.Step, doPopup: false, out string popup, out StepInvalidReason reason, out HashSet<EntityUid> _))
						{
							((Control)cMSurgeryStepButton).ToolTip = popup;
							((BaseButton)cMSurgeryStepButton.Button).Disabled = true;
							switch (reason)
							{
							case StepInvalidReason.MissingSkills:
								val.AddMarkupOrThrow(" [color=red](Missing surgery skill)[/color]");
								break;
							case StepInvalidReason.NeedsOperatingTable:
								val.AddMarkupOrThrow(" [color=red](Needs operating table)[/color]");
								break;
							case StepInvalidReason.Armor:
								val.AddMarkupOrThrow(" [color=red](Remove their armor!)[/color]");
								break;
							case StepInvalidReason.MissingTool:
								val.AddMarkupOrThrow(" [color=red](Missing tool)[/color]");
								break;
							}
						}
					}
				}
				SpriteComponent componentOrNull = EntityManagerExt.GetComponentOrNull<SpriteComponent>(_entities, cMSurgeryStepButton.Step);
				object obj;
				if (componentOrNull == null)
				{
					obj = null;
				}
				else
				{
					IRsiStateLike icon = componentOrNull.Icon;
					obj = ((icon != null) ? ((IDirectionalTextureProvider)icon).Default : null);
				}
				Texture texture = (Texture)obj;
				cMSurgeryStepButton.Set(val, texture);
				num++;
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		UpdateDisabledPanel();
	}

	private void UpdateDisabledPanel()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		if (_window != null)
		{
			if (_system.IsLyingDown(((BoundUserInterface)this).Owner))
			{
				((Control)_window.DisabledPanel).Visible = false;
				((Control)_window.DisabledPanel).MouseFilter = (MouseFilterMode)2;
				return;
			}
			((Control)_window.DisabledPanel).Visible = true;
			FormattedMessage val = new FormattedMessage();
			val.AddMarkupOrThrow("[color=red][font size=16]They need to be lying down![/font][/color]");
			_window.DisabledLabel.SetMessage(val, (Color?)null);
			((Control)_window.DisabledPanel).MouseFilter = (MouseFilterMode)0;
		}
	}

	private void View(ViewType type)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		if (_window != null)
		{
			((Control)_window.PartsButton).Parent.Margin = new Thickness(0f, 0f, 0f, 10f);
			((Control)_window.Parts).Visible = type == ViewType.Parts;
			((BaseButton)_window.PartsButton).Disabled = type == ViewType.Parts;
			((Control)_window.Surgeries).Visible = type == ViewType.Surgeries;
			((BaseButton)_window.SurgeriesButton).Disabled = type != ViewType.Steps;
			((Control)_window.Steps).Visible = type == ViewType.Steps;
			((BaseButton)_window.StepsButton).Disabled = type != ViewType.Steps || _previousSurgeries.Count == 0;
			MetaDataComponent val = default(MetaDataComponent);
			MetaDataComponent val2 = default(MetaDataComponent);
			if (_entities.TryGetComponent<MetaDataComponent>(_part, ref val) && _entities.TryGetComponent<MetaDataComponent>(_surgery?.Ent, ref val2))
			{
				((DefaultWindow)_window).Title = "Surgery - " + val.EntityName + ", " + val2.EntityName;
			}
			else if (val != null)
			{
				((DefaultWindow)_window).Title = "Surgery - " + val.EntityName;
			}
			else
			{
				((DefaultWindow)_window).Title = "Surgery";
			}
		}
	}
}
