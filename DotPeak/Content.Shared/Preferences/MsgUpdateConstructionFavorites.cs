// Decompiled with JetBrains decompiler
// Type: Content.Shared.Preferences.MsgUpdateConstructionFavorites
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Construction.Prototypes;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Preferences;

public sealed class MsgUpdateConstructionFavorites : NetMessage
{
  public List<ProtoId<ConstructionPrototype>> Favorites = new List<ProtoId<ConstructionPrototype>>();

  public override MsgGroups MsgGroup => MsgGroups.Command;

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    int num = ((NetBuffer) buffer).ReadVariableInt32();
    this.Favorites.Clear();
    for (int index = 0; index < num; ++index)
      this.Favorites.Add(new ProtoId<ConstructionPrototype>(((NetBuffer) buffer).ReadString()));
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    ((NetBuffer) buffer).WriteVariableInt32(this.Favorites.Count);
    foreach (ProtoId<ConstructionPrototype> favorite in this.Favorites)
      ((NetBuffer) buffer).Write((string) favorite);
  }
}
