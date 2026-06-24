// Decompiled with JetBrains decompiler
// Type: Content.Shared.Silicons.Laws.SharedSiliconLawSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Emag.Systems;
using Content.Shared.Mind;
using Content.Shared.Popups;
using Content.Shared.Silicons.Laws.Components;
using Content.Shared.Stunnable;
using Content.Shared.Wires;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared.Silicons.Laws;

public abstract class SharedSiliconLawSystem : EntitySystem
{
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedStunSystem _stunSystem;
  [Dependency]
  private EmagSystem _emag;
  [Dependency]
  private SharedMindSystem _mind;

  public override void Initialize()
  {
    this.InitializeUpdater();
    this.SubscribeLocalEvent<EmagSiliconLawComponent, GotEmaggedEvent>(new ComponentEventRefHandler<EmagSiliconLawComponent, GotEmaggedEvent>(this.OnGotEmagged));
  }

  private void OnGotEmagged(
    EntityUid uid,
    EmagSiliconLawComponent component,
    ref GotEmaggedEvent args)
  {
    if (!this._emag.CompareFlag(args.Type, EmagType.Interaction) || this._emag.CheckFlag(uid, EmagType.Interaction))
      return;
    if (uid == args.UserUid)
    {
      this._popup.PopupClient(this.Loc.GetString("law-emag-cannot-emag-self"), uid, new EntityUid?(args.UserUid));
    }
    else
    {
      WiresPanelComponent comp;
      if (component.RequireOpenPanel && this.TryComp<WiresPanelComponent>(uid, out comp) && !comp.Open)
      {
        this._popup.PopupClient(this.Loc.GetString("law-emag-require-panel"), uid, new EntityUid?(args.UserUid));
      }
      else
      {
        SiliconEmaggedEvent args1 = new SiliconEmaggedEvent(args.UserUid);
        this.RaiseLocalEvent<SiliconEmaggedEvent>(uid, ref args1);
        component.OwnerName = this.Name(args.UserUid);
        this.NotifyLawsChanged(uid, component.EmaggedSound);
        EntityUid mindId;
        if (this._mind.TryGetMind(uid, out mindId, out MindComponent _))
          this.EnsureSubvertedSiliconRole(mindId);
        this._stunSystem.TryParalyze(uid, component.StunTime, true);
        args.Handled = true;
      }
    }
  }

  public virtual void NotifyLawsChanged(EntityUid uid, SoundSpecifier? cue = null)
  {
  }

  protected virtual void EnsureSubvertedSiliconRole(EntityUid mindId)
  {
  }

  protected virtual void RemoveSubvertedSiliconRole(EntityUid mindId)
  {
  }

  private void InitializeUpdater()
  {
    this.SubscribeLocalEvent<SiliconLawUpdaterComponent, EntInsertedIntoContainerMessage>(new EntityEventRefHandler<SiliconLawUpdaterComponent, EntInsertedIntoContainerMessage>(this.OnUpdaterInsert));
  }

  protected virtual void OnUpdaterInsert(
    Entity<SiliconLawUpdaterComponent> ent,
    ref EntInsertedIntoContainerMessage args)
  {
  }
}
