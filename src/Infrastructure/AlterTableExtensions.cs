namespace RockPaperScissors.Infrastructure;

public static class AlterTableExtensions
{
    public static void AddConstraint(this FluentMigrator.Migration migration, string schemaName, 
        string tableName, string checkConstraintName, string checkConstraintFunction)
    {
        if (migration == null) throw new ArgumentNullException(nameof(migration));
        if (String.IsNullOrWhiteSpace(schemaName)) throw new ArgumentException(nameof(schemaName));
        if (String.IsNullOrWhiteSpace(tableName)) throw new ArgumentException(nameof(tableName));
        if (String.IsNullOrWhiteSpace(checkConstraintName)) throw new ArgumentException(nameof(checkConstraintName));
        if (String.IsNullOrWhiteSpace(checkConstraintFunction)) throw new ArgumentException(nameof(checkConstraintFunction));
        migration.Execute.Sql($@"
ALTER TABLE  {schemaName}.{tableName}
    ADD CONSTRAINT {checkConstraintName}
                   CHECK ({checkConstraintFunction})".Trim());
    }

    public static void Execute(this FluentMigrator.Migration migration, string function)
    {
        migration.Execute.Sql(function.Trim());
    }
}