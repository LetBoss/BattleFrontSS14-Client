using System.Numerics;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Robust.Shared.EntitySerialization;

public struct MapLoadOptions
{
	public static readonly MapLoadOptions Default = new MapLoadOptions();

	public MapId? MergeMap = null;

	public Vector2 Offset = default(Vector2);

	public Angle Rotation = default(Angle);

	public DeserializationOptions DeserializationOptions = DeserializationOptions.Default;

	public MapId? ForceMapId = null;

	public FileCategory? ExpectedCategory = null;

	public MapLoadOptions()
	{
	}//IL_0012: Unknown result type (might be due to invalid IL or missing references)

}
