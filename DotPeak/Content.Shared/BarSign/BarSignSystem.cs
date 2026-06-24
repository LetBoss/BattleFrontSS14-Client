// Decompiled with JetBrains decompiler
// Type: Content.Shared.BarSign.BarSignSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared.BarSign;

public sealed class BarSignSystem : EntitySystem
{
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private MetaDataSystem _metaData;

  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BarSignComponent, MapInitEvent>(new EntityEventRefHandler<BarSignComponent, MapInitEvent>((object) this, __methodptr(OnMapInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    BoundUserInterfaceRegisterExt.BuiEvents<BarSignComponent>(this.Subs, (object) BarSignUiKey.Key, new BoundUserInterfaceRegisterExt.BuiEventSubscriber<BarSignComponent>((object) this, __methodptr(\u003CInitialize\u003Eb__3_0)));
  }

  private void OnMapInit(Entity<BarSignComponent> ent, ref MapInitEvent args)
  {
    if (ent.Comp.Current.HasValue)
      return;
    BarSignPrototype newPrototype = RandomExtensions.Pick<BarSignPrototype>(this._random, (IReadOnlyList<BarSignPrototype>) BarSignSystem.GetAllBarSigns(this._prototypeManager));
    this.SetBarSign(ent, newPrototype);
  }

  private void OnSetBarSignMessage(Entity<BarSignComponent> ent, ref SetBarSignMessage args)
  {
    BarSignPrototype newPrototype;
    if (!this._prototypeManager.TryIndex<BarSignPrototype>(args.Sign, ref newPrototype))
      return;
    this.SetBarSign(ent, newPrototype);
  }

  public void SetBarSign(Entity<BarSignComponent> ent, BarSignPrototype newPrototype)
  {
    MetaDataComponent metaDataComponent = this.MetaData(Entity<BarSignComponent>.op_Implicit(ent));
    string str = this.Loc.GetString(LocId.op_Implicit(newPrototype.Name));
    this._metaData.SetEntityName(Entity<BarSignComponent>.op_Implicit(ent), str, metaDataComponent, true);
    this._metaData.SetEntityDescription(Entity<BarSignComponent>.op_Implicit(ent), this.Loc.GetString(LocId.op_Implicit(newPrototype.Description)), metaDataComponent);
    ent.Comp.Current = ProtoId<BarSignPrototype>.op_Implicit(newPrototype.ID);
    this.Dirty<BarSignComponent>(ent, (MetaDataComponent) null);
  }

  public static List<BarSignPrototype> GetAllBarSigns(IPrototypeManager prototypeManager)
  {
    return prototypeManager.EnumeratePrototypes<BarSignPrototype>().Where<BarSignPrototype>((Func<BarSignPrototype, bool>) (p => !p.Hidden)).ToList<BarSignPrototype>();
  }
}
