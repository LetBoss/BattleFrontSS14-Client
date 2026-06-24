using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared._RMC14.Lights;

public sealed class PointLightRotationSystem : EntitySystem
{
	[Dependency]
	private SharedPointLightSystem _pointLight;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<PointLightRotationComponent, ComponentStartup>((EntityEventRefHandler<PointLightRotationComponent, ComponentStartup>)OnSetRotation, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PointLightRotationComponent, MapInitEvent>((EntityEventRefHandler<PointLightRotationComponent, MapInitEvent>)OnSetRotation, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PointLightRotationComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<PointLightRotationComponent, AfterAutoHandleStateEvent>)OnSetRotation, (Type[])null, (Type[])null);
	}

	private void OnSetRotation<T>(Entity<PointLightRotationComponent> ent, ref T args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		SharedPointLightComponent light = default(SharedPointLightComponent);
		if (_pointLight.TryGetLight(Entity<PointLightRotationComponent>.op_Implicit(ent), ref light))
		{
			light.Rotation = ent.Comp.Rotation;
		}
		((EntitySystem)this).Dirty<PointLightRotationComponent>(ent, (MetaDataComponent)null);
	}
}
