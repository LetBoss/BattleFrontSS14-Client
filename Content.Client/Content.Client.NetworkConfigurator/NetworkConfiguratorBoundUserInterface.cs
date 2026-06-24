using System;
using System.Collections.Generic;
using Content.Client.NetworkConfigurator.Systems;
using Content.Shared.DeviceNetwork;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.NetworkConfigurator;

public sealed class NetworkConfiguratorBoundUserInterface : BoundUserInterface
{
	private readonly NetworkConfiguratorSystem _netConfig;

	[ViewVariables]
	private NetworkConfiguratorConfigurationMenu? _configurationMenu;

	[ViewVariables]
	private NetworkConfiguratorLinkMenu? _linkMenu;

	[ViewVariables]
	private NetworkConfiguratorListMenu? _listMenu;

	public NetworkConfiguratorBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		_netConfig = base.EntMan.System<NetworkConfiguratorSystem>();
	}

	public void OnRemoveButtonPressed(string address)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new NetworkConfiguratorRemoveDeviceMessage(address));
	}

	protected override void Open()
	{
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		Enum uiKey = base.UiKey;
		if (!(uiKey is NetworkConfiguratorUiKey))
		{
			return;
		}
		switch ((NetworkConfiguratorUiKey)(object)uiKey)
		{
		case NetworkConfiguratorUiKey.List:
			_listMenu = BoundUserInterfaceExt.CreateWindow<NetworkConfiguratorListMenu>((BoundUserInterface)(object)this);
			((BaseButton)_listMenu.ClearButton).OnPressed += delegate
			{
				OnClearButtonPressed();
			};
			_listMenu.OnRemoveAddress += OnRemoveButtonPressed;
			break;
		case NetworkConfiguratorUiKey.Configure:
			_configurationMenu = BoundUserInterfaceExt.CreateWindow<NetworkConfiguratorConfigurationMenu>((BoundUserInterface)(object)this);
			((BaseButton)_configurationMenu.Set).OnPressed += delegate
			{
				OnConfigButtonPressed(NetworkConfiguratorButtonKey.Set);
			};
			((BaseButton)_configurationMenu.Add).OnPressed += delegate
			{
				OnConfigButtonPressed(NetworkConfiguratorButtonKey.Add);
			};
			((BaseButton)_configurationMenu.Clear).OnPressed += delegate
			{
				OnConfigButtonPressed(NetworkConfiguratorButtonKey.Clear);
			};
			((BaseButton)_configurationMenu.Copy).OnPressed += delegate
			{
				OnConfigButtonPressed(NetworkConfiguratorButtonKey.Copy);
			};
			((BaseButton)_configurationMenu.Show).OnPressed += OnShowPressed;
			((BaseButton)_configurationMenu.Show).Pressed = _netConfig.ConfiguredListIsTracked(((BoundUserInterface)this).Owner);
			_configurationMenu.OnRemoveAddress += OnRemoveButtonPressed;
			break;
		case NetworkConfiguratorUiKey.Link:
			_linkMenu = BoundUserInterfaceExt.CreateWindow<NetworkConfiguratorLinkMenu>((BoundUserInterface)(object)this);
			_linkMenu.OnLinkDefaults += delegate(List<(string left, string right)> args)
			{
				((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new NetworkConfiguratorLinksSaveMessage(args));
			};
			_linkMenu.OnToggleLink += delegate(string left, string right)
			{
				((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new NetworkConfiguratorToggleLinkMessage(left, right));
			};
			_linkMenu.OnClearLinks += delegate
			{
				((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new NetworkConfiguratorClearLinksMessage());
			};
			break;
		}
	}

	private void OnShowPressed(ButtonEventArgs args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_netConfig.ToggleVisualization(((BoundUserInterface)this).Owner, args.Button.Pressed);
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (!(state is NetworkConfiguratorUserInterfaceState state2))
		{
			if (!(state is DeviceListUserInterfaceState state3))
			{
				if (state is DeviceLinkUserInterfaceState linkState)
				{
					_linkMenu?.UpdateState(linkState);
				}
			}
			else
			{
				_configurationMenu?.UpdateState(state3);
			}
		}
		else
		{
			_listMenu?.UpdateState(state2);
		}
	}

	private void OnClearButtonPressed()
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new NetworkConfiguratorClearDevicesMessage());
	}

	private void OnConfigButtonPressed(NetworkConfiguratorButtonKey buttonKey)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new NetworkConfiguratorButtonPressedMessage(buttonKey));
	}
}
