using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using Content.Client._RMC14.NightVision;
using Content.Shared._RMC14.Atmos;
using Content.Shared._RMC14.Teleporter;
using Content.Shared._RMC14.Xenonids;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Containers;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;

namespace Content.Client._RMC14.Teleporter;

public sealed class RMCTeleporterViewerOverlay : Overlay
{
	[Dependency]
	private IEntityManager _entity;

	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private IOverlayManager _overlay;

	private readonly SharedContainerSystem _container;

	private readonly EntityLookupSystem _entityLookup;

	private readonly SharedPhysicsSystem _physics;

	private readonly SpriteSystem _sprite;

	private readonly SharedRMCTeleporterSystem _teleporter;

	private readonly SharedTransformSystem _transform;

	private readonly EntityQuery<SpriteComponent> _spriteQuery;

	private readonly EntityQuery<RMCTeleporterViewerComponent> _teleporterViewerQuery;

	private readonly EntityQuery<TileFireComponent> _tileFireQuery;

	private readonly EntityQuery<TransformComponent> _transformQuery;

	private readonly EntityQuery<XenoComponent> _xenoQuery;

	private readonly List<(Entity<SpriteComponent> Ent, Vector2 Position, Angle Rotation)> _toDraw = new List<(Entity<SpriteComponent>, Vector2, Angle)>();

	public override OverlaySpace Space
	{
		get
		{
			if (_overlay.HasOverlay<NightVisionOverlay>())
			{
				return (OverlaySpace)4;
			}
			return (OverlaySpace)8;
		}
	}

	public RMCTeleporterViewerOverlay()
	{
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<RMCTeleporterViewerOverlay>(this);
		_container = _entity.System<SharedContainerSystem>();
		_entityLookup = _entity.System<EntityLookupSystem>();
		_physics = _entity.System<SharedPhysicsSystem>();
		_sprite = _entity.System<SpriteSystem>();
		_teleporter = _entity.System<SharedRMCTeleporterSystem>();
		_transform = _entity.System<SharedTransformSystem>();
		_spriteQuery = _entity.GetEntityQuery<SpriteComponent>();
		_teleporterViewerQuery = _entity.GetEntityQuery<RMCTeleporterViewerComponent>();
		_tileFireQuery = _entity.GetEntityQuery<TileFireComponent>();
		_transformQuery = _entity.GetEntityQuery<TransformComponent>();
		_xenoQuery = _entity.GetEntityQuery<XenoComponent>();
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (!localEntity.HasValue)
		{
			return;
		}
		EntityUid valueOrDefault = localEntity.GetValueOrDefault();
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		IEye eye = args.Viewport.Eye;
		Angle val = (Angle)((eye != null) ? eye.Rotation : default(Angle));
		RMCTeleporterViewerComponent item = default(RMCTeleporterViewerComponent);
		SpriteComponent val2 = default(SpriteComponent);
		TransformComponent val3 = default(TransformComponent);
		foreach (EntityUid item4 in _physics.GetEntitiesIntersectingBody(valueOrDefault, 65, true, (PhysicsComponent)null, (FixturesComponent)null, (TransformComponent)null))
		{
			if (!_teleporterViewerQuery.TryComp(item4, ref item))
			{
				continue;
			}
			Vector2 worldPosition = _transform.GetWorldPosition(item4);
			foreach (Entity<RMCTeleporterViewerComponent> matchingTeleporterViewer in _teleporter.GetMatchingTeleporterViewers(Entity<RMCTeleporterViewerComponent>.op_Implicit((item4, item))))
			{
				MapCoordinates mapCoordinates = _transform.GetMapCoordinates(Entity<RMCTeleporterViewerComponent>.op_Implicit(matchingTeleporterViewer), (TransformComponent)null);
				Vector2 vector = mapCoordinates.Position - worldPosition;
				Box2 worldAABB = _physics.GetWorldAABB(Entity<RMCTeleporterViewerComponent>.op_Implicit(matchingTeleporterViewer), (FixturesComponent)null, (PhysicsComponent)null, (TransformComponent)null);
				_toDraw.Clear();
				foreach (EntityUid item5 in _entityLookup.GetEntitiesIntersecting(mapCoordinates.MapId, worldAABB, (LookupFlags)78))
				{
					if (_spriteQuery.TryComp(item5, ref val2) && val2.Visible && _transformQuery.TryComp(item5, ref val3) && !_container.IsEntityInContainer(item5, (MetaDataComponent)null) && (!val3.Anchored || _xenoQuery.HasComp(item5) || _tileFireQuery.HasComp(item5)))
					{
						var (item2, item3) = _transform.GetWorldPositionRotation(val3);
						_toDraw.Add((Entity<SpriteComponent>.op_Implicit((item5, val2)), item2, item3));
					}
				}
				_toDraw.Sort(((Entity<SpriteComponent> Ent, Vector2 Position, Angle Rotation) a, (Entity<SpriteComponent> Ent, Vector2 Position, Angle Rotation) b) => a.Ent.Comp.DrawDepth.CompareTo(b.Ent.Comp.DrawDepth));
				Span<(Entity<SpriteComponent>, Vector2, Angle)> span = CollectionsMarshal.AsSpan(_toDraw);
				for (int num = 0; num < span.Length; num++)
				{
					ref(Entity<SpriteComponent>, Vector2, Angle) reference = ref span[num];
					reference.Item2 -= vector;
					_sprite.RenderSprite(reference.Item1, worldHandle, val, reference.Item3, reference.Item2, (Direction?)null);
				}
			}
		}
		Matrix3x2 identity = Matrix3x2.Identity;
		((DrawingHandleBase)worldHandle).SetTransform(ref identity);
	}
}
