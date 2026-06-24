using System;
using Content.Client.Examine;
using Content.Client.Machines.Components;
using Content.Shared.Machines.Components;
using Content.Shared.Machines.EntitySystems;
using Robust.Client.GameObjects;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Spawners;

namespace Content.Client.Machines.EntitySystems;

public sealed class MultipartMachineSystem : SharedMultipartMachineSystem
{
	private readonly EntProtoId _ghostPrototype = EntProtoId.op_Implicit("MultipartMachineGhost");

	private readonly Color _partiallyTransparent = new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)180);

	[Dependency]
	private SpriteSystem _sprite;

	[Dependency]
	private IPrototypeManager _prototype;

	[Dependency]
	private MetaDataSystem _metaData;

	[Dependency]
	private ISerializationManager _serialization;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<MultipartMachineComponent, ClientExaminedEvent>((EntityEventRefHandler<MultipartMachineComponent, ClientExaminedEvent>)OnMachineExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MultipartMachineComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<MultipartMachineComponent, AfterAutoHandleStateEvent>)OnHandleState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MultipartMachineGhostComponent, TimedDespawnEvent>((EntityEventRefHandler<MultipartMachineGhostComponent, TimedDespawnEvent>)OnGhostDespawned, (Type[])null, (Type[])null);
	}

	private void OnMachineExamined(Entity<MultipartMachineComponent> ent, ref ClientExaminedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Ghosts.Count != 0)
		{
			return;
		}
		EntityCoordinates val = default(EntityCoordinates);
		TransformComponent val3 = default(TransformComponent);
		IComponent val5 = default(IComponent);
		foreach (MachinePart value in ent.Comp.Parts.Values)
		{
			if (value.Entity.HasValue)
			{
				continue;
			}
			((EntityCoordinates)(ref val))._002Ector(ent.Owner, Vector2i.op_Implicit(value.Offset));
			EntityUid val2 = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(_ghostPrototype), val);
			if (!XformQuery.TryGetComponent(val2, ref val3))
			{
				break;
			}
			val3.LocalRotation = value.Rotation;
			((EntitySystem)this).Comp<MultipartMachineGhostComponent>(val2).LinkedMachine = Entity<MultipartMachineComponent>.op_Implicit(ent);
			ent.Comp.Ghosts.Add(val2);
			if (value.GhostProto.HasValue)
			{
				EntityPrototype val4 = _prototype.Index(value.GhostProto.Value);
				if (!val4.Components.TryGetComponent("Sprite", ref val5))
				{
					break;
				}
				SpriteComponent val6 = (SpriteComponent)(object)((val5 is SpriteComponent) ? val5 : null);
				if (val6 == null)
				{
					break;
				}
				SpriteComponent item = ((EntitySystem)this).EnsureComp<SpriteComponent>(val2);
				_serialization.CopyTo<SpriteComponent>(val6, ref item, (ISerializationContext)null, false, true);
				_sprite.SetColor(Entity<SpriteComponent>.op_Implicit((val2, item)), _partiallyTransparent);
				_metaData.SetEntityName(val2, val4.Name, (MetaDataComponent)null, true);
				_metaData.SetEntityDescription(val2, val4.Description, (MetaDataComponent)null);
			}
		}
	}

	private void OnHandleState(Entity<MultipartMachineComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		foreach (MachinePart value in ent.Comp.Parts.Values)
		{
			value.Entity = (value.NetEntity.HasValue ? new EntityUid?(((EntitySystem)this).EnsureEntity<MultipartMachinePartComponent>(value.NetEntity.Value, Entity<MultipartMachineComponent>.op_Implicit(ent))) : ((EntityUid?)null));
		}
	}

	private void OnGhostDespawned(Entity<MultipartMachineGhostComponent> ent, ref TimedDespawnEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		MultipartMachineComponent multipartMachineComponent = default(MultipartMachineComponent);
		if (((EntitySystem)this).TryComp<MultipartMachineComponent>(ent.Comp.LinkedMachine, ref multipartMachineComponent))
		{
			multipartMachineComponent.Ghosts.Remove(Entity<MultipartMachineGhostComponent>.op_Implicit(ent));
		}
	}
}
