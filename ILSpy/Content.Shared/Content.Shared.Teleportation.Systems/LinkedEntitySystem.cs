using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Content.Shared.Teleportation.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Teleportation.Systems;

public sealed class LinkedEntitySystem : EntitySystem
{
	[Dependency]
	private SharedAppearanceSystem _appearance;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<LinkedEntityComponent, ComponentShutdown>((ComponentEventHandler<LinkedEntityComponent, ComponentShutdown>)OnLinkShutdown, (Type[])null, (Type[])null);
	}

	private void OnLinkShutdown(EntityUid uid, LinkedEntityComponent component, ComponentShutdown args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Invalid comparison between Unknown and I4
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		EntityUid[] array = component.LinkedEntities.ToArray();
		LinkedEntityComponent link = default(LinkedEntityComponent);
		foreach (EntityUid ent in array)
		{
			if (!((EntitySystem)this).Deleted(ent, (MetaDataComponent)null) && (int)((EntitySystem)this).LifeStage(ent, (MetaDataComponent)null) < 4 && ((EntitySystem)this).TryComp<LinkedEntityComponent>(ent, ref link))
			{
				TryUnlink(uid, ent, component, link);
			}
		}
	}

	public bool TryLink(EntityUid first, EntityUid second, bool deleteOnEmptyLinks = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		LinkedEntityComponent firstLink = ((EntitySystem)this).EnsureComp<LinkedEntityComponent>(first);
		LinkedEntityComponent secondLink = ((EntitySystem)this).EnsureComp<LinkedEntityComponent>(second);
		firstLink.DeleteOnEmptyLinks = deleteOnEmptyLinks;
		secondLink.DeleteOnEmptyLinks = deleteOnEmptyLinks;
		_appearance.SetData(first, (Enum)LinkedEntityVisuals.HasAnyLinks, (object)true, (AppearanceComponent)null);
		_appearance.SetData(second, (Enum)LinkedEntityVisuals.HasAnyLinks, (object)true, (AppearanceComponent)null);
		((EntitySystem)this).Dirty(first, (IComponent)(object)firstLink, (MetaDataComponent)null);
		((EntitySystem)this).Dirty(second, (IComponent)(object)secondLink, (MetaDataComponent)null);
		if (firstLink.LinkedEntities.Add(second))
		{
			return secondLink.LinkedEntities.Add(first);
		}
		return false;
	}

	public bool OneWayLink(EntityUid source, EntityUid target, bool deleteOnEmptyLinks = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		LinkedEntityComponent firstLink = ((EntitySystem)this).EnsureComp<LinkedEntityComponent>(source);
		firstLink.DeleteOnEmptyLinks = deleteOnEmptyLinks;
		_appearance.SetData(source, (Enum)LinkedEntityVisuals.HasAnyLinks, (object)true, (AppearanceComponent)null);
		((EntitySystem)this).Dirty(source, (IComponent)(object)firstLink, (MetaDataComponent)null);
		return firstLink.LinkedEntities.Add(target);
	}

	public bool TryUnlink(EntityUid first, EntityUid second, LinkedEntityComponent? firstLink = null, LinkedEntityComponent? secondLink = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<LinkedEntityComponent>(first, ref firstLink, true))
		{
			return false;
		}
		if (!((EntitySystem)this).Resolve<LinkedEntityComponent>(second, ref secondLink, true))
		{
			return false;
		}
		bool result = firstLink.LinkedEntities.Remove(second) && secondLink.LinkedEntities.Remove(first);
		_appearance.SetData(first, (Enum)LinkedEntityVisuals.HasAnyLinks, (object)firstLink.LinkedEntities.Any(), (AppearanceComponent)null);
		_appearance.SetData(second, (Enum)LinkedEntityVisuals.HasAnyLinks, (object)secondLink.LinkedEntities.Any(), (AppearanceComponent)null);
		((EntitySystem)this).Dirty(first, (IComponent)(object)firstLink, (MetaDataComponent)null);
		((EntitySystem)this).Dirty(second, (IComponent)(object)secondLink, (MetaDataComponent)null);
		if (firstLink.LinkedEntities.Count == 0 && firstLink.DeleteOnEmptyLinks)
		{
			((EntitySystem)this).QueueDel((EntityUid?)first);
		}
		if (secondLink.LinkedEntities.Count == 0 && secondLink.DeleteOnEmptyLinks)
		{
			((EntitySystem)this).QueueDel((EntityUid?)second);
		}
		return result;
	}

	public bool GetLink(EntityUid uid, [NotNullWhen(true)] out EntityUid? dest, LinkedEntityComponent? comp = null)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		dest = null;
		if (!((EntitySystem)this).Resolve<LinkedEntityComponent>(uid, ref comp, false))
		{
			return false;
		}
		EntityUid first = comp.LinkedEntities.FirstOrDefault();
		if (first != default(EntityUid))
		{
			dest = first;
			return true;
		}
		return false;
	}
}
