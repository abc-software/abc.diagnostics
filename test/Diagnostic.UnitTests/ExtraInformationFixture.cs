using Diagnostic;
using Diagnostic.ExtraInformation;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Principal;

#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif 

namespace Diagnostic.UnitTests {

    [TestClass]
    public class ExtraInformationFixture {
        private IPrincipal savePrincipal;

        [TestInitialize()]
        public void MyTestInitialize() {
            savePrincipal = System.Threading.Thread.CurrentPrincipal;
        }

        [TestCleanup()]
        public void MyTestCleanup() {
            System.Threading.Thread.CurrentPrincipal = savePrincipal;
        }

        /// <summary>
        ///A test for ManagedSecurityContextInformationProvider
        ///</summary>
        [TestMethod()]
        public void ManagedSecurityContextInformationProviderTest() {
            string type = "Type";
            string name = "Name";
            IDictionary<string, object> dictionary = new Dictionary<string, object>();

            System.Threading.Thread.CurrentPrincipal = new GenericPrincipal(
                new GenericIdentity(name, type), new string[] {});  

            ManagedSecurityContextInformationProvider provider = new ManagedSecurityContextInformationProvider();
            provider.PopulateDictionary(dictionary);

            string actualType = provider.AuthenticationType;
            string actualName = provider.IdentityName;
            bool actualIsAuthenticated = provider.IsAuthenticated;

            Assert.AreEqual(type, actualType);
            Assert.AreEqual(name, actualName);
            Assert.AreEqual(true, actualIsAuthenticated);

            Assert.AreEqual(3, dictionary.Count);  
            Assert.AreEqual(type, dictionary["AuthenticationType"]);  
            Assert.AreEqual(name, dictionary["IdentityName"]);  
            Assert.AreEqual("True", dictionary["IsAuthenticated"]);  
        }

        /// <summary>
        ///A test for DebugInformationProvider
        ///</summary>
        [TestMethod()]
        public void DebugInformationProviderTest() {
            IDictionary<string, object> dictionary = new Dictionary<string, object>();

            DebugInformationProvider provider = new DebugInformationProvider();
            provider.PopulateDictionary(dictionary);

            Assert.AreEqual(1, dictionary.Count);
            Assert.IsNotNull(dictionary["StackTrace"]);
        }

        /// <summary>
        ///A test for UnmanagedSecurityContextInformationProvider
        ///</summary>
        /*[TestMethod()]
        public void UnmanagedSecurityContextInformationProviderTest() {
            IDictionary<string, object> dictionary = new Dictionary<string, object>();

            UnmanagedSecurityContextInformationProvider provider = new UnmanagedSecurityContextInformationProvider();
            provider.PopulateDictionary(dictionary);

            Assert.AreEqual(2, dictionary.Count);
            Assert.IsNotNull(dictionary["CurrentUser"]);
            Assert.IsNotNull(dictionary["ProcessAccountName"]);
        }*/
    }
}
