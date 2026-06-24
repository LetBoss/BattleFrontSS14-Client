using System;
using System.Collections.Generic;
using Content.Shared.Construction.Components;
using Content.Shared.Interaction;
using Content.Shared.Tag;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;

namespace Content.Shared.Construction;

public sealed class PartAssemblySystem : EntitySystem
{
	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private TagSystem _tag;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<PartAssemblyComponent, ComponentInit>((ComponentEventHandler<PartAssemblyComponent, ComponentInit>)OnInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PartAssemblyComponent, InteractUsingEvent>((ComponentEventHandler<PartAssemblyComponent, InteractUsingEvent>)OnInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PartAssemblyComponent, EntRemovedFromContainerMessage>((ComponentEventHandler<PartAssemblyComponent, EntRemovedFromContainerMessage>)OnEntRemoved, (Type[])null, (Type[])null);
	}

	private void OnInit(EntityUid uid, PartAssemblyComponent component, ComponentInit args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		component.PartsContainer = _container.EnsureContainer<Container>(uid, component.ContainerId, (ContainerManagerComponent)null);
	}

	private void OnInteractUsing(EntityUid uid, PartAssemblyComponent component, InteractUsingEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (TryInsertPart(args.Used, uid, component))
		{
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnEntRemoved(EntityUid uid, PartAssemblyComponent component, EntRemovedFromContainerMessage args)
	{
		if (!(((ContainerModifiedMessage)args).Container.ID != component.ContainerId) && ((BaseContainer)component.PartsContainer).ContainedEntities.Count == 0)
		{
			component.CurrentAssembly = null;
		}
	}

	public bool TryInsertPart(EntityUid part, EntityUid uid, PartAssemblyComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<PartAssemblyComponent>(uid, ref component, true))
		{
			return false;
		}
		string assemblyId = null;
		if (assemblyId == null)
		{
			assemblyId = component.CurrentAssembly;
		}
		if (assemblyId == null)
		{
			foreach (var (id, list2) in component.Parts)
			{
				foreach (string tag in list2)
				{
					if (_tag.HasTag(part, ProtoId<TagPrototype>.op_Implicit(tag)))
					{
						assemblyId = id;
						break;
					}
				}
				if (assemblyId != null)
				{
					break;
				}
			}
		}
		if (assemblyId == null)
		{
			return false;
		}
		if (!IsPartValid(uid, part, assemblyId, component))
		{
			return false;
		}
		component.CurrentAssembly = assemblyId;
		_container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(part), (BaseContainer)(object)component.PartsContainer, (TransformComponent)null, false);
		PartAssemblyPartInsertedEvent ev = new PartAssemblyPartInsertedEvent();
		((EntitySystem)this).RaiseLocalEvent<PartAssemblyPartInsertedEvent>(uid, ev, false);
		return true;
	}

	public bool IsPartValid(EntityUid uid, EntityUid part, string assemblyId, PartAssemblyComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<PartAssemblyComponent>(uid, ref component, false))
		{
			return true;
		}
		if (!component.Parts.TryGetValue(assemblyId, out List<string> tags))
		{
			return false;
		}
		List<string> openTags = new List<string>(tags);
		List<EntityUid> contained = new List<EntityUid>(((BaseContainer)component.PartsContainer).ContainedEntities);
		foreach (string tag in tags)
		{
			foreach (EntityUid ent in ((BaseContainer)component.PartsContainer).ContainedEntities)
			{
				if (contained.Contains(ent) && _tag.HasTag(ent, ProtoId<TagPrototype>.op_Implicit(tag)))
				{
					openTags.Remove(tag);
					contained.Remove(ent);
					break;
				}
			}
		}
		foreach (string tag2 in openTags)
		{
			if (_tag.HasTag(part, ProtoId<TagPrototype>.op_Implicit(tag2)))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsAssemblyFinished(EntityUid uid, string assemblyId, PartAssemblyComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<PartAssemblyComponent>(uid, ref component, false))
		{
			return true;
		}
		if (!component.Parts.TryGetValue(assemblyId, out List<string> parts))
		{
			return false;
		}
		List<EntityUid> contained = new List<EntityUid>(((BaseContainer)component.PartsContainer).ContainedEntities);
		foreach (string tag in parts)
		{
			bool valid = false;
			foreach (EntityUid ent in new List<EntityUid>(contained))
			{
				if (_tag.HasTag(ent, ProtoId<TagPrototype>.op_Implicit(tag)))
				{
					valid = true;
					contained.Remove(ent);
					break;
				}
			}
			if (!valid)
			{
				return false;
			}
		}
		return true;
	}
}
