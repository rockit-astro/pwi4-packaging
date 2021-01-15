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
			var module = ModuleDefinition.ReadModule(args[0]);
			var pathsType = module.Types.First(type => type.Name == "Paths");
			var homeDirType = pathsType.Properties.First(prop => prop.Name == "PlaneWaveHomeDir").GetMethod;
			var ilProcessor = homeDirType.Body.GetILProcessor();
			var firstInstruction = ilProcessor.Body.Instructions.First();
			ilProcessor.InsertBefore(firstInstruction, ilProcessor.Create(OpCodes.Ldstr, args[2]));
			ilProcessor.InsertBefore(firstInstruction, ilProcessor.Create(OpCodes.Ret));

			module.Write(args[1]);
		}
	}
}
