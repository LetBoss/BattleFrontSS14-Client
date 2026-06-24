// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.MenuBar.GameTopMenuBarUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

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
using System;

#nullable enable
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

  private GameTopMenuBar? GameTopMenuBar
  {
    get => this.UIManager.GetActiveUIWidgetOrNull<GameTopMenuBar>();
  }

  public virtual void Initialize()
  {
    base.Initialize();
    GameplayStateLoadController uiController = this.UIManager.GetUIController<GameplayStateLoadController>();
    uiController.OnScreenLoad += new Action(this.LoadButtons);
    uiController.OnScreenUnload += new Action(this.UnloadButtons);
  }

  public void UnloadButtons()
  {
    this._escape.UnloadButton();
    this._guidebook.UnloadButton();
    this._admin.UnloadButton();
    this._character.UnloadButton();
    this._crafting.UnloadButton();
    this._ahelp.UnloadButton();
    this._action.UnloadButton();
    this._sandbox.UnloadButton();
    this._emotes.UnloadButton();
  }

  public void LoadButtons()
  {
    this._escape.LoadButton();
    this._guidebook.LoadButton();
    this._admin.LoadButton();
    this._character.LoadButton();
    this._crafting.LoadButton();
    this._ahelp.LoadButton();
    this._action.LoadButton();
    this._sandbox.LoadButton();
    this._emotes.LoadButton();
  }
}
