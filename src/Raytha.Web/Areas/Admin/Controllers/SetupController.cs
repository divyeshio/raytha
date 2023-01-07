using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Raytha.Application.Common.Interfaces;
using Raytha.Application.OrganizationSettings.Commands;
using Raytha.Domain.Entities;
using Raytha.Web.Areas.Admin.Views.Setup;

namespace Raytha.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class SetupController : BaseController
{
    [Route(RAYTHA_ROUTE_PREFIX + "/setup", Name = "setupindex")]
    public IActionResult Index()
    {
        if (CurrentOrganization.InitialSetupComplete)
            return RedirectToAction("Index", "Dashboard");

        var viewModel = new Setup_ViewModel
        {
            MissingSmtpEnvironmentVariables = Emailer.IsMissingSmtpEnvVars()
        };
        return View(viewModel);
    }

    [Route(RAYTHA_ROUTE_PREFIX + "/setup", Name = "setupindex")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(Setup_ViewModel model)
    {
        if (CurrentOrganization.InitialSetupComplete)
            return RedirectToAction("Index", "Dashboard");

        var input = new InitialSetup.Command
        {
            SmtpHost = model.SmtpHost,
            SmtpPort = model.SmtpPort,
            SmtpUsername = model.SmtpUsername,
            SmtpPassword = model.SmtpPassword,
            FirstName = model.FirstName,
            LastName = model.LastName,
            SuperAdminEmailAddress = model.SuperAdminEmailAddress,
            SuperAdminPassword = model.SuperAdminPassword,
            SmtpDefaultFromAddress = model.SmtpDefaultFromAddress,
            SmtpDefaultFromName = model.SmtpDefaultFromName,
            OrganizationName = model.OrganizationName,
            TimeZone = model.TimeZone,
            WebsiteUrl = model.WebsiteUrl
        };

        var response = await Mediator.Send(input);
        if (response.Success)
        {
            return RedirectToAction("Index", "Dashboard");
        }
        else
        {
            ViewData["ErrorMessage"] = "There was an error submitting the initial setup. See the error below.";
            this.HttpContext.Response.StatusCode = 303;
            return View(model);
        }
    }
}
