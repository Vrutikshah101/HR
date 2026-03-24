using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeaveManagement.Infrastructure.Persistence.Migrations
{
    public partial class EmployeeDobDojFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE `employees` ADD COLUMN IF NOT EXISTS `date_of_birth` date NULL;");
            migrationBuilder.Sql("ALTER TABLE `employees` ADD COLUMN IF NOT EXISTS `gender` varchar(20) NULL;");
            migrationBuilder.Sql("ALTER TABLE `employees` ADD COLUMN IF NOT EXISTS `join_date` date NULL;");
            migrationBuilder.Sql("ALTER TABLE `employees` ADD COLUMN IF NOT EXISTS `date_of_relieving` date NULL;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE `employees` DROP COLUMN IF EXISTS `date_of_birth`;");
            migrationBuilder.Sql("ALTER TABLE `employees` DROP COLUMN IF EXISTS `gender`;");
            migrationBuilder.Sql("ALTER TABLE `employees` DROP COLUMN IF EXISTS `join_date`;");
            migrationBuilder.Sql("ALTER TABLE `employees` DROP COLUMN IF EXISTS `date_of_relieving`;");
        }
    }
}
