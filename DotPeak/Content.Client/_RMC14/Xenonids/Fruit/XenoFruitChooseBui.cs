// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Xenonids.Fruit.XenoFruitChooseBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._RMC14.Xenonids.UI;
using Content.Shared._RMC14.Xenonids.Fruit;
using Content.Shared._RMC14.Xenonids.Fruit.Components;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client._RMC14.Xenonids.Fruit;

public sealed class XenoFruitChooseBui : BoundUserInterface
{
  [Dependency]
  private IPrototypeManager _prototype;
  private readonly SpriteSystem _sprite;
  private readonly SharedXenoFruitSystem _xenoFruit;
  private readonly Dictionary<EntProtoId, XenoChoiceControl> _buttons = new Dictionary<EntProtoId, XenoChoiceControl>();
  [Robust.Shared.ViewVariables.ViewVariables]
  private XenoFruitChooseWindow? _window;

  public XenoFruitChooseBui(EntityUid owner, Enum uiKey)
    : base(owner, uiKey)
  {
    this._sprite = this.EntMan.System<SpriteSystem>();
    this._xenoFruit = this.EntMan.System<SharedXenoFruitSystem>();
  }

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<XenoFruitChooseWindow>((BoundUserInterface) this);
    this._buttons.Clear();
    ButtonGroup buttonGroup = new ButtonGroup(true);
    XenoFruitPlanterComponent planterComponent;
    if (this.EntMan.TryGetComponent<XenoFruitPlanterComponent>(this.Owner, ref planterComponent))
    {
      this._window.FruitCountLabel.Text = Loc.GetString("rmc-xeno-fruit-ui-count", new (string, object)[2]
      {
        ("count", (object) planterComponent.PlantedFruit.Count),
        ("max", (object) planterComponent.MaxFruitAllowed)
      });
      foreach (EntProtoId entProtoId in planterComponent.CanPlant)
      {
        EntProtoId fruitId = entProtoId;
        EntityPrototype ent;
        if (this._prototype.TryIndex(fruitId, ref ent))
        {
          string fruitSprite = this._xenoFruit.GetFruitSprite(ent);
          XenoChoiceControl xenoChoiceControl = new XenoChoiceControl();
          ((BaseButton) xenoChoiceControl.Button).Group = buttonGroup;
          string name = ent.Name;
          SpriteSpecifier.Rsi rsi = new SpriteSpecifier.Rsi(new ResPath("_RMC14/Structures/Xenos/xeno_fruit.rsi"), fruitSprite);
          xenoChoiceControl.Set(name, this._sprite.Frame0((SpriteSpecifier) rsi));
          ((BaseButton) xenoChoiceControl.Button).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new XenoFruitChooseBuiMsg(fruitId)));
          ((Control) xenoChoiceControl.Button).ToolTip = ent.Description;
          ((Control) xenoChoiceControl.Button).TooltipDelay = new float?(0.1f);
          ((Control) this._window.FruitContainer).AddChild((Control) xenoChoiceControl);
          this._buttons.Add(fruitId, xenoChoiceControl);
        }
      }
    }
    this.Refresh();
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    if (!(state is XenoFruitChooseBuiState state1))
      return;
    this.UpdateState(state1);
  }

  private void UpdateState(XenoFruitChooseBuiState state)
  {
    if (this._window == null)
      return;
    this._window.FruitCountLabel.Text = Loc.GetString("rmc-xeno-fruit-ui-count", new (string, object)[2]
    {
      ("count", (object) state.Count),
      ("max", (object) state.Max)
    });
  }

  public void Refresh()
  {
    XenoFruitPlanterComponent planterComponent;
    if (!this.EntMan.TryGetComponent<XenoFruitPlanterComponent>(this.Owner, ref planterComponent) || this._window == null)
      return;
    EntProtoId? fruitChoice = planterComponent.FruitChoice;
    XenoChoiceControl xenoChoiceControl;
    if (!fruitChoice.HasValue || !this._buttons.TryGetValue(fruitChoice.GetValueOrDefault(), out xenoChoiceControl))
      return;
    ((BaseButton) xenoChoiceControl.Button).Pressed = true;
  }
}
