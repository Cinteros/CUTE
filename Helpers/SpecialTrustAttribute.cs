﻿namespace Cinteros.Unit.Test.Extensions.Helpers
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Security;
    using System.Security.Policy;
    using NUnit.Framework;

    [AttributeUsage(AttributeTargets.Method)]
    public class SpecialTrustAttribute : TestActionAttribute
    {
        public override void BeforeTest(TestDetails testDetails)
        {
            var evidence = new Evidence();
            evidence.AddHostEvidence(new Zone(SecurityZone.Intranet));
            var permissions = SecurityManager.GetStandardSandbox(evidence);
            // var permissions = new PermissionSet(PermissionState.Unrestricted);

            var setup = new AppDomainSetup
            {
                ApplicationBase = Path.GetDirectoryName(Assembly.GetAssembly(typeof(SpecialTrustSandbox)).Location)
            };

            var domain = AppDomain.CreateDomain(testDetails.FullName, evidence, setup, permissions, null);

            var type = typeof(SpecialTrustSandbox);

            var sandbox = (SpecialTrustSandbox)domain.CreateInstanceAndUnwrap(type.Assembly.FullName, type.FullName);

            var exception = sandbox.Execute(new SpecialTrustTest(testDetails));

            if (exception == null)
            {
                Assert.Pass();
            }

            throw exception;
        }
    }
}
