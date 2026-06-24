using Content.Shared._CIV14merka.Input;
using Content.Shared._PUBG.Input;
using Content.Shared._RMC14.Input;
using Content.Shared.Input;
using Robust.Shared.Input;

namespace Content.Client.Input;

public static class ContentContexts
{
	public static void SetupContexts(IInputContextContainer contexts)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_044c: Unknown result type (might be due to invalid IL or missing references)
		IInputCmdContext context = contexts.GetContext("common");
		context.AddFunction(ContentKeyFunctions.FocusChat);
		context.AddFunction(ContentKeyFunctions.FocusLocalChat);
		context.AddFunction(ContentKeyFunctions.FocusEmote);
		context.AddFunction(ContentKeyFunctions.FocusWhisperChat);
		context.AddFunction(ContentKeyFunctions.FocusRadio);
		context.AddFunction(ContentKeyFunctions.FocusLOOC);
		context.AddFunction(ContentKeyFunctions.FocusOOC);
		context.AddFunction(ContentKeyFunctions.FocusAdminChat);
		context.AddFunction(ContentKeyFunctions.FocusConsoleChat);
		context.AddFunction(ContentKeyFunctions.FocusDeadChat);
		context.AddFunction(ContentKeyFunctions.CycleChatChannelForward);
		context.AddFunction(ContentKeyFunctions.CycleChatChannelBackward);
		context.AddFunction(ContentKeyFunctions.EscapeContext);
		context.AddFunction(ContentKeyFunctions.ExamineEntity);
		context.AddFunction(ContentKeyFunctions.OpenAHelp);
		context.AddFunction(ContentKeyFunctions.TakeScreenshot);
		context.AddFunction(ContentKeyFunctions.TakeScreenshotNoUI);
		context.AddFunction(ContentKeyFunctions.ToggleFullscreen);
		context.AddFunction(ContentKeyFunctions.MoveStoredItem);
		context.AddFunction(ContentKeyFunctions.RotateStoredItem);
		context.AddFunction(ContentKeyFunctions.SaveItemLocation);
		context.AddFunction(ContentKeyFunctions.Point);
		context.AddFunction(ContentKeyFunctions.ZoomOut);
		context.AddFunction(ContentKeyFunctions.ZoomIn);
		context.AddFunction(ContentKeyFunctions.ResetZoom);
		context.AddFunction(ContentKeyFunctions.InspectEntity);
		context.AddFunction(ContentKeyFunctions.ToggleRoundEndSummaryWindow);
		context.AddFunction(ContentKeyFunctions.EditorCopyObject);
		context.AddFunction(ContentKeyFunctions.EditorFlipObject);
		context.AddFunction(EngineKeyFunctions.EditorRotateObject);
		IInputCmdContext context2 = contexts.GetContext("human");
		context2.AddFunction(EngineKeyFunctions.MoveUp);
		context2.AddFunction(EngineKeyFunctions.MoveDown);
		context2.AddFunction(EngineKeyFunctions.MoveLeft);
		context2.AddFunction(EngineKeyFunctions.MoveRight);
		context2.AddFunction(EngineKeyFunctions.Walk);
		context2.AddFunction(ContentKeyFunctions.SwapHands);
		context2.AddFunction(ContentKeyFunctions.SwapHandsReverse);
		context2.AddFunction(ContentKeyFunctions.Drop);
		context2.AddFunction(ContentKeyFunctions.UseItemInHand);
		context2.AddFunction(ContentKeyFunctions.AltUseItemInHand);
		context2.AddFunction(ContentKeyFunctions.OpenCharacterMenu);
		context2.AddFunction(ContentKeyFunctions.OpenEmotesMenu);
		context2.AddFunction(ContentKeyFunctions.ActivateItemInWorld);
		context2.AddFunction(ContentKeyFunctions.ThrowItemInHand);
		context2.AddFunction(ContentKeyFunctions.AltActivateItemInWorld);
		context2.AddFunction(ContentKeyFunctions.TryPullObject);
		context2.AddFunction(ContentKeyFunctions.MovePulledObject);
		context2.AddFunction(ContentKeyFunctions.ReleasePulledObject);
		context2.AddFunction(ContentKeyFunctions.OpenCraftingMenu);
		context2.AddFunction(ContentKeyFunctions.OpenInventoryMenu);
		context2.AddFunction(ContentKeyFunctions.SmartEquipBackpack);
		context2.AddFunction(ContentKeyFunctions.SmartEquipBelt);
		context2.AddFunction(ContentKeyFunctions.OpenBackpack);
		context2.AddFunction(ContentKeyFunctions.OpenBelt);
		context2.AddFunction(ContentKeyFunctions.MouseMiddle);
		context2.AddFunction(ContentKeyFunctions.RotateObjectClockwise);
		context2.AddFunction(ContentKeyFunctions.RotateObjectCounterclockwise);
		context2.AddFunction(ContentKeyFunctions.FlipObject);
		context2.AddFunction(ContentKeyFunctions.ArcadeUp);
		context2.AddFunction(ContentKeyFunctions.ArcadeDown);
		context2.AddFunction(ContentKeyFunctions.ArcadeLeft);
		context2.AddFunction(ContentKeyFunctions.ArcadeRight);
		context2.AddFunction(ContentKeyFunctions.Arcade1);
		context2.AddFunction(ContentKeyFunctions.Arcade2);
		context2.AddFunction(ContentKeyFunctions.Arcade3);
		context.AddFunction(ContentKeyFunctions.OpenActionsMenu);
		BoundKeyFunction[] hotbarBoundKeys = ContentKeyFunctions.GetHotbarBoundKeys();
		foreach (BoundKeyFunction val in hotbarBoundKeys)
		{
			context.AddFunction(val);
		}
		IInputCmdContext obj = contexts.New("aghost", "common");
		obj.AddFunction(EngineKeyFunctions.MoveUp);
		obj.AddFunction(EngineKeyFunctions.MoveDown);
		obj.AddFunction(EngineKeyFunctions.MoveLeft);
		obj.AddFunction(EngineKeyFunctions.MoveRight);
		obj.AddFunction(EngineKeyFunctions.Walk);
		obj.AddFunction(ContentKeyFunctions.SwapHands);
		obj.AddFunction(ContentKeyFunctions.SwapHandsReverse);
		obj.AddFunction(ContentKeyFunctions.Drop);
		obj.AddFunction(ContentKeyFunctions.UseItemInHand);
		obj.AddFunction(ContentKeyFunctions.AltUseItemInHand);
		obj.AddFunction(ContentKeyFunctions.ActivateItemInWorld);
		obj.AddFunction(ContentKeyFunctions.ThrowItemInHand);
		obj.AddFunction(ContentKeyFunctions.AltActivateItemInWorld);
		obj.AddFunction(ContentKeyFunctions.TryPullObject);
		obj.AddFunction(ContentKeyFunctions.MovePulledObject);
		obj.AddFunction(ContentKeyFunctions.ReleasePulledObject);
		IInputCmdContext obj2 = contexts.New("ghost", "human");
		obj2.AddFunction(EngineKeyFunctions.MoveUp);
		obj2.AddFunction(EngineKeyFunctions.MoveDown);
		obj2.AddFunction(EngineKeyFunctions.MoveLeft);
		obj2.AddFunction(EngineKeyFunctions.MoveRight);
		obj2.AddFunction(EngineKeyFunctions.Walk);
		context.AddFunction(ContentKeyFunctions.OpenEntitySpawnWindow);
		context.AddFunction(ContentKeyFunctions.OpenSandboxWindow);
		context.AddFunction(ContentKeyFunctions.OpenTileSpawnWindow);
		context.AddFunction(ContentKeyFunctions.OpenDecalSpawnWindow);
		context.AddFunction(ContentKeyFunctions.OpenAdminMenu);
		context.AddFunction(ContentKeyFunctions.OpenGuidebook);
		CMFunctions(contexts);
	}

	private static void CMFunctions(IInputContextContainer contexts)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		IInputCmdContext context = contexts.GetContext("human");
		context.AddFunction(CMKeyFunctions.RMCActivateAttachableBarrel);
		context.AddFunction(CMKeyFunctions.RMCActivateAttachableRail);
		context.AddFunction(CMKeyFunctions.RMCActivateAttachableStock);
		context.AddFunction(CMKeyFunctions.RMCActivateAttachableUnderbarrel);
		context.AddFunction(CMKeyFunctions.RMCFieldStripHeldItem);
		context.AddFunction(CMKeyFunctions.RMCCycleFireMode);
		context.AddFunction(CMKeyFunctions.CMUniqueAction);
		context.AddFunction(CMKeyFunctions.CMHolsterPrimary);
		context.AddFunction(CMKeyFunctions.CMHolsterSecondary);
		context.AddFunction(CMKeyFunctions.CMHolsterTertiary);
		context.AddFunction(CMKeyFunctions.CMHolsterQuaternary);
		context.AddFunction(CMKeyFunctions.RMCPickUpDroppedItems);
		context.AddFunction(CMKeyFunctions.RMCInteractWithOtherHand);
		context.AddFunction(CMKeyFunctions.RMCRest);
		context.AddFunction(PubgKeyFunctions.PubgReload);
		context.AddFunction(PubgKeyFunctions.PubgUnload);
		context.AddFunction(PubgKeyFunctions.PubgFocusView);
		context.AddFunction(PubgKeyFunctions.PubgInventoryMenu);
		context.AddFunction(CivKeyFunctions.CivSquadRadial);
		context.AddFunction(CivKeyFunctions.CivGlobalMap);
		context.AddFunction(CivKeyFunctions.CivBotOrderRadial);
		context.AddFunction(CivKeyFunctions.CivCommanderDrawLine);
		context.AddFunction(CivKeyFunctions.CivCommanderEraseLine);
		context.AddFunction(CivKeyFunctions.CivCommanderLabelRotate);
		IInputCmdContext obj = contexts.New("xenonid", "human");
		obj.AddFunction(CMKeyFunctions.CMXenoWideSwing);
		obj.AddFunction(CMKeyFunctions.RMCXenoRest);
	}
}
