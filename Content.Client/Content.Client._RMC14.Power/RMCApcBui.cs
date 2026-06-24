using System;
using System.Linq;
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
using Robust.Shared.ViewVariables;

namespace Content.Client._RMC14.Power;

public sealed class RMCApcBui : BoundUserInterface
{
	[Dependency]
	private IConfigurationManager _config;

	private static readonly Color BlueBackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#3E6189", (Color?)null);

	private static readonly Color GreenBackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#1B9638", (Color?)null);

	private static readonly Color GreenColor = Color.FromHex((ReadOnlySpan<char>)"#5AC229", (Color?)null);

	private static readonly Color OrangeColor = Color.FromHex((ReadOnlySpan<char>)"#C99A29", (Color?)null);

	private static readonly Color RedColor = Color.FromHex((ReadOnlySpan<char>)"#CE3E31", (Color?)null);

	[ViewVariables]
	private RMCApcWindow? _window;

	public RMCApcBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<RMCApcWindow>((BoundUserInterface)(object)this);
		((BaseButton)_window.CoverButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new RMCApcCoverBuiMsg());
		};
		RMCPowerChannel[] values = Enum.GetValues<RMCPowerChannel>();
		foreach (RMCPowerChannel value in values)
		{
			RMCApcChannelRow rMCApcChannelRow = new RMCApcChannelRow();
			rMCApcChannelRow.Label.SetMarkupPermissive($"[color=#5B88B0]{value}:[/color]");
			((Control)_window.Channels).AddChild((Control)(object)rMCApcChannelRow);
		}
		Refresh();
	}

	public void Refresh()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		RMCApcWindow window = _window;
		RMCApcComponent rMCApcComponent = default(RMCApcComponent);
		if (window == null || !((BaseWindow)window).IsOpen || !base.EntMan.TryGetComponent<RMCApcComponent>(((BoundUserInterface)this).Owner, ref rMCApcComponent))
		{
			return;
		}
		string markup = (rMCApcComponent.Locked ? "[italic]Swipe an ID card or dogtags to unlock this interface.[/italic]" : "[italic]Swipe an ID card or dogtags to lock this interface.[/italic]");
		_window.LockedLabel.SetMarkupPermissive(markup);
		_window.PowerStatusLabel.SetMarkupPermissive(Header("Power Status"));
		_window.PowerChannelsLabel.SetMarkupPermissive(Header("Power Channels"));
		_window.MiscLabel.SetMarkupPermissive(Header("Misc"));
		_window.MainBreakerButton.Text = (rMCApcComponent.MainBreakerButton ? "On" : "Off");
		if (rMCApcComponent.MainBreakerButton)
		{
			_window.MainBreakerButton.Text = "On";
			((BaseButton)_window.MainBreakerButton).Pressed = true;
		}
		else
		{
			_window.MainBreakerButton.Text = "Off";
			((BaseButton)_window.MainBreakerButton).Pressed = false;
		}
		_window.MainBreakerStatus.SetMarkupPermissive(rMCApcComponent.ExternalPower ? Green("[ External Power ]") : Red("[ No External Power ]"));
		((Range)_window.PowerBar).MinValue = 0f;
		((Range)_window.PowerBar).MaxValue = 1f;
		((Range)_window.PowerBar).Value = rMCApcComponent.ChargePercentage;
		_window.PowerBarLabel.Text = $"{rMCApcComponent.ChargePercentage * 100f:F0}%";
		string markup2 = rMCApcComponent.ChargeStatus switch
		{
			RMCApcChargeStatus.NotCharging => Red("[ Not Charging ]"), 
			RMCApcChargeStatus.Charging => Orange("[ Charging ]"), 
			RMCApcChargeStatus.FullCharge => Green("[ Fully Charged ]"), 
			_ => throw new ArgumentOutOfRangeException(), 
		};
		_window.ChargeMode.SetMarkupPermissive(markup2);
		_window.ChargeModeButton.Text = (rMCApcComponent.ChargeModeButton ? "Auto" : "Off");
		RMCPowerChannel[] values = Enum.GetValues<RMCPowerChannel>();
		for (int i = 0; i < values.Length; i++)
		{
			int channel = (int)values[i];
			RMCApcChannelRow rMCApcChannelRow = (RMCApcChannelRow)(object)((Control)_window.Channels).GetChild(channel);
			SetButtons(rMCApcChannelRow, rMCApcComponent.Channels[channel]);
			((BaseButton)rMCApcChannelRow.Auto).OnPressed += delegate
			{
				((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new RMCApcSetChannelBuiMsg((RMCPowerChannel)channel, RMCApcButtonState.Auto));
			};
			((Control)rMCApcChannelRow.Off).Visible = false;
		}
		float cVar = _config.GetCVar<float>(RMCCVars.RMCPowerLoadMultiplier);
		int num = rMCApcComponent.Channels.Sum((RMCApcChannel c) => c.Watts);
		_window.TotalLoadWatts.SetMarkupPermissive($"[bold]{(float)num / cVar} W[/bold]");
		_window.CoverButton.Text = (rMCApcComponent.CoverLockedButton ? "Engaged" : "Disengaged");
		((BaseButton)_window.CoverButton).Disabled = rMCApcComponent.Locked;
	}

	private string Header(string header)
	{
		return "[bold]" + header + "[/bold]";
	}

	private string Green(string str)
	{
		return $"[color={((Color)(ref GreenColor)).ToHex()}]{str}[/color]";
	}

	private string Orange(string str)
	{
		return $"[color={((Color)(ref OrangeColor)).ToHex()}]{str}[/color]";
	}

	private string Red(string str)
	{
		return $"[color={((Color)(ref RedColor)).ToHex()}]{str}[/color]";
	}

	private void SetButtons(RMCApcChannelRow row, RMCApcChannel channel)
	{
		float cVar = _config.GetCVar<float>(RMCCVars.RMCPowerLoadMultiplier);
		((BaseButton)row.Auto).Pressed = channel.Button == RMCApcButtonState.Auto;
		((BaseButton)row.On).Pressed = channel.Button == RMCApcButtonState.On;
		((Control)row.On).Visible = false;
		((BaseButton)row.Off).Pressed = channel.Button == RMCApcButtonState.Off;
		row.Watts.SetMarkupPermissive($"{(float)channel.Watts / cVar} W");
		row.Status.SetMarkupPermissive(channel.On ? (Green("On") ?? "") : (Red("Off") ?? ""));
	}
}
