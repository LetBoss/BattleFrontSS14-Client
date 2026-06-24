// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Console.SharedPubgWeaponVendingConsoleSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._PUBG.Ammo.Components;
using Content.Shared.Tag;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._PUBG.Console;

public abstract class SharedPubgWeaponVendingConsoleSystem : EntitySystem
{
  [Dependency]
  private IPrototypeManager _protoManager;
  [Dependency]
  private IComponentFactory _compFactory;
  private Dictionary<string, string>? _ammoBoxCache;

  public override void Initialize() => base.Initialize();

  public string? GetAmmoBoxForWeapon(EntityPrototype weaponProto)
  {
    PubgAmmoProviderComponent component;
    if (!weaponProto.TryGetComponent<PubgAmmoProviderComponent>(out component, this._compFactory))
      return (string) null;
    string ammoTag = component.AmmoTag;
    if (string.IsNullOrEmpty(ammoTag))
      return (string) null;
    if (this._ammoBoxCache == null)
      this._ammoBoxCache = this.BuildAmmoBoxCache();
    string str;
    return !this._ammoBoxCache.TryGetValue(ammoTag, out str) ? (string) null : str;
  }

  private Dictionary<string, string> BuildAmmoBoxCache()
  {
    Dictionary<string, string> dictionary = new Dictionary<string, string>();
    foreach (EntityPrototype enumeratePrototype in this._protoManager.EnumeratePrototypes<EntityPrototype>())
    {
      TagComponent component;
      if (enumeratePrototype.TryGetComponent<TagComponent>(out component, this._compFactory))
      {
        foreach (ProtoId<TagPrototype> tag in component.Tags)
        {
          string key = tag.ToString();
          if (key.StartsWith("Ammo") && key.EndsWith("Pubg"))
            dictionary.TryAdd(key, enumeratePrototype.ID);
        }
      }
    }
    return dictionary;
  }
}
