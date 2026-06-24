// Decompiled with JetBrains decompiler
// Type: Content.Client.Humanoid.HumanoidMarkingModifierBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Markings;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Humanoid;

public sealed class HumanoidMarkingModifierBoundUserInterface(EntityUid owner, Enum uiKey) : 
  BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private HumanoidMarkingModifierWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindowCenteredLeft<HumanoidMarkingModifierWindow>((BoundUserInterface) this);
    this._window.OnMarkingAdded += new Action<MarkingSet>(this.SendMarkingSet);
    this._window.OnMarkingRemoved += new Action<MarkingSet>(this.SendMarkingSet);
    this._window.OnMarkingColorChange += new Action<MarkingSet>(this.SendMarkingSetNoResend);
    this._window.OnMarkingRankChange += new Action<MarkingSet>(this.SendMarkingSet);
    this._window.OnLayerInfoModified += new Action<HumanoidVisualLayers, CustomBaseLayerInfo?>(this.SendBaseLayer);
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (this._window == null || !(state is HumanoidMarkingModifierState markingModifierState))
      return;
    this._window.SetState(markingModifierState.MarkingSet, markingModifierState.Species, markingModifierState.Sex, markingModifierState.SkinColor, markingModifierState.CustomBaseLayers);
  }

  private void SendMarkingSet(MarkingSet set)
  {
    this.SendMessage((BoundUserInterfaceMessage) new HumanoidMarkingModifierMarkingSetMessage(set, true));
  }

  private void SendMarkingSetNoResend(MarkingSet set)
  {
    this.SendMessage((BoundUserInterfaceMessage) new HumanoidMarkingModifierMarkingSetMessage(set, false));
  }

  private void SendBaseLayer(HumanoidVisualLayers layer, CustomBaseLayerInfo? info)
  {
    this.SendMessage((BoundUserInterfaceMessage) new HumanoidMarkingModifierBaseLayersSetMessage(layer, info, true));
  }
}
