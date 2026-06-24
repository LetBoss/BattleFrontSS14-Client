// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Ghost.PubgGhostFollowSelectWindow
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG.Ghost;
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
namespace Content.Client._PUBG.Ghost;

public sealed class PubgGhostFollowSelectWindow : DefaultWindow
{
  private readonly BoxContainer _optionsContainer;

  public event Action<NetEntity>? FollowRequested;

  public PubgGhostFollowSelectWindow()
  {
    this.Title = Loc.GetString("pubg-ghost-follow-window-title");
    Label label = new Label()
    {
      Text = Loc.GetString("pubg-ghost-follow-window-subtitle")
    };
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer1.SeparationOverride = new int?(6);
    ((Control) boxContainer1).HorizontalExpand = true;
    this._optionsContainer = boxContainer1;
    ScrollContainer scrollContainer1 = new ScrollContainer();
    ((Control) scrollContainer1).HorizontalExpand = true;
    ((Control) scrollContainer1).VerticalExpand = true;
    ((Control) scrollContainer1).MinSize = new Vector2(320f, 240f);
    scrollContainer1.HScrollEnabled = false;
    ScrollContainer scrollContainer2 = scrollContainer1;
    ((Control) scrollContainer2).AddChild((Control) this._optionsContainer);
    Button button = new Button()
    {
      Text = Loc.GetString("pubg-ghost-follow-window-close")
    };
    ((BaseButton) button).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => ((BaseWindow) this).Close());
    BoxContainer boxContainer2 = new BoxContainer();
    boxContainer2.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer2.SeparationOverride = new int?(10);
    ((Control) boxContainer2).Margin = new Thickness(12f);
    BoxContainer boxContainer3 = boxContainer2;
    ((Control) boxContainer3).AddChild((Control) label);
    ((Control) boxContainer3).AddChild((Control) scrollContainer2);
    ((Control) boxContainer3).AddChild((Control) button);
    this.Contents.AddChild((Control) boxContainer3);
  }

  public void SetOptions(
    IReadOnlyList<PubgGhostFollowTeammateOptionState> options)
  {
    ((Control) this._optionsContainer).RemoveAllChildren();
    if (options.Count == 0)
    {
      ((Control) this._optionsContainer).AddChild((Control) new Label()
      {
        Text = Loc.GetString("pubg-ghost-follow-window-empty")
      });
    }
    else
    {
      foreach (PubgGhostFollowTeammateOptionState option in (IEnumerable<PubgGhostFollowTeammateOptionState>) options)
      {
        NetEntity entity = option.Entity;
        Button button1 = new Button();
        button1.Text = option.Name;
        ((Control) button1).HorizontalExpand = true;
        Button button2 = button1;
        ((BaseButton) button2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
        {
          Action<NetEntity> followRequested = this.FollowRequested;
          if (followRequested == null)
            return;
          followRequested(entity);
        });
        ((Control) this._optionsContainer).AddChild((Control) button2);
      }
    }
  }
}
