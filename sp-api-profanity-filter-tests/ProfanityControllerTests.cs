using System.Collections.Generic;
using SP.Profanity.Models;
using SP.Profanity.Helpers;
using Xunit;

namespace SP.Profanity.Tests
{
    public class ProfanityControllerTests
    {
        private readonly string BAD_COMMENT = "This comment is foobar";
        private readonly string GOOD_COMMENT = "This comment is very nice";

        [Fact]
        public async void test_get_words()
        {
            MockDataService svc = getMockDataService();
            List<Word> words = await svc.GetWordsAsync("");

            Assert.True(words.AnyExist());
        }

        [Fact]
        public async void test_is_text_clean()
        {
            MockDataService svc = getMockDataService();
            bool isClean = await svc.IsCleanAsync(GOOD_COMMENT, "");

            Assert.True(isClean == true);
        }

        [Fact]
        public async void test_is_text_unclean()
        {
            MockDataService svc = getMockDataService();
            bool isClean = await svc.IsCleanAsync(BAD_COMMENT, "");

            Assert.True(isClean == false);
        }

        private MockDataService getMockDataService()
        {
            return new MockDataService();
        }
    }
}
