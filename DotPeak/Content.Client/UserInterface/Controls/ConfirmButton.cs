// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Controls.ConfirmButton
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Client.UserInterface.Controls;

public sealed class ConfirmButton : Button
{
  [Dependency]
  private IGameTiming _gameTiming;
  public const string ConfirmPrefix = "confirm-";
  private TimeSpan? _nextReset;
  private TimeSpan? _nextCooldown;
  private string? _confirmationText;
  private string? _text;
  [Robust.Shared.ViewVariables.ViewVariables]
  public bool IsConfirming;

  public event Action<BaseButton.ButtonEventArgs>? OnPressed;

  public string? Text
  {
    get => this._text;
    set
    {
      this._text = value;
      base.Text = this.IsConfirming ? this._confirmationText : value;
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  public string ConfirmationText
  {
    get => this._confirmationText ?? Loc.GetString("generic-confirm");
    set => this._confirmationText = value;
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  public TimeSpan ResetTime { get; set; } = TimeSpan.FromSeconds(2L);

  [Robust.Shared.ViewVariables.ViewVariables]
  public TimeSpan CooldownTime { get; set; } = TimeSpan.FromSeconds(0.5);

  public ConfirmButton()
  {
    IoCManager.InjectDependencies<ConfirmButton>(this);
    ((BaseButton) this).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.HandleOnPressed);
  }

  protected virtual void FrameUpdate(FrameEventArgs args)
  {
    if (this.IsConfirming)
    {
      TimeSpan curTime = this._gameTiming.CurTime;
      TimeSpan? nextReset = this._nextReset;
      if ((nextReset.HasValue ? (curTime > nextReset.GetValueOrDefault() ? 1 : 0) : 0) != 0)
      {
        this.IsConfirming = false;
        base.Text = this.Text;
        ((BaseButton) this).DrawModeChanged();
      }
    }
    if (!((BaseButton) this).Disabled)
      return;
    TimeSpan curTime1 = this._gameTiming.CurTime;
    TimeSpan? nextCooldown = this._nextCooldown;
    if ((nextCooldown.HasValue ? (curTime1 > nextCooldown.GetValueOrDefault() ? 1 : 0) : 0) == 0)
      return;
    ((BaseButton) this).Disabled = false;
  }

  protected virtual void DrawModeChanged()
  {
    if (this.IsConfirming)
    {
      switch ((int) ((BaseButton) this).DrawMode)
      {
        case 0:
          ((Control) this).SetOnlyStylePseudoClass("confirm-normal");
          break;
        case 1:
          ((Control) this).SetOnlyStylePseudoClass("confirm-pressed");
          break;
        case 2:
          ((Control) this).SetOnlyStylePseudoClass("confirm-hover");
          break;
        case 3:
          ((Control) this).SetOnlyStylePseudoClass("confirm-disabled");
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }
    else
      ((ContainerButton) this).DrawModeChanged();
  }

  private void HandleOnPressed(BaseButton.ButtonEventArgs buttonEvent)
  {
    if (this.IsConfirming)
    {
      TimeSpan? nextCooldown = this._nextCooldown;
      TimeSpan curTime = this._gameTiming.CurTime;
      if ((nextCooldown.HasValue ? (nextCooldown.GetValueOrDefault() > curTime ? 1 : 0) : 0) != 0)
        return;
    }
    if (!this.IsConfirming)
    {
      this._nextCooldown = new TimeSpan?(this._gameTiming.CurTime + this.CooldownTime);
      this._nextReset = new TimeSpan?(this._gameTiming.CurTime + this.ResetTime);
      ((BaseButton) this).Disabled = true;
    }
    else
    {
      Action<BaseButton.ButtonEventArgs> onPressed = this.OnPressed;
      if (onPressed != null)
        onPressed(buttonEvent);
    }
    base.Text = this.IsConfirming ? this.Text : this.ConfirmationText;
    this.IsConfirming = !this.IsConfirming;
  }
}
