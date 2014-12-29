using System;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using Abp;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Modules;
using Abp.MultiTenancy;
using Abp.Runtime.Session;
using Abp.Zero.EntityFramework;

namespace ConsoleTester
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(ConfigurationManager.ConnectionStrings["Default"].ConnectionString);
            Console.ReadLine();
            Test();
        }

        private static void Test()
        {
            using (var bootstrapper = new AbpBootstrapper())
            {
                bootstrapper.Initialize();

                var tester = IocManager.Instance.Resolve<Tester>();
                tester.DoTests();

                Console.WriteLine("Press enter to stop application...");
                Console.ReadLine();
            }
        }
    }

    [DependsOn(typeof(AbpZeroEntityFrameworkModule))]
    public class MyModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }

    public class Tester : ITransientDependency
    {
        private readonly IRepository<User, long> _userRepository;
        private readonly MyUserStore _userStore;
        private readonly IRepository<MyEntity> _myEntityRepository;
        private readonly IRepository<Setting, long> _userSettingRepository;


        public Tester(IRepository<User, long> userRepository, MyUserStore userStore, 
            IRepository<MyEntity> myEntityRepository,
            IRepository<Setting, long> userSettingRepository)
        {
            _userRepository = userRepository;
            _userStore = userStore;
            _myEntityRepository = myEntityRepository;
            _userSettingRepository = userSettingRepository;
        }

        public void DoTests()
        {
            foreach (var user in _userRepository.GetAllList())
            {
                Console.WriteLine(user);
            }

            foreach (var userSetting in _userSettingRepository.GetAllList())
            {
                Console.WriteLine(userSetting);
            }

            var newUserSetting = new Setting()
            {
                UserId = 1,
                Name = "DefaultListSize",
                Value = "25",   
                CreationTime = DateTime.UtcNow 
            };

            _userSettingRepository.Insert(newUserSetting);


        }
    }

    public class MyDbContext : AbpZeroDbContext<Tenant, Role, User>
    {
        public IDbSet<MyEntity> MyEntities { get; set; }

        public MyDbContext()
            : base(ConfigurationManager.ConnectionStrings["Default"].ConnectionString)
        {

        }
    }

    public class Tenant : AbpTenant<Tenant, User>
    {
        protected Tenant()
        {

        }

        public Tenant(string tenancyName, string name)
            : base(tenancyName, name)
        {
        }
    }

    public class User : AbpUser<Tenant, User>
    {
        public override string ToString()
        {
            return string.Format("[User {0}] {1}", Id, UserName);
        }
    }

    public class Role : AbpRole<Tenant, User>
    {
        protected Role()

        {

        }

        public Role(int? tenantId, string name, string displayName)
            : base(tenantId, name, displayName)
        {

        }
    }

    public class MyEntity : Entity
    {
        public string MyEntityProp { get; set; }
    }

    public class MyUserManager : AbpUserManager<Tenant, Role, User>
    {
        public MyUserManager(MyUserStore store)
            : base(store)
        {
        }
    }

    public class MyUserStore : AbpUserStore<Tenant, Role, User>
    {
        public MyUserStore(
            IRepository<User, long> userRepository,
            IRepository<UserLogin, long> userLoginRepository,
            IRepository<UserRole, long> userRoleRepository,
            IRepository<Role> roleRepository,
            IAbpSession session,
            IUnitOfWorkManager unitOfWorkManager)
            : base(
                userRepository,
                userLoginRepository,
                userRoleRepository,
                roleRepository,
                session,
                unitOfWorkManager)
        {
        }
    }
}
