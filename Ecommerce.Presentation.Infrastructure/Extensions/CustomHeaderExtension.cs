using Ecommerce.Presentation.Infrastructure.Utils;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Presentation.Infrastructure.Extensions
{
    public static class CustomHeaderExtension
    {
        public static MetaData Initialize(this MetaData metaData, int page, int pageSize, int totalItemCount)
        {
            return new MetaData
            {
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)totalItemCount / pageSize),
                PageSize = pageSize,
                TotalCount = totalItemCount
            };
        }

        public static void AddResponseHeaders(this MetaData metaData, HttpResponse response)
        {
            response.Headers.Add("X-Current-Page", metaData.CurrentPage.ToString());
            response.Headers.Add("X-Total-Pages", metaData.TotalPages.ToString());
            response.Headers.Add("X-Page-Size", metaData.PageSize.ToString());
            response.Headers.Add("X-Total-Count", metaData.TotalCount.ToString());
            response.Headers.Add("X-Has-Previous", metaData.HasPrevious.ToString());
            response.Headers.Add("X-Has-Next", metaData.HasNext.ToString());
        }
    }
}
