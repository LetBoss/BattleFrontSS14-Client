// Decompiled with JetBrains decompiler
// Type: Content.Client.Materials.UI.MaterialStorageUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Materials;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Client.Materials.UI;

public sealed class MaterialStorageUIController : UIController
{
  public void SendLatheEjectMessage(EntityUid uid, string material, int sheetsToEject)
  {
    this.EntityManager.RaisePredictiveEvent<EjectMaterialMessage>(new EjectMaterialMessage(this.EntityManager.GetNetEntity(uid, (MetaDataComponent) null), material, sheetsToEject));
  }
}
