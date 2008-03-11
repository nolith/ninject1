#region License
//
// Author: Nate Kohari <nkohari@gmail.com>
// Copyright (c) 2007, Enkari, Ltd.
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
using Ninject.Core.Activation;
using Ninject.Core.Tests.Mocks;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
#endregion

namespace Ninject.Core.Tests.Activation
{
	[TestFixture]
	public class FactoryMethodProviderFixture
	{
		/*----------------------------------------------------------------------------------------*/
		[Test]
		public void CanBindToParameterlessStaticFactoryMethod()
		{
			IModule module = new TestableModule(delegate(TestableModule m)
			{
				m.Bind(typeof(IMock)).ToProvider(new FactoryMethodProvider<IMock>(MockFactory.CreateStatic));
			});

			using (IKernel kernel = new StandardKernel(module))
			{
				IMock mock = kernel.Get<IMock>();
				Assert.That(mock, Is.Not.Null);
				Assert.That(mock, Is.InstanceOfType(typeof(ImplA)));
			}
		}
		/*----------------------------------------------------------------------------------------*/
		[Test]
		public void CanBindToStaticFactoryMethodWithParameters()
		{
			IModule module = new TestableModule(delegate(TestableModule m)
			{
				m.Bind(typeof(IMock)).ToProvider(new FactoryMethodProvider<string, int, IMock>(MockFactory.CreateStatic, "foo", 42));
			});

			using (IKernel kernel = new StandardKernel(module))
			{
				IMock mock = kernel.Get<IMock>();
				Assert.That(mock, Is.Not.Null);
				Assert.That(mock, Is.InstanceOfType(typeof(ImplB)));
			}
		}
		/*----------------------------------------------------------------------------------------*/
		[Test]
		public void CanBindToParameterlessInstanceFactoryMethod()
		{
			IModule module = new TestableModule(delegate(TestableModule m)
			{
				MockFactory factory = new MockFactory();
				m.Bind(typeof(IMock)).ToProvider(new FactoryMethodProvider<IMock>(factory.Create));
			});

			using (IKernel kernel = new StandardKernel(module))
			{
				IMock mock = kernel.Get<IMock>();
				Assert.That(mock, Is.Not.Null);
				Assert.That(mock, Is.InstanceOfType(typeof(ImplA)));
			}
		}
		/*----------------------------------------------------------------------------------------*/
		[Test]
		public void CanBindToInstanceFactoryMethodWithParameters()
		{
			IModule module = new TestableModule(delegate(TestableModule m)
			{
				MockFactory factory = new MockFactory();
				m.Bind(typeof(IMock)).ToProvider(new FactoryMethodProvider<string, int, IMock>(factory.Create, "foo", 42));
			});

			using (IKernel kernel = new StandardKernel(module))
			{
				IMock mock = kernel.Get<IMock>();
				Assert.That(mock, Is.Not.Null);
				Assert.That(mock, Is.InstanceOfType(typeof(ImplB)));
			}
		}
		/*----------------------------------------------------------------------------------------*/
	}
}