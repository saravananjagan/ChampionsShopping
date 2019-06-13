using AuthenticationLearning_WithoutWebApi.Constants;
using AuthenticationLearning_WithoutWebApi.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PMSModel.Order;
using PMSProxy.Order;
using PMSProxy.Pricing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace AuthenticationLearning_WithoutWebApi.Controllers
{
    public class CustomerController : Controller
    {
        // GET: Customer
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
                    DataSet CumulativeCartDataSet = new DataSet();
                    DataTable CumulativeCartDataTable = new DataTable();
                    string UserId = User.Identity.GetUserId();
                    PricingDataSet = PricingDetailsProxy.FetchPricingDetails(UserId);
                    PricingDataTable = PricingDataSet.Tables[0];
                    CumulativeCartDataSet = PricingDetailsProxy.FetchCumulativeCartDetails(UserId);
                    CumulativeCartDataTable = CumulativeCartDataSet.Tables[0];
                    foreach (DataRow row in CumulativeCartDataTable.Rows)
                    {
                        managePricing_IndexViewModel.TotalItems = row["TotalItems"].ToString();
                        managePricing_IndexViewModel.TotalBuyValue = row["TotalSellValue"].ToString();
                    }
                    PricingDataTable = DataTablePhotoMapping(PricingDataTable);
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
                    string UserId = User.Identity.GetUserId();
                    PricingDataSet = PricingDetailsProxy.FetchPricingDetails(UserId);
                    PricingDataTable = PricingDataSet.Tables[0];

                    #region CumulativeCartData
                    DataSet CumulativeCartDataSet = new DataSet();
                    DataTable CumulativeCartDataTable = new DataTable();
                    CumulativeCartDataSet = PricingDetailsProxy.FetchCumulativeCartDetails(UserId);
                    CumulativeCartDataTable = CumulativeCartDataSet.Tables[0];
                    foreach (DataRow row in CumulativeCartDataTable.Rows)
                    {
                        managePricing_IndexViewModel.TotalItems = row["TotalItems"].ToString();
                        managePricing_IndexViewModel.TotalBuyValue = row["TotalSellValue"].ToString();
                    }
                    #endregion

                    PricingDataTable = DataTablePhotoMapping(PricingDataTable);
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

                return PartialView("CustomerPricingGrid", managePricing_IndexViewModel);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult DeleteCartItem(string ProductPricingId)
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
                    cartData.UserId = User.Identity.GetUserId();
                    UpdateResult = CartDetailsProxy.CUDCartValue(cartData, "Delete");
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
                    string UserId = User.Identity.GetUserId();
                    PricingDataSet = PricingDetailsProxy.FetchPricingDetails(UserId);
                    PricingDataTable = PricingDataSet.Tables[0];

                    #region CumulativeCartData
                    DataSet CumulativeCartDataSet = new DataSet();
                    DataTable CumulativeCartDataTable = new DataTable();
                    CumulativeCartDataSet = PricingDetailsProxy.FetchCumulativeCartDetails(UserId);
                    CumulativeCartDataTable = CumulativeCartDataSet.Tables[0];
                    foreach (DataRow row in CumulativeCartDataTable.Rows)
                    {
                        managePricing_IndexViewModel.TotalItems = row["TotalItems"].ToString();
                        managePricing_IndexViewModel.TotalBuyValue = row["TotalSellValue"].ToString();
                    }
                    #endregion

                    PricingDataTable = DataTablePhotoMapping(PricingDataTable);
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
                return PartialView("CustomerPricingGrid", managePricing_IndexViewModel);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public string GetCartCount()
        {
            if (isValidUser())
            {
                DataSet cartDetails = CartDetailsProxy.FetchCartDetails(User.Identity.GetUserId(), null, "CartDetail");
                string count = cartDetails.Tables[0].Rows.Count.ToString();
                return count;
            }
            else
            {
                return string.Empty;
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
                if (s[0].ToString() == "Customer")
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