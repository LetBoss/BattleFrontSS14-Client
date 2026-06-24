using System;
using System.Collections.Generic;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Client.Movement.Systems;

public sealed class FloorOcclusionSystem : SharedFloorOcclusionSystem
{
	private static readonly ProtoId<ShaderPrototype> HorizontalCut = ProtoId<ShaderPrototype>.op_Implicit("HorizontalCut");

	[Dependency]
	private IPrototypeManager _proto;

	private readonly Dictionary<EntityUid, ProtoId<ShaderPrototype>> _appliedShaders = new Dictionary<EntityUid, ProtoId<ShaderPrototype>>();

	private EntityQuery<FloorOccluderShaderComponent> _occluderShaderQuery;

	private EntityQuery<SpriteComponent> _spriteQuery;

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize();
		_occluderShaderQuery = ((EntitySystem)this).GetEntityQuery<FloorOccluderShaderComponent>();
		_spriteQuery = ((EntitySystem)this).GetEntityQuery<SpriteComponent>();
		((EntitySystem)this).SubscribeLocalEvent<FloorOcclusionComponent, ComponentStartup>((EntityEventRefHandler<FloorOcclusionComponent, ComponentStartup>)OnOcclusionStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FloorOcclusionComponent, ComponentShutdown>((EntityEventRefHandler<FloorOcclusionComponent, ComponentShutdown>)OnOcclusionShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FloorOcclusionComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<FloorOcclusionComponent, AfterAutoHandleStateEvent>)OnOcclusionAuto, (Type[])null, (Type[])null);
	}

	private void OnOcclusionAuto(Entity<FloorOcclusionComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		ProtoId<ShaderPrototype> occlusionShader = GetOcclusionShader(ent);
		SetShader(Entity<SpriteComponent>.op_Implicit(ent.Owner), occlusionShader, ent.Comp.Enabled);
	}

	private void OnOcclusionStartup(Entity<FloorOcclusionComponent> ent, ref ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		ProtoId<ShaderPrototype> occlusionShader = GetOcclusionShader(ent);
		SetShader(Entity<SpriteComponent>.op_Implicit(ent.Owner), occlusionShader, ent.Comp.Enabled);
	}

	private void OnOcclusionShutdown(Entity<FloorOcclusionComponent> ent, ref ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		SetShader(Entity<SpriteComponent>.op_Implicit(ent.Owner), HorizontalCut, enabled: false);
	}

	protected override void SetEnabled(Entity<FloorOcclusionComponent> entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		ProtoId<ShaderPrototype> occlusionShader = GetOcclusionShader(entity);
		SetShader(Entity<SpriteComponent>.op_Implicit(entity.Owner), occlusionShader, entity.Comp.Enabled);
	}

	private ProtoId<ShaderPrototype> GetOcclusionShader(Entity<FloorOcclusionComponent> entity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		FloorOccluderShaderComponent floorOccluderShaderComponent = default(FloorOccluderShaderComponent);
		foreach (EntityUid item in entity.Comp.Colliding)
		{
			if (_occluderShaderQuery.TryComp(item, ref floorOccluderShaderComponent))
			{
				return ProtoId<ShaderPrototype>.op_Implicit(floorOccluderShaderComponent.Shader);
			}
		}
		return HorizontalCut;
	}

	private void SetShader(Entity<SpriteComponent?> sprite, ProtoId<ShaderPrototype> shaderId, bool enabled)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		if (!_spriteQuery.Resolve(sprite.Owner, ref sprite.Comp, false))
		{
			return;
		}
		ProtoId<ShaderPrototype> value;
		if (enabled)
		{
			ShaderInstance val = _proto.Index<ShaderPrototype>(shaderId).Instance();
			if (sprite.Comp.PostShader == null || _appliedShaders.ContainsKey(sprite.Owner) || sprite.Comp.PostShader == val)
			{
				sprite.Comp.PostShader = val;
				_appliedShaders[sprite.Owner] = shaderId;
			}
		}
		else if (_appliedShaders.TryGetValue(sprite.Owner, out value))
		{
			if (sprite.Comp.PostShader == _proto.Index<ShaderPrototype>(value).Instance())
			{
				sprite.Comp.PostShader = null;
			}
			_appliedShaders.Remove(sprite.Owner);
		}
	}
}
