using Robust.Shared.Input;

namespace Content.Shared._PUBG.Input;

[KeyFunctions]
public sealed class PubgKeyFunctions
{
	public static readonly BoundKeyFunction PubgReload = BoundKeyFunction.op_Implicit("PubgReload");

	public static readonly BoundKeyFunction PubgUnload = BoundKeyFunction.op_Implicit("PubgUnload");

	public static readonly BoundKeyFunction PubgFocusView = BoundKeyFunction.op_Implicit("PubgFocusView");

	public static readonly BoundKeyFunction PubgInventoryMenu = BoundKeyFunction.op_Implicit("PubgInventoryMenu");
}
