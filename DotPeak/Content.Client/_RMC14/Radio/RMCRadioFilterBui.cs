// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Radio.RMCRadioFilterBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Radio;
using Content.Shared.Radio;
using Content.Shared.Radio.Components;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Client._RMC14.Radio;

public sealed class RMCRadioFilterBui(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  [Dependency]
  private IPrototypeManager _prototype;
  [Robust.Shared.ViewVariables.ViewVariables]
  private RMCRadioFilterWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<RMCRadioFilterWindow>((BoundUserInterface) this);
    this.Refresh();
  }

  public void Refresh()
  {
    RMCRadioFilterComponent radioFilterComponent;
    EncryptionKeyHolderComponent keyHolderComponent;
    if (this._window == null || !this.EntMan.TryGetComponent<RMCRadioFilterComponent>(this.Owner, ref radioFilterComponent) || !this.EntMan.TryGetComponent<EncryptionKeyHolderComponent>(this.Owner, ref keyHolderComponent))
      return;
    foreach (string channel1 in keyHolderComponent.Channels)
    {
      string channel = channel1;
      RadioChannelPrototype channelPrototype;
      if (this._prototype.TryIndex<RadioChannelPrototype>(channel, ref channelPrototype))
      {
        CheckBox checkBox1 = new CheckBox();
        checkBox1.Text = Loc.GetString(LocId.op_Implicit(channelPrototype.Name));
        ((BaseButton) checkBox1).Pressed = !radioFilterComponent.DisabledChannels.Contains(ProtoId<RadioChannelPrototype>.op_Implicit(channel));
        CheckBox checkBox2 = checkBox1;
        ((BaseButton) checkBox2).OnToggled += (Action<BaseButton.ButtonToggledEventArgs>) (args => this.SendPredictedMessage((BoundUserInterfaceMessage) new RMCRadioFilterBuiMsg(ProtoId<RadioChannelPrototype>.op_Implicit(channel), args.Pressed)));
        ((Control) this._window.CheckboxContainer).AddChild((Control) checkBox2);
      }
    }
  }
}
