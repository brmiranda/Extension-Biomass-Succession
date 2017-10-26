//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.SpatialModeling;
using Landis.Library.BiomassCohorts;
using Landis.Core;
using System.Collections.Generic;
using Edu.Wisc.Forest.Flel.Util;
using Landis.Library.Succession;
using Landis.Library.Biomass;

namespace Landis.Extension.Succession.Biomass
{
    public class SpeciesData
    {

        public static Landis.Library.Parameters.Species.AuxParm<double> WoodyDebrisDecay;
        public static Landis.Library.Parameters.Species.AuxParm<double> LeafLignin;
        public static Landis.Library.Parameters.Species.AuxParm<double> LeafLongevity;
        public static Landis.Library.Parameters.Species.AuxParm<double> MortCurveShapeParm;
        public static Landis.Library.Parameters.Species.AuxParm<double> GrowthCurveShapeParm;

        //  Establishment probability for each species in each ecoregion
        public static Landis.Library.Parameters.SpeciesEcoregionAuxParm<double> EstablishProbability;

        //  Mortality probability for each species in each ecoregion
        public static Landis.Library.Parameters.SpeciesEcoregionAuxParm<double> MortalityProbability;
        
        //  Establishment probability modifier for each species in each ecoregion
        public static Landis.Library.Parameters.SpeciesEcoregionAuxParm<double> EstablishModifier;

        //  Maximum ANPP for each species in each ecoregion
        public static Landis.Library.Parameters.SpeciesEcoregionAuxParm<int> ANPP_MAX_Spp;

        //  Maximum possible biomass for each species in each ecoregion
        public static Landis.Library.Parameters.SpeciesEcoregionAuxParm<int> B_MAX_Spp;

        //---------------------------------------------------------------------
        public static void Initialize(IInputParameters parameters)
        {
            LeafLignin              = parameters.LeafLignin;
            LeafLongevity           = parameters.LeafLongevity;
            MortCurveShapeParm      = parameters.MortCurveShapeParm;
            GrowthCurveShapeParm = parameters.GrowthCurveShapeParm;
            WoodyDebrisDecay = parameters.WoodyDecayRate;


        }

        public static void ChangeDynamicParameters(int year)
        {

            if(DynamicInputs.AllData.ContainsKey(year))
            {
                EstablishProbability = new Landis.Library.Parameters.SpeciesEcoregionAuxParm<double>(PlugIn.ModelCore.Species, PlugIn.ModelCore.Ecoregions);
                MortalityProbability = new Landis.Library.Parameters.SpeciesEcoregionAuxParm<double>(PlugIn.ModelCore.Species, PlugIn.ModelCore.Ecoregions);
                EstablishModifier = new Landis.Library.Parameters.SpeciesEcoregionAuxParm<double>(PlugIn.ModelCore.Species, PlugIn.ModelCore.Ecoregions);
                ANPP_MAX_Spp = new Landis.Library.Parameters.SpeciesEcoregionAuxParm<int>(PlugIn.ModelCore.Species, PlugIn.ModelCore.Ecoregions);
                B_MAX_Spp = new Landis.Library.Parameters.SpeciesEcoregionAuxParm<int>(PlugIn.ModelCore.Species, PlugIn.ModelCore.Ecoregions);


                DynamicInputs.TimestepData = DynamicInputs.AllData[year];

                foreach(ISpecies species in PlugIn.ModelCore.Species)
                {
                    foreach(IEcoregion ecoregion in PlugIn.ModelCore.Ecoregions)
                    {
                        if (!ecoregion.Active)
                            continue;

                        if (DynamicInputs.TimestepData[species.Index, ecoregion.Index] == null)
                            continue;
                        
                        EstablishProbability[species,ecoregion] = DynamicInputs.TimestepData[species.Index, ecoregion.Index].ProbEst;
                        MortalityProbability[species, ecoregion] = DynamicInputs.TimestepData[species.Index, ecoregion.Index].ProbMortality;
                        EstablishModifier[species, ecoregion] = 1.0;
                        ANPP_MAX_Spp[species,ecoregion] = DynamicInputs.TimestepData[species.Index, ecoregion.Index].ANPP_MAX_Spp;
                        B_MAX_Spp[species,ecoregion] = DynamicInputs.TimestepData[species.Index, ecoregion.Index].B_MAX_Spp;

                    }
                }

                //if(PlugIn.CalibrateMode)
                //    DynamicInputs.Write();


                EcoregionData.UpdateB_MAX();
            }

        }


    }
}
