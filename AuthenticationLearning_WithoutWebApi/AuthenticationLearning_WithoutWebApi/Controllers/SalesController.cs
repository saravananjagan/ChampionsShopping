using AuthenticationLearning_WithoutWebApi.Constants;
using AuthenticationLearning_WithoutWebApi.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PMSModel.Order;
using PMSProxy.Order;
using PMSProxy.Pricing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace AuthenticationLearning_WithoutWebApi.Controllers
{
    public class SalesController : Controller
    {
        // GET: Sales
        private ApplicationDbContext context;
        private ManagePricing_IndexViewModel managePricing_IndexViewModel;
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (isValidUser())
                {
                    managePricing_IndexViewModel = new ManagePricing_IndexViewModel();
                    DataSet PricingDataSet = new DataSet();
                    DataTable PricingDataTable = new DataTable();
                    string UserId = User.Identity.GetUserId();
                    PricingDataSet = PricingDetailsProxy.FetchPricingDetails();
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

        [HttpPost]
        public ActionResult UpdateCartItem(string ProductPricingId, string CartItemQuantity)
        {
            if (isValidUser())
            {
                StringBuilder ErrorMessages = new StringBuilder();
                StringBuilder SuccessMessages = new StringBuilder();
                try
                {
                    bool UpdateResult = false;
                    CartData cartData = new CartData();
                    cartData.ProductPricingId = ProductPricingId;
                    cartData.Quantity = int.Parse(CartItemQuantity);
                    cartData.UserId = User.Identity.GetUserId();
                    UpdateResult = CartDetailsProxy.CUDCartValue(cartData, "Insert/Update");
                    if (UpdateResult)
                    {
                        SuccessMessages.Append(CUDConstants.UpdateCartSuccess);
                    }
                    else
                    {
                        ErrorMessages.Append(CUDConstants.UpdateCartError);
                    }
                    managePricing_IndexViewModel = new ManagePricing_IndexViewModel();
                    DataSet PricingDataSet = new DataSet();
                    DataTable PricingDataTable = new DataTable();
                    PricingDataSet = PricingDetailsProxy.FetchPricingDetails();
                    PricingDataTable = PricingDataSet.Tables[0];
                    if (PricingDataTable != null)
                    {
                        managePricing_IndexViewModel.PricingDataTable = PricingDataTable;
                    }
                    managePricing_IndexViewModel.UserId = User.Identity.GetUserId();
                }
                catch (Exception e)
                {
                    ErrorMessages.Append(CUDConstants.UpdateCartError);
                }
                if (!String.IsNullOrEmpty(SuccessMessages.ToString()))
                {
                    managePricing_IndexViewModel.SuccessMessage = SuccessMessages.ToString();
                }
                if (!String.IsNullOrEmpty(ErrorMessages.ToString()))
                {
                    managePricing_IndexViewModel.ErrorMessage = ErrorMessages.ToString();
                }

                return PartialView("ChampionsPricingGrid", managePricing_IndexViewModel);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private Boolean isValidUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity;
                ApplicationDbContext context = new ApplicationDbContext();
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                var s = UserManager.GetRoles(user.GetUserId());
                if (s[0].ToString() == "Champion")
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
    }
}