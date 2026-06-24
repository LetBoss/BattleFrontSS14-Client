using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Content.Client.Administration.UI.CustomControls;
using Content.Client.Eui;
using Content.Shared.Administration.Logs;
using Content.Shared.Eui;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.IoC;
using Robust.Shared.Log;

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

	private WindowRoot? Root { get; set; }

	private IClydeWindow? ClydeWindow { get; set; }

	private AdminLogsWindow? LogsWindow { get; set; }

	private AdminLogsControl LogsControl { get; }

	private bool FirstState { get; set; } = true;

	public AdminLogsEui()
	{
		LogsWindow = new AdminLogsWindow();
		((BaseWindow)LogsWindow).OnClose += OnCloseWindow;
		LogsControl = LogsWindow.Logs;
		LogsControl.LogSearch.OnTextEntered += delegate
		{
			RequestLogs();
		};
		((BaseButton)LogsControl.RefreshButton).OnPressed += delegate
		{
			RequestLogs();
		};
		((BaseButton)LogsControl.NextButton).OnPressed += delegate
		{
			NextLogs();
		};
		((BaseButton)LogsControl.PopOutButton).OnPressed += delegate
		{
			PopOut();
		};
		((BaseButton)LogsControl.ExportLogs).OnPressed += delegate
		{
			ExportLogs();
		};
		_sawmill = _log.GetSawmill("admin.logs.ui");
	}

	private void OnRequestClosed(WindowRequestClosedEventArgs args)
	{
		SendMessage(new CloseEuiMessage());
	}

	private void OnCloseWindow()
	{
		if (ClydeWindow == null)
		{
			SendMessage(new CloseEuiMessage());
		}
	}

	private void RequestLogs()
	{
		AdminLogsEuiMsg.LogsRequest msg = new AdminLogsEuiMsg.LogsRequest(LogsControl.SelectedRoundId, LogsControl.Search, LogsControl.SelectedTypes.ToHashSet(), null, null, null, LogsControl.SelectedPlayers.Count != 0, LogsControl.SelectedPlayers.ToArray(), null, LogsControl.IncludeNonPlayerLogs, DateOrder.Descending);
		SendMessage(msg);
	}

	private void NextLogs()
	{
		((BaseButton)LogsControl.NextButton).Disabled = true;
		AdminLogsEuiMsg.NextLogsRequest msg = new AdminLogsEuiMsg.NextLogsRequest();
		SendMessage(msg);
	}

	private unsafe async void ExportLogs()
	{
		if (_currentlyExportingLogs)
		{
			return;
		}
		_currentlyExportingLogs = true;
		((BaseButton)LogsControl.ExportLogs).Disabled = true;
		(Stream fileStream, bool alreadyExisted)? file = await _dialogManager.SaveFile(new FileDialogFilters((Group[])(object)new Group[1]
		{
			new Group(new string[1] { "csv" })
		}), true, FileAccess.ReadWrite, FileShare.None);
		if (!file.HasValue)
		{
			return;
		}
		try
		{
			int num;
			_ = num - 1;
			_ = 15;
			try
			{
				await using StreamWriter writer = new StreamWriter(file.Value.fileStream, null, 4096, false);
				await writer.WriteLineAsync("Date,ID,PlayerID,Severity,Type,Message");
				Enumerator val = ((Control)LogsControl.LogsContainer).Children.GetEnumerator();
				try
				{
					while (((Enumerator)(ref val)).MoveNext())
					{
						Control current = ((Enumerator)(ref val)).Current;
						if (current is AdminLogLabel adminLogLabel && current.Visible)
						{
							SharedAdminLog log = adminLogLabel.Log;
							await writer.WriteAsync(log.Date.ToString("s", CultureInfo.InvariantCulture));
							await writer.WriteAsync(',');
							await writer.WriteAsync(log.Id.ToString());
							await writer.WriteAsync(',');
							Guid[] players = log.Players;
							for (int i = 0; i < players.Length; i++)
							{
								await writer.WriteAsync(players[i].ToString() + ((i == players.Length - 1) ? "" : " "));
							}
							await writer.WriteAsync(',');
							await writer.WriteAsync(log.Impact.ToString());
							await writer.WriteAsync(',');
							await writer.WriteAsync(log.Type.ToString());
							await writer.WriteAsync(',');
							await writer.WriteAsync("\"");
							await writer.WriteAsync(log.Message.Replace("\"", "\"\""));
							await writer.WriteAsync("\"");
							await writer.WriteLineAsync();
						}
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&val))/*cast due to constrained. prefix*/).Dispose();
				}
				val = default(Enumerator);
			}
			catch (Exception ex)
			{
				_sawmill.Error("Error when exporting admin log:\n" + ex.StackTrace);
			}
		}
		finally
		{
			await file.Value.fileStream.DisposeAsync();
			_currentlyExportingLogs = false;
			((BaseButton)LogsControl.ExportLogs).Disabled = false;
		}
	}

	private void PopOut()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
		if (LogsWindow != null)
		{
			IClydeMonitor monitor = _clyde.EnumerateMonitors().First();
			ClydeWindow = _clyde.CreateWindow(new WindowCreateParameters
			{
				Maximized = false,
				Title = "Admin Logs",
				Monitor = monitor,
				Width = 1100,
				Height = 400
			});
			((Control)LogsControl).Orphan();
			((Control)LogsWindow).Orphan();
			LogsWindow = null;
			ClydeWindow.RequestClosed += OnRequestClosed;
			ClydeWindow.DisposeOnClose = true;
			Root = _uiManager.CreateWindowRoot(ClydeWindow);
			((Control)Root).AddChild((Control)(object)LogsControl);
			((BaseButton)LogsControl.PopOutButton).Disabled = true;
			((Control)LogsControl.PopOutButton).Visible = false;
		}
	}

	public override void HandleState(EuiStateBase state)
	{
		AdminLogsEuiState adminLogsEuiState = (AdminLogsEuiState)state;
		if (!adminLogsEuiState.IsLoading)
		{
			LogsControl.SetCurrentRound(adminLogsEuiState.RoundId);
			LogsControl.SetPlayers(adminLogsEuiState.Players);
			AdminLogsControl logsControl = LogsControl;
			int? round = adminLogsEuiState.RoundLogs;
			logsControl.UpdateCount(null, null, round);
			if (FirstState)
			{
				FirstState = false;
				LogsControl.SetRoundSpinBox(adminLogsEuiState.RoundId);
				RequestLogs();
			}
		}
	}

	public override void HandleMessage(EuiMessageBase msg)
	{
		base.HandleMessage(msg);
		if (!(msg is AdminLogsEuiMsg.NewLogs newLogs))
		{
			if (msg is AdminLogsEuiMsg.SetLogFilter setLogFilter)
			{
				if (setLogFilter.Search != null)
				{
					LogsControl.LogSearch.SetText(setLogFilter.Search, false);
				}
				if (setLogFilter.Types != null)
				{
					LogsControl.SetTypesSelection(setLogFilter.Types, setLogFilter.InvertTypes);
				}
			}
		}
		else
		{
			if (newLogs.Replace)
			{
				LogsControl.SetLogs(newLogs.Logs);
			}
			else
			{
				LogsControl.AddLogs(newLogs.Logs);
			}
			((BaseButton)LogsControl.NextButton).Disabled = !newLogs.HasNext;
		}
	}

	public override void Opened()
	{
		base.Opened();
		AdminLogsWindow? logsWindow = LogsWindow;
		if (logsWindow != null)
		{
			((BaseWindow)logsWindow).OpenCentered();
		}
	}

	public override void Closed()
	{
		base.Closed();
		if (ClydeWindow != null)
		{
			ClydeWindow.RequestClosed -= OnRequestClosed;
		}
		((Control)LogsControl).Orphan();
		AdminLogsWindow? logsWindow = LogsWindow;
		if (logsWindow != null)
		{
			((Control)logsWindow).Orphan();
		}
		WindowRoot? root = Root;
		if (root != null)
		{
			((Control)root).Orphan();
		}
		((IDisposable)ClydeWindow)?.Dispose();
	}
}
