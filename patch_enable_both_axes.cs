using System;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;


namespace PWI4Patcher
{
	class Program
	{
		static void Main(string[] args)
		{
			Directory.SetCurrentDirectory(Path.GetDirectoryName(args[0]));

			// Insert a mount.axisN.is_homed line into the status output after mount.axisN.is_enabled
			var module = ModuleDefinition.ReadModule(args[0]);

			var mainModuleType = module.Types.Single(type => type.Name == "MainModule");
			var enableDisableMethod = mainModuleType.Methods.Single(m => m.Name == "MountEnableDisable");

			var appType = module.Types.Single(t => t.Name == "App");
			var mountManagerField = appType.Fields.Single(f => f.Name == "MountManager");

			var mountManagerType = module.Types.Single(t => t.Name == "MountManager");
			var setAxisEnabledMethod = mountManagerType.Methods.Single(m => m.Name == "SetAxisEnabled");			

			var ilProcessor = enableDisableMethod.Body.GetILProcessor();
			var insertionPoint = ilProcessor.Body.Instructions.Skip(7).First();
			var jump1 = ilProcessor.Body.Instructions.Skip(7).First(); // The 'nop' before 'ldsfld "PWI4.Mount.MountManager PWI4.App::MountManager"'
			var jump2 = ilProcessor.Body.Instructions.Skip(12).First(); // The 'nop' after 'callvirt "System.Void PWI4.Mount.MountManager::SetAxisEnabled(System.Int32,System.Boolean)"'

			ilProcessor.InsertBefore(insertionPoint, ilProcessor.Create(OpCodes.Ldloc_0));
			ilProcessor.InsertBefore(insertionPoint, ilProcessor.Create(OpCodes.Ldc_I4_0));
			ilProcessor.InsertBefore(insertionPoint, ilProcessor.Create(OpCodes.Bge_S, jump1)); // jump to original call

			// Enable axis 0
			ilProcessor.InsertBefore(insertionPoint, ilProcessor.Create(OpCodes.Nop));
			ilProcessor.InsertBefore(insertionPoint, ilProcessor.Create(OpCodes.Ldsfld, mountManagerField));	
			ilProcessor.InsertBefore(insertionPoint, ilProcessor.Create(OpCodes.Ldc_I4_0));
			ilProcessor.InsertBefore(insertionPoint, ilProcessor.Create(OpCodes.Ldarg_2));
			ilProcessor.InsertBefore(insertionPoint, ilProcessor.Create(OpCodes.Callvirt, setAxisEnabledMethod));
			
			// Enable axis 1
			ilProcessor.InsertBefore(insertionPoint, ilProcessor.Create(OpCodes.Nop));
			ilProcessor.InsertBefore(insertionPoint, ilProcessor.Create(OpCodes.Ldsfld, mountManagerField));	
			ilProcessor.InsertBefore(insertionPoint, ilProcessor.Create(OpCodes.Ldc_I4_1));
			ilProcessor.InsertBefore(insertionPoint, ilProcessor.Create(OpCodes.Ldarg_2));
			ilProcessor.InsertBefore(insertionPoint, ilProcessor.Create(OpCodes.Callvirt, setAxisEnabledMethod));

			// Jump past original call
			ilProcessor.InsertBefore(insertionPoint, ilProcessor.Create(OpCodes.Br_S, jump2));

			module.Write(args[1]);
		}
	}
}
