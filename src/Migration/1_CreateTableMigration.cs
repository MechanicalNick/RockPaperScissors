using FluentMigrator;
using RockPaperScissors.Infrastructure;

namespace RockPaperScissors.Migration;

[Migration(1)]
public class InitTablesMigration : FluentMigrator.Migration
{
    private const string UpdateRounds = @"CREATE FUNCTION update_rounds_count()
  RETURNS TRIGGER
  LANGUAGE plpgsql AS
$func$
BEGIN
   CASE TG_OP
   WHEN 'INSERT' THEN
      UPDATE game AS g
      SET    rounds_count = g.rounds_count + 1
      WHERE  g.id = NEW.game_id;
   WHEN 'DELETE' THEN
      UPDATE game AS g
      SET    rounds_count = g.rounds_count - 1 
      WHERE  g.id = OLD.game_id
      AND    g.rounds_count > 0;
   ELSE
      RAISE EXCEPTION 'Unexpected TG_OP';
   END CASE;
   
   RETURN NULL;
END
$func$";

    private const string RoundsTrigger = "CREATE TRIGGER updateRoundsCountTrigger " +
                                         "AFTER INSERT OR DELETE ON round " +
                                         "FOR EACH ROW EXECUTE PROCEDURE update_rounds_count();";

    public override void Up()
    {
        Create.Table("player")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("name").AsString().Unique()
            .WithColumn("is_bot").AsBoolean().WithDefaultValue(false);

        Create.Table("game")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("first_player_id").AsInt64()
            .ForeignKey("first_player_id_fk", "player", "id").NotNullable()
            .WithColumn("second_player_id").AsInt64()
                .ForeignKey("second_player_id_fk", "player", "id").Nullable()
            .WithColumn("rounds_count").AsInt32();

        Create.Table("round")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("game_id").AsInt64()
                .ForeignKey("game_id_fk", "game", "id").NotNullable()
            .WithColumn("number").AsInt32()
            .WithColumn("first_player_choice").AsInt32().Nullable()
            .WithColumn("second_player_choice").AsInt32().Nullable();

        this.AddConstraint("public", "game", "other_game_id",
            "first_player_id != second_player_id");

        this.Execute(UpdateRounds);

        this.Execute(RoundsTrigger);
    }

    public override void Down()
    {
        Delete.Table("round");
        Delete.Table("game");
        Delete.Table("player");
    }
}