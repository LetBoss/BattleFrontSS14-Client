using System;
using System.Collections.Generic;
using Content.Shared.Explosion;
using Content.Shared.Explosion.Components;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Graphics.RSI;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client.Explosion;

public sealed class ExplosionOverlaySystem : EntitySystem
{
	[Dependency]
	private IPrototypeManager _protoMan;

	[Dependency]
	private IResourceCache _resCache;

	[Dependency]
	private IOverlayManager _overlayMan;

	[Dependency]
	private SharedPointLightSystem _lights;

	[Dependency]
	private SharedMapSystem _mapSystem;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ExplosionVisualsComponent, ComponentInit>((ComponentEventHandler<ExplosionVisualsComponent, ComponentInit>)OnExplosionInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ExplosionVisualsComponent, ComponentRemove>((ComponentEventHandler<ExplosionVisualsComponent, ComponentRemove>)OnCompRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ExplosionVisualsComponent, ComponentHandleState>((ComponentEventRefHandler<ExplosionVisualsComponent, ComponentHandleState>)OnExplosionHandleState, (Type[])null, (Type[])null);
		_overlayMan.AddOverlay((Overlay)(object)new ExplosionOverlay(_appearance));
	}

	private void OnExplosionHandleState(EntityUid uid, ExplosionVisualsComponent component, ref ComponentHandleState args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		if (!(((ComponentHandleState)(ref args)).Current is ExplosionVisualsState explosionVisualsState))
		{
			return;
		}
		component.Epicenter = explosionVisualsState.Epicenter;
		component.SpaceTiles = explosionVisualsState.SpaceTiles;
		component.Tiles.Clear();
		foreach (var (val2, value) in explosionVisualsState.Tiles)
		{
			component.Tiles[((EntitySystem)this).GetEntity(val2)] = value;
		}
		component.Intensity = explosionVisualsState.Intensity;
		component.ExplosionType = explosionVisualsState.ExplosionType;
		component.SpaceMatrix = explosionVisualsState.SpaceMatrix;
		component.SpaceTileSize = explosionVisualsState.SpaceTileSize;
	}

	private void OnCompRemove(EntityUid uid, ExplosionVisualsComponent component, ComponentRemove args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		ExplosionVisualsTexturesComponent explosionVisualsTexturesComponent = default(ExplosionVisualsTexturesComponent);
		if (((EntitySystem)this).TryComp<ExplosionVisualsTexturesComponent>(uid, ref explosionVisualsTexturesComponent) && !((EntitySystem)this).Deleted(explosionVisualsTexturesComponent.LightEntity, (MetaDataComponent)null))
		{
			((EntitySystem)this).QueueDel((EntityUid?)explosionVisualsTexturesComponent.LightEntity);
		}
	}

	private void OnExplosionInit(EntityUid uid, ExplosionVisualsComponent component, ComponentInit args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).EnsureComp<ExplosionVisualsTexturesComponent>(uid);
		ExplosionPrototype explosionPrototype = default(ExplosionPrototype);
		ExplosionVisualsTexturesComponent explosionVisualsTexturesComponent = default(ExplosionVisualsTexturesComponent);
		if (!_protoMan.TryIndex<ExplosionPrototype>(component.ExplosionType, ref explosionPrototype) || !((EntitySystem)this).TryComp<ExplosionVisualsTexturesComponent>(uid, ref explosionVisualsTexturesComponent))
		{
			return;
		}
		if (_mapSystem.MapExists((MapId?)component.Epicenter.MapId))
		{
			EntityUid val = ((EntitySystem)this).Spawn("ExplosionLight", component.Epicenter, (ComponentRegistry)null, default(Angle));
			SharedPointLightComponent val2 = _lights.EnsureLight(val);
			_lights.SetRadius(val, (float)component.Intensity.Count, val2, (MetaDataComponent)null);
			_lights.SetEnergy(val, (float)component.Intensity.Count, val2);
			_lights.SetColor(val, explosionPrototype.LightColor, val2);
			explosionVisualsTexturesComponent.LightEntity = val;
		}
		explosionVisualsTexturesComponent.FireColor = explosionPrototype.FireColor;
		explosionVisualsTexturesComponent.IntensityPerState = explosionPrototype.IntensityPerState;
		foreach (State item in _resCache.GetResource<RSIResource>(explosionPrototype.TexturePath, true).RSI)
		{
			explosionVisualsTexturesComponent.FireFrames.Add(item.GetFrames((RsiDirection)0));
			if (explosionVisualsTexturesComponent.FireFrames.Count == explosionPrototype.FireStates)
			{
				break;
			}
		}
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		_overlayMan.RemoveOverlay<ExplosionOverlay>();
	}
}
