using System;
using Content.Shared.NameModifier.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.NameModifier.EntitySystems;

public sealed class NameModifierSystem : EntitySystem
{
	[Dependency]
	private MetaDataSystem _metaData;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<NameModifierComponent, EntityRenamedEvent>((EntityEventRefHandler<NameModifierComponent, EntityRenamedEvent>)OnEntityRenamed, (Type[])null, (Type[])null);
	}

	private void OnEntityRenamed(Entity<NameModifierComponent> ent, ref EntityRenamedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		SetBaseName(ent, ((EntityRenamedEvent)(ref args)).NewName);
		RefreshNameModifiers(Entity<NameModifierComponent>.op_Implicit((ent.Owner, ent.Comp)));
	}

	private void SetBaseName(Entity<NameModifierComponent> entity, string name)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (!(name == entity.Comp.BaseName))
		{
			entity.Comp.BaseName = name;
			((EntitySystem)this).Dirty<NameModifierComponent>(entity, (MetaDataComponent)null);
		}
	}

	public string GetBaseName(Entity<NameModifierComponent?> entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<NameModifierComponent>(Entity<NameModifierComponent>.op_Implicit(entity), ref entity.Comp, false))
		{
			return entity.Comp.BaseName;
		}
		return ((EntitySystem)this).Name(Entity<NameModifierComponent>.op_Implicit(entity), (MetaDataComponent)null);
	}

	public void RefreshNameModifiers(Entity<NameModifierComponent?> entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		MetaDataComponent meta = ((EntitySystem)this).MetaData(Entity<NameModifierComponent>.op_Implicit(entity));
		string baseName = meta.EntityName;
		if (((EntitySystem)this).Resolve<NameModifierComponent>(Entity<NameModifierComponent>.op_Implicit(entity), ref entity.Comp, false))
		{
			baseName = entity.Comp.BaseName;
		}
		RefreshNameModifiersEvent modifierEvent = new RefreshNameModifiersEvent(baseName);
		((EntitySystem)this).RaiseLocalEvent<RefreshNameModifiersEvent>(Entity<NameModifierComponent>.op_Implicit(entity), ref modifierEvent, false);
		if (modifierEvent.ModifierCount == 0)
		{
			if (entity.Comp != null)
			{
				_metaData.SetEntityName(Entity<NameModifierComponent>.op_Implicit(entity), entity.Comp.BaseName, meta, false);
				((EntitySystem)this).RemComp<NameModifierComponent>(Entity<NameModifierComponent>.op_Implicit(entity));
			}
			return;
		}
		string modifiedName = modifierEvent.GetModifiedName();
		NameModifierComponent comp = default(NameModifierComponent);
		if (!((EntitySystem)this).EnsureComp<NameModifierComponent>(Entity<NameModifierComponent>.op_Implicit(entity), ref comp))
		{
			SetBaseName(Entity<NameModifierComponent>.op_Implicit((Entity<NameModifierComponent>.op_Implicit(entity), comp)), meta.EntityName);
		}
		_metaData.SetEntityName(Entity<NameModifierComponent>.op_Implicit(entity), modifiedName, meta, false);
	}
}
