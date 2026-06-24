// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Strain.XenoStrainSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Xenonids.Evolution;
using Content.Shared.Examine;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Network;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Strain;

public sealed class XenoStrainSystem : EntitySystem
{
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPopupSystem _popup;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoStrainComponent, ExaminedEvent>(new EntityEventRefHandler<XenoStrainComponent, ExaminedEvent>(this.OnExamined));
    this.SubscribeLocalEvent<XenoStrainComponent, NewXenoEvolvedEvent>(new EntityEventRefHandler<XenoStrainComponent, NewXenoEvolvedEvent>(this.OnNewXenoEvolved));
  }

  private void OnExamined(Entity<XenoStrainComponent> ent, ref ExaminedEvent args)
  {
    if (string.IsNullOrWhiteSpace((string) ent.Comp.Name))
      return;
    using (args.PushGroup("XenoStrainComponent"))
      args.PushText(this.Loc.GetString("rmc-xeno-strain-specialized-into", ("strain", (object) this.Loc.GetString((string) ent.Comp.Name))));
  }

  private void OnNewXenoEvolved(Entity<XenoStrainComponent> ent, ref NewXenoEvolvedEvent args)
  {
    LocId? popup = ent.Comp.Popup;
    if (!popup.HasValue)
      return;
    LocId valueOrDefault = popup.GetValueOrDefault();
    if (this._net.IsClient)
      return;
    this._popup.PopupEntity(this.Loc.GetString((string) valueOrDefault), (EntityUid) ent, (EntityUid) ent, PopupType.MediumXeno);
  }

  public bool AreSameStrain(Entity<XenoStrainComponent?> one, Entity<XenoStrainComponent?> two)
  {
    return this.Resolve<XenoStrainComponent>((EntityUid) one, ref one.Comp, false) && this.Resolve<XenoStrainComponent>((EntityUid) two, ref two.Comp, false) && one.Comp.Name == two.Comp.Name;
  }
}
