using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Numerics;
using Content.Shared._RMC14.Mobs;
using Content.Shared._RMC14.Xenonids;
using Content.Shared._RMC14.Xenonids.HiveLeader;
using Content.Shared._RMC14.Xenonids.Pheromones;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Containers;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client._RMC14.Xenonids.Pheromones;

public sealed class XenoPheromonesOverlay : Overlay
{
	[Dependency]
	private IEntityManager _entity;

	[Dependency]
	private IPlayerManager _players;

	[Dependency]
	private IPrototypeManager _prototype;

	[Dependency]
	private IGameTiming _timing;

	private static readonly ImmutableArray<XenoPheromones> AllPheromones = ((IEnumerable<XenoPheromones>)Enum.GetValues<XenoPheromones>()).ToImmutableArray();

	private readonly ContainerSystem _container;

	private readonly SpriteSystem _sprite;

	private readonly TransformSystem _transform;

	private readonly EntityQuery<TransformComponent> _xformQuery;

	private readonly ShaderInstance _shader;

	public override OverlaySpace Space => (OverlaySpace)4;

	public XenoPheromonesOverlay()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<XenoPheromonesOverlay>(this);
		_container = _entity.System<ContainerSystem>();
		_sprite = _entity.System<SpriteSystem>();
		_transform = _entity.System<TransformSystem>();
		_xformQuery = _entity.GetEntityQuery<TransformComponent>();
		_shader = _prototype.Index<ShaderPrototype>(new ProtoId<ShaderPrototype>("unshaded")).Instance();
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Expected O, but got Unknown
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Expected O, but got Unknown
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		if (!_entity.HasComponent<XenoComponent>(((ISharedPlayerManager)_players).LocalEntity) && !_entity.HasComponent<CMGhostXenoHudComponent>(((ISharedPlayerManager)_players).LocalEntity))
		{
			return;
		}
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		IEye eye = args.Viewport.Eye;
		_003F val = ((eye != null) ? eye.Rotation : default(Angle));
		Matrix3x2 scaleMatrix = Matrix3x2.CreateScale(new Vector2(1f, 1f));
		Matrix3x2 rotationMatrix = Matrix3Helpers.CreateRotation(Angle.op_Implicit(-(Angle)val));
		((DrawingHandleBase)worldHandle).UseShader(_shader);
		AllEntityQueryEnumerator<XenoRecoveryPheromonesComponent, SpriteComponent, TransformComponent> val2 = _entity.AllEntityQueryEnumerator<XenoRecoveryPheromonesComponent, SpriteComponent, TransformComponent>();
		EntityUid item = default(EntityUid);
		XenoRecoveryPheromonesComponent xenoRecoveryPheromonesComponent = default(XenoRecoveryPheromonesComponent);
		SpriteComponent item2 = default(SpriteComponent);
		TransformComponent item3 = default(TransformComponent);
		while (val2.MoveNext(ref item, ref xenoRecoveryPheromonesComponent, ref item2, ref item3))
		{
			DrawIcon(Entity<SpriteComponent, TransformComponent>.op_Implicit((item, item2, item3)), in args, xenoRecoveryPheromonesComponent.Icon, scaleMatrix, rotationMatrix);
		}
		AllEntityQueryEnumerator<XenoWardingPheromonesComponent, SpriteComponent, TransformComponent> val3 = _entity.AllEntityQueryEnumerator<XenoWardingPheromonesComponent, SpriteComponent, TransformComponent>();
		EntityUid item4 = default(EntityUid);
		XenoWardingPheromonesComponent xenoWardingPheromonesComponent = default(XenoWardingPheromonesComponent);
		SpriteComponent item5 = default(SpriteComponent);
		TransformComponent item6 = default(TransformComponent);
		while (val3.MoveNext(ref item4, ref xenoWardingPheromonesComponent, ref item5, ref item6))
		{
			DrawIcon(Entity<SpriteComponent, TransformComponent>.op_Implicit((item4, item5, item6)), in args, xenoWardingPheromonesComponent.Icon, scaleMatrix, rotationMatrix);
		}
		AllEntityQueryEnumerator<XenoFrenzyPheromonesComponent, SpriteComponent, TransformComponent> val4 = _entity.AllEntityQueryEnumerator<XenoFrenzyPheromonesComponent, SpriteComponent, TransformComponent>();
		EntityUid item7 = default(EntityUid);
		XenoFrenzyPheromonesComponent xenoFrenzyPheromonesComponent = default(XenoFrenzyPheromonesComponent);
		SpriteComponent item8 = default(SpriteComponent);
		TransformComponent item9 = default(TransformComponent);
		while (val4.MoveNext(ref item7, ref xenoFrenzyPheromonesComponent, ref item8, ref item9))
		{
			DrawIcon(Entity<SpriteComponent, TransformComponent>.op_Implicit((item7, item8, item9)), in args, xenoFrenzyPheromonesComponent.Icon, scaleMatrix, rotationMatrix);
		}
		AllEntityQueryEnumerator<XenoActivePheromonesComponent, SpriteComponent, TransformComponent> val5 = _entity.AllEntityQueryEnumerator<XenoActivePheromonesComponent, SpriteComponent, TransformComponent>();
		EntityUid item10 = default(EntityUid);
		XenoActivePheromonesComponent xenoActivePheromonesComponent = default(XenoActivePheromonesComponent);
		SpriteComponent item11 = default(SpriteComponent);
		TransformComponent item12 = default(TransformComponent);
		while (val5.MoveNext(ref item10, ref xenoActivePheromonesComponent, ref item11, ref item12))
		{
			XenoPheromones pheromones = xenoActivePheromonesComponent.Pheromones;
			string text = "aura_" + pheromones.ToString().ToLowerInvariant();
			Rsi icon = new Rsi(new ResPath("/Textures/_RMC14/Interface/xeno_pheromones_hud.rsi"), text);
			DrawIcon(Entity<SpriteComponent, TransformComponent>.op_Implicit((item10, item11, item12)), in args, (SpriteSpecifier)(object)icon, scaleMatrix, rotationMatrix);
		}
		AllEntityQueryEnumerator<HiveLeaderComponent, SpriteComponent, TransformComponent> val6 = _entity.AllEntityQueryEnumerator<HiveLeaderComponent, SpriteComponent, TransformComponent>();
		EntityUid val7 = default(EntityUid);
		HiveLeaderComponent hiveLeaderComponent = default(HiveLeaderComponent);
		SpriteComponent item13 = default(SpriteComponent);
		TransformComponent item14 = default(TransformComponent);
		BaseContainer val8 = default(BaseContainer);
		EntityUid? val9 = default(EntityUid?);
		XenoActivePheromonesComponent xenoActivePheromonesComponent2 = default(XenoActivePheromonesComponent);
		while (val6.MoveNext(ref val7, ref hiveLeaderComponent, ref item13, ref item14))
		{
			if (((SharedContainerSystem)_container).TryGetContainer(val7, hiveLeaderComponent.PheromonesContainerId, ref val8, (ContainerManagerComponent)null) && Extensions.TryFirstOrNull<EntityUid>((IEnumerable<EntityUid>)val8.ContainedEntities, ref val9) && _entity.TryGetComponent<XenoActivePheromonesComponent>(val9, ref xenoActivePheromonesComponent2))
			{
				XenoPheromones pheromones2 = xenoActivePheromonesComponent2.Pheromones;
				string text2 = "aura_" + pheromones2.ToString().ToLowerInvariant();
				Rsi icon2 = new Rsi(new ResPath("/Textures/_RMC14/Interface/xeno_pheromones_hud.rsi"), text2);
				DrawIcon(Entity<SpriteComponent, TransformComponent>.op_Implicit((val7, item13, item14)), in args, (SpriteSpecifier)(object)icon2, scaleMatrix, rotationMatrix);
			}
		}
		((DrawingHandleBase)worldHandle).UseShader((ShaderInstance)null);
	}

	private void DrawIcon(Entity<SpriteComponent, TransformComponent> ent, in OverlayDrawArgs args, SpriteSpecifier icon, Matrix3x2 scaleMatrix, Matrix3x2 rotationMatrix)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		Entity<SpriteComponent, TransformComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		SpriteComponent val3 = default(SpriteComponent);
		TransformComponent val4 = default(TransformComponent);
		val.Deconstruct(ref val2, ref val3, ref val4);
		SpriteComponent val5 = val3;
		TransformComponent val6 = val4;
		if (!(val6.MapID != args.MapId))
		{
			Box2 localBounds = _sprite.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((ent.Owner, val5)));
			Vector2 worldPosition = ((SharedTransformSystem)_transform).GetWorldPosition(val6, _xformQuery);
			Box2 val7 = ((Box2)(ref localBounds)).Translated(worldPosition);
			if (((Box2)(ref val7)).Intersects(ref args.WorldAABB))
			{
				DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
				Matrix3x2 value = Matrix3x2.CreateTranslation(worldPosition);
				Matrix3x2 value2 = Matrix3x2.Multiply(scaleMatrix, value);
				Matrix3x2 matrix3x = Matrix3x2.Multiply(rotationMatrix, value2);
				((DrawingHandleBase)worldHandle).SetTransform(ref matrix3x);
				Texture frame = _sprite.GetFrame(icon, _timing.CurTime, true);
				float y = (((Box2)(ref localBounds)).Height + val5.Offset.Y) / 2f - (float)frame.Height / 32f * ((Box2)(ref localBounds)).Height;
				float x = (((Box2)(ref localBounds)).Width + val5.Offset.X) / 2f - (float)frame.Width / 32f - 0.25f;
				Vector2 vector = new Vector2(x, y);
				((DrawingHandleBase)worldHandle).DrawTexture(frame, vector, (Color?)null);
			}
		}
	}
}
