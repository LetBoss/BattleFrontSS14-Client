// Decompiled with JetBrains decompiler
// Type: Content.Client.Medical.Cryogenics.CryoPodSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Emag.Systems;
using Content.Shared.Medical.Cryogenics;
using Content.Shared.Verbs;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.Medical.Cryogenics;

public sealed class CryoPodSystem : SharedCryoPodSystem
{
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SpriteSystem _sprite;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CryoPodComponent, ComponentInit>(new ComponentEventHandler<CryoPodComponent, ComponentInit>((object) this, __methodptr(OnComponentInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CryoPodComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<CryoPodComponent, GetVerbsEvent<AlternativeVerb>>((object) this, __methodptr(AddAlternativeVerbs)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CryoPodComponent, GotEmaggedEvent>(new ComponentEventRefHandler<CryoPodComponent, GotEmaggedEvent>((object) this, __methodptr(OnEmagged)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CryoPodComponent, SharedCryoPodSystem.CryoPodPryFinished>(new ComponentEventHandler<CryoPodComponent, SharedCryoPodSystem.CryoPodPryFinished>((object) this, __methodptr(OnCryoPodPryFinished)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CryoPodComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<CryoPodComponent, AppearanceChangeEvent>((object) this, __methodptr(OnAppearanceChange)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<InsideCryoPodComponent, ComponentStartup>(new ComponentEventHandler<InsideCryoPodComponent, ComponentStartup>((object) this, __methodptr(OnCryoPodInsertion)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<InsideCryoPodComponent, ComponentRemove>(new ComponentEventHandler<InsideCryoPodComponent, ComponentRemove>((object) this, __methodptr(OnCryoPodRemoval)), (Type[]) null, (Type[]) null);
  }

  private void OnCryoPodInsertion(
    EntityUid uid,
    InsideCryoPodComponent component,
    ComponentStartup args)
  {
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(uid, ref spriteComponent))
      return;
    component.PreviousOffset = spriteComponent.Offset;
    this._sprite.SetOffset(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), new Vector2(0.0f, 1f));
  }

  private void OnCryoPodRemoval(
    EntityUid uid,
    InsideCryoPodComponent component,
    ComponentRemove args)
  {
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(uid, ref spriteComponent))
      return;
    this._sprite.SetOffset(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), component.PreviousOffset);
  }

  private void OnAppearanceChange(
    EntityUid uid,
    CryoPodComponent component,
    ref AppearanceChangeEvent args)
  {
    bool flag1;
    bool flag2;
    if (args.Sprite == null || !this._appearance.TryGetData<bool>(uid, (Enum) CryoPodComponent.CryoPodVisuals.ContainsEntity, ref flag1, args.Component) || !this._appearance.TryGetData<bool>(uid, (Enum) CryoPodComponent.CryoPodVisuals.IsOn, ref flag2, args.Component))
      return;
    if (flag1)
    {
      this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) CryoPodVisualLayers.Base, RSI.StateId.op_Implicit("pod-open"));
      this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) CryoPodVisualLayers.Cover, false);
    }
    else
    {
      this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) CryoPodVisualLayers.Base, RSI.StateId.op_Implicit(flag2 ? "pod-on" : "pod-off"));
      this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) CryoPodVisualLayers.Cover, RSI.StateId.op_Implicit(flag2 ? "cover-on" : "cover-off"));
      this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) CryoPodVisualLayers.Cover, true);
    }
  }
}
