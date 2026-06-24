using System.Numerics;
using Robust.Client.Placement;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Client.Placement.Modes;

public sealed class WallmountLight : PlacementMode
{
	public WallmountLight(PlacementManager pMan)
		: base(pMan)
	{
	}

	public override void AlignPlacementMode(ScreenCoordinates mouseScreen)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Expected I4, but got Unknown
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		((PlacementMode)this).MouseCoords = ((PlacementMode)this).ScreenToCursorGrid(mouseScreen);
		((PlacementMode)this).CurrentTile = ((PlacementMode)this).GetTileRef(((PlacementMode)this).MouseCoords);
		if (!base.pManager.CurrentPermission.IsTile)
		{
			EntityCoordinates val = default(EntityCoordinates);
			((EntityCoordinates)(ref val))._002Ector(((PlacementMode)this).MouseCoords.EntityId, Vector2i.op_Implicit(((PlacementMode)this).CurrentTile.GridIndices));
			Direction direction = base.pManager.Direction;
			Vector2 vector;
			switch ((int)direction)
			{
			default:
				return;
			case 4:
				vector = new Vector2(0.5f, 1f);
				break;
			case 0:
				vector = new Vector2(0.5f, 0f);
				break;
			case 2:
				vector = new Vector2(1f, 0.5f);
				break;
			case 6:
				vector = new Vector2(0f, 0.5f);
				break;
			case 1:
			case 3:
			case 5:
				return;
			}
			val = (((PlacementMode)this).MouseCoords = ((EntityCoordinates)(ref val)).Offset(vector));
		}
	}

	public override bool IsValidPosition(EntityCoordinates position)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (base.pManager.CurrentPermission.IsTile)
		{
			return false;
		}
		if (!((PlacementMode)this).RangeCheck(position))
		{
			return false;
		}
		return true;
	}
}
