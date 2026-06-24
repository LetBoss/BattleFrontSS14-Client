using System;
using System.Numerics;
using Content.Client.Movement.Components;
using Content.Client.Movement.Systems;
using Content.Shared.Camera;
using Content.Shared.Hands;
using Content.Shared.Movement.Components;
using Content.Shared.Wieldable;
using Content.Shared.Wieldable.Components;
using Robust.Client.Timing;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Client.Wieldable;

public sealed class WieldableSystem : SharedWieldableSystem
{
	[Dependency]
	private EyeCursorOffsetSystem _eyeOffset;

	[Dependency]
	private IClientGameTiming _gameTiming;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<CursorOffsetRequiresWieldComponent, ItemUnwieldedEvent>((EntityEventRefHandler<CursorOffsetRequiresWieldComponent, ItemUnwieldedEvent>)OnEyeOffsetUnwielded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CursorOffsetRequiresWieldComponent, HeldRelayedEvent<GetEyeOffsetRelayedEvent>>((EntityEventRefHandler<CursorOffsetRequiresWieldComponent, HeldRelayedEvent<GetEyeOffsetRelayedEvent>>)OnGetEyeOffset, (Type[])null, (Type[])null);
	}

	public void OnEyeOffsetUnwielded(Entity<CursorOffsetRequiresWieldComponent> entity, ref ItemUnwieldedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		EyeCursorOffsetComponent eyeCursorOffsetComponent = default(EyeCursorOffsetComponent);
		if (((EntitySystem)this).TryComp<EyeCursorOffsetComponent>(entity.Owner, ref eyeCursorOffsetComponent) && ((IGameTiming)_gameTiming).IsFirstTimePredicted)
		{
			eyeCursorOffsetComponent.CurrentPosition = Vector2.Zero;
		}
	}

	public void OnGetEyeOffset(Entity<CursorOffsetRequiresWieldComponent> entity, ref HeldRelayedEvent<GetEyeOffsetRelayedEvent> args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		WieldableComponent wieldableComponent = default(WieldableComponent);
		if (((EntitySystem)this).TryComp<WieldableComponent>(entity.Owner, ref wieldableComponent) && wieldableComponent.Wielded)
		{
			Vector2? vector = _eyeOffset.OffsetAfterMouse(entity.Owner, null);
			if (vector.HasValue)
			{
				args.Args.Offset += vector.Value;
			}
		}
	}
}
