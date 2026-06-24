using System;
using System.Collections.Generic;
using System.Linq;
using Content.Client.Gameplay;
using Content.Client.Guidebook;
using Content.Client.Guidebook.Controls;
using Content.Client.Lobby;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Systems.MenuBar.Widgets;
using Content.Shared._RMC14.Prototypes;
using Content.Shared.CCVar;
using Content.Shared.Guidebook;
using Content.Shared.Input;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Configuration;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Client.UserInterface.Systems.Guidebook;

public sealed class GuidebookUIController : UIController, IOnStateEntered<LobbyState>, IOnStateEntered<GameplayState>, IOnStateExited<LobbyState>, IOnStateExited<GameplayState>, IOnSystemChanged<GuidebookSystem>, IOnSystemLoaded<GuidebookSystem>, IOnSystemUnloaded<GuidebookSystem>
{
	[UISystemDependency]
	private readonly GuidebookSystem _guidebookSystem;

	[Dependency]
	private IPrototypeManager _prototypeManager;

	[Dependency]
	private IConfigurationManager _configuration;

	private GuidebookWindow? _guideWindow;

	private ProtoId<GuideEntryPrototype>? _lastEntry;

	private MenuButton? GuidebookButton => base.UIManager.GetActiveUIWidgetOrNull<GameTopMenuBar>()?.GuidebookButton;

	public void OnStateEntered(LobbyState state)
	{
		HandleStateEntered((State)(object)state);
	}

	public void OnStateEntered(GameplayState state)
	{
		HandleStateEntered((State)(object)state);
	}

	private void HandleStateEntered(State state)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected O, but got Unknown
		_guideWindow = base.UIManager.CreateWindow<GuidebookWindow>();
		((BaseWindow)_guideWindow).OnClose += OnWindowClosed;
		((BaseWindow)_guideWindow).OnOpen += OnWindowOpen;
		CommandBinds.Builder.Bind(ContentKeyFunctions.OpenGuidebook, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate
		{
			ToggleGuidebook();
		}, (StateInputCmdDelegate)null, true, true)).Register<GuidebookUIController>();
	}

	public void OnStateExited(LobbyState state)
	{
		HandleStateExited();
	}

	public void OnStateExited(GameplayState state)
	{
		HandleStateExited();
	}

	private void HandleStateExited()
	{
		if (_guideWindow != null)
		{
			((BaseWindow)_guideWindow).OnClose -= OnWindowClosed;
			((BaseWindow)_guideWindow).OnOpen -= OnWindowOpen;
			if (!((Control)_guideWindow).Disposed)
			{
				((Control)_guideWindow).Orphan();
			}
			_guideWindow = null;
			CommandBinds.Unregister<GuidebookUIController>();
		}
	}

	public void OnSystemLoaded(GuidebookSystem system)
	{
		_guidebookSystem.OnGuidebookOpen += OpenGuidebook;
	}

	public void OnSystemUnloaded(GuidebookSystem system)
	{
		_guidebookSystem.OnGuidebookOpen -= OpenGuidebook;
	}

	internal void UnloadButton()
	{
		if (GuidebookButton != null)
		{
			((BaseButton)GuidebookButton).OnPressed -= GuidebookButtonOnPressed;
		}
	}

	internal void LoadButton()
	{
		if (GuidebookButton != null)
		{
			((BaseButton)GuidebookButton).OnPressed += GuidebookButtonOnPressed;
		}
	}

	private void GuidebookButtonOnPressed(ButtonEventArgs obj)
	{
		ToggleGuidebook();
	}

	public void ToggleGuidebook()
	{
		if (_guideWindow != null)
		{
			if (((BaseWindow)_guideWindow).IsOpen)
			{
				base.UIManager.ClickSound();
				((BaseWindow)_guideWindow).Close();
			}
			else
			{
				OpenGuidebook();
			}
		}
	}

	private void OnWindowClosed()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if (GuidebookButton != null)
		{
			((BaseButton)GuidebookButton).Pressed = false;
		}
		if (_guideWindow != null)
		{
			((Control)_guideWindow.ReturnContainer).Visible = false;
			_lastEntry = _guideWindow.LastEntry;
		}
	}

	private void OnWindowOpen()
	{
		if (GuidebookButton != null)
		{
			((BaseButton)GuidebookButton).Pressed = true;
		}
	}

	public void OpenGuidebook(Dictionary<ProtoId<GuideEntryPrototype>, GuideEntry>? guides = null, List<ProtoId<GuideEntryPrototype>>? rootEntries = null, ProtoId<GuideEntryPrototype>? forceRoot = null, bool includeChildren = true, ProtoId<GuideEntryPrototype>? selected = null)
	{
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		if (_guideWindow == null)
		{
			return;
		}
		if (GuidebookButton != null)
		{
			((BaseButton)GuidebookButton).SetClickPressed(!((BaseWindow)_guideWindow).IsOpen);
		}
		if (guides == null)
		{
			guides = _prototypeManager.EnumerateCM<GuideEntryPrototype>().ToDictionary((Func<GuideEntryPrototype, ProtoId<GuideEntryPrototype>>)((GuideEntryPrototype x) => new ProtoId<GuideEntryPrototype>(x.ID)), (Func<GuideEntryPrototype, GuideEntry>)((GuideEntryPrototype x) => x));
		}
		else if (includeChildren)
		{
			Dictionary<ProtoId<GuideEntryPrototype>, GuideEntry>? dictionary = guides;
			guides = new Dictionary<ProtoId<GuideEntryPrototype>, GuideEntry>(dictionary);
			foreach (GuideEntry value in dictionary.Values)
			{
				RecursivelyAddChildren(value, guides);
			}
		}
		if (!selected.HasValue)
		{
			ProtoId<GuideEntryPrototype>? lastEntry = _lastEntry;
			if (lastEntry.HasValue)
			{
				ProtoId<GuideEntryPrototype> valueOrDefault = lastEntry.GetValueOrDefault();
				if (guides.ContainsKey(valueOrDefault))
				{
					selected = _lastEntry;
					goto IL_010d;
				}
			}
			selected = ProtoId<GuideEntryPrototype>.op_Implicit(_configuration.GetCVar<string>(CCVars.DefaultGuide));
		}
		goto IL_010d;
		IL_010d:
		_guideWindow.UpdateGuides(guides, rootEntries, forceRoot, selected);
		_guideWindow.Tree.SetAllExpanded(value: false);
		_guideWindow.Tree.SetAllExpanded(value: true, 1);
		((BaseWindow)_guideWindow).OpenCenteredRight();
	}

	public void OpenGuidebook(List<ProtoId<GuideEntryPrototype>> guideList, List<ProtoId<GuideEntryPrototype>>? rootEntries = null, ProtoId<GuideEntryPrototype>? forceRoot = null, bool includeChildren = true, ProtoId<GuideEntryPrototype>? selected = null)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<ProtoId<GuideEntryPrototype>, GuideEntry> dictionary = new Dictionary<ProtoId<GuideEntryPrototype>, GuideEntry>();
		GuideEntryPrototype value = default(GuideEntryPrototype);
		foreach (ProtoId<GuideEntryPrototype> guide in guideList)
		{
			if (!_prototypeManager.TryIndex<GuideEntryPrototype>(guide, ref value))
			{
				((UIController)this).Log.Error($"Encountered unknown guide prototype: {guide}");
			}
			else
			{
				dictionary.Add(guide, value);
			}
		}
		OpenGuidebook(dictionary, rootEntries, forceRoot, includeChildren, selected);
	}

	public void CloseGuidebook()
	{
		if (_guideWindow != null && ((BaseWindow)_guideWindow).IsOpen)
		{
			base.UIManager.ClickSound();
			((BaseWindow)_guideWindow).Close();
		}
	}

	private void RecursivelyAddChildren(GuideEntry guide, Dictionary<ProtoId<GuideEntryPrototype>, GuideEntry> guides)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		GuideEntryPrototype guideEntryPrototype = default(GuideEntryPrototype);
		foreach (ProtoId<GuideEntryPrototype> child in guide.Children)
		{
			if (!guides.ContainsKey(child))
			{
				if (!_prototypeManager.TryIndex<GuideEntryPrototype>(child, ref guideEntryPrototype))
				{
					((UIController)this).Log.Error($"Encountered unknown guide prototype: {child} as a child of {guide.Id}. If the child is not a prototype, it must be directly provided.");
				}
				else
				{
					guides.Add(child, guideEntryPrototype);
					RecursivelyAddChildren(guideEntryPrototype, guides);
				}
			}
		}
	}
}
