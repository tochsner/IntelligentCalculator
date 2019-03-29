using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentCalculator
{
    public class ModuleManager
    {        
        private static readonly Random randomizer = new Random();
        private static SelectionModes SelectionMode = SelectionModes.Best;
        
        private static readonly Dictionary<string, List<BaseModule>> identifierPopulations;
        private static readonly Dictionary<string, int> identifierInputCounts;
        private static readonly Dictionary<string, bool> identifierFullyTrained;

        static ModuleManager()
        {
            identifierPopulations = new Dictionary<string, List<BaseModule>>
            {
                { "+", new List<BaseModule>() { new AddModule() } } ,
                { "/", new List<BaseModule>() { new DivisionModule() } } ,
                { "f", new List<BaseModule>() { new FactorModule() } }
            };
            identifierInputCounts = new Dictionary<string, int>
            {
                { "+", 2 } ,
                { "/", 2 } ,
                { "f", 1 }
            };
            identifierFullyTrained = new Dictionary<string, bool>
            {
                { "+", true } ,
                { "/", true } ,
                { "f", true }
            };
        }

        public static void AddIdentifier(string identifier, int inputsCount)
        {
            identifierPopulations.Add(identifier, new List<BaseModule>());            

            for (int i = 0; i < Constants.PopulationSize; i++)
                identifierPopulations[identifier].Add(Module.GenerateSimpleModule(identifier, inputsCount));

            identifierInputCounts.Add(identifier, inputsCount);
            identifierFullyTrained.Add(identifier, false);
        }

        public static bool HasIdentifier(string identifier)
        {
            return identifierPopulations.ContainsKey(identifier);
        }

        public static string GetRandomIdentifier(string identifier)
        {
            return GetIdentifierOlderThan(identifier).Where(x => x != "f").OrderBy(x => randomizer.NextDouble()).Last();
        }

        public static IEnumerable<string> GetIdentifierOlderThan(string identifier)
        {
            return identifierPopulations.Keys.Take(identifierPopulations.Keys.ToList().IndexOf(identifier));
        }

        public static int GetInputCount(string identifier)
        {
            return identifierInputCounts[identifier];
        }

        public static BaseModule GetModule(string identifier)
        {            
            List<BaseModule> typePool = identifierPopulations[identifier];

            IEnumerable<BaseModule> best = typePool.Where(x => x.TimesUsed > 0).OrderBy(x => x.Fitness).ThenBy(x => x.TimesUsed);

            if (best.Any() && (SelectionMode == SelectionModes.Best || identifierFullyTrained[identifier]))
                return best.Last();
            else            
                return typePool.OrderBy(x => (x.Fitness + randomizer.NextDouble())).Last();                                    
        }
     
        public static void Train(string input, double expectedOutput)
        {
            for (int i = 0; i < Constants.NumberOfIterationsPerInput; i++)
            {
                RewardModules(input, expectedOutput);
                RunSelectionAndMutation();
                RewardModules(input, expectedOutput);
            }            
        }

        private static void RewardModules(string input, double expectedOutput)
        {
            SelectionMode = SelectionModes.Random;

            List<string> instructions = new List<string>();

            foreach (string s in input.Split(' '))
                instructions.Add(s);

            for (int i = 0; i < Constants.NumberOfSampleModules; i++)
            {
                Module module = new Module("", 0, instructions);

                double moduleOutput = module.Compute(new List<double>());

                double relativeError = Math.Abs((moduleOutput - expectedOutput) / expectedOutput);
                module.SaveError(relativeError);
            }            

            SelectionMode = SelectionModes.Best;
        }

        private static void RunSelectionAndMutation()
        {
            foreach (String identifier in identifierPopulations.Keys.ToList())
            {
                List<BaseModule> oldPopulation = identifierPopulations[identifier];

                if (oldPopulation.Count <= Constants.MinPopulationSize)
                    continue;

                List<BaseModule> newPopulation = new List<BaseModule>();                

                newPopulation.AddRange(oldPopulation.Where(x => x.TimesUsed == 0));
                oldPopulation.RemoveAll(x => x.TimesUsed == 0);

                int eliteSize = (int) Math.Max(1, oldPopulation.Count * Constants.EliteRatio);
                IEnumerable<BaseModule> elite = oldPopulation.OrderBy(x => x.AverageRelativeError).Take(eliteSize);
                elite = elite.Distinct();

                newPopulation.AddRange(elite);
                newPopulation.AddRange(elite.Select(x => {
                                                            if (randomizer.NextDouble() < Constants.ComplexifyProbability)
                                                                return x.GetComplexifiedModule();
                                                            else
                                                                return x.GetMutatedModule();
                                                        }));

                if (elite.Average(x => x.Fitness) < Constants.EliteFitnessThreshold)
                {
                    double averageComplexity = newPopulation.Average(x => x.Complexity);

                    for (int i = newPopulation.Count(); i < Constants.PopulationSize; i++)
                    {
                        BaseModule newModule = Module.GenerateSimpleModule(identifier, newPopulation.First().InputsCount);

                        if (newModule.Complexity < averageComplexity / 4)
                            newModule.Complexify();

                        newPopulation.Add(Module.GenerateSimpleModule(identifier, newPopulation.First().InputsCount));
                    }                                                              
                }
                else if (!identifierFullyTrained[identifier])
                {
                    identifierFullyTrained[identifier] = true;
                    Console.WriteLine(identifier + " is trained.");
                }
                  
                identifierPopulations[identifier] = newPopulation;
            }            
        }                
    }

    public enum SelectionModes
    {
        Best,
        Random
    }
}