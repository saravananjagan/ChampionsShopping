using AuthenticationLearning_WithoutWebApi.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PMSProxy.Pricing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AuthenticationLearning_WithoutWebApi.Controllers
{
    public class HomeController : Controller
    {
        private ManagePricing_IndexViewModel managePricing_IndexViewModel;
        public ActionResult Index()
        {
			string controllerName = GetControllerNameByItsRole();
			if (User.Identity.IsAuthenticated)
            {
                if (!isAdminUser())
                {
                    return RedirectToAction("Index", controllerName);
                }
                else
                {
                    return RedirectToAction("Index", "ManagePricing");
                }
            }
            else
            {
                managePricing_IndexViewModel = new ManagePricing_IndexViewModel();
                DataSet PricingDataSet = new DataSet();
                DataTable PricingDataTable = new DataTable();
                PricingDataSet = PricingDetailsProxy.FetchPricingDetails();
                PricingDataTable = PricingDataSet.Tables[0];
                PricingDataTable = DataTablePhotoMapping(PricingDataTable);
                if (PricingDataTable != null)
                {
                    managePricing_IndexViewModel.PricingDataTable = PricingDataTable;
                }
                return View(managePricing_IndexViewModel);
            }   
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

		private string GetControllerNameByItsRole()
		{
			if (User.Identity.IsAuthenticated)
			{
				var user = User.Identity;
				ApplicationDbContext context = new ApplicationDbContext();
				var UserManager = new UserManager<ApplicationUser>(new Microsoft.AspNet.Identity.EntityFramework.UserStore<ApplicationUser>(context));
				var s = UserManager.GetRoles(user.GetUserId());
				if (s[0].ToString() == "Champion")
				{
					return "Sales";
				}
				else if (s[0].ToString() == "Customer")
				{
					return "Customer";
				}
				else
					return "Home";
			}
			return "Home";
		}

		private bool isAdminUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity;
                ApplicationDbContext context = new ApplicationDbContext();
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                var s = UserManager.GetRoles(user.GetUserId());
                if (s[0].ToString() == "Admin")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        private DataTable RemoveDuplicateRows(DataTable dTable, string colName)
        {
            Hashtable hTable = new Hashtable();
            ArrayList duplicateList = new ArrayList();

            //Add list of all the unique item value to hashtable, which stores combination of key, value pair.
            //And add duplicate item value in arraylist.
            foreach (DataRow drow in dTable.Rows)
            {
                if (hTable.Contains(drow[colName]))
                    duplicateList.Add(drow);
                else
                    hTable.Add(drow[colName], string.Empty);
            }

            //Removing a list of duplicate items from datatable.
            foreach (DataRow dRow in duplicateList)
                dTable.Rows.Remove(dRow);

            //Datatable which contains unique records will be return as output.
            return dTable;
        }

        private DataTable DataTablePhotoMapping(DataTable PricingDataTable)
        {
            Dictionary<string, List<string>> PhotoMappingDic = new Dictionary<string, List<string>>();
            List<string> Photos = new List<string>();
            foreach (DataRow datarow in PricingDataTable.Rows)
            {
                if (!PhotoMappingDic.ContainsKey(datarow["ProductPricingId"].ToString()) && !String.IsNullOrEmpty(datarow["Photo"].ToString()))
                {
                    Photos = new List<string>();
                    Photos.Add(datarow["Photo"].ToString());
                    PhotoMappingDic.Add(datarow["ProductPricingId"].ToString(), Photos);
                }
                else if (!String.IsNullOrEmpty(datarow["Photo"].ToString()))
                {
                    PhotoMappingDic[datarow["ProductPricingId"].ToString()].Add(datarow["Photo"].ToString());
                }
            }
            managePricing_IndexViewModel.ProductPhotoMappingDic = PhotoMappingDic;
            PricingDataTable.Columns.Remove("Photo");
            PricingDataTable.Columns.Remove("Ordinal");
            PricingDataTable = RemoveDuplicateRows(PricingDataTable, "ProductPricingId");
            return PricingDataTable;
        }
    }
}