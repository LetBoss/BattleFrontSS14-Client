// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.Systems.Party.PubgPartyHud
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG.Party;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Client._PUBG.UserInterface.Systems.Party;

public sealed class PubgPartyHud : PanelContainer
{
  private readonly BoxContainer _root;
  private readonly Label _title;
  private readonly BoxContainer _rows;
  private readonly Label _compactInfo;
  private readonly Button _voiceButton;
  private readonly Dictionary<NetEntity, PubgPartyHudRow> _rowByEntity = new Dictionary<NetEntity, PubgPartyHudRow>();
  private bool _hasParty;

  public PubgPartyHud()
  {
    this.PanelOverride = (StyleBox) new StyleBoxFlat()
    {
      BackgroundColor = Color.FromHex((ReadOnlySpan<char>) "#111111CC", new Color?()),
      BorderColor = Color.FromHex((ReadOnlySpan<char>) "#444444", new Color?()),
      BorderThickness = new Thickness(1f)
    };
    this._title = new Label()
    {
      Text = Loc.GetString("pubg-party-hud-title"),
      FontColorOverride = new Color?(Color.White)
    };
    this._rows = new BoxContainer()
    {
      Orientation = (BoxContainer.LayoutOrientation) 1,
      SeparationOverride = new int?(6)
    };
    Label label = new Label();
    label.FontColorOverride = new Color?(Color.White);
    ((Control) label).Visible = false;
    this._compactInfo = label;
    BoxContainer boxContainer1 = new BoxContainer()
    {
      Orientation = (BoxContainer.LayoutOrientation) 0,
      SeparationOverride = new int?(6)
    };
    this._voiceButton = new Button()
    {
      Text = Loc.GetString("pubg-party-button-voice")
    };
    ((BaseButton) this._voiceButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      Action voicePressed = this.VoicePressed;
      if (voicePressed == null)
        return;
      voicePressed();
    });
    ((Control) boxContainer1).AddChild((Control) this._voiceButton);
    BoxContainer boxContainer2 = new BoxContainer();
    boxContainer2.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer2.SeparationOverride = new int?(6);
    ((Control) boxContainer2).Margin = new Thickness(8f);
    this._root = boxContainer2;
    ((Control) this._root).AddChild((Control) this._title);
    ((Control) this._root).AddChild((Control) this._rows);
    ((Control) this._root).AddChild((Control) this._compactInfo);
    ((Control) this._root).AddChild((Control) boxContainer1);
    ((Control) this).AddChild((Control) this._root);
  }

  public event Action? VoicePressed;

  public void UpdateMembers(
    IReadOnlyList<PubgPartyMemberState> members,
    bool compactMode,
    string? teamTag)
  {
    ((Control) this._rows).Visible = !compactMode;
    ((Control) this._compactInfo).Visible = compactMode;
    if (compactMode)
    {
      this.ClearRows();
      Label title = this._title;
      string str1;
      if (teamTag == null)
        str1 = Loc.GetString("pubg-party-hud-title");
      else
        str1 = Loc.GetString("pubg-party-hud-title-team", new (string, object)[1]
        {
          ("team", (object) teamTag)
        });
      title.Text = str1;
      Label compactInfo = this._compactInfo;
      string str2;
      if (teamTag == null)
        str2 = Loc.GetString("pubg-party-hud-members-count", new (string, object)[1]
        {
          ("count", (object) members.Count)
        });
      else
        str2 = Loc.GetString("pubg-party-hud-members-team-count", new (string, object)[2]
        {
          ("team", (object) teamTag),
          ("count", (object) members.Count)
        });
      compactInfo.Text = str2;
      this._voiceButton.Text = Loc.GetString("pubg-party-button-voice-count", new (string, object)[1]
      {
        ("count", (object) members.Count)
      });
    }
    else
    {
      this._title.Text = Loc.GetString("pubg-party-hud-title");
      this._voiceButton.Text = Loc.GetString("pubg-party-button-voice");
    }
    HashSet<NetEntity> alive = new HashSet<NetEntity>();
    if (!compactMode)
    {
      foreach (PubgPartyMemberState member in (IEnumerable<PubgPartyMemberState>) members)
      {
        alive.Add(member.Entity);
        PubgPartyHudRow pubgPartyHudRow;
        if (!this._rowByEntity.TryGetValue(member.Entity, out pubgPartyHudRow))
        {
          pubgPartyHudRow = new PubgPartyHudRow();
          this._rowByEntity.Add(member.Entity, pubgPartyHudRow);
          ((Control) this._rows).AddChild((Control) pubgPartyHudRow);
        }
        pubgPartyHudRow.UpdateRow(member);
      }
      foreach (NetEntity key in this._rowByEntity.Keys.Where<NetEntity>((Func<NetEntity, bool>) (key => !alive.Contains(key))).ToList<NetEntity>())
      {
        ((Control) this._rowByEntity[key]).Orphan();
        this._rowByEntity.Remove(key);
      }
    }
    this._hasParty = members.Count > 1;
    this.ApplyButtonState();
  }

  private void ApplyButtonState() => ((BaseButton) this._voiceButton).Disabled = !this._hasParty;

  private void ClearRows()
  {
    foreach (Control control in this._rowByEntity.Values)
      control.Orphan();
    this._rowByEntity.Clear();
  }
}
