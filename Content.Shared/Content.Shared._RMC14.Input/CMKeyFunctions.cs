using Robust.Shared.Input;

namespace Content.Shared._RMC14.Input;

[KeyFunctions]
public sealed class CMKeyFunctions
{
	public static readonly BoundKeyFunction RMCActivateAttachableBarrel = BoundKeyFunction.op_Implicit("RMCActivateAttachableBarrel");

	public static readonly BoundKeyFunction RMCActivateAttachableRail = BoundKeyFunction.op_Implicit("RMCActivateAttachableRail");

	public static readonly BoundKeyFunction RMCActivateAttachableStock = BoundKeyFunction.op_Implicit("RMCActivateAttachableStock");

	public static readonly BoundKeyFunction RMCActivateAttachableUnderbarrel = BoundKeyFunction.op_Implicit("RMCActivateAttachableUnderbarrel");

	public static readonly BoundKeyFunction RMCFieldStripHeldItem = BoundKeyFunction.op_Implicit("RMCFieldStripHeldItem");

	public static readonly BoundKeyFunction RMCCycleFireMode = BoundKeyFunction.op_Implicit("RMCCycleFireMode");

	public static readonly BoundKeyFunction CMUniqueAction = BoundKeyFunction.op_Implicit("CMUniqueAction");

	public static readonly BoundKeyFunction CMHolsterPrimary = BoundKeyFunction.op_Implicit("CMHolsterPrimary");

	public static readonly BoundKeyFunction CMHolsterSecondary = BoundKeyFunction.op_Implicit("CMHolsterSecondary");

	public static readonly BoundKeyFunction CMHolsterTertiary = BoundKeyFunction.op_Implicit("CMHolsterTertiary");

	public static readonly BoundKeyFunction CMHolsterQuaternary = BoundKeyFunction.op_Implicit("CMHolsterQuaternary");

	public static readonly BoundKeyFunction RMCPickUpDroppedItems = BoundKeyFunction.op_Implicit("RMCPickUpDroppedItems");

	public static readonly BoundKeyFunction RMCFocusMentorChat = BoundKeyFunction.op_Implicit("RMCFocusMentorChat");

	public static readonly BoundKeyFunction RMCInteractWithOtherHand = BoundKeyFunction.op_Implicit("RMCInteractWithOtherHand");

	public static readonly BoundKeyFunction RMCRest = BoundKeyFunction.op_Implicit("RMCRest");

	public static readonly BoundKeyFunction CMXenoWideSwing = BoundKeyFunction.op_Implicit("CMXenoWideSwing");

	public static readonly BoundKeyFunction RMCXenoRest = BoundKeyFunction.op_Implicit("RMCXenoRest");
}
