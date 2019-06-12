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
using System.IO.Compression;
using System.Drawing;
using System.Collections;

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
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (isAdminUser())
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
        public ActionResult CUProductImage(HttpPostedFileBase ImageFile, string ProductId, string ProductPricingId, string Ordinal, string ProductPhotoMappingId)
        {
            string filename = ImageFile.FileName;
            string ImagePath= Server.MapPath("~/ProductImages/");
            ImagePath = ImagePath + filename;
            try
            {
                if (isAdminUser())
                {
                    PricingPhotoData pricingPhotoData = new PricingPhotoData();
                    ImageFile.SaveAs(ImagePath);
                    string Base64 = ProcessFile(ImagePath);
                    int OrdinalInt;
                    int.TryParse(Ordinal, out OrdinalInt);
                    pricingPhotoData.Ordinal = OrdinalInt;
                    pricingPhotoData.Photo = Base64;
                    pricingPhotoData.ProductId = ProductId;
                    pricingPhotoData.ProductPricingId = ProductPricingId;
                    if (String.IsNullOrEmpty(ProductPhotoMappingId))
                    {
                        pricingPhotoData.ProductPhotoMappingId = Guid.NewGuid().ToString();
                        PricingDetailsProxy.CUDPricingPhotoDetails(pricingPhotoData, "Insert");
                    }
                    else
                    {
                        pricingPhotoData.ProductPhotoMappingId = ProductPhotoMappingId;
                        PricingDetailsProxy.CUDPricingPhotoDetails(pricingPhotoData, "Update");
                    }
                    System.IO.File.Delete(ImagePath);
                    managePricing_IndexViewModel.SuccessMessage = UploadConstants.UploadSuccessMessage;
                    return RedirectToAction("Index", "ManagePricing", managePricing_IndexViewModel);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch(Exception e)
            {
                managePricing_IndexViewModel.ErrorMessage = CUDConstants.InsertError;
                System.IO.File.Delete(ImagePath);
                return RedirectToAction("Index", "ManagePricing", managePricing_IndexViewModel);
            }
            return View();
        }

        [HttpPost]
        public ActionResult UploadProductImages(HttpPostedFileBase ImageZip)
        {

            string filename = ImageZip.FileName;
            string zippath = Server.MapPath("~/Zip/");
            string ExtractPath = Server.MapPath("~/ZipExtract/");
            zippath = zippath + filename;
            try
            {
                if (isAdminUser())
                {
                    ImageZip.SaveAs(zippath);
                    System.IO.Compression.ZipFile.ExtractToDirectory(zippath, ExtractPath);
                    string ImportValues = ProcessDirectory(ExtractPath);
                    bool uploadResult=PricingDetailsProxy.InsertBulkPricingPhotoDetails(ImportValues);
                    managePricing_IndexViewModel.SuccessMessage = UploadConstants.UploadSuccessMessage;
                    System.IO.File.Delete(zippath);
                    return RedirectToAction("Index", "ManagePricing", managePricing_IndexViewModel);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch(Exception e)
            {
                managePricing_IndexViewModel.ErrorMessage = CUDConstants.InsertError;
                return RedirectToAction("Index", "ManagePricing", managePricing_IndexViewModel);
            }            
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
                        PricingDataTable = DataTablePhotoMapping(PricingDataTable);
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
                        PricingDataTable = DataTablePhotoMapping(PricingDataTable);
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
                            PricingDataTable = DataTablePhotoMapping(PricingDataTable);
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

        [HttpPost]
        public ActionResult DeletePhotoDetails(string ProductPhotoMappingId)
        {

            StringBuilder ErrorMessages = new StringBuilder();
            StringBuilder SuccessMessages = new StringBuilder();
            if (isAdminUser())
            {
                try
                {
                    PricingPhotoData pricingPhotoData = new PricingPhotoData();
                    pricingPhotoData.ProductPhotoMappingId = ProductPhotoMappingId;
                    bool DeleteResult = false;
                    DeleteResult = PricingDetailsProxy.CUDPricingPhotoDetails(pricingPhotoData, "Delete");
                    DataSet PricingDataSet = new DataSet();
                    DataTable PricingDataTable = new DataTable();
                    PricingDataSet = PricingDetailsProxy.FetchPricingDetails();
                    PricingDataTable = PricingDataSet.Tables[0];
                    PricingDataTable = DataTablePhotoMapping(PricingDataTable);
                    managePricing_IndexViewModel.PricingDataTable = PricingDataTable;
                    if (DeleteResult == false)
                    {
                        ErrorMessages.Append(CUDConstants.DeleteError);
                    }
                    else
                    {
                        SuccessMessages.Append(CUDConstants.DeleteSuccess);
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

            return View();
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

        private string ProcessDirectory(string targetDirectory)
        {
            StringBuilder imageImportValues = new StringBuilder();
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
            {
                string[] fileEntries = Directory.GetFiles(subdirectory);
                //string DirectoryName = Path.GetDirectoryName(subdirectory);
                string DirectoryName = new DirectoryInfo(subdirectory).Name;
                foreach (string filepath in fileEntries)
                {
                    string Photo=ProcessFile(filepath);
                    string Ordinal = Path.GetFileNameWithoutExtension(filepath);
                    string value = "('" + DirectoryName + "','" + Photo + "','"+Ordinal+"'),";
                    imageImportValues.Append(value);
                    System.IO.File.Delete(filepath);
                }
                Directory.Delete(subdirectory);
            }
            string ImportData = imageImportValues.ToString();
            ImportData = ImportData.Substring(0, ImportData.Length - 1);
            return ImportData;
        }

        private string ProcessFile(string filepath)
        {
            ///TODO: Convert image to Base64 format and insert to a photo mapping table against ProductPricingId. Here select the Product Pricing Id using Product Id which is the filename.
            string Photo = ImageToBase64(filepath,360,360);
            return Photo;
        }

        private static string ImageToBase64(string FilePath, int Height, int Width)
        {
            string base64String = string.Empty;
            using (System.Drawing.Image image = System.Drawing.Image.FromFile(FilePath))
            {
                using (MemoryStream m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    byte[] imageBytes = m.ToArray();
                    imageBytes = CreateThumbnail(imageBytes, Height, Width);
                    base64String = Convert.ToBase64String(imageBytes);
                    return base64String;
                }
            }
        }

        private static byte[] CreateThumbnail(byte[] PassedImage, int Height, int width)
        {
            byte[] ReturnedThumbnail;

            using (MemoryStream StartMemoryStream = new MemoryStream(),
                                NewMemoryStream = new MemoryStream())
            {
                // write the string to the stream  
                StartMemoryStream.Write(PassedImage, 0, PassedImage.Length);

                // create the start Bitmap from the MemoryStream that contains the image  
                Bitmap startBitmap = new Bitmap(StartMemoryStream);

                // set thumbnail height and width proportional to the original image.  
                int newHeight;
                int newWidth;
                double HW_ratio;

                if (startBitmap.Height > 360 || startBitmap.Width > 360)
                {
                    if (startBitmap.Height > startBitmap.Width)
                    {
                        newHeight = Height;
                        HW_ratio = (double)((double)Height / (double)startBitmap.Height);
                        newWidth = (int)(HW_ratio * (double)startBitmap.Width);
                    }
                    else
                    {
                        newWidth = width;
                        HW_ratio = (double)((double)width / (double)startBitmap.Width);
                        newHeight = (int)(HW_ratio * (double)startBitmap.Height);
                    }
                }
                else
                {
                    newHeight = startBitmap.Height;
                    newWidth = startBitmap.Width;
                }

                // create a new Bitmap with dimensions for the thumbnail.  
                Bitmap newBitmap = new Bitmap(newWidth, newHeight);

                // Copy the image from the START Bitmap into the NEW Bitmap.  
                // This will create a thumnail size of the same image.  
                newBitmap = ResizeImage(startBitmap, newWidth, newHeight);

                // Save this image to the specified stream in the specified format.  
                newBitmap.Save(NewMemoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);

                // Fill the byte[] for the thumbnail from the new MemoryStream.  
                ReturnedThumbnail = NewMemoryStream.ToArray();
            }

            // return the resized image as a string of bytes.  
            return ReturnedThumbnail;
        }
        private static Bitmap ResizeImage(Bitmap image, int width, int height)
        {
            Bitmap resizedImage = new Bitmap(width, height);
            using (Graphics gfx = Graphics.FromImage(resizedImage))
            {
                gfx.DrawImage(image, new Rectangle(0, 0, width, height),
                    new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
            }
            return resizedImage;
        }

        private DataTable DataTablePhotoMapping(DataTable PricingDataTable)
        {
            Dictionary<string, List<string>> PhotoMappingDic = new Dictionary<string, List<string>>();
            List<string> Photos = new List<string>();
            List<PhotoMapping> PhotoMappingIdDic = new List<PhotoMapping>();
            foreach (DataRow datarow in PricingDataTable.Rows)
            {
                PhotoMapping photoMapping = new PhotoMapping();
                if (!PhotoMappingDic.ContainsKey(datarow["ProductPricingId"].ToString()) && !String.IsNullOrEmpty(datarow["Photo"].ToString()))
                {
                    Photos = new List<string>();                    
                    Photos.Add(datarow["Photo"].ToString());
                    PhotoMappingDic.Add(datarow["ProductPricingId"].ToString(), Photos);

                    photoMapping.ProductPricingId = datarow["ProductPricingId"].ToString();
                    photoMapping.ProductPhotoMappingId = datarow["ProductPhotoMappingId"].ToString();
                    photoMapping.Photo = datarow["Photo"].ToString();

                    PhotoMappingIdDic.Add(photoMapping);
                }
                else if (!String.IsNullOrEmpty(datarow["Photo"].ToString()))
                {
                    PhotoMappingDic[datarow["ProductPricingId"].ToString()].Add(datarow["Photo"].ToString());

                    photoMapping.ProductPricingId = datarow["ProductPricingId"].ToString();
                    photoMapping.ProductPhotoMappingId = datarow["ProductPhotoMappingId"].ToString();
                    photoMapping.Photo = datarow["Photo"].ToString();

                    PhotoMappingIdDic.Add(photoMapping);
                }
            }
            managePricing_IndexViewModel.ProductPhotoMappingDic = PhotoMappingDic;
            managePricing_IndexViewModel.PhotoMappingIdDic = PhotoMappingIdDic;
            PricingDataTable.Columns.Remove("Photo");
            PricingDataTable.Columns.Remove("Ordinal");
            PricingDataTable = RemoveDuplicateRows(PricingDataTable, "ProductPricingId");
            return PricingDataTable;
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

    }       
}
