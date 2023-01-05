using System;
using System.Data;

namespace VM.Lab.Interfaces.BlobAnalyzer
{
	/// <summary> 
	/// Blob Analyzer communication controller
	/// Types that implement this class will be detected and gives the implementer the ability to control the different Blob Analyzers
	/// </summary>
	public abstract class AutofeederControl : IDisposable
	{
		/// <summary>Implementer of autofeeder control commands</summary>
		protected IAutofeederControlListener _listener;

		/// <summary> Base class for autofeeder controller </summary>
		/// <param name="listener">Implementer of autofeeder control commands</param>
		public AutofeederControl(IAutofeederControlListener listener)
		{
			_listener = listener;
		}

		/// <summary>Tells the controller that the blob analyzer has changed state</summary>
		public abstract void StateChanged(BlobAnalyzerState newState);

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		/// <summary>
		/// Dispose duh
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					
				}
				disposedValue = true;
			}
		}


		/// <summary>
		/// This code added to correctly implement the disposable pattern.
		/// </summary>
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
		}
		#endregion
	}
}
