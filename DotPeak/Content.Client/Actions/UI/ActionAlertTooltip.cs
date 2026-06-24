// Decompiled with JetBrains decompiler
// Type: Content.Client.Actions.UI.ActionAlertTooltip
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Client.Actions.UI;

public sealed class ActionAlertTooltip : PanelContainer
{
  private const float TooltipTextMaxWidth = 350f;
  private readonly RichTextLabel _cooldownLabel;
  private readonly RichTextLabel _dynamicMessageLabel;
  private readonly IGameTiming _gameTiming;

  public (TimeSpan Start, TimeSpan End)? Cooldown { get; set; }

  public string? DynamicMessage { get; set; }

  public ActionAlertTooltip(
    FormattedMessage name,
    FormattedMessage? desc,
    string? requires = null,
    FormattedMessage? charges = null)
  {
    this._gameTiming = IoCManager.Resolve<IGameTiming>();
    ((Control) this).SetOnlyStyleClass("tooltipBox");
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer1).RectClipContent = true;
    BoxContainer boxContainer2 = boxContainer1;
    ((Control) this).AddChild((Control) boxContainer1);
    RichTextLabel richTextLabel1 = new RichTextLabel();
    ((Control) richTextLabel1).MaxWidth = 350f;
    ((Control) richTextLabel1).StyleClasses.Add("tooltipActionTitle");
    RichTextLabel richTextLabel2 = richTextLabel1;
    richTextLabel2.SetMessage(name, new Color?());
    ((Control) boxContainer2).AddChild((Control) richTextLabel2);
    if (desc != null && !string.IsNullOrWhiteSpace(desc.ToString()))
    {
      RichTextLabel richTextLabel3 = new RichTextLabel();
      ((Control) richTextLabel3).MaxWidth = 350f;
      ((Control) richTextLabel3).StyleClasses.Add("tooltipActionDesc");
      RichTextLabel richTextLabel4 = richTextLabel3;
      richTextLabel4.SetMessage(desc, new Color?());
      ((Control) boxContainer2).AddChild((Control) richTextLabel4);
    }
    if (charges != null && !string.IsNullOrWhiteSpace(charges.ToString()))
    {
      RichTextLabel richTextLabel5 = new RichTextLabel();
      ((Control) richTextLabel5).MaxWidth = 350f;
      ((Control) richTextLabel5).StyleClasses.Add("tooltipActionCharges");
      RichTextLabel richTextLabel6 = richTextLabel5;
      richTextLabel6.SetMessage(charges, new Color?());
      ((Control) boxContainer2).AddChild((Control) richTextLabel6);
    }
    BoxContainer boxContainer3 = boxContainer2;
    RichTextLabel richTextLabel7 = new RichTextLabel();
    ((Control) richTextLabel7).MaxWidth = 350f;
    ((Control) richTextLabel7).StyleClasses.Add("tooltipActionCooldown");
    ((Control) richTextLabel7).Visible = false;
    RichTextLabel richTextLabel8 = richTextLabel7;
    this._cooldownLabel = richTextLabel7;
    RichTextLabel richTextLabel9 = richTextLabel8;
    ((Control) boxContainer3).AddChild((Control) richTextLabel9);
    BoxContainer boxContainer4 = boxContainer2;
    RichTextLabel richTextLabel10 = new RichTextLabel();
    ((Control) richTextLabel10).MaxWidth = 350f;
    ((Control) richTextLabel10).StyleClasses.Add("tooltipActionDynamicMessage");
    ((Control) richTextLabel10).Visible = false;
    RichTextLabel richTextLabel11 = richTextLabel10;
    this._dynamicMessageLabel = richTextLabel10;
    RichTextLabel richTextLabel12 = richTextLabel11;
    ((Control) boxContainer4).AddChild((Control) richTextLabel12);
    if (string.IsNullOrWhiteSpace(requires))
      return;
    RichTextLabel richTextLabel13 = new RichTextLabel();
    ((Control) richTextLabel13).MaxWidth = 350f;
    ((Control) richTextLabel13).StyleClasses.Add("tooltipActionCooldown");
    RichTextLabel richTextLabel14 = richTextLabel13;
    FormattedMessage formattedMessage;
    if (!FormattedMessage.TryFromMarkup($"[color=#635c5c]{requires}[/color]", ref formattedMessage))
      return;
    richTextLabel14.SetMessage(formattedMessage, new Color?());
    ((Control) boxContainer2).AddChild((Control) richTextLabel14);
  }

  protected virtual void FrameUpdate(FrameEventArgs args)
  {
    ((Control) this).FrameUpdate(args);
    if (!this.Cooldown.HasValue)
    {
      ((Control) this._cooldownLabel).Visible = false;
    }
    else
    {
      TimeSpan timeSpan = this.Cooldown.Value.End - this._gameTiming.CurTime;
      if (timeSpan > TimeSpan.Zero)
      {
        FormattedMessage formattedMessage;
        if (FormattedMessage.TryFromMarkup(Loc.GetString("ui-actionslot-duration", new (string, object)[2]
        {
          ("duration", (object) (int) (this.Cooldown.Value.End - this.Cooldown.Value.Start).TotalSeconds),
          ("timeLeft", (object) ((int) timeSpan.TotalSeconds + 1))
        }), ref formattedMessage))
        {
          this._cooldownLabel.SetMessage(formattedMessage, new Color?());
          ((Control) this._cooldownLabel).Visible = true;
        }
      }
      else
        ((Control) this._cooldownLabel).Visible = false;
    }
    if (string.IsNullOrWhiteSpace(this.DynamicMessage))
    {
      ((Control) this._dynamicMessageLabel).Visible = false;
    }
    else
    {
      FormattedMessage formattedMessage;
      if (!FormattedMessage.TryFromMarkup($"[color=#ffffff]{this.DynamicMessage}[/color]", ref formattedMessage))
        return;
      this._dynamicMessageLabel.SetMessage(formattedMessage, new Color?());
      ((Control) this._dynamicMessageLabel).Visible = true;
    }
  }
}
