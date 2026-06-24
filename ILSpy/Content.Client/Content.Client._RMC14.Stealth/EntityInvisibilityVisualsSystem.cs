using System;
using Content.Shared._RMC14.Stealth;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Client._RMC14.Stealth;

public sealed class EntityInvisibilityVisualsSystem : EntitySystem
{
	[Dependency]
	private IPrototypeManager _prototypes;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<EntityTurnInvisibleComponent, ComponentStartup>((EntityEventRefHandler<EntityTurnInvisibleComponent, ComponentStartup>)OnStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EntityTurnInvisibleComponent, ComponentShutdown>((EntityEventRefHandler<EntityTurnInvisibleComponent, ComponentShutdown>)OnShutdown, (Type[])null, (Type[])null);
	}

	private void OnStartup(Entity<EntityTurnInvisibleComponent> ent, ref ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent val = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(Entity<EntityTurnInvisibleComponent>.op_Implicit(ent), ref val))
		{
			val.PostShader = _prototypes.Index<ShaderPrototype>(new ProtoId<ShaderPrototype>("RMCInvisible")).InstanceUnique();
		}
	}

	private void OnShutdown(Entity<EntityTurnInvisibleComponent> ent, ref ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent val = default(SpriteComponent);
		if (!((EntitySystem)this).TerminatingOrDeleted(Entity<EntityTurnInvisibleComponent>.op_Implicit(ent), (MetaDataComponent)null) && ((EntitySystem)this).TryComp<SpriteComponent>(Entity<EntityTurnInvisibleComponent>.op_Implicit(ent), ref val))
		{
			val.PostShader = null;
		}
	}

	public override void Update(float frameTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<EntityTurnInvisibleComponent, SpriteComponent> val = ((EntitySystem)this).EntityQueryEnumerator<EntityTurnInvisibleComponent, SpriteComponent>();
		EntityUid val2 = default(EntityUid);
		EntityTurnInvisibleComponent entityTurnInvisibleComponent = default(EntityTurnInvisibleComponent);
		SpriteComponent val3 = default(SpriteComponent);
		EntityActiveInvisibleComponent entityActiveInvisibleComponent = default(EntityActiveInvisibleComponent);
		while (val.MoveNext(ref val2, ref entityTurnInvisibleComponent, ref val3))
		{
			float num = (((EntitySystem)this).TryComp<EntityActiveInvisibleComponent>(val2, ref entityActiveInvisibleComponent) ? entityActiveInvisibleComponent.Opacity : 1f);
			ShaderInstance postShader = val3.PostShader;
			if (postShader != null)
			{
				postShader.SetParameter("visibility", num);
			}
		}
	}
}
