// Decompiled with JetBrains decompiler
// Type: Content.Client.Clothing.UI.ChameleonBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Clothing.Systems;
using Content.Shared.Clothing.Components;
using Content.Shared.Tag;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Clothing.UI;

public sealed class ChameleonBoundUserInterface : BoundUserInterface
{
  [Dependency]
  private IPrototypeManager _proto;
  private readonly ChameleonClothingSystem _chameleon;
  private readonly TagSystem _tag;
  [Robust.Shared.ViewVariables.ViewVariables]
  private ChameleonMenu? _menu;

  public ChameleonBoundUserInterface(EntityUid owner, Enum uiKey)
    : base(owner, uiKey)
  {
    this._chameleon = this.EntMan.System<ChameleonClothingSystem>();
    this._tag = this.EntMan.System<TagSystem>();
  }

  protected virtual void Open()
  {
    base.Open();
    this._menu = BoundUserInterfaceExt.CreateWindow<ChameleonMenu>((BoundUserInterface) this);
    this._menu.OnIdSelected += new Action<string>(this.OnIdSelected);
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (!(state is ChameleonBoundUserInterfaceState userInterfaceState))
      return;
    IEnumerable<EntProtoId> validTargets = this._chameleon.GetValidTargets(userInterfaceState.Slot);
    if (userInterfaceState.RequiredTag != null)
    {
      List<EntProtoId> possibleIds = new List<EntProtoId>();
      foreach (EntProtoId entProtoId in validTargets)
      {
        EntityPrototype entityPrototype;
        TagComponent component;
        if (!string.IsNullOrEmpty(EntProtoId.op_Implicit(entProtoId)) && this._proto.TryIndex(entProtoId, ref entityPrototype) && entityPrototype.TryGetComponent<TagComponent>(ref component, this.EntMan.ComponentFactory) && this._tag.HasTag(component, ProtoId<TagPrototype>.op_Implicit(userInterfaceState.RequiredTag)))
          possibleIds.Add(entProtoId);
      }
      this._menu?.UpdateState((IEnumerable<EntProtoId>) possibleIds, userInterfaceState.SelectedId);
    }
    else
      this._menu?.UpdateState(validTargets, userInterfaceState.SelectedId);
  }

  private void OnIdSelected(string selectedId)
  {
    this.SendMessage((BoundUserInterfaceMessage) new ChameleonPrototypeSelectedMessage(selectedId));
  }
}
