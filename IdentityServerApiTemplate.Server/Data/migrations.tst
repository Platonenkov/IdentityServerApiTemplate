Add-Migration Initial -Context ApplicationDbContext -OutputDir Data/Migrations/ApplicationDb
Update-Database -Context ApplicationDbContext

Add-Migration Initial -Context PersistedGrantDbContext -OutputDir Data/Migrations/PersistedGrantDb
Update-Database -Context PersistedGrantDbContext

Add-Migration Initial -Context ConfigurationDbContext -OutputDir Data/Migrations/ConfigurationDb
Update-Database -Context ConfigurationDbContext