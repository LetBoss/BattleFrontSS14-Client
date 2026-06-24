using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared._PUBG.Party;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

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

	public event Action? VoicePressed;

	public PubgPartyHud()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Expected O, but got Unknown
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Expected O, but got Unknown
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Expected O, but got Unknown
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Expected O, but got Unknown
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Expected O, but got Unknown
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Expected O, but got Unknown
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Expected O, but got Unknown
		((PanelContainer)this).PanelOverride = (StyleBox)new StyleBoxFlat
		{
			BackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#111111CC", (Color?)null),
			BorderColor = Color.FromHex((ReadOnlySpan<char>)"#444444", (Color?)null),
			BorderThickness = new Thickness(1f)
		};
		_title = new Label
		{
			Text = Loc.GetString("pubg-party-hud-title"),
			FontColorOverride = Color.White
		};
		_rows = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 6
		};
		_compactInfo = new Label
		{
			FontColorOverride = Color.White,
			Visible = false
		};
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			SeparationOverride = 6
		};
		_voiceButton = new Button
		{
			Text = Loc.GetString("pubg-party-button-voice")
		};
		((BaseButton)_voiceButton).OnPressed += delegate
		{
			this.VoicePressed?.Invoke();
		};
		((Control)val).AddChild((Control)(object)_voiceButton);
		_root = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 6,
			Margin = new Thickness(8f)
		};
		((Control)_root).AddChild((Control)(object)_title);
		((Control)_root).AddChild((Control)(object)_rows);
		((Control)_root).AddChild((Control)(object)_compactInfo);
		((Control)_root).AddChild((Control)(object)val);
		((Control)this).AddChild((Control)(object)_root);
	}

	public void UpdateMembers(IReadOnlyList<PubgPartyMemberState> members, bool compactMode, string? teamTag)
	{
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		((Control)_rows).Visible = !compactMode;
		((Control)_compactInfo).Visible = compactMode;
		if (compactMode)
		{
			ClearRows();
			_title.Text = ((teamTag != null) ? Loc.GetString("pubg-party-hud-title-team", new(string, object)[1] { ("team", teamTag) }) : Loc.GetString("pubg-party-hud-title"));
			_compactInfo.Text = ((teamTag != null) ? Loc.GetString("pubg-party-hud-members-team-count", new(string, object)[2]
			{
				("team", teamTag),
				("count", members.Count)
			}) : Loc.GetString("pubg-party-hud-members-count", new(string, object)[1] { ("count", members.Count) }));
			_voiceButton.Text = Loc.GetString("pubg-party-button-voice-count", new(string, object)[1] { ("count", members.Count) });
		}
		else
		{
			_title.Text = Loc.GetString("pubg-party-hud-title");
			_voiceButton.Text = Loc.GetString("pubg-party-button-voice");
		}
		HashSet<NetEntity> alive = new HashSet<NetEntity>();
		if (!compactMode)
		{
			foreach (PubgPartyMemberState member in members)
			{
				alive.Add(member.Entity);
				if (!_rowByEntity.TryGetValue(member.Entity, out PubgPartyHudRow value))
				{
					value = new PubgPartyHudRow();
					_rowByEntity.Add(member.Entity, value);
					((Control)_rows).AddChild((Control)(object)value);
				}
				value.UpdateRow(member);
			}
			foreach (NetEntity item in _rowByEntity.Keys.Where((NetEntity key) => !alive.Contains(key)).ToList())
			{
				((Control)_rowByEntity[item]).Orphan();
				_rowByEntity.Remove(item);
			}
		}
		_hasParty = members.Count > 1;
		ApplyButtonState();
	}

	private void ApplyButtonState()
	{
		((BaseButton)_voiceButton).Disabled = !_hasParty;
	}

	private void ClearRows()
	{
		foreach (PubgPartyHudRow value in _rowByEntity.Values)
		{
			((Control)value).Orphan();
		}
		_rowByEntity.Clear();
	}
}
