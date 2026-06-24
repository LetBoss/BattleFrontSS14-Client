using System;
using Content.Shared._RMC14.Input;
using Content.Shared.ActionBlocker;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Player;

namespace Content.Shared._RMC14.Weapons.Common;

public sealed class UniqueActionSystem : EntitySystem
{
	[Dependency]
	private ActionBlockerSystem _actionBlocker;

	[Dependency]
	private SharedHandsSystem _hands;

	public override void Initialize()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		((EntitySystem)this).SubscribeLocalEvent<UniqueActionComponent, GetVerbsEvent<InteractionVerb>>((EntityEventRefHandler<UniqueActionComponent, GetVerbsEvent<InteractionVerb>>)OnGetVerbs, (Type[])null, (Type[])null);
		CommandBinds.Builder.Bind(CMKeyFunctions.CMUniqueAction, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate(ICommonSession? session)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			EntityUid? val = ((session != null) ? session.AttachedEntity : ((EntityUid?)null));
			if (val.HasValue)
			{
				EntityUid valueOrDefault = val.GetValueOrDefault();
				TryUniqueAction(valueOrDefault);
			}
		}, (StateInputCmdDelegate)null, false, true)).Register<UniqueActionSystem>();
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		CommandBinds.Unregister<UniqueActionSystem>();
	}

	private void OnGetVerbs(Entity<UniqueActionComponent> ent, ref GetVerbsEvent<InteractionVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanAccess && args.CanInteract && _actionBlocker.CanInteract(args.User, args.Target))
		{
			EntityUid user = args.User;
			args.Verbs.Add(new InteractionVerb
			{
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					TryUniqueAction(user, ent.Owner);
				},
				Text = "Unique action"
			});
		}
	}

	private void TryUniqueAction(EntityUid userUid)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (_hands.TryGetActiveItem(Entity<HandsComponent>.op_Implicit(userUid), out var held))
		{
			TryUniqueAction(userUid, held.Value);
		}
	}

	private void TryUniqueAction(EntityUid userUid, EntityUid targetUid)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (_actionBlocker.CanInteract(userUid, targetUid))
		{
			((EntitySystem)this).RaiseLocalEvent<UniqueActionEvent>(targetUid, new UniqueActionEvent(userUid), false);
		}
	}
}
