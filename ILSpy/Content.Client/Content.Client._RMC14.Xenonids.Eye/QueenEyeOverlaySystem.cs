using System;
using Content.Shared._RMC14.Xenonids.Eye;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;

namespace Content.Client._RMC14.Xenonids.Eye;

public sealed class QueenEyeOverlaySystem : EntitySystem
{
	[Dependency]
	private IOverlayManager _overlay;

	[Dependency]
	private IPlayerManager _player;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<QueenEyeActionComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<QueenEyeActionComponent, AfterAutoHandleStateEvent>)OnUpdated, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<QueenEyeActionComponent, QueenEyeActionUpdated>((EntityEventRefHandler<QueenEyeActionComponent, QueenEyeActionUpdated>)OnUpdated, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<QueenEyeActionComponent, LocalPlayerAttachedEvent>((EntityEventRefHandler<QueenEyeActionComponent, LocalPlayerAttachedEvent>)OnAttached, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<QueenEyeActionComponent, LocalPlayerDetachedEvent>((EntityEventRefHandler<QueenEyeActionComponent, LocalPlayerDetachedEvent>)OnDetached, (Type[])null, (Type[])null);
	}

	private void OnUpdated<T>(Entity<QueenEyeActionComponent> ent, ref T args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		Updated(ent);
	}

	private void OnAttached(Entity<QueenEyeActionComponent> ent, ref LocalPlayerAttachedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		Updated(ent);
	}

	private void OnDetached(Entity<QueenEyeActionComponent> ent, ref LocalPlayerDetachedEvent args)
	{
		_overlay.RemoveOverlay<QueenEyeOverlay>();
	}

	private void Updated(Entity<QueenEyeActionComponent> ent)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		EntityUid val = Entity<QueenEyeActionComponent>.op_Implicit(ent);
		if (localEntity.HasValue && !(localEntity.GetValueOrDefault() != val))
		{
			if (!ent.Comp.Eye.HasValue)
			{
				_overlay.RemoveOverlay<QueenEyeOverlay>();
			}
			else if (!_overlay.HasOverlay<QueenEyeOverlay>())
			{
				_overlay.AddOverlay((Overlay)(object)new QueenEyeOverlay());
			}
		}
	}
}
