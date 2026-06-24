using System;
using Content.Shared.MapText;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.RichText;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client.MapText;

public sealed class MapTextSystem : SharedMapTextSystem
{
	[Dependency]
	private IConfigurationManager _configManager;

	[Dependency]
	private IUserInterfaceManager _uiManager;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private IResourceCache _resourceCache;

	[Dependency]
	private IPrototypeManager _prototypeManager;

	[Dependency]
	private IOverlayManager _overlayManager;

	private MapTextOverlay _overlay;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<MapTextComponent, ComponentStartup>((EntityEventRefHandler<MapTextComponent, ComponentStartup>)OnComponentStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MapTextComponent, ComponentHandleState>((EntityEventRefHandler<MapTextComponent, ComponentHandleState>)HandleCompState, (Type[])null, (Type[])null);
		_overlay = new MapTextOverlay(_configManager, (IEntityManager)(object)((EntitySystem)this).EntityManager, _uiManager, _transform, _resourceCache, _prototypeManager);
		_overlayManager.AddOverlay((Overlay)(object)_overlay);
	}

	private void OnComponentStartup(Entity<MapTextComponent> ent, ref ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		CacheText(ent.Comp);
	}

	private void HandleCompState(Entity<MapTextComponent> ent, ref ComponentHandleState args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		if (((ComponentHandleState)(ref args)).Current is MapTextComponentState mapTextComponentState)
		{
			ent.Comp.Text = mapTextComponentState.Text;
			ent.Comp.LocText = mapTextComponentState.LocText;
			ent.Comp.Color = mapTextComponentState.Color;
			ent.Comp.FontId = mapTextComponentState.FontId;
			ent.Comp.FontSize = mapTextComponentState.FontSize;
			ent.Comp.Offset = mapTextComponentState.Offset;
			CacheText(ent.Comp);
		}
	}

	private void CacheText(MapTextComponent component)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Expected O, but got Unknown
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Expected O, but got Unknown
		component.CachedFont = null;
		component.CachedText = (string.IsNullOrWhiteSpace(component.Text) ? ((EntitySystem)this).Loc.GetString(LocId.op_Implicit(component.LocText)) : component.Text);
		FontPrototype val = default(FontPrototype);
		if (!_prototypeManager.TryIndex<FontPrototype>(component.FontId, ref val))
		{
			component.CachedText = ((EntitySystem)this).Loc.GetString("map-text-font-error");
			component.Color = Color.Red;
			FontPrototype val2 = default(FontPrototype);
			if (_prototypeManager.TryIndex<FontPrototype>("Default", ref val2))
			{
				component.CachedFont = new VectorFont(_resourceCache.GetResource<FontResource>(val2.Path, true), 14);
			}
		}
		else
		{
			FontResource resource = _resourceCache.GetResource<FontResource>(val.Path, true);
			component.CachedFont = new VectorFont(resource, component.FontSize);
		}
	}
}
