// Decompiled with JetBrains decompiler
// Type: Content.Client.Administration.UI.PermissionsEui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Administration.Managers;
using Content.Client.Eui;
using Content.Shared.Administration;
using Content.Shared.Eui;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Client.Administration.UI;

public sealed class PermissionsEui : BaseEui
{
  private const int NoRank = -1;
  [Dependency]
  private IClientAdminManager _adminManager;
  private readonly PermissionsEui.Menu _menu;
  private readonly List<DefaultWindow> _subWindows = new List<DefaultWindow>();
  private Dictionary<int, PermissionsEuiState.AdminRankData> _ranks = new Dictionary<int, PermissionsEuiState.AdminRankData>();

  public PermissionsEui()
  {
    IoCManager.InjectDependencies<PermissionsEui>(this);
    this._menu = new PermissionsEui.Menu(this);
    ((BaseButton) this._menu.AddAdminButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.AddAdminPressed);
    ((BaseButton) this._menu.AddAdminRankButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.AddAdminRankPressed);
    ((BaseWindow) this._menu).OnClose += new Action(this.CloseEverything);
  }

  public override void Closed()
  {
    base.Closed();
    this.SendMessage((EuiMessageBase) new CloseEuiMessage());
    this.CloseEverything();
  }

  private void CloseEverything()
  {
    foreach (BaseWindow baseWindow in this._subWindows.ToArray())
      baseWindow.Close();
    ((BaseWindow) this._menu).Close();
  }

  private void AddAdminPressed(BaseButton.ButtonEventArgs obj)
  {
    this.OpenEditWindow(new PermissionsEuiState.AdminData?());
  }

  private void AddAdminRankPressed(BaseButton.ButtonEventArgs obj)
  {
    this.OpenRankEditWindow(new KeyValuePair<int, PermissionsEuiState.AdminRankData>?());
  }

  private void OnEditPressed(PermissionsEuiState.AdminData admin)
  {
    this.OpenEditWindow(new PermissionsEuiState.AdminData?(admin));
  }

  private void OpenEditWindow(PermissionsEuiState.AdminData? data)
  {
    PermissionsEui.EditAdminWindow window = new PermissionsEui.EditAdminWindow(this, data);
    ((BaseButton) window.SaveButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SaveAdminPressed(window));
    ((BaseWindow) window).OpenCentered();
    ((BaseWindow) window).OnClose += (Action) (() => this._subWindows.Remove((DefaultWindow) window));
    if (data.HasValue)
      ((BaseButton) window.RemoveButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.RemoveButtonPressed(window));
    this._subWindows.Add((DefaultWindow) window);
  }

  private void OpenRankEditWindow(
    KeyValuePair<int, PermissionsEuiState.AdminRankData>? rank)
  {
    PermissionsEui.EditAdminRankWindow window = new PermissionsEui.EditAdminRankWindow(this, rank);
    ((BaseButton) window.SaveButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SaveAdminRankPressed(window));
    ((BaseWindow) window).OpenCentered();
    ((BaseWindow) window).OnClose += (Action) (() => this._subWindows.Remove((DefaultWindow) window));
    if (rank.HasValue)
      ((BaseButton) window.RemoveButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.RemoveRankButtonPressed(window));
    this._subWindows.Add((DefaultWindow) window);
  }

  private void RemoveButtonPressed(PermissionsEui.EditAdminWindow window)
  {
    this.SendMessage((EuiMessageBase) new PermissionsEuiMsg.RemoveAdmin()
    {
      UserId = window.SourceData.Value.UserId
    });
    ((BaseWindow) window).Close();
  }

  private void RemoveRankButtonPressed(PermissionsEui.EditAdminRankWindow window)
  {
    this.SendMessage((EuiMessageBase) new PermissionsEuiMsg.RemoveAdminRank()
    {
      Id = window.SourceId.Value
    });
    ((BaseWindow) window).Close();
  }

  private void SaveAdminPressed(PermissionsEui.EditAdminWindow popup)
  {
    AdminFlags pos;
    AdminFlags neg;
    popup.CollectSetFlags(out pos, out neg);
    int? nullable = new int?(popup.RankButton.SelectedId);
    if (nullable.GetValueOrDefault() == -1)
      nullable = new int?();
    string text = string.IsNullOrWhiteSpace(popup.TitleEdit.Text) ? (string) null : popup.TitleEdit.Text;
    bool pressed = ((BaseButton) popup.SuspendedCheckbox).Pressed;
    PermissionsEuiState.AdminData? sourceData = popup.SourceData;
    if (sourceData.HasValue)
    {
      PermissionsEuiState.AdminData valueOrDefault = sourceData.GetValueOrDefault();
      this.SendMessage((EuiMessageBase) new PermissionsEuiMsg.UpdateAdmin()
      {
        UserId = valueOrDefault.UserId,
        Title = text,
        PosFlags = pos,
        NegFlags = neg,
        RankId = nullable,
        Suspended = pressed
      });
    }
    else
      this.SendMessage((EuiMessageBase) new PermissionsEuiMsg.AddAdmin()
      {
        UserNameOrId = popup.NameEdit.Text,
        Title = text,
        PosFlags = pos,
        NegFlags = neg,
        RankId = nullable,
        Suspended = pressed
      });
    ((BaseWindow) popup).Close();
  }

  private void SaveAdminRankPressed(PermissionsEui.EditAdminRankWindow popup)
  {
    AdminFlags adminFlags = popup.CollectSetFlags();
    string text = popup.NameEdit.Text;
    int? sourceId = popup.SourceId;
    if (sourceId.HasValue)
    {
      int valueOrDefault = sourceId.GetValueOrDefault();
      this.SendMessage((EuiMessageBase) new PermissionsEuiMsg.UpdateAdminRank()
      {
        Id = valueOrDefault,
        Flags = adminFlags,
        Name = text
      });
    }
    else
      this.SendMessage((EuiMessageBase) new PermissionsEuiMsg.AddAdminRank()
      {
        Flags = adminFlags,
        Name = text
      });
    ((BaseWindow) popup).Close();
  }

  public override void Opened() => ((BaseWindow) this._menu).OpenCentered();

  public override void HandleState(EuiStateBase state)
  {
    PermissionsEuiState permissionsEuiState = (PermissionsEuiState) state;
    if (permissionsEuiState.IsLoading)
      return;
    this._ranks = permissionsEuiState.AdminRanks;
    ((Control) this._menu.AdminsList).RemoveAllChildren();
    foreach (PermissionsEuiState.AdminData adminData in (IEnumerable<PermissionsEuiState.AdminData>) ((IEnumerable<PermissionsEuiState.AdminData>) permissionsEuiState.Admins).OrderBy<PermissionsEuiState.AdminData, string>((Func<PermissionsEuiState.AdminData, string>) (d => d.UserName)))
    {
      PermissionsEuiState.AdminData admin = adminData;
      GridContainer adminsList = this._menu.AdminsList;
      string str1 = admin.UserName ?? admin.UserId.ToString();
      ((Control) adminsList).AddChild((Control) new Label()
      {
        Text = str1
      });
      Label label1 = new Label()
      {
        Text = admin.Title ?? Loc.GetString("permissions-eui-edit-admin-title-control-text").ToLowerInvariant()
      };
      if (admin.Title == null)
        ((Control) label1).StyleClasses.Add("Italic");
      ((Control) adminsList).AddChild((Control) label1);
      AdminFlags posFlags = admin.PosFlags;
      int? rankId = admin.RankId;
      bool flag;
      string str2;
      if (rankId.HasValue)
      {
        int valueOrDefault = rankId.GetValueOrDefault();
        flag = false;
        PermissionsEuiState.AdminRankData adminRank = permissionsEuiState.AdminRanks[valueOrDefault];
        str2 = adminRank.Name;
        posFlags |= adminRank.Flags;
      }
      else
      {
        flag = true;
        str2 = Loc.GetString("permissions-eui-edit-no-rank-text").ToLowerInvariant();
      }
      Label label2 = new Label() { Text = str2 };
      if (flag)
        ((Control) label2).StyleClasses.Add("Italic");
      ((Control) adminsList).AddChild((Control) label2);
      string str3 = AdminFlagsHelper.PosNegFlagsText(admin.PosFlags, admin.NegFlags);
      Label label3 = new Label();
      label3.Text = str3;
      ((Control) label3).HorizontalExpand = true;
      ((Control) label3).HorizontalAlignment = (Control.HAlignment) 2;
      ((Control) adminsList).AddChild((Control) label3);
      Button button = new Button()
      {
        Text = Loc.GetString("permissions-eui-edit-title-button")
      };
      ((BaseButton) button).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.OnEditPressed(admin));
      ((Control) adminsList).AddChild((Control) button);
      if (!this._adminManager.HasFlag(posFlags))
      {
        ((BaseButton) button).Disabled = true;
        ((Control) button).ToolTip = Loc.GetString("permissions-eui-do-not-have-required-flags-to-edit-admin-tooltip");
      }
    }
    ((Control) this._menu.AdminRanksList).RemoveAllChildren();
    foreach (KeyValuePair<int, PermissionsEuiState.AdminRankData> adminRank in permissionsEuiState.AdminRanks)
    {
      KeyValuePair<int, PermissionsEuiState.AdminRankData> kv = adminRank;
      PermissionsEuiState.AdminRankData adminRankData = kv.Value;
      string str = string.Join<string>(' ', ((IEnumerable<string>) AdminFlagsHelper.FlagsToNames(adminRankData.Flags)).Select<string, string>((Func<string, string>) (f => "+" + f)));
      ((Control) this._menu.AdminRanksList).AddChild((Control) new Label()
      {
        Text = adminRankData.Name
      });
      GridContainer adminRanksList = this._menu.AdminRanksList;
      Label label = new Label();
      label.Text = str;
      ((Control) label).HorizontalExpand = true;
      ((Control) label).HorizontalAlignment = (Control.HAlignment) 2;
      ((Control) adminRanksList).AddChild((Control) label);
      Button button = new Button()
      {
        Text = Loc.GetString("permissions-eui-edit-admin-rank-button")
      };
      ((BaseButton) button).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.OnEditRankPressed(kv));
      ((Control) this._menu.AdminRanksList).AddChild((Control) button);
      if (!this._adminManager.HasFlag(adminRankData.Flags))
      {
        ((BaseButton) button).Disabled = true;
        ((Control) button).ToolTip = Loc.GetString("permissions-eui-do-not-have-required-flags-to-edit-rank-tooltip");
      }
    }
  }

  private void OnEditRankPressed(
    KeyValuePair<int, PermissionsEuiState.AdminRankData> rank)
  {
    this.OpenRankEditWindow(new KeyValuePair<int, PermissionsEuiState.AdminRankData>?(rank));
  }

  private sealed class Menu : DefaultWindow
  {
    private readonly PermissionsEui _ui;
    public readonly GridContainer AdminsList;
    public readonly GridContainer AdminRanksList;
    public readonly Button AddAdminButton;
    public readonly Button AddAdminRankButton;

    public Menu(PermissionsEui ui)
    {
      this._ui = ui;
      this.Title = Loc.GetString("permissions-eui-menu-title");
      TabContainer tabContainer = new TabContainer();
      Button button1 = new Button();
      button1.Text = Loc.GetString("permissions-eui-menu-add-admin-button");
      ((Control) button1).HorizontalAlignment = (Control.HAlignment) 3;
      this.AddAdminButton = button1;
      Button button2 = new Button();
      button2.Text = Loc.GetString("permissions-eui-menu-add-admin-rank-button");
      ((Control) button2).HorizontalAlignment = (Control.HAlignment) 3;
      this.AddAdminRankButton = button2;
      GridContainer gridContainer1 = new GridContainer();
      gridContainer1.Columns = 5;
      ((Control) gridContainer1).VerticalExpand = true;
      this.AdminsList = gridContainer1;
      BoxContainer boxContainer1 = new BoxContainer();
      boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
      Control.OrderedChildCollection children1 = ((Control) boxContainer1).Children;
      ScrollContainer scrollContainer1 = new ScrollContainer();
      ((Control) scrollContainer1).VerticalExpand = true;
      ((Control) scrollContainer1).Children.Add((Control) this.AdminsList);
      children1.Add((Control) scrollContainer1);
      ((Control) boxContainer1).Children.Add((Control) this.AddAdminButton);
      BoxContainer boxContainer2 = boxContainer1;
      TabContainer.SetTabTitle((Control) boxContainer2, Loc.GetString("permissions-eui-menu-admins-tab-title"));
      GridContainer gridContainer2 = new GridContainer();
      gridContainer2.Columns = 3;
      ((Control) gridContainer2).VerticalExpand = true;
      this.AdminRanksList = gridContainer2;
      BoxContainer boxContainer3 = new BoxContainer();
      boxContainer3.Orientation = (BoxContainer.LayoutOrientation) 1;
      Control.OrderedChildCollection children2 = ((Control) boxContainer3).Children;
      ScrollContainer scrollContainer2 = new ScrollContainer();
      ((Control) scrollContainer2).VerticalExpand = true;
      ((Control) scrollContainer2).Children.Add((Control) this.AdminRanksList);
      children2.Add((Control) scrollContainer2);
      ((Control) boxContainer3).Children.Add((Control) this.AddAdminRankButton);
      BoxContainer boxContainer4 = boxContainer3;
      TabContainer.SetTabTitle((Control) boxContainer4, Loc.GetString("permissions-eui-menu-admin-ranks-tab-title"));
      ((Control) tabContainer).AddChild((Control) boxContainer2);
      ((Control) tabContainer).AddChild((Control) boxContainer4);
      this.Contents.AddChild((Control) tabContainer);
    }

    protected virtual Vector2 ContentsMinimumSize => new Vector2(600f, 400f);
  }

  private sealed class EditAdminWindow : DefaultWindow
  {
    public readonly PermissionsEuiState.AdminData? SourceData;
    public readonly LineEdit? NameEdit;
    public readonly LineEdit TitleEdit;
    public readonly OptionButton RankButton;
    public readonly Button SaveButton;
    public readonly Button? RemoveButton;
    public readonly CheckBox SuspendedCheckbox;
    public readonly Dictionary<AdminFlags, (Button inherit, Button sub, Button plus)> FlagButtons = new Dictionary<AdminFlags, (Button, Button, Button)>();

    public EditAdminWindow(PermissionsEui ui, PermissionsEuiState.AdminData? data)
    {
      ((Control) this).MinSize = new Vector2(600f, 400f);
      this.SourceData = data;
      Control control;
      if (data.HasValue)
      {
        PermissionsEuiState.AdminData valueOrDefault = data.GetValueOrDefault();
        string str = valueOrDefault.UserName ?? valueOrDefault.UserId.ToString();
        this.Title = Loc.GetString("permissions-eui-edit-admin-window-edit-admin-label", new (string, object)[1]
        {
          ("admin", (object) str)
        });
        control = (Control) new Label() { Text = str };
      }
      else
      {
        this.Title = Loc.GetString("permissions-eui-menu-add-admin-button");
        LineEdit lineEdit1 = new LineEdit();
        lineEdit1.PlaceHolder = Loc.GetString("permissions-eui-edit-admin-window-name-edit-placeholder");
        LineEdit lineEdit2 = lineEdit1;
        this.NameEdit = lineEdit1;
        control = (Control) lineEdit2;
      }
      this.TitleEdit = new LineEdit()
      {
        PlaceHolder = Loc.GetString("permissions-eui-edit-admin-window-title-edit-placeholder")
      };
      this.RankButton = new OptionButton();
      Button button1 = new Button();
      button1.Text = Loc.GetString("permissions-eui-edit-admin-window-save-button");
      ((Control) button1).HorizontalAlignment = (Control.HAlignment) 3;
      this.SaveButton = button1;
      CheckBox checkBox = new CheckBox();
      checkBox.Text = Loc.GetString("permissions-eui-edit-admin-window-suspended");
      ((BaseButton) checkBox).Pressed = data.HasValue && data.GetValueOrDefault().Suspended;
      this.SuspendedCheckbox = checkBox;
      this.RankButton.AddItem(Loc.GetString("permissions-eui-edit-admin-window-no-rank-button"), new int?(-1));
      foreach ((int key, PermissionsEuiState.AdminRankData adminRankData) in ui._ranks)
        this.RankButton.AddItem(adminRankData.Name, new int?(key));
      this.RankButton.SelectId((int?) data?.RankId ?? -1);
      this.RankButton.OnItemSelected += new Action<OptionButton.ItemSelectedEventArgs>(this.RankSelected);
      GridContainer gridContainer = new GridContainer()
      {
        Columns = 4,
        HSeparationOverride = new int?(0),
        VSeparationOverride = new int?(0)
      };
      foreach (AdminFlags allFlag in (IEnumerable<AdminFlags>) AdminFlagsHelper.AllFlags)
      {
        bool flag = !ui._adminManager.HasFlag(allFlag);
        string upper = allFlag.ToString().ToUpper();
        ButtonGroup buttonGroup = new ButtonGroup(true);
        Button button2 = new Button();
        button2.Text = "I";
        ((Control) button2).StyleClasses.Add("OpenRight");
        ((BaseButton) button2).Disabled = flag;
        ((BaseButton) button2).Group = buttonGroup;
        Button button3 = button2;
        Button button4 = new Button();
        button4.Text = "-";
        ((Control) button4).StyleClasses.Add("OpenBoth");
        ((BaseButton) button4).Disabled = flag;
        ((BaseButton) button4).Group = buttonGroup;
        Button button5 = button4;
        Button button6 = new Button();
        button6.Text = "+";
        ((Control) button6).StyleClasses.Add("OpenLeft");
        ((BaseButton) button6).Disabled = flag;
        ((BaseButton) button6).Group = buttonGroup;
        Button button7 = button6;
        if (data.HasValue)
        {
          PermissionsEuiState.AdminData valueOrDefault = data.GetValueOrDefault();
          if ((valueOrDefault.NegFlags & allFlag) != AdminFlags.None)
            ((BaseButton) button5).Pressed = true;
          else if ((valueOrDefault.PosFlags & allFlag) != AdminFlags.None)
            ((BaseButton) button7).Pressed = true;
          else
            ((BaseButton) button3).Pressed = true;
        }
        else
          ((BaseButton) button3).Pressed = true;
        ((Control) gridContainer).AddChild((Control) new Label()
        {
          Text = upper
        });
        ((Control) gridContainer).AddChild((Control) button3);
        ((Control) gridContainer).AddChild((Control) button5);
        ((Control) gridContainer).AddChild((Control) button7);
        this.FlagButtons.Add(allFlag, (button3, button5, button7));
      }
      BoxContainer boxContainer1 = new BoxContainer()
      {
        Orientation = (BoxContainer.LayoutOrientation) 0
      };
      if (data.HasValue)
      {
        this.RemoveButton = new Button()
        {
          Text = Loc.GetString("permissions-eui-edit-admin-window-remove-flag-button")
        };
        ((Control) boxContainer1).AddChild((Control) this.RemoveButton);
      }
      ((Control) boxContainer1).AddChild((Control) this.SaveButton);
      Control contents = this.Contents;
      BoxContainer boxContainer2 = new BoxContainer();
      boxContainer2.Orientation = (BoxContainer.LayoutOrientation) 1;
      Control.OrderedChildCollection children1 = ((Control) boxContainer2).Children;
      BoxContainer boxContainer3 = new BoxContainer();
      boxContainer3.Orientation = (BoxContainer.LayoutOrientation) 0;
      boxContainer3.SeparationOverride = new int?(2);
      Control.OrderedChildCollection children2 = ((Control) boxContainer3).Children;
      BoxContainer boxContainer4 = new BoxContainer();
      boxContainer4.Orientation = (BoxContainer.LayoutOrientation) 1;
      ((Control) boxContainer4).HorizontalExpand = true;
      ((Control) boxContainer4).Children.Add(control);
      ((Control) boxContainer4).Children.Add((Control) this.TitleEdit);
      ((Control) boxContainer4).Children.Add((Control) this.RankButton);
      ((Control) boxContainer4).Children.Add((Control) this.SuspendedCheckbox);
      children2.Add((Control) boxContainer4);
      ((Control) boxContainer3).Children.Add((Control) gridContainer);
      ((Control) boxContainer3).VerticalExpand = true;
      children1.Add((Control) boxContainer3);
      ((Control) boxContainer2).Children.Add((Control) boxContainer1);
      contents.AddChild((Control) boxContainer2);
    }

    private void RankSelected(OptionButton.ItemSelectedEventArgs obj)
    {
      this.RankButton.SelectId(obj.Id);
    }

    public void CollectSetFlags(out AdminFlags pos, out AdminFlags neg)
    {
      pos = AdminFlags.None;
      neg = AdminFlags.None;
      foreach ((AdminFlags key, (Button _, Button sub, Button plus)) in this.FlagButtons)
      {
        if (((BaseButton) sub).Pressed)
          neg |= key;
        else if (((BaseButton) plus).Pressed)
          pos |= key;
      }
    }
  }

  private sealed class EditAdminRankWindow : DefaultWindow
  {
    public readonly int? SourceId;
    public readonly LineEdit NameEdit;
    public readonly Button SaveButton;
    public readonly Button? RemoveButton;
    public readonly Dictionary<AdminFlags, CheckBox> FlagCheckBoxes = new Dictionary<AdminFlags, CheckBox>();

    public EditAdminRankWindow(
      PermissionsEui ui,
      KeyValuePair<int, PermissionsEuiState.AdminRankData>? data)
    {
      this.Title = Loc.GetString("permissions-eui-edit-admin-rank-window-title");
      ((Control) this).MinSize = new Vector2(600f, 400f);
      this.SourceId = data?.Key;
      this.NameEdit = new LineEdit()
      {
        PlaceHolder = Loc.GetString("permissions-eui-edit-admin-rank-window-name-edit-placeholder")
      };
      if (data.HasValue)
        this.NameEdit.Text = data.Value.Value.Name;
      Button button = new Button();
      button.Text = Loc.GetString("permissions-eui-menu-save-admin-rank-button");
      ((Control) button).HorizontalAlignment = (Control.HAlignment) 3;
      ((Control) button).HorizontalExpand = true;
      this.SaveButton = button;
      BoxContainer boxContainer1 = new BoxContainer()
      {
        Orientation = (BoxContainer.LayoutOrientation) 1
      };
      foreach (AdminFlags allFlag in (IEnumerable<AdminFlags>) AdminFlagsHelper.AllFlags)
      {
        bool flag = !ui._adminManager.HasFlag(allFlag);
        string upper = allFlag.ToString().ToUpper();
        CheckBox checkBox1 = new CheckBox();
        ((BaseButton) checkBox1).Disabled = flag;
        checkBox1.Text = upper;
        CheckBox checkBox2 = checkBox1;
        if (data.HasValue && (data.Value.Value.Flags & allFlag) != AdminFlags.None)
          ((BaseButton) checkBox2).Pressed = true;
        this.FlagCheckBoxes.Add(allFlag, checkBox2);
        ((Control) boxContainer1).AddChild((Control) checkBox2);
      }
      BoxContainer boxContainer2 = new BoxContainer()
      {
        Orientation = (BoxContainer.LayoutOrientation) 0
      };
      if (data.HasValue)
      {
        this.RemoveButton = new Button()
        {
          Text = Loc.GetString("permissions-eui-menu-remove-admin-rank-button")
        };
        ((Control) boxContainer2).AddChild((Control) this.RemoveButton);
      }
      ((Control) boxContainer2).AddChild((Control) this.SaveButton);
      Control contents = this.Contents;
      BoxContainer boxContainer3 = new BoxContainer();
      boxContainer3.Orientation = (BoxContainer.LayoutOrientation) 1;
      ((Control) boxContainer3).Children.Add((Control) this.NameEdit);
      ((Control) boxContainer3).Children.Add((Control) boxContainer1);
      ((Control) boxContainer3).Children.Add((Control) boxContainer2);
      contents.AddChild((Control) boxContainer3);
    }

    public AdminFlags CollectSetFlags()
    {
      AdminFlags adminFlags = AdminFlags.None;
      foreach ((AdminFlags key, CheckBox checkBox) in this.FlagCheckBoxes)
      {
        if (((BaseButton) checkBox).Pressed)
          adminFlags |= key;
      }
      return adminFlags;
    }
  }
}
