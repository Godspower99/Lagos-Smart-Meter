using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LagosSmartMeter.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    FullName = table.Column<string>(nullable: true),
                    Birthday = table.Column<DateTime>(nullable: false),
                    Gender = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FireAndForgetMeterJobs",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Completed = table.Column<bool>(nullable: false),
                    MeterID = table.Column<string>(nullable: true),
                    Job = table.Column<string>(nullable: true),
                    EnquedTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FireAndForgetMeterJobs", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "MeterAlerts",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Limit = table.Column<double>(nullable: false),
                    MeterID = table.Column<string>(nullable: true),
                    AutoDeactivate = table.Column<bool>(nullable: false),
                    State = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    Time = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeterAlerts", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "MeterMonthlyOnHours",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MeterID = table.Column<string>(nullable: false),
                    Year = table.Column<int>(nullable: false),
                    Month = table.Column<int>(nullable: false),
                    OnHours = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeterMonthlyOnHours", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "MeterNotifications",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MeterID = table.Column<string>(nullable: true),
                    NotificationType = table.Column<string>(nullable: true),
                    Time = table.Column<DateTime>(nullable: false),
                    Seen = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeterNotifications", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Meters",
                columns: table => new
                {
                    MeterID = table.Column<string>(nullable: false),
                    TarriffID = table.Column<string>(nullable: true),
                    TypeID = table.Column<string>(nullable: true),
                    MeterState = table.Column<string>(nullable: true),
                    ProvisionState = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meters", x => x.MeterID);
                });

            migrationBuilder.CreateTable(
                name: "MeterTariffs",
                columns: table => new
                {
                    TarrifID = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    KwhPrice = table.Column<double>(nullable: false),
                    AccessRatePeriod = table.Column<int>(nullable: false),
                    AccessRatePrice = table.Column<double>(nullable: false),
                    HasAccessRate = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeterTariffs", x => x.TarrifID);
                });

            migrationBuilder.CreateTable(
                name: "MeterTokens",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TokenID = table.Column<string>(nullable: true),
                    MeterID = table.Column<string>(nullable: true),
                    Value = table.Column<double>(nullable: false),
                    Used = table.Column<bool>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    SubscriberID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeterTokens", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "MeterTypes",
                columns: table => new
                {
                    ID = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeterTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SubscriberMeters",
                columns: table => new
                {
                    MeterID = table.Column<string>(nullable: false),
                    SubscriberID = table.Column<string>(nullable: true),
                    AssignedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriberMeters", x => x.MeterID);
                });

            migrationBuilder.CreateTable(
                name: "SubscriberNotifications",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubscriberID = table.Column<string>(nullable: true),
                    NotificationType = table.Column<string>(nullable: true),
                    Sent = table.Column<bool>(nullable: false),
                    Time = table.Column<DateTime>(nullable: false),
                    Content = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriberNotifications", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    TransactionID = table.Column<string>(nullable: false),
                    Amount = table.Column<double>(nullable: false),
                    Sender = table.Column<string>(nullable: true),
                    PaymentType = table.Column<string>(nullable: true),
                    PaymentTypeID = table.Column<string>(nullable: true),
                    MeterID = table.Column<string>(nullable: true),
                    SubscriberID = table.Column<string>(nullable: true),
                    TransactionDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.TransactionID);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubscriberAddresses",
                columns: table => new
                {
                    SubscriberID = table.Column<string>(nullable: false),
                    city = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Street = table.Column<string>(nullable: true),
                    BuildingNumber = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriberAddresses", x => x.SubscriberID);
                    table.ForeignKey(
                        name: "FK_SubscriberAddresses_AspNetUsers_SubscriberID",
                        column: x => x.SubscriberID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MeterLocations",
                columns: table => new
                {
                    MeterID = table.Column<string>(nullable: false),
                    city = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Street = table.Column<string>(nullable: true),
                    BuildingNumber = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeterLocations", x => x.MeterID);
                    table.ForeignKey(
                        name: "FK_MeterLocations_Meters_MeterID",
                        column: x => x.MeterID,
                        principalTable: "Meters",
                        principalColumn: "MeterID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MeterReadings",
                columns: table => new
                {
                    MeterID = table.Column<string>(nullable: false),
                    CurrentPower = table.Column<double>(nullable: false),
                    CurrentVoltage = table.Column<double>(nullable: false),
                    EnergyUsedInSeconds = table.Column<double>(nullable: false),
                    EnergyUsedInHours = table.Column<double>(nullable: false),
                    LastUpdate = table.Column<DateTime>(nullable: false),
                    AvailableEnergy = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeterReadings", x => x.MeterID);
                    table.ForeignKey(
                        name: "FK_MeterReadings_Meters_MeterID",
                        column: x => x.MeterID,
                        principalTable: "Meters",
                        principalColumn: "MeterID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MeterTimeRecords",
                columns: table => new
                {
                    MeterID = table.Column<string>(nullable: false),
                    InitialActivation = table.Column<DateTime>(nullable: false),
                    LastActivation = table.Column<DateTime>(nullable: false),
                    LastDeactivation = table.Column<DateTime>(nullable: false),
                    LastShutDown = table.Column<DateTime>(nullable: false),
                    LastStartUp = table.Column<DateTime>(nullable: false),
                    LastUpdate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeterTimeRecords", x => x.MeterID);
                    table.ForeignKey(
                        name: "FK_MeterTimeRecords_Meters_MeterID",
                        column: x => x.MeterID,
                        principalTable: "Meters",
                        principalColumn: "MeterID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "FireAndForgetMeterJobs");

            migrationBuilder.DropTable(
                name: "MeterAlerts");

            migrationBuilder.DropTable(
                name: "MeterLocations");

            migrationBuilder.DropTable(
                name: "MeterMonthlyOnHours");

            migrationBuilder.DropTable(
                name: "MeterNotifications");

            migrationBuilder.DropTable(
                name: "MeterReadings");

            migrationBuilder.DropTable(
                name: "MeterTariffs");

            migrationBuilder.DropTable(
                name: "MeterTimeRecords");

            migrationBuilder.DropTable(
                name: "MeterTokens");

            migrationBuilder.DropTable(
                name: "MeterTypes");

            migrationBuilder.DropTable(
                name: "SubscriberAddresses");

            migrationBuilder.DropTable(
                name: "SubscriberMeters");

            migrationBuilder.DropTable(
                name: "SubscriberNotifications");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Meters");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
