using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TightWiki.Shared.ADO;
using TightWiki.Shared.Library;
using TightWiki.Shared.Models.Data;
using TightWiki.Shared.Wiki;

namespace TightWiki.Shared.Repository
{
    public static partial class PageRepository
    {
        public static void InsertPageStatistics(int pageId,
            double wikifyTimeMs, int matchCount, int errorCount, int outgoingLinkCount,
            int tagCount, int processedBodySize, int bodySize)
        {
            using var handler = new SqlConnectionHandler();
            var param = new
            {
                PageId = pageId,
                WikifyTimeMs = wikifyTimeMs,
                MatchCount = matchCount,
                ErrorCount = errorCount,
                OutgoingLinkCount = outgoingLinkCount,
                TagCount = tagCount,
                ProcessedBodySize = processedBodySize,
                BodySize = bodySize
            };

            handler.Connection.Execute("InsertPageStatistics",
                param, null, Singletons.CommandTimeout, CommandType.StoredProcedure);
        }

        public static void InsetPageComment(int pageId, int userId, string body)
        {
            using var handler = new SqlConnectionHandler();
            var param = new
            {
                PageId = pageId,
                UserId = userId,
                Body = body
            };

            handler.Connection.Execute("InsetPageComment",
                param, null, Singletons.CommandTimeout, CommandType.StoredProcedure);
        }

        public static void DeletePageCommentById(int pageId, int userId, int commentId)
        {
            using var handler = new SqlConnectionHandler();
            var param = new
            {
                PageId = pageId,
                UserId = userId,
                CommentId = commentId
            };

            handler.Connection.Execute("DeletePageCommentById",
                param, null, Singletons.CommandTimeout, CommandType.StoredProcedure);
        }

        public static List<PageComment> GetPageCommentsPaged(string navigation, int pageNumber, int pageSize = 0)
        {
            using var handler = new SqlConnectionHandler();
            var param = new
            {
                Navigation = navigation,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return handler.Connection.Query<PageComment>("GetPageCommentsPaged",
                param, null, true, Singletons.CommandTimeout, CommandType.StoredProcedure).ToList();
        }

        public static List<NonexistentPage> GetNonexistentPagesPaged(int pageNumber, int pageSize = 0)
        {
            using var handler = new SqlConnectionHandler();
            var param = new
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return handler.Connection.Query<NonexistentPage>("GetNonexistentPagesPaged",
                param, null, true, Singletons.CommandTimeout, CommandType.StoredProcedure).ToList();
        }

        public static void UpdateSinglePageReference(string pageNavigation)
        {
            using var handler = new SqlConnectionHandler();
            var param = new
            {
                @PageNavigation = pageNavigation
            };

            handler.Connection.Execute("UpdateSinglePageReference",
                param, null, Singletons.CommandTimeout, CommandType.StoredProcedure);
        }

        public static void UpdatePageReferences(int pageId, List<NameNav> referencesPageNavigations)
        {
            using var handler = new SqlConnectionHandler();

            var param = new
            {
                PageId = pageId,
                References = referencesPageNavigations.ToDataTable()
            };

            handler.Connection.Execute("UpdatePageReferences",
                param, null, Singletons.CommandTimeout, CommandType.StoredProcedure);
        }

        public static List<Page> PageSearch(List<PageToken> items)
        {
            using var handler = new SqlConnectionHandler();
            var param = new
            {
                SearchTerms = items.ToDataTable()
            };

            return handler.Connection.Query<Page>("PageSearchPaged",
                param, null, true, Singletons.CommandTimeout, CommandType.StoredProcedure).ToList();
        }

        public static List<Page> GetAllPagesByInstructionPaged(int pageNumber, int pageSize = 0, string instruction = null)
        {
            using var handler = new SqlConnectionHandler();
            var param = new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Instruction = instruction
            };

            return handler.Connection.Query<Page>("GetAllPagesByInstructionPaged",
                param, null, true, Singletons.CommandTimeout, CommandType.StoredProcedure).ToList();
        }

        public static List<Page> PageSearchPaged(List<PageToken> items, int pageNumber, int pageSize = 0, bool? allowFuzzyMatching = null)
        {
            using var handler = new SqlConnectionHandler();
            var param = new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                AllowFuzzyMatching = allowFuzzyMatching,
                SearchTerms = items.ToDataTable()
            };

            return handler.Connection.Query<Page>("PageSearchPaged",
                param, null, true, Singletons.CommandTimeout, CommandType.StoredProcedure).ToList();
        }

        public static List<Page> PageSearchRussianPages(List<string> items)
        {
            static DataTable CreateKeywordsDataTable(List<string> keywords)
            {
                DataTable table = new DataTable();
                table.Columns.Add("Value", typeof(string));

                foreach (var keyword in keywords)
                {
                    table.Rows.Add(keyword);
                }

                return table;
            }

            using var handler = new SqlConnectionHandler();
            var param = new
            {
                SearchTerms = CreateKeywordsDataTable(items)
            };

            return handler.Connection.Query<Page>("PageSearchRussianPages",
                param, null, true, Singletons.CommandTimeout, CommandType.StoredProcedure).ToList();
        }

        /// <summary>
        /// Unlike the search, this method retunrs all pages and allows them to be paired down using the search terms.
        /// Whereas the search requires a search term to get results. The matching here is also exact, no score based matching.
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchTerms"></param>
        /// <returns></returns>
        public static List<Page> GetAllPagesPaged(int pageNumber, int pageSize = 0, string searchTerms = null)
        {
            using var handler = new SqlConnectionHandler();
            var param = new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchTerms = searchTerms
            };

            return handler.Connection.Query<Page>("GetAllPagesPaged",
                param, null, true, Singletons.CommandTimeout, CommandType.StoredProcedure).ToList();
        }

        public static List<NamespaceStat> GetAllNamespacesPaged(int pageNumber, int pageSize = 0)
        {
            using var handler = new SqlConnectionHandler();
            var param = new
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return handler.Connection.Query<NamespaceStat>("GetAllNamespacesPaged",
                param, null, true, Singletons.CommandTimeout, CommandType.StoredProcedure).ToList();
        }

        public static List<Page> GetAllPages()
        {
            using var handler = new SqlConnectionHandler();

            return handler.Connection.Query<Page>("GetAllPages",
                null, null, true, Singletons.CommandTimeout, CommandType.StoredProcedure).ToList();
        }

        public static void UpdatePageProcessingInstructions(int pageId, List<string> instructions)
        {
            using var handler = new SqlConnectionHandler();
            var param = new
            {
                PageId = pageId,
                Instructions = string.Join(",", instructions)
            };

            handler.Connection.Execute("UpdatePageProcessingInstructions",
                param, null, Singletons.CommandTimeout, CommandType.StoredProcedure);
        }

        public static List<ProcessingInstruction> GetPageProcessingInstructionsByPageId(int pageId)
        {
            using var handler = new SqlConnectionHandler();
            var param = new
            {
                PageId = pageId
            };

            return handler.Connection.Query<ProcessingInstruction>("GetPageProcessingInstructionsByPageId",
                param, null, true, Singletons.CommandTimeout, CommandType.StoredProcedure).ToList();
        }

        public static Page GetPageRevisionInfoById(int pageId, int? revision = null)
        {
            using var handler = new SqlConnectionHandler();
            return handler.Connection.Query<Page>("GetPageRevisionInfoById",
                new { PageId = pageId, Revision = revision }, null, true, Singletons.CommandTimeout, CommandType.StoredProcedure).FirstOrDefault();
        }

        public static List<PageTag> GetPageTagsById(int pageId)
        {
            using var handler = new SqlConnectionHandler();
            return handler.Connection.Query<PageTag>("GetPageTagsById",
                new { PageId = pageId }, null, true, Singletons.CommandTimeout, CommandType.StoredProcedure).ToList();
        }

        public static Page GetPageInfoById(int pageId)
        {
            using var handler = new SqlConnectionHandler();
            return handler.Connection.Query<Page>("GetPageInfoById",
                new { PageId = pageId }, null, true, Singletons.CommandTimeout, CommandType.StoredProcedure).FirstOrDefault();
        }

        public static Page GetPageRevisionById(int pageId, int? revision = null)
        {
            using var handler = new SqlConnectionHandler();
            return handler.Connection.Query<Page>("GetPageRevisionById",
                new { PageId = pageId, Revision = revision }, null, true, Singletons.CommandTimeout, CommandType.StoredProcedure).FirstOrDefault();
        }

        public static void SavePageTokens(List<PageToken> items)
        {
            using var handler = new SqlConnectionHandler();
            handler.Connection.Execute("SavePageTokens",
                new { PageTokens = items.ToDataTable() }, null, Singletons.CommandTimeout, CommandType.StoredProcedure);
        }

        public static void TruncateAllPageHistory(string confirm)
        {
            using var handler = new SqlConnectionHandler();
            handler.Connection.Execute("TruncateAllPageHistory",
                new { Confirm = confirm }, null, Singletons.CommandTimeout, CommandType.StoredProcedure);
        }

        public static int SavePage(Page item)
        {
            using var handler = new SqlConnectionHandler();
            var param = new
            {
                Id = item.Id,
                Name = item.Name,
                Navigation = WikiUtility.CleanPartialURI(item.Name),
                Description = item.Description,
                Body = item.Body,
                Namespace = item.Namespace,
                CreatedByUserId = item.CreatedByUserId,
                CreatedDate = item.CreatedDate,
                ModifiedByUserId = item.ModifiedByUserId,
                ModifiedDate = item.ModifiedDate
            };

            return handler.Connection.ExecuteScalar<int>("SavePage",
                param, null, Singletons.CommandTimeout, CommandType.StoredProcedure);
        }

        public static List<PageRevisionHistory> GetPageRevisionHistoryInfoByNavigationPaged(string navigation, int pageNumber, int pageSize = 0)
        {
            using var handler = new SqlConnectionHandler();

            var param = new
            {
                Navigation = navigation,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return handler.Connection.Query<PageRevisionHistory>("GetPageRevisionHistoryInfoByNavigationPaged",
                param, null, true, Singletons.CommandTimeout, CommandType.StoredProcedure).ToList();
        }

        /// <summary>
        /// Gets the page info without the content.
        /// </summary>
        /// <param name="navigation"></param>
        /// <returns></returns>
        public static Page GetPageInfoByNavigation(string navigation)
        {
            using var handler = new SqlConnectionHandler();

            var param = new
            {
                Navigation = navigation
            };

            return handler.Connection.Query<Page>("GetPageInfoByNavigation",
                param, null, true, Singletons.CommandTimeout, CommandType.StoredProcedure).FirstOrDefault();
        }

        public static void DeletePageById(int pageId)
        {
            using var handler = new SqlConnectionHandler();
            var param = new
            {
                PageId = pageId
            };

            handler.Connection.Execute("DeletePageById",
                param, null, Singletons.CommandTimeout, CommandType.StoredProcedure);
        }

        public static int GetCountOfPageAttachmentsById(int pageId)
        {
            using var handler = new SqlConnectionHandler();
            var param = new
            {
                PageId = pageId
            };

            return handler.Connection.Query<int?>("GetCountOfPageAttachmentsById",
                param, null, true, Singletons.CommandTimeout, CommandType.StoredProcedure).FirstOrDefault() ?? 0;
        }

        public static Page GetPageRevisionByNavigation(string navigation, int? revision = null, bool allowCache = true)
        {
            if (allowCache)
            {
                string cacheKey = $"Page:{navigation}:{revision}:GetPageRevisionByNavigation";
                var result = Cache.Get<Page>(cacheKey);
                if (result == null)
                {
                    result = GetPageRevisionByNavigation(navigation, revision, false);
                    Cache.Put(cacheKey, result);
                }

                return result;
            }

            using var handler = new SqlConnectionHandler();
            var param = new
            {
                Navigation = navigation,
                Revision = revision
            };

            return handler.Connection.Query<Page>("GetPageRevisionByNavigation",
                param, null, true, Singletons.CommandTimeout, CommandType.StoredProcedure).FirstOrDefault();
        }

        public static List<Page> GetTopRecentlyModifiedPagesInfoByUserId(int userId, int topCount)
        {
            using var handler = new SqlConnectionHandler();
            var param = new
            {
                UserId = userId,
                TopCount = topCount
            };

            return handler.Connection.Query<Page>("GetTopRecentlyModifiedPagesInfoByUserId",
                param, null, true, Singletons.CommandTimeout, CommandType.StoredProcedure).ToList();
        }

        public static List<Page> GetTopRecentlyModifiedPagesInfo(int topCount)
        {
            using var handler = new SqlConnectionHandler();
            var param = new
            {
                TopCount = topCount
            };

            return handler.Connection.Query<Page>("GetTopRecentlyModifiedPagesInfo",
                param, null, true, Singletons.CommandTimeout, CommandType.StoredProcedure).ToList();
        }

        public static List<RelatedPage> GetSimilarPagesPaged(int pageId, int pageNumber, int pageSize = 0)
        {
            using var handler = new SqlConnectionHandler();
            var param = new
            {
                PageId = pageId,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return handler.Connection.Query<RelatedPage>("GetSimilarPagesPaged",
                param, null, true, Singletons.CommandTimeout, CommandType.StoredProcedure).ToList();
        }

        public static List<RelatedPage> GetRelatedPagesPaged(int pageId, int pageNumber, int pageSize = 0)
        {
            using var handler = new SqlConnectionHandler();
            var param = new
            {
                PageId = pageId,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return handler.Connection.Query<RelatedPage>("GetRelatedPagesPaged",
                param, null, true, Singletons.CommandTimeout, CommandType.StoredProcedure).ToList();
        }
    }
}
