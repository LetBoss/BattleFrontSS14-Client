// Decompiled with JetBrains decompiler
// Type: Content.Client.Wieldable.WieldableSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Movement.Components;
using Content.Client.Movement.Systems;
using Content.Shared.Camera;
using Content.Shared.Hands;
using Content.Shared.Movement.Components;
using Content.Shared.Wieldable;
using Content.Shared.Wieldable.Components;
using Robust.Client.Timing;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.Wieldable;

public sealed class WieldableSystem : SharedWieldableSystem
{
  [Dependency]
  private EyeCursorOffsetSystem _eyeOffset;
  [Dependency]
  private IClientGameTiming _gameTiming;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CursorOffsetRequiresWieldComponent, ItemUnwieldedEvent>(new EntityEventRefHandler<CursorOffsetRequiresWieldComponent, ItemUnwieldedEvent>((object) this, __methodptr(OnEyeOffsetUnwielded)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CursorOffsetRequiresWieldComponent, HeldRelayedEvent<GetEyeOffsetRelayedEvent>>(new EntityEventRefHandler<CursorOffsetRequiresWieldComponent, HeldRelayedEvent<GetEyeOffsetRelayedEvent>>((object) this, __methodptr(OnGetEyeOffset)), (Type[]) null, (Type[]) null);
  }

  public void OnEyeOffsetUnwielded(
    Entity<CursorOffsetRequiresWieldComponent> entity,
    ref ItemUnwieldedEvent args)
  {
    EyeCursorOffsetComponent cursorOffsetComponent;
    if (!this.TryComp<EyeCursorOffsetComponent>(entity.Owner, ref cursorOffsetComponent) || !((IGameTiming) this._gameTiming).IsFirstTimePredicted)
      return;
    cursorOffsetComponent.CurrentPosition = Vector2.Zero;
  }

  public void OnGetEyeOffset(
    Entity<CursorOffsetRequiresWieldComponent> entity,
    ref HeldRelayedEvent<GetEyeOffsetRelayedEvent> args)
  {
    WieldableComponent wieldableComponent;
    if (!this.TryComp<WieldableComponent>(entity.Owner, ref wieldableComponent) || !wieldableComponent.Wielded)
      return;
    Vector2? nullable = this._eyeOffset.OffsetAfterMouse(entity.Owner, (EyeCursorOffsetComponent) null);
    if (!nullable.HasValue)
      return;
    args.Args.Offset += nullable.Value;
  }
}
