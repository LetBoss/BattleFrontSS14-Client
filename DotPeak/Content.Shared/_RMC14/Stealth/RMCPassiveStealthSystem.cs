// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Stealth.RMCPassiveStealthSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Foldable;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Storage.Components;
using Content.Shared.Storage.EntitySystems;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared._RMC14.Stealth;

public sealed class RMCPassiveStealthSystem : EntitySystem
{
  [Dependency]
  private SharedEntityStorageSystem _entityStorage;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private EntityWhitelistSystem _whitelist;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<RMCPassiveStealthComponent, ComponentInit>(new EntityEventRefHandler<RMCPassiveStealthComponent, ComponentInit>(this.OnInit));
    this.SubscribeLocalEvent<RMCPassiveStealthComponent, StorageAfterOpenEvent>(new EntityEventRefHandler<RMCPassiveStealthComponent, StorageAfterOpenEvent>(this.OnStorageAfterOpen));
    this.SubscribeLocalEvent<RMCPassiveStealthComponent, FoldedEvent>(new EntityEventRefHandler<RMCPassiveStealthComponent, FoldedEvent>(this.OnFolded), after: new Type[1]
    {
      typeof (SharedEntityStorageSystem)
    });
    this.SubscribeLocalEvent<RMCPassiveStealthComponent, ActivateInWorldEvent>(new EntityEventRefHandler<RMCPassiveStealthComponent, ActivateInWorldEvent>(this.OnToggle));
  }

  private void OnInit(Entity<RMCPassiveStealthComponent> ent, ref ComponentInit args)
  {
    if (this._timing.ApplyingState || this.Paused(ent.Owner))
      return;
    ent.Comp.Enabled = new bool?(false);
    this.EnsureComp<EntityTurnInvisibleComponent>(ent.Owner);
  }

  private void OnStorageAfterOpen(
    Entity<RMCPassiveStealthComponent> ent,
    ref StorageAfterOpenEvent args)
  {
    if (this._timing.ApplyingState || !ent.Comp.Enabled.HasValue)
      return;
    ent.Comp.Enabled = new bool?(false);
    ent.Comp.ToggleTime = this._timing.CurTime;
    this.Dirty(ent.Owner, (IComponent) ent.Comp);
  }

  private void OnFolded(Entity<RMCPassiveStealthComponent> ent, ref FoldedEvent args)
  {
    if (this._timing.ApplyingState || !ent.Comp.Enabled.HasValue)
      return;
    if (!args.IsFolded)
    {
      this._entityStorage.OpenStorage(ent.Owner);
      ent.Comp.Enabled = new bool?(false);
    }
    else
    {
      ent.Comp.Enabled = new bool?(false);
      this.RemCompDeferred<EntityActiveInvisibleComponent>(ent.Owner);
    }
  }

  private void OnToggle(Entity<RMCPassiveStealthComponent> ent, ref ActivateInWorldEvent args)
  {
    FoldableComponent comp1;
    if (this._timing.ApplyingState || !ent.Comp.Toggleable || this.TryComp<FoldableComponent>(ent.Owner, out comp1) && comp1.IsFolded)
      return;
    RMCPassiveStealthComponent comp2 = ent.Comp;
    comp2.Enabled.GetValueOrDefault();
    if (!comp2.Enabled.HasValue)
    {
      bool flag = false;
      comp2.Enabled = new bool?(flag);
    }
    if (!ent.Comp.Enabled.Value && !this._whitelist.IsValid(ent.Comp.Whitelist, args.User))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-skills-cant-use", ("item", (object) ent.Owner)), args.User, new EntityUid?(args.User), PopupType.SmallCaution);
      args.Handled = true;
    }
    else if (ent.Comp.Enabled.Value)
    {
      ent.Comp.Enabled = new bool?(false);
      ent.Comp.ToggleTime = this._timing.CurTime;
      this.Dirty(ent.Owner, (IComponent) ent.Comp);
    }
    else
    {
      ent.Comp.Enabled = new bool?(true);
      ent.Comp.ToggleTime = this._timing.CurTime;
      this.Dirty(ent.Owner, (IComponent) ent.Comp);
    }
  }

  public override void Update(float frameTime)
  {
    if (!this._timing.IsFirstTimePredicted)
      return;
    Robust.Shared.GameObjects.EntityQueryEnumerator<RMCPassiveStealthComponent> entityQueryEnumerator = this.EntityQueryEnumerator<RMCPassiveStealthComponent>();
    EntityUid uid;
    RMCPassiveStealthComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (comp1.Enabled.HasValue && !this._net.IsClient)
      {
        TimeSpan timeSpan = this._timing.CurTime - comp1.ToggleTime;
        if (comp1.Enabled.Value)
        {
          EntityActiveInvisibleComponent invisibleComponent = this.EnsureComp<EntityActiveInvisibleComponent>(uid);
          invisibleComponent.Opacity = !(timeSpan < comp1.Delay) ? comp1.MinOpacity : comp1.MaxOpacity - (float) (timeSpan / comp1.Delay * ((double) comp1.MaxOpacity - (double) comp1.MinOpacity));
          this.Dirty(uid, (IComponent) invisibleComponent);
        }
        else
        {
          EntityActiveInvisibleComponent comp;
          if (this.TryComp<EntityActiveInvisibleComponent>(uid, out comp))
          {
            if (timeSpan < comp1.UnCloakDelay)
            {
              comp.Opacity = comp1.MinOpacity + (float) (timeSpan / comp1.UnCloakDelay * ((double) comp1.MaxOpacity - (double) comp1.MinOpacity));
              this.Dirty(uid, (IComponent) comp);
            }
            else
              this.RemCompDeferred<EntityActiveInvisibleComponent>(uid);
          }
        }
      }
    }
  }
}
