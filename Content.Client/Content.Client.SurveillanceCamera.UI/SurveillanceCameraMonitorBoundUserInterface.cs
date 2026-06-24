using System;
using Content.Client.Eye;
using Content.Shared.SurveillanceCamera;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.ViewVariables;

namespace Content.Client.SurveillanceCamera.UI;

public sealed class SurveillanceCameraMonitorBoundUserInterface : BoundUserInterface
{
	private readonly EyeLerpingSystem _eyeLerpingSystem;

	private readonly SurveillanceCameraMonitorSystem _surveillanceCameraMonitorSystem;

	[ViewVariables]
	private SurveillanceCameraMonitorWindow? _window;

	[ViewVariables]
	private EntityUid? _currentCamera;

	public SurveillanceCameraMonitorBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		_eyeLerpingSystem = base.EntMan.System<EyeLerpingSystem>();
		_surveillanceCameraMonitorSystem = base.EntMan.System<SurveillanceCameraMonitorSystem>();
	}

	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<SurveillanceCameraMonitorWindow>((BoundUserInterface)(object)this);
		_window.CameraSelected += OnCameraSelected;
		_window.SubnetOpened += OnSubnetRequest;
		_window.CameraRefresh += OnCameraRefresh;
		_window.SubnetRefresh += OnSubnetRefresh;
		_window.CameraSwitchTimer += OnCameraSwitchTimer;
		_window.CameraDisconnect += OnCameraDisconnect;
	}

	private void OnCameraSelected(string address)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new SurveillanceCameraMonitorSwitchMessage(address));
	}

	private void OnSubnetRequest(string subnet)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new SurveillanceCameraMonitorSubnetRequestMessage(subnet));
	}

	private void OnCameraSwitchTimer()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_surveillanceCameraMonitorSystem.AddTimer(((BoundUserInterface)this).Owner, _window.OnSwitchTimerComplete);
	}

	private void OnCameraRefresh()
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new SurveillanceCameraRefreshCamerasMessage());
	}

	private void OnSubnetRefresh()
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new SurveillanceCameraRefreshSubnetsMessage());
	}

	private void OnCameraDisconnect()
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new SurveillanceCameraDisconnectMessage());
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		if (_window == null || !(state is SurveillanceCameraMonitorUiState surveillanceCameraMonitorUiState))
		{
			return;
		}
		EntityUid? entity = base.EntMan.GetEntity(surveillanceCameraMonitorUiState.ActiveCamera);
		if (!entity.HasValue)
		{
			_window.UpdateState(null, surveillanceCameraMonitorUiState.Subnets, surveillanceCameraMonitorUiState.ActiveAddress, surveillanceCameraMonitorUiState.ActiveSubnet, surveillanceCameraMonitorUiState.Cameras);
			if (_currentCamera.HasValue)
			{
				_surveillanceCameraMonitorSystem.RemoveTimer(((BoundUserInterface)this).Owner);
				_eyeLerpingSystem.RemoveEye(_currentCamera.Value);
				_currentCamera = null;
			}
			return;
		}
		if (!_currentCamera.HasValue)
		{
			_eyeLerpingSystem.AddEye(entity.Value);
			_currentCamera = entity;
		}
		else
		{
			EntityUid? currentCamera = _currentCamera;
			EntityUid? val = entity;
			if (currentCamera.HasValue != val.HasValue || (currentCamera.HasValue && currentCamera.GetValueOrDefault() != val.GetValueOrDefault()))
			{
				_eyeLerpingSystem.RemoveEye(_currentCamera.Value);
				_eyeLerpingSystem.AddEye(entity.Value);
				_currentCamera = entity;
			}
		}
		EyeComponent val2 = default(EyeComponent);
		if (base.EntMan.TryGetComponent<EyeComponent>(entity, ref val2))
		{
			_window.UpdateState((IEye?)(object)val2.Eye, surveillanceCameraMonitorUiState.Subnets, surveillanceCameraMonitorUiState.ActiveAddress, surveillanceCameraMonitorUiState.ActiveSubnet, surveillanceCameraMonitorUiState.Cameras);
		}
	}

	protected override void Dispose(bool disposing)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Dispose(disposing);
		if (_currentCamera.HasValue)
		{
			_eyeLerpingSystem.RemoveEye(_currentCamera.Value);
			_currentCamera = null;
		}
		if (disposing)
		{
			SurveillanceCameraMonitorWindow? window = _window;
			if (window != null)
			{
				((Control)window).Orphan();
			}
		}
	}
}
