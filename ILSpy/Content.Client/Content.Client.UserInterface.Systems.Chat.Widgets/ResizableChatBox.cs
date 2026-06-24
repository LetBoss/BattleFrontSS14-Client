using System;
using System.Numerics;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Client.UserInterface.Systems.Chat.Widgets;

public sealed class ResizableChatBox : ChatBox
{
	[Flags]
	private enum DragMode : byte
	{
		None = 0,
		Bottom = 2,
		Left = 4
	}

	[Dependency]
	private IClyde _clyde;

	private const int DragMarginSize = 7;

	private const int MinDistanceFromBottom = 255;

	private const int MinLeft = 500;

	private DragMode _currentDrag;

	private Vector2 _dragOffsetTopLeft;

	private Vector2 _dragOffsetBottomRight;

	private byte _clampIn;

	public Action<Vector2>? OnChatResizeFinish;

	public ResizableChatBox()
	{
		IoCManager.InjectDependencies<ResizableChatBox>(this);
	}

	protected override void EnteredTree()
	{
		((Control)this).EnteredTree();
		_clyde.OnWindowResized += ClydeOnOnWindowResized;
	}

	protected override void ExitedTree()
	{
		((Control)this).ExitedTree();
		_clyde.OnWindowResized -= ClydeOnOnWindowResized;
	}

	protected override void KeyBindDown(GUIBoundKeyEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		if (((BoundKeyEventArgs)args).Function == EngineKeyFunctions.UIClick)
		{
			_currentDrag = GetDragModeFor(args.RelativePosition);
			if (_currentDrag != DragMode.None)
			{
				_dragOffsetTopLeft = ((BoundKeyEventArgs)args).PointerLocation.Position / ((Control)this).UIScale - ((Control)this).Position;
				_dragOffsetBottomRight = ((Control)this).Position + ((Control)this).Size - ((BoundKeyEventArgs)args).PointerLocation.Position / ((Control)this).UIScale;
			}
		}
		((Control)this).KeyBindDown(args);
	}

	protected override void KeyBindUp(GUIBoundKeyEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		if (((BoundKeyEventArgs)args).Function != EngineKeyFunctions.UIClick)
		{
			return;
		}
		if (_currentDrag != DragMode.None)
		{
			_dragOffsetTopLeft = (_dragOffsetBottomRight = Vector2.Zero);
			_currentDrag = DragMode.None;
			Control keyboardFocused = ((Control)this).UserInterfaceManager.KeyboardFocused;
			if (keyboardFocused != null)
			{
				keyboardFocused.ReleaseKeyboardFocus();
			}
			OnChatResizeFinish?.Invoke(((Control)this).Size);
		}
		((Control)this).KeyBindUp(args);
	}

	private DragMode GetDragModeFor(Vector2 relativeMousePos)
	{
		DragMode dragMode = DragMode.None;
		if (relativeMousePos.Y > ((Control)this).Size.Y - 7f)
		{
			dragMode = DragMode.Bottom;
		}
		if (relativeMousePos.X < 7f)
		{
			dragMode |= DragMode.Left;
		}
		return dragMode;
	}

	protected override void MouseMove(GUIMouseMoveEventArgs args)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).MouseMove(args);
		if (((Control)this).Parent == null)
		{
			return;
		}
		if (_currentDrag == DragMode.None)
		{
			CursorShape defaultCursorShape = (CursorShape)0;
			switch (GetDragModeFor(((GUIMouseEventArgs)args).RelativePosition))
			{
			case DragMode.Bottom:
				defaultCursorShape = (CursorShape)5;
				break;
			case DragMode.Left:
				defaultCursorShape = (CursorShape)4;
				break;
			case DragMode.Bottom | DragMode.Left:
				defaultCursorShape = (CursorShape)2;
				break;
			}
			((Control)this).DefaultCursorShape = defaultCursorShape;
			return;
		}
		float top = ((Control)this).Rect.Top;
		float value = ((Control)this).Rect.Bottom;
		float value2 = ((Control)this).Rect.Left;
		float right = ((Control)this).Rect.Right;
		float num = default(float);
		float num2 = default(float);
		Vector2Helpers.Deconstruct(((Control)this).MinSize, ref num, ref num2);
		float num3 = num;
		float num4 = num2;
		if ((_currentDrag & DragMode.Bottom) == DragMode.Bottom)
		{
			value = Math.Max(((GUIMouseEventArgs)args).GlobalPosition.Y + _dragOffsetBottomRight.Y, top + num4);
		}
		if ((_currentDrag & DragMode.Left) == DragMode.Left)
		{
			float val = right - num3;
			value2 = Math.Min(((GUIMouseEventArgs)args).GlobalPosition.X - _dragOffsetTopLeft.X, val);
		}
		ClampSize(value2, value);
	}

	protected override void UIScaleChanged()
	{
		((Control)this).UIScaleChanged();
		ClampAfterDelay();
	}

	private void ClydeOnOnWindowResized(WindowResizedEventArgs obj)
	{
		ClampAfterDelay();
	}

	private void ClampAfterDelay()
	{
		_clampIn = 2;
	}

	protected override void FrameUpdate(FrameEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).FrameUpdate(args);
		if (_clampIn > 0)
		{
			_clampIn--;
			if (_clampIn == 0)
			{
				ClampSize();
			}
		}
	}

	private void ClampSize(float? desiredLeft = null, float? desiredBottom = null)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (((Control)this).Parent != null)
		{
			float right = ((Control)this).Rect.Right;
			float value = desiredLeft ?? ((Control)this).Rect.Left;
			float value2 = desiredBottom ?? ((Control)this).Rect.Bottom;
			float num = ((Control)this).Parent.Size.Y - 255f;
			value2 = ((!(num <= ((Control)this).MinHeight)) ? Math.Clamp(value2, ((Control)this).MinHeight, num) : ((Control)this).MinHeight);
			float num2 = ((Control)this).Parent.Size.X - ((Control)this).MinWidth;
			value = ((!(num2 <= 500f)) ? Math.Clamp(value, 500f, num2) : num2);
			LayoutContainer.SetMarginLeft((Control)(object)this, 0f - (right + 10f - value));
			LayoutContainer.SetMarginBottom((Control)(object)this, value2);
		}
	}

	protected override void MouseExited()
	{
		((Control)this).MouseExited();
		if (_currentDrag == DragMode.None)
		{
			((Control)this).DefaultCursorShape = (CursorShape)0;
		}
	}
}
