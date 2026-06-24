using System;
using System.Runtime.InteropServices;
using Content.Client._RMC14.UserInterface;
using Content.Client.Eye;
using Content.Client.Message;
using Content.Client.UserInterface.ControlExtensions;
using Content.Shared._RMC14.Camera;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.Localization;

namespace Content.Client._RMC14.Camera;

public sealed class RMCCameraBui : RMCPopOutBui<RMCCameraWindow>
{
	private EntityUid? _currentCamera;

	private Button? _currentCameraButton;

	private readonly EyeLerpingSystem _eyeLerping;

	private readonly RMCCameraSystem _system;

	protected override RMCCameraWindow? Window { get; set; }

	public RMCCameraBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		_eyeLerping = ((BoundUserInterface)this).EntMan.System<EyeLerpingSystem>();
		_system = ((BoundUserInterface)this).EntMan.System<RMCCameraSystem>();
	}

	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		Window = ((BoundUserInterface)(object)this).CreatePopOutableWindow<RMCCameraWindow>();
		Window.SearchBar.OnTextChanged += delegate
		{
			RefreshSearch();
		};
		Window.PreviousCameraButton.Text = "<";
		Window.NextCameraButton.Text = ">";
		((BaseButton)Window.PreviousCameraButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new RMCCameraPreviousBuiMsg());
		};
		((BaseButton)Window.NextCameraButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new RMCCameraNextBuiMsg());
		};
		Refresh();
	}

	public void Refresh()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		RMCCameraComputerComponent rMCCameraComputerComponent = default(RMCCameraComputerComponent);
		if (Window == null || !((BoundUserInterface)this).EntMan.TryGetComponent<RMCCameraComputerComponent>(((BoundUserInterface)this).Owner, ref rMCCameraComputerComponent))
		{
			return;
		}
		LocId? title = rMCCameraComputerComponent.Title;
		if (title.HasValue)
		{
			LocId valueOrDefault = title.GetValueOrDefault();
			((DefaultWindow)Window).Title = Loc.GetString(LocId.op_Implicit(valueOrDefault));
		}
		NetEntity? netEntity = ((BoundUserInterface)this).EntMan.GetNetEntity(rMCCameraComputerComponent.CurrentCamera, (MetaDataComponent)null);
		Span<NetEntity> span = CollectionsMarshal.AsSpan(rMCCameraComputerComponent.CameraIds);
		Span<string> span2 = CollectionsMarshal.AsSpan(rMCCameraComputerComponent.CameraNames);
		for (int i = 0; i < span.Length; i++)
		{
			if (i >= span2.Length)
			{
				continue;
			}
			RMCCameraButton button;
			if (i < ((Control)Window.CamerasContainer).ChildCount)
			{
				if (!(((Control)Window.CamerasContainer).GetChild(i) is RMCCameraButton rMCCameraButton))
				{
					continue;
				}
				button = rMCCameraButton;
			}
			else
			{
				button = new RMCCameraButton();
				((Control)Window.CamerasContainer).AddChild((Control)(object)button);
			}
			NetEntity id = span[i];
			string text = span2[i];
			button.TextLabel.SetMarkupPermissive("[font size=11][color=white]" + text + "[/color][/font]");
			RMCCameraButton rMCCameraButton2 = button;
			NetEntity val = id;
			NetEntity? val2 = netEntity;
			((BaseButton)rMCCameraButton2).Pressed = val2.HasValue && val == val2.GetValueOrDefault();
			((BaseButton)button).OnPressed += delegate
			{
				//IL_0036: Unknown result type (might be due to invalid IL or missing references)
				if (_currentCameraButton != null)
				{
					((BaseButton)_currentCameraButton).Pressed = false;
				}
				_currentCameraButton = (Button?)(object)button;
				((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new RMCCameraWatchBuiMsg(id));
			};
		}
		for (int num = ((Control)Window.CamerasContainer).ChildCount - 1; num >= span.Length; num--)
		{
			((Control)Window.CamerasContainer).RemoveChild(num);
		}
		RefreshSearch();
		RefreshCamera();
	}

	private unsafe void RefreshSearch()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (Window == null)
		{
			return;
		}
		Enumerator enumerator = ((Control)Window.CamerasContainer).Children.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				Control current = ((Enumerator)(ref enumerator)).Current;
				Button val = (Button)(object)((current is Button) ? current : null);
				if (val != null)
				{
					((Control)val).Visible = ((Control)(object)val).ChildrenContainText(Window.SearchBar.Text);
				}
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	private void RefreshCamera()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		RMCCameraComputerComponent rMCCameraComputerComponent = default(RMCCameraComputerComponent);
		if (Window == null || !((BoundUserInterface)this).EntMan.TryGetComponent<RMCCameraComputerComponent>(((BoundUserInterface)this).Owner, ref rMCCameraComputerComponent))
		{
			return;
		}
		EntityUid? currentCamera = _currentCamera;
		if (currentCamera.HasValue)
		{
			EntityUid valueOrDefault = currentCamera.GetValueOrDefault();
			_eyeLerping.RemoveEye(valueOrDefault);
		}
		currentCamera = rMCCameraComputerComponent.CurrentCamera;
		if (currentCamera.HasValue)
		{
			EntityUid valueOrDefault2 = currentCamera.GetValueOrDefault();
			_eyeLerping.AddEye(valueOrDefault2);
			_currentCamera = valueOrDefault2;
			EyeComponent val = default(EyeComponent);
			if (((BoundUserInterface)this).EntMan.TryGetComponent<EyeComponent>(valueOrDefault2, ref val))
			{
				Window.Viewport.Eye = (IEye?)(object)val.Eye;
			}
			if (_system.GetComputerCameraName(Entity<RMCCameraComputerComponent>.op_Implicit((((BoundUserInterface)this).Owner, rMCCameraComputerComponent)), valueOrDefault2, out string name))
			{
				Window.CameraName.Text = name;
			}
		}
	}
}
