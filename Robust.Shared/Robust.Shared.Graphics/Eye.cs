using System.Numerics;
using Robust.Shared.Analyzers;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.ViewVariables;

namespace Robust.Shared.Graphics;

[Virtual]
public class Eye : IEye
{
	private Vector2 _scale = Vector2.One / 2f;

	private Angle _rotation = Angle.Zero;

	private MapCoordinates _coords;

	[ViewVariables(VVAccess.ReadWrite)]
	public bool DrawFov { get; set; } = true;

	[ViewVariables]
	public bool DrawLight { get; set; } = true;

	[ViewVariables(VVAccess.ReadWrite)]
	public virtual MapCoordinates Position
	{
		get
		{
			return _coords;
		}
		set
		{
			_coords = value;
		}
	}

	[ViewVariables(VVAccess.ReadWrite)]
	public Vector2 Offset { get; set; }

	[ViewVariables(VVAccess.ReadWrite)]
	public Angle Rotation
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _rotation;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_rotation = value;
		}
	}

	[ViewVariables(VVAccess.ReadWrite)]
	public Vector2 Zoom
	{
		get
		{
			return new Vector2(1f / _scale.X, 1f / _scale.Y);
		}
		set
		{
			_scale = new Vector2(1f / value.X, 1f / value.Y);
		}
	}

	[ViewVariables(VVAccess.ReadWrite)]
	public Vector2 Scale
	{
		get
		{
			return _scale;
		}
		set
		{
			_scale = value;
		}
	}

	public void GetViewMatrix(out Matrix3x2 viewMatrix, Vector2 renderScale)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		viewMatrix = Matrix3Helpers.CreateInverseTransform(_coords.Position.X + Offset.X, _coords.Position.Y + Offset.Y, (double)(float)(0.0 - Rotation.Theta), 1f / (_scale.X * renderScale.X), 1f / (_scale.Y * renderScale.Y));
	}

	public void GetViewMatrixNoOffset(out Matrix3x2 viewMatrix, Vector2 renderScale)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		viewMatrix = Matrix3Helpers.CreateInverseTransform(_coords.Position.X, _coords.Position.Y, (double)(float)(0.0 - Rotation.Theta), 1f / (_scale.X * renderScale.X), 1f / (_scale.Y * renderScale.Y));
	}

	public void GetViewMatrixInv(out Matrix3x2 viewMatrixInv, Vector2 renderScale)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		viewMatrixInv = Matrix3Helpers.CreateTransform(_coords.Position.X + Offset.X, _coords.Position.Y + Offset.Y, (double)(float)(0.0 - Rotation.Theta), 1f / (_scale.X * renderScale.X), 1f / (_scale.Y * renderScale.Y));
	}
}
