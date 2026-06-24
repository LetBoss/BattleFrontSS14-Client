using System;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Markers;

public sealed class MarkerSystem : EntitySystem
{
	[Dependency]
	private SpriteSystem _sprite;

	private bool _markersVisible;

	public bool MarkersVisible
	{
		get
		{
			return _markersVisible;
		}
		set
		{
			_markersVisible = value;
			UpdateMarkers();
		}
	}

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<MarkerComponent, ComponentStartup>((ComponentEventHandler<MarkerComponent, ComponentStartup>)OnStartup, (Type[])null, (Type[])null);
	}

	private void OnStartup(EntityUid uid, MarkerComponent marker, ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateVisibility(uid);
	}

	private void UpdateVisibility(EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(uid, ref item))
		{
			_sprite.SetVisible(Entity<SpriteComponent>.op_Implicit((uid, item)), MarkersVisible);
		}
	}

	private void UpdateMarkers()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		AllEntityQueryEnumerator<MarkerComponent> val = ((EntitySystem)this).AllEntityQuery<MarkerComponent>();
		EntityUid uid = default(EntityUid);
		MarkerComponent markerComponent = default(MarkerComponent);
		while (val.MoveNext(ref uid, ref markerComponent))
		{
			UpdateVisibility(uid);
		}
	}
}
