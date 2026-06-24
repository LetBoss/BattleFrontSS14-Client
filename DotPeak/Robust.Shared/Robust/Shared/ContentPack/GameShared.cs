// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ContentPack.GameShared
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.ContentPack;

public abstract class GameShared : IDisposable
{
  protected internal IDependencyCollection Dependencies { get; internal set; }

  protected List<ModuleTestingCallbacks> TestingCallbacks { get; private set; } = new List<ModuleTestingCallbacks>();

  public void SetTestingCallbacks(List<ModuleTestingCallbacks> testingCallbacks)
  {
    this.TestingCallbacks = testingCallbacks;
  }

  public virtual void PreInit()
  {
  }

  public virtual void Init()
  {
  }

  public virtual void PostInit()
  {
  }

  public virtual void Update(ModUpdateLevel level, FrameEventArgs frameEventArgs)
  {
  }

  public virtual void Shutdown()
  {
  }

  public void Dispose()
  {
    this.Dispose(true);
    GC.SuppressFinalize((object) this);
  }

  protected virtual void Dispose(bool disposing)
  {
  }
}
