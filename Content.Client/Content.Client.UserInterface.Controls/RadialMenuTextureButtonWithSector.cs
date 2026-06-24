using System;
using System.Numerics;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Analyzers;
using Robust.Shared.Maths;

namespace Content.Client.UserInterface.Controls;

[Virtual]
public class RadialMenuTextureButtonWithSector : RadialMenuTextureButton, IRadialMenuItemWithSector
{
	private Vector2[]? _sectorPointsForDrawing;

	private float _angleSectorFrom;

	private float _angleSectorTo;

	private float _outerRadius;

	private float _innerRadius;

	private float _angleOffset;

	private bool _isWholeCircle;

	private Vector2? _parentCenter;

	private Color _backgroundColorSrgb = Color.ToSrgb(new Color((byte)70, (byte)73, (byte)102, (byte)128));

	private Color _hoverBackgroundColorSrgb = Color.ToSrgb(new Color((byte)87, (byte)91, (byte)127, (byte)128));

	private Color _borderColorSrgb = Color.ToSrgb(new Color((byte)173, (byte)216, (byte)230, (byte)70));

	private Color _hoverBorderColorSrgb = Color.ToSrgb(new Color((byte)87, (byte)91, (byte)127, (byte)128));

	public bool DrawBorder { get; set; }

	public bool DrawBackground { get; set; } = true;

	public Color BackgroundColor
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return Color.FromSrgb(_backgroundColorSrgb);
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_backgroundColorSrgb = Color.ToSrgb(value);
		}
	}

	public Color HoverBackgroundColor
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return Color.FromSrgb(_hoverBackgroundColorSrgb);
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_hoverBackgroundColorSrgb = Color.ToSrgb(value);
		}
	}

	public Color BorderColor
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return Color.FromSrgb(_borderColorSrgb);
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_borderColorSrgb = Color.ToSrgb(value);
		}
	}

	public Color HoverBorderColor
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return Color.FromSrgb(_hoverBorderColorSrgb);
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_hoverBorderColorSrgb = Color.ToSrgb(value);
		}
	}

	public Color SeparatorColor { get; set; } = new Color((byte)128, (byte)128, (byte)128, (byte)128);

	float IRadialMenuItemWithSector.AngleSectorFrom
	{
		set
		{
			_angleSectorFrom = value;
			_isWholeCircle = IsWholeCircle(value, _angleSectorTo);
		}
	}

	float IRadialMenuItemWithSector.AngleSectorTo
	{
		set
		{
			_angleSectorTo = value;
			_isWholeCircle = IsWholeCircle(_angleSectorFrom, value);
		}
	}

	float IRadialMenuItemWithSector.OuterRadius
	{
		set
		{
			_outerRadius = value;
		}
	}

	float IRadialMenuItemWithSector.InnerRadius
	{
		set
		{
			_innerRadius = value;
		}
	}

	public float AngleOffset
	{
		set
		{
			_angleOffset = value;
		}
	}

	Vector2 IRadialMenuItemWithSector.ParentCenter
	{
		set
		{
			_parentCenter = value;
		}
	}

	protected override void Draw(DrawingHandleScreen handle)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Invalid comparison between Unknown and I4
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Invalid comparison between Unknown and I4
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		((TextureButton)this).Draw(handle);
		if (_parentCenter.HasValue)
		{
			Vector2 center = (_parentCenter.Value - ((Control)this).Position) * ((Control)this).UIScale;
			float angleSectorFrom = _angleSectorFrom + _angleOffset;
			float angleSectorTo = _angleSectorTo + _angleOffset;
			if (DrawBackground)
			{
				Color color = (((int)((BaseButton)this).DrawMode == 2) ? _hoverBackgroundColorSrgb : _backgroundColorSrgb);
				DrawAnnulusSector(handle, center, _innerRadius * ((Control)this).UIScale, _outerRadius * ((Control)this).UIScale, angleSectorFrom, angleSectorTo, color);
			}
			if (DrawBorder)
			{
				Color color2 = (((int)((BaseButton)this).DrawMode == 2) ? _hoverBorderColorSrgb : _borderColorSrgb);
				DrawAnnulusSector(handle, center, _innerRadius * ((Control)this).UIScale, _outerRadius * ((Control)this).UIScale, angleSectorFrom, angleSectorTo, color2, filled: false);
			}
			if (!_isWholeCircle && DrawBorder)
			{
				DrawSeparatorLines(handle, center, _innerRadius * ((Control)this).UIScale, _outerRadius * ((Control)this).UIScale, angleSectorFrom, angleSectorTo, SeparatorColor);
			}
		}
	}

	protected override bool HasPoint(Vector2 point)
	{
		if (!_parentCenter.HasValue)
		{
			return ((Control)this).HasPoint(point);
		}
		float num = _outerRadius * _outerRadius;
		float num2 = _innerRadius * _innerRadius;
		float num3 = (point + ((Control)this).Position - _parentCenter.Value).LengthSquared();
		if (!(num3 < num) || !(num3 > num2))
		{
			return false;
		}
		Vector2 vector = point + ((Control)this).Position - _parentCenter.Value;
		float num4 = MathF.Atan2(0f - vector.Y, vector.X) - _angleOffset;
		if (num4 < 0f)
		{
			num4 = MathF.PI * 2f + num4;
		}
		if (num4 >= _angleSectorFrom)
		{
			return num4 < _angleSectorTo;
		}
		return false;
	}

	private void DrawAnnulusSector(DrawingHandleScreen drawingHandleScreen, Vector2 center, float radiusInner, float radiusOuter, float angleSectorFrom, float angleSectorTo, Color color, bool filled = true)
	{
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		float num = angleSectorTo - angleSectorFrom;
		int num2 = (int)(num / (MathF.PI / 64f)) + 1;
		float num3 = num / (float)(num2 - 1);
		int num4 = num2 * 2;
		if ((_sectorPointsForDrawing == null || _sectorPointsForDrawing.Length != num4) && _sectorPointsForDrawing == null)
		{
			_sectorPointsForDrawing = new Vector2[num4];
		}
		for (int i = 0; i < num2; i++)
		{
			float x = angleSectorFrom + num3 * (float)i;
			Vector2 vector = new Vector2(MathF.Cos(x), 0f - MathF.Sin(x));
			Vector2 vector2 = center + vector * radiusOuter;
			Vector2 vector3 = center + vector * radiusInner;
			if (filled)
			{
				_sectorPointsForDrawing[i * 2] = vector2;
				_sectorPointsForDrawing[i * 2 + 1] = vector3;
			}
			else
			{
				_sectorPointsForDrawing[i] = vector2;
				_sectorPointsForDrawing[num4 - 1 - i] = vector3;
			}
		}
		DrawPrimitiveTopology val = (DrawPrimitiveTopology)(filled ? 3 : 5);
		((DrawingHandleBase)drawingHandleScreen).DrawPrimitives(val, (ReadOnlySpan<Vector2>)_sectorPointsForDrawing, color);
	}

	private static void DrawSeparatorLines(DrawingHandleScreen drawingHandleScreen, Vector2 center, float radiusInner, float radiusOuter, float angleSectorFrom, float angleSectorTo, Color color)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		Angle val = new Angle((double)(0f - angleSectorFrom));
		Vector2 unitX = Vector2.UnitX;
		Vector2 vector = ((Angle)(ref val)).RotateVec(ref unitX);
		((DrawingHandleBase)drawingHandleScreen).DrawLine(center + vector * radiusOuter, center + vector * radiusInner, color);
		val = new Angle((double)(0f - angleSectorTo));
		unitX = Vector2.UnitX;
		Vector2 vector2 = ((Angle)(ref val)).RotateVec(ref unitX);
		((DrawingHandleBase)drawingHandleScreen).DrawLine(center + vector2 * radiusOuter, center + vector2 * radiusInner, color);
	}

	private static bool IsWholeCircle(float angleSectorFrom, float angleSectorTo)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		Angle val = new Angle((double)angleSectorFrom);
		return ((Angle)(ref val)).EqualsApprox(new Angle((double)angleSectorTo));
	}
}
