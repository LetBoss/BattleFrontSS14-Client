// Decompiled with JetBrains decompiler
// Type: Content.Client.BarSign.BarSignSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.BarSign.Ui;
using Content.Shared.BarSign;
using Content.Shared.Power;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Client.BarSign;

public sealed class BarSignSystem : VisualizerSystem<BarSignComponent>
{
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private UserInterfaceSystem _ui;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<BarSignComponent, AfterAutoHandleStateEvent>(new ComponentEventRefHandler<BarSignComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnAfterAutoHandleState)), (Type[]) null, (Type[]) null);
  }

  private void OnAfterAutoHandleState(
    EntityUid uid,
    BarSignComponent component,
    ref AfterAutoHandleStateEvent args)
  {
    BarSignBoundUserInterface boundUserInterface;
    if (((SharedUserInterfaceSystem) this._ui).TryGetOpenUi<BarSignBoundUserInterface>(Entity<UserInterfaceComponent>.op_Implicit(uid), (Enum) BarSignUiKey.Key, ref boundUserInterface))
      boundUserInterface.Update(component.Current);
    this.UpdateAppearance(uid, component);
  }

  protected virtual void OnAppearanceChange(
    EntityUid uid,
    BarSignComponent component,
    ref AppearanceChangeEvent args)
  {
    this.UpdateAppearance(uid, component, args.Component, args.Sprite);
  }

  private void UpdateAppearance(
    EntityUid id,
    BarSignComponent sign,
    AppearanceComponent? appearance = null,
    SpriteComponent? sprite = null)
  {
    if (!((EntitySystem) this).Resolve<AppearanceComponent, SpriteComponent>(id, ref appearance, ref sprite, true))
      return;
    bool flag;
    ((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(id, (Enum) PowerDeviceVisuals.Powered, ref flag, appearance);
    BarSignPrototype barSignPrototype;
    if (flag && sign.Current.HasValue && this._prototypeManager.TryIndex<BarSignPrototype>(sign.Current, ref barSignPrototype))
    {
      this.SpriteSystem.LayerSetSprite(Entity<SpriteComponent>.op_Implicit((id, sprite)), 0, barSignPrototype.Icon);
      sprite.LayerSetShader(0, "unshaded");
    }
    else
    {
      this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((id, sprite)), 0, RSI.StateId.op_Implicit("empty"));
      sprite.LayerSetShader(0, (ShaderInstance) null, (string) null);
    }
  }
}
