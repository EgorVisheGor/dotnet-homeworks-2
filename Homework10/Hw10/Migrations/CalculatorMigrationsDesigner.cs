using Hw10.DbModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Homework10.Migrations;

[DbContext(typeof(ApplicationContext))]
[Migration("20221211195825_CalculatorMigrations")]
partial class CalculatorMigrations
{
    /// <inheritdoc />
    protected override void BuildTargetModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder
            .HasAnnotation("ProductVersion", "7.0.0")
            .HasAnnotation("Relational:MaxIdentifierLength", 63);

        NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

        modelBuilder.Entity("Hw10.DbModels.SolvingExpression", b =>
        {
            b.Property<int>("SolvingExpressionId")
                .ValueGeneratedOnAdd()
                .HasColumnType("integer");

            NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("SolvingExpressionId"));

            b.Property<string>("Expression")
                .IsRequired()
                .HasColumnType("text");

            b.Property<double>("Result")
                .HasColumnType("double precision");

            b.HasKey("SolvingExpressionId");

            b.ToTable("SolvingExpressions");
        });
#pragma warning restore 612, 618
    }
}