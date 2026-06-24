using System;
using Content.Shared._RMC14.Xenonids.Eye;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared.Movement.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Player;

namespace Content.Shared._RMC14.Xenonids.Watch;

public abstract class SharedXenoWatchSystem : EntitySystem
{
	[Dependency]
	private SharedEyeSystem _eye;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private ISharedPlayerManager _player;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<XenoComponent, XenoWatchActionEvent>((EntityEventRefHandler<XenoComponent, XenoWatchActionEvent>)OnXenoWatchAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoWatchingComponent, MoveInputEvent>((EntityEventRefHandler<XenoWatchingComponent, MoveInputEvent>)OnXenoMoveInput, (Type[])null, (Type[])null);
		BoundUserInterfaceRegisterExt.BuiEvents<XenoComponent>(((EntitySystem)this).Subs, (object)XenoWatchUIKey.Key, (BuiEventSubscriber<XenoComponent>)delegate(Subscriber<XenoComponent> subs)
		{
			subs.Event<XenoWatchBuiMsg>((EntityEventRefHandler<XenoComponent, XenoWatchBuiMsg>)OnXenoWatchBui);
		});
	}

	private void OnXenoMoveInput(Entity<XenoWatchingComponent> xeno, ref MoveInputEvent args)
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		if (!args.HasDirectionalMovement)
		{
			return;
		}
		if (_net.IsClient)
		{
			EntityUid? localEntity = _player.LocalEntity;
			EntityUid owner = xeno.Owner;
			if (localEntity.HasValue && localEntity.GetValueOrDefault() == owner && _player.LocalSession != null)
			{
				Unwatch(Entity<EyeComponent>.op_Implicit(xeno.Owner), _player.LocalSession);
				return;
			}
		}
		ActorComponent actor = default(ActorComponent);
		QueenEyeComponent eye = default(QueenEyeComponent);
		ActorComponent eyeActor = default(ActorComponent);
		if (((EntitySystem)this).TryComp<ActorComponent>(Entity<XenoWatchingComponent>.op_Implicit(xeno), ref actor))
		{
			Unwatch(Entity<EyeComponent>.op_Implicit(xeno.Owner), actor.PlayerSession);
		}
		else if (((EntitySystem)this).TryComp<QueenEyeComponent>(Entity<XenoWatchingComponent>.op_Implicit(xeno), ref eye) && ((EntitySystem)this).TryComp<ActorComponent>(eye.Queen, ref eyeActor))
		{
			Unwatch(Entity<EyeComponent>.op_Implicit(xeno.Owner), eyeActor.PlayerSession);
		}
	}

	private void OnXenoWatchBui(Entity<XenoComponent> xeno, ref XenoWatchBuiMsg args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? target = default(EntityUid?);
		if (((EntitySystem)this).TryGetEntity(args.Target, ref target))
		{
			Watch(Entity<HiveMemberComponent, ActorComponent, EyeComponent>.op_Implicit(xeno.Owner), Entity<HiveMemberComponent>.op_Implicit(target.Value));
		}
	}

	protected virtual void OnXenoWatchAction(Entity<XenoComponent> ent, ref XenoWatchActionEvent args)
	{
	}

	public virtual void Watch(Entity<HiveMemberComponent?, ActorComponent?, EyeComponent?> watcher, Entity<HiveMemberComponent?> toWatch)
	{
	}

	protected virtual void Unwatch(Entity<EyeComponent?> watcher, ICommonSession player)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<EyeComponent>(Entity<EyeComponent>.op_Implicit(watcher), ref watcher.Comp, true))
		{
			_eye.SetTarget(Entity<EyeComponent>.op_Implicit(watcher), (EntityUid?)null, Entity<EyeComponent>.op_Implicit(watcher));
			XenoUnwatchEvent ev = default(XenoUnwatchEvent);
			((EntitySystem)this).RaiseLocalEvent<XenoUnwatchEvent>(Entity<EyeComponent>.op_Implicit(watcher), ref ev, false);
		}
	}

	public bool TryGetWatched(Entity<XenoWatchingComponent?> watching, out EntityUid watched)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<XenoWatchingComponent>(Entity<XenoWatchingComponent>.op_Implicit(watching), ref watching.Comp, false) || !watching.Comp.Watching.HasValue)
		{
			watched = default(EntityUid);
			return false;
		}
		watched = watching.Comp.Watching.Value;
		return true;
	}

	public void SetWatching(Entity<XenoWatchingComponent?> watching, EntityUid target)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		watching.Comp = ((EntitySystem)this).EnsureComp<XenoWatchingComponent>(Entity<XenoWatchingComponent>.op_Implicit(watching));
		watching.Comp.Watching = target;
		((EntitySystem)this).Dirty<XenoWatchingComponent>(watching, (MetaDataComponent)null);
	}
}
