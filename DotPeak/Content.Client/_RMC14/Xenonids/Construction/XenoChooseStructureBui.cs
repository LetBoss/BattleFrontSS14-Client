// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Xenonids.Construction.XenoChooseStructureBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._RMC14.Xenonids.UI;
using Content.Shared._RMC14.Xenonids.Construction;
using Content.Shared.FixedPoint;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client._RMC14.Xenonids.Construction;

public sealed class XenoChooseStructureBui : BoundUserInterface
{
  [Dependency]
  private IPrototypeManager _prototype;
  private readonly SpriteSystem _sprite;
  private readonly SharedXenoConstructionSystem _xenoConstruction;
  private readonly Dictionary<EntProtoId, XenoChoiceControl> _buttons = new Dictionary<EntProtoId, XenoChoiceControl>();
  [Robust.Shared.ViewVariables.ViewVariables]
  private XenoChooseStructureWindow? _window;

  public XenoChooseStructureBui(EntityUid owner, Enum uiKey)
    : base(owner, uiKey)
  {
    this._sprite = this.EntMan.System<SpriteSystem>();
    this._xenoConstruction = this.EntMan.System<SharedXenoConstructionSystem>();
  }

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<XenoChooseStructureWindow>((BoundUserInterface) this);
    this._buttons.Clear();
    XenoConstructionComponent constructionComponent;
    if (this.EntMan.TryGetComponent<XenoConstructionComponent>(this.Owner, ref constructionComponent))
    {
      bool flag = this.EntMan.HasComponent<QueenBuildingBoostComponent>(this.Owner);
      foreach (EntProtoId entProtoId1 in constructionComponent.CanBuild)
      {
        EntProtoId structureId = entProtoId1;
        EntityPrototype entityPrototype1;
        if (this._prototype.TryIndex(structureId, ref entityPrototype1))
        {
          XenoChoiceControl xenoChoiceControl = new XenoChoiceControl();
          ((BaseButton) xenoChoiceControl.Button).ToggleMode = true;
          EntProtoId entProtoId2 = structureId;
          string name = entityPrototype1.Name;
          if (flag)
          {
            EntProtoId queenVariant = this.GetQueenVariant(structureId);
            EntityPrototype entityPrototype2;
            if (this._prototype.TryIndex(queenVariant, ref entityPrototype2) && EntProtoId.op_Inequality(queenVariant, structureId))
            {
              entProtoId2 = queenVariant;
              name = entityPrototype2.Name;
            }
            name += " (0 plasma)";
          }
          else
          {
            FixedPoint2? structurePlasmaCost = this._xenoConstruction.GetStructurePlasmaCost(structureId);
            if (structurePlasmaCost.HasValue)
            {
              FixedPoint2 valueOrDefault = structurePlasmaCost.GetValueOrDefault();
              name += $" ({valueOrDefault} plasma)";
            }
          }
          xenoChoiceControl.Set(name, this._sprite.Frame0(this._prototype.Index(entProtoId2)));
          ((BaseButton) xenoChoiceControl.Button).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
          {
            this.SendPredictedMessage((BoundUserInterfaceMessage) new XenoChooseStructureBuiMsg(structureId));
            this.UpdateButtonStates(structureId);
          });
          ((Control) this._window.StructureContainer).AddChild((Control) xenoChoiceControl);
          this._buttons.Add(structureId, xenoChoiceControl);
        }
      }
    }
    this.Refresh();
  }

  private EntProtoId GetQueenVariant(EntProtoId originalId)
  {
    EntProtoId queenVariant;
    switch (((EntProtoId) ref originalId).Id)
    {
      case "WallXenoResin":
        queenVariant = EntProtoId.op_Implicit("WallXenoResinQueen");
        break;
      case "WallXenoMembrane":
        queenVariant = EntProtoId.op_Implicit("WallXenoMembraneQueen");
        break;
      case "DoorXenoResin":
        queenVariant = EntProtoId.op_Implicit("DoorXenoResinQueen");
        break;
      default:
        queenVariant = originalId;
        break;
    }
    return queenVariant;
  }

  private void UpdateButtonStates(EntProtoId selectedId)
  {
    foreach ((EntProtoId key, XenoChoiceControl xenoChoiceControl) in this._buttons)
      ((BaseButton) xenoChoiceControl.Button).Pressed = EntProtoId.op_Equality(key, selectedId);
  }

  public void Refresh()
  {
    foreach ((EntProtoId _, XenoChoiceControl xenoChoiceControl) in this._buttons)
      ((BaseButton) xenoChoiceControl.Button).Pressed = false;
    EntProtoId? buildChoice = (EntProtoId?) EntityManagerExt.GetComponentOrNull<XenoConstructionComponent>(this.EntMan, this.Owner)?.BuildChoice;
    XenoChoiceControl xenoChoiceControl1;
    if (!buildChoice.HasValue || !this._buttons.TryGetValue(buildChoice.GetValueOrDefault(), out xenoChoiceControl1))
      return;
    ((BaseButton) xenoChoiceControl1.Button).Pressed = true;
  }
}
