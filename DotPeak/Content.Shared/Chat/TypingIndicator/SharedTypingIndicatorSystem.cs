// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chat.TypingIndicator.SharedTypingIndicatorSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.ActionBlocker;
using Content.Shared.Clothing;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.Chat.TypingIndicator;

public abstract class SharedTypingIndicatorSystem : EntitySystem
{
  [Dependency]
  private ActionBlockerSystem _actionBlocker;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private IGameTiming _timing;
  public static readonly ProtoId<TypingIndicatorPrototype> InitialIndicatorId = ProtoId<TypingIndicatorPrototype>.op_Implicit("default");

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<PlayerAttachedEvent>(new EntityEventHandler<PlayerAttachedEvent>(this.OnPlayerAttached), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<TypingIndicatorComponent, PlayerDetachedEvent>(new ComponentEventHandler<TypingIndicatorComponent, PlayerDetachedEvent>((object) this, __methodptr(OnPlayerDetached)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<TypingIndicatorClothingComponent, ClothingGotEquippedEvent>(new EntityEventRefHandler<TypingIndicatorClothingComponent, ClothingGotEquippedEvent>((object) this, __methodptr(OnGotEquipped)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<TypingIndicatorClothingComponent, ClothingGotUnequippedEvent>(new EntityEventRefHandler<TypingIndicatorClothingComponent, ClothingGotUnequippedEvent>((object) this, __methodptr(OnGotUnequipped)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<TypingIndicatorClothingComponent, InventoryRelayedEvent<BeforeShowTypingIndicatorEvent>>(new EntityEventRefHandler<TypingIndicatorClothingComponent, InventoryRelayedEvent<BeforeShowTypingIndicatorEvent>>((object) this, __methodptr(BeforeShow)), (Type[]) null, (Type[]) null);
    this.SubscribeAllEvent<TypingChangedEvent>(new EntitySessionEventHandler<TypingChangedEvent>(this.OnTypingChanged), (Type[]) null, (Type[]) null);
  }

  private void OnPlayerAttached(PlayerAttachedEvent ev)
  {
    this.EnsureComp<TypingIndicatorComponent>(ev.Entity);
    this.EnsureComp<AppearanceComponent>(ev.Entity);
  }

  private void OnPlayerDetached(
    EntityUid uid,
    TypingIndicatorComponent component,
    PlayerDetachedEvent args)
  {
    this.SetTypingIndicatorState(uid, TypingIndicatorState.None);
  }

  private void OnGotEquipped(
    Entity<TypingIndicatorClothingComponent> entity,
    ref ClothingGotEquippedEvent args)
  {
    entity.Comp.GotEquippedTime = new TimeSpan?(this._timing.CurTime);
  }

  private void OnGotUnequipped(
    Entity<TypingIndicatorClothingComponent> entity,
    ref ClothingGotUnequippedEvent args)
  {
    entity.Comp.GotEquippedTime = new TimeSpan?();
  }

  private void BeforeShow(
    Entity<TypingIndicatorClothingComponent> entity,
    ref InventoryRelayedEvent<BeforeShowTypingIndicatorEvent> args)
  {
    args.Args.TryUpdateTimeAndIndicator(new ProtoId<TypingIndicatorPrototype>?(entity.Comp.TypingIndicatorPrototype), entity.Comp.GotEquippedTime);
  }

  private void OnTypingChanged(TypingChangedEvent ev, EntitySessionEventArgs args)
  {
    EntityUid? attachedEntity = ((EntitySessionEventArgs) ref args).SenderSession.AttachedEntity;
    if (!this.Exists(attachedEntity))
      this.Log.Warning($"Client {((EntitySessionEventArgs) ref args).SenderSession} sent TypingChangedEvent without an attached entity.");
    else if (!this._actionBlocker.CanEmote(attachedEntity.Value) && !this._actionBlocker.CanSpeak(attachedEntity.Value))
      this.SetTypingIndicatorState(attachedEntity.Value, TypingIndicatorState.None);
    else
      this.SetTypingIndicatorState(attachedEntity.Value, ev.State);
  }

  private void SetTypingIndicatorState(
    EntityUid uid,
    TypingIndicatorState state,
    AppearanceComponent? appearance = null)
  {
    if (!this.Resolve<AppearanceComponent>(uid, ref appearance, false))
      return;
    this._appearance.SetData(uid, (Enum) TypingIndicatorVisuals.State, (object) state, appearance);
  }
}
