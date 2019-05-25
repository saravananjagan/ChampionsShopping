using AuthenticationLearning_WithoutWebApi.Constants;
using AuthenticationLearning_WithoutWebApi.Models;
using ICSharpCode.SharpZipLib.Zip;
using LinqToExcel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PMSModel.Pricing;
using PMSProxy.Pricing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace AuthenticationLearning_WithoutWebApi.Controllers
{
    public class ManagePricingController : Controller
    {
        // GET: ManagePricing
        private ApplicationDbContext context;
        private ManagePricing_IndexViewModel managePricing_IndexViewModel;
        public ManagePricingController()
        {
            context = new ApplicationDbContext();
            managePricing_IndexViewModel = new ManagePricing_IndexViewModel();
        }
        public ActionResult Index(ManagePricing_IndexViewModel managePricing_IndexViewModel)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (isAdminUser())
                {
                    DataSet PricingDataSet = new DataSet();
                    DataTable PricingDataTable = new DataTable();
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
        public ActionResult UploadPricingDetails(PricingList pricingList, HttpPostedFileBase FileUpload)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (isAdminUser())
                {
                    StringBuilder ErrorMessages = new StringBuilder();
                    StringBuilder SuccessMessages = null;
                    if (FileUpload != null)
                    {
                        if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                        {
                            string filename = FileUpload.FileName;
                            string targetpath = Server.MapPath("~/Doc/");
                            FileUpload.SaveAs(targetpath + filename);
                            string pathToExcelFile = targetpath + filename;
                            var connectionString = "";
                            if (filename.EndsWith(".xls"))
                            {
                                connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", pathToExcelFile);
                            }
                            else if (filename.EndsWith(".xlsx"))
                            {
                                connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", pathToExcelFile);
                            }

                            var adapter = new OleDbDataAdapter("SELECT * FROM [Sheet1$]", connectionString);
                            var ds = new DataSet();

                            adapter.Fill(ds, "ExcelTable");

                            DataTable dtable = ds.Tables["ExcelTable"];

                            string sheetName = "Sheet1";

                            var excelFile = new ExcelQueryFactory(pathToExcelFile);
                            var pricingLists = from a in excelFile.Worksheet<PricingList>(sheetName) select a;
                            StringBuilder ImportValues = new StringBuilder();

                            foreach (var a in pricingLists)
                            {
                                try
                                {
                                    if (a.ProductId != "")
                                    {
                                        PricingList PL = new PricingList();
                                        PL.ProductId = a.ProductId;
                                        PL.ProductName = a.ProductName;
                                        PL.BuyPrice = a.BuyPrice;
                                        PL.SellPrice = a.SellPrice;
                                        PL.Profit = a.Profit;
                                        PL.MRP = a.MRP;
                                        string value = "('" + Guid.NewGuid() + "','" + PL.ProductId + "','" + PL.ProductName + "','" + PL.BuyPrice + "','" + PL.SellPrice + "','" + PL.Profit + "','" + PL.MRP + "',1,getdate(),suser_sname(),null,null),";
                                        ImportValues.Append(value);
                                    }
                                }

                                catch (DbEntityValidationException ex)
                                {
                                    foreach (var entityValidationErrors in ex.EntityValidationErrors)
                                    {

                                        foreach (var validationError in entityValidationErrors.ValidationErrors)
                                        {

                                            ErrorMessages.Append("\nProperty: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);

                                        }

                                    }
                                }
                            }
                            //deleting excel file from folder  
                            if ((System.IO.File.Exists(pathToExcelFile)))
                            {
                                System.IO.File.Delete(pathToExcelFile);
                            }
                            string ImportData = ImportValues.ToString();
                            ImportData = ImportData.Substring(0, ImportData.Length - 1);
                            PricingDetailsProxy.InsertBulkPricingDetails(ImportData);
                            SuccessMessages = new StringBuilder();
                            SuccessMessages.Append(UploadConstants.UploadSuccessMessage);
                        }
                        else
                        {
                            ErrorMessages.Append(UploadConstants.InvalidFileFormat);
                        }
                    }
                    else
                    {
                        ErrorMessages.Append(UploadConstants.FileNotFound);   
                    }
                    if (!String.IsNullOrEmpty(SuccessMessages.ToString()))
                    {
                        TempData[UploadConstants.UploadSuccess] = SuccessMessages.ToString();
                    }
                    if (!String.IsNullOrEmpty(ErrorMessages.ToString()))
                    {
                        TempData[UploadConstants.UploadError] = ErrorMessages.ToString();
                    }                    
                    return RedirectToAction("Index","ManagePricing");
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
        public JsonResult UploadPricing(PricingList pricingList, HttpPostedFileBase FileUpload)
        {
            List<string> data = new List<string>();
            if (FileUpload != null)
            { 
                if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {

                    string filename = FileUpload.FileName;
                    string targetpath = Server.MapPath("~/Doc/");
                    FileUpload.SaveAs(targetpath + filename);
                    string pathToExcelFile = targetpath + filename;
                    var connectionString = "";
                    if (filename.EndsWith(".xls"))
                    {
                        connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", pathToExcelFile);
                    }
                    else if (filename.EndsWith(".xlsx"))
                    {
                        connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", pathToExcelFile);
                    }

                    var adapter = new OleDbDataAdapter("SELECT * FROM [Sheet1$]", connectionString);
                    var ds = new DataSet();

                    adapter.Fill(ds, "ExcelTable");

                    DataTable dtable = ds.Tables["ExcelTable"];

                    string sheetName = "Sheet1";

                    var excelFile = new ExcelQueryFactory(pathToExcelFile);
                    var pricingLists = from a in excelFile.Worksheet<PricingList>(sheetName) select a;
                    StringBuilder ImportValues = new StringBuilder();

                    foreach (var a in pricingLists)
                    {
                        try
                        {
                            if (a.ProductId!="")
                            {
                                PricingList PL = new PricingList();
                                PL.ProductId = a.ProductId;
                                PL.ProductName = a.ProductName;
                                PL.BuyPrice = a.BuyPrice;
                                PL.SellPrice = a.SellPrice;
                                PL.Profit = a.Profit;
                                PL.MRP = a.MRP;
                                string value = "('"+Guid.NewGuid()+"','"+PL.ProductId+"','"+PL.ProductName+"','"+PL.BuyPrice+"','"+PL.SellPrice+"','"+PL.Profit+"','"+PL.MRP+"',1,getdate(),suser_sname(),null,null),";
                                ImportValues.Append(value);
                            }
                        }

                        catch (DbEntityValidationException ex)
                        {
                            foreach (var entityValidationErrors in ex.EntityValidationErrors)
                            {

                                foreach (var validationError in entityValidationErrors.ValidationErrors)
                                {

                                    Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);

                                }

                            }
                        }
                    }
                    //deleting excel file from folder  
                    if ((System.IO.File.Exists(pathToExcelFile)))
                    {
                        System.IO.File.Delete(pathToExcelFile);
                    }
                    string ImportData = ImportValues.ToString();
                    ImportData=ImportData.Substring(0, ImportData.Length - 1);
                    PricingDetailsProxy.InsertBulkPricingDetails(ImportData);
                    return Json("success", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //alert message for invalid file format  
                    data.Add("<ul>");
                    data.Add("<li>Only Excel file format is allowed</li>");
                    data.Add("</ul>");
                    data.ToArray();
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                data.Add("<ul>");
                if (FileUpload == null) data.Add("<li>Please choose Excel file</li>");
                data.Add("</ul>");
                data.ToArray();
                return Json(data, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public JsonResult UploadImages(HttpPostedFileBase ImageZip)
        {
            List<String> imagesList = new List<string>();
            List<string> inValidphotosList = new List<string>();
            using (ZipInputStream s= new ZipInputStream(ImageZip.InputStream))
            {
                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    string fileName = Path.GetFileName(theEntry.Name);
                    if (fileName != String.Empty)
                    {
                        FileInfo info = new FileInfo(theEntry.Name);
                        if (info.Extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase) || info.Extension.Equals(".png", StringComparison.OrdinalIgnoreCase) || info.Extension.Equals(".gif", StringComparison.OrdinalIgnoreCase) || info.Extension.Equals(".bmp", StringComparison.OrdinalIgnoreCase))
                        {
                            imagesList.Add(info.Name);
                            if ((theEntry.Size / 1024f) / 1024f >= 2)
                            {
                                if (!inValidphotosList.Contains(info.Name))
                                {
                                    inValidphotosList.Add(info.Name);
                                    continue;
                                }
                                imagesList.Remove(info.Name);
                            }
                            string directoryName = "C:\\Users\\Public\\CHImages\\Photos";
                            Directory.CreateDirectory(directoryName);
                            using (FileStream streamWriter = System.IO.File.Create(directoryName + "\\" + info.Name))
                            {
                                int size = 2048;
                                byte[] data = new byte[2048];
                                while (true)
                                {
                                    size = s.Read(data, 0, data.Length);
                                    if (size > 0)
                                    {
                                        streamWriter.Write(data, 0, size);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

                return Json("success", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdatePricingDetails(PricingData pricingData)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (isAdminUser())
                {
                    StringBuilder ErrorMessages = new StringBuilder();
                    StringBuilder SuccessMessages = new StringBuilder();
                    try
                    {
                        bool UpdateResult = false;
                        UpdateResult=PricingDetailsProxy.CUDPricingDetails(pricingData,"Update");
                        DataSet PricingDataSet = new DataSet();
                        DataTable PricingDataTable = new DataTable();
                        PricingDataSet = PricingDetailsProxy.FetchPricingDetails();
                        PricingDataTable = PricingDataSet.Tables[0];
                        managePricing_IndexViewModel.PricingDataTable = PricingDataTable;
                        if (UpdateResult == false)
                        {
                            ErrorMessages.Append(CUDConstants.UpdateError);
                        }
                        else
                        {
                            SuccessMessages.Append(CUDConstants.UpdateSuccess);
                        }
                    }
                    catch(Exception e)
                    {
                        ErrorMessages.Append(CUDConstants.UpdateError);
                    }
                    if (!String.IsNullOrEmpty(SuccessMessages.ToString()))
                    {
                        managePricing_IndexViewModel.SuccessMessage = SuccessMessages.ToString();
                    }
                    if (!String.IsNullOrEmpty(ErrorMessages.ToString()))
                    {
                        managePricing_IndexViewModel.ErrorMessage = ErrorMessages.ToString();
                    }

                    return PartialView("PricingDataTable", managePricing_IndexViewModel);
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
        public ActionResult AddPricingDetails(PricingData pricingData)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (isAdminUser())
                {
                    StringBuilder ErrorMessages = new StringBuilder();
                    StringBuilder SuccessMessages = new StringBuilder();
                    try
                    {
                        bool InsertResult = false;
                        pricingData.ProductPricingId = Guid.NewGuid().ToString();
                        InsertResult = PricingDetailsProxy.CUDPricingDetails(pricingData, "Insert");
                        DataSet PricingDataSet = new DataSet();
                        DataTable PricingDataTable = new DataTable();
                        PricingDataSet = PricingDetailsProxy.FetchPricingDetails();
                        PricingDataTable = PricingDataSet.Tables[0];
                        managePricing_IndexViewModel.PricingDataTable = PricingDataTable;
                        if (InsertResult == false)
                        {
                            ErrorMessages.Append(CUDConstants.InsertError);
                        }
                        else
                        {
                            SuccessMessages.Append(CUDConstants.InsertSuccess);
                        }
                    }
                    catch (Exception e)
                    {
                        ErrorMessages.Append(CUDConstants.InsertError);
                    }
                    if (!String.IsNullOrEmpty(SuccessMessages.ToString()))
                    {
                        managePricing_IndexViewModel.SuccessMessage = SuccessMessages.ToString();
                    }
                    if (!String.IsNullOrEmpty(ErrorMessages.ToString()))
                    {
                        managePricing_IndexViewModel.ErrorMessage = ErrorMessages.ToString();
                    }
                    return PartialView("PricingDataTable", managePricing_IndexViewModel);
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
        public ActionResult DeletePricingDetails(string ProductPricingId)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (isAdminUser())
                {
                    StringBuilder ErrorMessages = new StringBuilder();
                    StringBuilder SuccessMessages = new StringBuilder();
                    try
                    {
                        PricingData pricingData = new PricingData();
                        if (!string.IsNullOrEmpty(ProductPricingId))
                        {
                            pricingData.ProductPricingId = ProductPricingId;
                            bool InsertResult = false;
                            InsertResult = PricingDetailsProxy.CUDPricingDetails(pricingData, "Delete");
                            DataSet PricingDataSet = new DataSet();
                            DataTable PricingDataTable = new DataTable();
                            PricingDataSet = PricingDetailsProxy.FetchPricingDetails();
                            PricingDataTable = PricingDataSet.Tables[0];
                            managePricing_IndexViewModel.PricingDataTable = PricingDataTable;
                            if (InsertResult == false)
                            {
                                ErrorMessages.Append(CUDConstants.DeleteError);
                            }
                            else
                            {
                                SuccessMessages.Append(CUDConstants.DeleteSuccess);
                            }
                        }
                        else
                        {
                            ErrorMessages.Append(CUDConstants.DeleteError);
                        }

                    }
                    catch (Exception e)
                    {
                        ErrorMessages.Append(CUDConstants.DeleteError);
                    }
                    if (!String.IsNullOrEmpty(SuccessMessages.ToString()))
                    {
                        managePricing_IndexViewModel.SuccessMessage = SuccessMessages.ToString();
                    }
                    if (!String.IsNullOrEmpty(ErrorMessages.ToString()))
                    {
                        managePricing_IndexViewModel.ErrorMessage = ErrorMessages.ToString();
                    }
                    
                    return PartialView("PricingDataTable", managePricing_IndexViewModel);
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

        private Boolean isAdminUser()
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
    }       
}
