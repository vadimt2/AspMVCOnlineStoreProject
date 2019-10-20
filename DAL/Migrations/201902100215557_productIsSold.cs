namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class productIsSold : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "isSold", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "isSold");
        }
    }
}
