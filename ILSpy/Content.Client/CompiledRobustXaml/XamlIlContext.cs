using System;
using System.ComponentModel;
using Robust.Client.UserInterface.XAML;

namespace CompiledRobustXaml;

internal class XamlIlContext
{
	public class Context<TTarget> : ITestRootObjectProvider, ITypeDescriptorContext, ITestProvideValueTarget, ITestUriContext, IServiceProvider
	{
		public TTarget RootObject;

		public object IntermediateRoot;

		private IServiceProvider _sp;

		private object[] _staticProviders;

		public object ProvideTargetObject;

		public object ProvideTargetProperty;

		private Uri _baseUri;

		public NameScope RobustNameScope;

		virtual object ITestRootObjectProvider.RootObject
		{
			get
			{
				//IL_003c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0045: Expected O, but got Unknown
				if (RootObject != null)
				{
					return RootObject;
				}
				if (_sp != null)
				{
					ITestRootObjectProvider val = (ITestRootObjectProvider)_sp.GetService(typeof(ITestRootObjectProvider));
					if (val != null)
					{
						return val.RootObject;
					}
				}
				return null;
			}
		}

		virtual IContainer ITypeDescriptorContext.Container => null;

		virtual object ITypeDescriptorContext.Instance => null;

		virtual PropertyDescriptor ITypeDescriptorContext.PropertyDescriptor => null;

		virtual object ITestProvideValueTarget.TargetObject => ProvideTargetObject;

		virtual object ITestProvideValueTarget.TargetProperty => ProvideTargetProperty;

		public virtual Uri BaseUri
		{
			get
			{
				return _baseUri;
			}
			set
			{
				_baseUri = baseUri;
			}
		}

		virtual bool ITypeDescriptorContext.OnComponentChanging()
		{
			throw new NotSupportedException();
		}

		virtual void ITypeDescriptorContext.OnComponentChanged()
		{
			throw new NotSupportedException();
		}

		public virtual object GetService(Type P_0)
		{
			if (typeof(ITestRootObjectProvider).Equals(P_0))
			{
				return this;
			}
			if (typeof(ITypeDescriptorContext).Equals(P_0))
			{
				return this;
			}
			if (typeof(ITestProvideValueTarget).Equals(P_0))
			{
				return this;
			}
			if (typeof(ITestUriContext).Equals(P_0))
			{
				return this;
			}
			if (_staticProviders != null)
			{
				for (int i = 0; i < (nint)_staticProviders.LongLength; i++)
				{
					object obj = _staticProviders[i];
					if (P_0.IsAssignableFrom(obj.GetType()))
					{
						return obj;
					}
				}
			}
			if (_sp != null)
			{
				return _sp.GetService(P_0);
			}
			return null;
		}

		public Context(IServiceProvider P_0, object[] P_1, string P_2)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Expected O, but got Unknown
			_sp = P_0;
			_staticProviders = P_1;
			if (P_2 != null)
			{
				_baseUri = new Uri(P_2);
			}
			RobustNameScope = new NameScope();
		}
	}
}
