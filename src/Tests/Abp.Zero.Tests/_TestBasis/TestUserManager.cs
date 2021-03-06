﻿using Abp.Authorization.Users;

namespace Abp.Tests._TestBasis
{
    public class TestUserManager : AbpUserManager<TestTenant, TestRole, TestUser>
    {
        public TestUserManager(TestUserStore store)
            : base(store)
        {
        }
    }
}