using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._CIV14merka.Particles;
using Content.Shared.Light.Components;
using Content.Shared.Light.EntitySystems;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client._CIV14merka.Particles;

public sealed class CivParticleOverlay : Overlay
{
	private struct Particle
	{
		public Vector2 Pos;

		public Vector2 Vel;

		public float Size;
	}

	private readonly struct RoofGrid(EntityUid uid, MapGridComponent grid, RoofComponent roof, Matrix3x2 inv)
	{
		public readonly EntityUid Uid = uid;

		public readonly MapGridComponent Grid = grid;

		public readonly RoofComponent Roof = roof;

		public readonly Matrix3x2 Inv = inv;
	}

	private static readonly ResPath DefaultParticle = new ResPath("/Textures/_CIV14merka/Particles/soft.png");

	[Dependency]
	private readonly IRobustRandom _random;

	[Dependency]
	private readonly IGameTiming _timing;

	private readonly IEntityManager _entMan;

	private readonly IMapManager _mapMan;

	private readonly SharedTransformSystem _xform;

	private readonly SharedRoofSystem _roof;

	private readonly IResourceCache _resource;

	public CivParticlePresetPrototype? Preset;

	public float Intensity;

	public Vector2 Wind;

	private Particle[] _particles = Array.Empty<Particle>();

	private int _active;

	private DrawVertexUV2DColor[] _verts = Array.Empty<DrawVertexUV2DColor>();

	private List<Entity<MapGridComponent>> _grids = new List<Entity<MapGridComponent>>();

	private readonly List<RoofGrid> _roofGrids = new List<RoofGrid>();

	private CivParticlePresetPrototype? _builtFor;

	private Texture? _defaultTex;

	private TimeSpan _lastDraw;

	public override OverlaySpace Space => (OverlaySpace)8;

	public CivParticleOverlay(IEntityManager entMan, IMapManager mapMan, SharedTransformSystem xform, SharedRoofSystem roof, IResourceCache resource)
	{
		IoCManager.InjectDependencies<CivParticleOverlay>(this);
		_entMan = entMan;
		_mapMan = mapMan;
		_xform = xform;
		_roof = roof;
		_resource = resource;
		((Overlay)this).ZIndex = 0;
	}

	protected override bool BeforeDraw(in OverlayDrawArgs args)
	{
		if (Preset != null)
		{
			return Intensity > 0.01f;
		}
		return false;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		CivParticlePresetPrototype preset = Preset;
		if (preset == null || Intensity <= 0.01f)
		{
			return;
		}
		TimeSpan realTime = _timing.RealTime;
		float num = (float)(realTime - _lastDraw).TotalSeconds;
		_lastDraw = realTime;
		if (num <= 0f || num > 0.1f)
		{
			num = MathF.Min(MathF.Max(num, 0f), 0.1f);
		}
		Box2 box = ((Box2)(ref args.WorldAABB)).Enlarged(3f);
		int num2 = (int)MathF.Min(((Box2)(ref box)).Width * ((Box2)(ref box)).Height * preset.Density * Intensity, preset.MaxParticles);
		if (num2 <= 0)
		{
			return;
		}
		EnsureCapacity(num2, preset, box);
		Vector2 vector = Wind * preset.WindResponse;
		for (int i = 0; i < _active; i++)
		{
			ref Particle reference = ref _particles[i];
			reference.Pos += (reference.Vel + vector) * num;
			if (!((Box2)(ref box)).Contains(reference.Pos, true))
			{
				Respawn(ref reference, box, preset);
			}
		}
		BuildRoofGrids(args.MapId, box);
		Texture val = ResolveTexture(preset);
		Color color = Color.FromSrgb(((Color)(ref preset.Color)).WithAlpha(MathF.Min(preset.Alpha * Intensity, 1f)));
		int num3 = BuildVertices(vector, color, preset.Stretch);
		if (num3 != 0)
		{
			((DrawingHandleBase)((OverlayDrawArgs)(ref args)).WorldHandle).DrawPrimitives((DrawPrimitiveTopology)1, val, (ReadOnlySpan<DrawVertexUV2DColor>)_verts.AsSpan(0, num3 * 6));
		}
	}

	private int BuildVertices(Vector2 wind, Color color, float stretch)
	{
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		for (int i = 0; i < _active; i++)
		{
			Particle particle = _particles[i];
			if (!IsRoofed(particle.Pos))
			{
				Vector2 value = particle.Vel + wind;
				Vector2 vector = ((value.LengthSquared() > 0.0001f) ? Vector2.Normalize(value) : new Vector2(0f, -1f));
				Vector2 vector2 = new Vector2(vector.Y, 0f - vector.X);
				float num2 = particle.Size * 0.5f;
				float num3 = particle.Size * stretch * 0.5f;
				Vector2 vector3 = particle.Pos - vector2 * num2 - vector * num3;
				Vector2 vector4 = particle.Pos + vector2 * num2 - vector * num3;
				Vector2 vector5 = particle.Pos + vector2 * num2 + vector * num3;
				Vector2 vector6 = particle.Pos - vector2 * num2 + vector * num3;
				int num4 = num * 6;
				_verts[num4] = new DrawVertexUV2DColor(vector3, new Vector2(0f, 1f), color);
				_verts[num4 + 1] = new DrawVertexUV2DColor(vector4, new Vector2(1f, 1f), color);
				_verts[num4 + 2] = new DrawVertexUV2DColor(vector5, new Vector2(1f, 0f), color);
				_verts[num4 + 3] = new DrawVertexUV2DColor(vector3, new Vector2(0f, 1f), color);
				_verts[num4 + 4] = new DrawVertexUV2DColor(vector5, new Vector2(1f, 0f), color);
				_verts[num4 + 5] = new DrawVertexUV2DColor(vector6, new Vector2(0f, 0f), color);
				num++;
			}
		}
		return num;
	}

	private bool IsRoofed(Vector2 worldPos)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		Vector2i index = default(Vector2i);
		foreach (RoofGrid roofGrid in _roofGrids)
		{
			Vector2 vector = Vector2.Transform(worldPos, roofGrid.Inv);
			ushort tileSize = roofGrid.Grid.TileSize;
			((Vector2i)(ref index))._002Ector((int)MathF.Floor(vector.X / (float)(int)tileSize), (int)MathF.Floor(vector.Y / (float)(int)tileSize));
			if (_roof.IsRooved(Entity<MapGridComponent, RoofComponent>.op_Implicit((roofGrid.Uid, roofGrid.Grid, roofGrid.Roof)), index))
			{
				return true;
			}
		}
		return false;
	}

	private void BuildRoofGrids(MapId mapId, Box2 box)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		_roofGrids.Clear();
		_grids.Clear();
		_mapMan.FindGridsIntersecting(mapId, box, ref _grids, false, true);
		RoofComponent roof = default(RoofComponent);
		foreach (Entity<MapGridComponent> grid in _grids)
		{
			if (_entMan.TryGetComponent<RoofComponent>(grid.Owner, ref roof) && Matrix3x2.Invert(_xform.GetWorldMatrix(grid.Owner), out var result))
			{
				_roofGrids.Add(new RoofGrid(grid.Owner, grid.Comp, roof, result));
			}
		}
	}

	private Texture ResolveTexture(CivParticlePresetPrototype preset)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (preset.Texture.HasValue)
		{
			return _resource.GetResource<TextureResource>(preset.Texture.Value, true).Texture;
		}
		if (_defaultTex == null)
		{
			_defaultTex = _resource.GetResource<TextureResource>(DefaultParticle, true).Texture;
		}
		return _defaultTex;
	}

	private void Respawn(ref Particle p, Box2 box, CivParticlePresetPrototype preset)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		p.Pos = new Vector2(_random.NextFloat(box.Left, box.Right), _random.NextFloat(box.Bottom, box.Top));
		p.Vel = preset.Velocity + new Vector2(_random.NextFloat(0f - preset.VelocityVariance.X, preset.VelocityVariance.X), _random.NextFloat(0f - preset.VelocityVariance.Y, preset.VelocityVariance.Y));
		p.Size = _random.NextFloat(preset.SizeMin, preset.SizeMax);
	}

	private void EnsureCapacity(int count, CivParticlePresetPrototype preset, Box2 box)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		bool flag = _builtFor != preset;
		if (count > _particles.Length || flag)
		{
			Particle[] array = new Particle[count];
			int num = ((!flag) ? Math.Min(_active, _particles.Length) : 0);
			Array.Copy(_particles, array, num);
			for (int i = num; i < count; i++)
			{
				Respawn(ref array[i], box, preset);
			}
			_particles = array;
		}
		if (count * 6 > _verts.Length)
		{
			_verts = (DrawVertexUV2DColor[])(object)new DrawVertexUV2DColor[count * 6];
		}
		_builtFor = preset;
		_active = count;
	}
}
