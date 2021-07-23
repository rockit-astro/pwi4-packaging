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
			// Replace the body of the Paths.PlaneWaveHomeDir property with `return "/var/opt/pwi4";`
			var module = ModuleDefinition.ReadModule(args[0]);
			var pathsType = module.Types.Single(type => type.Name == "Paths");
			var homeDirMethod = pathsType.Properties.Single(prop => prop.Name == "PlaneWaveHomeDir").GetMethod;
			var ilProcessor = homeDirMethod.Body.GetILProcessor();
			var firstInstruction = ilProcessor.Body.Instructions.First();
			ilProcessor.InsertBefore(firstInstruction, ilProcessor.Create(OpCodes.Ldstr, "/var/opt/pwi4"));
			ilProcessor.InsertBefore(firstInstruction, ilProcessor.Create(OpCodes.Ret));

			module.Write(args[1]);
		}
	}
}
