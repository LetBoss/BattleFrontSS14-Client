using System;
using Content.Client.Administration.Components;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Client.Administration.Systems;

public sealed class HeadstandSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<HeadstandComponent, ComponentStartup>((ComponentEventHandler<HeadstandComponent, ComponentStartup>)OnHeadstandAdded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HeadstandComponent, ComponentShutdown>((ComponentEventHandler<HeadstandComponent, ComponentShutdown>)OnHeadstandRemoved, (Type[])null, (Type[])null);
	}

	private void OnHeadstandAdded(EntityUid uid, HeadstandComponent component, ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent val = default(SpriteComponent);
		if (!((EntitySystem)this).TryComp<SpriteComponent>(uid, ref val))
		{
			return;
		}
		foreach (ISpriteLayer allLayer in val.AllLayers)
		{
			allLayer.Rotation += Angle.FromDegrees(180.0);
		}
	}

	private void OnHeadstandRemoved(EntityUid uid, HeadstandComponent component, ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent val = default(SpriteComponent);
		if (!((EntitySystem)this).TryComp<SpriteComponent>(uid, ref val))
		{
			return;
		}
		foreach (ISpriteLayer allLayer in val.AllLayers)
		{
			allLayer.Rotation -= Angle.FromDegrees(180.0);
		}
	}
}
