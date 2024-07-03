﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlannerCRM.Server.Migrations
{
    /// <inheritdoc />
    public partial class RenameDbTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.RenameTable(
                name: "WorkTimeRecords",
                newName: "WorkTimeRecords",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "WorkOrders",
                newName: "WorkOrders",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "WorkOrderCosts",
                newName: "WorkOrderCosts",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "ProfilePictures",
                newName: "ProfilePictures",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "EmployeeSalary",
                newName: "EmployeeSalary",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "EmployeeActivities",
                newName: "EmployeeActivities",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "ClientWorkOrders",
                newName: "ClientWorkOrders",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Clients",
                newName: "Clients",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "AspNetUserTokens",
                newName: "AspNetUserTokens",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                newName: "AspNetUsers",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "AspNetUserRoles",
                newName: "AspNetUserRoles",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "AspNetUserLogins",
                newName: "AspNetUserLogins",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "AspNetUserClaims",
                newName: "AspNetUserClaims",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "AspNetRoles",
                newName: "AspNetRoles",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "AspNetRoleClaims",
                newName: "AspNetRoleClaims",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "ActivityCost",
                newName: "ActivityCost",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Activities",
                newName: "Activities",
                newSchema: "public");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "WorkTimeRecords",
                schema: "public",
                newName: "WorkTimeRecords");

            migrationBuilder.RenameTable(
                name: "WorkOrders",
                schema: "public",
                newName: "WorkOrders");

            migrationBuilder.RenameTable(
                name: "WorkOrderCosts",
                schema: "public",
                newName: "WorkOrderCosts");

            migrationBuilder.RenameTable(
                name: "ProfilePictures",
                schema: "public",
                newName: "ProfilePictures");

            migrationBuilder.RenameTable(
                name: "EmployeeSalary",
                schema: "public",
                newName: "EmployeeSalary");

            migrationBuilder.RenameTable(
                name: "EmployeeActivities",
                schema: "public",
                newName: "EmployeeActivities");

            migrationBuilder.RenameTable(
                name: "ClientWorkOrders",
                schema: "public",
                newName: "ClientWorkOrders");

            migrationBuilder.RenameTable(
                name: "Clients",
                schema: "public",
                newName: "Clients");

            migrationBuilder.RenameTable(
                name: "AspNetUserTokens",
                schema: "public",
                newName: "AspNetUserTokens");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                schema: "public",
                newName: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "AspNetUserRoles",
                schema: "public",
                newName: "AspNetUserRoles");

            migrationBuilder.RenameTable(
                name: "AspNetUserLogins",
                schema: "public",
                newName: "AspNetUserLogins");

            migrationBuilder.RenameTable(
                name: "AspNetUserClaims",
                schema: "public",
                newName: "AspNetUserClaims");

            migrationBuilder.RenameTable(
                name: "AspNetRoles",
                schema: "public",
                newName: "AspNetRoles");

            migrationBuilder.RenameTable(
                name: "AspNetRoleClaims",
                schema: "public",
                newName: "AspNetRoleClaims");

            migrationBuilder.RenameTable(
                name: "ActivityCost",
                schema: "public",
                newName: "ActivityCost");

            migrationBuilder.RenameTable(
                name: "Activities",
                schema: "public",
                newName: "Activities");
        }
    }
}
