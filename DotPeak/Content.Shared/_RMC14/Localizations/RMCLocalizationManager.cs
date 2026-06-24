// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Localizations.RMCLocalizationManager
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.IdentityManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.GameObjects.Components.Localization;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

#nullable enable
namespace Content.Shared._RMC14.Localizations;

public sealed class RMCLocalizationManager
{
  [Dependency]
  private IEntityManager _entity;
  [Dependency]
  private ILocalizationManager _loc;

  public void Initialize(CultureInfo culture)
  {
    this._loc.AddFunction(culture, "GENDER", new LocFunction(this.FuncGender));
    this._loc.AddFunction(culture, "REFLEXIVE", new LocFunction(this.FuncReflexive));
    this._loc.AddFunction(culture, "PROPER", new LocFunction(this.FuncProper));
  }

  private ILocValue FuncGender(LocArgs args)
  {
    if (args.Args.Count < 1)
      return (ILocValue) new LocValueString("Neuter");
    object obj = args.Args[0].Value;
    if (obj is IdentityEntity)
      obj = (object) (IdentityEntity) obj;
    if (obj is EntityUid entityUid)
    {
      GrammarComponent component;
      if (this._entity.TryGetComponent<GrammarComponent>(entityUid, out component) && component.Gender.HasValue)
        return (ILocValue) new LocValueString(component.Gender.Value.ToString().ToLowerInvariant());
      string str;
      if (this.TryGetEntityLocAttrib(entityUid, "gender", out str))
        return (ILocValue) new LocValueString(str);
    }
    return (ILocValue) new LocValueString("Neuter");
  }

  private ILocValue FuncReflexive(LocArgs args)
  {
    ILocValue locValue = args.Args[0];
    if (locValue.Value is IdentityEntity identityEntity)
      locValue = (ILocValue) new LocValueEntity(identityEntity.Entity);
    return (ILocValue) new LocValueString(this._loc.GetString("zzzz-reflexive-pronoun", ("ent", (object) locValue)));
  }

  private ILocValue FuncProper(LocArgs args)
  {
    if (args.Args.Count < 1)
      return (ILocValue) new LocValueString("false");
    object obj = args.Args[0].Value;
    if (obj is IdentityEntity)
      obj = (object) (IdentityEntity) obj;
    if (obj is EntityUid entityUid)
    {
      GrammarComponent component;
      if (this._entity.TryGetComponent<GrammarComponent>(entityUid, out component) && component.ProperNoun.HasValue)
        return (ILocValue) new LocValueString(component.ProperNoun.Value.ToString().ToLowerInvariant());
      string str;
      if (this.TryGetEntityLocAttrib(entityUid, "proper", out str))
        return (ILocValue) new LocValueString(str);
    }
    return (ILocValue) new LocValueString("false");
  }

  private bool TryGetEntityLocAttrib(EntityUid entity, string attribute, [NotNullWhen(true)] out string? value)
  {
    GrammarComponent component;
    if (this._entity.TryGetComponent<GrammarComponent>(entity, out component) && component.Attributes.TryGetValue(attribute, out value))
      return true;
    EntityPrototype entityPrototype = this._entity.GetComponent<MetaDataComponent>(entity).EntityPrototype;
    if (entityPrototype != null)
      return this._loc.GetEntityData(entityPrototype.ID).Attributes.TryGetValue(attribute, out value);
    value = (string) null;
    return false;
  }
}
