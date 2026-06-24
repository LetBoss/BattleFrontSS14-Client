// Decompiled with JetBrains decompiler
// Type: Content.Client.SSDIndicator.SSDIndicatorSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.CCVar;
using Content.Shared.Mind.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.NPC;
using Content.Shared.SSDIndicator;
using Content.Shared.StatusIcon;
using Content.Shared.StatusIcon.Components;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Client.SSDIndicator;

public sealed class SSDIndicatorSystem : EntitySystem
{
  [Dependency]
  private IPrototypeManager _prototype;
  [Dependency]
  private IConfigurationManager _cfg;
  [Dependency]
  private MobStateSystem _mobState;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SSDIndicatorComponent, GetStatusIconsEvent>(new ComponentEventRefHandler<SSDIndicatorComponent, GetStatusIconsEvent>((object) this, __methodptr(OnGetStatusIcon)), (Type[]) null, (Type[]) null);
  }

  private void OnGetStatusIcon(
    EntityUid uid,
    SSDIndicatorComponent component,
    ref GetStatusIconsEvent args)
  {
    MindContainerComponent containerComponent;
    if (!component.IsSSD || !this._cfg.GetCVar<bool>(CCVars.ICShowSSDIndicator) || this._mobState.IsDead(uid) || this.HasComp<ActiveNPCComponent>(uid) || !this.TryComp<MindContainerComponent>(uid, ref containerComponent) || !containerComponent.ShowExamineInfo)
      return;
    args.StatusIcons.Add((StatusIconData) this._prototype.Index<SsdIconPrototype>(component.Icon));
  }
}
