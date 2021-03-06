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
using System.Collections.Generic;
using Ninject.Core;
using Ninject.Core.Activation;
using Ninject.Core.Infrastructure;
using Ninject.Core.Interception;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
#endregion

namespace Ninject.Tests.Interception
{
	[TestFixture]
	public class InterceptionFixture
	{
		/*----------------------------------------------------------------------------------------*/
		[Test]
		public void StaticInterceptorsAreRegisteredFromAttributesDefinedOnMethods()
		{
			var module = new InlineModule(m => m.Bind<ObjectWithMethodInterceptor>().ToSelf());

			using (var kernel = new StandardKernel(module))
			{
				kernel.Components.Connect<IProxyFactory>(new DummyProxyFactory());

				var obj = kernel.Get<ObjectWithMethodInterceptor>();

				IContext context = kernel.Components.ContextFactory.Create(typeof(ObjectWithMethodInterceptor));

				IRequest request = new StandardRequest(
					context,
					obj,
					typeof(ObjectWithMethodInterceptor).GetMethod("Foo"),
					new object[0],
					Type.EmptyTypes
				);

				ICollection<IInterceptor> interceptors = kernel.Components.AdviceRegistry.GetInterceptors(request);
				IEnumerator<IInterceptor> enumerator = interceptors.GetEnumerator();
				enumerator.MoveNext();

				Assert.That(interceptors.Count, Is.EqualTo(1));
				Assert.That(enumerator.Current, Is.InstanceOfType(typeof(CountInterceptor)));
			}
		}
		/*----------------------------------------------------------------------------------------*/
		[Test]
		public void StaticInterceptorsAreRegisteredFromAttributesDefinedOnClasses()
		{
			var module = new InlineModule(m => m.Bind<ObjectWithClassInterceptor>().ToSelf());

			using (var kernel = new StandardKernel(module))
			{
				kernel.Components.Connect<IProxyFactory>(new DummyProxyFactory());

				var obj = kernel.Get<ObjectWithClassInterceptor>();

				IContext context1 = kernel.Components.ContextFactory.Create(typeof(ObjectWithClassInterceptor));

				IRequest request1 = new StandardRequest(
					context1,
					obj,
					typeof(ObjectWithClassInterceptor).GetMethod("Foo"),
					new object[0],
					Type.EmptyTypes
				);

				var registry = kernel.Components.AdviceRegistry;

				ICollection<IInterceptor> interceptors1 = registry.GetInterceptors(request1);
				Assert.That(interceptors1.Count, Is.EqualTo(1));

				IContext context2 = kernel.Components.ContextFactory.Create(typeof(ObjectWithClassInterceptor));

				IRequest request2 = new StandardRequest(
					context2,
					obj,
					typeof(ObjectWithClassInterceptor).GetMethod("Bar"),
					new object[0],
					Type.EmptyTypes
				);

				ICollection<IInterceptor> interceptors2 = registry.GetInterceptors(request2);
				Assert.That(interceptors2.Count, Is.EqualTo(1));
			}
		}
		/*----------------------------------------------------------------------------------------*/
		[Test]
		public void StaticInterceptorsAreRegisteredFromInterceptWithAttributes()
		{
			var module = new InlineModule(m => m.Bind<ObjectWithInterceptWithAttribute>().ToSelf());

			using (var kernel = new StandardKernel(module))
			{
				kernel.Components.Connect<IProxyFactory>(new DummyProxyFactory());

				var obj = kernel.Get<ObjectWithInterceptWithAttribute>();

				IContext context = kernel.Components.ContextFactory.Create(typeof(ObjectWithInterceptWithAttribute));

				IRequest request = new StandardRequest(
					context,
					obj,
					typeof(ObjectWithInterceptWithAttribute).GetMethod("Foo"),
					new object[0],
					Type.EmptyTypes
				);

				ICollection<IInterceptor> interceptors = kernel.Components.AdviceRegistry.GetInterceptors(request);
				IEnumerator<IInterceptor> enumerator = interceptors.GetEnumerator();
				enumerator.MoveNext();

				Assert.That(interceptors.Count, Is.EqualTo(1));
				Assert.That(enumerator.Current, Is.InstanceOfType(typeof(CountInterceptor)));
			}
		}
		/*----------------------------------------------------------------------------------------*/
		[Test]
		public void StaticInterceptorsNotRegisteredForMethodsDecoratedWithDoNotInterceptAttribute()
		{
			var module = new InlineModule(m => m.Bind<ObjectWithClassInterceptor>().ToSelf());

			using (var kernel = new StandardKernel(module))
			{
				kernel.Components.Connect<IProxyFactory>(new DummyProxyFactory());

				var obj = kernel.Get<ObjectWithClassInterceptor>();

				IContext context = kernel.Components.ContextFactory.Create(typeof(ObjectWithClassInterceptor));

				IRequest request = new StandardRequest(
					context,
					obj,
					typeof(ObjectWithClassInterceptor).GetMethod("Baz"),
					new object[0],
					Type.EmptyTypes
				);

				ICollection<IInterceptor> interceptors = kernel.Components.AdviceRegistry.GetInterceptors(request);

				Assert.That(interceptors.Count, Is.EqualTo(0));
			}
		}
		/*----------------------------------------------------------------------------------------*/
		[Test]
		public void DynamicInterceptorsCanBeRegistered()
		{
			var module = new InlineModule(m => m.Bind<ObjectWithMethodInterceptor>().ToSelf());

			using (var kernel = new StandardKernel(module))
			{
				kernel.Components.Connect<IProxyFactory>(new DummyProxyFactory());

				var factory = kernel.Components.AdviceFactory;
				var registry = kernel.Components.AdviceRegistry;

				IAdvice advice = factory.Create(new PredicateCondition<IRequest>(r => r.Method.Name.Equals("Bar")));
				advice.Callback = r => r.Kernel.Get<FlagInterceptor>();
				registry.Register(advice);

				var obj = kernel.Get<ObjectWithMethodInterceptor>();

				IContext context = kernel.Components.ContextFactory.Create(typeof(ObjectWithMethodInterceptor));

				IRequest request = new StandardRequest(
					context,
					obj,
					typeof(ObjectWithMethodInterceptor).GetMethod("Bar"),
					new object[0],
					Type.EmptyTypes
				);

				ICollection<IInterceptor> interceptors = registry.GetInterceptors(request);

				IEnumerator<IInterceptor> enumerator = interceptors.GetEnumerator();
				enumerator.MoveNext();

				Assert.That(interceptors.Count, Is.EqualTo(1));
				Assert.That(enumerator.Current, Is.InstanceOfType(typeof(FlagInterceptor)));
			}
		}
		/*----------------------------------------------------------------------------------------*/
		[Test]
		public void InterceptorsAreReturnedInAscendingOrder()
		{
			var module = new InlineModule(m => m.Bind<ObjectWithMethodInterceptor>().ToSelf());

			using (var kernel = new StandardKernel(module))
			{
				kernel.Components.Connect<IProxyFactory>(new DummyProxyFactory());

				var factory = kernel.Components.AdviceFactory;
				var registry = kernel.Components.AdviceRegistry;

				IAdvice advice = factory.Create(new PredicateCondition<IRequest>(r => r.Method.Name.Equals("Foo")));

				advice.Callback = r => r.Kernel.Get<FlagInterceptor>();
				advice.Order = -1;

				registry.Register(advice);

				var obj = kernel.Get<ObjectWithMethodInterceptor>();

				IContext context = kernel.Components.ContextFactory.Create(typeof(ObjectWithMethodInterceptor));

				IRequest request = new StandardRequest(
					context,
					obj,
					typeof(ObjectWithMethodInterceptor).GetMethod("Foo"),
					new object[0],
					Type.EmptyTypes
				);

				ICollection<IInterceptor> interceptors = registry.GetInterceptors(request);
				Assert.That(interceptors.Count, Is.EqualTo(2));

				IEnumerator<IInterceptor> enumerator = interceptors.GetEnumerator();

				enumerator.MoveNext();
				Assert.That(enumerator.Current, Is.InstanceOfType(typeof(FlagInterceptor)));

				enumerator.MoveNext();
				Assert.That(enumerator.Current, Is.InstanceOfType(typeof(CountInterceptor)));
			}
		}
		/*----------------------------------------------------------------------------------------*/
	}
}