// Decompiled with JetBrains decompiler
// Type: Content.Shared.Input.ContentKeyFunctions
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Input;

#nullable enable
namespace Content.Shared.Input;

[KeyFunctions]
public static class ContentKeyFunctions
{
  public static readonly BoundKeyFunction UseItemInHand = (BoundKeyFunction) "ActivateItemInHand";
  public static readonly BoundKeyFunction AltUseItemInHand = (BoundKeyFunction) "AltActivateItemInHand";
  public static readonly BoundKeyFunction ActivateItemInWorld = (BoundKeyFunction) nameof (ActivateItemInWorld);
  public static readonly BoundKeyFunction AltActivateItemInWorld = (BoundKeyFunction) nameof (AltActivateItemInWorld);
  public static readonly BoundKeyFunction Drop = (BoundKeyFunction) nameof (Drop);
  public static readonly BoundKeyFunction ExamineEntity = (BoundKeyFunction) nameof (ExamineEntity);
  public static readonly BoundKeyFunction FocusChat = (BoundKeyFunction) "FocusChatInputWindow";
  public static readonly BoundKeyFunction FocusLocalChat = (BoundKeyFunction) "FocusLocalChatWindow";
  public static readonly BoundKeyFunction FocusEmote = (BoundKeyFunction) nameof (FocusEmote);
  public static readonly BoundKeyFunction FocusWhisperChat = (BoundKeyFunction) "FocusWhisperChatWindow";
  public static readonly BoundKeyFunction FocusRadio = (BoundKeyFunction) "FocusRadioWindow";
  public static readonly BoundKeyFunction FocusLOOC = (BoundKeyFunction) "FocusLOOCWindow";
  public static readonly BoundKeyFunction FocusOOC = (BoundKeyFunction) "FocusOOCWindow";
  public static readonly BoundKeyFunction FocusAdminChat = (BoundKeyFunction) "FocusAdminChatWindow";
  public static readonly BoundKeyFunction FocusDeadChat = (BoundKeyFunction) "FocusDeadChatWindow";
  public static readonly BoundKeyFunction FocusConsoleChat = (BoundKeyFunction) "FocusConsoleChatWindow";
  public static readonly BoundKeyFunction CycleChatChannelForward = (BoundKeyFunction) nameof (CycleChatChannelForward);
  public static readonly BoundKeyFunction CycleChatChannelBackward = (BoundKeyFunction) nameof (CycleChatChannelBackward);
  public static readonly BoundKeyFunction EscapeContext = (BoundKeyFunction) nameof (EscapeContext);
  public static readonly BoundKeyFunction OpenCharacterMenu = (BoundKeyFunction) nameof (OpenCharacterMenu);
  public static readonly BoundKeyFunction OpenEmotesMenu = (BoundKeyFunction) nameof (OpenEmotesMenu);
  public static readonly BoundKeyFunction OpenCraftingMenu = (BoundKeyFunction) nameof (OpenCraftingMenu);
  public static readonly BoundKeyFunction OpenGuidebook = (BoundKeyFunction) nameof (OpenGuidebook);
  public static readonly BoundKeyFunction OpenInventoryMenu = (BoundKeyFunction) nameof (OpenInventoryMenu);
  public static readonly BoundKeyFunction SmartEquipBackpack = (BoundKeyFunction) nameof (SmartEquipBackpack);
  public static readonly BoundKeyFunction SmartEquipBelt = (BoundKeyFunction) nameof (SmartEquipBelt);
  public static readonly BoundKeyFunction OpenBackpack = (BoundKeyFunction) nameof (OpenBackpack);
  public static readonly BoundKeyFunction OpenBelt = (BoundKeyFunction) nameof (OpenBelt);
  public static readonly BoundKeyFunction OpenAHelp = (BoundKeyFunction) nameof (OpenAHelp);
  public static readonly BoundKeyFunction SwapHands = (BoundKeyFunction) nameof (SwapHands);
  public static readonly BoundKeyFunction SwapHandsReverse = (BoundKeyFunction) nameof (SwapHandsReverse);
  public static readonly BoundKeyFunction MoveStoredItem = (BoundKeyFunction) nameof (MoveStoredItem);
  public static readonly BoundKeyFunction RotateStoredItem = (BoundKeyFunction) nameof (RotateStoredItem);
  public static readonly BoundKeyFunction SaveItemLocation = (BoundKeyFunction) nameof (SaveItemLocation);
  public static readonly BoundKeyFunction ThrowItemInHand = (BoundKeyFunction) nameof (ThrowItemInHand);
  public static readonly BoundKeyFunction TryPullObject = (BoundKeyFunction) nameof (TryPullObject);
  public static readonly BoundKeyFunction MovePulledObject = (BoundKeyFunction) nameof (MovePulledObject);
  public static readonly BoundKeyFunction ReleasePulledObject = (BoundKeyFunction) nameof (ReleasePulledObject);
  public static readonly BoundKeyFunction MouseMiddle = (BoundKeyFunction) nameof (MouseMiddle);
  public static readonly BoundKeyFunction RotateObjectClockwise = (BoundKeyFunction) nameof (RotateObjectClockwise);
  public static readonly BoundKeyFunction RotateObjectCounterclockwise = (BoundKeyFunction) nameof (RotateObjectCounterclockwise);
  public static readonly BoundKeyFunction FlipObject = (BoundKeyFunction) nameof (FlipObject);
  public static readonly BoundKeyFunction ToggleRoundEndSummaryWindow = (BoundKeyFunction) nameof (ToggleRoundEndSummaryWindow);
  public static readonly BoundKeyFunction OpenEntitySpawnWindow = (BoundKeyFunction) nameof (OpenEntitySpawnWindow);
  public static readonly BoundKeyFunction OpenSandboxWindow = (BoundKeyFunction) nameof (OpenSandboxWindow);
  public static readonly BoundKeyFunction OpenTileSpawnWindow = (BoundKeyFunction) nameof (OpenTileSpawnWindow);
  public static readonly BoundKeyFunction OpenDecalSpawnWindow = (BoundKeyFunction) nameof (OpenDecalSpawnWindow);
  public static readonly BoundKeyFunction OpenAdminMenu = (BoundKeyFunction) nameof (OpenAdminMenu);
  public static readonly BoundKeyFunction TakeScreenshot = (BoundKeyFunction) nameof (TakeScreenshot);
  public static readonly BoundKeyFunction TakeScreenshotNoUI = (BoundKeyFunction) nameof (TakeScreenshotNoUI);
  public static readonly BoundKeyFunction ToggleFullscreen = (BoundKeyFunction) nameof (ToggleFullscreen);
  public static readonly BoundKeyFunction Point = (BoundKeyFunction) nameof (Point);
  public static readonly BoundKeyFunction ZoomOut = (BoundKeyFunction) nameof (ZoomOut);
  public static readonly BoundKeyFunction ZoomIn = (BoundKeyFunction) nameof (ZoomIn);
  public static readonly BoundKeyFunction ResetZoom = (BoundKeyFunction) nameof (ResetZoom);
  public static readonly BoundKeyFunction ArcadeUp = (BoundKeyFunction) nameof (ArcadeUp);
  public static readonly BoundKeyFunction ArcadeDown = (BoundKeyFunction) nameof (ArcadeDown);
  public static readonly BoundKeyFunction ArcadeLeft = (BoundKeyFunction) nameof (ArcadeLeft);
  public static readonly BoundKeyFunction ArcadeRight = (BoundKeyFunction) nameof (ArcadeRight);
  public static readonly BoundKeyFunction Arcade1 = (BoundKeyFunction) nameof (Arcade1);
  public static readonly BoundKeyFunction Arcade2 = (BoundKeyFunction) nameof (Arcade2);
  public static readonly BoundKeyFunction Arcade3 = (BoundKeyFunction) nameof (Arcade3);
  public static readonly BoundKeyFunction OpenActionsMenu = (BoundKeyFunction) "OpenAbilitiesMenu";
  public static readonly BoundKeyFunction ShuttleStrafeLeft = (BoundKeyFunction) nameof (ShuttleStrafeLeft);
  public static readonly BoundKeyFunction ShuttleStrafeUp = (BoundKeyFunction) nameof (ShuttleStrafeUp);
  public static readonly BoundKeyFunction ShuttleStrafeRight = (BoundKeyFunction) nameof (ShuttleStrafeRight);
  public static readonly BoundKeyFunction ShuttleStrafeDown = (BoundKeyFunction) nameof (ShuttleStrafeDown);
  public static readonly BoundKeyFunction ShuttleRotateLeft = (BoundKeyFunction) nameof (ShuttleRotateLeft);
  public static readonly BoundKeyFunction ShuttleRotateRight = (BoundKeyFunction) nameof (ShuttleRotateRight);
  public static readonly BoundKeyFunction ShuttleBrake = (BoundKeyFunction) nameof (ShuttleBrake);
  public static readonly BoundKeyFunction Hotbar0 = (BoundKeyFunction) nameof (Hotbar0);
  public static readonly BoundKeyFunction Hotbar1 = (BoundKeyFunction) nameof (Hotbar1);
  public static readonly BoundKeyFunction Hotbar2 = (BoundKeyFunction) nameof (Hotbar2);
  public static readonly BoundKeyFunction Hotbar3 = (BoundKeyFunction) nameof (Hotbar3);
  public static readonly BoundKeyFunction Hotbar4 = (BoundKeyFunction) nameof (Hotbar4);
  public static readonly BoundKeyFunction Hotbar5 = (BoundKeyFunction) nameof (Hotbar5);
  public static readonly BoundKeyFunction Hotbar6 = (BoundKeyFunction) nameof (Hotbar6);
  public static readonly BoundKeyFunction Hotbar7 = (BoundKeyFunction) nameof (Hotbar7);
  public static readonly BoundKeyFunction Hotbar8 = (BoundKeyFunction) nameof (Hotbar8);
  public static readonly BoundKeyFunction Hotbar9 = (BoundKeyFunction) nameof (Hotbar9);
  public static readonly BoundKeyFunction HotbarShift0 = (BoundKeyFunction) nameof (HotbarShift0);
  public static readonly BoundKeyFunction HotbarShift1 = (BoundKeyFunction) nameof (HotbarShift1);
  public static readonly BoundKeyFunction HotbarShift2 = (BoundKeyFunction) nameof (HotbarShift2);
  public static readonly BoundKeyFunction HotbarShift3 = (BoundKeyFunction) nameof (HotbarShift3);
  public static readonly BoundKeyFunction HotbarShift4 = (BoundKeyFunction) nameof (HotbarShift4);
  public static readonly BoundKeyFunction HotbarShift5 = (BoundKeyFunction) nameof (HotbarShift5);
  public static readonly BoundKeyFunction HotbarShift6 = (BoundKeyFunction) nameof (HotbarShift6);
  public static readonly BoundKeyFunction HotbarShift7 = (BoundKeyFunction) nameof (HotbarShift7);
  public static readonly BoundKeyFunction HotbarShift8 = (BoundKeyFunction) nameof (HotbarShift8);
  public static readonly BoundKeyFunction HotbarShift9 = (BoundKeyFunction) nameof (HotbarShift9);
  public static readonly BoundKeyFunction Vote0 = (BoundKeyFunction) nameof (Vote0);
  public static readonly BoundKeyFunction Vote1 = (BoundKeyFunction) nameof (Vote1);
  public static readonly BoundKeyFunction Vote2 = (BoundKeyFunction) nameof (Vote2);
  public static readonly BoundKeyFunction Vote3 = (BoundKeyFunction) nameof (Vote3);
  public static readonly BoundKeyFunction Vote4 = (BoundKeyFunction) nameof (Vote4);
  public static readonly BoundKeyFunction Vote5 = (BoundKeyFunction) nameof (Vote5);
  public static readonly BoundKeyFunction Vote6 = (BoundKeyFunction) nameof (Vote6);
  public static readonly BoundKeyFunction Vote7 = (BoundKeyFunction) nameof (Vote7);
  public static readonly BoundKeyFunction Vote8 = (BoundKeyFunction) nameof (Vote8);
  public static readonly BoundKeyFunction Vote9 = (BoundKeyFunction) nameof (Vote9);
  public static readonly BoundKeyFunction EditorCopyObject = (BoundKeyFunction) nameof (EditorCopyObject);
  public static readonly BoundKeyFunction EditorFlipObject = (BoundKeyFunction) nameof (EditorFlipObject);
  public static readonly BoundKeyFunction InspectEntity = (BoundKeyFunction) nameof (InspectEntity);
  public static readonly BoundKeyFunction MappingUnselect = (BoundKeyFunction) nameof (MappingUnselect);
  public static readonly BoundKeyFunction SaveMap = (BoundKeyFunction) nameof (SaveMap);
  public static readonly BoundKeyFunction MappingEnablePick = (BoundKeyFunction) nameof (MappingEnablePick);
  public static readonly BoundKeyFunction MappingEnableDelete = (BoundKeyFunction) nameof (MappingEnableDelete);
  public static readonly BoundKeyFunction MappingPick = (BoundKeyFunction) nameof (MappingPick);
  public static readonly BoundKeyFunction MappingRemoveDecal = (BoundKeyFunction) nameof (MappingRemoveDecal);
  public static readonly BoundKeyFunction MappingCancelEraseDecal = (BoundKeyFunction) nameof (MappingCancelEraseDecal);
  public static readonly BoundKeyFunction MappingOpenContextMenu = (BoundKeyFunction) nameof (MappingOpenContextMenu);

  public static BoundKeyFunction[] GetHotbarBoundKeys()
  {
    return new BoundKeyFunction[20]
    {
      ContentKeyFunctions.Hotbar1,
      ContentKeyFunctions.Hotbar2,
      ContentKeyFunctions.Hotbar3,
      ContentKeyFunctions.Hotbar4,
      ContentKeyFunctions.Hotbar5,
      ContentKeyFunctions.Hotbar6,
      ContentKeyFunctions.Hotbar7,
      ContentKeyFunctions.Hotbar8,
      ContentKeyFunctions.Hotbar9,
      ContentKeyFunctions.Hotbar0,
      ContentKeyFunctions.HotbarShift1,
      ContentKeyFunctions.HotbarShift2,
      ContentKeyFunctions.HotbarShift3,
      ContentKeyFunctions.HotbarShift4,
      ContentKeyFunctions.HotbarShift5,
      ContentKeyFunctions.HotbarShift6,
      ContentKeyFunctions.HotbarShift7,
      ContentKeyFunctions.HotbarShift8,
      ContentKeyFunctions.HotbarShift9,
      ContentKeyFunctions.HotbarShift0
    };
  }
}
