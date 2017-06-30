using System.Web.Mvc;
using EPiServer.PlugIn;
using System.Collections.Generic;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.DataAbstraction;
using System.Linq;
using EPiServer;
using EPiServer.Filters;
using EPiServer.Globalization;
using System;
using Episerver_Playground.Business.Plugins.Admin.ListPagesByPageTypes;

namespace Episerver_Playground.Business.Plugins.Admin.ListPagesAndPageTypes
{
    [GuiPlugIn(
        DisplayName = "[Custom] List Pages by Page Types",
        Description = "Lists all pages by filtering their belonging page type.",
        SortIndex = 100,
        Area = PlugInArea.AdminMenu,
        Url = "/custom-plugins/list-pages-by-page-types")]
    [Authorize(Roles = Constants.Authorization.CmsAdmins)]
    public class ListPagesByPageTypesController : Controller
    {
        private readonly int _pageSize;

        public ListPagesByPageTypesController() : base()
        {
            _pageSize = 20;
        }

        public ActionResult Index()
        {
            var pageTypes = GetAllPageTypes();

            var model = new ListPagesViewModel
            {
                PageTypesList = pageTypes
            };
            
            return View("~/Business/Plugins/Admin/ListPagesByPageTypes/Views/Index.cshtml", model);
        }

        /// <summary>
        /// Action result for viewing partial model of List pages by page types
        /// </summary>
        /// <param name="id">page type id</param>
        /// <param name="page">page id</param>
        /// <returns>Pages by their page type</returns>
        public PartialViewResult LoadPagesFromPageType(int id, int page)
        {
            var currentPage = page != 0 ? page : 1;
            var startPage = currentPage - 10;
            var endPage = currentPage + 11;

            var pages = GetAllPagesOfPageType(id).Skip(_pageSize * (page - 1)).Take(_pageSize).ToList();
            var totalCount = GetAllPagesOfPageType(id).Count();
            var numberOfPages = Convert.ToInt32(Math.Ceiling((double)totalCount / _pageSize));
           
            if (startPage <= 0)
            {
                endPage -= (startPage - 1);
                startPage = 1;
            }
            if (endPage > numberOfPages)
            {
                endPage = numberOfPages;
                if (endPage > 20)
                {
                    startPage = endPage - 21;
                }
            }

            ListPagesViewModel viewModel = new ListPagesViewModel
            {
                PageSize = _pageSize,
                TotalPages = totalCount,
                NumberOfPages = numberOfPages,
                CurrentPage = page,
                PagesList = pages,
                StartPage = startPage,
                EndPage = endPage,

            };

            return PartialView("~/Business/Plugins/Admin/ListPagesByPageTypes/Views/_Pages.cshtml", viewModel);
        }

        /// <summary>
        /// Method for getting all page types, excluded SysRoot and SysRecycleBin
        /// </summary>
        /// <returns>Gets all page types defined in Episerver</returns>
        private static IEnumerable<PageType> GetAllPageTypes()
        {
            var contentTypeRepository = ServiceLocator.Current.GetInstance<IContentTypeRepository>();
            var sysRoot = contentTypeRepository.Load("SysRoot") as PageType;
            var sysRecycleBin = contentTypeRepository.Load("SysRecycleBin") as PageType;

            return contentTypeRepository.List().OfType<PageType>()
                .Where(t => t != sysRoot)
                .Where(t => t != sysRecycleBin)
                .OrderBy(t => t.DisplayName);
        }

        /// <summary>
        /// Method for retrieving all pages passed by a page type id
        /// </summary>
        /// <returns>Returns pages for each page type given by pagetypeid</returns>
        private static IEnumerable<PageData> GetAllPagesOfPageType(int id)
        {
            var pageTypeId = id;
            var repository = ServiceLocator.Current.GetInstance<IPageCriteriaQueryService>();

            var pageTypeCriteria = new PropertyCriteria
            {
                Name = "PageTypeID",
                Type = PropertyDataType.PageType,
                Value = pageTypeId.ToString(),
                Condition = CompareCondition.Equal,
                Required = true
            };

            var criteria = new PropertyCriteriaCollection
            {
                pageTypeCriteria
            };

            var currentCulture = ContentLanguage.PreferredCulture;
            var languageBranch = currentCulture.Name;

            var pageDataCollection = repository.
                FindAllPagesWithCriteria(ContentReference.RootPage, 
                criteria, 
                languageBranch, 
                LanguageSelector.AutoDetect(true));

            if (pageDataCollection != null)
            { 
                var sortedPageDataCollection = pageDataCollection.OrderByDescending(c => c.StartPublish);

                return sortedPageDataCollection;
            }

            return pageDataCollection;
        }
    }
}