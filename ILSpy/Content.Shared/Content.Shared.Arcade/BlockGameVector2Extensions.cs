using Robust.Shared.Maths;

namespace Content.Shared.Arcade;

public static class BlockGameVector2Extensions
{
	public static BlockGameBlock ToBlockGameBlock(this Vector2i vector2, BlockGameBlock.BlockGameBlockColor gameBlockColor)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return new BlockGameBlock(vector2, gameBlockColor);
	}

	public static Vector2i AddToX(this Vector2i vector2, int amount)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2i(vector2.X + amount, vector2.Y);
	}

	public static Vector2i AddToY(this Vector2i vector2, int amount)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2i(vector2.X, vector2.Y + amount);
	}

	public static Vector2i Rotate90DegreesAsOffset(this Vector2i vector)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2i(-vector.Y, vector.X);
	}
}
