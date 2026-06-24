// Decompiled with JetBrains decompiler
// Type: Content.Client.Silicons.StationAi.StationAiCustomizationBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Silicons.StationAi;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Client.Silicons.StationAi;

public sealed class StationAiCustomizationBoundUserInterface(EntityUid owner, Enum uiKey) : 
  BoundUserInterface(owner, uiKey)
{
  private StationAiCustomizationMenu? _menu;

  protected virtual void Open()
  {
    base.Open();
    this._menu = new StationAiCustomizationMenu(this.Owner);
    this._menu.OpenCentered();
    this._menu.OnClose += new Action(((BoundUserInterface) this).Close);
    this._menu.SendStationAiCustomizationMessageAction += new Action<ProtoId<StationAiCustomizationGroupPrototype>, ProtoId<StationAiCustomizationPrototype>>(this.SendStationAiCustomizationMessage);
  }

  public void SendStationAiCustomizationMessage(
    ProtoId<StationAiCustomizationGroupPrototype> groupProtoId,
    ProtoId<StationAiCustomizationPrototype> customizationProtoId)
  {
    this.SendPredictedMessage((BoundUserInterfaceMessage) new StationAiCustomizationMessage(groupProtoId, customizationProtoId));
  }

  protected virtual void Dispose(bool disposing)
  {
    base.Dispose(disposing);
    if (!disposing)
      return;
    ((Control) this._menu)?.Orphan();
    this._menu = (StationAiCustomizationMenu) null;
  }
}
