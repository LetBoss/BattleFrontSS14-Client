// Decompiled with JetBrains decompiler
// Type: Content.Client.Holopad.HolopadBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Holopad;
using Content.Shared.Silicons.StationAi;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client.Holopad;

public sealed class HolopadBoundUserInterface : BoundUserInterface
{
  [Dependency]
  private ISharedPlayerManager _playerManager;
  [Robust.Shared.ViewVariables.ViewVariables]
  private HolopadWindow? _window;

  public HolopadBoundUserInterface(EntityUid owner, Enum uiKey)
    : base(owner, uiKey)
  {
    IoCManager.InjectDependencies<HolopadBoundUserInterface>(this);
  }

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<HolopadWindow>((BoundUserInterface) this);
    this._window.Title = Loc.GetString("holopad-window-title", new (string, object)[1]
    {
      ("title", (object) this.EntMan.GetComponent<MetaDataComponent>(this.Owner).EntityName)
    });
    if (!(this.UiKey is HolopadUiKey))
    {
      this.Close();
    }
    else
    {
      HolopadUiKey uiKey = (HolopadUiKey) this.UiKey;
      if (uiKey == HolopadUiKey.InteractionWindow && this.EntMan.HasComponent<StationAiHeldComponent>(this._playerManager.LocalEntity))
        uiKey = HolopadUiKey.InteractionWindowForAi;
      this._window.SetState(this.Owner, uiKey);
      this._window.UpdateState(new Dictionary<NetEntity, string>());
      this._window.SendHolopadStartNewCallMessageAction += new Action<NetEntity>(this.SendHolopadStartNewCallMessage);
      this._window.SendHolopadAnswerCallMessageAction += new Action(this.SendHolopadAnswerCallMessage);
      this._window.SendHolopadEndCallMessageAction += new Action(this.SendHolopadEndCallMessage);
      this._window.SendHolopadStartBroadcastMessageAction += new Action(this.SendHolopadStartBroadcastMessage);
      this._window.SendHolopadActivateProjectorMessageAction += new Action(this.SendHolopadActivateProjectorMessage);
      this._window.SendHolopadRequestStationAiMessageAction += new Action(this.SendHolopadRequestStationAiMessage);
      if (uiKey == HolopadUiKey.AiRequestWindow)
        this._window.OpenCenteredAt(new Vector2(1f, 1f));
      else
        this._window.OpenCenteredAt(new Vector2(0.3333f, 0.5f));
    }
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    HolopadBoundInterfaceState boundInterfaceState = (HolopadBoundInterfaceState) state;
    TransformComponent transformComponent;
    this.EntMan.TryGetComponent<TransformComponent>(this.Owner, ref transformComponent);
    this._window?.UpdateState(boundInterfaceState.Holopads);
  }

  public void SendHolopadStartNewCallMessage(NetEntity receiver)
  {
    this.SendMessage((BoundUserInterfaceMessage) new HolopadStartNewCallMessage(receiver));
  }

  public void SendHolopadAnswerCallMessage()
  {
    this.SendMessage((BoundUserInterfaceMessage) new HolopadAnswerCallMessage());
  }

  public void SendHolopadEndCallMessage()
  {
    this.SendMessage((BoundUserInterfaceMessage) new HolopadEndCallMessage());
  }

  public void SendHolopadStartBroadcastMessage()
  {
    this.SendMessage((BoundUserInterfaceMessage) new HolopadStartBroadcastMessage());
  }

  public void SendHolopadActivateProjectorMessage()
  {
    this.SendMessage((BoundUserInterfaceMessage) new HolopadActivateProjectorMessage());
  }

  public void SendHolopadRequestStationAiMessage()
  {
    this.SendMessage((BoundUserInterfaceMessage) new HolopadStationAiRequestMessage());
  }
}
