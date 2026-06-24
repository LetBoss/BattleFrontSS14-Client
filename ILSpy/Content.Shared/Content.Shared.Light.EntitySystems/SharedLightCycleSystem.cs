using System;
using Content.Shared.Light.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;

namespace Content.Shared.Light.EntitySystems;

public abstract class SharedLightCycleSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<LightCycleComponent, MapInitEvent>((EntityEventRefHandler<LightCycleComponent, MapInitEvent>)OnCycleMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LightCycleComponent, ComponentShutdown>((EntityEventRefHandler<LightCycleComponent, ComponentShutdown>)OnCycleShutdown, (Type[])null, (Type[])null);
	}

	protected virtual void OnCycleMapInit(Entity<LightCycleComponent> ent, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		MapLightComponent mapLight = default(MapLightComponent);
		if (((EntitySystem)this).TryComp<MapLightComponent>(ent.Owner, ref mapLight))
		{
			ent.Comp.OriginalColor = mapLight.AmbientLightColor;
			((EntitySystem)this).Dirty<LightCycleComponent>(ent, (MetaDataComponent)null);
		}
	}

	private void OnCycleShutdown(Entity<LightCycleComponent> ent, ref ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		MapLightComponent mapLight = default(MapLightComponent);
		if (((EntitySystem)this).TryComp<MapLightComponent>(ent.Owner, ref mapLight))
		{
			mapLight.AmbientLightColor = ent.Comp.OriginalColor;
			((EntitySystem)this).Dirty(ent.Owner, (IComponent)(object)mapLight, (MetaDataComponent)null);
		}
	}

	public void SetOffset(Entity<LightCycleComponent> entity, TimeSpan offset)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		entity.Comp.Offset = offset;
		LightCycleOffsetEvent ev = new LightCycleOffsetEvent(offset);
		((EntitySystem)this).RaiseLocalEvent<LightCycleOffsetEvent>(Entity<LightCycleComponent>.op_Implicit(entity), ref ev, false);
		((EntitySystem)this).Dirty<LightCycleComponent>(entity, (MetaDataComponent)null);
	}

	public static Color GetColor(Entity<LightCycleComponent> cycle, Color color, float time)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		if (cycle.Comp.Enabled)
		{
			double lightLevel = CalculateLightLevel(cycle.Comp, time);
			Color colorLevel = CalculateColorLevel(cycle.Comp, time);
			return new Color((byte)Math.Min(255.0, (double)((float)(int)((Color)(ref color)).RByte * colorLevel.R) * lightLevel), (byte)Math.Min(255.0, (double)((float)(int)((Color)(ref color)).GByte * colorLevel.G) * lightLevel), (byte)Math.Min(255.0, (double)((float)(int)((Color)(ref color)).BByte * colorLevel.B) * lightLevel), byte.MaxValue);
		}
		return color;
	}

	public static double CalculateLightLevel(LightCycleComponent comp, float time)
	{
		float waveLength = MathF.Max(1f, (float)comp.Duration.TotalSeconds);
		float crest = MathF.Max(0f, comp.MaxLightLevel);
		float shift = MathF.Max(0f, comp.MinLightLevel);
		return Math.Min(comp.ClipLight, CalculateCurve(time, waveLength, crest, shift, 6f));
	}

	public static Color CalculateColorLevel(LightCycleComponent comp, float time)
	{
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		float waveLength = MathF.Max(1f, (float)comp.Duration.TotalSeconds);
		float num = MathF.Min(comp.ClipLevel.R, CalculateCurve(time, waveLength, MathF.Max(0f, comp.MaxLevel.R), MathF.Max(0f, comp.MinLevel.R), 4f));
		float green = MathF.Min(comp.ClipLevel.G, CalculateCurve(time, waveLength, MathF.Max(0f, comp.MaxLevel.G), MathF.Max(0f, comp.MinLevel.G), 10f));
		float blue = MathF.Min(comp.ClipLevel.B, CalculateCurve(time, waveLength / 2f, MathF.Max(0f, comp.MaxLevel.B), MathF.Max(0f, comp.MinLevel.B), 2f, waveLength / 4f));
		return new Color(num, green, blue, 1f);
	}

	public static float CalculateCurve(float x, float waveLength, float crest, float shift, float exponent, float phase = 0f)
	{
		float sen = MathF.Pow(MathF.Sin((float)Math.PI * (phase + x) / waveLength), exponent);
		return (crest - shift) * sen + shift;
	}
}
