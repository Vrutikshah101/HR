using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeaveManagement.Infrastructure.Persistence.Migrations;

public partial class DepartmentDesignationMaps : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
CREATE TABLE IF NOT EXISTS `department_designation_maps` (
  `id` char(36) NOT NULL,
  `department_id` char(36) NOT NULL,
  `designation_id` char(36) NOT NULL,
  `created_at_utc` datetime(6) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `IX_department_designation_maps_department_id_designation_id` (`department_id`,`designation_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP TABLE IF EXISTS `department_designation_maps`;");
    }
}
