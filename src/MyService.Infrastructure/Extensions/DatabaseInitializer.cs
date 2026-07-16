using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;
using MyService.Infrastructure.Data;

namespace MyService.Infrastructure.Extensions;

/// <summary>数据库初始化器，按指定实体类型自动创建缺失的表</summary>
public static class DatabaseInitializer
{
    /// <summary>仅对指定的实体类型检查并创建缺失的表（未指定的不处理）</summary>
    public static void CreateMissingTables(this AppDbContext db, List<Type> entityTypes)
    {
        if (entityTypes.Count == 0) return;

        var creator = db.GetService<IRelationalDatabaseCreator>();
        if (!creator.Exists())
        {
            creator.Create();
            creator.CreateTables();
            return;
        }

        var sqlGenerator = db.GetService<IMigrationsSqlGenerator>();

        foreach (var clrType in entityTypes)
        {
            var entity = db.Model.FindEntityType(clrType);
            if (entity?.GetTableName() is not { } tableName) continue;

            // 只单独检查该表是否存在
            var exists = db.Database
                .SqlQuery<int>($"SELECT COUNT(1) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = {tableName}")
                .FirstOrDefault() > 0;

            if (exists) continue;

            var operations = BuildCreateOperations(entity, tableName);
            var sqlCommands = sqlGenerator.Generate(operations, db.Model);
            foreach (var cmd in sqlCommands)
                db.Database.ExecuteSqlRaw(cmd.CommandText);
        }
    }

    private static List<MigrationOperation> BuildCreateOperations(IEntityType entityType, string tableName)
    {
        var operations = new List<MigrationOperation>();

        var pk = entityType.FindPrimaryKey();
        var pkColumns = pk?.Properties.Select(p => p.GetColumnName()).ToArray() ?? [];

        var createTable = new CreateTableOperation { Name = tableName };

        foreach (var prop in entityType.GetProperties())
        {
            var colName = prop.GetColumnName();
            createTable.Columns.Add(new AddColumnOperation
            {
                Name = colName,
                Table = tableName,
                ClrType = prop.ClrType,
                ColumnType = prop.GetColumnType(),
                IsNullable = prop.IsNullable,
                MaxLength = prop.GetMaxLength()
            });
        }

        if (pkColumns.Length > 0)
        {
            createTable.PrimaryKey = new AddPrimaryKeyOperation
            {
                Name = $"PK_{tableName}",
                Table = tableName,
                Columns = pkColumns
            };
        }

        operations.Add(createTable);

        foreach (var index in entityType.GetIndexes())
        {
            var idxName = index.GetDatabaseName();
            if (idxName == null) continue;

            operations.Add(new CreateIndexOperation
            {
                Name = idxName,
                Table = tableName,
                Columns = index.Properties.Select(p => p.GetColumnName()).ToArray(),
                IsUnique = index.IsUnique
            });
        }

        foreach (var fk in entityType.GetForeignKeys())
        {
            if (fk.PrincipalEntityType.GetTableName() is not { } principalTable) continue;
            var fkName = fk.GetConstraintName();
            if (fkName == null) continue;

            operations.Add(new AddForeignKeyOperation
            {
                Name = fkName,
                Table = tableName,
                Columns = fk.Properties.Select(p => p.GetColumnName()).ToArray(),
                PrincipalTable = principalTable,
                PrincipalColumns = fk.PrincipalKey.Properties.Select(p => p.GetColumnName()).ToArray(),
                OnDelete = MapDeleteBehavior(fk.DeleteBehavior)
            });
        }

        return operations;
    }

    private static ReferentialAction MapDeleteBehavior(DeleteBehavior behavior) => behavior switch
    {
        DeleteBehavior.Cascade => ReferentialAction.Cascade,
        DeleteBehavior.Restrict => ReferentialAction.Restrict,
        DeleteBehavior.SetNull => ReferentialAction.SetNull,
        _ => ReferentialAction.NoAction
    };
}
