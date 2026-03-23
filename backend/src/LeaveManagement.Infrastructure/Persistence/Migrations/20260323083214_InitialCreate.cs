using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeaveManagement.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    password_hash = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "employees",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    employee_code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    full_name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    department = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    designation = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employees", x => x.id);
                    table.ForeignKey(
                        name: "FK_employees_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "user_roles",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    role_code = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_roles", x => new { x.user_id, x.role_code });
                    table.ForeignKey(
                        name: "FK_user_roles_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "leave_balances",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    employee_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    leave_type_code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    opening_balance = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    used = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    adjustments = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_leave_balances", x => x.id);
                    table.ForeignKey(
                        name: "FK_leave_balances_employees_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "leave_requests",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    employee_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    leave_type_code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    end_date = table.Column<DateOnly>(type: "date", nullable: false),
                    days = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    reason = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<int>(type: "int", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_leave_requests", x => x.id);
                    table.ForeignKey(
                        name: "FK_leave_requests_employees_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "reporting_hierarchies",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    employee_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    level1_approver_employee_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    level2_approver_employee_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reporting_hierarchies", x => x.id);
                    table.ForeignKey(
                        name: "FK_reporting_hierarchies_employees_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_reporting_hierarchies_employees_level1_approver_employee_id",
                        column: x => x.level1_approver_employee_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_reporting_hierarchies_employees_level2_approver_employee_id",
                        column: x => x.level2_approver_employee_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "leave_request_approvals",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    leave_request_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    approval_level = table.Column<int>(type: "int", nullable: false),
                    approver_employee_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    action = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    comment = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    actioned_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_leave_request_approvals", x => x.id);
                    table.ForeignKey(
                        name: "FK_leave_request_approvals_employees_approver_employee_id",
                        column: x => x.approver_employee_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_leave_request_approvals_leave_requests_leave_request_id",
                        column: x => x.leave_request_id,
                        principalTable: "leave_requests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_employees_employee_code",
                table: "employees",
                column: "employee_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_employees_user_id",
                table: "employees",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_leave_balances_employee_id_leave_type_code",
                table: "leave_balances",
                columns: new[] { "employee_id", "leave_type_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_leave_request_approvals_approver_employee_id",
                table: "leave_request_approvals",
                column: "approver_employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_leave_request_approvals_leave_request_id",
                table: "leave_request_approvals",
                column: "leave_request_id");

            migrationBuilder.CreateIndex(
                name: "IX_leave_requests_employee_id",
                table: "leave_requests",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_reporting_hierarchies_employee_id",
                table: "reporting_hierarchies",
                column: "employee_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_reporting_hierarchies_level1_approver_employee_id",
                table: "reporting_hierarchies",
                column: "level1_approver_employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_reporting_hierarchies_level2_approver_employee_id",
                table: "reporting_hierarchies",
                column: "level2_approver_employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                table: "users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "leave_balances");

            migrationBuilder.DropTable(
                name: "leave_request_approvals");

            migrationBuilder.DropTable(
                name: "reporting_hierarchies");

            migrationBuilder.DropTable(
                name: "user_roles");

            migrationBuilder.DropTable(
                name: "leave_requests");

            migrationBuilder.DropTable(
                name: "employees");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
