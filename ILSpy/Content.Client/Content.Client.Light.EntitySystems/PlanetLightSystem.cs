using System;
using Content.Shared.CCVar;
using Robust.Client.Graphics;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client.Light.EntitySystems;

public sealed class PlanetLightSystem : EntitySystem
{
	[Dependency]
	private IConfigurationManager _cfgManager;

	[Dependency]
	private IOverlayManager _overlayMan;

	private bool _ambientOcclusion;

	public bool AmbientOcclusion
	{
		get
		{
			return _ambientOcclusion;
		}
		set
		{
			if (_ambientOcclusion != value)
			{
				_ambientOcclusion = value;
				if (value)
				{
					_overlayMan.AddOverlay((Overlay)(object)new AmbientOcclusionOverlay());
				}
				else
				{
					_overlayMan.RemoveOverlay<AmbientOcclusionOverlay>();
				}
			}
		}
	}

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<GetClearColorEvent>((EntityEventRefHandler<GetClearColorEvent>)OnClearColor, (Type[])null, (Type[])null);
		_cfgManager.OnValueChanged<bool>(CCVars.AmbientOcclusion, (Action<bool>)delegate(bool val)
		{
			AmbientOcclusion = val;
		}, true);
		_overlayMan.AddOverlay((Overlay)(object)new BeforeLightTargetOverlay());
		_overlayMan.AddOverlay((Overlay)(object)new RoofOverlay((IEntityManager)(object)base.EntityManager));
		_overlayMan.AddOverlay((Overlay)(object)new TileEmissionOverlay((IEntityManager)(object)base.EntityManager));
		_overlayMan.AddOverlay((Overlay)(object)new LightBlurOverlay());
		_overlayMan.AddOverlay((Overlay)(object)new SunShadowOverlay());
		_overlayMan.AddOverlay((Overlay)(object)new AfterLightTargetOverlay());
	}

	private void OnClearColor(ref GetClearColorEvent ev)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		ev.Color = Color.Transparent;
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		_overlayMan.RemoveOverlay<BeforeLightTargetOverlay>();
		_overlayMan.RemoveOverlay<RoofOverlay>();
		_overlayMan.RemoveOverlay<TileEmissionOverlay>();
		_overlayMan.RemoveOverlay<LightBlurOverlay>();
		_overlayMan.RemoveOverlay<SunShadowOverlay>();
		_overlayMan.RemoveOverlay<AfterLightTargetOverlay>();
	}
}
