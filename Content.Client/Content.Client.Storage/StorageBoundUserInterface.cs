using System;
using System.Numerics;
using Content.Client.UserInterface.Systems.Storage;
using Content.Client.UserInterface.Systems.Storage.Controls;
using Content.Shared.Storage;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Storage;

public sealed class StorageBoundUserInterface : BoundUserInterface
{
	private StorageWindow? _window;

	public Vector2? Position
	{
		get
		{
			StorageWindow? window = _window;
			if (window == null)
			{
				return null;
			}
			return ((Control)window).Position;
		}
	}

	public StorageBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_window = IoCManager.Resolve<IUserInterfaceManager>().GetUIController<StorageUIController>().CreateStorageWindow(this);
		StorageComponent item = default(StorageComponent);
		if (base.EntMan.TryGetComponent<StorageComponent>(((BoundUserInterface)this).Owner, ref item))
		{
			_window.UpdateContainer(Entity<StorageComponent>.op_Implicit((((BoundUserInterface)this).Owner, item)));
		}
		((BaseWindow)_window).OnClose += base.Close;
		_window.FlagDirty();
	}

	public void Refresh()
	{
		_window?.FlagDirty();
	}

	public void Reclaim()
	{
		if (_window != null)
		{
			((BaseWindow)_window).OnClose -= base.Close;
			((Control)_window).Orphan();
			_window = null;
		}
	}

	protected override void Dispose(bool disposing)
	{
		((BoundUserInterface)this).Dispose(disposing);
		Reclaim();
	}

	public void CloseWindow(Vector2 position)
	{
		if (_window != null)
		{
			LayoutContainer.SetPosition((Control)(object)_window, position);
			StorageWindow? window = _window;
			if (window != null)
			{
				((BaseWindow)window).Close();
			}
		}
	}

	public void Hide()
	{
		if (_window != null)
		{
			((Control)_window).Visible = false;
		}
	}

	public void Show()
	{
		if (_window != null)
		{
			((Control)_window).Visible = true;
		}
	}

	public void Show(Vector2 position)
	{
		if (_window != null)
		{
			Show();
			LayoutContainer.SetPosition((Control)(object)_window, position);
		}
	}
}
