using System;
using System.Collections.Generic;
using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.IdentityManagement;
using Content.Shared._RMC14.Xenonids.Evolution;
using Content.Shared.Mind;
using Content.Shared.Mind.Components;
using Content.Shared.NameModifier.Components;
using Content.Shared.NameModifier.EntitySystems;
using Content.Shared.Players.PlayTimeTracking;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Xenonids.Name;

public abstract class SharedXenoNameSystem : EntitySystem
{
	[Dependency]
	private IConfigurationManager _config;

	[Dependency]
	private SharedMindSystem _mind;

	[Dependency]
	private NameModifierSystem _nameModifier;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private ISharedPlaytimeManager _playtime;

	[Dependency]
	private IPrototypeManager _prototype;

	private const string DefaultPrefix = "XX";

	private TimeSpan _xenoPrefixThreeTime;

	private TimeSpan _xenoPostfixTime;

	private TimeSpan _xenoPostfixTwoTime;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<NewXenoEvolvedEvent>((EntityEventRefHandler<NewXenoEvolvedEvent>)OnNewXenoEvolved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoDevolvedEvent>((EntityEventRefHandler<XenoDevolvedEvent>)OnXenoDevolved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoNameComponent, RefreshNameModifiersEvent>((EntityEventRefHandler<XenoNameComponent, RefreshNameModifiersEvent>)OnRefreshNameModifiers, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoNameComponent, MindAddedMessage>((EntityEventRefHandler<XenoNameComponent, MindAddedMessage>)OnMindAdded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoNameComponent, RMCGetFixedIdentityEvent>((EntityEventRefHandler<XenoNameComponent, RMCGetFixedIdentityEvent>)OnGetFixedIdentity, (Type[])null, (Type[])null);
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _config, RMCCVars.RMCPlaytimeXenoPrefixThreeTimeHours, (Action<int>)delegate(int v)
		{
			_xenoPrefixThreeTime = TimeSpan.FromHours(v);
		}, true);
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _config, RMCCVars.RMCPlaytimeXenoPostfixTimeHours, (Action<int>)delegate(int v)
		{
			_xenoPostfixTime = TimeSpan.FromHours(v);
		}, true);
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _config, RMCCVars.RMCPlaytimeXenoPostfixTwoTimeHours, (Action<int>)delegate(int v)
		{
			_xenoPostfixTwoTime = TimeSpan.FromHours(v);
		}, true);
	}

	private void OnNewXenoEvolved(ref NewXenoEvolvedEvent ev)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		TransferName(Entity<XenoEvolutionComponent>.op_Implicit(ev.OldXeno), ev.NewXeno);
	}

	private void OnXenoDevolved(ref XenoDevolvedEvent ev)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		TransferName(ev.OldXeno, ev.NewXeno);
	}

	private void OnRefreshNameModifiers(Entity<XenoNameComponent> ent, ref RefreshNameModifiersEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		string rank = ent.Comp.Rank;
		if (rank.Length > 0)
		{
			rank += " ";
		}
		string prefix = ent.Comp.Prefix;
		if (prefix.Length == 0)
		{
			prefix = "XX";
		}
		string postfix = ent.Comp.Postfix;
		int number = ent.Comp.Number;
		if (((EntitySystem)this).HasComp<XenoOmitNumberComponent>(Entity<XenoNameComponent>.op_Implicit(ent)))
		{
			args.AddModifier(LocId.op_Implicit("rmc-xeno-name"), 0, ("rank", rank), ("prefix", prefix), ("postfix", postfix));
		}
		else
		{
			if (postfix.Length > 0)
			{
				postfix = "-" + postfix;
			}
			args.AddModifier(LocId.op_Implicit("rmc-xeno-name-number"), 0, ("rank", rank), ("prefix", prefix), ("number", number), ("postfix", postfix));
		}
		if (_mind.TryGetMind(Entity<XenoNameComponent>.op_Implicit(ent), out EntityUid _, out MindComponent mind))
		{
			mind.CharacterName = args.GetModifiedName();
		}
	}

	private void OnMindAdded(Entity<XenoNameComponent> ent, ref MindAddedMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		SetupName(Entity<XenoNameComponent>.op_Implicit(ent));
	}

	private void OnGetFixedIdentity(Entity<XenoNameComponent> ent, ref RMCGetFixedIdentityEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<XenoOmitNumberComponent>(Entity<XenoNameComponent>.op_Implicit(ent)))
		{
			args.Name = base.Loc.GetString("rmc-xeno-name", new(string, object)[3]
			{
				("baseName", args.Name),
				("prefix", "XX"),
				("postfix", string.Empty)
			});
		}
		else
		{
			args.Name = base.Loc.GetString("rmc-xeno-name-number", new(string, object)[4]
			{
				("baseName", args.Name),
				("prefix", "XX"),
				("number", ent.Comp.Number),
				("postfix", string.Empty)
			});
		}
	}

	private TimeSpan GetXenoPlaytime(ICommonSession player)
	{
		TimeSpan xenoPlaytime = TimeSpan.Zero;
		try
		{
			PlayTimeTrackerPrototype tracker = default(PlayTimeTrackerPrototype);
			foreach (var (id, time) in _playtime.GetPlayTimes(player))
			{
				if (_prototype.TryIndex<PlayTimeTrackerPrototype>(id, ref tracker) && tracker.IsXeno)
				{
					xenoPlaytime += time;
				}
			}
		}
		catch (Exception value)
		{
			((EntitySystem)this).Log.Error($"Error reading total xeno playtime:\n{value}");
		}
		return xenoPlaytime;
	}

	public int GetMaxXenoPrefixLength(ICommonSession player)
	{
		if (!(GetXenoPlaytime(player) < _xenoPrefixThreeTime))
		{
			return 3;
		}
		return 2;
	}

	public int GetMaxXenoPostfixLength(ICommonSession player)
	{
		TimeSpan time = GetXenoPlaytime(player);
		if (time > _xenoPostfixTwoTime)
		{
			return 2;
		}
		if (time > _xenoPostfixTime)
		{
			return 1;
		}
		return 0;
	}

	private void TransferName(EntityUid oldXeno, EntityUid newXeno)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		XenoNameComponent oldName = default(XenoNameComponent);
		if (!_net.IsClient && ((EntitySystem)this).TryComp<XenoNameComponent>(oldXeno, ref oldName))
		{
			XenoNameComponent newName = ((EntitySystem)this).EnsureComp<XenoNameComponent>(newXeno);
			newName.Rank = oldName.Rank;
			newName.Prefix = oldName.Prefix;
			newName.Number = oldName.Number;
			newName.Postfix = oldName.Postfix;
			((EntitySystem)this).Dirty(newXeno, (IComponent)(object)newName, (MetaDataComponent)null);
			((EntitySystem)this).RemComp<AssignXenoNameComponent>(newXeno);
			_nameModifier.RefreshNameModifiers(Entity<NameModifierComponent>.op_Implicit(newXeno));
		}
	}

	public virtual void SetupName(EntityUid xeno)
	{
	}

	public override void Update(float frameTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<AssignXenoNameComponent> query = ((EntitySystem)this).EntityQueryEnumerator<AssignXenoNameComponent>();
		EntityUid uid = default(EntityUid);
		AssignXenoNameComponent assignXenoNameComponent = default(AssignXenoNameComponent);
		while (query.MoveNext(ref uid, ref assignXenoNameComponent))
		{
			SetupName(uid);
		}
	}
}
