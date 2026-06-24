// Decompiled with JetBrains decompiler
// Type: Content.Client.CriminalRecords.CriminalRecordsConsoleBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Access.Systems;
using Content.Shared.CriminalRecords;
using Content.Shared.CriminalRecords.Components;
using Content.Shared.Security;
using Content.Shared.StationRecords;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using System;

#nullable enable
namespace Content.Client.CriminalRecords;

public sealed class CriminalRecordsConsoleBoundUserInterface : BoundUserInterface
{
  [Dependency]
  private IPrototypeManager _proto;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private IPlayerManager _playerManager;
  private readonly AccessReaderSystem _accessReader;
  private CriminalRecordsConsoleWindow? _window;
  private CrimeHistoryWindow? _historyWindow;

  public CriminalRecordsConsoleBoundUserInterface(EntityUid owner, Enum uiKey)
    : base(owner, uiKey)
  {
    this._accessReader = this.EntMan.System<AccessReaderSystem>();
  }

  protected virtual void Open()
  {
    base.Open();
    CriminalRecordsConsoleComponent component = this.EntMan.GetComponent<CriminalRecordsConsoleComponent>(this.Owner);
    this._window = new CriminalRecordsConsoleWindow(this.Owner, component.MaxStringLength, this._playerManager, this._proto, this._random, this._accessReader);
    this._window.OnKeySelected += (Action<uint?>) (key => this.SendMessage((BoundUserInterfaceMessage) new SelectStationRecord(key)));
    this._window.OnFiltersChanged += (Action<StationRecordFilterType, string>) ((type, filterValue) => this.SendMessage((BoundUserInterfaceMessage) new SetStationRecordFilter(type, filterValue)));
    this._window.OnStatusSelected += (Action<SecurityStatus>) (status => this.SendMessage((BoundUserInterfaceMessage) new CriminalRecordChangeStatus(status, (string) null)));
    this._window.OnDialogConfirmed += (Action<SecurityStatus, string>) ((status, reason) => this.SendMessage((BoundUserInterfaceMessage) new CriminalRecordChangeStatus(status, reason)));
    this._window.OnStatusFilterPressed += (Action<SecurityStatus>) (statusFilter => this.SendMessage((BoundUserInterfaceMessage) new CriminalRecordSetStatusFilter(statusFilter)));
    this._window.OnHistoryUpdated += new Action<CriminalRecord, bool, bool>(this.UpdateHistory);
    this._window.OnHistoryClosed += (Action) (() => this._historyWindow?.Close());
    this._window.OnClose += new Action(((BoundUserInterface) this).Close);
    this._historyWindow = new CrimeHistoryWindow(component.MaxStringLength);
    this._historyWindow.OnAddHistory += (Action<string>) (line => this.SendMessage((BoundUserInterfaceMessage) new CriminalRecordAddHistory(line)));
    this._historyWindow.OnDeleteHistory += (Action<uint>) (index => this.SendMessage((BoundUserInterfaceMessage) new CriminalRecordDeleteHistory(index)));
    this._historyWindow.Close();
  }

  private void UpdateHistory(CriminalRecord record, bool access, bool open)
  {
    this._historyWindow.UpdateHistory(record, access);
    if (!open)
      return;
    this._historyWindow.OpenCentered();
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (!(state is CriminalRecordsConsoleState state1))
      return;
    this._window?.UpdateState(state1);
  }

  protected virtual void Dispose(bool disposing)
  {
    base.Dispose(disposing);
    this._window?.Close();
    this._historyWindow?.Close();
  }
}
