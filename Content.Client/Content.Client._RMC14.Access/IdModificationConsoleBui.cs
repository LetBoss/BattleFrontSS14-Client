using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared._RMC14.Marines.Access;
using Content.Shared._RMC14.UserInterface;
using Content.Shared.Access;
using Content.Shared.Access.Components;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client._RMC14.Access;

public sealed class IdModificationConsoleBui : BoundUserInterface, IRefreshableBui
{
	[Dependency]
	private IPrototypeManager _prototype;

	private readonly ContainerSystem _container;

	private IdModificationConsoleWindow? _window;

	private readonly HashSet<IdModificationConsoleAccessButton> _accessButtons = new HashSet<IdModificationConsoleAccessButton>();

	private readonly HashSet<IdModificationConsoleAccessGroupButton> _accessGroupButtons = new HashSet<IdModificationConsoleAccessGroupButton>();

	private string _currentAccessGroup = "";

	private readonly HashSet<IdModificationConsoleAccessGroupButton> _jobGroupButtons = new HashSet<IdModificationConsoleAccessGroupButton>();

	private readonly HashSet<IdModificationConsoleAccessButton> _jobButtons = new HashSet<IdModificationConsoleAccessButton>();

	private string _currentJobGroup = "";

	private readonly HashSet<IdModificationConsoleTabButton> tabs = new HashSet<IdModificationConsoleTabButton>();

	private string _currenttab = "";

	public IdModificationConsoleBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		_container = base.EntMan.System<ContainerSystem>();
	}

	public void Refresh()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		IdModificationConsoleWindow window = _window;
		IdModificationConsoleComponent idModificationConsoleComponent = default(IdModificationConsoleComponent);
		if (window == null || !((BaseWindow)window).IsOpen || !base.EntMan.TryGetComponent<IdModificationConsoleComponent>(((BoundUserInterface)this).Owner, ref idModificationConsoleComponent))
		{
			return;
		}
		TryContainerEntity(((BoundUserInterface)this).Owner, idModificationConsoleComponent.TargetIdSlot, out var contained);
		TryContainerEntity(((BoundUserInterface)this).Owner, idModificationConsoleComponent.PrivilegedIdSlot, out var contained2);
		MetaDataComponent val = default(MetaDataComponent);
		base.EntMan.TryGetComponent<MetaDataComponent>(contained, ref val);
		IdCardComponent idCardComponent = default(IdCardComponent);
		base.EntMan.TryGetComponent<IdCardComponent>(contained, ref idCardComponent);
		AccessComponent accessComponent = default(AccessComponent);
		base.EntMan.TryGetComponent<AccessComponent>(contained, ref accessComponent);
		if (idModificationConsoleComponent.Authenticated)
		{
			_window.SignInButton.Text = "Log Out";
		}
		else if (contained2.HasValue)
		{
			_window.SignInButton.Text = "Eject Card";
		}
		else
		{
			_window.SignInButton.Text = "Sign In";
		}
		if (contained.HasValue)
		{
			string text = ((val != null) ? val.EntityName : null) ?? "Unknown Name";
			string text2 = idCardComponent?.FullName ?? "Unknown Name";
			_window.SignInTargetButton.Text = "Eject Card: " + text;
			_window.SignInTargetAccount.Text = text2 + "'s Account Number:";
			_window.SignInTargetName.Text = text2;
		}
		else
		{
			_window.SignInTargetButton.Text = "Insert Id To Modify";
			_window.SignInTargetAccount.Text = "No Card Inserted";
			_window.SignInTargetName.Text = string.Empty;
		}
		foreach (IdModificationConsoleTabButton tab in tabs)
		{
			if (tab.TabButton.Text != _currenttab)
			{
				((BaseButton)tab.TabButton).Disabled = false;
			}
		}
		if (idModificationConsoleComponent.Authenticated && idCardComponent != null && accessComponent != null)
		{
			((Control)_window.TabsContainer).Visible = true;
			((Control)_window.AccessContainer).Visible = false;
			((Control)_window.JobContainer).Visible = false;
			string currenttab = _currenttab;
			if (!(currenttab == "Access"))
			{
				if (!(currenttab == "Jobs"))
				{
					_ = currenttab == "Ranks";
					return;
				}
				((Control)_window.JobContainer).Visible = true;
				jobGroupButtonRefresh();
			}
			else
			{
				((Control)_window.AccessContainer).Visible = true;
				AccessGroupRefresh(idModificationConsoleComponent, accessComponent);
				AccessButtonRefresh(accessComponent);
				RefreshIFFButton(idModificationConsoleComponent);
			}
		}
		else
		{
			((Control)_window.TabsContainer).Visible = false;
			((Control)_window.AccessContainer).Visible = false;
			((Control)_window.JobContainer).Visible = false;
		}
	}

	protected override void Open()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<IdModificationConsoleWindow>((BoundUserInterface)(object)this);
		IdModificationConsoleComponent console = default(IdModificationConsoleComponent);
		if (!base.EntMan.TryGetComponent<IdModificationConsoleComponent>(((BoundUserInterface)this).Owner, ref console))
		{
			return;
		}
		TryContainerEntity(((BoundUserInterface)this).Owner, console.TargetIdSlot, out var contained);
		MetaDataComponent val = default(MetaDataComponent);
		base.EntMan.TryGetComponent<MetaDataComponent>(contained, ref val);
		IdCardComponent idCardComponent = default(IdCardComponent);
		base.EntMan.TryGetComponent<IdCardComponent>(contained, ref idCardComponent);
		AccessComponent accessComponent = default(AccessComponent);
		base.EntMan.TryGetComponent<AccessComponent>(contained, ref accessComponent);
		((BaseButton)_window.SignInButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new IdModificationConsoleSignInBuiMsg());
		};
		((BaseButton)_window.SignInTargetButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new IdModificationConsoleSignInTargetBuiMsg());
		};
		IdModificationConsoleTabButton tab1 = new IdModificationConsoleTabButton();
		tab1.TabButton.Text = "Access";
		((BaseButton)tab1.TabButton).Disabled = true;
		((BaseButton)tab1.TabButton).OnPressed += delegate
		{
			((Control)_window.AccessGroups).RemoveAllChildren();
			_currentAccessGroup = "";
			((Control)_window.Accesses).RemoveAllChildren();
			_currenttab = "Access";
			((BaseButton)tab1.TabButton).Disabled = true;
			foreach (IdModificationConsoleAccessGroupButton accessGroupButton in _accessGroupButtons)
			{
				((Control)_window.AccessGroups).AddChild((Control)(object)accessGroupButton);
			}
			((Control)_window.GrantAllButton).Visible = false;
			((Control)_window.RevokeAllButton).Visible = false;
			Refresh();
		};
		tabs.Add(tab1);
		((Control)_window.Tabs).AddChild((Control)(object)tab1);
		((BaseButton)_window.GrantAllButton).OnPressed += delegate
		{
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new IdModificationConsoleMultipleAccessChangeBuiMsg("GrantAll", _currentAccessGroup));
			foreach (IdModificationConsoleAccessButton accessButton in _accessButtons)
			{
				((Control)accessButton.AccessButton).ModulateSelfOverride = Color.Green;
			}
		};
		((BaseButton)_window.RevokeAllButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new IdModificationConsoleMultipleAccessChangeBuiMsg("RevokeAll", _currentAccessGroup));
			foreach (IdModificationConsoleAccessButton accessButton2 in _accessButtons)
			{
				((Control)accessButton2.AccessButton).ModulateSelfOverride = null;
			}
		};
		((BaseButton)_window.GrantAllGroupButton).OnPressed += delegate
		{
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new IdModificationConsoleMultipleAccessChangeBuiMsg("GrantAllGroup", _currentAccessGroup));
			foreach (IdModificationConsoleAccessButton accessButton3 in _accessButtons)
			{
				((Control)accessButton3.AccessButton).ModulateSelfOverride = Color.Green;
			}
		};
		((BaseButton)_window.RevokeAllGroupButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new IdModificationConsoleMultipleAccessChangeBuiMsg("RevokeAllGroup", _currentAccessGroup));
			foreach (IdModificationConsoleAccessButton accessButton4 in _accessButtons)
			{
				((Control)accessButton4.AccessButton).ModulateSelfOverride = null;
			}
		};
		((BaseButton)_window.IFF).OnPressed += delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new IdModificationConsoleIFFChangeBuiMsg(console.HasIFF));
			Refresh();
		};
		DisplayAccessGroups(console);
		IdModificationConsoleTabButton tab2 = new IdModificationConsoleTabButton();
		tab2.TabButton.Text = "Jobs";
		((BaseButton)tab2.TabButton).OnPressed += delegate
		{
			_currenttab = "Jobs";
			_currentJobGroup = "";
			((BaseButton)tab2.TabButton).Disabled = true;
			Refresh();
		};
		tabs.Add(tab2);
		((Control)_window.Tabs).AddChild((Control)(object)tab2);
		((BaseButton)_window.Terminate).OnPressed += delegate
		{
			((Control)_window.Terminate).Visible = false;
			((Control)_window.TerminateConfirm).Visible = true;
		};
		((BaseButton)_window.TerminateConfirm).OnPressed += delegate
		{
			_window.TerminateConfirm.Text = "ID Terminated";
			((BaseButton)_window.TerminateConfirm).Disabled = true;
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new IdModificationConsoleTerminateConfirmBuiMsg());
		};
		DisplayJobGroups(console);
		Refresh();
	}

	private void RefreshIFFButton(IdModificationConsoleComponent console)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		IdModificationConsoleWindow window = _window;
		if (window != null && ((BaseWindow)window).IsOpen)
		{
			if (console.HasIFF)
			{
				((Control)_window.IFF).ModulateSelfOverride = Color.Maroon;
				_window.IFF.Text = "Revoke IFF";
			}
			else
			{
				((Control)_window.IFF).ModulateSelfOverride = Color.Green;
				_window.IFF.Text = "Grant IFF";
			}
		}
	}

	private void DisplayAccessGroups(IdModificationConsoleComponent console)
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		IdModificationConsoleWindow window = _window;
		if (window == null || !((BaseWindow)window).IsOpen)
		{
			return;
		}
		((Control)_window.AccessGroups).RemoveAllChildren();
		List<ProtoId<AccessLevelPrototype>> list = console.AccessGroups.ToList();
		list.Sort();
		List<ProtoId<AccessLevelPrototype>> listAccess = console.AccessList.ToList();
		listAccess.Sort();
		AccessLevelPrototype accessGroupPrototype = default(AccessLevelPrototype);
		foreach (ProtoId<AccessLevelPrototype> item in list)
		{
			if (!_prototype.TryIndex<AccessLevelPrototype>(item, ref accessGroupPrototype))
			{
				continue;
			}
			IdModificationConsoleAccessGroupButton button = new IdModificationConsoleAccessGroupButton();
			button.Tag = accessGroupPrototype.AccessGroup;
			((Control)button.AccessButton).HorizontalExpand = true;
			((Control)button.AccessButton).SetHeight = 30f;
			((BaseButton)button.AccessButton).OnPressed += delegate
			{
				//IL_0026: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00db: Unknown result type (might be due to invalid IL or missing references)
				_currentAccessGroup = button.Tag;
				((Control)button.AccessButton).ModulateSelfOverride = Color.Green;
				((Control)_window.GrantAllButton).Visible = true;
				((Control)_window.RevokeAllButton).Visible = true;
				_accessButtons.Clear();
				((Control)_window.Accesses).RemoveAllChildren();
				AccessLevelPrototype accessLevelPrototype = default(AccessLevelPrototype);
				foreach (ProtoId<AccessLevelPrototype> access in listAccess)
				{
					if (_prototype.TryIndex<AccessLevelPrototype>(access, ref accessLevelPrototype) && !(accessLevelPrototype.AccessGroup != accessGroupPrototype.AccessGroup))
					{
						IdModificationConsoleAccessButton accessButton = new IdModificationConsoleAccessButton();
						if (accessLevelPrototype.Name != null)
						{
							accessButton.AccessButton.Text = Loc.GetString(accessLevelPrototype.Name);
							accessButton.Tag = accessLevelPrototype.Name;
						}
						((Control)accessButton.AccessButton).HorizontalExpand = true;
						((Control)accessButton.AccessButton).SetHeight = 30f;
						((BaseButton)accessButton.AccessButton).OnPressed += delegate
						{
							//IL_002c: Unknown result type (might be due to invalid IL or missing references)
							ToggleAccessButtonColor(accessButton);
							((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new IdModificationConsoleAccessChangeBuiMsg(access, ((Control)accessButton.AccessButton).ModulateSelfOverride.HasValue));
							Refresh();
						};
						_accessButtons.Add(accessButton);
						((Control)_window.Accesses).AddChild((Control)(object)accessButton);
						Refresh();
					}
				}
			};
			_accessGroupButtons.Add(button);
		}
	}

	private void DisplayJobGroups(IdModificationConsoleComponent console)
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		IdModificationConsoleWindow window = _window;
		if (window == null || !((BaseWindow)window).IsOpen)
		{
			return;
		}
		((Control)_window.AccessGroups).RemoveAllChildren();
		List<ProtoId<AccessGroupPrototype>> list = console.JobGroups.ToList();
		list.Sort();
		List<ProtoId<AccessGroupPrototype>> listJob = console.JobList.ToList();
		listJob.Sort();
		AccessGroupPrototype jobGroupPrototype = default(AccessGroupPrototype);
		foreach (ProtoId<AccessGroupPrototype> item in list)
		{
			if (!_prototype.TryIndex<AccessGroupPrototype>(item, ref jobGroupPrototype))
			{
				continue;
			}
			IdModificationConsoleAccessGroupButton button = new IdModificationConsoleAccessGroupButton();
			button.Tag = jobGroupPrototype.AccessGroup;
			button.AccessButton.Text = jobGroupPrototype.AccessGroup;
			((Control)button.AccessButton).HorizontalExpand = true;
			((Control)button.AccessButton).SetHeight = 30f;
			((BaseButton)button.AccessButton).OnPressed += delegate
			{
				//IL_0026: Unknown result type (might be due to invalid IL or missing references)
				//IL_007c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0081: Unknown result type (might be due to invalid IL or missing references)
				//IL_009f: Unknown result type (might be due to invalid IL or missing references)
				_currentJobGroup = button.Tag;
				((Control)button.AccessButton).ModulateSelfOverride = Color.Green;
				_jobButtons.Clear();
				((Control)_window.Jobs).RemoveAllChildren();
				AccessGroupPrototype jobPrototype = default(AccessGroupPrototype);
				foreach (ProtoId<AccessGroupPrototype> item2 in listJob)
				{
					if (_prototype.TryIndex<AccessGroupPrototype>(item2, ref jobPrototype) && !(jobPrototype.AccessGroup != jobGroupPrototype.AccessGroup))
					{
						IdModificationConsoleAccessButton jobButton = new IdModificationConsoleAccessButton();
						if (jobPrototype.Name != null)
						{
							jobButton.AccessButton.Text = Loc.GetString(jobPrototype.Name);
							jobButton.Tag = jobPrototype.Name;
						}
						((Control)jobButton.AccessButton).HorizontalExpand = true;
						((Control)jobButton.AccessButton).SetHeight = 30f;
						((BaseButton)jobButton.AccessButton).OnPressed += delegate
						{
							//IL_0053: Unknown result type (might be due to invalid IL or missing references)
							//IL_0078: Unknown result type (might be due to invalid IL or missing references)
							foreach (IdModificationConsoleAccessButton jobButton2 in _jobButtons)
							{
								((BaseButton)jobButton2.AccessButton).Disabled = true;
							}
							((Control)jobButton.AccessButton).ModulateSelfOverride = Color.Green;
							((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new IdModificationConsoleJobChangeBuiMsg(ProtoId<AccessGroupPrototype>.op_Implicit(jobPrototype)));
							Refresh();
						};
						_jobButtons.Add(jobButton);
						((Control)_window.Jobs).AddChild((Control)(object)jobButton);
						Refresh();
					}
				}
			};
			_jobGroupButtons.Add(button);
			((Control)_window.JobGroups).AddChild((Control)(object)button);
		}
	}

	private void jobGroupButtonRefresh()
	{
		foreach (IdModificationConsoleAccessGroupButton jobGroupButton in _jobGroupButtons)
		{
			if (jobGroupButton.Tag != _currentJobGroup)
			{
				((Control)jobGroupButton.AccessButton).ModulateSelfOverride = null;
			}
		}
	}

	private void AccessButtonRefresh(AccessComponent access)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		AccessLevelPrototype accessLevelPrototype = default(AccessLevelPrototype);
		foreach (IdModificationConsoleAccessButton accessButton in _accessButtons)
		{
			if (accessButton.AccessButton.Text == null)
			{
				break;
			}
			foreach (ProtoId<AccessLevelPrototype> tag in access.Tags)
			{
				if (_prototype.TryIndex<AccessLevelPrototype>(tag, ref accessLevelPrototype) && accessLevelPrototype.Name != null && !(accessButton.Tag != accessLevelPrototype.Name))
				{
					((Control)accessButton.AccessButton).ModulateSelfOverride = Color.Green;
					break;
				}
			}
		}
	}

	private void ToggleAccessButtonColor(IdModificationConsoleAccessButton accessButton)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		((Control)accessButton.AccessButton).ModulateSelfOverride = ((!((Control)accessButton.AccessButton).ModulateSelfOverride.HasValue) ? new Color?(Color.Green) : ((Color?)null));
	}

	private void AccessGroupRefresh(IdModificationConsoleComponent console, AccessComponent? targetCardAccessComponent)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		AccessLevelPrototype accessLevelPrototype = default(AccessLevelPrototype);
		foreach (IdModificationConsoleAccessGroupButton accessGroupButton in _accessGroupButtons)
		{
			if (targetCardAccessComponent != null)
			{
				string tag = accessGroupButton.Tag;
				int num = 0;
				int num2 = 0;
				foreach (ProtoId<AccessLevelPrototype> access in console.AccessList)
				{
					if (_prototype.TryIndex<AccessLevelPrototype>(access, ref accessLevelPrototype) && !(accessLevelPrototype.AccessGroup != accessGroupButton.Tag))
					{
						num++;
						if (targetCardAccessComponent.Tags.Contains(access))
						{
							num2++;
						}
					}
				}
				if (num2 >= num)
				{
					accessGroupButton.AccessButton.Text = "[ ◆ ] " + tag;
				}
				else if (num2 > 0)
				{
					accessGroupButton.AccessButton.Text = "[ ◈ ] " + tag;
				}
				else
				{
					accessGroupButton.AccessButton.Text = "[ ◇ ] " + tag;
				}
			}
			else
			{
				accessGroupButton.AccessButton.Text = accessGroupButton.Tag;
			}
			if (!(accessGroupButton.Tag == _currentAccessGroup))
			{
				((Control)accessGroupButton.AccessButton).ModulateSelfOverride = null;
				((BaseButton)accessGroupButton.AccessButton).Disabled = false;
			}
		}
	}

	private bool TryContainerEntity(EntityUid ent, string containerType, out EntityUid? contained)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		ContainerSlot val = ((SharedContainerSystem)_container).EnsureContainer<ContainerSlot>(ent, containerType, (ContainerManagerComponent)null);
		contained = val.ContainedEntity;
		return contained.HasValue;
	}
}
