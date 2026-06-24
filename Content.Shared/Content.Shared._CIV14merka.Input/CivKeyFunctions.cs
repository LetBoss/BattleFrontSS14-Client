using Robust.Shared.Input;

namespace Content.Shared._CIV14merka.Input;

[KeyFunctions]
public sealed class CivKeyFunctions
{
	public static readonly BoundKeyFunction CivSquadRadial = BoundKeyFunction.op_Implicit("CivSquadRadial");

	public static readonly BoundKeyFunction CivGlobalMap = BoundKeyFunction.op_Implicit("CivGlobalMap");

	public static readonly BoundKeyFunction CivBotOrderRadial = BoundKeyFunction.op_Implicit("CivBotOrderRadial");

	public static readonly BoundKeyFunction CivCommanderDrawLine = BoundKeyFunction.op_Implicit("CivCommanderDrawLine");

	public static readonly BoundKeyFunction CivCommanderEraseLine = BoundKeyFunction.op_Implicit("CivCommanderEraseLine");

	public static readonly BoundKeyFunction CivCommanderLabelRotate = BoundKeyFunction.op_Implicit("CivCommanderLabelRotate");
}
