// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.MainMenu.Tabs.Shop.PubgShopHeroCaseCard
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG.Shop;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client._PUBG.UserInterface.MainMenu.Tabs.Shop;

public sealed class PubgShopHeroCaseCard : PanelContainer
{
  [Dependency]
  private IEntityManager _entityManager;
  [Dependency]
  private IPrototypeManager _prototypeManager;
  private readonly TextureRect _iconView;
  private readonly Label _titleLabel;
  private readonly Label _descriptionLabel;
  private readonly Label _priceLabel;
  private readonly Label _statusLabel;
  private readonly Button _ctaButton;
  private bool _featureEnabled;

  public event Action? OnPressed;

  public PubgShopHeroCaseCard()
  {
    IoCManager.InjectDependencies<PubgShopHeroCaseCard>(this);
    StyleBoxFlat styleBoxFlat = new StyleBoxFlat();
    styleBoxFlat.BackgroundColor = Color.FromHex((ReadOnlySpan<char>) "#1A1410", new Color?());
    styleBoxFlat.BorderColor = Color.FromHex((ReadOnlySpan<char>) "#FFB800", new Color?());
    styleBoxFlat.BorderThickness = new Thickness(2f);
    ((StyleBox) styleBoxFlat).ContentMarginLeftOverride = new float?(10f);
    ((StyleBox) styleBoxFlat).ContentMarginTopOverride = new float?(10f);
    ((StyleBox) styleBoxFlat).ContentMarginRightOverride = new float?(10f);
    ((StyleBox) styleBoxFlat).ContentMarginBottomOverride = new float?(10f);
    this.PanelOverride = (StyleBox) styleBoxFlat;
    ((Control) this).MinHeight = 126f;
    ((Control) this).MaxHeight = 146f;
    ((Control) this).HorizontalExpand = true;
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 0;
    ((Control) boxContainer1).HorizontalExpand = true;
    ((Control) boxContainer1).VerticalExpand = true;
    BoxContainer boxContainer2 = boxContainer1;
    TextureRect textureRect = new TextureRect();
    ((Control) textureRect).MinSize = new Vector2(76f, 80f);
    textureRect.Stretch = (TextureRect.StretchMode) 7;
    ((Control) textureRect).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) textureRect).VerticalAlignment = (Control.VAlignment) 2;
    this._iconView = textureRect;
    BoxContainer boxContainer3 = new BoxContainer();
    boxContainer3.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer3).HorizontalExpand = true;
    ((Control) boxContainer3).VerticalExpand = true;
    ((Control) boxContainer3).Margin = new Thickness(10f, 0.0f, 0.0f, 0.0f);
    BoxContainer boxContainer4 = boxContainer3;
    Label label1 = new Label();
    label1.Text = Loc.GetString("mainmenu-shop-hero-case-title");
    label1.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#FFD36A", new Color?()));
    ((Control) label1).HorizontalAlignment = (Control.HAlignment) 1;
    this._titleLabel = label1;
    ((Control) this._titleLabel).SetOnlyStyleClass("LabelHeading");
    Label label2 = new Label();
    label2.Text = Loc.GetString("mainmenu-shop-hero-case-description");
    ((Control) label2).HorizontalAlignment = (Control.HAlignment) 1;
    label2.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#D6CBA7", new Color?()));
    ((Control) label2).Margin = new Thickness(0.0f, 4f, 0.0f, 0.0f);
    ((Control) label2).MaxWidth = 250f;
    label2.ClipText = true;
    this._descriptionLabel = label2;
    Label label3 = new Label();
    label3.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#FFDF8C", new Color?()));
    ((Control) label3).HorizontalAlignment = (Control.HAlignment) 1;
    ((Control) label3).Margin = new Thickness(0.0f, 4f, 0.0f, 0.0f);
    this._priceLabel = label3;
    Label label4 = new Label();
    label4.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#FF8F8F", new Color?()));
    ((Control) label4).HorizontalAlignment = (Control.HAlignment) 1;
    ((Control) label4).Margin = new Thickness(0.0f, 4f, 0.0f, 0.0f);
    this._statusLabel = label4;
    Button button = new Button();
    button.Text = Loc.GetString("mainmenu-shop-hero-case-soon");
    ((BaseButton) button).Disabled = true;
    ((Control) button).MinHeight = 30f;
    ((Control) button).HorizontalAlignment = (Control.HAlignment) 1;
    ((Control) button).Margin = new Thickness(0.0f, 6f, 0.0f, 0.0f);
    this._ctaButton = button;
    ((Control) this._ctaButton).StyleClasses.Add("OpenBoth");
    ((BaseButton) this._ctaButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      Action onPressed = this.OnPressed;
      if (onPressed == null)
        return;
      onPressed();
    });
    ((Control) boxContainer2).AddChild((Control) this._iconView);
    ((Control) boxContainer4).AddChild((Control) this._titleLabel);
    ((Control) boxContainer4).AddChild((Control) this._descriptionLabel);
    ((Control) boxContainer4).AddChild((Control) this._priceLabel);
    ((Control) boxContainer4).AddChild((Control) this._statusLabel);
    ((Control) boxContainer4).AddChild((Control) this._ctaButton);
    ((Control) boxContainer2).AddChild((Control) boxContainer4);
    ((Control) this).AddChild((Control) boxContainer2);
  }

  public void UpdateFeature(PubgShopFeaturePrototype? feature)
  {
    if (feature == null || !feature.Enabled)
    {
      ((Control) this).Visible = false;
      this._featureEnabled = false;
    }
    else
    {
      ((Control) this).Visible = true;
      this._featureEnabled = true;
      this._titleLabel.Text = Loc.GetString(feature.TitleLocKey);
      this._descriptionLabel.Text = Loc.GetString(feature.DescriptionLocKey);
      string str = feature.Currency == "coins" ? Loc.GetString("mainmenu-shop-currency-coins") : Loc.GetString("mainmenu-shop-currency-premium");
      this._priceLabel.Text = Loc.GetString("mainmenu-shop-card-price", new (string, object)[2]
      {
        ("price", (object) feature.Price),
        ("currency", (object) str)
      });
      Texture texture = (Texture) null;
      SpriteSystem spriteSystem = this._entityManager.System<SpriteSystem>();
      if (feature.Icon != null)
      {
        texture = spriteSystem.Frame0(feature.Icon);
      }
      else
      {
        EntityPrototype entityPrototype;
        if (!string.IsNullOrWhiteSpace(feature.IconEntity) && this._prototypeManager.TryIndex<EntityPrototype>(feature.IconEntity, ref entityPrototype))
          texture = ((IDirectionalTextureProvider) spriteSystem.GetPrototypeIcon(entityPrototype)).Default;
      }
      this._iconView.Texture = texture;
      this._statusLabel.Text = string.Empty;
      this._ctaButton.Text = Loc.GetString("mainmenu-shop-hero-case-open");
      ((BaseButton) this._ctaButton).Disabled = false;
    }
  }

  public void UpdateAvailability(
    bool enabled,
    bool hasEnoughCurrency,
    bool pending,
    string? errorCode,
    int price,
    string currency)
  {
    if (!this._featureEnabled || !enabled)
    {
      ((BaseButton) this._ctaButton).Disabled = true;
      this._ctaButton.Text = Loc.GetString("mainmenu-shop-hero-case-soon");
      this._statusLabel.Text = string.Empty;
    }
    else
    {
      string str1 = currency == "premium" ? Loc.GetString("mainmenu-shop-currency-premium") : Loc.GetString("mainmenu-shop-currency-coins");
      if (pending)
      {
        ((BaseButton) this._ctaButton).Disabled = true;
        this._ctaButton.Text = Loc.GetString("mainmenu-shop-hero-case-opening");
        this._statusLabel.Text = Loc.GetString("mainmenu-shop-hero-case-opening-status");
      }
      else
      {
        ((BaseButton) this._ctaButton).Disabled = !hasEnoughCurrency;
        this._ctaButton.Text = Loc.GetString("mainmenu-shop-hero-case-open-for", new (string, object)[2]
        {
          (nameof (price), (object) price),
          (nameof (currency), (object) str1)
        });
        if (!hasEnoughCurrency)
          this._statusLabel.Text = Loc.GetString("mainmenu-shop-hero-case-error-not-enough-currency");
        else if (string.IsNullOrWhiteSpace(errorCode))
        {
          this._statusLabel.Text = string.Empty;
        }
        else
        {
          string str2;
          switch (errorCode)
          {
            case "CASE_NOT_FOUND":
              str2 = "mainmenu-shop-hero-case-error-case-not-found";
              break;
            case "CASE_DISABLED":
              str2 = "mainmenu-shop-hero-case-error-case-disabled";
              break;
            case "CASE_POOL_EMPTY":
              str2 = "mainmenu-shop-hero-case-error-case-pool-empty";
              break;
            case "NOT_ENOUGH_COINS":
              str2 = "mainmenu-shop-hero-case-error-not-enough-coins";
              break;
            case "NOT_ENOUGH_PREMIUM":
              str2 = "mainmenu-shop-hero-case-error-not-enough-premium";
              break;
            case "PLAYER_NOT_FOUND":
              str2 = "mainmenu-shop-hero-case-error-player-not-found";
              break;
            default:
              str2 = "mainmenu-shop-hero-case-error-internal";
              break;
          }
          this._statusLabel.Text = Loc.GetString(str2);
        }
      }
    }
  }
}
