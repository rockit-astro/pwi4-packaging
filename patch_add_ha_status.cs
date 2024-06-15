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

			// Insert a mount.ha_hours line into the status output between mount.dec_j2000_degs and mount.target_ra_apparent_hours
			var module = ModuleDefinition.ReadModule(args[0]);
			var libModule = ModuleDefinition.ReadModule(args[1]);

			var mainModuleType = module.Types.Single(type => type.Name == "MainModule");
			var statusMethod = mainModuleType.Methods.Single(prop => prop.Name == "Status");

			var addDoubleMethod = mainModuleType.Methods.Single(m => m.Name == "<Status>g__addDouble|20_1");
			var v0 = statusMethod.Body.Variables.Single(v => v.Index == 0);
			var v4 = statusMethod.Body.Variables.Single(v => v.Index == 4);

			var mountManagerStatusType = module.Types.Single(type => type.Name == "MountManagerStatus");
			var teleHAField = mountManagerStatusType.Fields.Single(prop => prop.Name == "TeleHa");

			var angleType = libModule.Types.Single(type => type.Name == "Angle");
			var angleGetHoursMethod = module.ImportReference(angleType.Properties.Single(prop => prop.Name == "Hours").GetMethod);
			var ilProcessor = statusMethod.Body.GetILProcessor();

			// The insertion point is found by printing the IL and identifying the ldstr "mount.ra_apparent_hours" instruction
			//
			// var i = 0;
			// foreach (var instruction in statusMethod.Body.Instructions)
		        //      Console.WriteLine($"{i++} {instruction.OpCode} \"{instruction.Operand}\"");
			//

			var insertionPoint = ilProcessor.Body.Instructions.Skip(166).First();
			ilProcessor.InsertBefore(insertionPoint, ilProcessor.Create(OpCodes.Ldstr, "mount.ha_hours"));
			ilProcessor.InsertBefore(insertionPoint, ilProcessor.Create(OpCodes.Ldloc_S, v4));
			ilProcessor.InsertBefore(insertionPoint, ilProcessor.Create(OpCodes.Ldflda, teleHAField));
			ilProcessor.InsertBefore(insertionPoint, ilProcessor.Create(OpCodes.Call, angleGetHoursMethod));
			ilProcessor.InsertBefore(insertionPoint, ilProcessor.Create(OpCodes.Ldloca_S, v0));
			ilProcessor.InsertBefore(insertionPoint, ilProcessor.Create(OpCodes.Call, addDoubleMethod));
			ilProcessor.InsertBefore(insertionPoint, ilProcessor.Create(OpCodes.Nop));

			module.Write(args[2]);
		}
	}
}
