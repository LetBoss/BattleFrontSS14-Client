// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.IconLabel.SharedRMCIconLabelSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared._RMC14.IconLabel;

public abstract class SharedRMCIconLabelSystem : EntitySystem
{
  public void Label(
    Entity<IconLabelComponent?> ent,
    LocId newLocId,
    List<(string, object)> newParams)
  {
    ent.Comp = this.EnsureComp<IconLabelComponent>((EntityUid) ent);
    ent.Comp.LabelTextLocId = new LocId?(newLocId);
    ent.Comp.LabelTextParams = new List<(string, object)>((IEnumerable<(string, object)>) newParams);
    this.Dirty<IconLabelComponent>(ent);
  }

  public void Label(
    Entity<IconLabelComponent?> ent,
    LocId newLocId,
    params (string, object)[] newParams)
  {
    this.Label(ent, newLocId, ((IEnumerable<(string, object)>) newParams).ToList<(string, object)>());
  }
}
