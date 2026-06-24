using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Shared.Power;

public abstract class SharedPowerMonitoringConsoleSystem : EntitySystem
{
	public const int ChunkSize = 5;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetFlag(Vector2i relativeTile)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		return 1 << relativeTile.X * 5 + relativeTile.Y;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2i GetTileFromIndex(int index)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		int num = index / 5;
		int y = index % 5;
		return new Vector2i(num, y);
	}
}
