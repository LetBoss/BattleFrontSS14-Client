// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Dropship.Fabricator.DropshipFabricatorBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Message;
using Content.Shared._RMC14.Dropship.Fabricator;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Client._RMC14.Dropship.Fabricator;

public sealed class DropshipFabricatorBui : BoundUserInterface
{
  [Dependency]
  private IComponentFactory _compFactory;
  [Dependency]
  private IPrototypeManager _prototypes;
  [Robust.Shared.ViewVariables.ViewVariables]
  private DropshipFabricatorWindow? _window;
  private readonly DropshipFabricatorSystem _system;

  public DropshipFabricatorBui(EntityUid owner, Enum uiKey)
    : base(owner, uiKey)
  {
    IoCManager.InjectDependencies<DropshipFabricatorBui>(this);
    this._system = this.EntMan.System<DropshipFabricatorSystem>();
  }

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<DropshipFabricatorWindow>((BoundUserInterface) this);
    this._window.EquipmentLabel.SetMarkupPermissive(Loc.GetString("rmc-dropship-fabricator-equipment"));
    this._window.AmmoLabel.SetMarkupPermissive(Loc.GetString("rmc-dropship-fabricator-ammo"));
    this.Refresh();
    foreach (EntProtoId<DropshipFabricatorPrintableComponent> printable in this._system.Printables)
    {
      EntProtoId<DropshipFabricatorPrintableComponent> id = printable;
      EntityPrototype entityPrototype;
      DropshipFabricatorPrintableComponent printableComponent;
      if (this._prototypes.TryIndex(EntProtoId<DropshipFabricatorPrintableComponent>.op_Implicit(id), ref entityPrototype) && id.TryGet(ref printableComponent, this._prototypes, this._compFactory))
      {
        RichTextLabel richTextLabel = new RichTextLabel();
        ((Control) richTextLabel).Margin = new Thickness(4f, 2f);
        ((Control) richTextLabel).HorizontalExpand = false;
        RichTextLabel label = richTextLabel;
        label.SetMarkupPermissive(entityPrototype.Name);
        Button button1 = new Button();
        button1.Text = Loc.GetString("rmc-dropship-fabricator-fabricate", new (string, object)[1]
        {
          ("cost", (object) printableComponent.Cost)
        });
        ((Control) button1).StyleClasses.Add("OpenBoth");
        ((Control) button1).MinWidth = 120f;
        Button button2 = button1;
        ((BaseButton) button2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new DropshipFabricatorPrintMsg(EntProtoId<DropshipFabricatorPrintableComponent>.op_Implicit(id))));
        BoxContainer boxContainer1 = new BoxContainer();
        boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 0;
        ((Control) boxContainer1).Margin = new Thickness(0.0f, 4f);
        ((Control) boxContainer1).Children.Add((Control) label);
        ((Control) boxContainer1).Children.Add(new Control()
        {
          HorizontalExpand = true
        });
        ((Control) boxContainer1).Children.Add((Control) button2);
        ((Control) boxContainer1).HorizontalExpand = true;
        BoxContainer boxContainer2 = boxContainer1;
        if (printableComponent.Category == DropshipFabricatorPrintableComponent.CategoryType.Equipment)
          ((Control) this._window.EquipmentContainer).AddChild((Control) boxContainer2);
        else
          ((Control) this._window.AmmoContainer).AddChild((Control) boxContainer2);
      }
    }
  }

  public void Refresh()
  {
    DropshipFabricatorWindow window = this._window;
    DropshipFabricatorComponent fabricatorComponent;
    if (window == null || ((Control) window).Disposed || !this.EntMan.TryGetComponent<DropshipFabricatorComponent>(this.Owner, ref fabricatorComponent))
      return;
    this._window.PointsLabel.Text = Loc.GetString("rmc-dropship-fabricator-points", new (string, object)[1]
    {
      ("points", (object) fabricatorComponent.Points)
    });
  }
}
