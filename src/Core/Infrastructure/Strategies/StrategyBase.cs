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
using Ninject.Core.Logging;
#endregion

namespace Ninject.Core.Infrastructure
{
	/// <summary>
	/// A baseline definition of a strategy. This type may be extended to create
	/// custom strategy types.
	/// </summary>
	public abstract class StrategyBase : DisposableObject, IStrategy
	{
		/*----------------------------------------------------------------------------------------*/
		#region Fields
		private ILogger _logger;
		#endregion
		/*----------------------------------------------------------------------------------------*/
		#region Properties
		/// <summary>
		/// Gets the kernel associated with the strategy.
		/// </summary>
		public IKernel Kernel { get; private set; }
		/*----------------------------------------------------------------------------------------*/
		/// <summary>
		/// Gets the logger associated with the strategy.
		/// </summary>
		public ILogger Logger
		{
			get
			{
				if (_logger == null)
					_logger = Kernel.Components.LoggerFactory.GetLogger(GetType());

				return _logger;
			}
		}
		/*----------------------------------------------------------------------------------------*/
		/// <summary>
		/// Gets a value indicating whether the strategy has been connected to its environment.
		/// </summary>
		public bool IsConnected
		{
			get { return Kernel != null; }
		}
		#endregion
		/*----------------------------------------------------------------------------------------*/
		#region Events
		/// <summary>
		/// Indicates that the strategy has been connected to its environment.
		/// </summary>
		public event EventHandler Connected = delegate { };
		/*----------------------------------------------------------------------------------------*/
		/// <summary>
		/// Indicates that the strategy has been disconnected from its environment.
		/// </summary>
		public event EventHandler Disconnected = delegate { };
		#endregion
		/*----------------------------------------------------------------------------------------*/
		#region Event Sources
		/// <summary>
		/// Called when the strategy is connected to its environment.
		/// </summary>
		/// <param name="args">The event arguments.</param>
		protected virtual void OnConnected(EventArgs args)
		{
			Connected(this, args);
		}
		/*----------------------------------------------------------------------------------------*/
		/// <summary>
		/// Called when the strategy is disconnected from its environment.
		/// </summary>
		/// <param name="args">The event arguments.</param>
		protected virtual void OnDisconnected(EventArgs args)
		{
			Disconnected(this, args);
		}
		#endregion
		/*----------------------------------------------------------------------------------------*/
		#region Disposal
		/// <summary>
		/// Releases all resources held by the object.
		/// </summary>
		/// <param name="disposing"><see langword="True"/> if managed objects should be disposed, otherwise <see langword="false"/>.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && !IsDisposed)
			{
				if (IsConnected)
					Disconnect();

				DisposeMember(_logger);

				Kernel = null;
				_logger = null;
			}

			base.Dispose(disposing);
		}
		#endregion
		/*----------------------------------------------------------------------------------------*/
		#region Public Methods
		/// <summary>
		/// Connects the strategy to its environment.
		/// </summary>
		/// <param name="kernel">The kernel to associate the strategy with.</param>
		public void Connect(IKernel kernel)
		{
			Ensure.NotDisposed(this);
			Kernel = kernel;
			OnConnected(new EventArgs());
		}
		/*----------------------------------------------------------------------------------------*/
		/// <summary>
		/// Disconnects the strategy from its environment.
		/// </summary>
		public void Disconnect()
		{
			Ensure.NotDisposed(this);
			OnDisconnected(new EventArgs());
			Kernel = null;
		}
		#endregion
		/*----------------------------------------------------------------------------------------*/
	}
}