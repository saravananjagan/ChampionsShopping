using AuthenticationLearning_WithoutWebApi.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PMSProxy.Pricing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AuthenticationLearning_WithoutWebApi.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        private ApplicationDbContext context;
        private ManagePricing_IndexViewModel managePricing_IndexViewModel;
        public ActionResult Index()
        {
            managePricing_IndexViewModel = new ManagePricing_IndexViewModel();
            if (User.Identity.IsAuthenticated)
            {
                if (isValidUser())
                {
                    managePricing_IndexViewModel.IsChampion = true;
                    DataSet PricingDataSet = new DataSet();
                    DataTable PricingDataTable = new DataTable();
                    string UserId = User.Identity.GetUserId();
                    PricingDataSet = PricingDetailsProxy.FetchUserPricingDetails(UserId);
                    PricingDataTable = PricingDataSet.Tables[0];
                    if (PricingDataTable != null)
                    {
                        managePricing_IndexViewModel.PricingDataTable = PricingDataTable;
                    }
                    return View(managePricing_IndexViewModel);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }



        private bool isValidUser()
        {
            managePricing_IndexViewModel = new ManagePricing_IndexViewModel();
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity;
                ApplicationDbContext context = new ApplicationDbContext();
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                var s = UserManager.GetRoles(user.GetUserId());
                if (s[0].ToString() == "Champion")
                {
                    managePricing_IndexViewModel.IsChampion = true;
                    return true;
                }
                else
                {
                    managePricing_IndexViewModel.IsChampion = false;
                    return false;
                }
            }
            return false;
        }
    }
}