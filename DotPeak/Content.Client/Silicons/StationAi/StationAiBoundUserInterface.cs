// Decompiled with JetBrains decompiler
// Type: Content.Client.Silicons.StationAi.StationAiBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.UserInterface.Controls;
using Content.Shared.Silicons.StationAi;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Silicons.StationAi;

public sealed class StationAiBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  private SimpleRadialMenu? _menu;

  protected virtual void Open()
  {
    base.Open();
    GetStationAiRadialEvent stationAiRadialEvent = new GetStationAiRadialEvent();
    ((IDirectedEventBus) this.EntMan.EventBus).RaiseLocalEvent<GetStationAiRadialEvent>(this.Owner, ref stationAiRadialEvent, false);
    this._menu = BoundUserInterfaceExt.CreateWindow<SimpleRadialMenu>((BoundUserInterface) this);
    this._menu.Track(this.Owner);
    this._menu.SetButtons((IEnumerable<RadialMenuOption>) this.ConvertToButtons((IReadOnlyList<StationAiRadial>) stationAiRadialEvent.Actions));
    this._menu.Open();
  }

  private IEnumerable<RadialMenuActionOption> ConvertToButtons(
    IReadOnlyList<StationAiRadial> actions)
  {
    RadialMenuActionOption[] buttons = new RadialMenuActionOption[actions.Count];
    for (int index1 = 0; index1 < actions.Count; ++index1)
    {
      StationAiRadial action = actions[index1];
      RadialMenuActionOption[] menuActionOptionArray = buttons;
      int index2 = index1;
      RadialMenuActionOption<BaseStationAiAction> menuActionOption = new RadialMenuActionOption<BaseStationAiAction>(new Action<BaseStationAiAction>(this.HandleRadialMenuClick), action.Event);
      menuActionOption.Sprite = action.Sprite;
      menuActionOption.ToolTip = action.Tooltip;
      menuActionOptionArray[index2] = (RadialMenuActionOption) menuActionOption;
    }
    return (IEnumerable<RadialMenuActionOption>) buttons;
  }

  private void HandleRadialMenuClick(BaseStationAiAction p)
  {
    this.SendPredictedMessage((BoundUserInterfaceMessage) new StationAiRadialMessage()
    {
      Event = p
    });
  }
}
