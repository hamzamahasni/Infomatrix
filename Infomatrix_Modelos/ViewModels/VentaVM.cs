namespace Infomatrix_Modelos.ViewModels
{
    public class VentaVM
    {
        public Venta Venta { get; set; }
        public IEnumerable<VentaDetalle> VentaDetalle { get; set; }
    }
}
