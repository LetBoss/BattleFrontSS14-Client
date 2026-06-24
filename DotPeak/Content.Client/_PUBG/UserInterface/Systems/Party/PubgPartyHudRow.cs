// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.Systems.Party.PubgPartyHudRow
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG.Party;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Client._PUBG.UserInterface.Systems.Party;

public sealed class PubgPartyHudRow : BoxContainer
{
  private readonly Label _nameLabel;
  private readonly ProgressBar _hpBar;
  private readonly Label _voiceLabel;

  public PubgPartyHudRow()
  {
    this.Orientation = (BoxContainer.LayoutOrientation) 1;
    this.SeparationOverride = new int?(2);
    this._nameLabel = new Label()
    {
      FontColorOverride = new Color?(Color.White)
    };
    this._voiceLabel = new Label()
    {
      FontColorOverride = new Color?(Color.Gray)
    };
    ProgressBar progressBar = new ProgressBar();
    ((Range) progressBar).MinValue = 0.0f;
    ((Range) progressBar).MaxValue = 1f;
    ((Range) progressBar).Value = 1f;
    ((Control) progressBar).SetHeight = 8f;
    ((Control) progressBar).HorizontalExpand = true;
    this._hpBar = progressBar;
    ((Control) this).AddChild((Control) this._nameLabel);
    ((Control) this).AddChild((Control) this._voiceLabel);
    ((Control) this).AddChild((Control) this._hpBar);
  }

  public void UpdateRow(PubgPartyMemberState member)
  {
    this._nameLabel.Text = Loc.GetString("pubg-party-member-ckey-with-level", new (string, object)[2]
    {
      ("ckey", (object) member.Username),
      ("level", (object) member.Level)
    });
    ((Range) this._hpBar).Value = Math.Clamp(member.HpPercent, 0.0f, 1f);
    ((Control) this._hpBar).Visible = !member.IsDead;
    this._voiceLabel.Text = Loc.GetString(member.IsInVoice ? "pubg-party-voice-on" : "pubg-party-voice-off");
    this._nameLabel.FontColorOverride = new Color?(member.IsDead ? Color.Gray : PubgPartyHudRow.GetPartyColor(member.SlotIndex));
  }

  private static Color GetPartyColor(int slotIndex)
  {
    Color partyColor;
    switch (slotIndex)
    {
      case 1:
        partyColor = Color.FromHex((ReadOnlySpan<char>) "#00bcd4", new Color?());
        break;
      case 2:
        partyColor = Color.FromHex((ReadOnlySpan<char>) "#ffeb3b", new Color?());
        break;
      case 3:
        partyColor = Color.FromHex((ReadOnlySpan<char>) "#ff9800", new Color?());
        break;
      default:
        partyColor = Color.FromHex((ReadOnlySpan<char>) "#4caf50", new Color?());
        break;
    }
    return partyColor;
  }
}
