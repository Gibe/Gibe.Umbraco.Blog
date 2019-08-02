using System;
using System.Globalization;
using System.Linq;
using Gibe.Umbraco.Blog.Filters;
using Gibe.Umbraco.Blog.Models;
using Gibe.Umbraco.Blog.Sort;
using Umbraco.Core.Models.PublishedContent;

namespace Gibe.Umbraco.Blog
{
	public class BlogArchive : IBlogArchive
	{
		private readonly IBlogSearch _blogSearch;

		public BlogArchive(IBlogSearch blogSearch)
		{
			_blogSearch = blogSearch;
		}

		public BlogArchiveModel All(IPublishedContent blogRoot)
		{
			var mfi = new DateTimeFormatInfo();
			var blogPosts = _blogSearch.Search(new SectionBlogPostFilter(blogRoot.Id), new DateSort());
			var years = blogPosts.GroupBy(x => GetPostDate(x.Values[ExamineFields.PostDate]).Year, (key, g) => 
				new BlogArchiveYear
				{
					Name = key.ToString("0000"),
					Count = g.Count(),
					Year = key,
					Url = $"{blogRoot.Url}?year={key.ToString("0000")}"
				}).ToList();

			foreach (var year in years)
			{
				year.Months =
					blogPosts.Where(x => GetPostDate(x.Values[ExamineFields.PostDate]).Year == year.Year)
					.GroupBy(x => GetPostDate(x.Values[ExamineFields.PostDate]).Month,
						(key, g) => new BlogArchiveMonth
						{
							Name = mfi.GetMonthName(key),
							Month = key, Count = g.Count(),
							Url = $"{blogRoot.Url}?year={year.Year.ToString("0000")}&month={key.ToString("00")}"
						}).ToList();
			}

			return new BlogArchiveModel { Years = years };
		}

		private DateTime GetPostDate(string value)
		{
			return DateTime.ParseExact(value.Substring(0, 8), "yyyyMMdd", CultureInfo.InvariantCulture);
		}
	}
}
