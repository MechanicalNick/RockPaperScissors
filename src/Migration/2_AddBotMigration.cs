using FluentMigrator;

namespace RockPaperScissors.Migration;

[Migration(2)]
public class AddBotMigration : FluentMigrator.Migration
{
    public override void Up()
    {
        Insert.IntoTable("player").Row(new { name = "pc", is_bot = true});
        
        Alter.Table("game")
            .AddColumn("with_bot")
            .AsBoolean()
            .WithDefaultValue(false);
    }

    public override void Down()
    {
        Delete.FromTable("player").Row(new { name = "pc" });
        Delete.Column("with_bot").FromTable("game");
    }
}