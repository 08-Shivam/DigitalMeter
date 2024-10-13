using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalMeter
{
    public static class GlobalState
    {
        public static int MeterID { get; set; }
        public static int CurrentBalance { get; set; }
        
        // Send1 button action
        public static string AverageCurrent { get; set; }
        public static float AverageLLVoltage { get; set; }
        public static float AverageLNVoltage {  get; set; }
        public static float Frequency {  get; set; }
        public static float PowerFactor {  get; set; }
        public static int EBDBSW { get; set; }
        public static int AppEnergyTotal { get; set; }
        public static int ActEnergyTotal { get; set; }
        public static int AppEnergyEB {  get; set; }
        public static int ActEnergyEB {  get; set; }

        // send2 button action
        public static string Amount {  get; set; }
        public static string Number {  get; set; }
        public static string Serial {  get; set; }
        public static string SerialNumber {  get; set; }
        public static int RechargeValue {  get; set; }
        public static int RechargeDate { get; set; }
        public static string RechargeCode { get; set; }
        public static int CoverOpenlight {  get; set; }
        public static int CoverOpenTime1 {  get; set; }
        public static int CoverOpenTime2 { get; set; }
        public static int CoverOpenDate1 { get; set; }
        public static int CoverOpenDate2 { get; set; }

        //send3 button action
        public static int AppEnergyDG { get; set; }
        public static int ActEnergyDG { get; set; }
        public static int AppEnergySolar { get; set; }
        public static int ActEnergySolar { get; set; }
        public static int AppEnergyWind { get; set; }
        public static int ActEnergyWind { get; set; }

        //Send6 button action
        public static int OperationMode { get; set; }
        public static int PerUnitS1 { get; set; }
        public static int PerUnitS2 { get; set; }
        public static int PerUnitS3 { get; set; }
        public static int PerUnitS4 { get; set; }
        public static int AutoCut { get; set; }
        public static int GracePeriod { get; set; }
        public static int MaxDemandCut { get; set; }
        public static int SLS1 { get; set; }
        public static int SLS2 { get; set; }
        public static int SLS3 { get; set; }
        public static int SLS4 { get; set; }
        public static int RateS1 { get; set; }
        public static int RateS2 { get; set; }
        public static int RateS3 { get; set; }
        public static int RateS4 { get; set; }
        public static int RateFPD { get; set; }
        public static int Load { get; set; }
        public static int PhaseS1 { get; set; }
        public static int PhaseS2 { get; set; }
        public static int PhaseS3 { get; set; }
        public static int PhaseS4 { get; set; }
        //public static FormData formData = new FormData();
    }

    public class FormData
    {
       
        //public int PerUnitS1 { get; set; }
        //public int PerUnitS2 { get; set; }
        //public int PerUnitS3 { get; set; }
        //public int PerUnitS4 { get; set; }
        //public int AutoCut { get; set; }
        //public int GracePeriod { get; set; }
        //public int MaxDemandCut { get; set; }
        //public int SLS1 { get; set; }
        //public int SLS2 { get; set; }
        //public int SLS3 { get; set; }
        //public int SLS4 { get; set; }
        //public int RateS1 { get; set; }
        //public int RateS2 { get; set; }
        //public int RateS3 { get; set; }
        //public int RateS4 { get; set; }
        //public int RateFPD { get; set; }
        //public int Load { get; set; }
        //public int PhaseS1 { get; set; }
        //public int PhaseS2 { get; set; }
    }
}
