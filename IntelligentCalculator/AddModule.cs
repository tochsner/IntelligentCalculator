using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentCalculator
{
    public class AddModule : BaseModule
    {
        public AddModule()
        {
            Identifier = "+";
            InputsCount = 2;
            Complexity = 2;
        }

        public override double Fitness => 1;

        public override void Complexify()
        {
        }

        public override double Compute(List<double> inputs)
        {
            return inputs[0] + inputs[1];
        }

        public override BaseModule GetMutatedModule()
        {
            return new AddModule();
        }
        public override BaseModule GetComplexifiedModule()
        {
            return new AddModule();
        }

        public override void SaveError(double relativeError)
        {
            
        }
    }
    public class DivisionModule : BaseModule
    {
        public DivisionModule()
        {
            Identifier = "/";
            InputsCount = 2;
            Complexity = 2;
        }

        public override double Fitness => 1;

        public override void Complexify()
        {
        }

        public override double Compute(List<double> inputs)
        {
            return inputs[1] == 0 ? 0 : inputs[0] / inputs[1];
        }

        public override BaseModule GetMutatedModule()
        {
            return new DivisionModule();
        }
        public override BaseModule GetComplexifiedModule()
        {
            return new DivisionModule();
        }

        public override void SaveError(double relativeError)
        {

        }
    }
    public class FactorModule : BaseModule
    {        
        public FactorModule()
        {
            Identifier = "f";
            InputsCount = 2;
            Complexity = 1;
        }

        public override double Fitness => 1;

        public override void Complexify()
        {
        }

        public override double Compute(List<double> inputs)
        {
            return inputs[0] * inputs[1];
        }

        public override BaseModule GetMutatedModule()
        {
            return new FactorModule();
        }
        public override BaseModule GetComplexifiedModule()
        {
            return new FactorModule();
        }

        public override void SaveError(double relativeError)
        {

        }
    }
}
