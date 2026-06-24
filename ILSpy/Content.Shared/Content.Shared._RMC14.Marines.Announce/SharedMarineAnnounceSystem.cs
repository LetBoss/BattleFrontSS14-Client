using System;
using Content.Shared._RMC14.Dialog;
using Content.Shared._RMC14.Marines.ControlComputer;
using Content.Shared._RMC14.Marines.Roles.Ranks;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Marines.Squads;
using Content.Shared._RMC14.Overwatch;
using Content.Shared._RMC14.TacticalMap;
using Content.Shared.Administration.Logs;
using Content.Shared.CCVar;
using Content.Shared.Database;
using Content.Shared.Popups;
using Content.Shared.Radio;
using Robust.Shared.Audio;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Marines.Announce;

public abstract class SharedMarineAnnounceSystem : EntitySystem
{
	[Dependency]
	private ISharedAdminLogManager _adminLog;

	[Dependency]
	private IConfigurationManager _config;

	[Dependency]
	private DialogSystem _dialog;

	[Dependency]
	private SharedMarineControlComputerSystem _marineControlComputer;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedRankSystem _rankSystem;

	[Dependency]
	private SkillsSystem _skills;

	[Dependency]
	private SquadSystem _squad;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedUserInterfaceSystem _ui;

	public static readonly SoundSpecifier DefaultAnnouncementSound = (SoundSpecifier)new SoundPathSpecifier("/Audio/_RMC14/Announcements/Marine/notice2.ogg", (AudioParams?)null);

	public static readonly SoundSpecifier DefaultSquadSound = (SoundSpecifier)new SoundPathSpecifier("/Audio/_RMC14/Effects/tech_notification.ogg", (AudioParams?)null);

	public static readonly SoundSpecifier AresAnnouncementSound = (SoundSpecifier)new SoundPathSpecifier("/Audio/_RMC14/AI/announce.ogg", (AudioParams?)null);

	public int CharacterLimit = 1000;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<MarineCommunicationsComputerComponent, EchoSquadReasonEvent>((EntityEventRefHandler<MarineCommunicationsComputerComponent, EchoSquadReasonEvent>)OnEchoSquadReason, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MarineCommunicationsComputerComponent, EchoSquadConfirmEvent>((EntityEventRefHandler<MarineCommunicationsComputerComponent, EchoSquadConfirmEvent>)OnEchoSquadConfirm, (Type[])null, (Type[])null);
		BoundUserInterfaceRegisterExt.BuiEvents<MarineCommunicationsComputerComponent>(((EntitySystem)this).Subs, (object)MarineCommunicationsComputerUI.Key, (BuiEventSubscriber<MarineCommunicationsComputerComponent>)delegate(Subscriber<MarineCommunicationsComputerComponent> subs)
		{
			subs.Event<MarineCommunicationsComputerMsg>((EntityEventRefHandler<MarineCommunicationsComputerComponent, MarineCommunicationsComputerMsg>)OnMarineCommunicationsComputerMsg);
			subs.Event<MarineCommunicationsOpenMapMsg>((EntityEventRefHandler<MarineCommunicationsComputerComponent, MarineCommunicationsOpenMapMsg>)OnMarineCommunicationsOpenMapMsg);
			subs.Event<MarineCommunicationsEchoSquadMsg>((EntityEventRefHandler<MarineCommunicationsComputerComponent, MarineCommunicationsEchoSquadMsg>)OnMarineCommunicationsEchoMsg);
			subs.Event<MarineCommunicationsOverwatchMsg>((EntityEventRefHandler<MarineCommunicationsComputerComponent, MarineCommunicationsOverwatchMsg>)OnMarineCommunicationsOverwatchMsg);
			subs.Event<MarineControlComputerMedalMsg>((EntityEventRefHandler<MarineCommunicationsComputerComponent, MarineControlComputerMedalMsg>)OnMarineCommunicationsMedalMsg);
		});
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _config, CCVars.ChatMaxMessageLength, (Action<int>)delegate(int limit)
		{
			CharacterLimit = limit;
		}, true);
	}

	private void OnEchoSquadReason(Entity<MarineCommunicationsComputerComponent> ent, ref EchoSquadReasonEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? user = default(EntityUid?);
		if (ent.Comp.CanCreateEcho && ((EntitySystem)this).TryGetEntity(args.User, ref user))
		{
			EchoSquadConfirmEvent ev = new EchoSquadConfirmEvent(args.User, args.Message);
			_dialog.OpenConfirmation(Entity<MarineCommunicationsComputerComponent>.op_Implicit(ent), user.Value, "Confirm Activation", "Confirm activation of Echo Squad for " + args.Message, ev);
		}
	}

	private void OnEchoSquadConfirm(Entity<MarineCommunicationsComputerComponent> ent, ref EchoSquadConfirmEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? user = default(EntityUid?);
		if (ent.Comp.CanCreateEcho && ((EntitySystem)this).TryGetEntity(args.User, ref user))
		{
			ent.Comp.CanCreateEcho = false;
			((EntitySystem)this).Dirty<MarineCommunicationsComputerComponent>(ent, (MetaDataComponent)null);
			if (!_squad.HasSquad(EntProtoId<SquadTeamComponent>.op_Implicit(SquadSystem.EchoSquadId)))
			{
				_squad.TryEnsureSquad(EntProtoId<SquadTeamComponent>.op_Implicit(SquadSystem.EchoSquadId), out Entity<SquadTeamComponent> _);
				ISharedAdminLogManager adminLog = _adminLog;
				LogStringHandler handler = new LogStringHandler(39, 2);
				handler.AppendLiteral("Echo squad was created by ");
				handler.AppendFormatted(((EntitySystem)this).ToPrettyString(user, (MetaDataComponent)null), "ToPrettyString(user)");
				handler.AppendLiteral(" with reason ");
				handler.AppendFormatted(args.Message);
				adminLog.Add(LogType.RMCSquadCreated, ref handler);
			}
		}
	}

	private void OnMarineCommunicationsComputerMsg(Entity<MarineCommunicationsComputerComponent> ent, ref MarineCommunicationsComputerMsg args)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		if (string.IsNullOrWhiteSpace(args.Text))
		{
			return;
		}
		if (!_skills.HasSkill(Entity<SkillsComponent>.op_Implicit(((BaseBoundUserInterfaceEvent)args).Actor), ent.Comp.AnnounceSkill, ent.Comp.AnnounceSkillLevel))
		{
			_popup.PopupClient(base.Loc.GetString("rmc-skills-no-training", (ValueTuple<string, object>)("target", ent)), ((BaseBoundUserInterfaceEvent)args).Actor, PopupType.MediumCaution);
			return;
		}
		TimeSpan time = _timing.CurTime;
		TimeSpan curTime = _timing.CurTime;
		TimeSpan? timeSpan = ent.Comp.LastAnnouncement + ent.Comp.Cooldown;
		if (curTime < timeSpan)
		{
			string cooldownMessage = base.Loc.GetString("rmc-announcement-cooldown", (ValueTuple<string, object>)("seconds", (int)ent.Comp.Cooldown.TotalSeconds));
			_popup.PopupClient(cooldownMessage, ((BaseBoundUserInterfaceEvent)args).Actor, PopupType.SmallCaution);
			return;
		}
		_ui.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)MarineCommunicationsComputerUI.Key);
		string text = args.Text;
		if (text.Length > CharacterLimit)
		{
			text = text.Substring(0, CharacterLimit).Trim();
		}
		AnnounceSigned(((BaseBoundUserInterfaceEvent)args).Actor, text, null, ent.Comp.AnnounceName);
		ent.Comp.LastAnnouncement = time;
		((EntitySystem)this).Dirty<MarineCommunicationsComputerComponent>(ent, (MetaDataComponent)null);
	}

	private void OnMarineCommunicationsOpenMapMsg(Entity<MarineCommunicationsComputerComponent> ent, ref MarineCommunicationsOpenMapMsg args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		_ui.TryOpenUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)TacticalMapComputerUi.Key, ((BaseBoundUserInterfaceEvent)args).Actor, false);
	}

	private void OnMarineCommunicationsEchoMsg(Entity<MarineCommunicationsComputerComponent> ent, ref MarineCommunicationsEchoSquadMsg args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.CanCreateEcho && !_squad.HasSquad(EntProtoId<SquadTeamComponent>.op_Implicit(SquadSystem.EchoSquadId)))
		{
			EchoSquadReasonEvent ev = new EchoSquadReasonEvent(((EntitySystem)this).GetNetEntity(((BaseBoundUserInterfaceEvent)args).Actor, (MetaDataComponent)null));
			_dialog.OpenInput(Entity<MarineCommunicationsComputerComponent>.op_Implicit(ent), ((BaseBoundUserInterfaceEvent)args).Actor, "What is the purpose of Echo Squad?", ev);
		}
	}

	private void OnMarineCommunicationsOverwatchMsg(Entity<MarineCommunicationsComputerComponent> ent, ref MarineCommunicationsOverwatchMsg args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (!_skills.HasSkill(Entity<SkillsComponent>.op_Implicit(((BaseBoundUserInterfaceEvent)args).Actor), ent.Comp.OverwatchSkill, ent.Comp.OverwatchSkillLevel))
		{
			_popup.PopupClient("You are not trained in overwatch!", ((BaseBoundUserInterfaceEvent)args).Actor, PopupType.LargeCaution);
		}
		else
		{
			_ui.TryOpenUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)OverwatchConsoleUI.Key, ((BaseBoundUserInterfaceEvent)args).Actor, false);
		}
	}

	private void OnMarineCommunicationsMedalMsg(Entity<MarineCommunicationsComputerComponent> ent, ref MarineControlComputerMedalMsg args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.CanGiveMedals)
		{
			_marineControlComputer.GiveMedal(Entity<MarineCommunicationsComputerComponent>.op_Implicit(ent), ((BaseBoundUserInterfaceEvent)args).Actor);
		}
	}

	public virtual void AnnounceRadio(EntityUid sender, string message, ProtoId<RadioChannelPrototype> channel)
	{
	}

	public virtual void AnnounceARESStaging(EntityUid? source, string message, SoundSpecifier? sound = null, LocId? announcement = null)
	{
	}

	public void AnnounceARES(EntityUid? source, string message, SoundSpecifier? sound = null)
	{
		AnnounceARESStaging(source, message, sound, LocId.op_Implicit("rmc-announcement-ares-command"));
	}

	public virtual void AnnounceSquad(string message, EntProtoId<SquadTeamComponent> squad, SoundSpecifier? sound = null)
	{
	}

	public virtual void AnnounceSquad(string message, EntityUid squad, SoundSpecifier? sound = null)
	{
	}

	public virtual void AnnounceSingle(string message, EntityUid receiver, SoundSpecifier? sound = null)
	{
	}

	public virtual void AnnounceToMarines(string message, SoundSpecifier? sound = null, Filter? filter = null, bool excludeSurvivors = true)
	{
	}

	public virtual void AnnounceHighCommand(string message, string? author = null, SoundSpecifier? sound = null)
	{
	}

	public void AnnounceSigned(EntityUid sender, string message, string? author = null, string? name = null, SoundSpecifier? sound = null, Filter? filter = null, bool excludeSurvivors = true)
	{
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient)
		{
			if (author == null)
			{
				author = base.Loc.GetString("rmc-announcement-author");
			}
			if (name == null)
			{
				name = _rankSystem.GetSpeakerFullRankName(sender) ?? ((EntitySystem)this).Name(sender, (MetaDataComponent)null);
			}
			string wrappedMessage = base.Loc.GetString("rmc-announcement-message-signed", new(string, object)[3]
			{
				("author", author),
				("message", message),
				("name", name)
			});
			AnnounceToMarines(wrappedMessage, sound, filter, excludeSurvivors);
			ISharedAdminLogManager adminLog = _adminLog;
			LogStringHandler handler = new LogStringHandler(27, 2);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(sender)), "source", "ToPrettyString(sender)");
			handler.AppendLiteral(" marine announced message: ");
			handler.AppendFormatted(message);
			adminLog.Add(LogType.RMCMarineAnnounce, ref handler);
		}
	}

	public string FormatHighCommand(string? author, string message)
	{
		if (author == null)
		{
			author = base.Loc.GetString("rmc-announcement-author-highcommand");
		}
		return base.Loc.GetString("rmc-announcement-message", (ValueTuple<string, object>)("author", author), (ValueTuple<string, object>)("message", message));
	}

	public string FormatARESStaging(LocId? author, string message)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		LocId valueOrDefault = author.GetValueOrDefault();
		if (!author.HasValue)
		{
			valueOrDefault = LocId.op_Implicit("rmc-announcement-ares-message");
			author = valueOrDefault;
		}
		ILocalizationManager loc = base.Loc;
		LocId? val = author;
		return loc.GetString(val.HasValue ? LocId.op_Implicit(val.GetValueOrDefault()) : null, (ValueTuple<string, object>)("message", FormattedMessage.EscapeText(message)));
	}

	public string FormatARES(string message)
	{
		return FormatARESStaging(LocId.op_Implicit("rmc-announcement-ares-command"), message);
	}
}
