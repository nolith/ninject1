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
using Ninject.Core.Infrastructure;
using Ninject.Core.Interception;
#endregion

namespace Ninject.Core.Interception
{
	/// <summary>
	/// An interceptor registration.
	/// </summary>
	public class InterceptorRegistration : IComparable<InterceptorRegistration>
	{
		/*----------------------------------------------------------------------------------------*/
		#region Properties
		/// <summary>
		/// Gets the type of interceptor that should be invoked.
		/// </summary>
		public Type InterceptorType { get; private set; }
		/*----------------------------------------------------------------------------------------*/
		/// <summary>
		/// Gets the condition that should be evaluated, if this registration is dynamic.
		/// </summary>
		/// <value>The condition.</value>
		public ICondition<IRequest> Condition { get; private set; }
		/*----------------------------------------------------------------------------------------*/
		/// <summary>
		/// Gets the order of precedence that the interceptor should be called in.
		/// </summary>
		public int Order { get; private set; }
		#endregion
		/*----------------------------------------------------------------------------------------*/
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="InterceptorRegistration"/> class.
		/// </summary>
		/// <param name="interceptorType">The type of interceptor that should be invoked.</param>
		/// <param name="order">The order of precedence that the interceptor should be called in.</param>
		public InterceptorRegistration(Type interceptorType, int order)
		{
			Ensure.ArgumentNotNull(interceptorType, "interceptorType");
			
			InterceptorType = interceptorType;
			Order = order;
		}
		/*----------------------------------------------------------------------------------------*/
		/// <summary>
		/// Initializes a new instance of the <see cref="InterceptorRegistration"/> class.
		/// </summary>
		/// <param name="interceptorType">The type of interceptor that should be invoked.</param>
		/// <param name="order">The order of precedence that the interceptor should be called in.</param>
		/// <param name="condition">The condition that should be evaluated.</param>
		public InterceptorRegistration(Type interceptorType, int order, ICondition<IRequest> condition)
			: this(interceptorType, order)
		{
			Ensure.ArgumentNotNull(condition, "condition");
			Condition = condition;
		}
		#endregion
		/*----------------------------------------------------------------------------------------*/
		#region IComparable Implementation
		int IComparable<InterceptorRegistration>.CompareTo(InterceptorRegistration other)
		{
			return Order - other.Order;
		}
		#endregion
		/*----------------------------------------------------------------------------------------*/
	}
}