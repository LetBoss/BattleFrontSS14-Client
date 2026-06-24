// Decompiled with JetBrains decompiler
// Type: Content.Shared.Spider.SharedSpiderSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared.Spider;

public abstract class SharedSpiderSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _action;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<SpiderComponent, MapInitEvent>(new ComponentEventHandler<SpiderComponent, MapInitEvent>(this.OnInit));
  }

  private void OnInit(EntityUid uid, SpiderComponent component, MapInitEvent args)
  {
    this._action.AddAction(uid, ref component.Action, component.WebAction, uid);
  }
}
