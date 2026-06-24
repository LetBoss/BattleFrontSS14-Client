// Decompiled with JetBrains decompiler
// Type: Content.Client.Verbs.UI.VerbMenuUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.CombatMode;
using Content.Client.ContextMenu.UI;
using Content.Client.Gameplay;
using Content.Client.Mapping;
using Content.Shared.Input;
using Content.Shared.Verbs;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.Collections;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Client.Verbs.UI;

public sealed class VerbMenuUIController : 
  UIController,
  IOnStateEntered<GameplayState>,
  IOnStateExited<GameplayState>,
  IOnStateEntered<MappingState>,
  IOnStateExited<MappingState>
{
  [Dependency]
  private IPlayerManager _playerManager;
  [Dependency]
  private ContextMenuUIController _context;
  [UISystemDependency]
  private readonly CombatModeSystem _combatMode;
  [UISystemDependency]
  private readonly VerbSystem _verbSystem;
  public NetEntity CurrentTarget;
  public SortedSet<Verb> CurrentVerbs = new SortedSet<Verb>();
  public List<VerbCategory> ExtraCategories = new List<VerbCategory>();
  public ContextMenuPopup? OpenMenu;

  public void OnStateEntered(GameplayState state)
  {
    this._context.OnContextKeyEvent += new Action<ContextMenuElement, GUIBoundKeyEventArgs>(this.OnKeyBindDown);
    this._context.OnContextClosed += new Action(this.Close);
    this._verbSystem.OnVerbsResponse += new Action<VerbsResponseEvent>(this.HandleVerbsResponse);
  }

  public void OnStateExited(GameplayState state)
  {
    this._context.OnContextKeyEvent -= new Action<ContextMenuElement, GUIBoundKeyEventArgs>(this.OnKeyBindDown);
    this._context.OnContextClosed -= new Action(this.Close);
    if (this._verbSystem != null)
      this._verbSystem.OnVerbsResponse -= new Action<VerbsResponseEvent>(this.HandleVerbsResponse);
    this.Close();
  }

  public void OnStateEntered(MappingState state)
  {
    this._context.OnContextKeyEvent += new Action<ContextMenuElement, GUIBoundKeyEventArgs>(this.OnKeyBindDown);
    this._context.OnContextClosed += new Action(this.Close);
    this._verbSystem.OnVerbsResponse += new Action<VerbsResponseEvent>(this.HandleVerbsResponse);
  }

  public void OnStateExited(MappingState state)
  {
    this._context.OnContextKeyEvent -= new Action<ContextMenuElement, GUIBoundKeyEventArgs>(this.OnKeyBindDown);
    this._context.OnContextClosed -= new Action(this.Close);
    if (this._verbSystem != null)
      this._verbSystem.OnVerbsResponse -= new Action<VerbsResponseEvent>(this.HandleVerbsResponse);
    this.Close();
  }

  public void OpenVerbMenu(EntityUid target, bool force = false, ContextMenuPopup? popup = null)
  {
    this.OpenVerbMenu(this.EntityManager.GetNetEntity(target, (MetaDataComponent) null), force, popup);
  }

  public void OpenVerbMenu(NetEntity target, bool force = false, ContextMenuPopup? popup = null)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    if (!localEntity.HasValue)
      return;
    EntityUid valueOrDefault = localEntity.GetValueOrDefault();
    if (!((EntityUid) ref valueOrDefault).Valid || !force && this._combatMode.IsInCombatMode(new EntityUid?(valueOrDefault)))
      return;
    this.Close();
    ContextMenuPopup popup1 = popup ?? this._context.RootMenu;
    ((Control) popup1.MenuBody).DisposeAllChildren();
    this.CurrentTarget = target;
    this.CurrentVerbs = this._verbSystem.GetVerbs(target, valueOrDefault, Verb.VerbTypes, out this.ExtraCategories, force);
    this.OpenMenu = popup1;
    this.FillVerbPopup(popup1);
    if (popup != null)
      return;
    ((Control) popup1).SetPositionLast();
    UIBox2 uiBox2 = UIBox2.FromDimensions(this.UIManager.MousePositionScaled.Position, new Vector2(1f, 1f));
    popup1.Open(new UIBox2?(uiBox2), new Vector2?(), new Vector2?());
  }

  private void FillVerbPopup(ContextMenuPopup popup)
  {
    HashSet<string> stringSet = new HashSet<string>();
    ValueList<string> valueList = new ValueList<string>(this.ExtraCategories.Count);
    foreach (VerbCategory extraCategory in this.ExtraCategories)
      valueList.Add(extraCategory.Text);
    foreach (Verb currentVerb in this.CurrentVerbs)
    {
      if (currentVerb.Category == null)
      {
        VerbMenuElement element = new VerbMenuElement(currentVerb);
        this._context.AddElement(popup, (ContextMenuElement) element);
      }
      else if (!valueList.Contains(currentVerb.Category.Text) && stringSet.Add(currentVerb.Category.Text))
        this.AddVerbCategory(currentVerb.Category, popup);
    }
    foreach (VerbCategory extraCategory in this.ExtraCategories)
    {
      if (stringSet.Add(extraCategory.Text))
        this.AddVerbCategory(extraCategory, popup);
    }
    ((Control) popup).InvalidateMeasure();
  }

  public void AddVerbCategory(VerbCategory category, ContextMenuPopup popup)
  {
    List<Verb> source = new List<Verb>();
    bool flag = false;
    foreach (Verb currentVerb in this.CurrentVerbs)
    {
      if (currentVerb.Category?.Text == category.Text)
      {
        source.Add(currentVerb);
        flag = flag || currentVerb.Icon != null || currentVerb.IconEntity.HasValue;
      }
    }
    if (source.Count == 0 && !this.ExtraCategories.Contains(category))
      return;
    string styleClass = source.FirstOrDefault<Verb>()?.TextStyleClass ?? Verb.DefaultTextStyleClass;
    VerbMenuElement verbMenuElement = new VerbMenuElement(category, styleClass);
    this._context.AddElement(popup, (ContextMenuElement) verbMenuElement);
    verbMenuElement.SubMenu = new ContextMenuPopup(this._context, (ContextMenuElement) verbMenuElement);
    foreach (Verb verb in source)
    {
      VerbMenuElement element = new VerbMenuElement(verb)
      {
        IconVisible = flag,
        TextVisible = !category.IconsOnly
      };
      this._context.AddElement(verbMenuElement.SubMenu, (ContextMenuElement) element);
    }
    verbMenuElement.SubMenu.MenuBody.Columns = category.Columns;
  }

  public void AddServerVerbs(List<Verb>? verbs, ContextMenuPopup popup)
  {
    ((Control) popup.MenuBody).DisposeAllChildren();
    if (verbs == null)
    {
      this._context.AddElement(popup, new ContextMenuElement(Loc.GetString("verb-system-null-server-response")));
    }
    else
    {
      this.CurrentVerbs.UnionWith((IEnumerable<Verb>) verbs);
      this.FillVerbPopup(popup);
    }
  }

  public void OnKeyBindDown(ContextMenuElement element, GUIBoundKeyEventArgs args)
  {
    if (BoundKeyFunction.op_Inequality(((BoundKeyEventArgs) args).Function, EngineKeyFunctions.Use) && BoundKeyFunction.op_Inequality(((BoundKeyEventArgs) args).Function, ContentKeyFunctions.ActivateItemInWorld))
      return;
    switch (element)
    {
      case VerbMenuElement verbMenuElement1:
        ((BoundKeyEventArgs) args).Handle();
        Verb verb = verbMenuElement1.Verb;
        if (verb == null)
        {
          if (verbMenuElement1.SubMenu == null || ((Control) verbMenuElement1.SubMenu).ChildCount == 0)
            break;
          if (((Control) verbMenuElement1.SubMenu.MenuBody).ChildCount != 1 || !(((IEnumerable<Control>) ((Control) verbMenuElement1.SubMenu.MenuBody).Children).First<Control>() is VerbMenuElement verbMenuElement))
          {
            this._context.OpenSubMenu((ContextMenuElement) verbMenuElement1);
            break;
          }
          verb = verbMenuElement.Verb;
          if (verb == null)
            break;
        }
        if (!verb.ConfirmationPopup)
        {
          this.ExecuteVerb(verb);
          break;
        }
        if (verbMenuElement1.SubMenu == null)
        {
          ConfirmationMenuElement element1 = new ConfirmationMenuElement(verb, "Confirm");
          verbMenuElement1.SubMenu = new ContextMenuPopup(this._context, (ContextMenuElement) verbMenuElement1);
          this._context.AddElement(verbMenuElement1.SubMenu, (ContextMenuElement) element1);
        }
        this._context.OpenSubMenu((ContextMenuElement) verbMenuElement1);
        break;
      case ConfirmationMenuElement confirmationMenuElement:
        ((BoundKeyEventArgs) args).Handle();
        this.ExecuteVerb(confirmationMenuElement.Verb);
        break;
    }
  }

  private void Close()
  {
    if (this.OpenMenu == null)
      return;
    this.OpenMenu.Close();
    this.OpenMenu = (ContextMenuPopup) null;
  }

  private void HandleVerbsResponse(VerbsResponseEvent msg)
  {
    if (this.OpenMenu == null || !((Control) this.OpenMenu).Visible || NetEntity.op_Inequality(this.CurrentTarget, msg.Entity))
      return;
    this.AddServerVerbs(msg.Verbs, this.OpenMenu);
  }

  private void ExecuteVerb(Verb verb)
  {
    this.UIManager.ClickSound();
    this._verbSystem.ExecuteVerb(this.CurrentTarget, verb);
    if (((int) verb.CloseMenu ?? (verb.CloseMenuDefault ? 1 : 0)) == 0)
      return;
    this._context.Close();
  }
}
