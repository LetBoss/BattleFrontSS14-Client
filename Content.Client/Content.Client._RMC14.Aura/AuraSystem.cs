using System;
using Content.Shared._RMC14.Aura;
using Content.Shared._RMC14.Stealth;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Client._RMC14.Aura;

public sealed class AuraSystem : SharedAuraSystem
{
	[Dependency]
	private IPrototypeManager _prototypes;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<AuraComponent, ComponentStartup>((EntityEventRefHandler<AuraComponent, ComponentStartup>)OnStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AuraComponent, ComponentShutdown>((EntityEventRefHandler<AuraComponent, ComponentShutdown>)OnShutdown, (Type[])null, (Type[])null);
	}

	private void OnStartup(Entity<AuraComponent> ent, ref ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent val = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(Entity<AuraComponent>.op_Implicit(ent), ref val))
		{
			val.PostShader = _prototypes.Index<ShaderPrototype>(new ProtoId<ShaderPrototype>("RMCAuraOutline")).InstanceUnique();
		}
	}

	private void OnShutdown(Entity<AuraComponent> ent, ref ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent val = default(SpriteComponent);
		if (!((EntitySystem)this).TerminatingOrDeleted(Entity<AuraComponent>.op_Implicit(ent), (MetaDataComponent)null) && !((EntitySystem)this).HasComp<EntityActiveInvisibleComponent>(Entity<AuraComponent>.op_Implicit(ent)) && ((EntitySystem)this).TryComp<SpriteComponent>(Entity<AuraComponent>.op_Implicit(ent), ref val))
		{
			val.PostShader = null;
		}
	}

	public override void Update(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		base.Update(frameTime);
		EntityQueryEnumerator<AuraComponent, SpriteComponent> val = ((EntitySystem)this).EntityQueryEnumerator<AuraComponent, SpriteComponent>();
		EntityUid val2 = default(EntityUid);
		AuraComponent auraComponent = default(AuraComponent);
		SpriteComponent val3 = default(SpriteComponent);
		while (val.MoveNext(ref val2, ref auraComponent, ref val3))
		{
			ShaderInstance postShader = val3.PostShader;
			if (postShader != null)
			{
				postShader.SetParameter("outline_color", auraComponent.Color);
			}
			ShaderInstance postShader2 = val3.PostShader;
			if (postShader2 != null)
			{
				postShader2.SetParameter("outline_width", auraComponent.OutlineWidth);
			}
		}
	}
}
