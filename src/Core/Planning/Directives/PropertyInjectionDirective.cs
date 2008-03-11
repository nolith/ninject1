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
using System.Reflection;
using System.Text;
using Ninject.Core.Injection;
using Ninject.Core.Planning.Targets;
#endregion

namespace Ninject.Core.Planning.Directives
{
	/// <summary>
	/// A directive that describes a property injection.
	/// </summary>
	[Serializable]
	public class PropertyInjectionDirective : SingleInjectionDirective<PropertyInfo, IPropertyInjector>
	{
		/*----------------------------------------------------------------------------------------*/
		#region Constructors
		/// <summary>
		/// Creates a new PropertyInjectionDirective.
		/// </summary>
		/// <param name="member">The member that the directive relates to.</param>
		/// <param name="injector">The injector that will be used to inject a value into the property.</param>
		public PropertyInjectionDirective(PropertyInfo member, IPropertyInjector injector)
			: base(member, injector, new PropertyTarget(member))
		{
		}
		#endregion
		/*----------------------------------------------------------------------------------------*/
		#region Protected Methods
		/// <summary>
		/// Builds the value that uniquely identifies the directive. This is called the first time
		/// the key is accessed, and then cached in the directive.
		/// </summary>
		/// <returns>The directive's unique key.</returns>
		protected override object BuildKey()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append(Member.Name);
			sb.Append(Member.PropertyType);

			return sb.ToString();
		}
		#endregion
		/*----------------------------------------------------------------------------------------*/
	}
}