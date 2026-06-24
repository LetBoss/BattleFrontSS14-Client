using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Admin;
using Content.Shared.ActionBlocker;
using Content.Shared.Hands.Components;
using Content.Shared.Interaction;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Verbs;

public abstract class SharedVerbSystem : EntitySystem
{
	[Dependency]
	private SharedInteractionSystem _interactionSystem;

	[Dependency]
	private ActionBlockerSystem _actionBlockerSystem;

	[Dependency]
	protected SharedContainerSystem ContainerSystem;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeAllEvent<ExecuteVerbEvent>((EntitySessionEventHandler<ExecuteVerbEvent>)HandleExecuteVerb, (Type[])null, (Type[])null);
	}

	private void HandleExecuteVerb(ExecuteVerbEvent args, EntitySessionEventArgs eventArgs)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? user = ((EntitySessionEventArgs)(ref eventArgs)).SenderSession.AttachedEntity;
		EntityUid? target = default(EntityUid?);
		if (user.HasValue && ((EntitySystem)this).TryGetEntity(args.Target, ref target) && !((EntitySystem)this).Deleted(user) && GetLocalVerbs(target.Value, user.Value, args.RequestedVerb.GetType()).TryGetValue(args.RequestedVerb, out Verb verb))
		{
			ExecuteVerb(verb, user.Value, target.Value);
		}
	}

	public SortedSet<Verb> GetLocalVerbs(EntityUid target, EntityUid user, Type type, bool force = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return GetLocalVerbs(target, user, new List<Type> { type }, force);
	}

	public SortedSet<Verb> GetLocalVerbs(EntityUid target, EntityUid user, List<Type> types, bool force = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		List<VerbCategory> extraCategories;
		return GetLocalVerbs(target, user, types, out extraCategories, force);
	}

	public SortedSet<Verb> GetLocalVerbs(EntityUid target, EntityUid user, List<Type> types, out List<VerbCategory> extraCategories, bool force = false)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		SortedSet<Verb> verbs = new SortedSet<Verb>();
		extraCategories = new List<VerbCategory>();
		bool canAccess = force || _interactionSystem.InRangeAndAccessible(Entity<TransformComponent>.op_Implicit(user), Entity<TransformComponent>.op_Implicit(target));
		bool canInteract = force || _actionBlockerSystem.CanInteract(user, target);
		bool canComplexInteract = force || _actionBlockerSystem.CanComplexInteract(user);
		_interactionSystem.TryGetUsedEntity(user, out var @using);
		HandsComponent hands = default(HandsComponent);
		((EntitySystem)this).TryComp<HandsComponent>(user, ref hands);
		if (types.Contains(typeof(InteractionVerb)))
		{
			GetVerbsEvent<InteractionVerb> verbEvent = new GetVerbsEvent<InteractionVerb>(user, target, @using, hands, canInteract, canComplexInteract, canAccess, extraCategories);
			((EntitySystem)this).RaiseLocalEvent<GetVerbsEvent<InteractionVerb>>(target, verbEvent, true);
			verbs.UnionWith(verbEvent.Verbs);
		}
		if (types.Contains(typeof(UtilityVerb)) && @using.HasValue)
		{
			EntityUid? val = @using;
			if (!val.HasValue || val.GetValueOrDefault() != target)
			{
				GetVerbsEvent<UtilityVerb> verbEvent2 = new GetVerbsEvent<UtilityVerb>(user, target, @using, hands, canInteract, canComplexInteract, canAccess, extraCategories);
				((EntitySystem)this).RaiseLocalEvent<GetVerbsEvent<UtilityVerb>>(@using.Value, verbEvent2, true);
				verbs.UnionWith(verbEvent2.Verbs);
			}
		}
		if (types.Contains(typeof(InnateVerb)))
		{
			GetVerbsEvent<InnateVerb> verbEvent3 = new GetVerbsEvent<InnateVerb>(user, target, @using, hands, canInteract, canComplexInteract, canAccess, extraCategories);
			((EntitySystem)this).RaiseLocalEvent<GetVerbsEvent<InnateVerb>>(user, verbEvent3, true);
			verbs.UnionWith(verbEvent3.Verbs);
		}
		if (types.Contains(typeof(AlternativeVerb)))
		{
			GetVerbsEvent<AlternativeVerb> verbEvent4 = new GetVerbsEvent<AlternativeVerb>(user, target, @using, hands, canInteract, canComplexInteract, canAccess, extraCategories);
			((EntitySystem)this).RaiseLocalEvent<GetVerbsEvent<AlternativeVerb>>(target, verbEvent4, true);
			verbs.UnionWith(verbEvent4.Verbs);
		}
		if (types.Contains(typeof(ActivationVerb)))
		{
			GetVerbsEvent<ActivationVerb> verbEvent5 = new GetVerbsEvent<ActivationVerb>(user, target, @using, hands, canInteract, canComplexInteract, canAccess, extraCategories);
			((EntitySystem)this).RaiseLocalEvent<GetVerbsEvent<ActivationVerb>>(target, verbEvent5, true);
			verbs.UnionWith(verbEvent5.Verbs);
		}
		if (types.Contains(typeof(ExamineVerb)))
		{
			GetVerbsEvent<ExamineVerb> verbEvent6 = new GetVerbsEvent<ExamineVerb>(user, target, @using, hands, canInteract, canComplexInteract, canAccess, extraCategories);
			((EntitySystem)this).RaiseLocalEvent<GetVerbsEvent<ExamineVerb>>(target, verbEvent6, true);
			verbs.UnionWith(verbEvent6.Verbs);
		}
		if (types.Contains(typeof(Verb)))
		{
			GetVerbsEvent<Verb> verbEvent7 = new GetVerbsEvent<Verb>(user, target, @using, hands, canInteract, canComplexInteract, canAccess, extraCategories);
			((EntitySystem)this).RaiseLocalEvent<GetVerbsEvent<Verb>>(target, verbEvent7, true);
			verbs.UnionWith(verbEvent7.Verbs);
		}
		if (types.Contains(typeof(EquipmentVerb)))
		{
			if (canAccess)
			{
				_ = 1;
			}
			else
				_interactionSystem.CanAccessEquipment(user, target);
			GetVerbsEvent<EquipmentVerb> verbEvent8 = new GetVerbsEvent<EquipmentVerb>(user, target, @using, hands, canInteract, canComplexInteract, canAccess, extraCategories);
			((EntitySystem)this).RaiseLocalEvent<GetVerbsEvent<EquipmentVerb>>(target, verbEvent8, false);
			verbs.UnionWith(verbEvent8.Verbs);
		}
		if (types.Contains(typeof(RMCAdminVerb)))
		{
			GetVerbsEvent<RMCAdminVerb> verbEvent9 = new GetVerbsEvent<RMCAdminVerb>(user, target, @using, hands, canInteract, canComplexInteract, canAccess, extraCategories);
			((EntitySystem)this).RaiseLocalEvent<GetVerbsEvent<RMCAdminVerb>>(target, verbEvent9, true);
			verbs.UnionWith(verbEvent9.Verbs);
		}
		return verbs;
	}

	public virtual void ExecuteVerb(Verb verb, EntityUid user, EntityUid target, bool forced = false)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		verb.Act?.Invoke();
		if (verb.ExecutionEventArgs != null)
		{
			if (((EntityUid)(ref verb.EventTarget)).IsValid())
			{
				((EntitySystem)this).RaiseLocalEvent(verb.EventTarget, verb.ExecutionEventArgs, false);
			}
			else
			{
				((EntitySystem)this).RaiseLocalEvent(verb.ExecutionEventArgs);
			}
		}
		if (((EntitySystem)this).Deleted(user, (MetaDataComponent)null) || ((EntitySystem)this).Deleted(target, (MetaDataComponent)null))
		{
			return;
		}
		bool? doContactInteraction = verb.DoContactInteraction;
		bool num;
		if (!doContactInteraction.HasValue)
		{
			if (!verb.DefaultDoContactInteraction)
			{
				return;
			}
			num = _interactionSystem.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(user), Entity<TransformComponent>.op_Implicit(target));
		}
		else
		{
			num = doContactInteraction == true;
		}
		if (num)
		{
			_interactionSystem.DoContactInteraction(user, target);
		}
	}
}
