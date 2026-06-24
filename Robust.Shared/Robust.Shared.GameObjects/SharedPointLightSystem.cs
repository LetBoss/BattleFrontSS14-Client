using System.Diagnostics.CodeAnalysis;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;

namespace Robust.Shared.GameObjects;

public abstract class SharedPointLightSystem : EntitySystem
{
	public abstract SharedPointLightComponent EnsureLight(EntityUid uid);

	public abstract bool ResolveLight(EntityUid uid, [NotNullWhen(true)] ref SharedPointLightComponent? component);

	public abstract bool TryGetLight(EntityUid uid, [NotNullWhen(true)] out SharedPointLightComponent? component);

	public abstract bool RemoveLightDeferred(EntityUid uid);

	protected abstract void UpdatePriority(EntityUid uid, SharedPointLightComponent comp, MetaDataComponent meta);

	public void SetCastShadows(EntityUid uid, bool value, SharedPointLightComponent? comp = null, MetaDataComponent? meta = null)
	{
		if (ResolveLight(uid, ref comp) && value != comp.CastShadows)
		{
			comp.CastShadows = value;
			if (Resolve(uid, ref meta))
			{
				Dirty(uid, comp, meta);
				UpdatePriority(uid, comp, meta);
			}
		}
	}

	public void SetColor(EntityUid uid, Color value, SharedPointLightComponent? comp = null)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (ResolveLight(uid, ref comp) && !(value == comp.Color))
		{
			comp.Color = value;
			Dirty(uid, comp);
		}
	}

	public virtual void SetEnabled(EntityUid uid, bool enabled, SharedPointLightComponent? comp = null, MetaDataComponent? meta = null)
	{
		if (!ResolveLight(uid, ref comp) || enabled == comp.Enabled)
		{
			return;
		}
		AttemptPointLightToggleEvent args = new AttemptPointLightToggleEvent(enabled);
		RaiseLocalEvent(uid, ref args);
		if (!args.Cancelled)
		{
			comp.Enabled = enabled;
			RaiseLocalEvent(uid, new PointLightToggleEvent(comp.Enabled));
			if (Resolve(uid, ref meta))
			{
				Dirty(uid, comp, meta);
				UpdatePriority(uid, comp, meta);
			}
		}
	}

	public void SetEnergy(EntityUid uid, float value, SharedPointLightComponent? comp = null)
	{
		if (ResolveLight(uid, ref comp) && !MathHelper.CloseToPercent(comp.Energy, value, 1E-05))
		{
			comp.Energy = value;
			Dirty(uid, comp);
		}
	}

	public virtual void SetRadius(EntityUid uid, float radius, SharedPointLightComponent? comp = null, MetaDataComponent? meta = null)
	{
		if (ResolveLight(uid, ref comp) && !MathHelper.CloseToPercent(comp.Radius, radius, 1E-05))
		{
			comp.Radius = radius;
			if (Resolve(uid, ref meta))
			{
				Dirty(uid, comp, meta);
				UpdatePriority(uid, comp, meta);
			}
		}
	}

	public void SetSoftness(EntityUid uid, float value, SharedPointLightComponent? comp = null)
	{
		if (ResolveLight(uid, ref comp) && !MathHelper.CloseToPercent(comp.Softness, value, 1E-05))
		{
			comp.Softness = value;
			Dirty(uid, comp);
		}
	}

	public void SetFalloff(EntityUid uid, float value, SharedPointLightComponent? comp = null)
	{
		if (ResolveLight(uid, ref comp) && !MathHelper.CloseToPercent(comp.Falloff, value, 1E-05))
		{
			comp.Falloff = value;
			Dirty(uid, comp);
		}
	}

	public void SetCurveFactor(EntityUid uid, float value, SharedPointLightComponent? comp = null)
	{
		if (ResolveLight(uid, ref comp) && !MathHelper.CloseToPercent(comp.CurveFactor, value, 1E-05))
		{
			comp.CurveFactor = value;
			Dirty(uid, comp);
		}
	}

	protected static void OnLightGetState(EntityUid uid, SharedPointLightComponent component, ref ComponentGetState args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		args.State = new PointLightComponentState
		{
			Color = component.Color,
			Enabled = component.Enabled,
			Energy = component.Energy,
			Offset = component.Offset,
			Radius = component.Radius,
			Softness = component.Softness,
			Falloff = component.Falloff,
			CurveFactor = component.CurveFactor,
			CastShadows = component.CastShadows
		};
	}
}
