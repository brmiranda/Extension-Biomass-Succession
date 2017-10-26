//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.Core;
using System.Collections.Generic;
using Edu.Wisc.Forest.Flel.Util;
using System.Text;

namespace Landis.Extension.Succession.Biomass
{
    /// <summary>
    /// A parser that reads the tool parameters from text input.
    /// </summary>
    public class DynamicInputsParser
        : TextParser<Dictionary<int, IDynamicInputRecord[,]>>
    {

        public override string LandisDataValue
        {
            get
            {
                return "Dynamic Input Data";
            }
        }


        //---------------------------------------------------------------------
        public DynamicInputsParser()
        {
        }

        //---------------------------------------------------------------------

        protected override Dictionary<int, IDynamicInputRecord[,]> Parse()
        {

            ReadLandisDataVar();
            
            Dictionary<int, IDynamicInputRecord[,]> allData = new Dictionary<int, IDynamicInputRecord[,]>();

            //---------------------------------------------------------------------
            //Read in climate data:
            InputVar<int>    year       = new InputVar<int>("Time step for updating values");
            InputVar<string> ecoregionName = new InputVar<string>("Ecoregion Name");
            InputVar<string> speciesName = new InputVar<string>("Species Name");
            InputVar<double> pest = new InputVar<double>("Probability of Establishment");
            InputVar<int> anpp = new InputVar<int>("ANPP");
            InputVar<int> bmax = new InputVar<int>("Maximum Biomass");
            InputVar<double> pmort = new InputVar<double>("Probability of Mortality");

            while (! AtEndOfInput)
            {
                StringReader currentLine = new StringReader(CurrentLine);

                ReadValue(year, currentLine);
                int yr = year.Value.Actual;

                if(!allData.ContainsKey(yr))
                {
                    IDynamicInputRecord[,] inputTable = new IDynamicInputRecord[PlugIn.ModelCore.Species.Count, PlugIn.ModelCore.Ecoregions.Count];
                    allData.Add(yr, inputTable);
                    PlugIn.ModelCore.UI.WriteLine("  Dynamic Input Parser:  Add new year = {0}.", yr);
                }

                ReadValue(ecoregionName, currentLine);

                IEcoregion ecoregion = GetEcoregion(ecoregionName.Value);

                ReadValue(speciesName, currentLine);

                ISpecies species = GetSpecies(speciesName.Value);

                IDynamicInputRecord dynamicInputRecord = new DynamicInputRecord();

                ReadValue(pest, currentLine);
                dynamicInputRecord.ProbEst = pest.Value;

                ReadValue(anpp, currentLine);
                dynamicInputRecord.ANPP_MAX_Spp = anpp.Value;

                ReadValue(bmax, currentLine);
                dynamicInputRecord.B_MAX_Spp = bmax.Value;

                ReadValue(pmort, currentLine);
                dynamicInputRecord.ProbMortality = pmort.Value;

                allData[yr][species.Index, ecoregion.Index] = dynamicInputRecord;

                CheckNoDataAfter("the " + pmort.Name + " column",
                                 currentLine);

                GetNextLine();

            }

            return allData;
        }

        //---------------------------------------------------------------------

        private IEcoregion GetEcoregion(InputValue<string>      ecoregionName)
        {
            IEcoregion ecoregion = PlugIn.ModelCore.Ecoregions[ecoregionName.Actual];
            if (ecoregion == null)
                throw new InputValueException(ecoregionName.String,
                                              "{0} is not an ecoregion name.",
                                              ecoregionName.String);

            return ecoregion;
        }

        //---------------------------------------------------------------------

        private ISpecies GetSpecies(InputValue<string> speciesName)
        {
            ISpecies species = PlugIn.ModelCore.Species[speciesName.Actual];
            if (species == null)
                throw new InputValueException(speciesName.String,
                                              "{0} is not a recognized species name.",
                                              speciesName.String);

            return species;
        }


    }
}
