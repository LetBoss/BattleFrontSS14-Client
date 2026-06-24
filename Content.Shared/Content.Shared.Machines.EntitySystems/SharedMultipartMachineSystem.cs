using System;
using System.Diagnostics.CodeAnalysis;
using Content.Shared.Machines.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Machines.EntitySystems;

public abstract class SharedMultipartMachineSystem : EntitySystem
{
	protected EntityQuery<TransformComponent> XformQuery;

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		XformQuery = ((EntitySystem)this).GetEntityQuery<TransformComponent>();
	}

	public bool IsAssembled(Entity<MultipartMachineComponent?> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MultipartMachineComponent>(Entity<MultipartMachineComponent>.op_Implicit(ent), ref ent.Comp, true))
		{
			return false;
		}
		foreach (MachinePart part in ent.Comp.Parts.Values)
		{
			if (!part.Entity.HasValue && !part.Optional)
			{
				return false;
			}
		}
		return true;
	}

	public bool HasPartEntity(Entity<MultipartMachineComponent?> machine, EntityUid entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MultipartMachineComponent>(Entity<MultipartMachineComponent>.op_Implicit(machine), ref machine.Comp, true))
		{
			return false;
		}
		foreach (MachinePart part in machine.Comp.Parts.Values)
		{
			if (part.Entity.HasValue && part.Entity.Value == entity)
			{
				return true;
			}
		}
		return false;
	}

	public EntityUid? GetPartEntity(Entity<MultipartMachineComponent?> ent, Enum part)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetPartEntity(ent, part, out var entity))
		{
			return null;
		}
		return entity;
	}

	public bool TryGetPartEntity(Entity<MultipartMachineComponent?> ent, Enum part, [NotNullWhen(true)] out EntityUid? entity)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		entity = null;
		if (!((EntitySystem)this).Resolve<MultipartMachineComponent>(Entity<MultipartMachineComponent>.op_Implicit(ent), ref ent.Comp, true))
		{
			return false;
		}
		if (ent.Comp.Parts.TryGetValue(part, out MachinePart value) && value.Entity.HasValue)
		{
			entity = value.Entity.Value;
			return true;
		}
		return false;
	}

	public bool HasPart(Entity<MultipartMachineComponent?> ent, Enum part)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MultipartMachineComponent>(Entity<MultipartMachineComponent>.op_Implicit(ent), ref ent.Comp, true))
		{
			return false;
		}
		if (!ent.Comp.Parts.TryGetValue(part, out MachinePart value))
		{
			return false;
		}
		return value.Entity.HasValue;
	}
}
