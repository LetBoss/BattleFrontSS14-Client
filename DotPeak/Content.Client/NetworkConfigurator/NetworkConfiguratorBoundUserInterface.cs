// Decompiled with JetBrains decompiler
// Type: Content.Client.NetworkConfigurator.NetworkConfiguratorBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.NetworkConfigurator.Systems;
using Content.Shared.DeviceNetwork;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.NetworkConfigurator;

public sealed class NetworkConfiguratorBoundUserInterface : BoundUserInterface
{
  private readonly NetworkConfiguratorSystem _netConfig;
  [Robust.Shared.ViewVariables.ViewVariables]
  private NetworkConfiguratorConfigurationMenu? _configurationMenu;
  [Robust.Shared.ViewVariables.ViewVariables]
  private NetworkConfiguratorLinkMenu? _linkMenu;
  [Robust.Shared.ViewVariables.ViewVariables]
  private NetworkConfiguratorListMenu? _listMenu;

  public NetworkConfiguratorBoundUserInterface(EntityUid owner, Enum uiKey)
    : base(owner, uiKey)
  {
    this._netConfig = this.EntMan.System<NetworkConfiguratorSystem>();
  }

  public void OnRemoveButtonPressed(string address)
  {
    this.SendMessage((BoundUserInterfaceMessage) new NetworkConfiguratorRemoveDeviceMessage(address));
  }

  protected virtual void Open()
  {
    base.Open();
    if (!(this.UiKey is NetworkConfiguratorUiKey uiKey))
      return;
    switch (uiKey)
    {
      case NetworkConfiguratorUiKey.List:
        this._listMenu = BoundUserInterfaceExt.CreateWindow<NetworkConfiguratorListMenu>((BoundUserInterface) this);
        ((BaseButton) this._listMenu.ClearButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.OnClearButtonPressed());
        this._listMenu.OnRemoveAddress += new Action<string>(this.OnRemoveButtonPressed);
        break;
      case NetworkConfiguratorUiKey.Configure:
        this._configurationMenu = BoundUserInterfaceExt.CreateWindow<NetworkConfiguratorConfigurationMenu>((BoundUserInterface) this);
        ((BaseButton) this._configurationMenu.Set).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.OnConfigButtonPressed(NetworkConfiguratorButtonKey.Set));
        ((BaseButton) this._configurationMenu.Add).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.OnConfigButtonPressed(NetworkConfiguratorButtonKey.Add));
        ((BaseButton) this._configurationMenu.Clear).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.OnConfigButtonPressed(NetworkConfiguratorButtonKey.Clear));
        ((BaseButton) this._configurationMenu.Copy).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.OnConfigButtonPressed(NetworkConfiguratorButtonKey.Copy));
        ((BaseButton) this._configurationMenu.Show).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.OnShowPressed);
        ((BaseButton) this._configurationMenu.Show).Pressed = this._netConfig.ConfiguredListIsTracked(this.Owner);
        this._configurationMenu.OnRemoveAddress += new Action<string>(this.OnRemoveButtonPressed);
        break;
      case NetworkConfiguratorUiKey.Link:
        this._linkMenu = BoundUserInterfaceExt.CreateWindow<NetworkConfiguratorLinkMenu>((BoundUserInterface) this);
        this._linkMenu.OnLinkDefaults += (Action<List<(string, string)>>) (args => this.SendMessage((BoundUserInterfaceMessage) new NetworkConfiguratorLinksSaveMessage(args)));
        this._linkMenu.OnToggleLink += (Action<string, string>) ((left, right) => this.SendMessage((BoundUserInterfaceMessage) new NetworkConfiguratorToggleLinkMessage(left, right)));
        this._linkMenu.OnClearLinks += (Action) (() => this.SendMessage((BoundUserInterfaceMessage) new NetworkConfiguratorClearLinksMessage()));
        break;
    }
  }

  private void OnShowPressed(BaseButton.ButtonEventArgs args)
  {
    this._netConfig.ToggleVisualization(this.Owner, args.Button.Pressed);
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    switch (state)
    {
      case NetworkConfiguratorUserInterfaceState state1:
        this._listMenu?.UpdateState(state1);
        break;
      case DeviceListUserInterfaceState state2:
        this._configurationMenu?.UpdateState(state2);
        break;
      case DeviceLinkUserInterfaceState linkState:
        this._linkMenu?.UpdateState(linkState);
        break;
    }
  }

  private void OnClearButtonPressed()
  {
    this.SendMessage((BoundUserInterfaceMessage) new NetworkConfiguratorClearDevicesMessage());
  }

  private void OnConfigButtonPressed(NetworkConfiguratorButtonKey buttonKey)
  {
    this.SendMessage((BoundUserInterfaceMessage) new NetworkConfiguratorButtonPressedMessage(buttonKey));
  }
}
