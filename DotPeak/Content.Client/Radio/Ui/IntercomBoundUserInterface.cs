// Decompiled with JetBrains decompiler
// Type: Content.Client.Radio.Ui.IntercomBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Radio;
using Content.Shared.Radio.Components;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Radio.Ui;

public sealed class IntercomBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private IntercomMenu? _menu;

  protected virtual void Open()
  {
    base.Open();
    this._menu = BoundUserInterfaceExt.CreateWindow<IntercomMenu>((BoundUserInterface) this);
    IntercomComponent intercomComponent;
    if (this.EntMan.TryGetComponent<IntercomComponent>(this.Owner, ref intercomComponent))
      this._menu.Update(Entity<IntercomComponent>.op_Implicit((this.Owner, intercomComponent)));
    this._menu.OnMicPressed += (Action<bool>) (enabled => this.SendMessage((BoundUserInterfaceMessage) new ToggleIntercomMicMessage(enabled)));
    this._menu.OnSpeakerPressed += (Action<bool>) (enabled => this.SendMessage((BoundUserInterfaceMessage) new ToggleIntercomSpeakerMessage(enabled)));
    this._menu.OnChannelSelected += (Action<string>) (channel => this.SendMessage((BoundUserInterfaceMessage) new SelectIntercomChannelMessage(channel)));
  }

  public void Update(Entity<IntercomComponent> ent) => this._menu?.Update(ent);
}
