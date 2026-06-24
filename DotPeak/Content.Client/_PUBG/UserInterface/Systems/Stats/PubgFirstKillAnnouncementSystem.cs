// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.Systems.Stats.PubgFirstKillAnnouncementSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Client._PUBG.UserInterface.Systems.Stats;

public sealed class PubgFirstKillAnnouncementSystem : EntitySystem
{
  private static readonly TimeSpan BannerLifetime = TimeSpan.FromSeconds(3L);
  [Dependency]
  private IUserInterfaceManager _ui;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private IPrototypeManager _prototype;
  private PanelContainer? _banner;
  private TimeSpan _hideAt;

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<PubgFirstKillAnnouncementEvent>(new EntitySessionEventHandler<PubgFirstKillAnnouncementEvent>(this.OnAnnouncement), (Type[]) null, (Type[]) null);
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    ((Control) this._banner)?.Orphan();
    this._banner = (PanelContainer) null;
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    if (((Control) this._banner)?.Parent == null || this._timing.CurTime < this._hideAt)
      return;
    ((Control) this._banner).Orphan();
  }

  private void OnAnnouncement(PubgFirstKillAnnouncementEvent msg, EntitySessionEventArgs args)
  {
    UIScreen activeScreen = this._ui.ActiveScreen;
    PubgFirstKillBannerPrototype prototype;
    if (activeScreen == null || !this._prototype.TryIndex<PubgFirstKillBannerPrototype>(msg.BannerPrototypeId, ref prototype))
      return;
    ((Control) this._banner)?.Orphan();
    this._banner = this.BuildBanner(prototype);
    ((Control) activeScreen).AddChild((Control) this._banner);
    LayoutContainer.SetAnchorAndMarginPreset((Control) this._banner, (LayoutContainer.LayoutPreset) 10, (LayoutContainer.LayoutPresetMode) 0, 0);
    LayoutContainer.SetMarginTop((Control) this._banner, 16f);
    this._hideAt = this._timing.CurTime + PubgFirstKillAnnouncementSystem.BannerLifetime;
  }

  private PanelContainer BuildBanner(PubgFirstKillBannerPrototype prototype)
  {
    int num1 = Math.Max(1, prototype.Width);
    int num2 = Math.Max(1, prototype.Height);
    PanelContainer panelContainer = new PanelContainer();
    ((Control) panelContainer).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) panelContainer).MinWidth = (float) num1;
    ((Control) panelContainer).MinHeight = (float) num2;
    panelContainer.PanelOverride = (StyleBox) new StyleBoxFlat()
    {
      BackgroundColor = Color.FromHex((ReadOnlySpan<char>) "#00000000", new Color?()),
      BorderColor = Color.FromHex((ReadOnlySpan<char>) "#00000000", new Color?()),
      BorderThickness = new Thickness(0.0f)
    };
    LayoutContainer layoutContainer1 = new LayoutContainer();
    ((Control) layoutContainer1).MinWidth = (float) num1;
    ((Control) layoutContainer1).MinHeight = (float) num2;
    LayoutContainer layoutContainer2 = layoutContainer1;
    AnimatedTextureRect animatedTextureRect1 = new AnimatedTextureRect();
    ((Control) animatedTextureRect1).HorizontalExpand = true;
    ((Control) animatedTextureRect1).VerticalExpand = true;
    AnimatedTextureRect animatedTextureRect2 = animatedTextureRect1;
    animatedTextureRect2.SetFromSpriteSpecifier((SpriteSpecifier) new SpriteSpecifier.Rsi(prototype.BackgroundRsi, prototype.BackgroundState));
    animatedTextureRect2.DisplayRect.Stretch = (TextureRect.StretchMode) 7;
    LayoutContainer.SetAnchorAndMarginPreset((Control) animatedTextureRect2, (LayoutContainer.LayoutPreset) 15, (LayoutContainer.LayoutPresetMode) 0, 0);
    ((Control) layoutContainer2).AddChild((Control) animatedTextureRect2);
    ((Control) panelContainer).AddChild((Control) layoutContainer2);
    return panelContainer;
  }
}
