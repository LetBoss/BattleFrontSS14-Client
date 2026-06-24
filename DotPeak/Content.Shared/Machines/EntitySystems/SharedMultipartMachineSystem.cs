// Decompiled with JetBrains decompiler
// Type: Content.Shared.Machines.EntitySystems.SharedMultipartMachineSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Machines.Components;
using Robust.Shared.GameObjects;
using System;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Shared.Machines.EntitySystems;

public abstract class SharedMultipartMachineSystem : EntitySystem
{
  protected Robust.Shared.GameObjects.EntityQuery<TransformComponent> XformQuery;

  public override void Initialize()
  {
    base.Initialize();
    this.XformQuery = this.GetEntityQuery<TransformComponent>();
  }

  public bool IsAssembled(Entity<MultipartMachineComponent?> ent)
  {
    if (!this.Resolve<MultipartMachineComponent>((EntityUid) ent, ref ent.Comp))
      return false;
    foreach (MachinePart machinePart in ent.Comp.Parts.Values)
    {
      if (!machinePart.Entity.HasValue && !machinePart.Optional)
        return false;
    }
    return true;
  }

  public bool HasPartEntity(Entity<MultipartMachineComponent?> machine, EntityUid entity)
  {
    if (!this.Resolve<MultipartMachineComponent>((EntityUid) machine, ref machine.Comp))
      return false;
    foreach (MachinePart machinePart in machine.Comp.Parts.Values)
    {
      if (machinePart.Entity.HasValue && machinePart.Entity.Value == entity)
        return true;
    }
    return false;
  }

  public EntityUid? GetPartEntity(Entity<MultipartMachineComponent?> ent, Enum part)
  {
    EntityUid? entity;
    return !this.TryGetPartEntity(ent, part, out entity) ? new EntityUid?() : entity;
  }

  public bool TryGetPartEntity(
    Entity<MultipartMachineComponent?> ent,
    Enum part,
    [NotNullWhen(true)] out EntityUid? entity)
  {
    entity = new EntityUid?();
    MachinePart machinePart;
    if (!this.Resolve<MultipartMachineComponent>((EntityUid) ent, ref ent.Comp) || !ent.Comp.Parts.TryGetValue(part, out machinePart) || !machinePart.Entity.HasValue)
      return false;
    entity = new EntityUid?(machinePart.Entity.Value);
    return true;
  }

  public bool HasPart(Entity<MultipartMachineComponent?> ent, Enum part)
  {
    MachinePart machinePart;
    return this.Resolve<MultipartMachineComponent>((EntityUid) ent, ref ent.Comp) && ent.Comp.Parts.TryGetValue(part, out machinePart) && machinePart.Entity.HasValue;
  }
}
