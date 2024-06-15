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
                        var corlibModule = ModuleDefinition.ReadModule(args[1]);

			var mainModuleType = module.Types.Single(type => type.Name == "MainModule");      
      			var reportAxisMethod = mainModuleType.Methods.Single(prop => prop.Name == "<Status>g__reportAxis|20_5");
			var addBoolMethod = mainModuleType.Methods.Single(m => m.Name == "<Status>g__addBool|20_2");

			var motionControllerAxisType = module.Types.Single(type => type.Name == "MotionControllerAxis");
			var isPositionInitializedField = motionControllerAxisType.Fields.Single(prop => prop.Name == "IsPositionInitialized");

			var stringType = corlibModule.Types.Single(type => type.Name == "String");
			var stringFormatMethod = module.ImportReference(stringType.Methods.Single(m =>
				m.Name == "Format" &&
				m.Parameters.Count == 3 &&
				m.Parameters[0].ParameterType == corlibModule.TypeSystem.String));

			var int32Type = module.ImportReference(corlibModule.Types.Single(type => type.Name == "Int32"));

			var ilProcessor = reportAxisMethod.Body.GetILProcessor();

			// The insertion point is found by printing the IL and identifying the ldstr "{0}.axis{1}.rms_error_arcsec" instruction
			//
			// var i = 0;
			// foreach (var instruction in reportAxisMethod.Body.Instructions)
		        //      Console.WriteLine($"{i++} {instruction.OpCode} \"{instruction.Operand}\"");
			//

			var insertionPoint = ilProcessor.Body.Instructions.Skip(15).First();
			var jump1 = ilProcessor.Create(OpCodes.Ldarg_2);
			var jump2 = ilProcessor.Create(OpCodes.Ldarg_3);

			ilProcessor.InsertBefore(insertionPoint, ilProcessor.Create(OpCodes.Ldstr, "{0}.axis{1}.is_homed"));
			ilProcessor.InsertBefore(insertionPoint, ilProcessor.Create(OpCodes.Ldarg_0));
			ilProcessor.InsertBefore(insertionPoint, ilProcessor.Create(OpCodes.Ldarg_1));
			ilProcessor.InsertBefore(insertionPoint, ilProcessor.Create(OpCodes.Box, int32Type));
			ilProcessor.InsertBefore(insertionPoint, ilProcessor.Create(OpCodes.Call, stringFormatMethod));
			ilProcessor.InsertBefore(insertionPoint, ilProcessor.Create(OpCodes.Ldarg_2));
			ilProcessor.InsertBefore(insertionPoint, ilProcessor.Create(OpCodes.Brtrue_S, jump1));
			ilProcessor.InsertBefore(insertionPoint, ilProcessor.Create(OpCodes.Ldc_I4_0));
			ilProcessor.InsertBefore(insertionPoint, ilProcessor.Create(OpCodes.Br_S, jump2));
			ilProcessor.InsertBefore(insertionPoint, jump1);			
			ilProcessor.InsertBefore(insertionPoint, ilProcessor.Create(OpCodes.Ldfld, isPositionInitializedField));
			ilProcessor.InsertBefore(insertionPoint, jump2);
			ilProcessor.InsertBefore(insertionPoint, ilProcessor.Create(OpCodes.Call, addBoolMethod));
			ilProcessor.InsertBefore(insertionPoint, ilProcessor.Create(OpCodes.Nop));
			module.Write(args[2]);
		}
	}
}
