namespace Infomatrix.Models.ViewModels
{
    public class OrdenMV
    {
        public Orden Orden { get; set; }
        public IEnumerable<OrdenDetalle> OrdenDetalle { get; set; }
    }
}
