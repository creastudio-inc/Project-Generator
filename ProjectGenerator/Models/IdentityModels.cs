 
using Infrastructure.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectGenerator.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        #region Properties
        public Guid UserId { get; set; }

    

        public ObjectContext ObjectContext
        {
            get
            {
                var objectContext = (this as IObjectContextAdapter);
                if (objectContext != null)
                    return (this as IObjectContextAdapter).ObjectContext;
                else
                    return null;
            }
        }

        #endregion

        public static List<ApplicationDbContext> listDBContext = new List<ApplicationDbContext>();
        private static readonly object lockRoot = new object();
        public static int elapseTime = 5000;

        public bool isBusy = false;
        public Stopwatch stopWatch = new Stopwatch();

        public void UpdateDatabase()
        {
            var Migrator = new DbMigrator(new Migrations.Configuration() { TargetDatabase = new DbConnectionInfo(this.Database.Connection.ConnectionString, "System.Data.SqlClient") });
            IEnumerable<string> PendingMigrations = Migrator.GetPendingMigrations();
            foreach (var Migration in PendingMigrations)
                Migrator.Update(Migration);
        }

        public void doUpdateAccessDbContext(bool isBusy)
        {
            this.isBusy = isBusy;
            this.stopWatch = new Stopwatch();
            this.stopWatch.Start();
        }

        public static ApplicationDbContext doGetDbContext()
        {
            lock (lockRoot)
            {
                ApplicationDbContext temp = null;
                ApplicationDbContext db;
                try
                {

                    for (int i = 0; i < listDBContext.Count; i++)
                    {
                        db = listDBContext.ElementAt(i);
                        if (db.Database.Connection != null && db.Database.Connection.State == ConnectionState.Open && db.isBusy == false)
                        {
                            temp = db;
                            break;
                        }
                    }
                    if (temp == null)
                    {
                        temp = new ApplicationDbContext();
                        listDBContext.Add(temp);
                    }
                    while (temp.Database.Connection.State == ConnectionState.Closed)
                    {
                        temp.Database.Connection.Open();
                        Thread.Sleep(10);
                    }
                    temp.doUpdateAccessDbContext(true);
                    return temp;
                }
                catch (System.Exception ex)
                {
                    return null;

                }
            }
        }

        public static void doCheckListDBContext()
        {
            lock (lockRoot)
            {
                try
                {
                    ApplicationDbContext temp = null;
                    for (int i = 0; i < listDBContext.Count; i++)
                    {
                        temp = listDBContext.ElementAt(i);
                        if (temp.stopWatch.ElapsedMilliseconds > elapseTime)
                        {
                            temp.Dispose();
                            listDBContext.Remove(temp);
                            i--;
                        }
                    }
                }
                catch (System.Exception ex)
                {
                }
            }
        }


        public override int SaveChanges()
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is EntityBase && (
                        e.State == EntityState.Added
                        || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {


                if (entityEntry.State == EntityState.Added)
                {
                    ((Infrastructure.Entity.EntityBase)entityEntry.Entity).CreatedOn = DateTime.Now;
                    ((Infrastructure.Entity.EntityBase)entityEntry.Entity).Id = Guid.NewGuid();
                    ((Infrastructure.Entity.EntityBase)entityEntry.Entity).CreatedBy = UserId;
                }
                if (entityEntry.State == EntityState.Modified)
                {
                    ((Infrastructure.Entity.EntityBase)entityEntry.Entity).ModifiedBy = UserId;
                    ((Infrastructure.Entity.EntityBase)entityEntry.Entity).LatestUpdatedOn = DateTime.Now;
                    // ((Infrastructure.Entity.EntityBase)entityEntry.Entity).IsDeleted = false;
                }

            }

            //UpdateDates();
            return base.SaveChanges();
        }

        private void UpdateDates()
        {
            foreach (var change in ChangeTracker.Entries().Where(x => (x.State == EntityState.Added || x.State == EntityState.Modified)))
            {
                var values = change.CurrentValues;
                foreach (var name in values.PropertyNames)
                {
                    var value = values[name];
                    if (value is DateTime)
                    {
                        var date = (DateTime)value;
                        if (date < SqlDateTime.MinValue.Value)
                        {
                            values[name] = SqlDateTime.MinValue.Value;
                        }
                        else if (date > SqlDateTime.MaxValue.Value)
                        {
                            values[name] = SqlDateTime.MaxValue.Value;
                        }
                    }
                }
            }
        }



        public DbSet<Project> ProjectContext { get; set; }
       public DbSet<Table> TableContext { get; set; }
       public DbSet<DataField> DataFieldContext { get; set; }
       public DbSet<DataFieldForeignKey> DataFieldForeignKeyContext { get; set; }
       public DbSet<DataAnnotationAttrible> DataAnnotationAttribleContext { get; set; }
       public DbSet<EnumView> EnumViewContext { get; set; }
       public DbSet<EnumDetail> EnumDetailContext { get; set; }

    }
}