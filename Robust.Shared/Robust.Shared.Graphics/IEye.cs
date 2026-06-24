using System.Numerics;
using Robust.Shared.Analyzers;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Robust.Shared.Graphics;

[NotContentImplementable]
public interface IEye
{
	bool DrawFov { get; set; }

	bool DrawLight { get; set; }

	MapCoordinates Position { get; }

	Vector2 Offset { get; set; }

	Angle Rotation { get; set; }

	Vector2 Zoom { get; set; }

	Vector2 Scale { get; set; }

	void GetViewMatrix(out Matrix3x2 viewMatrix, Vector2 renderScale);

	void GetViewMatrixInv(out Matrix3x2 viewMatrixInv, Vector2 renderScale);

	void GetViewMatrixNoOffset(out Matrix3x2 viewMatrix, Vector2 renderScale);
}
