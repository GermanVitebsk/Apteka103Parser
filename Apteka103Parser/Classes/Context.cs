using System.Data.Entity;

namespace Apteka103Parser
{
    public class MedicineContext : DbContext
    {
        public MedicineContext() : base("MedicineConnection")
        {
        }

        public DbSet<Medicine> Medicines { get; set; }
    }
}