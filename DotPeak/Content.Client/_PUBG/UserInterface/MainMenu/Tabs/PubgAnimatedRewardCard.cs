// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.MainMenu.Tabs.PubgAnimatedRewardCard
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Client._PUBG.UserInterface.MainMenu.Tabs;

public sealed class PubgAnimatedRewardCard : Control
{
  private static readonly Color GoldAccent = Color.FromHex((ReadOnlySpan<char>) "#FFB800", new Color?());
  private static readonly Color GreenSuccess = Color.FromHex((ReadOnlySpan<char>) "#00FF88", new Color?());
  private static readonly Color AccentGlow = Color.FromHex((ReadOnlySpan<char>) "#FFD700", new Color?());
  private static readonly Color PremiumGlow = Color.FromHex((ReadOnlySpan<char>) "#FF8C00", new Color?());
  private static readonly Color CompletedGlow = Color.FromHex((ReadOnlySpan<char>) "#00FFB3", new Color?());
  private bool _canClaim;
  private bool _isClaimed;
  private bool _isPremium;
  private bool _canClaimPremium;
  private bool _hovered;
  private Control? _content;
  [Dependency]
  private IGameTiming _timing;

  public event Action? OnClaimPressed;

  public bool CanClaim
  {
    get => this._canClaim;
    set
    {
      this._canClaim = value;
      this.InvalidateMeasure();
    }
  }

  public bool IsClaimed
  {
    get => this._isClaimed;
    set
    {
      this._isClaimed = value;
      this.InvalidateMeasure();
    }
  }

  public bool IsPremium
  {
    get => this._isPremium;
    set
    {
      this._isPremium = value;
      this.InvalidateMeasure();
    }
  }

  public bool CanClaimPremium
  {
    get => this._canClaimPremium;
    set
    {
      this._canClaimPremium = value;
      this.InvalidateMeasure();
    }
  }

  public PubgAnimatedRewardCard()
  {
    IoCManager.InjectDependencies<PubgAnimatedRewardCard>(this);
    this.MouseFilter = (Control.MouseFilterMode) 0;
  }

  public void SetContent(Control content)
  {
    this._content?.Orphan();
    this._content = content;
    this.AddChild(content);
  }

  protected virtual void Draw(DrawingHandleScreen handle)
  {
    base.Draw(handle);
    UIBox2 uiBox2_1;
    // ISSUE: explicit constructor call
    ((UIBox2) ref uiBox2_1).\u002Ector(0.0f, 0.0f, (float) this.PixelSize.X, (float) this.PixelSize.Y);
    if (this._canClaim && this._canClaimPremium && !this._isClaimed)
    {
      float num = (float) (0.25 + (double) MathF.Sin((float) this._timing.RealTime.TotalSeconds * 2.5f) * 0.15000000596046448);
      Color color = this._isPremium ? PubgAnimatedRewardCard.PremiumGlow : PubgAnimatedRewardCard.GreenSuccess;
      UIBox2 uiBox2_2;
      // ISSUE: explicit constructor call
      ((UIBox2) ref uiBox2_2).\u002Ector(uiBox2_1.Left - 3f, uiBox2_1.Top - 3f, uiBox2_1.Right + 3f, uiBox2_1.Bottom + 3f);
      handle.DrawRect(uiBox2_2, ((Color) ref color).WithAlpha(num), true);
      UIBox2 uiBox2_3;
      // ISSUE: explicit constructor call
      ((UIBox2) ref uiBox2_3).\u002Ector(uiBox2_1.Left - 1f, uiBox2_1.Top - 1f, uiBox2_1.Right + 1f, uiBox2_1.Bottom + 1f);
      handle.DrawRect(uiBox2_3, ((Color) ref color).WithAlpha(num * 1.5f), true);
    }
    if (this._hovered && this._canClaim && this._canClaimPremium && !this._isClaimed)
    {
      Color color = this._isPremium ? PubgAnimatedRewardCard.AccentGlow : PubgAnimatedRewardCard.GreenSuccess;
      UIBox2 uiBox2_4;
      // ISSUE: explicit constructor call
      ((UIBox2) ref uiBox2_4).\u002Ector(uiBox2_1.Left - 2f, uiBox2_1.Top - 2f, uiBox2_1.Right + 2f, uiBox2_1.Bottom + 2f);
      handle.DrawRect(uiBox2_4, ((Color) ref color).WithAlpha(0.3f), true);
    }
    if (!this._isClaimed)
      return;
    UIBox2 uiBox2_5;
    // ISSUE: explicit constructor call
    ((UIBox2) ref uiBox2_5).\u002Ector(uiBox2_1.Left - 1f, uiBox2_1.Top - 1f, uiBox2_1.Right + 1f, uiBox2_1.Bottom + 1f);
    handle.DrawRect(uiBox2_5, ((Color) ref PubgAnimatedRewardCard.CompletedGlow).WithAlpha(0.1f), true);
  }

  protected virtual void MouseEntered()
  {
    base.MouseEntered();
    if (!this._canClaim || !this._canClaimPremium || this._isClaimed)
      return;
    this._hovered = true;
    this.UserInterfaceManager.HoverSound();
  }

  protected virtual void MouseExited()
  {
    base.MouseExited();
    this._hovered = false;
  }

  protected virtual void KeyBindDown(GUIBoundKeyEventArgs args)
  {
    base.KeyBindDown(args);
    if (BoundKeyFunction.op_Inequality(((BoundKeyEventArgs) args).Function, EngineKeyFunctions.UIClick) || !this._canClaim || !this._canClaimPremium || this._isClaimed)
      return;
    this.UserInterfaceManager.ClickSound();
    Action onClaimPressed = this.OnClaimPressed;
    if (onClaimPressed != null)
      onClaimPressed();
    ((BoundKeyEventArgs) args).Handle();
  }

  protected virtual void FrameUpdate(FrameEventArgs args)
  {
    base.FrameUpdate(args);
    if (!this._canClaim || !this._canClaimPremium || this._isClaimed)
      return;
    this.InvalidateMeasure();
  }
}
