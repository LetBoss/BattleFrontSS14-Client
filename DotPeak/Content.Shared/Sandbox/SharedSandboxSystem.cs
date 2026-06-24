// Decompiled with JetBrains decompiler
// Type: Content.Shared.Sandbox.SharedSandboxSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Sandbox;

public abstract class SharedSandboxSystem : EntitySystem
{
  [Dependency]
  protected IPrototypeManager PrototypeManager;

  [NetSerializable]
  [Serializable]
  protected sealed class MsgSandboxStatus : EntityEventArgs
  {
    public bool SandboxAllowed { get; set; }
  }

  [NetSerializable]
  [Serializable]
  protected sealed class MsgSandboxRespawn : EntityEventArgs
  {
  }

  [NetSerializable]
  [Serializable]
  protected sealed class MsgSandboxGiveAccess : EntityEventArgs
  {
  }

  [NetSerializable]
  [Serializable]
  protected sealed class MsgSandboxGiveAghost : EntityEventArgs
  {
  }

  [NetSerializable]
  [Serializable]
  protected sealed class MsgSandboxSuicide : EntityEventArgs
  {
  }
}
