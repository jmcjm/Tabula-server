using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Tabula.Infrastructure.Identity.Database;

public class IdentityDatabaseContext(DbContextOptions<IdentityDatabaseContext> options) : IdentityDbContext(options);