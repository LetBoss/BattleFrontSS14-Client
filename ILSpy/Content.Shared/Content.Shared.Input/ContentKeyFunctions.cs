using Robust.Shared.Input;

namespace Content.Shared.Input;

[KeyFunctions]
public static class ContentKeyFunctions
{
	public static readonly BoundKeyFunction UseItemInHand = BoundKeyFunction.op_Implicit("ActivateItemInHand");

	public static readonly BoundKeyFunction AltUseItemInHand = BoundKeyFunction.op_Implicit("AltActivateItemInHand");

	public static readonly BoundKeyFunction ActivateItemInWorld = BoundKeyFunction.op_Implicit("ActivateItemInWorld");

	public static readonly BoundKeyFunction AltActivateItemInWorld = BoundKeyFunction.op_Implicit("AltActivateItemInWorld");

	public static readonly BoundKeyFunction Drop = BoundKeyFunction.op_Implicit("Drop");

	public static readonly BoundKeyFunction ExamineEntity = BoundKeyFunction.op_Implicit("ExamineEntity");

	public static readonly BoundKeyFunction FocusChat = BoundKeyFunction.op_Implicit("FocusChatInputWindow");

	public static readonly BoundKeyFunction FocusLocalChat = BoundKeyFunction.op_Implicit("FocusLocalChatWindow");

	public static readonly BoundKeyFunction FocusEmote = BoundKeyFunction.op_Implicit("FocusEmote");

	public static readonly BoundKeyFunction FocusWhisperChat = BoundKeyFunction.op_Implicit("FocusWhisperChatWindow");

	public static readonly BoundKeyFunction FocusRadio = BoundKeyFunction.op_Implicit("FocusRadioWindow");

	public static readonly BoundKeyFunction FocusLOOC = BoundKeyFunction.op_Implicit("FocusLOOCWindow");

	public static readonly BoundKeyFunction FocusOOC = BoundKeyFunction.op_Implicit("FocusOOCWindow");

	public static readonly BoundKeyFunction FocusAdminChat = BoundKeyFunction.op_Implicit("FocusAdminChatWindow");

	public static readonly BoundKeyFunction FocusDeadChat = BoundKeyFunction.op_Implicit("FocusDeadChatWindow");

	public static readonly BoundKeyFunction FocusConsoleChat = BoundKeyFunction.op_Implicit("FocusConsoleChatWindow");

	public static readonly BoundKeyFunction CycleChatChannelForward = BoundKeyFunction.op_Implicit("CycleChatChannelForward");

	public static readonly BoundKeyFunction CycleChatChannelBackward = BoundKeyFunction.op_Implicit("CycleChatChannelBackward");

	public static readonly BoundKeyFunction EscapeContext = BoundKeyFunction.op_Implicit("EscapeContext");

	public static readonly BoundKeyFunction OpenCharacterMenu = BoundKeyFunction.op_Implicit("OpenCharacterMenu");

	public static readonly BoundKeyFunction OpenEmotesMenu = BoundKeyFunction.op_Implicit("OpenEmotesMenu");

	public static readonly BoundKeyFunction OpenCraftingMenu = BoundKeyFunction.op_Implicit("OpenCraftingMenu");

	public static readonly BoundKeyFunction OpenGuidebook = BoundKeyFunction.op_Implicit("OpenGuidebook");

	public static readonly BoundKeyFunction OpenInventoryMenu = BoundKeyFunction.op_Implicit("OpenInventoryMenu");

	public static readonly BoundKeyFunction SmartEquipBackpack = BoundKeyFunction.op_Implicit("SmartEquipBackpack");

	public static readonly BoundKeyFunction SmartEquipBelt = BoundKeyFunction.op_Implicit("SmartEquipBelt");

	public static readonly BoundKeyFunction OpenBackpack = BoundKeyFunction.op_Implicit("OpenBackpack");

	public static readonly BoundKeyFunction OpenBelt = BoundKeyFunction.op_Implicit("OpenBelt");

	public static readonly BoundKeyFunction OpenAHelp = BoundKeyFunction.op_Implicit("OpenAHelp");

	public static readonly BoundKeyFunction SwapHands = BoundKeyFunction.op_Implicit("SwapHands");

	public static readonly BoundKeyFunction SwapHandsReverse = BoundKeyFunction.op_Implicit("SwapHandsReverse");

	public static readonly BoundKeyFunction MoveStoredItem = BoundKeyFunction.op_Implicit("MoveStoredItem");

	public static readonly BoundKeyFunction RotateStoredItem = BoundKeyFunction.op_Implicit("RotateStoredItem");

	public static readonly BoundKeyFunction SaveItemLocation = BoundKeyFunction.op_Implicit("SaveItemLocation");

	public static readonly BoundKeyFunction ThrowItemInHand = BoundKeyFunction.op_Implicit("ThrowItemInHand");

	public static readonly BoundKeyFunction TryPullObject = BoundKeyFunction.op_Implicit("TryPullObject");

	public static readonly BoundKeyFunction MovePulledObject = BoundKeyFunction.op_Implicit("MovePulledObject");

	public static readonly BoundKeyFunction ReleasePulledObject = BoundKeyFunction.op_Implicit("ReleasePulledObject");

	public static readonly BoundKeyFunction MouseMiddle = BoundKeyFunction.op_Implicit("MouseMiddle");

	public static readonly BoundKeyFunction RotateObjectClockwise = BoundKeyFunction.op_Implicit("RotateObjectClockwise");

	public static readonly BoundKeyFunction RotateObjectCounterclockwise = BoundKeyFunction.op_Implicit("RotateObjectCounterclockwise");

	public static readonly BoundKeyFunction FlipObject = BoundKeyFunction.op_Implicit("FlipObject");

	public static readonly BoundKeyFunction ToggleRoundEndSummaryWindow = BoundKeyFunction.op_Implicit("ToggleRoundEndSummaryWindow");

	public static readonly BoundKeyFunction OpenEntitySpawnWindow = BoundKeyFunction.op_Implicit("OpenEntitySpawnWindow");

	public static readonly BoundKeyFunction OpenSandboxWindow = BoundKeyFunction.op_Implicit("OpenSandboxWindow");

	public static readonly BoundKeyFunction OpenTileSpawnWindow = BoundKeyFunction.op_Implicit("OpenTileSpawnWindow");

	public static readonly BoundKeyFunction OpenDecalSpawnWindow = BoundKeyFunction.op_Implicit("OpenDecalSpawnWindow");

	public static readonly BoundKeyFunction OpenAdminMenu = BoundKeyFunction.op_Implicit("OpenAdminMenu");

	public static readonly BoundKeyFunction TakeScreenshot = BoundKeyFunction.op_Implicit("TakeScreenshot");

	public static readonly BoundKeyFunction TakeScreenshotNoUI = BoundKeyFunction.op_Implicit("TakeScreenshotNoUI");

	public static readonly BoundKeyFunction ToggleFullscreen = BoundKeyFunction.op_Implicit("ToggleFullscreen");

	public static readonly BoundKeyFunction Point = BoundKeyFunction.op_Implicit("Point");

	public static readonly BoundKeyFunction ZoomOut = BoundKeyFunction.op_Implicit("ZoomOut");

	public static readonly BoundKeyFunction ZoomIn = BoundKeyFunction.op_Implicit("ZoomIn");

	public static readonly BoundKeyFunction ResetZoom = BoundKeyFunction.op_Implicit("ResetZoom");

	public static readonly BoundKeyFunction ArcadeUp = BoundKeyFunction.op_Implicit("ArcadeUp");

	public static readonly BoundKeyFunction ArcadeDown = BoundKeyFunction.op_Implicit("ArcadeDown");

	public static readonly BoundKeyFunction ArcadeLeft = BoundKeyFunction.op_Implicit("ArcadeLeft");

	public static readonly BoundKeyFunction ArcadeRight = BoundKeyFunction.op_Implicit("ArcadeRight");

	public static readonly BoundKeyFunction Arcade1 = BoundKeyFunction.op_Implicit("Arcade1");

	public static readonly BoundKeyFunction Arcade2 = BoundKeyFunction.op_Implicit("Arcade2");

	public static readonly BoundKeyFunction Arcade3 = BoundKeyFunction.op_Implicit("Arcade3");

	public static readonly BoundKeyFunction OpenActionsMenu = BoundKeyFunction.op_Implicit("OpenAbilitiesMenu");

	public static readonly BoundKeyFunction ShuttleStrafeLeft = BoundKeyFunction.op_Implicit("ShuttleStrafeLeft");

	public static readonly BoundKeyFunction ShuttleStrafeUp = BoundKeyFunction.op_Implicit("ShuttleStrafeUp");

	public static readonly BoundKeyFunction ShuttleStrafeRight = BoundKeyFunction.op_Implicit("ShuttleStrafeRight");

	public static readonly BoundKeyFunction ShuttleStrafeDown = BoundKeyFunction.op_Implicit("ShuttleStrafeDown");

	public static readonly BoundKeyFunction ShuttleRotateLeft = BoundKeyFunction.op_Implicit("ShuttleRotateLeft");

	public static readonly BoundKeyFunction ShuttleRotateRight = BoundKeyFunction.op_Implicit("ShuttleRotateRight");

	public static readonly BoundKeyFunction ShuttleBrake = BoundKeyFunction.op_Implicit("ShuttleBrake");

	public static readonly BoundKeyFunction Hotbar0 = BoundKeyFunction.op_Implicit("Hotbar0");

	public static readonly BoundKeyFunction Hotbar1 = BoundKeyFunction.op_Implicit("Hotbar1");

	public static readonly BoundKeyFunction Hotbar2 = BoundKeyFunction.op_Implicit("Hotbar2");

	public static readonly BoundKeyFunction Hotbar3 = BoundKeyFunction.op_Implicit("Hotbar3");

	public static readonly BoundKeyFunction Hotbar4 = BoundKeyFunction.op_Implicit("Hotbar4");

	public static readonly BoundKeyFunction Hotbar5 = BoundKeyFunction.op_Implicit("Hotbar5");

	public static readonly BoundKeyFunction Hotbar6 = BoundKeyFunction.op_Implicit("Hotbar6");

	public static readonly BoundKeyFunction Hotbar7 = BoundKeyFunction.op_Implicit("Hotbar7");

	public static readonly BoundKeyFunction Hotbar8 = BoundKeyFunction.op_Implicit("Hotbar8");

	public static readonly BoundKeyFunction Hotbar9 = BoundKeyFunction.op_Implicit("Hotbar9");

	public static readonly BoundKeyFunction HotbarShift0 = BoundKeyFunction.op_Implicit("HotbarShift0");

	public static readonly BoundKeyFunction HotbarShift1 = BoundKeyFunction.op_Implicit("HotbarShift1");

	public static readonly BoundKeyFunction HotbarShift2 = BoundKeyFunction.op_Implicit("HotbarShift2");

	public static readonly BoundKeyFunction HotbarShift3 = BoundKeyFunction.op_Implicit("HotbarShift3");

	public static readonly BoundKeyFunction HotbarShift4 = BoundKeyFunction.op_Implicit("HotbarShift4");

	public static readonly BoundKeyFunction HotbarShift5 = BoundKeyFunction.op_Implicit("HotbarShift5");

	public static readonly BoundKeyFunction HotbarShift6 = BoundKeyFunction.op_Implicit("HotbarShift6");

	public static readonly BoundKeyFunction HotbarShift7 = BoundKeyFunction.op_Implicit("HotbarShift7");

	public static readonly BoundKeyFunction HotbarShift8 = BoundKeyFunction.op_Implicit("HotbarShift8");

	public static readonly BoundKeyFunction HotbarShift9 = BoundKeyFunction.op_Implicit("HotbarShift9");

	public static readonly BoundKeyFunction Vote0 = BoundKeyFunction.op_Implicit("Vote0");

	public static readonly BoundKeyFunction Vote1 = BoundKeyFunction.op_Implicit("Vote1");

	public static readonly BoundKeyFunction Vote2 = BoundKeyFunction.op_Implicit("Vote2");

	public static readonly BoundKeyFunction Vote3 = BoundKeyFunction.op_Implicit("Vote3");

	public static readonly BoundKeyFunction Vote4 = BoundKeyFunction.op_Implicit("Vote4");

	public static readonly BoundKeyFunction Vote5 = BoundKeyFunction.op_Implicit("Vote5");

	public static readonly BoundKeyFunction Vote6 = BoundKeyFunction.op_Implicit("Vote6");

	public static readonly BoundKeyFunction Vote7 = BoundKeyFunction.op_Implicit("Vote7");

	public static readonly BoundKeyFunction Vote8 = BoundKeyFunction.op_Implicit("Vote8");

	public static readonly BoundKeyFunction Vote9 = BoundKeyFunction.op_Implicit("Vote9");

	public static readonly BoundKeyFunction EditorCopyObject = BoundKeyFunction.op_Implicit("EditorCopyObject");

	public static readonly BoundKeyFunction EditorFlipObject = BoundKeyFunction.op_Implicit("EditorFlipObject");

	public static readonly BoundKeyFunction InspectEntity = BoundKeyFunction.op_Implicit("InspectEntity");

	public static readonly BoundKeyFunction MappingUnselect = BoundKeyFunction.op_Implicit("MappingUnselect");

	public static readonly BoundKeyFunction SaveMap = BoundKeyFunction.op_Implicit("SaveMap");

	public static readonly BoundKeyFunction MappingEnablePick = BoundKeyFunction.op_Implicit("MappingEnablePick");

	public static readonly BoundKeyFunction MappingEnableDelete = BoundKeyFunction.op_Implicit("MappingEnableDelete");

	public static readonly BoundKeyFunction MappingPick = BoundKeyFunction.op_Implicit("MappingPick");

	public static readonly BoundKeyFunction MappingRemoveDecal = BoundKeyFunction.op_Implicit("MappingRemoveDecal");

	public static readonly BoundKeyFunction MappingCancelEraseDecal = BoundKeyFunction.op_Implicit("MappingCancelEraseDecal");

	public static readonly BoundKeyFunction MappingOpenContextMenu = BoundKeyFunction.op_Implicit("MappingOpenContextMenu");

	public static BoundKeyFunction[] GetHotbarBoundKeys()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		return (BoundKeyFunction[])(object)new BoundKeyFunction[20]
		{
			Hotbar1, Hotbar2, Hotbar3, Hotbar4, Hotbar5, Hotbar6, Hotbar7, Hotbar8, Hotbar9, Hotbar0,
			HotbarShift1, HotbarShift2, HotbarShift3, HotbarShift4, HotbarShift5, HotbarShift6, HotbarShift7, HotbarShift8, HotbarShift9, HotbarShift0
		};
	}
}
