// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Xenonids.Hide.XenoHideVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._RMC14.Sprite;
using Content.Shared._RMC14.Sprite;
using Content.Shared._RMC14.Xenonids;
using Content.Shared._RMC14.Xenonids.Hide;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client._RMC14.Xenonids.Hide;

public sealed class XenoHideVisualizerSystem : VisualizerSystem<XenoHideComponent>
{
  [Dependency]
  private RMCSpriteSystem _rmcSprite;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<XenoHideComponent, GetDrawDepthEvent>(new EntityEventRefHandler<XenoHideComponent, GetDrawDepthEvent>((object) this, __methodptr(OnXenoHideGetDrawDepth)), new Type[1]
    {
      typeof (XenoVisualizerSystem)
    }, (Type[]) null);
  }

  private void OnXenoHideGetDrawDepth(Entity<XenoHideComponent> ent, ref GetDrawDepthEvent args)
  {
    bool flag;
    if (!(((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(Entity<XenoHideComponent>.op_Implicit(ent), (Enum) XenoVisualLayers.Hide, ref flag, (AppearanceComponent) null) & flag))
      return;
    args.DrawDepth = Content.Shared.DrawDepth.DrawDepth.Walls;
  }

  protected virtual void OnAppearanceChange(
    EntityUid uid,
    XenoHideComponent component,
    ref AppearanceChangeEvent args)
  {
    int num = (int) this._rmcSprite.UpdateDrawDepth(uid);
  }
}
