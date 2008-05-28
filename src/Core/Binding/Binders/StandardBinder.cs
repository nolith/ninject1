#region License
//
// Author: Nate Kohari <nkohari@gmail.com>
// Copyright (c) 2007-2008, Enkari, Ltd.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion
#region Using Directives
using System;
using System.Collections;
using Ninject.Core.Activation;
using Ninject.Core.Behavior;
using Ninject.Core.Binding.Syntax;
using Ninject.Core.Creation;
using Ninject.Core.Creation.Providers;
using Ninject.Core.Infrastructure;
#endregion

namespace Ninject.Core.Binding
{
	/// <summary>
	/// The stock definition of a binder.
	/// </summary>
	public class StandardBinder : BinderBase, IBindingTargetSyntax, IBindingConditionBehaviorOrArgumentSyntax, IBindingBehaviorOrArgumentSyntax
	{
		/*----------------------------------------------------------------------------------------*/
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="StandardBinder"/> class.
		/// </summary>
		/// <param name="binding">The binding that the binder should manipulate.</param>
		public StandardBinder(IBinding binding)
			: base(binding)
		{
		}
		#endregion
		/*----------------------------------------------------------------------------------------*/
		#region IBindingTargetSyntax Members
		IBindingConditionBehaviorOrArgumentSyntax IBindingTargetSyntax.ToSelf()
		{
			Binding.Provider = Binding.Kernel.Components.Get<IProviderFactory>().Create(Binding.Service);
			return this;
		}
		/*----------------------------------------------------------------------------------------*/
		IBindingConditionBehaviorOrArgumentSyntax IBindingTargetSyntax.To<T>()
		{
			Binding.Provider = Binding.Kernel.Components.Get<IProviderFactory>().Create(typeof(T));
			return this;
		}
		/*----------------------------------------------------------------------------------------*/
		IBindingConditionBehaviorOrArgumentSyntax IBindingTargetSyntax.To(Type type)
		{
			Binding.Provider = Binding.Kernel.Components.Get<IProviderFactory>().Create(type);
			return this;
		}
		/*----------------------------------------------------------------------------------------*/
		IBindingConditionBehaviorOrArgumentSyntax IBindingTargetSyntax.ToProvider<T>()
		{
			Binding.Provider = Binding.Kernel.Get<T>();
			return this;
		}
		/*----------------------------------------------------------------------------------------*/
		IBindingConditionBehaviorOrArgumentSyntax IBindingTargetSyntax.ToProvider(Type providerType)
		{
			if (!typeof(IProvider).IsAssignableFrom(providerType))
				throw new NotSupportedException(ExceptionFormatter.InvalidProviderType(Binding, providerType));

			Binding.Provider = Binding.Kernel.Get(providerType) as IProvider;
			return this;
		}
		/*----------------------------------------------------------------------------------------*/
		IBindingConditionBehaviorOrArgumentSyntax IBindingTargetSyntax.ToProvider(IProvider provider)
		{
			Binding.Provider = provider;
			return this;
		}
		/*----------------------------------------------------------------------------------------*/
		IBindingConditionBehaviorOrArgumentSyntax IBindingTargetSyntax.ToConstant<T>(T value)
		{
			Binding.Provider = new ConstantProvider(value);
			return this;
		}
		/*----------------------------------------------------------------------------------------*/
#if !NO_REMOTING
		IBindingConditionBehaviorOrArgumentSyntax IBindingTargetSyntax.ToRemotingChannel(string uri)
		{
			Binding.Provider = new RemotingProvider(Binding.Service, uri);
			return this;
		}
#endif
		/*----------------------------------------------------------------------------------------*/
		IBindingConditionBehaviorOrArgumentSyntax IBindingTargetSyntax.ToFactoryMethod<R>(Func<R> method)
		{
			Binding.Provider = new FactoryMethodProvider<R>(method);
			return this;
		}
		/*----------------------------------------------------------------------------------------*/
		IBindingConditionBehaviorOrArgumentSyntax IBindingTargetSyntax.ToFactoryMethod<A1, R>(Func<A1, R> method, A1 arg1)
		{
			Binding.Provider = new FactoryMethodProvider<A1, R>(method, arg1);
			return this;
		}
		/*----------------------------------------------------------------------------------------*/
		IBindingConditionBehaviorOrArgumentSyntax IBindingTargetSyntax.ToFactoryMethod<A1, A2, R>(Func<A1, A2, R> method, A1 arg1,
			A2 arg2)
		{
			Binding.Provider = new FactoryMethodProvider<A1, A2, R>(method, arg1, arg2);
			return this;
		}
		/*----------------------------------------------------------------------------------------*/
		IBindingConditionBehaviorOrArgumentSyntax IBindingTargetSyntax.ToFactoryMethod<A1, A2, A3, R>(Func<A1, A2, A3, R> method,
			A1 arg1, A2 arg2, A3 arg3)
		{
			Binding.Provider = new FactoryMethodProvider<A1, A2, A3, R>(method, arg1, arg2, arg3);
			return this;
		}
		/*----------------------------------------------------------------------------------------*/
		IBindingConditionBehaviorOrArgumentSyntax IBindingTargetSyntax.ToFactoryMethod<A1, A2, A3, A4, R>(
			Func<A1, A2, A3, A4, R> method, A1 arg1, A2 arg2, A3 arg3, A4 arg4)
		{
			Binding.Provider = new FactoryMethodProvider<A1, A2, A3, A4, R>(method, arg1, arg2, arg3, arg4);
			return this;
		}
		#endregion
		/*----------------------------------------------------------------------------------------*/
		#region IBindingConditionSyntax Members
		IBindingBehaviorOrArgumentSyntax IBindingConditionSyntax.Always()
		{
			Binding.Condition = null;
			return this;
		}
		/*----------------------------------------------------------------------------------------*/
		IBindingBehaviorOrArgumentSyntax IBindingConditionSyntax.Only(ICondition<IContext> condition)
		{
			Binding.Condition = condition;
			return this;
		}
		/*----------------------------------------------------------------------------------------*/
		IBindingBehaviorOrArgumentSyntax IBindingConditionSyntax.OnlyIf(Predicate<IContext> predicate)
		{
			Binding.Condition = new PredicateCondition<IContext>(predicate);
			return this;
		}
		/*----------------------------------------------------------------------------------------*/
		IBindingBehaviorOrArgumentSyntax IBindingConditionSyntax.ForMembersOf<T>()
		{
			Binding.Condition = new PredicateCondition<IContext>(ctx => ctx.Member.ReflectedType == typeof(T));
			return this;
		}
		/*----------------------------------------------------------------------------------------*/
		IBindingBehaviorOrArgumentSyntax IBindingConditionSyntax.ForMembersOf(Type type)
		{
			Binding.Condition = new PredicateCondition<IContext>(ctx => ctx.Member.ReflectedType == type);
			return this;
		}
		/*----------------------------------------------------------------------------------------*/
		IBindingBehaviorOrArgumentSyntax IBindingConditionSyntax.WhereMemberHas<T>()
		{
			// Uses non-generic version to dodge problem with generic constraints on the Mono compiler.
			Binding.Condition = new PredicateCondition<IContext>(ctx => ctx.Member.HasAttribute(typeof(T)));
			return this;
		}
		/*----------------------------------------------------------------------------------------*/
		IBindingBehaviorOrArgumentSyntax IBindingConditionSyntax.WhereMemberHas(Type attribute)
		{
			if (!typeof(Attribute).IsAssignableFrom(attribute))
				throw new NotSupportedException(ExceptionFormatter.InvalidAttributeTypeUsedInBindingCondition(Binding, attribute));

			Binding.Condition = new PredicateCondition<IContext>(ctx => ctx.Member.HasAttribute(attribute));
			return this;
		}
		/*----------------------------------------------------------------------------------------*/
		IBindingBehaviorOrArgumentSyntax IBindingConditionSyntax.WhereTargetHas<T>()
		{
			// Uses non-generic version to dodge problem with generic constraints on the Mono compiler.
			Binding.Condition = new PredicateCondition<IContext>(ctx => ctx.Target.HasAttribute(typeof(T)));
			return this;
		}
		/*----------------------------------------------------------------------------------------*/
		IBindingBehaviorOrArgumentSyntax IBindingConditionSyntax.WhereTargetHas(Type attribute)
		{
			if (!typeof(Attribute).IsAssignableFrom(attribute))
				throw new NotSupportedException(ExceptionFormatter.InvalidAttributeTypeUsedInBindingCondition(Binding, attribute));

			Binding.Condition = new PredicateCondition<IContext>(ctx => ctx.Target.HasAttribute(attribute));
			return this;
		}
		#endregion
		/*----------------------------------------------------------------------------------------*/
		#region IBindingBehaviorSyntax Members
		IBindingInlineArgumentSyntax IBindingBehaviorSyntax.Using<T>()
		{
			IBehavior behavior = new T();

			behavior.Kernel = Binding.Kernel;
			Binding.Behavior = behavior;

			return this;
		}
		/*----------------------------------------------------------------------------------------*/
		IBindingInlineArgumentSyntax IBindingBehaviorSyntax.Using(IBehavior behavior)
		{
			behavior.Kernel = Binding.Kernel;
			Binding.Behavior = behavior;

			return this;
		}
		#endregion
		/*----------------------------------------------------------------------------------------*/
		#region IBindingInlineArgumentSyntax Members
		IBindingInlineArgumentSyntax IBindingInlineArgumentSyntax.WithArgument<T>(string name, T value)
		{
			Binding.InlineArguments.Add(name, value);
			return this;
		}
		/*----------------------------------------------------------------------------------------*/
		IBindingInlineArgumentSyntax IBindingInlineArgumentSyntax.WithArguments(IDictionary arguments)
		{
			foreach (DictionaryEntry entry in arguments)
        Binding.InlineArguments.Add(entry.Key.ToString(), entry.Value);

			return this;
		}
		/*----------------------------------------------------------------------------------------*/
		IBindingInlineArgumentSyntax IBindingInlineArgumentSyntax.WithArguments(object values)
		{
			IDictionary dictionary = ReflectionDictionaryBuilder.Create(values);

			foreach (DictionaryEntry entry in dictionary)
				Binding.InlineArguments.Add(entry.Key.ToString(), entry.Value);

			return this;
		}
		#endregion
		/*----------------------------------------------------------------------------------------*/
	}
}