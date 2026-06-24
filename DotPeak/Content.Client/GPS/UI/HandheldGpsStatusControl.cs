// Decompiled with JetBrains decompiler
// Type: Content.Client.GPS.UI.HandheldGpsStatusControl
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Message;
using Content.Shared.GPS.Components;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Timing;

#nullable enable
namespace Content.Client.GPS.UI;

public sealed class HandheldGpsStatusControl : Control
{
  private readonly Entity<HandheldGPSComponent> _parent;
  private readonly RichTextLabel _label;
  private float _updateDif;
  private readonly IEntityManager _entMan;
  private readonly SharedTransformSystem _transform;

  public HandheldGpsStatusControl(Entity<HandheldGPSComponent> parent)
  {
    this._parent = parent;
    this._entMan = IoCManager.Resolve<IEntityManager>();
    this._transform = (SharedTransformSystem) this._entMan.System<TransformSystem>();
    RichTextLabel richTextLabel = new RichTextLabel();
    ((Control) richTextLabel).StyleClasses.Add("ItemStatus");
    this._label = richTextLabel;
    this.AddChild((Control) this._label);
    this.UpdateGpsDetails();
  }

  protected virtual void FrameUpdate(FrameEventArgs args)
  {
    base.FrameUpdate(args);
    if (this._parent.Comp.LifeStage > 6)
    {
      ((Control) this._label).Visible = false;
    }
    else
    {
      this._updateDif += ((FrameEventArgs) ref args).DeltaSeconds;
      if ((double) this._updateDif < (double) this._parent.Comp.UpdateRate)
        return;
      this._updateDif -= this._parent.Comp.UpdateRate;
      this.UpdateGpsDetails();
    }
  }

  private void UpdateGpsDetails()
  {
    string str = "Error";
    TransformComponent transformComponent;
    if (this._entMan.TryGetComponent<TransformComponent>(Entity<HandheldGPSComponent>.op_Implicit(this._parent), ref transformComponent))
    {
      MapCoordinates mapCoordinates = this._transform.GetMapCoordinates(this._parent.Owner, transformComponent);
      str = $"({(int) ((MapCoordinates) ref mapCoordinates).X}, {(int) ((MapCoordinates) ref mapCoordinates).Y})";
    }
    this._label.SetMarkup(Loc.GetString("handheld-gps-coordinates-title", new (string, object)[1]
    {
      ("coordinates", (object) str)
    }));
  }
}
