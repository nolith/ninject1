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
using System.Reflection;
using Ninject.Core.Binding;
using Ninject.Core.Infrastructure;
using Ninject.Core.Injection;
using Ninject.Core.Planning;
using Ninject.Core.Planning.Strategies;
#endregion

namespace Ninject.Extensions.MessageBroker.Infrastructure
{
	/// <summary>
	/// A planning strategy that examines types via reflection to determine if there are any
	/// message publications or subscriptions defined.
	/// </summary>
	public class EventReflectionStrategy : PlanningStrategyBase
	{
		/*----------------------------------------------------------------------------------------*/
		/// <summary>
		/// Executed to build the activation plan.
		/// </summary>
		/// <param name="binding">The binding that points at the type whose activation plan is being released.</param>
		/// <param name="type">The type whose activation plan is being manipulated.</param>
		/// <param name="plan">The activation plan that is being manipulated.</param>
		/// <returns>
		/// A value indicating whether to proceed or interrupt the strategy chain.
		/// </returns>
		public override StrategyResult Build(IBinding binding, Type type, IActivationPlan plan)
		{
			EventInfo[] events = type.GetEvents(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			foreach (EventInfo evt in events)
			{
#if !MONO
				PublishAttribute[] attributes = evt.GetAllAttributes<PublishAttribute>();
#else
				PublishAttribute[] attributes = ExtensionsForICustomAttributeProvider.GetAllAttributes<PublishAttribute>(evt);
#endif

				foreach (PublishAttribute attribute in attributes)
					plan.Directives.Add(new PublicationDirective(attribute.Channel, evt));
			}

			MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			var injectorFactory = binding.Components.Get<IInjectorFactory>();

			foreach (MethodInfo method in methods)
			{
#if !MONO
				SubscribeAttribute[] attributes = method.GetAllAttributes<SubscribeAttribute>();
#else
				SubscribeAttribute[] attributes = ExtensionsForICustomAttributeProvider.GetAllAttributes<SubscribeAttribute>(method);
#endif
                foreach (SubscribeAttribute attribute in attributes)
				{
					IMethodInjector injector = injectorFactory.GetInjector(method);
					plan.Directives.Add(new SubscriptionDirective(attribute.Channel, injector, attribute.Thread));
				}
			}

			return StrategyResult.Proceed;
		}
		/*----------------------------------------------------------------------------------------*/
	}
}