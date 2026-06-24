// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Xenonids.XenoVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._RMC14.Sprite;
using Content.Shared._RMC14.Sprite;
using Content.Shared._RMC14.Xenonids;
using Content.Shared._RMC14.Xenonids.Charge;
using Content.Shared._RMC14.Xenonids.Egg;
using Content.Shared._RMC14.Xenonids.Leap;
using Content.Shared._RMC14.Xenonids.Movement;
using Content.Shared._RMC14.Xenonids.Parasite;
using Content.Shared._RMC14.Xenonids.Rest;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.StatusEffect;
using Content.Shared.Stunnable;
using Content.Shared.Throwing;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client._RMC14.Xenonids;

public sealed class XenoVisualizerSystem : VisualizerSystem<XenoComponent>
{
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private RMCSpriteSystem _rmcSprite;
  [Dependency]
  private SpriteSystem _sprite;
  private EntityQuery<XenoAnimateMovementComponent> _animateQuery;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<XenoComponent, KnockedDownEvent>(new EntityEventRefHandler<XenoComponent, KnockedDownEvent>((object) this, __methodptr(OnXenoKnockedDown)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<XenoComponent, StatusEffectEndedEvent>(new EntityEventRefHandler<XenoComponent, StatusEffectEndedEvent>((object) this, __methodptr(OnXenoStatusEffectEnded)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<XenoComponent, GetDrawDepthEvent>(new EntityEventRefHandler<XenoComponent, GetDrawDepthEvent>((object) this, __methodptr(OnXenoGetDrawDepth)), (Type[]) null, (Type[]) null);
    this._animateQuery = ((EntitySystem) this).GetEntityQuery<XenoAnimateMovementComponent>();
  }

  private void OnXenoKnockedDown(Entity<XenoComponent> xeno, ref KnockedDownEvent args)
  {
    this.UpdateSprite(Entity<SpriteComponent, MobStateComponent, AppearanceComponent, InputMoverComponent, ThrownItemComponent, XenoLeapingComponent, KnockedDownComponent>.op_Implicit(xeno.Owner));
  }

  private void OnXenoStatusEffectEnded(Entity<XenoComponent> xeno, ref StatusEffectEndedEvent args)
  {
    this.UpdateSprite(Entity<SpriteComponent, MobStateComponent, AppearanceComponent, InputMoverComponent, ThrownItemComponent, XenoLeapingComponent, KnockedDownComponent>.op_Implicit(xeno.Owner));
  }

  private void OnXenoGetDrawDepth(Entity<XenoComponent> ent, ref GetDrawDepthEvent args)
  {
    if (!this._mobState.IsDead(Entity<XenoComponent>.op_Implicit(ent)) || args.DrawDepth <= Content.Shared.DrawDepth.DrawDepth.DeadMobs)
      return;
    args.DrawDepth = Content.Shared.DrawDepth.DrawDepth.DeadMobs;
  }

  protected virtual void OnAppearanceChange(
    EntityUid uid,
    XenoComponent component,
    ref AppearanceChangeEvent args)
  {
    SpriteComponent sprite = args.Sprite;
    this.UpdateSprite(Entity<SpriteComponent, MobStateComponent, AppearanceComponent, InputMoverComponent, ThrownItemComponent, XenoLeapingComponent, KnockedDownComponent>.op_Implicit((uid, sprite, (MobStateComponent) null, args.Component, (InputMoverComponent) null, (ThrownItemComponent) null)));
    int num = (int) this._rmcSprite.UpdateDrawDepth(uid);
  }

  public void UpdateSprite(
    Entity<SpriteComponent?, MobStateComponent?, AppearanceComponent?, InputMoverComponent?, ThrownItemComponent?, XenoLeapingComponent?, KnockedDownComponent?> entity)
  {
    EntityUid entityUid;
    SpriteComponent spriteComponent1;
    MobStateComponent mobStateComponent1;
    AppearanceComponent appearanceComponent1;
    InputMoverComponent inputMoverComponent1;
    ThrownItemComponent thrownItemComponent1;
    XenoLeapingComponent leapingComponent1;
    KnockedDownComponent knockedDownComponent1;
    entity.Deconstruct(ref entityUid, ref spriteComponent1, ref mobStateComponent1, ref appearanceComponent1, ref inputMoverComponent1, ref thrownItemComponent1, ref leapingComponent1, ref knockedDownComponent1);
    SpriteComponent spriteComponent2 = spriteComponent1;
    MobStateComponent mobStateComponent2 = mobStateComponent1;
    AppearanceComponent appearanceComponent2 = appearanceComponent1;
    InputMoverComponent inputMoverComponent2 = inputMoverComponent1;
    ThrownItemComponent thrownItemComponent2 = thrownItemComponent1;
    XenoLeapingComponent leapingComponent2 = leapingComponent1;
    KnockedDownComponent knockedDownComponent2 = knockedDownComponent1;
    if (!((EntitySystem) this).Resolve<SpriteComponent, AppearanceComponent>(Entity<SpriteComponent, MobStateComponent, AppearanceComponent, InputMoverComponent, ThrownItemComponent, XenoLeapingComponent, KnockedDownComponent>.op_Implicit(entity), ref spriteComponent2, ref appearanceComponent2, false))
      return;
    MobState mobState = MobState.Alive;
    if (((EntitySystem) this).Resolve<MobStateComponent>(Entity<SpriteComponent, MobStateComponent, AppearanceComponent, InputMoverComponent, ThrownItemComponent, XenoLeapingComponent, KnockedDownComponent>.op_Implicit(entity), ref mobStateComponent2, false))
      mobState = mobStateComponent2.CurrentState;
    ((EntitySystem) this).Resolve<InputMoverComponent, ThrownItemComponent, XenoLeapingComponent, KnockedDownComponent>(Entity<SpriteComponent, MobStateComponent, AppearanceComponent, InputMoverComponent, ThrownItemComponent, XenoLeapingComponent, KnockedDownComponent>.op_Implicit(entity), ref inputMoverComponent2, ref thrownItemComponent2, ref leapingComponent2, ref knockedDownComponent2, false);
    if (knockedDownComponent2 != null && mobState != MobState.Dead)
      mobState = MobState.Critical;
    if (spriteComponent2 == null)
      return;
    RSI baseRsi = spriteComponent2.BaseRSI;
    int num1;
    if (baseRsi == null || !this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((entity.Owner, spriteComponent2)), (Enum) XenoVisualLayers.Base, ref num1, false))
      return;
    string str = (string) null;
    RSI.State state1;
    switch (mobState)
    {
      case MobState.Critical:
        if (baseRsi.TryGetState(RSI.StateId.op_Implicit("crit"), ref state1))
        {
          this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((entity.Owner, spriteComponent2)), num1, RSI.StateId.op_Implicit("crit"));
          break;
        }
        break;
      case MobState.Dead:
        if (((EntitySystem) this).HasComp<ParasiteSpentComponent>(Entity<SpriteComponent, MobStateComponent, AppearanceComponent, InputMoverComponent, ThrownItemComponent, XenoLeapingComponent, KnockedDownComponent>.op_Implicit(entity)))
        {
          this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((entity.Owner, spriteComponent2)), num1, RSI.StateId.op_Implicit("impregnated"));
          break;
        }
        if (baseRsi.TryGetState(RSI.StateId.op_Implicit("dead"), ref state1))
        {
          this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((entity.Owner, spriteComponent2)), num1, RSI.StateId.op_Implicit("dead"));
          break;
        }
        break;
      default:
        XenoOvipositorCapableComponent capableComponent;
        if (((EntitySystem) this).HasComp<XenoAttachedOvipositorComponent>(Entity<SpriteComponent, MobStateComponent, AppearanceComponent, InputMoverComponent, ThrownItemComponent, XenoLeapingComponent, KnockedDownComponent>.op_Implicit(entity)) && ((EntitySystem) this).TryComp<XenoOvipositorCapableComponent>(Entity<SpriteComponent, MobStateComponent, AppearanceComponent, InputMoverComponent, ThrownItemComponent, XenoLeapingComponent, KnockedDownComponent>.op_Implicit(entity), ref capableComponent))
        {
          str = capableComponent.AttachedState;
          break;
        }
        XenoRestState xenoRestState;
        RSI.State state2;
        if (((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<XenoRestState>(Entity<SpriteComponent, MobStateComponent, AppearanceComponent, InputMoverComponent, ThrownItemComponent, XenoLeapingComponent, KnockedDownComponent>.op_Implicit(entity), (Enum) XenoVisualLayers.Base, ref xenoRestState, appearanceComponent2) && xenoRestState == XenoRestState.Resting && baseRsi.TryGetState(RSI.StateId.op_Implicit("sleeping"), ref state2))
        {
          this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((entity.Owner, spriteComponent2)), num1, RSI.StateId.op_Implicit("sleeping"));
          break;
        }
        if (baseRsi.TryGetState(RSI.StateId.op_Implicit("thrown"), ref state2) && this.IsThrown(Entity<XenoLeapingComponent, ThrownItemComponent, ActiveXenoToggleChargingComponent>.op_Implicit((Entity<SpriteComponent, MobStateComponent, AppearanceComponent, InputMoverComponent, ThrownItemComponent, XenoLeapingComponent, KnockedDownComponent>.op_Implicit(entity), leapingComponent2, thrownItemComponent2, (ActiveXenoToggleChargingComponent) null))))
        {
          this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((entity.Owner, spriteComponent2)), num1, RSI.StateId.op_Implicit("thrown"));
          break;
        }
        bool flag1;
        if (((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(Entity<SpriteComponent, MobStateComponent, AppearanceComponent, InputMoverComponent, ThrownItemComponent, XenoLeapingComponent, KnockedDownComponent>.op_Implicit(entity), (Enum) XenoVisualLayers.Fortify, ref flag1, appearanceComponent2) & flag1 && baseRsi.TryGetState(RSI.StateId.op_Implicit("fortify"), ref state2))
        {
          this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((entity.Owner, spriteComponent2)), num1, RSI.StateId.op_Implicit("fortify"));
          break;
        }
        bool flag2;
        if (((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(Entity<SpriteComponent, MobStateComponent, AppearanceComponent, InputMoverComponent, ThrownItemComponent, XenoLeapingComponent, KnockedDownComponent>.op_Implicit(entity), (Enum) XenoVisualLayers.Crest, ref flag2, appearanceComponent2) & flag2 && baseRsi.TryGetState(RSI.StateId.op_Implicit("crest"), ref state2))
        {
          this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((entity.Owner, spriteComponent2)), num1, RSI.StateId.op_Implicit("crest"));
          break;
        }
        bool flag3;
        if (((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(Entity<SpriteComponent, MobStateComponent, AppearanceComponent, InputMoverComponent, ThrownItemComponent, XenoLeapingComponent, KnockedDownComponent>.op_Implicit(entity), (Enum) XenoVisualLayers.Burrow, ref flag3, appearanceComponent2) & flag3 && baseRsi.TryGetState(RSI.StateId.op_Implicit("burrowed"), ref state2))
        {
          this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((entity.Owner, spriteComponent2)), num1, RSI.StateId.op_Implicit("burrowed"));
          break;
        }
        if (inputMoverComponent2 != null && inputMoverComponent2.HeldMoveButtons > MoveButtons.None && inputMoverComponent2.HeldMoveButtons != MoveButtons.Walk && baseRsi.TryGetState(RSI.StateId.op_Implicit("running"), ref state2))
        {
          this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((entity.Owner, spriteComponent2)), num1, RSI.StateId.op_Implicit("running"));
          break;
        }
        if (baseRsi.TryGetState(RSI.StateId.op_Implicit("alive"), ref state2))
        {
          this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((entity.Owner, spriteComponent2)), num1, RSI.StateId.op_Implicit("alive"));
          break;
        }
        break;
    }
    int num2;
    if (!this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((entity.Owner, spriteComponent2)), (Enum) XenoVisualLayers.Ovipositor, ref num2, false))
      return;
    if (str == null)
    {
      this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((entity.Owner, spriteComponent2)), num2, false);
      this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((entity.Owner, spriteComponent2)), num1, true);
    }
    else
    {
      this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((entity.Owner, spriteComponent2)), num2, RSI.StateId.op_Implicit(str));
      this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((entity.Owner, spriteComponent2)), num2, true);
      this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((entity.Owner, spriteComponent2)), num1, false);
    }
  }

  private bool IsThrown(
    Entity<XenoLeapingComponent?, ThrownItemComponent?, ActiveXenoToggleChargingComponent?> xeno)
  {
    if (xeno.Comp1 != null || xeno.Comp2 != null)
      return true;
    return ((EntitySystem) this).Resolve<ActiveXenoToggleChargingComponent>(Entity<XenoLeapingComponent, ThrownItemComponent, ActiveXenoToggleChargingComponent>.op_Implicit(xeno), ref xeno.Comp3, false) && xeno.Comp3.Stage > 0;
  }

  public virtual void Update(float frameTime)
  {
    EntityQueryEnumerator<XenoComponent> entityQueryEnumerator = ((EntitySystem) this).EntityQueryEnumerator<XenoComponent>();
    EntityUid entityUid;
    XenoComponent xenoComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref xenoComponent))
    {
      if (!this._animateQuery.HasComp(entityUid))
        this.UpdateSprite(Entity<SpriteComponent, MobStateComponent, AppearanceComponent, InputMoverComponent, ThrownItemComponent, XenoLeapingComponent, KnockedDownComponent>.op_Implicit(entityUid));
    }
  }

  public virtual void FrameUpdate(float frameTime)
  {
    EntityQueryEnumerator<XenoAnimateMovementComponent> entityQueryEnumerator = ((EntitySystem) this).EntityQueryEnumerator<XenoAnimateMovementComponent>();
    EntityUid entityUid;
    XenoAnimateMovementComponent movementComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref movementComponent))
      this.UpdateSprite(Entity<SpriteComponent, MobStateComponent, AppearanceComponent, InputMoverComponent, ThrownItemComponent, XenoLeapingComponent, KnockedDownComponent>.op_Implicit(entityUid));
  }
}
