using DocumentManagementSystem.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentManagementSystem.Controllers
{
    public class PdfController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            
            return View();
        }
        [HttpPost]
        public IActionResult Index(PdfViewModel model)
        {
            if (ModelState.IsValid)
            {
                TempData["data"] = model.Message;
                TempData["orientation"] = model.Orientation;

                

                if (model.Orientation == "Landscape")
                {
                    var viewLandscape = new ViewAsPdf("download")
                    {
                        FileName = $"MyCreatedDocument_{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.pdf",
                        PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape,
                        PageSize = Rotativa.AspNetCore.Options.Size.A4,
                        CustomSwitches = "--page-offset 0 --footer-center [page] --footer-font-size 12"
                    };
                    return viewLandscape;
                }
                else
                {
                    var viewPortrait = new ViewAsPdf("download")
                    {
                        FileName = $"MyCreatedDocument_{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.pdf",
                        PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                        PageSize = Rotativa.AspNetCore.Options.Size.A4,
                        CustomSwitches = "--page-offset 0 --footer-center [page] --footer-font-size 12"
                    };
                    return viewPortrait;
                }
            }
            ModelState.AddModelError(string.Empty, "Message field is required");
            return View();
        }
       
        [HttpPost]
        public IActionResult Print(PdfViewModel model)
        {
            TempData["data"] = model.Message;
            TempData["orientation"] = model.Orientation;
            //var printPdf = new ViewAsPdf("print")
            //{

            //    PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
            //    PageSize = Rotativa.AspNetCore.Options.Size.A4,
            //    CustomSwitches = "--page-offset 0 --footer-center [page] --footer-font-size 12"
            //};
            //return printPdf;
            string Orientation = TempData["orientation"].ToString();
            if (Orientation == "Landscape")
            {
                var viewLandscape = new ViewAsPdf("Print")
                {
                    PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape,
                    PageSize = Rotativa.AspNetCore.Options.Size.A4,
                    CustomSwitches = "--page-offset 0 --footer-center [page] --footer-font-size 12"
                };
                return viewLandscape;
            }
            else
            {
                var viewPortrait = new ViewAsPdf("Print")
                {
                    PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                    PageSize = Rotativa.AspNetCore.Options.Size.A4,
                    CustomSwitches = "--page-offset 0 --footer-center [page] --footer-font-size 12"
                };
                return viewPortrait;
            }
        }
        [HttpGet]
        public IActionResult Download()
        {
            return new ViewAsPdf("download");
        }
    }
}
