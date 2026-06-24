using System;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Tools.Components;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;

namespace Content.Shared.Tools.Systems;

public sealed class SimpleToolUsageSystem : EntitySystem
{
	[Dependency]
	private SharedDoAfterSystem _doAfterSystem;

	[Dependency]
	private SharedToolSystem _tools;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<SimpleToolUsageComponent, AfterInteractUsingEvent>((EntityEventRefHandler<SimpleToolUsageComponent, AfterInteractUsingEvent>)OnAfterInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SimpleToolUsageComponent, GetVerbsEvent<InteractionVerb>>((EntityEventRefHandler<SimpleToolUsageComponent, GetVerbsEvent<InteractionVerb>>)OnGetInteractionVerbs, (Type[])null, (Type[])null);
	}

	private void OnAfterInteract(Entity<SimpleToolUsageComponent> ent, ref AfterInteractUsingEvent args)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanReach && !((HandledEntityEventArgs)args).Handled && _tools.HasQuality(args.Used, ProtoId<ToolQualityPrototype>.op_Implicit(ent.Comp.Quality)))
		{
			AttemptToolUsage(ent, args.User, args.Used);
		}
	}

	public void OnGetInteractionVerbs(Entity<SimpleToolUsageComponent> ent, ref GetVerbsEvent<InteractionVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		if (!ent.Comp.UsageVerb.HasValue || !args.CanAccess || !args.CanInteract)
		{
			return;
		}
		bool disabled = !args.Using.HasValue || !_tools.HasQuality(args.Using.Value, ProtoId<ToolQualityPrototype>.op_Implicit(ent.Comp.Quality));
		EntityUid? used = args.Using;
		EntityUid user = args.User;
		InteractionVerb obj = new InteractionVerb
		{
			Act = delegate
			{
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				if (used.HasValue)
				{
					AttemptToolUsage(ent, user, used.Value);
				}
			},
			Disabled = disabled,
			Message = (disabled ? base.Loc.GetString(LocId.op_Implicit(ent.Comp.BlockedMessage), (ValueTuple<string, object>)("quality", ent.Comp.Quality)) : null)
		};
		ILocalizationManager loc = base.Loc;
		LocId? usageVerb = ent.Comp.UsageVerb;
		obj.Text = loc.GetString(usageVerb.HasValue ? LocId.op_Implicit(usageVerb.GetValueOrDefault()) : null);
		InteractionVerb verb = obj;
		args.Verbs.Add(verb);
	}

	private void AttemptToolUsage(Entity<SimpleToolUsageComponent> ent, EntityUid user, EntityUid tool)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		AttemptSimpleToolUseEvent attemptEv = new AttemptSimpleToolUseEvent(user);
		((EntitySystem)this).RaiseLocalEvent<AttemptSimpleToolUseEvent>(Entity<SimpleToolUsageComponent>.op_Implicit(ent), ref attemptEv, false);
		if (!attemptEv.Cancelled)
		{
			DoAfterArgs doAfterArgs = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, ent.Comp.DoAfter, new SimpleToolDoAfterEvent(), Entity<SimpleToolUsageComponent>.op_Implicit(ent), Entity<SimpleToolUsageComponent>.op_Implicit(ent), tool)
			{
				BreakOnDamage = true,
				BreakOnDropItem = true,
				BreakOnMove = true,
				BreakOnHandChange = true,
				NeedHand = true
			};
			_doAfterSystem.TryStartDoAfter(doAfterArgs);
		}
	}
}
