// Decompiled with JetBrains decompiler
// Type: Content.Client.MagicMirror.MagicMirrorBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Humanoid.Markings;
using Content.Shared.MagicMirror;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.MagicMirror;

public sealed class MagicMirrorBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private MagicMirrorWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<MagicMirrorWindow>((BoundUserInterface) this);
    this._window.OnHairSelected += (Action<(int, string)>) (tuple => this.SelectHair(MagicMirrorCategory.Hair, tuple.id, tuple.slot));
    this._window.OnHairColorChanged += (Action<(int, Marking)>) (args => this.ChangeColor(MagicMirrorCategory.Hair, args.marking, args.slot));
    this._window.OnHairSlotAdded += (Action) (() => this.AddSlot(MagicMirrorCategory.Hair));
    this._window.OnHairSlotRemoved += (Action<int>) (args => this.RemoveSlot(MagicMirrorCategory.Hair, args));
    this._window.OnFacialHairSelected += (Action<(int, string)>) (tuple => this.SelectHair(MagicMirrorCategory.FacialHair, tuple.id, tuple.slot));
    this._window.OnFacialHairColorChanged += (Action<(int, Marking)>) (args => this.ChangeColor(MagicMirrorCategory.FacialHair, args.marking, args.slot));
    this._window.OnFacialHairSlotAdded += (Action) (() => this.AddSlot(MagicMirrorCategory.FacialHair));
    this._window.OnFacialHairSlotRemoved += (Action<int>) (args => this.RemoveSlot(MagicMirrorCategory.FacialHair, args));
  }

  private void SelectHair(MagicMirrorCategory category, string marking, int slot)
  {
    this.SendMessage((BoundUserInterfaceMessage) new MagicMirrorSelectMessage(category, marking, slot));
  }

  private void ChangeColor(MagicMirrorCategory category, Marking marking, int slot)
  {
    this.SendMessage((BoundUserInterfaceMessage) new MagicMirrorChangeColorMessage(category, new List<Color>((IEnumerable<Color>) marking.MarkingColors), slot));
  }

  private void RemoveSlot(MagicMirrorCategory category, int slot)
  {
    this.SendMessage((BoundUserInterfaceMessage) new MagicMirrorRemoveSlotMessage(category, slot));
  }

  private void AddSlot(MagicMirrorCategory category)
  {
    this.SendMessage((BoundUserInterfaceMessage) new MagicMirrorAddSlotMessage(category));
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (!(state is MagicMirrorUiState state1) || this._window == null)
      return;
    this._window.UpdateState(state1);
  }
}
