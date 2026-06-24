using System;
using System.Collections.Generic;
using Content.Shared.Access.Components;
using Content.Shared.Roles;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.Access.Systems;

public abstract class SharedAccessSystem : EntitySystem
{
	[Dependency]
	private IPrototypeManager _prototypeManager;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<AccessComponent, MapInitEvent>((ComponentEventHandler<AccessComponent, MapInitEvent>)OnAccessInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AccessComponent, GetAccessTagsEvent>((ComponentEventRefHandler<AccessComponent, GetAccessTagsEvent>)OnGetAccessTags, (Type[])null, (Type[])null);
	}

	private void OnAccessInit(EntityUid uid, AccessComponent component, MapInitEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		AccessGroupPrototype proto = default(AccessGroupPrototype);
		foreach (ProtoId<AccessGroupPrototype> group in component.Groups)
		{
			if (_prototypeManager.TryIndex<AccessGroupPrototype>(group, ref proto))
			{
				component.Tags.UnionWith(proto.Tags);
				((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
			}
		}
	}

	private void OnGetAccessTags(EntityUid uid, AccessComponent component, ref GetAccessTagsEvent args)
	{
		if (component.Enabled)
		{
			args.Tags.UnionWith(component.Tags);
		}
	}

	public void SetAccessEnabled(EntityUid uid, bool val, AccessComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<AccessComponent>(uid, ref component, false))
		{
			component.Enabled = val;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		}
	}

	public bool TrySetTags(EntityUid uid, IEnumerable<ProtoId<AccessLevelPrototype>> newTags, AccessComponent? access = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<AccessComponent>(uid, ref access, true))
		{
			return false;
		}
		access.Tags.Clear();
		access.Tags.UnionWith(newTags);
		((EntitySystem)this).Dirty(uid, (IComponent)(object)access, (MetaDataComponent)null);
		return true;
	}

	public IEnumerable<ProtoId<AccessLevelPrototype>>? TryGetTags(EntityUid uid, AccessComponent? access = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<AccessComponent>(uid, ref access, true))
		{
			return access.Tags;
		}
		return null;
	}

	public bool TryAddGroups(EntityUid uid, IEnumerable<ProtoId<AccessGroupPrototype>> newGroups, AccessComponent? access = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<AccessComponent>(uid, ref access, true))
		{
			return false;
		}
		AccessGroupPrototype proto = default(AccessGroupPrototype);
		foreach (ProtoId<AccessGroupPrototype> group in newGroups)
		{
			if (_prototypeManager.TryIndex<AccessGroupPrototype>(group, ref proto))
			{
				access.Tags.UnionWith(proto.Tags);
			}
		}
		((EntitySystem)this).Dirty(uid, (IComponent)(object)access, (MetaDataComponent)null);
		return true;
	}

	public void SetAccessToJob(EntityUid uid, JobPrototype prototype, bool extended, AccessComponent? access = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<AccessComponent>(uid, ref access, true))
		{
			access.Tags.Clear();
			access.Tags.UnionWith(prototype.Access);
			((EntitySystem)this).Dirty(uid, (IComponent)(object)access, (MetaDataComponent)null);
			TryAddGroups(uid, prototype.AccessGroups, access);
			if (extended)
			{
				access.Tags.UnionWith(prototype.ExtendedAccess);
				TryAddGroups(uid, prototype.ExtendedAccessGroups, access);
			}
		}
	}
}
