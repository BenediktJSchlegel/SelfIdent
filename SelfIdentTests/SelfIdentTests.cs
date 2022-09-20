using System;
using NUnit.Framework;
using SelfIdent;
using SelfIdent.Exceptions;

namespace SelfIdentTests
{
    /// <summary>
    /// Testcases for SelfIdent using a MySQL Database
    /// </summary>
    [TestFixture]
    public class SelfIdentTestsMySQL
    {

        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        public void Setup_ThrowsArgumentException_WhenPassingNull()
        {

        }

        [Test]
        public void Setup_ThrowsArgumentNullException_WhenConnectionStringNotFilled()
        {
            
        }

        [Test]  
        public void Setup_ThrowsArgumentNullException_WhenDatabaseNameNotFilled()
        {

        }



    }
}