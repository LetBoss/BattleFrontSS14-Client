using System;
using System.Numerics;
using Content.Client._RMC14.Medical.HUD;
using Content.Client._RMC14.NightVision;
using Content.Shared._RMC14.Mobs;
using Content.Shared._RMC14.Shields;
using Content.Shared._RMC14.Slow;
using Content.Shared._RMC14.Stealth;
using Content.Shared._RMC14.Synth;
using Content.Shared._RMC14.Xenonids;
using Content.Shared._RMC14.Xenonids.Energy;
using Content.Shared._RMC14.Xenonids.Finesse;
using Content.Shared._RMC14.Xenonids.Maturing;
using Content.Shared._RMC14.Xenonids.Parasite;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared._RMC14.Xenonids.Projectile.Spit.Stacks;
using Content.Shared._RMC14.Xenonids.Rank;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Content.Shared.Ghost;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Rounding;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.ResourceManagement;
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

namespace Content.Client._RMC14.Xenonids.Hud;

public sealed class XenoHudOverlay : Overlay
{
	[Dependency]
	private IEntityManager _entity;

	[Dependency]
	private IOverlayManager _overlay;

	[Dependency]
	private IPlayerManager _players;

	[Dependency]
	private IPrototypeManager _prototype;

	[Dependency]
	private IResourceCache _resourceCache;

	[Dependency]
	private IGameTiming _timing;

	private readonly ContainerSystem _container;

	private readonly CMHealthIconsSystem _healthIcons;

	private readonly MobStateSystem _mobState;

	private readonly MobThresholdSystem _mobThresholds;

	private readonly SpriteSystem _sprite;

	private readonly TransformSystem _transform;

	private readonly EntityQuery<DamageableComponent> _damageableQuery;

	private readonly EntityQuery<XenoParasiteComponent> _xenoParasiteQuery;

	private readonly EntityQuery<MobStateComponent> _mobStateQuery;

	private readonly EntityQuery<MobThresholdsComponent> _mobThresholdsQuery;

	private readonly EntityQuery<XenoEnergyComponent> _xenoEnergyQuery;

	private readonly EntityQuery<XenoMaturingComponent> _xenoMaturingQuery;

	private readonly EntityQuery<XenoPlasmaComponent> _xenoPlasmaQuery;

	private readonly EntityQuery<TransformComponent> _xformQuery;

	private readonly EntityQuery<XenoShieldComponent> _xenoShieldQuery;

	private readonly EntityQuery<EntityActiveInvisibleComponent> _invisQuery;

	private readonly EntityQuery<XenoComponent> _xenoQuery;

	private readonly ShaderInstance _shader;

	private readonly ResPath _rsiPath = new ResPath("/Textures/_RMC14/Interface/xeno_hud.rsi");

	private readonly ResPath _rsiPathSlow = new ResPath("/Textures/_RMC14/Effects/xeno_stomp.rsi");

	private readonly ResPath _rsiPathFreeze = new ResPath("/Textures/_RMC14/Effects/xeno_freeze.rsi");

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

	public XenoHudOverlay()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<XenoHudOverlay>(this);
		_container = _entity.System<ContainerSystem>();
		_healthIcons = _entity.System<CMHealthIconsSystem>();
		_mobState = _entity.System<MobStateSystem>();
		_mobThresholds = _entity.System<MobThresholdSystem>();
		_sprite = _entity.System<SpriteSystem>();
		_transform = _entity.System<TransformSystem>();
		_damageableQuery = _entity.GetEntityQuery<DamageableComponent>();
		_xenoParasiteQuery = _entity.GetEntityQuery<XenoParasiteComponent>();
		_mobStateQuery = _entity.GetEntityQuery<MobStateComponent>();
		_mobThresholdsQuery = _entity.GetEntityQuery<MobThresholdsComponent>();
		_xenoEnergyQuery = _entity.GetEntityQuery<XenoEnergyComponent>();
		_xenoMaturingQuery = _entity.GetEntityQuery<XenoMaturingComponent>();
		_xenoPlasmaQuery = _entity.GetEntityQuery<XenoPlasmaComponent>();
		_xformQuery = _entity.GetEntityQuery<TransformComponent>();
		_xenoShieldQuery = _entity.GetEntityQuery<XenoShieldComponent>();
		_invisQuery = _entity.GetEntityQuery<EntityActiveInvisibleComponent>();
		_xenoQuery = _entity.GetEntityQuery<XenoComponent>();
		_shader = _prototype.Index<ShaderPrototype>(new ProtoId<ShaderPrototype>("unshaded")).Instance();
		((Overlay)this).ZIndex = 1;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		GhostComponent ghostComponent = default(GhostComponent);
		bool flag = _entity.TryGetComponent<GhostComponent>(((ISharedPlayerManager)_players).LocalEntity, ref ghostComponent) && ghostComponent.CanGhostInteract;
		bool flag2 = _entity.HasComponent<XenoComponent>(((ISharedPlayerManager)_players).LocalEntity);
		bool flag3 = false;
		if (!_entity.HasComponent<CMGhostXenoHudComponent>(((ISharedPlayerManager)_players).LocalEntity))
		{
			if (!flag2 && !flag)
			{
				return;
			}
		}
		else
		{
			if (_entity.HasComponent<CMGhostXenoHudComponent>(((ISharedPlayerManager)_players).LocalEntity))
			{
				flag3 = true;
			}
			flag2 = true;
		}
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		IEye eye = args.Viewport.Eye;
		_003F val = ((eye != null) ? eye.Rotation : default(Angle));
		Matrix3x2 scaleMatrix = Matrix3x2.CreateScale(new Vector2(1f, 1f));
		Matrix3x2 rotationMatrix = Matrix3Helpers.CreateRotation(Angle.op_Implicit(-(Angle)val));
		((DrawingHandleBase)worldHandle).UseShader(_shader);
		if (flag2)
		{
			DrawBars(in args, scaleMatrix, rotationMatrix);
			if (!flag3)
			{
				DrawDeadIcon(in args, scaleMatrix, rotationMatrix);
			}
			DrawAcidStacks(in args, scaleMatrix, rotationMatrix);
			DrawMarkedIcons(in args, scaleMatrix, rotationMatrix);
			DrawRank(in args, scaleMatrix, rotationMatrix);
			DrawSlow(in args, scaleMatrix, rotationMatrix);
			DrawStun(in args, scaleMatrix, rotationMatrix);
		}
		if (flag2 || flag)
		{
			DrawInfectedIcon(in args, scaleMatrix, rotationMatrix);
			DrawSynthIcon(in args, scaleMatrix, rotationMatrix);
		}
		((DrawingHandleBase)worldHandle).UseShader((ShaderInstance)null);
		Matrix3x2 identity = Matrix3x2.Identity;
		((DrawingHandleBase)worldHandle).SetTransform(ref identity);
	}

	private void DrawBars(in OverlayDrawArgs args, Matrix3x2 scaleMatrix, Matrix3x2 rotationMatrix)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		AllEntityQueryEnumerator<XenoComponent, SpriteComponent, TransformComponent> val = _entity.AllEntityQueryEnumerator<XenoComponent, SpriteComponent, TransformComponent>();
		EntityUid val2 = default(EntityUid);
		XenoComponent item = default(XenoComponent);
		SpriteComponent val3 = default(SpriteComponent);
		TransformComponent val4 = default(TransformComponent);
		MobStateComponent mobStateComponent = default(MobStateComponent);
		while (val.MoveNext(ref val2, ref item, ref val3, ref val4))
		{
			if (val4.MapID != args.MapId || ((SharedContainerSystem)_container).IsEntityOrParentInContainer(val2, (MetaDataComponent)null, val4))
			{
				continue;
			}
			Box2 localBounds = _sprite.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((val2, val3)));
			Vector2 worldPosition = ((SharedTransformSystem)_transform).GetWorldPosition(val4, _xformQuery);
			Box2 val5 = ((Box2)(ref localBounds)).Translated(worldPosition);
			if (((Box2)(ref val5)).Intersects(ref args.WorldAABB))
			{
				Matrix3x2 value = Matrix3x2.CreateTranslation(worldPosition);
				Matrix3x2 value2 = Matrix3x2.Multiply(scaleMatrix, value);
				Matrix3x2 matrix3x = Matrix3x2.Multiply(rotationMatrix, value2);
				((DrawingHandleBase)worldHandle).SetTransform(ref matrix3x);
				if (!_mobStateQuery.TryComp(val2, ref mobStateComponent) || !_mobState.IsDead(val2, mobStateComponent))
				{
					UpdateHealth(Entity<XenoComponent, SpriteComponent, MobStateComponent>.op_Implicit((val2, item, val3, mobStateComponent)), worldHandle);
					UpdatePlasma(Entity<XenoComponent, SpriteComponent>.op_Implicit((val2, item, val3)), worldHandle);
					UpdateShields(Entity<XenoComponent, SpriteComponent>.op_Implicit((val2, item, val3)), worldHandle);
					UpdateEnergy(Entity<XenoComponent, SpriteComponent>.op_Implicit((val2, item, val3)), worldHandle);
				}
			}
		}
	}

	private void DrawDeadIcon(in OverlayDrawArgs args, Matrix3x2 scaleMatrix, Matrix3x2 rotationMatrix)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		SpriteSpecifier icon = _healthIcons.GetDeadIcon().Icon;
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		AllEntityQueryEnumerator<MobStateComponent, SpriteComponent, TransformComponent> val = _entity.AllEntityQueryEnumerator<MobStateComponent, SpriteComponent, TransformComponent>();
		EntityUid val2 = default(EntityUid);
		MobStateComponent mobStateComponent = default(MobStateComponent);
		SpriteComponent val3 = default(SpriteComponent);
		TransformComponent val4 = default(TransformComponent);
		while (val.MoveNext(ref val2, ref mobStateComponent, ref val3, ref val4))
		{
			if (!(val4.MapID != args.MapId) && mobStateComponent.CurrentState == MobState.Dead && !((SharedContainerSystem)_container).IsEntityOrParentInContainer(val2, (MetaDataComponent)null, val4) && !_xenoParasiteQuery.HasComp(val2) && !_invisQuery.HasComp(val2))
			{
				Box2 localBounds = _sprite.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((val2, val3)));
				Vector2 worldPosition = ((SharedTransformSystem)_transform).GetWorldPosition(val4, _xformQuery);
				Box2 val5 = ((Box2)(ref localBounds)).Translated(worldPosition);
				if (((Box2)(ref val5)).Intersects(ref args.WorldAABB))
				{
					Matrix3x2 value = Matrix3x2.CreateTranslation(worldPosition);
					Matrix3x2 value2 = Matrix3x2.Multiply(scaleMatrix, value);
					Matrix3x2 matrix3x = Matrix3x2.Multiply(rotationMatrix, value2);
					((DrawingHandleBase)worldHandle).SetTransform(ref matrix3x);
					Texture frame = _sprite.GetFrame(icon, _timing.CurTime, true);
					float y = (((Box2)(ref localBounds)).Height + val3.Offset.Y) / 2f - (float)frame.Height / 32f * ((Box2)(ref localBounds)).Height;
					float x = (((Box2)(ref localBounds)).Width + val3.Offset.X) / 2f - (float)frame.Width / 32f * ((Box2)(ref localBounds)).Width;
					Vector2 vector = new Vector2(x, y);
					((DrawingHandleBase)worldHandle).DrawTexture(frame, vector, (Color?)null);
				}
			}
		}
	}

	private void DrawAcidStacks(in OverlayDrawArgs args, Matrix3x2 scaleMatrix, Matrix3x2 rotationMatrix)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Expected O, but got Unknown
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		AllEntityQueryEnumerator<VictimXenoAcidStacksComponent, SpriteComponent, TransformComponent> val = _entity.AllEntityQueryEnumerator<VictimXenoAcidStacksComponent, SpriteComponent, TransformComponent>();
		EntityUid val2 = default(EntityUid);
		VictimXenoAcidStacksComponent victimXenoAcidStacksComponent = default(VictimXenoAcidStacksComponent);
		SpriteComponent val3 = default(SpriteComponent);
		TransformComponent val4 = default(TransformComponent);
		while (val.MoveNext(ref val2, ref victimXenoAcidStacksComponent, ref val3, ref val4))
		{
			if (!(val4.MapID != args.MapId) && !((SharedContainerSystem)_container).IsEntityOrParentInContainer(val2, (MetaDataComponent)null, val4) && !_invisQuery.HasComp(val2))
			{
				Box2 localBounds = _sprite.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((val2, val3)));
				Vector2 worldPosition = ((SharedTransformSystem)_transform).GetWorldPosition(val4, _xformQuery);
				Box2 val5 = ((Box2)(ref localBounds)).Translated(worldPosition);
				if (((Box2)(ref val5)).Intersects(ref args.WorldAABB))
				{
					Matrix3x2 value = Matrix3x2.CreateTranslation(worldPosition);
					Matrix3x2 value2 = Matrix3x2.Multiply(scaleMatrix, value);
					Matrix3x2 matrix3x = Matrix3x2.Multiply(rotationMatrix, value2);
					((DrawingHandleBase)worldHandle).SetTransform(ref matrix3x);
					int value3 = Math.Clamp(victimXenoAcidStacksComponent.Current, 0, 4);
					Rsi val6 = new Rsi(_rsiPath, $"acid_stacks{value3}");
					Texture frame = _sprite.GetFrame((SpriteSpecifier)(object)val6, _timing.CurTime, true);
					float y = (((Box2)(ref localBounds)).Height + val3.Offset.Y) / 2f - (float)frame.Height / 32f * ((Box2)(ref localBounds)).Height;
					float x = (((Box2)(ref localBounds)).Width + val3.Offset.X) / 2f - (float)frame.Width / 32f * ((Box2)(ref localBounds)).Width;
					Vector2 vector = new Vector2(x, y);
					((DrawingHandleBase)worldHandle).DrawTexture(frame, vector, (Color?)null);
				}
			}
		}
	}

	private void DrawRank(in OverlayDrawArgs args, Matrix3x2 scaleMatrix, Matrix3x2 rotationMatrix)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Expected O, but got Unknown
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		EntityQueryEnumerator<XenoRankComponent, SpriteComponent, TransformComponent> val = _entity.EntityQueryEnumerator<XenoRankComponent, SpriteComponent, TransformComponent>();
		EntityUid val2 = default(EntityUid);
		XenoRankComponent xenoRankComponent = default(XenoRankComponent);
		SpriteComponent val3 = default(SpriteComponent);
		TransformComponent val4 = default(TransformComponent);
		while (val.MoveNext(ref val2, ref xenoRankComponent, ref val3, ref val4))
		{
			if (xenoRankComponent.Rank >= 2 && xenoRankComponent.Rank <= 6 && !_xenoMaturingQuery.HasComp(val2) && !(val4.MapID != args.MapId) && !((SharedContainerSystem)_container).IsEntityOrParentInContainer(val2, (MetaDataComponent)null, val4) && !_invisQuery.HasComp(val2))
			{
				Box2 localBounds = _sprite.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((val2, val3)));
				Vector2 worldPosition = ((SharedTransformSystem)_transform).GetWorldPosition(val4, _xformQuery);
				Box2 val5 = ((Box2)(ref localBounds)).Translated(worldPosition);
				if (((Box2)(ref val5)).Intersects(ref args.WorldAABB))
				{
					Matrix3x2 value = Matrix3x2.CreateTranslation(worldPosition);
					Matrix3x2 value2 = Matrix3x2.Multiply(scaleMatrix, value);
					Matrix3x2 matrix3x = Matrix3x2.Multiply(rotationMatrix, value2);
					((DrawingHandleBase)worldHandle).SetTransform(ref matrix3x);
					Rsi val6 = new Rsi(_rsiPath, $"hudxenoupgrade{xenoRankComponent.Rank}");
					Texture frame = _sprite.GetFrame((SpriteSpecifier)(object)val6, _timing.CurTime, true);
					float y = (((Box2)(ref localBounds)).Height + val3.Offset.Y) / 2f - (float)frame.Height / 32f * ((Box2)(ref localBounds)).Height;
					float x = (((Box2)(ref localBounds)).Width + val3.Offset.X) / 2f - (float)frame.Width / 32f * ((Box2)(ref localBounds)).Width;
					Vector2 vector = new Vector2(x, y);
					((DrawingHandleBase)worldHandle).DrawTexture(frame, vector, (Color?)null);
				}
			}
		}
	}

	private void DrawMarkedIcons(in OverlayDrawArgs args, Matrix3x2 scaleMatrix, Matrix3x2 rotationMatrix)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Expected O, but got Unknown
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		AllEntityQueryEnumerator<XenoMarkedComponent, SpriteComponent, TransformComponent> val = _entity.AllEntityQueryEnumerator<XenoMarkedComponent, SpriteComponent, TransformComponent>();
		EntityUid val2 = default(EntityUid);
		XenoMarkedComponent xenoMarkedComponent = default(XenoMarkedComponent);
		SpriteComponent val3 = default(SpriteComponent);
		TransformComponent val4 = default(TransformComponent);
		while (val.MoveNext(ref val2, ref xenoMarkedComponent, ref val3, ref val4))
		{
			if (!(val4.MapID != args.MapId) && !((SharedContainerSystem)_container).IsEntityOrParentInContainer(val2, (MetaDataComponent)null, val4) && !_invisQuery.HasComp(val2))
			{
				Box2 localBounds = _sprite.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((val2, val3)));
				Vector2 worldPosition = ((SharedTransformSystem)_transform).GetWorldPosition(val4, _xformQuery);
				Box2 val5 = ((Box2)(ref localBounds)).Translated(worldPosition);
				if (((Box2)(ref val5)).Intersects(ref args.WorldAABB))
				{
					Matrix3x2 value = Matrix3x2.CreateTranslation(worldPosition);
					Matrix3x2 value2 = Matrix3x2.Multiply(scaleMatrix, value);
					Matrix3x2 matrix3x = Matrix3x2.Multiply(rotationMatrix, value2);
					((DrawingHandleBase)worldHandle).SetTransform(ref matrix3x);
					Rsi val6 = new Rsi(_rsiPath, "prae_tag");
					Texture frame = _sprite.GetFrame((SpriteSpecifier)(object)val6, _timing.CurTime - xenoMarkedComponent.TimeAdded, false);
					float y = (((Box2)(ref localBounds)).Height + val3.Offset.Y) / 2f - (float)frame.Height / 32f * ((Box2)(ref localBounds)).Height;
					float x = (((Box2)(ref localBounds)).Width + val3.Offset.X) / 2f - (float)frame.Width / 32f * ((Box2)(ref localBounds)).Width;
					Vector2 vector = new Vector2(x, y);
					((DrawingHandleBase)worldHandle).DrawTexture(frame, vector, (Color?)null);
				}
			}
		}
	}

	private void DrawSlow(in OverlayDrawArgs args, Matrix3x2 scaleMatrix, Matrix3x2 rotationMatrix)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Expected O, but got Unknown
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		AllEntityQueryEnumerator<XenoSlowVisualsComponent, SpriteComponent, TransformComponent> val = _entity.AllEntityQueryEnumerator<XenoSlowVisualsComponent, SpriteComponent, TransformComponent>();
		EntityUid val2 = default(EntityUid);
		XenoSlowVisualsComponent xenoSlowVisualsComponent = default(XenoSlowVisualsComponent);
		SpriteComponent val3 = default(SpriteComponent);
		TransformComponent val4 = default(TransformComponent);
		while (val.MoveNext(ref val2, ref xenoSlowVisualsComponent, ref val3, ref val4))
		{
			if (!(val4.MapID != args.MapId) && !((SharedContainerSystem)_container).IsEntityOrParentInContainer(val2, (MetaDataComponent)null, val4) && !_invisQuery.HasComp(val2) && !_xenoQuery.HasComp(val2))
			{
				Box2 localBounds = _sprite.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((val2, val3)));
				Vector2 worldPosition = ((SharedTransformSystem)_transform).GetWorldPosition(val4, _xformQuery);
				Box2 val5 = ((Box2)(ref localBounds)).Translated(worldPosition);
				if (((Box2)(ref val5)).Intersects(ref args.WorldAABB))
				{
					Matrix3x2 value = Matrix3x2.CreateTranslation(worldPosition);
					Matrix3x2 value2 = Matrix3x2.Multiply(scaleMatrix, value);
					Matrix3x2 matrix3x = Matrix3x2.Multiply(rotationMatrix, value2);
					((DrawingHandleBase)worldHandle).SetTransform(ref matrix3x);
					Rsi val6 = new Rsi(_rsiPathSlow, "stomp");
					Texture frame = _sprite.GetFrame((SpriteSpecifier)(object)val6, _timing.CurTime, true);
					float y = (((Box2)(ref localBounds)).Height + val3.Offset.Y) / 2f - (float)frame.Height / 32f * ((Box2)(ref localBounds)).Height;
					float x = (((Box2)(ref localBounds)).Width + val3.Offset.X) / 2f - (float)frame.Width / 32f * ((Box2)(ref localBounds)).Width;
					Vector2 vector = new Vector2(x, y);
					((DrawingHandleBase)worldHandle).DrawTexture(frame, vector, (Color?)null);
				}
			}
		}
	}

	private void DrawStun(in OverlayDrawArgs args, Matrix3x2 scaleMatrix, Matrix3x2 rotationMatrix)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Expected O, but got Unknown
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		AllEntityQueryEnumerator<XenoImmobileVisualsComponent, SpriteComponent, TransformComponent> val = _entity.AllEntityQueryEnumerator<XenoImmobileVisualsComponent, SpriteComponent, TransformComponent>();
		EntityUid val2 = default(EntityUid);
		XenoImmobileVisualsComponent xenoImmobileVisualsComponent = default(XenoImmobileVisualsComponent);
		SpriteComponent val3 = default(SpriteComponent);
		TransformComponent val4 = default(TransformComponent);
		while (val.MoveNext(ref val2, ref xenoImmobileVisualsComponent, ref val3, ref val4))
		{
			if (!(val4.MapID != args.MapId) && !((SharedContainerSystem)_container).IsEntityOrParentInContainer(val2, (MetaDataComponent)null, val4) && !_invisQuery.HasComp(val2) && !_xenoQuery.HasComp(val2))
			{
				Box2 localBounds = _sprite.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((val2, val3)));
				Vector2 worldPosition = ((SharedTransformSystem)_transform).GetWorldPosition(val4, _xformQuery);
				Box2 val5 = ((Box2)(ref localBounds)).Translated(worldPosition);
				if (((Box2)(ref val5)).Intersects(ref args.WorldAABB))
				{
					Matrix3x2 value = Matrix3x2.CreateTranslation(worldPosition);
					Matrix3x2 value2 = Matrix3x2.Multiply(scaleMatrix, value);
					Matrix3x2 matrix3x = Matrix3x2.Multiply(rotationMatrix, value2);
					((DrawingHandleBase)worldHandle).SetTransform(ref matrix3x);
					Rsi val6 = new Rsi(_rsiPathFreeze, "freeze");
					Texture frame = _sprite.GetFrame((SpriteSpecifier)(object)val6, _timing.CurTime, true);
					float y = (((Box2)(ref localBounds)).Height + val3.Offset.Y) / 2f - (float)frame.Height / 32f * ((Box2)(ref localBounds)).Height;
					float x = (((Box2)(ref localBounds)).Width + val3.Offset.X) / 2f - (float)frame.Width / 32f * ((Box2)(ref localBounds)).Width;
					Vector2 vector = new Vector2(x, y);
					((DrawingHandleBase)worldHandle).DrawTexture(frame, vector, (Color?)null);
				}
			}
		}
	}

	private void DrawInfectedIcon(in OverlayDrawArgs args, Matrix3x2 scaleMatrix, Matrix3x2 rotationMatrix)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		AllEntityQueryEnumerator<VictimInfectedComponent, SpriteComponent, TransformComponent> val = _entity.AllEntityQueryEnumerator<VictimInfectedComponent, SpriteComponent, TransformComponent>();
		EntityUid val2 = default(EntityUid);
		VictimInfectedComponent victimInfectedComponent = default(VictimInfectedComponent);
		SpriteComponent val3 = default(SpriteComponent);
		TransformComponent val4 = default(TransformComponent);
		while (val.MoveNext(ref val2, ref victimInfectedComponent, ref val3, ref val4))
		{
			if (!(val4.MapID != args.MapId) && !((SharedContainerSystem)_container).IsEntityOrParentInContainer(val2, (MetaDataComponent)null, val4) && !_invisQuery.HasComp(val2))
			{
				Box2 localBounds = _sprite.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((val2, val3)));
				Vector2 worldPosition = ((SharedTransformSystem)_transform).GetWorldPosition(val4, _xformQuery);
				Box2 val5 = ((Box2)(ref localBounds)).Translated(worldPosition);
				if (((Box2)(ref val5)).Intersects(ref args.WorldAABB))
				{
					Matrix3x2 value = Matrix3x2.CreateTranslation(worldPosition);
					Matrix3x2 value2 = Matrix3x2.Multiply(scaleMatrix, value);
					Matrix3x2 matrix3x = Matrix3x2.Multiply(rotationMatrix, value2);
					((DrawingHandleBase)worldHandle).SetTransform(ref matrix3x);
					int num = Math.Min(victimInfectedComponent.CurrentStage, victimInfectedComponent.InfectedIcons.Length - 1);
					SpriteSpecifier val6 = victimInfectedComponent.InfectedIcons[num];
					Texture frame = _sprite.GetFrame(val6, _timing.CurTime, true);
					float y = (((Box2)(ref localBounds)).Height + val3.Offset.Y) / 2f - (float)frame.Height / 32f * ((Box2)(ref localBounds)).Height;
					float x = (((Box2)(ref localBounds)).Width + val3.Offset.X) / 2f - (float)frame.Width / 32f * ((Box2)(ref localBounds)).Width;
					Vector2 vector = new Vector2(x, y);
					((DrawingHandleBase)worldHandle).DrawTexture(frame, vector, (Color?)null);
				}
			}
		}
	}

	private void DrawSynthIcon(in OverlayDrawArgs args, Matrix3x2 scaleMatrix, Matrix3x2 rotationMatrix)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Expected O, but got Unknown
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		AllEntityQueryEnumerator<SynthComponent, SpriteComponent, TransformComponent> val = _entity.AllEntityQueryEnumerator<SynthComponent, SpriteComponent, TransformComponent>();
		EntityUid val2 = default(EntityUid);
		SynthComponent synthComponent = default(SynthComponent);
		SpriteComponent val3 = default(SpriteComponent);
		TransformComponent val4 = default(TransformComponent);
		while (val.MoveNext(ref val2, ref synthComponent, ref val3, ref val4))
		{
			if (!(val4.MapID != args.MapId) && !((SharedContainerSystem)_container).IsEntityOrParentInContainer(val2, (MetaDataComponent)null, val4) && !_invisQuery.HasComp(val2))
			{
				Box2 localBounds = _sprite.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((val2, val3)));
				Vector2 worldPosition = ((SharedTransformSystem)_transform).GetWorldPosition(val4, _xformQuery);
				Box2 val5 = ((Box2)(ref localBounds)).Translated(worldPosition);
				if (((Box2)(ref val5)).Intersects(ref args.WorldAABB))
				{
					Matrix3x2 value = Matrix3x2.CreateTranslation(worldPosition);
					Matrix3x2 value2 = Matrix3x2.Multiply(scaleMatrix, value);
					Matrix3x2 matrix3x = Matrix3x2.Multiply(rotationMatrix, value2);
					((DrawingHandleBase)worldHandle).SetTransform(ref matrix3x);
					Rsi val6 = new Rsi(_rsiPath, "fake_tall");
					Texture frame = _sprite.GetFrame((SpriteSpecifier)(object)val6, _timing.CurTime, true);
					float y = (((Box2)(ref localBounds)).Height + val3.Offset.Y) / 2f - (float)frame.Height / 32f * ((Box2)(ref localBounds)).Height;
					float x = (((Box2)(ref localBounds)).Width + val3.Offset.X) / 2f - (float)frame.Width / 32f * ((Box2)(ref localBounds)).Width;
					Vector2 vector = new Vector2(x, y);
					((DrawingHandleBase)worldHandle).DrawTexture(frame, vector, (Color?)null);
				}
			}
		}
	}

	private void UpdateHealth(Entity<XenoComponent, SpriteComponent, MobStateComponent?> ent, DrawingHandleWorld handle)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Expected O, but got Unknown
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		Entity<XenoComponent, SpriteComponent, MobStateComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		XenoComponent xenoComponent = default(XenoComponent);
		SpriteComponent val3 = default(SpriteComponent);
		MobStateComponent mobStateComponent = default(MobStateComponent);
		val.Deconstruct(ref val2, ref xenoComponent, ref val3, ref mobStateComponent);
		EntityUid val4 = val2;
		XenoComponent xenoComponent2 = xenoComponent;
		SpriteComponent val5 = val3;
		MobStateComponent component = mobStateComponent;
		DamageableComponent damageableComponent = default(DamageableComponent);
		if (!_damageableQuery.TryComp(val4, ref damageableComponent))
		{
			return;
		}
		FixedPoint2 totalDamage = damageableComponent.TotalDamage;
		FixedPoint2? threshold = null;
		FixedPoint2? threshold2 = null;
		MobThresholdsComponent thresholdComponent = default(MobThresholdsComponent);
		if (_mobThresholdsQuery.TryComp(val4, ref thresholdComponent))
		{
			_mobThresholds.TryGetThresholdForState(val4, MobState.Critical, out threshold, thresholdComponent);
			_mobThresholds.TryGetDeadThreshold(val4, out threshold2, thresholdComponent);
		}
		if (_mobState.IsCritical(val4, component))
		{
			goto IL_00d4;
		}
		FixedPoint2? fixedPoint;
		if (_mobState.IsAlive(val4) && threshold.HasValue)
		{
			FixedPoint2 totalDamage2 = damageableComponent.TotalDamage;
			fixedPoint = threshold;
			if (totalDamage2 > fixedPoint)
			{
				goto IL_00d4;
			}
		}
		fixedPoint = threshold;
		if (!fixedPoint.HasValue)
		{
			threshold = threshold2;
		}
		if (!threshold.HasValue)
		{
			return;
		}
		int num = ContentHelpers.RoundToLevels((threshold - totalDamage).Value.Double(), threshold.Value.Double(), 11);
		string text = ((num > 0) ? $"{num * 10}" : "0");
		string text2 = "xenohealth" + text;
		goto IL_0213;
		IL_0213:
		Rsi val6 = new Rsi(_rsiPath, text2);
		State val7 = default(State);
		if (_resourceCache.GetResource<RSIResource>(val6.RsiPath, true).RSI.TryGetState(StateId.op_Implicit(val6.RsiState), ref val7))
		{
			Texture frame = _sprite.GetFrame((SpriteSpecifier)(object)val6, _timing.CurTime, true);
			Box2 localBounds = _sprite.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((ent.Owner, val5)));
			float y = (((Box2)(ref localBounds)).Height + val5.Offset.Y) / 2f - (float)frame.Height / 32f * ((Box2)(ref localBounds)).Height + xenoComponent2.HudOffset.Y;
			float x = (((Box2)(ref localBounds)).Width + val5.Offset.X) / 2f - (float)frame.Width / 32f * ((Box2)(ref localBounds)).Width + xenoComponent2.HudOffset.X;
			Vector2 vector = new Vector2(x, y);
			((DrawingHandleBase)handle).DrawTexture(frame, vector, (Color?)null);
		}
		return;
		IL_00d4:
		if (threshold.HasValue)
		{
			FixedPoint2 valueOrDefault = threshold.GetValueOrDefault();
			if (threshold2.HasValue)
			{
				FixedPoint2 valueOrDefault2 = threshold2.GetValueOrDefault();
				valueOrDefault2 -= valueOrDefault;
				int num2 = ContentHelpers.RoundToLevels((totalDamage - valueOrDefault).Double(), valueOrDefault2.Double(), 11);
				string text3 = ((num2 > 0) ? $"{num2 * 10}" : "1");
				text2 = "xenohealth-" + text3;
				goto IL_0213;
			}
		}
	}

	private void UpdatePlasma(Entity<XenoComponent, SpriteComponent> ent, DrawingHandleWorld handle)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		Entity<XenoComponent, SpriteComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		XenoComponent xenoComponent = default(XenoComponent);
		SpriteComponent val3 = default(SpriteComponent);
		val.Deconstruct(ref val2, ref xenoComponent, ref val3);
		EntityUid val4 = val2;
		XenoComponent xenoComponent2 = xenoComponent;
		SpriteComponent val5 = val3;
		XenoPlasmaComponent xenoPlasmaComponent = default(XenoPlasmaComponent);
		if (_xenoPlasmaQuery.TryComp(val4, ref xenoPlasmaComponent) && xenoPlasmaComponent.MaxPlasma != 0)
		{
			FixedPoint2 plasma = xenoPlasmaComponent.Plasma;
			int maxPlasma = xenoPlasmaComponent.MaxPlasma;
			int num = ContentHelpers.RoundToLevels(plasma.Double(), maxPlasma, 11);
			string text = ((num > 0) ? $"{num * 10}" : "0");
			string text2 = "plasma" + text;
			Rsi val6 = new Rsi(new ResPath("/Textures/_RMC14/Interface/xeno_hud.rsi"), text2);
			Texture frame = _sprite.GetFrame((SpriteSpecifier)(object)val6, _timing.CurTime, true);
			Box2 localBounds = _sprite.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((ent.Owner, val5)));
			float y = (((Box2)(ref localBounds)).Height + val5.Offset.Y) / 2f - (float)frame.Height / 32f * ((Box2)(ref localBounds)).Height + xenoComponent2.HudOffset.Y;
			float x = (((Box2)(ref localBounds)).Width + val5.Offset.X) / 2f - (float)frame.Width / 32f * ((Box2)(ref localBounds)).Width + xenoComponent2.HudOffset.X;
			Vector2 vector = new Vector2(x, y);
			((DrawingHandleBase)handle).DrawTexture(frame, vector, (Color?)null);
		}
	}

	private void UpdateShields(Entity<XenoComponent, SpriteComponent> ent, DrawingHandleWorld handle)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Expected O, but got Unknown
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		Entity<XenoComponent, SpriteComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		XenoComponent xenoComponent = default(XenoComponent);
		SpriteComponent val3 = default(SpriteComponent);
		val.Deconstruct(ref val2, ref xenoComponent, ref val3);
		EntityUid val4 = val2;
		XenoComponent xenoComponent2 = xenoComponent;
		SpriteComponent val5 = val3;
		_ = (FixedPoint2)0;
		XenoShieldComponent xenoShieldComponent = default(XenoShieldComponent);
		if (_xenoShieldQuery.TryComp(val4, ref xenoShieldComponent))
		{
			FixedPoint2? threshold = null;
			FixedPoint2? threshold2 = null;
			MobThresholdsComponent thresholdComponent = default(MobThresholdsComponent);
			if (_mobThresholdsQuery.TryComp(val4, ref thresholdComponent))
			{
				_mobThresholds.TryGetThresholdForState(val4, MobState.Critical, out threshold, thresholdComponent);
				_mobThresholds.TryGetDeadThreshold(val4, out threshold2, thresholdComponent);
			}
			FixedPoint2? fixedPoint = threshold;
			if (!fixedPoint.HasValue)
			{
				threshold = threshold2;
			}
			if (threshold.HasValue)
			{
				FixedPoint2 shieldAmount = xenoShieldComponent.ShieldAmount;
				double max = threshold.Value.Double();
				int num = ContentHelpers.RoundToLevels(shieldAmount.Double(), max, 11);
				string text = ((num > 0) ? $"{num * 10}" : "0");
				string text2 = "xenoshield" + text;
				Rsi val6 = new Rsi(new ResPath("/Textures/_RMC14/Interface/xeno_hud.rsi"), text2);
				Texture frame = _sprite.GetFrame((SpriteSpecifier)(object)val6, _timing.CurTime, true);
				Box2 localBounds = _sprite.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((ent.Owner, val5)));
				float y = (((Box2)(ref localBounds)).Height + val5.Offset.Y) / 2f - (float)frame.Height / 32f * ((Box2)(ref localBounds)).Height + xenoComponent2.HudOffset.Y;
				float x = (((Box2)(ref localBounds)).Width + val5.Offset.X) / 2f - (float)frame.Width / 32f * ((Box2)(ref localBounds)).Width + xenoComponent2.HudOffset.X;
				Vector2 vector = new Vector2(x, y);
				((DrawingHandleBase)handle).DrawTexture(frame, vector, (Color?)null);
			}
		}
	}

	private void UpdateEnergy(Entity<XenoComponent, SpriteComponent> ent, DrawingHandleWorld handle)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		XenoEnergyComponent xenoEnergyComponent = default(XenoEnergyComponent);
		if (_xenoEnergyQuery.TryComp(Entity<XenoComponent, SpriteComponent>.op_Implicit(ent), ref xenoEnergyComponent) && xenoEnergyComponent.Max != 0)
		{
			UpdatePurpleBar(ent, handle, xenoEnergyComponent.Current, xenoEnergyComponent.Max, xenoEnergyComponent.GenerationCap);
		}
	}

	private void UpdatePurpleBar(Entity<XenoComponent, SpriteComponent> ent, DrawingHandleWorld handle, double energy, double max, int? generationCap)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Expected O, but got Unknown
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Expected O, but got Unknown
		Entity<XenoComponent, SpriteComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		XenoComponent xenoComponent = default(XenoComponent);
		SpriteComponent val3 = default(SpriteComponent);
		val.Deconstruct(ref val2, ref xenoComponent, ref val3);
		XenoComponent xenoComponent2 = xenoComponent;
		SpriteComponent val4 = val3;
		int num = ContentHelpers.RoundToLevels(energy, max, 11);
		string text = ((num > 0) ? $"{num * 10}" : "0");
		string text2 = "xenoenergy" + text;
		Rsi val5 = new Rsi(new ResPath("/Textures/_RMC14/Interface/xeno_hud.rsi"), text2);
		Texture frame = _sprite.GetFrame((SpriteSpecifier)(object)val5, _timing.CurTime, true);
		Box2 localBounds = _sprite.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((ent.Owner, val4)));
		float y = (((Box2)(ref localBounds)).Height + val4.Offset.Y) / 2f - (float)frame.Height / 32f * ((Box2)(ref localBounds)).Height + xenoComponent2.HudOffset.Y;
		float x = (((Box2)(ref localBounds)).Width + val4.Offset.X) / 2f - (float)frame.Width / 32f * ((Box2)(ref localBounds)).Width + xenoComponent2.HudOffset.X;
		Vector2 vector = new Vector2(x, y);
		((DrawingHandleBase)handle).DrawTexture(frame, vector, (Color?)null);
		if (generationCap.HasValue && energy >= (double?)generationCap)
		{
			int num2 = ContentHelpers.RoundToLevels(generationCap.Value, max, 11);
			string text3 = ((num2 > 0) ? $"{num2 * 10}" : "0");
			string text4 = "cap" + text3;
			Rsi val6 = new Rsi(new ResPath("/Textures/_RMC14/Interface/xeno_hud.rsi"), text4);
			Texture frame2 = _sprite.GetFrame((SpriteSpecifier)(object)val6, _timing.CurTime, true);
			((DrawingHandleBase)handle).DrawTexture(frame2, vector, (Color?)null);
		}
	}
}
