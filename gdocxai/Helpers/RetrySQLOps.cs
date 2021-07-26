using System.Data.Entity;
using System.Data.Entity.SqlServer;

namespace Indexai.Helpers
{
    class RetrySQLOps : DbConfiguration
    {
        public RetrySQLOps()
        {
            SetExecutionStrategy("System.Data.SqlClient", () => new SqlAzureExecutionStrategy());
        }
    }
}
