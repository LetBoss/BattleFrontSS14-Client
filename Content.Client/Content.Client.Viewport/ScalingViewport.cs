using System;
using System.Collections.Generic;
using System.Numerics;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.ViewVariables;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Content.Client.Viewport;

public sealed class ScalingViewport : Control, IViewportControl
{
	[Dependency]
	private IClyde _clyde;

	[Dependency]
	private IEntityManager _entityManager;

	[Dependency]
	private IInputManager _inputManager;

	private IClydeViewport? _viewport;

	private IEye? _eye;

	private Vector2i _viewportSize;

	private int _curRenderScale;

	private ScalingViewportStretchMode _stretchMode;

	private ScalingViewportRenderScaleMode _renderScaleMode;

	private ScalingViewportIgnoreDimension _ignoreDimension;

	private int _fixedRenderScale = 1;

	private readonly List<CopyPixelsDelegate<Rgba32>> _queuedScreenshots = new List<CopyPixelsDelegate<Rgba32>>();

	public int CurrentRenderScale => _curRenderScale;

	public IEye? Eye
	{
		get
		{
			return _eye;
		}
		set
		{
			_eye = value;
			if (_viewport != null)
			{
				_viewport.Eye = value;
			}
		}
	}

	public Vector2i ViewportSize
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _viewportSize;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_viewportSize = value;
			InvalidateViewport();
		}
	}

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public Vector2i? FixedStretchSize { get; set; }

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public ScalingViewportStretchMode StretchMode
	{
		get
		{
			return _stretchMode;
		}
		set
		{
			_stretchMode = value;
			InvalidateViewport();
		}
	}

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public ScalingViewportRenderScaleMode RenderScaleMode
	{
		get
		{
			return _renderScaleMode;
		}
		set
		{
			_renderScaleMode = value;
			InvalidateViewport();
		}
	}

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public int FixedRenderScale
	{
		get
		{
			return _fixedRenderScale;
		}
		set
		{
			_fixedRenderScale = value;
			InvalidateViewport();
		}
	}

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public ScalingViewportIgnoreDimension IgnoreDimension
	{
		get
		{
			return _ignoreDimension;
		}
		set
		{
			_ignoreDimension = value;
			InvalidateViewport();
		}
	}

	public ScalingViewport()
	{
		IoCManager.InjectDependencies<ScalingViewport>(this);
		((Control)this).RectClipContent = true;
	}

	protected override void KeyBindDown(GUIBoundKeyEventArgs args)
	{
		((Control)this).KeyBindDown(args);
		if (!((BoundKeyEventArgs)args).Handled)
		{
			_inputManager.ViewportKeyEvent((Control)(object)this, (BoundKeyEventArgs)(object)args);
		}
	}

	protected override void KeyBindUp(GUIBoundKeyEventArgs args)
	{
		((Control)this).KeyBindUp(args);
		if (!((BoundKeyEventArgs)args).Handled)
		{
			_inputManager.ViewportKeyEvent((Control)(object)this, (BoundKeyEventArgs)(object)args);
		}
	}

	protected override void Draw(IRenderHandle handle)
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		EnsureViewportCreated();
		_viewport.Render();
		if (_queuedScreenshots.Count != 0)
		{
			CopyPixelsDelegate<Rgba32>[] callbacks = _queuedScreenshots.ToArray();
			((IRenderTarget)_viewport.RenderTarget).CopyPixelsToMemory<Rgba32>((CopyPixelsDelegate<Rgba32>)delegate(Image<Rgba32> image)
			{
				CopyPixelsDelegate<Rgba32>[] array = callbacks;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].Invoke(image);
				}
			}, (UIBox2i?)null);
			_queuedScreenshots.Clear();
		}
		UIBox2i drawBox = GetDrawBox();
		UIBox2i val = ((UIBox2i)(ref drawBox)).Translated(((Control)this).GlobalPixelPosition);
		_viewport.RenderScreenOverlaysBelow(handle, (IViewportControl)(object)this, ref val);
		handle.DrawingHandleScreen.DrawTextureRect(_viewport.RenderTarget.Texture, UIBox2i.op_Implicit(drawBox), (Color?)null);
		_viewport.RenderScreenOverlaysAbove(handle, (IViewportControl)(object)this, ref val);
	}

	public void Screenshot(CopyPixelsDelegate<Rgba32> callback)
	{
		_queuedScreenshots.Add(callback);
	}

	private UIBox2i GetDrawBox()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		Vector2i size = _viewport.Size;
		Vector2 vector = Vector2i.op_Implicit(((Control)this).PixelSize);
		if (!FixedStretchSize.HasValue)
		{
			float num = default(float);
			float num2 = default(float);
			Vector2Helpers.Deconstruct(vector / Vector2i.op_Implicit(size), ref num, ref num2);
			float num3 = num;
			float num4 = num2;
			float num5 = 1f;
			switch (_ignoreDimension)
			{
			case ScalingViewportIgnoreDimension.None:
				num5 = Math.Min(num3, num4);
				break;
			case ScalingViewportIgnoreDimension.Vertical:
				num5 = num3;
				break;
			case ScalingViewportIgnoreDimension.Horizontal:
				num5 = num4;
				break;
			}
			Vector2 vector2 = size * num5;
			return (UIBox2i)UIBox2.FromDimensions((vector - vector2) / 2f, vector2);
		}
		return (UIBox2i)UIBox2.FromDimensions((vector - Vector2i.op_Implicit(FixedStretchSize.Value)) / 2f, Vector2i.op_Implicit(FixedStretchSize.Value));
	}

	private void RegenerateViewport()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		Vector2i viewportSize = ViewportSize;
		float num = default(float);
		float num2 = default(float);
		Vector2Helpers.Deconstruct(Vector2i.op_Implicit(((Control)this).PixelSize) / Vector2i.op_Implicit(viewportSize), ref num, ref num2);
		float val = num;
		float val2 = num2;
		float num3 = Math.Min(val, val2);
		int val3 = 1;
		switch (_renderScaleMode)
		{
		case ScalingViewportRenderScaleMode.CeilInt:
			val3 = (int)Math.Ceiling(num3);
			break;
		case ScalingViewportRenderScaleMode.FloorInt:
			val3 = (int)Math.Floor(num3);
			break;
		case ScalingViewportRenderScaleMode.Fixed:
			val3 = _fixedRenderScale;
			break;
		}
		val3 = (_curRenderScale = Math.Max(1, val3));
		IClyde clyde = _clyde;
		Vector2i val4 = ViewportSize * val3;
		TextureSampleParameters value = default(TextureSampleParameters);
		((TextureSampleParameters)(ref value)).Filter = StretchMode == ScalingViewportStretchMode.Bilinear;
		_viewport = clyde.CreateViewport(val4, (TextureSampleParameters?)value, (string)null);
		_viewport.RenderScale = new Vector2(val3, val3);
		_viewport.Eye = _eye;
	}

	protected override void Resized()
	{
		((Control)this).Resized();
		InvalidateViewport();
	}

	private void InvalidateViewport()
	{
		((IDisposable)_viewport)?.Dispose();
		_viewport = null;
	}

	public MapCoordinates ScreenToMap(Vector2 coords)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (_eye == null)
		{
			return default(MapCoordinates);
		}
		EnsureViewportCreated();
		Matrix3x2.Invert(GetLocalToScreenMatrix(), out var result);
		coords = Vector2.Transform(coords, result);
		return _viewport.LocalToWorld(coords);
	}

	public MapCoordinates PixelToMap(Vector2 coords)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (_eye == null)
		{
			return default(MapCoordinates);
		}
		EnsureViewportCreated();
		Matrix3x2.Invert(GetLocalToScreenMatrix(), out var result);
		coords = Vector2.Transform(coords, result);
		PixelToMapEvent val = default(PixelToMapEvent);
		((PixelToMapEvent)(ref val))._002Ector(coords, (IViewportControl)(object)this, _viewport);
		((IBroadcastEventBus)_entityManager.EventBus).RaiseEvent<PixelToMapEvent>((EventSource)1, ref val);
		return _viewport.LocalToWorld(val.VisiblePosition);
	}

	public Vector2 WorldToScreen(Vector2 map)
	{
		if (_eye == null)
		{
			return default(Vector2);
		}
		EnsureViewportCreated();
		Vector2 position = _viewport.WorldToLocal(map);
		Matrix3x2 localToScreenMatrix = GetLocalToScreenMatrix();
		return Vector2.Transform(position, localToScreenMatrix);
	}

	public Matrix3x2 GetWorldToScreenMatrix()
	{
		EnsureViewportCreated();
		return _viewport.GetWorldToLocalMatrix() * GetLocalToScreenMatrix();
	}

	public Matrix3x2 GetLocalToScreenMatrix()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		EnsureViewportCreated();
		UIBox2i drawBox = GetDrawBox();
		Vector2 vector = Vector2i.op_Implicit(((UIBox2i)(ref drawBox)).Size) / Vector2i.op_Implicit(_viewport.Size);
		if (vector.X == 0f || vector.Y == 0f)
		{
			return Matrix3x2.Identity;
		}
		Vector2 vector2 = Vector2i.op_Implicit(((Control)this).GlobalPixelPosition + drawBox.TopLeft);
		Angle val = Angle.op_Implicit(0f);
		return Matrix3Helpers.CreateTransform(ref vector2, ref val, ref vector);
	}

	private void EnsureViewportCreated()
	{
		if (_viewport == null)
		{
			RegenerateViewport();
		}
	}
}
