// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Intel.TechControlConsoleBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._RMC14.UserInterface;
using Content.Shared._RMC14.Intel.Tech;
using Content.Shared.FixedPoint;
using Content.Shared.GameTicking;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client._RMC14.Intel;

public sealed class TechControlConsoleBui : BoundUserInterface
{
  [Dependency]
  private IEntityManager _entities;
  private TechControlConsoleWindow? _window;
  private TechControlConsoleOptionWindow? _optionWindow;
  private readonly SharedGameTicker _ticker;
  private readonly SpriteSystem _sprite;

  public TechControlConsoleBui(EntityUid owner, Enum uiKey)
    : base(owner, uiKey)
  {
    this._ticker = this._entities.System<SharedGameTicker>();
    this._sprite = this._entities.System<SpriteSystem>();
  }

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<TechControlConsoleWindow>((BoundUserInterface) this);
    this.Refresh();
  }

  public void Refresh()
  {
    TechControlConsoleWindow window = this._window;
    TechControlConsoleComponent console;
    if (window == null || !((BaseWindow) window).IsOpen || !this.EntMan.TryGetComponent<TechControlConsoleComponent>(this.Owner, ref console))
      return;
    ((Control) this._window.Options).DisposeAllChildren();
    for (int index1 = console.Tree.Options.Count - 1; index1 >= 0; --index1)
    {
      BoxContainer boxContainer1 = new BoxContainer()
      {
        Orientation = (BoxContainer.LayoutOrientation) 0
      };
      ((Control) boxContainer1).AddChild((Control) new RichTextLabel()
      {
        Text = Loc.GetString("rmc-ui-tech-tier-header", new (string, object)[1]
        {
          ("tier", (object) index1)
        })
      });
      if (index1 == console.Tree.Options.Count - 1)
      {
        ((Control) boxContainer1).AddChild(new Control()
        {
          HorizontalExpand = true
        });
        ((Control) boxContainer1).AddChild((Control) new RichTextLabel()
        {
          Text = Loc.GetString("rmc-ui-tech-points", new (string, object)[1]
          {
            ("points", (object) console.Tree.Points)
          })
        });
      }
      ((Control) this._window.Options).AddChild((Control) boxContainer1);
      ((Control) this._window.Options).AddChild((Control) new BlueHorizontalSeparator());
      BoxContainer boxContainer2 = new BoxContainer()
      {
        Orientation = (BoxContainer.LayoutOrientation) 0
      };
      ((Control) this._window.Options).AddChild((Control) boxContainer2);
      List<TechOption> option1 = console.Tree.Options[index1];
      for (int index2 = 0; index2 < option1.Count; ++index2)
      {
        TechOption option = option1[index2];
        Control control1 = new Control();
        Texture texture = ((IDirectionalTextureProvider) this._sprite.RsiStateLike((SpriteSpecifier) option.Icon)).TextureFor((Direction) 0);
        Control control2 = control1;
        TextureButton textureButton1 = new TextureButton();
        textureButton1.TextureNormal = texture;
        ((Control) textureButton1).SetSize = new Vector2(64f, 64f);
        control2.AddChild((Control) textureButton1);
        SpriteSpecifier.Rsi rsi = option.Purchased ? console.UnlockedRsi : console.LockedRsi;
        TextureButton textureButton2 = new TextureButton()
        {
          TextureNormal = ((IDirectionalTextureProvider) this._sprite.RsiStateLike((SpriteSpecifier) rsi)).TextureFor((Direction) 0),
          Scale = new Vector2(2f, 2f)
        };
        control1.AddChild((Control) textureButton2);
        int tier = index1;
        int optionIndex = index2;
        ((BaseButton) textureButton2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.OpenOptionWindow(option, tier, optionIndex, console.Tree.Points, console.Tree.Tier));
        ((Control) textureButton2).ToolTip = option.Name;
        ((Control) textureButton2).TooltipDelay = new float?(0.1f);
        ((Control) boxContainer2).AddChild(new Control()
        {
          HorizontalExpand = true
        });
        ((Control) boxContainer2).AddChild(control1);
        if (index2 == option1.Count - 1)
          ((Control) boxContainer2).AddChild(new Control()
          {
            HorizontalExpand = true
          });
      }
    }
  }

  private void OpenOptionWindow(
    TechOption option,
    int tier,
    int optionIndex,
    FixedPoint2 points,
    int currentTier)
  {
    TechControlConsoleOptionWindow optionWindow = this._optionWindow;
    if (optionWindow != null && ((BaseWindow) optionWindow).IsOpen)
    {
      ((BaseWindow) this._optionWindow).Close();
      this._optionWindow = (TechControlConsoleOptionWindow) null;
    }
    this._optionWindow = new TechControlConsoleOptionWindow();
    ((BaseWindow) this._optionWindow).OpenCentered();
    ((BaseWindow) this._optionWindow).OnClose += (Action) (() => this._optionWindow = (TechControlConsoleOptionWindow) null);
    this._optionWindow.Title = option.Name;
    this._optionWindow.CurrentPointsLabel.Text = Loc.GetString("rmc-ui-tech-points-value", new (string, object)[1]
    {
      ("value", (object) points.Double().ToString("F1"))
    });
    this._optionWindow.NameLabel.Text = option.Name;
    this._optionWindow.DescriptionLabel.Text = option.Description;
    this._optionWindow.CostLabel.Text = $"{option.CurrentCost}";
    ((Control) this._optionWindow.Statistics).DisposeAllChildren();
    bool flag1 = false;
    if (option.Repurchasable)
    {
      flag1 = true;
      ((Control) this._optionWindow.Statistics).AddChild((Control) new Label()
      {
        Text = Loc.GetString("rmc-ui-tech-repurchasable")
      });
    }
    if (option.Increase != 0)
    {
      flag1 = true;
      ((Control) this._optionWindow.Statistics).AddChild((Control) new Label()
      {
        Text = Loc.GetString("rmc-ui-tech-incremental-price", new (string, object)[1]
        {
          ("increase", (object) option.Increase)
        })
      });
    }
    ((Control) this._optionWindow.StatisticsContainer).Visible = flag1;
    bool flag2 = points >= option.CurrentCost && currentTier >= tier && (!option.Purchased || option.Repurchasable) && option.TimeLock < this._ticker.RoundDuration();
    this._optionWindow.PurchaseButton.Text = Loc.GetString("rmc-ui-tech-purchase-button");
    ((BaseButton) this._optionWindow.PurchaseButton).Disabled = !flag2;
    ((BaseButton) this._optionWindow.PurchaseButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      this.SendPredictedMessage((BoundUserInterfaceMessage) new TechPurchaseOptionBuiMsg(tier, optionIndex));
      ((BaseWindow) this._optionWindow).Close();
    });
  }
}
