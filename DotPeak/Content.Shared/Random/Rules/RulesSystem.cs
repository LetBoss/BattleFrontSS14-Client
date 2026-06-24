// Decompiled with JetBrains decompiler
// Type: Content.Shared.Random.Rules.RulesSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared.Random.Rules;

public sealed class RulesSystem : EntitySystem
{
  public bool IsTrue(EntityUid uid, RulesPrototype rules)
  {
    foreach (RulesRule rule in rules.Rules)
    {
      if (!rule.Check(this.EntityManager, uid))
        return false;
    }
    return true;
  }
}
