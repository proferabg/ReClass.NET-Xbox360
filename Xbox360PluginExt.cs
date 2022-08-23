using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using JRPC_Client;
using ReClassNET.Core;
using ReClassNET.Debugger;
using ReClassNET.Memory;
using ReClassNET.Nodes;
using ReClassNET.Plugins;
using ReClassNET.Util.Conversion;
using Xbox360Plugin.Pointer32;
using XDevkit;

namespace Xbox360Plugin
{
    public class Xbox360PluginExt : Plugin, ICoreProcessFunctions
    {
        private readonly object sync = new object();

        private IPluginHost host;
        private IXboxConsole console;
        private IXboxDebugTarget debugTarget;

        private byte[] lastBuffer;
        private uint lastAddress;

        public override Image Icon => Properties.Resources.icon;

        public override bool Initialize(IPluginHost host)
        {
            this.host = host ?? throw new ArgumentNullException(nameof(host));

            host.Process.CoreFunctions.RegisterFunctions("Xbox 360", this);
            host.Process.BitConverter = EndianBitConverter.Big;

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
            host.MainWindow.CurrentClassNode.AddressFormula = ((uint)(id.ToInt64() & 0xFFFFFFFF)).ToString("X");
            return id;
        }

        public void CloseRemoteProcess(IntPtr process)
        {
            debugTarget.DisconnectAsDebugger();
        }

        public bool ReadRemoteMemory(IntPtr process, IntPtr address, ref byte[] buffer, int offset, int size)
        {
            lock (sync)
            {
                try
                {
                    uint newAddress = (uint)(address.ToInt64() & 0xFFFFFFFF);
                    byte[] newBuffer = console.GetMemory(newAddress, (uint)size);
                    if (checkNotEmpty(newBuffer))
                    {
                        Buffer.BlockCopy(newBuffer, 0, buffer, 0, size);
                        lastAddress = newAddress;
                        lastBuffer = newBuffer;
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                // Prevents data from nulling when peeking memory fails.
                if (lastBuffer != null && (uint)(address.ToInt64() & 0xFFFFFFFF) == lastAddress)
                {
                    Buffer.BlockCopy(lastBuffer, 0, buffer, 0, size);
                    return true;
                }

                return false;
            }
        }

        private bool checkNotEmpty(byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0) return false;
            foreach (byte b in buffer)
            {
                if (b > 0) return true;
            }

            return false;
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
            lock (sync)
            {
                try
                {
                    console.Connect(out console);
                    debugTarget = console.DebugTarget;
                    debugTarget.ConnectAsDebugger(console.XboxIP(), XboxDebugConnectFlags.Force);

                    foreach (IXboxModule module in debugTarget.Modules)
                    {
                        XBOX_MODULE_INFO moduleInfo = module.ModuleInfo;
                        var data = new EnumerateProcessData
                        {
                            Id = new IntPtr(moduleInfo.BaseAddress),
                            Name = moduleInfo.Name,
                            Path = "Base(0x" + moduleInfo.BaseAddress.ToString("X") + "), Entry(0x" +
                                   module.GetEntryPointAddress().ToString("X") + "), Size(0x" +
                                   moduleInfo.Size.ToString("X") + "), CRC32(0x" + moduleInfo.GetHashCode().ToString("X") + ")",
                        };
                        callbackProcess(ref data);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not connect Xbox 360.", "Connection Error");
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public void EnumerateRemoteSectionsAndModules(IntPtr process, EnumerateRemoteSectionCallback callbackSection, EnumerateRemoteModuleCallback callbackModule)
        {
            lock (sync)
            {
                try
                {
                    foreach (IXboxModule module in debugTarget.Modules)
                    {
                        uint processAddress = (uint)(process.ToInt64() & 0xFFFFFFFF);
                        if (processAddress != module.ModuleInfo.BaseAddress) continue;

                        XBOX_MODULE_INFO moduleInfo = module.ModuleInfo;
                        var moduleData = new EnumerateRemoteModuleData
                        {
                            BaseAddress = new IntPtr(moduleInfo.BaseAddress),
                            Path = moduleInfo.Name,
                            Size = new IntPtr(moduleInfo.Size),
                        };
                        callbackModule(ref moduleData);

                        foreach (IXboxSection section in module.Sections)
                        {
                            XBOX_SECTION_INFO sectionInfo = section.SectionInfo;
                            SectionCategory category = SectionCategory.Unknown;
                            if ((sectionInfo.Flags & XboxSectionInfoFlags.Executable) == XboxSectionInfoFlags.Executable || new string[] { ".text", ".ptext" }.Contains(sectionInfo.Name)) category = SectionCategory.CODE;
                            else if (new string[]{ ".data", ".rdata" }.Contains(sectionInfo.Name)) category = SectionCategory.DATA;

                            SectionProtection protection = SectionProtection.NoAccess;
                            if ((sectionInfo.Flags & XboxSectionInfoFlags.Executable) == XboxSectionInfoFlags.Executable)
                                protection |= SectionProtection.Execute;
                            if ((sectionInfo.Flags & XboxSectionInfoFlags.Writeable) == XboxSectionInfoFlags.Writeable)
                                protection |= SectionProtection.Write;
                            if ((sectionInfo.Flags & XboxSectionInfoFlags.Readable) == XboxSectionInfoFlags.Readable)
                                protection |= SectionProtection.Read;

                            var sectionData = new EnumerateRemoteSectionData
                            {
                                BaseAddress = new IntPtr(sectionInfo.BaseAddress),
                                Size = new IntPtr(sectionInfo.Size),
                                Type = SectionType.Image,
                                Category = category,
                                ModulePath = moduleInfo.Name,
                                Name = sectionInfo.Name,
                                Protection = protection
                            };
                            callbackSection(ref sectionData);
                        }

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public void ControlRemoteProcess(IntPtr process, ControlRemoteProcessAction action)
        {
            MessageBox.Show("Not yet implemented!", "Error");
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

        public override CustomNodeTypes GetCustomNodeTypes()
        {
            return new CustomNodeTypes
            {
                CodeGenerator = new Pointer32CodeGenerator(),
                Serializer = new Pointer32NodeConverter(),
                NodeTypes = new[] { typeof(Pointer32Node) }
            };
        }
    }
}
