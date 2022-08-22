using System;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Windows.Forms;
using JRPC_Client;
using ReClassNET.Core;
using ReClassNET.Debugger;
using ReClassNET.Plugins;
using XDevkit;

namespace Xbox360Plugin
{
	public class Xbox360PluginExt : Plugin, ICoreProcessFunctions
	{
		private readonly object sync = new object();

		private IPluginHost host;
        IXboxConsole console;
        public override Image Icon => Properties.Resources.icon;

		public override bool Initialize(IPluginHost host)
		{
			Contract.Requires(host != null);

			this.host = host ?? throw new ArgumentNullException(nameof(host));

			host.Process.CoreFunctions.RegisterFunctions("Xbox 360", this);

			return true;
		}

		public override void Terminate()
		{
            host = null;
		}
		
		public bool IsProcessValid(IntPtr process)
		{
			return true;
		}
		
		public IntPtr OpenRemoteProcess(IntPtr id, ProcessAccess desiredAccess)
		{
            return IntPtr.Zero;
		}
		
		public void CloseRemoteProcess(IntPtr process)
		{
			
		}
		
		public bool ReadRemoteMemory(IntPtr process, IntPtr address, ref byte[] buffer, int offset, int size)
		{
			lock (sync)
            {
                try
                {
                    uint newAddr = (uint)(address.ToInt64() & 0xFFFFFFFF);
                    byte[] data = console.GetMemory(newAddr, (uint)size);
                    Buffer.BlockCopy(data, 0, buffer, 0, size);
                    return true;
                }
                catch (Exception ex)
                {
					Console.WriteLine(ex.Message);
                    return false;
                }
			}
		}

		public bool WriteRemoteMemory(IntPtr process, IntPtr address, ref byte[] buffer, int offset, int size)
		{
            lock (sync)
            {
                try
                {
                    uint newAddr = (uint)(address.ToInt64() & 0xFFFFFFFF);
                    byte[] newBuffer = new byte[size];
                    Buffer.BlockCopy(buffer, 0, newBuffer, 0, size);
                    console.SetMemory(newAddr, newBuffer);
                    return true;
                }
                catch (Exception ex)
                {
					Console.WriteLine(ex.Message);
                    return false;
                }
            }
		}

		public void EnumerateProcesses(EnumerateProcessCallback callbackProcess)
        {
            try
            {
                console.Connect(out console);

                var data = new EnumerateProcessData
                {
                    Id = (IntPtr)0,
                    Name = console.Name,
                    Path = console.XboxIP()
                };
                callbackProcess(ref data);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error", "Could not connect Xbox 360.");
                Console.WriteLine(ex.Message);
}
        }

		public void EnumerateRemoteSectionsAndModules(IntPtr process, EnumerateRemoteSectionCallback callbackSection, EnumerateRemoteModuleCallback callbackModule)
		{
            // Not supported.
		}

		public void ControlRemoteProcess(IntPtr process, ControlRemoteProcessAction action)
		{
			// Not supported.
		}

		public bool AttachDebuggerToProcess(IntPtr id)
		{
			// Not supported.
			return false;
		}

		public void DetachDebuggerFromProcess(IntPtr id)
		{
			// Not supported.
		}

		public bool AwaitDebugEvent(ref DebugEvent evt, int timeoutInMilliseconds)
		{
			// Not supported.

			return false;
		}

		public void HandleDebugEvent(ref DebugEvent evt)
		{
			// Not supported.
		}

		public bool SetHardwareBreakpoint(IntPtr id, IntPtr address, HardwareBreakpointRegister register, HardwareBreakpointTrigger trigger, HardwareBreakpointSize size, bool set)
		{
			// Not supported.

			return false;
		}
	}
}
