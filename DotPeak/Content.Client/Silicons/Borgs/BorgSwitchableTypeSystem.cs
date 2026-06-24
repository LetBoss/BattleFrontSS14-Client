// Decompiled with JetBrains decompiler
// Type: Content.Client.Silicons.Borgs.BorgSwitchableTypeSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Movement.Components;
using Content.Shared.Silicons.Borgs;
using Content.Shared.Silicons.Borgs.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.Silicons.Borgs;

public sealed class BorgSwitchableTypeSystem : SharedBorgSwitchableTypeSystem
{
  [Dependency]
  private BorgSystem _borgSystem;
  [Dependency]
  private AppearanceSystem _appearance;
  [Dependency]
  private SpriteSystem _sprite;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BorgSwitchableTypeComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<BorgSwitchableTypeComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(AfterStateHandler)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BorgSwitchableTypeComponent, ComponentStartup>(new EntityEventRefHandler<BorgSwitchableTypeComponent, ComponentStartup>((object) this, __methodptr(OnComponentStartup)), (Type[]) null, (Type[]) null);
  }

  private void OnComponentStartup(
    Entity<BorgSwitchableTypeComponent> ent,
    ref ComponentStartup args)
  {
    this.UpdateEntityAppearance(ent);
  }

  private void AfterStateHandler(
    Entity<BorgSwitchableTypeComponent> ent,
    ref AfterAutoHandleStateEvent args)
  {
    this.UpdateEntityAppearance(ent);
  }

  protected override void UpdateEntityAppearance(
    Entity<BorgSwitchableTypeComponent> entity,
    BorgTypePrototype prototype)
  {
    SpriteComponent spriteComponent;
    if (this.TryComp<SpriteComponent>(Entity<BorgSwitchableTypeComponent>.op_Implicit(entity), ref spriteComponent))
    {
      this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((Entity<BorgSwitchableTypeComponent>.op_Implicit(entity), spriteComponent)), (Enum) BorgVisualLayers.Body, RSI.StateId.op_Implicit(prototype.SpriteBodyState));
      this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((Entity<BorgSwitchableTypeComponent>.op_Implicit(entity), spriteComponent)), (Enum) BorgVisualLayers.LightStatus, RSI.StateId.op_Implicit(prototype.SpriteToggleLightState));
    }
    BorgChassisComponent chassisComponent;
    if (this.TryComp<BorgChassisComponent>(Entity<BorgSwitchableTypeComponent>.op_Implicit(entity), ref chassisComponent))
    {
      this._borgSystem.SetMindStates(Entity<BorgChassisComponent>.op_Implicit((entity.Owner, chassisComponent)), prototype.SpriteHasMindState, prototype.SpriteNoMindState);
      AppearanceComponent appearanceComponent;
      if (this.TryComp<AppearanceComponent>(Entity<BorgSwitchableTypeComponent>.op_Implicit(entity), ref appearanceComponent))
        ((SharedAppearanceSystem) this._appearance).QueueUpdate(Entity<BorgSwitchableTypeComponent>.op_Implicit(entity), appearanceComponent);
    }
    string bodyMovementState = prototype.SpriteBodyMovementState;
    if (bodyMovementState != null)
    {
      SpriteMovementComponent movementComponent = this.EnsureComp<SpriteMovementComponent>(Entity<BorgSwitchableTypeComponent>.op_Implicit(entity));
      movementComponent.NoMovementLayers.Clear();
      movementComponent.NoMovementLayers["movement"] = new PrototypeLayerData()
      {
        State = prototype.SpriteBodyState
      };
      movementComponent.MovementLayers.Clear();
      movementComponent.MovementLayers["movement"] = new PrototypeLayerData()
      {
        State = bodyMovementState
      };
    }
    else
      this.RemComp<SpriteMovementComponent>(Entity<BorgSwitchableTypeComponent>.op_Implicit(entity));
    base.UpdateEntityAppearance(entity, prototype);
  }
}
