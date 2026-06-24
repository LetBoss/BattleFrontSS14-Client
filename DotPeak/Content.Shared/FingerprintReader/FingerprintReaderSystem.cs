// Decompiled with JetBrains decompiler
// Type: Content.Shared.FingerprintReader.FingerprintReaderSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Forensics.Components;
using Content.Shared.Inventory;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Shared.FingerprintReader;

public sealed class FingerprintReaderSystem : EntitySystem
{
  [Dependency]
  private InventorySystem _inventory;
  [Dependency]
  private SharedPopupSystem _popup;

  public bool IsAllowed(Entity<FingerprintReaderComponent?> target, EntityUid user, bool showPopup = true)
  {
    if (!this.Resolve<FingerprintReaderComponent>((EntityUid) target, ref target.Comp, false) || target.Comp.AllowedFingerprints.Count == 0)
      return true;
    EntityUid? blocker;
    if (!target.Comp.IgnoreGloves && this.TryGetBlockingGloves(user, out blocker))
    {
      if (target.Comp.FailGlovesPopup.HasValue & showPopup)
      {
        SharedPopupSystem popup = this._popup;
        ILocalizationManager loc = this.Loc;
        LocId? failGlovesPopup = target.Comp.FailGlovesPopup;
        string valueOrDefault = failGlovesPopup.HasValue ? (string) failGlovesPopup.GetValueOrDefault() : (string) null;
        (string, object) valueTuple = ("blocker", (object) blocker);
        string message = loc.GetString(valueOrDefault, valueTuple);
        EntityUid uid = (EntityUid) target;
        EntityUid? recipient = new EntityUid?(user);
        popup.PopupClient(message, uid, recipient);
      }
      return false;
    }
    FingerprintComponent comp;
    if (this.TryComp<FingerprintComponent>(user, out comp) && comp.Fingerprint != null && target.Comp.AllowedFingerprints.Contains(comp.Fingerprint))
      return true;
    if (target.Comp.FailPopup.HasValue & showPopup)
    {
      SharedPopupSystem popup = this._popup;
      ILocalizationManager loc = this.Loc;
      LocId? failPopup = target.Comp.FailPopup;
      string valueOrDefault = failPopup.HasValue ? (string) failPopup.GetValueOrDefault() : (string) null;
      string message = loc.GetString(valueOrDefault);
      EntityUid uid = (EntityUid) target;
      EntityUid? recipient = new EntityUid?(user);
      popup.PopupClient(message, uid, recipient);
    }
    return false;
  }

  public bool TryGetBlockingGloves(EntityUid user, [NotNullWhen(true)] out EntityUid? blocker)
  {
    blocker = new EntityUid?();
    EntityUid? entityUid;
    if (!this._inventory.TryGetSlotEntity(user, "gloves", out entityUid) || !this.HasComp<FingerprintMaskComponent>(entityUid))
      return false;
    blocker = entityUid;
    return true;
  }

  public void SetAllowedFingerprints(
    Entity<FingerprintReaderComponent> target,
    HashSet<string> fingerprints)
  {
    target.Comp.AllowedFingerprints = fingerprints;
    this.Dirty<FingerprintReaderComponent>(target);
  }

  public void AddAllowedFingerprint(Entity<FingerprintReaderComponent> target, string fingerprint)
  {
    target.Comp.AllowedFingerprints.Add(fingerprint);
    this.Dirty<FingerprintReaderComponent>(target);
  }

  public void RemoveAllowedFingerprint(
    Entity<FingerprintReaderComponent> target,
    string fingerprint)
  {
    target.Comp.AllowedFingerprints.Remove(fingerprint);
    this.Dirty<FingerprintReaderComponent>(target);
  }
}
