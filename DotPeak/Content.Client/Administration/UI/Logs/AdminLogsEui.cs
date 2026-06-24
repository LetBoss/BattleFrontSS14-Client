// Decompiled with JetBrains decompiler
// Type: Content.Client.Administration.UI.Logs.AdminLogsEui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Administration.UI.CustomControls;
using Content.Client.Eui;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Eui;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;

#nullable enable
namespace Content.Client.Administration.UI.Logs;

public sealed class AdminLogsEui : BaseEui
{
  [Dependency]
  private IClyde _clyde;
  [Dependency]
  private IUserInterfaceManager _uiManager;
  [Dependency]
  private IFileDialogManager _dialogManager;
  [Dependency]
  private ILogManager _log;
  private const char CsvSeparator = ',';
  private const string CsvQuote = "\"";
  private const string CsvHeader = "Date,ID,PlayerID,Severity,Type,Message";
  private ISawmill _sawmill;
  private bool _currentlyExportingLogs;

  public AdminLogsEui()
  {
    this.LogsWindow = new AdminLogsWindow();
    ((BaseWindow) this.LogsWindow).OnClose += new Action(this.OnCloseWindow);
    this.LogsControl = this.LogsWindow.Logs;
    this.LogsControl.LogSearch.OnTextEntered += (Action<LineEdit.LineEditEventArgs>) (_ => this.RequestLogs());
    ((BaseButton) this.LogsControl.RefreshButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.RequestLogs());
    ((BaseButton) this.LogsControl.NextButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.NextLogs());
    ((BaseButton) this.LogsControl.PopOutButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.PopOut());
    ((BaseButton) this.LogsControl.ExportLogs).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.ExportLogs());
    this._sawmill = this._log.GetSawmill("admin.logs.ui");
  }

  private WindowRoot? Root { get; set; }

  private IClydeWindow? ClydeWindow { get; set; }

  private AdminLogsWindow? LogsWindow { get; set; }

  private AdminLogsControl LogsControl { get; }

  private bool FirstState { get; set; } = true;

  private void OnRequestClosed(WindowRequestClosedEventArgs args)
  {
    this.SendMessage((EuiMessageBase) new CloseEuiMessage());
  }

  private void OnCloseWindow()
  {
    if (this.ClydeWindow != null)
      return;
    this.SendMessage((EuiMessageBase) new CloseEuiMessage());
  }

  private void RequestLogs()
  {
    this.SendMessage((EuiMessageBase) new AdminLogsEuiMsg.LogsRequest(new int?(this.LogsControl.SelectedRoundId), this.LogsControl.Search, this.LogsControl.SelectedTypes.ToHashSet<LogType>(), (HashSet<LogImpact>) null, new DateTime?(), new DateTime?(), this.LogsControl.SelectedPlayers.Count != 0, this.LogsControl.SelectedPlayers.ToArray<Guid>(), (Guid[]) null, this.LogsControl.IncludeNonPlayerLogs, DateOrder.Descending));
  }

  private void NextLogs()
  {
    ((BaseButton) this.LogsControl.NextButton).Disabled = true;
    this.SendMessage((EuiMessageBase) new AdminLogsEuiMsg.NextLogsRequest());
  }

  private async void ExportLogs()
  {
    if (this._currentlyExportingLogs)
      return;
    this._currentlyExportingLogs = true;
    ((BaseButton) this.LogsControl.ExportLogs).Disabled = true;
    (Stream, bool)? file = await this._dialogManager.SaveFile(new FileDialogFilters(new FileDialogFilters.Group[1]
    {
      new FileDialogFilters.Group(new string[1]{ "csv" })
    }), true, FileAccess.ReadWrite, FileShare.None);
    if (!file.HasValue)
      return;
    try
    {
      int num1;
      int num2 = num1 - 1;
      try
      {
        StreamWriter writer;
        object obj;
        if ((uint) (num1 - 1) > 14U)
        {
          writer = new StreamWriter(file.Value.Item1, bufferSize: 4096 /*0x1000*/);
          obj = (object) null;
          int num = 0;
        }
        try
        {
          Control.OrderedChildCollection.Enumerator enumerator;
          if ((uint) (num1 - 2) > 13U)
          {
            await writer.WriteLineAsync("Date,ID,PlayerID,Severity,Type,Message");
            enumerator = ((Control) this.LogsControl.LogsContainer).Children.GetEnumerator();
          }
          try
          {
            while (((Control.OrderedChildCollection.Enumerator) ref enumerator).MoveNext())
            {
              Control current = ((Control.OrderedChildCollection.Enumerator) ref enumerator).Current;
              if (current is AdminLogLabel adminLogLabel && current.Visible)
              {
                SharedAdminLog log = adminLogLabel.Log;
                await writer.WriteAsync(log.Date.ToString("s", (IFormatProvider) CultureInfo.InvariantCulture));
                await writer.WriteAsync(',');
                await writer.WriteAsync(log.Id.ToString());
                await writer.WriteAsync(',');
                Guid[] players = log.Players;
                for (int i = 0; i < players.Length; ++i)
                  await writer.WriteAsync(players[i].ToString() + (i == players.Length - 1 ? "" : " "));
                await writer.WriteAsync(',');
                await writer.WriteAsync(log.Impact.ToString());
                await writer.WriteAsync(',');
                await writer.WriteAsync(log.Type.ToString());
                await writer.WriteAsync(',');
                await writer.WriteAsync("\"");
                await writer.WriteAsync(log.Message.Replace("\"", "\"\""));
                await writer.WriteAsync("\"");
                await writer.WriteLineAsync();
                log = new SharedAdminLog();
                players = (Guid[]) null;
              }
            }
          }
          finally
          {
            enumerator.Dispose();
          }
          enumerator = new Control.OrderedChildCollection.Enumerator();
        }
        catch (object ex)
        {
          obj = ex;
        }
        if (writer != null)
          await writer.DisposeAsync();
        object obj1 = obj;
        if (obj1 != null)
        {
          if (!(obj1 is Exception source))
            throw obj1;
          ExceptionDispatchInfo.Capture(source).Throw();
        }
        obj = (object) null;
        writer = (StreamWriter) null;
      }
      catch (Exception ex)
      {
        this._sawmill.Error("Error when exporting admin log:\n" + ex.StackTrace);
      }
    }
    finally
    {
      await file.Value.Item1.DisposeAsync();
      this._currentlyExportingLogs = false;
      ((BaseButton) this.LogsControl.ExportLogs).Disabled = false;
    }
  }

  private void PopOut()
  {
    if (this.LogsWindow == null)
      return;
    IClydeMonitor iclydeMonitor = this._clyde.EnumerateMonitors().First<IClydeMonitor>();
    this.ClydeWindow = this._clyde.CreateWindow(new WindowCreateParameters()
    {
      Maximized = false,
      Title = "Admin Logs",
      Monitor = iclydeMonitor,
      Width = 1100,
      Height = 400
    });
    this.LogsControl.Orphan();
    ((Control) this.LogsWindow).Orphan();
    this.LogsWindow = (AdminLogsWindow) null;
    this.ClydeWindow.RequestClosed += new Action<WindowRequestClosedEventArgs>(this.OnRequestClosed);
    this.ClydeWindow.DisposeOnClose = true;
    this.Root = this._uiManager.CreateWindowRoot(this.ClydeWindow);
    ((Control) this.Root).AddChild((Control) this.LogsControl);
    ((BaseButton) this.LogsControl.PopOutButton).Disabled = true;
    ((Control) this.LogsControl.PopOutButton).Visible = false;
  }

  public override void HandleState(EuiStateBase state)
  {
    AdminLogsEuiState adminLogsEuiState = (AdminLogsEuiState) state;
    if (adminLogsEuiState.IsLoading)
      return;
    this.LogsControl.SetCurrentRound(adminLogsEuiState.RoundId);
    this.LogsControl.SetPlayers(adminLogsEuiState.Players);
    AdminLogsControl logsControl = this.LogsControl;
    int? nullable = new int?(adminLogsEuiState.RoundLogs);
    int? shown = new int?();
    int? total = new int?();
    int? round = nullable;
    logsControl.UpdateCount(shown, total, round);
    if (!this.FirstState)
      return;
    this.FirstState = false;
    this.LogsControl.SetRoundSpinBox(adminLogsEuiState.RoundId);
    this.RequestLogs();
  }

  public override void HandleMessage(EuiMessageBase msg)
  {
    base.HandleMessage(msg);
    switch (msg)
    {
      case AdminLogsEuiMsg.NewLogs newLogs:
        if (newLogs.Replace)
          this.LogsControl.SetLogs(newLogs.Logs);
        else
          this.LogsControl.AddLogs(newLogs.Logs);
        ((BaseButton) this.LogsControl.NextButton).Disabled = !newLogs.HasNext;
        break;
      case AdminLogsEuiMsg.SetLogFilter setLogFilter:
        if (setLogFilter.Search != null)
          this.LogsControl.LogSearch.SetText(setLogFilter.Search, false);
        if (setLogFilter.Types == null)
          break;
        this.LogsControl.SetTypesSelection(setLogFilter.Types, setLogFilter.InvertTypes);
        break;
    }
  }

  public override void Opened()
  {
    base.Opened();
    ((BaseWindow) this.LogsWindow)?.OpenCentered();
  }

  public override void Closed()
  {
    base.Closed();
    if (this.ClydeWindow != null)
      this.ClydeWindow.RequestClosed -= new Action<WindowRequestClosedEventArgs>(this.OnRequestClosed);
    this.LogsControl.Orphan();
    ((Control) this.LogsWindow)?.Orphan();
    ((Control) this.Root)?.Orphan();
    ((IDisposable) this.ClydeWindow)?.Dispose();
  }
}
