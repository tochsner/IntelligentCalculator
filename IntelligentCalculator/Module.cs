using System;
using System.Collections.Generic;
using System.Linq;

namespace IntelligentCalculator
{
    public class Module : BaseModule
    {
        private static readonly Random randomizer = new Random();

        public static List<string> GenerateInstructions(int inputsCount, string parentIdentifier, bool useLoops = true)
        {
            List<string> instructions = new List<string>();

            List<string> operands = new List<string>();

            for (int i = 0; i < inputsCount; i++)
            {
                operands.Add("i" + i);
                operands.Add("i" + i);
            }                

            double rand = randomizer.NextDouble();

            if (rand < Constants.SimpleScalingPropability)
            {
                instructions.Add(operands[randomizer.Next(operands.Count)]);
                instructions.Add("1");
                instructions.Add("f");
            }
            else if (rand < (Constants.LoopPropability + Constants.SimpleScalingPropability) && useLoops)
            {
                string randomIdentfier = ModuleManager.GetRandomIdentifier(parentIdentifier);

                while (ModuleManager.GetInputCount(randomIdentfier) != 2)
                    randomIdentfier = ModuleManager.GetRandomIdentifier(parentIdentifier);

                instructions.Add(operands[randomizer.Next(operands.Count)]);
                instructions.Add("1");
                instructions.Add("f");
                instructions.Add("r");
                instructions.Add(randomIdentfier);

                operands.Add("k");

                instructions.Add(operands[randomizer.Next(operands.Count)]);
                instructions.Add("1");
                instructions.Add("f");
            }
            else
            {
                string randomIdentfier = ModuleManager.GetRandomIdentifier(parentIdentifier);

                for (int i = 0; i < ModuleManager.GetInputCount(randomIdentfier); i++)
                {
                    instructions.Add(operands[randomizer.Next(operands.Count)]);
                    instructions.Add("1");
                    instructions.Add("f");
                }

                instructions.Add(randomIdentfier);               
            }

            return instructions;
        }
        public static Module GenerateSimpleModule(string identifier, int inputsCount)
        {         
            return new Module(identifier, inputsCount, GenerateInstructions(inputsCount, identifier));
        }
        
        public List<string> Instructions;
        private List<BaseModule> modulesUsedLast = new List<BaseModule>();

        public Module(string identifier, int inputsCount, List<string> instructions)
        {
            this.Identifier = identifier;

            this.Instructions = instructions;
            this.InputsCount = inputsCount;

            CalculateComplexity();
        }
  
        public override double Compute(List<double> inputs)
        {
            if (Instructions.Contains("k") && !Instructions.Contains("r"))
                return 0;

            Stack<double> stack = new Stack<double>();
            List<string> instructionsToBeDone = new List<string>(Instructions);

            stack.Push(0);

            while (instructionsToBeDone.Count != 0)
            {            
                string instruction = instructionsToBeDone[0];
                instructionsToBeDone.RemoveAt(0);

                if (instructionsToBeDone.Count > 500)
                    return 0;

                if (Double.TryParse(instruction, out double currentValue))
                {
                    stack.Push(currentValue);
                }
                else if (instruction.StartsWith("i") && int.TryParse(instruction.Substring(1), out int index))
                {
                    stack.Push(inputs[index]);
                }
                else if (instruction == "r")
                {
                    int repetitions = (int) Math.Abs(stack.Pop());
                    string operation = instructionsToBeDone[0];
                    instructionsToBeDone.RemoveAt(0);

                    List<string> instructionsToBeRepeated = new List<string>(instructionsToBeDone);
                    instructionsToBeDone.Clear();                      

                    for (int k = 1; k <= repetitions; k++)
                    {
                        foreach (string instructionToBeRepeated in instructionsToBeRepeated)
                        {
                            if (instructionToBeRepeated == "k")
                                instructionsToBeDone.Add(k.ToString());
                            else
                                instructionsToBeDone.Add(instructionToBeRepeated);
                        }
                        
                        if (k > 1)
                            instructionsToBeDone.Add(operation);
                    }
                }
                else
                {
                    if (!ModuleManager.HasIdentifier(instruction))
                        ModuleManager.AddIdentifier(instruction, stack.Count - 1);

                    BaseModule currentModule = ModuleManager.GetModule(instruction);                    

                    modulesUsedLast.Add(currentModule);

                    List<double> arguments = new List<double>();

                    for (int i = 0; i < currentModule.InputsCount; i++)
                    {
                        arguments.Insert(0, stack.Pop());
                    }

                    stack.Push(currentModule.Compute(arguments));
                }
            }

            return stack.Pop();
        }

        private void CalculateComplexity()
        {
            Complexity = 0;

            bool inLoop = false;

            foreach (string instruction in Instructions)
            {
                if (instruction == "r")
                    inLoop = true;
                else if (!inLoop && ModuleManager.HasIdentifier(instruction))
                    Complexity += ModuleManager.GetModule(instruction).Complexity;
                else if (inLoop && ModuleManager.HasIdentifier(instruction))
                    Complexity += Constants.LoopComplexity * ModuleManager.GetModule(instruction).Complexity;
            }
        }

        public override void Complexify()
        {
            List<string> operands = new List<string>();

            for (int i = 0; i < InputsCount; i++)
            {
                operands.Add("i" + i);
                operands.Add("i" + i);
            }

            operands.Add("k");

            if (!Instructions.Contains("r"))
            {
                string randomIdentfier = ModuleManager.GetRandomIdentifier(Identifier);

                for (int i = 1; i < ModuleManager.GetInputCount(randomIdentfier); i++)
                {
                    Instructions.Add(operands[randomizer.Next(operands.Count)]);
                    Instructions.Add("1");
                    Instructions.Add("f");
                }

                Instructions.Add(randomIdentfier);
            }
            else
            {
                int mutationIndex = randomizer.Next(Instructions.Count);

                while (!operands.Contains(Instructions[mutationIndex]))
                    mutationIndex = randomizer.Next(Instructions.Count);

                List<string> newInstruction = GenerateInstructions(InputsCount, Identifier, false);

                Instructions.RemoveAt(mutationIndex);
                Instructions.RemoveAt(mutationIndex);
                Instructions.RemoveAt(mutationIndex);
                Instructions.InsertRange(mutationIndex, newInstruction);
            }            

            CalculateComplexity();
        }

        public override double Fitness {
            get {
                return Math.Min(1, Constants.NoveltyFactor * Math.Exp(-TimesUsed) + Math.Exp(-5 * AverageRelativeError));
            }
        }

        public override void SaveError(double relativeError)
        {
            AverageRelativeError = (AverageRelativeError * TimesUsed + relativeError) / (TimesUsed + 1);

            TimesUsed++;

            double totalWeightedFitness = modulesUsedLast.Sum(x => x.AverageRelativeError);

            foreach (BaseModule module in modulesUsedLast)
            {
                if (module.TimesUsed > 0 && totalWeightedFitness > 0)
                    module.SaveError(relativeError * module.AverageRelativeError / totalWeightedFitness);
                else
                    module.SaveError(relativeError);
            }

            modulesUsedLast.Clear();        
        }

        public override string ToString()
        {
            return String.Join(" ", Instructions) + " | " + Complexity + " | " + Math.Round(Fitness, 1);
        }

        public override BaseModule GetMutatedModule()
        {
            List<string> newInstructions = new List<string>(Instructions);

            int mutationIndex = randomizer.Next(newInstructions.Count);

            while (newInstructions[mutationIndex] == "r" || newInstructions[mutationIndex] == "f" || ModuleManager.HasIdentifier(newInstructions[mutationIndex]))
                mutationIndex = randomizer.Next(newInstructions.Count);

            List<string> operands = new List<string>();

            for (int i = 0; i < InputsCount; i++)
                operands.Add("i" + i);

            if (newInstructions.Contains("r") && newInstructions.IndexOf("r") < mutationIndex)
                operands.Add("k");

            double rand = randomizer.NextDouble();

            if (Double.TryParse(newInstructions[mutationIndex], out double value) && newInstructions[mutationIndex + 1] == "f")
            {
                if (rand < Constants.FactorPertubationProbability / 2)
                    newInstructions[mutationIndex] = (value * randomizer.Next(3)).ToString();
                else if (rand < Constants.FactorPertubationProbability)
                    newInstructions[mutationIndex] = (value / randomizer.Next(1, 3)).ToString();
            }
            else if (operands.Contains(newInstructions[mutationIndex]))
            {
                newInstructions[mutationIndex] = operands[randomizer.Next(operands.Count)];
            }
            
            return new Module(Identifier, InputsCount, newInstructions);
        }

        public override BaseModule GetComplexifiedModule()
        {
            BaseModule newModule = new Module(Identifier, InputsCount, Instructions);
            newModule.Complexify();
            return newModule;
        }
    }
}