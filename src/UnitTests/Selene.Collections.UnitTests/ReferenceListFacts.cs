using System;
using Xunit;

namespace Selene.Collections.UnitTests
{
    public class ReferenceListFacts
    {
        [Fact]
        public void InitializesEmptyReferenceList()
        {
            // Given
            
            // When
            var sut = new ReferenceList<int>();
            
            // Then
            Assert.Empty(sut);
        }
    }
}