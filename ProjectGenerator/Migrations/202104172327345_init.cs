namespace ProjectGenerator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DataAnnotationAttribles",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.Int(nullable: false),
                        Param1 = c.String(),
                        Param2 = c.String(),
                        ErrorMessage = c.String(),
                        DataFieldId = c.Guid(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        LatestUpdatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DataFields", t => t.DataFieldId, cascadeDelete: true)
                .Index(t => t.DataFieldId);
            
            CreateTable(
                "dbo.DataFields",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Type = c.Int(nullable: false),
                        Lengh = c.String(),
                        Nullable = c.Boolean(nullable: false),
                        TableId = c.Guid(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        LatestUpdatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tables", t => t.TableId, cascadeDelete: true)
                .Index(t => t.TableId);
            
            CreateTable(
                "dbo.Tables",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        DataFieldType = c.String(),
                        ProjectID = c.Guid(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        LatestUpdatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Projects", t => t.ProjectID, cascadeDelete: true)
                .Index(t => t.ProjectID);
            
            CreateTable(
                "dbo.DataFieldForeignKeys",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Required = c.Boolean(nullable: false),
                        DataFieldViewID = c.Guid(),
                        TableViewID = c.Guid(),
                        CreatedOn = c.DateTime(nullable: false),
                        LatestUpdatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DataFields", t => t.DataFieldViewID)
                .ForeignKey("dbo.Tables", t => t.TableViewID)
                .Index(t => t.DataFieldViewID)
                .Index(t => t.TableViewID);
            
            CreateTable(
                "dbo.Projects",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Version = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        LatestUpdatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EnumDetails",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        LatestUpdatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        EnumView_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EnumViews", t => t.EnumView_Id)
                .Index(t => t.EnumView_Id);
            
            CreateTable(
                "dbo.EnumViews",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Descriptions = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        LatestUpdatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.EnumDetails", "EnumView_Id", "dbo.EnumViews");
            DropForeignKey("dbo.DataAnnotationAttribles", "DataFieldId", "dbo.DataFields");
            DropForeignKey("dbo.DataFields", "TableId", "dbo.Tables");
            DropForeignKey("dbo.Tables", "ProjectID", "dbo.Projects");
            DropForeignKey("dbo.DataFieldForeignKeys", "TableViewID", "dbo.Tables");
            DropForeignKey("dbo.DataFieldForeignKeys", "DataFieldViewID", "dbo.DataFields");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.EnumDetails", new[] { "EnumView_Id" });
            DropIndex("dbo.DataFieldForeignKeys", new[] { "TableViewID" });
            DropIndex("dbo.DataFieldForeignKeys", new[] { "DataFieldViewID" });
            DropIndex("dbo.Tables", new[] { "ProjectID" });
            DropIndex("dbo.DataFields", new[] { "TableId" });
            DropIndex("dbo.DataAnnotationAttribles", new[] { "DataFieldId" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.EnumViews");
            DropTable("dbo.EnumDetails");
            DropTable("dbo.Projects");
            DropTable("dbo.DataFieldForeignKeys");
            DropTable("dbo.Tables");
            DropTable("dbo.DataFields");
            DropTable("dbo.DataAnnotationAttribles");
        }
    }
}
