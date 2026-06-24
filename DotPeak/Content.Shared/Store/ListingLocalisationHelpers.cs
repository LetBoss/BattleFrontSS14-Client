// Decompiled with JetBrains decompiler
// Type: Content.Shared.Store.ListingLocalisationHelpers
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Localization;
using Robust.Shared.Prototypes;

#nullable enable
namespace Content.Shared.Store;

public static class ListingLocalisationHelpers
{
  public static string GetLocalisedNameOrEntityName(
    ListingData listingData,
    IPrototypeManager prototypeManager)
  {
    string nameOrEntityName = string.Empty;
    if (listingData.Name != null)
      nameOrEntityName = Loc.GetString(listingData.Name);
    else if (listingData.ProductEntity.HasValue)
      nameOrEntityName = prototypeManager.Index(listingData.ProductEntity.Value).Name;
    return nameOrEntityName;
  }

  public static string GetLocalisedDescriptionOrEntityDescription(
    ListingData listingData,
    IPrototypeManager prototypeManager)
  {
    string entityDescription = string.Empty;
    if (listingData.Description != null)
      entityDescription = Loc.GetString(listingData.Description);
    else if (listingData.ProductEntity.HasValue)
      entityDescription = prototypeManager.Index(listingData.ProductEntity.Value).Description;
    return entityDescription;
  }
}
