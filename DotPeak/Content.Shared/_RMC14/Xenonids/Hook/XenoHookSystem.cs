// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Hook.XenoHookSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Line;
using Content.Shared.Throwing;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Hook;

public sealed class XenoHookSystem : EntitySystem
{
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private LineSystem _line;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<XenoHookComponent, MoveEvent>(new EntityEventRefHandler<XenoHookComponent, MoveEvent>(this.OnHookSourceMove));
    this.SubscribeLocalEvent<XenoHookComponent, EntityTerminatingEvent>(new EntityEventRefHandler<XenoHookComponent, EntityTerminatingEvent>(this.OnHookDelete));
    this.SubscribeLocalEvent<XenoHookedComponent, MoveEvent>(new EntityEventRefHandler<XenoHookedComponent, MoveEvent>(this.OnHookedMove));
    this.SubscribeLocalEvent<XenoHookedComponent, StopThrowEvent>(new EntityEventRefHandler<XenoHookedComponent, StopThrowEvent>(this.OnHookedStop));
    this.SubscribeLocalEvent<XenoHookedComponent, ComponentShutdown>(new EntityEventRefHandler<XenoHookedComponent, ComponentShutdown>(this.OnHookedRemoved));
  }

  private void OnHookSourceMove(Entity<XenoHookComponent> xeno, ref MoveEvent args)
  {
    if (xeno.Comp.Hooked.Count == 0)
      return;
    List<EntityUid> entityUidList = new List<EntityUid>();
    foreach (EntityUid uid in xeno.Comp.Hooked)
    {
      XenoHookedComponent comp;
      if (!this.TryComp<XenoHookedComponent>(uid, out comp))
        entityUidList.Add(uid);
      else
        this.UpdateTail((Entity<XenoHookedComponent>) (uid, comp));
    }
    foreach (EntityUid entityUid in entityUidList)
      xeno.Comp.Hooked.Remove(entityUid);
  }

  private void OnHookDelete(Entity<XenoHookComponent> xeno, ref EntityTerminatingEvent args)
  {
    if (xeno.Comp.Hooked.Count == 0)
      return;
    foreach (EntityUid uid in xeno.Comp.Hooked)
      this.RemCompDeferred<XenoHookedComponent>(uid);
    xeno.Comp.Hooked.Clear();
  }

  private void OnHookedMove(Entity<XenoHookedComponent> ent, ref MoveEvent args)
  {
    this.UpdateTail(ent);
  }

  private void OnHookedStop(Entity<XenoHookedComponent> ent, ref StopThrowEvent args)
  {
    this.RemCompDeferred<XenoHookedComponent>((EntityUid) ent);
  }

  private void OnHookedRemoved(Entity<XenoHookedComponent> ent, ref ComponentShutdown args)
  {
    XenoHookComponent comp;
    if (this.TryComp<XenoHookComponent>(ent.Comp.Source, out comp))
      comp.Hooked.Remove((EntityUid) ent);
    ent.Comp.StopUpdating = true;
    this.Dirty<XenoHookedComponent>(ent);
    this._line.DeleteBeam(ent.Comp.Tail);
    this._appearance.SetData((EntityUid) ent, (Enum) HookedVisuals.Hooked, (object) false);
  }

  public bool TryHookTarget(Entity<XenoHookComponent> xeno, EntityUid target)
  {
    if (this.HasComp<XenoHookedComponent>(target))
      return false;
    XenoHookedComponent xenoHookedComponent = this.EnsureComp<XenoHookedComponent>(target);
    xenoHookedComponent.Source = (EntityUid) xeno;
    xenoHookedComponent.TailProto = xeno.Comp.TailProto;
    xeno.Comp.Hooked.Add(target);
    this.Dirty<XenoHookComponent>(xeno);
    this._appearance.SetData(target, (Enum) HookedVisuals.Hooked, (object) true);
    this.UpdateTail((Entity<XenoHookedComponent>) (target, xenoHookedComponent));
    return true;
  }

  public void UpdateTail(Entity<XenoHookedComponent> ent)
  {
    if (this._net.IsClient)
      return;
    XenoHookedComponent comp = ent.Comp;
    if (comp.StopUpdating)
      return;
    if (comp.Tail.Count != 0)
      this._line.DeleteBeam(comp.Tail);
    List<EntityUid> lines;
    if (!this._line.TryCreateLine(comp.Source, (EntityUid) ent, (string) comp.TailProto, out lines))
      return;
    comp.Tail = lines;
  }
}
