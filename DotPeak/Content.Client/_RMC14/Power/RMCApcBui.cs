// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Power.RMCApcBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Message;
using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Power;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Client._RMC14.Power;

public sealed class RMCApcBui(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  [Dependency]
  private IConfigurationManager _config;
  private static readonly Color BlueBackgroundColor = Color.FromHex((ReadOnlySpan<char>) "#3E6189", new Color?());
  private static readonly Color GreenBackgroundColor = Color.FromHex((ReadOnlySpan<char>) "#1B9638", new Color?());
  private static readonly Color GreenColor = Color.FromHex((ReadOnlySpan<char>) "#5AC229", new Color?());
  private static readonly Color OrangeColor = Color.FromHex((ReadOnlySpan<char>) "#C99A29", new Color?());
  private static readonly Color RedColor = Color.FromHex((ReadOnlySpan<char>) "#CE3E31", new Color?());
  [Robust.Shared.ViewVariables.ViewVariables]
  private RMCApcWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<RMCApcWindow>((BoundUserInterface) this);
    ((BaseButton) this._window.CoverButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new RMCApcCoverBuiMsg()));
    foreach (RMCPowerChannel rmcPowerChannel in Enum.GetValues<RMCPowerChannel>())
    {
      RMCApcChannelRow rmcApcChannelRow = new RMCApcChannelRow();
      rmcApcChannelRow.Label.SetMarkupPermissive($"[color=#5B88B0]{rmcPowerChannel}:[/color]");
      ((Control) this._window.Channels).AddChild((Control) rmcApcChannelRow);
    }
    this.Refresh();
  }

  public void Refresh()
  {
    RMCApcWindow window = this._window;
    RMCApcComponent rmcApcComponent;
    if (window == null || !((BaseWindow) window).IsOpen || !this.EntMan.TryGetComponent<RMCApcComponent>(this.Owner, ref rmcApcComponent))
      return;
    this._window.LockedLabel.SetMarkupPermissive(rmcApcComponent.Locked ? "[italic]Swipe an ID card or dogtags to unlock this interface.[/italic]" : "[italic]Swipe an ID card or dogtags to lock this interface.[/italic]");
    this._window.PowerStatusLabel.SetMarkupPermissive(this.Header("Power Status"));
    this._window.PowerChannelsLabel.SetMarkupPermissive(this.Header("Power Channels"));
    this._window.MiscLabel.SetMarkupPermissive(this.Header("Misc"));
    this._window.MainBreakerButton.Text = rmcApcComponent.MainBreakerButton ? "On" : "Off";
    if (rmcApcComponent.MainBreakerButton)
    {
      this._window.MainBreakerButton.Text = "On";
      ((BaseButton) this._window.MainBreakerButton).Pressed = true;
    }
    else
    {
      this._window.MainBreakerButton.Text = "Off";
      ((BaseButton) this._window.MainBreakerButton).Pressed = false;
    }
    this._window.MainBreakerStatus.SetMarkupPermissive(rmcApcComponent.ExternalPower ? this.Green("[ External Power ]") : this.Red("[ No External Power ]"));
    ((Range) this._window.PowerBar).MinValue = 0.0f;
    ((Range) this._window.PowerBar).MaxValue = 1f;
    ((Range) this._window.PowerBar).Value = rmcApcComponent.ChargePercentage;
    this._window.PowerBarLabel.Text = $"{rmcApcComponent.ChargePercentage * 100f:F0}%";
    string markup;
    switch (rmcApcComponent.ChargeStatus)
    {
      case RMCApcChargeStatus.NotCharging:
        markup = this.Red("[ Not Charging ]");
        break;
      case RMCApcChargeStatus.Charging:
        markup = this.Orange("[ Charging ]");
        break;
      case RMCApcChargeStatus.FullCharge:
        markup = this.Green("[ Fully Charged ]");
        break;
      default:
        throw new ArgumentOutOfRangeException();
    }
    this._window.ChargeMode.SetMarkupPermissive(markup);
    this._window.ChargeModeButton.Text = rmcApcComponent.ChargeModeButton ? "Auto" : "Off";
    foreach (int num in Enum.GetValues<RMCPowerChannel>())
    {
      int channel = num;
      RMCApcChannelRow child = (RMCApcChannelRow) ((Control) this._window.Channels).GetChild(channel);
      this.SetButtons(child, rmcApcComponent.Channels[channel]);
      ((BaseButton) child.Auto).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new RMCApcSetChannelBuiMsg((RMCPowerChannel) channel, RMCApcButtonState.Auto)));
      ((Control) child.Off).Visible = false;
    }
    float cvar = this._config.GetCVar<float>(RMCCVars.RMCPowerLoadMultiplier);
    this._window.TotalLoadWatts.SetMarkupPermissive($"[bold]{(float) ((IEnumerable<RMCApcChannel>) rmcApcComponent.Channels).Sum<RMCApcChannel>((Func<RMCApcChannel, int>) (c => c.Watts)) / cvar} W[/bold]");
    this._window.CoverButton.Text = rmcApcComponent.CoverLockedButton ? "Engaged" : "Disengaged";
    ((BaseButton) this._window.CoverButton).Disabled = rmcApcComponent.Locked;
  }

  private string Header(string header) => $"[bold]{header}[/bold]";

  private string Green(string str)
  {
    return $"[color={((Color) ref RMCApcBui.GreenColor).ToHex()}]{str}[/color]";
  }

  private string Orange(string str)
  {
    return $"[color={((Color) ref RMCApcBui.OrangeColor).ToHex()}]{str}[/color]";
  }

  private string Red(string str)
  {
    return $"[color={((Color) ref RMCApcBui.RedColor).ToHex()}]{str}[/color]";
  }

  private void SetButtons(RMCApcChannelRow row, RMCApcChannel channel)
  {
    float cvar = this._config.GetCVar<float>(RMCCVars.RMCPowerLoadMultiplier);
    ((BaseButton) row.Auto).Pressed = channel.Button == RMCApcButtonState.Auto;
    ((BaseButton) row.On).Pressed = channel.Button == RMCApcButtonState.On;
    ((Control) row.On).Visible = false;
    ((BaseButton) row.Off).Pressed = channel.Button == RMCApcButtonState.Off;
    row.Watts.SetMarkupPermissive($"{(float) channel.Watts / cvar} W");
    row.Status.SetMarkupPermissive(channel.On ? this.Green("On") ?? "" : this.Red("Off") ?? "");
  }
}
