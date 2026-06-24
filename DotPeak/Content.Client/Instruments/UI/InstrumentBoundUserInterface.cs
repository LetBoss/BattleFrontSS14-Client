// Decompiled with JetBrains decompiler
// Type: Content.Client.Instruments.UI.InstrumentBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.ActionBlocker;
using Content.Shared.Instruments.UI;
using Content.Shared.Interaction;
using Robust.Client.Audio.Midi;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using System;

#nullable enable
namespace Content.Client.Instruments.UI;

public sealed class InstrumentBoundUserInterface : BoundUserInterface
{
  [Dependency]
  public IMidiManager MidiManager;
  [Dependency]
  public IFileDialogManager FileDialogManager;
  [Dependency]
  public ILocalizationManager Loc;
  public readonly InstrumentSystem Instruments;
  public readonly ActionBlockerSystem ActionBlocker;
  public readonly SharedInteractionSystem Interactions;
  [Robust.Shared.ViewVariables.ViewVariables]
  private InstrumentMenu? _instrumentMenu;
  [Robust.Shared.ViewVariables.ViewVariables]
  private BandMenu? _bandMenu;
  [Robust.Shared.ViewVariables.ViewVariables]
  private ChannelsMenu? _channelsMenu;

  public IEntityManager Entities => this.EntMan;

  public InstrumentBoundUserInterface(EntityUid owner, Enum uiKey)
    : base(owner, uiKey)
  {
    IoCManager.InjectDependencies<InstrumentBoundUserInterface>(this);
    this.Instruments = this.Entities.System<InstrumentSystem>();
    this.ActionBlocker = this.Entities.System<ActionBlockerSystem>();
    this.Interactions = this.Entities.System<SharedInteractionSystem>();
  }

  protected virtual void ReceiveMessage(BoundUserInterfaceMessage message)
  {
    if (!(message is InstrumentBandResponseBuiMessage responseBuiMessage))
      return;
    this._bandMenu?.Populate(responseBuiMessage.Nearby, this.EntMan);
  }

  protected virtual void Open()
  {
    base.Open();
    this._instrumentMenu = BoundUserInterfaceExt.CreateWindow<InstrumentMenu>((BoundUserInterface) this);
    this._instrumentMenu.Title = this.EntMan.GetComponent<MetaDataComponent>(this.Owner).EntityName;
    this._instrumentMenu.OnOpenBand += new Action(this.OpenBandMenu);
    this._instrumentMenu.OnOpenChannels += new Action(this.OpenChannelsMenu);
    this._instrumentMenu.OnCloseChannels += new Action(this.CloseChannelsMenu);
    this._instrumentMenu.OnCloseBands += new Action(this.CloseBandMenu);
    this._instrumentMenu.SetMIDI(this.MidiManager.IsAvailable);
    InstrumentComponent instrumentComponent;
    if (!this.EntMan.TryGetComponent<InstrumentComponent>(this.Owner, ref instrumentComponent))
      return;
    this._instrumentMenu.SetInstrument(Entity<InstrumentComponent>.op_Implicit((this.Owner, instrumentComponent)));
  }

  protected virtual void Dispose(bool disposing)
  {
    base.Dispose(disposing);
    if (!disposing)
      return;
    InstrumentComponent component;
    if (this.EntMan.TryGetComponent<InstrumentComponent>(this.Owner, ref component))
      this._instrumentMenu?.RemoveInstrument(component);
    ((Control) this._bandMenu)?.Orphan();
    ((Control) this._channelsMenu)?.Orphan();
  }

  public void RefreshBands()
  {
    this.SendMessage((BoundUserInterfaceMessage) new InstrumentBandRequestBuiMessage());
  }

  public void OpenBandMenu()
  {
    if (this._bandMenu == null)
      this._bandMenu = new BandMenu(this);
    InstrumentComponent instrumentComponent;
    if (this.EntMan.TryGetComponent<InstrumentComponent>(this.Owner, ref instrumentComponent))
      this._bandMenu.Master = instrumentComponent.Master;
    this.RefreshBands();
    ((BaseWindow) this._bandMenu).OpenCenteredLeft();
  }

  public void CloseBandMenu()
  {
    BandMenu bandMenu = this._bandMenu;
    if ((bandMenu != null ? (((BaseWindow) bandMenu).IsOpen ? 1 : 0) : 0) == 0)
      return;
    ((BaseWindow) this._bandMenu).Close();
  }

  public void OpenChannelsMenu()
  {
    if (this._channelsMenu == null)
      this._channelsMenu = new ChannelsMenu(this);
    this._channelsMenu.Populate();
    ((BaseWindow) this._channelsMenu).OpenCenteredRight();
  }

  public void CloseChannelsMenu()
  {
    ChannelsMenu channelsMenu = this._channelsMenu;
    if ((channelsMenu != null ? (((BaseWindow) channelsMenu).IsOpen ? 1 : 0) : 0) == 0)
      return;
    ((BaseWindow) this._channelsMenu).Close();
  }
}
