﻿using NHS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using NHS.Common;
using NHS.Data;
using System.Web.UI;
using System.Configuration;
using System.Security.Principal;
using System.IO;
using System.DirectoryServices;
using System.Web.SessionState;
using NHS.Common.DTO;
using System.Net.Mail;
using NHS.Helper;


namespace NHS.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class HomeController : Controller
    {
        //NHSEntities ent = new NHSEntities();
        public ActionResult Index()
        {
            Session["FullName"] = "";
            Session["TotalDeaths"] = "";
            if (Convert.ToInt32(Session["LoginUserID"]) > 0)
            {
                bool isuser = GetUserDetailsFromAD();
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }

        public ActionResult NotImplemented()
        {
            return View();
        }

        
        public void Export()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            try
            {                
                if (Session["StartDate"] == null)
                {
                    Session["StartDate"] = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
                }
                else if (Convert.ToString(Session["StartDate"]) == "")
                {
                    Session["StartDate"] = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
                }
                if (Session["EndDate"] == null)
                {
                    Session["EndDate"] = DateTime.Now.ToString("dd/MM/yyyy");
                }
                else if (Convert.ToString(Session["EndDate"]) == "")
                {
                    Session["EndDate"] = DateTime.Now.ToString("dd/MM/yyyy");
                }
                if (Session["WardDeath"] == null)
                {
                    Session["WardDeath"] = "0";
                }
                else if (Convert.ToString(Session["WardDeath"]) == "" && Session["PatientDetails"] == null)
                {
                    Session["WardDeath"] = "0";
                }
                if (Session["PatientType"] == null)
                {
                    Session["PatientType"] = "0";
                }
                else if (Convert.ToString(Session["PatientType"]) == "" && Session["PatientDetails"] == null)
                {
                    Session["PatientType"] = "0";
                }
                if (Session["DischargeConsultant"] == null)
                {
                    Session["DischargeConsultant"] = "0";
                }
                else if (Convert.ToString(Session["DischargeConsultant"]) == "" && Session["PatientDetails"] == null)
                {
                    Session["DischargeConsultant"] = "0";
                }
                if (Session["Speciality"] == null)
                {
                    Session["Speciality"] = "0";
                }
                else if (Convert.ToString(Session["Speciality"]) == "" && Session["PatientDetails"] == null)
                {
                    Session["Speciality"] = "0";
                }
                List<CORSExtract> patients = dBEngine.GetCORSExtract(DateTime.ParseExact(Convert.ToString(Session["StartDate"]), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), DateTime.ParseExact(Convert.ToString(Session["EndDate"]), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), Convert.ToString(Session["DischargeConsultant"]), Convert.ToString(Session["WardDeath"]), Convert.ToString(Session["Speciality"]), Convert.ToString(Session["PatientType"]), Convert.ToInt32(Session["LoginUserID"]));

                ExportAsCSV(patients);
            }
            catch (Exception exception)
            {
                dBEngine.LogException(exception.Message, this.ToString(), "Export", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
            }
        }

        private void ExportAsCSV(List<CORSExtract> extract)
        {
            var sw = new StringWriter();
            //write the header
            sw.WriteLine(String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}, {11}, {12}, {13}, {14}, {15}, {16},{17}, {18}, {19}, {20}, {21}, {22}, {23}, {24}, {25}, {26}, {27}, {28}, {29}", "ID",
            "PatientID",
            "SpellNumber",
            "PatientName",
            "NHSNumber",
            "Gender",
            "Age",
            "DOB",
            "DateofAdmission",
            "TimeofAdmission",
            "DischargeWard",
            "DateofDeath",
            "TimeofDeath",
            "WardofDeath",
            "DischargeConsultantCode",
            "DischargeConsultantName",
            "DischargeSpecialityCode",
            "DischargeSpeciality",
            "CareGroup",
            "AdmissionType",
            "AgeAtDeath",
            "PatientType",
            "SHMIGroup",
            "QAPReview",
            "MedTriage",
            "MEOReview",
            "SJR1",
            "SJR2",
            "SJROutcome",
            "CodingReview"));

            foreach (var record in extract)
            {
                sw.WriteLine(String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}, {11}, {12}, {13}, {14}, {15}, {16},{17}, {18}, {19}, {20}, {21}, {22}, {23}, {24}, {25}, {26}, {27}, {28}, {29}", record.ID,
                    record.PatientID,
                    record.SpellNumber,
                    record.PatientName,
                    record.NHSNumber,
                    record.Gender,
                    record.Age,
                    record.DOB,
                    record.DateofAdmission,
                    record.TimeofAdmission,
                    record.DischargeWard,
                    record.DateofDeath,
                    record.TimeofDeath,
                    record.WardofDeath,
                    record.DischargeConsultantCode,
                    record.DischargeConsultantName,
                    record.DischargeSpecialityCode,
                    record.DischargeSpeciality,
                    record.CareGroup,
                    record.AdmissionType,
                    record.AgeAtDeath,
                    record.PatientType,
                    record.SHMIGroup,
                    record.QAPReview,
                    record.MedTriage,
                    record.MEOReview,
                    record.SJR1,
                    record.SJR2,
                    record.SJROutcome,
                    record.CodingReview));
            }
            Response.Clear();
            Response.AddHeader("Content-Disposition", "attachment; filename=patientdetails.csv");
            Response.ContentType = "text/csv";
            Response.Write(sw);
            Response.End();
        }

        public ActionResult UsersStatsDashboard()
        {
            return View();
        }

        #region Chart Load

        [HttpGet]
        public JsonResult AssignedLicenseBarChartResult(ChartLoadRequestObject reqObj)
        {
            var assignedLicense = "30";
            return Json(assignedLicense, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult UserLicenseChartResult(ChartLoadRequestObject reqObj)
        {
            var result = new
            {
                LicenseInfo = new LicenseInformation
                {
                    TotalNodLicenseAllocated = "100",
                    NoOfUsedLicense = "30",
                    NoOfUnusedLicense = "70"
                },
                ChartInfo = new List<ChartResponseInformation>()
                {
                    new ChartResponseInformation()
                    {
                        name = "Used",
                        data = new []
                        {
                            "44","55","41"
                        }
                    },
                    new ChartResponseInformation()
                    {
                        name = "Unused",
                        data = new []
                        {
                            "30","42","60"
                        }
                    }
                },
                ChartOptions = new ChartOptionInformation
                {
                    X_axisCategoryType = "category",
                    X_axisCategories = new[]
                    {
                        "Jan",
                        "Feb",
                        "Mar"
                    }
                }
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult AvgTimeModuleChartResult(ChartLoadRequestObject reqObj)
        {
            var result = new
            {
                ChartInfo = new List<ChartResponseInformation>()
                {
                    new ChartResponseInformation()
                    {
                        name = "AVG Time",
                        data = new []
                        {
                            $"{MinuteToDecimal(10,0):##.##}",
                            $"{MinuteToDecimal(8,0):##.##}",
                            $"{MinuteToDecimal(11,25):##.##}"
                        }
                    },

                },
                ChartOptions = new ChartOptionInformation
                {
                    X_axisCategoryType = "category",
                    X_axisCategories = new[]
                    {
                        "ME",
                        "STR",
                        "SJRL"
                    }
                }
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult UserOverTimeChartResult(ChartLoadRequestObject reqObj)
        {
            var result = new
            {
                ChartInfo = new List<ChartResponseInformation>()
                {
                    new ChartResponseInformation()
                    {
                        name = "Used License",
                        data = new []
                        {
                            "31", "40", "28", "51", "42", "109", "100"
                        }
                    },
                    new ChartResponseInformation()
                    {
                        name = "Unused License",
                        data = new []
                        {
                            "11", "32", "45", "32", "34", "52", "41"
                        }
                    },
                },
                ChartOptions = new ChartOptionInformation
                {
                    X_axisCategoryType = "category",
                    X_axisCategories = new[]
                    {
                        "1",
                        "2",
                        "3",
                        "4",
                        "5",
                        "6",
                        "7",
                    }
                }
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult AvgSpentTimeByModuleChartResult(ChartLoadRequestObject reqObj)
        {
            var result = new
            {
                ChartInfo = new List<ChartResponseInformation>()
                {
                    new ChartResponseInformation()
                    {
                        name = "MZ",
                        data = new []
                        {
                            "31", "40", "28", "51", "42", "109", "100"
                        }
                    },
                    new ChartResponseInformation()
                    {
                        name = "SJRI",
                        data = new []
                        {
                            "11", "32", "45", "32", "34", "52", "41"
                        }
                    },
                    new ChartResponseInformation()
                    {
                        name = "SJRL",
                        data = new []
                        {
                            "15", "45", "30", "28", "48", "55", "67"
                        }
                    }
                },
                ChartOptions = new ChartOptionInformation
                {
                    X_axisCategoryType = "category",
                    X_axisCategories = new[]
                    {
                        "1", "2", "3", "4", "5", "6", "7"
                    }
                }
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SessionAvgTimeByUserByModuleChartResult(ChartLoadRequestObject reqObj)
        {
            var result = new
            {
                ChartInfo = new List<ChartResponseInformation>()
                {
                    new ChartResponseInformation()
                    {
                        name = "ME",
                        data = new []
                        {
                            "20","50","13","18"
                        }
                    },
                    new ChartResponseInformation()
                    {
                        name = "STR",
                        data = new []
                        {
                            "46","0","28","32"
                        }
                    },
                    new ChartResponseInformation()
                    {
                        name = "SJRL",
                        data = new []
                        {
                            "30","30","25","15"
                        }
                    }
                },
                ChartOptions = new ChartOptionInformation
                {
                    X_axisCategoryType = "category",
                    X_axisCategories = new[]
                    {
                        "U1", "U2", "U3", "U4"
                    }
                }
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SessionChartResult(ChartLoadRequestObject reqObj)
        {
            var result = new
            {
                ChartInfo = new List<ChartResponseInformation>()
                {
                    new ChartResponseInformation
                    {
                        name = "",
                        data = new[]
                        {
                            "30", "46", "25", "50","38"
                        }
                    }
                },
                ChartOptions = new ChartOptionInformation
                {
                    X_axisCategoryType = "category",
                    X_axisCategories = new[]
                    {
                        "U1", "U2", "U3", "U4","U5"
                    }
                }
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SessionTimeChartResult(ChartLoadRequestObject reqObj)
        {
            var result = new
            {
                ChartInfo = new List<ChartResponseInformation>()
                {
                    new ChartResponseInformation()
                    {
                        name = "Session",
                        data = new[]
                        {
                            "10", "41", "35", "51", "49", "62", "69", "91", "148"
                        }
                    }
                },
                ChartOptions = new ChartOptionInformation()
                {
                    X_axisCategoryType = "category",
                    X_axisCategories = new[]
                    {
                        "1", "2", "3", "4", "5", "6", "7", "8", "9", "10"
                    }
                }
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private decimal MinuteToDecimal(int min, int sec)
        {
            return Convert.ToDecimal(min + sec * 0.0168);
        }


        #endregion

        public ActionResult ApproveUser(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            try
            {
                int retVal = dBEngine.ApproveUser(id, Convert.ToInt32(Session["LoginUserID"]));
                List<NotificationSettings> settings = dBEngine.GetNotificationSettingsByTrigger("Approve User", Convert.ToInt32(Session["LoginUserID"]));
                if (settings.Count > 0)
                {
                    for (int counter = 0; counter < settings.Count; counter++)
                    {
                        AppUsers user = dBEngine.GetUserByID(id, Convert.ToInt32(Session["LoginUserID"]));
                        SendEmail(user.EmailID, "CORS - Registration Approved", settings[counter].EmailTemplate);
                    }
                }                
            }
            catch(Exception exception)
            {
                dBEngine.LogException(exception.Message, this.ToString(), "Export", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
            }
            return RedirectToAction("Users");
        }

        public ActionResult AddPatient()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            try
            {
                ViewBag.PatientTypeDDM = dBEngine.GetPatientTypes(Convert.ToInt32(Session["LoginUserID"]));
                ViewBag.SpecialityDDM = dBEngine.GetSpecialitiesForDropDown(Convert.ToInt32(Session["LoginUserID"]));
                ViewBag.AdmissionType = dBEngine.GetAdmissionTypes(Convert.ToInt32(Session["LoginUserID"]));
                ViewBag.Wards = dBEngine.GetWards(Convert.ToInt32(Session["LoginUserID"]));
            }
            catch (Exception exception)
            {
                dBEngine.LogException(exception.Message, this.ToString(), "Export", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
            }
            return View();
        }

        public ActionResult EditPatient(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            clsPatientDetails patient = new clsPatientDetails();
            try
            {
                ViewBag.PatientTypeDDM = dBEngine.GetPatientTypes(Convert.ToInt32(Session["LoginUserID"]));
                ViewBag.SpecialityDDM = dBEngine.GetSpecialitiesForDropDown(Convert.ToInt32(Session["LoginUserID"]));
                ViewBag.AdmissionType = dBEngine.GetAdmissionTypes(Convert.ToInt32(Session["LoginUserID"]));
                ViewBag.Wards = dBEngine.GetWards(Convert.ToInt32(Session["LoginUserID"]));
                patient = dBEngine.GetPatientDetailsByPatientID(id, Convert.ToInt32(Session["LoginUserID"]));
            }
            catch (Exception exception)
            {
                dBEngine.LogException(exception.Message, this.ToString(), "Export", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
            }
            return View(patient);
        }

        [HttpPost]
        public ActionResult UpdatePatient(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            bool isenabled = false;
            string timeofdeath = Request.Form["TimeofDeath"];
            string timeofadmission = Request.Form["TimeOfAdmission"];
            if (Request.Form["IsEnabled"] == "on") isenabled = true;
            if (Request.Form["DOB"] == "") Request.Form["DOB"] = "01/01/1900";
            if (Request.Form["DateOfDeath"] == "") Request.Form["DateOfDeath"] = "01/01/1900";
            if (Request.Form["DateOfAdmission"] == "") Request.Form["DateOfAdmission"] = "01/01/1900";
            if (timeofdeath == "") timeofdeath = "00:00";
            if (timeofadmission == "") timeofadmission = "00:00";

            DBEngine dBEngine = new DBEngine(connectionString);
            int dbReturn = dBEngine.UpdatePatientByID(id, Request.Form["PatientID"], Request.Form["PatientName"], Request.Form["SpellNumber"], Request.Form["NHSNumber"], Request.Form["ddlGender"], Convert.ToDateTime(Request.Form["DOB"]), Convert.ToDateTime(Request.Form["DateOfAdmission"]), Convert.ToDateTime(timeofadmission), Request.Form["DischargeWard"], Convert.ToDateTime(Request.Form["DateOfDeath"]), Convert.ToDateTime(timeofdeath), Request.Form["WardofDeath"], Request.Form["DischargeConsutantName"], Request.Form["DischargeSpeciality"], Request.Form["AdmissionType"], Request.Form["Occupation"], Convert.ToInt32(Request.Form["ddlPatientType"]), isenabled, Convert.ToInt32(Session["LoginUserID"]));
            return RedirectToAction("PatientList", new { id = 0, isReset = false });
        }

        public ActionResult Users()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            Session["FullName"] = Session["FirstName"] + " " + Session["LastName"];
            Session["CommonPageSize"] = 10;
            Session["CommonColumn"] = null;
            List<CORSUsers> users = dBEngine.GetUsers(1,Convert.ToInt32(Session["CommonPageSize"]),"","CreatedDate","DESC", Convert.ToInt32(Session["LoginUserID"]));
            int totalRecords = 0;
            if (users.Count > 0)
            {
                totalRecords = users[0].TotalRecords;
            }

            int totalPagesCount = 0;
            totalPagesCount = (int)Math.Ceiling((float)totalRecords / Convert.ToInt32(Session["CommonPageSize"]));

            ViewBag.PageNumber = 1;
            ViewBag.SearchText = "";
            ViewBag.TotalPagesCount = totalPagesCount;
            ViewBag.TotalRecordCount = totalRecords;
            ViewBag.ModalRecordCount = users.Count;
            ViewBag.PageSize = Convert.ToInt32(Session["CommonPageSize"]);
            return View(users);
        }

        public ActionResult MoreUserDetails(int pageNumber = 1, int pageSize = 10, string searchfield = "", string SortColumn = "", bool frompager = false, bool fromfilter = false )
        {
            Session["CommonSearchText"] = searchfield;
            if (SortColumn != "")
            {
                if (!frompager)
                {
                    if (Session["CommonColumn"] == null || Convert.ToString(Session["CommonColumn"]) != SortColumn)
                    {
                        Session["CommonColumn"] = SortColumn;
                        Session["UserSortType"] = null;
                    }
                    if (Session["UserSortType"] == null)
                        Session["UserSortType"] = "DESC";
                    else if (Session["UserSortType"] != null && Convert.ToString(Session["CommonColumn"]) == SortColumn && Convert.ToString(Session["UserSortType"]) == "DESC")
                        Session["UserSortType"] = "ASC";
                    else if (Session["UserSortType"] != null && Convert.ToString(Session["CommonColumn"]) == SortColumn && Convert.ToString(Session["UserSortType"]) == "ASC")
                        Session["UserSortType"] = "DESC";
                }
                else
                {
                    if (Session["CommonColumn"] == null || Convert.ToString(Session["CommonColumn"]) != SortColumn)
                    {
                        Session["CommonColumn"] = SortColumn;
                        Session["UserSortType"] = null;
                    }
                    if (Session["UserSortType"] == null)
                        Session["UserSortType"] = "DESC";
                }
            }
            else
            {
                Session["CommonColumn"] = "CreatedDate";
                Session["UserSortType"] = "DESC";
            }
            if (pageSize != Convert.ToInt32(Session["CommonPageSize"]))
                Session["CommonPageSize"] = pageSize;
            if (Session["UserSortType"] == null)
                Session["UserSortType"] = "DESC";
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            Session["FullName"] = Session["FirstName"] + " " + Session["LastName"];
            List<CORSUsers> users = dBEngine.GetUsers(pageNumber, Convert.ToInt32(Session["CommonPageSize"]), searchfield, Convert.ToString(Session["CommonColumn"]), Convert.ToString(Session["UserSortType"]), Convert.ToInt32(Session["LoginUserID"]));
            int totalRecords = 0;
            if (users.Count > 0)
            {
                totalRecords = users[0].TotalRecords;
            }

            int totalPagesCount = 0;
            totalPagesCount = (int)Math.Ceiling((float)totalRecords / Convert.ToInt32(Session["CommonPageSize"]));

            ViewBag.PageNumber = pageNumber;
            ViewBag.SearchText = "";
            ViewBag.TotalPagesCount = totalPagesCount;
            ViewBag.TotalRecordCount = totalRecords;
            ViewBag.ModalRecordCount = users.Count;
            ViewBag.PageSize = Convert.ToInt32(Session["CommonPageSize"]);
            return PartialView("_UserList", users);
        }

        public JsonResult GetFilteredWards(string term="")
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            Session["FullName"] = Session["FirstName"] + " " + Session["LastName"];
            List<NHS.Common.Wards> wards = dBEngine.GetWards(Convert.ToInt32(Session["LoginUserID"]),term);
            return Json(wards, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFilteredConsultants(string term = "")
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            Session["FullName"] = Session["FirstName"] + " " + Session["LastName"];
            List<NHS.Common.Consultant> consultants = dBEngine.GetCORSConsultants(Convert.ToInt32(Session["LoginUserID"]),term);
            return Json(consultants, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFilteredSpecialities(string term = "")
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            Session["FullName"] = Session["FirstName"] + " " + Session["LastName"];
            List<NHS.Common.Specialities> specialities = dBEngine.GetSpecialitiesForDropDown(Convert.ToInt32(Session["LoginUserID"]), term );
            return Json(specialities, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Wards()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            Session["FullName"] = Session["FirstName"] + " " + Session["LastName"];
            List<Wards> wards = dBEngine.GetWards(Convert.ToInt32(Session["LoginUserID"]));
            return View(wards);
        }

        public ActionResult CodingIssueSummary()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            Session["FullName"] = Session["FirstName"] + " " + Session["LastName"];
            List<CodingIssueSummary> wards = dBEngine.GetCodingIssueSummary(Convert.ToInt32(Session["LoginUserID"]));
            return View(wards);
        }

        public ActionResult DQIssueSummary()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            Session["FullName"] = Session["FirstName"] + " " + Session["LastName"];
            List<DQIssueSummary> wards = dBEngine.GetDQIssueSummary(Convert.ToInt32(Session["LoginUserID"]));
            return View(wards);
        }

        public ActionResult UrgentMESummary()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            Session["FullName"] = Session["FirstName"] + " " + Session["LastName"];
            List<UrgentMESummary> wards = dBEngine.GetUrgentMESummary(Convert.ToInt32(Session["LoginUserID"]));
            return View(wards);
        }

        public ActionResult OtherReferralSummary()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            Session["FullName"] = Session["FirstName"] + " " + Session["LastName"];
            List<OtherReferralSummary> wards = dBEngine.GetOtherReferralSummary(Convert.ToInt32(Session["LoginUserID"]));
            return View(wards);
        }

        public ActionResult FullSJR2Summary()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            Session["FullName"] = Session["FirstName"] + " " + Session["LastName"];
            List<FullSJR2Summary> wards = dBEngine.GetFullSJR2Summary(Convert.ToInt32(Session["LoginUserID"]));
            return View(wards);
        }

        public ActionResult FullSJRSummary()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            Session["FullName"] = Session["FirstName"] + " " + Session["LastName"];
            List<FullSJRSummary> wards = dBEngine.GetFullSJRSummary(Convert.ToInt32(Session["LoginUserID"]));
            return View(wards);
        }

        public ActionResult NotificationSettings()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            Session["FullName"] = Session["FirstName"] + " " + Session["LastName"];
            List<NotificationSettings> wards = dBEngine.GetNotificationSettings(Convert.ToInt32(Session["LoginUserID"]));
            return View(wards);
        }

        public ActionResult RolePermission()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            Session["FullName"] = Session["FirstName"] + " " + Session["LastName"];
            Session["CommonPageSize"] = 10;
            Session["CommonColumn"] = null;
            List<RolePermission> users = dBEngine.GetRolePermission(1, Convert.ToInt32(Session["CommonPageSize"]),"", "","", Convert.ToInt32(Session["LoginUserID"]));
            int totalRecords = 0;
            if (users.Count > 0)
            {
                totalRecords = users[0].TotalRecords;
            }

            int totalPagesCount = 0;
            totalPagesCount = (int)Math.Ceiling((float)totalRecords / Convert.ToInt32(Session["CommonPageSize"]));

            ViewBag.PageNumber = 1;
            ViewBag.SearchText = "";
            ViewBag.TotalPagesCount = totalPagesCount;
            ViewBag.TotalRecordCount = totalRecords;
            ViewBag.ModalRecordCount = users.Count;
            ViewBag.PageSize = Convert.ToInt32(Session["CommonPageSize"]);
            return View(users);
        }

        public ActionResult MorePermissionDetails(int pageNumber = 1, int pageSize = 10, string searchfield = "", string SortColumn = "", bool frompager = false, bool fromfilter = false)
        {
            Session["CommonSearchText"] = searchfield;
            if (SortColumn != "")
            {
                if (!frompager)
                {
                    if (Session["CommonColumn"] == null || Convert.ToString(Session["CommonColumn"]) != SortColumn)
                    {
                        Session["CommonColumn"] = SortColumn;
                        Session["PermissionSortType"] = null;
                    }
                    if (Session["PermissionSortType"] == null)
                        Session["PermissionSortType"] = "DESC";
                    else if (Session["PermissionSortType"] != null && Convert.ToString(Session["CommonColumn"]) == SortColumn && Convert.ToString(Session["PermissionSortType"]) == "DESC")
                        Session["PermissionSortType"] = "ASC";
                    else if (Session["PermissionSortType"] != null && Convert.ToString(Session["CommonColumn"]) == SortColumn && Convert.ToString(Session["PermissionSortType"]) == "ASC")
                        Session["PermissionSortType"] = "DESC";
                }
                else
                {
                    if (Session["CommonColumn"] == null || Convert.ToString(Session["CommonColumn"]) != SortColumn)
                    {
                        Session["CommonColumn"] = SortColumn;
                        Session["PermissionSortType"] = null;
                    }
                    if (Session["UserSortType"] == null)
                        Session["PermissionSortType"] = "DESC";
                }
            }
            else
            {
                Session["CommonColumn"] = "RoleName";
                Session["PermissionSortType"] = "ASC";
            }
            if (pageSize != Convert.ToInt32(Session["CommonPageSize"]))
                Session["CommonPageSize"] = pageSize;
            if (Session["PermissionSortType"] == null)
                Session["PermissionSortType"] = "DESC";
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            Session["FullName"] = Session["FirstName"] + " " + Session["LastName"];
            List<RolePermission> users = dBEngine.GetRolePermission(pageNumber, Convert.ToInt32(Session["CommonPageSize"]), Convert.ToString(Session["CommonColumn"]), Convert.ToString(Session["PermissionSortType"]), Convert.ToString(Session["CommonSearchText"]), Convert.ToInt32(Session["LoginUserID"]), "","");
            int totalRecords = 0;
            if (users.Count > 0)
            {
                totalRecords = users[0].TotalRecords;
            }

            int totalPagesCount = 0;
            totalPagesCount = (int)Math.Ceiling((float)totalRecords / Convert.ToInt32(Session["CommonPageSize"]));

            ViewBag.PageNumber = pageNumber;
            ViewBag.SearchText = "";
            ViewBag.TotalPagesCount = totalPagesCount;
            ViewBag.TotalRecordCount = totalRecords;
            ViewBag.ModalRecordCount = users.Count;
            ViewBag.PageSize = Convert.ToInt32(Session["CommonPageSize"]);
            return PartialView("_PermissionsList", users);
        }

        public ActionResult Consultants()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            Session["FullName"] = Session["FirstName"] + " " + Session["LastName"];
            List<Consultant> wards = dBEngine.GetCORSConsultants(Convert.ToInt32(Session["LoginUserID"]));
            return View(wards);
        }

        [HttpPost]
        public ActionResult AddPatient(FormCollection formCollection, string btnSave)
        {
            string actionName = "";
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            int retVal = 0;
            try
            {
                if (formCollection["Stage2SJRDateSent"] == "") formCollection["Stage2SJRDateSent"] = "";
                if (formCollection["TimeofAdmission"] == "") formCollection["TimeofAdmission"] = "00:00";
                if (formCollection["TimeofDeath"] == "") formCollection["TimeofDeath"] = "00:00";
                if (formCollection["DateOfDeath"] == "") formCollection["DateOfDeath"] = System.DateTime.Now.ToString("dd/MM/yyyy");
                if (formCollection["DateOfAdmission"] == "") formCollection["DateOfAdmission"] = System.DateTime.Now.ToString("dd/MM/yyyy");
                if (formCollection["DOB"] == "") formCollection["DOB"] = System.DateTime.Now.ToString("dd/MM/yyyy");

                if (formCollection["DateOfDeath"] != "")
                {
                    retVal = dBEngine.AddNewPatient(formCollection["PatientName"], formCollection["PatientId"], formCollection["SpellNumber"],
                        formCollection["NHSNumber"], DateTime.ParseExact(formCollection["DOB"], "dd/MM/yyyy", CultureInfo.InvariantCulture),
                        DateTime.ParseExact(formCollection["TimeofDeath"], "HH:mm", CultureInfo.InvariantCulture),
                        DateTime.ParseExact(formCollection["DateOfDeath"], "dd/MM/yyyy", CultureInfo.InvariantCulture), Convert.ToInt32(formCollection["ddlPatientType"]), formCollection["ddlGender"], DateTime.ParseExact(formCollection["DateOfAdmission"], "dd/MM/yyyy", CultureInfo.InvariantCulture), DateTime.ParseExact(formCollection["TimeofAdmission"], "HH:mm",  CultureInfo.InvariantCulture), formCollection["DischargeWard"], formCollection["WardofDeath"], formCollection["DischargeConsutantName"], formCollection["DischargeSpeciality"], formCollection["AdmissionType"],
                          formCollection["Occupation"],
                          Convert.ToInt32(Session["LoginUserID"]));
                }
                else
                {
                    retVal = dBEngine.AddNewPatient(formCollection["PatientName"], formCollection["PatientId"], formCollection["SpellNumber"],
                        formCollection["NHSNumber"], DateTime.ParseExact(formCollection["DOB"], "dd/MM/yyyy", CultureInfo.InvariantCulture),
                        DateTime.ParseExact(formCollection["TimeofDeath"], "HH:mm", CultureInfo.InvariantCulture),
                        DateTime.ParseExact(formCollection["DateOfDeath"], "dd/MM/yyyy", CultureInfo.InvariantCulture), Convert.ToInt32(formCollection["ddlPatientType"]), formCollection["ddlGender"], DateTime.ParseExact(formCollection["DateOfAdmission"], "dd/MM/yyyy", CultureInfo.InvariantCulture), DateTime.ParseExact(formCollection["TimeofAdmission"], "HH:mm", CultureInfo.InvariantCulture), formCollection["DischargeWard"], formCollection["WardofDeath"], formCollection["DischargeConsutantName"], formCollection["DischargeSpeciality"], formCollection["AdmissionType"],
                          formCollection["Occupation"],
                          Convert.ToInt32(Session["LoginUserID"]));
                }
                if (retVal == 0)
                {
                    Alert alertMessage = new Alert();
                    alertMessage.AlertType = ALERTTYPE.Error;
                    alertMessage.MessageType = ALERTMESSAGETYPE.TextWithClose;
                    alertMessage.Message = "Patient ID already exists in the EPR data. Please search on the dashboard to find the patient and proceed further.";
                    TempData["AlertMessage"] = alertMessage.Message;
                    actionName = "AddPatient";
                }
                else
                    actionName = "MedicalExaminerDashboard";
            }
            catch (Exception ex)
            {
                throw ex;
            }            
            if (actionName == "AddPatient")
                return RedirectToAction(actionName, "home");
            else
                return RedirectToAction(actionName, new { id = 0, startdate = Convert.ToString(Session["StartDate"]), enddate = Convert.ToString(Session["EndDate"]), patientype = Convert.ToString(Session["PatientType"]), dischargespeciality = Convert.ToString(Session["Speciality"]),
                  wardofdeath = Convert.ToString(Session["WardDeath"]),
                  consultant = Convert.ToString(Session["DischargeConsultant"]), isReset = false });
        }

        public ActionResult MROutcomeDashboard(FormCollection formCollection)
        {
            Session["MROutcomeDashboard"] = true;
            Session["SJRTracking"] = false;
            Session["PatientDetails"] = null;
            Session["A&EDeath"] = false;
            Session["MEOutstanding"] = false;
            Session["MECompleted"] = false;
            Session["MCCDIssue"] = false;
            Session["Referral"] = false;
            Session["PostMortem"] = false;
            Session["CauseID"] = 0;
            Session["CareQuality"] = false;
            Session["RecommndedReferral"] = false;
            Session["SJRRequired"] = false;
            Session["SJRNotRequired"] = false;
            Session["PatientSIRI"] = false;
            Session["SafeGuard"] = false;
            Session["LearningDisability"] = false;
            Session["ChildDeath"] = false;
            Session["WardTeam"] = false;
            Session["HeadOfCompliance"] = false;
            Session["PALsComplaints"] = false;
            Session["Other"] = false;
            Session["ProblemOccured"] = false;
            Session["KeyLearnings"] = false;
            Session["SJR1Completed"] = false;
            Session["SJR1Outstanding"] = false;
            Session["SJR1Grade0"] = false;
            Session["SJR1Grade2"] = false;
            Session["SJR1Grade3"] = false;
            Session["MSGGrade0"] = false;
            Session["MSGGrade2"] = false;
            Session["MSG1Grade3"] = false;
            Session["CoronerDecisionNFAction"] = false;
            Session["CoronerDecisionPostMortem"] = false;
            Session["CoronerDecisionForensicPM"] = false;
            Session["CoronerDecisionGPIssue"] = false;
            Session["CoronerDecisionInquest"] = false;
            Session["CoronerDecision100A"] = false;
            Session["FeedbackTypeID"] = 0;
            Session["FormCompleted"] = false;
            Session["Learning"] = false;
            Session["Paeds"] = false;
            Session["Mental"] = false;
            Session["Elective"] = false;
            Session["NOK"] = false;
            Session["Chemo"] = false;
            Session["OtherReason"] = false;
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            ViewBag.CareGroups = dBEngine.GetCareGroups(Convert.ToInt32(Session["LoginUserID"]));
            ViewBag.Consultants = dBEngine.GetCORSConsultants(Convert.ToInt32(Session["LoginUserID"]));
            ViewBag.PatientTypeDDM = dBEngine.GetPatientTypes(Convert.ToInt32(Session["LoginUserID"]));
            ViewBag.Wards = dBEngine.GetWards(Convert.ToInt32(Session["LoginUserID"]));
            ViewBag.MENames = dBEngine.GetMENames(Convert.ToInt32(Session["LoginUserID"]));
            ViewBag.LoadDischargeSpecialityDropdown = dBEngine.GetSpecialities(Convert.ToInt32(Session["LoginUserID"]));
            MROutcomeDashbaord dashboard = new MROutcomeDashbaord();
            if (formCollection.Count == 0)
            {
                dashboard = dBEngine.GetMROutComeDashboard(Convert.ToString(Session["ReportStartDate"]), Convert.ToString(Session["ReportEndDate"]), Convert.ToString(Session["ReportCaregroup"]), Convert.ToString(Session["ReportPatientType"]), Convert.ToString(Session["ReportWardDeath"]), Convert.ToString(Session["ReportDischargeConsultant"]), Convert.ToString(Session["ReportSpeciality"]), Convert.ToString(Session["ReportMEName"]), Convert.ToInt32(Session["LoginUserID"]));                
            }
            else
            {
                dBEngine.LogException(formCollection["StartDate"].ToString(),"", "", System.DateTime.Now, 1);
                dashboard = dBEngine.GetMROutComeDashboard(formCollection["StartDate"], formCollection["EndDate"], formCollection["ddlCareGroups"], formCollection["ddlPatientType"], formCollection["ddlWardDeath"], formCollection["ddlDischargeConsultant"], formCollection["ddlSpeciality"], formCollection["ddlMENames"], Convert.ToInt32(Session["LoginUserID"]));
            }
            if (formCollection.Count > 0)
            {
                Session["ReportStartDate"] = DateTime.ParseExact(Convert.ToString(formCollection["StartDate"]), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                Session["ReportEndDate"] = DateTime.ParseExact(Convert.ToString(formCollection["EndDate"]), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                if (formCollection["ddlCareGroups"] != "")
                    Session["ReportCaregroup"] = formCollection["ddlCareGroups"];
                if (formCollection["ddlPatientType"] != "")
                    Session["ReportPatientType"] = formCollection["ddlPatientType"];
                if (formCollection["ddlWardDeath"] != "")
                    Session["ReportWardDeath"] = formCollection["ddlWardDeath"];
                if (formCollection["ddlDischargeConsultant"] != "")
                    Session["ReportDischargeConsultant"] = formCollection["ddlDischargeConsultant"];
                if (formCollection["ddlSpeciality"] != null)
                    Session["ReportSpeciality"] = formCollection["ddlSpeciality"];
                if (formCollection["ddlMENames"] != null)
                    Session["ReportMEName"] = formCollection["ddlMENames"];
            }
            return View(dashboard);
        }

        public ActionResult QAPReport(int id)
        {
            clsQAPReview qapreview = new clsQAPReview();
            if (Session["FullName"] != null)
            {
                if (string.IsNullOrEmpty(Session["FullName"].ToString()))
                    return RedirectToAction("Index", "Account");
            }
            else
                return RedirectToAction("Index", "Account");

            if (id == 0)
            {
                if (Session["PatientID"] != null)
                    id = Convert.ToInt32(Session["PatientID"]);
                else
                    return RedirectToAction("Index", "Account");
            }
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            qapreview = dBEngine.GetQAPReview(id, Convert.ToInt32(Session["LoginUserID"]));
            List<clsPatientDetails> patientDetails = dBEngine.GetPatientDetails(id, Convert.ToInt32(Session["LoginUserID"]));
            qapreview.UserRole = patientDetails.ToList()[0].UserRole;
            qapreview.PatientId = patientDetails.ToList()[0].PatientId.ToString();
            qapreview.PatientName = patientDetails.ToList()[0].PatientName;
            qapreview.DOB = patientDetails.ToList()[0].DOB;
            return View(qapreview);
        }

        public ActionResult MEReport()
        {
            return View();
        }

        public ActionResult MEReviewTrackingReport()
        {
            return View();
        }

        public ActionResult SJR1Report(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            //dBEngine.LogException("Inside SJR1", "", "", System.DateTime.Now);
            SJR1Report patientDetails = new SJR1Report();
            if (Session["FullName"] != null)
            {
                if (string.IsNullOrEmpty(Session["FullName"].ToString()))
                    return RedirectToAction("Index", "Account");
            }
            else
                return RedirectToAction("Index", "Account");
            bool isUser = GetUserDetailsFromAD();
            List<clsPatientDetails> medicalExaminerReview = new List<clsPatientDetails>();
            //dBEngine.LogException("before try", "", "", System.DateTime.Now);
            try
            {
                if (id == 0)
                {
                    if (Session["PatientID"] != null)
                        id = Convert.ToInt32(Session["PatientID"]);
                    else
                        return RedirectToAction("Index", "Account");
                }
                //if (ApplicationSession.LoginUserID > 0)
                //{
                try
                {
                    if (Request.HttpMethod != "POST")
                    {
                        medicalExaminerReview = dBEngine.GetPatientDetailsByID(id, Convert.ToInt32(Session["LoginUserID"]));
                        patientDetails = dBEngine.GetSJR1Report(id, Convert.ToInt32(Session["LoginUserID"]));
                        patientDetails.PatientID = medicalExaminerReview.ToList()[0].PatientId.ToString();
                        patientDetails.PatientName = medicalExaminerReview.ToList()[0].PatientName;
                        patientDetails.DOB = medicalExaminerReview.ToList()[0].DOB;
                    }
                    //ViewBag.ExcellentRatingID = dBEngine.GetRatingIDByName("Excellent");
                    //ViewBag.GoodRatingID = dBEngine.GetRatingIDByName("Good");
                    //ViewBag.AdequateRatingID = dBEngine.GetRatingIDByName("Adequate");
                    //ViewBag.PoorRatingID = dBEngine.GetRatingIDByName("Poor");
                    //ViewBag.VeryPoorRatingID = dBEngine.GetRatingIDByName("Very Poor");
                }
                catch (Exception ex)
                {
                    dBEngine.LogException(ex.Message, this.ToString(), "ValidateUser", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View(patientDetails);
        }

        public ActionResult SJR2Report(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            //dBEngine.LogException("Inside SJR1", "", "", System.DateTime.Now);
            SJR2Report patientDetails = new SJR2Report();
            if (Session["FullName"] != null)
            {
                if (string.IsNullOrEmpty(Session["FullName"].ToString()))
                    return RedirectToAction("Index", "Account");
            }
            else
                return RedirectToAction("Index", "Account");
            bool isUser = GetUserDetailsFromAD();
            List<clsPatientDetails> medicalExaminerReview = new List<clsPatientDetails>();
            //dBEngine.LogException("before try", "", "", System.DateTime.Now);
            try
            {
                if (id == 0)
                {
                    if (Session["PatientID"] != null)
                        id = Convert.ToInt32(Session["PatientID"]);
                    else
                        return RedirectToAction("Index", "Account");
                }
                //if (ApplicationSession.LoginUserID > 0)
                //{
                try
                {
                    if (Request.HttpMethod != "POST")
                    {
                        medicalExaminerReview = dBEngine.GetPatientDetailsByID(id, Convert.ToInt32(Session["LoginUserID"]));
                        patientDetails = dBEngine.GetSJR2Report(id, Convert.ToInt32(Session["LoginUserID"]));
                        patientDetails.PatientID = medicalExaminerReview.ToList()[0].PatientId.ToString();
                        patientDetails.PatientName = medicalExaminerReview.ToList()[0].PatientName;
                        patientDetails.DOB = medicalExaminerReview.ToList()[0].DOB;
                    }
                    //ViewBag.ExcellentRatingID = dBEngine.GetRatingIDByName("Excellent");
                    //ViewBag.GoodRatingID = dBEngine.GetRatingIDByName("Good");
                    //ViewBag.AdequateRatingID = dBEngine.GetRatingIDByName("Adequate");
                    //ViewBag.PoorRatingID = dBEngine.GetRatingIDByName("Poor");
                    //ViewBag.VeryPoorRatingID = dBEngine.GetRatingIDByName("Very Poor");
                }
                catch (Exception ex)
                {
                    dBEngine.LogException(ex.Message, this.ToString(), "ValidateUser", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View(patientDetails);
        }

        public ActionResult FeedbackReport()
        {
            return View();
        }

        public ActionResult SJRReviewStatus()
        {
            return View();
        }

        public ActionResult SJRTracking(FormCollection formCollection)
        {
            Session["MROutcomeDashboard"] = false;
            Session["SJRTracking"] = true;
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            ViewBag.CareGroups = dBEngine.GetCareGroups(Convert.ToInt32(Session["LoginUserID"]));
            ViewBag.Consultants = dBEngine.GetCORSConsultants(Convert.ToInt32(Session["LoginUserID"]));
            ViewBag.PatientTypeDDM = dBEngine.GetPatientTypes(Convert.ToInt32(Session["LoginUserID"]));
            ViewBag.Wards = dBEngine.GetWards(Convert.ToInt32(Session["LoginUserID"]));
            ViewBag.MENames = dBEngine.GetMENames(Convert.ToInt32(Session["LoginUserID"]));
            ViewBag.LoadDischargeSpecialityDropdown = dBEngine.GetSpecialities(Convert.ToInt32(Session["LoginUserID"]));
            SJRTracking dashboard = new SJRTracking();
            if (formCollection.Count == 0)
            {
                dashboard = dBEngine.GetSJRTrackingDashboard(Convert.ToString(Session["ReportStartDate"]), Convert.ToString(Session["ReportEndDate"]), Convert.ToString(Session["ReportCaregroup"]), Convert.ToString(Session["ReportPatientType"]), Convert.ToString(Session["ReportWardDeath"]), Convert.ToString(Session["ReportDischargeConsultant"]), Convert.ToString(Session["ReportSpeciality"]), Convert.ToString(Session["ReportMEName"]), Convert.ToInt32(Session["LoginUserID"]));
            }
            else
            {
                dashboard = dBEngine.GetSJRTrackingDashboard(formCollection["StartDate"], formCollection["EndDate"], formCollection["ddlCareGroups"], formCollection["ddlPatientType"], formCollection["ddlWardDeath"], formCollection["ddlDischargeConsultant"], formCollection["ddlSpeciality"], formCollection["ddlMENames"], Convert.ToInt32(Session["LoginUserID"]));
            }
            if (formCollection.Count > 0)
            {
                Session["ReportStartDate"] = DateTime.ParseExact(Convert.ToString(formCollection["StartDate"]),"dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                Session["ReportEndDate"] = DateTime.ParseExact(Convert.ToString(formCollection["EndDate"]), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture); 
                if (formCollection["ddlCareGroups"] != "")
                    Session["ReportCaregroup"] = formCollection["ddlCareGroups"];
                if (formCollection["ddlPatientType"] != "")
                    Session["ReportPatientType"] = formCollection["ddlPatientType"];
                if (formCollection["ddlWardDeath"] != "")
                    Session["ReportWardDeath"] = formCollection["ddlWardDeath"];
                if (formCollection["ddlDischargeConsultant"] != "")
                    Session["ReportDischargeConsultant"] = formCollection["ddlDischargeConsultant"];
                if (formCollection["ddlSpeciality"] != null)
                    Session["ReportSpeciality"] = formCollection["ddlSpeciality"];
                if (formCollection["ddlMENames"] != null)
                    Session["ReportMEName"] = formCollection["ddlMENames"];
            }
            return View(dashboard);
        }

        public ActionResult MEOReport(int id)
        {
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            if (Session["FullName"] != null)
            {
                if (string.IsNullOrEmpty(Session["FullName"].ToString()))
                    return RedirectToAction("Index", "Account");
            }
            else
                return RedirectToAction("Index", "Account");
            bool isUser = GetUserDetailsFromAD();
            clsMedicalExaminerDecision medicalExaminerDecision = new clsMedicalExaminerDecision();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            ViewBag.SpecialityDDM = dBEngine.GetSpecialitiesForDropDown(Convert.ToInt32(Session["LoginUserID"]));
            try
            {
                if (id == null || id == 0)
                {
                    if (Session["PatientID"] != null)
                        id = Convert.ToInt32(Session["PatientID"]);
                    else
                        return RedirectToAction("Index", "Account");
                }
                try
                {
                    medicalExaminerDecision = dBEngine.GetMEOOutcome(id, Convert.ToInt32(Session["LoginUserID"]));

                    patientDetails = dBEngine.GetPatientDetails(id, Convert.ToInt32(Session["LoginUserID"]));

                    medicalExaminerDecision.Patient_Id = patientDetails.ToList()[0].PatientId;
                    medicalExaminerDecision.PatientName = patientDetails.ToList()[0].PatientName;
                    medicalExaminerDecision.DOB = patientDetails.ToList()[0].DOB;
                    medicalExaminerDecision.UserRole = patientDetails.ToList()[0].UserRole;
                    medicalExaminerDecision.a = "1a";
                    medicalExaminerDecision.b = "1b";
                    medicalExaminerDecision.c = "1c";
                    medicalExaminerDecision.d = "1d";
                    if (medicalExaminerDecision.ID == 0 || medicalExaminerDecision.ID == null)
                        medicalExaminerDecision.ID = Convert.ToInt32(id);
                    //ViewBag.ReasonGroups = dBEngine.GetReasonGroups();
                }
                catch (Exception ex)
                {
                    dBEngine.LogException(ex.Message, this.ToString(), "ValidateUser", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View(medicalExaminerDecision);
        }

        [HttpPost]
        public ActionResult CreateUser()
        {
            string controllername = "";
            string actionname = ""; ;
            AppUsers usermodel = new AppUsers();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            int dbReturn = dBEngine.CreateUserByAdmin(Request.Form["EmailID"], Request.Form["FirstName"], Request.Form["LastName"], Request.Form["UserName"], Request.Form["ddlSpeciality"], Convert.ToInt32(Request.Form["ddlRole"]), Request.Form["Code"], Convert.ToInt32(Session["LoginUserID"]));
            if (dbReturn == 0)
            {
                Alert alertMessage = new Alert();
                alertMessage.AlertType = ALERTTYPE.Success;
                alertMessage.MessageType = ALERTMESSAGETYPE.TextWithClose;
                alertMessage.Message = "User has been added successfully.";
                TempData["AlertMessage"] = alertMessage.Message;
                controllername = "Home";
                actionname = "Users";
            }
            else
            {
                Alert alertMessage = new Alert();
                alertMessage.AlertType = ALERTTYPE.Error;
                alertMessage.MessageType = ALERTMESSAGETYPE.TextWithClose;
                alertMessage.Message = "Could not add user.";
                TempData["AlertMessage"] = alertMessage.Message;
                controllername = "Home";
                actionname = "AddUser";
            }
            return RedirectToAction(actionname, controllername);
        }

        public ActionResult AddUser()
        {
            AppUsers usermodel = new AppUsers();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            ViewBag.RoleDDM = dBEngine.GetRoles(Convert.ToInt32(Session["LoginUserID"]), true);
            ViewBag.SpecialityDDM = dBEngine.GetSpecialitiesForDropDown(Convert.ToInt32(Session["LoginUserID"]));
            return View(usermodel);
        }

        public ActionResult NotAuthorizedPatientDetails(int? id)
        {
            bool isUser = GetUserDetailsFromAD();
            Session["FullName"] = Session["FirstName"] + " " + Session["LastName"];
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            Session["PatientID"] = id;


            DBEngine dBEngine = new DBEngine(connectionString);
            //dBEngine.LogException("set session id " + id.ToString(), this.ToString(), "ValidateUser", System.DateTime.Now);
            if (id == null || id == 0)
            {
                if (Session["PatientID"] != null)
                    id = Convert.ToInt32(Session["PatientID"]);
                else
                    return RedirectToAction("Index", "Account");
            }
            //dBEngine.LogException("after id null and 0 check", this.ToString(), "ValidateUser", System.DateTime.Now);
            try
            {
                patientDetails = dBEngine.GetPatientDetailsByID(id, Convert.ToInt32(Session["LoginUserID"]));
                //dBEngine.LogException("after patient details call" + patientDetails.Count.ToString(), this.ToString(), "ValidateUser", System.DateTime.Now);
                if (patientDetails.Count == 0)
                {
                    clsPatientDetails clsPatientDetail = new clsPatientDetails();
                    clsPatientDetail.ID = Convert.ToInt32(id);
                    patientDetails.Add(clsPatientDetail);
                    //dBEngine.LogException("in patient details count 0" + clsPatientDetail.ID.ToString(), this.ToString(), "ValidateUser", System.DateTime.Now);
                }
            }
            catch (Exception ex)
            {
                dBEngine.LogException(ex.Message, this.ToString(), "ValidateUser", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
            }
            return View(patientDetails[0]);
        }

        public ActionResult NotAuthorizedSJR(int? id, bool isdashboard = false)
        {
            bool isUser = GetUserDetailsFromAD();
            Session["FullName"] = Session["FirstName"] + " " + Session["LastName"];
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            Session["PatientID"] = id;


            DBEngine dBEngine = new DBEngine(connectionString);
            //dBEngine.LogException("set session id " + id.ToString(), this.ToString(), "ValidateUser", System.DateTime.Now);
            if (!isdashboard)
            {
                if (id == null || id == 0)
                {
                    if (Session["PatientID"] != null)
                        id = Convert.ToInt32(Session["PatientID"]);
                    else
                        return RedirectToAction("Index", "Account");
                }

                //dBEngine.LogException("after id null and 0 check", this.ToString(), "ValidateUser", System.DateTime.Now);
                try
                {
                    patientDetails = dBEngine.GetPatientDetailsByID(id, Convert.ToInt32(Session["LoginUserID"]));
                    //dBEngine.LogException("after patient details call" + patientDetails.Count.ToString(), this.ToString(), "ValidateUser", System.DateTime.Now);
                    if (patientDetails.Count == 0)
                    {
                        clsPatientDetails clsPatientDetail = new clsPatientDetails();
                        clsPatientDetail.ID = Convert.ToInt32(id);
                        patientDetails.Add(clsPatientDetail);
                        //dBEngine.LogException("in patient details count 0" + clsPatientDetail.ID.ToString(), this.ToString(), "ValidateUser", System.DateTime.Now);
                    }
                }
                catch (Exception ex)
                {
                    dBEngine.LogException(ex.Message, this.ToString(), "ValidateUser", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
                }
                ViewBag.IsDashboard = false;
                return View(patientDetails[0]);
            }
            else
            {
                ViewBag.IsDashboard = true;
                return View();
            }
        }

        public ActionResult NotAuthorizedCOVID(int id, string patientID, bool isdashboard = false)
        {
            bool isUser = GetUserDetailsFromAD();
            Session["FullName"] = Session["FirstName"] + " " + Session["LastName"];
            clsCOVIDDetails patientDetails = new clsCOVIDDetails();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            //Session["PatientID"] = id;


            DBEngine dBEngine = new DBEngine(connectionString);
            //dBEngine.LogException("set session id " + id.ToString(), this.ToString(), "ValidateUser", System.DateTime.Now);
            if (!isdashboard)
            {
                try
                {
                    patientDetails = dBEngine.GetCOVIDPatientDetailsByID(patientID, Convert.ToInt32(Session["LoginUserID"]));
                }
                catch (Exception ex)
                {
                    dBEngine.LogException(ex.Message, this.ToString(), "ValidateUser", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
                }
                ViewBag.IsDashboard = false;
                return View(patientDetails);
            }
            else
            {
                ViewBag.IsDashboard = true;
                return View();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult CodingReview(int? id)
        {

            if (Session["FullName"] != null)
            {
                if (string.IsNullOrEmpty(Session["FullName"].ToString()))
                    return RedirectToAction("Index", "Account");
            }
            else
                return RedirectToAction("Index", "Account");
            bool isUser = GetUserDetailsFromAD();
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            clsPatientDetails patient = new clsPatientDetails();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            try
            {
                try
                {
                    if (id != null)
                    {
                        ViewBag.Diagnoses = dBEngine.GetDiagnosisDetails(id, Convert.ToInt32(Session["LoginUserID"]));
                        ViewBag.Procedures = dBEngine.GetProcedureDetails(id, Convert.ToInt32(Session["LoginUserID"]));

                    }
                    else if (id == null || id == 0)
                    {
                        if (Session["PatientID"] != null)
                            id = Convert.ToInt32(Session["PatientID"]);
                        else
                            return RedirectToAction("Index", "Account");
                    }
                    if (Request.HttpMethod != "POST")
                        patientDetails = dBEngine.GetPatientDetailsByID(id, Convert.ToInt32(Session["LoginUserID"]));
                    //patient = dBEngine.GetCodingReviewByID(id);

                    //patientDetails[0].CodingIssueIdentified = patient.CodingIssueIdentified;
                    //patientDetails[0].Comments = patient.Comments;
                }
                catch (Exception ex)
                {
                    dBEngine.LogException(ex.Message, this.ToString(), "ValidateUser", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //List<clsPatientDetails> patientDetailsLst = new List<clsPatientDetails>();
            //patientDetailsLst = dBEngine.GetPatientDetails(id, Convert.ToInt32(Session["LoginUserID"]));
            //patientDetailsLst.ToList()[0].PatientId = patientDetails.ToList()[0].PatientId.ToString();
            //patientDetailsLst.ToList()[0].PatientName = patientDetails.ToList()[0].PatientName;
            //patientDetailsLst.ToList()[0].DOB = patientDetails.ToList()[0].DOB;
            List<RolePermission> permissions = dBEngine.GetRolePermission(0,0,"","","",Convert.ToInt32(Session["LoginUserID"]), "Coding Review", Convert.ToString(Session["Role"]));
            bool isnoaccess = false;
            if (permissions.Count > 0)
            {
                ViewBag.IsReadOnly = permissions[0].IsReadOnly;
                isnoaccess = permissions[0].NoAccess;
            }
            else
            {
                ViewBag.IsReadOnly = true;
                isnoaccess = true;
            }
            if (patientDetails.Count > 0)
            {
                if (isnoaccess == false)
                {
                    return View(patientDetails[0]);
                }
                else
                {
                    return RedirectToAction("NotAuthorizedSJR", new { id = id });
                }
            }
            else
            {
                if (isnoaccess == false)
                {
                    clsPatientDetails clspdtls = new clsPatientDetails();
                    patientDetails.Add(clspdtls);
                    return View(patientDetails[0]);
                }
                else
                {
                    return RedirectToAction("NotAuthorizedSJR", new { id = id });
                }

            }
            //return View(patientDetails[0]);
        }

        public ActionResult QualityReview(int? id)
        {

            clsQualityReview clsQReview = new clsQualityReview();
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            if (Session["FullName"] != null)
            {
                if (string.IsNullOrEmpty(Session["FullName"].ToString()))
                    return RedirectToAction("Index", "Account");
            }
            else
                return RedirectToAction("Index", "Account");
            bool isUser = GetUserDetailsFromAD();
            //List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            ViewBag.SpecialityDDM = dBEngine.GetSpecialitiesForDropDown(Convert.ToInt32(Session["LoginUserID"]));

            try
            {
                try
                {
                    if (id != null)
                    {
                        ViewBag.Diagnoses = dBEngine.GetDiagnosisDetails(id, Convert.ToInt32(Session["LoginUserID"]));
                        ViewBag.Procedures = dBEngine.GetProcedureDetails(id, Convert.ToInt32(Session["LoginUserID"]));
                        clsQReview = dBEngine.GetQualityReviewByID(id, Convert.ToInt32(Session["LoginUserID"]));
                        if (clsQReview.CreatedBy == null) clsQReview.CreatedBy = "";

                        patientDetails = dBEngine.GetPatientDetails(id, Convert.ToInt32(Session["LoginUserID"]));
                        clsQReview.PatientId = patientDetails.ToList()[0].PatientId.ToString();
                        clsQReview.PatientName = patientDetails.ToList()[0].PatientName;
                        clsQReview.DOB = patientDetails.ToList()[0].DOB;
                        clsQReview.UserRole = patientDetails.ToList()[0].UserRole;
                        if (clsQReview.Patient_ID == 0)
                            clsQReview.Patient_ID = Convert.ToInt32(id);
                    }
                    else if (id == null || id == 0)
                    {
                        if (Session["PatientID"] != null)
                            id = Convert.ToInt32(Session["PatientID"]);
                        else
                            return RedirectToAction("Index", "Account");
                    }
                    //if (Request.HttpMethod != "POST")
                    //patientDetails = dBEngine.GetPatientDetailsByID(id, Convert.ToInt32(Session["LoginUserID"]));
                }
                catch (Exception ex)
                {
                    dBEngine.LogException(ex.Message, this.ToString(), "ValidateUser", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            List<RolePermission> permissions = dBEngine.GetRolePermission(0, 0, "", "", "", Convert.ToInt32(Session["LoginUserID"]), "Quality Review", Convert.ToString(Session["Role"]));
            bool isnoaccess = false;
            if (permissions.Count > 0)
            {
                ViewBag.IsReadOnly = permissions[0].IsReadOnly;
                isnoaccess = permissions[0].NoAccess;
            }
            else
            {
                ViewBag.IsReadOnly = true;
                isnoaccess = true;
            }
            if (isnoaccess == false)
            {
                return View(clsQReview);
            }
            else
            {
                return RedirectToAction("NotAuthorizedSJR", new { id = id });
            }
        }

        public ActionResult QualityReviewReport(int? id)
        {

            clsQualityReview clsQReview = new clsQualityReview();
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            if (Session["FullName"] != null)
            {
                if (string.IsNullOrEmpty(Session["FullName"].ToString()))
                    return RedirectToAction("Index", "Account");
            }
            else
                return RedirectToAction("Index", "Account");
            bool isUser = GetUserDetailsFromAD();
            //List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            ViewBag.SpecialityDDM = dBEngine.GetSpecialitiesForDropDown(Convert.ToInt32(Session["LoginUserID"]));

            try
            {
                try
                {
                    if (id != null)
                    {
                        ViewBag.Diagnoses = dBEngine.GetDiagnosisDetails(id, Convert.ToInt32(Session["LoginUserID"]));
                        ViewBag.Procedures = dBEngine.GetProcedureDetails(id, Convert.ToInt32(Session["LoginUserID"]));
                        clsQReview = dBEngine.GetQualityReviewByID(id, Convert.ToInt32(Session["LoginUserID"]));
                        if (clsQReview.CreatedBy == null) clsQReview.CreatedBy = "";

                        patientDetails = dBEngine.GetPatientDetails(id, Convert.ToInt32(Session["LoginUserID"]));
                        clsQReview.PatientId = patientDetails.ToList()[0].PatientId.ToString();
                        clsQReview.PatientName = patientDetails.ToList()[0].PatientName;
                        clsQReview.DOB = patientDetails.ToList()[0].DOB;
                        clsQReview.UserRole = patientDetails.ToList()[0].UserRole;
                        if (clsQReview.Patient_ID == 0)
                            clsQReview.Patient_ID = Convert.ToInt32(id);
                    }
                    else if (id == null || id == 0)
                    {
                        if (Session["PatientID"] != null)
                            id = Convert.ToInt32(Session["PatientID"]);
                        else
                            return RedirectToAction("Index", "Account");
                    }
                    //if (Request.HttpMethod != "POST")
                    //patientDetails = dBEngine.GetPatientDetailsByID(id, Convert.ToInt32(Session["LoginUserID"]));
                }
                catch (Exception ex)
                {
                    dBEngine.LogException(ex.Message, this.ToString(), "ValidateUser", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View(clsQReview);
        }
        [HttpPost]
        public ActionResult QualityReview(FormCollection formCollection, string BtnPrevious, string BtnFinish, int? id)
        {

            string actionName = "";
            bool isCodingIssue = false;
            bool isTimingIssue = false;
            bool isData_SystemIssue = false;
            bool isClinicalReview = false;
            bool isProcessReview = false;
            bool isReviewComplete = false;
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            try
            {
                if (BtnPrevious != null)
                    actionName = "CodingReview";
                else
                {
                    if (id == null || id == 0)
                    {
                        if (Session["PatientID"] != null)
                            id = Convert.ToInt32(Session["PatientID"]);
                        else
                            return RedirectToAction("Index", "Account");
                    }
                    if (string.IsNullOrEmpty(formCollection["isReviewCompleted"]))
                        isReviewComplete = false;
                    else
                    {
                        if (formCollection["isReviewCompleted"] == "on")
                        {
                            isReviewComplete = true;
                        }
                        else
                        {
                            isReviewComplete = false;
                        }

                    }
                    int retVal = dBEngine.UpdateQualityReview(formCollection["ddlSourceReview"], formCollection["ReviewDate"], formCollection["ReviewerName"], formCollection["Spell"], formCollection["Summary"], formCollection["isCodingIssue"], formCollection["isTimingIssue"], formCollection["isDataSysIssue"], formCollection["isClinicalReview"], formCollection["isProcessReview"], formCollection["Recom"], isReviewComplete, Convert.ToInt32(formCollection["SpecialityID"]),  Convert.ToInt32(id), Convert.ToInt32(Session["LoginUserID"]));
                    actionName = "MortalityReview";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return RedirectToAction(actionName, new { id = id });
            return View();
        }
        public ActionResult QAPReviewFirstStep(int? id)
        {
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            if (Session["FullName"] != null)
            {
                if (string.IsNullOrEmpty(Session["FullName"].ToString()))
                    return RedirectToAction("Index", "Account");
            }
            else
                return RedirectToAction("Index", "Account");

            if (id == null || id == 0)
            {
                if (Session["PatientID"] != null)
                    id = Convert.ToInt32(Session["PatientID"]);
                else
                    return RedirectToAction("Index", "Account");
            }
            bool isUser = GetUserDetailsFromAD();
            clsQAPReview qapreview = new clsQAPReview();
            
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            ViewBag.SpecialityDDM = dBEngine.GetSpecialitiesForDropDown(Convert.ToInt32(Session["LoginUserID"]));
            try
            {
                try
                {
                    if (Request.HttpMethod != "POST")
                    {
                        qapreview = dBEngine.GetQAPReview(id, Convert.ToInt32(Session["LoginUserID"]));

                        patientDetails = dBEngine.GetPatientDetails(id, Convert.ToInt32(Session["LoginUserID"]));
                        qapreview.UserRole = patientDetails.ToList()[0].UserRole;
                        qapreview.PatientId = patientDetails.ToList()[0].PatientId.ToString();
                        qapreview.PatientName = patientDetails.ToList()[0].PatientName;
                        qapreview.DOB = patientDetails.ToList()[0].DOB;
                    }
                    if (qapreview.Patient_ID == 0 || qapreview.Patient_ID == null)
                        qapreview.Patient_ID = Convert.ToInt32(id);
                    if (qapreview.QAPReview == 3)
                    {
                        Session["QAPName"] = qapreview.CreatedBy;
                    }
                }
                catch (Exception ex)
                {
                    dBEngine.LogException(ex.Message, this.ToString(), "ValidateUser", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            List<RolePermission> permissions = dBEngine.GetRolePermission(0, 0, "", "", "",Convert.ToInt32(Session["LoginUserID"]), "QAP Review", Convert.ToString(Session["Role"]));
            bool isnoaccess = false;
            if (permissions.Count > 0)
            {
                ViewBag.IsReadOnly = permissions[0].IsReadOnly;
                isnoaccess = permissions[0].NoAccess;
            }
            else
            {
                ViewBag.IsReadOnly = true;
                isnoaccess = true;
            }
            if (isnoaccess == false)
            {
                return View(qapreview);
            }
            else
            {
                return RedirectToAction("NotAuthorizedSJR", new { id = id });
            }            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formCollection"></param>
        /// <param name="btnSave"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CodingReview(FormCollection formCollection, string btnSave, int? id)
        {
            string actionName = "";
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            try
            {
                if (id == null || id == 0)
                {
                    if (Session["PatientID"] != null)
                        id = Convert.ToInt32(Session["PatientID"]);
                    else
                        return RedirectToAction("Index", "Account");
                }
                int patientid = Convert.ToInt32(id);
                if (formCollection["CodingIssueIdentified"] == "on") formCollection["CodingIssueIdentified"] = "true"; else formCollection["CodingIssueIdentified"] = "false";
                int retVal = dBEngine.UpdateCodingReview(Convert.ToBoolean(formCollection["CodingIssueIdentified"]), formCollection["Comments"], id, Convert.ToInt32(Session["LoginUserID"]));
                if(Convert.ToBoolean(formCollection["CodingIssueIdentified"]) == true)
                {
                    List<clsPatientDetails> patient = new List<clsPatientDetails>();
                    patient = dBEngine.GetPatientDetailsByID(id,Convert.ToInt32(Session["LoginUserID"]));
                    List<NotificationSettings> settings = dBEngine.GetNotificationSettingsByTrigger("Coding Issue Identified", Convert.ToInt32(Session["LoginUserID"]));
                    if(settings.Count > 0)
                    {
                        for (int counter = 0; counter < settings.Count; counter++)
                        {
                            List<AppUsers> users = dBEngine.GetUsersByRoleID(settings[counter].RoleID, Convert.ToInt32(Session["LoginUserID"]));
                            if (patient.Count > 0)
                            {
                                int dbReturn = dBEngine.UpdateCodingIssue(patient[0].PatientId, patient[0].PatientName, DateTime.ParseExact(patient[0].DateofDeath,"dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), DateTime.ParseExact(Convert.ToString(patient[0].CRCreatedDate), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), patient[0].CRCreatedByName, patient[0].Comments, Convert.ToInt32(Session["LoginUserID"]));
                                PatientNotification notification = dBEngine.GetNotificationSentStatusByPatientID(patientid, "Coding Issue Identified", Convert.ToInt32(Session["LoginUserID"]));
                                if (notification.NotificationTrigger == null)
                                {
                                    for (int count = 0; count < users.Count; count++)
                                    {

                                        SendEmail(users[count].EmailID, "CORS - Coding Issue", settings[counter].EmailTemplate.Replace("##PatientID##", patient[0].PatientId).Replace("##PatientName##", patient[0].PatientName).Replace("##DateOfDeath##", patient[0].DateofDeath).Replace("##IssueDate##", patient[0].CRCreatedDate).Replace("##IssueRaisedby##", patient[0].CRCreatedByName).Replace("##Comment##", patient[0].Comments));
                                    }
                                    dBEngine.UpdatePatientNotification(patientid, "Coding Issue Identified", Convert.ToInt32(Session["LoginUserID"]));
                                }
                            }
                        }
                    }
                }
                actionName = "MortalityReview";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return RedirectToAction(actionName, new { id = id });
        }
        [HttpPost]
        public ActionResult QAPReviewFirstStep(FormCollection formCollection, string btnSave, int? id)
        {
            string actionName = "";
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            try
            {
                if (id == null || id == 0)
                {
                    if (Session["PatientID"] != null)
                        id = Convert.ToInt32(Session["PatientID"]);
                    else
                        return RedirectToAction("Index", "Account");
                }
                if (formCollection["Value1a"] == "") formCollection["Value1a"] = "0";
                if (formCollection["Value1b"] == "") formCollection["Value1b"] = "0";
                if (formCollection["Value1c"] == "") formCollection["Value1c"] = "0";
                if (formCollection["Value2"] == "") formCollection["Value2"] = "0";
                if (formCollection["MCCD"] == "on") formCollection["MCCD"] = "true"; else formCollection["MCCD"] = "false";
                if (formCollection["QAPReview"] == "on") formCollection["QAPReview"] = "true"; else formCollection["QAPReview"] = "false";
                if (formCollection["Concern"] == "on") formCollection["Concern"] = "true"; else formCollection["Concern"] = "false";
                if (formCollection["Referral"] == "on") formCollection["Referral"] = "true"; else formCollection["Referral"] = "false";
                int retVal = dBEngine.UpdateQAPReview(Convert.ToBoolean(formCollection["MCCD"]), Convert.ToBoolean(formCollection["Referral"]), formCollection["Synopsis"], formCollection["Reason"], formCollection["FullName"],
                    formCollection["GMCNo"], formCollection["Location"], formCollection["Phone"], formCollection["AlternatePhone"],
                    Convert.ToBoolean(formCollection["Concern"]), formCollection["Reason1a"], formCollection["Period1a"], Convert.ToDecimal(formCollection["Value1a"]), formCollection["Reason1b"], formCollection["Period1b"], Convert.ToDecimal(formCollection["Value1b"]), formCollection["Reason1c"], formCollection["Period1c"], Convert.ToDecimal(formCollection["Value1c"]), formCollection["Reason2"], formCollection["Period2"], Convert.ToDecimal(formCollection["Value2"]), Convert.ToBoolean(formCollection["QAPReview"]), id, Convert.ToInt32(Session["LoginUserID"]));
                actionName = "MortalityReview";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return RedirectToAction(actionName, new { id = id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult SJROutcome(int? id)
        {
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            if (Session["FullName"] != null)
            {
                if (string.IsNullOrEmpty(Session["FullName"].ToString()))
                    return RedirectToAction("Index", "Account");
            }
            else
                return RedirectToAction("Index", "Account");

            bool isUser = GetUserDetailsFromAD();
            clsSJROutcome sjrOutcome = new clsSJROutcome();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            ViewBag.SpecialityDDM = dBEngine.GetSpecialitiesForDropDown(Convert.ToInt32(Session["LoginUserID"]));
            try
            {
                if (id == null || id == 0)
                {
                    if (Session["PatientID"] != null)
                        id = Convert.ToInt32(Session["PatientID"]);
                    else
                        return RedirectToAction("Index", "Account");
                }
                //if (ApplicationSession.LoginUserID > 0)
                //{
                try
                {
                    if (Request.HttpMethod != "POST")
                    {
                        sjrOutcome = dBEngine.GetSJROutcome(id, Convert.ToInt32(Session["LoginUserID"]));

                        patientDetails = dBEngine.GetPatientDetails(id, Convert.ToInt32(Session["LoginUserID"]));
                        sjrOutcome.PatientId = patientDetails.ToList()[0].PatientId.ToString();
                        sjrOutcome.PatientName = patientDetails.ToList()[0].PatientName;
                        sjrOutcome.DOB = patientDetails.ToList()[0].DOB;
                    }
                    if (sjrOutcome.Patient_ID == 0 || sjrOutcome.Patient_ID == null)
                    {
                        sjrOutcome.Patient_ID = Convert.ToInt32(id);
                        sjrOutcome.AvoidabilityScoreID = -1;
                    }
                }
                catch (Exception ex)
                {
                    dBEngine.LogException(ex.Message, this.ToString(), "ValidateUser", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            List<RolePermission> permissions = dBEngine.GetRolePermission(0, 0, "", "", "",Convert.ToInt32(Session["LoginUserID"]), "SJR Outcome", Convert.ToString(Session["Role"]));
            bool isnoaccess = false;
            if (permissions.Count > 0)
            {
                ViewBag.IsReadOnly = permissions[0].IsReadOnly;
                isnoaccess = permissions[0].NoAccess;
            }
            else
            {
                ViewBag.IsReadOnly = true;
                isnoaccess = true;
            }
            if (isnoaccess == false)
            {
                return View(sjrOutcome);
            }
            else
            {
                return RedirectToAction("NotAuthorizedSJR", new { id = id });
            }
        }

        public ActionResult SJROutcomeView(int? id)
        {
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            if (Session["FullName"] != null)
            {
                if (string.IsNullOrEmpty(Session["FullName"].ToString()))
                    return RedirectToAction("Index", "Account");
            }
            else
                return RedirectToAction("Index", "Account");

            bool isUser = GetUserDetailsFromAD();
            clsSJROutcome sjrOutcome = new clsSJROutcome();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            ViewBag.SpecialityDDM = dBEngine.GetSpecialitiesForDropDown(Convert.ToInt32(Session["LoginUserID"]));
            try
            {
                if (id == null || id == 0)
                {
                    if (Session["PatientID"] != null)
                        id = Convert.ToInt32(Session["PatientID"]);
                    else
                        return RedirectToAction("Index", "Account");
                }
                //if (ApplicationSession.LoginUserID > 0)
                //{
                try
                {
                    if (Request.HttpMethod != "POST")
                    {
                        sjrOutcome = dBEngine.GetSJROutcome(id, Convert.ToInt32(Session["LoginUserID"]));

                        patientDetails = dBEngine.GetPatientDetails(id, Convert.ToInt32(Session["LoginUserID"]));
                        sjrOutcome.PatientId = patientDetails.ToList()[0].PatientId.ToString();
                        sjrOutcome.PatientName = patientDetails.ToList()[0].PatientName;
                        sjrOutcome.DOB = patientDetails.ToList()[0].DOB;
                    }
                    if (sjrOutcome.Patient_ID == 0 || sjrOutcome.Patient_ID == null)
                    {
                        sjrOutcome.Patient_ID = Convert.ToInt32(id);
                        sjrOutcome.AvoidabilityScoreID = -1;
                    }
                }
                catch (Exception ex)
                {
                    dBEngine.LogException(ex.Message, this.ToString(), "ValidateUser", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View(sjrOutcome);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formCollection"></param>
        /// <param name="btnSave"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SJROutcome(FormCollection formCollection, string btnSave, string btnFinish, int? id)
        {
            string actionName = "";
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            bool IsFinished = false;
            DBEngine dBEngine = new DBEngine(connectionString);
            try
            {
                if (id == null || id == 0)
                {
                    if (Session["PatientID"] != null)
                        id = Convert.ToInt32(Session["PatientID"]);
                    else
                        return RedirectToAction("Index", "Account");
                }
                int patientid = Convert.ToInt32(id);
                if (formCollection["Stage2SJRRequired"] == "Yes") formCollection["Stage2SJRRequired"] = "true"; else formCollection["Stage2SJRRequired"] = "false";
                if (formCollection["MSGRequired"] == "Yes") formCollection["MSGRequired"] = "true"; else formCollection["MSGRequired"] = "false";
                if (formCollection["RandomSampleReview"] == "Yes") formCollection["RandomSampleReview"] = "true"; else formCollection["RandomSampleReview"] = "false";
                if (formCollection["Stage2SJRDateSent"] == "") formCollection["Stage2SJRDateSent"] = "";
                if (formCollection["DateReceived"] == "") formCollection["DateReceived"] = "";
                if (formCollection["MSGDiscussionDate"] == "") formCollection["MSGDiscussionDate"] = "";
                if (formCollection["AvoidabilityScoreID"] == "") formCollection["AvoidabilityScoreID"] = "0";
                if (formCollection["ReviewCompleted"] == "on") { formCollection["ReviewCompleted"] = "true"; } else formCollection["ReviewCompleted"] = "false";

                if (btnSave != null)
                {
                    IsFinished = false;
                    int retVal = dBEngine.UpdateSJROutcome(Convert.ToBoolean(formCollection["Stage2SJRRequired"]), formCollection["Stage2SJRDateSent"],
                    formCollection["Stage2SJRSentTo"], formCollection["ReferenceNumber"], formCollection["DateReceived"], formCollection["SIRIComments"],
                    Convert.ToBoolean(formCollection["MSGRequired"]), formCollection["MSGDiscussionDate"], Convert.ToInt32(formCollection["AvoidabilityScoreID"]),
                    formCollection["Comments"], formCollection["FeedbackToNoK"], Convert.ToInt32(formCollection["SpecialityID"]), Convert.ToBoolean(formCollection["ReviewCompleted"]), formCollection["DateSJR1Requested"], formCollection["SJR1RequestSentTo"], Convert.ToBoolean(formCollection["RandomSampleReview"]), id, Convert.ToInt32(Session["LoginUserID"]), IsFinished);
                    if (Convert.ToBoolean(formCollection["Stage2SJRRequired"]) && Convert.ToBoolean(formCollection["ReviewCompleted"]))
                    {
                        List<clsPatientDetails> patient = dBEngine.GetPatientDetailsByID(id, Convert.ToInt32(Session["LoginUserID"]));
                        if (patient.Count > 0)
                        {
                            int dbReturn = dBEngine.UpdateFullSJR2Details(patient[0].PatientId, patient[0].PatientName, DateTime.ParseExact(patient[0].DateofDeath, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), System.DateTime.Now, Convert.ToString(Session["FullName"]), Convert.ToInt32(Session["LoginUserID"]));

                            List<NotificationSettings> settings = dBEngine.GetNotificationSettingsByTrigger("Stage 2 SJR Required", Convert.ToInt32(Session["LoginUserID"]));
                            if (settings.Count > 0)
                            {
                                for (int counter = 0; counter < settings.Count; counter++)
                                {
                                    List<AppUsers> users = dBEngine.GetUsersByRoleID(settings[counter].RoleID, Convert.ToInt32(Session["LoginUserID"]));
                                    PatientNotification notification = dBEngine.GetNotificationSentStatusByPatientID(patientid, "Stage 2 SJR Required", Convert.ToInt32(Session["LoginUserID"]));
                                    if (notification.NotificationTrigger == null)
                                    {
                                        for (int count = 0; count < users.Count; count++)
                                        {
                                            SendEmail(users[count].EmailID, "CORS - Stage 2 SJR Required", settings[counter].EmailTemplate.Replace("##PatientID##", patient[0].PatientId).Replace("##PatientName##", patient[0].PatientName).Replace("##DateOfDeath##", patient[0].DateofDeath).Replace("##RequestedDate##", formCollection["Stage2SJRDateSent"]).Replace("##RequestedBy##", Convert.ToString(Session["FullName"])));
                                        }                                        
                                    }
                                }
                                dBEngine.UpdatePatientNotification(patientid, "Stage 2 SJR Required", Convert.ToInt32(Session["LoginUserID"]));
                            }
                        }
                    }
                    actionName = "MortalityReview";
                }
                if (btnFinish != null)
                {
                    IsFinished = true;
                    int retVal = dBEngine.UpdateSJROutcome(Convert.ToBoolean(formCollection["Stage2SJRRequired"]), formCollection["Stage2SJRDateSent"],
                    formCollection["Stage2SJRSentTo"], formCollection["ReferenceNumber"], formCollection["DateReceived"], formCollection["SIRIComments"],
                    Convert.ToBoolean(formCollection["MSGRequired"]), formCollection["MSGDiscussionDate"], Convert.ToInt32(formCollection["AvoidabilityScoreID"]),
                    formCollection["Comments"], formCollection["FeedbackToNoK"], Convert.ToInt32(formCollection["SpecialityID"]), Convert.ToBoolean(formCollection["ReviewCompleted"]), formCollection["DateSJR1Requested"], formCollection["SJR1RequestSentTo"], Convert.ToBoolean(formCollection["RandomSampleReview"]), id, Convert.ToInt32(Session["LoginUserID"]), IsFinished);
                    if (Convert.ToBoolean(formCollection["Stage2SJRRequired"]) && Convert.ToBoolean(formCollection["ReviewCompleted"]))
                    {
                        List<clsPatientDetails> patient = dBEngine.GetPatientDetailsByID(id, Convert.ToInt32(Session["LoginUserID"]));
                        if (patient.Count > 0)
                        {
                            int dbReturn = dBEngine.UpdateFullSJR2Details(patient[0].PatientId, patient[0].PatientName, DateTime.ParseExact(patient[0].DateofDeath, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), System.DateTime.Now, Convert.ToString(Session["FullName"]), Convert.ToInt32(Session["LoginUserID"]));

                            List<NotificationSettings> settings = dBEngine.GetNotificationSettingsByTrigger("Stage 2 SJR Required", Convert.ToInt32(Session["LoginUserID"]));
                            if (settings.Count > 0)
                            {
                                for (int counter = 0; counter < settings.Count; counter++)
                                {
                                    List<AppUsers> users = dBEngine.GetUsersByRoleID(settings[counter].RoleID, Convert.ToInt32(Session["LoginUserID"]));
                                    PatientNotification notification = dBEngine.GetNotificationSentStatusByPatientID(patientid, "Stage 2 SJR Required", Convert.ToInt32(Session["LoginUserID"]));
                                    if (notification.NotificationTrigger == null)
                                    {
                                        for (int count = 0; count < users.Count; count++)
                                        {
                                            SendEmail(users[count].EmailID, "CORS - Stage 2 SJR Required", settings[counter].EmailTemplate.Replace("##PatientID##", patient[0].PatientId).Replace("##PatientName##", patient[0].PatientName).Replace("##DateOfDeath##", patient[0].DateofDeath).Replace("##RequestedDate##", formCollection["Stage2SJRDateSent"]).Replace("##RequestedBy##", Convert.ToString(Session["FullName"])));
                                        }
                                        dBEngine.UpdatePatientNotification(patientid, "Stage 2 SJR Required", Convert.ToInt32(Session["LoginUserID"]));
                                    }
                                }
                            }
                        }
                    }
                    actionName = "MortalityReview";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return RedirectToAction(actionName, new { id = id });
        }

        public static void SendEmail(string ReceiverEmailID, string EmailSubject, string MessageBody, string application = "")
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);

            WebsiteSetting setting = dBEngine.GetWebsiteSettings(0);

            #region Email
            string SMTPCLIENT = setting.SMTPServer;
            string EMAILID = setting.EmailID;
            string PASSWORD = setting.Password;
            int PORT = setting.Port;
            bool ENABLESSL = false;
            string EMAIL_DISPLAYNAME = "CORS";
            #endregion


            try
            {

                MailMessage mail = new MailMessage();
                string smtp = SMTPCLIENT; //ServerSettings.SMTPCLIENT;

                SmtpClient SmtpServer = new SmtpClient(SMTPCLIENT);//new SmtpClient(ServerSettings.SMTPCLIENT);

                //mail.From = new MailAddress(ServerSettings.EMAILID, ServerSettings.EMAIL_DISPLAYNAME);
                mail.From = new MailAddress(EMAILID, EMAIL_DISPLAYNAME);
                mail.To.Add(ReceiverEmailID);

                mail.Subject = HttpUtility.HtmlDecode(EmailSubject);
                String emailBody = MessageBody;
                mail.Body = HttpUtility.HtmlDecode(emailBody);


                mail.IsBodyHtml = true;
                //SmtpServer.Port = ServerSettings.PORT;
                SmtpServer.Port = PORT;
                //SmtpServer.Credentials = new System.Net.NetworkCredential(ServerSettings.EMAILID, ServerSettings.PASSWORD);
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new System.Net.NetworkCredential(EMAILID, PASSWORD);
                //SmtpServer.EnableSsl = ServerSettings.ENABLESSL;
                SmtpServer.EnableSsl = ENABLESSL;

                SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                dBEngine.LogException(ex.Message, "HomeController", "SendEmail", System.DateTime.Now, 0);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Stage2SJRformFirstStep(int? id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            //dBEngine.LogException("Inside SJR1", "", "", System.DateTime.Now);
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            if (Session["FullName"] != null)
            {
                if (string.IsNullOrEmpty(Session["FullName"].ToString()))
                    return RedirectToAction("Index", "Account");
            }
            else
                return RedirectToAction("Index", "Account");
            bool isUser = GetUserDetailsFromAD();
            clsSJRFormInitial medicalExaminerReview = new clsSJRFormInitial();
            //dBEngine.LogException("before try", "", "", System.DateTime.Now);
            try
            {
                if (id == null || id == 0)
                {
                    if (Session["PatientID"] != null)
                        id = Convert.ToInt32(Session["PatientID"]);
                    else
                        return RedirectToAction("Index", "Account");
                }
                //if (ApplicationSession.LoginUserID > 0)
                //{
                try
                {
                    if (Request.HttpMethod != "POST")
                    {
                        medicalExaminerReview = dBEngine.GetSJRFormInitial(id, Convert.ToInt32(Session["LoginUserID"]));
                        clsSJRFormProblemType problemtype = dBEngine.GetSJRProblemType(id, Convert.ToInt32(Session["LoginUserID"]));
                        if (medicalExaminerReview.SJR1 == 3)
                            medicalExaminerReview.CreatedBy = problemtype.CreatedBy;
                        if (medicalExaminerReview.CreatedBy == null) medicalExaminerReview.CreatedBy = "";
                        //dBEngine.LogException("after db method call", "", "", System.DateTime.Now);
                        patientDetails = dBEngine.GetPatientDetails(id, Convert.ToInt32(Session["LoginUserID"]));
                        medicalExaminerReview.PatientId = patientDetails.ToList()[0].PatientId.ToString();
                        medicalExaminerReview.PatientName = patientDetails.ToList()[0].PatientName;
                        medicalExaminerReview.DOB = patientDetails.ToList()[0].DOB;
                    }
                    if (medicalExaminerReview.Patient_ID == 0 || medicalExaminerReview.Patient_ID == null)
                        medicalExaminerReview.Patient_ID = Convert.ToInt32(id);
                    ViewBag.ExcellentRatingID = dBEngine.GetRatingIDByName("Excellent", Convert.ToInt32(Session["LoginUserID"]));
                    ViewBag.GoodRatingID = dBEngine.GetRatingIDByName("Good", Convert.ToInt32(Session["LoginUserID"]));
                    ViewBag.AdequateRatingID = dBEngine.GetRatingIDByName("Adequate", Convert.ToInt32(Session["LoginUserID"]));
                    ViewBag.PoorRatingID = dBEngine.GetRatingIDByName("Poor", Convert.ToInt32(Session["LoginUserID"]));
                    ViewBag.VeryPoorRatingID = dBEngine.GetRatingIDByName("Very Poor", Convert.ToInt32(Session["LoginUserID"]));
                }
                catch (Exception ex)
                {
                    dBEngine.LogException(ex.Message, this.ToString(), "ValidateUser", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            List<RolePermission> permissions = dBEngine.GetRolePermission(0, 0, "", "", "",Convert.ToInt32(Session["LoginUserID"]), "SJR1", Convert.ToString(Session["Role"]));
            bool isnoaccess = false;
           
            if (permissions.Count > 0)
            {
                ViewBag.IsReadOnly = permissions[0].IsReadOnly;
                isnoaccess = permissions[0].NoAccess;
            }
            else
            {
                ViewBag.IsReadOnly = true;
                isnoaccess = true;
            }
            if (isnoaccess == false)
            {
                //dBEngine.LogException("Inside isnoaccess false", "", "", System.DateTime.Now);
                return View(medicalExaminerReview);
            }
            else
            {
                //dBEngine.LogException("Inside isnoaccess true", "", "", System.DateTime.Now);
                return RedirectToAction("NotAuthorizedSJR", new { id = id });
            }            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Stage2SJRFormSecondStep(int? id)
        {
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            if (Session["FullName"] != null)
            {
                if (string.IsNullOrEmpty(Session["FullName"].ToString()))
                    return RedirectToAction("Index", "Account");
            }
            else
                return RedirectToAction("Index", "Account");

            clsSJRFormProblemType sjrProblemType = new clsSJRFormProblemType();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            NHS.Common.clsSJRFormInitial sjr1 = dBEngine.GetSJRFormInitial(id, Convert.ToInt32(Session["LoginUserID"]));
            ViewBag.InitialManagement = sjr1.InitialManagement;
            ViewBag.OngoingCare = sjr1.OngoingCare;
            ViewBag.CareDuringProcedure = sjr1.CareDuringProcedure;
            ViewBag.EndLifeCare = sjr1.EndLifeCare;
            ViewBag.OverAllAssessment = sjr1.OverAllAssessment;
            ViewBag.SpecialityDDM = dBEngine.GetSpecialitiesForDropDown(Convert.ToInt32(Session["LoginUserID"]));
            try
            {
                if (id == null || id == 0)
                {
                    if (Session["PatientID"] != null)
                        id = Convert.ToInt32(Session["PatientID"]);
                    else
                        return RedirectToAction("Index", "Account");
                }

                try
                {
                    if (Request.HttpMethod != "POST")
                    {
                        sjrProblemType = dBEngine.GetSJRProblemType(id, Convert.ToInt32(Session["LoginUserID"]));

                        patientDetails = dBEngine.GetPatientDetails(id, Convert.ToInt32(Session["LoginUserID"]));
                        sjrProblemType.PatientId = patientDetails.ToList()[0].PatientId.ToString();
                        sjrProblemType.PatientName = patientDetails.ToList()[0].PatientName;
                        sjrProblemType.DOB = patientDetails.ToList()[0].DOB;
                    }
                    if (sjrProblemType.Patient_ID == 0 || sjrProblemType.Patient_ID == null)
                        sjrProblemType.Patient_ID = Convert.ToInt32(id);
                }
                catch (Exception ex)
                {
                    dBEngine.LogException(ex.Message, this.ToString(), "ValidateUser", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            List<RolePermission> permissions = dBEngine.GetRolePermission(0, 0, "", "", "", Convert.ToInt32(Session["LoginUserID"]), "SJR1", Convert.ToString(Session["Role"]));
            bool isnoaccess = false;
            if (permissions.Count > 0)
            {
                ViewBag.IsReadOnly = permissions[0].IsReadOnly;
                isnoaccess = permissions[0].NoAccess;
            }
            else
            {
                ViewBag.IsReadOnly = true;
                isnoaccess = true;
            }
            if (isnoaccess == false)
            {
                return View(sjrProblemType);
            }
            else
            {
                return RedirectToAction("NotAuthorizedSJR", new { id = id });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Stage3SJRFormSecondStep(int? id)
        {
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            if (Session["FullName"] != null)
            {
                if (string.IsNullOrEmpty(Session["FullName"].ToString()))
                    return RedirectToAction("Index", "Account");
            }
            else
                return RedirectToAction("Index", "Account");

            bool isUser = GetUserDetailsFromAD();
            clsSJRFormProblemType sjrProblemType = new clsSJRFormProblemType();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            NHS.Common.clsSJRFormInitial sjr1 = dBEngine.GetSJR2FormInitial(id, Convert.ToInt32(Session["LoginUserID"]));
            ViewBag.InitialManagement = sjr1.InitialManagement;
            ViewBag.OngoingCare = sjr1.OngoingCare;
            ViewBag.CareDuringProcedure = sjr1.CareDuringProcedure;
            ViewBag.EndLifeCare = sjr1.EndLifeCare;
            ViewBag.OverAllAssessment = sjr1.OverAllAssessment;
            ViewBag.SpecialityDDM = dBEngine.GetSpecialitiesForDropDown(Convert.ToInt32(Session["LoginUserID"]));
            try
            {
                if (id == null || id == 0)
                {
                    if (Session["PatientID"] != null)
                        id = Convert.ToInt32(Session["PatientID"]);
                    else
                        return RedirectToAction("Index", "Account");
                }

                try
                {
                    if (Request.HttpMethod != "POST")
                    {
                        sjrProblemType = dBEngine.GetSJR2ProblemType(id, Convert.ToInt32(Session["LoginUserID"]));

                        patientDetails = dBEngine.GetPatientDetails(id, Convert.ToInt32(Session["LoginUserID"]));
                        sjrProblemType.PatientId = patientDetails.ToList()[0].PatientId.ToString();
                        sjrProblemType.PatientName = patientDetails.ToList()[0].PatientName;
                        sjrProblemType.DOB = patientDetails.ToList()[0].DOB;
                    }
                    if (sjrProblemType.Patient_ID == 0 || sjrProblemType.Patient_ID == null)
                        sjrProblemType.Patient_ID = Convert.ToInt32(id);
                }
                catch (Exception ex)
                {
                    dBEngine.LogException(ex.Message, this.ToString(), "ValidateUser", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            List<RolePermission> permissions = dBEngine.GetRolePermission(0, 0, "", "", "", Convert.ToInt32(Session["LoginUserID"]), "SJR2", Convert.ToString(Session["Role"]));
            bool isnoaccess = false;
            if (permissions.Count > 0)
            {
                ViewBag.IsReadOnly = permissions[0].IsReadOnly;
                isnoaccess = permissions[0].NoAccess;
            }
            else
            {
                ViewBag.IsReadOnly = true;
                isnoaccess = true;
            }
            if (isnoaccess == false)
            {
                return View(sjrProblemType);
            }
            else
            {
                return RedirectToAction("NotAuthorizedSJR", new { id = id });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formCollection"></param>
        /// <param name="BtnNext"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Stage2SJRformFirstStep(FormCollection formCollection, string BtnNext, int? id)
        {
            string actionName = "";
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);

            try
            {
                if (id == null || id == 0)
                {
                    if (Session["PatientID"] != null)
                        id = Convert.ToInt32(Session["PatientID"]);
                    else
                        return RedirectToAction("Index", "Account");
                }
                int retVal = dBEngine.UpdateSJRFormInitial(formCollection["InitialManagement"], formCollection["hdfInitialManagementCareRating"],
                    formCollection["OngoingCare"], formCollection["hdfOnGoingCareRating"], formCollection["CareDuringProcedure"], formCollection["hdfCareDuringProcedureCareRating"],
                    formCollection["EndLifeCare"], formCollection["hdfEndLifeCareRating"], formCollection["OverAllAssessment"], formCollection["hdfOverAllAssessmentCareRating"],
                    formCollection["hdfQualityDocumentationCareRating"], id, Convert.ToInt32(Session["LoginUserID"]));
                actionName = "Stage2SJRFormSecondStep";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return RedirectToAction(actionName, new { id = id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formCollection"></param>
        /// <param name="BtnPrevious"></param>
        /// <param name="BtnFinish"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Stage2SJRformSecondStep(FormCollection formCollection, string BtnPrevious, string BtnSave, string BtnFinish, int? id)
        {
            string actionName = "";
            bool IsFinish = false;
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            try
            {
                int PID = Convert.ToInt32(Session["PId"]);
                if (BtnPrevious != null)
                    actionName = "Stage2SJRformFirstStep";
                if (id == null || id == 0)
                {
                    if (Session["PatientID"] != null)
                        id = Convert.ToInt32(Session["PatientID"]);
                    else
                        return RedirectToAction("Index", "Account");
                }
                if (BtnSave != null)
                {
                    IsFinish = false;
                    if (formCollection["hdfProblemOccured"] == "Yes") formCollection["hdfProblemOccured"] = "true"; else formCollection["hdfProblemOccured"] = "false";
                    if (formCollection["hdfReviewCompleted"] == "on") formCollection["hdfReviewCompleted"] = "true"; else formCollection["hdfReviewCompleted"] = "false";
                    int retVal = dBEngine.UpdateSJR1ProblemType(Convert.ToInt32(formCollection["AssessmentResponseID"]), Convert.ToInt32(formCollection["AssessmentCarePhaseID"]),
                        Convert.ToInt32(formCollection["MedicationResponseID"]), Convert.ToInt32(formCollection["MedicationCarePhaseID"]), Convert.ToInt32(formCollection["TreatmentResponseID"]),
                        Convert.ToInt32(formCollection["TreatmentCarePhaseID"]), Convert.ToInt32(formCollection["InfectionResponseID"]), Convert.ToInt32(formCollection["InfectionCarePhaseID"]),
                        Convert.ToInt32(formCollection["ProcedureResponseID"]), Convert.ToInt32(formCollection["ProcedureCarePhaseID"]),
                        Convert.ToInt32(formCollection["MonitoringResponseID"]), Convert.ToInt32(formCollection["ResuscitationResponseID"]),
                        Convert.ToInt32(formCollection["OthertypeResponseID"]), Convert.ToInt32(formCollection["OthertypeCarePhaseID"]), Convert.ToInt32(formCollection["AvoidabilityScoreID"]),
                        formCollection["Comments"], formCollection["SIRIComments"], Convert.ToBoolean(formCollection["hdfProblemOccured"]), Convert.ToBoolean(formCollection["hdfReviewCompleted"]), Convert.ToInt32(formCollection["SpecialityID"]), id, Convert.ToInt32(Session["LoginUserID"]), IsFinish);
                    actionName = "MortalityReview";
                }



                if (BtnFinish != null)
                {
                    IsFinish = true;
                    if (formCollection["hdfProblemOccured"] == "Yes") formCollection["hdfProblemOccured"] = "true"; else formCollection["hdfProblemOccured"] = "false";
                    if (formCollection["hdfReviewCompleted"] == "on") formCollection["hdfReviewCompleted"] = "true"; else formCollection["hdfReviewCompleted"] = "false";
                    int retVal = dBEngine.UpdateSJR1ProblemType(Convert.ToInt32(formCollection["AssessmentResponseID"]), Convert.ToInt32(formCollection["AssessmentCarePhaseID"]),
                        Convert.ToInt32(formCollection["MedicationResponseID"]), Convert.ToInt32(formCollection["MedicationCarePhaseID"]), Convert.ToInt32(formCollection["TreatmentResponseID"]),
                        Convert.ToInt32(formCollection["TreatmentCarePhaseID"]), Convert.ToInt32(formCollection["InfectionResponseID"]), Convert.ToInt32(formCollection["InfectionCarePhaseID"]),
                        Convert.ToInt32(formCollection["ProcedureResponseID"]), Convert.ToInt32(formCollection["ProcedureCarePhaseID"]),
                        Convert.ToInt32(formCollection["MonitoringResponseID"]), Convert.ToInt32(formCollection["ResuscitationResponseID"]),
                        Convert.ToInt32(formCollection["OthertypeResponseID"]), Convert.ToInt32(formCollection["OthertypeCarePhaseID"]), Convert.ToInt32(formCollection["AvoidabilityScoreID"]),
                        formCollection["Comments"], formCollection["SIRIComments"], Convert.ToBoolean(formCollection["hdfProblemOccured"]), Convert.ToBoolean(formCollection["hdfReviewCompleted"]), Convert.ToInt32(formCollection["SpecialityID"]), id, Convert.ToInt32(Session["LoginUserID"]), IsFinish);
                    actionName = "MortalityReview";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return RedirectToAction(actionName, new { id = id });
        }

        public ActionResult Setting()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            WebsiteSetting setting = dBEngine.GetWebsiteSettings(Convert.ToInt32(Session["LoginUserID"]));
            return View(setting);
        }

        [HttpPost]
        public ActionResult NoKFeedback(FormCollection feedback, int? id)
        {
            if (Session["FullName"] != null)
            {
                if (string.IsNullOrEmpty(Session["FullName"].ToString()))
                    return RedirectToAction("Index", "Account");
            }
            else
                return RedirectToAction("Index", "Account");
            if (id == null || id == 0)
            {
                if (Session["PatientID"] != null)
                    id = Convert.ToInt32(Session["PatientID"]);
                else
                    return RedirectToAction("Index", "Account");
            }
            
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            NoKFeedback feedback1 = dBEngine.GetNoKFeedback(id, Convert.ToInt32(Session["LoginUserID"]));
            bool isnokconcern = false;
            bool isnokcompleted = false;
            if (Request.Form["NOK"] == "on") { isnokcompleted = true; } else isnokcompleted = false;
            if (Request.Form["NoKConcerns"] == "Yes") isnokconcern = true; else isnokconcern = false;
            int dbReturnValue = dBEngine.UpdateNoKFeedback(isnokconcern, Request.Form["NokConcernComments"], Request.Form["InitialLetterDate"], Request.Form["HoldingLetterDate"], Request.Form["HoldingLetterComments"], Convert.ToBoolean(Request.Form["SuboptimalCareIdentified"]), Request.Form["FollowUpCallDate"], Request.Form["FollowUpLetterSent"], Request.Form["FollowLetterComments"], isnokcompleted,  id, Convert.ToInt32(Session["LoginUserID"]));
            return RedirectToAction("MortalityReview", new { id = id });
        }


        [HttpPost]
        public ActionResult WebsiteSetting()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            int dbReturnValue = dBEngine.UpdateWebsiteSetting(Request.Form["EmailID"], Request.Form["Password"], Request.Form["DomainName"], Request.Form["SMTPServer"], Convert.ToInt32(Request.Form["Port"]), Convert.ToInt32(Session["LoginUserID"]));
            return RedirectToAction("Setting");
        }

        public ActionResult Roles()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            Session["FullName"] = Session["FirstName"] + " " + Session["LastName"];
            List<NHS.Common.Roles> setting = dBEngine.GetRoles(Convert.ToInt32(Session["LoginUserID"]));
            return View(setting);
        }

        public ActionResult Permission()
        {
            return View();
        }

        public ActionResult NoK(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            NoKFeedback feedback = dBEngine.GetNoKFeedback(id, Convert.ToInt32(Session["LoginUserID"]));
            Session["PatientID"] = id;
            List<RolePermission> permissions = dBEngine.GetRolePermission(0, 0, "", "", "", Convert.ToInt32(Session["LoginUserID"]), "NoK Feedback", Convert.ToString(Session["Role"]));
            bool isnoaccess = false;
            if (permissions.Count > 0)
            {
                ViewBag.IsReadOnly = permissions[0].IsReadOnly;
                isnoaccess = permissions[0].NoAccess;
            }
            else
            {
                ViewBag.IsReadOnly = true;
                isnoaccess = true;
            }
            if (isnoaccess == false)
            {
                return View(feedback);
            }
            else
            {
                return RedirectToAction("NotAuthorizedSJR", new { id = id });
            }
        }

        public ActionResult AddFeedbackType()
        {
            return View();
        }

        public ActionResult AddRole()
        {
            return View();
        }

        public ActionResult AddConsultant()
        {
            return View();
        }

        public ActionResult AddNotificationSettings()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            ViewBag.RoleDDM = dBEngine.GetRoles(Convert.ToInt32(Session["LoginUserID"]), true);
            ViewBag.NotificationTriggerDDM = dBEngine.GetNotificationTriggers(Convert.ToInt32(Session["LoginUserID"]));
            return View();
        }

        public ActionResult AddWard()
        {
            return View();
        }

        public ActionResult AddPermission()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            ViewBag.RoleDDM = dBEngine.GetRoles(Convert.ToInt32(Session["LoginUserID"]), true);
            ViewBag.ModuleDDM = dBEngine.GetModules(Convert.ToInt32(Session["LoginUserID"]));
            return View();
        }

        public ActionResult AddCommentType()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateSpeciality()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            int dbReturn = dBEngine.CreateSpeciality(Request.Form["SpecialityCode"],Request.Form["SpecialityName"], Request.Form["CareGroup"],  Convert.ToInt32(Session["LoginUserID"]));
            return RedirectToAction("Speciality");
        }

        [HttpPost]
        public ActionResult CreateWard()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            int dbReturn = dBEngine.CreateWard(Request.Form["WardCode"], Request.Form["WardName"], Convert.ToInt32(Session["LoginUserID"]));
            return RedirectToAction("Wards");
        }

        [HttpPost]
        public ActionResult CreatePermission()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            bool IsFullAccess = false;
            bool IsReadOnly = false;
            bool NoAccess = false;
            DBEngine dBEngine = new DBEngine(connectionString);
            if (Request.Form["Access"] == "1") { IsFullAccess = true; IsReadOnly = false; NoAccess = false; }
            else if (Request.Form["Access"] == "2") { IsFullAccess = false; IsReadOnly = true; NoAccess = false; }
            else if (Request.Form["Access"] == "3") { IsFullAccess = false; IsReadOnly = false; NoAccess = true; }
            else { IsFullAccess = false; IsReadOnly = false; NoAccess = true; }

            int dbReturn = dBEngine.CreatePermission(Convert.ToInt32(Request.Form["ddlModule"]), Convert.ToInt32(Request.Form["ddlRole"]), IsFullAccess, IsReadOnly, NoAccess, Convert.ToInt32(Session["LoginUserID"]));
            return RedirectToAction("RolePermission");
        }

        [HttpPost]
        public ActionResult CreateRole()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            int dbReturn = dBEngine.CreateRole(Request.Form["RoleName"], Convert.ToInt32(Session["LoginUserID"]));
            return RedirectToAction("Roles");
        }

        [HttpPost]
        public ActionResult CreateFeedbackType()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            int dbReturn = dBEngine.CreateFeedbackType(Request.Form["Type"], Convert.ToInt32(Session["LoginUserID"]));
            return RedirectToAction("FeedbackType");
        }

        [HttpPost]
        public ActionResult CreateCommentType()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            int dbReturn = dBEngine.CreateCommentType(Request.Form["Type"], Convert.ToInt32(Session["LoginUserID"]));
            return RedirectToAction("CommentType");
        }

        [HttpPost]
        public ActionResult CreateNotificationSettings()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            int dbReturn = dBEngine.CreateNotificationSettings(Convert.ToInt32(Request.Form["ddlNotification"]), Convert.ToInt32(Request.Form["ddlRole"]), Convert.ToInt32(Session["LoginUserID"]));
            return RedirectToAction("NotificationSettings");
        }

        [HttpPost]
        public ActionResult CreateConsultant()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            int dbReturn = dBEngine.CreateConsultant(Request.Form["GMCCode"], Request.Form["FirstName"], Request.Form["LastName"], Request.Form["EmailID"], Convert.ToInt32(Session["LoginUserID"]));
            return RedirectToAction("Consultants");
        }

        [HttpPost]
        public JsonResult IsEmailExists(string Email)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            int consid = dBEngine.IsEmailExists(Email, Convert.ToInt32(Session["LoginUserID"]));
            if(consid == 0)
                return Json("SUCCESS", JsonRequestBehavior.AllowGet);
            else
                return Json("ERROR", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult IsUserNameExists(string UserName)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            int consid = dBEngine.IsUserNameExists(UserName, Convert.ToInt32(Session["LoginUserID"]));
            if (consid == 0)
                return Json("SUCCESS", JsonRequestBehavior.AllowGet);
            else
                return Json("ERROR", JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditFeedbackType(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            CommentType feedback = dBEngine.GetFeedbackTypeByID(id, Convert.ToInt32(Session["LoginUserID"]));
            feedback.CommonTypeID = id;
            return View(feedback);
        }

        public ActionResult EditNotificationSettings(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            NotificationSettings feedback = dBEngine.GetNotificationSettingsByID(id, Convert.ToInt32(Session["LoginUserID"]));
            ViewBag.RoleDDM = dBEngine.GetRoles(Convert.ToInt32(Session["LoginUserID"]), true);
            ViewBag.NotificationTriggerDDM = dBEngine.GetNotificationTriggers(Convert.ToInt32(Session["LoginUserID"]));
            feedback.ID = id;
            return View(feedback);
        }

        public ActionResult EditWard(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            Wards ward = dBEngine.GetWardByID(id, Convert.ToInt32(Session["LoginUserID"]));
            //feedback.CommonTypeID = id;
            return View(ward);
        }

        public ActionResult EditPermission(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            ViewBag.RoleDDM = dBEngine.GetRoles(Convert.ToInt32(Session["LoginUserID"]), true);
            ViewBag.ModuleDDM = dBEngine.GetModules(Convert.ToInt32(Session["LoginUserID"]));
            RolePermission ward = dBEngine.GetPermissionByID(id, Convert.ToInt32(Session["LoginUserID"]));
            //feedback.CommonTypeID = id;
            return View(ward);
        }

        public ActionResult EditUser(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            ViewBag.RoleDDM = dBEngine.GetRoles(Convert.ToInt32(Session["LoginUserID"]), true);
            ViewBag.SpecialityDDM = dBEngine.GetSpecialitiesForDropDown(Convert.ToInt32(Session["LoginUserID"]));
            NHS.Common.AppUsers feedback = dBEngine.GetUserByID(id, Convert.ToInt32(Session["LoginUserID"]));
            feedback.ID = id;
            return View(feedback);
        }

        public ActionResult EditRole(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            NHS.Common.Roles feedback = dBEngine.GetRoleByID(id, Convert.ToInt32(Session["LoginUserID"]));
            feedback.ID = id;
            return View(feedback);
        }

        public ActionResult EditConsultant(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            NHS.Common.Consultant feedback = dBEngine.GetCORSConsultantsByID(id, Convert.ToInt32(Session["LoginUserID"]));
            feedback.ConsID = id;
            return View(feedback);
        }

        public ActionResult EditSpeciality(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            Specialities feedback = dBEngine.GetSpecialitybyID(id, Convert.ToInt32(Session["LoginUserID"]));
            feedback.SpecialityID = id;
            return View(feedback);
        }

        public ActionResult EditCommentType(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            CommentType feedback = dBEngine.GetCommentTypeByID(id, Convert.ToInt32(Session["LoginUserID"]));
            feedback.CommonTypeID = id;
            return View(feedback);
        }

        [HttpPost]
        public ActionResult UpdateFeedbackType(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            int dbReturn = dBEngine.UpdateFeedbackType(Request.Form["Type"], id, Convert.ToInt32(Session["LoginUserID"]));
            return RedirectToAction("FeedbackType");
        }

        [HttpPost]
        public ActionResult UpdateWard(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            int dbReturn = dBEngine.UpdateWard(Request.Form["WardCode"], Request.Form["WardName"], id, Convert.ToInt32(Session["LoginUserID"]));
            return RedirectToAction("Wards");
        }

        [HttpPost]
        public ActionResult UpdateNotificationSettings(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            int dbReturn = dBEngine.UpdateNotificationSettings(Convert.ToInt32(Request.Form["ddlNotification"]), Convert.ToInt32(Request.Form["ddlRole"]), id, Convert.ToInt32(Session["LoginUserID"]));
            return RedirectToAction("NotificationSettings");
        }

        [HttpPost]
        public ActionResult UpdatePermission(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            bool IsFullAccess = false;
            bool IsReadOnly = false;
            bool NoAccess = false;
            ViewBag.RoleDDM = dBEngine.GetRoles(Convert.ToInt32(Session["LoginUserID"]));
            ViewBag.ModuleDDM = dBEngine.GetModules(Convert.ToInt32(Session["LoginUserID"]));
            
            if(Request.Form["Access"] == "1") { IsFullAccess = true; IsReadOnly = false; NoAccess = false; }
            else if (Request.Form["Access"] == "2") { IsFullAccess = false; IsReadOnly = true; NoAccess = false; }
            else if (Request.Form["Access"] == "3") { IsFullAccess = false; IsReadOnly = false; NoAccess = true; }
            else { IsFullAccess = false; IsReadOnly = false; NoAccess = true; }

            int dbReturn = dBEngine.UpdatePermission(Convert.ToInt32(Request.Form["ddlModule"]), Convert.ToInt32(Request.Form["ddlRole"]), IsFullAccess, IsReadOnly, NoAccess, id, Convert.ToInt32(Session["LoginUserID"]));
            return RedirectToAction("RolePermission");
        }

        [HttpPost]
        public ActionResult UpdateConsultant(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            int dbReturn = dBEngine.UpdateConsultant(Request.Form["GMCCode"], Request.Form["FirstName"], Request.Form["LastName"],
                Request.Form["EmailID"], id, Convert.ToInt32(Session["LoginUserID"]));
            return RedirectToAction("Consultants");
        }

        [HttpPost]
        public ActionResult UpdateUser(int id)
        {
            string approved = "";
            if (Request.Form["Approved"] == "on") approved = "true"; else approved = "false";
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            int dbReturn = dBEngine.UpdateUser(id, Request.Form["EmailID"], Request.Form["FirstName"], Request.Form["LastName"],
                Request.Form["UserName"], Request.Form["Code"], Request.Form["ddlSpeciality"], Convert.ToInt32(Request.Form["ddlRole"]), Convert.ToBoolean(approved), Convert.ToInt32(Session["LoginUserID"]));
            return RedirectToAction("Users");
        }

        [HttpPost]
        public ActionResult UpdateSpeciality(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            int dbReturn = dBEngine.UpdateSpeciality(Request.Form["SpecialityCode"], Request.Form["SpecialityName"], Request.Form["CareGroup"], id, Convert.ToInt32(Session["LoginUserID"]));
            return RedirectToAction("Speciality");
        }

        [HttpPost]
        public ActionResult UpdateRole(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            int dbReturn = dBEngine.UpdateRole(Request.Form["RoleName"], id, Convert.ToInt32(Session["LoginUserID"]));
            return RedirectToAction("Roles");
        }

        [HttpPost]
        public ActionResult UpdateCommentType(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            int dbReturn = dBEngine.UpdateCommentType(Request.Form["Type"], id, Convert.ToInt32(Session["LoginUserID"]));
            return RedirectToAction("CommentType");
        }

        public ActionResult DeleteFeedbackType(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            int dbReturnValue = dBEngine.DeleteFeedbackType(id, Convert.ToInt32(Session["LoginUserID"]));
            return RedirectToAction("FeedbackType");
        }

        public ActionResult DeleteWard(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            int dbReturnValue = dBEngine.DeleteWard(id, Convert.ToInt32(Session["LoginUserID"]));
            return RedirectToAction("Wards");
        }

        public ActionResult DeletePermission(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            int dbReturnValue = dBEngine.DeletePermission(id, Convert.ToInt32(Session["LoginUserID"]));
            return RedirectToAction("RolePermission");
        }

        public ActionResult DeleteConsultant(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            int dbReturnValue = dBEngine.DeleteConsultant(id, Convert.ToInt32(Session["LoginUserID"]));
            return RedirectToAction("Consultants");
        }

        public ActionResult DeleteUser(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            int dbReturnValue = dBEngine.DeleteUser(id, Convert.ToInt32(Session["LoginUserID"]));
            return RedirectToAction("Users");
        }

        public ActionResult DeleteCommentType(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            int dbReturnValue = dBEngine.DeleteCommentType(id, Convert.ToInt32(Session["LoginUserID"]));
            return RedirectToAction("CommentType");
        }

        public ActionResult DeleteNotificationSettings(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            int dbReturnValue = dBEngine.DeleteNotificationSettings(id, Convert.ToInt32(Session["LoginUserID"]));
            return RedirectToAction("NotificationSettings");
        }

        public ActionResult DeleteRole(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            int dbReturnValue = dBEngine.DeleteRole(id, Convert.ToInt32(Session["LoginUserID"]));
            return RedirectToAction("Roles");
        }

        public ActionResult DeleteSpeciality(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            int dbReturnValue = dBEngine.DeleteSpeciality(id, Convert.ToInt32(Session["LoginUserID"]));
            return RedirectToAction("Speciality");
        }

        public ActionResult FeedbackType()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            Session["FullName"] = Session["FirstName"] + " " + Session["LastName"];
            List<CommentType> feedbacks = dBEngine.GetCommentType("Feedback", Convert.ToInt32(Session["LoginUserID"]));
            return View(feedbacks);
        }

        public ActionResult CommentType()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            Session["FullName"] = Session["FirstName"] + " " + Session["LastName"];
            List<CommentType> feedbacks = dBEngine.GetCommentType("MEScrutiny", Convert.ToInt32(Session["LoginUserID"]));
            return View(feedbacks);
        }

        public ActionResult Speciality()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            Session["FullName"] = Session["FirstName"] + " " + Session["LastName"];
            List<Specialities> feedbacks = dBEngine.GetSpecialitiesForDropDown(Convert.ToInt32(Session["LoginUserID"]));
            return View(feedbacks);
        }

        public ActionResult AddSpeciality()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Stage3SJRformFirstStep(int? id)
        {
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            if (Session["FullName"] != null)
            {
                if (string.IsNullOrEmpty(Session["FullName"].ToString()))
                    return RedirectToAction("Index", "Account");
            }
            else
                return RedirectToAction("Index", "Account");

            bool isUser = GetUserDetailsFromAD();
            clsSJRFormInitial medicalExaminerReview = new clsSJRFormInitial();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            ViewBag.SpecialityDDM = dBEngine.GetSpecialitiesForDropDown(Convert.ToInt32(Session["LoginUserID"]));
            clsSJRFormInitial sjr2 = dBEngine.GetSJR2FormInitial(id, Convert.ToInt32(Session["LoginUserID"]));
            ViewBag.InitialManagement = sjr2.InitialManagement;
            ViewBag.OngoingCare = sjr2.OngoingCare;
            ViewBag.CareDuringProcedure = sjr2.CareDuringProcedure;
            ViewBag.EndLifeCare = sjr2.EndLifeCare;
            ViewBag.OverAllAssessment = sjr2.OverAllAssessment;
            try
            {
                if (id == null || id == 0)
                {
                    if (Session["PatientID"] != null)
                        id = Convert.ToInt32(Session["PatientID"]);
                    else
                        return RedirectToAction("Index", "Account");
                }
                try
                {
                    if (Request.HttpMethod != "POST")
                    {
                        medicalExaminerReview = dBEngine.GetSJR2FormInitial(id, Convert.ToInt32(Session["LoginUserID"]));
                        clsSJRFormProblemType problemtype = dBEngine.GetSJR2ProblemType(id, Convert.ToInt32(Session["LoginUserID"]));
                        if (medicalExaminerReview.SJR2 == 3)
                            medicalExaminerReview.CreatedBy = problemtype.CreatedBy;
                        if (medicalExaminerReview.CreatedBy == null) medicalExaminerReview.CreatedBy = "";
                        patientDetails = dBEngine.GetPatientDetails(id, Convert.ToInt32(Session["LoginUserID"]));
                        medicalExaminerReview.PatientId = patientDetails.ToList()[0].PatientId.ToString();
                        medicalExaminerReview.PatientName = patientDetails.ToList()[0].PatientName;
                        medicalExaminerReview.DOB = patientDetails.ToList()[0].DOB;
                    }
                    if (medicalExaminerReview.Patient_ID == 0 || medicalExaminerReview.Patient_ID == null)
                        medicalExaminerReview.Patient_ID = Convert.ToInt32(id);
                    ViewBag.ExcellentRatingID = dBEngine.GetRatingIDByName("Excellent", Convert.ToInt32(Session["LoginUserID"]));
                    ViewBag.GoodRatingID = dBEngine.GetRatingIDByName("Good", Convert.ToInt32(Session["LoginUserID"]));
                    ViewBag.AdequateRatingID = dBEngine.GetRatingIDByName("Adequate", Convert.ToInt32(Session["LoginUserID"]));
                    ViewBag.PoorRatingID = dBEngine.GetRatingIDByName("Poor", Convert.ToInt32(Session["LoginUserID"]));
                    ViewBag.VeryPoorRatingID = dBEngine.GetRatingIDByName("Very Poor", Convert.ToInt32(Session["LoginUserID"]));
                }
                catch (Exception ex)
                {
                    dBEngine.LogException(ex.Message, this.ToString(), "ValidateUser", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            List<RolePermission> permissions = dBEngine.GetRolePermission(0, 0, "", "", "", Convert.ToInt32(Session["LoginUserID"]), "SJR2", Convert.ToString(Session["Role"]));
            bool isnoaccess = false;
            if (permissions.Count > 0)
            {
                ViewBag.IsReadOnly = permissions[0].IsReadOnly;
                isnoaccess = permissions[0].NoAccess;
            }
            else
            {
                ViewBag.IsReadOnly = true;
                isnoaccess = true;
            }
            if (isnoaccess == false)
            {
                return View(medicalExaminerReview);
            }
            else
            {
                return RedirectToAction("NotAuthorizedSJR", new { id = id });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formCollection"></param>
        /// <param name="BtnNext"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Stage3SJRformFirstStep(FormCollection formCollection, string BtnNext, int? id)
        {
            string actionName = "";
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            try
            {
                if (id == null || id == 0)
                {
                    if (Session["PatientID"] != null)
                        id = Convert.ToInt32(Session["PatientID"]);
                    else
                        return RedirectToAction("Index", "Account");
                }                
                int retVal = dBEngine.UpdateSJR2FormInitial(formCollection["InitialManagement"], formCollection["hdfInitialManagementCareRating"],
                    formCollection["OngoingCare"], formCollection["hdfOnGoingCareRating"], formCollection["CareDuringProcedure"], formCollection["hdfCareDuringProcedureCareRating"],
                    formCollection["EndLifeCare"], formCollection["hdfEndLifeCareRating"], formCollection["OverAllAssessment"], formCollection["hdfOverAllAssessmentCareRating"],
                    formCollection["hdfQualityDocumentationCareRating"], id, Convert.ToInt32(Session["LoginUserID"]));
                actionName = "Stage3SJRFormSecondStep";
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return RedirectToAction(actionName, new { id = id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formCollection"></param>
        /// <param name="BtnPrevious"></param>
        /// <param name="BtnFinish"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Stage3SJRformSecondStep(FormCollection formCollection, string BtnPrevious, string BtnSave, string BtnFinish, int? id)
        {
            string actionName = "";
            bool IsFinish = false;
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            //bool reviewcompleted = false;
            
            try
            {
                int PID = Convert.ToInt32(Session["PId"]);
                if (BtnPrevious != null)
                    actionName = "Stage3SJRformFirstStep";
                if (id == null || id == 0)
                {
                    if (Session["PatientID"] != null)
                        id = Convert.ToInt32(Session["PatientID"]);
                    else
                        return RedirectToAction("Index", "Account");
                }
                if (BtnSave != null)
                {
                    if (formCollection["hdfReviewCompleted"] == "on") formCollection["hdfReviewCompleted"] = "true"; else formCollection["hdfReviewCompleted"] = "false";                    
                    if (formCollection["hdfProblemOccured"] == "Yes") formCollection["hdfProblemOccured"] = "true"; else formCollection["hdfProblemOccured"] = "false";
                    int retVal = dBEngine.UpdateSJR2ProblemType(Convert.ToInt32(formCollection["AssessmentResponseID"]), Convert.ToInt32(formCollection["AssessmentCarePhaseID"]),
                        Convert.ToInt32(formCollection["MedicationResponseID"]), Convert.ToInt32(formCollection["MedicationCarePhaseID"]), Convert.ToInt32(formCollection["TreatmentResponseID"]),
                        Convert.ToInt32(formCollection["TreatmentCarePhaseID"]), Convert.ToInt32(formCollection["InfectionResponseID"]), Convert.ToInt32(formCollection["InfectionCarePhaseID"]),
                        Convert.ToInt32(formCollection["ProcedureResponseID"]), Convert.ToInt32(formCollection["ProcedureCarePhaseID"]),
                        Convert.ToInt32(formCollection["MonitoringResponseID"]), Convert.ToInt32(formCollection["ResuscitationResponseID"]),
                        Convert.ToInt32(formCollection["OthertypeResponseID"]), Convert.ToInt32(formCollection["OthertypeCarePhaseID"]), Convert.ToInt32(formCollection["AvoidabilityScoreID"]),
                        formCollection["Comments"], formCollection["SIRIComments"], Convert.ToBoolean(formCollection["hdfProblemOccured"]), Convert.ToBoolean(formCollection["hdfReviewCompleted"]), Convert.ToInt32(formCollection["SpecialityID"]), id, Convert.ToInt32(Session["LoginUserID"]), IsFinish);
                    actionName = "MortalityReview";
                }

                 if (BtnFinish != null)
                {
                    IsFinish = true;
                    if (formCollection["hdfProblemOccured"] == "Yes") formCollection["hdfProblemOccured"] = "true"; else formCollection["hdfProblemOccured"] = "false";
                     if (formCollection["hdfReviewCompleted"] == "on") formCollection["hdfReviewCompleted"] = "true"; else formCollection["hdfReviewCompleted"] = "false";

                    int retVal = dBEngine.UpdateSJR2ProblemType(Convert.ToInt32(formCollection["AssessmentResponseID"]), Convert.ToInt32(formCollection["AssessmentCarePhaseID"]),
                        Convert.ToInt32(formCollection["MedicationResponseID"]), Convert.ToInt32(formCollection["MedicationCarePhaseID"]), Convert.ToInt32(formCollection["TreatmentResponseID"]),
                        Convert.ToInt32(formCollection["TreatmentCarePhaseID"]), Convert.ToInt32(formCollection["InfectionResponseID"]), Convert.ToInt32(formCollection["InfectionCarePhaseID"]),
                        Convert.ToInt32(formCollection["ProcedureResponseID"]), Convert.ToInt32(formCollection["ProcedureCarePhaseID"]),
                        Convert.ToInt32(formCollection["MonitoringResponseID"]), Convert.ToInt32(formCollection["ResuscitationResponseID"]),
                        Convert.ToInt32(formCollection["OthertypeResponseID"]), Convert.ToInt32(formCollection["OthertypeCarePhaseID"]), Convert.ToInt32(formCollection["AvoidabilityScoreID"]),
                        formCollection["Comments"], formCollection["SIRIComments"], Convert.ToBoolean(formCollection["hdfProblemOccured"]), Convert.ToBoolean(formCollection["hdfReviewCompleted"]), Convert.ToInt32(formCollection["SpecialityID"]), id, Convert.ToInt32(Session["LoginUserID"]), IsFinish);
                    actionName = "MortalityReview";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return RedirectToAction(actionName, new { id = id });
        }

        public ActionResult PatientDetailView(int? id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            PatientDetailView patientDetails = new PatientDetailView();
            ViewBag.Comments = dBEngine.GetComments(id, "MEScrutiny", Convert.ToInt32(Session["LoginUserID"]));
            ViewBag.FeedbackComments = dBEngine.GetComments(id, "Feedback", Convert.ToInt32(Session["LoginUserID"]));
            ViewBag.CommentType = dBEngine.GetCommentType("MEScrutiny", Convert.ToInt32(Session["LoginUserID"]));
            ViewBag.FeedbackCommentType = dBEngine.GetCommentType("Feedback", Convert.ToInt32(Session["LoginUserID"]));
            foreach (var b in ViewBag.CommentType)
            {
                foreach (var a in ViewBag.Comments)
                {
                    if (a.CommentTypeID == b.CommonTypeID)
                    {
                        a.CommentType = b.Type;
                    }
                }
            }
            patientDetails = dBEngine.GetPatientDetailViewByID(id, Convert.ToInt32(Session["LoginUserID"]));
            if (patientDetails.lstNEXTKin.ToList().Count > 0)
            {
                for (int item = 0; item < patientDetails.lstNEXTKin.ToList().Count; item++)
                {
                    NextOfKin n = new NextOfKin();
                    n = patientDetails.lstNEXTKin.ToList()[item];
                    if (item == 0)
                    {
                        patientDetails.RelativeName = n.RelativeName;
                        patientDetails.RelativeTelNo = n.RelativeTelNo;
                        patientDetails.Relationship = n.Relationship;
                        patientDetails.PresentAtDeath = n.PresentAtDeath;
                        patientDetails.IsInformed = n.IsInformed;
                        patientDetails.KinId = n.NextOfKinID;

                    }
                    else if (item == 1)
                    {
                        patientDetails.RelativeName1 = n.RelativeName;
                        patientDetails.RelativeTelNo1 = n.RelativeTelNo;
                        patientDetails.Relationship1 = n.Relationship;
                        patientDetails.PresentAtDeath1 = n.PresentAtDeath;
                        patientDetails.IsInformed1 = n.IsInformed;
                        patientDetails.KinId1 = n.NextOfKinID;
                    }
                    else if (item == 2)
                    {
                        patientDetails.RelativeName2 = n.RelativeName;
                        patientDetails.RelativeTelNo2 = n.RelativeTelNo;
                        patientDetails.Relationship2 = n.Relationship;
                        patientDetails.PresentAtDeath2 = n.PresentAtDeath;
                        patientDetails.IsInformed2 = n.IsInformed;
                        patientDetails.KinId2 = n.NextOfKinID;
                    }
                    else
                    { }
                }
            }
            else
            {

            }
            return View(patientDetails);
        } 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult PatientDetails(int? id)
        {
            if (Session["FullName"] != null)
            {
                if (string.IsNullOrEmpty(Session["FullName"].ToString()))
                    return RedirectToAction("Index", "Account");
            }
            else
                return RedirectToAction("Index", "Account");
            bool isUser = GetUserDetailsFromAD();
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            try
            {
                try
                {
                    if (id != null)
                    {
                        ViewBag.Diagnoses = dBEngine.GetDiagnosisDetails(id, Convert.ToInt32(Session["LoginUserID"]));
                        ViewBag.Procedures = dBEngine.GetProcedureDetails(id, Convert.ToInt32(Session["LoginUserID"]));
                        
                    }
                    else if (id == null || id == 0)
                    {
                            id = 0;
                            ViewBag.PatientTypeDDM = dBEngine.GetPatientTypes(Convert.ToInt32(Session["LoginUserID"]));
                    }
                    if (Request.HttpMethod != "POST")
                    {
                        patientDetails = dBEngine.GetPatientDetailsByID(id, Convert.ToInt32(Session["LoginUserID"]));
                        //if (patientDetails.ToList().Count > 0)
                        //{
                        if (patientDetails.ToList()[0].lstNEXTKin.ToList().Count > 0)
                        {
                            for (int item = 0; item < patientDetails.ToList()[0].lstNEXTKin.ToList().Count; item++)
                            {
                                NextOfKin n = new NextOfKin();
                                n = patientDetails.ToList()[0].lstNEXTKin.ToList()[item];
                                if (item == 0)
                                {
                                    patientDetails.ToList()[0].RelativeName = n.RelativeName;
                                    patientDetails.ToList()[0].RelativeTelNo = n.RelativeTelNo;
                                    patientDetails.ToList()[0].Relationship = n.Relationship;
                                    patientDetails.ToList()[0].PresentAtDeath = n.PresentAtDeath;
                                    patientDetails.ToList()[0].IsInformed = n.IsInformed;
                                    patientDetails.ToList()[0].KinId = n.NextOfKinID;

                                }
                                else if (item == 1)
                                {
                                    patientDetails.ToList()[0].RelativeName1 = n.RelativeName;
                                    patientDetails.ToList()[0].RelativeTelNo1 = n.RelativeTelNo;
                                    patientDetails.ToList()[0].Relationship1 = n.Relationship;
                                    patientDetails.ToList()[0].PresentAtDeath1 = n.PresentAtDeath;
                                    patientDetails.ToList()[0].IsInformed1 = n.IsInformed;
                                    patientDetails.ToList()[0].KinId1 = n.NextOfKinID;
                                }
                                else if (item == 2)
                                {
                                    patientDetails.ToList()[0].RelativeName2 = n.RelativeName;
                                    patientDetails.ToList()[0].RelativeTelNo2 = n.RelativeTelNo;
                                    patientDetails.ToList()[0].Relationship2 = n.Relationship;
                                    patientDetails.ToList()[0].PresentAtDeath2 = n.PresentAtDeath;
                                    patientDetails.ToList()[0].IsInformed2 = n.IsInformed;
                                    patientDetails.ToList()[0].KinId2 = n.NextOfKinID;
                                }
                                else
                                { }
                            }
                        }
                        else
                        {

                        }
                        //}
                        //else
                        //{
                        //    clsPatientDetails clspdtls = new clsPatientDetails();
                        //    patientDetails.Add(clspdtls);
                        //}
                    }
                }
                catch (Exception ex)
                {
                    dBEngine.LogException(ex.Message, this.ToString(), "ValidateUser", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            List<RolePermission> permissions = dBEngine.GetRolePermission(0, 0, "", "", "", Convert.ToInt32(Session["LoginUserID"]), "MedTriage", Convert.ToString(Session["Role"]));
            bool isnoaccess = false;
            if (permissions.Count > 0)
            {
                ViewBag.IsReadOnly = permissions[0].IsReadOnly;
                isnoaccess = permissions[0].NoAccess;
            }
            else
            {
                ViewBag.IsReadOnly = true;
                isnoaccess = true;
            }
            if (patientDetails.Count > 0)
            {                
                if (isnoaccess == false)
                {
                    return View(patientDetails[0]);
                }
                else
                {
                    return RedirectToAction("NotAuthorizedPatientDetails", new { id = id });
                }
            }
            else
            {
                if (permissions[0].NoAccess == false)
                {
                    clsPatientDetails clspdtls = new clsPatientDetails();
                    patientDetails.Add(clspdtls);
                    return View(patientDetails[0]);
                }
                else
                {
                    return RedirectToAction("NotAuthorizedPatientDetails", new { id = id });
                }

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formCollection"></param>
        /// <param name="btnCloseYes"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PatientDetails(FormCollection formCollection, string btnCloseYes, int? id)
        {
            //List<string> ids = new List<string>();
            int ids = 0;
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            bool isDataQualityIssuesIdentified = false;
            bool isUrgentMEReview = false;
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dbEngine = new DBEngine(connectionString);
            NextOfKin nextOfKin;
            if (id == null || id == 0)
            {
                if (Session["PatientID"] != null)
                    id = Convert.ToInt32(Session["PatientID"]);
                else
                {
                    //if (string.IsNullOrEmpty(formCollection["txtFName"]))
                    //{ }
                }
                //return RedirectToAction("Index", "Account");
            }
            int patientid = Convert.ToInt32(id);
            try
            {

                if (Convert.ToString(formCollection["DataQualityIssuesIdentified"]) == "on") isDataQualityIssuesIdentified = true;
                if (Convert.ToString(formCollection["UrgentMEReview"]) == "on") isUrgentMEReview = true;

                if (string.IsNullOrEmpty(formCollection["txtFName"]))
                {
                    formCollection["txtFName"] = "";
                }
                if (string.IsNullOrEmpty(formCollection["txtLName"]))
                {
                    formCollection["txtLName"] = "";
                }
                if (string.IsNullOrEmpty(formCollection["ddlGender"]))
                {
                    formCollection["ddlGender"] = "";
                }
                if (string.IsNullOrEmpty(formCollection["DateofDeath"]))
                {
                    formCollection["DateofDeath"] = "02/02/1753"; 
                }
                if (string.IsNullOrEmpty(formCollection["DateofDeath"]))
                {
                    formCollection["DateofDeath"] = "02/02/1753";
                }
                if (string.IsNullOrEmpty(formCollection["DateofBirth"]))
                {
                    formCollection["DateofBirth"] = "02/02/1753";
                }
                if (string.IsNullOrEmpty(formCollection["ddlPatientType"]))
                {
                    formCollection["ddlPatientType"] = "";
                }
          
                ids = dbEngine.UpdatePatientDetailsV2(isDataQualityIssuesIdentified, formCollection["DataQualityIssueComments"], false, "", formCollection["Occupation"],
                    isUrgentMEReview, formCollection["UrgentMEReviewComment"], formCollection["RelativeName"], formCollection["RelativeTelNo"], formCollection["Relationship"],
                    formCollection["GPSurgery"], formCollection["txtFName"], formCollection["txtLName"], formCollection["ddlGender"], formCollection["DateofDeath"], formCollection["ddlPatientType"], formCollection["DateofBirth"], id, Convert.ToInt32(Session["LoginUserID"]));
                //if (ids.ToList().Count != 0)
                //{
                //    id = Convert.ToInt32(ids.ToList()[1]);
                    
                //}
                
                nextOfKin = new NextOfKin();
                if (formCollection["RelativeName"] != "" && formCollection["RelativeTelNo"] != "" && formCollection["Relationship"] != "")
                {
                    nextOfKin.NextOfKinID = Convert.ToInt32(formCollection["KinId"]);
                    nextOfKin.RelativeName = (formCollection["RelativeName"]).ToString();
                    nextOfKin.RelativeTelNo = (formCollection["RelativeTelNo"]).ToString();
                    nextOfKin.Relationship = formCollection["Relationship"];
                    nextOfKin.PresentAtDeath = Convert.ToBoolean(formCollection["PresentAtDeath"].Split(',')[0]);
                    nextOfKin.IsInformed = Convert.ToBoolean(formCollection["IsInformed"].Split(',')[0]);
                    nextOfKin.PatientID = id.ToString();
                    nextOfKin.NextOfKinID = dbEngine.InsertKin(nextOfKin, Convert.ToInt32(Session["LoginUserID"]));
                    if (nextOfKin.NextOfKinID > 0)
                    {
                        nextOfKin = new NextOfKin();
                        if (formCollection["RelativeName1"] != "" && formCollection["RelativeTelNo1"] != "" && formCollection["Relationship1"] != "")
                        {
                            nextOfKin.NextOfKinID = Convert.ToInt32(formCollection["KinId1"]);
                            nextOfKin.RelativeName = (formCollection["RelativeName1"]).ToString();
                            nextOfKin.RelativeTelNo = (formCollection["RelativeTelNo1"]).ToString();
                            nextOfKin.Relationship = formCollection["Relationship1"];
                            nextOfKin.PresentAtDeath = Convert.ToBoolean(formCollection["PresentAtDeath1"].Split(',')[0]);
                            nextOfKin.IsInformed = Convert.ToBoolean(formCollection["IsInformed1"].Split(',')[0]);
                            nextOfKin.PatientID = id.ToString();
                            nextOfKin.NextOfKinID = dbEngine.InsertKin(nextOfKin, Convert.ToInt32(Session["LoginUserID"]));
                            if (nextOfKin.NextOfKinID > 0)
                            {
                                nextOfKin = new NextOfKin();
                                if (formCollection["RelativeName2"] != "" && formCollection["RelativeTelNo2"] != "" && formCollection["Relationship2"] != "")
                                {
                                    nextOfKin.NextOfKinID = Convert.ToInt32(formCollection["KinId2"]);
                                    nextOfKin.RelativeName = (formCollection["RelativeName2"]).ToString();
                                    nextOfKin.RelativeTelNo = (formCollection["RelativeTelNo2"]).ToString();
                                    nextOfKin.Relationship = formCollection["Relationship2"];
                                    nextOfKin.PresentAtDeath = Convert.ToBoolean(formCollection["PresentAtDeath2"].Split(',')[0]);
                                    nextOfKin.IsInformed = Convert.ToBoolean(formCollection["IsInformed2"].Split(',')[0]);
                                    nextOfKin.PatientID = id.ToString();
                                    nextOfKin.NextOfKinID = dbEngine.InsertKin(nextOfKin, Convert.ToInt32(Session["LoginUserID"]));
                                }
                            }
                        }
                    }

                }
                if (isDataQualityIssuesIdentified)
                {
                    List<clsPatientDetails> patient = dbEngine.GetPatientDetailsByID(id, Convert.ToInt32(Session["LoginUserID"]));
                    if (patient.Count > 0)
                    {
                        //Specialities speciality = dbEngine.GetSpecialitybyID(Convert.ToInt32(formCollection["ddlCoronerReferral"]));
                        int dbReturn = dbEngine.UpdateDQIssueDetails(patient[0].PatientId, patient[0].PatientName, DateTime.ParseExact(patient[0].DateofDeath, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), System.DateTime.Now, Convert.ToString(Session["FullName"]), formCollection["DataQualityIssueComments"], Convert.ToInt32(Session["LoginUserID"]));

                        List<NotificationSettings> settings = dbEngine.GetNotificationSettingsByTrigger("Data Quality Issue Identified", Convert.ToInt32(Session["LoginUserID"]));
                        if (settings.Count > 0)
                        {
                            for (int counter = 0; counter < settings.Count; counter++)
                            {
                                List<AppUsers> users = dbEngine.GetUsersByRoleID(settings[counter].RoleID, Convert.ToInt32(Session["LoginUserID"]));
                                PatientNotification notification = dbEngine.GetNotificationSentStatusByPatientID(patientid, "Data Quality Issue Identified", Convert.ToInt32(Session["LoginUserID"]));
                                if (notification.NotificationTrigger == null)
                                {
                                    for (int count = 0; count < users.Count; count++)
                                    {
                                        SendEmail(users[count].EmailID, "CORS - DQ issue", settings[counter].EmailTemplate.Replace("##PatientID##", patient[0].PatientId).Replace("##PatientName##", patient[0].PatientName).Replace("##DateOfDeath##", patient[0].DateofDeath).Replace("##IssueDate##", System.DateTime.Now.ToString("dd/MM/yyyy")).Replace("##IssuedBy##", Convert.ToString(Session["FullName"])).Replace("##Comments##", formCollection["DataQualityIssueComments"]));
                                    }                                    
                                }
                            }
                            dbEngine.UpdatePatientNotification(patientid, "Data Quality Issue Identified", Convert.ToInt32(Session["LoginUserID"]));
                        }
                    }
                }
                if(isUrgentMEReview)
                {
                    List<clsPatientDetails> patient = dbEngine.GetPatientDetailsByID(id, Convert.ToInt32(Session["LoginUserID"]));
                    if (patient.Count > 0)
                    {
                        //Specialities speciality = dbEngine.GetSpecialitybyID(Convert.ToInt32(formCollection["ddlCoronerReferral"]));
                        int dbReturn = dbEngine.UpdateUrgentMEDetails(patient[0].PatientId, patient[0].PatientName, DateTime.ParseExact(patient[0].DateofDeath, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), System.DateTime.Now, Convert.ToString(Session["FullName"]), formCollection["UrgentMEReviewComment"], Convert.ToInt32(Session["LoginUserID"]));

                        List<NotificationSettings> settings = dbEngine.GetNotificationSettingsByTrigger("Urgent ME Review Required", Convert.ToInt32(Session["LoginUserID"]));
                        if (settings.Count > 0)
                        {
                            for (int counter = 0; counter < settings.Count; counter++)
                            {
                                List<AppUsers> users = dbEngine.GetUsersByRoleID(settings[counter].RoleID, Convert.ToInt32(Session["LoginUserID"]));
                                PatientNotification notification = dbEngine.GetNotificationSentStatusByPatientID(patientid, "Urgent ME Review Required", Convert.ToInt32(Session["LoginUserID"]));
                                if (notification.NotificationTrigger == null)
                                {
                                    for (int count = 0; count < users.Count; count++)
                                    {
                                        SendEmail(users[count].EmailID, "CORS - Urgent ME Required", settings[counter].EmailTemplate.Replace("##PatientID##", patient[0].PatientId).Replace("##PatientName##", patient[0].PatientName).Replace("##DateOfDeath##", patient[0].DateofDeath).Replace("##IssueDate##", System.DateTime.Now.ToString("dd/MM/yyyy")).Replace("##IssuedBy##", Convert.ToString(Session["FullName"])).Replace("##Comments##", formCollection["UrgentMEReviewComment"]));
                                    }                                    
                                }
                            }
                            dbEngine.UpdatePatientNotification(patientid, "Urgent ME Review Required", Convert.ToInt32(Session["LoginUserID"]));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return RedirectToAction("MedicalExaminerReview", new { id = id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult MedicalExaminerReview(int? id)
        {
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            if (Session["FullName"] != null)
            {
                if (string.IsNullOrEmpty(Session["FullName"].ToString()))
                    return RedirectToAction("Index", "Account");
            }
            else
                return RedirectToAction("Index", "Account");
            //bool isUser = GetUserDetailsFromAD();
            TempData["Role"] = Session["Role"];
            clsMedicalExaminerReview medicalExaminerReview = new clsMedicalExaminerReview();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            try
            {
                if (id == null || id == 0)
                {
                    if (Session["PatientID"] != null)
                        id = Convert.ToInt32(Session["PatientID"]);
                    else
                        return RedirectToAction("Index", "Account");
                }
                //if (ApplicationSession.LoginUserID > 0)
                //{
                try
                {
                    Session["PatientID"] = id;
                    ViewBag.MedicalExaminers = dBEngine.GetMedicalExaminers(Convert.ToInt32(Session["LoginUserID"]));
                    ViewBag.Comments = dBEngine.GetComments(id, "MEScrutiny", Convert.ToInt32(Session["LoginUserID"]));
                    ViewBag.CommentType = dBEngine.GetCommentType("MEScrutiny", Convert.ToInt32(Session["LoginUserID"]));
                    foreach (var b in ViewBag.CommentType)
                    {
                        foreach (var a in ViewBag.Comments)
                        {
                            if (a.CommentTypeID == b.CommonTypeID)
                            {
                                a.CommentType = b.Type;
                            }
                        }
                    }


                    if (Request.HttpMethod != "POST")
                    {
                        medicalExaminerReview = dBEngine.GetMedicalExaminerReview(id, Convert.ToInt32(Session["LoginUserID"]));
                        clsDeclarationModel declaration = new clsDeclarationModel();
                        declaration = dBEngine.GetDeclaration(id, Convert.ToInt32(Session["LoginUserID"]));
                        if (declaration.Declaration)
                            medicalExaminerReview.CreatedBy = declaration.CreatedBy;
                        if (medicalExaminerReview.CreatedBy == null) medicalExaminerReview.CreatedBy = "";
                        patientDetails = dBEngine.GetPatientDetails(id, Convert.ToInt32(Session["LoginUserID"]));
                        medicalExaminerReview.PatientId = patientDetails.ToList()[0].PatientId.ToString();
                        medicalExaminerReview.PatientName = patientDetails.ToList()[0].PatientName;
                        medicalExaminerReview.DOB = patientDetails.ToList()[0].DOB;
                        medicalExaminerReview.MedTriage = patientDetails[0].MedTriage;
                    }
                    if (medicalExaminerReview.Patient_ID == 0 || medicalExaminerReview.Patient_ID == null)
                        medicalExaminerReview.Patient_ID = Convert.ToInt32(id);
                }
                catch (Exception ex)
                {
                    dBEngine.LogException(ex.Message, this.ToString(), "ValidateUser", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            List<RolePermission> permissions = dBEngine.GetRolePermission(0, 0, "", "", "", Convert.ToInt32(Session["LoginUserID"]), "MedTriage", Convert.ToString(Session["Role"]));
            bool isnoaccess = false;
            if (permissions.Count > 0)
            {
                ViewBag.IsReadOnly = permissions[0].IsReadOnly;
                isnoaccess = permissions[0].NoAccess;
            }
            else
            {
                ViewBag.IsReadOnly = true;
                isnoaccess = true;
            }
            if (isnoaccess == false)
            {
                return View(medicalExaminerReview);
            }
            else
            {
                return RedirectToAction("NotAuthorizedPatientDetails", new { id = id });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupid"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetReferralReasons(int? groupid)
        {
            List<clsCoronerReferralReason> referralReasons = new List<clsCoronerReferralReason>();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            try
            {
                referralReasons = dBEngine.GetCoronerReferralReasons(groupid, Convert.ToInt32(Session["LoginUserID"]));
                ViewBag.CoronerReferralReasons = referralReasons;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(referralReasons, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formCollection"></param>
        /// <param name="BtnPrevious"></param>
        /// <param name="BtnNext"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult MedicalExaminerReview(FormCollection formCollection, string BtnPrevious, string BtnNext, int? id)
        {
            int commentTypeID = 0;
            if (string.IsNullOrEmpty(Session["FullName"].ToString()))
                return RedirectToAction("Index", "Account");
            string actionName = "";
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            try
            {
                bool isQAP_Discussion = false;
                bool isNotes_Review = false;
                bool isNok_Discussion = false;

                if (BtnPrevious != null)
                    actionName = "PatientDetails";
                if (id == null || id == 0)
                {
                    if (Session["PatientID"] != null)
                        id = Convert.ToInt32(Session["PatientID"]);
                    else
                        return RedirectToAction("Index", "Account");
                }
                if (BtnNext != null)
                {
                    ViewBag.MedicalExaminers = dBEngine.GetMedicalExaminers(Convert.ToInt32(Session["LoginUserID"]));
                    if (Convert.ToString(formCollection["QAP_Discussion"]) == "on") isQAP_Discussion = true;
                    if (Convert.ToString(formCollection["Notes_Review"]) == "on") isNotes_Review = true;
                    if (Convert.ToString(formCollection["Nok_Discussion"]) == "on") isNok_Discussion = true;
                    if (formCollection["ddlDischargeSpeciality"] == "" || formCollection["ddlDischargeSpeciality"] == null)
                        formCollection["ddlDischargeSpeciality"] = Session["LoginUserID"].ToString();
                    if (formCollection["QAPName"] == "") formCollection["QAPName"] = "0";
                    if (string.IsNullOrEmpty(formCollection["Comments"])) formCollection["Comments"] = "";
            

                    string role = Convert.ToString(formCollection["ddlRole"]);
                    if (formCollection["ddlCommentType"] != "")
                    {
                        commentTypeID = Convert.ToInt32(formCollection["ddlCommentType"]);
                    }
                    else
                    {
                        commentTypeID = 0;
                    }

                     
                    if (formCollection["IsComment"] != "")
                    {
                        bool IsComment = Convert.ToBoolean(formCollection["IsComment"]);
                        if (IsComment)
                        {
                            string CommentType = "";
                            string comments = "";
                            CommentType = formCollection["ddlCommentType"].ToString();
                            comments = formCollection["txtComments"].ToString();
                            int result = SaveComment(CommentType, comments);
                        }
                    }
                    
                    //int retVal = dBEngine.UpdateMedicalExaminerReview(isQAP_Discussion, isNotes_Review, isNok_Discussion,
                    //   Convert.ToInt32(formCollection["ddlDischargeSpeciality"]), formCollection["QAPName"], formCollection["Comments"], id, Convert.ToInt32(Session["LoginUserID"]));

                    int retVal = dBEngine.UpdateMedicalExaminerReview(isQAP_Discussion, isNotes_Review, isNok_Discussion,
                        Convert.ToInt32(formCollection["ddlDischargeSpeciality"]), formCollection["Comments"], id, Convert.ToInt32(Session["LoginUserID"]), commentTypeID);
                    actionName = "MedicalExaminerDecision";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return RedirectToAction(actionName, new { id = id });
        }
        public int SaveComment(string CommentType, string comments)
        {
            string actionName = "";
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            int commentTypeID = 0, id = 0;
            string Comments = comments;
            commentTypeID = Convert.ToInt32(CommentType);


            if (id == null || id == 0)
            {
                if (Session["PatientID"] != null)
                    id = Convert.ToInt32(Session["PatientID"]);
                else
                { }
                    //return RedirectToAction("Index", "Account");
            }
            int retVal = dBEngine.MRESaveComments(Comments, id, Convert.ToInt32(Session["LoginUserID"]), commentTypeID);
            return retVal;
        }




        //changes for saving MER comments
        public ActionResult PFBSaveComment(string CommentType, string comments)
        {
            string actionName = "";
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            int commentTypeID = 0, id = 0;

            commentTypeID = Convert.ToInt32(CommentType);


            if (id == null || id == 0)
            {
                if (Session["PatientID"] != null)
                    id = Convert.ToInt32(Session["PatientID"]);
                else
                    return RedirectToAction("Index", "Account");
            }
            int retVal = dBEngine.MRESaveComments(comments, id, Convert.ToInt32(Session["LoginUserID"]), commentTypeID);
            return RedirectToAction("Declaration", new { id = id });
        }

        // for saving feedback comments
        public ActionResult MERSaveComment(string CommentType, string comments)
        {
            string actionName = "";
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            int commentTypeID = 0, id = 0;

            commentTypeID = Convert.ToInt32(CommentType);


            if (id == null || id == 0)
            {
                if (Session["PatientID"] != null)
                    id = Convert.ToInt32(Session["PatientID"]);
                else
                    return RedirectToAction("Index", "Account");
            }
            int retVal = dBEngine.MRESaveComments(comments, id, Convert.ToInt32(Session["LoginUserID"]), commentTypeID);
            return RedirectToAction("MedicalExaminerReview", new { id = id });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult MedicalExaminerDecision(int? id)
        {
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            if (Session["FullName"] != null)
            {
                if (string.IsNullOrEmpty(Session["FullName"].ToString()))
                    return RedirectToAction("Index", "Account");
            }
            else
                return RedirectToAction("Index", "Account");
            bool isUser = GetUserDetailsFromAD();
            clsMedicalExaminerDecision medicalExaminerDecision = new clsMedicalExaminerDecision();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            try
            {
                if (id == null || id == 0)
                {
                    if (Session["PatientID"] != null)
                        id = Convert.ToInt32(Session["PatientID"]);
                    else
                        return RedirectToAction("Index", "Account");
                }
                try
                {
                    int medtriage = 0;
                    medicalExaminerDecision = dBEngine.GetMedicalExaminerDecision(id, Convert.ToInt32(Session["LoginUserID"]));
                    clsDeclarationModel declaration = new clsDeclarationModel();
                    declaration = dBEngine.GetDeclaration(id, Convert.ToInt32(Session["LoginUserID"]));
                    if (declaration.Declaration)
                        medicalExaminerDecision.CreatedBy = declaration.CreatedBy;
                    if (medicalExaminerDecision.CreatedBy == null) medicalExaminerDecision.CreatedBy = "";
                    patientDetails = dBEngine.GetPatientDetails(id, Convert.ToInt32(Session["LoginUserID"]));

                    medicalExaminerDecision.Patient_Id = patientDetails.ToList()[0].PatientId;
                    medicalExaminerDecision.PatientName = patientDetails.ToList()[0].PatientName;
                    medicalExaminerDecision.DOB = patientDetails.ToList()[0].DOB;
                    if (medicalExaminerDecision.MedTriage == 0)
                    {
                        medtriage = dBEngine.GetMedTriageByPatientID(id, Convert.ToInt32(Session["LoginUserID"]));
                        medicalExaminerDecision.MedTriage = medtriage;
                    }
                    medicalExaminerDecision.a = "1a";
                    medicalExaminerDecision.b = "1b";
                    medicalExaminerDecision.c = "1c";
                    medicalExaminerDecision.d = "1d";
                    if (medicalExaminerDecision.ID == 0 || medicalExaminerDecision.ID == null)
                        medicalExaminerDecision.ID = Convert.ToInt32(id);
                    //ViewBag.ReasonGroups = dBEngine.GetReasonGroups();
                }
                catch (Exception ex)
                {
                    dBEngine.LogException(ex.Message, this.ToString(), "ValidateUser", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            List<RolePermission> permissions = dBEngine.GetRolePermission(0, 0, "", "", "", Convert.ToInt32(Session["LoginUserID"]), "MedTriage", Convert.ToString(Session["Role"]));
            bool isnoaccess = false;
            if (permissions.Count > 0)
            {
                ViewBag.IsReadOnly = permissions[0].IsReadOnly;
                isnoaccess = permissions[0].NoAccess;
            }
            else
            {
                ViewBag.IsReadOnly = true;
                isnoaccess = true;
            }
            if (isnoaccess == false)
            {
                return View(medicalExaminerDecision);
            }
            else
            {
                return RedirectToAction("NotAuthorizedPatientDetails", new { id = id });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formCollection"></param>
        /// <param name="BtnPrevious"></param>
        /// <param name="BtnNext"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult MedicalExaminerDecision(FormCollection formCollection, string BtnPrevious, string BtnNext, int id)
        {
            bool isMCCDissue = false;
            bool isCoronerReferral = false;
            bool isHospitalPostMortem = false;
            bool isDeathCertificate = false;
            bool isCornerReferralComplete = false;
            bool isCoronerDecisionInquest = false;
            bool isCoronerDecisionPostMortem = false;
            bool isCoronerDecision100A = false;
            bool isCoronerDecisionGPissue = false;
            bool isCoronerDecisionNFAction = false;
            string actionName = "";
            clsMedicalExaminerDecision medicalExaminerDecision = new clsMedicalExaminerDecision();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            DateTime? deathCertificateDate = null;
            if (id == null || id == 0)
            {
                if (Session["PatientID"] != null)
                    id = Convert.ToInt32(Session["PatientID"]);
                else
                    return RedirectToAction("Index", "Account");
            }
            try
            {
                if (BtnPrevious != null)
                    actionName = "MedicalExaminerReview";
                if (BtnNext != null)
                {
                    actionName = "SJRAssessmentTriage";
                    medicalExaminerDecision = dBEngine.GetMedicalExaminerDecision(id, Convert.ToInt32(Session["LoginUserID"]));
                    if (Convert.ToString(formCollection["IssueMCCD"]) == "on") isMCCDissue = true;
                    if (Convert.ToString(formCollection["CoronerReferral"]) == "on") isCoronerReferral = true;
                    if (Convert.ToString(formCollection["PostMortem"]) == "on") isHospitalPostMortem = true;
                    if (Convert.ToString(formCollection["DeathCertificate"]) == null) isDeathCertificate = medicalExaminerDecision.DeathCertificate;
                    if (Convert.ToString(formCollection["CoronerReferralComplete"]) == "on") isCornerReferralComplete = true;
                    if (Convert.ToString(formCollection["Inquest"]) == null) isCoronerDecisionInquest = medicalExaminerDecision.CoronerDecisionInquest;
                    if (Convert.ToString(formCollection["Post-Mortem"]) == null) isCoronerDecisionPostMortem = medicalExaminerDecision.CoronerDecisionPostMortem;
                    if (Convert.ToString(formCollection["100A"]) == null) isCoronerDecision100A = medicalExaminerDecision.CoronerDecision100A;
                    if (Convert.ToString(formCollection["GPIssue"]) == null) isCoronerDecisionGPissue = medicalExaminerDecision.CoronerDecisionGPissue;
                    if (Convert.ToString(formCollection["NoFurAction"]) == null) isCoronerDecisionNFAction = medicalExaminerDecision.NoFurtherAction;
                    if (isDeathCertificate == true)
                    {
                        if (formCollection["DeathCertificateDate"] == null)
                            deathCertificateDate = DateTime.ParseExact(medicalExaminerDecision.DeathCertificateDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        //deathCertificateDate = DateTime.ParseExact(Convert.ToDateTime(formCollection["DeathCertificateDate"]).ToString("dd/MM/yyyy"), "dd/MM/yyyy", null);
                        //if (formCollection["DeathCertificateTime"] == "") formCollection["DeathCertificateTime"] = DateTime.ParseExact("00:00", "HH:mm", null).ToString();
                    }
                    else
                    {
                        deathCertificateDate = null;
                        formCollection["DeathCertificateTime"] = "";
                        formCollection["TimeType"] = "0";
                    }
                    if (Convert.ToString(formCollection["CauseOfDeath1"]) == null)
                    {
                        formCollection["CauseOfDeath1"] = medicalExaminerDecision.CauseOfDeath1!=null? medicalExaminerDecision.CauseOfDeath1:"";
                    }

                    if (Convert.ToString(formCollection["CauseOfDeath2"]) == null)
                    {
                        formCollection["CauseOfDeath2"] = medicalExaminerDecision.CauseOfDeath2!=null ? medicalExaminerDecision.CauseOfDeath2:"";
                    }
                    if (Convert.ToString(formCollection["CauseOfDeath3"]) == null)
                    {
                        formCollection["CauseOfDeath3"] = medicalExaminerDecision.CauseOfDeath3!=null ? medicalExaminerDecision.CauseOfDeath3:"";
                    }
                    if (Convert.ToString(formCollection["CauseOfDeath4"]) == null)
                    {
                        formCollection["CauseOfDeath4"] = medicalExaminerDecision.CauseOfDeath4!=null?medicalExaminerDecision.CauseOfDeath4:"";
                    }
                    int retVal = dBEngine.UpdateMedicalExaminerDecision(isMCCDissue, isCoronerReferral, isHospitalPostMortem, isCornerReferralComplete, formCollection["CoronerReferralReason"], formCollection["CauseOfDeath1"], formCollection["CauseOfDeath2"], formCollection["CauseOfDeath3"],
                       formCollection["CauseOfDeath4"], formCollection["ddlCauseOdfDeath"], id, Convert.ToInt32(Session["LoginUserID"]));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return RedirectToAction(actionName, new { id = id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult SJRAssessmentTriage(int? id)
        {
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            if (Session["FullName"] != null)
            {
                if (string.IsNullOrEmpty(Session["FullName"].ToString()))
                    return RedirectToAction("Index", "Account");
            }
            else
                return RedirectToAction("Index", "Account");
            bool isUser = GetUserDetailsFromAD();
            clsSJRReview sJRAssement = new clsSJRReview();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            try
            {
                if (id == null || id == 0)
                {
                    if (Session["PatientID"] != null)
                        id = Convert.ToInt32(Session["PatientID"]);
                    else
                        return RedirectToAction("Index", "Account");
                }
                //if (ApplicationSession.LoginUserID > 0)
                //{
                try
                {
                    sJRAssement = dBEngine.GetSJRAssesmentTraige(id, Convert.ToInt32(Session["LoginUserID"]));
                    clsDeclarationModel declaration = new clsDeclarationModel();
                    declaration = dBEngine.GetDeclaration(id, Convert.ToInt32(Session["LoginUserID"]));
                    if (declaration.Declaration)
                        sJRAssement.CreatedBy = declaration.CreatedBy;
                    if (sJRAssement.CreatedBy == null) sJRAssement.CreatedBy = "";
                    patientDetails = dBEngine.GetPatientDetails(id, Convert.ToInt32(Session["LoginUserID"]));

                    sJRAssement.PatientID = patientDetails.ToList()[0].PatientId;
                    sJRAssement.PatientName = patientDetails.ToList()[0].PatientName;
                    sJRAssement.DOB = patientDetails.ToList()[0].DOB;
                    if (sJRAssement.Patient_ID == 0 || sJRAssement.Patient_ID == null)
                        sJRAssement.Patient_ID = Convert.ToInt32(id);
                    ViewBag.Specialities = dBEngine.GetSpecialitiesForDropDown(Convert.ToInt32(Session["LoginUserID"]));
                }
                catch (Exception ex)
                {
                    dBEngine.LogException(ex.Message, this.ToString(), "ValidateUser", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
                }
                //}
                //else
                //{
                //    return RedirectToAction("Index", "Account");
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
            List<RolePermission> permissions = dBEngine.GetRolePermission(0, 0, "", "", "", Convert.ToInt32(Session["LoginUserID"]), "MedTriage", Convert.ToString(Session["Role"]));
            bool isnoaccess = false;
            if (permissions.Count > 0)
            {
                ViewBag.IsReadOnly = permissions[0].IsReadOnly;
                isnoaccess = permissions[0].NoAccess;
            }
            else
            {
                ViewBag.IsReadOnly = true;
                isnoaccess = true;
            }
            if (isnoaccess == false)
            {
                return View(sJRAssement);
            }
            else
            {
                return RedirectToAction("NotAuthorizedPatientDetails", new { id = id });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formCollection"></param>
        /// <param name="BtnPrevious"></param>
        /// <param name="BtnNext"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SJRAssessmentTriage(FormCollection formCollection, string BtnPrevious, string BtnNext, int? id)
        {
            string actionName = "";
            bool isPaediatricPatient = false;
            bool isLearningDisabilityPatient = false;
            bool isMentalillnessPatient = false;
            bool isElectiveAdmission = false;
            bool isNoKConcernsDeath = false;
            bool isDeathChemo = false;
            bool isOtherConcern = false;
            bool isFullSJRRequired = false;
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            try
            {
                if (id == null || id == 0)
                {
                    if (Session["PatientID"] != null)
                        id = Convert.ToInt32(Session["PatientID"]);
                    else
                        return RedirectToAction("Index", "Account");
                }
                int patientid = Convert.ToInt32(id);
                //if (ApplicationSession.LoginUserID > 0)
                //{
                try
                {
                    if (BtnPrevious != null)
                    {
                        actionName = "MedicalExaminerDecision";
                    }
                    if (BtnNext != null)
                    {
                        actionName = "OtherReferrals";
                        if (Convert.ToString(formCollection["PaediatricPatient"]) == "on") isPaediatricPatient = true;
                        if (Convert.ToString(formCollection["LearningDisabilityPatient"]) == "on") isLearningDisabilityPatient = true;
                        if (Convert.ToString(formCollection["MentalillnessPatient"]) == "on") isMentalillnessPatient = true;
                        if (Convert.ToString(formCollection["ElectiveAdmission"]) == "on") isElectiveAdmission = true;
                        if (Convert.ToString(formCollection["NoKConcernsDeath"]) == "on") isNoKConcernsDeath = true;
                        if (Convert.ToString(formCollection["DeathChemo"]) == "on") isDeathChemo = true;
                        if (Convert.ToString(formCollection["OtherConcern"]) == "on") isOtherConcern = true;
                        if (Convert.ToString(formCollection["FullSJRRequired"]) == "Yes") isFullSJRRequired = true;
                        int retVal = dBEngine.UpdateSJRAssessmentTriage(isPaediatricPatient, isLearningDisabilityPatient, isMentalillnessPatient, isElectiveAdmission, isNoKConcernsDeath, isDeathChemo, isOtherConcern, isFullSJRRequired,
                           formCollection["OtherConcernDetails"], Convert.ToInt32(formCollection["ddlCoronerReferral"]), id, formCollection["Comments"], Convert.ToInt32(Session["LoginUserID"]));

                        if(isFullSJRRequired)
                        {
                            List<clsPatientDetails> patient = dBEngine.GetPatientDetailsByID(id, Convert.ToInt32(Session["LoginUserID"]));
                            if (patient.Count > 0)
                            {
                                Specialities speciality = dBEngine.GetSpecialitybyID(Convert.ToInt32(formCollection["ddlCoronerReferral"]), Convert.ToInt32(Session["LoginUserID"]));
                                int dbReturn = dBEngine.UpdateFullSJRDetails(patient[0].PatientId, patient[0].PatientName, DateTime.ParseExact(patient[0].DateofDeath, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), System.DateTime.Now, Convert.ToString(Session["FullName"]), speciality.SpecialityName, Convert.ToInt32(Session["LoginUserID"]));
                                
                                List<NotificationSettings> settings = dBEngine.GetNotificationSettingsByTrigger("Full SJR Required", Convert.ToInt32(Session["LoginUserID"]));
                                if (settings.Count > 0)
                                {
                                    for (int counter = 0; counter < settings.Count; counter++)
                                    {
                                        List<AppUsers> users = dBEngine.GetUsersByRoleID(settings[counter].RoleID, Convert.ToInt32(Session["LoginUserID"]));
                                        PatientNotification notification = dBEngine.GetNotificationSentStatusByPatientID(patientid, "Full SJR Required", Convert.ToInt32(Session["LoginUserID"]));
                                        if (notification.NotificationTrigger == null)
                                        {
                                            //if (dbReturn == 0)
                                            //{
                                            for (int count = 0; count < users.Count; count++)
                                            {
                                                SendEmail(users[count].EmailID, "CORS - Full SJR Required", settings[counter].EmailTemplate.Replace("##PatientID##", patient[0].PatientId).Replace("##PatientName##", patient[0].PatientName).Replace("##DateOfDeath##", patient[0].DateofDeath).Replace("##RequestedDate##", System.DateTime.Now.ToString("dd/MM/yyyy")).Replace("##RequestedBy##", Convert.ToString(Session["FullName"])).Replace("##Speciality##", speciality.SpecialityName));
                                            }                                            //}
                                            
                                        }
                                    }
                                    dBEngine.UpdatePatientNotification(patientid, "Full SJR Required", Convert.ToInt32(Session["LoginUserID"]));
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    dBEngine.LogException(ex.Message, this.ToString(), "ValidateUser", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
                }
                return RedirectToAction(actionName, new { id = id });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get Other referral details for a particular patient ID
        /// </summary>
        /// <param name="id">int</param>
        /// <returns>ActionResult</returns>
        public ActionResult OtherReferrals(int? id)
        {
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            if (Session["FullName"] != null)
            {
                if (string.IsNullOrEmpty(Session["FullName"].ToString()))
                    return RedirectToAction("Index", "Account");
            }
            else
                return RedirectToAction("Index", "Account");
            bool isUser = GetUserDetailsFromAD();
            clsOtherReferralModel sJRAssement = new clsOtherReferralModel();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            try
            {
                if (id == null || id == 0)
                {
                    if (Session["PatientID"] != null)
                        id = Convert.ToInt32(Session["PatientID"]);
                    else
                        return RedirectToAction("Index", "Account");
                }
                //if (ApplicationSession.LoginUserID > 0)
                //{
                try
                {
                    sJRAssement = dBEngine.GetOtherReferrals(id, Convert.ToInt32(Session["LoginUserID"]));
                    clsDeclarationModel declaration = new clsDeclarationModel();
                    declaration = dBEngine.GetDeclaration(id, Convert.ToInt32(Session["LoginUserID"]));
                    if (declaration.Declaration)
                        sJRAssement.CreatedBy = declaration.CreatedBy;
                    if (sJRAssement.CreatedBy == null) sJRAssement.CreatedBy = "";
                    patientDetails = dBEngine.GetPatientDetails(id, Convert.ToInt32(Session["LoginUserID"]));

                    sJRAssement.PatientID = patientDetails.ToList()[0].PatientId;
                    sJRAssement.PatientName = patientDetails.ToList()[0].PatientName;
                    sJRAssement.DOB = patientDetails.ToList()[0].DOB;
                    sJRAssement.MedTriage = patientDetails[0].MedTriage;
                    if (sJRAssement.Patient_ID == 0 || sJRAssement.Patient_ID == null)
                        sJRAssement.Patient_ID = Convert.ToInt32(id);
                }
                catch (Exception ex)
                {
                    dBEngine.LogException(ex.Message, this.ToString(), "ValidateUser", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
                }
                //}
                //else
                //{
                //    return RedirectToAction("Index", "Account");
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
            List<RolePermission> permissions = dBEngine.GetRolePermission(0, 0, "", "", "", Convert.ToInt32(Session["LoginUserID"]), "MedTriage", Convert.ToString(Session["Role"]));
            bool isnoaccess = false;
            if (permissions.Count > 0)
            {
                ViewBag.IsReadOnly = permissions[0].IsReadOnly;
                isnoaccess = permissions[0].NoAccess;
            }
            else
            {
                ViewBag.IsReadOnly = true;
                isnoaccess = true;
            }
            if (isnoaccess == false)
            {
                return View(sJRAssement);
            }
            else
            {
                return RedirectToAction("NotAuthorizedPatientDetails", new { id = id });
            }
        }

        /// <summary>
        /// Get Declaration for a particular patient ID
        /// </summary>
        /// <param name="id">int</param>
        /// <returns>ActionResult</returns>
        public ActionResult Declaration(int? id)
        {
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            if (Session["FullName"] != null)
            {
                if (string.IsNullOrEmpty(Session["FullName"].ToString()))
                    return RedirectToAction("Index", "Account");
            }
            else
                return RedirectToAction("Index", "Account");
            bool isUser = GetUserDetailsFromAD();
            clsDeclarationModel declaration = new clsDeclarationModel();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            ViewBag.SpecialityDDM = dBEngine.GetSpecialitiesForDropDown(Convert.ToInt32(Session["LoginUserID"]));
            try
            {
                if (id == null || id == 0)
                {
                    if (Session["PatientID"] != null)
                        id = Convert.ToInt32(Session["PatientID"]);
                    else
                        return RedirectToAction("Index", "Account");
                }
                try
                {
                    declaration = dBEngine.GetDeclaration(id, Convert.ToInt32(Session["LoginUserID"]));
                    if (declaration.CreatedBy == null) declaration.CreatedBy = "";
                    patientDetails = dBEngine.GetPatientDetails(id, Convert.ToInt32(Session["LoginUserID"]));

                    declaration.PatientID = patientDetails.ToList()[0].PatientId;
                    declaration.PatientName = patientDetails.ToList()[0].PatientName;
                    declaration.DOB = patientDetails.ToList()[0].DOB;
                    if (declaration.Patient_ID == 0 || declaration.Patient_ID == null)
                        declaration.Patient_ID = Convert.ToInt32(id);
                }
                catch (Exception ex)
                {
                    dBEngine.LogException(ex.Message, this.ToString(), "ValidateUser", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            List<RolePermission> permissions = dBEngine.GetRolePermission(0, 0, "", "", "", Convert.ToInt32(Session["LoginUserID"]), "MedTriage", Convert.ToString(Session["Role"]));
            bool isnoaccess = false;
            if (permissions.Count > 0)
            {
                ViewBag.IsReadOnly = permissions[0].IsReadOnly;
                isnoaccess = permissions[0].NoAccess;
            }
            else
            {
                ViewBag.IsReadOnly = true;
                isnoaccess = true;
            }
            if (isnoaccess == false)
            {
                return View(declaration);
            }
            else
            {
                return RedirectToAction("NotAuthorizedPatientDetails", new { id = id });
            }
        }
        /// <summary>
        /// Get Fimal Outcome for a particular patient ID
        /// </summary>
        /// <param name="id">int</param>
        /// <returns>ActionResult</returns>
        public ActionResult FinalOutcome(int? id)
        {
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            if (Session["FullName"] != null)
            {
                if (string.IsNullOrEmpty(Session["FullName"].ToString()))
                    return RedirectToAction("Index", "Account");
            }
            else
                return RedirectToAction("Index", "Account");
            bool isUser = GetUserDetailsFromAD();
            clsMedicalExaminerDecision medicalExaminerDecision = new clsMedicalExaminerDecision();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            ViewBag.SpecialityDDM = dBEngine.GetSpecialitiesForDropDown(Convert.ToInt32(Session["LoginUserID"]));
            try
            {
                if (id == null || id == 0)
                {
                    if (Session["PatientID"] != null)
                        id = Convert.ToInt32(Session["PatientID"]);
                    else
                        return RedirectToAction("Index", "Account");
                }
                try
                {
                    medicalExaminerDecision = dBEngine.GetMEOOutcome(id, Convert.ToInt32(Session["LoginUserID"]));
           
                    patientDetails = dBEngine.GetPatientDetails(id, Convert.ToInt32(Session["LoginUserID"]));

                    medicalExaminerDecision.Patient_Id = patientDetails.ToList()[0].PatientId;
                    medicalExaminerDecision.PatientName = patientDetails.ToList()[0].PatientName;
                    medicalExaminerDecision.DOB = patientDetails.ToList()[0].DOB;
                    medicalExaminerDecision.UserRole = patientDetails.ToList()[0].UserRole;
                    medicalExaminerDecision.a = "1a";
                    medicalExaminerDecision.b = "1b";
                    medicalExaminerDecision.c = "1c";
                    medicalExaminerDecision.d = "1d";
                    if (medicalExaminerDecision.ID == 0 || medicalExaminerDecision.ID == null)
                        medicalExaminerDecision.ID = Convert.ToInt32(id);
                    //ViewBag.ReasonGroups = dBEngine.GetReasonGroups();
                }
                catch (Exception ex)
                {
                    dBEngine.LogException(ex.Message, this.ToString(), "ValidateUser", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            List<RolePermission> permissions = dBEngine.GetRolePermission(0, 0, "", "", "",Convert.ToInt32(Session["LoginUserID"]), "MEO Outcome", Convert.ToString(Session["Role"]));
            bool isnoaccess = false;
            if (permissions.Count > 0)
            {
                ViewBag.IsReadOnly = permissions[0].IsReadOnly;
                isnoaccess = permissions[0].NoAccess;
            }
            else
            {
                ViewBag.IsReadOnly = true;
                isnoaccess = true;
            }
            if (isnoaccess == false)
            {
                return View(medicalExaminerDecision);
            }
            else
            {
                return RedirectToAction("NotAuthorizedSJR", new { id = id });
            }
            //return View(medicalExaminerDecision);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formCollection"></param>
        /// <param name="BtnPrevious"></param>
        /// <param name="BtnNext"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult FinalOutcome(FormCollection formCollection, string BtnPrevious, string BtnNext, int id)
        {
            clsMedicalExaminerDecision medicalExaminerDecision = new clsMedicalExaminerDecision();
            bool isMCCDissue = false;
            bool isCoronerReferral = false;
            bool isHospitalPostMortem = false;
            bool isDeathCertificate = false;
            bool isCornerReferralComplete = false;
            bool isCoronerDecisionInquest = false;
            bool isCoronerDecisionPostMortem = false;
            bool isCoronerDecision100A = false;
            bool isCoronerDecisionGPissue = false;
            bool isCoronerDecisionNFAction = false;
            bool isForensicPM = false;
            bool isMEOReviewCompleted = false;
            bool isBuried = false;
            bool isCremated = false;
            bool isBypassedMESystem = false;
            string actionName = "";

            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            DateTime? deathCertificateDate = null;
            if (id == null || id == 0)
            {
                if (Session["PatientID"] != null)
                    id = Convert.ToInt32(Session["PatientID"]);
                else
                    return RedirectToAction("Index", "Account");
            }
            try
            {
                if (BtnPrevious != null)
                    actionName = "Declaration";
                if (BtnNext != null)
                {
                    actionName = "MortalityReview";
                    medicalExaminerDecision = dBEngine.GetMEOOutcome(id, Convert.ToInt32(Session["LoginUserID"]));

                    if (Convert.ToString(formCollection["IssueMCCD"]) == null) isMCCDissue = medicalExaminerDecision.MCCDissue;
                    if (Convert.ToString(formCollection["CoronerReferral"]) == null) isCoronerReferral = medicalExaminerDecision.CoronerReferral;
                    if (Convert.ToString(formCollection["CoronerReferralReason"]) == null) formCollection["CoronerReferralReason"] = medicalExaminerDecision.CoronerReferralReason;
                    if (Convert.ToString(formCollection["PostMortem"]) == null) isHospitalPostMortem = medicalExaminerDecision.HospitalPostMortem;
                    if (Convert.ToString(formCollection["ddlCauseOdfDeath"]) == null) formCollection["ddlCauseOdfDeath"] = medicalExaminerDecision.CauseID.ToString();

                    if (Convert.ToString(formCollection["CoronerReferralComplete"]) == null) isCornerReferralComplete = medicalExaminerDecision.CornerReferralComplete;
                    if (Convert.ToString(formCollection["DeathCertificate"]) == "on") isDeathCertificate = true;
                    if (Convert.ToString(formCollection["Inquest"]) == "on") isCoronerDecisionInquest = true;
                    if (Convert.ToString(formCollection["Post-Mortem"]) == "on") isCoronerDecisionPostMortem = true;
                    if (Convert.ToString(formCollection["100A"]) == "on") isCoronerDecision100A = true;
                    if (Convert.ToString(formCollection["GPIssue"]) == "on") isCoronerDecisionGPissue = true;
                    if (Convert.ToString(formCollection["NoFurAction"]) == "on") isCoronerDecisionNFAction = true;
                    if (Convert.ToString(formCollection["ForensicPM"]) == "on") isForensicPM = true;
                    if (Convert.ToString(formCollection["MEOReviewCompleted"]) == "on") isMEOReviewCompleted = true;
                    if (Convert.ToString(formCollection["IsBuried"]) == "on") isBuried = true;
                    if (Convert.ToString(formCollection["IsCremated"]) == "on") isCremated = true;
                    if (Convert.ToString(formCollection["IsBypassedMESystem"]) == "on") isBypassedMESystem = true;
                    if (medicalExaminerDecision.MEOReviewCompleted == true)
                        isMEOReviewCompleted = true;
                    if (isDeathCertificate == true)
                    {
                        try {
                            if (formCollection["DeathCertificateDate"] != "")
                            {
                                formCollection["DeathCertificateDate"] = formCollection["DeathCertificateDate"].Replace("-", "/");
                                deathCertificateDate = DateTime.ParseExact(formCollection["DeathCertificateDate"].ToString(), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                       
                    }
                    else
                    {
                        deathCertificateDate = null;
                        formCollection["DeathCertificateTime"] = "";
                        formCollection["TimeType"] = "0";
                    }
                    string timetype = "";
                    if (formCollection["DeathCertificateTime"] != "")
                    {
                        if (Convert.ToInt32(formCollection["DeathCertificateTime"].Split(':')[0]) >= 12) timetype = "PM"; else timetype = "AM";
                    }
                    if (Convert.ToString(formCollection["ddlCauseOdfDeath"]) == null) formCollection["ddlCauseOdfDeath"] = medicalExaminerDecision.CauseID.ToString();
                    int retVal = dBEngine.UpdateFinalOutcome(isDeathCertificate, isCornerReferralComplete, isCoronerDecisionInquest, isCoronerDecisionPostMortem,
                       isCoronerDecision100A, isCoronerDecisionGPissue, formCollection["CauseOfDeath1Final"], formCollection["CauseOfDeath2Final"], formCollection["CauseOfDeath3Final"],
                       formCollection["CauseOfDeath4Final"], deathCertificateDate, formCollection["DeathCertificateTime"], timetype, formCollection["ddlCauseOdfDeath"], id, isCoronerDecisionNFAction, isForensicPM, isBuried,
                       isCremated, isBypassedMESystem, isMEOReviewCompleted, 0, Convert.ToInt32(Session["LoginUserID"]));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return RedirectToAction(actionName, new { id = id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formCollection"></param>
        /// <param name="BtnPrevious"></param>
        /// <param name="BtnNext"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult OtherReferrals(FormCollection formCollection, string BtnPrevious, string BtnNext, int? id)
        {
            string actionName = "";
            bool isPatientSafetySIRI = false;
            bool isChildDeathCoordinator = false;
            bool isLearningDisabilityNurse = false;
            bool isHeadOfCompliance = false;
            bool isPALsComplaints = false;
            bool isWardTeam = false;
            bool isOther = false;
            bool isSafeGuard = false;
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            string section = "";
            string repeatingsection = "<tr> <td style = 'padding:5px; font-family:Arial, Helvetica, sans-serif; font-size:15px; color:#000000;border-bottom:1px #70a397 solid;  border - right:1px #70a397 solid; border-left:1px #70a397 solid;'>##PatientID##</td> <td style = 'padding:5px; font-family:Arial, Helvetica, sans-serif; font-size:15px; color:#000000;border-bottom:1px #70a397 solid; border - right:1px #70a397 solid; border-left:0px #70a397 solid;'>##PatientName##</td><td style = 'padding:5px; font-family:Arial, Helvetica, sans-serif; font-size:15px; color:#000000;border-bottom:1px #70a397 solid; border - right:1px #70a397 solid; border-left:0px #70a397 solid;'>##DateOfDeath##</td><td style = 'padding:5px; font-family:Arial, Helvetica, sans-serif; font-size:15px; color:#000000;border-bottom:1px #70a397 solid; border - right:1px #70a397 solid; border-left:0px #70a397 solid;'>##ReferralType##</td><td style = 'padding:5px; font-family:Arial, Helvetica, sans-serif; font-size:15px; color:#000000;border-bottom:1px #70a397 solid; border - right:1px #70a397 solid; border-left:0px #70a397 solid;'>##ReferralDate##</td><td style = 'padding:5px; font-family:Arial, Helvetica, sans-serif; font-size:15px; color:#000000;border-bottom:1px #70a397 solid; border - right:1px #70a397 solid; border-left:0px #70a397 solid;'>##ReferredBy##</td><td style = 'padding:5px; font-family:Arial, Helvetica, sans-serif; font-size:15px; color:#000000;border-bottom:1px #70a397 solid; border - right:1px #70a397 solid; border-left:0px #70a397 solid;'>##ReferralReason##</td></tr> ";

            try
            {
                if (id == null || id == 0)
                {
                    if (Session["PatientID"] != null)
                        id = Convert.ToInt32(Session["PatientID"]);
                    else
                        return RedirectToAction("Index", "Account");
                }
                int patientid = Convert.ToInt32(id);
                if (BtnPrevious != null)
                {
                    actionName = "SJRAssessmentTriage";
                }
                if (BtnNext != null)
                {
                    actionName = "PositiveFeedback";
                    if (Convert.ToString(formCollection["PatientSafetySIRI"]) == "on") isPatientSafetySIRI = true;
                    if (Convert.ToString(formCollection["ChildDeathCoordinator"]) == "on") isChildDeathCoordinator = true;
                    if (Convert.ToString(formCollection["LearningDisabilityNurse"]) == "on") isLearningDisabilityNurse = true;
                    if (Convert.ToString(formCollection["HeadOfCompliance"]) == "on") isHeadOfCompliance = true;
                    if (Convert.ToString(formCollection["PALsComplaints"]) == "on") isPALsComplaints = true;
                    if (Convert.ToString(formCollection["WardTeam"]) == "on") isWardTeam = true;
                    if (Convert.ToString(formCollection["Other"]) == "on") isOther = true;
                    if (Convert.ToString(formCollection["SafeGuard"]) == "on") isSafeGuard = true;
                    int retVal = dBEngine.UpdateOtherReferrals(isPatientSafetySIRI, isChildDeathCoordinator, isLearningDisabilityNurse, isHeadOfCompliance, isPALsComplaints, isWardTeam, isOther,
                       formCollection["PatientSafetySIRIReason"], formCollection["HeadOfComplianceReason"], formCollection["PALsComplaintsReason"], formCollection["WardTeamReason"], formCollection["OtherReason"], isSafeGuard, id,Convert.ToInt32(Session["LoginUserID"]));
                    int dbReturn = 0;
                    if (isPatientSafetySIRI == true || isChildDeathCoordinator == true || isLearningDisabilityNurse == true || isHeadOfCompliance == true || isPALsComplaints == true || isWardTeam == true || isOther == true || isSafeGuard == true)
                    {
                        List<clsPatientDetails> patient = dBEngine.GetPatientDetailsByID(id, Convert.ToInt32(Session["LoginUserID"]));
                        if(isPatientSafetySIRI)
                        {
                            section = repeatingsection.Replace("##PatientID##", patient[0].PatientId).Replace("##PatientName##", patient[0].PatientName).Replace("##DateOfDeath##", patient[0].DateofDeath).Replace("##ReferralType##", "Patient Safety for SIRI Scoping").Replace("##ReferralDate##", System.DateTime.Now.ToString("dd/MM/yyyy")).Replace("##ReferredBy##", Convert.ToString(Session["FullName"])).Replace("##ReferralReason##", formCollection["PatientSafetySIRIReason"]);
                            dbReturn = dBEngine.UpdateOtherReferralDetails(patient[0].PatientId, patient[0].PatientName, DateTime.ParseExact(patient[0].DateofDeath, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), "Patient Safety for SIRI Scoping", System.DateTime.Now, Convert.ToString(Session["FullName"]), formCollection["PatientSafetySIRIReason"], Convert.ToInt32(Session["LoginUserID"]));
                            if (patient.Count > 0)
                            {
                                //Specialities speciality = dBEngine.GetSpecialitybyID(Convert.ToInt32(formCollection["ddlCoronerReferral"]));


                                List<NotificationSettings> settings = dBEngine.GetNotificationSettingsByTrigger("ME Patient Safety for SIRI Scoping", Convert.ToInt32(Session["LoginUserID"]));
                                if (settings.Count > 0)
                                {
                                    for (int counter = 0; counter < settings.Count; counter++)
                                    {
                                        List<AppUsers> users = dBEngine.GetUsersByRoleID(settings[counter].RoleID, Convert.ToInt32(Session["LoginUserID"]));
                                        PatientNotification notification = dBEngine.GetNotificationSentStatusByPatientID(patientid, "ME Patient Safety for SIRI Scoping", Convert.ToInt32(Session["LoginUserID"]));
                                        if (notification.NotificationTrigger == null)
                                        {
                                            for (int count = 0; count < users.Count; count++)
                                            {
                                                SendEmail(users[count].EmailID, "CORS - Patient Safety for SIRI Scoping Referral Requested", settings[counter].EmailTemplate.Replace("</table></td>", section + "</table></td>"));
                                            }                                            
                                        }
                                    }
                                    dBEngine.UpdatePatientNotification(patientid, "ME Patient Safety for SIRI Scoping", Convert.ToInt32(Session["LoginUserID"]));
                                }
                            }
                        }
                        if(isChildDeathCoordinator)
                        {
                            section = repeatingsection.Replace("##PatientID##", patient[0].PatientId).Replace("##PatientName##", patient[0].PatientName).Replace("##DateOfDeath##", patient[0].DateofDeath).Replace("##ReferralType##", "Child Death Co-ordinator").Replace("##ReferralDate##", System.DateTime.Now.ToString("dd/MM/yyyy")).Replace("##ReferredBy##", Convert.ToString(Session["FullName"])).Replace("##ReferralReason##", "");
                            dbReturn = dBEngine.UpdateOtherReferralDetails(patient[0].PatientId, patient[0].PatientName, DateTime.ParseExact(patient[0].DateofDeath, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), "Child Death Co-ordinator", System.DateTime.Now, Convert.ToString(Session["FullName"]), "", Convert.ToInt32(Session["LoginUserID"]));
                            if (patient.Count > 0)
                            {
                                //Specialities speciality = dBEngine.GetSpecialitybyID(Convert.ToInt32(formCollection["ddlCoronerReferral"]));


                                List<NotificationSettings> settings = dBEngine.GetNotificationSettingsByTrigger("ME Child Death Co-ordinator", Convert.ToInt32(Session["LoginUserID"]));
                                if (settings.Count > 0)
                                {
                                    for (int counter = 0; counter < settings.Count; counter++)
                                    {
                                        List<AppUsers> users = dBEngine.GetUsersByRoleID(settings[counter].RoleID, Convert.ToInt32(Session["LoginUserID"]));
                                        PatientNotification notification = dBEngine.GetNotificationSentStatusByPatientID(patientid, "ME Child Death Co-ordinator", Convert.ToInt32(Session["LoginUserID"]));
                                        if (notification.NotificationTrigger == null)
                                        {
                                            for (int count = 0; count < users.Count; count++)
                                            {
                                                SendEmail(users[count].EmailID, "CORS - Child Death Co-ordinator Referral Requested", settings[counter].EmailTemplate.Replace("</table></td>", section + "</table></td>"));
                                            }                                            
                                        }
                                    }
                                    dBEngine.UpdatePatientNotification(patientid, "ME Child Death Co-ordinator", Convert.ToInt32(Session["LoginUserID"]));
                                }
                            }
                        }
                        if (isLearningDisabilityNurse)
                        {
                            section = repeatingsection.Replace("##PatientID##", patient[0].PatientId).Replace("##PatientName##", patient[0].PatientName).Replace("##DateOfDeath##", patient[0].DateofDeath).Replace("##ReferralType##", "Learning Disability Nurse for LEDER reporting").Replace("##ReferralDate##", System.DateTime.Now.ToString("dd/MM/yyyy")).Replace("##ReferredBy##", Convert.ToString(Session["FullName"])).Replace("##ReferralReason##", "");
                            dbReturn = dBEngine.UpdateOtherReferralDetails(patient[0].PatientId, patient[0].PatientName, DateTime.ParseExact(patient[0].DateofDeath, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), "Learning Disability Nurse for LEDER reporting", System.DateTime.Now, Convert.ToString(Session["FullName"]), "", Convert.ToInt32(Session["LoginUserID"]));
                            if (patient.Count > 0)
                            {
                                //Specialities speciality = dBEngine.GetSpecialitybyID(Convert.ToInt32(formCollection["ddlCoronerReferral"]));


                                List<NotificationSettings> settings = dBEngine.GetNotificationSettingsByTrigger("ME Learning Disability Nurse for LEDER reporting", Convert.ToInt32(Session["LoginUserID"]));
                                if (settings.Count > 0)
                                {
                                    for (int counter = 0; counter < settings.Count; counter++)
                                    {
                                        List<AppUsers> users = dBEngine.GetUsersByRoleID(settings[counter].RoleID, Convert.ToInt32(Session["LoginUserID"]));
                                        PatientNotification notification = dBEngine.GetNotificationSentStatusByPatientID(patientid, "ME Learning Disability Nurse for LEDER reporting", Convert.ToInt32(Session["LoginUserID"]));
                                        if (notification.NotificationTrigger == null)
                                        {
                                            for (int count = 0; count < users.Count; count++)
                                            {
                                                SendEmail(users[count].EmailID, "CORS - Learning Disability Nurse for LEDER reporting Referral Requested", settings[counter].EmailTemplate.Replace("</table></td>", section + "</table></td>"));
                                            }
                                            
                                        }
                                    }
                                    dBEngine.UpdatePatientNotification(patientid, "ME Learning Disability Nurse for LEDER reporting", Convert.ToInt32(Session["LoginUserID"]));
                                }
                            }
                        }
                        if (isHeadOfCompliance)
                        {
                            section = repeatingsection.Replace("##PatientID##", patient[0].PatientId).Replace("##PatientName##", patient[0].PatientName).Replace("##DateOfDeath##", patient[0].DateofDeath).Replace("##ReferralType##", "Head of Compliance for CQC reporting").Replace("##ReferralDate##", System.DateTime.Now.ToString("dd/MM/yyyy")).Replace("##ReferredBy##", Convert.ToString(Session["FullName"])).Replace("##ReferralReason##", formCollection["HeadOfComplianceReason"]);
                            dbReturn = dBEngine.UpdateOtherReferralDetails(patient[0].PatientId, patient[0].PatientName, DateTime.ParseExact(patient[0].DateofDeath, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), "Head of Compliance for CQC Reporting", System.DateTime.Now, Convert.ToString(Session["FullName"]), formCollection["HeadOfComplianceReason"], Convert.ToInt32(Session["LoginUserID"]));
                            if (patient.Count > 0)
                            {
                                //Specialities speciality = dBEngine.GetSpecialitybyID(Convert.ToInt32(formCollection["ddlCoronerReferral"]));


                                List<NotificationSettings> settings = dBEngine.GetNotificationSettingsByTrigger("ME Head of Compliance for CQC Reporting", Convert.ToInt32(Session["LoginUserID"]));
                                if (settings.Count > 0)
                                {
                                    for (int counter = 0; counter < settings.Count; counter++)
                                    {
                                        List<AppUsers> users = dBEngine.GetUsersByRoleID(settings[counter].RoleID, Convert.ToInt32(Session["LoginUserID"]));
                                        PatientNotification notification = dBEngine.GetNotificationSentStatusByPatientID(patientid, "ME Head of Compliance for CQC Reporting", Convert.ToInt32(Session["LoginUserID"]));
                                        if (notification.NotificationTrigger == null)
                                        {
                                            for (int count = 0; count < users.Count; count++)
                                            {
                                                SendEmail(users[count].EmailID, "CORS - Head of Compliance for CQC Reporting Referral Requested", settings[counter].EmailTemplate.Replace("</table></td>", section + "</table></td>"));
                                            }
                                            
                                        }
                                    }
                                    dBEngine.UpdatePatientNotification(patientid, "ME Learning Disability Nurse for LEDER reporting", Convert.ToInt32(Session["LoginUserID"]));
                                }
                            }
                        }
                        if (isPALsComplaints)
                        {
                            section = repeatingsection.Replace("##PatientID##", patient[0].PatientId).Replace("##PatientName##", patient[0].PatientName).Replace("##DateOfDeath##", patient[0].DateofDeath).Replace("##ReferralType##", "PALs/ Complaints").Replace("##ReferralDate##", System.DateTime.Now.ToString("dd/MM/yyyy")).Replace("##ReferredBy##", Convert.ToString(Session["FullName"])).Replace("##ReferralReason##", formCollection["PALsComplaintsReason"]);
                            dbReturn = dBEngine.UpdateOtherReferralDetails(patient[0].PatientId, patient[0].PatientName, DateTime.ParseExact(patient[0].DateofDeath, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), "PALs Complaints", System.DateTime.Now, Convert.ToString(Session["FullName"]), formCollection["PALsComplaintsReason"], Convert.ToInt32(Session["LoginUserID"]));
                            if (patient.Count > 0)
                            {
                                //Specialities speciality = dBEngine.GetSpecialitybyID(Convert.ToInt32(formCollection["ddlCoronerReferral"]));


                                List<NotificationSettings> settings = dBEngine.GetNotificationSettingsByTrigger("ME PALs Complaints", Convert.ToInt32(Session["LoginUserID"]));
                                if (settings.Count > 0)
                                {
                                    for (int counter = 0; counter < settings.Count; counter++)
                                    {
                                        List<AppUsers> users = dBEngine.GetUsersByRoleID(settings[counter].RoleID, Convert.ToInt32(Session["LoginUserID"]));
                                        PatientNotification notification = dBEngine.GetNotificationSentStatusByPatientID(patientid, "ME PALs Complaints", Convert.ToInt32(Session["LoginUserID"]));
                                        if (notification.NotificationTrigger == null)
                                        {
                                            for (int count = 0; count < users.Count; count++)
                                            {
                                                SendEmail(users[count].EmailID, "CORS - PALs Complaints Referral Requested", settings[counter].EmailTemplate.Replace("</table></td>", section + "</table></td>"));
                                            }                                            
                                        }
                                    }
                                    dBEngine.UpdatePatientNotification(patientid, "ME PALs Complaints", Convert.ToInt32(Session["LoginUserID"]));
                                }
                            }
                        }
                        if (isWardTeam)
                        {
                            section = repeatingsection.Replace("##PatientID##", patient[0].PatientId).Replace("##PatientName##", patient[0].PatientName).Replace("##DateOfDeath##", patient[0].DateofDeath).Replace("##ReferralType##", "Referred to Ward team").Replace("##ReferralDate##", System.DateTime.Now.ToString("dd/MM/yyyy")).Replace("##ReferredBy##", Convert.ToString(Session["FullName"])).Replace("##ReferralReason##", formCollection["WardTeamReason"]);
                            dbReturn = dBEngine.UpdateOtherReferralDetails(patient[0].PatientId, patient[0].PatientName, DateTime.ParseExact(patient[0].DateofDeath, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), "Referred to Ward team", System.DateTime.Now, Convert.ToString(Session["FullName"]), formCollection["WardTeamReason"], Convert.ToInt32(Session["LoginUserID"]));
                            if (patient.Count > 0)
                            {
                                //Specialities speciality = dBEngine.GetSpecialitybyID(Convert.ToInt32(formCollection["ddlCoronerReferral"]));


                                List<NotificationSettings> settings = dBEngine.GetNotificationSettingsByTrigger("ME Referred to Ward team", Convert.ToInt32(Session["LoginUserID"]));
                                if (settings.Count > 0)
                                {
                                    for (int counter = 0; counter < settings.Count; counter++)
                                    {
                                        List<AppUsers> users = dBEngine.GetUsersByRoleID(settings[counter].RoleID, Convert.ToInt32(Session["LoginUserID"]));
                                        PatientNotification notification = dBEngine.GetNotificationSentStatusByPatientID(patientid, "ME Referred to Ward team", Convert.ToInt32(Session["LoginUserID"]));
                                        if (notification.NotificationTrigger == null)
                                        {
                                            for (int count = 0; count < users.Count; count++)
                                            {
                                                SendEmail(users[count].EmailID, "CORS - Referred to Ward team", settings[counter].EmailTemplate.Replace("</table></td>", section + "</table></td>"));
                                            }                                            
                                        }
                                    }
                                    dBEngine.UpdatePatientNotification(patientid, "ME Referred to Ward team", Convert.ToInt32(Session["LoginUserID"]));
                                }
                            }
                        }
                        if (isOther)
                        {
                            section += repeatingsection.Replace("##PatientID##", patient[0].PatientId).Replace("##PatientName##", patient[0].PatientName).Replace("##DateOfDeath##", patient[0].DateofDeath).Replace("##ReferralType##", "Other").Replace("##ReferralDate##", System.DateTime.Now.ToString("dd/MM/yyyy")).Replace("##ReferredBy##", Convert.ToString(Session["FullName"])).Replace("##ReferralReason##", formCollection["OtherReason"]);
                            dbReturn = dBEngine.UpdateOtherReferralDetails(patient[0].PatientId, patient[0].PatientName, DateTime.ParseExact(patient[0].DateofDeath, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), "Other", System.DateTime.Now, Convert.ToString(Session["FullName"]), formCollection["OtherReason"], Convert.ToInt32(Session["LoginUserID"]));
                            if (patient.Count > 0)
                            {
                                //Specialities speciality = dBEngine.GetSpecialitybyID(Convert.ToInt32(formCollection["ddlCoronerReferral"]));


                                List<NotificationSettings> settings = dBEngine.GetNotificationSettingsByTrigger("ME Other", Convert.ToInt32(Session["LoginUserID"]));
                                if (settings.Count > 0)
                                {
                                    for (int counter = 0; counter < settings.Count; counter++)
                                    {
                                        List<AppUsers> users = dBEngine.GetUsersByRoleID(settings[counter].RoleID, Convert.ToInt32(Session["LoginUserID"]));
                                        PatientNotification notification = dBEngine.GetNotificationSentStatusByPatientID(patientid, "ME Other", Convert.ToInt32(Session["LoginUserID"]));
                                        if (notification.NotificationTrigger == null)
                                        {
                                            for (int count = 0; count < users.Count; count++)
                                            {
                                                SendEmail(users[count].EmailID, "CORS - Other Referral Requested", settings[counter].EmailTemplate.Replace("</table></td>", section + "</table></td>"));
                                            }                                            
                                        }
                                    }
                                    dBEngine.UpdatePatientNotification(patientid, "ME Other", Convert.ToInt32(Session["LoginUserID"]));
                                }
                            }
                        }
                        if (isSafeGuard)
                        {
                            section = repeatingsection.Replace("##PatientID##", patient[0].PatientId).Replace("##PatientName##", patient[0].PatientName).Replace("##DateOfDeath##", patient[0].DateofDeath).Replace("##ReferralType##", "SafeGuarding Team Notified").Replace("##ReferralDate##", System.DateTime.Now.ToString("dd/MM/yyyy")).Replace("##ReferredBy##", Convert.ToString(Session["FullName"])).Replace("##ReferralReason##", "");
                            dbReturn = dBEngine.UpdateOtherReferralDetails(patient[0].PatientId, patient[0].PatientName, DateTime.ParseExact(patient[0].DateofDeath, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), "SafeGuarding Team Notified", System.DateTime.Now, Convert.ToString(Session["FullName"]), "", Convert.ToInt32(Session["LoginUserID"]));
                            if (patient.Count > 0)
                            {
                                //Specialities speciality = dBEngine.GetSpecialitybyID(Convert.ToInt32(formCollection["ddlCoronerReferral"]));


                                List<NotificationSettings> settings = dBEngine.GetNotificationSettingsByTrigger("ME SafeGuarding Team Notified", Convert.ToInt32(Session["LoginUserID"]));
                                if (settings.Count > 0)
                                {
                                    for (int counter = 0; counter < settings.Count; counter++)
                                    {
                                        List<AppUsers> users = dBEngine.GetUsersByRoleID(settings[counter].RoleID, Convert.ToInt32(Session["LoginUserID"]));
                                        PatientNotification notification = dBEngine.GetNotificationSentStatusByPatientID(patientid, "ME SafeGuarding Team Notified", Convert.ToInt32(Session["LoginUserID"]));
                                        if (notification.NotificationTrigger == null)
                                        {
                                            for (int count = 0; count < users.Count; count++)
                                            {
                                                SendEmail(users[count].EmailID, "CORS - SafeGuarding Team Notified", settings[counter].EmailTemplate.Replace("</table></td>", section + "</table></td>"));
                                            }                                            
                                        }
                                    }
                                    dBEngine.UpdatePatientNotification(patientid, "ME SafeGuarding Team Notified", Convert.ToInt32(Session["LoginUserID"]));
                                }
                            }
                        }                        
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return RedirectToAction(actionName, new { id = id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formCollection"></param>
        /// <param name="BtnPrevious"></param>
        /// <param name="BtnNext"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        //[HttpPost]
        public ActionResult SaveDeclaration(bool Isdec,  int? id, int specialityid)
        {
            string actionName = "";
            bool isDeclaration = false;
            bool isChildDeathCoordinator = false;
            bool isLearningDisabilityNurse = false;
            bool isHeadOfCompliance = false;
            bool isPALsComplaints = false;
            bool isWardTeam = false;
            bool isOther = false;
            bool isSafeGuard = false;
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            bool retVal;
            try
            {
                if (id == null || id == 0)
                {
                    if (Session["PatientID"] != null)
                        id = Convert.ToInt32(Session["PatientID"]);
                    else
                        return RedirectToAction("Index", "Account");
                }
                    //if (Convert.ToString(formCollection["Declaration"]) == "on") isDeclaration = true;
                     retVal = dBEngine.UpdateDeclaration(Isdec, Convert.ToInt32(Session["LoginUserID"]), DateTime.Now.ToString("dd/MM/yyyy"), "Royal Berkshire Hospital", id);
                 
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //return RedirectToAction(actionName, new { id = id });
            return Json(retVal);
        }

        public ActionResult COVIDBreathingSupportReport(FormCollection formCollection, bool isReset = false)
        {
            Session["PatientListPageNo"] = null;
            Session["PatientListSearchText"] = null;
            Session["PatientListPageSize"] = null;
            Session["PatientListColumn"] = null;
            Session["PatientLocation"] = null;
            Session["IsCurrentIPCount"] = null;
            Session["IsNotUpdatedLast20HoursCount"] = null;
            Session["IsNotUpdatedLast12HoursCount"] = null;
            Session["IsUpdatedLast12HoursCount"] = null;
            Session["IsNewPositive"] = null;
            Session["IsPostiveBacklog"] = null;
            Session["IsPositiveBacklogPending"] = null;
            Session["IsPositiveLive"] = null;
            Session["IsPositiveLivePending"] = null;
            Session["IsDischargeDeath"] = null;
            Session["IsDischargeDeathBacklog"] = null;
            Session["IsDischargeDeathBacklogPending"] = null;
            Session["IsDischargeDeathLive"] = null;
            Session["IsDischargeDeathLivePending"] = null;
            Session["IsDeath"] = null;
            Session["IsDeathBacklog"] = null;
            Session["IsDeathBacklogPending"] = null;
            Session["IsDeathLive"] = null;
            Session["IsDeathLivePending"] = null;
            Session["IsExternalComms"] = false;
            Session["IsBreathing"] = true;
            Session["IsLevelOfCare"] = false;
            Session["BreathingSupportReportPageSize"] = 50;
            if (string.IsNullOrEmpty(Convert.ToString(Session["BreathingReportTestResults"])))
            {
                Session["BreathingReportTestResults"] = "0";
            }
            if (!isReset)
            {
                if(formCollection["ddlTestResults"] != null)
                    Session["BreathingReportTestResults"] = formCollection["ddlTestResults"];
            }                    
            else
                Session["BreathingReportTestResults"] = "0";

            
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            COVIDPatientFilterDDM filterddm = dBEngine.GetCOVIDpatientFilterDDM(Convert.ToInt32(Session["LoginUserID"]));
            ViewBag.TestResults = filterddm.lstTestResult;
            BreathingSupportReport breathingreport = dBEngine.GetCOVIDBreathingSupportReport(1, Convert.ToInt32(Session["BreathingSupportReportPageSize"]), Convert.ToString(Session["BreathingReportTestResults"]), "", Convert.ToString(Session["BreathingReportTestResults"]), Convert.ToInt32(Session["LoginUserID"]));
            int totalRecords = 0;
            if (breathingreport.BreathingTracker.Count > 0)
            {
                totalRecords = breathingreport.BreathingTracker[0].TotalRecords;
            }

            int totalPagesCount = 0;
            if (Convert.ToInt32(Session["BreathingSupportReportPageSize"]) != -1)
                totalPagesCount = (int)Math.Ceiling((float)totalRecords / Convert.ToInt32(Session["BreathingSupportReportPageSize"]));
            else
                totalPagesCount = 1;

            ViewBag.PageNumber = 1;
            ViewBag.SearchText = "";
            ViewBag.TotalPagesCount = totalPagesCount;
            ViewBag.TotalRecordCount = totalRecords;
            ViewBag.ModalRecordCount = breathingreport.BreathingTracker.Count;
            ViewBag.PageSize = Convert.ToInt32(Session["BreathingSupportReportPageSize"]);
            return View(breathingreport);
        }



        public ActionResult COVIDDashboard(FormCollection formCollection, bool isReset = false, bool buttonsubmit = false)
        {
            Session["TestID"] = 0;
            Session["COVIDPatientID"] = "";
            Session.Timeout = 30;
            Session["PatientDashboard"] = false;
            Session["TestDashboard"] = true;
            Session["COVIDPatientStartDate"] = null;
            Session["COVIDPatientEndDate"] = null;
            if (string.IsNullOrEmpty(Convert.ToString(Session["TestPatientType"])))
                Session["TestPatientType"] = "0";
            if (string.IsNullOrEmpty(Convert.ToString(Session["TestAgeGroup"])))
                Session["TestAgeGroup"] = "0";
            if (string.IsNullOrEmpty(Convert.ToString(Session["TestBreathingStatus"])))
                Session["TestBreathingStatus"] = "0";
            if (string.IsNullOrEmpty(Convert.ToString(Session["TestTestResults"])))
                Session["TestTestResults"] = "0";
            if (string.IsNullOrEmpty(Convert.ToString(Session["TestAdmissionStatus"])))
                Session["TestAdmissionStatus"] = "0";
            if (string.IsNullOrEmpty(Convert.ToString(Session["TestLastLocation"])))
                Session["TestLastLocation"] = "0";
            Session["TotalTestsOrdered"] = null;
            Session["TotalTestsOrderedInPatient"] = null;
            Session["PositiveTestCases"] = null;
            Session["PositiveTestCasesICU"] = null;
            Session["NegativeTestCases"] = null;
            Session["PositiveDischarges"] = null;
            Session["TestPageSize"] = 10;

            Session["PositiveDeaths"] = null;
            Session["PositiveInPatient"] = null;
            Session["PositiveInPatientLast24hours"] = null;
            Session["NegativeInPatientLast24hours"] = null;
            Session["PositiveICUInPatientLast24hours"] = null;
            Session["PendingICUInPatient"] = null;
            Session["PositiveDischargeCountLast24hours"] = null;
            Session["PositiveDeathCountLast24hours"] = null;
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            COVIDFilterDDM filterddm = new COVIDFilterDDM();
            COVIDDefaultDate defaultdate = new COVIDDefaultDate();
            List<clsCOVIDDetails> coviddetails = new List<clsCOVIDDetails>();
            List<RolePermission> permissions = new List<Common.RolePermission>();
            
            try
            {                
                if (Convert.ToInt32(Session["LoginUserID"]) > 0)
                {
                    filterddm = dBEngine.GetCOVIDFilterDDM(Convert.ToInt32(Session["LoginUserID"]));
                    ViewBag.LoadTestStatus = filterddm.lstTestStatus;
                    ViewBag.TestResults = filterddm.lstTestResult;
                    ViewBag.TestOrderLocation = filterddm.lstTestOrderLocation;
                    ViewBag.LastLocation = filterddm.lstLastLocation;
                    ViewBag.AdmissionStatus = filterddm.lstAdmissionStatus;
                    ViewBag.Notification = filterddm.lstNotification;
                    ViewBag.UpdatedSource = filterddm.datamanagement.SourceSystem;
                    ViewBag.UpdatedDate = filterddm.datamanagement.UpdateDate;
                    ViewBag.UpdatedTime = filterddm.datamanagement.UpdateTime;
                    //int pagesize = Convert.ToInt32(formCollection["tblPatientDetails_length"]);
                    if (Session["TestPageSize"] == null)
                        Session["TestPageSize"] = 10;
                    if (Request.HttpMethod != "POST")
                    {
                        defaultdate = dBEngine.GetCOVIDDefaultDate(Convert.ToInt32(Session["LoginUserID"]));
                            if (isReset)
                            {
                                if (defaultdate.StartDate == null)
                                    Session["COVIDStartDate"] = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
                                else
                                    Session["COVIDStartDate"] = defaultdate.StartDate;
                                if (defaultdate.EndDate == null)
                                    Session["COVIDEndDate"] = DateTime.Now.ToString("dd/MM/yyyy");
                                else
                                    Session["COVIDEndDate"] = defaultdate.EndDate;
                                Session["TestNotification"] = "0";
                                Session["TestTestStatus"] = "0";
                                Session["TestTestResults"] = "0";
                                Session["TestTestOrderLocation"] = "0";
                                Session["TestLastLocation"] = "0";
                                Session["TestAdmissionStatus"] = "0";
                                Session["IsTotalTestOrders"] = false;
                                Session["IsPostive"] = false;
                                Session["IsNegative"] = false;
                                Session["IsPending"] = false;
                                Session["IsPostiveNotNotified"] = false;
                                Session["IsPostiveNotNotified"] = false;
                                Session["IsNegativeNotNotified"] = false;
                                Session["IsPendingGreaterthan2days"] = false;
                                Session["IsTotalTestOrdersLast24hours"] = false;
                                Session["IsPositiveLast24hours"] = false;
                                Session["IsPositiveInPatientLast24hours"] = false;
                                Session["IsNegativeLast24hours"] = false;
                                Session["IsPostiveNotNotifiedLast24hours"] = false;
                                Session["IsInPatientNotNotifiedLast24hours"] = false;
                                Session["IsNegativeNotNotifiedLast24hours"] = false;
                                Session["IsPositiveAdmissions"] = false;
                                Session["IsPositiveDeaths"] = false;
                                Session["IsPositiveDischargesLast24hours"] = false;
                                Session["IsPositiveDeathLast24hours"] = false;
                                Session["TestPageNo"] = 1;
                                Session["TestPageSize"] = 10;
                                Session["TestColumn"] = "TestResultDateTime";
                                Session["TestSortType"] = "DESC";
                        }
                            else
                            {
                                if (defaultdate.StartDate == null)
                                    Session["COVIDStartDate"] = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
                                else
                                    Session["COVIDStartDate"] = defaultdate.StartDate;
                                if (defaultdate.EndDate == null)
                                    Session["COVIDEndDate"] = DateTime.Now.ToString("dd/MM/yyyy");
                                else
                                    Session["COVIDEndDate"] = defaultdate.EndDate;
                                if (Session["TestNotification"] == null)
                                    Session["TestNotification"] = "0";
                                if (Session["TestTestStatus"] == null)
                                    Session["TestTestStatus"] = "0";
                                if (Session["TestTestResults"] == null)
                                    Session["TestTestResults"] = "0";
                                if (Session["TestTestOrderLocation"] == null)
                                    Session["TestTestOrderLocation"] = "0";
                                if (Session["TestLastLocation"] == null)
                                    Session["TestLastLocation"] = "0";
                                if (Session["TestAdmissionStatus"] == null)
                                    Session["TestAdmissionStatus"] = "0";
                                Session["IsTotalTestOrders"] = null;
                                Session["IsPostive"] = null;
                                Session["IsNegative"] = null;
                                Session["IsPending"] = null;
                                Session["PendingTestsOver120mins"] = null;
                                Session["AdmissionCount"] = null;
                                Session["DeathCount"] = null;

                                if (Session["IsPostiveNotNotified"] == null)
                                    Session["IsPostiveNotNotified"] = false;
                                if (Session["IsNegativeNotNotified"] == null)
                                    Session["IsNegativeNotNotified"] = false;
                                if (Session["IsPendingGreaterthan2days"] == null)
                                    Session["IsPendingGreaterthan2days"] = false;
                                if (Session["NotifiedAdmissionCount"] == null)
                                    Session["NotifiedAdmissionCount"] = false;
                                if (Session["NotifiedDeathCount"] == null)
                                    Session["NotifiedDeathCount"] = false;
                                if (Session["TestPageNo"] == null)
                                    Session["TestPageNo"] = 1;
                                if (Session["TestPageSize"] == null)
                                    Session["TestPageSize"] = 10;
                                if (Session["TestColumn"] == null)
                                    Session["TestColumn"] = "TestResultDateTime";
                                if (Session["TestSortType"] == null)
                                    Session["TestSortType"] = "DESC";
                                if (Session["IsTotalTestOrdersLast24hours"] == null)
                                    Session["IsTotalTestOrdersLast24hours"] = false;
                                if (Session["IsPositiveLast24hours"] == null)
                                    Session["IsPositiveLast24hours"] = false;
                                if (Session["IsPositiveInPatientLast24hours"] == null)
                                    Session["IsPositiveInPatientLast24hours"] = false;
                                if (Session["IsNegativeLast24hours"] == null)
                                    Session["IsNegativeLast24hours"] = false;
                                if (Session["IsPositiveDeathLast24hours"] == null)
                                    Session["IsPositiveDeathLast24hours"] = false;
                                if (Session["IsPositiveDischargesLast24hours"] == null)
                                    Session["IsPositiveDischargesLast24hours"] = false;
                                if (Session["IsPostiveNotNotifiedLast24hours"] == null)
                                    Session["IsPostiveNotNotifiedLast24hours"] = false;
                                if (Session["IsInPatientNotNotifiedLast24hours"] == null)
                                    Session["IsInPatientNotNotifiedLast24hours"] = false;
                                if (Session["IsNegativeNotNotifiedLast24hours"] == null)
                                    Session["IsNegativeNotNotifiedLast24hours"] = false;
                            }
                    }
                    else
                    {
                        if (isReset == false && buttonsubmit)
                        {
                            Session["COVIDStartDate"] = formCollection["txtStartDate"].Replace("Order Start Date ", "");
                            Session["COVIDEndDate"] = formCollection["txtEndDate"].Replace("Order End Date ", "");
                            Session["TestNotification"] = formCollection["ddlNotification"];
                            Session["TestTestStatus"] = formCollection["ddlTestStatus"];
                            Session["TestTestResults"] = formCollection["ddlTestResults"];
                            Session["TestTestOrderLocation"] = formCollection["ddlTestOrderLocation"];
                            Session["TestLastLocation"] = formCollection["ddlLastLocation"];
                            Session["TestAdmissionStatus"] = formCollection["ddlAdmissionStatus"];

                            Session["IsTotalTestOrders"] = null;
                            Session["IsPostive"] = null;
                            Session["IsNegative"] = null;
                            Session["IsPending"] = null;
                            Session["PendingTestsOver120mins"] = null;
                            Session["AdmissionCount"] = null;
                            Session["DeathCount"] = null;
                            Session["IsPostiveNotNotified"] = null;
                            Session["IsNegativeNotNotified"] = null;
                            Session["IsPendingGreaterthan2days"] = null;
                            Session["NotifiedAdmissionCount"] = null;
                            Session["NotifiedDeathCount"] = null;

                            Session["IsTotalTestOrdersLast24hours"] = null;
                            Session["IsPositiveLast24hours"] = null;
                            Session["IsPositiveInPatientLast24hours"] = null;
                            Session["IsNegativeLast24hours"] = null;
                            Session["IsPositiveDeathLast24hours"] = null;
                            Session["IsPositiveDischargesLast24hours"] = null;
                            Session["IsPostiveNotNotifiedLast24hours"] = null;
                            Session["IsInPatientNotNotifiedLast24hours"] = null;
                            Session["IsNegativeNotNotifiedLast24hours"] = null;
                        }
                    }
                    coviddetails = dBEngine.GetCOVIDDetails(DateTime.ParseExact(Session["COVIDStartDate"].ToString(), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), DateTime.ParseExact(Session["COVIDEndDate"].ToString(), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), Session["TestNotification"].ToString(), Session["TestTestStatus"].ToString(),
                            Session["TestTestResults"].ToString(), Session["TestTestOrderLocation"].ToString(), Session["TestLastLocation"].ToString(), Session["TestAdmissionStatus"].ToString(), Convert.ToInt32(Session["TestPageNo"]), Convert.ToInt32(Session["TestPageSize"]), "", Convert.ToBoolean(Session["IsTotalTestOrders"]), Convert.ToBoolean(Session["IsTotalTestOrdersLast24hours"]), Convert.ToBoolean(Session["IsPostive"]), Convert.ToBoolean(Session["IsNegative"]), Convert.ToBoolean(Session["IsPending"]), Convert.ToBoolean(Session["IsPostiveNotNotified"]), Convert.ToBoolean(Session["IsNegativeNotNotified"]), Convert.ToBoolean(Session["IsPendingGreaterthan2days"]), Convert.ToBoolean(Session["IsPositiveLast24hours"]), Convert.ToBoolean(Session["IsPositiveInPatientLast24hours"]), Convert.ToBoolean(Session["IsNegativeLast24hours"]), Convert.ToBoolean(Session["IsPostiveNotNotifiedLast24hours"]), Convert.ToBoolean(Session["IsInPatientNotNotifiedLast24hours"]), Convert.ToBoolean(Session["IsNegativeNotNotifiedLast24hours"]), Convert.ToBoolean(Session["IsPositiveAdmissions"]), Convert.ToBoolean(Session["IsPositiveDeaths"]), Convert.ToBoolean(Session["IsPositiveDischargesLast24hours"]), Convert.ToBoolean(Session["IsPositiveDeathLast24hours"]), Session["TestColumn"].ToString(), Session["TestSortType"].ToString(), Convert.ToInt32(Session["LoginUserID"]));
                    Session["IsPositiveLast24hours"] = null;
                    Session["IsPositiveInPatientLast24hours"] = null;
                    Session["IsNegativeLast24hours"] = null;
                    Session["IsPostiveNotNotifiedLast24hours"] = null;
                    Session["IsInPatientNotNotifiedLast24hours"] = null;
                    Session["IsNegativeNotNotifiedLast24hours"] = null;
                    Session["IsPositiveAdmissions"] = null;
                    Session["IsPositiveDeaths"] = null;
                    Session["IsPositiveDischargesLast24hours"] = null;
                    Session["IsPositiveDeathLast24hours"] = null;
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.TotalTestsOrdered = coviddetails[0].TotalTestsOrdered.ToString();
                        Session["TotalTestsOrdered"] = ViewBag.TotalTestsOrdered;
                    }
                    else
                        ViewBag.TotalTestsOrdered = "0";
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.PositiveTestCases = coviddetails[0].PositiveTestCases.ToString();
                        Session["PositiveTestCases"] = ViewBag.PositiveTestCases;
                    }
                    else
                        ViewBag.PositiveTestCases = "0";

                    if (coviddetails.Count > 0)
                    {
                        ViewBag.NegativeTestCases = coviddetails[0].NegativeTestCases.ToString();
                        Session["NegativeTestCases"] = ViewBag.NegativeTestCases;
                    }
                    else
                        ViewBag.NegativeTestCases = "0";
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.PendingTestCases = coviddetails[0].PendingTestCases.ToString();
                        Session["PendingTestCases"] = ViewBag.PendingTestCases;
                    }
                    else
                        ViewBag.PendingTestCases = "0";
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.AdmissionCount = coviddetails[0].AdmissionCount.ToString();
                        Session["AdmissionCount"] = ViewBag.AdmissionCount;
                    }
                    else
                        ViewBag.AdmissionCount = "0";
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.DeathCount = coviddetails[0].DeathCount.ToString();
                        Session["DeathCount"] = ViewBag.DeathCount;
                    }
                    else
                        ViewBag.DeathCount = "0";

                    if (coviddetails.Count > 0)
                    {
                        ViewBag.NotNotifiedPositiveCount = coviddetails[0].NotNotifiedPositiveCount.ToString();
                        Session["NotNotifiedPositiveCount"] = ViewBag.NotNotifiedPositiveCount;
                    }
                    else
                        ViewBag.NotNotifiedPositiveCount = "0";
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.NotNotifiedNegativeCount = coviddetails[0].NotNotifiedNegativeCount.ToString();
                        Session["NotNotifiedNegativeCount"] = ViewBag.NotNotifiedNegativeCount;
                    }
                    else
                        ViewBag.NotNotifiedNegativeCount = "0";
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.PendingTestsOver2days = coviddetails[0].PendingTestsOver2days.ToString();
                        Session["PendingTestsOver2days"] = ViewBag.PendingTestsOver2days;
                    }
                    else
                        ViewBag.PendingTestsOver2days = "0";
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.NotNotifiedAdmissionCount = coviddetails[0].NotNotifiedAdmissionCount.ToString();
                        Session["NotNotifiedAdmissionCount"] = ViewBag.NotNotifiedAdmissionCount;
                    }
                    else
                        ViewBag.NotNotifiedAdmissionCount = "0";

                    if (coviddetails.Count > 0)
                    {
                        ViewBag.NotNotifiedDeathCount = coviddetails[0].NotNotifiedDeathCount.ToString();
                        Session["NotNotifiedDeathCount"] = ViewBag.NotNotifiedDeathCount;
                    }
                    else
                        ViewBag.NotNotifiedDeathCount = "0";
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.TotalTestsOrderedLast24hrs = coviddetails[0].TotalTestsOrderedLast24hrs.ToString();
                        Session["TotalTestsOrderedLast24hrs"] = ViewBag.TotalTestsOrderedLast24hrs;
                    }
                    else
                        ViewBag.TotalTestsOrderedLast24hrs = "0";

                    if (coviddetails.Count > 0)
                    {
                        ViewBag.PostivePatientDiagnosisLast24hrs = coviddetails[0].PostivePatientDiagnosisLast24hrs.ToString();
                        Session["PostivePatientDiagnosisLast24hrs"] = ViewBag.PostivePatientDiagnosisLast24hrs;
                    }
                    else
                        ViewBag.PostivePatientDiagnosisLast24hrs = "0";

                    if (coviddetails.Count > 0)
                    {
                        ViewBag.PositiveInpatientDiagnosisLast24hrs = coviddetails[0].PositiveInpatientDiagnosisLast24hrs.ToString();
                        Session["PositiveInpatientDiagnosisLast24hrs"] = ViewBag.PositiveInpatientDiagnosisLast24hrs;
                    }
                    else
                        ViewBag.PositiveInpatientDiagnosisLast24hrs = "0";
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.NegativeLast24hours = coviddetails[0].NegativeLast24hours.ToString();
                        Session["NegativeLast24hours"] = ViewBag.NegativeLast24hours;
                    }
                    else
                        ViewBag.NegativeLast24hours = "0";
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.DeathsLast24hrs = coviddetails[0].DeathsLast24hrs.ToString();
                        Session["DeathsLast24hrs"] = ViewBag.DeathsLast24hrs;
                    }
                    else
                        ViewBag.DeathsLast24hrs = "0";
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.DischargesLast24hrs = coviddetails[0].DischargesLast24hrs.ToString();
                        Session["DischargesLast24hrs"] = ViewBag.DischargesLast24hrs;
                    }
                    else
                        ViewBag.DischargesLast24hrs = "0";
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.NotNotifiedPostivePatientDiagnosisLast24hrs = coviddetails[0].NotNotifiedPostivePatientDiagnosisLast24hrs.ToString();
                        Session["NotNotifiedPostivePatientDiagnosisLast24hrs"] = ViewBag.NotNotifiedPostivePatientDiagnosisLast24hrs;
                    }
                    else
                        ViewBag.NotNotifiedPostivePatientDiagnosisLast24hrs = "0";
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.NotNotifiedInPatientDiagnosisLast24hrs = coviddetails[0].NotNotifiedInPatientDiagnosisLast24hrs.ToString();
                        Session["NotNotifiedInPatientDiagnosisLast24hrs"] = ViewBag.NotNotifiedInPatientDiagnosisLast24hrs;
                    }
                    else
                        ViewBag.NotNotifiedInPatientDiagnosisLast24hrs = "0";

                    if (coviddetails.Count > 0)
                    {
                        ViewBag.NotNotifiedNegativeLast24hours = coviddetails[0].NotNotifiedNegativeLast24hours.ToString();
                        Session["NotNotifiedNegativeLast24hours"] = ViewBag.NotNotifiedNegativeLast24hours;
                    }
                    else
                        ViewBag.NotNotifiedNegativeLast24hours = "0";

                    int totalRecords = 0;
                    if (coviddetails.Count > 0)
                    {
                        totalRecords = coviddetails[0].TotalRecords;
                    }

                    int totalPagesCount = 0;
                    if (Convert.ToInt32(Session["TestPageSize"]) != -1)
                        totalPagesCount = (int)Math.Ceiling((float)totalRecords / Convert.ToInt32(Session["TestPageSize"]));
                    else
                        totalPagesCount = 1;

                    ViewBag.PageNumber = Convert.ToInt32(Session["TestPageNo"]);
                    ViewBag.SearchText = "";
                    ViewBag.TotalPagesCount = totalPagesCount;
                    ViewBag.TotalRecordCount = totalRecords;
                    ViewBag.ModalRecordCount = coviddetails.Count;
                    ViewBag.PageSize = Convert.ToInt32(Session["TestPageSize"]);
                    permissions = dBEngine.GetRolePermission(0, 0, "", "", "",Convert.ToInt32(Session["LoginUserID"]), "COVID Dashboard", Convert.ToString(Session["Role"]));
                    bool isnoaccess = false;
                    if (permissions.Count > 0)
                    {
                        ViewBag.IsReadOnly = permissions[0].IsReadOnly;
                        isnoaccess = permissions[0].NoAccess;
                    }
                    else
                    {
                        ViewBag.IsReadOnly = true;
                        isnoaccess = true;
                    }
                    if (isnoaccess == true)
                    {
                       return RedirectToAction("NotAuthorizedCOVID", new { patientID = "", isdashboard = true });
                    }
                }
                else
                    return RedirectToAction("Index", "Account");
            }
            catch(Exception ex)
            {
                dBEngine.LogException(ex.Message, "HomeController", "COVIDDashboard", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
            }
            finally
            {
                permissions = null;
                defaultdate = null;
                filterddm = null;
                dBEngine = null;
            }
            return View(coviddetails);
        }

        public ActionResult COVIDPatientDashboard(FormCollection formCollection, bool isReset = false, bool buttonsubmit = false, bool ismenu = false)
        {
            Session["TestID"] = 0;
            Session["COVIDPatientID"] = "";
            Session.Timeout = 30;
            Session["PatientDashboard"] = true;
            Session["TestDashboard"] = false;
            Session["COVIDStartDate"] = null;
            Session["COVIDEndDate"] = null;
            if (Session["COVIDNotification"] == null)
                Session["COVIDNotification"] = "0";
            if (Session["COVIDTestStatus"] == null)
                Session["COVIDTestStatus"] = "0";
            if (Session["COVIDTestResults"] == null)
                Session["COVIDTestResults"] = "0";
            if (Session["COVIDTestOrderLocation"] == null)
                Session["COVIDTestOrderLocation"] = "0";
            if (Session["COVIDLastLocation"] == null)
                Session["COVIDLastLocation"] = "0";
            if (Session["COVIDAdmissionStatus"] == null)
                Session["COVIDAdmissionStatus"] = "0";
            if (Session["COVIDColumn"] == null)
                Session["COVIDColumn"] = "";
            if (Session["COVIDSortType"] == null)
                Session["COVIDSortType"] = "";
            Session["IsTotalTestOrders"] = null;
            Session["IsPostive"] = null;
            Session["IsNegative"] = null;
            Session["IsPending"] = null;
            Session["PendingTestsOver120mins"] = null;
            Session["AdmissionCount"] = null;
            Session["DeathCount"] = null;
            Session["IsPostiveNotNotified"] = null;
            Session["IsNegativeNotNotified"] = null;
            Session["IsPendingGreaterthan2days"] = null;
            Session["NotifiedAdmissionCount"] = null;
            Session["NotifiedDeathCount"] = null;
            Session["PageSize"] = 10;
            
            Session["IsTotalTestOrdersLast24hours"] = null;
            Session["IsPositiveLast24hours"] = null;
            Session["IsPositiveInPatientLast24hours"] = null;
            Session["IsNegativeLast24hours"] = null;
            Session["DeathsLast24hrs"] = null;
            Session["DischargesLast24hrs"] = null;
            Session["IsPostiveNotNotifiedLast24hours"] = null;
            Session["IsInPatientNotNotifiedLast24hours"] = null;
            Session["IsNegativeNotNotifiedLast24hours"] = null;
            Session["IsPositiveICUInPatient"] = null;
            if (Session["COVIDPatientPageNo"] == null)
                Session["COVIDPatientPageNo"] = 1;
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            COVIDDefaultDate defaultdate = new COVIDDefaultDate();
            COVIDPatientFilterDDM filterddm = new COVIDPatientFilterDDM();
            List<clsCOVIDPatientDetails> coviddetails = new List<clsCOVIDPatientDetails>();
            List<RolePermission> permissions = new List<Common.RolePermission>();
            try
            {
                if (Convert.ToInt32(Session["LoginUserID"]) > 0)
                {
                    filterddm = dBEngine.GetCOVIDpatientFilterDDM(Convert.ToInt32(Session["LoginUserID"]));
                    ViewBag.AgeGroup = filterddm.lstAgeGroup;
                    ViewBag.TestResults = filterddm.lstTestResult;
                    ViewBag.BreathingStatus = filterddm.lstBreathingStatuses;
                    ViewBag.LastLocation = filterddm.lstLastLocation;
                    ViewBag.AdmissionStatus = filterddm.lstAdmissionStatus;
                    ViewBag.PatientType = filterddm.lstPatientType;
                    ViewBag.UpdatedSource = filterddm.datamanagement.SourceSystem;
                    ViewBag.UpdatedDate = filterddm.datamanagement.UpdateDate;
                    ViewBag.UpdatedTime = filterddm.datamanagement.UpdateTime;

                    if (Session["COVIDPageSize"] == null)
                        Session["COVIDPageSize"] = 10;
                    if (Request.HttpMethod != "POST")
                    {
                        defaultdate = dBEngine.GetCOVIDPatientDefaultDate(Convert.ToInt32(Session["LoginUserID"]));
                        if (isReset)
                        {
                            if (defaultdate.StartDate == null)
                                Session["COVIDPatientStartDate"] = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
                            else
                                Session["COVIDPatientStartDate"] = defaultdate.StartDate;
                            if (defaultdate.EndDate == null)
                                Session["COVIDPatientEndDate"] = DateTime.Now.ToString("dd/MM/yyyy");
                            else
                                Session["COVIDPatientEndDate"] = defaultdate.EndDate;
                            if(!ismenu)
                                Session["COVIDPatientType"] = "0";
                            else
                                Session["COVIDPatientType"] = "Inpatient";
                            Session["COVIDAgeGroup"] = "0";
                            Session["COVIDTestResults"] = "0";
                            Session["COVIDBreathingStatus"] = "0";
                            Session["COVIDLastLocation"] = "0";
                            Session["COVIDAdmissionStatus"] = "0";
                            Session["TotalTestsOrdered"] = null;
                            Session["TotalTestsOrderedInPatient"] = null;
                            Session["PositiveTestCases"] = null;
                            Session["PositiveTestCasesICU"] = null;
                            Session["NegativeTestCases"] = null;
                            Session["PositiveDischarges"] = null;
                            Session["COVIDPageSize"] = 10;
                            Session["COVIDPatientPageNo"] = 1;

                            Session["PositiveDeaths"] = null;
                            Session["PositiveInPatient"] = null;
                            Session["PendingInPatient"] = null;
                            Session["PositiveInPatientLast24hours"] = null;
                            Session["NegativeInPatientLast24hours"] = null;
                            Session["PositiveICUInPatientLast24hours"] = null;
                            Session["PendingICUInPatient"] = null;
                            Session["PositiveDischargeCountLast24hours"] = null;
                            Session["PositiveDeathCountLast24hours"] = null;
                            Session["COVIDSortType"] = "DESC";
                            Session["COVIDColumn"] = "AdmissionDateTime";
                            Session["IsTotalTestsOrdered"] = null;
                            Session["IsTotalTestsOrderedInPatient"] = null;
                            Session["IsPositiveTestCases"] = null;
                            Session["IsPositiveTestCasesICU"] = null;
                            Session["IsNegativeTestCases"] = null;
                            Session["IsPositiveDischarges"] = null;
                            Session["IsPositiveDeaths"] = null;
                            Session["IsPositiveInPatient"] = null;
                            Session["IsPendingInPatient"] = null;
                            Session["IsPositiveICUInPatient"] = null;
                            Session["IsPendingICUInPatient"] = null;
                            Session["IsPositiveLast24hours"] = null;
                            Session["IsNegativeLast24hours"] = null;
                            Session["IsPositiveDischargeCountLast24hours"] = null;
                            Session["IsPositiveDeathCountLast24hours"] = null;
                            Session["IsPositiveICUInPatient"] = null;
                        }
                        else
                        {
                            if (defaultdate.StartDate == null)
                                Session["COVIDPatientStartDate"] = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
                            else
                                Session["COVIDPatientStartDate"] = defaultdate.StartDate;
                            if (defaultdate.EndDate == null)
                                Session["COVIDPatientEndDate"] = DateTime.Now.ToString("dd/MM/yyyy");
                            else
                                Session["COVIDPatientEndDate"] = defaultdate.EndDate;
                            if (string.IsNullOrEmpty(Convert.ToString(Session["COVIDPatientType"])) || Convert.ToString(Session["COVIDPatientType"]) == "0")
                                Session["COVIDPatientType"] = "Inpatient";
                            if (string.IsNullOrEmpty(Convert.ToString(Session["COVIDAgeGroup"])))
                                Session["COVIDAgeGroup"] = "0";
                            if (string.IsNullOrEmpty(Convert.ToString(Session["COVIDBreathingStatus"])))
                                Session["COVIDBreathingStatus"] = "0";
                            if (string.IsNullOrEmpty(Convert.ToString(Session["COVIDTestResults"])))
                                Session["COVIDTestResults"] = "0";
                            if (string.IsNullOrEmpty(Convert.ToString(Session["COVIDAdmissionStatus"])))
                                Session["COVIDAdmissionStatus"] = "0";
                            if (string.IsNullOrEmpty(Convert.ToString(Session["COVIDLastLocation"])))
                                Session["COVIDLastLocation"] = "0";
                            if (Session["IsTotalTestsOrdered"] == null)
                                Session["IsTotalTestsOrdered"] = false;
                            if (Session["IsTotalTestsOrderedInPatient"] == null)
                                Session["IsTotalTestsOrderedInPatient"] = false;
                            if (Session["IsPositiveTestCases"] == null)
                                Session["IsPositiveTestCases"] = false;
                            if (Session["IsPositiveTestCasesICU"] == null)
                                Session["IsPositiveTestCasesICU"] = false;
                            if (Session["IsNegativeTestCases"] == null)
                                Session["IsNegativeTestCases"] = false;
                            if (Session["IsPositiveDischarges"] == null)
                                Session["IsPositiveDischarges"] = false;
                            if (Session["COVIDPageSize"] == null)
                                Session["COVIDPageSize"] = 10;
                            if (Session["COVIDPatientPageNo"] == null)
                                Session["COVIDPatientPageNo"] = 1;
                            if (Session["COVIDColumn"] == null)
                                Session["COVIDColumn"] = "";
                            if (Session["COVIDSortType"] == null)
                                Session["COVIDSortType"] = "";
                            if (Session["IsPositiveDeaths"] == null)
                                Session["IsPositiveDeaths"] = false;
                            if (Session["IsPositiveInPatient"] == null)
                                Session["IsPositiveInPatient"] = false;
                            if (Session["IsPendingInPatient"] == null)
                                Session["IsPendingInPatient"] = false;
                            if (Session["IsPositiveInPatientLast24hours"] == null)
                                Session["IsPositiveInPatientLast24hours"] = false;
                            if (Session["IsNegativeInPatientLast24hours"] == null)
                                Session["IsNegativeInPatientLast24hours"] = false;
                            if (Session["IsPositiveICUInPatientLast24hours"] == null)
                                Session["IsPositiveICUInPatientLast24hours"] = false;
                            if (Session["IsPendingICUInPatient"] == null)
                                Session["IsPendingICUInPatient"] = false;
                            if (Session["IsPositiveDischargeCountLast24hours"] == null)
                                Session["IsPositiveDischargeCountLast24hours"] = false;
                            if (Session["IsPositiveDeathCountLast24hours"] == null)
                                Session["IsPositiveDeathCountLast24hours"] = false;
                        }
                    }
                    else
                    {
                        if (isReset == false && buttonsubmit)
                        {
                            Session["COVIDPatientStartDate"] = formCollection["txtStartDate"].Replace("Admission Start Date ", "");
                            Session["COVIDPatientEndDate"] = formCollection["txtEndDate"].Replace("Admission End Date ", "");
                            Session["COVIDPatientType"] = formCollection["ddlPatientType"];
                            Session["COVIDAgeGroup"] = formCollection["ddlAgeGroup"];
                            Session["COVIDTestResults"] = formCollection["ddlTestResults"];
                            Session["COVIDBreathingStatus"] = formCollection["ddlBreathingStatus"];
                            Session["COVIDLastLocation"] = formCollection["ddlLastLocation"];
                            Session["COVIDAdmissionStatus"] = formCollection["ddlAdmissionStatus"];

                            Session["TotalTestsOrdered"] = null;
                            Session["TotalTestsOrderedInPatient"] = null;
                            Session["PositiveTestCases"] = null;
                            Session["PositiveTestCasesICU"] = null;
                            Session["NegativeTestCases"] = null;
                            Session["PositiveDischarges"] = null;
                            Session["COVIDPageSize"] = 10;

                            Session["PositiveDeaths"] = null;
                            Session["PositiveInPatient"] = null;
                            Session["PendingInPatient"] = null;
                            Session["PositiveInPatientLast24hours"] = null;
                            Session["NegativeInPatientLast24hours"] = null;
                            Session["PositiveICUInPatientLast24hours"] = null;
                            Session["PendingICUInPatient"] = null;
                            Session["PositiveDischargeCountLast24hours"] = null;
                            Session["PositiveDeathCountLast24hours"] = null;
                        }
                    }
                    coviddetails = dBEngine.GetCOVIDPatientDetails(DateTime.ParseExact(Session["COVIDPatientStartDate"].ToString(), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), DateTime.ParseExact(Session["COVIDPatientEndDate"].ToString(), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), Session["COVIDPatientType"].ToString(), Session["COVIDAgeGroup"].ToString(), Session["COVIDTestResults"].ToString(), Session["COVIDBreathingStatus"].ToString(), Session["COVIDLastLocation"].ToString(), Session["COVIDAdmissionStatus"].ToString(), Convert.ToInt32(Session["COVIDPatientPageNo"]), Convert.ToInt32(Session["COVIDPageSize"]), "", Convert.ToBoolean(Session["IsTotalTestsOrdered"]), Convert.ToBoolean(Session["IsTotalTestsOrderedInPatient"]), Convert.ToBoolean(Session["IsPositiveTestCases"]), Convert.ToBoolean(Session["IsPositiveTestCasesICU"]), Convert.ToBoolean(Session["IsNegativeTestCases"]), Convert.ToBoolean(Session["IsPositiveDischarges"]), Convert.ToBoolean(Session["IsPositiveDeaths"]), Convert.ToBoolean(Session["IsPositiveInPatient"]), Convert.ToBoolean(Session["IsPendingInPatient"]), Convert.ToBoolean(Session["IsPositiveICUInPatient"]), Convert.ToBoolean(Session["IsPendingICUInPatient"]), Convert.ToBoolean(Session["IsPositiveInPatientLast24hours"]), Convert.ToBoolean(Session["IsNegativeInPatientLast24hours"]), Convert.ToBoolean(Session["IsPositiveDischargeCountLast24hours"]), Convert.ToBoolean(Session["IsPositiveDeathCountLast24hours"]), Convert.ToString(Session["COVIDColumn"]), Convert.ToString(Session["COVIDSortType"]), Convert.ToInt32(Session["LoginUserID"]));

                    if (coviddetails.Count > 0)
                    {
                        ViewBag.TotalTestsOrdered = coviddetails[0].TotalTestsOrdered.ToString();
                        if(buttonsubmit == true || isReset == true)
                            Session["TotalTestsOrdered"] = ViewBag.TotalTestsOrdered;
                    }
                    else
                        ViewBag.TotalTestsOrdered = "0";
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.TotalTestsOrderedInPatient = coviddetails[0].TotalTestsOrderedInPatient.ToString();
                        if (buttonsubmit == true || isReset == true)
                            Session["TotalTestsOrderedInPatient"] = ViewBag.TotalTestsOrderedInPatient;
                    }
                    else
                        ViewBag.TotalTestsOrderedInPatient = "0";
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.PositiveTestCases = coviddetails[0].PositiveTestCases.ToString();
                        if (buttonsubmit == true || isReset == true)
                            Session["PositiveTestCases"] = ViewBag.PositiveTestCases;
                    }
                    else
                        ViewBag.PositiveTestCases = "0";
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.PositiveRatePercentage = coviddetails[0].PositiveRatePercentage.ToString();
                        if (buttonsubmit == true || isReset == true)
                            Session["PositiveRatePercentage"] = ViewBag.PositiveRatePercentage;
                    }
                    else
                        ViewBag.PositiveRatePercentage = "0";
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.PositiveTestCasesICU = coviddetails[0].PositiveTestCasesICU.ToString();
                        if (buttonsubmit == true || isReset == true)
                            Session["PositiveTestCasesICU"] = ViewBag.PositiveTestCasesICU;
                    }
                    else
                        ViewBag.PositiveTestCasesICU = "0";
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.PositiveICURatePercentage = coviddetails[0].PositiveICURatePercentage.ToString();
                        if (buttonsubmit == true || isReset == true)
                            Session["PositiveICURatePercentage"] = ViewBag.PositiveICURatePercentage;
                    }
                    else
                        ViewBag.PositiveICURatePercentage = "0";
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.NegativeTestCases = coviddetails[0].NegativeTestCases.ToString();
                        if (buttonsubmit == true || isReset == true)
                            Session["NegativeTestCases"] = ViewBag.NegativeTestCases;
                    }
                    else
                        ViewBag.NegativeTestCases = "0";
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.NegativeRatePercentage = coviddetails[0].NegativeRatePercentage.ToString();
                        if (buttonsubmit == true || isReset == true)
                            Session["NegativeRatePercentage"] = ViewBag.NegativeRatePercentage;
                    }
                    else
                        ViewBag.NegativeRatePercentage = "0";
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.PositiveDischarges = coviddetails[0].PositiveDischarges.ToString();
                        if (buttonsubmit == true || isReset == true)
                            Session["PositiveDischarges"] = ViewBag.PositiveDischarges;
                    }
                    else
                        ViewBag.PositiveDischarges = "0";

                    if (coviddetails.Count > 0)
                    {
                        ViewBag.PositiveDeaths = coviddetails[0].PositiveDeaths.ToString();
                        if (buttonsubmit == true || isReset == true)
                            Session["PositiveDeaths"] = ViewBag.PositiveDeaths;
                    }
                    else
                        ViewBag.PositiveDeaths = "0";
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.PositiveInPatient = coviddetails[0].PositiveInPatient.ToString();
                        if (buttonsubmit == true || isReset == true)
                            Session["PositiveInPatient"] = ViewBag.PositiveInPatient;
                    }
                    else
                        ViewBag.PositiveInPatient = "0";
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.PendingInPatient = coviddetails[0].PendingInPatient.ToString();
                        if (buttonsubmit == true || isReset == true)
                            Session["PendingInPatient"] = ViewBag.PendingInPatient;
                    }
                    else
                        ViewBag.PendingInPatient = "0";
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.PositiveInPatientICU = coviddetails[0].PositiveInPatientICU.ToString();
                        if (buttonsubmit == true || isReset == true)
                            Session["PositiveInPatientICU"] = ViewBag.PositiveInPatientICU;
                    }
                    else
                        ViewBag.PositiveInPatientICU = "0";

                    if (coviddetails.Count > 0)
                    {
                        ViewBag.PendingICUInPatient = coviddetails[0].PendingICUInPatient.ToString();
                        if (buttonsubmit == true || isReset == true)
                            Session["PendingICUInPatient"] = ViewBag.PendingICUInPatient;
                    }
                    else
                        ViewBag.PendingICUInPatient = "0";
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.PositiveInPatientLast24hours = coviddetails[0].PositiveInPatientLast24hours.ToString();
                        if (buttonsubmit == true || isReset == true)
                            Session["PositiveInPatientLast24hours"] = ViewBag.PositiveInPatientLast24hours;
                    }
                    else
                        ViewBag.PositiveInPatientLast24hours = "0";

                    if (coviddetails.Count > 0)
                    {
                        ViewBag.NegativeInPatientLast24hours = coviddetails[0].NegativeInPatientLast24hours.ToString();
                        if (buttonsubmit == true || isReset == true)
                            Session["NegativeInPatientLast24hours"] = ViewBag.PostivePatientDiagnosisLast24hrs;
                    }
                    else
                        ViewBag.NegativeInPatientLast24hours = "0";

                    if (coviddetails.Count > 0)
                    {
                        ViewBag.PositiveDischargeCountLast24hours = coviddetails[0].PositiveDischargeCountLast24hours.ToString();
                        if (buttonsubmit == true || isReset == true)
                            Session["PositiveDischargeCountLast24hours"] = ViewBag.PositiveDischargeCountLast24hours;
                    }
                    else
                        ViewBag.PositiveDischargeCountLast24hours = "0";
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.PositiveDeathCountLast24hours = coviddetails[0].PositiveDeathCountLast24hours.ToString();
                        if (buttonsubmit == true || isReset == true)
                            Session["PositiveDeathCountLast24hours"] = ViewBag.PositiveDeathCountLast24hours;
                    }
                    else
                        ViewBag.PositiveDeathCountLast24hours = "0";

                    int totalRecords = 0;
                    if (coviddetails.Count > 0)
                    {
                        totalRecords = coviddetails[0].TotalRecords;
                    }

                    int totalPagesCount = 0;
                    if (Convert.ToInt32(Session["COVIDPageSize"].ToString()) != -1)
                    {
                        totalPagesCount = (int)Math.Ceiling((float)totalRecords / Convert.ToInt32(Session["COVIDPageSize"].ToString()));
                        ViewBag.PageSize = (int)Math.Ceiling((float)totalRecords / Convert.ToInt32(Session["COVIDPageSize"].ToString()));
                    }
                    else
                    {
                        ViewBag.PageSize = 1;
                    }

                    ViewBag.PageNumber = Convert.ToInt32(Session["COVIDPatientPageNo"]);
                    ViewBag.SearchText = "";
                    ViewBag.TotalPagesCount = totalPagesCount;
                    ViewBag.TotalRecordCount = totalRecords;
                    ViewBag.ModalRecordCount = coviddetails.Count;
                    permissions = dBEngine.GetRolePermission(0, 0, "", "", "", Convert.ToInt32(Session["LoginUserID"]), "COVID Patient Dashboard", Convert.ToString(Session["Role"]));
                    bool isnoaccess = false;
                    if (permissions.Count > 0)
                    {
                        ViewBag.IsReadOnly = permissions[0].IsReadOnly;
                        isnoaccess = permissions[0].NoAccess;
                    }
                    else
                    {
                        ViewBag.IsReadOnly = true;
                        isnoaccess = true;
                    }
                    if (isnoaccess == true)
                    {
                        return RedirectToAction("NotAuthorizedCOVID", new { id = 0, isdashboard = true });
                    }
                }
                else
                    return RedirectToAction("Index", "Account");
            }
            catch(Exception ex)
            {
                dBEngine.LogException(ex.Message, "HomeController", "COVIDPatientDashboard", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
            }
            finally
            {
                permissions = null;
                defaultdate = null;
                filterddm = null;
                dBEngine = null;
            }
            return View(coviddetails);
        }

        [HttpPost]
        public ActionResult MoreCOVIDPatientDetails(int pageNumber = 1, int pageSize = 10, string searchfield = "", string SortColumn = "", bool frompager = false, bool IsTotalTestsOrdered = false, bool IsTotalTestsOrderedInPatient = false, bool IsPositiveTestCases = false, bool IsPositiveTestCasesICU = false, bool IsNegativeTestCases = false, bool IsPositiveDischarges = false, bool IsPositiveDeaths = false, bool IsPositiveInPatient = false, bool IsPendingInPatient = false, bool IsPositiveLast24hours = false, bool IsNegativeLast24hours = false, bool IsPositiveICUInPatient = false,  bool IsPendingICUInPatient = false, bool IsPositiveDischargeCountLast24hours = false, bool IsPositiveDeathCountLast24hours = false, bool fromfilter = false)
        {
            if (SortColumn != "")
            {
                if (!frompager)
                {
                    if (Session["COVIDColumn"] == null || Convert.ToString(Session["COVIDColumn"]) != SortColumn)
                    {
                        Session["COVIDColumn"] = SortColumn;
                        Session["COVIDSortType"] = null;
                    }
                    if (Session["COVIDSortType"] == null)
                        Session["COVIDSortType"] = "DESC";
                    else if (Session["COVIDSortType"] != null && Convert.ToString(Session["COVIDColumn"]) == SortColumn && Convert.ToString(Session["COVIDSortType"]) == "DESC")
                        Session["COVIDSortType"] = "ASC";
                    else if (Session["COVIDSortType"] != null && Convert.ToString(Session["COVIDColumn"]) == SortColumn && Convert.ToString(Session["COVIDSortType"]) == "ASC")
                        Session["COVIDSortType"] = "DESC";
                }
                else
                {
                    if (Session["COVIDColumn"] == null || Convert.ToString(Session["COVIDColumn"]) != SortColumn)
                    {
                        Session["COVIDColumn"] = SortColumn;
                        Session["COVIDSortType"] = null;
                    }                    
                    if (Session["COVIDSortType"] == null)
                        Session["COVIDSortType"] = "DESC";
                }
            }
            else
            {
                Session["COVIDColumn"] = "AdmissionDateTime";
                Session["COVIDSortType"] = "DESC";
            }
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            List<clsCOVIDPatientDetails> coviddetails = new List<clsCOVIDPatientDetails>();

            try
            {
                if (Session["IsTotalTestsOrdered"] == null || fromfilter == true)
                    Session["IsTotalTestsOrdered"] = IsTotalTestsOrdered;
                else
                    IsTotalTestsOrdered = Convert.ToBoolean(Session["IsTotalTestsOrdered"]);

                if (Session["IsTotalTestsOrderedInPatient"] == null || fromfilter == true)
                    Session["IsTotalTestsOrderedInPatient"] = IsTotalTestsOrderedInPatient;
                else
                    IsTotalTestsOrderedInPatient = Convert.ToBoolean(Session["IsTotalTestsOrderedInPatient"]);
                if (Session["IsPositiveTestCases"] == null || fromfilter == true)
                    Session["IsPositiveTestCases"] = IsPositiveTestCases;
                else
                    IsPositiveTestCases = Convert.ToBoolean(Session["IsPositiveTestCases"]);
                if (Session["IsPositiveTestCasesICU"] == null || fromfilter == true)
                    Session["IsPositiveTestCasesICU"] = IsPositiveTestCasesICU;
                else
                    IsPositiveTestCasesICU = Convert.ToBoolean(Session["IsPositiveTestCasesICU"]);
                if (Session["IsNegativeTestCases"] == null || fromfilter == true)
                    Session["IsNegativeTestCases"] = IsNegativeTestCases;
                else
                    IsNegativeTestCases = Convert.ToBoolean(Session["IsNegativeTestCases"]);
                if (Session["IsPositiveDischarges"] == null || fromfilter == true)
                    Session["IsPositiveDischarges"] = IsPositiveDischarges;
                else
                    IsPositiveDischarges = Convert.ToBoolean(Session["IsPositiveDischarges"]);
                if (Session["IsPositiveDeaths"] == null || fromfilter == true)
                    Session["IsPositiveDeaths"] = IsPositiveDeaths;
                else
                    IsPositiveDeaths = Convert.ToBoolean(Session["IsPositiveDeaths"]);
                if (Session["IsPositiveInPatient"] == null || fromfilter == true)
                    Session["IsPositiveInPatient"] = IsPositiveInPatient;
                else
                    IsPositiveInPatient = Convert.ToBoolean(Session["IsPositiveInPatient"]);
                if (Session["IsPendingInPatient"] == null || fromfilter == true)
                    Session["IsPendingInPatient"] = IsPendingInPatient;
                else
                    IsPendingInPatient = Convert.ToBoolean(Session["IsPendingInPatient"]);
                if (Session["IsPositiveICUInPatient"] == null || fromfilter == true)
                    Session["IsPositiveICUInPatient"] = IsPositiveICUInPatient;
                else
                    IsPositiveICUInPatient = Convert.ToBoolean(Session["IsPositiveICUInPatient"]);
                if (Session["IsPendingICUInPatient"] == null || fromfilter == true)
                    Session["IsPendingICUInPatient"] = IsPendingICUInPatient;
                else
                    IsPendingICUInPatient = Convert.ToBoolean(Session["IsPendingICUInPatient"]);
                if (Session["IsPositiveLast24hours"] == null || fromfilter == true)
                    Session["IsPositiveLast24hours"] = IsPositiveLast24hours;
                else
                    IsPositiveLast24hours = Convert.ToBoolean(Session["IsPositiveLast24hours"]);
                if (Session["IsNegativeLast24hours"] == null || fromfilter == true)
                    Session["IsNegativeLast24hours"] = IsNegativeLast24hours;
                else
                    IsNegativeLast24hours = Convert.ToBoolean(Session["IsNegativeLast24hours"]);
                if (Session["IsPositiveDischargeCountLast24hours"] == null || fromfilter == true)
                    Session["IsPositiveDischargeCountLast24hours"] = IsPositiveDischargeCountLast24hours;
                else
                    IsPositiveDischargeCountLast24hours = Convert.ToBoolean(Session["IsPositiveDischargeCountLast24hours"]);
                if (Session["IsPositiveDeathCountLast24hours"] == null || fromfilter == true)
                    Session["IsPositiveDeathCountLast24hours"] = IsPositiveDeathCountLast24hours;
                else
                    IsPositiveDeathCountLast24hours = Convert.ToBoolean(Session["IsPositiveDeathCountLast24hours"]);
                if (pageSize != Convert.ToInt32(Session["COVIDPageSize"]))
                    Session["COVIDPageSize"] = pageSize;
                if (Session["COVIDSortType"] == null)
                    Session["COVIDSortType"] = "DESC";
                if (Session["COVIDPatientPageNo"] == null || Convert.ToInt32(Session["COVIDPatientPageNo"]) != pageNumber)
                    Session["COVIDPatientPageNo"] = pageNumber;

                coviddetails = dBEngine.GetCOVIDPatientDetails(DateTime.ParseExact(Session["COVIDPatientStartDate"].ToString(), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), DateTime.ParseExact(Session["COVIDPatientEndDate"].ToString(), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), Session["COVIDPatientType"].ToString(), Session["COVIDAgeGroup"].ToString(), Session["COVIDTestResults"].ToString(), Session["COVIDBreathingStatus"].ToString(), Session["COVIDLastLocation"].ToString(), Session["COVIDAdmissionStatus"].ToString(), Convert.ToInt32(Session["COVIDPatientPageNo"]), Convert.ToInt32(Session["COVIDPageSize"]), searchfield, IsTotalTestsOrdered, IsTotalTestsOrderedInPatient, IsPositiveTestCases, IsPositiveTestCasesICU, IsNegativeTestCases, IsPositiveDischarges, IsPositiveDeaths, IsPositiveInPatient, IsPendingInPatient, IsPositiveICUInPatient, IsPendingICUInPatient, IsPositiveLast24hours, IsNegativeLast24hours, IsPositiveDischargeCountLast24hours, IsPositiveDeathCountLast24hours, Session["COVIDColumn"].ToString(), Session["COVIDSortType"].ToString(), Convert.ToInt32(Session["LoginUserID"]));
                if (Session["TotalTestsOrdered"] != null)
                {
                    ViewBag.TotalTestsOrdered = Session["TotalTestsOrdered"];
                }
                else
                {
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.TotalTestsOrdered = coviddetails[0].TotalTestsOrdered.ToString();
                        Session["TotalTestsOrdered"] = ViewBag.TotalTestsOrdered;
                    }
                    else
                        ViewBag.TotalTestsOrdered = "0";
                }
                if (Session["TotalTestsOrderedInPatient"] != null)
                {
                    ViewBag.TotalTestsOrderedInPatient = Session["TotalTestsOrderedInPatient"];
                }
                else
                {
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.TotalTestsOrderedInPatient = coviddetails[0].TotalTestsOrderedInPatient.ToString();
                        Session["TotalTestsOrderedInPatient"] = ViewBag.TotalTestsOrderedInPatient;
                    }
                    else
                        ViewBag.TotalTestsOrderedInPatient = "0";
                }
                if (Session["PositiveTestCases"] != null)
                {
                    ViewBag.PositiveTestCases = Session["PositiveTestCases"];
                }
                else
                {
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.PositiveTestCases = coviddetails[0].PositiveTestCases.ToString();
                        Session["PositiveTestCases"] = ViewBag.PositiveTestCases;
                    }
                    else
                        ViewBag.PositiveTestCases = "0";
                }
                if (Session["PositiveRatePercentage"] != null)
                {
                    ViewBag.PositiveRatePercentage = Session["PositiveRatePercentage"];
                }
                else
                {
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.PositiveRatePercentage = coviddetails[0].PositiveRatePercentage.ToString();
                        Session["PositiveRatePercentage"] = ViewBag.PositiveRatePercentage;
                    }
                    else
                        ViewBag.PositiveRatePercentage = "0";
                }
                if (Session["PositiveTestCasesICU"] != null)
                {
                    ViewBag.PositiveTestCasesICU = Session["PositiveTestCasesICU"];
                }
                else
                {
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.PositiveTestCasesICU = coviddetails[0].PositiveTestCasesICU.ToString();
                        Session["PositiveTestCasesICU"] = ViewBag.PositiveTestCasesICU;
                    }
                    else
                        ViewBag.PositiveTestCasesICU = "0";
                }
                if (Session["PositiveICURatePercentage"] != null)
                {
                    ViewBag.PositiveICURatePercentage = Session["PositiveICURatePercentage"];
                }
                else
                {
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.PositiveICURatePercentage = coviddetails[0].PositiveICURatePercentage.ToString();
                        Session["PositiveICURatePercentage"] = ViewBag.PositiveICURatePercentage;
                    }
                    else
                        ViewBag.PositiveICURatePercentage = "0";
                }
                if (Session["NegativeRatePercentage"] != null)
                {
                    ViewBag.NegativeRatePercentage = Session["NegativeRatePercentage"];
                }
                else
                {
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.NegativeRatePercentage = coviddetails[0].NegativeRatePercentage.ToString();
                        Session["NegativeRatePercentage"] = ViewBag.NegativeRatePercentage;
                    }
                    else
                        ViewBag.NegativeRatePercentage = "0";
                }
                if (Session["PositiveDischarges"] != null)
                {
                    ViewBag.PositiveDischarges = Session["PositiveDischarges"];
                }
                else
                {
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.PositiveDischarges = coviddetails[0].PositiveDischarges.ToString();
                        Session["PositiveDischarges"] = ViewBag.PositiveDischarges;
                    }
                    else
                        ViewBag.PositiveDischarges = "0";
                }
                if (Session["PositiveDeaths"] != null)
                {
                    ViewBag.PositiveDeaths = Session["PositiveDeaths"];
                }
                else
                {
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.PositiveDeaths = coviddetails[0].PositiveDeaths.ToString();
                        Session["PositiveDeaths"] = ViewBag.PositiveDeaths;
                    }
                    else
                        ViewBag.PositiveDeaths = "0";
                }
                if (Session["PositiveInPatient"] != null)
                {
                    ViewBag.PositiveInPatient = Session["PositiveInPatient"];
                }
                else
                {
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.PositiveInPatient = coviddetails[0].PositiveInPatient.ToString();
                        Session["PositiveInPatient"] = ViewBag.PositiveInPatient;
                    }
                    else
                        ViewBag.PositiveInPatient = "0";
                }
                if (Session["PendingInPatient"] != null)
                {
                    ViewBag.PendingInPatient = Session["PendingInPatient"];
                }
                else
                {
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.PendingInPatient = coviddetails[0].PendingInPatient.ToString();
                        Session["PendingInPatient"] = ViewBag.PendingInPatient;
                    }
                    else
                        ViewBag.PendingInPatient = "0";
                }
                if (Session["PositiveInPatientICU"] != null)
                {
                    ViewBag.PositiveInPatientICU = Session["PositiveInPatientICU"];
                }
                else
                {
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.PositiveInPatientICU = coviddetails[0].PositiveInPatientICU.ToString();
                        Session["PositiveInPatientICU"] = ViewBag.PositiveInPatientICU;
                    }
                    else
                        ViewBag.PositiveInPatientICU = "0";
                }
                if (Session["PendingICUInPatient"] != null)
                {
                    ViewBag.PendingICUInPatient = Session["PendingICUInPatient"];
                }
                else
                {
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.PendingICUInPatient = coviddetails[0].PendingICUInPatient.ToString();
                        Session["PendingICUInPatient"] = ViewBag.PendingICUInPatient;
                    }
                    else
                        ViewBag.PendingICUInPatient = "0";
                }
                if (Session["PositiveInPatientLast24hours"] != null)
                {
                    ViewBag.PositiveInPatientLast24hours = Session["PositiveInPatientLast24hours"];
                }
                else
                {
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.PositiveInPatientLast24hours = coviddetails[0].PositiveInPatientLast24hours.ToString();
                        Session["PositiveInPatientLast24hours"] = ViewBag.PositiveInPatientLast24hours;
                    }
                    else
                        ViewBag.PositiveInPatientLast24hours = "0";
                }
                if (Session["NegativeInPatientLast24hours"] != null)
                {
                    ViewBag.NegativeInPatientLast24hours = Session["NegativeInPatientLast24hours"];
                }
                else
                {
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.NegativeInPatientLast24hours = coviddetails[0].NegativeInPatientLast24hours.ToString();
                        Session["NegativeInPatientLast24hours"] = ViewBag.PostivePatientDiagnosisLast24hrs;
                    }
                    else
                        ViewBag.NegativeInPatientLast24hours = "0";
                }
                if (Session["PositiveDischargeCountLast24hours"] != null)
                {
                    ViewBag.PositiveDischargeCountLast24hours = Session["PositiveDischargeCountLast24hours"];
                }
                else
                {
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.PositiveDischargeCountLast24hours = coviddetails[0].PositiveDischargeCountLast24hours.ToString();
                        Session["PositiveDischargeCountLast24hours"] = ViewBag.PositiveDischargeCountLast24hours;
                    }
                    else
                        ViewBag.PositiveDischargeCountLast24hours = "0";
                }
                if (Session["PositiveDeathCountLast24hours"] != null)
                {
                    ViewBag.PositiveDeathCountLast24hours = Session["PositiveDeathCountLast24hours"];
                }
                else
                {
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.PositiveDeathCountLast24hours = coviddetails[0].PositiveDeathCountLast24hours.ToString();
                        Session["PositiveDeathCountLast24hours"] = ViewBag.PositiveDeathCountLast24hours;
                    }
                    else
                        ViewBag.PositiveDeathCountLast24hours = "0";
                }
                int totalRecords = 0;
                if (coviddetails.Count > 0)
                {
                    totalRecords = coviddetails[0].TotalRecords;
                }

                int totalPagesCount = 0;
                int totalpages = 1;
                if (Convert.ToInt32(Session["COVIDPageSize"]) != -1) totalpages = Convert.ToInt32(Session["COVIDPageSize"]);
                else totalpages = totalRecords;
                totalPagesCount = (int)Math.Ceiling((float)totalRecords / totalpages);

                ViewBag.PageNumber = pageNumber;
                ViewBag.TotalPagesCount = totalPagesCount;
                ViewBag.TotalRecordCount = totalRecords;
                ViewBag.ModalRecordCount = coviddetails.Count;
                ViewBag.PageSize = 10;
                ViewBag.SearchText = searchfield;
            }
            catch(Exception ex)
            {
                dBEngine.LogException(ex.Message, "HomeController", "MoreCOVIDPatientDetails", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));                
            }
            finally
            {
                dBEngine = null;
            }
            return PartialView("_COVIDPatientDetails", coviddetails);
        }

        [HttpPost]
        public ActionResult MoreMortalityDetails(int pageNumber = 1, int pageSize = 10, string searchfield = "", string SortColumn = "", bool frompager = false, bool IsTotal = false, bool IsQAP = false, bool IsMedTriage = false, bool IsMEO = false, bool fromfilter = false)
        {
            bool showdisabled = false;
            if (SortColumn != "")
            {
                if (!frompager)
                {
                    if (Session["Column"] == null || Convert.ToString(Session["Column"]) != SortColumn)
                    {
                        Session["Column"] = SortColumn;
                        Session["SortType"] = null;
                    }
                    if (Session["SortType"] == null)
                        Session["SortType"] = "DESC";
                    else if (Session["SortType"] != null && Convert.ToString(Session["Column"]) == SortColumn && Convert.ToString(Session["SortType"]) == "DESC")
                        Session["SortType"] = "ASC";
                    else if (Session["SortType"] != null && Convert.ToString(Session["Column"]) == SortColumn && Convert.ToString(Session["SortType"]) == "ASC")
                        Session["SortType"] = "DESC";
                }
                else
                {
                    if (Session["Column"] == null || Convert.ToString(Session["Column"]) != SortColumn)
                    {
                        Session["Column"] = SortColumn;
                        Session["SortType"] = null;
                    }
                    if (Session["SortType"] == null)
                        Session["SortType"] = "DESC";
                }
            }
            else
            {
                Session["Column"] = "DateofDeath";
                Session["SortType"] = "DESC";
            }
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);

            if (Session["IsTotal"] == null || fromfilter == true)
                Session["IsTotal"] = IsTotal;
            else
                IsTotal = Convert.ToBoolean(Session["IsTotal"]);

            if (Session["IsQAP"] == null || fromfilter == true)
                Session["IsQAP"] = IsQAP;
            else
                IsQAP = Convert.ToBoolean(Session["IsQAP"]);
            if (Session["IsMedTriage"] == null || fromfilter == true)
                Session["IsMedTriage"] = IsMedTriage;
            else
                IsMedTriage = Convert.ToBoolean(Session["IsMedTriage"]);
            if (Session["IsMEO"] == null || fromfilter == true)
                Session["IsMEO"] = IsMEO;
            else
                IsMEO = Convert.ToBoolean(Session["IsMEO"]);

            if (pageSize != Convert.ToInt32(Session["MortalityPageSize"]))
                Session["MortalityPageSize"] = pageSize;
            if (Session["MortalitySortType"] == null)
                Session["MortalitySortType"] = "DESC";
            List<clsPatientDetails> patientDetails = dBEngine.GetFilteredPatientDetails(pageNumber, Convert.ToInt32(Session["MortalityPageSize"]), DateTime.ParseExact(Session["StartDate"].ToString(), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), DateTime.ParseExact(Session["EndDate"].ToString(), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), Convert.ToString(Session["PatientType"]), Convert.ToString(Session["Speciality"]), Convert.ToString(Session["WardDeath"]), Convert.ToString(Session["DischargeConsultant"]), IsTotal, IsQAP, IsMedTriage, IsMEO, Session["Column"].ToString(), Session["SortType"].ToString(), searchfield, showdisabled, Convert.ToInt32(Session["LoginUserID"]));
            if (Session["TotalRecords"] != null)
                ViewBag.Total = Session["TotalRecords"];
            else
            {
                if (patientDetails.Count > 0)
                {
                    ViewBag.TotalRecords = patientDetails[0].TotalRecords.ToString();
                    Session["TotalRecords"] = ViewBag.TotalRecords;
                }
                else
                    ViewBag.TotalRecords = "0";
            }
            if (Session["QAPCount"] != null)
                ViewBag.QAPCount = Session["QAPCount"];
            else
            {
                if (patientDetails.Count > 0)
                {
                    ViewBag.QAPCount = patientDetails[0].QAPCount.ToString();
                    Session["QAPCount"] = ViewBag.QAPCount;
                }
                else
                    ViewBag.QAPCount = "0";
            }
            if (Session["MedCount"] != null)
                ViewBag.MedCount = Session["MedCount"];
            else
            {
                if (patientDetails.Count > 0)
                {
                    ViewBag.MedCount = patientDetails[0].MedCount.ToString();
                    Session["MedCount"] = ViewBag.MedCount;
                }
                else
                    ViewBag.MedCount = "0";
            }
            if (Session["MEOCount"] != null)
                ViewBag.MEOCount = Session["MEOCount"];
            else
            {
                if (patientDetails.Count > 0)
                {
                    ViewBag.MEOCount = patientDetails[0].MEOCount.ToString();
                    Session["MEOCount"] = ViewBag.PendingTestCases;
                }
                else
                    ViewBag.MEOCount = "0";
            }

            int totalRecords = 0;
            if (patientDetails.Count > 0)
            {
                totalRecords = patientDetails[0].TotalRecords;
            }

            int totalPagesCount = 0;
            int totalpages = 1;
            if (Convert.ToInt32(Session["MortalityPageSize"]) != -1) totalpages = Convert.ToInt32(Session["MortalityPageSize"]);
            else totalpages = totalRecords;
            totalPagesCount = (int)Math.Ceiling((float)totalRecords / totalpages);

            ViewBag.PageNumber = pageNumber;
            ViewBag.TotalPagesCount = totalPagesCount;
            ViewBag.TotalRecordCount = totalRecords;
            ViewBag.ModalRecordCount = patientDetails.Count;
            ViewBag.PageSize = 10;
            ViewBag.SearchText = searchfield;
            return PartialView("_MortalityDashboard", patientDetails);
        }

        [HttpPost]
        public ActionResult MorePatientExtract(int pageNumber = 1, int pageSize = 10, string searchfield = "", string SortColumn = "", bool frompager = false, bool fromfilter = false)
        {
            bool showdisabled = false;
            Session["CommonSearchText"] = searchfield;
            if (SortColumn != "")
            {
                if (!frompager)
                {
                    if (Session["CommonColumn"] == null || Convert.ToString(Session["CommonColumn"]) != SortColumn)
                    {
                        Session["CommonColumn"] = SortColumn;
                        Session["ExtractSortType"] = null;
                    }
                    if (Session["ExtractSortType"] == null)
                        Session["ExtractSortType"] = "DESC";
                    else if (Session["ExtractSortType"] != null && Convert.ToString(Session["CommonColumn"]) == SortColumn && Convert.ToString(Session["ExtractSortType"]) == "DESC")
                        Session["ExtractSortType"] = "ASC";
                    else if (Session["ExtractSortType"] != null && Convert.ToString(Session["CommonColumn"]) == SortColumn && Convert.ToString(Session["ExtractSortType"]) == "ASC")
                        Session["ExtractSortType"] = "DESC";
                }
                else
                {
                    if (Session["CommonColumn"] == null || Convert.ToString(Session["CommonColumn"]) != SortColumn)
                    {
                        Session["CommonColumn"] = SortColumn;
                        Session["ExtractSortType"] = null;
                    }
                    if (Session["ExtractSortType"] == null)
                        Session["ExtractSortType"] = "DESC";
                }
            }
            else
            {
                Session["CommonColumn"] = "DateofDeath";
                Session["ExtractSortType"] = "DESC";
            }            
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);

            if (pageSize != Convert.ToInt32(Session["CommonPageSize"]))
                Session["CommonPageSize"] = pageSize;
            if (Session["ExtractSortType"] == null)
                Session["ExtractSortType"] = "DESC";
            List<clsPatientDetails> patientDetails = dBEngine.GetFilteredPatientDetails(pageNumber, Convert.ToInt32(Session["CommonPageSize"]), DateTime.ParseExact(Session["ReportStartDate"].ToString(), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), DateTime.ParseExact(Session["ReportEndDate"].ToString(), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), Convert.ToString(Session["ReportPatientType"]), Convert.ToString(Session["ReportDischargeSpeciality"]), Convert.ToString(Session["ReportWardDeath"]), Convert.ToString(Session["ReportDischargeConsultant"]),false,false,false,false, Session["CommonColumn"].ToString(), Session["ExtractSortType"].ToString(), searchfield, showdisabled, Convert.ToInt32(Session["LoginUserID"]));
            int totalRecords = 0;
            if (patientDetails.Count > 0)
            {
                totalRecords = patientDetails[0].TotalRecords;
            }

            int totalPagesCount = 0;
            int totalpages = 1;
            if (Convert.ToInt32(Session["CommonPageSize"]) != -1) totalpages = Convert.ToInt32(Session["CommonPageSize"]);
            else totalpages = totalRecords;
            totalPagesCount = (int)Math.Ceiling((float)totalRecords / totalpages);

            ViewBag.PageNumber = pageNumber;
            ViewBag.TotalPagesCount = totalPagesCount;
            ViewBag.TotalRecordCount = totalRecords;
            ViewBag.ModalRecordCount = patientDetails.Count;
            if (Convert.ToInt32(Session["CommonPageSize"]) != -1)
                ViewBag.PageSize = Convert.ToInt32(Session["CommonPageSize"]);
            else
                ViewBag.PageSize = totalRecords;
            ViewBag.SearchText = searchfield;
            return PartialView("_PatientExtract", patientDetails);
        }

        [HttpPost]
        public ActionResult MorePatientListing(int pPageNo = 1, string searchfield = "", int pageSize = 0, string SortColumn = "", bool frompager = false, bool IsExternalComms = false, bool IsBreathing = false, bool IsLevelOfCare = false, bool IsTotalCurrentIPCount = false, string PatientLocation = "", bool IsCurrentIPCount = false, bool IsNotUpdatedLast20HoursCount = false, bool IsNotUpdatedLast12HoursCount = false, bool IsUpdatedLast12HoursCount = false, bool NoOxygen = false, bool Oxygen = false, bool NonInvasiveVentilation = false, bool MechanicalVentilation = false, bool NotUpdatedAtAll = false, bool IsNewPositiveYTD = false, bool IsNewPositiveYTDPending = false, bool IsICUHDUStepUpYTD = false, bool IsICUHDUStepUpYTDPending = false, bool IsICUHDUStepDownYTD = false, bool IsICUHDUStepDownYTDPending = false, bool IsDischargeDeathYTD = false, bool IsDischargeDeathYTDPending = false, bool IsDischargesYTD = false, bool IsDischargesYTDPending = false, bool IsPositiveDeathsYTD = false, bool IsPositiveDeathsYTDPending = false, bool IsDeathYTD = false, bool IsDeathYTDPending = false, bool IsDeathDetected = false, bool IsDeathDetectedPending = false, bool IsDeathDiagnosed = false, bool IsDeathDiagnosedPending = false, bool IsDeathReAdmission = false, bool IsDeathReAdmissionPending = false, bool IsCHESSNewPositiveNotRequiredYTD = false, bool IsCHESSICUHDUStepUpNotRequiredYTD = false, bool IsCHESSICUHDUStepDownNotRequiredYTD = false, bool IsCHESSDischargeDeathNotRequiredYTD = false, bool IsCHESSDischargesNotRequiredYTD = false, bool IsCHESSPositiveDeathsNotRequiredYTD = false, bool IsCPNSDeathNotRequiredYTD = false, bool IsCPNSDeathDetectedNotRequired = false, bool IsCPNSDeathDiagnosedNotRequired = false, bool IsCPNSDeathReAdmissionNotRequired = false, bool fromfilter = false)
        {
            Session["PatientListSearchText"] = searchfield;
            if (SortColumn != "")
            {
                if (!frompager)
                {
                    if (Session["PatientListColumn"] == null || Convert.ToString(Session["PatientListColumn"]) != SortColumn)
                    {
                        Session["PatientListColumn"] = SortColumn;
                        Session["PatientListSortType"] = null;
                    }
                    if (Session["PatientListSortType"] == null)
                        Session["PatientListSortType"] = "DESC";
                    else if (Session["PatientListSortType"] != null && Convert.ToString(Session["PatientListColumn"]) == SortColumn && Convert.ToString(Session["PatientListSortType"]) == "DESC")
                        Session["PatientListSortType"] = "ASC";
                    else if (Session["PatientListSortType"] != null && Convert.ToString(Session["PatientListColumn"]) == SortColumn && Convert.ToString(Session["PatientListSortType"]) == "ASC")
                        Session["PatientListSortType"] = "DESC";
                }
                else
                {
                    if (Session["PatientListColumn"] == null || Convert.ToString(Session["PatientListColumn"]) != SortColumn)
                    {
                        Session["PatientListColumn"] = SortColumn;
                        Session["PatientListSortType"] = null;
                    }
                    if (Session["PatientListSortType"] == null)
                        Session["PatientListSortType"] = "DESC";
                }
            }
            else
            {
                Session["PatientListColumn"] = "TestResultDateTime";
                Session["PatientListSortType"] = "DESC";
            }
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);

            if (pageSize != Convert.ToInt32(Session["PatientListPageSize"]))
                Session["PatientListPageSize"] = pageSize;
            if (Session["ExtractSortType"] == null)
                Session["ExtractSortType"] = "DESC";
            Session["PatientListPageNo"] = pPageNo;
            Session["PatientListSearchText"] = searchfield;

            List<clsCOVIDPatientList> patientDetails = dBEngine.GetCOVIDPatientList(Convert.ToInt32(Session["PatientListPageNo"]), Convert.ToInt32(Session["PatientListPageSize"]), Convert.ToString(Session["PatientListSearchText"]), Convert.ToString(Session["PatientLocation"]), Convert.ToBoolean(Session["IsCurrentIPCount"]), Convert.ToBoolean(Session["IsNotUpdatedLast20HoursCount"]),
                Convert.ToBoolean(Session["IsNotUpdatedLast12HoursCount"]), Convert.ToBoolean(Session["IsUpdatedLast12HoursCount"]), Convert.ToBoolean(Session["NoOxygen"]), Convert.ToBoolean(Session["Oxygen"]), Convert.ToBoolean(Session["NonInvasiveVentilation"]), Convert.ToBoolean(Session["MechanicalVentilation"]), Convert.ToBoolean(Session["NotUpdatedAtAll"]),
                 Convert.ToBoolean(Session["IsNewPositiveYTD"]), Convert.ToBoolean(Session["IsNewPositiveYTDPending"]), Convert.ToBoolean(Session["IsICUHDUStepUpYTD"]), Convert.ToBoolean(Session["IsICUHDUStepUpYTDPending"]), Convert.ToBoolean(Session["IsICUHDUStepDownYTD"]),
                Convert.ToBoolean(Session["IsICUHDUStepDownYTDPending"]), Convert.ToBoolean(Session["IsDischargeDeathYTD"]), Convert.ToBoolean(Session["IsDischargeDeathYTDPending"]), Convert.ToBoolean(Session["IsDischargesYTD"]), Convert.ToBoolean(Session["IsDischargesYTDPending"]), Convert.ToBoolean(Session["IsPositiveDeathsYTD"]), Convert.ToBoolean(Session["IsPositiveDeathsYTDPending"]), Convert.ToBoolean(Session["IsDeathYTD"]), Convert.ToBoolean(Session["IsDeathYTDPending"]), Convert.ToBoolean(Session["IsDeathDetected"]), Convert.ToBoolean(Session["IsDeathDetectedPending"]), Convert.ToBoolean(Session["IsDeathDiagnosed"]), Convert.ToBoolean(Session["IsDeathDiagnosedPending"]), Convert.ToBoolean(Session["IsDeathReAdmission"]), Convert.ToBoolean(Session["IsDeathReAdmissionPending"]), Convert.ToBoolean(Session["IsCHESSNewPositiveNotRequiredYTD"]), Convert.ToBoolean(Session["IsCHESSICUHDUStepUpNotRequiredYTD"]), Convert.ToBoolean(Session["IsCHESSICUHDUStepDownNotRequiredYTD"]), Convert.ToBoolean(Session["IsCHESSDischargeDeathNotRequiredYTD"]), Convert.ToBoolean(Session["IsCHESSDischargesNotRequiredYTD"]), Convert.ToBoolean(Session["IsCHESSPositiveDeathsNotRequiredYTD"]), Convert.ToBoolean(Session["IsCPNSDeathNotRequiredYTD"]), Convert.ToBoolean(Session["IsCPNSDeathDetectedNotRequired"]), Convert.ToBoolean(Session["IsCPNSDeathDiagnosedNotRequired"]), Convert.ToBoolean(Session["IsCPNSDeathReAdmissionNotRequired"]), Convert.ToString(Session["PatientListColumn"]), Convert.ToString(Session["PatientListSortType"]), Convert.ToString(Session["BreathingReportTestResults"]), Convert.ToInt32(Session["LoginUserID"]));
            int totalRecords = 0;
            if (patientDetails.Count > 0)
            {
                totalRecords = patientDetails[0].TotalRecords;
            }

            int totalPagesCount = 0;
            int totalpages = 1;
            if (Convert.ToInt32(Session["PatientListPageSize"]) != -1) totalpages = Convert.ToInt32(Session["PatientListPageSize"]);
            else totalpages = totalRecords;
            totalPagesCount = (int)Math.Ceiling((float)totalRecords / totalpages);

            ViewBag.PageNumber = Convert.ToInt32(Session["PatientListPageNo"]);
            ViewBag.SearchText = Convert.ToString(Session["PatientListSearchText"]);
            ViewBag.TotalPagesCount = totalPagesCount;
            ViewBag.TotalRecordCount = totalRecords;
            ViewBag.ModalRecordCount = patientDetails.Count;
            if (Convert.ToInt32(Session["PatientListPageSize"]) != -1)
                ViewBag.PageSize = Convert.ToInt32(Session["PatientListPageSize"]);
            else
                ViewBag.PageSize = totalRecords;
            ViewBag.SearchText = searchfield;
            return PartialView("_COVIDPatientListing", patientDetails);
        }

        

        [HttpPost]

        public ActionResult MoreBreathingSupportReport (int pageNumber = 1, int pageSize = 50, string searchfield = "", string SortColumn = "", bool frompager = false)
        {
            if (SortColumn != "")
            {
                if (!frompager)
                {
                    if (Session["BreathingReportColumn"] == null || Convert.ToString(Session["TestColumn"]) != SortColumn)
                    {
                        Session["BreathingReportColumn"] = SortColumn;
                        Session["BreathingReportSortType"] = null;
                    }
                    if (Session["BreathingReportSortType"] == null)
                        Session["BreathingReportSortType"] = "DESC";
                    else if (Session["BreathingReportSortType"] != null && Convert.ToString(Session["BreathingReportColumn"]) == SortColumn && Convert.ToString(Session["BreathingReportSortType"]) == "DESC")
                        Session["BreathingReportSortType"] = "ASC";
                    else if (Session["BreathingReportSortType"] != null && Convert.ToString(Session["BreathingReportColumn"]) == SortColumn && Convert.ToString(Session["BreathingReportSortType"]) == "ASC")
                        Session["BreathingReportSortType"] = "DESC";
                }
                else
                {
                    if (Session["BreathingReportColumn"] == null || Convert.ToString(Session["BreathingReportColumn"]) != SortColumn)
                    {
                        Session["BreathingReportColumn"] = SortColumn;
                        Session["BreathingReportSortType"] = null;
                    }
                    if (Session["BreathingReportSortType"] == null)
                        Session["BreathingReportSortType"] = "DESC";
                }
            }
            else
            {
                Session["BreathingReportColumn"] = "LastPatientLocation";
                Session["BreathingReportSortType"] = "ASC";
            }
            if (pageSize != Convert.ToInt32(Session["BreathingSupportReportPageSize"]))
                Session["BreathingSupportReportPageSize"] = pageSize;
            if (Session["BreathingReportSortType"] == null)
                Session["BreathingReportSortType"] = "DESC";
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            BreathingSupportReport breathingreport = dBEngine.GetCOVIDBreathingSupportReport(pageNumber, Convert.ToInt32(Session["BreathingSupportReportPageSize"]), "", "", Convert.ToString(Session["BreathingReportTestResults"]), Convert.ToInt32(Session["LoginUserID"]));
            int totalRecords = 0;
            if (breathingreport.BreathingTracker.Count > 0)
            {
                totalRecords = breathingreport.BreathingTracker[0].TotalRecords;
            }

            int totalPagesCount = 0;
            int totalpages = 1;
            if (Convert.ToInt32(Session["BreathingSupportReportPageSize"]) != -1) totalpages = Convert.ToInt32(Session["BreathingSupportReportPageSize"]);
            else totalpages = totalRecords;
            totalPagesCount = (int)Math.Ceiling((float)totalRecords / totalpages);

            ViewBag.PageNumber = pageNumber;
            ViewBag.TotalPagesCount = totalPagesCount;
            ViewBag.TotalRecordCount = totalRecords;
            ViewBag.ModalRecordCount = breathingreport.BreathingTracker.Count;
            ViewBag.PageSize = Convert.ToInt32(Session["BreathingReportPageSize"]);
            ViewBag.SearchText = searchfield;
            return PartialView("_BreathingTracker", breathingreport);
        }

        [HttpPost]
        public ActionResult MoreCOVIDDetails(int pageNumber = 1, int pageSize = 10, string searchfield ="", string SortColumn = "", bool frompager = false, bool IsTotalTestOrders = false, bool IsTotalTestOrdersLast24hours = false, bool IsPostive = false, bool IsNegative = false, bool IsPending = false, bool IsPostiveNotNotified = false, bool IsNegativeNotNotified = false, bool IsPendingGreaterthan2days = false, bool IsPositiveLast24hours = false, bool IsPositiveInPatientLast24hours = false, bool IsNegativeLast24hours = false, bool IsPostiveNotNotifiedLast24hours = false, bool IsInPatientNotNotifiedLast24hours = false, bool IsNegativeNotNotifiedLast24hours = false, bool IsPositiveAdmissions = false, bool IsPositiveDeaths = false, bool IsPositiveDischargesLast24hours = false, bool IsPositiveDeathLast24hours = false, bool fromfilter = false)
        {
            if (SortColumn != "")
            {
                if (!frompager)
                {
                    if (Session["TestColumn"] == null || Convert.ToString(Session["TestColumn"]) != SortColumn)
                    {
                        Session["TestColumn"] = SortColumn;
                        Session["TestSortType"] = null;
                    }
                    if (Session["TestSortType"] == null)
                        Session["TestSortType"] = "DESC";
                    else if (Session["TestSortType"] != null && Convert.ToString(Session["TestColumn"]) == SortColumn && Convert.ToString(Session["TestSortType"]) == "DESC")
                        Session["TestSortType"] = "ASC";
                    else if (Session["TestSortType"] != null && Convert.ToString(Session["TestColumn"]) == SortColumn && Convert.ToString(Session["TestSortType"]) == "ASC")
                        Session["TestSortType"] = "DESC";
                }
                else
                {
                    if (Session["TestColumn"] == null || Convert.ToString(Session["TestColumn"]) != SortColumn)
                    {
                        Session["TestColumn"] = SortColumn;
                        Session["TestSortType"] = null;
                    }
                    if (Session["TestSortType"] == null)
                        Session["TestSortType"] = "DESC";
                }
            }
            else
            {
                Session["TestColumn"] = "OrderDateTime";
                Session["TestSortType"] = "DESC";
            }
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            List<clsCOVIDDetails> coviddetails = new List<clsCOVIDDetails>();
            try
            {
                if (Session["IsTotalTestOrders"] == null || fromfilter == true)
                    Session["IsTotalTestOrders"] = IsTotalTestOrders;
                else
                    IsTotalTestOrders = Convert.ToBoolean(Session["IsTotalTestOrders"]);

                if (Session["IsPostive"] == null || fromfilter == true)
                    Session["IsPostive"] = IsPostive;
                else
                    IsPostive = Convert.ToBoolean(Session["IsPostive"]);
                if (Session["IsNegative"] == null || fromfilter == true)
                    Session["IsNegative"] = IsNegative;
                else
                    IsNegative = Convert.ToBoolean(Session["IsNegative"]);
                if (Session["IsPending"] == null || fromfilter == true)
                    Session["IsPending"] = IsPending;
                else
                    IsPending = Convert.ToBoolean(Session["IsPending"]);
                if (Session["IsPostiveNotNotified"] == null || fromfilter == true)
                    Session["IsPostiveNotNotified"] = IsPostiveNotNotified;
                else
                    IsPostiveNotNotified = Convert.ToBoolean(Session["IsPostiveNotNotified"]);
                if (Session["IsNegativeNotNotified"] == null || fromfilter == true)
                    Session["IsNegativeNotNotified"] = IsNegativeNotNotified;
                else
                    IsNegativeNotNotified = Convert.ToBoolean(Session["IsNegativeNotNotified"]);
                if (Session["IsPendingGreaterthan2days"] == null || fromfilter == true)
                    Session["IsPendingGreaterthan2days"] = IsPendingGreaterthan2days;
                else
                    IsPendingGreaterthan2days = Convert.ToBoolean(Session["IsPendingGreaterthan2days"]);
                if (Session["IsTotalTestOrdersLast24hours"] == null || fromfilter == true)
                    Session["IsTotalTestOrdersLast24hours"] = IsTotalTestOrdersLast24hours;
                else
                    IsTotalTestOrdersLast24hours = Convert.ToBoolean(Session["IsTotalTestOrdersLast24hours"]);
                if (Session["IsPositiveLast24hours"] == null || fromfilter == true)
                    Session["IsPositiveLast24hours"] = IsPositiveLast24hours;
                else
                    IsPositiveLast24hours = Convert.ToBoolean(Session["IsPositiveLast24hours"]);
                if (Session["IsPositiveInPatientLast24hours"] == null || fromfilter == true)
                    Session["IsPositiveInPatientLast24hours"] = IsPositiveInPatientLast24hours;
                else
                    IsPositiveInPatientLast24hours = Convert.ToBoolean(Session["IsPositiveInPatientLast24hours"]);
                if (Session["IsNegativeLast24hours"] == null || fromfilter == true)
                    Session["IsNegativeLast24hours"] = IsNegativeLast24hours;
                else
                    IsNegativeLast24hours = Convert.ToBoolean(Session["IsNegativeLast24hours"]);
                if (Session["IsPostiveNotNotifiedLast24hours"] == null || fromfilter == true)
                    Session["IsPostiveNotNotifiedLast24hours"] = IsPostiveNotNotifiedLast24hours;
                else
                    IsPostiveNotNotifiedLast24hours = Convert.ToBoolean(Session["IsPostiveNotNotifiedLast24hours"]);
                if (Session["IsInPatientNotNotifiedLast24hours"] == null || fromfilter == true)
                    Session["IsInPatientNotNotifiedLast24hours"] = IsInPatientNotNotifiedLast24hours;
                else
                    IsInPatientNotNotifiedLast24hours = Convert.ToBoolean(Session["IsInPatientNotNotifiedLast24hours"]);
                if (Session["IsNegativeNotNotifiedLast24hours"] == null || fromfilter == true)
                    Session["IsNegativeNotNotifiedLast24hours"] = IsNegativeNotNotifiedLast24hours;
                else
                    IsNegativeNotNotifiedLast24hours = Convert.ToBoolean(Session["IsNegativeNotNotifiedLast24hours"]);
                if (Session["IsPositiveAdmissions"] == null || fromfilter == true)
                    Session["IsPositiveAdmissions"] = IsPositiveAdmissions;
                else
                    IsPositiveAdmissions = Convert.ToBoolean(Session["IsPositiveAdmissions"]);
                if (Session["IsPositiveDeaths"] == null || fromfilter == true)
                    Session["IsPositiveDeaths"] = IsPositiveDeaths;
                else
                    IsPositiveDeaths = Convert.ToBoolean(Session["IsPositiveDeaths"]);
                if (Session["IsPositiveDischargesLast24hours"] == null || fromfilter == true)
                    Session["IsPositiveDischargesLast24hours"] = IsPositiveDischargesLast24hours;
                else
                    IsPositiveDischargesLast24hours = Convert.ToBoolean(Session["IsPositiveDischargesLast24hours"]);
                if (Session["IsPositiveDeathLast24hours"] == null || fromfilter == true)
                    Session["IsPositiveDeathLast24hours"] = IsPositiveDeathLast24hours;
                else
                    IsPositiveDeathLast24hours = Convert.ToBoolean(Session["IsPositiveDeathLast24hours"]);
                if (pageSize != Convert.ToInt32(Session["TestPageSize"]))
                    Session["TestPageSize"] = pageSize;
                if (Session["TestSortType"] == null)
                    Session["TestSortType"] = "DESC";
                Session["TestPageNo"] = pageNumber;
                coviddetails = dBEngine.GetCOVIDDetails(DateTime.ParseExact(Session["COVIDStartDate"].ToString(), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), DateTime.ParseExact(Session["COVIDEndDate"].ToString(), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), Session["TestNotification"].ToString(), Session["TestTestStatus"].ToString(),
                            Session["TestTestResults"].ToString(), Session["TestTestOrderLocation"].ToString(), Session["TestLastLocation"].ToString(), Session["TestAdmissionStatus"].ToString(), pageNumber, Convert.ToInt32(Session["TestPageSize"]), searchfield, Convert.ToBoolean(Session["IsTotalTestOrders"]), Convert.ToBoolean(Session["IsTotalTestOrdersLast24hours"]), Convert.ToBoolean(Session["IsPostive"]), Convert.ToBoolean(Session["IsNegative"]), Convert.ToBoolean(Session["IsPending"]), Convert.ToBoolean(Session["IsPostiveNotNotified"]), Convert.ToBoolean(Session["IsNegativeNotNotified"]), Convert.ToBoolean(Session["IsPendingGreaterthan2days"]), Convert.ToBoolean(Session["IsPositiveLast24hours"]), Convert.ToBoolean(Session["IsPositiveInPatientLast24hours"]), Convert.ToBoolean(Session["IsNegativeLast24hours"]), Convert.ToBoolean(Session["IsPostiveNotNotifiedLast24hours"]), Convert.ToBoolean(Session["IsInPatientNotNotifiedLast24hours"]), Convert.ToBoolean(Session["IsNegativeNotNotifiedLast24hours"]), Convert.ToBoolean(Session["IsPositiveAdmissions"]), Convert.ToBoolean(Session["IsPositiveDeaths"]), Convert.ToBoolean(Session["IsPositiveDischargesLast24hours"]), Convert.ToBoolean(Session["IsPositiveDeathLast24hours"]), Session["TestColumn"].ToString(), Session["TestSortType"].ToString(), Convert.ToInt32(Session["LoginUserID"]));
                if (Session["TotalTestsOrdered"] != null)
                    ViewBag.TotalTestsOrdered = Session["TotalTestsOrdered"];
                else
                {
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.TotalTestsOrdered = coviddetails[0].TotalTestsOrdered.ToString();
                        Session["TotalTestsOrdered"] = ViewBag.TotalTestsOrdered;
                    }
                    else
                        ViewBag.TotalTestsOrdered = "0";
                }
                if (Session["PositiveTestCases"] != null)
                    ViewBag.PositiveTestCases = Session["PositiveTestCases"];
                else
                {
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.PositiveTestCases = coviddetails[0].PositiveTestCases.ToString();
                        Session["PositiveTestCases"] = ViewBag.PositiveTestCases;
                    }
                    else
                        ViewBag.PositiveTestCases = "0";
                }
                if (Session["NegativeTestCases"] != null)
                    ViewBag.NegativeTestCases = Session["NegativeTestCases"];
                else
                {
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.NegativeTestCases = coviddetails[0].NegativeTestCases.ToString();
                        Session["NegativeTestCases"] = ViewBag.NegativeTestCases;
                    }
                    else
                        ViewBag.NegativeTestCases = "0";
                }
                if (Session["PendingTestCases"] != null)
                    ViewBag.PendingTestCases = Session["PendingTestCases"];
                else
                {
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.PendingTestCases = coviddetails[0].PendingTestCases.ToString();
                        Session["PendingTestCases"] = ViewBag.PendingTestCases;
                    }
                    else
                        ViewBag.PendingTestCases = "0";
                }
                if (Session["AdmissionCount"] != null)
                    ViewBag.AdmissionCount = Session["AdmissionCount"];
                else
                {
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.AdmissionCount = coviddetails[0].AdmissionCount.ToString();
                        Session["AdmissionCount"] = ViewBag.AdmissionCount;
                    }
                    else
                        ViewBag.AdmissionCount = "0";
                }
                if (Session["DeathCount"] != null)
                    ViewBag.DeathCount = Session["DeathCount"];
                else
                {
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.DeathCount = coviddetails[0].DeathCount.ToString();
                        Session["DeathCount"] = ViewBag.DeathCount;
                    }
                    else
                        ViewBag.DeathCount = "0";
                }
                if (Session["NotNotifiedPositiveCount"] != null)
                    ViewBag.NotNotifiedPositiveCount = Session["NotNotifiedPositiveCount"];
                else
                {
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.NotNotifiedPositiveCount = coviddetails[0].NotNotifiedPositiveCount.ToString();
                        Session["NotNotifiedPositiveCount"] = ViewBag.NotNotifiedPositiveCount;
                    }
                    else
                        ViewBag.NotNotifiedPositiveCount = "0";
                }
                if (Session["NotNotifiedNegativeCount"] != null)
                    ViewBag.NotNotifiedNegativeCount = Session["NotNotifiedNegativeCount"];
                else
                {
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.NotNotifiedNegativeCount = coviddetails[0].NotNotifiedNegativeCount.ToString();
                        Session["NotNotifiedNegativeCount"] = ViewBag.NotNotifiedNegativeCount;
                    }
                    else
                        ViewBag.NotNotifiedNegativeCount = "0";
                }
                if (Session["PendingTestsOver2days"] != null)
                    ViewBag.PendingTestsOver2days = Session["PendingTestsOver2days"];
                else
                {
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.PendingTestsOver2days = coviddetails[0].PendingTestsOver2days.ToString();
                        Session["PendingTestsOver2days"] = ViewBag.PendingTestsOver2days;
                    }
                    else
                        ViewBag.PendingTestsOver2days = "0";
                }
                if (Session["NotifiedAdmissionCount"] != null)
                    ViewBag.NotNotifiedAdmissionCount = Session["NotifiedAdmissionCount"];
                else
                {
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.NotNotifiedAdmissionCount = coviddetails[0].NotNotifiedAdmissionCount.ToString();
                        Session["NotifiedAdmissionCount"] = ViewBag.NotNotifiedAdmissionCount;
                    }
                    else
                        ViewBag.NotNotifiedAdmissionCount = "0";
                }
                if (Session["NotifiedDeathCount"] != null)
                    ViewBag.NotNotifiedDeathCount = Session["NotifiedDeathCount"];
                else
                {
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.NotNotifiedDeathCount = coviddetails[0].NotNotifiedDeathCount.ToString();
                        Session["NotifiedDeathCount"] = ViewBag.NotNotifiedDeathCount;
                    }
                    else
                        ViewBag.NotNotifiedDeathCount = "0";
                }
                if (Session["TotalTestsOrderedLast24hrs"] != null)
                    ViewBag.TotalTestsOrderedLast24hrs = Session["TotalTestsOrderedLast24hrs"];
                else
                {
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.TotalTestsOrderedLast24hrs = coviddetails[0].TotalTestsOrderedLast24hrs.ToString();
                        Session["TotalTestsOrderedLast24hrs"] = ViewBag.TotalTestsOrderedLast24hrs;
                    }
                    else
                        ViewBag.TotalTestsOrderedLast24hrs = "0";
                }
                if (Session["PostivePatientDiagnosisLast24hrs"] != null)
                    ViewBag.PostivePatientDiagnosisLast24hrs = Session["PostivePatientDiagnosisLast24hrs"];
                else
                {
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.PostivePatientDiagnosisLast24hrs = coviddetails[0].PostivePatientDiagnosisLast24hrs.ToString();
                        Session["PostivePatientDiagnosisLast24hrs"] = ViewBag.PostivePatientDiagnosisLast24hrs;
                    }
                    else
                        ViewBag.PostivePatientDiagnosisLast24hrs = "0";
                }
                if (Session["PositiveInpatientDiagnosisLast24hrs"] != null)
                    ViewBag.PositiveInpatientDiagnosisLast24hrs = Session["PositiveInpatientDiagnosisLast24hrs"];
                else
                {
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.PositiveInpatientDiagnosisLast24hrs = coviddetails[0].PositiveInpatientDiagnosisLast24hrs.ToString();
                        Session["PositiveInpatientDiagnosisLast24hrs"] = ViewBag.PositiveInpatientDiagnosisLast24hrs;
                    }
                    else
                        ViewBag.PositiveInpatientDiagnosisLast24hrs = "0";
                }
                if (Session["NegativeLast24hours"] != null)
                    ViewBag.NegativeLast24hours = Session["NegativeLast24hours"];
                else
                {
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.NegativeLast24hours = coviddetails[0].NegativeLast24hours.ToString();
                        Session["NegativeLast24hours"] = ViewBag.NegativeLast24hours;
                    }
                    else
                        ViewBag.NegativeLast24hours = "0";
                }
                if (Session["DeathsLast24hrs"] != null)
                    ViewBag.DeathsLast24hrs = Session["DeathsLast24hrs"];
                else
                {
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.DeathsLast24hrs = coviddetails[0].DeathsLast24hrs.ToString();
                        Session["DeathsLast24hrs"] = ViewBag.DeathsLast24hrs;
                    }
                    else
                        ViewBag.DeathsLast24hrs = "0";
                }
                if (Session["DischargesLast24hrs"] != null)
                    ViewBag.DischargesLast24hrs = Session["DischargesLast24hrs"];
                else
                {
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.DischargesLast24hrs = coviddetails[0].DischargesLast24hrs.ToString();
                        Session["DischargesLast24hrs"] = ViewBag.DischargesLast24hrs;
                    }
                    else
                        ViewBag.DischargesLast24hrs = "0";
                }

                if (Session["NotNotifiedPostivePatientDiagnosisLast24hrs"] != null)
                    ViewBag.NotNotifiedPostivePatientDiagnosisLast24hrs = Session["NotNotifiedPostivePatientDiagnosisLast24hrs"];
                else
                {
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.NotNotifiedPostivePatientDiagnosisLast24hrs = coviddetails[0].NotNotifiedPostivePatientDiagnosisLast24hrs.ToString();
                        Session["NotNotifiedPostivePatientDiagnosisLast24hrs"] = ViewBag.NotNotifiedPostivePatientDiagnosisLast24hrs;
                    }
                    else
                        ViewBag.NotNotifiedPostivePatientDiagnosisLast24hrs = "0";
                }
                if (Session["NotNotifiedInPatientDiagnosisLast24hrs"] != null)
                    ViewBag.NotNotifiedInPatientDiagnosisLast24hrs = Session["NotNotifiedInPatientDiagnosisLast24hrs"];
                else
                {
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.NotNotifiedInPatientDiagnosisLast24hrs = coviddetails[0].NotNotifiedInPatientDiagnosisLast24hrs.ToString();
                        Session["NotNotifiedInPatientDiagnosisLast24hrs"] = ViewBag.NotNotifiedInPatientDiagnosisLast24hrs;
                    }
                    else
                        ViewBag.NotNotifiedInPatientDiagnosisLast24hrs = "0";
                }
                if (Session["NotNotifiedNegativeLast24hours"] != null)
                    ViewBag.NotNotifiedNegativeLast24hours = Session["NotNotifiedNegativeLast24hours"];
                else
                {
                    if (coviddetails.Count > 0)
                    {
                        ViewBag.NotNotifiedNegativeLast24hours = coviddetails[0].NotNotifiedNegativeLast24hours.ToString();
                        Session["NotNotifiedNegativeLast24hours"] = ViewBag.NotNotifiedNegativeLast24hours;
                    }
                    else
                        ViewBag.NotNotifiedNegativeLast24hours = "0";
                }
                int totalRecords = 0;
                if (coviddetails.Count > 0)
                {
                    totalRecords = coviddetails[0].TotalRecords;
                }

                int totalPagesCount = 0;
                int totalpages = 1;
                if (Convert.ToInt32(Session["TestPageSize"]) != -1) totalpages = Convert.ToInt32(Session["TestPageSize"]);
                else totalpages = totalRecords;
                totalPagesCount = (int)Math.Ceiling((float)totalRecords / totalpages);

                ViewBag.PageNumber = pageNumber;
                ViewBag.TotalPagesCount = totalPagesCount;
                ViewBag.TotalRecordCount = totalRecords;
                ViewBag.ModalRecordCount = coviddetails.Count;
                ViewBag.PageSize = Convert.ToInt32(Session["TestPageSize"]);
                ViewBag.SearchText = searchfield;
            }
            catch(Exception ex)
            {
                dBEngine.LogException(ex.Message, "HomeController", "MoreCOVIDDetails", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
            }
            finally
            { dBEngine = null; }
            return PartialView("_COVIDDetails", coviddetails);
        }

        public ActionResult COVIDReviewCycle(int id, string patientID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            COVIDReviewCycle covidcycle = new Common.DTO.COVIDReviewCycle();
            if (Session["FullName"] != null)
            {
                if (string.IsNullOrEmpty(Session["FullName"].ToString()))
                    return RedirectToAction("Index", "Account");
            }
            else
                return RedirectToAction("Index", "Account");
            try
            {
                covidcycle = dBEngine.GetCOVIDReviewCycle(id, patientID, Convert.ToInt32(Session["LoginUserID"]));
            }
            catch(Exception ex)
            {
                dBEngine.LogException(ex.Message, "HomeController", "COVIDReviewCycle", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
            }
            finally
            {
                dBEngine = null;
            }
            return View(covidcycle);
        }

        public ActionResult COVIDPatientDetails(int id, string patientID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            clsCOVIDDetails patient = new clsCOVIDDetails();
            try
            {
                patient = dBEngine.GetCOVIDPatientDetailsByID(patientID, Convert.ToInt32(Session["LoginUserID"]));
                if (patient.ID != id)
                    patient.ID = id;
                ViewBag.PatientHistoryLink = "'" + "http://rbhdbsred011/TIPS/ReportViewer.aspx?ReportPath=R720_CORS_PatientHistory&MRN=" + patient.PatientID + "'";
            }
            catch(Exception ex)
            {
                dBEngine.LogException(ex.Message, "HomeController", "COVIDPatientDetails", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
            }
            finally
            {
                dBEngine = null;
            }
            return View(patient);
        }

        [HttpPost]
        public ActionResult COVIDTestResults(FormCollection formCollection, int id, string patientID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            bool covidStatusKnown = false;
            try
            {
                patientID = formCollection["hdnPatientID"];
                if (Convert.ToString(formCollection["COVIDStatusKnown1"]) == "on") covidStatusKnown = true;
                if (Convert.ToString(formCollection["COVIDStatusKnown2"]) == "on") covidStatusKnown = false;
                int dbReturnValue = dBEngine.UpdateCOVIDTestResults(id, covidStatusKnown, formCollection["ddlTestResult"], formCollection["TestResults.Comments"], Convert.ToInt32(Session["LoginUserID"]));
            }
            catch(Exception ex)
            {
                dBEngine.LogException(ex.Message, "HomeController", "COVIDTestResults", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
            }
            finally
            {
                dBEngine = null;
            }
            return RedirectToAction("COVIDReviewCycle", new { id = id, patientID = patientID });
        }

        [HttpPost]
        public ActionResult COVIDBreathing(FormCollection formCollection, int? id, string patientID, bool IsPatientList = false)
        {
            if (Session["IsPatientList"] != null)
                IsPatientList = Convert.ToBoolean(Session["IsPatientList"]);
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            bool isoxygen = false;
            bool isnooxygen = false;
            bool isnonivasiveventialtion = false;
            bool ismechanicalventilation = false;
            try
            {
                patientID = formCollection["hdnPatientID"];
                if (Convert.ToString(formCollection["IsOxygen"]) == "on") isoxygen = true;
                if (Convert.ToString(formCollection["IsNoOxygen"]) == "on") isnooxygen = true;
                if (Convert.ToString(formCollection["IsNonEvasiveVentilation"]) == "on") isnonivasiveventialtion = true;
                if (Convert.ToString(formCollection["IsMechanicalVentilation"]) == "on") ismechanicalventilation = true;
                int dbReturnValue = dBEngine.UpdateCOVIDBreathing(patientID, isoxygen, isnooxygen, isnonivasiveventialtion, ismechanicalventilation, Convert.ToInt32(Session["LoginUserID"]));                
            }
            catch(Exception ex)
            {
                dBEngine.LogException(ex.Message, "HomeController", "COVIDBreathingPost", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
            }
            finally
            {
                dBEngine = null;
            }
            if (!IsPatientList)
                return RedirectToAction("COVIDReviewCycle", new { id = id, patientID = patientID });
            else
                return RedirectToAction("COVIDPatientListing", new
                {
                    pPageNo = 1,
                    searchfield = "",
                    pageSize = 10,
                    SortColumn = "",
                    frompager = false,
                    IsExternalComms = Convert.ToBoolean(Session["IsExternalComms"]),
                    IsBreathing = Convert.ToBoolean(Session["IsBreathing"]),
                    IsLevelOfCare = Convert.ToBoolean(Session["IsLevelOfCare"]),
                    PatientLocation = Convert.ToString(Session["PatientLocation"]),
                    IsCurrentIPCount = Convert.ToBoolean(Session["IsCurrentIPCount"]),
                    IsNotUpdatedLast20HoursCount = Convert.ToBoolean(Session["IsNotUpdatedLast20HoursCount"]),
                    IsNotUpdatedLast12HoursCount = Convert.ToBoolean(Session["IsNotUpdatedLast12HoursCount"]),
                    IsUpdatedLast12HoursCount = Convert.ToBoolean(Session["IsUpdatedLast12HoursCount"]),
                    NoOxygen = Convert.ToBoolean(Session["NoOxygen"]),
                    Oxygen = Convert.ToBoolean(Session["Oxygen"]),
                    NonInvasiveVentilation = Convert.ToBoolean(Session["NonInvasiveVentilation"]),
                    MechanicalVentilation = Convert.ToBoolean(Session["MechanicalVentilation"]),
                    NotUpdatedAtAll = Convert.ToBoolean(Session["NotUpdatedAtAll"]),
                    IsNewPositive = Convert.ToBoolean(Session["IsNewPositive"]),
                    IsPostiveBacklog = Convert.ToBoolean(Session["IsPostiveBacklog"]),
                    IsPositiveBacklogPending = Convert.ToBoolean(Session["IsPositiveBacklogPending"]),
                    IsPositiveLive = Convert.ToBoolean(Session["IsPositiveLive"]),
                    IsPositiveLivePending = Convert.ToBoolean(Session["IsPositiveLivePending"]),
                    IsDischargeDeath = Convert.ToBoolean(Session["IsDischargeDeath"]),
                    IsDischargeDeathBacklog = Convert.ToBoolean(Session["IsDischargeDeathBacklog"]),
                    IsDischargeDeathBacklogPending = Convert.ToBoolean(Session["IsDischargeDeathBacklogPending"]),
                    IsDischargeDeathLive = Convert.ToBoolean(Session["IsDischargeDeathLive"]),
                    IsDischargeDeathLivePending = Convert.ToBoolean(Session["IsDischargeDeathLivePending"]),
                    IsDeath = Convert.ToBoolean(Session["IsDeath"]),
                    IsDeathBacklog = Convert.ToBoolean(Session["IsDeathBacklog"]),
                    IsDeathBacklogPending = Convert.ToBoolean(Session["IsDeathBacklogPending"]),
                    IsDeathLive = Convert.ToBoolean(Session["IsDeathLive"]),
                    IsDeathLivePending = Convert.ToBoolean(Session["IsDeathLivePending"])
                });
        }

        [HttpPost]
        public ActionResult UpdateCOVIDTest(FormCollection formCollection, int id, string patientID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            bool isdisabled = false;
            try
            {
                patientID = formCollection["hdnPatientID"];
                if (Convert.ToString(formCollection["IsDisabled"]) == "on") isdisabled = true;
                int dbReturnValue = dBEngine.UpdateCOVIDDataAssurance(id, isdisabled, formCollection["DAComments"], Convert.ToInt32(Session["LoginUserID"]));
            }
            catch(Exception ex)
            {
                dBEngine.LogException(ex.Message, "HomeController", "EditCOVIDTestPost", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
            }
            finally { dBEngine = null; }
            return RedirectToAction("COVIDDataAssurance", new { id = id, patientID = patientID });
        }

        public ActionResult EditCOVIDTest(int? id, string patientID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            COVIDDataAssurance dataassurance = new Common.DTO.COVIDDataAssurance();
            try
            {
                dataassurance = dBEngine.GetCOVIDDataAssurance(id, Convert.ToInt32(Session["LoginUserID"]));
            }
            catch(Exception ex)
            {
                dBEngine.LogException(ex.Message, "HomeController", "EditCOVIDTestGet", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
            }
            finally { dBEngine = null; }
            return View(dataassurance);
        }

        public ActionResult COVIDTestResults(int? id, string patientID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            COVIDTestResultsComplexDTO patient = new COVIDTestResultsComplexDTO();
            List<RolePermission> permissions = new List<Common.RolePermission>();
            try
            {
                ViewBag.TestResults = dBEngine.GetTestResults(Convert.ToInt32(Session["LoginUserID"]));
                patient = dBEngine.GetCOVIDTestResults(id, Convert.ToInt32(Session["LoginUserID"]));
                permissions = dBEngine.GetRolePermission(0, 0, "", "", "", Convert.ToInt32(Session["LoginUserID"]), "COVID Test Results", Convert.ToString(Session["Role"]));
                bool isnoaccess = false;
                if (permissions.Count > 0)
                {
                    ViewBag.IsReadOnly = permissions[0].IsReadOnly;
                    isnoaccess = permissions[0].NoAccess;
                }
                else
                {
                    ViewBag.IsReadOnly = true;
                    isnoaccess = true;
                }
                if (isnoaccess == true)
                {
                    return RedirectToAction("NotAuthorizedCOVID", new { id = 0, patientID = patientID });
                }
            }
            catch(Exception ex)
            {
                dBEngine.LogException(ex.Message, "HomeController", "COVIDTestResultsGet", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
            }
            finally
            {
                permissions = null;
                dBEngine = null;
            }
            return View(patient);
        }

        public ActionResult COVIDBreathing(int id, string patientID, bool IsPatientList = false)
        {
            Session["IsPatientList"] = IsPatientList;
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            COVIDBreathingSupport patient = new COVIDBreathingSupport();
            List<RolePermission> permissions = new List<Common.RolePermission>();
            try
            {
                ViewBag.TestResults = dBEngine.GetTestResults(Convert.ToInt32(Session["LoginUserID"]));
                patient = dBEngine.GetCOVIDBreathingSupport(patientID, Convert.ToInt32(Session["LoginUserID"]));
                permissions = dBEngine.GetRolePermission(0, 0, "", "", "", Convert.ToInt32(Session["LoginUserID"]), "COVID Breathing Support", Convert.ToString(Session["Role"]));
                bool isnoaccess = false;
                patient.Test_ID = id;

                if (permissions.Count > 0)
                {
                    ViewBag.IsReadOnly = permissions[0].IsReadOnly;
                    isnoaccess = permissions[0].NoAccess;
                }
                else
                {
                    ViewBag.IsReadOnly = true;
                    isnoaccess = true;
                }
                if (isnoaccess == true)
                {
                    return RedirectToAction("NotAuthorizedCOVID", new { id = id, patientID = patientID });
                }
            }
            catch(Exception ex)
            {
                dBEngine.LogException(ex.Message, "HomeController", "COVIDBreathingGet", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
            }
            finally
            {
                dBEngine = null;
                permissions = null;
            }
            return View(patient);
        }

        public ActionResult COVIDDataAssurance(int? id, string patientID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            COVIDTestResultsComplexDTO patient = new COVIDTestResultsComplexDTO();
            List<RolePermission> permissions = new List<Common.RolePermission>();
            try
            {
                ViewBag.TestResults = dBEngine.GetTestResults(Convert.ToInt32(Session["LoginUserID"]));
                patient = dBEngine.GetCOVIDTestResults(id, Convert.ToInt32(Session["LoginUserID"]));
                if (patient.LstTestHistory.Count > 0)
                {
                    ViewBag.PatientID = patient.LstTestHistory[0].PatientID;
                    ViewBag.Patient_ID = patient.LstTestHistory[0].Test_ID;
                    ViewBag.PatientName = patient.LstTestHistory[0].PatientName;
                    ViewBag.TestResult = patient.LstTestHistory[0].TestResult;
                    ViewBag.Age = patient.LstTestHistory[0].Age;
                    ViewBag.Gender = patient.LstTestHistory[0].Gender;
                }
                permissions = dBEngine.GetRolePermission(0, 0, "", "", "", Convert.ToInt32(Session["LoginUserID"]), "COVID Data Assurance", Convert.ToString(Session["Role"]));
                bool isnoaccess = false;
                if (permissions.Count > 0)
                {
                    ViewBag.IsReadOnly = permissions[0].IsReadOnly;
                    isnoaccess = permissions[0].NoAccess;
                }
                else
                {
                    ViewBag.IsReadOnly = true;
                    isnoaccess = true;
                }
                if (isnoaccess == true)
                {
                    return RedirectToAction("NotAuthorizedCOVID", new { id = id });
                }
            }
            catch(Exception ex)
            {
                dBEngine.LogException(ex.Message, "HomeController", "COVIDDataAssuranceGet", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
            }
            finally
            {
                dBEngine = null;
                permissions = null;
            }
            return View(patient);
        }

        public ActionResult COVIDExternalCommsReport()
        {
            Session["PatientListPageNo"] = null;
            Session["PatientListSearchText"] = null;
            Session["PatientListPageSize"] = null;
            Session["PatientListColumn"] = null;
            Session["PatientLocation"] = null;
            Session["IsCurrentIPCount"] = null;
            Session["IsNotUpdatedLast20HoursCount"] = null;
            Session["IsNotUpdatedLast12HoursCount"] = null;
            Session["IsUpdatedLast12HoursCount"] = null;
            Session["IsNewPositiveYTD"] = null;
            Session["IsNewPositiveYTDPending"] = null;
            Session["IsICUHDUStepUpYTD"] = null;
            Session["IsICUHDUStepUpYTDPending"] = null;
            Session["IsICUHDUStepDownYTD"] = null;
            Session["IsICUHDUStepDownYTDPending"] = null;
            Session["IsDischargeDeathYTD"] = null;
            Session["IsDischargeDeathYTDPending"] = null;
            Session["IsDischargesYTD"] = null;
            Session["IsDischargesYTDPending"] = null;
            Session["IsPositiveDeathsYTD"] = null;
            Session["IsPositiveDeathsYTDPending"] = null;
            Session["IsDeathYTD"] = null;
            Session["IsDeathYTDPending"] = null;
            Session["IsDeathDetected"] = null;
            Session["IsDeathDetectedPending"] = null;
            Session["IsDeathDiagnosed"] = null;
            Session["IsDeathDiagnosedPending"] = null;
            Session["IsDeathReAdmission"] = null;
            Session["IsDeathReAdmissionPending"] = null;
            Session["IsCHESSNewPositiveNotRequiredYTD"] = null;
            Session["IsCHESSICUHDUStepUpNotRequiredYTD"] = null;
            Session["IsCHESSICUHDUStepUpNotRequiredYTD"] = null;
            Session["IsCHESSDischargeDeathNotRequiredYTD"] = null;
            Session["IsCHESSDischargesNotRequiredYTD"] = null;
            Session["IsCHESSPositiveDeathsNotRequiredYTD"] = null;
            Session["IsCPNSDeathNotRequiredYTD"] = null;
            Session["IsCPNSDeathDetectedNotRequired"] = null;
            Session["IsCPNSDeathDiagnosedNotRequired"] = null;
            Session["IsCPNSDeathReAdmissionNotRequired"] = null;
            Session["IsExternalComms"] = true;
            Session["IsBreathing"] = false;
            Session["IsLevelOfCare"] = false;
            COVIDExternalCommsReport externalreport = new COVIDExternalCommsReport();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dbEngine = new DBEngine(connectionString);
            externalreport = dbEngine.GetCOVIDExternalCommsReport(Convert.ToInt32(Session["LoginUserID"]));
            return View(externalreport);
        }

        public ActionResult COVIDPatientListing(int pPageNo = 1, string searchfield = "", int pageSize = 0, string SortColumn = "", bool frompager = false, bool IsExternalComms = false, bool IsBreathing = false, bool IsLevelOfCare = false, string PatientLocation = "", bool IsCurrentIPCount = false, bool IsNotUpdatedLast20HoursCount = false,
           bool IsNotUpdatedLast12HoursCount = false, bool IsUpdatedLast12HoursCount = false,
           bool NoOxygen = false, bool Oxygen = false, bool NonInvasiveVentilation = false,
           bool MechanicalVentilation = false, bool NotUpdatedAtAll = false, bool IsNewPositiveYTD = false, bool IsNewPositiveYTDPending = false, bool IsICUHDUStepUpYTD = false, bool IsICUHDUStepUpYTDPending = false, bool IsICUHDUStepDownYTD = false, bool IsICUHDUStepDownYTDPending = false, bool IsDischargeDeathYTD = false, bool IsDischargeDeathYTDPending = false, bool IsDischargesYTD = false, bool IsDischargesYTDPending = false, bool IsPositiveDeathsYTD = false, bool IsPositiveDeathsYTDPending = false, bool IsDeathYTD = false, bool IsDeathYTDPending = false, bool IsDeathDetected = false, bool IsDeathDetectedPending = false, bool IsDeathDiagnosed = false, bool IsDeathDiagnosedPending = false,bool IsDeathReAdmission = false, bool IsDeathReAdmissionPending = false, bool IsCHESSNewPositiveNotRequiredYTD = false, bool IsCHESSICUHDUStepUpNotRequiredYTD = false, bool IsCHESSICUHDUStepDownNotRequiredYTD = false, bool IsCHESSDischargeDeathNotRequiredYTD = false, bool IsCHESSDischargesNotRequiredYTD = false, bool IsCHESSPositiveDeathsNotRequiredYTD = false, bool IsCPNSDeathNotRequiredYTD = false, bool IsCPNSDeathDetectedNotRequired = false, bool IsCPNSDeathDiagnosedNotRequired = false, bool IsCPNSDeathReAdmissionNotRequired = false, bool fromfilter = false)
        {
            if (IsExternalComms)
            {
                Session["IsExternalComms"] = true;
                Session["IsBreathing"] = false;
                Session["IsLevelOfCare"] = false;
            }
            else if (IsBreathing)
            {
                Session["IsExternalComms"] = false;
                Session["IsBreathing"] = true;
                Session["IsLevelOfCare"] = false;
            }
            else if (IsLevelOfCare)
            {
                Session["IsExternalComms"] = false;
                Session["IsBreathing"] = false;
                Session["IsLevelOfCare"] = true;
            }
            if (Session["PatientListPageNo"] == null)
                Session["PatientListPageNo"] = 1;
            if(Session["PatientListSearchText"] == null)
                Session["PatientListSearchText"] = searchfield;
            if (Session["PatientListColumn"] == null)
                Session["PatientListColumn"] = "";
            if (Session["PatientListPageSize"] == null)
                Session["PatientListPageSize"] = 10;
            if (Session["PatientLocation"] == null)
             Session["PatientLocation"] = PatientLocation;
            if (Session["IsCurrentIPCount"] == null)
                Session["IsCurrentIPCount"] = IsCurrentIPCount;
            else if(Convert.ToBoolean(Session["IsCurrentIPCount"]) != IsCurrentIPCount)
                Session["IsCurrentIPCount"] = IsCurrentIPCount;
            if (Session["IsNotUpdatedLast20HoursCount"] == null)
                Session["IsNotUpdatedLast20HoursCount"] = IsNotUpdatedLast20HoursCount;
            else if (Convert.ToBoolean(Session["IsNotUpdatedLast20HoursCount"]) != IsNotUpdatedLast20HoursCount)
                Session["IsNotUpdatedLast20HoursCount"] = IsNotUpdatedLast20HoursCount;
            if (Session["IsNotUpdatedLast12HoursCount"] == null)
                Session["IsNotUpdatedLast12HoursCount"] = IsNotUpdatedLast12HoursCount;
            else if (Convert.ToBoolean(Session["IsNotUpdatedLast12HoursCount"]) != IsNotUpdatedLast12HoursCount)
                Session["IsNotUpdatedLast12HoursCount"] = IsNotUpdatedLast12HoursCount;
            if (Session["IsUpdatedLast12HoursCount"] == null)
                Session["IsUpdatedLast12HoursCount"] = IsUpdatedLast12HoursCount;
            else if (Convert.ToBoolean(Session["IsUpdatedLast12HoursCount"]) != IsUpdatedLast12HoursCount)
                Session["IsUpdatedLast12HoursCount"] = IsUpdatedLast12HoursCount;
            if (Session["NoOxygen"] == null)
                Session["NoOxygen"] = NoOxygen;
            else if (Convert.ToBoolean(Session["NoOxygen"]) != NoOxygen)
                Session["NoOxygen"] = NoOxygen;
            if (Session["Oxygen"] == null)
                Session["Oxygen"] = Oxygen;
            else if (Convert.ToBoolean(Session["Oxygen"]) != Oxygen)
                Session["Oxygen"] = Oxygen;
            if (Session["NonInvasiveVentilation"] == null)
                Session["NonInvasiveVentilation"] = NonInvasiveVentilation;
            else if (Convert.ToBoolean(Session["NonInvasiveVentilation"]) != NonInvasiveVentilation)
                Session["NonInvasiveVentilation"] = NonInvasiveVentilation;
            if (Session["MechanicalVentilation"] == null || Convert.ToBoolean(Session["MechanicalVentilation"]) != MechanicalVentilation)
                Session["MechanicalVentilation"] = MechanicalVentilation;
            else if (Convert.ToBoolean(Session["MechanicalVentilation"]) != MechanicalVentilation)
                Session["MechanicalVentilation"] = MechanicalVentilation;
            if (Session["NotUpdatedAtAll"] == null || Convert.ToBoolean(Session["NotUpdatedAtAll"]) != NotUpdatedAtAll)
                Session["NotUpdatedAtAll"] = NotUpdatedAtAll;
            else if (Convert.ToBoolean(Session["NotUpdatedAtAll"]) != NotUpdatedAtAll)
                Session["NotUpdatedAtAll"] = NotUpdatedAtAll;
            if (Session["IsNewPositiveYTD"] == null)
                Session["IsNewPositiveYTD"] = IsNewPositiveYTD;
            else if (Convert.ToBoolean(Session["IsNewPositiveYTD"]) != IsNewPositiveYTD)
                Session["IsNewPositiveYTD"] = IsNewPositiveYTD;
            if (Session["IsNewPositiveYTDPending"] == null)
                Session["IsNewPositiveYTDPending"] = IsNewPositiveYTDPending;
            else if (Convert.ToBoolean(Session["IsNewPositiveYTDPending"]) != IsNewPositiveYTDPending)
                Session["IsNewPositiveYTDPending"] = IsNewPositiveYTDPending;
            if (Session["IsICUHDUStepUpYTD"] == null)
                Session["IsICUHDUStepUpYTD"] = IsICUHDUStepUpYTD;
            else if (Convert.ToBoolean(Session["IsICUHDUStepUpYTD"]) != IsICUHDUStepUpYTD)
                Session["IsICUHDUStepUpYTD"] = IsICUHDUStepUpYTD;
            if (Session["IsICUHDUStepUpYTDPending"] == null)
                Session["IsICUHDUStepUpYTDPending"] = IsICUHDUStepUpYTDPending;
            else if (Convert.ToBoolean(Session["IsICUHDUStepUpYTDPending"]) != IsICUHDUStepUpYTDPending)
                Session["IsICUHDUStepUpYTDPending"] = IsICUHDUStepUpYTDPending;
            if (Session["IsICUHDUStepDownYTD"] == null)
                Session["IsICUHDUStepDownYTD"] = IsICUHDUStepDownYTD;
            else if (Convert.ToBoolean(Session["IsICUHDUStepDownYTD"]) != IsICUHDUStepDownYTD)
                Session["IsICUHDUStepDownYTD"] = IsICUHDUStepDownYTD;
            if (Session["IsICUHDUStepDownYTDPending"] == null)
                Session["IsICUHDUStepDownYTDPending"] = IsICUHDUStepDownYTDPending;
            else if (Convert.ToBoolean(Session["IsICUHDUStepDownYTDPending"]) != IsICUHDUStepDownYTDPending)
                Session["IsICUHDUStepDownYTDPending"] = IsICUHDUStepDownYTDPending;
            if (Session["IsDischargeDeathYTD"] == null)
                Session["IsDischargeDeathYTD"] = IsDischargeDeathYTD;
            else if (Convert.ToBoolean(Session["IsDischargeDeathYTD"]) != IsDischargeDeathYTD)
                Session["IsDischargeDeathYTD"] = IsDischargeDeathYTD;
            if (Session["IsDischargeDeathYTDPending"] == null)
                Session["IsDischargeDeathYTDPending"] = IsDischargeDeathYTDPending;
            else if (Convert.ToBoolean(Session["IsDischargeDeathYTDPending"]) != IsDischargeDeathYTDPending)
                Session["IsDischargeDeathYTDPending"] = IsDischargeDeathYTDPending;
            if (Session["IsDischargesYTD"] == null)
                Session["IsDischargesYTD"] = IsDischargesYTD;
            else if (Convert.ToBoolean(Session["IsDischargesYTD"]) != IsDischargesYTD)
                Session["IsDischargesYTD"] = IsDischargesYTD;
            if (Session["IsDischargesYTDPending"] == null)
                Session["IsDischargesYTDPending"] = IsDischargesYTDPending;
            else if (Convert.ToBoolean(Session["IsDischargesYTDPending"]) != IsDischargesYTDPending)
                Session["IsDischargesYTDPending"] = IsDischargesYTDPending;
            if (Session["IsPositiveDeathsYTD"] == null)
                Session["IsPositiveDeathsYTD"] = IsPositiveDeathsYTD;
            else if (Convert.ToBoolean(Session["IsPositiveDeathsYTD"]) != IsPositiveDeathsYTD)
                Session["IsPositiveDeathsYTD"] = IsPositiveDeathsYTD;
            if (Session["IsPositiveDeathsYTDPending"] == null)
                Session["IsPositiveDeathsYTDPending"] = IsPositiveDeathsYTDPending;
            else if (Convert.ToBoolean(Session["IsPositiveDeathsYTDPending"]) != IsPositiveDeathsYTDPending)
                Session["IsPositiveDeathsYTDPending"] = IsPositiveDeathsYTDPending;
            if (Session["IsDeathYTD"] == null)
                Session["IsDeathYTD"] = IsDeathYTD;
            else if (Convert.ToBoolean(Session["IsDeathYTD"]) != IsDeathYTD)
                Session["IsDeathYTD"] = IsDeathYTD;
            if (Session["IsDeathYTDPending"] == null)
                Session["IsDeathYTDPending"] = IsDeathYTDPending;
            else if (Convert.ToBoolean(Session["IsDeathYTDPending"]) != IsDeathYTDPending)
                Session["IsDeathYTDPending"] = IsDeathYTDPending;
            if (Session["IsDeathDetected"] == null)
                Session["IsDeathDetected"] = IsDeathDetected;
            else if (Convert.ToBoolean(Session["IsDeathDetected"]) != IsDeathDetected)
                Session["IsDeathDetected"] = IsDeathDetected;
            if (Session["IsDeathDetectedPending"] == null)
                Session["IsDeathDetectedPending"] = IsDeathDetectedPending;
            else if (Convert.ToBoolean(Session["IsDeathDetectedPending"]) != IsDeathDetectedPending)
                Session["IsDeathDetectedPending"] = IsDeathDetectedPending;
            if (Session["IsDeathDiagnosed"] == null)
                Session["IsDeathDiagnosed"] = IsDeathDiagnosed;
            else if (Convert.ToBoolean(Session["IsDeathDiagnosed"]) != IsDeathDiagnosed)
                Session["IsDeathDiagnosed"] = IsDeathDiagnosed;
            if (Session["IsDeathDiagnosedPending"] == null)
                Session["IsDeathDiagnosedPending"] = IsDeathDiagnosedPending;
            else if (Convert.ToBoolean(Session["IsDeathDiagnosedPending"]) != IsDeathDiagnosedPending)
                Session["IsDeathDiagnosedPending"] = IsDeathDiagnosedPending;
            if (Session["IsDeathReAdmission"] == null)
                Session["IsDeathReAdmission"] = IsDeathReAdmission;
            else if (Convert.ToBoolean(Session["IsDeathReAdmission"]) != IsDeathReAdmission)
                Session["IsDeathReAdmission"] = IsDeathReAdmission;
            if (Session["IsDeathReAdmissionPending"] == null)
                Session["IsDeathReAdmissionPending"] = IsDeathReAdmissionPending;
            else if (Convert.ToBoolean(Session["IsDeathReAdmissionPending"]) != IsDeathReAdmissionPending)
                Session["IsDeathReAdmissionPending"] = IsDeathReAdmissionPending;
            
            if (Session["IsCHESSNewPositiveNotRequiredYTD"] == null)
                Session["IsCHESSNewPositiveNotRequiredYTD"] = IsCHESSNewPositiveNotRequiredYTD;
            else if (Convert.ToBoolean(Session["IsCHESSNewPositiveNotRequiredYTD"]) != IsCHESSNewPositiveNotRequiredYTD)
                Session["IsCHESSNewPositiveNotRequiredYTD"] = IsCHESSNewPositiveNotRequiredYTD;
            if (Session["IsCHESSICUHDUStepUpNotRequiredYTD"] == null)
                Session["IsCHESSICUHDUStepUpNotRequiredYTD"] = IsCHESSICUHDUStepUpNotRequiredYTD;
            else if (Convert.ToBoolean(Session["IsCHESSICUHDUStepUpNotRequiredYTD"]) != IsCHESSICUHDUStepUpNotRequiredYTD)
                Session["IsCHESSICUHDUStepUpNotRequiredYTD"] = IsCHESSICUHDUStepUpNotRequiredYTD;
            if (Session["IsCHESSICUHDUStepDownNotRequiredYTD"] == null)
                Session["IsCHESSICUHDUStepDownNotRequiredYTD"] = IsCHESSICUHDUStepDownNotRequiredYTD;
            else if (Convert.ToBoolean(Session["IsCHESSICUHDUStepDownNotRequiredYTD"]) != IsCHESSICUHDUStepDownNotRequiredYTD)
                Session["IsCHESSICUHDUStepDownNotRequiredYTD"] = IsCHESSICUHDUStepDownNotRequiredYTD;
            if (Session["IsCHESSICUHDUStepUpNotRequiredYTD"] == null)
                Session["IsCHESSICUHDUStepUpNotRequiredYTD"] = IsCHESSICUHDUStepUpNotRequiredYTD;
            else if (Convert.ToBoolean(Session["IsCHESSICUHDUStepUpNotRequiredYTD"]) != IsCHESSICUHDUStepUpNotRequiredYTD)
                Session["IsCHESSICUHDUStepUpNotRequiredYTD"] = IsCHESSICUHDUStepUpNotRequiredYTD;
            if (Session["IsCHESSDischargeDeathNotRequiredYTD"] == null)
                Session["IsCHESSDischargeDeathNotRequiredYTD"] = IsCHESSDischargeDeathNotRequiredYTD;
            else if (Convert.ToBoolean(Session["IsCHESSDischargeDeathNotRequiredYTD"]) != IsCHESSDischargeDeathNotRequiredYTD)
                Session["IsCHESSDischargeDeathNotRequiredYTD"] = IsCHESSDischargeDeathNotRequiredYTD;
            if (Session["IsCHESSDischargesNotRequiredYTD"] == null)
                Session["IsCHESSDischargesNotRequiredYTD"] = IsCHESSDischargesNotRequiredYTD;
            else if (Convert.ToBoolean(Session["IsCHESSDischargesNotRequiredYTD"]) != IsCHESSDischargesNotRequiredYTD)
                Session["IsCHESSDischargesNotRequiredYTD"] = IsCHESSDischargesNotRequiredYTD;
            if (Session["IsCHESSPositiveDeathsNotRequiredYTD"] == null)
                Session["IsCHESSPositiveDeathsNotRequiredYTD"] = IsCHESSPositiveDeathsNotRequiredYTD;
            else if (Convert.ToBoolean(Session["IsCHESSPositiveDeathsNotRequiredYTD"]) != IsCHESSPositiveDeathsNotRequiredYTD)
                Session["IsCHESSPositiveDeathsNotRequiredYTD"] = IsCHESSPositiveDeathsNotRequiredYTD;
            if (Session["IsCPNSDeathDetectedNotRequired"] == null)
                Session["IsCPNSDeathDetectedNotRequired"] = IsCPNSDeathDetectedNotRequired;
            else if (Convert.ToBoolean(Session["IsCPNSDeathDetectedNotRequired"]) != IsCPNSDeathDetectedNotRequired)
                Session["IsCPNSDeathDetectedNotRequired"] = IsCPNSDeathDetectedNotRequired;
            if (Session["IsCPNSDeathNotRequiredYTD"] == null)
                Session["IsCPNSDeathNotRequiredYTD"] = IsCPNSDeathNotRequiredYTD;
            else if (Convert.ToBoolean(Session["IsCPNSDeathNotRequiredYTD"]) != IsCPNSDeathNotRequiredYTD)
                Session["IsCPNSDeathNotRequiredYTD"] = IsCPNSDeathNotRequiredYTD;
            if (Session["IsCPNSDeathDiagnosedNotRequired"] == null)
                Session["IsCPNSDeathDiagnosedNotRequired"] = IsCPNSDeathDiagnosedNotRequired;
            else if (Convert.ToBoolean(Session["IsCPNSDeathDiagnosedNotRequired"]) != IsCPNSDeathDiagnosedNotRequired)
                Session["IsCPNSDeathDiagnosedNotRequired"] = IsCPNSDeathDiagnosedNotRequired;
            if (Session["IsCPNSDeathReAdmissionNotRequired"] == null)
                Session["IsCPNSDeathReAdmissionNotRequired"] = IsCPNSDeathReAdmissionNotRequired;
            else if (Convert.ToBoolean(Session["IsCPNSDeathReAdmissionNotRequired"]) != IsCPNSDeathReAdmissionNotRequired)
                Session["IsCPNSDeathReAdmissionNotRequired"] = IsCPNSDeathReAdmissionNotRequired;
            Session["PatientListPageSize"] = 10;
            if (Session["PatientListSortType"] == null)
                Session["PatientListSortType"] = "DESC";
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dbEngine = new DBEngine(connectionString);
            List<clsCOVIDPatientList> coviddetails = dbEngine.GetCOVIDPatientList(Convert.ToInt32(Session["PatientListPageNo"]), Convert.ToInt32(Session["PatientListPageSize"]), Convert.ToString(Session["PatientListSearchText"]),
                Convert.ToString(Session["PatientLocation"]), Convert.ToBoolean(Session["IsCurrentIPCount"]), Convert.ToBoolean(Session["IsNotUpdatedLast20HoursCount"]),
                Convert.ToBoolean(Session["IsNotUpdatedLast12HoursCount"]), Convert.ToBoolean(Session["IsUpdatedLast12HoursCount"]), Convert.ToBoolean(Session["NoOxygen"]), Convert.ToBoolean(Session["Oxygen"]), Convert.ToBoolean(Session["NonInvasiveVentilation"]), Convert.ToBoolean(Session["MechanicalVentilation"]), Convert.ToBoolean(Session["NotUpdatedAtAll"]),
                Convert.ToBoolean(Session["IsNewPositiveYTD"]), Convert.ToBoolean(Session["IsNewPositiveYTDPending"]), Convert.ToBoolean(Session["IsICUHDUStepUpYTD"]), Convert.ToBoolean(Session["IsICUHDUStepUpYTDPending"]), Convert.ToBoolean(Session["IsICUHDUStepDownYTD"]),
                Convert.ToBoolean(Session["IsICUHDUStepDownYTDPending"]), Convert.ToBoolean(Session["IsDischargeDeathYTD"]), Convert.ToBoolean(Session["IsDischargeDeathYTDPending"]), Convert.ToBoolean(Session["IsDischargesYTD"]), Convert.ToBoolean(Session["IsDischargesYTDPending"]), Convert.ToBoolean(Session["IsPositiveDeathsYTD"]), Convert.ToBoolean(Session["IsPositiveDeathsYTDPending"]), Convert.ToBoolean(Session["IsDeathYTD"]), Convert.ToBoolean(Session["IsDeathYTDPending"]), Convert.ToBoolean(Session["IsDeathDetected"]), Convert.ToBoolean(Session["IsDeathDetectedPending"]), Convert.ToBoolean(Session["IsDeathDiagnosed"]), Convert.ToBoolean(Session["IsDeathDiagnosedPending"]), Convert.ToBoolean(Session["IsDeathReAdmission"]), Convert.ToBoolean(Session["IsDeathReAdmissionPending"]), Convert.ToBoolean(Session["IsCHESSNewPositiveNotRequiredYTD"]), Convert.ToBoolean(Session["IsCHESSICUHDUStepUpNotRequiredYTD"]), Convert.ToBoolean(Session["IsCHESSICUHDUStepDownNotRequiredYTD"]), Convert.ToBoolean(Session["IsCHESSDischargeDeathNotRequiredYTD"]), Convert.ToBoolean(Session["IsCHESSDischargesNotRequiredYTD"]), Convert.ToBoolean(Session["IsCHESSPositiveDeathsNotRequiredYTD"]), Convert.ToBoolean(Session["IsCPNSDeathNotRequiredYTD"]), Convert.ToBoolean(Session["IsCPNSDeathDetectedNotRequired"]), Convert.ToBoolean(Session["IsCPNSDeathDiagnosedNotRequired"]), Convert.ToBoolean(Session["IsCPNSDeathReAdmissionNotRequired"]),
                 Convert.ToString(Session["PatientListColumn"]), Convert.ToString(Session["PatientListSortType"]), Convert.ToString(Session["BreathingReportTestResults"]), Convert.ToInt32(Session["LoginUserID"]));
            
            int totalRecords = 0;
            if (coviddetails.Count > 0)
            {
                totalRecords = coviddetails[0].TotalRecords;
            }

            int totalPagesCount = 0;
            totalPagesCount = (int)Math.Ceiling((float)totalRecords / Convert.ToInt32(Session["PatientListPageSize"]));

            ViewBag.PageNumber = Convert.ToInt32(Session["PatientListPageNo"]);
            ViewBag.SearchText = Convert.ToString(Session["PatientListSearchText"]);
            ViewBag.TotalPagesCount = totalPagesCount;
            ViewBag.TotalRecordCount = totalRecords;
            ViewBag.ModalRecordCount = coviddetails.Count;
            ViewBag.PageSize = Convert.ToInt32(Session["PatientListPageSize"]);

            return View(coviddetails);
        }

        public ActionResult COVIDLevelOfCare(int id, string patientID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            COVIDLevelOfCare patient = new Common.DTO.COVIDLevelOfCare();
            List<RolePermission> permissions = new List<Common.RolePermission>();
            try
            {
                patient = dBEngine.GetCOVIDLevelOfCare(patientID, Convert.ToInt32(Session["LoginUserID"]));
                patient.Test_ID = id;
                permissions = dBEngine.GetRolePermission(0, 0, "", "", "", Convert.ToInt32(Session["LoginUserID"]), "COVID Level Of Care", Convert.ToString(Session["Role"]));
                bool isnoaccess = false;
                if (permissions.Count > 0)
                {
                    ViewBag.IsReadOnly = permissions[0].IsReadOnly;
                    isnoaccess = permissions[0].NoAccess;
                }
                else
                {
                    ViewBag.IsReadOnly = true;
                    isnoaccess = true;
                }
                if (isnoaccess == true)
                {
                    return RedirectToAction("NotAuthorizedCOVID", new { id = id, patientID = patientID });
                }
            }
            catch(Exception ex)
            {
                dBEngine.LogException(ex.Message, "HomeController", "COVIDLevelOfCareGet", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
            }
            finally
            {
                dBEngine = null;
                permissions = null;
            }
            return View(patient);
        }

        public ActionResult COVIDComms(int id, string patientID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            COVIDComms patient = new Common.DTO.COVIDComms();
            List<RolePermission> permissions = new List<Common.RolePermission>();
            try
            {
                bool isnoaccess = false;
                patient = dBEngine.GetCOVIDComms(id, Convert.ToInt32(Session["LoginUserID"]));
                permissions = dBEngine.GetRolePermission(0, 0, "", "", "", Convert.ToInt32(Session["LoginUserID"]), "COVID Comms", Convert.ToString(Session["Role"]));
                if (permissions.Count > 0)
                {
                    ViewBag.IsReadOnly = permissions[0].IsReadOnly;
                    isnoaccess = permissions[0].NoAccess;
                }
                else
                {
                    ViewBag.IsReadOnly = true;
                    isnoaccess = true;
                }
                if (isnoaccess == true)
                {
                    return RedirectToAction("NotAuthorizedCOVID", new { id = id, patientID = patientID });
                }
            }
            catch(Exception ex)
            {
                dBEngine.LogException(ex.Message, "HomeController", "COVIDCommsGet", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
            }
            finally { dBEngine = null; permissions = null; }
            return View(patient);
        }

        public ActionResult COVIDExternalComms(int? id, string patientID, bool IsPatientList = false)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            COVIDExternalCommsComplexDTO patient = new COVIDExternalCommsComplexDTO();
            List<RolePermission> permissions = new List<Common.RolePermission>();            
            bool isnoaccess = false;
            Session["IsPatientList"] = IsPatientList;
            try
            {
                patient = dBEngine.GetCOVIDExternalComms(patientID, Convert.ToInt32(Session["LoginUserID"]));
                permissions = dBEngine.GetRolePermission(0, 0, "", "", "", Convert.ToInt32(Session["LoginUserID"]), "COVID Comms", Convert.ToString(Session["Role"]));
                if (permissions.Count > 0)
                {
                    ViewBag.IsReadOnly = permissions[0].IsReadOnly;
                    isnoaccess = permissions[0].NoAccess;
                }
                else
                {
                    ViewBag.IsReadOnly = true;
                    isnoaccess = true;
                }
                if (isnoaccess == true)
                {
                    return RedirectToAction("NotAuthorizedCOVID", new { id = id });
                }
            }
            catch(Exception ex)
            {
                dBEngine.LogException(ex.Message, "HomeController", "COVIDExternalCommsGet", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
            }
            finally
            { dBEngine = null;  permissions = null; }
            if (patient != null)
            {
                if (patient.CovidChessComms.TestResult == "DETECTED" || patient.CovidChessComms.TestResult == "DIAGNOSED")
                    ViewBag.CHESSReadOnly = false;
                else
                    ViewBag.CHESSReadOnly = true;
                if (patient.CovidCpnsComms.ME_COVID_DEATH == 1)
                    ViewBag.CPNSReadOnly = false;
                else
                    ViewBag.CPNSReadOnly = true;
            }
            return View(patient);
        }

        [HttpPost]
        public ActionResult COVIDComms(FormCollection formCollection, int id, string patientID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            bool isnone = false;
            bool ispositiveward = false;
            bool ispositivepatient = false;
            bool isnegativecomm = false;
            try
            {
                patientID = formCollection["hdnPatientID"];
                if (Convert.ToString(formCollection["IsNone"]) == "on") isnone = true;
                if (Convert.ToString(formCollection["IsPositiveWardContacted"]) == "on") ispositiveward = true;
                if (Convert.ToString(formCollection["IsPositivePatientContacted"]) == "on") ispositivepatient = true;
                if (Convert.ToString(formCollection["IsNegativeLetterSent"]) == "on") isnegativecomm = true;
                int dbReturnValue = dBEngine.UpdateCOVIDComms(id, isnone, ispositiveward, ispositivepatient, isnegativecomm, formCollection["Comments"], Convert.ToInt32(Session["LoginUserID"]));
            }
            catch(Exception ex)
            {
                dBEngine.LogException(ex.Message, "HomeController", "COVIDCommsPost", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
            }
            finally { dBEngine = null; }
            return RedirectToAction("COVIDReviewCycle", new { id = id, patientID = patientID });
        }

        [HttpPost]
        public ActionResult COVIDCHESSComms(FormCollection formCollection, int? id, string patientID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            bool ischesscontacted = false;
            try
            {
                id = Convert.ToInt32(formCollection["hdnTestID"]);
                patientID = formCollection["hdnPatientID"];
                if (Convert.ToString(formCollection["CHESSContacted"]) == "Yes") ischesscontacted = true;
                int dbReturnValue = dBEngine.UpdateCOVIDCHESSComms(patientID, ischesscontacted, formCollection["CovidChessComms.CHESSComments"], Convert.ToInt32(Session["LoginUserID"]));
            }
            catch(Exception ex)
            {
                dBEngine.LogException(ex.Message, "HomeController", "COVIDChessCommsPost", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
            }
            finally { dBEngine = null; }
            return RedirectToAction("COVIDExternalComms", new { id = id, patientID = patientID, IsPatientList = Convert.ToBoolean(Session["IsPatientList"]) });
        }

        [HttpPost]
        public ActionResult COVIDLOCComms(FormCollection formCollection, int? id, string patientID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            bool ischesscontacted = false;
            try
            {
                id = Convert.ToInt32(formCollection["hdnTestID1"]);
                patientID = formCollection["hdnPatientID1"];
                if (Convert.ToString(formCollection["LevelOfCareUpdated"]) == "Yes") ischesscontacted = true;
                int dbReturnValue = dBEngine.UpdateCOVIDLOCComms(patientID, ischesscontacted, formCollection["CovidChessLOCComms.LOCComments"], Convert.ToInt32(Session["LoginUserID"]));
            }
            catch(Exception ex)
            {
                dBEngine.LogException(ex.Message, "HomeController", "CovidLocCommsPost", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
            }
            finally { dBEngine = null; }
            return RedirectToAction("COVIDExternalComms", new { id = id, patientID = patientID, IsPatientList = Convert.ToBoolean(Session["IsPatientList"]) });
        }

        [HttpPost]
        public ActionResult COVIDDischargeDeathComms(FormCollection formCollection, int? id, string patientID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            bool ischesscontacted = false;
            try
            {
                id = Convert.ToInt32(formCollection["hdnTestID2"]);
                patientID = formCollection["hdnPatientID2"];
                if (Convert.ToString(formCollection["DichargeDeathContacted"]) == "Yes") ischesscontacted = true;
                int dbReturnValue = dBEngine.UpdateCOVIDDischargeDeathComms(patientID, ischesscontacted, formCollection["CovidDischargeDeathComms.DischargeDeathComments"], Convert.ToInt32(Session["LoginUserID"]));
            }
            catch(Exception ex)
            { dBEngine.LogException(ex.Message, "HomeController", "COVIDDischargeDeathCommsPost", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"])); }
            finally { dBEngine = null; }
            return RedirectToAction("COVIDExternalComms", new { id = id, patientID = patientID, IsPatientList = Convert.ToBoolean(Session["IsPatientList"]) });
        }

        [HttpPost]
        public ActionResult COVIDCPNSComms(FormCollection formCollection, int? id, string patientID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            bool ischesscontacted = false;
            try
            {
                id = Convert.ToInt32(formCollection["hdnTestID3"]);
                patientID = formCollection["hdnPatientID3"];
                if (Convert.ToString(formCollection["CPNSContacted"]) == "Yes") ischesscontacted = true;
                int dbReturnValue = dBEngine.UpdateCOVIDCPNSComms(patientID, ischesscontacted, formCollection["CovidCpnsComms.CPNSComments"], Convert.ToInt32(Session["LoginUserID"]));
            }
            catch(Exception ex)
            {
                dBEngine.LogException(ex.Message, "HomeController", "COVIDCpnsCommsPost", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
            }
            finally { dBEngine = null; }
            return RedirectToAction("COVIDExternalComms", new { id = id, patientID = patientID, IsPatientList = Convert.ToBoolean(Session["IsPatientList"])});
        }

        [HttpPost]
        public ActionResult COVIDLevelOfCare(FormCollection formCollection, int id, string patientid)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            try
            {
                patientid = formCollection["hdnPatientID"];
                bool ituLevel1 = false;
                bool ituLevel2 = false;
                bool ituLevel3 = false;
                bool infectionDiseaseUnitBed = false;
                bool otherbed = false;
                if (Convert.ToString(formCollection["ITULevel1"]) == "on") ituLevel1 = true;
                if (Convert.ToString(formCollection["ITULevel2"]) == "on") ituLevel2 = true;
                if (Convert.ToString(formCollection["ITULevel3"]) == "on") ituLevel3 = true;
                if (Convert.ToString(formCollection["InfectionDiseaseUnitBed"]) == "on") infectionDiseaseUnitBed = true;
                if (Convert.ToString(formCollection["OtherBeds"]) == "on") otherbed = true;
                int dbReturnValue = dBEngine.UpdateCOVIDLevelOfCare(patientid, ituLevel1, ituLevel2, ituLevel3, infectionDiseaseUnitBed, otherbed, Convert.ToInt32(Session["LoginUserID"]));
            }
            catch(Exception ex)
            {
                dBEngine.LogException(ex.Message, "HomeController", "COVIDLevelOfCarePost", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
            }
            finally
            {
                dBEngine = null;
            }
            return RedirectToAction("COVIDReviewCycle", new { id = id, patientid = patientid });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult PositiveFeedback(int? id)
        {
            if (Session["FullName"] != null)
            {
                if (string.IsNullOrEmpty(Session["FullName"].ToString()))
                    return RedirectToAction("Index", "Account");
            }
            else
                return RedirectToAction("Index", "Account");
            bool isUser = GetUserDetailsFromAD();
            clsFeedBackModel feedback = new clsFeedBackModel();
            List<clsFeedBackModel> lstFBM = new List<clsFeedBackModel>();
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            try
            {
                if (id == null || id == 0)
                {
                    if (Session["PatientID"] != null)
                        id = Convert.ToInt32(Session["PatientID"]);
                    else
                        return RedirectToAction("Index", "Account");
                }
                try
                {
                    Session["PatientID"] = id;
                    ViewBag.CommentType = dBEngine.GetCommentType("Feedback", Convert.ToInt32(Session["LoginUserID"]));
                    ViewBag.Comments = dBEngine.GetComments(id, "Feedback", Convert.ToInt32(Session["LoginUserID"]));
                    feedback = dBEngine.GetFeedback(id, Convert.ToInt32(Session["LoginUserID"]));
                    clsDeclarationModel declaration = new clsDeclarationModel();
                    declaration = dBEngine.GetDeclaration(id, Convert.ToInt32(Session["LoginUserID"]));
                    if (declaration.Declaration)
                        feedback.CreatedBy = declaration.CreatedBy;
                    if (feedback.CreatedBy == null) feedback.CreatedBy = "";
                    patientDetails = dBEngine.GetPatientDetails(id, Convert.ToInt32(Session["LoginUserID"]));

                    feedback.PatientID = patientDetails.ToList()[0].PatientId;
                    feedback.PatientName = patientDetails.ToList()[0].PatientName;
                    feedback.DOB = patientDetails.ToList()[0].DOB;
                    feedback.MedTriage = patientDetails.ToList()[0].MedTriage;
                    CultureInfo enUS = new CultureInfo("en-US");
                    CultureInfo provider = CultureInfo.InvariantCulture;
                    DateTime dateValue;

                    foreach (var b in ViewBag.CommentType)
                    {
                        foreach (var a in ViewBag.Comments)
                        {
                            if (a.CommentTypeID == b.CommonTypeID)
                            {
                                a.CommentType = b.Type;
                            }
                        }
                    }

                    TempData["Role"] = Session["Role"];
                    if (feedback.Patient_ID == 0 || feedback.Patient_ID == null)
                        feedback.Patient_ID = Convert.ToInt32(id);
                }
                catch (Exception ex)
                {
                    dBEngine.LogException(ex.Message, this.ToString(), "ValidateUser", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            List<RolePermission> permissions = dBEngine.GetRolePermission(0, 0, "", "", "", Convert.ToInt32(Session["LoginUserID"]), "MedTriage", Convert.ToString(Session["Role"]));
            bool isnoaccess = false;
            if (permissions.Count > 0)
            {
                ViewBag.IsReadOnly = permissions[0].IsReadOnly;
                isnoaccess = permissions[0].NoAccess;
            }
            else
            {
                ViewBag.IsReadOnly = true;
                isnoaccess = true;
            }
            if (isnoaccess == false)
            {
                return View(feedback);
            }
            else
            {
                return RedirectToAction("NotAuthorizedPatientDetails", new { id = id });
            }            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clsFeedBackModel"></param>
        /// <param name="BtnPrevious"></param>
        /// <param name="BtnFinish"></param>
        /// <param name="BtnNext"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PositiveFeedback(FormCollection formCollection, string BtnPrevious, string BtnSave, string BtnFinish, string BtnNext, int id)
        {
            string actionName = "";
            string CommentType = "";
            string comments = "";
            bool isFormCompleted = false;
            bool isComplementsFedBack = false;
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            int FBTypeID;
            try
            {
                if (id == null || id == 0)
                {
                    if (Session["PatientID"] != null)
                        id = Convert.ToInt32(Session["PatientID"]);
                    else
                        return RedirectToAction("Index", "Account");
                }
                if (BtnPrevious != null)
                    actionName = "OtherReferrals";
                if (BtnSave != null)
                    actionName = "MortalityReview";
                if (BtnFinish != null)
                {
                    actionName = "Declaration";
                    if (Convert.ToString(formCollection["FormCompleted"]) == "on") isFormCompleted = true;
                    if (Convert.ToString(formCollection["ComplementsFedBack"]) == "on") isComplementsFedBack = true;
                    if (formCollection["ddlFeedbackType"] != "")
                    {
                        FBTypeID = Convert.ToInt32(formCollection["ddlFeedbackType"]);
                    }
                    else
                    {
                        FBTypeID = 0;
                    }
                  
                    if (formCollection["IsComment"] != "")
                    {
                        bool IsComment = Convert.ToBoolean(formCollection["IsComment"]);
                        if (IsComment)
                        {
                           
                            CommentType = formCollection["ddlFeedbackType"].ToString();
                            comments = formCollection["cmnt"].ToString();
                            int result = SaveComment(CommentType, comments);
                        }
                    }

                    if (string.IsNullOrEmpty(formCollection["Comments"]))
                    {
                        formCollection["Comments"] = "";
                    }

                    int retVal = dBEngine.UpdatePositiveFeedback(isFormCompleted, isComplementsFedBack, formCollection["Comments"], FBTypeID, id, Convert.ToInt32(Session["LoginUserID"]));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return RedirectToAction(actionName, new { id = id });
        }

       
        public ActionResult MedicalExaminerDashboard(FormCollection formCollection, bool isReset = false)
        {
            bool showdisabled = false;
            Session["MedicalExaminerDashboard"] = true;
            Session["PatientExtract"] = false;
            Session["PatientReportList"] = false;
            Session["InsightReport"] = false;
            Session.Timeout = 30;
            Session["StartDate"] = null;
            Session["EndDate"] = null;
            if (string.IsNullOrEmpty(Convert.ToString(Session["PatientType"])))
                Session["PatientType"] = "0";
            if (string.IsNullOrEmpty(Convert.ToString(Session["Speciality"])))
                Session["Speciality"] = "0";
            if (string.IsNullOrEmpty(Convert.ToString(Session["WardDeath"])))
                Session["WardDeath"] = "0";
            if (string.IsNullOrEmpty(Convert.ToString(Session["DischargeConsultant"])))
                Session["DischargeConsultant"] = "0";
            Session["TotalRecords"] = null;
            Session["QAPCount"] = null;
            Session["MedCount"] = null;
            Session["MEOCount"] = null;
            Session["MortalityPageSize"] = 10;
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            if (Convert.ToInt32(Session["LoginUserID"]) > 0)
            {
                MortalityFilterDDM filterddm = dBEngine.GetMortalityFilterDDM(Convert.ToInt32(Session["LoginUserID"]));
                ViewBag.LoadDischargeSpecialityDropdown = filterddm.lstSpecialities;
                ViewBag.PatientTypeDDM = filterddm.lstPatientTypes;
                ViewBag.wardDeathDropdown = filterddm.lstWardOfDeaths;
                ViewBag.dischargeConsultantDropdown = filterddm.lstDischargeConsultants;
                if (Session["MortalityPageSize"] == null)
                    Session["MortalityPageSize"] = 10;
                if (Request.HttpMethod != "POST")
                {
                    COVIDDefaultDate defaultdate = dBEngine.GetMortalityDefaultDate(Convert.ToInt32(Session["LoginUserID"]));
                    if (isReset)
                    {
                        if (defaultdate.StartDate == null)
                            Session["StartDate"] = DateTime.Now.AddDays(-180).ToString("dd/MM/yyyy");
                        else
                            Session["StartDate"] = defaultdate.StartDate;
                        if (defaultdate.EndDate == null)
                            Session["EndDate"] = DateTime.Now.ToString("dd/MM/yyyy");
                        else
                            Session["EndDate"] = defaultdate.EndDate;
                        Session["PatientType"] = "0";
                        Session["Speciality"] = "0";
                        Session["WardDeath"] = "0";
                        Session["DischargeConsultant"] = "0";

                    }
                    if (defaultdate.StartDate == null)
                        Session["StartDate"] = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
                    else
                        Session["StartDate"] = defaultdate.StartDate;
                    if (defaultdate.EndDate == null)
                        Session["EndDate"] = DateTime.Now.ToString("dd/MM/yyyy");
                    else
                        Session["EndDate"] = defaultdate.EndDate;
                    if (Session["PatientType"] == null)
                        Session["PatientType"] = "0";
                    if (Session["Speciality"] == null)
                        Session["Speciality"] = "0";
                    if (Session["WardDeath"] == null)
                        Session["WardDeath"] = "0";
                    if (Session["DischargeConsultant"] == null)
                        Session["DischargeConsultant"] = "0";

                    Session["IsTotal"] = null;
                    Session["IsQAP"] = null;
                    Session["IsMedTriage"] = null;
                    Session["IsMEO"] = null;
                    Session["MortalityPageSize"] = 10;
                }
                else
                {
                    if (isReset == false)
                    {
                        Session["StartDate"] = formCollection["txtStartDate"].Replace("Order Start Date ", "");
                        Session["EndDate"] = formCollection["txtEndDate"].Replace("Order End Date ", "");
                        Session["PatientType"] = formCollection["ddlPatientType"];
                        Session["Speciality"] = formCollection["ddlDischargeSpeciality"];
                        Session["WardDeath"] = formCollection["ddlWardDeath"];
                        Session["DischargeConsultant"] = formCollection["ddlDischargeConsultant"];

                        Session["IsTotal"] = null;
                        Session["IsQAP"] = null;
                        Session["IsMedTriage"] = null;
                        Session["IsMEO"] = null;
                    }
                }
                List<clsPatientDetails> patientDetails = dBEngine.GetFilteredPatientDetails(1, Convert.ToInt32(Session["MortalityPageSize"].ToString()), DateTime.ParseExact(Session["StartDate"].ToString(), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), DateTime.ParseExact(Session["EndDate"].ToString(), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), Session["PatientType"].ToString(), Session["Speciality"].ToString(), Session["WardDeath"].ToString(), Session["DischargeConsultant"].ToString(), false, false, false, false, "", "", "", showdisabled, Convert.ToInt32(Session["LoginUserID"]));
                if (patientDetails.Count > 0)
                {
                    ViewBag.TotalRecords = patientDetails[0].TotalRecords.ToString();
                    Session["TotalRecords"] = ViewBag.TotalRecords;
                }
                else
                    ViewBag.TotalRecords = "0";
                if (patientDetails.Count > 0)
                {
                    ViewBag.QAPCount = patientDetails[0].QAPCount.ToString();
                    Session["QAPCount"] = ViewBag.QAPCount;
                }
                else
                    ViewBag.QAPCount = "0";

                if (patientDetails.Count > 0)
                {
                    ViewBag.MedCount = patientDetails[0].MedCount.ToString();
                    Session["MedCount"] = ViewBag.MedCount;
                }
                else
                    ViewBag.MedCount = "0";
                if (patientDetails.Count > 0)
                {
                    ViewBag.MEOCount = patientDetails[0].MEOCount.ToString();
                    Session["MEOCount"] = ViewBag.MEOCount;
                }
                else
                    ViewBag.MEOCount = "0";

                int totalRecords = 0;
                if (patientDetails.Count > 0)
                {
                    totalRecords = patientDetails[0].TotalRecords;
                }

                int totalPagesCount = 0;
                totalPagesCount = (int)Math.Ceiling((float)totalRecords / 10);

                ViewBag.PageNumber = 1;
                ViewBag.SearchText = "";
                ViewBag.TotalPagesCount = totalPagesCount;
                ViewBag.TotalRecordCount = totalRecords;
                ViewBag.ModalRecordCount = patientDetails.Count;
                ViewBag.PageSize = 10;
                List<RolePermission> permissions = dBEngine.GetRolePermission(0, 0, "", "", "",Convert.ToInt32(Session["LoginUserID"]), "Mortality Dashboard", Convert.ToString(Session["Role"]));
                bool isnoaccess = false;
                if (permissions.Count > 0)
                {
                    ViewBag.IsReadOnly = permissions[0].IsReadOnly;
                    isnoaccess = permissions[0].NoAccess;
                }
                else
                {
                    ViewBag.IsReadOnly = true;
                    isnoaccess = true;
                }
                if (isnoaccess == false)
                {
                    return View(patientDetails);
                }
                else
                {
                    return RedirectToAction("NotAuthorizedSJR", new { id = 0, isdashboard = true });
                }
            }
            else
                return RedirectToAction("Index", "Account");
        }
        public ActionResult PatientList(FormCollection formCollection, bool isReset)
        {
            bool showdisabled = true;
            Session["PatientList"] = true;
            Session.Timeout = 30;
            Session["PatientStartDate"] = null;
            Session["PatientEndDate"] = null;
            if (string.IsNullOrEmpty(Convert.ToString(Session["PatientType"])))
                Session["PatientType"] = "0";
            if (string.IsNullOrEmpty(Convert.ToString(Session["Speciality"])))
                Session["Speciality"] = "0";
            if (string.IsNullOrEmpty(Convert.ToString(Session["WardDeath"])))
                Session["WardDeath"] = "0";
            if (string.IsNullOrEmpty(Convert.ToString(Session["DischargeConsultant"])))
                Session["DischargeConsultant"] = "0";
            Session["TotalRecords"] = null;
            Session["QAPCount"] = null;
            Session["MedCount"] = null;
            Session["MEOCount"] = null;
            Session["CommonPageSize"] = 10;
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            if (Convert.ToInt32(Session["LoginUserID"]) > 0)
            {
                MortalityFilterDDM filterddm = dBEngine.GetMortalityFilterDDM(Convert.ToInt32(Session["LoginUserID"]));
                ViewBag.LoadDischargeSpecialityDropdown = filterddm.lstSpecialities;
                ViewBag.PatientTypeDDM = filterddm.lstPatientTypes;
                ViewBag.wardDeathDropdown = filterddm.lstWardOfDeaths;
                ViewBag.dischargeConsultantDropdown = filterddm.lstDischargeConsultants;
                if (Session["CommonPageSize"] == null)
                    Session["CommonPageSize"] = 10;
                if (Request.HttpMethod != "POST")
                {
                    COVIDDefaultDate defaultdate = dBEngine.GetMortalityDefaultDate(Convert.ToInt32(Session["LoginUserID"]));
                    if (isReset)
                    {
                        if (defaultdate.StartDate == null)
                            Session["PatientStartDate"] = DateTime.Now.AddDays(-180).ToString("dd/MM/yyyy");
                        else
                            Session["PatientStartDate"] = defaultdate.StartDate;
                        if (defaultdate.EndDate == null)
                            Session["PatientEndDate"] = DateTime.Now.ToString("dd/MM/yyyy");
                        else
                            Session["PatientEndDate"] = defaultdate.EndDate;
                        Session["PatientType"] = "0";
                        Session["Speciality"] = "0";
                        Session["WardDeath"] = "0";
                        Session["DischargeConsultant"] = "0";

                    }
                    if (defaultdate.StartDate == null)
                        Session["PatientStartDate"] = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
                    else
                        Session["PatientStartDate"] = defaultdate.StartDate;
                    if (defaultdate.EndDate == null)
                        Session["PatientEndDate"] = DateTime.Now.ToString("dd/MM/yyyy");
                    else
                        Session["PatientEndDate"] = defaultdate.EndDate;
                    if (Session["PatientType"] == null)
                        Session["PatientType"] = "0";
                    if (Session["Speciality"] == null)
                        Session["Speciality"] = "0";
                    if (Session["WardDeath"] == null)
                        Session["WardDeath"] = "0";
                    if (Session["DischargeConsultant"] == null)
                        Session["DischargeConsultant"] = "0";

                    Session["IsTotal"] = null;
                    Session["IsQAP"] = null;
                    Session["IsMedTriage"] = null;
                    Session["IsMEO"] = null;
                    Session["CommonPageSize"] = 10;
                }
                else
                {
                    if (isReset == false)
                    {
                        Session["PatientStartDate"] = formCollection["txtStartDate"].Replace("Order Start Date ", "");
                        Session["PatientEndDate"] = formCollection["txtEndDate"].Replace("Order End Date ", "");
                        Session["PatientType"] = formCollection["ddlPatientType"];
                        Session["Speciality"] = formCollection["ddlDischargeSpeciality"];
                        Session["WardDeath"] = formCollection["ddlWardDeath"];
                        Session["DischargeConsultant"] = formCollection["ddlDischargeConsultant"];

                        Session["IsTotal"] = null;
                        Session["IsQAP"] = null;
                        Session["IsMedTriage"] = null;
                        Session["IsMEO"] = null;
                    }
                }
                List<clsPatientDetails> patientDetails = dBEngine.GetFilteredPatientDetails(1, Convert.ToInt32(Session["CommonPageSize"].ToString()), DateTime.ParseExact(Session["PatientStartDate"].ToString(), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), DateTime.ParseExact(Session["PatientEndDate"].ToString(), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), Session["PatientType"].ToString(), Session["Speciality"].ToString(), Session["WardDeath"].ToString(), Session["DischargeConsultant"].ToString(), false, false, false, false, "", "", "", showdisabled, Convert.ToInt32(Session["LoginUserID"]));
                if (patientDetails.Count > 0)
                {
                    ViewBag.TotalRecords = patientDetails[0].TotalRecords.ToString();
                    Session["TotalRecords"] = ViewBag.TotalRecords;
                }
                else
                    ViewBag.TotalRecords = "0";
                if (patientDetails.Count > 0)
                {
                    ViewBag.QAPCount = patientDetails[0].QAPCount.ToString();
                    Session["QAPCount"] = ViewBag.QAPCount;
                }
                else
                    ViewBag.QAPCount = "0";

                if (patientDetails.Count > 0)
                {
                    ViewBag.MedCount = patientDetails[0].MedCount.ToString();
                    Session["MedCount"] = ViewBag.MedCount;
                }
                else
                    ViewBag.MedCount = "0";
                if (patientDetails.Count > 0)
                {
                    ViewBag.MEOCount = patientDetails[0].MEOCount.ToString();
                    Session["MEOCount"] = ViewBag.MEOCount;
                }
                else
                    ViewBag.MEOCount = "0";

                int totalRecords = 0;
                if (patientDetails.Count > 0)
                {
                    totalRecords = patientDetails[0].TotalRecords;
                }

                int totalPagesCount = 0;
                totalPagesCount = (int)Math.Ceiling((float)totalRecords / 10);

                ViewBag.PageNumber = 1;
                ViewBag.SearchText = "";
                ViewBag.TotalPagesCount = totalPagesCount;
                ViewBag.TotalRecordCount = totalRecords;
                ViewBag.ModalRecordCount = patientDetails.Count;
                ViewBag.PageSize = 10;
                return View(patientDetails);              
            }
            else
                return RedirectToAction("Index", "Account");
        }
        [HttpPost]
        public ActionResult MorePatientDetails(int pageNumber = 1, int pageSize = 10, string searchfield = "", string SortColumn = "", bool frompager = false, bool IsTotal = false, bool IsQAP = false, bool IsMedTriage = false, bool IsMEO = false, bool fromfilter = false)
        {
            bool showdisabled = true;
            if (SortColumn != "")
            {
                if (!frompager)
                {
                    if (Session["CommonColumn"] == null || Convert.ToString(Session["CommonColumn"]) != SortColumn)
                    {
                        Session["CommonColumn"] = SortColumn;
                        Session["PatientSortType"] = null;
                    }
                    if (Session["PatientSortType"] == null)
                        Session["PatientSortType"] = "DESC";
                    else if (Session["PatientSortType"] != null && Convert.ToString(Session["CommonColumn"]) == SortColumn && Convert.ToString(Session["PatientSortType"]) == "DESC")
                        Session["PatientSortType"] = "ASC";
                    else if (Session["PatientSortType"] != null && Convert.ToString(Session["CommonColumn"]) == SortColumn && Convert.ToString(Session["PatientSortType"]) == "ASC")
                        Session["PatientSortType"] = "DESC";
                }
                else
                {
                    if (Session["CommonColumn"] == null || Convert.ToString(Session["CommonColumn"]) != SortColumn)
                    {
                        Session["CommonColumn"] = SortColumn;
                        Session["PatientSortType"] = null;
                    }
                    if (Session["PatientSortType"] == null)
                        Session["PatientSortType"] = "DESC";
                }
            }
            else
            {
                Session["CommonColumn"] = "DateofDeath";
                Session["PatientSortType"] = "DESC";
            }
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);

            if (Session["IsTotal"] == null || fromfilter == true)
                Session["IsTotal"] = IsTotal;
            else
                IsTotal = Convert.ToBoolean(Session["IsTotal"]);

            if (Session["IsQAP"] == null || fromfilter == true)
                Session["IsQAP"] = IsQAP;
            else
                IsQAP = Convert.ToBoolean(Session["IsQAP"]);
            if (Session["IsMedTriage"] == null || fromfilter == true)
                Session["IsMedTriage"] = IsMedTriage;
            else
                IsMedTriage = Convert.ToBoolean(Session["IsMedTriage"]);
            if (Session["IsMEO"] == null || fromfilter == true)
                Session["IsMEO"] = IsMEO;
            else
                IsMEO = Convert.ToBoolean(Session["IsMEO"]);

            if (pageSize != Convert.ToInt32(Session["MortalityPageSize"]))
                Session["CommonPageSize"] = pageSize;
            if (Session["PatientSortType"] == null)
                Session["PatientSortType"] = "DESC";
            List<clsPatientDetails> patientDetails = dBEngine.GetFilteredPatientDetails(pageNumber, Convert.ToInt32(Session["CommonPageSize"]), DateTime.ParseExact(Session["StartDate"].ToString(), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), DateTime.ParseExact(Session["EndDate"].ToString(), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), Convert.ToString(Session["PatientType"]), Convert.ToString(Session["Speciality"]), Convert.ToString(Session["WardDeath"]), Convert.ToString(Session["DischargeConsultant"]), IsTotal, IsQAP, IsMedTriage, IsMEO, Session["CommonColumn"].ToString(), Session["PatientSortType"].ToString(), searchfield, showdisabled, Convert.ToInt32(Session["LoginUserID"]));
            if (Session["TotalRecords"] != null)
                ViewBag.Total = Session["TotalRecords"];
            else
            {
                if (patientDetails.Count > 0)
                {
                    ViewBag.TotalRecords = patientDetails[0].TotalRecords.ToString();
                    Session["TotalRecords"] = ViewBag.TotalRecords;
                }
                else
                    ViewBag.TotalRecords = "0";
            }
            if (Session["QAPCount"] != null)
                ViewBag.QAPCount = Session["QAPCount"];
            else
            {
                if (patientDetails.Count > 0)
                {
                    ViewBag.QAPCount = patientDetails[0].QAPCount.ToString();
                    Session["QAPCount"] = ViewBag.QAPCount;
                }
                else
                    ViewBag.QAPCount = "0";
            }
            if (Session["MedCount"] != null)
                ViewBag.MedCount = Session["MedCount"];
            else
            {
                if (patientDetails.Count > 0)
                {
                    ViewBag.MedCount = patientDetails[0].MedCount.ToString();
                    Session["MedCount"] = ViewBag.MedCount;
                }
                else
                    ViewBag.MedCount = "0";
            }
            if (Session["MEOCount"] != null)
                ViewBag.MEOCount = Session["MEOCount"];
            else
            {
                if (patientDetails.Count > 0)
                {
                    ViewBag.MEOCount = patientDetails[0].MEOCount.ToString();
                    Session["MEOCount"] = ViewBag.PendingTestCases;
                }
                else
                    ViewBag.MEOCount = "0";
            }

            int totalRecords = 0;
            if (patientDetails.Count > 0)
            {
                totalRecords = patientDetails[0].TotalRecords;
            }

            int totalPagesCount = 0;
            int totalpages = 1;
            if (Convert.ToInt32(Session["CommonPageSize"]) != -1) totalpages = Convert.ToInt32(Session["CommonPageSize"]);
            else totalpages = totalRecords;
            totalPagesCount = (int)Math.Ceiling((float)totalRecords / totalpages);

            ViewBag.PageNumber = pageNumber;
            ViewBag.TotalPagesCount = totalPagesCount;
            ViewBag.TotalRecordCount = totalRecords;
            ViewBag.ModalRecordCount = patientDetails.Count;
            ViewBag.PageSize = 10;
            ViewBag.SearchText = searchfield;
            return PartialView("_PatientDetails", patientDetails);
        }

        public ActionResult PatientReportList(FormCollection formCollection, string btnSearch, int? id, bool qapreview, bool medtriage, bool meoreview)
        {
            bool showdisabled = false;
            Session["PatientReportList"] = true;
            Session["MedicalExaminerDashboard"] = false;
            if (Session["FullName"] != null)
            {
                if (string.IsNullOrEmpty(Session["FullName"].ToString()))
                    return RedirectToAction("Index", "Account");
            }
            else
                return RedirectToAction("Index", "Account");


            bool isUser = GetUserDetailsFromAD();
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            bool isSession = true;
            ViewBag.CareGroups = dBEngine.GetCareGroups(Convert.ToInt32(Session["LoginUserID"]));
            ViewBag.Consultants = dBEngine.GetCORSConsultants(Convert.ToInt32(Session["LoginUserID"]));
            ViewBag.PatientTypeDDM = dBEngine.GetPatientTypes(Convert.ToInt32(Session["LoginUserID"]));
            ViewBag.Wards = dBEngine.GetWards(Convert.ToInt32(Session["LoginUserID"]));
            ViewBag.MENames = dBEngine.GetMENames(Convert.ToInt32(Session["LoginUserID"]));
            ViewBag.LoadDischargeSpecialityDropdown = dBEngine.GetSpecialities(Convert.ToInt32(Session["LoginUserID"]));
            List<clsPatientDetails> patients = new List<clsPatientDetails>();
            if (formCollection.Count > 0)
                Session["PatientDetails"] = null;
            if (Session["PatientDetails"] != null)
                patients = (List<clsPatientDetails>)Session["patientDetails"];
            if (Session["PatientDetails"] != null && !qapreview && !medtriage && !meoreview)
            {

                ViewBag.LoadDischargeSpecialityDropdown = dBEngine.GetSpecialities(Convert.ToInt32(Session["LoginUserID"]));
                ViewBag.wardDeathDropdown = dBEngine.GetWardOfDeaths(Convert.ToInt32(Session["LoginUserID"]));
                ViewBag.dischargeConsultantDropdown = dBEngine.GetConsultants(Convert.ToInt32(Session["LoginUserID"]));
                ViewBag.PatientTypeDDM = dBEngine.GetPatientTypes(Convert.ToInt32(Session["LoginUserID"]));
                if (Convert.ToBoolean(Session["A&EDeath"]))
                    patients = patients.ToList().Where(a => a.PatientTypeActual == 1).ToList();
                if (Convert.ToBoolean(Session["AdultDeath"]))
                    patients = patients.ToList().Where(a => a.PatientTypeActual == 2).ToList();
                if (Convert.ToBoolean(Session["MECompleted"]))
                    patients = patients.ToList().Where(a => a.MedTriage == 3).ToList();
                if (Convert.ToBoolean(Session["MEOutstanding"]))
                    patients = patients.ToList().Where(a => a.MedTriage != 3).ToList();
                if (patients.Count > 0)
                {
                    patients[0].MEOCount = patients.ToList().Where(a => a.MEOReview != 3).Count();
                    patients[0].QAPCount = patients.ToList().Where(a => a.QAPReview == 2).Count();
                    patients[0].MedCount = patients.ToList().Where(a => a.MedTriage == 2).Count() + patients.ToList().Where(a => a.MedTriage == 1).Count();
                    Session["TotalDeaths"] = patients.Count;
                }
                if (patients.Count > 0)
                {
                    Session["QAPCount"] = patients[0].QAPCount;
                    Session["MedCount"] = patients[0].MedCount;
                    Session["MEOCount"] = patients[0].MEOCount;
                }
                else
                {
                    Session["QAPCount"] = 0; Session["MedCount"] = 0; Session["MEOCount"] = 0;
                }
                Session["TotalDeaths"] = patients.Count;
                return View(patients);
            }
            if (Convert.ToInt32(Session["LoginUserID"]) > 0)
            {
                try
                {
                    ViewBag.LoadDischargeSpecialityDropdown = dBEngine.GetSpecialities(Convert.ToInt32(Session["LoginUserID"]));
                    ViewBag.wardDeathDropdown = dBEngine.GetWardOfDeaths(Convert.ToInt32(Session["LoginUserID"]));
                    ViewBag.dischargeConsultantDropdown = dBEngine.GetConsultants(Convert.ToInt32(Session["LoginUserID"]));
                    ViewBag.PatientTypeDDM = dBEngine.GetPatientTypes(Convert.ToInt32(Session["LoginUserID"]));

                    if (Session["ReportStartDate"] == null)
                    {
                        Session["ReportStartDate"] = DateTime.Now.AddDays(-180).ToString("dd/MM/yyyy");
                        Session.Timeout = 30;
                        isSession = false;
                    }
                    else if (Convert.ToString(Session["ReportStartDate"]) == "" || Convert.ToString(Session["ReportStartDate"]) == "01/01/0001")
                    {
                        Session["ReportStartDate"] = DateTime.Now.AddDays(-180).ToString("dd/MM/yyyy");
                        Session.Timeout = 30;
                        isSession = false;
                    }
                    if (Session["ReportEndDate"] == null)
                    {
                        Session["ReportEndDate"] = DateTime.Now.ToString("dd/MM/yyyy");
                        Session.Timeout = 30;
                        isSession = false;
                    }
                    else if (Convert.ToString(Session["ReportEndDate"]) == "" || Convert.ToString(Session["ReportEndDate"]) == "01/01/0001")
                    {
                        Session["ReportEndDate"] = DateTime.Now.ToString("dd/MM/yyyy");
                        Session.Timeout = 30;
                        isSession = false;
                    }
                    if (Session["ReportWardDeath"] == null)
                    {
                        Session["ReportWardDeath"] = "0";
                        Session.Timeout = 30;
                        isSession = false;
                    }
                    else if (Convert.ToString(Session["ReportWardDeath"]) == "" && Session["PatientDetails"] == null)
                    {
                        Session["ReportWardDeath"] = "0";
                        Session.Timeout = 30;
                        isSession = false;
                    }
                    if (Session["ReportPatientType"] == null)
                    {
                        Session["ReportPatientType"] = "0";
                        Session.Timeout = 30;
                        isSession = false;
                    }
                    else if (Convert.ToString(Session["ReportPatientType"]) == "" && Session["PatientDetails"] == null)
                    {
                        Session["ReportPatientType"] = "0";
                        Session.Timeout = 30;
                        isSession = false;
                    }
                    if (Session["ReportDischargeConsultant"] == null)
                    {
                        Session["ReportDischargeConsultant"] = "0";
                        Session.Timeout = 30;
                        isSession = false;
                    }
                    else if (Convert.ToString(Session["ReportDischargeConsultant"]) == "" && Session["PatientDetails"] == null)
                    {
                        Session["ReportDischargeConsultant"] = "0";
                        Session.Timeout = 30;
                        isSession = false;
                    }
                    if (Session["ReportSpeciality"] == null)
                    {
                        Session["ReportSpeciality"] = "0";
                        Session.Timeout = 30;
                        isSession = false;
                    }
                    else if (Convert.ToString(Session["ReportSpeciality"]) == "" && Session["PatientDetails"] == null)
                    {
                        Session["ReportSpeciality"] = "0";
                        Session.Timeout = 30;
                        isSession = false;
                    }

                    patientDetails = dBEngine.GetFilteredPatientDetails(1, Convert.ToInt32(Session["MedDashboardPageSize"]), DateTime.ParseExact(Session["ReportStartDate"].ToString(), "dd/MM/yyyy", null), DateTime.ParseExact(Session["ReportEndDate"].ToString(), "dd/MM/yyyy", null), Convert.ToString(Session["ReportDischargeConsultant"]),
                    Convert.ToString(Session["ReportWardDeath"]), Convert.ToString(Session["ReportSpeciality"]), Convert.ToString(Session["ReportPatientType"]), false, false, false, false, "", "", "", showdisabled, Convert.ToInt32(Session["LoginUserID"]));
                    if (Convert.ToBoolean(Session["A&EDeath"]))
                        patientDetails = patientDetails.ToList().Where(a => a.PatientTypeActual == 1).ToList();
                    if (Convert.ToBoolean(Session["AdultDeath"]))
                        patientDetails = patientDetails.ToList().Where(a => a.PatientTypeActual == 2).ToList();
                    if (Convert.ToBoolean(Session["MECompleted"]))
                        patientDetails = patientDetails.ToList().Where(a => a.MedTriage == 3).ToList();
                    if (Convert.ToBoolean(Session["MEOutstanding"]))
                        patientDetails = patientDetails.ToList().Where(a => a.MedTriage != 3).ToList();
                    if (Convert.ToBoolean(Session["QAPCompleted"]))
                        patientDetails = patientDetails.ToList().Where(a => a.QAPReview == 3).ToList();
                    if (Convert.ToBoolean(Session["QAPOutstanding"]))
                        patientDetails = patientDetails.ToList().Where(a => a.QAPReview != 3).ToList();
                    if (Convert.ToInt32(Session["CauseID"]) != 0)
                        patientDetails = patientDetails.ToList().Where(a => a.CauseID == Convert.ToInt32(Session["CauseID"])).ToList();
                    if (Convert.ToBoolean(Session["MCCDIssue"]))
                        patientDetails = patientDetails.ToList().Where(a => a.MCCDIssue == Convert.ToBoolean(Session["MCCDIssue"])).ToList();
                    if (Convert.ToBoolean(Session["Referral"]))
                        patientDetails = patientDetails.ToList().Where(a => a.CoronerReferral == Convert.ToBoolean(Session["Referral"])).ToList();
                    if (Convert.ToBoolean(Session["PostMortem"]))
                        patientDetails = patientDetails.ToList().Where(a => a.HospitalPostMortem == Convert.ToBoolean(Session["PostMortem"])).ToList();
                    if (Convert.ToBoolean(Session["Concern"]))
                        patientDetails = patientDetails.ToList().Where(a => a.Concern == Convert.ToBoolean(Session["Concern"])).ToList();
                    if (Convert.ToBoolean(Session["RecommndedReferral"]))
                        patientDetails = patientDetails.ToList().Where(a => a.Referral == Convert.ToBoolean(Session["RecommndedReferral"])).ToList();
                    if (Convert.ToBoolean(Session["PatientSIRI"]))
                        patientDetails = patientDetails.ToList().Where(a => a.PatientSIRI == Convert.ToBoolean(Session["PatientSIRI"])).ToList();
                    if (Convert.ToBoolean(Session["SafeGuard"]))
                        patientDetails = patientDetails.ToList().Where(a => a.SafeGuard == Convert.ToBoolean(Session["SafeGuard"])).ToList();
                    if (Convert.ToBoolean(Session["LearningDisability"]))
                        patientDetails = patientDetails.ToList().Where(a => a.LearningDisability == Convert.ToBoolean(Session["LearningDisability"])).ToList();
                    if (Convert.ToBoolean(Session["ChildDeath"]))
                        patientDetails = patientDetails.ToList().Where(a => a.ChildDeath == Convert.ToBoolean(Session["ChildDeath"])).ToList();
                    if (Convert.ToBoolean(Session["WardTeam"]))
                        patientDetails = patientDetails.ToList().Where(a => a.WardTeam == Convert.ToBoolean(Session["WardTeam"])).ToList();
                    if (Convert.ToBoolean(Session["HeadOfCompliance"]))
                        patientDetails = patientDetails.ToList().Where(a => a.HeadOfCompliance == Convert.ToBoolean(Session["HeadOfCompliance"])).ToList();
                    if (Convert.ToBoolean(Session["PALsComplaints"]))
                        patientDetails = patientDetails.ToList().Where(a => a.PALsComplaints == Convert.ToBoolean(Session["PALsComplaints"])).ToList();
                    if (Convert.ToBoolean(Session["Other"]))
                        patientDetails = patientDetails.ToList().Where(a => a.Other == Convert.ToBoolean(Session["Other"])).ToList();
                    if (Convert.ToBoolean(Session["SJRRequired"]))
                        patientDetails = patientDetails.ToList().Where(a => a.IsFullSJRRequired == true).ToList();
                    if (Convert.ToBoolean(Session["ProblemOccured"]))
                        patientDetails = patientDetails.ToList().Where(a => a.ProblemOccured == true).ToList();
                    if (Convert.ToBoolean(Session["KeyLearnings"]))
                        patientDetails = patientDetails.ToList().Where(a => a.KeyLearnings.Trim() != "").ToList();
                    if (Convert.ToBoolean(Session["SJR1Completed"]))
                        patientDetails = patientDetails.ToList().Where(a => a.SJR1 == 3).ToList();
                    if (Convert.ToBoolean(Session["SJR1Outstanding"]))
                        patientDetails = patientDetails.ToList().Where(a => a.SJR1 != 3).ToList();
                    if (Convert.ToBoolean(Session["SJR1Grade0"]))
                        patientDetails = patientDetails.ToList().Where(a => a.AvoidabilityScoreID == 0 || a.AvoidabilityScoreID == 1).ToList();
                    if (Convert.ToBoolean(Session["SJR1Grade2"]))
                        patientDetails = patientDetails.ToList().Where(a => a.AvoidabilityScoreID == 2).ToList();
                    if (Convert.ToBoolean(Session["SJR1Grade3"]))
                        patientDetails = patientDetails.ToList().Where(a => a.AvoidabilityScoreID == 3).ToList();
                    if (Convert.ToBoolean(Session["MSGGrade0"]))
                        patientDetails = patientDetails.ToList().Where(a => a.MSGAvoidabilityScoreID == 0 || a.AvoidabilityScoreID == 1).ToList();
                    if (Convert.ToBoolean(Session["MSGGrade2"]))
                        patientDetails = patientDetails.ToList().Where(a => a.MSGAvoidabilityScoreID == 2).ToList();
                    if (Convert.ToBoolean(Session["MSGGrade3"]))
                        patientDetails = patientDetails.ToList().Where(a => a.MSGAvoidabilityScoreID == 3).ToList();
                    if (Convert.ToBoolean(Session["CoronerDecisionNFAction"]))
                        patientDetails = patientDetails.ToList().Where(a => a.CoronerDecisionNFAction == true).ToList();
                    if (Convert.ToBoolean(Session["CoronerDecisionPostMortem"]))
                        patientDetails = patientDetails.ToList().Where(a => a.CoronerDecisionPostMortem == true).ToList();
                    if (Convert.ToBoolean(Session["CoronerDecisionForensicPM"]))
                        patientDetails = patientDetails.ToList().Where(a => a.CoronerDecisionForensicPM == true).ToList();
                    if (Convert.ToBoolean(Session["CoronerDecisionGPIssue"]))
                        patientDetails = patientDetails.ToList().Where(a => a.CoronerDecisionGPIssue == true).ToList();
                    if (Convert.ToBoolean(Session["CoronerDecisionInquest"]))
                        patientDetails = patientDetails.ToList().Where(a => a.CoronerDecisionInquest == true).ToList();
                    if (Convert.ToBoolean(Session["CoronerDecision100A"]))
                        patientDetails = patientDetails.ToList().Where(a => a.CoronerDecision100A == true).ToList();
                    if (Convert.ToInt32(Session["FeedbackTypeID"]) != 0)
                        patientDetails = patientDetails.ToList().Where(a => a.FeedbackTypeID == Convert.ToInt32(Session["FeedbackTypeID"])).ToList();
                    if (Convert.ToBoolean(Session["FormCompleted"]))
                        patientDetails = patientDetails.ToList().Where(a => a.FormCompleted == true).ToList();
                    if (Convert.ToBoolean(Session["Learning"]))
                        patientDetails = patientDetails.ToList().Where(a => a.LearningDisabilityPatient == true).ToList();
                    if (Convert.ToBoolean(Session["Paeds"]))
                        patientDetails = patientDetails.ToList().Where(a => a.PaediatricPatient == true).ToList();
                    if (Convert.ToBoolean(Session["Mental"]))
                        patientDetails = patientDetails.ToList().Where(a => a.MentalillnessPatient == true).ToList();
                    if (Convert.ToBoolean(Session["Elective"]))
                        patientDetails = patientDetails.ToList().Where(a => a.ElectiveAdmission == true).ToList();
                    if (Convert.ToBoolean(Session["NOK"]))
                        patientDetails = patientDetails.ToList().Where(a => a.NoKConcernsDeath == true).ToList();
                    if (Convert.ToBoolean(Session["Chemo"]))
                        patientDetails = patientDetails.ToList().Where(a => a.DeathChemo == true).ToList();
                    if (Convert.ToBoolean(Session["OtherReason"]))
                        patientDetails = patientDetails.ToList().Where(a => a.OtherConcern == true).ToList();
                    Session["TotalDeaths"] = patientDetails.Count;
                }
                catch (Exception ex)
                {
                    dBEngine.LogException(ex.Message, this.ToString(), "ValidateUser", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
                    throw ex;
                }
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
            patientDetails = patientDetails.OrderByDescending(a => a.DateofDeath).ToList();
            return View(patientDetails);
        }

        public ActionResult PatientExtract(FormCollection formCollection, string btnSearch, int? id, bool isReset = false)
        {
            bool showdisabled = false;
            Session["MedicalExaminerDashboard"] = false;
            Session["PatientExtract"] = true;
            Session["PatientReportList"] = false;
            Session.Timeout = 30;
            Session["ReportStartDate"] = null;
            Session["ReportEndDate"] = null;
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportCareGroup"])))
                Session["ReportCareGroup"] = "0";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeSpeciality"])))
                Session["ReportDischargeSpeciality"] = "0";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportPatientType"])))
                Session["ReportPatientType"] = "0";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportWardDeath"])))
                Session["ReportWardDeath"] = "0";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeConsultant"])))
                Session["ReportDischargeConsultant"] = "0";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportMEName"])))
                Session["ReportMEName"] = "0";
            Session["TotalRecords"] = null;
            Session["QAPCount"] = null;
            Session["MedCount"] = null;
            Session["MEOCount"] = null;
            Session["CommonPageSize"] = 10;
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            if (Convert.ToInt32(Session["LoginUserID"]) > 0)
            {
                PatientExtractFilterDDM filterddm = dBEngine.GetPatientExtractFilterDDM(Convert.ToInt32(Session["LoginUserID"]));
                ViewBag.CareGroups = filterddm.lstCareGroups;
                ViewBag.LoadDischargeSpecialityDropdown = filterddm.lstSpecialities;
                ViewBag.PatientTypeDDM = filterddm.lstPatientTypes;
                ViewBag.wardDeathDropdown = filterddm.lstWardOfDeaths;
                ViewBag.dischargeConsultantDropdown = filterddm.lstDischargeConsultants;
                ViewBag.MENames = filterddm.lstMENames;

                if (Session["CommonPageSize"] == null)
                    Session["CommonPageSize"] = 10;
                if (Request.HttpMethod != "POST")
                {
                    COVIDDefaultDate defaultdate = dBEngine.GetMortalityDefaultDate(Convert.ToInt32(Session["LoginUserID"]));
                    if (isReset)
                    {
                        if (defaultdate.StartDate == null)
                            Session["ReportStartDate"] = DateTime.Now.AddDays(-180).ToString("dd/MM/yyyy");
                        else
                            Session["ReportStartDate"] = defaultdate.StartDate;
                        if (defaultdate.EndDate == null)
                            Session["ReportEndDate"] = DateTime.Now.ToString("dd/MM/yyyy");
                        else
                            Session["ReportEndDate"] = defaultdate.EndDate;
                        Session["ReportCareGroup"] = "0";
                        Session["ReportDischargeSpeciality"] = "0";
                        Session["ReportPatientType"] = "0";
                        Session["ReportWardDeath"] = "0";
                        Session["ReportDischargeConsultant"] = "0";
                        Session["ReportMEName"] = "0";
                    }
                    if (defaultdate.StartDate == null)
                        Session["ReportStartDate"] = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
                    else
                        Session["ReportStartDate"] = defaultdate.StartDate;
                    if (defaultdate.EndDate == null)
                        Session["ReportEndDate"] = DateTime.Now.ToString("dd/MM/yyyy");
                    else
                        Session["ReportEndDate"] = defaultdate.EndDate;
                    if (Session["ReportCareGroup"] == null)
                        Session["ReportCareGroup"] = "0";
                    if (Session["ReportPatientType"] == null)
                        Session["ReportPatientType"] = "0";
                    if (Session["ReportDischargeSpeciality"] == null)
                        Session["ReportDischargeSpeciality"] = "0";
                    if (Session["ReportWardDeath"] == null)
                        Session["ReportWardDeath"] = "0";
                    if (Session["ReportDischargeConsultant"] == null)
                        Session["ReportDischargeConsultant"] = "0";
                    if (Session["ReportMEName"] == null)
                        Session["ReportMEName"] = "0";

                    Session["IsTotal"] = null;
                    Session["IsQAP"] = null;
                    Session["IsMedTriage"] = null;
                    Session["IsMEO"] = null;
                    Session["CommonPageSize"] = 10;
                }
                else
                {
                    if (isReset == false)
                    {
                        Session["ReportStartDate"] = formCollection["txtStartDate"].Replace("Order Start Date ", "");
                        Session["ReportEndDate"] = formCollection["txtEndDate"].Replace("Order End Date ", "");
                        Session["ReportCareGroup"] = formCollection["ddlCareGroups"];
                        Session["ReportPatientType"] = formCollection["ddlPatientType"];
                        Session["ReportDischargeSpeciality"] = formCollection["ddlDischargeSpeciality"];
                        Session["ReportWardDeath"] = formCollection["ddlWardDeath"];
                        Session["ReportDischargeConsultant"] = formCollection["ddlDischargeConsultant"];
                        Session["ReportMEName"] = formCollection["ddlMENames"];

                        Session["IsTotal"] = null;
                        Session["IsQAP"] = null;
                        Session["IsMedTriage"] = null;
                        Session["IsMEO"] = null;
                    }
                }
                patientDetails = dBEngine.GetFilteredPatientDetails(1, Convert.ToInt32(Session["CommonPageSize"].ToString()), DateTime.ParseExact(Session["ReportStartDate"].ToString(), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), DateTime.ParseExact(Session["ReportEndDate"].ToString(), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), Session["ReportPatientType"].ToString(), Session["ReportDischargeSpeciality"].ToString(), Session["ReportWardDeath"].ToString(), Session["ReportDischargeConsultant"].ToString(), false, false, false, false, "", "", "", showdisabled, Convert.ToInt32(Session["LoginUserID"]));
                if (patientDetails.Count > 0)
                {
                    ViewBag.TotalRecords = patientDetails[0].TotalRecords.ToString();
                    Session["TotalRecords"] = ViewBag.TotalRecords;
                }
                else
                    ViewBag.TotalRecords = "0";
                if (patientDetails.Count > 0)
                {
                    ViewBag.QAPCount = patientDetails[0].QAPCount.ToString();
                    Session["QAPCount"] = ViewBag.QAPCount;
                }
                else
                    ViewBag.QAPCount = "0";

                if (patientDetails.Count > 0)
                {
                    ViewBag.MedCount = patientDetails[0].MedCount.ToString();
                    Session["MedCount"] = ViewBag.MedCount;
                }
                else
                    ViewBag.MedCount = "0";
                if (patientDetails.Count > 0)
                {
                    ViewBag.MEOCount = patientDetails[0].MEOCount.ToString();
                    Session["MEOCount"] = ViewBag.MEOCount;
                }
                else
                    ViewBag.MEOCount = "0";

                int totalRecords = 0;
                if (patientDetails.Count > 0)
                {
                    totalRecords = patientDetails[0].TotalRecords;
                }

                int totalPagesCount = 0;
                totalPagesCount = (int)Math.Ceiling((float)totalRecords / Convert.ToInt32(Session["CommonPageSize"]));

                ViewBag.PageNumber = 1;
                ViewBag.SearchText = "";
                ViewBag.TotalPagesCount = totalPagesCount;
                ViewBag.TotalRecordCount = totalRecords;
                ViewBag.ModalRecordCount = patientDetails.Count;
                ViewBag.PageSize = Convert.ToInt32(Session["CommonPageSize"]);
            }
            return View(patientDetails);
        }

        public ActionResult TotalDeaths()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportStartDate"])) || Convert.ToString(Session["ReportStartDate"]) == "01/01/0001")
                Session["ReportStartDate"] = System.DateTime.Now.AddDays(-30);
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportEndDate"])) || Convert.ToString(Session["ReportEndDate"]) == "01/01/0001")
                Session["ReportEndDate"] = System.DateTime.Now;
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportCaregroup"])))
                Session["ReportCaregroup"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportPatientType"])))
                Session["ReportPatientType"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportWardDeath"])))
                Session["ReportWardDeath"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeConsultant"])))
                Session["ReportDischargeConsultant"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeSpeciality"])))
                Session["ReportDischargeSpeciality"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportMEName"])))
                Session["ReportMEName"] = "";
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            patientDetails = dBEngine.GetPatientDetailsByTotalDeaths(DateTime.ParseExact(Convert.ToString(Session["ReportStartDate"]), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), DateTime.ParseExact(Convert.ToString(Session["ReportEndDate"]), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), Convert.ToString(Session["ReportCaregroup"]), Convert.ToString(Session["ReportPatientType"]),Convert.ToString(Session["ReportWardDeath"]), Convert.ToString(Session["ReportDischargeConsultant"]), Convert.ToString(Session["ReportDischargeSpeciality"]), Convert.ToString(Session["ReportMEName"]), Convert.ToInt32(Session["LoginUserID"]));
            Session["PatientDetails"] = patientDetails;
            Session["A&EDeath"] = false;
            Session["MEOutstanding"] = false;
            Session["MECompleted"] = false;
            Session["MCCDIssue"] = false;
            Session["Referral"] = false;
            Session["PostMortem"] = false;
            Session["QAPCompleted"] = false;
            Session["QAPOutstanding"] = false;
            Session["CauseID"] = 0;
            Session["CareQuality"] = false;
            Session["RecommndedReferral"] = false;
            Session["PatientSIRI"] = false;
            Session["SafeGuard"] = false;
            Session["LearningDisability"] = false;
            Session["ChildDeath"] = false;
            Session["WardTeam"] = false;
            Session["HeadOfCompliance"] = false;
            Session["PALsComplaints"] = false;
            Session["Other"] = false;
            Session["SJRRequired"] = false;
            Session["ProblemOccured"] = false;
            Session["KeyLearnings"] = false;
            Session["SJR1Completed"] = false;
            Session["SJR1Outstanding"] = false;
            Session["SJR1Grade0"] = false;
            Session["SJR1Grade2"] = false;
            Session["SJR1Grade3"] = false;
            Session["MSGGrade0"] = false;
            Session["MSGGrade2"] = false;
            Session["MSG1Grade3"] = false;
            Session["CoronerDecisionNFAction"] = false;
            Session["CoronerDecisionPostMortem"] = false;
            Session["CoronerDecisionForensicPM"] = false;
            Session["CoronerDecisionGPIssue"] = false;
            Session["CoronerDecisionInquest"] = false;
            Session["CoronerDecision100A"] = false;
            Session["FeedbackTypeID"] = 0;
            Session["SJR2Completed"] = false;
            Session["SJR2Outstanding"] = false;
            Session["SJR2Grade0"] = false;
            Session["SJR2Grade1"] = false;
            Session["SJR2Grade2"] = false;
            Session["SJR2Grade3"] = false;
            Session["SJR1Grade0"] = false;
            Session["SJR1Grade1"] = false;
            Session["SJR1Grade2"] = false;
            Session["SJR1Grade3"] = false;
            Session["SJR1VeryPoor"] = false;
            Session["SJR1Poor"] = false;
            Session["SJR1Adequate"] = false;
            Session["SJR1Good"] = false;
            Session["SJR1Excellent"] = false;
            Session["SJR2VeryPoor"] = false;
            Session["SJR2Poor"] = false;
            Session["SJR2Adequate"] = false;
            Session["SJR2Good"] = false;
            Session["SJR2Excellent"] = false;
            return RedirectToAction("PatientReportList", new { id = 0, qapreview = false, medtriage = false, meoreview = false });
        }

        public ActionResult AandEDeaths()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportStartDate"])) || Convert.ToString(Session["ReportStartDate"]) == "01/01/0001")
                Session["ReportStartDate"] = System.DateTime.Now.AddDays(-30);
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportEndDate"])) || Convert.ToString(Session["ReportEndDate"]) == "01/01/0001")
                Session["ReportEndDate"] = System.DateTime.Now;
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportCaregroup"])))
                Session["ReportCaregroup"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportPatientType"])))
                Session["ReportPatientType"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportWardDeath"])))
                Session["ReportWardDeath"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeConsultant"])))
                Session["ReportDischargeConsultant"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeSpeciality"])))
                Session["ReportDischargeSpeciality"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportMEName"])))
                Session["ReportMEName"] = "";
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            patientDetails = dBEngine.GetPatientDetailsByTotalDeaths(DateTime.ParseExact(Convert.ToString(Session["ReportStartDate"]).Split(" ".ToCharArray())[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), DateTime.ParseExact(Convert.ToString(Session["ReportEndDate"]).Split(" ".ToCharArray())[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), Convert.ToString(Session["ReportCaregroup"]), Convert.ToString(Session["ReportPatientType"]), Convert.ToString(Session["ReportWardDeath"]), Convert.ToString(Session["ReportDischargeConsultant"]), Convert.ToString(Session["ReportDischargeSpeciality"]), Convert.ToString(Session["ReportMEName"]), Convert.ToInt32(Session["LoginUserID"]));
            Session["PatientDetails"] = patientDetails;
            Session["A&EDeath"] = true;
            Session["MEOutstanding"] = false;
            Session["MECompleted"] = false;
            Session["MCCDIssue"] = false;
            Session["Referral"] = false;
            Session["PostMortem"] = false;
            Session["QAPCompleted"] = false;
            Session["QAPOutstanding"] = false;
            Session["CauseID"] = 0;
            Session["CareQuality"] = false;
            Session["RecommndedReferral"] = false;
            Session["PatientSIRI"] = false;
            Session["SafeGuard"] = false;
            Session["LearningDisability"] = false;
            Session["ChildDeath"] = false;
            Session["WardTeam"] = false;
            Session["HeadOfCompliance"] = false;
            Session["PALsComplaints"] = false;
            Session["Other"] = false;
            Session["SJRRequired"] = false;
            Session["ProblemOccured"] = false;
            Session["KeyLearnings"] = false;
            Session["SJR1Completed"] = false;
            Session["SJR1Outstanding"] = false;
            Session["SJR1Grade0"] = false;
            Session["SJR1Grade2"] = false;
            Session["SJR1Grade3"] = false;
            Session["MSGGrade0"] = false;
            Session["MSGGrade2"] = false;
            Session["MSG1Grade3"] = false;
            Session["CoronerDecisionNFAction"] = false;
            Session["CoronerDecisionPostMortem"] = false;
            Session["CoronerDecisionForensicPM"] = false;
            Session["CoronerDecisionGPIssue"] = false;
            Session["CoronerDecisionInquest"] = false;
            Session["CoronerDecision100A"] = false;
            Session["FeedbackTypeID"] = 0;
            Session["SJR2Completed"] = false;
            Session["SJR2Outstanding"] = false;
            Session["SJR2Required"] = false;
            Session["SJR2NotRequired"] = false;
            Session["SJR2Grade0"] = false;
            Session["SJR2Grade1"] = false;
            Session["SJR2Grade2"] = false;
            Session["SJR2Grade3"] = false;
            Session["SJR1Grade0"] = false;
            Session["SJR1Grade1"] = false;
            Session["SJR1Grade2"] = false;
            Session["SJR1Grade3"] = false;
            Session["SJR1VeryPoor"] = false;
            Session["SJR1Poor"] = false;
            Session["SJR1Adequate"] = false;
            Session["SJR1Good"] = false;
            Session["SJR1Excellent"] = false;
            Session["SJR2VeryPoor"] = false;
            Session["SJR2Poor"] = false;
            Session["SJR2Adequate"] = false;
            Session["SJR2Good"] = false;
            Session["SJR2Excellent"] = false;
            return RedirectToAction("PatientReportList", new { id = 0, qapreview = false, medtriage = false, meoreview = false });
        }

        public ActionResult AAPCDeaths()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportStartDate"])) || Convert.ToString(Session["ReportStartDate"]) == "01/01/0001")
                Session["ReportStartDate"] = System.DateTime.Now.AddDays(-30);
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportEndDate"])) || Convert.ToString(Session["ReportEndDate"]) == "01/01/0001")
                Session["ReportEndDate"] = System.DateTime.Now;
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportCaregroup"])))
                Session["ReportCaregroup"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportPatientType"])))
                Session["ReportPatientType"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportWardDeath"])))
                Session["ReportWardDeath"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeConsultant"])))
                Session["ReportDischargeConsultant"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeSpeciality"])))
                Session["ReportDischargeSpeciality"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportMEName"])))
                Session["ReportMEName"] = "";
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            patientDetails = dBEngine.GetPatientDetailsByTotalDeaths(DateTime.ParseExact(Convert.ToString(Session["ReportStartDate"]).Split(" ".ToCharArray())[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), DateTime.ParseExact(Convert.ToString(Session["ReportEndDate"]).Split(" ".ToCharArray())[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), Convert.ToString(Session["ReportCaregroup"]), Convert.ToString(Session["ReportPatientType"]), Convert.ToString(Session["ReportWardDeath"]), Convert.ToString(Session["ReportDischargeConsultant"]), Convert.ToString(Session["ReportDischargeSpeciality"]), Convert.ToString(Session["ReportMEName"]), Convert.ToInt32(Session["LoginUserID"]));
            Session["PatientDetails"] = patientDetails;
            Session["AdultDeath"] = true;
            Session["MEOutstanding"] = false;
            Session["MECompleted"] = false;
            Session["MCCDIssue"] = false;
            Session["Referral"] = false;
            Session["PostMortem"] = false;
            Session["QAPCompleted"] = false;
            Session["QAPOutstanding"] = false;
            Session["CauseID"] = 0;
            Session["CareQuality"] = false;
            Session["RecommndedReferral"] = false;
            Session["PatientSIRI"] = false;
            Session["SafeGuard"] = false;
            Session["LearningDisability"] = false;
            Session["ChildDeath"] = false;
            Session["WardTeam"] = false;
            Session["HeadOfCompliance"] = false;
            Session["PALsComplaints"] = false;
            Session["Other"] = false;
            Session["SJRRequired"] = false;
            Session["ProblemOccured"] = false;
            Session["KeyLearnings"] = false;
            Session["SJR1Completed"] = false;
            Session["SJR1Outstanding"] = false;
            Session["SJR1Grade0"] = false;
            Session["SJR1Grade2"] = false;
            Session["SJR1Grade3"] = false;
            Session["MSGGrade0"] = false;
            Session["MSGGrade2"] = false;
            Session["MSG1Grade3"] = false;
            Session["CoronerDecisionNFAction"] = false;
            Session["CoronerDecisionPostMortem"] = false;
            Session["CoronerDecisionForensicPM"] = false;
            Session["CoronerDecisionGPIssue"] = false;
            Session["CoronerDecisionInquest"] = false;
            Session["CoronerDecision100A"] = false;
            Session["FeedbackTypeID"] = 0;
            Session["SJR2Completed"] = false;
            Session["SJR2Outstanding"] = false;
            Session["SJR2Required"] = false;
            Session["SJR2NotRequired"] = false;
            Session["SJR2Grade0"] = false;
            Session["SJR2Grade1"] = false;
            Session["SJR2Grade2"] = false;
            Session["SJR2Grade3"] = false;
            Session["SJR1Grade0"] = false;
            Session["SJR1Grade1"] = false;
            Session["SJR1Grade2"] = false;
            Session["SJR1Grade3"] = false;
            Session["SJR1VeryPoor"] = false;
            Session["SJR1Poor"] = false;
            Session["SJR1Adequate"] = false;
            Session["SJR1Good"] = false;
            Session["SJR1Excellent"] = false;
            Session["SJR2VeryPoor"] = false;
            Session["SJR2Poor"] = false;
            Session["SJR2Adequate"] = false;
            Session["SJR2Good"] = false;
            Session["SJR2Excellent"] = false;
            return RedirectToAction("PatientReportList", new { id = 0, qapreview = false, medtriage = false, meoreview = false });
        }

        public ActionResult MEReviewCount(bool completed)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportStartDate"])) || Convert.ToString(Session["ReportStartDate"]) == "01/01/0001")
                Session["ReportStartDate"] = System.DateTime.Now.AddDays(-30);
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportEndDate"])) || Convert.ToString(Session["ReportEndDate"]) == "01/01/0001")
                Session["ReportEndDate"] = System.DateTime.Now;
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportCaregroup"])))
                Session["ReportCaregroup"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportPatientType"])))
                Session["ReportPatientType"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportWardDeath"])))
                Session["ReportWardDeath"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeConsultant"])))
                Session["ReportDischargeConsultant"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeSpeciality"])))
                Session["ReportDischargeSpeciality"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportMEName"])))
                Session["ReportMEName"] = "";
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            patientDetails = dBEngine.GetPatientDetailsByMEReview(DateTime.ParseExact(Convert.ToString(Session["ReportStartDate"]).Split(" ".ToCharArray())[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), DateTime.ParseExact(Convert.ToString(Session["ReportEndDate"]).Split(" ".ToCharArray())[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), Convert.ToString(Session["ReportCaregroup"]), Convert.ToString(Session["ReportPatientType"]), Convert.ToString(Session["ReportWardDeath"]), Convert.ToString(Session["ReportDischargeConsultant"]), Convert.ToString(Session["ReportDischargeSpeciality"]), Convert.ToString(Session["ReportMEName"]), completed, Convert.ToInt32(Session["LoginUserID"]));
            Session["PatientDetails"] = patientDetails;
            if (completed)
            {
                Session["MECompleted"] = true;
                Session["MEOutstanding"] = false;
                Session["AdultDeath"] = false;
                Session["A&EDeath"] = false;
                Session["MCCDIssue"] = false;
                Session["Referral"] = false;
                Session["PostMortem"] = false;
                Session["QAPCompleted"] = false;
                Session["QAPOutstanding"] = false;
                Session["CauseID"] = 0;
                Session["CareQuality"] = false;
                Session["RecommndedReferral"] = false;
                Session["PatientSIRI"] = false;
                Session["SafeGuard"] = false;
                Session["LearningDisability"] = false;
                Session["ChildDeath"] = false;
                Session["WardTeam"] = false;
                Session["HeadOfCompliance"] = false;
                Session["PALsComplaints"] = false;
                Session["Other"] = false;
                Session["SJRRequired"] = false;
                Session["ProblemOccured"] = false;
                Session["KeyLearnings"] = false;
                Session["SJR1Completed"] = false;
                Session["SJR1Outstanding"] = false;
                Session["SJR1Grade0"] = false;
                Session["SJR1Grade2"] = false;
                Session["SJR1Grade3"] = false;
                Session["MSGGrade0"] = false;
                Session["MSGGrade2"] = false;
                Session["MSG1Grade3"] = false;
                Session["CoronerDecisionNFAction"] = false;
                Session["CoronerDecisionPostMortem"] = false;
                Session["CoronerDecisionForensicPM"] = false;
                Session["CoronerDecisionGPIssue"] = false;
                Session["CoronerDecisionInquest"] = false;
                Session["CoronerDecision100A"] = false;
                Session["FeedbackTypeID"] = 0;
                Session["SJR2Completed"] = false;
                Session["SJR2Outstanding"] = false;
                Session["SJR2Required"] = false;
                Session["SJR2NotRequired"] = false;
                Session["SJR2Grade0"] = false;
                Session["SJR2Grade1"] = false;
                Session["SJR2Grade2"] = false;
                Session["SJR2Grade3"] = false;
                Session["SJR1Grade0"] = false;
                Session["SJR1Grade1"] = false;
                Session["SJR1Grade2"] = false;
                Session["SJR1Grade3"] = false;
                Session["SJR1VeryPoor"] = false;
                Session["SJR1Poor"] = false;
                Session["SJR1Adequate"] = false;
                Session["SJR1Good"] = false;
                Session["SJR1Excellent"] = false;
                Session["SJR2VeryPoor"] = false;
                Session["SJR2Poor"] = false;
                Session["SJR2Adequate"] = false;
                Session["SJR2Good"] = false;
                Session["SJR2Excellent"] = false;
            }
            else
            {
                Session["MEOutstanding"] = true;
                Session["MECompleted"] = false;
                Session["AdultDeath"] = false;
                Session["A&EDeath"] = false;
                Session["MCCDIssue"] = false;
                Session["Referral"] = false;
                Session["PostMortem"] = false;
                Session["QAPCompleted"] = false;
                Session["QAPOutstanding"] = false;
                Session["CauseID"] = 0;
                Session["CareQuality"] = false;
                Session["RecommndedReferral"] = false;
                Session["PatientSIRI"] = false;
                Session["SafeGuard"] = false;
                Session["LearningDisability"] = false;
                Session["ChildDeath"] = false;
                Session["WardTeam"] = false;
                Session["HeadOfCompliance"] = false;
                Session["PALsComplaints"] = false;
                Session["Other"] = false;
                Session["SJRRequired"] = false;
                Session["ProblemOccured"] = false;
                Session["KeyLearnings"] = false;
                Session["SJR1Completed"] = false;
                Session["SJR1Outstanding"] = false;
                Session["SJR1Grade0"] = false;
                Session["SJR1Grade2"] = false;
                Session["SJR1Grade3"] = false;
                Session["MSGGrade0"] = false;
                Session["MSGGrade2"] = false;
                Session["MSG1Grade3"] = false;
                Session["CoronerDecisionNFAction"] = false;
                Session["CoronerDecisionPostMortem"] = false;
                Session["CoronerDecisionForensicPM"] = false;
                Session["CoronerDecisionGPIssue"] = false;
                Session["CoronerDecisionInquest"] = false;
                Session["CoronerDecision100A"] = false;
                Session["FeedbackTypeID"] = 0;
                Session["SJR2Completed"] = false;
                Session["SJR2Outstanding"] = false;
                Session["SJR2Required"] = false;
                Session["SJR2NotRequired"] = false;
                Session["SJR2Grade0"] = false;
                Session["SJR2Grade1"] = false;
                Session["SJR2Grade2"] = false;
                Session["SJR2Grade3"] = false;
                Session["SJR1Grade0"] = false;
                Session["SJR1Grade1"] = false;
                Session["SJR1Grade2"] = false;
                Session["SJR1Grade3"] = false;
                Session["SJR1VeryPoor"] = false;
                Session["SJR1Poor"] = false;
                Session["SJR1Adequate"] = false;
                Session["SJR1Good"] = false;
                Session["SJR1Excellent"] = false;
                Session["SJR2VeryPoor"] = false;
                Session["SJR2Poor"] = false;
                Session["SJR2Adequate"] = false;
                Session["SJR2Good"] = false;
                Session["SJR2Excellent"] = false;
            }
            return RedirectToAction("PatientReportList", new { id = 0, qapreview = false, medtriage = false, meoreview = false });
        }

        public ActionResult QAPReviewCount(bool completed)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportStartDate"])) || Convert.ToString(Session["ReportStartDate"]) == "01/01/0001")
                Session["ReportStartDate"] = System.DateTime.Now.AddDays(-30);
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportEndDate"])) || Convert.ToString(Session["ReportEndDate"]) == "01/01/0001")
                Session["ReportEndDate"] = System.DateTime.Now;
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportCaregroup"])))
                Session["ReportCaregroup"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportPatientType"])))
                Session["ReportPatientType"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportWardDeath"])))
                Session["ReportWardDeath"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeConsultant"])))
                Session["ReportDischargeConsultant"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeSpeciality"])))
                Session["ReportDischargeSpeciality"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportMEName"])))
                Session["ReportMEName"] = "";
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            patientDetails = dBEngine.GetPatientDetailsByQAPReview(DateTime.ParseExact(Convert.ToString(Session["ReportStartDate"]).Split(" ".ToCharArray())[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), DateTime.ParseExact(Convert.ToString(Session["ReportEndDate"]).Split(" ".ToCharArray())[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), Convert.ToString(Session["ReportCaregroup"]), Convert.ToString(Session["ReportPatientType"]), Convert.ToString(Session["ReportWardDeath"]), Convert.ToString(Session["ReportDischargeConsultant"]), Convert.ToString(Session["ReportDischargeSpeciality"]), Convert.ToString(Session["ReportMEName"]), completed, Convert.ToInt32(Session["LoginUserID"]));
            Session["PatientDetails"] = patientDetails;
            if (completed)
            {
                Session["QAPCompleted"] = true;
                Session["QAPOutstanding"] = false;
                Session["MECompleted"] = false;
                Session["MEOutstanding"] = false;
                Session["AdultDeath"] = false;
                Session["A&EDeath"] = false;
                Session["MCCDIssue"] = false;
                Session["Referral"] = false;
                Session["PostMortem"] = false;
                Session["CauseID"] = 0;
                Session["CareQuality"] = false;
                Session["RecommndedReferral"] = false;
                Session["PatientSIRI"] = false;
                Session["SafeGuard"] = false;
                Session["LearningDisability"] = false;
                Session["ChildDeath"] = false;
                Session["WardTeam"] = false;
                Session["HeadOfCompliance"] = false;
                Session["PALsComplaints"] = false;
                Session["Other"] = false;
                Session["SJRRequired"] = false;
                Session["ProblemOccured"] = false;
                Session["KeyLearnings"] = false;
                Session["SJR1Completed"] = false;
                Session["SJR1Outstanding"] = false;
                Session["SJR1Grade0"] = false;
                Session["SJR1Grade2"] = false;
                Session["SJR1Grade3"] = false;
                Session["MSGGrade0"] = false;
                Session["MSGGrade2"] = false;
                Session["MSG1Grade3"] = false;
                Session["CoronerDecisionNFAction"] = false;
                Session["CoronerDecisionPostMortem"] = false;
                Session["CoronerDecisionForensicPM"] = false;
                Session["CoronerDecisionGPIssue"] = false;
                Session["CoronerDecisionInquest"] = false;
                Session["CoronerDecision100A"] = false;
                Session["FeedbackTypeID"] = 0;
                Session["SJR2Completed"] = false;
                Session["SJR2Outstanding"] = false;
                Session["SJR2Required"] = false;
                Session["SJR2NotRequired"] = false;
                Session["SJR2Grade0"] = false;
                Session["SJR2Grade1"] = false;
                Session["SJR2Grade2"] = false;
                Session["SJR2Grade3"] = false;
                Session["SJR1Grade0"] = false;
                Session["SJR1Grade1"] = false;
                Session["SJR1Grade2"] = false;
                Session["SJR1Grade3"] = false;
                Session["SJR1VeryPoor"] = false;
                Session["SJR1Poor"] = false;
                Session["SJR1Adequate"] = false;
                Session["SJR1Good"] = false;
                Session["SJR1Excellent"] = false;
                Session["SJR2VeryPoor"] = false;
                Session["SJR2Poor"] = false;
                Session["SJR2Adequate"] = false;
                Session["SJR2Good"] = false;
                Session["SJR2Excellent"] = false;
            }
            else
            {
                Session["QAPCompleted"] = false;
                Session["QAPOutstanding"] = true;
                Session["MEOutstanding"] = false;
                Session["MECompleted"] = false;
                Session["AdultDeath"] = false;
                Session["A&EDeath"] = false;
                Session["MCCDIssue"] = false;
                Session["Referral"] = false;
                Session["PostMortem"] = false;
                Session["CauseID"] = 0;
                Session["CareQuality"] = false;
                Session["RecommndedReferral"] = false;
                Session["PatientSIRI"] = false;
                Session["SafeGuard"] = false;
                Session["LearningDisability"] = false;
                Session["ChildDeath"] = false;
                Session["WardTeam"] = false;
                Session["HeadOfCompliance"] = false;
                Session["PALsComplaints"] = false;
                Session["Other"] = false;
                Session["SJRRequired"] = false;
                Session["ProblemOccured"] = false;
                Session["KeyLearnings"] = false;
                Session["SJR1Completed"] = false;
                Session["SJR1Outstanding"] = false;
                Session["SJR1Grade0"] = false;
                Session["SJR1Grade2"] = false;
                Session["SJR1Grade3"] = false;
                Session["MSGGrade0"] = false;
                Session["MSGGrade2"] = false;
                Session["MSG1Grade3"] = false;
                Session["CoronerDecisionNFAction"] = false;
                Session["CoronerDecisionPostMortem"] = false;
                Session["CoronerDecisionForensicPM"] = false;
                Session["CoronerDecisionGPIssue"] = false;
                Session["CoronerDecisionInquest"] = false;
                Session["CoronerDecision100A"] = false;
                Session["FeedbackTypeID"] = 0;
                Session["SJR2Completed"] = false;
                Session["SJR2Outstanding"] = false;
                Session["SJR2Required"] = false;
                Session["SJR2NotRequired"] = false;
                Session["SJR2Grade0"] = false;
                Session["SJR2Grade1"] = false;
                Session["SJR2Grade2"] = false;
                Session["SJR2Grade3"] = false;
                Session["SJR1Grade0"] = false;
                Session["SJR1Grade1"] = false;
                Session["SJR1Grade2"] = false;
                Session["SJR1Grade3"] = false;
                Session["SJR1VeryPoor"] = false;
                Session["SJR1Poor"] = false;
                Session["SJR1Adequate"] = false;
                Session["SJR1Good"] = false;
                Session["SJR1Excellent"] = false;
                Session["SJR2VeryPoor"] = false;
                Session["SJR2Poor"] = false;
                Session["SJR2Adequate"] = false;
                Session["SJR2Good"] = false;
                Session["SJR2Excellent"] = false;
            }
            return RedirectToAction("PatientReportList", new { id = 0, qapreview = false, medtriage = false, meoreview = false });
        }

        public ActionResult MEReviewDeathExpectancy(int causeid)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportStartDate"])) || Convert.ToString(Session["ReportStartDate"]) == "01/01/0001")
                Session["ReportStartDate"] = System.DateTime.Now.AddDays(-30);
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportEndDate"])) || Convert.ToString(Session["ReportEndDate"]) == "01/01/0001")
                Session["ReportEndDate"] = System.DateTime.Now;
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportCaregroup"])))
                Session["ReportCaregroup"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportPatientType"])))
                Session["ReportPatientType"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportWardDeath"])))
                Session["ReportWardDeath"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeConsultant"])))
                Session["ReportDischargeConsultant"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeSpeciality"])))
                Session["ReportDischargeSpeciality"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportMEName"])))
                Session["ReportMEName"] = "";
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            patientDetails = dBEngine.GetPatientDetailsByExpectedDeath(DateTime.ParseExact(Convert.ToString(Session["ReportStartDate"]).Split(" ".ToCharArray())[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), DateTime.ParseExact(Convert.ToString(Session["ReportEndDate"]).Split(" ".ToCharArray())[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), Convert.ToString(Session["ReportCaregroup"]), Convert.ToString(Session["ReportPatientType"]), Convert.ToString(Session["ReportWardDeath"]), Convert.ToString(Session["ReportDischargeConsultant"]), Convert.ToString(Session["ReportDischargeSpeciality"]), Convert.ToString(Session["ReportMEName"]), causeid, Convert.ToInt32(Session["LoginUserID"]));
            Session["PatientDetails"] = patientDetails;
            Session["AdultDeath"] = false;
            Session["MEOutstanding"] = false;
            Session["A&EDeath"] = false;
            Session["MECompleted"] = false;
            Session["DeathExpectency"] = true;
            Session["CauseID"] = causeid;
            Session["MCCDIssue"] = false;
            Session["Referral"] = false;
            Session["PostMortem"] = false;
            Session["CareQuality"] = false;
            Session["RecommndedReferral"] = false;
            Session["PatientSIRI"] = false;
            Session["SafeGuard"] = false;
            Session["LearningDisability"] = false;
            Session["ChildDeath"] = false;
            Session["WardTeam"] = false;
            Session["HeadOfCompliance"] = false;
            Session["PALsComplaints"] = false;
            Session["Other"] = false;
            Session["SJRRequired"] = false;
            Session["ProblemOccured"] = false;
            Session["KeyLearnings"] = false;
            Session["SJR1Completed"] = false;
            Session["SJR1Outstanding"] = false;
            Session["SJR1Grade0"] = false;
            Session["SJR1Grade2"] = false;
            Session["SJR1Grade3"] = false;
            Session["MSGGrade0"] = false;
            Session["MSGGrade2"] = false;
            Session["MSG1Grade3"] = false;
            Session["CoronerDecisionNFAction"] = false;
            Session["CoronerDecisionPostMortem"] = false;
            Session["CoronerDecisionForensicPM"] = false;
            Session["CoronerDecisionGPIssue"] = false;
            Session["CoronerDecisionInquest"] = false;
            Session["CoronerDecision100A"] = false;
            Session["FeedbackTypeID"] = 0;
            Session["SJR2Completed"] = false;
            Session["SJR2Outstanding"] = false;
            Session["SJR2Required"] = false;
            Session["SJR2NotRequired"] = false;
            Session["SJR2Grade0"] = false;
            Session["SJR2Grade1"] = false;
            Session["SJR2Grade2"] = false;
            Session["SJR2Grade3"] = false;
            Session["SJR1Grade0"] = false;
            Session["SJR1Grade1"] = false;
            Session["SJR1Grade2"] = false;
            Session["SJR1Grade3"] = false;
            Session["SJR1VeryPoor"] = false;
            Session["SJR1Poor"] = false;
            Session["SJR1Adequate"] = false;
            Session["SJR1Good"] = false;
            Session["SJR1Excellent"] = false;
            Session["SJR2VeryPoor"] = false;
            Session["SJR2Poor"] = false;
            Session["SJR2Adequate"] = false;
            Session["SJR2Good"] = false;
            Session["SJR2Excellent"] = false;
            return RedirectToAction("PatientReportList", new { id = 0, qapreview = false, medtriage = false, meoreview = false });
        }

        public ActionResult ReviewOutcome(bool mccd, bool referral, bool postmortem)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportStartDate"])) || Convert.ToString(Session["ReportStartDate"]) == "01/01/0001")
                Session["ReportStartDate"] = System.DateTime.Now.AddDays(-30);
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportEndDate"])) || Convert.ToString(Session["ReportEndDate"]) == "01/01/0001")
                Session["ReportEndDate"] = System.DateTime.Now;
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportCaregroup"])))
                Session["ReportCaregroup"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportPatientType"])))
                Session["ReportPatientType"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportWardDeath"])))
                Session["ReportWardDeath"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeConsultant"])))
                Session["ReportDischargeConsultant"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeSpeciality"])))
                Session["ReportDischargeSpeciality"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportMEName"])))
                Session["ReportMEName"] = "";
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            patientDetails = dBEngine.GetPatientDetailsByReviewOutcome(DateTime.ParseExact(Convert.ToString(Session["ReportStartDate"]).Split(" ".ToCharArray())[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), DateTime.ParseExact(Convert.ToString(Session["ReportEndDate"]).Split(" ".ToCharArray())[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), Convert.ToString(Session["ReportCaregroup"]), Convert.ToString(Session["ReportPatientType"]), Convert.ToString(Session["ReportWardDeath"]), Convert.ToString(Session["ReportDischargeConsultant"]), Convert.ToString(Session["ReportDischargeSpeciality"]), Convert.ToString(Session["ReportMEName"]), mccd, referral, postmortem, Convert.ToInt32(Session["LoginUserID"]));
            Session["PatientDetails"] = patientDetails;
            Session["AdultDeath"] = false;
            Session["MEOutstanding"] = false;
            Session["A&EDeath"] = false;
            Session["MECompleted"] = false;
            Session["DeathExpectency"] = false;
            Session["QAPCompleted"] = false;
            Session["QAPOutstanding"] = false;
            Session["CauseID"] = 0;
            if (mccd)
                Session["MCCDIssue"] = mccd;
            else if (referral)
                Session["Referral"] = referral;
            else if (postmortem)
                Session["PostMortem"] = postmortem;
            Session["CareQuality"] = false;
            Session["RecommndedReferral"] = false;
            Session["PatientSIRI"] = false;
            Session["SafeGuard"] = false;
            Session["LearningDisability"] = false;
            Session["ChildDeath"] = false;
            Session["WardTeam"] = false;
            Session["HeadOfCompliance"] = false;
            Session["PALsComplaints"] = false;
            Session["Other"] = false;
            Session["SJRRequired"] = false;
            Session["ProblemOccured"] = false;
            Session["KeyLearnings"] = false;
            Session["SJR1Completed"] = false;
            Session["SJR1Outstanding"] = false;
            Session["SJR1Grade0"] = false;
            Session["SJR1Grade2"] = false;
            Session["SJR1Grade3"] = false;
            Session["MSGGrade0"] = false;
            Session["MSGGrade2"] = false;
            Session["MSG1Grade3"] = false;
            Session["CoronerDecisionNFAction"] = false;
            Session["CoronerDecisionPostMortem"] = false;
            Session["CoronerDecisionForensicPM"] = false;
            Session["CoronerDecisionGPIssue"] = false;
            Session["CoronerDecisionInquest"] = false;
            Session["CoronerDecision100A"] = false;
            Session["FeedbackTypeID"] = 0;
            Session["SJR2Completed"] = false;
            Session["SJR2Outstanding"] = false;
            Session["SJR2Required"] = false;
            Session["SJR2NotRequired"] = false;
            Session["SJR2Grade0"] = false;
            Session["SJR2Grade1"] = false;
            Session["SJR2Grade2"] = false;
            Session["SJR2Grade3"] = false;
            Session["SJR1Grade0"] = false;
            Session["SJR1Grade1"] = false;
            Session["SJR1Grade2"] = false;
            Session["SJR1Grade3"] = false;
            Session["SJR1VeryPoor"] = false;
            Session["SJR1Poor"] = false;
            Session["SJR1Adequate"] = false;
            Session["SJR1Good"] = false;
            Session["SJR1Excellent"] = false;
            Session["SJR2VeryPoor"] = false;
            Session["SJR2Poor"] = false;
            Session["SJR2Adequate"] = false;
            Session["SJR2Good"] = false;
            Session["SJR2Excellent"] = false;
            return RedirectToAction("PatientReportList", new { id = 0, qapreview = false, medtriage = false, meoreview = false });
        }
        public ActionResult CareQualityCount()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportStartDate"])) || Convert.ToString(Session["ReportStartDate"]) == "01/01/0001")
                Session["ReportStartDate"] = System.DateTime.Now.AddDays(-30);
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportEndDate"])) || Convert.ToString(Session["ReportEndDate"]) == "01/01/0001")
                Session["ReportEndDate"] = System.DateTime.Now;
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportCaregroup"])))
                Session["ReportCaregroup"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportPatientType"])))
                Session["ReportPatientType"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportWardDeath"])))
                Session["ReportWardDeath"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeConsultant"])))
                Session["ReportDischargeConsultant"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeSpeciality"])))
                Session["ReportDischargeSpeciality"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportMEName"])))
                Session["ReportMEName"] = "";
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            patientDetails = dBEngine.GetPatientDetailsByCareQuality(DateTime.ParseExact(Convert.ToString(Session["ReportStartDate"]).Split(" ".ToCharArray())[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), DateTime.ParseExact(Convert.ToString(Session["ReportEndDate"]).Split(" ".ToCharArray())[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), Convert.ToString(Session["ReportCaregroup"]), Convert.ToString(Session["ReportPatientType"]), Convert.ToString(Session["ReportWardDeath"]), Convert.ToString(Session["ReportDischargeConsultant"]), Convert.ToString(Session["ReportDischargeSpeciality"]), Convert.ToString(Session["ReportMEName"]), Convert.ToInt32(Session["LoginUserID"]));
            Session["PatientDetails"] = patientDetails;
            Session["AdultDeath"] = false;
            Session["MEOutstanding"] = false;
            Session["A&EDeath"] = false;
            Session["MECompleted"] = false;
            Session["DeathExpectency"] = false;
            Session["QAPCompleted"] = false;
            Session["QAPOutstanding"] = false;
            Session["CauseID"] = 0;
            Session["MCCDIssue"] = false;
            Session["Referral"] = false;
            Session["PostMortem"] = false;
            Session["CareQuality"] = true;
            Session["RecommndedReferral"] = false;
            Session["PatientSIRI"] = false;
            Session["SafeGuard"] = false;
            Session["LearningDisability"] = false;
            Session["ChildDeath"] = false;
            Session["WardTeam"] = false;
            Session["HeadOfCompliance"] = false;
            Session["PALsComplaints"] = false;
            Session["Other"] = false;
            Session["SJRRequired"] = false;
            Session["ProblemOccured"] = false;
            Session["KeyLearnings"] = false;
            Session["SJR1Completed"] = false;
            Session["SJR1Outstanding"] = false;
            Session["SJR1Grade0"] = false;
            Session["SJR1Grade2"] = false;
            Session["SJR1Grade3"] = false;
            Session["MSGGrade0"] = false;
            Session["MSGGrade2"] = false;
            Session["MSG1Grade3"] = false;
            Session["CoronerDecisionNFAction"] = false;
            Session["CoronerDecisionPostMortem"] = false;
            Session["CoronerDecisionForensicPM"] = false;
            Session["CoronerDecisionGPIssue"] = false;
            Session["CoronerDecisionInquest"] = false;
            Session["CoronerDecision100A"] = false;
            Session["FeedbackTypeID"] = 0;
            Session["SJR2Completed"] = false;
            Session["SJR2Outstanding"] = false;
            Session["SJR2Required"] = false;
            Session["SJR2NotRequired"] = false;
            Session["SJR2Grade0"] = false;
            Session["SJR2Grade1"] = false;
            Session["SJR2Grade2"] = false;
            Session["SJR2Grade3"] = false;
            Session["SJR1Grade0"] = false;
            Session["SJR1Grade1"] = false;
            Session["SJR1Grade2"] = false;
            Session["SJR1Grade3"] = false;
            Session["SJR1VeryPoor"] = false;
            Session["SJR1Poor"] = false;
            Session["SJR1Adequate"] = false;
            Session["SJR1Good"] = false;
            Session["SJR1Excellent"] = false;
            Session["SJR2VeryPoor"] = false;
            Session["SJR2Poor"] = false;
            Session["SJR2Adequate"] = false;
            Session["SJR2Good"] = false;
            Session["SJR2Excellent"] = false;
            return RedirectToAction("PatientReportList", new { id = 0, qapreview = false, medtriage = false, meoreview = false });
        }

        public ActionResult RecommendedReferralCount()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportStartDate"])) || Convert.ToString(Session["ReportStartDate"]) == "01/01/0001")
                Session["ReportStartDate"] = System.DateTime.Now.AddDays(-30);
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportEndDate"])) || Convert.ToString(Session["ReportEndDate"]) == "01/01/0001")
                Session["ReportEndDate"] = System.DateTime.Now;
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportCaregroup"])))
                Session["ReportCaregroup"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportPatientType"])))
                Session["ReportPatientType"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportWardDeath"])))
                Session["ReportWardDeath"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeConsultant"])))
                Session["ReportDischargeConsultant"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeSpeciality"])))
                Session["ReportDischargeSpeciality"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportMEName"])))
                Session["ReportMEName"] = "";
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            patientDetails = dBEngine.GetPatientDetailsByRecommendedReferral(DateTime.ParseExact(Convert.ToString(Session["ReportStartDate"]).Split(" ".ToCharArray())[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), DateTime.ParseExact(Convert.ToString(Session["ReportEndDate"]).Split(" ".ToCharArray())[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), Convert.ToString(Session["ReportCaregroup"]), Convert.ToString(Session["ReportPatientType"]), Convert.ToString(Session["ReportWardDeath"]), Convert.ToString(Session["ReportDischargeConsultant"]), Convert.ToString(Session["ReportDischargeSpeciality"]), Convert.ToString(Session["ReportMEName"]), Convert.ToInt32(Session["LoginUserID"]));
            Session["PatientDetails"] = patientDetails;
            Session["AdultDeath"] = false;
            Session["MEOutstanding"] = false;
            Session["A&EDeath"] = false;
            Session["MECompleted"] = false;
            Session["DeathExpectency"] = false;
            Session["QAPCompleted"] = false;
            Session["QAPOutstanding"] = false;
            Session["CauseID"] = 0;
            Session["MCCDIssue"] = false;
            Session["Referral"] = false;
            Session["PostMortem"] = false;
            Session["CareQuality"] = false;
            Session["RecommndedReferral"] = true;
            Session["PatientSIRI"] = false;
            Session["SafeGuard"] = false;
            Session["LearningDisability"] = false;
            Session["ChildDeath"] = false;
            Session["WardTeam"] = false;
            Session["HeadOfCompliance"] = false;
            Session["PALsComplaints"] = false;
            Session["Other"] = false;
            Session["SJRRequired"] = false;
            Session["ProblemOccured"] = false;
            Session["KeyLearnings"] = false;
            Session["SJR1Completed"] = false;
            Session["SJR1Outstanding"] = false;
            Session["SJR1Grade0"] = false;
            Session["SJR1Grade2"] = false;
            Session["SJR1Grade3"] = false;
            Session["MSGGrade0"] = false;
            Session["MSGGrade2"] = false;
            Session["MSG1Grade3"] = false;
            Session["CoronerDecisionNFAction"] = false;
            Session["CoronerDecisionPostMortem"] = false;
            Session["CoronerDecisionForensicPM"] = false;
            Session["CoronerDecisionGPIssue"] = false;
            Session["CoronerDecisionInquest"] = false;
            Session["CoronerDecision100A"] = false;
            Session["FeedbackTypeID"] = 0;
            Session["SJR2Completed"] = false;
            Session["SJR2Outstanding"] = false;
            Session["SJR2Required"] = false;
            Session["SJR2NotRequired"] = false;
            Session["SJR2Grade0"] = false;
            Session["SJR2Grade1"] = false;
            Session["SJR2Grade2"] = false;
            Session["SJR2Grade3"] = false;
            Session["SJR1Grade0"] = false;
            Session["SJR1Grade1"] = false;
            Session["SJR1Grade2"] = false;
            Session["SJR1Grade3"] = false;
            Session["SJR1VeryPoor"] = false;
            Session["SJR1Poor"] = false;
            Session["SJR1Adequate"] = false;
            Session["SJR1Good"] = false;
            Session["SJR1Excellent"] = false;
            Session["SJR2VeryPoor"] = false;
            Session["SJR2Poor"] = false;
            Session["SJR2Adequate"] = false;
            Session["SJR2Good"] = false;
            Session["SJR2Excellent"] = false;
            return RedirectToAction("PatientReportList", new { id = 0, qapreview = false, medtriage = false, meoreview = false });
        }

        public ActionResult SJRRequiredCount(bool required)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportStartDate"])) || Convert.ToString(Session["ReportStartDate"]) == "01/01/0001")
                Session["ReportStartDate"] = System.DateTime.Now.AddDays(-30);
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportEndDate"])) || Convert.ToString(Session["ReportEndDate"]) == "01/01/0001")
                Session["ReportEndDate"] = System.DateTime.Now;
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportCaregroup"])))
                Session["ReportCaregroup"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportPatientType"])))
                Session["ReportPatientType"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportWardDeath"])))
                Session["ReportWardDeath"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeConsultant"])))
                Session["ReportDischargeConsultant"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeSpeciality"])))
                Session["ReportDischargeSpeciality"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportMEName"])))
                Session["ReportMEName"] = "";
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            patientDetails = dBEngine.GetPatientDetailsBySJRRequired(DateTime.ParseExact(Convert.ToString(Session["ReportStartDate"]).Split(" ".ToCharArray())[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), DateTime.ParseExact(Convert.ToString(Session["ReportEndDate"]).Split(" ".ToCharArray())[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), Convert.ToString(Session["ReportCaregroup"]), Convert.ToString(Session["ReportPatientType"]), Convert.ToString(Session["ReportWardDeath"]), Convert.ToString(Session["ReportDischargeConsultant"]), Convert.ToString(Session["ReportDischargeSpeciality"]), Convert.ToString(Session["ReportMEName"]), required, Convert.ToInt32(Session["LoginUserID"]));
            Session["PatientDetails"] = patientDetails;
            Session["AdultDeath"] = false;
            Session["MEOutstanding"] = false;
            Session["A&EDeath"] = false;
            Session["MECompleted"] = false;
            Session["DeathExpectency"] = false;
            Session["QAPCompleted"] = false;
            Session["QAPOutstanding"] = false;
            Session["CauseID"] = 0;
            Session["MCCDIssue"] = false;
            Session["Referral"] = false;
            Session["PostMortem"] = false;
            Session["CareQuality"] = false;
            Session["RecommndedReferral"] = false;
            Session["PatientSIRI"] = false;
            Session["SafeGuard"] = false;
            Session["LearningDisability"] = false;
            Session["ChildDeath"] = false;
            Session["WardTeam"] = false;
            Session["HeadOfCompliance"] = false;
            Session["PALsComplaints"] = false;
            Session["Other"] = false;
            if(required)
                Session["SJRRequired"] = true;
            else
                Session["SJRNotRequired"] = true;
            Session["ProblemOccured"] = false;
            Session["KeyLearnings"] = false;
            Session["SJR1Completed"] = false;
            Session["SJR1Outstanding"] = false;
            Session["SJR1Grade0"] = false;
            Session["SJR1Grade2"] = false;
            Session["SJR1Grade3"] = false;
            Session["MSGGrade0"] = false;
            Session["MSGGrade2"] = false;
            Session["MSG1Grade3"] = false;
            Session["CoronerDecisionNFAction"] = false;
            Session["CoronerDecisionPostMortem"] = false;
            Session["CoronerDecisionForensicPM"] = false;
            Session["CoronerDecisionGPIssue"] = false;
            Session["CoronerDecisionInquest"] = false;
            Session["CoronerDecision100A"] = false;
            Session["FeedbackTypeID"] = 0;
            Session["SJR2Completed"] = false;
            Session["SJR2Outstanding"] = false;
            Session["SJR2Required"] = false;
            Session["SJR2NotRequired"] = false;
            Session["SJR2Grade0"] = false;
            Session["SJR2Grade1"] = false;
            Session["SJR2Grade2"] = false;
            Session["SJR2Grade3"] = false;
            Session["SJR1Grade0"] = false;
            Session["SJR1Grade1"] = false;
            Session["SJR1Grade2"] = false;
            Session["SJR1Grade3"] = false;
            Session["SJR1VeryPoor"] = false;
            Session["SJR1Poor"] = false;
            Session["SJR1Adequate"] = false;
            Session["SJR1Good"] = false;
            Session["SJR1Excellent"] = false;
            Session["SJR2VeryPoor"] = false;
            Session["SJR2Poor"] = false;
            Session["SJR2Adequate"] = false;
            Session["SJR2Good"] = false;
            Session["SJR2Excellent"] = false;
            return RedirectToAction("PatientReportList", new { id = 0, qapreview = false, medtriage = false, meoreview = false });
        }

        public ActionResult SJR2RequiredCount(bool required)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportStartDate"])) || Convert.ToString(Session["ReportStartDate"]) == "01/01/0001")
                Session["ReportStartDate"] = System.DateTime.Now.AddDays(-30);
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportEndDate"])) || Convert.ToString(Session["ReportEndDate"]) == "01/01/0001")
                Session["ReportEndDate"] = System.DateTime.Now;
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportCaregroup"])))
                Session["ReportCaregroup"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportPatientType"])))
                Session["ReportPatientType"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportWardDeath"])))
                Session["ReportWardDeath"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeConsultant"])))
                Session["ReportDischargeConsultant"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeSpeciality"])))
                Session["ReportDischargeSpeciality"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportMEName"])))
                Session["ReportMEName"] = "";
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            patientDetails = dBEngine.GetPatientDetailsBySJR2Required(DateTime.ParseExact(Convert.ToString(Session["ReportStartDate"]).Split(" ".ToCharArray())[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), DateTime.ParseExact(Convert.ToString(Session["ReportEndDate"]).Split(" ".ToCharArray())[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), Convert.ToString(Session["ReportCaregroup"]), Convert.ToString(Session["ReportPatientType"]), Convert.ToString(Session["ReportWardDeath"]), Convert.ToString(Session["ReportDischargeConsultant"]), Convert.ToString(Session["ReportDischargeSpeciality"]), Convert.ToString(Session["ReportMEName"]), required, Convert.ToInt32(Session["LoginUserID"]));
            Session["PatientDetails"] = patientDetails;
            Session["AdultDeath"] = false;
            Session["MEOutstanding"] = false;
            Session["A&EDeath"] = false;
            Session["MECompleted"] = false;
            Session["DeathExpectency"] = false;
            Session["QAPCompleted"] = false;
            Session["QAPOutstanding"] = false;
            Session["CauseID"] = 0;
            Session["MCCDIssue"] = false;
            Session["Referral"] = false;
            Session["PostMortem"] = false;
            Session["CareQuality"] = false;
            Session["RecommndedReferral"] = false;
            Session["PatientSIRI"] = false;
            Session["SafeGuard"] = false;
            Session["LearningDisability"] = false;
            Session["ChildDeath"] = false;
            Session["WardTeam"] = false;
            Session["HeadOfCompliance"] = false;
            Session["PALsComplaints"] = false;
            Session["Other"] = false;
            Session["SJRRequired"] = false;
            Session["SJRNotRequired"] = false;
            if (required)
                Session["SJR2Required"] = true;
            else
                Session["SJR2NotRequired"] = true;
            Session["ProblemOccured"] = false;
            Session["KeyLearnings"] = false;
            Session["SJR1Completed"] = false;
            Session["SJR1Outstanding"] = false;
            Session["SJR1Grade0"] = false;
            Session["SJR1Grade2"] = false;
            Session["SJR1Grade3"] = false;
            Session["MSGGrade0"] = false;
            Session["MSGGrade2"] = false;
            Session["MSG1Grade3"] = false;
            Session["CoronerDecisionNFAction"] = false;
            Session["CoronerDecisionPostMortem"] = false;
            Session["CoronerDecisionForensicPM"] = false;
            Session["CoronerDecisionGPIssue"] = false;
            Session["CoronerDecisionInquest"] = false;
            Session["CoronerDecision100A"] = false;
            Session["FeedbackTypeID"] = 0;
            Session["SJR2Completed"] = false;
            Session["SJR2Outstanding"] = false;
            Session["SJR2Grade0"] = false;
            Session["SJR2Grade1"] = false;
            Session["SJR2Grade2"] = false;
            Session["SJR2Grade3"] = false;
            Session["SJR1Grade0"] = false;
            Session["SJR1Grade1"] = false;
            Session["SJR1Grade2"] = false;
            Session["SJR1Grade3"] = false;
            Session["SJR1VeryPoor"] = false;
            Session["SJR1Poor"] = false;
            Session["SJR1Adequate"] = false;
            Session["SJR1Good"] = false;
            Session["SJR1Excellent"] = false;
            Session["SJR2VeryPoor"] = false;
            Session["SJR2Poor"] = false;
            Session["SJR2Adequate"] = false;
            Session["SJR2Good"] = false;
            Session["SJR2Excellent"] = false;
            return RedirectToAction("PatientReportList", new { id = 0, qapreview = false, medtriage = false, meoreview = false });
        }

        public ActionResult OtherReferralCount(bool patientsiri, bool safeguard, bool learningdisability, bool childdeath, bool wardteam, bool headcompliance, bool palscomplaints, bool other)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportStartDate"])) || Convert.ToString(Session["ReportStartDate"]) == "01/01/0001")
                Session["ReportStartDate"] = System.DateTime.Now.AddDays(-30);
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportEndDate"])) || Convert.ToString(Session["ReportEndDate"]) == "01/01/0001")
                Session["ReportEndDate"] = System.DateTime.Now;
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportCaregroup"])))
                Session["ReportCaregroup"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportPatientType"])))
                Session["ReportPatientType"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportWardDeath"])))
                Session["ReportWardDeath"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeConsultant"])))
                Session["ReportDischargeConsultant"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeSpeciality"])))
                Session["ReportDischargeSpeciality"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportMEName"])))
                Session["ReportMEName"] = "";
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            patientDetails = dBEngine.GetPatientDetailsByOtherReferral(DateTime.ParseExact(Convert.ToString(Session["ReportStartDate"]).Split(" ".ToCharArray())[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), DateTime.ParseExact(Convert.ToString(Session["ReportEndDate"]).Split(" ".ToCharArray())[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), Convert.ToString(Session["ReportCaregroup"]), Convert.ToString(Session["ReportPatientType"]), Convert.ToString(Session["ReportWardDeath"]), Convert.ToString(Session["ReportDischargeConsultant"]), Convert.ToString(Session["ReportDischargeSpeciality"]), Convert.ToString(Session["ReportMEName"]),
                patientsiri, safeguard, learningdisability, childdeath, wardteam, headcompliance, palscomplaints, other, Convert.ToInt32(Session["LoginUserID"]));
            Session["PatientDetails"] = patientDetails;
            Session["AdultDeath"] = false;
            Session["MEOutstanding"] = false;
            Session["A&EDeath"] = false;
            Session["MECompleted"] = false;
            Session["DeathExpectency"] = true;
            Session["QAPCompleted"] = false;
            Session["QAPOutstanding"] = false;
            Session["CauseID"] = 0;
            Session["MCCDIssue"] = false;
            Session["Referral"] = false;
            Session["PostMortem"] = false;
            Session["CareQuality"] = false;
            Session["RecommndedReferral"] = false;
            Session["OtherReferral"] = true;
            Session["SJRRequired"] = false;
            if (patientsiri)
                Session["PatientSIRI"] = patientsiri;
            else if (safeguard)
                Session["SafeGuard"] = safeguard;
            else if (learningdisability)
                Session["LearningDisability"] = learningdisability;
            else if (childdeath)
                Session["ChildDeath"] = childdeath;
            else if (wardteam)
                Session["WardTeam"] = wardteam;
            else if (headcompliance)
                Session["HeadOfCompliance"] = headcompliance;
            else if (palscomplaints)
                Session["PALsComplaints"] = palscomplaints;
            else if (other)
                Session["Other"] = other;
            Session["ProblemOccured"] = false;
            Session["KeyLearnings"] = false;
            Session["SJR1Completed"] = false;
            Session["SJR1Outstanding"] = false;
            Session["SJR1Grade0"] = false;
            Session["SJR1Grade2"] = false;
            Session["SJR1Grade3"] = false;
            Session["MSGGrade0"] = false;
            Session["MSGGrade2"] = false;
            Session["MSG1Grade3"] = false;
            Session["CoronerDecisionNFAction"] = false;
            Session["CoronerDecisionPostMortem"] = false;
            Session["CoronerDecisionForensicPM"] = false;
            Session["CoronerDecisionGPIssue"] = false;
            Session["CoronerDecisionInquest"] = false;
            Session["CoronerDecision100A"] = false;
            Session["FeedbackTypeID"] = 0;
            Session["SJR2Completed"] = false;
            Session["SJR2Outstanding"] = false;
            Session["SJR2Grade0"] = false;
            Session["SJR2Grade1"] = false;
            Session["SJR2Grade2"] = false;
            Session["SJR2Grade3"] = false;
            Session["SJR1Grade0"] = false;
            Session["SJR1Grade1"] = false;
            Session["SJR1Grade2"] = false;
            Session["SJR1Grade3"] = false;
            Session["SJR1VeryPoor"] = false;
            Session["SJR1Poor"] = false;
            Session["SJR1Adequate"] = false;
            Session["SJR1Good"] = false;
            Session["SJR1Excellent"] = false;
            Session["SJR2VeryPoor"] = false;
            Session["SJR2Poor"] = false;
            Session["SJR2Adequate"] = false;
            Session["SJR2Good"] = false;
            Session["SJR2Excellent"] = false;
            return RedirectToAction("PatientReportList", new { id = 0, qapreview = false, medtriage = false, meoreview = false });
        }

        public ActionResult ProblemOccuredCount()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportStartDate"])) || Convert.ToString(Session["ReportStartDate"]) == "01/01/0001")
                Session["ReportStartDate"] = System.DateTime.Now.AddDays(-30);
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportEndDate"])) || Convert.ToString(Session["ReportEndDate"]) == "01/01/0001")
                Session["ReportEndDate"] = System.DateTime.Now;
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportCaregroup"])))
                Session["ReportCaregroup"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportPatientType"])))
                Session["ReportPatientType"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportWardDeath"])))
                Session["ReportWardDeath"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeConsultant"])))
                Session["ReportDischargeConsultant"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeSpeciality"])))
                Session["ReportDischargeSpeciality"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportMEName"])))
                Session["ReportMEName"] = "";
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            patientDetails = dBEngine.GetPatientDetailsByProblemOccured(DateTime.ParseExact(Convert.ToString(Session["ReportStartDate"]).Split(" ".ToCharArray())[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), DateTime.ParseExact(Convert.ToString(Session["ReportEndDate"]).Split(" ".ToCharArray())[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), Convert.ToString(Session["ReportCaregroup"]), Convert.ToString(Session["ReportPatientType"]), Convert.ToString(Session["ReportWardDeath"]), Convert.ToString(Session["ReportDischargeConsultant"]), Convert.ToString(Session["ReportDischargeSpeciality"]), Convert.ToString(Session["ReportMEName"]), Convert.ToInt32(Session["LoginUserID"]));
            Session["PatientDetails"] = patientDetails;
            Session["AdultDeath"] = false;
            Session["MEOutstanding"] = false;
            Session["A&EDeath"] = false;
            Session["MECompleted"] = false;
            Session["DeathExpectency"] = true;
            Session["QAPCompleted"] = false;
            Session["QAPOutstanding"] = false;
            Session["CauseID"] = 0;
            Session["MCCDIssue"] = false;
            Session["Referral"] = false;
            Session["PostMortem"] = false;
            Session["CareQuality"] = false;
            Session["RecommndedReferral"] = false;
            Session["OtherReferral"] = true;
            Session["SJRRequired"] = false;
            Session["PatientSIRI"] = false;
            Session["SafeGuard"] = false;
            Session["LearningDisability"] = false;
            Session["ChildDeath"] = false;
            Session["WardTeam"] = false;
            Session["HeadOfCompliance"] = false;
            Session["PALsComplaints"] = false;
            Session["Other"] = false;
            Session["ProblemOccured"] = true;
            Session["KeyLearnings"] = false;
            Session["SJR1Completed"] = false;
            Session["SJR1Outstanding"] = false;
            Session["SJR1Grade0"] = false;
            Session["SJR1Grade2"] = false;
            Session["SJR1Grade3"] = false;
            Session["MSGGrade0"] = false;
            Session["MSGGrade2"] = false;
            Session["MSG1Grade3"] = false;
            Session["CoronerDecisionNFAction"] = false;
            Session["CoronerDecisionPostMortem"] = false;
            Session["CoronerDecisionForensicPM"] = false;
            Session["CoronerDecisionGPIssue"] = false;
            Session["CoronerDecisionInquest"] = false;
            Session["CoronerDecision100A"] = false;
            Session["FeedbackTypeID"] = 0;
            Session["SJR2Completed"] = false;
            Session["SJR2Outstanding"] = false;
            Session["SJR2Grade0"] = false;
            Session["SJR2Grade1"] = false;
            Session["SJR2Grade2"] = false;
            Session["SJR2Grade3"] = false;
            Session["SJR1Grade0"] = false;
            Session["SJR1Grade1"] = false;
            Session["SJR1Grade2"] = false;
            Session["SJR1Grade3"] = false;
            Session["SJR1VeryPoor"] = false;
            Session["SJR1Poor"] = false;
            Session["SJR1Adequate"] = false;
            Session["SJR1Good"] = false;
            Session["SJR1Excellent"] = false;
            Session["SJR2VeryPoor"] = false;
            Session["SJR2Poor"] = false;
            Session["SJR2Adequate"] = false;
            Session["SJR2Good"] = false;
            Session["SJR2Excellent"] = false;
            return RedirectToAction("PatientReportList", new { id = 0, qapreview = false, medtriage = false, meoreview = false });
        }

        public ActionResult KeyLearningsCount()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportStartDate"])) || Convert.ToString(Session["ReportStartDate"]) == "01/01/0001")
                Session["ReportStartDate"] = System.DateTime.Now.AddDays(-30);
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportEndDate"])) || Convert.ToString(Session["ReportEndDate"]) == "01/01/0001")
                Session["ReportEndDate"] = System.DateTime.Now;
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportCaregroup"])))
                Session["ReportCaregroup"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportPatientType"])))
                Session["ReportPatientType"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportWardDeath"])))
                Session["ReportWardDeath"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeConsultant"])))
                Session["ReportDischargeConsultant"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeSpeciality"])))
                Session["ReportDischargeSpeciality"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportMEName"])))
                Session["ReportMEName"] = "";
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            patientDetails = dBEngine.GetPatientDetailsByKeyLearnings(DateTime.ParseExact(Convert.ToString(Session["ReportStartDate"]).Split(" ".ToCharArray())[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), DateTime.ParseExact(Convert.ToString(Session["ReportEndDate"]).Split(" ".ToCharArray())[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), Convert.ToString(Session["ReportCaregroup"]), Convert.ToString(Session["ReportPatientType"]), Convert.ToString(Session["ReportWardDeath"]), Convert.ToString(Session["ReportDischargeConsultant"]), Convert.ToString(Session["ReportDischargeSpeciality"]), Convert.ToString(Session["ReportMEName"]), Convert.ToInt32(Session["LoginUserID"]));
            Session["PatientDetails"] = patientDetails;
            Session["AdultDeath"] = false;
            Session["MEOutstanding"] = false;
            Session["A&EDeath"] = false;
            Session["MECompleted"] = false;
            Session["DeathExpectency"] = true;
            Session["QAPCompleted"] = false;
            Session["QAPOutstanding"] = false;
            Session["CauseID"] = 0;
            Session["MCCDIssue"] = false;
            Session["Referral"] = false;
            Session["PostMortem"] = false;
            Session["CareQuality"] = false;
            Session["RecommndedReferral"] = false;
            Session["OtherReferral"] = true;
            Session["SJRRequired"] = false;
            Session["PatientSIRI"] = false;
            Session["SafeGuard"] = false;
            Session["LearningDisability"] = false;
            Session["ChildDeath"] = false;
            Session["WardTeam"] = false;
            Session["HeadOfCompliance"] = false;
            Session["PALsComplaints"] = false;
            Session["Other"] = false;
            Session["ProblemOccured"] = false;
            Session["KeyLearnings"] = true;
            Session["SJR1Completed"] = false;
            Session["SJR1Outstanding"] = false;
            Session["SJR1Grade0"] = false;
            Session["SJR1Grade2"] = false;
            Session["SJR1Grade3"] = false;
            Session["MSGGrade0"] = false;
            Session["MSGGrade2"] = false;
            Session["MSG1Grade3"] = false;
            Session["CoronerDecisionNFAction"] = false;
            Session["CoronerDecisionPostMortem"] = false;
            Session["CoronerDecisionForensicPM"] = false;
            Session["CoronerDecisionGPIssue"] = false;
            Session["CoronerDecisionInquest"] = false;
            Session["CoronerDecision100A"] = false;
            Session["FeedbackTypeID"] = 0;
            Session["SJR2Completed"] = false;
            Session["SJR2Outstanding"] = false;
            Session["SJR2Grade0"] = false;
            Session["SJR2Grade1"] = false;
            Session["SJR2Grade2"] = false;
            Session["SJR2Grade3"] = false;
            Session["SJR1Grade0"] = false;
            Session["SJR1Grade1"] = false;
            Session["SJR1Grade2"] = false;
            Session["SJR1Grade3"] = false;
            Session["SJR1VeryPoor"] = false;
            Session["SJR1Poor"] = false;
            Session["SJR1Adequate"] = false;
            Session["SJR1Good"] = false;
            Session["SJR1Excellent"] = false;
            Session["SJR2VeryPoor"] = false;
            Session["SJR2Poor"] = false;
            Session["SJR2Adequate"] = false;
            Session["SJR2Good"] = false;
            Session["SJR2Excellent"] = false;
            return RedirectToAction("PatientReportList", new { id = 0, qapreview = false, medtriage = false, meoreview = false });
        }

        public ActionResult SJR1CompletedCount(bool completed)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportStartDate"])) || Convert.ToString(Session["ReportStartDate"]) == "01/01/0001")
                Session["ReportStartDate"] = System.DateTime.Now.AddDays(-30);
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportEndDate"])) || Convert.ToString(Session["ReportEndDate"]) == "01/01/0001")
                Session["ReportEndDate"] = System.DateTime.Now;
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportCaregroup"])))
                Session["ReportCaregroup"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportPatientType"])))
                Session["ReportPatientType"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportWardDeath"])))
                Session["ReportWardDeath"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeConsultant"])))
                Session["ReportDischargeConsultant"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeSpeciality"])))
                Session["ReportDischargeSpeciality"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportMEName"])))
                Session["ReportMEName"] = "";
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            patientDetails = dBEngine.GetPatientDetailsBySJR(DateTime.ParseExact(Convert.ToString(Session["ReportStartDate"]).Split(" ".ToCharArray())[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), DateTime.ParseExact(Convert.ToString(Session["ReportEndDate"]).Split(" ".ToCharArray())[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), Convert.ToString(Session["ReportCaregroup"]), Convert.ToString(Session["ReportPatientType"]), Convert.ToString(Session["ReportWardDeath"]), Convert.ToString(Session["ReportDischargeConsultant"]), Convert.ToString(Session["ReportDischargeSpeciality"]), Convert.ToString(Session["ReportMEName"]), completed, Convert.ToInt32(Session["LoginUserID"]));
            Session["PatientDetails"] = patientDetails;
            Session["AdultDeath"] = false;
            Session["MEOutstanding"] = false;
            Session["A&EDeath"] = false;
            Session["MECompleted"] = false;
            Session["DeathExpectency"] = true;
            Session["QAPCompleted"] = false;
            Session["QAPOutstanding"] = false;
            Session["CauseID"] = 0;
            Session["MCCDIssue"] = false;
            Session["Referral"] = false;
            Session["PostMortem"] = false;
            Session["CareQuality"] = false;
            Session["RecommndedReferral"] = false;
            Session["OtherReferral"] = true;
            Session["SJRRequired"] = false;
            Session["PatientSIRI"] = false;
            Session["SafeGuard"] = false;
            Session["LearningDisability"] = false;
            Session["ChildDeath"] = false;
            Session["WardTeam"] = false;
            Session["HeadOfCompliance"] = false;
            Session["PALsComplaints"] = false;
            Session["Other"] = false;
            Session["ProblemOccured"] = false;
            Session["KeyLearnings"] = false;
            if (completed)
                Session["SJR1Completed"] = true;
            else
                Session["SJR1Outstanding"] = true;
            Session["SJR1Grade0"] = false;
            Session["SJR1Grade2"] = false;
            Session["SJR1Grade3"] = false;
            Session["MSGGrade0"] = false;
            Session["MSGGrade2"] = false;
            Session["MSG1Grade3"] = false;
            Session["CoronerDecisionNFAction"] = false;
            Session["CoronerDecisionPostMortem"] = false;
            Session["CoronerDecisionForensicPM"] = false;
            Session["CoronerDecisionGPIssue"] = false;
            Session["CoronerDecisionInquest"] = false;
            Session["CoronerDecision100A"] = false;
            Session["FeedbackTypeID"] = 0;
            Session["SJR2Completed"] = false;
            Session["SJR2Outstanding"] = false;
            Session["SJR2Grade0"] = false;
            Session["SJR2Grade1"] = false;
            Session["SJR2Grade2"] = false;
            Session["SJR2Grade3"] = false;
            Session["SJR1Grade0"] = false;
            Session["SJR1Grade1"] = false;
            Session["SJR1Grade2"] = false;
            Session["SJR1Grade3"] = false;
            Session["SJR1VeryPoor"] = false;
            Session["SJR1Poor"] = false;
            Session["SJR1Adequate"] = false;
            Session["SJR1Good"] = false;
            Session["SJR1Excellent"] = false;
            Session["SJR2VeryPoor"] = false;
            Session["SJR2Poor"] = false;
            Session["SJR2Adequate"] = false;
            Session["SJR2Good"] = false;
            Session["SJR2Excellent"] = false;
            return RedirectToAction("PatientReportList", new { id = 0, qapreview = false, medtriage = false, meoreview = false });
        }

        public ActionResult SJR2CompletedCount(bool completed)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportStartDate"])) || Convert.ToString(Session["ReportStartDate"]) == "01/01/0001")
                Session["ReportStartDate"] = System.DateTime.Now.AddDays(-30);
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportEndDate"])) || Convert.ToString(Session["ReportEndDate"]) == "01/01/0001")
                Session["ReportEndDate"] = System.DateTime.Now;
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportCaregroup"])))
                Session["ReportCaregroup"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportPatientType"])))
                Session["ReportPatientType"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportWardDeath"])))
                Session["ReportWardDeath"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeConsultant"])))
                Session["ReportDischargeConsultant"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeSpeciality"])))
                Session["ReportDischargeSpeciality"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportMEName"])))
                Session["ReportMEName"] = "";
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            patientDetails = dBEngine.GetPatientDetailsBySJR2(DateTime.ParseExact(Convert.ToString(Session["ReportStartDate"]).Split(" ".ToCharArray())[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), DateTime.ParseExact(Convert.ToString(Session["ReportEndDate"]).Split(" ".ToCharArray())[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), Convert.ToString(Session["ReportCaregroup"]), Convert.ToString(Session["ReportPatientType"]), Convert.ToString(Session["ReportWardDeath"]), Convert.ToString(Session["ReportDischargeConsultant"]), Convert.ToString(Session["ReportDischargeSpeciality"]), Convert.ToString(Session["ReportMEName"]), completed, Convert.ToInt32(Session["LoginUserID"]));
            Session["PatientDetails"] = patientDetails;
            Session["AdultDeath"] = false;
            Session["MEOutstanding"] = false;
            Session["A&EDeath"] = false;
            Session["MECompleted"] = false;
            Session["DeathExpectency"] = true;
            Session["QAPCompleted"] = false;
            Session["QAPOutstanding"] = false;
            Session["CauseID"] = 0;
            Session["MCCDIssue"] = false;
            Session["Referral"] = false;
            Session["PostMortem"] = false;
            Session["CareQuality"] = false;
            Session["RecommndedReferral"] = false;
            Session["OtherReferral"] = true;
            Session["SJRRequired"] = false;
            Session["PatientSIRI"] = false;
            Session["SafeGuard"] = false;
            Session["LearningDisability"] = false;
            Session["ChildDeath"] = false;
            Session["WardTeam"] = false;
            Session["HeadOfCompliance"] = false;
            Session["PALsComplaints"] = false;
            Session["Other"] = false;
            Session["ProblemOccured"] = false;
            Session["KeyLearnings"] = false;
            Session["SJR1Completed"] = false;
            Session["SJR1Outstanding"] = false;
            Session["SJR1Grade0"] = false;
            Session["SJR1Grade2"] = false;
            Session["SJR1Grade3"] = false;
            Session["MSGGrade0"] = false;
            Session["MSGGrade2"] = false;
            Session["MSG1Grade3"] = false;
            Session["CoronerDecisionNFAction"] = false;
            Session["CoronerDecisionPostMortem"] = false;
            Session["CoronerDecisionForensicPM"] = false;
            Session["CoronerDecisionGPIssue"] = false;
            Session["CoronerDecisionInquest"] = false;
            Session["CoronerDecision100A"] = false;
            Session["FeedbackTypeID"] = 0;
            if (completed)
                Session["SJR2Completed"] = true;
            else
                Session["SJR2Outstanding"] = true;
            Session["SJR2Grade0"] = false;
            Session["SJR2Grade1"] = false;
            Session["SJR2Grade2"] = false;
            Session["SJR2Grade3"] = false;
            Session["SJR1Grade0"] = false;
            Session["SJR1Grade1"] = false;
            Session["SJR1Grade2"] = false;
            Session["SJR1Grade3"] = false;
            Session["SJR1VeryPoor"] = false;
            Session["SJR1Poor"] = false;
            Session["SJR1Adequate"] = false;
            Session["SJR1Good"] = false;
            Session["SJR1Excellent"] = false;
            Session["SJR2VeryPoor"] = false;
            Session["SJR2Poor"] = false;
            Session["SJR2Adequate"] = false;
            Session["SJR2Good"] = false;
            Session["SJR2Excellent"] = false;
            return RedirectToAction("PatientReportList", new { id = 0, qapreview = false, medtriage = false, meoreview = false });
        }

        public ActionResult SJR1GradeCount(bool grade0, bool grade2, bool grade3)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportStartDate"])) || Convert.ToString(Session["ReportStartDate"]) == "01/01/0001")
                Session["ReportStartDate"] = System.DateTime.Now.AddDays(-30);
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportEndDate"])) || Convert.ToString(Session["ReportEndDate"]) == "01/01/0001")
                Session["ReportEndDate"] = System.DateTime.Now;
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportCaregroup"])))
                Session["ReportCaregroup"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportPatientType"])))
                Session["ReportPatientType"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportWardDeath"])))
                Session["ReportWardDeath"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeConsultant"])))
                Session["ReportDischargeConsultant"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeSpeciality"])))
                Session["ReportDischargeSpeciality"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportMEName"])))
                Session["ReportMEName"] = "";
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            patientDetails = dBEngine.GetPatientDetailsBySJRGrade(DateTime.ParseExact(Convert.ToString(Session["ReportStartDate"]).Split(" ".ToCharArray())[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), DateTime.ParseExact(Convert.ToString(Session["ReportEndDate"]).Split(" ".ToCharArray())[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), Convert.ToString(Session["ReportCaregroup"]), Convert.ToString(Session["ReportPatientType"]), Convert.ToString(Session["ReportWardDeath"]), Convert.ToString(Session["ReportDischargeConsultant"]), Convert.ToString(Session["ReportDischargeSpeciality"]), Convert.ToString(Session["ReportMEName"]), grade0, grade2, grade3, Convert.ToInt32(Session["LoginUserID"]));
            Session["PatientDetails"] = patientDetails;
            Session["AdultDeath"] = false;
            Session["MEOutstanding"] = false;
            Session["A&EDeath"] = false;
            Session["MECompleted"] = false;
            Session["DeathExpectency"] = true;
            Session["QAPCompleted"] = false;
            Session["QAPOutstanding"] = false;
            Session["CauseID"] = 0;
            Session["MCCDIssue"] = false;
            Session["Referral"] = false;
            Session["PostMortem"] = false;
            Session["CareQuality"] = false;
            Session["RecommndedReferral"] = false;
            Session["OtherReferral"] = true;
            Session["SJRRequired"] = false;
            Session["PatientSIRI"] = false;
            Session["SafeGuard"] = false;
            Session["LearningDisability"] = false;
            Session["ChildDeath"] = false;
            Session["WardTeam"] = false;
            Session["HeadOfCompliance"] = false;
            Session["PALsComplaints"] = false;
            Session["Other"] = false;
            Session["ProblemOccured"] = false;
            Session["KeyLearnings"] = false;
            Session["SJR1Completed"] = false;
            Session["SJR1Outstanding"] = false;
            if(grade0)
                Session["SJR1Grade0"] = true;
            if (grade2)
                Session["SJR1Grade2"] = true;
            if (grade3)
                Session["SJR1Grade3"] = true;
            Session["MSGGrade0"] = false;
            Session["MSGGrade2"] = false;
            Session["MSG1Grade3"] = false;
            Session["CoronerDecisionNFAction"] = false;
            Session["CoronerDecisionPostMortem"] = false;
            Session["CoronerDecisionForensicPM"] = false;
            Session["CoronerDecisionGPIssue"] = false;
            Session["CoronerDecisionInquest"] = false;
            Session["CoronerDecision100A"] = false;
            Session["FeedbackTypeID"] = 0;
            Session["SJR2Completed"] = false;
            Session["SJR2Outstanding"] = false;
            Session["SJR2Grade0"] = false;
            Session["SJR2Grade1"] = false;
            Session["SJR2Grade2"] = false;
            Session["SJR2Grade3"] = false;
            Session["SJR1Grade0"] = false;
            Session["SJR1Grade1"] = false;
            Session["SJR1Grade2"] = false;
            Session["SJR1Grade3"] = false;
            Session["SJR1VeryPoor"] = false;
            Session["SJR1Poor"] = false;
            Session["SJR1Adequate"] = false;
            Session["SJR1Good"] = false;
            Session["SJR1Excellent"] = false;
            Session["SJR2VeryPoor"] = false;
            Session["SJR2Poor"] = false;
            Session["SJR2Adequate"] = false;
            Session["SJR2Good"] = false;
            Session["SJR2Excellent"] = false;
            return RedirectToAction("PatientReportList", new { id = 0, qapreview = false, medtriage = false, meoreview = false });
        }

        public ActionResult SJR1GradeCount1(bool grade0, bool grade1, bool grade2, bool grade3)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportStartDate"])) || Convert.ToString(Session["ReportStartDate"]) == "01/01/0001")
                Session["ReportStartDate"] = System.DateTime.Now.AddDays(-30);
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportEndDate"])) || Convert.ToString(Session["ReportEndDate"]) == "01/01/0001")
                Session["ReportEndDate"] = System.DateTime.Now;
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportCaregroup"])))
                Session["ReportCaregroup"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportPatientType"])))
                Session["ReportPatientType"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportWardDeath"])))
                Session["ReportWardDeath"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeConsultant"])))
                Session["ReportDischargeConsultant"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeSpeciality"])))
                Session["ReportDischargeSpeciality"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportMEName"])))
                Session["ReportMEName"] = "";
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            patientDetails = dBEngine.GetPatientDetailsBySJRGrade1(DateTime.ParseExact(Convert.ToString(Session["ReportStartDate"]).Split(" ".ToCharArray())[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), DateTime.ParseExact(Convert.ToString(Session["ReportEndDate"]).Split(" ".ToCharArray())[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), Convert.ToString(Session["ReportCaregroup"]), Convert.ToString(Session["ReportPatientType"]), Convert.ToString(Session["ReportWardDeath"]), Convert.ToString(Session["ReportDischargeConsultant"]), Convert.ToString(Session["ReportDischargeSpeciality"]), Convert.ToString(Session["ReportMEName"]), grade0, grade1, grade2, grade3, Convert.ToInt32(Session["LoginUserID"]));
            Session["PatientDetails"] = patientDetails;
            Session["AdultDeath"] = false;
            Session["MEOutstanding"] = false;
            Session["A&EDeath"] = false;
            Session["MECompleted"] = false;
            Session["DeathExpectency"] = true;
            Session["QAPCompleted"] = false;
            Session["QAPOutstanding"] = false;
            Session["CauseID"] = 0;
            Session["MCCDIssue"] = false;
            Session["Referral"] = false;
            Session["PostMortem"] = false;
            Session["CareQuality"] = false;
            Session["RecommndedReferral"] = false;
            Session["OtherReferral"] = true;
            Session["SJRRequired"] = false;
            Session["PatientSIRI"] = false;
            Session["SafeGuard"] = false;
            Session["LearningDisability"] = false;
            Session["ChildDeath"] = false;
            Session["WardTeam"] = false;
            Session["HeadOfCompliance"] = false;
            Session["PALsComplaints"] = false;
            Session["Other"] = false;
            Session["ProblemOccured"] = false;
            Session["KeyLearnings"] = false;
            Session["SJR1Completed"] = false;
            Session["SJR1Outstanding"] = false;
            if (grade0)
                Session["SJR1Grade0"] = true;
            if (grade1)
                Session["SJR1Grade1"] = true;
            if (grade2)
                Session["SJR1Grade2"] = true;
            if (grade3)
                Session["SJR1Grade3"] = true;
            Session["MSGGrade0"] = false;
            Session["MSGGrade2"] = false;
            Session["MSG1Grade3"] = false;
            Session["CoronerDecisionNFAction"] = false;
            Session["CoronerDecisionPostMortem"] = false;
            Session["CoronerDecisionForensicPM"] = false;
            Session["CoronerDecisionGPIssue"] = false;
            Session["CoronerDecisionInquest"] = false;
            Session["CoronerDecision100A"] = false;
            Session["FeedbackTypeID"] = 0;
            Session["SJR2Completed"] = false;
            Session["SJR2Outstanding"] = false;
            Session["SJR2Grade0"] = false;
            Session["SJR2Grade1"] = false;
            Session["SJR2Grade2"] = false;
            Session["SJR2Grade3"] = false;
            Session["SJR1VeryPoor"] = false;
            Session["SJR1Poor"] = false;
            Session["SJR1Adequate"] = false;
            Session["SJR1Good"] = false;
            Session["SJR1Excellent"] = false;
            Session["SJR2VeryPoor"] = false;
            Session["SJR2Poor"] = false;
            Session["SJR2Adequate"] = false;
            Session["SJR2Good"] = false;
            Session["SJR2Excellent"] = false;
            return RedirectToAction("PatientReportList", new { id = 0, qapreview = false, medtriage = false, meoreview = false });
        }

        public ActionResult SJR2GradeCount(bool grade0, bool grade1, bool grade2, bool grade3)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportStartDate"])) || Convert.ToString(Session["ReportStartDate"]) == "01/01/0001")
                Session["ReportStartDate"] = System.DateTime.Now.AddDays(-30);
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportEndDate"])) || Convert.ToString(Session["ReportEndDate"]) == "01/01/0001")
                Session["ReportEndDate"] = System.DateTime.Now;
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportCaregroup"])))
                Session["ReportCaregroup"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportPatientType"])))
                Session["ReportPatientType"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportWardDeath"])))
                Session["ReportWardDeath"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeConsultant"])))
                Session["ReportDischargeConsultant"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeSpeciality"])))
                Session["ReportDischargeSpeciality"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportMEName"])))
                Session["ReportMEName"] = "";
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            patientDetails = dBEngine.GetPatientDetailsBySJR2Grade(DateTime.ParseExact(Convert.ToString(Session["ReportStartDate"]).Split(" ".ToCharArray())[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), DateTime.ParseExact(Convert.ToString(Session["ReportEndDate"]).Split(" ".ToCharArray())[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), Convert.ToString(Session["ReportCaregroup"]), Convert.ToString(Session["ReportPatientType"]), Convert.ToString(Session["ReportWardDeath"]), Convert.ToString(Session["ReportDischargeConsultant"]), Convert.ToString(Session["ReportDischargeSpeciality"]), Convert.ToString(Session["ReportMEName"]), grade0, grade1, grade2, grade3, Convert.ToInt32(Session["LoginUserID"]));
            Session["PatientDetails"] = patientDetails;
            Session["AdultDeath"] = false;
            Session["MEOutstanding"] = false;
            Session["A&EDeath"] = false;
            Session["MECompleted"] = false;
            Session["DeathExpectency"] = true;
            Session["QAPCompleted"] = false;
            Session["QAPOutstanding"] = false;
            Session["CauseID"] = 0;
            Session["MCCDIssue"] = false;
            Session["Referral"] = false;
            Session["PostMortem"] = false;
            Session["CareQuality"] = false;
            Session["RecommndedReferral"] = false;
            Session["OtherReferral"] = true;
            Session["SJRRequired"] = false;
            Session["PatientSIRI"] = false;
            Session["SafeGuard"] = false;
            Session["LearningDisability"] = false;
            Session["ChildDeath"] = false;
            Session["WardTeam"] = false;
            Session["HeadOfCompliance"] = false;
            Session["PALsComplaints"] = false;
            Session["Other"] = false;
            Session["ProblemOccured"] = false;
            Session["KeyLearnings"] = false;
            Session["SJR1Completed"] = false;
            Session["SJR1Grade0"] = false;
            Session["SJR1Grade1"] = false;
            Session["SJR1Grade2"] = false;
            Session["SJR1Grade3"] = false;
            Session["SJR1Outstanding"] = false;
            if (grade0)
                Session["SJR2Grade0"] = true;
            if (grade1)
                Session["SJR2Grade1"] = true;
            if (grade2)
                Session["SJR2Grade2"] = true;
            if (grade3)
                Session["SJR2Grade3"] = true;
            Session["MSGGrade0"] = false;
            Session["MSGGrade2"] = false;
            Session["MSG1Grade3"] = false;
            Session["CoronerDecisionNFAction"] = false;
            Session["CoronerDecisionPostMortem"] = false;
            Session["CoronerDecisionForensicPM"] = false;
            Session["CoronerDecisionGPIssue"] = false;
            Session["CoronerDecisionInquest"] = false;
            Session["CoronerDecision100A"] = false;
            Session["FeedbackTypeID"] = 0;
            Session["SJR2Completed"] = false;
            Session["SJR2Outstanding"] = false;
            Session["SJR1VeryPoor"] = false;
            Session["SJR1Poor"] = false;
            Session["SJR1Adequate"] = false;
            Session["SJR1Good"] = false;
            Session["SJR1Excellent"] = false;
            Session["SJR2VeryPoor"] = false;
            Session["SJR2Poor"] = false;
            Session["SJR2Adequate"] = false;
            Session["SJR2Good"] = false;
            Session["SJR2Excellent"] = false;
            return RedirectToAction("PatientReportList", new { id = 0, qapreview = false, medtriage = false, meoreview = false });
        }

        public ActionResult SJR1CareCount(bool verypoor, bool poor, bool adequate, bool good, bool excellent)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportStartDate"])) || Convert.ToString(Session["ReportStartDate"]) == "01/01/0001")
                Session["ReportStartDate"] = System.DateTime.Now.AddDays(-30);
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportEndDate"])) || Convert.ToString(Session["ReportEndDate"]) == "01/01/0001")
                Session["ReportEndDate"] = System.DateTime.Now;
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportCaregroup"])))
                Session["ReportCaregroup"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportPatientType"])))
                Session["ReportPatientType"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportWardDeath"])))
                Session["ReportWardDeath"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeConsultant"])))
                Session["ReportDischargeConsultant"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeSpeciality"])))
                Session["ReportDischargeSpeciality"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportMEName"])))
                Session["ReportMEName"] = "";
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            patientDetails = dBEngine.GetPatientDetailsBySJR1Care(DateTime.ParseExact(Convert.ToString(Session["ReportStartDate"]).Split(" ".ToCharArray())[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), DateTime.ParseExact(Convert.ToString(Session["ReportEndDate"]).Split(" ".ToCharArray())[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), Convert.ToString(Session["ReportCaregroup"]), Convert.ToString(Session["ReportPatientType"]), Convert.ToString(Session["ReportWardDeath"]), Convert.ToString(Session["ReportDischargeConsultant"]), Convert.ToString(Session["ReportDischargeSpeciality"]), Convert.ToString(Session["ReportMEName"]), verypoor, poor, adequate, good, excellent, Convert.ToInt32(Session["LoginUserID"]));
            Session["PatientDetails"] = patientDetails;
            Session["AdultDeath"] = false;
            Session["MEOutstanding"] = false;
            Session["A&EDeath"] = false;
            Session["MECompleted"] = false;
            Session["DeathExpectency"] = true;
            Session["QAPCompleted"] = false;
            Session["QAPOutstanding"] = false;
            Session["CauseID"] = 0;
            Session["MCCDIssue"] = false;
            Session["Referral"] = false;
            Session["PostMortem"] = false;
            Session["CareQuality"] = false;
            Session["RecommndedReferral"] = false;
            Session["OtherReferral"] = true;
            Session["SJRRequired"] = false;
            Session["PatientSIRI"] = false;
            Session["SafeGuard"] = false;
            Session["LearningDisability"] = false;
            Session["ChildDeath"] = false;
            Session["WardTeam"] = false;
            Session["HeadOfCompliance"] = false;
            Session["PALsComplaints"] = false;
            Session["Other"] = false;
            Session["ProblemOccured"] = false;
            Session["KeyLearnings"] = false;
            Session["SJR1Completed"] = false;
            Session["SJR1Grade0"] = false;
            Session["SJR1Grade1"] = false;
            Session["SJR1Grade2"] = false;
            Session["SJR1Grade3"] = false;
            Session["SJR1Outstanding"] = false;
            Session["SJR2Grade0"] = false;
            Session["SJR2Grade1"] = false;
            Session["SJR2Grade2"] = false;
            Session["SJR2Grade3"] = false;
            if (verypoor)
                Session["SJR1VeryPoor"] = true;
            if (poor)
                Session["SJR1Poor"] = true;
            if (adequate)
                Session["SJR1Adequate"] = true;
            if (good)
                Session["SJR1Good"] = true;
            if (excellent)
                Session["SJR1Excellent"] = true;
            Session["SJR2VeryPoor"] = false;
            Session["SJR2Poor"] = false;
            Session["SJR2Adequate"] = false;
            Session["SJR2Good"] = false;
            Session["SJR2Excellent"] = false;
            Session["MSGGrade0"] = false;
            Session["MSGGrade2"] = false;
            Session["MSG1Grade3"] = false;
            Session["CoronerDecisionNFAction"] = false;
            Session["CoronerDecisionPostMortem"] = false;
            Session["CoronerDecisionForensicPM"] = false;
            Session["CoronerDecisionGPIssue"] = false;
            Session["CoronerDecisionInquest"] = false;
            Session["CoronerDecision100A"] = false;
            Session["FeedbackTypeID"] = 0;
            Session["SJR2Completed"] = false;
            Session["SJR2Outstanding"] = false;
            return RedirectToAction("PatientReportList", new { id = 0, qapreview = false, medtriage = false, meoreview = false });
        }

        public ActionResult SJR2CareCount(bool verypoor, bool poor, bool adequate, bool good, bool excellent)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportStartDate"])) || Convert.ToString(Session["ReportStartDate"]) == "01/01/0001")
                Session["ReportStartDate"] = System.DateTime.Now.AddDays(-30);
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportEndDate"])) || Convert.ToString(Session["ReportEndDate"]) == "01/01/0001")
                Session["ReportEndDate"] = System.DateTime.Now;
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportCaregroup"])))
                Session["ReportCaregroup"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportPatientType"])))
                Session["ReportPatientType"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportWardDeath"])))
                Session["ReportWardDeath"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeConsultant"])))
                Session["ReportDischargeConsultant"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeSpeciality"])))
                Session["ReportDischargeSpeciality"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportMEName"])))
                Session["ReportMEName"] = "";
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            patientDetails = dBEngine.GetPatientDetailsBySJR2Care(DateTime.ParseExact(Convert.ToString(Session["ReportStartDate"]).Split(" ".ToCharArray())[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), DateTime.ParseExact(Convert.ToString(Session["ReportEndDate"]).Split(" ".ToCharArray())[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), Convert.ToString(Session["ReportCaregroup"]), Convert.ToString(Session["ReportPatientType"]), Convert.ToString(Session["ReportWardDeath"]), Convert.ToString(Session["ReportDischargeConsultant"]), Convert.ToString(Session["ReportDischargeSpeciality"]), Convert.ToString(Session["ReportMEName"]), verypoor, poor, adequate, good, excellent, Convert.ToInt32(Session["LoginUserID"]));
            Session["PatientDetails"] = patientDetails;
            Session["AdultDeath"] = false;
            Session["MEOutstanding"] = false;
            Session["A&EDeath"] = false;
            Session["MECompleted"] = false;
            Session["DeathExpectency"] = true;
            Session["QAPCompleted"] = false;
            Session["QAPOutstanding"] = false;
            Session["CauseID"] = 0;
            Session["MCCDIssue"] = false;
            Session["Referral"] = false;
            Session["PostMortem"] = false;
            Session["CareQuality"] = false;
            Session["RecommndedReferral"] = false;
            Session["OtherReferral"] = true;
            Session["SJRRequired"] = false;
            Session["PatientSIRI"] = false;
            Session["SafeGuard"] = false;
            Session["LearningDisability"] = false;
            Session["ChildDeath"] = false;
            Session["WardTeam"] = false;
            Session["HeadOfCompliance"] = false;
            Session["PALsComplaints"] = false;
            Session["Other"] = false;
            Session["ProblemOccured"] = false;
            Session["KeyLearnings"] = false;
            Session["SJR1Completed"] = false;
            Session["SJR1Grade0"] = false;
            Session["SJR1Grade1"] = false;
            Session["SJR1Grade2"] = false;
            Session["SJR1Grade3"] = false;
            Session["SJR1Outstanding"] = false;
            Session["SJR1VeryPoor"] = false;
            Session["SJR1Poor"] = false;
            Session["SJR1Adequate"] = false;
            Session["SJR1Good"] = false;
            Session["SJR1Excellent"] = false;
            if (verypoor)
                Session["SJR2VeryPoor"] = true;
            if (poor)
                Session["SJR2Poor"] = true;
            if (adequate)
                Session["SJR2Adequate"] = true;
            if (good)
                Session["SJR2Good"] = true;
            if (excellent)
                Session["SJR2Excellent"] = true;
            Session["MSGGrade0"] = false;
            Session["MSGGrade2"] = false;
            Session["MSG1Grade3"] = false;
            Session["CoronerDecisionNFAction"] = false;
            Session["CoronerDecisionPostMortem"] = false;
            Session["CoronerDecisionForensicPM"] = false;
            Session["CoronerDecisionGPIssue"] = false;
            Session["CoronerDecisionInquest"] = false;
            Session["CoronerDecision100A"] = false;
            Session["FeedbackTypeID"] = 0;
            Session["SJR2Completed"] = false;
            Session["SJR2Outstanding"] = false;
            return RedirectToAction("PatientReportList", new { id = 0, qapreview = false, medtriage = false, meoreview = false });
        }
        public ActionResult MSGGradeCount(bool grade0, bool grade2, bool grade3)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportStartDate"])) || Convert.ToString(Session["ReportStartDate"]) == "01/01/0001")
                Session["ReportStartDate"] = System.DateTime.Now.AddDays(-30);
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportEndDate"])) || Convert.ToString(Session["ReportEndDate"]) == "01/01/0001")
                Session["ReportEndDate"] = System.DateTime.Now;
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportCaregroup"])))
                Session["ReportCaregroup"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportPatientType"])))
                Session["ReportPatientType"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportWardDeath"])))
                Session["ReportWardDeath"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeConsultant"])))
                Session["ReportDischargeConsultant"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeSpeciality"])))
                Session["ReportDischargeSpeciality"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportMEName"])))
                Session["ReportMEName"] = "";
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            patientDetails = dBEngine.GetPatientDetailsByMSGGrade(DateTime.ParseExact(Convert.ToString(Session["ReportStartDate"]).Split(" ".ToCharArray())[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), DateTime.ParseExact(Convert.ToString(Session["ReportEndDate"]).Split(" ".ToCharArray())[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), Convert.ToString(Session["ReportCaregroup"]), Convert.ToString(Session["ReportPatientType"]), Convert.ToString(Session["ReportWardDeath"]), Convert.ToString(Session["ReportDischargeConsultant"]), Convert.ToString(Session["ReportDischargeSpeciality"]), Convert.ToString(Session["ReportMEName"]), grade0, grade2, grade3, Convert.ToInt32(Session["LoginUserID"]));
            Session["PatientDetails"] = patientDetails;
            Session["AdultDeath"] = false;
            Session["MEOutstanding"] = false;
            Session["A&EDeath"] = false;
            Session["MECompleted"] = false;
            Session["DeathExpectency"] = true;
            Session["QAPCompleted"] = false;
            Session["QAPOutstanding"] = false;
            Session["CauseID"] = 0;
            Session["MCCDIssue"] = false;
            Session["Referral"] = false;
            Session["PostMortem"] = false;
            Session["CareQuality"] = false;
            Session["RecommndedReferral"] = false;
            Session["OtherReferral"] = true;
            Session["SJRRequired"] = false;
            Session["PatientSIRI"] = false;
            Session["SafeGuard"] = false;
            Session["LearningDisability"] = false;
            Session["ChildDeath"] = false;
            Session["WardTeam"] = false;
            Session["HeadOfCompliance"] = false;
            Session["PALsComplaints"] = false;
            Session["Other"] = false;
            Session["ProblemOccured"] = false;
            Session["KeyLearnings"] = false;
            Session["SJR1Completed"] = false;
            Session["SJR1Outstanding"] = false;
            Session["SJR1Grade0"] = false;
            Session["SJR1Grade2"] = false;
            Session["SJR1Grade3"] = false;
            if (grade0)
                Session["MSGGrade0"] = true;
            if (grade2)
                Session["MSGGrade2"] = true;
            if (grade3)
                Session["MSG1Grade3"] = true;
            Session["CoronerDecisionNFAction"] = false;
            Session["CoronerDecisionPostMortem"] = false;
            Session["CoronerDecisionForensicPM"] = false;
            Session["CoronerDecisionGPIssue"] = false;
            Session["CoronerDecisionInquest"] = false;
            Session["CoronerDecision100A"] = false;
            Session["FeedbackTypeID"] = 0;
            Session["SJR2Completed"] = false;
            Session["SJR2Outstanding"] = false;
            return RedirectToAction("PatientReportList", new { id = 0, qapreview = false, medtriage = false, meoreview = false });
        }

        public ActionResult CoronerDecisionCount(bool nfaction, bool postmortem, bool forensicpm, bool gpissue, bool inquest, bool a100)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportStartDate"])) || Convert.ToString(Session["ReportStartDate"]) == "01/01/0001")
                Session["ReportStartDate"] = System.DateTime.Now.AddDays(-30);
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportEndDate"])) || Convert.ToString(Session["ReportEndDate"]) == "01/01/0001")
                Session["ReportEndDate"] = System.DateTime.Now;
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportCaregroup"])))
                Session["ReportCaregroup"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportPatientType"])))
                Session["ReportPatientType"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportWardDeath"])))
                Session["ReportWardDeath"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeConsultant"])))
                Session["ReportDischargeConsultant"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeSpeciality"])))
                Session["ReportDischargeSpeciality"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportMEName"])))
                Session["ReportMEName"] = "";
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            patientDetails = dBEngine.GetPatientDetailsByCoronerDecision(DateTime.ParseExact(Convert.ToString(Session["ReportStartDate"]).Split(" ".ToCharArray())[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), DateTime.ParseExact(Convert.ToString(Session["ReportEndDate"]).Split(" ".ToCharArray())[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), Convert.ToString(Session["ReportCaregroup"]), Convert.ToString(Session["ReportPatientType"]), Convert.ToString(Session["ReportWardDeath"]), Convert.ToString(Session["ReportDischargeConsultant"]), Convert.ToString(Session["ReportDischargeSpeciality"]), Convert.ToString(Session["ReportMEName"]), nfaction, postmortem, forensicpm, gpissue, inquest, a100, Convert.ToInt32(Session["LoginUserID"]));
            Session["PatientDetails"] = patientDetails;
            Session["AdultDeath"] = false;
            Session["MEOutstanding"] = false;
            Session["A&EDeath"] = false;
            Session["MECompleted"] = false;
            Session["DeathExpectency"] = true;
            Session["QAPCompleted"] = false;
            Session["QAPOutstanding"] = false;
            Session["CauseID"] = 0;
            Session["MCCDIssue"] = false;
            Session["Referral"] = false;
            Session["PostMortem"] = false;
            Session["CareQuality"] = false;
            Session["RecommndedReferral"] = false;
            Session["OtherReferral"] = true;
            Session["SJRRequired"] = false;
            Session["PatientSIRI"] = false;
            Session["SafeGuard"] = false;
            Session["LearningDisability"] = false;
            Session["ChildDeath"] = false;
            Session["WardTeam"] = false;
            Session["HeadOfCompliance"] = false;
            Session["PALsComplaints"] = false;
            Session["Other"] = false;
            Session["ProblemOccured"] = false;
            Session["KeyLearnings"] = false;
            Session["SJR1Completed"] = false;
            Session["SJR1Outstanding"] = false;
            Session["SJR1Grade0"] = false;
            Session["SJR1Grade2"] = false;
            Session["SJR1Grade3"] = false;
            if (nfaction)
                Session["CoronerDecisionNFAction"] = true;
            if (postmortem)
                Session["CoronerDecisionPostMortem"] = true;
            if (forensicpm)
                Session["CoronerDecisionForensicPM"] = true;
            if (gpissue)
                Session["CoronerDecisionGPIssue"] = true;
            if (inquest)
                Session["CoronerDecisionInquest"] = true;
            if (a100)
                Session["CoronerDecision100A"] = true;
            Session["FeedbackTypeID"] = 0;
            Session["SJR2Completed"] = false;
            Session["SJR2Outstanding"] = false;
            return RedirectToAction("PatientReportList", new { id = 0, qapreview = false, medtriage = false, meoreview = false });
        }

        public ActionResult ReasonCount(bool learning, bool paeds, bool mental, bool elective, bool nok, bool chemo, bool other)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportStartDate"])) || Convert.ToString(Session["ReportStartDate"]) == "01/01/0001")
                Session["ReportStartDate"] = System.DateTime.Now.AddDays(-30);
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportEndDate"])) || Convert.ToString(Session["ReportEndDate"]) == "01/01/0001")
                Session["ReportEndDate"] = System.DateTime.Now;
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportCaregroup"])))
                Session["ReportCaregroup"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportPatientType"])))
                Session["ReportPatientType"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportWardDeath"])))
                Session["ReportWardDeath"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeConsultant"])))
                Session["ReportDischargeConsultant"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeSpeciality"])))
                Session["ReportDischargeSpeciality"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportMEName"])))
                Session["ReportMEName"] = "";
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            patientDetails = dBEngine.GetPatientDetailsByReason(DateTime.ParseExact(Convert.ToString(Session["ReportStartDate"]).Split(" ".ToCharArray())[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), DateTime.ParseExact(Convert.ToString(Session["ReportEndDate"]).Split(" ".ToCharArray())[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), Convert.ToString(Session["ReportCaregroup"]), Convert.ToString(Session["ReportPatientType"]), Convert.ToString(Session["ReportWardDeath"]), Convert.ToString(Session["ReportDischargeConsultant"]), Convert.ToString(Session["ReportDischargeSpeciality"]), Convert.ToString(Session["ReportMEName"]), learning, paeds, mental, elective, nok, chemo, other, Convert.ToInt32(Session["LoginUserID"]));
            Session["PatientDetails"] = patientDetails;
            Session["AdultDeath"] = false;
            Session["MEOutstanding"] = false;
            Session["A&EDeath"] = false;
            Session["MECompleted"] = false;
            Session["DeathExpectency"] = true;
            Session["QAPCompleted"] = false;
            Session["QAPOutstanding"] = false;
            Session["CauseID"] = 0;
            Session["MCCDIssue"] = false;
            Session["Referral"] = false;
            Session["PostMortem"] = false;
            Session["CareQuality"] = false;
            Session["RecommndedReferral"] = false;
            Session["OtherReferral"] = true;
            Session["SJRRequired"] = false;
            Session["PatientSIRI"] = false;
            Session["SafeGuard"] = false;
            Session["LearningDisability"] = false;
            Session["ChildDeath"] = false;
            Session["WardTeam"] = false;
            Session["HeadOfCompliance"] = false;
            Session["PALsComplaints"] = false;
            Session["Other"] = false;
            Session["ProblemOccured"] = false;
            Session["KeyLearnings"] = false;
            Session["SJR1Completed"] = false;
            Session["SJR1Outstanding"] = false;
            Session["SJR1Grade0"] = false;
            Session["SJR1Grade2"] = false;
            Session["SJR1Grade3"] = false;
            Session["CoronerDecisionNFAction"] = false;
            Session["CoronerDecisionPostMortem"] = false;
            Session["CoronerDecisionForensicPM"] = false;
            Session["CoronerDecisionGPIssue"] = false;
            Session["CoronerDecisionInquest"] = false;
            Session["CoronerDecision100A"] = false;
            Session["FeedbackTypeID"] = 0;
            if (learning)
                Session["Learning"] = true;
            if (paeds)
                Session["Paeds"] = true;
            if (mental)
                Session["Mental"] = true;
            if (elective)
                Session["Elective"] = true;
            if (nok)
                Session["NOK"] = true;
            if (chemo)
                Session["Chemo"] = true;
            if (other)
                Session["OtherReason"] = true;
            Session["SJR2Completed"] = false;
            Session["SJR2Outstanding"] = false;
            return RedirectToAction("PatientReportList", new { id = 0, qapreview = false, medtriage = false, meoreview = false });
        }

        public ActionResult FeedbackTypeCount(int feedbacktypeID)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportStartDate"])) || Convert.ToString(Session["ReportStartDate"]) == "01/01/0001")
                Session["ReportStartDate"] = System.DateTime.Now.AddDays(-30);
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportEndDate"])) || Convert.ToString(Session["ReportEndDate"]) == "01/01/0001")
                Session["ReportEndDate"] = System.DateTime.Now;
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportCaregroup"])))
                Session["ReportCaregroup"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportPatientType"])))
                Session["ReportPatientType"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportWardDeath"])))
                Session["ReportWardDeath"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeConsultant"])))
                Session["ReportDischargeConsultant"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportDischargeSpeciality"])))
                Session["ReportDischargeSpeciality"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["ReportMEName"])))
                Session["ReportMEName"] = "";
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            patientDetails = dBEngine.GetPatientDetailsByFeedbackType(DateTime.ParseExact(Convert.ToString(Session["ReportStartDate"]).Split(" ".ToCharArray())[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), DateTime.ParseExact(Convert.ToString(Session["ReportEndDate"]).Split(" ".ToCharArray())[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), Convert.ToString(Session["ReportCaregroup"]), Convert.ToString(Session["ReportPatientType"]), Convert.ToString(Session["ReportWardDeath"]), Convert.ToString(Session["ReportDischargeConsultant"]), Convert.ToString(Session["ReportDischargeSpeciality"]), Convert.ToString(Session["ReportMEName"]), feedbacktypeID, Convert.ToInt32(Session["LoginUserID"]));
            Session["PatientDetails"] = patientDetails;
            Session["AdultDeath"] = false;
            Session["MEOutstanding"] = false;
            Session["A&EDeath"] = false;
            Session["MECompleted"] = false;
            Session["DeathExpectency"] = true;
            Session["QAPCompleted"] = false;
            Session["QAPOutstanding"] = false;
            Session["CauseID"] = 0;
            Session["MCCDIssue"] = false;
            Session["Referral"] = false;
            Session["PostMortem"] = false;
            Session["CareQuality"] = false;
            Session["RecommndedReferral"] = false;
            Session["OtherReferral"] = true;
            Session["SJRRequired"] = false;
            Session["PatientSIRI"] = false;
            Session["SafeGuard"] = false;
            Session["LearningDisability"] = false;
            Session["ChildDeath"] = false;
            Session["WardTeam"] = false;
            Session["HeadOfCompliance"] = false;
            Session["PALsComplaints"] = false;
            Session["Other"] = false;
            Session["ProblemOccured"] = false;
            Session["KeyLearnings"] = false;
            Session["SJR1Completed"] = false;
            Session["SJR1Outstanding"] = false;
            Session["SJR1Grade0"] = false;
            Session["SJR1Grade2"] = false;
            Session["SJR1Grade3"] = false;
            Session["CoronerDecisionNFAction"] = false;
            Session["CoronerDecisionPostMortem"] = false;
            Session["CoronerDecisionForensicPM"] = false;
            Session["CoronerDecisionGPIssue"] = false;
            Session["CoronerDecisionInquest"] = false;
            Session["CoronerDecision100A"] = false;
            Session["FeedbackTypeID"] = feedbacktypeID;
            Session["SJR2Completed"] = false;
            Session["SJR2Outstanding"] = false;
            return RedirectToAction("PatientReportList", new { id = 0, qapreview = false, medtriage = false, meoreview = false });
        }

        public ActionResult AboveBeyondCount()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["StartDate"])) || Convert.ToString(Session["StartDate"]) == "01/01/0001")
                Session["StartDate"] = System.DateTime.Now.AddDays(-30);
            if (string.IsNullOrEmpty(Convert.ToString(Session["EndDate"])) || Convert.ToString(Session["EndDate"]) == "01/01/0001")
                Session["EndDate"] = System.DateTime.Now;
            if (string.IsNullOrEmpty(Convert.ToString(Session["Caregroup"])))
                Session["Caregroup"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["PatientType"])))
                Session["PatientType"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["WardDeath"])))
                Session["WardDeath"] = "";
            if (string.IsNullOrEmpty(Convert.ToString(Session["DischargeConsultant"])))
                Session["DischargeConsultant"] = "";
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            patientDetails = dBEngine.GetPatientDetailsByAboveRecognition(DateTime.ParseExact(Convert.ToString(Session["ReportStartDate"]).Split(" ".ToCharArray())[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), DateTime.ParseExact(Convert.ToString(Session["ReportEndDate"]).Split(" ".ToCharArray())[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), Convert.ToString(Session["ReportCaregroup"]), Convert.ToString(Session["ReportPatientType"]), Convert.ToString(Session["ReportWardDeath"]), Convert.ToString(Session["ReportDischargeConsultant"]), Convert.ToString(Session["ReportDischargeSpeciality"]), Convert.ToString(Session["ReportMEName"]), Convert.ToInt32(Session["LoginUserID"]));
            Session["PatientDetails"] = patientDetails;
            Session["AdultDeath"] = false;
            Session["MEOutstanding"] = false;
            Session["A&EDeath"] = false;
            Session["MECompleted"] = false;
            Session["DeathExpectency"] = true;
            Session["QAPCompleted"] = false;
            Session["QAPOutstanding"] = false;
            Session["CauseID"] = 0;
            Session["MCCDIssue"] = false;
            Session["Referral"] = false;
            Session["PostMortem"] = false;
            Session["CareQuality"] = false;
            Session["RecommndedReferral"] = false;
            Session["OtherReferral"] = true;
            Session["SJRRequired"] = false;
            Session["PatientSIRI"] = false;
            Session["SafeGuard"] = false;
            Session["LearningDisability"] = false;
            Session["ChildDeath"] = false;
            Session["WardTeam"] = false;
            Session["HeadOfCompliance"] = false;
            Session["PALsComplaints"] = false;
            Session["Other"] = false;
            Session["ProblemOccured"] = false;
            Session["KeyLearnings"] = false;
            Session["SJR1Completed"] = false;
            Session["SJR1Outstanding"] = false;
            Session["SJR1Grade0"] = false;
            Session["SJR1Grade2"] = false;
            Session["SJR1Grade3"] = false;
            Session["CoronerDecisionNFAction"] = false;
            Session["CoronerDecisionPostMortem"] = false;
            Session["CoronerDecisionForensicPM"] = false;
            Session["CoronerDecisionGPIssue"] = false;
            Session["CoronerDecisionInquest"] = false;
            Session["CoronerDecision100A"] = false;
            Session["FeedbackTypeID"] = 0;
            Session["AboveRecognition"] = true;
            Session["SJR2Completed"] = false;
            Session["SJR2Outstanding"] = false;
            return RedirectToAction("PatientReportList", new { id = 0, qapreview = false, medtriage = false, meoreview = false });
        }

        public ActionResult QAPRedOrAmber(int? id)
        {
            if (Session["FullName"] != null)
            {
                if (string.IsNullOrEmpty(Session["FullName"].ToString()))
                    return RedirectToAction("Index", "Account");
            }
            else
                return RedirectToAction("Index", "Account");
            bool isUser = GetUserDetailsFromAD();
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            if (Convert.ToInt32(Session["LoginUserID"]) > 0)
            {
                try
                {
                    ViewBag.LoadDischargeSpecialityDropdown = dBEngine.GetSpecialities(Convert.ToInt32(Session["LoginUserID"]));
                    ViewBag.wardDeathDropdown = dBEngine.GetWardOfDeaths(Convert.ToInt32(Session["LoginUserID"]));
                    ViewBag.dischargeConsultantDropdown = dBEngine.GetConsultants(Convert.ToInt32(Session["LoginUserID"]));

                }
                catch (Exception ex)
                {
                    dBEngine.LogException(ex.Message, this.ToString(), "ValidateUser", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
                    throw ex;
                }
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
            return View(patientDetails);
        }

        public ActionResult MedTriageRedOrAmber(int? id)
        {
            if (Session["FullName"] != null)
            {
                if (string.IsNullOrEmpty(Session["FullName"].ToString()))
                    return RedirectToAction("Index", "Account");
            }
            else
                return RedirectToAction("Index", "Account");
            bool isUser = GetUserDetailsFromAD();
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            if (Convert.ToInt32(Session["LoginUserID"]) > 0)
            {
                try
                {
                    ViewBag.LoadDischargeSpecialityDropdown = dBEngine.GetSpecialities(Convert.ToInt32(Session["LoginUserID"]));
                    ViewBag.wardDeathDropdown = dBEngine.GetWardOfDeaths(Convert.ToInt32(Session["LoginUserID"]));
                    ViewBag.dischargeConsultantDropdown = dBEngine.GetConsultants(Convert.ToInt32(Session["LoginUserID"]));

                }
                catch (Exception ex)
                {
                    dBEngine.LogException(ex.Message, this.ToString(), "ValidateUser", System.DateTime.Now, Convert.ToInt32(Session["LoginUserID"]));
                    throw ex;
                }
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
            return View(patientDetails);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult MortalityReview(int? id)
        {
            if (Session["FullName"] != null)
            {
                if (string.IsNullOrEmpty(Session["FullName"].ToString()))
                    return RedirectToAction("Index", "Account");
            }
            else
                return RedirectToAction("Index", "Account");
            bool isUser = GetUserDetailsFromAD();
            Session["PatientID"] = id;
            Session.Timeout = 4000;
            List<clsPatientDetails> patientDetails = new List<clsPatientDetails>();
            string connectionString = ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString;
            DBEngine dBEngine = new DBEngine(connectionString);
            //if (ApplicationSession.LoginUserID > 0)
            //{
            if (id == null || id == 0)
            {
                if (Session["PatientID"] != null)
                    id = Convert.ToInt32(Session["PatientID"]);
                else
                    return RedirectToAction("Index", "Account");
            }

            try
            {
                patientDetails = dBEngine.GetPatientDetails(id, Convert.ToInt32(Session["LoginUserID"]));
                ViewBag.PatientHistoryLink = "'" + "http://rbhdbsred011/TIPS/ReportViewer.aspx?ReportPath=R720_CORS_PatientHistory&MRN=" + patientDetails[0].PatientId + "'";
                //ViewBag.PatientHistoryLink = "'" + "http://rbhdbsred011/TIPSTest/ReportViewer.aspx?ReportPath=R720_CORS_PatientHistory&MRN=" + patientDetails[0].PatientId + ":iid=" + id.ToString() + "'";
                patientDetails.ToList()[0].MedTriage = patientDetails.ToList()[0].MedTriage == 0 ? 2 : patientDetails.ToList()[0].MedTriage;
                patientDetails.ToList()[0].QAPReview = patientDetails.ToList()[0].QAPReview == 0 ? 2 : patientDetails.ToList()[0].QAPReview;
                //patientDetails.ToList()[0].SJR1 = patientDetails.ToList()[0].SJR1 == 0 ? 2 : patientDetails.ToList()[0].SJR1;
                //patientDetails.ToList()[0].SJR2 = patientDetails.ToList()[0].SJR2 == 0 ? 2 : patientDetails.ToList()[0].SJR2;
                //patientDetails.ToList()[0].SJROutcome = patientDetails.ToList()[0].SJROutcome == 0 ? 2 : patientDetails.ToList()[0].SJROutcome;
                 
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //}
            //else
            //{
            //    return RedirectToAction("Index", "Account");
            //}
            return View(patientDetails[0]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool GetUserDetailsFromAD()
        {
            bool isValid = false;
            string FullName = string.Empty;
            DBEngine dBEngine = new DBEngine(ConfigurationManager.ConnectionStrings["NHSConStr"].ConnectionString);
            string userName = "";
            try
            {
                //string LDAPUrl = ConfigurationManager.AppSettings["LDAPURL"].ToString();
                userName = Request.ServerVariables["REMOTE_USER"];
                //userName = Environment.GetEnvironmentVariable("USERNAME");
                //dBEngine.LogException("before username", this.ToString(), "ValidateUser", System.DateTime.Now);
                //if (WindowsIdentity.GetCurrent() != null)
                //userName = WindowsIdentity.GetCurrent().Name;
                //else
                //userName = "Nothing found";
                if (string.IsNullOrEmpty(userName))
                    userName = "Nothing found";
                //dBEngine.LogException(userName, this.ToString(), "ValidateUser", System.DateTime.Now);
                //StreamWriter sw = new StreamWriter(@"E:\Publish\log1.txt", true);
                //sw.WriteLine(userName);
                //sw.Close();
                //userName = null;
                //if (string.IsNullOrWhiteSpace(LDAPUrl))
                //{
                //    if (string.IsNullOrWhiteSpace(UserName))
                //    {
                //        dBEngine.WriteLog("LDAPUrl is blank or not provided & UserName could not be retieved from environment");
                //        TempData["FullName"] = "";
                //    }
                //    else
                //    {
                //        dBEngine.WriteLog($"LDAPUrl is blank or not provided but UserName {UserName} was retieved from environment");
                //        TempData["FullName"] = UserName;
                //    }
                //}
                //else
                //{
                if (string.IsNullOrWhiteSpace(userName))
                {
                    Session["FullName"] = Session["FirstName"] + " " + Session["LastName"];
                    Session.Timeout = 4000;
                    //dBEngine.WriteLog($"UserName could not be retieved from environment");
                }
                //else
                //{
                //    DirectoryEntry rootEntry = new DirectoryEntry(LDAPUrl);
                //    rootEntry.AuthenticationType = AuthenticationTypes.None;
                //    DirectorySearcher searcher = new DirectorySearcher(rootEntry);
                //    var queryFormat = "(&(objectClass=user)(objectCategory=person)(|(SAMAccountName=*{0}*)(cn=*{0}*)(gn=*{0}*)(sn=*{0}*)(email=*{0}*)))";
                //    searcher.Filter = string.Format(queryFormat, UserName);
                //    foreach (SearchResult result in searcher.FindAll())
                //    {
                //        FullName = result.Properties["cn"].Count > 0 ? result.Properties["cn"][0].ToString() : string.Empty;
                //    }
                //    if (string.IsNullOrWhiteSpace(FullName))
                //    {
                //        dBEngine.WriteLog($"LDAP URL was provided {LDAPUrl} BUT User - {UserName} not found in AD");
                //        FullName = UserName;
                //        isValid = false;
                //    }
                //else
                //{
                else
                {
                    //AppUsers user = dBEngine.ValidateUser(userName.Split("\\".ToCharArray())[1]);
                    //if (user.IsFound == true)
                    //    isValid = true;
                    //else
                    //    isValid = false;
                    //Session["FullName"] = user.FirstName + " " + user.LastName;
                    Session["FullName"] = Session["FirstName"] + " " + Session["LastName"];
                    Session.Timeout = 4000;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return isValid;
        }
    }
}
