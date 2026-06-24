// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Xenonids.Construction.XenoOrderConstructionBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._RMC14.Xenonids.UI;
using Content.Shared._RMC14.Xenonids.Construction;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client._RMC14.Xenonids.Construction;

public sealed class XenoOrderConstructionBui : BoundUserInterface
{
  [Dependency]
  private IPrototypeManager _prototype;
  private readonly SpriteSystem _sprite;
  private readonly Dictionary<EntProtoId, XenoChoiceControl> _buttons = new Dictionary<EntProtoId, XenoChoiceControl>();
  [Robust.Shared.ViewVariables.ViewVariables]
  private XenoChooseStructureWindow? _window;

  public XenoOrderConstructionBui(EntityUid owner, Enum uiKey)
    : base(owner, uiKey)
  {
    this._sprite = this.EntMan.System<SpriteSystem>();
  }

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<XenoChooseStructureWindow>((BoundUserInterface) this);
    this._window.Title = Loc.GetString("cm-xeno-order-construction");
    this._buttons.Clear();
    XenoConstructionComponent constructionComponent;
    if (!this.EntMan.TryGetComponent<XenoConstructionComponent>(this.Owner, ref constructionComponent))
      return;
    foreach (EntProtoId entProtoId in constructionComponent.CanOrderConstruction)
    {
      EntProtoId structureId = entProtoId;
      EntityPrototype entityPrototype;
      if (this._prototype.TryIndex(structureId, ref entityPrototype))
      {
        XenoChoiceControl xenoChoiceControl = new XenoChoiceControl();
        ((BaseButton) xenoChoiceControl.Button).ToggleMode = false;
        xenoChoiceControl.Set(entityPrototype.Name, this._sprite.Frame0(entityPrototype));
        ((BaseButton) xenoChoiceControl.Button).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
        {
          this.SendPredictedMessage((BoundUserInterfaceMessage) new XenoOrderConstructionBuiMsg(structureId));
          this.Close();
        });
        ((Control) this._window.StructureContainer).AddChild((Control) xenoChoiceControl);
        this._buttons.Add(structureId, xenoChoiceControl);
      }
    }
  }
}
