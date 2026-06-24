using System;
using Content.Shared.Weapons.Marker;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Client.Weapons.Marker;

public sealed class DamageMarkerSystem : SharedDamageMarkerSystem
{
	private enum DamageMarkerKey : byte
	{
		Key
	}

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<DamageMarkerComponent, ComponentStartup>((ComponentEventHandler<DamageMarkerComponent, ComponentStartup>)OnMarkerStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DamageMarkerComponent, ComponentShutdown>((ComponentEventHandler<DamageMarkerComponent, ComponentShutdown>)OnMarkerShutdown, (Type[])null, (Type[])null);
	}

	private void OnMarkerStartup(EntityUid uid, DamageMarkerComponent component, ComponentStartup args)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		if (_timing.ApplyingState && component.Effect != null && ((EntitySystem)this).TryComp<SpriteComponent>(uid, ref item))
		{
			int num = _sprite.LayerMapReserve(Entity<SpriteComponent>.op_Implicit((uid, item)), (Enum)DamageMarkerKey.Key);
			_sprite.LayerSetRsi(Entity<SpriteComponent>.op_Implicit((uid, item)), num, component.Effect.RsiPath, (StateId?)StateId.op_Implicit(component.Effect.RsiState));
		}
	}

	private void OnMarkerShutdown(EntityUid uid, DamageMarkerComponent component, ComponentShutdown args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		int num = default(int);
		if (_timing.ApplyingState && ((EntitySystem)this).TryComp<SpriteComponent>(uid, ref item) && _sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, item)), (Enum)DamageMarkerKey.Key, ref num, false))
		{
			_sprite.RemoveLayer(Entity<SpriteComponent>.op_Implicit((uid, item)), num, true);
		}
	}
}
