// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Commander.UI.CivCommanderTeleportWindow
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client._CIV14merka.Commander.UI;

public sealed class CivCommanderTeleportWindow : DefaultWindow
{
  private readonly LineEdit _search;
  private readonly ScrollContainer _scroll;
  private readonly BoxContainer _buttons;
  private readonly Label _emptyLabel;
  private readonly List<(string Name, NetEntity Entity)> _targets = new List<(string, NetEntity)>();

  public event Action<NetEntity>? TargetSelected;

  public CivCommanderTeleportWindow()
  {
    this.Title = Loc.GetString("civ-cmd-teleport-title");
    ((Control) this).MinSize = new Vector2(500f, 560f);
    ((Control) this).SetSize = new Vector2(560f, 700f);
    this._search = new LineEdit()
    {
      PlaceHolder = Loc.GetString("civ-cmd-teleport-search")
    };
    ScrollContainer scrollContainer = new ScrollContainer();
    ((Control) scrollContainer).VerticalExpand = true;
    scrollContainer.HScrollEnabled = false;
    this._scroll = scrollContainer;
    this._buttons = new BoxContainer()
    {
      Orientation = (BoxContainer.LayoutOrientation) 1,
      SeparationOverride = new int?(4)
    };
    Label label = new Label();
    label.Text = Loc.GetString("civ-cmd-teleport-empty");
    ((Control) label).HorizontalAlignment = (Control.HAlignment) 2;
    this._emptyLabel = label;
    this._search.OnTextChanged += (Action<LineEdit.LineEditEventArgs>) (_ => this.Rebuild());
    ((Control) this._scroll).AddChild((Control) this._buttons);
    Control contents = this.Contents;
    BoxContainer boxContainer = new BoxContainer();
    boxContainer.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer.SeparationOverride = new int?(8);
    ((Control) boxContainer).Margin = new Thickness(8f);
    ((Control) boxContainer).Children.Add((Control) this._search);
    ((Control) boxContainer).Children.Add((Control) this._scroll);
    contents.AddChild((Control) boxContainer);
  }

  public void UpdateTargets(
    IEnumerable<(string Name, NetEntity Entity)> targets)
  {
    this._targets.Clear();
    this._targets.AddRange(targets);
    this.Rebuild();
  }

  private void Rebuild()
  {
    ((Control) this._buttons).DisposeAllChildren();
    string str = this._search.Text.Trim();
    bool flag = false;
    foreach ((string Name, NetEntity Entity) target in this._targets)
    {
      if (str.Length <= 0 || target.Name.Contains(str, StringComparison.OrdinalIgnoreCase))
      {
        flag = true;
        Button button1 = new Button();
        button1.Text = target.Name;
        ((Control) button1).HorizontalExpand = true;
        ((Control) button1).MinSize = new Vector2(420f, 30f);
        button1.ClipText = true;
        Button button2 = button1;
        NetEntity entity = target.Entity;
        ((BaseButton) button2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
        {
          Action<NetEntity> targetSelected = this.TargetSelected;
          if (targetSelected == null)
            return;
          targetSelected(entity);
        });
        ((Control) this._buttons).AddChild((Control) button2);
      }
    }
    if (!flag)
      ((Control) this._buttons).AddChild((Control) this._emptyLabel);
    this._scroll.SetScrollValue(Vector2.Zero);
  }
}
