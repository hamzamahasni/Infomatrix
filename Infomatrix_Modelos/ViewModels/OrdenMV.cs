﻿namespace Infomatrix_Modelos.ViewModels
{
    public class OrdenMV
    {
        public Orden Orden { get; set; }
        public IEnumerable<OrdenDetalle> OrdenDetalle { get; set; }
    }
}