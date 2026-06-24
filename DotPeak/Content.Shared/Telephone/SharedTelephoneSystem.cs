// Decompiled with JetBrains decompiler
// Type: Content.Shared.Telephone.SharedTelephoneSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using System.Linq;

#nullable enable
namespace Content.Shared.Telephone;

public abstract class SharedTelephoneSystem : EntitySystem
{
  public bool IsTelephoneEngaged(Entity<TelephoneComponent> entity)
  {
    return entity.Comp.LinkedTelephones.Any<Entity<TelephoneComponent>>();
  }

  public string GetFormattedCallerIdForEntity(
    string? presumedName,
    string? presumedJob,
    Color fontColor,
    string fontType = "Default",
    int fontSize = 12)
  {
    string callerIdForEntity1 = this.Loc.GetString("chat-telephone-unknown-caller", ("color", (object) fontColor), (nameof (fontType), (object) fontType), (nameof (fontSize), (object) fontSize));
    if (presumedName == null)
      return callerIdForEntity1;
    string callerIdForEntity2;
    if (presumedJob != null)
      callerIdForEntity2 = this.Loc.GetString("chat-telephone-caller-id-with-job", ("callerName", (object) presumedName), ("callerJob", (object) presumedJob), ("color", (object) fontColor), (nameof (fontType), (object) fontType), (nameof (fontSize), (object) fontSize));
    else
      callerIdForEntity2 = this.Loc.GetString("chat-telephone-caller-id-without-job", ("callerName", (object) presumedName), ("color", (object) fontColor), (nameof (fontType), (object) fontType), (nameof (fontSize), (object) fontSize));
    return callerIdForEntity2;
  }

  public string GetFormattedDeviceIdForEntity(
    string? deviceName,
    Color fontColor,
    string fontType = "Default",
    int fontSize = 12)
  {
    return deviceName == null ? this.Loc.GetString("chat-telephone-unknown-device", ("color", (object) fontColor), (nameof (fontType), (object) fontType), (nameof (fontSize), (object) fontSize)) : this.Loc.GetString("chat-telephone-device-id", (nameof (deviceName), (object) deviceName), ("color", (object) fontColor), (nameof (fontType), (object) fontType), (nameof (fontSize), (object) fontSize));
  }
}
