// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.Guidebook.GuidebookUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

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
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Configuration;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Client.UserInterface.Systems.Guidebook;

public sealed class GuidebookUIController : 
  UIController,
  IOnStateEntered<LobbyState>,
  IOnStateEntered<GameplayState>,
  IOnStateExited<LobbyState>,
  IOnStateExited<GameplayState>,
  IOnSystemChanged<GuidebookSystem>,
  IOnSystemLoaded<GuidebookSystem>,
  IOnSystemUnloaded<GuidebookSystem>
{
  [UISystemDependency]
  private readonly GuidebookSystem _guidebookSystem;
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private IConfigurationManager _configuration;
  private GuidebookWindow? _guideWindow;
  private ProtoId<GuideEntryPrototype>? _lastEntry;

  private MenuButton? GuidebookButton
  {
    get => this.UIManager.GetActiveUIWidgetOrNull<GameTopMenuBar>()?.GuidebookButton;
  }

  public void OnStateEntered(LobbyState state) => this.HandleStateEntered((Robust.Client.State.State) state);

  public void OnStateEntered(GameplayState state) => this.HandleStateEntered((Robust.Client.State.State) state);

  private void HandleStateEntered(Robust.Client.State.State state)
  {
    this._guideWindow = this.UIManager.CreateWindow<GuidebookWindow>();
    this._guideWindow.OnClose += new Action(this.OnWindowClosed);
    this._guideWindow.OnOpen += new Action(this.OnWindowOpen);
    // ISSUE: method pointer
    CommandBinds.Builder.Bind(ContentKeyFunctions.OpenGuidebook, InputCmdHandler.FromDelegate(new StateInputCmdDelegate((object) this, __methodptr(\u003CHandleStateEntered\u003Eb__9_0)), (StateInputCmdDelegate) null, true, true)).Register<GuidebookUIController>();
  }

  public void OnStateExited(LobbyState state) => this.HandleStateExited();

  public void OnStateExited(GameplayState state) => this.HandleStateExited();

  private void HandleStateExited()
  {
    if (this._guideWindow == null)
      return;
    this._guideWindow.OnClose -= new Action(this.OnWindowClosed);
    this._guideWindow.OnOpen -= new Action(this.OnWindowOpen);
    if (!((Control) this._guideWindow).Disposed)
      ((Control) this._guideWindow).Orphan();
    this._guideWindow = (GuidebookWindow) null;
    CommandBinds.Unregister<GuidebookUIController>();
  }

  public void OnSystemLoaded(GuidebookSystem system)
  {
    this._guidebookSystem.OnGuidebookOpen += new Action<List<ProtoId<GuideEntryPrototype>>, List<ProtoId<GuideEntryPrototype>>, ProtoId<GuideEntryPrototype>?, bool, ProtoId<GuideEntryPrototype>?>(this.OpenGuidebook);
  }

  public void OnSystemUnloaded(GuidebookSystem system)
  {
    this._guidebookSystem.OnGuidebookOpen -= new Action<List<ProtoId<GuideEntryPrototype>>, List<ProtoId<GuideEntryPrototype>>, ProtoId<GuideEntryPrototype>?, bool, ProtoId<GuideEntryPrototype>?>(this.OpenGuidebook);
  }

  internal void UnloadButton()
  {
    if (this.GuidebookButton == null)
      return;
    ((BaseButton) this.GuidebookButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.GuidebookButtonOnPressed);
  }

  internal void LoadButton()
  {
    if (this.GuidebookButton == null)
      return;
    ((BaseButton) this.GuidebookButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.GuidebookButtonOnPressed);
  }

  private void GuidebookButtonOnPressed(BaseButton.ButtonEventArgs obj) => this.ToggleGuidebook();

  public void ToggleGuidebook()
  {
    if (this._guideWindow == null)
      return;
    if (this._guideWindow.IsOpen)
    {
      this.UIManager.ClickSound();
      this._guideWindow.Close();
    }
    else
      this.OpenGuidebook();
  }

  private void OnWindowClosed()
  {
    if (this.GuidebookButton != null)
      ((BaseButton) this.GuidebookButton).Pressed = false;
    if (this._guideWindow == null)
      return;
    ((Control) this._guideWindow.ReturnContainer).Visible = false;
    this._lastEntry = new ProtoId<GuideEntryPrototype>?(this._guideWindow.LastEntry);
  }

  private void OnWindowOpen()
  {
    if (this.GuidebookButton == null)
      return;
    ((BaseButton) this.GuidebookButton).Pressed = true;
  }

  public void OpenGuidebook(
    Dictionary<ProtoId<GuideEntryPrototype>, GuideEntry>? guides = null,
    List<ProtoId<GuideEntryPrototype>>? rootEntries = null,
    ProtoId<GuideEntryPrototype>? forceRoot = null,
    bool includeChildren = true,
    ProtoId<GuideEntryPrototype>? selected = null)
  {
    if (this._guideWindow == null)
      return;
    if (this.GuidebookButton != null)
      ((BaseButton) this.GuidebookButton).SetClickPressed(!this._guideWindow.IsOpen);
    if (guides == null)
      guides = this._prototypeManager.EnumerateCM<GuideEntryPrototype>().ToDictionary<GuideEntryPrototype, ProtoId<GuideEntryPrototype>, GuideEntry>((Func<GuideEntryPrototype, ProtoId<GuideEntryPrototype>>) (x => new ProtoId<GuideEntryPrototype>(x.ID)), (Func<GuideEntryPrototype, GuideEntry>) (x => (GuideEntry) x));
    else if (includeChildren)
    {
      Dictionary<ProtoId<GuideEntryPrototype>, GuideEntry> dictionary = guides;
      guides = new Dictionary<ProtoId<GuideEntryPrototype>, GuideEntry>((IDictionary<ProtoId<GuideEntryPrototype>, GuideEntry>) dictionary);
      foreach (GuideEntry guide in dictionary.Values)
        this.RecursivelyAddChildren(guide, guides);
    }
    if (!selected.HasValue)
    {
      ProtoId<GuideEntryPrototype>? lastEntry = this._lastEntry;
      if (lastEntry.HasValue)
      {
        ProtoId<GuideEntryPrototype> valueOrDefault = lastEntry.GetValueOrDefault();
        if (guides.ContainsKey(valueOrDefault))
        {
          selected = this._lastEntry;
          goto label_17;
        }
      }
      selected = ProtoId<GuideEntryPrototype>.op_Implicit(this._configuration.GetCVar<string>(CCVars.DefaultGuide));
    }
label_17:
    this._guideWindow.UpdateGuides(guides, rootEntries, forceRoot, selected);
    this._guideWindow.Tree.SetAllExpanded(false);
    this._guideWindow.Tree.SetAllExpanded(true, 1);
    this._guideWindow.OpenCenteredRight();
  }

  public void OpenGuidebook(
    List<ProtoId<GuideEntryPrototype>> guideList,
    List<ProtoId<GuideEntryPrototype>>? rootEntries = null,
    ProtoId<GuideEntryPrototype>? forceRoot = null,
    bool includeChildren = true,
    ProtoId<GuideEntryPrototype>? selected = null)
  {
    Dictionary<ProtoId<GuideEntryPrototype>, GuideEntry> guides = new Dictionary<ProtoId<GuideEntryPrototype>, GuideEntry>();
    foreach (ProtoId<GuideEntryPrototype> guide in guideList)
    {
      GuideEntryPrototype guideEntryPrototype;
      if (!this._prototypeManager.TryIndex<GuideEntryPrototype>(guide, ref guideEntryPrototype))
        this.Log.Error($"Encountered unknown guide prototype: {guide}");
      else
        guides.Add(guide, (GuideEntry) guideEntryPrototype);
    }
    this.OpenGuidebook(guides, rootEntries, forceRoot, includeChildren, selected);
  }

  public void CloseGuidebook()
  {
    if (this._guideWindow == null || !this._guideWindow.IsOpen)
      return;
    this.UIManager.ClickSound();
    this._guideWindow.Close();
  }

  private void RecursivelyAddChildren(
    GuideEntry guide,
    Dictionary<ProtoId<GuideEntryPrototype>, GuideEntry> guides)
  {
    foreach (ProtoId<GuideEntryPrototype> child in guide.Children)
    {
      if (!guides.ContainsKey(child))
      {
        GuideEntryPrototype guide1;
        if (!this._prototypeManager.TryIndex<GuideEntryPrototype>(child, ref guide1))
        {
          this.Log.Error($"Encountered unknown guide prototype: {child} as a child of {guide.Id}. If the child is not a prototype, it must be directly provided.");
        }
        else
        {
          guides.Add(child, (GuideEntry) guide1);
          this.RecursivelyAddChildren((GuideEntry) guide1, guides);
        }
      }
    }
  }
}
