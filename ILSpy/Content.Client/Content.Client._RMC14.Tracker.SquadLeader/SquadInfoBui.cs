using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Marines.Squads;
using Content.Shared._RMC14.Tracker.SquadLeader;
using Content.Shared.Roles;
using Content.Shared.StatusIcon;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client._RMC14.Tracker.SquadLeader;

public sealed class SquadInfoBui : BoundUserInterface
{
	[Dependency]
	private IPrototypeManager _prototype;

	private SquadInfoWindow? _window;

	private readonly SpriteSystem _sprite;

	public SquadInfoBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<SquadInfoBui>(this);
		_sprite = base.EntMan.System<SpriteSystem>();
	}

	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<SquadInfoWindow>((BoundUserInterface)(object)this);
		Refresh();
	}

	public void Refresh()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_050a: Unknown result type (might be due to invalid IL or missing references)
		//IL_050f: Unknown result type (might be due to invalid IL or missing references)
		//IL_051f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0524: Unknown result type (might be due to invalid IL or missing references)
		//IL_0539: Unknown result type (might be due to invalid IL or missing references)
		//IL_0567: Unknown result type (might be due to invalid IL or missing references)
		//IL_056c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0577: Unknown result type (might be due to invalid IL or missing references)
		//IL_0582: Unknown result type (might be due to invalid IL or missing references)
		//IL_0589: Unknown result type (might be due to invalid IL or missing references)
		//IL_0599: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bc: Expected O, but got Unknown
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Expected O, but got Unknown
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Expected O, but got Unknown
		SquadInfoWindow window = _window;
		SquadLeaderTrackerComponent squadLeaderTrackerComponent = default(SquadLeaderTrackerComponent);
		if (window == null || !((BaseWindow)window).IsOpen || !base.EntMan.TryGetComponent<SquadLeaderTrackerComponent>(((BoundUserInterface)this).Owner, ref squadLeaderTrackerComponent))
		{
			return;
		}
		Texture background = null;
		Color backgroundColor = default(Color);
		SquadMemberComponent squadMemberComponent = default(SquadMemberComponent);
		if (base.EntMan.TryGetComponent<SquadMemberComponent>(((BoundUserInterface)this).Owner, ref squadMemberComponent))
		{
			background = _sprite.Frame0(squadMemberComponent.Background);
			backgroundColor = squadMemberComponent.BackgroundColor;
		}
		bool flag = base.EntMan.HasComponent<SquadLeaderComponent>(((BoundUserInterface)this).Owner);
		string text = ((squadLeaderTrackerComponent.Fireteams.SquadLeader == null) ? Loc.GetString("rmc-squad-info-squad-leader-none") : Loc.GetString("rmc-squad-info-squad-leader-name", new(string, object)[1] { ("leader", squadLeaderTrackerComponent.Fireteams.SquadLeader) }));
		_window.SquadLeaderLabel.Text = text;
		((BaseButton)_window.ChangeTrackerButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new SquadLeaderTrackerChangeTrackedMsg());
		};
		((Control)_window.FireteamsContainer).DisposeAllChildren();
		SquadLeaderTrackerMarine value;
		NetEntity key;
		for (int num = 0; num < squadLeaderTrackerComponent.Fireteams.Fireteams.Length; num++)
		{
			SquadLeaderTrackerFireteam squadLeaderTrackerFireteam = squadLeaderTrackerComponent.Fireteams.Fireteams[num];
			Dictionary<NetEntity, SquadLeaderTrackerMarine> dictionary = squadLeaderTrackerFireteam?.Members;
			if (dictionary == null || dictionary.Count <= 0)
			{
				continue;
			}
			SquadFireteamContainer squadFireteamContainer = new SquadFireteamContainer();
			string text2;
			if (squadLeaderTrackerFireteam.Leader.HasValue)
			{
				(string, object)[] array = new(string, object)[1];
				value = squadLeaderTrackerFireteam.Leader.Value;
				array[0] = ("leader", value.Name);
				text2 = Loc.GetString("rmc-squad-info-team-leader-name", array);
			}
			else
			{
				text2 = Loc.GetString("rmc-squad-info-team-leader-none");
			}
			string text3 = text2;
			squadFireteamContainer.LeaderLabel.Text = text3;
			int fireatemIndex = num;
			((BaseButton)squadFireteamContainer.RemoveLeaderButton).OnPressed += delegate
			{
				((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new SquadLeaderTrackerDemoteFireteamLeaderMsg(fireatemIndex));
			};
			((Control)squadFireteamContainer.RemoveLeaderButton).Visible = squadLeaderTrackerFireteam.Leader.HasValue && flag;
			squadFireteamContainer.FireteamLabel.Text = Loc.GetString("rmc-squad-info-fireteam", new(string, object)[1] { ("fireteam", num + 1) });
			foreach (KeyValuePair<NetEntity, SquadLeaderTrackerMarine> member2 in squadLeaderTrackerFireteam.Members)
			{
				member2.Deconstruct(out key, out value);
				SquadLeaderTrackerMarine member = value;
				key = member.Id;
				ref SquadLeaderTrackerMarine? leader = ref squadLeaderTrackerFireteam.Leader;
				NetEntity? val;
				if (!leader.HasValue)
				{
					val = null;
				}
				else
				{
					value = leader.GetValueOrDefault();
					val = value.Id;
				}
				NetEntity? val2 = val;
				if (!val2.HasValue || !(key == val2.GetValueOrDefault()))
				{
					SquadInfoRow squadInfoRow = CreateRow(member, background, backgroundColor);
					((Control)squadFireteamContainer.MembersContainer).AddChild((Control)(object)squadInfoRow);
					Button val3 = new Button
					{
						MaxWidth = 25f,
						MaxHeight = 25f,
						VerticalAlignment = (VAlignment)2,
						StyleClasses = { "OpenBoth" },
						Text = "^",
						TextAlign = (AlignMode)1,
						ToolTip = Loc.GetString("rmc-squad-info-promote-team-leader"),
						Margin = new Thickness(0f, 0f, 2f, 0f)
					};
					((Control)val3).Visible = flag;
					((BaseButton)val3).OnPressed += delegate
					{
						//IL_0011: Unknown result type (might be due to invalid IL or missing references)
						((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new SquadLeaderTrackerPromoteFireteamLeaderMsg(member.Id));
					};
					((Control)squadInfoRow.ActionsContainer).AddChild((Control)(object)val3);
					Button val4 = new Button
					{
						MaxWidth = 25f,
						MaxHeight = 25f,
						VerticalAlignment = (VAlignment)2,
						StyleClasses = { "OpenBoth" },
						Text = "x",
						TextAlign = (AlignMode)1,
						ToolTip = Loc.GetString("rmc-squad-info-unassign-fireteam"),
						Margin = new Thickness(0f, 0f, 2f, 0f)
					};
					((Control)val4).Visible = flag;
					((BaseButton)val4).OnPressed += delegate
					{
						//IL_0011: Unknown result type (might be due to invalid IL or missing references)
						((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new SquadLeaderTrackerUnassignFireteamMsg(member.Id));
					};
					((Control)squadInfoRow.ActionsContainer).AddChild((Control)(object)val4);
				}
			}
			((Control)_window.FireteamsContainer).AddChild((Control)(object)squadFireteamContainer);
		}
		SquadFireteamContainer squadFireteamContainer2 = new SquadFireteamContainer();
		((Control)squadFireteamContainer2.LeaderContainer).Visible = false;
		((Control)squadFireteamContainer2.RemoveLeaderButton).Visible = false;
		squadFireteamContainer2.FireteamLabel.Text = Loc.GetString("rmc-squad-info-unassigned");
		squadFireteamContainer2.ActionsLabel.Text = Loc.GetString("rmc-squad-info-actions");
		foreach (KeyValuePair<NetEntity, SquadLeaderTrackerMarine> item in squadLeaderTrackerComponent.Fireteams.Unassigned)
		{
			item.Deconstruct(out key, out value);
			SquadLeaderTrackerMarine unassigned = value;
			NetEntity? val2 = squadLeaderTrackerComponent.Fireteams.SquadLeaderId;
			key = unassigned.Id;
			if (val2.HasValue && val2.GetValueOrDefault() == key)
			{
				continue;
			}
			SquadInfoRow squadInfoRow2 = CreateRow(unassigned, background, backgroundColor);
			((Control)squadFireteamContainer2.MembersContainer).AddChild((Control)(object)squadInfoRow2);
			for (int num2 = 0; num2 < squadLeaderTrackerComponent.Fireteams.Fireteams.Length; num2++)
			{
				Button val5 = new Button
				{
					MaxWidth = 25f,
					MaxHeight = 25f,
					VerticalAlignment = (VAlignment)1,
					StyleClasses = { "OpenBoth" },
					Text = $"{num2 + 1}"
				};
				int fireteamIndex = num2;
				((Control)val5).Visible = flag;
				((BaseButton)val5).OnPressed += delegate
				{
					//IL_0016: Unknown result type (might be due to invalid IL or missing references)
					((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new SquadLeaderTrackerAssignFireteamMsg(unassigned.Id, fireteamIndex));
				};
				((Control)squadInfoRow2.ActionsContainer).AddChild((Control)(object)val5);
			}
		}
		((Control)_window.FireteamsContainer).AddChild((Control)(object)squadFireteamContainer2);
	}

	private SquadInfoRow CreateRow(SquadLeaderTrackerMarine member, Texture? background, Color backgroundColor)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		SquadInfoRow squadInfoRow = new SquadInfoRow();
		ProtoId<JobPrototype>? role = member.Role;
		if (role.HasValue)
		{
			ProtoId<JobPrototype> valueOrDefault = role.GetValueOrDefault();
			JobPrototype jobPrototype = default(JobPrototype);
			if (_prototype.TryIndex<JobPrototype>(valueOrDefault, ref jobPrototype))
			{
				JobIconPrototype jobIconPrototype = default(JobIconPrototype);
				if (member.IconOverride != null)
				{
					squadInfoRow.RoleIcon.Texture = _sprite.Frame0((SpriteSpecifier)(object)member.IconOverride);
				}
				else if (_prototype.TryIndex<JobIconPrototype>(jobPrototype.Icon, ref jobIconPrototype))
				{
					squadInfoRow.RoleIcon.Texture = _sprite.Frame0(jobIconPrototype.Icon);
				}
				squadInfoRow.RoleBackground.Texture = background;
				((Control)squadInfoRow.RoleBackground).ModulateSelfOverride = backgroundColor;
			}
		}
		squadInfoRow.NameLabel.Text = "[bold]" + FormattedMessage.EscapeText(member.Name) + "[/bold]";
		return squadInfoRow;
	}
}
