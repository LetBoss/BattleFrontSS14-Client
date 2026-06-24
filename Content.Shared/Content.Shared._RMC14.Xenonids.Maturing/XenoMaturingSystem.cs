using System;
using Content.Shared.Actions;
using Content.Shared.Examine;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Content.Shared.NameModifier.Components;
using Content.Shared.NameModifier.EntitySystems;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.Maturing;

public sealed class XenoMaturingSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private MobThresholdSystem _mobThreshold;

	[Dependency]
	private NameModifierSystem _nameModifier;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private IGameTiming _timing;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoMaturingComponent, MapInitEvent>((EntityEventRefHandler<XenoMaturingComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoMaturingComponent, ComponentRemove>((EntityEventRefHandler<XenoMaturingComponent, ComponentRemove>)OnRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoMaturingComponent, RefreshNameModifiersEvent>((EntityEventRefHandler<XenoMaturingComponent, RefreshNameModifiersEvent>)OnRefreshNameModifiers, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoMaturingComponent, ExaminedEvent>((EntityEventRefHandler<XenoMaturingComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
	}

	private void OnMapInit(Entity<XenoMaturingComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.MatureAt = _timing.CurTime + ent.Comp.Delay;
		((EntitySystem)this).Dirty<XenoMaturingComponent>(ent, (MetaDataComponent)null);
		_nameModifier.RefreshNameModifiers(Entity<NameModifierComponent>.op_Implicit(ent.Owner));
	}

	private void OnRemove(Entity<XenoMaturingComponent> ent, ref ComponentRemove args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).TerminatingOrDeleted(Entity<XenoMaturingComponent>.op_Implicit(ent), (MetaDataComponent)null))
		{
			_nameModifier.RefreshNameModifiers(Entity<NameModifierComponent>.op_Implicit(ent.Owner));
		}
	}

	private void OnRefreshNameModifiers(Entity<XenoMaturingComponent> ent, ref RefreshNameModifiersEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		args.AddModifier(LocId.op_Implicit("rmc-xeno-immature-prefix"), 0);
	}

	private void OnExamined(Entity<XenoMaturingComponent> ent, ref ExaminedEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<XenoComponent>(args.Examiner))
		{
			return;
		}
		TimeSpan time = ent.Comp.MatureAt - _timing.CurTime;
		if (time <= TimeSpan.Zero)
		{
			return;
		}
		using (args.PushGroup("XenoMaturingSystem"))
		{
			int minutes = (int)time.TotalMinutes;
			int seconds = time.Seconds;
			if (minutes > 0)
			{
				args.PushText(base.Loc.GetString("rmc-xeno-immature-matures-in-minutes", (ValueTuple<string, object>)("minutes", minutes), (ValueTuple<string, object>)("seconds", seconds)));
			}
			else if (seconds > 0)
			{
				args.PushText(base.Loc.GetString("rmc-xeno-immature-matures-in-seconds", (ValueTuple<string, object>)("seconds", seconds)));
			}
		}
	}

	public void Mature(Entity<XenoMaturingComponent> maturing)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		maturing.Comp.MatureAt = TimeSpan.Zero;
		((EntitySystem)this).Dirty<XenoMaturingComponent>(maturing, (MetaDataComponent)null);
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<XenoMaturingComponent> query = ((EntitySystem)this).EntityQueryEnumerator<XenoMaturingComponent>();
		EntityUid uid = default(EntityUid);
		XenoMaturingComponent comp = default(XenoMaturingComponent);
		while (query.MoveNext(ref uid, ref comp) && !(time < comp.MatureAt))
		{
			_mobThreshold.SetMobStateThreshold(uid, comp.DeadThreshold, MobState.Dead);
			_mobThreshold.SetMobStateThreshold(uid, comp.CritThreshold, MobState.Critical);
			base.EntityManager.AddComponents(uid, comp.AddComponents, true);
			foreach (EntProtoId action in comp.AddActions)
			{
				_actions.AddAction(uid, EntProtoId.op_Implicit(action));
			}
			_popup.PopupEntity(base.Loc.GetString("rmc-xeno-immature-mature"), uid, uid, PopupType.Large);
			((EntitySystem)this).RemCompDeferred<XenoMaturingComponent>(uid);
		}
	}
}
