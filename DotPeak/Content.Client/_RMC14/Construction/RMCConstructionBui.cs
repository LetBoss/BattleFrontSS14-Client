// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Construction.RMCConstructionBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._RMC14.UserInterface;
using Content.Shared._CIV14merka.Teams;
using Content.Shared._RMC14.Construction;
using Content.Shared._RMC14.Construction.Prototypes;
using Content.Shared.Stacks;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client._RMC14.Construction;

public sealed class RMCConstructionBui : BoundUserInterface
{
  [Dependency]
  private IPrototypeManager _prototype;
  [Dependency]
  private IPlayerManager _player;
  private readonly SpriteSystem _sprite;
  private Texture? _materialIcon;
  private Texture? _hammerIcon;
  [Robust.Shared.ViewVariables.ViewVariables]
  private RMCConstructionWindow? _window;

  public RMCConstructionBui(EntityUid owner, Enum uiKey)
    : base(owner, uiKey)
  {
    this._sprite = this.EntMan.System<SpriteSystem>();
  }

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<RMCConstructionWindow>((BoundUserInterface) this);
    this._window.Title = "Construction using the " + this.EntMan.GetComponent<MetaDataComponent>(this.Owner).EntityName;
    RMCConstructionItemComponent constructionItemComponent;
    if (!this.EntMan.TryGetComponent<RMCConstructionItemComponent>(this.Owner, ref constructionItemComponent))
      return;
    ProtoId<RMCConstructionPrototype>[] buildable = constructionItemComponent.Buildable;
    if (buildable == null)
      return;
    this.Refresh(buildable);
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (!(this.State is RMCConstructionBuiState))
      return;
    this.RefreshStackAmount();
  }

  private void AddEntry(ProtoId<RMCConstructionPrototype> prototypeId)
  {
    RMCConstructionPrototype build;
    if (!this._prototype.TryIndex<RMCConstructionPrototype>(prototypeId, ref build))
      return;
    if (!string.IsNullOrEmpty(build.SideId))
    {
      EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
      CivTeamMemberComponent teamMemberComponent;
      if (!localEntity.HasValue || !this.EntMan.TryGetComponent<CivTeamMemberComponent>(localEntity.GetValueOrDefault(), ref teamMemberComponent) || teamMemberComponent.SideId != build.SideId)
        return;
    }
    if (build.IsDivider)
    {
      BlueHorizontalSeparator horizontalSeparator = new BlueHorizontalSeparator();
      horizontalSeparator.Margin = new Thickness(5f);
      ((Control) this._window?.ConstructionContainer).AddChild((Control) horizontalSeparator);
    }
    else if (build.Listed != null)
    {
      this.AddListButton(build);
    }
    else
    {
      string name = Loc.GetString("rmc-construction-list", new (string, object)[1]
      {
        ("name", (object) build.Name)
      });
      if (build.MaterialCost.HasValue)
        name = Loc.GetString("rmc-construction-entry", new (string, object)[3]
        {
          ("name", (object) build.Name),
          ("amount", (object) build.MaterialCost),
          ("material", (object) this.Owner)
        });
      RMCBuildChoiceControl buildChoiceControl1 = new RMCBuildChoiceControl();
      buildChoiceControl1.Set(name);
      RMCBuildChoiceControl buildChoiceControl2 = buildChoiceControl1;
      SpriteSpecifier icon = build.Icon;
      Texture texture = icon != null ? this._sprite.Frame0(icon) : (Texture) null;
      buildChoiceControl2.SetIcon(texture);
      int valueOrDefault = build.MaterialCost.GetValueOrDefault();
      double a = Math.Max(build.DoAfterTime.TotalSeconds, build.DoAfterTimeMin.TotalSeconds);
      int hammer = a > 0.0 ? Math.Max(1, (int) Math.Ceiling(a)) : 0;
      if (this._materialIcon == null)
        this._materialIcon = this._sprite.Frame0((SpriteSpecifier) new SpriteSpecifier.Rsi(new ResPath("_PUBG/Interface/Construction/build_menu.rsi"), "craft_material"));
      if (this._hammerIcon == null)
        this._hammerIcon = this._sprite.Frame0((SpriteSpecifier) new SpriteSpecifier.Rsi(new ResPath("_PUBG/Interface/Construction/build_menu.rsi"), "craft_hammer"));
      buildChoiceControl1.SetCosts(valueOrDefault > 0 ? this._materialIcon : (Texture) null, valueOrDefault, hammer > 0 ? this._hammerIcon : (Texture) null, hammer);
      HashSet<int> stackAmounts = build.StackAmounts;
      if (stackAmounts != null)
      {
        foreach (int num in stackAmounts)
        {
          int stack = num;
          Button button1 = new Button();
          button1.Text = "x" + stack.ToString();
          ((Control) button1).StyleClasses.Add("OpenBoth");
          ((Control) button1).SetWidth = 45f;
          ((Control) button1).Margin = new Thickness(0.0f, 0.0f, 0.0f, 3f);
          ((Control) button1).HorizontalAlignment = (Control.HAlignment) 3;
          Button button2 = button1;
          ((Control) buildChoiceControl1.StackAmountContainer).AddChild((Control) button2);
          ((BaseButton) button2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new RMCConstructionBuiMsg(ProtoId<RMCConstructionPrototype>.op_Implicit(build), stack)));
          ((Control) buildChoiceControl1.Button).SetWidth = 250f;
          ((Control) buildChoiceControl1.Button).HorizontalAlignment = (Control.HAlignment) 1;
        }
      }
      ((BaseButton) buildChoiceControl1.Button).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new RMCConstructionBuiMsg(ProtoId<RMCConstructionPrototype>.op_Implicit(build), build.Amount)));
      ((Control) this._window?.ConstructionContainer).AddChild((Control) buildChoiceControl1);
    }
  }

  private void AddListButton(RMCConstructionPrototype build)
  {
    ProtoId<RMCConstructionPrototype>[] listed = build.Listed;
    if (listed == null)
      return;
    RMCBuildChoiceControl buildChoiceControl = new RMCBuildChoiceControl();
    buildChoiceControl.Set(build.Name);
    ((BaseButton) buildChoiceControl.Button).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      ((Control) this._window?.ConstructionContainer).Children.Clear();
      this.Refresh(listed);
    });
    ((Control) this._window?.ConstructionContainer).AddChild((Control) buildChoiceControl);
  }

  public void Refresh(ProtoId<RMCConstructionPrototype>[] entries)
  {
    if (this._window == null)
      return;
    this.RefreshStackAmount();
    foreach (ProtoId<RMCConstructionPrototype> entry in entries)
      this.AddEntry(entry);
  }

  public void RefreshStackAmount()
  {
    StackComponent stackComponent;
    if (this._window == null || !this.EntMan.TryGetComponent<StackComponent>(this.Owner, ref stackComponent))
      return;
    this._window.MaterialLabel.Text = $"Amount Left: {stackComponent.Count}";
  }
}
