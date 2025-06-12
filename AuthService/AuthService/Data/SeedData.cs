using AuthService.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Data
{
    public class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
                var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<SeedData>>();

                logger.LogInformation("Starting database seeding...");

                try
                {
                    await dbContext.Database.MigrateAsync();
                    logger.LogInformation("Database migrations applied successfully.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while applying database migrations.");
                    throw;
                }

                await SeedRoles(roleManager, logger);

                await SeedAdminUser(userManager, roleManager, logger);
                await SeedManagerUser(userManager, roleManager, logger);

                logger.LogInformation("Database seeding completed.");
            }
        }

        private static async Task SeedRoles(RoleManager<IdentityRole<Guid>> roleManager, ILogger<SeedData> logger)
        {
            string[] roleNames = { "Admin", "Manager", "User" };

            foreach (var roleName in roleNames)
            {
                if (await roleManager.FindByNameAsync(roleName) == null)
                {
                    logger.LogInformation($"Creating {roleName} role...");
                    var role = new IdentityRole<Guid>(roleName);
                    var result = await roleManager.CreateAsync(role);
                    if (result.Succeeded)
                    {
                        logger.LogInformation($"{roleName} role created successfully.");
                    }
                    else
                    {
                        logger.LogError($"Error creating {roleName} role: {{Errors}}", string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
                else
                {
                    logger.LogInformation($"{roleName} role already exists.");
                }
            }
        }

        private static async Task SeedAdminUser(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager, ILogger<SeedData> logger)
        {
            string adminEmail = "admin@fpt.com";
            string adminPassword = "Admin@123";
            string adminRoleName = "Admin";

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                logger.LogInformation("Creating default Admin user...");
                var adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    logger.LogInformation("Default Admin user created successfully.");

                    if (await roleManager.RoleExistsAsync(adminRoleName))
                    {
                        var roleResult = await userManager.AddToRoleAsync(adminUser, adminRoleName);
                        if (roleResult.Succeeded)
                        {
                            logger.LogInformation("Default Admin user assigned to Admin role successfully.");
                        }
                        else
                        {
                            logger.LogError("Error assigning Admin role to user: {Errors}", string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                        }
                    }
                    else
                    {
                        logger.LogWarning($"Role '{adminRoleName}' does not exist.  Cannot assign to user.");
                    }
                }
                else
                {
                    logger.LogError("Error creating default Admin user: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
            else
            {
                logger.LogInformation("Default Admin user already exists.");
            }
        }

        private static async Task SeedManagerUser(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager, ILogger<SeedData> logger)
        {
            string managerEmail = "manager@fpt.com";
            string managerPassword = "Manager@123";
            string managerRoleName = "Manager";

            if (await userManager.FindByEmailAsync(managerEmail) == null)
            {
                logger.LogInformation("Creating default Manager user...");
                var managerUser = new ApplicationUser
                {
                    UserName = managerEmail,
                    Email = managerEmail,
                    EmailConfirmed = true,
                };

                var result = await userManager.CreateAsync(managerUser, managerPassword);
                if (result.Succeeded)
                {
                    logger.LogInformation("Default Manager user created successfully.");

                    if (await roleManager.RoleExistsAsync(managerRoleName))
                    {
                        var roleResult = await userManager.AddToRoleAsync(managerUser, managerRoleName);
                        if (roleResult.Succeeded)
                        {
                            logger.LogInformation("Default Manager user assigned to Manager role successfully.");
                        }
                        else
                        {
                            logger.LogError("Error assigning Manager role to user: {Errors}", string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                        }
                    }
                    else
                    {
                        logger.LogWarning($"Role '{managerRoleName}' does not exist. Cannot assign to user.");
                    }
                }
                else
                {
                    logger.LogError("Error creating default Manager user: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
            else
            {
                logger.LogInformation("Default Manager user already exists.");
            }
        }
    }
}
