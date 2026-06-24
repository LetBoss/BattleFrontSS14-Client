// Decompiled with JetBrains decompiler
// Type: Content.Shared.Roles.RoleCodeword.SharedRoleCodewordSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Roles.RoleCodeword;

public abstract class SharedRoleCodewordSystem : EntitySystem
{
  public void SetRoleCodewords(
    Entity<RoleCodewordComponent> ent,
    string key,
    List<string> codewords,
    Color color)
  {
    CodewordsData codewordsData = new CodewordsData(color, codewords);
    ent.Comp.RoleCodewords[key] = codewordsData;
    this.Dirty<RoleCodewordComponent>(ent);
  }
}
