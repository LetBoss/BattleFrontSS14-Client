// Decompiled with JetBrains decompiler
// Type: Content.Client.Tools.UI.WelderStatusControl
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Items.UI;
using Content.Client.Message;
using Content.Shared.FixedPoint;
using Content.Shared.Tools.Components;
using Content.Shared.Tools.Systems;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;

#nullable enable
namespace Content.Client.Tools.UI;

public sealed class WelderStatusControl : PollingItemStatusControl<WelderStatusControl.Data>
{
  private readonly Entity<WelderComponent> _parent;
  private readonly IEntityManager _entityManager;
  private readonly SharedToolSystem _toolSystem;
  private readonly RichTextLabel _label;

  public WelderStatusControl(
    Entity<WelderComponent> parent,
    IEntityManager entityManager,
    SharedToolSystem toolSystem)
  {
    this._parent = parent;
    this._entityManager = entityManager;
    this._toolSystem = toolSystem;
    RichTextLabel richTextLabel = new RichTextLabel();
    ((Control) richTextLabel).StyleClasses.Add("ItemStatus");
    this._label = richTextLabel;
    this.AddChild((Control) this._label);
    this.UpdateDraw();
  }

  protected override WelderStatusControl.Data PollData()
  {
    (FixedPoint2 fuel, FixedPoint2 capacity) welderFuelAndCapacity = this._toolSystem.GetWelderFuelAndCapacity(Entity<WelderComponent>.op_Implicit(this._parent), this._parent.Comp);
    return new WelderStatusControl.Data(welderFuelAndCapacity.fuel, welderFuelAndCapacity.capacity, this._parent.Comp.Enabled);
  }

  protected override void Update(in WelderStatusControl.Data data)
  {
    this._label.SetMarkup(Loc.GetString("welder-component-on-examine-detailed-message", new (string, object)[4]
    {
      ("colorName", data.Fuel < data.FuelCapacity / 4f ? (object) "darkorange" : (object) "orange"),
      ("fuelLeft", (object) data.Fuel),
      ("fuelCapacity", (object) data.FuelCapacity),
      ("status", (object) Loc.GetString(data.Lit ? "welder-component-on-examine-welder-lit-message" : "welder-component-on-examine-welder-not-lit-message"))
    }));
  }

  public record struct Data(FixedPoint2 Fuel, FixedPoint2 FuelCapacity, bool Lit);
}
