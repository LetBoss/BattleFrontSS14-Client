using System;
using Content.Shared.CCVar;
using Robust.Client.Input;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Map;

namespace Content.Client.Interaction;

public sealed class DragDropHelper<T>
{
	private enum DragState : byte
	{
		NotDragging,
		MouseDown,
		Dragging
	}

	[Dependency]
	private IInputManager _inputManager;

	[Dependency]
	private IConfigurationManager _cfg;

	private readonly OnBeginDrag _onBeginDrag;

	private readonly OnEndDrag _onEndDrag;

	private readonly OnContinueDrag _onContinueDrag;

	private float _deadzone;

	private ScreenCoordinates _mouseDownScreenPos;

	private DragState _state;

	public ScreenCoordinates MouseScreenPosition => _inputManager.MouseScreenPosition;

	public bool IsDragging => _state == DragState.Dragging;

	public T? Dragged { get; private set; }

	public DragDropHelper(OnBeginDrag onBeginDrag, OnContinueDrag onContinueDrag, OnEndDrag onEndDrag)
	{
		IoCManager.InjectDependencies<DragDropHelper<T>>(this);
		_onBeginDrag = onBeginDrag;
		_onEndDrag = onEndDrag;
		_onContinueDrag = onContinueDrag;
		_cfg.OnValueChanged<float>(CCVars.DragDropDeadZone, (Action<float>)SetDeadZone, true);
	}

	public void MouseDown(T target)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (_state != DragState.NotDragging)
		{
			EndDrag();
		}
		Dragged = target;
		_state = DragState.MouseDown;
		_mouseDownScreenPos = _inputManager.MouseScreenPosition;
	}

	public void EndDrag()
	{
		Dragged = default(T);
		_state = DragState.NotDragging;
		_onEndDrag();
	}

	private void StartDragging()
	{
		if (_onBeginDrag())
		{
			_state = DragState.Dragging;
		}
		else
		{
			EndDrag();
		}
	}

	public void Update(float frameTime)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		switch (_state)
		{
		case DragState.MouseDown:
		{
			ScreenCoordinates mouseScreenPosition = _inputManager.MouseScreenPosition;
			if ((_mouseDownScreenPos.Position - mouseScreenPosition.Position).Length() > _deadzone)
			{
				StartDragging();
			}
			break;
		}
		case DragState.Dragging:
			if (!_onContinueDrag(frameTime))
			{
				EndDrag();
			}
			break;
		}
	}

	private void SetDeadZone(float value)
	{
		_deadzone = value;
	}
}
