// Decompiled with JetBrains decompiler
// Type: Content.Shared.Preferences.ICharacterProfile
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Humanoid;
using Robust.Shared.IoC;
using Robust.Shared.Player;

#nullable enable
namespace Content.Shared.Preferences;

public interface ICharacterProfile
{
  string Name { get; }

  ICharacterAppearance CharacterAppearance { get; }

  bool MemberwiseEquals(ICharacterProfile other);

  void EnsureValid(ICommonSession session, IDependencyCollection collection);

  ICharacterProfile Validated(ICommonSession session, IDependencyCollection collection);
}
