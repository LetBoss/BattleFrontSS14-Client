// Decompiled with JetBrains decompiler
// Type: Content.Shared.Construction.ResponseConstructionGuide
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Construction;

[NetSerializable]
[Serializable]
public sealed class ResponseConstructionGuide : EntityEventArgs
{
  public readonly string ConstructionId;
  public readonly ConstructionGuide Guide;

  public ResponseConstructionGuide(string constructionId, ConstructionGuide guide)
  {
    this.ConstructionId = constructionId;
    this.Guide = guide;
  }
}
