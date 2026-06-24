// Decompiled with JetBrains decompiler
// Type: Robust.Shared.IoC.DynamicTypeFactory
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.ContentPack;
using System;

#nullable enable
namespace Robust.Shared.IoC;

internal sealed class DynamicTypeFactory : IDynamicTypeFactoryInternal, IDynamicTypeFactory
{
  [Dependency]
  private readonly IDependencyCollection _dependencies;
  [Dependency]
  private readonly IModLoader _modLoader;

  public object CreateInstance(Type type, bool oneOff = false, bool inject = true)
  {
    if (!this._modLoader.IsContentTypeAccessAllowed(type))
      throw new SandboxArgumentException("Creating non-content types is not allowed.");
    return this.CreateInstanceUnchecked(type, oneOff, inject);
  }

  public object CreateInstance(Type type, object[] args, bool oneOff = false, bool inject = true)
  {
    if (!this._modLoader.IsContentTypeAccessAllowed(type))
      throw new SandboxArgumentException("Creating non-content types is not allowed.");
    return this.CreateInstanceUnchecked(type, args, oneOff, inject);
  }

  public T CreateInstance<T>(bool oneOff = false, bool inject = true) where T : new()
  {
    if (!this._modLoader.IsContentTypeAccessAllowed(typeof (T)))
      throw new SandboxArgumentException("Creating non-content types is not allowed.");
    return this.CreateInstanceUnchecked<T>(oneOff, inject);
  }

  public object CreateInstanceUnchecked(Type type, bool oneOff = false, bool inject = true)
  {
    object instanceUnchecked = !(type == (Type) null) ? Activator.CreateInstance(type) : throw new ArgumentNullException(nameof (type));
    if (inject)
      this._dependencies.InjectDependencies(instanceUnchecked, oneOff);
    return instanceUnchecked;
  }

  public object CreateInstanceUnchecked(Type type, object[] args, bool oneOff = false, bool inject = true)
  {
    object instanceUnchecked = !(type == (Type) null) ? Activator.CreateInstance(type, args) : throw new ArgumentNullException(nameof (type));
    if (inject)
      this._dependencies.InjectDependencies(instanceUnchecked, oneOff);
    return instanceUnchecked;
  }

  public T CreateInstanceUnchecked<T>(bool oneOff = false, bool inject = true) where T : new()
  {
    T instanceUnchecked = new T();
    if (inject)
      this._dependencies.InjectDependencies((object) instanceUnchecked, oneOff);
    return instanceUnchecked;
  }
}
