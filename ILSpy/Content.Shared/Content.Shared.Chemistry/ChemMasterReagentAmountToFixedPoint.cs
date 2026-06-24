using Content.Shared.FixedPoint;

namespace Content.Shared.Chemistry;

public static class ChemMasterReagentAmountToFixedPoint
{
	public static FixedPoint2 GetFixedPoint(this ChemMasterReagentAmount amount)
	{
		if (amount == ChemMasterReagentAmount.All)
		{
			return FixedPoint2.MaxValue;
		}
		return FixedPoint2.New((int)amount);
	}
}
