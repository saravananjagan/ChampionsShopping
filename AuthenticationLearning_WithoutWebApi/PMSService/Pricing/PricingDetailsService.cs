using IPMSService.Pricing;
using PMSDAL.Pricing;
using PMSIDAL.Pricing;
using PMSModel.Pricing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMSService.Pricing
{
    public class PricingDetailsService: IPricingDetailsService
    {
        public bool InsertBulkPricingDetails(string ImportValues)
        {
            try
            {
                IPricingDetailsDAL pricingDetailsDAL = new PricingDetailsDAL();
                pricingDetailsDAL.InsertBulkPricingDetails(ImportValues);
            }
            catch (Exception e)
            {

            }
            return true;
        }

        public DataSet FetchPricingDetails()
        {
            DataSet dataSet = new DataSet();
            try
            {
                IPricingDetailsDAL pricingDetailsDAL = new PricingDetailsDAL();
                dataSet = pricingDetailsDAL.FetchPricingDetails(); 
            }
            catch (Exception e)
            {

            }
            return dataSet;
        }
        public bool CUDPricingDetails(PricingData pricingData,string QuerySelector)
        {
            try
            {
                IPricingDetailsDAL pricingDetailsDAL = new PricingDetailsDAL();
                pricingDetailsDAL.CUDPricingDetails(pricingData,QuerySelector);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }            
        }
        public DataSet FetchUserPricingDetails(string UserId)
        {
            DataSet dataSet = new DataSet();
            try
            {
                IPricingDetailsDAL pricingDetailsDAL = new PricingDetailsDAL();
                dataSet = pricingDetailsDAL.FetchUserPricingDetails(UserId);
            }
            catch (Exception e)
            {

            }
            return dataSet;
        }

        public bool InsertBulkPricingPhotoDetails(string ImportValues)
        {
            try
            {
                IPricingDetailsDAL pricingDetailsDAL = new PricingDetailsDAL();
                pricingDetailsDAL.InsertBulkPricingPhotoDetails(ImportValues);
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

		public PricingData GetPricingDetailsByProductId(string productId)
		{
			PricingData pricingData = new PricingData();
			try
			{
				IPricingDetailsDAL pricingDetailsDAL = new PricingDetailsDAL();
				pricingDetailsDAL.GetPricingDetailsByProductId(productId);
			}
			catch (Exception e)
			{
				//Need to logg errors
			}
			return pricingData;
		}

	}
}
