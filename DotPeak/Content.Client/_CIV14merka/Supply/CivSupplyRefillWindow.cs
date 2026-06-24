// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Supply.CivSupplyRefillWindow
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._CIV14merka.Supply;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Client._CIV14merka.Supply;

public sealed class CivSupplyRefillWindow : DefaultWindow
{
  private const int StockWidth = 56;
  private const int PriceWidth = 86;
  private const int InputWidth = 60;
  private const int ToggleWidth = 96 /*0x60*/;
  private const int NowWidth = 104;
  private const int IconSize = 28;
  private static readonly Color OnColor = Color.FromHex((ReadOnlySpan<char>) "#5FA85F", new Color?());
  private static readonly Color OffColor = Color.FromHex((ReadOnlySpan<char>) "#9A5C5C", new Color?());
  private static readonly Color MutedColor = Color.FromHex((ReadOnlySpan<char>) "#B7C2D8", new Color?());
  private static readonly Color StockColor = Color.FromHex((ReadOnlySpan<char>) "#FFD05C", new Color?());
  private static readonly Color EmptyStockColor = Color.FromHex((ReadOnlySpan<char>) "#E0544E", new Color?());
  private readonly BoxContainer _list;
  private readonly LineEdit _thresholdInput;
  private readonly SpriteSystem _sprite;
  private readonly Dictionary<string, CivSupplyRefillWindow.RowRefs> _rows = new Dictionary<string, CivSupplyRefillWindow.RowRefs>();
  private List<string> _lastProtoIds = new List<string>();

  public event Action<string, int>? OnSetPeriodic;

  public event Action<string, int>? OnRefillNow;

  public event Action<int>? OnSetThreshold;

  public CivSupplyRefillWindow()
  {
    this._sprite = IoCManager.Resolve<IEntityManager>().System<SpriteSystem>();
    this.Title = Loc.GetString("civ-supply-refill-title");
    ((Control) this).MinSize = new Vector2(740f, 560f);
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer1.SeparationOverride = new int?(8);
    ((Control) boxContainer1).Margin = new Thickness(8f);
    ((Control) boxContainer1).HorizontalExpand = true;
    ((Control) boxContainer1).VerticalExpand = true;
    BoxContainer boxContainer2 = boxContainer1;
    BoxContainer boxContainer3 = boxContainer2;
    Label label1 = new Label();
    label1.Text = Loc.GetString("civ-supply-refill-hint");
    label1.FontColorOverride = new Color?(CivSupplyRefillWindow.MutedColor);
    ((Control) label1).Margin = new Thickness(2f, 0.0f, 2f, 2f);
    ((Control) boxContainer3).AddChild((Control) label1);
    BoxContainer boxContainer4 = new BoxContainer();
    boxContainer4.Orientation = (BoxContainer.LayoutOrientation) 0;
    boxContainer4.SeparationOverride = new int?(6);
    ((Control) boxContainer4).HorizontalExpand = true;
    BoxContainer boxContainer5 = boxContainer4;
    BoxContainer boxContainer6 = boxContainer5;
    Label label2 = new Label();
    label2.Text = Loc.GetString("civ-supply-refill-threshold-label");
    label2.FontColorOverride = new Color?(CivSupplyRefillWindow.MutedColor);
    ((Control) label2).VerticalAlignment = (Control.VAlignment) 2;
    ((Control) boxContainer6).AddChild((Control) label2);
    LineEdit lineEdit = new LineEdit();
    lineEdit.PlaceHolder = Loc.GetString("civ-supply-refill-threshold-placeholder");
    ((Control) lineEdit).MinWidth = 110f;
    this._thresholdInput = lineEdit;
    this._thresholdInput.OnTextEntered += (Action<LineEdit.LineEditEventArgs>) (args =>
    {
      int result;
      int num = !int.TryParse(args.Text, out result) || result <= 0 ? 0 : result;
      Action<int> onSetThreshold = this.OnSetThreshold;
      if (onSetThreshold == null)
        return;
      onSetThreshold(num);
    });
    ((Control) boxContainer5).AddChild((Control) this._thresholdInput);
    ((Control) boxContainer2).AddChild((Control) boxContainer5);
    PanelContainer panelContainer = new PanelContainer()
    {
      PanelOverride = (StyleBox) CivSupplyRefillWindow.BuildPanelStyle("#1B1F2CEE", "#445574")
    };
    BoxContainer boxContainer7 = new BoxContainer();
    boxContainer7.Orientation = (BoxContainer.LayoutOrientation) 0;
    boxContainer7.SeparationOverride = new int?(8);
    ((Control) boxContainer7).HorizontalExpand = true;
    BoxContainer boxContainer8 = boxContainer7;
    ((Control) boxContainer8).AddChild((Control) CivSupplyRefillWindow.ColLabel(Loc.GetString("civ-supply-refill-col-item"), true));
    ((Control) boxContainer8).AddChild((Control) CivSupplyRefillWindow.ColLabel(Loc.GetString("civ-supply-refill-col-stock"), width: 56));
    ((Control) boxContainer8).AddChild((Control) CivSupplyRefillWindow.ColLabel(Loc.GetString("civ-supply-refill-col-price"), width: 86));
    ((Control) boxContainer8).AddChild((Control) CivSupplyRefillWindow.ColLabel(Loc.GetString("civ-supply-refill-col-target"), width: 60));
    ((Control) boxContainer8).AddChild((Control) CivSupplyRefillWindow.ColLabel(Loc.GetString("civ-supply-refill-col-auto"), width: 96 /*0x60*/));
    ((Control) boxContainer8).AddChild((Control) CivSupplyRefillWindow.ColLabel(Loc.GetString("civ-supply-refill-col-now"), width: 104));
    ((Control) panelContainer).AddChild((Control) boxContainer8);
    ((Control) boxContainer2).AddChild((Control) panelContainer);
    ScrollContainer scrollContainer1 = new ScrollContainer();
    ((Control) scrollContainer1).HorizontalExpand = true;
    ((Control) scrollContainer1).VerticalExpand = true;
    ScrollContainer scrollContainer2 = scrollContainer1;
    BoxContainer boxContainer9 = new BoxContainer();
    boxContainer9.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer9.SeparationOverride = new int?(4);
    ((Control) boxContainer9).HorizontalExpand = true;
    this._list = boxContainer9;
    ((Control) scrollContainer2).AddChild((Control) this._list);
    ((Control) boxContainer2).AddChild((Control) scrollContainer2);
    this.Contents.AddChild((Control) boxContainer2);
  }

  public void Populate(List<CivSupplyRefillEntry> entries)
  {
    List<string> list = entries.Select<CivSupplyRefillEntry, string>((Func<CivSupplyRefillEntry, string>) (e => e.ProtoId)).ToList<string>();
    if (list.SequenceEqual<string>((IEnumerable<string>) this._lastProtoIds))
    {
      foreach (CivSupplyRefillEntry entry in entries)
      {
        CivSupplyRefillWindow.RowRefs r;
        if (this._rows.TryGetValue(entry.ProtoId, out r))
        {
          r.Stock = entry.Count;
          r.CountLabel.Text = $"x{entry.Count}";
          r.CountLabel.FontColorOverride = new Color?(entry.Count <= 0 ? CivSupplyRefillWindow.EmptyStockColor : CivSupplyRefillWindow.StockColor);
          bool active = entry.Periodic > 0;
          if (active != r.Active)
          {
            this.SetRowActive(r, active);
            if (!((Control) r.Input).HasKeyboardFocus())
              r.Input.Text = active ? entry.Periodic.ToString() : string.Empty;
          }
        }
      }
    }
    else
      this.RebuildList(entries, list);
  }

  private void RebuildList(List<CivSupplyRefillEntry> entries, List<string> protoIds)
  {
    ((Control) this._list).RemoveAllChildren();
    this._rows.Clear();
    this._lastProtoIds = protoIds;
    string str = (string) null;
    foreach (CivSupplyRefillEntry entry in entries)
    {
      if (entry.Category != str)
      {
        str = entry.Category;
        BoxContainer list = this._list;
        Label label = new Label();
        label.Text = entry.Category;
        label.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#FFE39A", new Color?()));
        ((Control) label).StyleClasses.Add("LabelHeading");
        ((Control) label).Margin = new Thickness(2f, 8f, 0.0f, 2f);
        ((Control) list).AddChild((Control) label);
      }
      ((Control) this._list).AddChild(this.BuildRow(entry));
    }
  }

  public void SetThreshold(int threshold)
  {
    if (((Control) this._thresholdInput).HasKeyboardFocus())
      return;
    this._thresholdInput.Text = threshold > 0 ? threshold.ToString() : string.Empty;
  }

  private Control BuildRow(CivSupplyRefillEntry entry)
  {
    bool flag = entry.Periodic > 0;
    string proto = entry.ProtoId;
    CivSupplyRefillWindow.RowRefs refs = new CivSupplyRefillWindow.RowRefs()
    {
      Active = flag,
      Stock = entry.Count
    };
    PanelContainer panelContainer = new PanelContainer()
    {
      PanelOverride = (StyleBox) CivSupplyRefillWindow.BuildPanelStyle(flag ? "#1E2A1BF4" : "#141B2BF4", flag ? "#5FA85F" : "#445574")
    };
    refs.Panel = panelContainer;
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 0;
    boxContainer1.SeparationOverride = new int?(8);
    ((Control) boxContainer1).HorizontalExpand = true;
    BoxContainer boxContainer2 = boxContainer1;
    BoxContainer boxContainer3 = boxContainer2;
    TextureRect textureRect = new TextureRect();
    textureRect.Texture = ((IDirectionalTextureProvider) this._sprite.GetPrototypeIcon(entry.ProtoId)).Default;
    textureRect.Stretch = (TextureRect.StretchMode) 7;
    ((Control) textureRect).MinSize = new Vector2(28f, 28f);
    ((Control) textureRect).VerticalAlignment = (Control.VAlignment) 2;
    ((Control) boxContainer3).AddChild((Control) textureRect);
    BoxContainer boxContainer4 = boxContainer2;
    Label label1 = new Label();
    label1.Text = entry.Name;
    ((Control) label1).HorizontalExpand = true;
    label1.ClipText = true;
    ((Control) boxContainer4).AddChild((Control) label1);
    Label label2 = new Label();
    label2.Text = $"x{entry.Count}";
    ((Control) label2).MinWidth = 56f;
    label2.FontColorOverride = new Color?(entry.Count <= 0 ? CivSupplyRefillWindow.EmptyStockColor : CivSupplyRefillWindow.StockColor);
    Label label3 = label2;
    refs.CountLabel = label3;
    ((Control) boxContainer2).AddChild((Control) label3);
    BoxContainer boxContainer5 = boxContainer2;
    Label label4 = new Label();
    label4.Text = Loc.GetString("civ-supply-refill-unit-price", new (string, object)[1]
    {
      ("price", (object) entry.UnitPrice)
    });
    ((Control) label4).MinWidth = 86f;
    label4.FontColorOverride = new Color?(CivSupplyRefillWindow.MutedColor);
    Label label5 = label4;
    ((Control) boxContainer5).AddChild((Control) label5);
    LineEdit lineEdit = new LineEdit();
    lineEdit.PlaceHolder = Loc.GetString("civ-supply-refill-amount-placeholder");
    lineEdit.Text = flag ? entry.Periodic.ToString() : string.Empty;
    ((Control) lineEdit).MinWidth = 60f;
    LineEdit input = lineEdit;
    refs.Input = input;
    ((Control) boxContainer2).AddChild((Control) input);
    Button button1 = new Button();
    button1.Text = Loc.GetString(flag ? "civ-supply-refill-toggle-on" : "civ-supply-refill-toggle-off");
    ((Control) button1).MinWidth = 96f;
    ((Control) button1).ModulateSelfOverride = new Color?(flag ? CivSupplyRefillWindow.OnColor : CivSupplyRefillWindow.OffColor);
    Button button2 = button1;
    refs.Toggle = button2;
    ((BaseButton) button2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      if (refs.Active)
      {
        this.SetRowActive(refs, false);
        Action<string, int> onSetPeriodic = this.OnSetPeriodic;
        if (onSetPeriodic == null)
          return;
        onSetPeriodic(proto, 0);
      }
      else
      {
        int result;
        int num = !int.TryParse(input.Text, out result) || result <= 0 ? (refs.Stock > 0 ? refs.Stock : 1) : result;
        if (string.IsNullOrWhiteSpace(input.Text))
          input.Text = num.ToString();
        this.SetRowActive(refs, true);
        Action<string, int> onSetPeriodic = this.OnSetPeriodic;
        if (onSetPeriodic == null)
          return;
        onSetPeriodic(proto, num);
      }
    });
    ((Control) boxContainer2).AddChild((Control) button2);
    Button button3 = new Button();
    button3.Text = Loc.GetString("civ-supply-refill-now");
    ((Control) button3).MinWidth = 104f;
    Button button4 = button3;
    ((BaseButton) button4).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      int result;
      if (!int.TryParse(input.Text, out result) || result <= 0)
        return;
      Action<string, int> onRefillNow = this.OnRefillNow;
      if (onRefillNow == null)
        return;
      onRefillNow(proto, result);
    });
    ((Control) boxContainer2).AddChild((Control) button4);
    input.OnTextEntered += (Action<LineEdit.LineEditEventArgs>) (args =>
    {
      int result;
      int num = !int.TryParse(args.Text, out result) || result <= 0 ? 0 : result;
      this.SetRowActive(refs, num > 0);
      Action<string, int> onSetPeriodic = this.OnSetPeriodic;
      if (onSetPeriodic == null)
        return;
      onSetPeriodic(proto, num);
    });
    this._rows[proto] = refs;
    ((Control) panelContainer).AddChild((Control) boxContainer2);
    return (Control) panelContainer;
  }

  private void SetRowActive(CivSupplyRefillWindow.RowRefs r, bool active)
  {
    r.Active = active;
    r.Toggle.Text = Loc.GetString(active ? "civ-supply-refill-toggle-on" : "civ-supply-refill-toggle-off");
    ((Control) r.Toggle).ModulateSelfOverride = new Color?(active ? CivSupplyRefillWindow.OnColor : CivSupplyRefillWindow.OffColor);
    r.Panel.PanelOverride = (StyleBox) CivSupplyRefillWindow.BuildPanelStyle(active ? "#1E2A1BF4" : "#141B2BF4", active ? "#5FA85F" : "#445574");
  }

  private static Label ColLabel(string text, bool expand = false, int width = 0)
  {
    Label label = new Label()
    {
      Text = text,
      FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#8FA0BE", new Color?()))
    };
    if (expand)
      ((Control) label).HorizontalExpand = true;
    if (width > 0)
      ((Control) label).MinWidth = (float) width;
    return label;
  }

  private static StyleBoxFlat BuildPanelStyle(string backgroundColor, string borderColor)
  {
    StyleBoxFlat styleBoxFlat = new StyleBoxFlat();
    styleBoxFlat.BackgroundColor = Color.FromHex((ReadOnlySpan<char>) backgroundColor, new Color?());
    styleBoxFlat.BorderColor = Color.FromHex((ReadOnlySpan<char>) borderColor, new Color?());
    styleBoxFlat.BorderThickness = new Thickness(1f);
    ((StyleBox) styleBoxFlat).SetContentMarginOverride((StyleBox.Margin) 15, 8f);
    return styleBoxFlat;
  }

  private sealed class RowRefs
  {
    public PanelContainer Panel;
    public Button Toggle;
    public LineEdit Input;
    public Label CountLabel;
    public bool Active;
    public int Stock;
  }
}
