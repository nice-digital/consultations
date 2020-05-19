using System.Collections.Generic;
using System.Linq;
using Comments.Services;
using Comments.Test.Infrastructure;
using Comments.ViewModels;
using Shouldly;
using Xunit;

namespace Comments.Test.UnitTests
{
	public class ReviewPageTests : TestBase
	{

		[Theory]
		[InlineData("", "0")]
		[InlineData(null, "0")]
		[InlineData("1", "1")]
		[InlineData("1.1", "1.1")]
		[InlineData("1.10", "01.10")]
		[InlineData("2.1.0.0.2.1.1.0.0.1.2.167", "002.001.000.000.002.001.001.000.000.001.002.167")]

		public void OrderingSetAccessorInViewModelPadsZeros(string orderStringPassedIn, string expectedOrderString)
		{
			//Arrange 
			var location = new ViewModels.Location();

			//Act
			location.Order = orderStringPassedIn;

			//Assert
			location.Order.ShouldBe(expectedOrderString);
		}

		[Theory]
		[InlineData(null, null)]
		[InlineData(new QuestionsOrComments[0], new int[0])]
		public void No_Filtering_Tests(IEnumerable<QuestionsOrComments> typesToFilter, IEnumerable<int> documentIdsToFilter)
		{
			//Arrange
			var unfilteredCommentsAndQuestions = SampleListData.GetReviewPageViewModelWith1CommentAnd1Question().CommentsAndQuestions;
			var commentService = new CommentService(null, _fakeUserService, null, _fakeHttpContextAccessor);

			//Act
			var filteredCommentsAndQuestions = commentService.FilterCommentsAndQuestions(unfilteredCommentsAndQuestions, typesToFilter, documentIdsToFilter);

			//Assert
			filteredCommentsAndQuestions.Comments.Count.ShouldBe(unfilteredCommentsAndQuestions.Comments.Count);
			filteredCommentsAndQuestions.Comments.ForEach(c => c.Show.ShouldBeTrue());

			filteredCommentsAndQuestions.Questions.Count.ShouldBe(unfilteredCommentsAndQuestions.Questions.Count);
			filteredCommentsAndQuestions.Questions.ForEach(c => c.Show.ShouldBeTrue());
		}

		[Fact]
		public void FilterByComments()
		{
			//Arrange
			var unfilteredCommentsAndQuestions = SampleListData.GetReviewPageViewModelWith1CommentAnd1Question().CommentsAndQuestions;
			var commentService = new CommentService(null, _fakeUserService, null, _fakeHttpContextAccessor);

			//Act
			var filteredCommentsAndQuestions = commentService.FilterCommentsAndQuestions(unfilteredCommentsAndQuestions, new List<QuestionsOrComments> { QuestionsOrComments.Comments }, null);

			//Assert
			filteredCommentsAndQuestions.Comments.ForEach(c => c.Show.ShouldBeTrue());
			filteredCommentsAndQuestions.Questions.ForEach(c => c.Show.ShouldBeFalse());
		}

		[Fact]
		public void FilterByQuestions()
		{
			//Arrange
			var unfilteredCommentsAndQuestions = SampleListData.GetReviewPageViewModelWith1CommentAnd1Question().CommentsAndQuestions;
			var commentService = new CommentService(null, _fakeUserService, null, _fakeHttpContextAccessor);

			//Act
			var filteredCommentsAndQuestions = commentService.FilterCommentsAndQuestions(unfilteredCommentsAndQuestions, new List<QuestionsOrComments> { QuestionsOrComments.Questions }, null);

			//Assert
			filteredCommentsAndQuestions.Comments.ForEach(c => c.Show.ShouldBeFalse());
			filteredCommentsAndQuestions.Questions.ForEach(c => c.Show.ShouldBeTrue());
		}

		[Fact]
		public void FilterByDocumentWhenSourceUriHasThatDocumentInIt()
		{
			//Arrange
			var unfilteredCommentsAndQuestions = SampleListData.GetReviewPageViewModelWith1CommentAnd1Question().CommentsAndQuestions;
			var commentService = new CommentService(null, _fakeUserService, null, _fakeHttpContextAccessor);

			//Act
			var filteredCommentsAndQuestions = commentService.FilterCommentsAndQuestions(unfilteredCommentsAndQuestions, null, new List<int>(){ 1 });

			//Assert
			filteredCommentsAndQuestions.Comments.ForEach(c => c.Show.ShouldBeTrue());
			filteredCommentsAndQuestions.Questions.ForEach(c => c.Show.ShouldBeTrue());
		}

		[Fact]
		public void FilterByDocumentWhenSourceUriDoesNotHaveThatDocumentInIt()
		{
			//Arrange
			var unfilteredCommentsAndQuestions = SampleListData.GetReviewPageViewModelWith1CommentAnd1Question().CommentsAndQuestions;
			var commentService = new CommentService(null, _fakeUserService, null, _fakeHttpContextAccessor);

			//Act
			var filteredCommentsAndQuestions = commentService.FilterCommentsAndQuestions(unfilteredCommentsAndQuestions, null, new List<int>() { 99 });

			//Assert
			filteredCommentsAndQuestions.Comments.ForEach(c => c.Show.ShouldBeFalse());
			filteredCommentsAndQuestions.Questions.ForEach(c => c.Show.ShouldBeFalse());
		}
	}
}
