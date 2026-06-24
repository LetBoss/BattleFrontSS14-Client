// Decompiled with JetBrains decompiler
// Type: Content.Client.Input.ContentContexts
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._CIV14merka.Input;
using Content.Shared._PUBG.Input;
using Content.Shared._RMC14.Input;
using Content.Shared.Input;
using Robust.Shared.Input;

#nullable enable
namespace Content.Client.Input;

public static class ContentContexts
{
  public static void SetupContexts(IInputContextContainer contexts)
  {
    IInputCmdContext context1 = contexts.GetContext("common");
    context1.AddFunction(ContentKeyFunctions.FocusChat);
    context1.AddFunction(ContentKeyFunctions.FocusLocalChat);
    context1.AddFunction(ContentKeyFunctions.FocusEmote);
    context1.AddFunction(ContentKeyFunctions.FocusWhisperChat);
    context1.AddFunction(ContentKeyFunctions.FocusRadio);
    context1.AddFunction(ContentKeyFunctions.FocusLOOC);
    context1.AddFunction(ContentKeyFunctions.FocusOOC);
    context1.AddFunction(ContentKeyFunctions.FocusAdminChat);
    context1.AddFunction(ContentKeyFunctions.FocusConsoleChat);
    context1.AddFunction(ContentKeyFunctions.FocusDeadChat);
    context1.AddFunction(ContentKeyFunctions.CycleChatChannelForward);
    context1.AddFunction(ContentKeyFunctions.CycleChatChannelBackward);
    context1.AddFunction(ContentKeyFunctions.EscapeContext);
    context1.AddFunction(ContentKeyFunctions.ExamineEntity);
    context1.AddFunction(ContentKeyFunctions.OpenAHelp);
    context1.AddFunction(ContentKeyFunctions.TakeScreenshot);
    context1.AddFunction(ContentKeyFunctions.TakeScreenshotNoUI);
    context1.AddFunction(ContentKeyFunctions.ToggleFullscreen);
    context1.AddFunction(ContentKeyFunctions.MoveStoredItem);
    context1.AddFunction(ContentKeyFunctions.RotateStoredItem);
    context1.AddFunction(ContentKeyFunctions.SaveItemLocation);
    context1.AddFunction(ContentKeyFunctions.Point);
    context1.AddFunction(ContentKeyFunctions.ZoomOut);
    context1.AddFunction(ContentKeyFunctions.ZoomIn);
    context1.AddFunction(ContentKeyFunctions.ResetZoom);
    context1.AddFunction(ContentKeyFunctions.InspectEntity);
    context1.AddFunction(ContentKeyFunctions.ToggleRoundEndSummaryWindow);
    context1.AddFunction(ContentKeyFunctions.EditorCopyObject);
    context1.AddFunction(ContentKeyFunctions.EditorFlipObject);
    context1.AddFunction(EngineKeyFunctions.EditorRotateObject);
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
    context1.AddFunction(ContentKeyFunctions.OpenActionsMenu);
    foreach (BoundKeyFunction hotbarBoundKey in ContentKeyFunctions.GetHotbarBoundKeys())
      context1.AddFunction(hotbarBoundKey);
    IInputCmdContext iinputCmdContext1 = contexts.New("aghost", "common");
    iinputCmdContext1.AddFunction(EngineKeyFunctions.MoveUp);
    iinputCmdContext1.AddFunction(EngineKeyFunctions.MoveDown);
    iinputCmdContext1.AddFunction(EngineKeyFunctions.MoveLeft);
    iinputCmdContext1.AddFunction(EngineKeyFunctions.MoveRight);
    iinputCmdContext1.AddFunction(EngineKeyFunctions.Walk);
    iinputCmdContext1.AddFunction(ContentKeyFunctions.SwapHands);
    iinputCmdContext1.AddFunction(ContentKeyFunctions.SwapHandsReverse);
    iinputCmdContext1.AddFunction(ContentKeyFunctions.Drop);
    iinputCmdContext1.AddFunction(ContentKeyFunctions.UseItemInHand);
    iinputCmdContext1.AddFunction(ContentKeyFunctions.AltUseItemInHand);
    iinputCmdContext1.AddFunction(ContentKeyFunctions.ActivateItemInWorld);
    iinputCmdContext1.AddFunction(ContentKeyFunctions.ThrowItemInHand);
    iinputCmdContext1.AddFunction(ContentKeyFunctions.AltActivateItemInWorld);
    iinputCmdContext1.AddFunction(ContentKeyFunctions.TryPullObject);
    iinputCmdContext1.AddFunction(ContentKeyFunctions.MovePulledObject);
    iinputCmdContext1.AddFunction(ContentKeyFunctions.ReleasePulledObject);
    IInputCmdContext iinputCmdContext2 = contexts.New("ghost", "human");
    iinputCmdContext2.AddFunction(EngineKeyFunctions.MoveUp);
    iinputCmdContext2.AddFunction(EngineKeyFunctions.MoveDown);
    iinputCmdContext2.AddFunction(EngineKeyFunctions.MoveLeft);
    iinputCmdContext2.AddFunction(EngineKeyFunctions.MoveRight);
    iinputCmdContext2.AddFunction(EngineKeyFunctions.Walk);
    context1.AddFunction(ContentKeyFunctions.OpenEntitySpawnWindow);
    context1.AddFunction(ContentKeyFunctions.OpenSandboxWindow);
    context1.AddFunction(ContentKeyFunctions.OpenTileSpawnWindow);
    context1.AddFunction(ContentKeyFunctions.OpenDecalSpawnWindow);
    context1.AddFunction(ContentKeyFunctions.OpenAdminMenu);
    context1.AddFunction(ContentKeyFunctions.OpenGuidebook);
    ContentContexts.CMFunctions(contexts);
  }

  private static void CMFunctions(IInputContextContainer contexts)
  {
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
    IInputCmdContext iinputCmdContext = contexts.New("xenonid", "human");
    iinputCmdContext.AddFunction(CMKeyFunctions.CMXenoWideSwing);
    iinputCmdContext.AddFunction(CMKeyFunctions.RMCXenoRest);
  }
}
