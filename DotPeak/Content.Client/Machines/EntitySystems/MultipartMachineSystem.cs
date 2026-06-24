// Decompiled with JetBrains decompiler
// Type: Content.Client.Machines.EntitySystems.MultipartMachineSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

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
using System;

#nullable enable
namespace Content.Client.Machines.EntitySystems;

public sealed class MultipartMachineSystem : SharedMultipartMachineSystem
{
  private readonly EntProtoId _ghostPrototype = EntProtoId.op_Implicit("MultipartMachineGhost");
  private readonly Color _partiallyTransparent = new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte) 180);
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
    // ISSUE: method pointer
    this.SubscribeLocalEvent<MultipartMachineComponent, ClientExaminedEvent>(new EntityEventRefHandler<MultipartMachineComponent, ClientExaminedEvent>((object) this, __methodptr(OnMachineExamined)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<MultipartMachineComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<MultipartMachineComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<MultipartMachineGhostComponent, TimedDespawnEvent>(new EntityEventRefHandler<MultipartMachineGhostComponent, TimedDespawnEvent>((object) this, __methodptr(OnGhostDespawned)), (Type[]) null, (Type[]) null);
  }

  private void OnMachineExamined(
    Entity<MultipartMachineComponent> ent,
    ref ClientExaminedEvent args)
  {
    if (ent.Comp.Ghosts.Count != 0)
      return;
    foreach (MachinePart machinePart in ent.Comp.Parts.Values)
    {
      if (!machinePart.Entity.HasValue)
      {
        EntityCoordinates entityCoordinates;
        // ISSUE: explicit constructor call
        ((EntityCoordinates) ref entityCoordinates).\u002Ector(ent.Owner, Vector2i.op_Implicit(machinePart.Offset));
        EntityUid entityUid = this.Spawn(EntProtoId.op_Implicit(this._ghostPrototype), entityCoordinates);
        TransformComponent transformComponent;
        if (!this.XformQuery.TryGetComponent(entityUid, ref transformComponent))
          break;
        transformComponent.LocalRotation = machinePart.Rotation;
        this.Comp<MultipartMachineGhostComponent>(entityUid).LinkedMachine = new EntityUid?(Entity<MultipartMachineComponent>.op_Implicit(ent));
        ent.Comp.Ghosts.Add(entityUid);
        if (machinePart.GhostProto.HasValue)
        {
          EntityPrototype entityPrototype = this._prototype.Index(machinePart.GhostProto.Value);
          IComponent icomponent;
          if (!entityPrototype.Components.TryGetComponent("Sprite", ref icomponent) || !(icomponent is SpriteComponent spriteComponent1))
            break;
          SpriteComponent spriteComponent2 = this.EnsureComp<SpriteComponent>(entityUid);
          this._serialization.CopyTo<SpriteComponent>(spriteComponent1, ref spriteComponent2, (ISerializationContext) null, false, true);
          this._sprite.SetColor(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent2)), this._partiallyTransparent);
          this._metaData.SetEntityName(entityUid, entityPrototype.Name, (MetaDataComponent) null, true);
          this._metaData.SetEntityDescription(entityUid, entityPrototype.Description, (MetaDataComponent) null);
        }
      }
    }
  }

  private void OnHandleState(
    Entity<MultipartMachineComponent> ent,
    ref AfterAutoHandleStateEvent args)
  {
    foreach (MachinePart machinePart in ent.Comp.Parts.Values)
      machinePart.Entity = machinePart.NetEntity.HasValue ? new EntityUid?(this.EnsureEntity<MultipartMachinePartComponent>(machinePart.NetEntity.Value, Entity<MultipartMachineComponent>.op_Implicit(ent))) : new EntityUid?();
  }

  private void OnGhostDespawned(
    Entity<MultipartMachineGhostComponent> ent,
    ref TimedDespawnEvent args)
  {
    MultipartMachineComponent machineComponent;
    if (!this.TryComp<MultipartMachineComponent>(ent.Comp.LinkedMachine, ref machineComponent))
      return;
    machineComponent.Ghosts.Remove(Entity<MultipartMachineGhostComponent>.op_Implicit(ent));
  }
}
