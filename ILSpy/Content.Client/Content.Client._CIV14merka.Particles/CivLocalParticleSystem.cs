using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._CIV14merka.Particles;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.ResourceManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client._CIV14merka.Particles;

public sealed class CivLocalParticleSystem : EntitySystem
{
	[Dependency]
	private readonly IOverlayManager _overlay;

	[Dependency]
	private readonly IPrototypeManager _proto;

	[Dependency]
	private readonly IPlayerManager _player;

	[Dependency]
	private readonly IResourceCache _resource;

	[Dependency]
	private readonly IGameTiming _timing;

	[Dependency]
	private readonly IRobustRandom _random;

	[Dependency]
	private readonly SharedTransformSystem _transform;

	private static readonly ResPath DefaultParticle = new ResPath("/Textures/_CIV14merka/Particles/soft.png");

	private const float DegToRad = MathF.PI / 180f;

	private const int MaxParticles = 4000;

	private CivLocalParticleOverlay? _overlayInstance;

	private readonly List<CivLocalParticle> _particles = new List<CivLocalParticle>();

	private readonly Dictionary<EntityUid, float> _accum = new Dictionary<EntityUid, float>();

	private readonly Dictionary<string, Texture> _texCache = new Dictionary<string, Texture>();

	private Texture? _defaultTex;

	private MapId _playerMap = MapId.Nullspace;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<CivParticleEmitterComponent, ComponentStartup>((EntityEventRefHandler<CivParticleEmitterComponent, ComponentStartup>)OnStartup, (Type[])null, (Type[])null);
		_proto.PrototypesReloaded += delegate
		{
			_texCache.Clear();
		};
		_overlayInstance = new CivLocalParticleOverlay
		{
			Particles = _particles
		};
		_overlay.AddOverlay((Overlay)(object)_overlayInstance);
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		if (_overlayInstance != null)
		{
			_overlay.RemoveOverlay((Overlay)(object)_overlayInstance);
		}
		_overlayInstance = null;
	}

	private void OnStartup(Entity<CivParticleEmitterComponent> ent, ref ComponentStartup args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		CivEmitterPresetPrototype civEmitterPresetPrototype = default(CivEmitterPresetPrototype);
		if (_proto.TryIndex<CivEmitterPresetPrototype>(ent.Comp.Preset, ref civEmitterPresetPrototype) && civEmitterPresetPrototype.BurstCount > 0)
		{
			MapId mapID = ((EntitySystem)this).Transform(Entity<CivParticleEmitterComponent>.op_Implicit(ent)).MapID;
			if (!(_playerMap != MapId.Nullspace) || !(mapID != _playerMap))
			{
				Emit(civEmitterPresetPrototype, _transform.GetWorldPosition(ent.Owner), civEmitterPresetPrototype.BurstCount);
			}
		}
	}

	public override void FrameUpdate(float frameTime)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).FrameUpdate(frameTime);
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		_playerMap = (localEntity.HasValue ? ((EntitySystem)this).Transform(localEntity.Value).MapID : MapId.Nullspace);
		Vector2 wind = GetWind();
		EntityQueryEnumerator<CivParticleEmitterComponent, TransformComponent> val = ((EntitySystem)this).EntityQueryEnumerator<CivParticleEmitterComponent, TransformComponent>();
		EntityUid val2 = default(EntityUid);
		CivParticleEmitterComponent civParticleEmitterComponent = default(CivParticleEmitterComponent);
		TransformComponent val3 = default(TransformComponent);
		CivEmitterPresetPrototype civEmitterPresetPrototype = default(CivEmitterPresetPrototype);
		while (val.MoveNext(ref val2, ref civParticleEmitterComponent, ref val3))
		{
			if (civParticleEmitterComponent.Active && !(val3.MapID != _playerMap) && !(_playerMap == MapId.Nullspace) && _proto.TryIndex<CivEmitterPresetPrototype>(civParticleEmitterComponent.Preset, ref civEmitterPresetPrototype) && !(civEmitterPresetPrototype.Rate <= 0f))
			{
				float num = _accum.GetValueOrDefault(val2) + civEmitterPresetPrototype.Rate * frameTime;
				int num2 = (int)num;
				_accum[val2] = num - (float)num2;
				if (num2 > 0)
				{
					Emit(civEmitterPresetPrototype, _transform.GetWorldPosition(val2), num2);
				}
			}
		}
		for (int num3 = _particles.Count - 1; num3 >= 0; num3--)
		{
			CivLocalParticle value = _particles[num3];
			value.Age += frameTime;
			if (value.Age >= value.Life)
			{
				List<CivLocalParticle> particles = _particles;
				int index = num3;
				List<CivLocalParticle> particles2 = _particles;
				particles[index] = particles2[particles2.Count - 1];
				_particles.RemoveAt(_particles.Count - 1);
			}
			else
			{
				value.Vel += value.Gravity * frameTime;
				value.Vel *= MathF.Max(0f, 1f - value.Drag * frameTime);
				value.Pos += (value.Vel + wind * value.Wind) * frameTime;
				_particles[num3] = value;
			}
		}
		if (_accum.Count > 256)
		{
			PruneAccum();
		}
	}

	public void EmitBurst(ProtoId<CivEmitterPresetPrototype> presetId, MapCoordinates coords, float scale = 1f, float? direction = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		CivEmitterPresetPrototype civEmitterPresetPrototype = default(CivEmitterPresetPrototype);
		if (!(_playerMap == MapId.Nullspace) && !(coords.MapId != _playerMap) && _proto.TryIndex<CivEmitterPresetPrototype>(presetId, ref civEmitterPresetPrototype))
		{
			int count = (int)MathF.Max(1f, (float)civEmitterPresetPrototype.BurstCount * scale);
			Emit(civEmitterPresetPrototype, coords.Position, count, direction);
		}
	}

	private void Emit(CivEmitterPresetPrototype preset, Vector2 origin, int count, float? directionOverride = null)
	{
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		Texture tex = ResolveTexture(preset);
		float num = directionOverride ?? preset.Direction;
		for (int i = 0; i < count; i++)
		{
			if (_particles.Count >= 4000)
			{
				break;
			}
			float x = (num + _random.NextFloat((0f - preset.Spread) * 0.5f, preset.Spread * 0.5f)) * (MathF.PI / 180f);
			float num2 = MathF.Max(0f, preset.Speed + _random.NextFloat(0f - preset.SpeedVariance, preset.SpeedVariance));
			Vector2 vel = new Vector2(MathF.Cos(x), MathF.Sin(x)) * num2;
			float life = MathF.Max(0.05f, preset.Life + _random.NextFloat(0f - preset.LifeVariance, preset.LifeVariance));
			_particles.Add(new CivLocalParticle
			{
				Pos = origin + RandomDisk(preset.Radius),
				Vel = vel,
				Gravity = preset.Gravity,
				Drag = preset.Drag,
				Wind = preset.WindResponse,
				Age = 0f,
				Life = life,
				Size0 = preset.SizeStart,
				Size1 = preset.SizeEnd,
				A0 = preset.AlphaStart,
				A1 = preset.AlphaEnd,
				Stretch = preset.Stretch,
				Rgb = preset.Color,
				Tex = tex
			});
		}
	}

	private Vector2 RandomDisk(float radius)
	{
		if (radius <= 0f)
		{
			return Vector2.Zero;
		}
		float x = _random.NextFloat(0f, MathF.PI * 2f);
		float num = MathF.Sqrt(_random.NextFloat()) * radius;
		return new Vector2(MathF.Cos(x), MathF.Sin(x)) * num;
	}

	private Texture ResolveTexture(CivEmitterPresetPrototype preset)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (!preset.Texture.HasValue)
		{
			return _defaultTex ?? (_defaultTex = _resource.GetResource<TextureResource>(DefaultParticle, true).Texture);
		}
		if (_texCache.TryGetValue(preset.ID, out Texture value))
		{
			return value;
		}
		Texture texture = _resource.GetResource<TextureResource>(preset.Texture.Value, true).Texture;
		_texCache[preset.ID] = texture;
		return texture;
	}

	private void PruneAccum()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		List<EntityUid> list = new List<EntityUid>();
		foreach (EntityUid key in _accum.Keys)
		{
			if (!base.EntityManager.EntityExists(key))
			{
				list.Add(key);
			}
		}
		foreach (EntityUid item in list)
		{
			_accum.Remove(item);
		}
	}

	private Vector2 GetWind()
	{
		float num = (float)_timing.RealTime.TotalSeconds;
		float num2 = 1.4f + MathF.Sin(num * 0.13f) * 0.9f + MathF.Sin(num * 0.41f) * 0.4f;
		float x = MathF.Sin(num * 0.05f) * 0.6f;
		return new Vector2(MathF.Cos(x), MathF.Sin(x)) * num2;
	}
}
