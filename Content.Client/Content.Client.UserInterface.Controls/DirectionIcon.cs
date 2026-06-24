using System.Numerics;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;

namespace Content.Client.UserInterface.Controls;

public sealed class DirectionIcon : TextureRect
{
	public static string StyleClassDirectionIconArrow = "direction-icon-arrow";

	public static string StyleClassDirectionIconHere = "direction-icon-here";

	public static string StyleClassDirectionIconUnknown = "direction-icon-unknown";

	private Angle? _rotation;

	private bool _snap;

	private float _minDistance;

	public Angle? Rotation
	{
		get
		{
			return _rotation;
		}
		set
		{
			_rotation = value;
			((Control)this).SetOnlyStyleClass((!value.HasValue) ? StyleClassDirectionIconUnknown : StyleClassDirectionIconArrow);
		}
	}

	public DirectionIcon()
	{
		((TextureRect)this).Stretch = (StretchMode)7;
		((Control)this).SetOnlyStyleClass(StyleClassDirectionIconUnknown);
	}

	public DirectionIcon(bool snap = true, float minDistance = 0.1f)
		: this()
	{
		_snap = snap;
		_minDistance = minDistance;
	}

	public void UpdateDirection(Direction direction)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		Rotation = DirectionExtensions.ToAngle(direction);
	}

	public void UpdateDirection(Vector2 direction, Angle relativeAngle)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (Vector2Helpers.EqualsApprox(direction, Vector2.Zero, (double)_minDistance))
		{
			((Control)this).SetOnlyStyleClass(StyleClassDirectionIconHere);
			return;
		}
		Angle val = DirectionExtensions.ToWorldAngle(direction) - relativeAngle;
		Rotation = (_snap ? DirectionExtensions.ToAngle(((Angle)(ref val)).GetDir()) : val);
	}

	protected override void Draw(DrawingHandleScreen handle)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		if (_rotation.HasValue)
		{
			Angle val = -_rotation.Value;
			Vector2 vector = ((Control)this).Size * ((Control)this).UIScale / 2f;
			Vector2 vector2 = ((Angle)(ref val)).RotateVec(ref vector) - ((Control)this).Size * ((Control)this).UIScale / 2f;
			vector = Vector2i.op_Implicit(((Control)this).GlobalPixelPosition) - vector2;
			val = -_rotation.Value;
			Matrix3x2 matrix3x = Matrix3Helpers.CreateTransform(ref vector, ref val);
			((DrawingHandleBase)handle).SetTransform(ref matrix3x);
		}
		((TextureRect)this).Draw(handle);
	}
}
