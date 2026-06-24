// Decompiled with JetBrains decompiler
// Type: CompiledRobustXaml.XamlIlContext
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface.XAML;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

#nullable disable
namespace CompiledRobustXaml;

internal class XamlIlContext
{
  public class Context<TTarget> : 
    ITestRootObjectProvider,
    ITypeDescriptorContext,
    ITestProvideValueTarget,
    ITestUriContext,
    IServiceProvider
  {
    public TTarget RootObject;
    public object IntermediateRoot;
    IServiceProvider _sp;
    object[] _staticProviders;
    public object ProvideTargetObject;
    public object ProvideTargetProperty;
    Uri _baseUri;
    public NameScope RobustNameScope;

    virtual object ITestRootObjectProvider.RootObject
    {
      get
      {
        if ((object) this.RootObject != null)
          return (object) this.RootObject;
        if (this._sp != null)
        {
          ITestRootObjectProvider service = (ITestRootObjectProvider) this._sp.GetService(typeof (ITestRootObjectProvider));
          if (service != null)
            return service.RootObject;
        }
        return (object) null;
      }
    }

    virtual object ITestRootObjectProvider.get_RootObject()
    {
      if ((object) this.RootObject != null)
        return (object) this.RootObject;
      if (this._sp != null)
      {
        ITestRootObjectProvider service = (ITestRootObjectProvider) this._sp.GetService(typeof (ITestRootObjectProvider));
        if (service != null)
          return service.RootObject;
      }
      return (object) null;
    }

    virtual IContainer ITypeDescriptorContext.Container => (IContainer) null;

    virtual IContainer ITypeDescriptorContext.get_Container() => (IContainer) null;

    virtual object ITypeDescriptorContext.get_Instance() => (object) null;

    virtual object ITypeDescriptorContext.Instance => (object) null;

    virtual PropertyDescriptor ITypeDescriptorContext.PropertyDescriptor
    {
      get => (PropertyDescriptor) null;
    }

    virtual PropertyDescriptor ITypeDescriptorContext.get_PropertyDescriptor()
    {
      return (PropertyDescriptor) null;
    }

    virtual bool ITypeDescriptorContext.OnComponentChanging() => throw new NotSupportedException();

    virtual void ITypeDescriptorContext.OnComponentChanged() => throw new NotSupportedException();

    virtual object ITestProvideValueTarget.get_TargetObject() => this.ProvideTargetObject;

    virtual object ITestProvideValueTarget.TargetObject => this.ProvideTargetObject;

    virtual object ITestProvideValueTarget.get_TargetProperty() => this.ProvideTargetProperty;

    virtual object ITestProvideValueTarget.TargetProperty => this.ProvideTargetProperty;

    public virtual Uri BaseUri
    {
      get => this._baseUri;
      [param: In] set => this._baseUri = value;
    }

    public virtual Uri get_BaseUri() => this._baseUri;

    public virtual void set_BaseUri([In] Uri obj0) => this._baseUri = obj0;

    public virtual object GetService([In] Type obj0)
    {
      if (typeof (ITestRootObjectProvider).Equals(obj0))
        return (object) this;
      if (typeof (ITypeDescriptorContext).Equals(obj0))
        return (object) this;
      if (typeof (ITestProvideValueTarget).Equals(obj0))
        return (object) this;
      if (typeof (ITestUriContext).Equals(obj0))
        return (object) this;
      if (this._staticProviders != null)
      {
        for (int index = 0; index < this._staticProviders.Length; ++index)
        {
          object staticProvider = this._staticProviders[index];
          if (obj0.IsAssignableFrom(staticProvider.GetType()))
            return staticProvider;
        }
      }
      return this._sp != null ? this._sp.GetService(obj0) : (object) null;
    }

    public Context([In] IServiceProvider obj0, [In] object[] obj1, [In] string obj2)
    {
      this._sp = obj0;
      this._staticProviders = obj1;
      if (obj2 != null)
        this._baseUri = new Uri(obj2);
      this.RobustNameScope = new NameScope();
    }
  }
}
