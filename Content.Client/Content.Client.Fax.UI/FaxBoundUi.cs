using System;
using System.IO;
using Content.Shared.Fax;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.ViewVariables;

namespace Content.Client.Fax.UI;

public sealed class FaxBoundUi : BoundUserInterface
{
	[Dependency]
	private IFileDialogManager _fileDialogManager;

	[ViewVariables]
	private FaxWindow? _window;

	private bool _dialogIsOpen;

	public FaxBoundUi(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<FaxWindow>((BoundUserInterface)(object)this);
		_window.FileButtonPressed += OnFileButtonPressed;
		_window.CopyButtonPressed += OnCopyButtonPressed;
		_window.SendButtonPressed += OnSendButtonPressed;
		_window.RefreshButtonPressed += OnRefreshButtonPressed;
		_window.PeerSelected += OnPeerSelected;
	}

	private async void OnFileButtonPressed()
	{
		if (_dialogIsOpen)
		{
			return;
		}
		_dialogIsOpen = true;
		FileDialogFilters val = new FileDialogFilters((Group[])(object)new Group[1]
		{
			new Group(new string[1] { "txt" })
		});
		await using Stream file = await _fileDialogManager.OpenFile(val, FileAccess.ReadWrite, (FileShare?)null);
		_dialogIsOpen = false;
		if (_window == null || ((Control)_window).Disposed || file == null)
		{
			return;
		}
		using StreamReader reader = new StreamReader(file);
		string firstLine = await reader.ReadLineAsync();
		string label = null;
		string text = await reader.ReadToEndAsync();
		if (firstLine != null)
		{
			if (firstLine.StartsWith('#'))
			{
				string text2 = firstLine;
				label = text2.Substring(1, text2.Length - 1).Trim();
			}
			else
			{
				text = firstLine + "\n" + text;
			}
		}
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new FaxFileMessage(label?.Substring(0, Math.Min(label.Length, 50)), text.Substring(0, Math.Min(text.Length, 10000)), _window.OfficePaper));
	}

	private void OnSendButtonPressed()
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new FaxSendMessage());
	}

	private void OnCopyButtonPressed()
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new FaxCopyMessage());
	}

	private void OnRefreshButtonPressed()
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new FaxRefreshMessage());
	}

	private void OnPeerSelected(string address)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new FaxDestinationMessage(address));
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (_window != null && state is FaxUiState state2)
		{
			_window.UpdateState(state2);
		}
	}
}
