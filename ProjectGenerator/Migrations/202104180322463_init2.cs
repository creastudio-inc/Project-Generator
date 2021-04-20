namespace ProjectGenerator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DataFieldForeignKeys", "DataFieldViewID", "dbo.DataFields");
            DropIndex("dbo.DataFieldForeignKeys", new[] { "DataFieldViewID" });
            DropColumn("dbo.Tables", "DataFieldType");
            DropColumn("dbo.DataFieldForeignKeys", "DataFieldViewID");
            DropColumn("dbo.Projects", "Version");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Projects", "Version", c => c.String());
            AddColumn("dbo.DataFieldForeignKeys", "DataFieldViewID", c => c.Guid());
            AddColumn("dbo.Tables", "DataFieldType", c => c.String());
            CreateIndex("dbo.DataFieldForeignKeys", "DataFieldViewID");
            AddForeignKey("dbo.DataFieldForeignKeys", "DataFieldViewID", "dbo.DataFields", "Id");
        }
    }
}
