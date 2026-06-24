// Decompiled with JetBrains decompiler
// Type: Content.Client.Polymorph.Systems.ChameleonProjectorSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Effects;
using Content.Client.Smoking;
using Content.Shared.Chemistry.Components;
using Content.Shared.Polymorph.Components;
using Content.Shared.Polymorph.Systems;
using Robust.Client.GameObjects;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.Polymorph.Systems;

public sealed class ChameleonProjectorSystem : SharedChameleonProjectorSystem
{
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SpriteSystem _sprite;
  private EntityQuery<AppearanceComponent> _appearanceQuery;
  private EntityQuery<SpriteComponent> _spriteQuery;

  public override void Initialize()
  {
    base.Initialize();
    this._appearanceQuery = this.GetEntityQuery<AppearanceComponent>();
    this._spriteQuery = this.GetEntityQuery<SpriteComponent>();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ChameleonDisguiseComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<ChameleonDisguiseComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ChameleonDisguisedComponent, ComponentStartup>(new EntityEventRefHandler<ChameleonDisguisedComponent, ComponentStartup>((object) this, __methodptr(OnStartup)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ChameleonDisguisedComponent, ComponentShutdown>(new EntityEventRefHandler<ChameleonDisguisedComponent, ComponentShutdown>((object) this, __methodptr(OnShutdown)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ChameleonDisguisedComponent, GetFlashEffectTargetEvent>(new EntityEventRefHandler<ChameleonDisguisedComponent, GetFlashEffectTargetEvent>((object) this, __methodptr(OnGetFlashEffectTargetEvent)), (Type[]) null, (Type[]) null);
  }

  private void OnHandleState(
    Entity<ChameleonDisguiseComponent> ent,
    ref AfterAutoHandleStateEvent args)
  {
    this.CopyComp<SpriteComponent>(ent);
    this.CopyComp<GenericVisualizerComponent>(ent);
    this.CopyComp<SolutionContainerVisualsComponent>(ent);
    this.CopyComp<BurnStateVisualsComponent>(ent);
    AppearanceComponent appearanceComponent;
    if (!this._appearanceQuery.TryComp(Entity<ChameleonDisguiseComponent>.op_Implicit(ent), ref appearanceComponent))
      return;
    this._appearance.QueueUpdate(Entity<ChameleonDisguiseComponent>.op_Implicit(ent), appearanceComponent);
  }

  private void OnStartup(Entity<ChameleonDisguisedComponent> ent, ref ComponentStartup args)
  {
    SpriteComponent spriteComponent;
    if (!this._spriteQuery.TryComp(Entity<ChameleonDisguisedComponent>.op_Implicit(ent), ref spriteComponent))
      return;
    ent.Comp.WasVisible = spriteComponent.Visible;
    this._sprite.SetVisible(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent)), false);
  }

  private void OnShutdown(Entity<ChameleonDisguisedComponent> ent, ref ComponentShutdown args)
  {
    SpriteComponent spriteComponent;
    if (!this._spriteQuery.TryComp(Entity<ChameleonDisguisedComponent>.op_Implicit(ent), ref spriteComponent))
      return;
    this._sprite.SetVisible(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent)), ent.Comp.WasVisible);
  }

  private void OnGetFlashEffectTargetEvent(
    Entity<ChameleonDisguisedComponent> ent,
    ref GetFlashEffectTargetEvent args)
  {
    args.Target = ent.Comp.Disguise;
  }
}
