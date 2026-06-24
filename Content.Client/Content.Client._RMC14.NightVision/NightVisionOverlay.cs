using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Client.Examine;
using Content.Shared._RMC14.NightVision;
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
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Client._RMC14.NightVision;

public sealed class NightVisionOverlay : Overlay
{
	[Dependency]
	private IEntityManager _entity;

	[Dependency]
	private IPlayerManager _players;

	[Dependency]
	private IPrototypeManager _prototype;

	private readonly ContainerSystem _container;

	private readonly ExamineSystem _examine;

	private readonly SpriteSystem _sprite;

	private readonly TransformSystem _transform;

	private readonly EntityQuery<XenoComponent> _xenoQuery;

	private readonly ShaderInstance _shader;

	private readonly List<NightVisionRenderEntry> _entries = new List<NightVisionRenderEntry>();

	public override OverlaySpace Space => (OverlaySpace)4;

	public NightVisionOverlay()
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<NightVisionOverlay>(this);
		_container = _entity.System<ContainerSystem>();
		_examine = _entity.System<ExamineSystem>();
		_sprite = _entity.System<SpriteSystem>();
		_transform = _entity.System<TransformSystem>();
		_xenoQuery = _entity.GetEntityQuery<XenoComponent>();
		_shader = _prototype.Index<ShaderPrototype>(new ProtoId<ShaderPrototype>("RMCNightVision")).Instance().Duplicate();
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		NightVisionComponent nightVisionComponent = default(NightVisionComponent);
		if (!_entity.TryGetComponent<NightVisionComponent>(((ISharedPlayerManager)_players).LocalEntity, ref nightVisionComponent) || nightVisionComponent.State == NightVisionState.Off)
		{
			return;
		}
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		IEye eye = args.Viewport.Eye;
		Angle eyeRot = (Angle)((eye != null) ? eye.Rotation : default(Angle));
		_entries.Clear();
		EntityQueryEnumerator<RMCNightVisionVisibleComponent, SpriteComponent, TransformComponent> val = _entity.EntityQueryEnumerator<RMCNightVisionVisibleComponent, SpriteComponent, TransformComponent>();
		EntityUid item = default(EntityUid);
		RMCNightVisionVisibleComponent rMCNightVisionVisibleComponent = default(RMCNightVisionVisibleComponent);
		SpriteComponent item2 = default(SpriteComponent);
		TransformComponent item3 = default(TransformComponent);
		while (val.MoveNext(ref item, ref rMCNightVisionVisibleComponent, ref item2, ref item3))
		{
			_entries.Add(new NightVisionRenderEntry((item, item2, item3), (eye != null) ? new MapId?(eye.Position.MapId) : ((MapId?)null), nightVisionComponent.SeeThroughContainers, rMCNightVisionVisibleComponent.Priority, rMCNightVisionVisibleComponent.Transparency));
		}
		_entries.Sort(SortPriority);
		foreach (NightVisionRenderEntry entry in _entries)
		{
			Render(Entity<SpriteComponent, TransformComponent>.op_Implicit(entry.Ent), entry.Map, worldHandle, eyeRot, entry.NightVisionSeeThroughContainers, entry.Transparency);
		}
		EntityUid? localEntity = ((ISharedPlayerManager)_players).LocalEntity;
		if (localEntity.HasValue)
		{
			EntityUid valueOrDefault = localEntity.GetValueOrDefault();
			EntityQueryEnumerator<RMCNightVisionVisibleInViewComponent, SpriteComponent, TransformComponent> val2 = _entity.EntityQueryEnumerator<RMCNightVisionVisibleInViewComponent, SpriteComponent, TransformComponent>();
			EntityUid val3 = default(EntityUid);
			RMCNightVisionVisibleInViewComponent rMCNightVisionVisibleInViewComponent = default(RMCNightVisionVisibleInViewComponent);
			SpriteComponent item4 = default(SpriteComponent);
			TransformComponent item5 = default(TransformComponent);
			while (val2.MoveNext(ref val3, ref rMCNightVisionVisibleInViewComponent, ref item4, ref item5))
			{
				if (_examine.InRangeUnOccluded(val3, valueOrDefault))
				{
					Render(Entity<SpriteComponent, TransformComponent>.op_Implicit((val3, item4, item5)), (eye != null) ? new MapId?(eye.Position.MapId) : ((MapId?)null), worldHandle, eyeRot, seeThroughContainers: false, null);
				}
			}
		}
		Matrix3x2 identity = Matrix3x2.Identity;
		((DrawingHandleBase)worldHandle).SetTransform(ref identity);
		if (nightVisionComponent.Green && base.ScreenTexture != null && args.Viewport.Eye != null)
		{
			_shader.SetParameter("renderScale", args.Viewport.RenderScale * args.Viewport.Eye.Scale);
			_shader.SetParameter("SCREEN_TEXTURE", base.ScreenTexture);
			DrawingHandleWorld worldHandle2 = ((OverlayDrawArgs)(ref args)).WorldHandle;
			((DrawingHandleBase)worldHandle2).UseShader(_shader);
			worldHandle2.DrawRect(ref args.WorldBounds, Color.White, true);
			((DrawingHandleBase)worldHandle2).UseShader((ShaderInstance)null);
		}
	}

	private static int SortPriority(NightVisionRenderEntry x, NightVisionRenderEntry y)
	{
		return x.Priority.CompareTo(y.Priority);
	}

	private void Render(Entity<SpriteComponent, TransformComponent> ent, MapId? map, DrawingHandleWorld handle, Angle eyeRot, bool seeThroughContainers, float? transparency)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		Entity<SpriteComponent, TransformComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		SpriteComponent val3 = default(SpriteComponent);
		TransformComponent val4 = default(TransformComponent);
		val.Deconstruct(ref val2, ref val3, ref val4);
		EntityUid val5 = val2;
		SpriteComponent val6 = val3;
		TransformComponent val7 = val4;
		MapId mapID = val7.MapID;
		MapId? val8 = map;
		if (val8.HasValue && !(mapID != val8.GetValueOrDefault()) && ((seeThroughContainers && !_xenoQuery.HasComp(val5)) || !((SharedContainerSystem)_container).IsEntityOrParentInContainer(val5, (MetaDataComponent)null, val7)))
		{
			ValueTuple<Vector2, Angle> worldPositionRotation = ((SharedTransformSystem)_transform).GetWorldPositionRotation(val7);
			Vector2 item = worldPositionRotation.Item1;
			Angle item2 = worldPositionRotation.Item2;
			Color color = val6.Color;
			if (transparency.HasValue)
			{
				Color color2 = val6.Color;
				Color val9 = Color.White;
				val9 = ((Color)(ref val9)).WithAlpha(transparency.Value);
				Color val10 = (ref color2) * (ref val9);
				_sprite.SetColor(Entity<SpriteComponent>.op_Implicit((val5, val6)), val10);
			}
			_sprite.RenderSprite(Entity<SpriteComponent>.op_Implicit((val5, val6)), handle, eyeRot, item2, item, (Direction?)null);
			if (transparency.HasValue)
			{
				_sprite.SetColor(Entity<SpriteComponent>.op_Implicit((val5, val6)), color);
			}
		}
	}
}
