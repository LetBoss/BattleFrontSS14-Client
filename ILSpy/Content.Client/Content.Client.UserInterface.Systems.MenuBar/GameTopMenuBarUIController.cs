using System;
using Content.Client.UserInterface.Systems.Actions;
using Content.Client.UserInterface.Systems.Admin;
using Content.Client.UserInterface.Systems.Bwoink;
using Content.Client.UserInterface.Systems.Character;
using Content.Client.UserInterface.Systems.Crafting;
using Content.Client.UserInterface.Systems.Emotes;
using Content.Client.UserInterface.Systems.EscapeMenu;
using Content.Client.UserInterface.Systems.Gameplay;
using Content.Client.UserInterface.Systems.Guidebook;
using Content.Client.UserInterface.Systems.MenuBar.Widgets;
using Content.Client.UserInterface.Systems.Sandbox;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.IoC;

namespace Content.Client.UserInterface.Systems.MenuBar;

public sealed class GameTopMenuBarUIController : UIController
{
	[Dependency]
	private EscapeUIController _escape;

	[Dependency]
	private AdminUIController _admin;

	[Dependency]
	private CharacterUIController _character;

	[Dependency]
	private CraftingUIController _crafting;

	[Dependency]
	private AHelpUIController _ahelp;

	[Dependency]
	private ActionUIController _action;

	[Dependency]
	private SandboxUIController _sandbox;

	[Dependency]
	private GuidebookUIController _guidebook;

	[Dependency]
	private EmotesUIController _emotes;

	private GameTopMenuBar? GameTopMenuBar => base.UIManager.GetActiveUIWidgetOrNull<GameTopMenuBar>();

	public override void Initialize()
	{
		((UIController)this).Initialize();
		GameplayStateLoadController uIController = base.UIManager.GetUIController<GameplayStateLoadController>();
		uIController.OnScreenLoad = (Action)Delegate.Combine(uIController.OnScreenLoad, new Action(LoadButtons));
		uIController.OnScreenUnload = (Action)Delegate.Combine(uIController.OnScreenUnload, new Action(UnloadButtons));
	}

	public void UnloadButtons()
	{
		_escape.UnloadButton();
		_guidebook.UnloadButton();
		_admin.UnloadButton();
		_character.UnloadButton();
		_crafting.UnloadButton();
		_ahelp.UnloadButton();
		_action.UnloadButton();
		_sandbox.UnloadButton();
		_emotes.UnloadButton();
	}

	public void LoadButtons()
	{
		_escape.LoadButton();
		_guidebook.LoadButton();
		_admin.LoadButton();
		_character.LoadButton();
		_crafting.LoadButton();
		_ahelp.LoadButton();
		_action.LoadButton();
		_sandbox.LoadButton();
		_emotes.LoadButton();
	}
}
