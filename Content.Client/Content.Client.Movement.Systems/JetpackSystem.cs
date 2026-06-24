using System;
using Content.Shared.Clothing.Components;
using Content.Shared.Clothing.EntitySystems;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Robust.Client.GameObjects;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Physics.Components;
using Robust.Shared.Timing;

namespace Content.Client.Movement.Systems;

public sealed class JetpackSystem : SharedJetpackSystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private ClothingSystem _clothing;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedMapSystem _mapSystem;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<JetpackComponent, AppearanceChangeEvent>((ComponentEventRefHandler<JetpackComponent, AppearanceChangeEvent>)OnJetpackAppearance, (Type[])null, (Type[])null);
	}

	protected override bool CanEnable(EntityUid uid, JetpackComponent component)
	{
		return false;
	}

	private void OnJetpackAppearance(EntityUid uid, JetpackComponent component, ref AppearanceChangeEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		bool flag = default(bool);
		Appearance.TryGetData<bool>(uid, (Enum)JetpackVisuals.Enabled, ref flag, args.Component);
		ClothingComponent clothing = default(ClothingComponent);
		if (((EntitySystem)this).TryComp<ClothingComponent>(uid, ref clothing))
		{
			_clothing.SetEquippedPrefix(uid, flag ? "on" : null, clothing);
		}
	}

	public override void Update(float frameTime)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		if (!_timing.IsFirstTimePredicted)
		{
			return;
		}
		EntityQueryEnumerator<ActiveJetpackComponent, TransformComponent> val = ((EntitySystem)this).EntityQueryEnumerator<ActiveJetpackComponent, TransformComponent>();
		EntityUid uid = default(EntityUid);
		ActiveJetpackComponent activeJetpackComponent = default(ActiveJetpackComponent);
		TransformComponent val2 = default(TransformComponent);
		while (val.MoveNext(ref uid, ref activeJetpackComponent, ref val2))
		{
			if (!_transform.InRange(val2.Coordinates, activeJetpackComponent.LastCoordinates, activeJetpackComponent.MaxDistance) || !(_timing.CurTime < activeJetpackComponent.TargetTime))
			{
				activeJetpackComponent.LastCoordinates = _transform.GetMoverCoordinates(val2.Coordinates);
				activeJetpackComponent.TargetTime = _timing.CurTime + TimeSpan.FromSeconds(activeJetpackComponent.EffectCooldown);
				CreateParticles(uid);
			}
		}
	}

	private void CreateParticles(EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent val = ((EntitySystem)this).Transform(uid);
		BaseContainer val2 = default(BaseContainer);
		PhysicsComponent val3 = default(PhysicsComponent);
		if (Container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent, MetaDataComponent>)(uid, val, null)), ref val2) && ((EntitySystem)this).TryComp<PhysicsComponent>(val2.Owner, ref val3) && val3.LinearVelocity.LengthSquared() < 1f)
		{
			return;
		}
		EntityCoordinates coordinates = val.Coordinates;
		EntityUid? grid = _transform.GetGrid(coordinates);
		MapGridComponent val4 = default(MapGridComponent);
		if (((EntitySystem)this).TryComp<MapGridComponent>(grid, ref val4))
		{
			((EntityCoordinates)(ref coordinates))._002Ector(grid.Value, _mapSystem.WorldToLocal(grid.Value, val4, _transform.ToMapCoordinates(coordinates, true).Position));
		}
		else
		{
			if (!val.MapUid.HasValue)
			{
				return;
			}
			((EntityCoordinates)(ref coordinates))._002Ector(val.MapUid.Value, _transform.GetWorldPosition(val));
		}
		((EntitySystem)this).Spawn("JetpackEffect", coordinates);
	}
}
