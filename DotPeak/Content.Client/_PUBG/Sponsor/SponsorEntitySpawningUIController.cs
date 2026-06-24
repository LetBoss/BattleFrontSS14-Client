// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Sponsor.SponsorEntitySpawningUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Placement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Client._PUBG.Sponsor;

public sealed class SponsorEntitySpawningUIController : UIController
{
  [Dependency]
  private IConfigurationManager _cfg;
  [Dependency]
  private IPlacementManager _placement;
  [Dependency]
  private IPrototypeManager _prototypes;
  private EntitySpawnWindow? _window;
  private readonly List<EntityPrototype> _shownEntities = new List<EntityPrototype>();
  private bool _init;
  private bool _allowEraseEntities;
  private HashSet<string> _disallowedEntityIds = new HashSet<string>();
  private (int start, int end) _lastEntityIndices;

  public virtual void Initialize()
  {
    this._init = true;
    this._placement.DirectionChanged += new EventHandler(this.OnDirectionChanged);
    this._placement.PlacementChanged += new EventHandler(this.ClearSelection);
  }

  public void UpdatePermissions(SponsorSandboxState state)
  {
    this._allowEraseEntities = state.AllowEraseEntities;
    this._disallowedEntityIds = new HashSet<string>(state.DisallowedEntityIds.Select<string, string>((Func<string, string>) (id => id.ToLowerInvariant())));
    if (this._window == null || ((Control) this._window).Disposed)
      return;
    ((BaseButton) this._window.EraseButton).Disabled = !this._allowEraseEntities;
    if (!this._allowEraseEntities && ((BaseButton) this._window.EraseButton).Pressed)
    {
      ((BaseButton) this._window.EraseButton).Pressed = false;
      this._placement.Clear();
    }
    this.BuildEntityList(this._window.SearchBar.Text);
  }

  public void ToggleWindow()
  {
    this.EnsureWindow();
    if (((BaseWindow) this._window).IsOpen)
    {
      ((BaseWindow) this._window).Close();
    }
    else
    {
      ((BaseWindow) this._window).Open();
      this.UpdateEntityDirectionLabel();
      ((Control) this._window.SearchBar).GrabKeyboardFocus();
    }
  }

  public void CloseWindow()
  {
    if (this._window == null || ((Control) this._window).Disposed)
      return;
    ((BaseWindow) this._window).Close();
  }

  private void EnsureWindow()
  {
    EntitySpawnWindow window = this._window;
    if (window != null && !((Control) window).Disposed)
      return;
    this._window = this.UIManager.CreateWindow<EntitySpawnWindow>();
    LayoutContainer.SetAnchorPreset((Control) this._window, (LayoutContainer.LayoutPreset) 4, false);
    ((BaseWindow) this._window).OnClose += new Action(this.WindowClosed);
    ((BaseButton) this._window.ReplaceButton).Pressed = this._placement.Replacement;
    ((BaseButton) this._window.ReplaceButton).OnToggled += new Action<BaseButton.ButtonToggledEventArgs>(this.OnEntityReplaceToggled);
    ((BaseButton) this._window.EraseButton).Pressed = this._placement.Eraser;
    ((BaseButton) this._window.EraseButton).OnToggled += new Action<BaseButton.ButtonToggledEventArgs>(this.OnEntityEraseToggled);
    ((BaseButton) this._window.EraseButton).Disabled = !this._allowEraseEntities;
    this._window.OverrideMenu.OnItemSelected += new Action<OptionButton.ItemSelectedEventArgs>(this.OnEntityOverrideSelected);
    this._window.SearchBar.OnTextChanged += new Action<LineEdit.LineEditEventArgs>(this.OnEntitySearchChanged);
    ((BaseButton) this._window.ClearButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.OnEntityClearPressed);
    this._window.PrototypeScrollContainer.OnScrolled += new Action(this.UpdateVisiblePrototypes);
    ((Control) this._window).OnResized += new Action(this.UpdateVisiblePrototypes);
    this.BuildEntityList();
  }

  private void WindowClosed()
  {
    if (this._window == null || ((Control) this._window).Disposed)
      return;
    if (this._window.SelectedButton != null)
    {
      ((BaseButton) this._window.SelectedButton.ActualButton).Pressed = false;
      this._window.SelectedButton = (EntitySpawnButton) null;
    }
    this._placement.Clear();
  }

  private void OnEntityReplaceToggled(BaseButton.ButtonToggledEventArgs args)
  {
    if (this._window == null || ((Control) this._window).Disposed)
      return;
    if (args.Pressed)
    {
      IPlacementManager placement = this._placement;
      placement.Replacement = !placement.Replacement;
    }
    ((BaseButton.ButtonEventArgs) args).Button.Pressed = args.Pressed;
  }

  private void OnEntityEraseToggled(BaseButton.ButtonToggledEventArgs args)
  {
    if (this._window == null || ((Control) this._window).Disposed)
      return;
    if (!this._allowEraseEntities)
    {
      ((BaseButton.ButtonEventArgs) args).Button.Pressed = false;
    }
    else
    {
      this._placement.Clear();
      if (args.Pressed)
        this._placement.ToggleEraser();
      ((BaseButton.ButtonEventArgs) args).Button.Pressed = args.Pressed;
      ((BaseButton) this._window.OverrideMenu).Disabled = args.Pressed;
    }
  }

  private void ClearSelection(object? sender, EventArgs e)
  {
    if (this._window == null || ((Control) this._window).Disposed)
      return;
    if (this._window.SelectedButton != null)
    {
      ((BaseButton) this._window.SelectedButton.ActualButton).Pressed = false;
      this._window.SelectedButton = (EntitySpawnButton) null;
    }
    ((BaseButton) this._window.EraseButton).Pressed = false;
    ((BaseButton) this._window.OverrideMenu).Disabled = false;
  }

  private void OnEntityOverrideSelected(OptionButton.ItemSelectedEventArgs args)
  {
    if (this._window == null || ((Control) this._window).Disposed)
      return;
    this._window.OverrideMenu.SelectId(args.Id);
    if (this._placement.CurrentMode == null)
      return;
    PlacementInformation placementInformation = new PlacementInformation()
    {
      PlacementOption = this._placement.AllModeNames[args.Id],
      EntityType = this._placement.CurrentPermission.EntityType,
      Range = 2,
      IsTile = this._placement.CurrentPermission.IsTile
    };
    this._placement.Clear();
    this._placement.BeginPlacing(placementInformation, (PlacementHijack) null);
  }

  private void OnEntitySearchChanged(LineEdit.LineEditEventArgs args)
  {
    if (this._window == null || ((Control) this._window).Disposed)
      return;
    this._placement.Clear();
    this.BuildEntityList(args.Text);
    ((BaseButton) this._window.ClearButton).Disabled = string.IsNullOrEmpty(args.Text);
  }

  private void OnEntityClearPressed(BaseButton.ButtonEventArgs args)
  {
    if (this._window == null || ((Control) this._window).Disposed)
      return;
    this._placement.Clear();
    this._window.SearchBar.Clear();
    this.BuildEntityList(string.Empty);
  }

  private void BuildEntityList(string? searchStr = null)
  {
    if (this._window == null || ((Control) this._window).Disposed)
      return;
    this._shownEntities.Clear();
    ((Control) this._window.PrototypeList).RemoveAllChildren();
    this._lastEntityIndices = (0, -1);
    ((Control) this._window.PrototypeList).RemoveAllChildren();
    this._window.SelectedButton = (EntitySpawnButton) null;
    searchStr = searchStr?.ToLowerInvariant();
    EntityCategoryPrototype categoryPrototype;
    this._prototypes.TryIndex<EntityCategoryPrototype>(this._cfg.GetCVar<string>(CVars.EntitiesCategoryFilter), ref categoryPrototype);
    foreach (EntityPrototype enumeratePrototype in this._prototypes.EnumeratePrototypes<EntityPrototype>())
    {
      if (!enumeratePrototype.Abstract && !enumeratePrototype.HideSpawnMenu && !this._disallowedEntityIds.Contains(enumeratePrototype.ID.ToLowerInvariant()) && (categoryPrototype == null || enumeratePrototype.Categories.Contains(categoryPrototype)) && (searchStr == null || SponsorEntitySpawningUIController.DoesEntityMatchSearch(enumeratePrototype, searchStr)))
        this._shownEntities.Add(enumeratePrototype);
    }
    this._shownEntities.Sort((Comparison<EntityPrototype>) ((a, b) =>
    {
      int num = string.Compare(a.Name, b.Name, StringComparison.Ordinal);
      return num == 0 ? string.Compare(a.EditorSuffix, b.EditorSuffix, StringComparison.Ordinal) : num;
    }));
    this._window.PrototypeList.TotalItemCount = this._shownEntities.Count;
    this._window.PrototypeScrollContainer.SetScrollValue(new Vector2(0.0f, 0.0f));
    this.UpdateVisiblePrototypes();
  }

  private static bool DoesEntityMatchSearch(EntityPrototype prototype, string searchStr)
  {
    return string.IsNullOrEmpty(searchStr) || prototype.ID.Contains(searchStr, StringComparison.InvariantCultureIgnoreCase) || prototype.EditorSuffix != null && prototype.EditorSuffix.Contains(searchStr, StringComparison.InvariantCultureIgnoreCase) || !string.IsNullOrEmpty(prototype.Name) && prototype.Name.Contains(searchStr, StringComparison.InvariantCultureIgnoreCase);
  }

  private void UpdateEntityDirectionLabel()
  {
    if (this._window == null || ((Control) this._window).Disposed)
      return;
    this._window.RotationLabel.Text = this._placement.Direction.ToString();
  }

  private void OnDirectionChanged(object? sender, EventArgs e) => this.UpdateEntityDirectionLabel();

  private void UpdateVisiblePrototypes()
  {
    if (this._window == null || ((Control) this._window).Disposed)
      return;
    float num1 = ((Control) this._window.MeasureButton).DesiredSize.Y + 2f;
    int val2_1 = (int) Math.Floor((double) Math.Max(-((Control) this._window.PrototypeList).Position.Y, 0.0f) / (double) num1);
    this._window.PrototypeList.ItemOffset = val2_1;
    (int start, int end) = this._lastEntityIndices;
    int val1 = val2_1 - 1;
    float num2 = -num1;
    while ((double) num2 < (double) ((Control) this._window.PrototypeList).Parent.Height)
    {
      num2 += num1;
      ++val1;
    }
    int val2_2 = Math.Min(val1, this._shownEntities.Count - 1);
    if (val2_2 == end && val2_1 == start)
      return;
    this._lastEntityIndices = (val2_1, val2_2);
    for (int index = start; index < val2_1 && index <= end; ++index)
      ((Control) this._window.PrototypeList).RemoveChild(((Control) this._window.PrototypeList).GetChild(0));
    for (int index = end; index > val2_2 && index >= start; --index)
      ((Control) this._window.PrototypeList).RemoveChild(((Control) this._window.PrototypeList).GetChild(((Control) this._window.PrototypeList).ChildCount - 1));
    for (int index = Math.Min(start - 1, val2_2); index >= val2_1; --index)
      this.InsertEntityButton(this._shownEntities[index], true, index);
    for (int index = Math.Max(end + 1, val2_1); index <= val2_2; ++index)
      this.InsertEntityButton(this._shownEntities[index], false, index);
  }

  private void InsertEntityButton(EntityPrototype prototype, bool insertFirst, int index)
  {
    if (this._window == null || ((Control) this._window).Disposed)
      return;
    ((BaseButton) this._window.InsertEntityButton(prototype, insertFirst, index).ActualButton).OnToggled += new Action<BaseButton.ButtonToggledEventArgs>(this.OnEntityButtonToggled);
  }

  private void OnEntityButtonToggled(BaseButton.ButtonToggledEventArgs args)
  {
    if (this._window == null || ((Control) this._window).Disposed)
      return;
    EntitySpawnButton parent = (EntitySpawnButton) ((Control) ((BaseButton.ButtonEventArgs) args).Button).Parent;
    if (this._window.SelectedButton == parent)
    {
      this._window.SelectedButton = (EntitySpawnButton) null;
      this._window.SelectedPrototype = (EntityPrototype) null;
      this._placement.Clear();
    }
    else
    {
      if (this._window.SelectedButton != null)
        ((BaseButton) this._window.SelectedButton.ActualButton).Pressed = false;
      this._window.SelectedButton = (EntitySpawnButton) null;
      this._window.SelectedPrototype = (EntityPrototype) null;
      string allModeName = this._placement.AllModeNames[this._window.OverrideMenu.SelectedId];
      this._placement.BeginPlacing(new PlacementInformation()
      {
        PlacementOption = allModeName != "Default" ? allModeName : parent.Prototype.PlacementMode,
        EntityType = parent.PrototypeID,
        Range = 2,
        IsTile = false
      }, (PlacementHijack) null);
      this._window.SelectedButton = parent;
      this._window.SelectedPrototype = parent.Prototype;
    }
  }
}
