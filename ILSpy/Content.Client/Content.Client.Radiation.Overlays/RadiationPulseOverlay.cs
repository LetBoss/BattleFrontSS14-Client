using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using Content.Shared.Radiation.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client.Radiation.Overlays;

public sealed class RadiationPulseOverlay : Overlay
{
	private sealed record RadiationShaderInstance(MapCoordinates CurrentMapCoords, float Range, TimeSpan Start, float Duration)
	{
		public MapCoordinates CurrentMapCoords = CurrentMapCoords;

		public float Range = Range;

		public TimeSpan Start = Start;

		public float Duration = Duration;

		[CompilerGenerated]
		public void Deconstruct(out MapCoordinates CurrentMapCoords, out float Range, out TimeSpan Start, out float Duration)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			CurrentMapCoords = this.CurrentMapCoords;
			Range = this.Range;
			Start = this.Start;
			Duration = this.Duration;
		}
	}

	private static readonly ProtoId<ShaderPrototype> RadiationShader = ProtoId<ShaderPrototype>.op_Implicit("Radiation");

	[Dependency]
	private IEntityManager _entityManager;

	[Dependency]
	private IPrototypeManager _prototypeManager;

	[Dependency]
	private IGameTiming _gameTiming;

	private TransformSystem? _transform;

	private const float MaxDist = 15f;

	private readonly ShaderInstance _baseShader;

	private readonly Dictionary<EntityUid, (ShaderInstance shd, RadiationShaderInstance instance)> _pulses = new Dictionary<EntityUid, (ShaderInstance, RadiationShaderInstance)>();

	public override OverlaySpace Space => (OverlaySpace)4;

	public override bool RequestScreenTexture => true;

	public RadiationPulseOverlay()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<RadiationPulseOverlay>(this);
		_baseShader = _prototypeManager.Index<ShaderPrototype>(RadiationShader).Instance().Duplicate();
	}

	protected override bool BeforeDraw(in OverlayDrawArgs args)
	{
		RadiationQuery(args.Viewport.Eye);
		return _pulses.Count > 0;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		if (base.ScreenTexture == null)
		{
			return;
		}
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		IClydeViewport viewport = args.Viewport;
		foreach (var (val, radiationShaderInstance) in _pulses.Values)
		{
			if (!(radiationShaderInstance.CurrentMapCoords.MapId != args.MapId))
			{
				Vector2 vector = viewport.WorldToLocal(radiationShaderInstance.CurrentMapCoords.Position);
				vector.Y = (float)viewport.Size.Y - vector.Y;
				if (val != null)
				{
					val.SetParameter("renderScale", viewport.RenderScale);
				}
				if (val != null)
				{
					val.SetParameter("positionInput", vector);
				}
				if (val != null)
				{
					val.SetParameter("range", radiationShaderInstance.Range);
				}
				double num = (_gameTiming.RealTime - radiationShaderInstance.Start).TotalSeconds / (double)radiationShaderInstance.Duration;
				if (val != null)
				{
					val.SetParameter("life", (float)num);
				}
				if (val != null)
				{
					val.SetParameter("SCREEN_TEXTURE", viewport.RenderTarget.Texture);
				}
				((DrawingHandleBase)worldHandle).UseShader(val);
				worldHandle.DrawRect(Box2.CenteredAround(radiationShaderInstance.CurrentMapCoords.Position, new Vector2(radiationShaderInstance.Range, radiationShaderInstance.Range) * 2f), Color.White, true);
			}
		}
		((DrawingHandleBase)worldHandle).UseShader((ShaderInstance)null);
	}

	private void RadiationQuery(IEye? currentEye)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		if (_transform == null)
		{
			_transform = _entityManager.System<TransformSystem>();
		}
		if (currentEye == null)
		{
			_pulses.Clear();
			return;
		}
		MapCoordinates position = currentEye.Position;
		EntityQueryEnumerator<RadiationPulseComponent> val = _entityManager.EntityQueryEnumerator<RadiationPulseComponent>();
		EntityUid val2 = default(EntityUid);
		RadiationPulseComponent radiationPulseComponent = default(RadiationPulseComponent);
		while (val.MoveNext(ref val2, ref radiationPulseComponent))
		{
			if (!_pulses.ContainsKey(val2) && PulseQualifies(val2, position))
			{
				_pulses.Add(val2, (_baseShader.Duplicate(), new RadiationShaderInstance(((SharedTransformSystem)_transform).GetMapCoordinates(val2, (TransformComponent)null), radiationPulseComponent.VisualRange, radiationPulseComponent.StartTime, radiationPulseComponent.VisualDuration)));
			}
		}
		RadiationPulseComponent radiationPulseComponent2 = default(RadiationPulseComponent);
		foreach (EntityUid key in _pulses.Keys)
		{
			if (_entityManager.EntityExists(key) && PulseQualifies(key, position) && _entityManager.TryGetComponent<RadiationPulseComponent>(key, ref radiationPulseComponent2))
			{
				(ShaderInstance shd, RadiationShaderInstance instance) tuple = _pulses[key];
				tuple.instance.CurrentMapCoords = ((SharedTransformSystem)_transform).GetMapCoordinates(key, (TransformComponent)null);
				tuple.instance.Range = radiationPulseComponent2.VisualRange;
			}
			else
			{
				_pulses[key].shd.Dispose();
				_pulses.Remove(key);
			}
		}
	}

	private bool PulseQualifies(EntityUid pulseEntity, MapCoordinates currentEyeLoc)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent component = _entityManager.GetComponent<TransformComponent>(pulseEntity);
		SharedTransformSystem val = _entityManager.System<SharedTransformSystem>();
		if (component.MapID == currentEyeLoc.MapId)
		{
			return val.InRange(component.Coordinates, val.ToCoordinates(Entity<TransformComponent>.op_Implicit(component.ParentUid), currentEyeLoc), 15f);
		}
		return false;
	}
}
